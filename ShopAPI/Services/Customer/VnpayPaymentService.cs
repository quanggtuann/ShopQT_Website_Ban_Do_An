using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ShopDAL.Context;
using ShopDAL.Models;

namespace ShopAPI.Services.Customer;

public sealed class VnpayPaymentService
{
    private readonly ApplicationDbContext _db;
    private readonly IConfiguration _configuration;
    private readonly ILogger<VnpayPaymentService> _logger;
    private static readonly TimeZoneInfo VietnamTimeZone =
        TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

    public VnpayPaymentService(ApplicationDbContext db, IConfiguration configuration,
        ILogger<VnpayPaymentService> logger)
    {
        _db = db;
        _configuration = configuration;
        _logger = logger;
    }

    public bool IsConfigured =>
        _configuration["Vnpay:TmnCode"]?.Length == 8
        && !string.IsNullOrWhiteSpace(_configuration["Vnpay:HashSecret"])
        && Uri.TryCreate(_configuration["Vnpay:BaseUrl"], UriKind.Absolute, out var baseUrl)
        && baseUrl.Scheme == Uri.UriSchemeHttps
        && Uri.TryCreate(_configuration["Vnpay:PaymentBackReturnUrl"], UriKind.Absolute, out var returnUrl)
        && returnUrl.Scheme == Uri.UriSchemeHttps
        && Uri.TryCreate(_configuration["Vnpay:FrontendResultUrl"], UriKind.Absolute, out var resultUrl)
        && resultUrl.Scheme == Uri.UriSchemeHttps;

    public async Task<string> CreatePaymentUrlAsync(int userId, int orderId, string ipAddress)
    {
        if (!IsConfigured)
            throw new InvalidOperationException("VNPay is not configured. Set the merchant keys and HTTPS return URLs.");

        var order = await _db.Orders.AsNoTracking().FirstOrDefaultAsync(x => x.OrderId == orderId && x.UserId == userId);
        if (order == null) throw new KeyNotFoundException("Order not found.");
        if (order.PaymentMethod != PaymentMethod.VNPay || order.IsPaid || order.Status != OrderStatus.Pending)
            throw new InvalidOperationException("This order cannot be paid with VNPay.");
        if (order.TotalAmount <= 0 || order.TotalAmount > 9_999_999_999m
            || order.TotalAmount != decimal.Truncate(order.TotalAmount))
            throw new InvalidOperationException("VNPay requires a positive whole-VND amount within its 12-digit limit.");

        var activeAttempt = await _db.VnpayPaymentAttempts.AsNoTracking()
            .Where(x => x.OrderId == orderId && x.Status == "Pending"
                && x.CreatedAtUtc > DateTime.UtcNow.AddMinutes(-15))
            .OrderByDescending(x => x.Id).FirstOrDefaultAsync();
        if (activeAttempt != null) return activeAttempt.PaymentUrl;

        var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, VietnamTimeZone);
        var attempt = new VnpayPaymentAttempt
        {
            OrderId = orderId,
            TxnRef = $"{orderId}{Guid.NewGuid():N}",
            Amount = order.TotalAmount
        };
        var values = new SortedDictionary<string, string>(StringComparer.Ordinal)
        {
            ["vnp_Amount"] = decimal.ToInt64(order.TotalAmount * 100).ToString(CultureInfo.InvariantCulture),
            ["vnp_Command"] = _configuration["Vnpay:Command"] ?? "pay",
            ["vnp_CreateDate"] = now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture),
            ["vnp_CurrCode"] = _configuration["Vnpay:CurrCode"] ?? "VND",
            ["vnp_ExpireDate"] = now.AddMinutes(15).ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture),
            ["vnp_IpAddr"] = ipAddress,
            ["vnp_Locale"] = _configuration["Vnpay:Locale"] ?? "vn",
            ["vnp_OrderInfo"] = $"Thanh toan don hang {orderId}",
            ["vnp_OrderType"] = "other",
            ["vnp_ReturnUrl"] = _configuration["Vnpay:PaymentBackReturnUrl"]!,
            ["vnp_TmnCode"] = _configuration["Vnpay:TmnCode"]!,
            ["vnp_TxnRef"] = attempt.TxnRef,
            ["vnp_Version"] = _configuration["Vnpay:Version"] ?? "2.1.0"
        };
        var query = BuildQuery(values);
        var hash = Sign(query);
        attempt.PaymentUrl = $"{_configuration["Vnpay:BaseUrl"]}?{query}&vnp_SecureHash={hash}";
        _db.VnpayPaymentAttempts.Add(attempt);
        await _db.SaveChangesAsync();
        return attempt.PaymentUrl;
    }

    public bool IsValidSignature(IQueryCollection query)
    {
        if (!IsConfigured || !TryGet(query, "vnp_SecureHash", out var supplied)
            || !Regex.IsMatch(supplied, "\\A[0-9a-fA-F]{128}\\z"))
            return false;

        var values = new SortedDictionary<string, string>(StringComparer.Ordinal);
        foreach (var pair in query)
        {
            if (pair.Value.Count != 1) return false;
            if (pair.Key.StartsWith("vnp_", StringComparison.Ordinal)
                && pair.Key is not ("vnp_SecureHash" or "vnp_SecureHashType")
                && !string.IsNullOrEmpty(pair.Value[0]))
                values[pair.Key] = pair.Value[0]!;
        }
        var expected = Convert.FromHexString(Sign(BuildQuery(values)));
        var actual = Convert.FromHexString(supplied);
        return CryptographicOperations.FixedTimeEquals(actual, expected);
    }

    public async Task<(string Code, string Message)> ProcessIpnAsync(IQueryCollection query)
    {
        if (!IsValidSignature(query)) return ("97", "Invalid checksum");
        if (!TryGet(query, "vnp_TmnCode", out var tmnCode) || tmnCode != _configuration["Vnpay:TmnCode"])
            return ("97", "Invalid merchant");
        if (!TryGet(query, "vnp_TxnRef", out var reference)) return ("01", "Order not found");

        var attempt = await _db.VnpayPaymentAttempts.AsNoTracking()
            .FirstOrDefaultAsync(x => x.TxnRef == reference);
        if (attempt == null) return ("01", "Order not found");
        if (!TryGet(query, "vnp_Amount", out var amountText)
            || !long.TryParse(amountText, NumberStyles.None, CultureInfo.InvariantCulture, out var amount)
            || amount != attempt.Amount * 100)
            return ("04", "Invalid amount");
        if (attempt.Status != "Pending") return ("02", "Order already confirmed");

        var order = await _db.Orders.AsNoTracking().FirstOrDefaultAsync(x => x.OrderId == attempt.OrderId);
        if (order == null) return ("01", "Order not found");
        if (order.TotalAmount != attempt.Amount) return ("04", "Invalid amount");

        if (!TryGet(query, "vnp_ResponseCode", out var responseCode)
            || !TryGet(query, "vnp_TransactionStatus", out var transactionStatus))
            return ("97", "Missing payment result");
        var paid = responseCode == "00" && transactionStatus == "00";
        await using var transaction = await _db.Database.BeginTransactionAsync();
        var requiresRefund = false;
        if (paid)
        {
            var updated = await _db.Orders
                .Where(x => x.OrderId == attempt.OrderId && !x.IsPaid && x.TotalAmount == attempt.Amount)
                .ExecuteUpdateAsync(x => x.SetProperty(o => o.IsPaid, true));
            var currentStatus = await _db.Orders.AsNoTracking()
                .Where(x => x.OrderId == attempt.OrderId).Select(x => x.Status).SingleAsync();
            requiresRefund = updated != 1 || currentStatus == OrderStatus.Cancelled;
        }
        var transactionNumber = query.TryGetValue("vnp_TransactionNo", out var number)
            ? number.ToString() : null;
        var attemptUpdated = await _db.VnpayPaymentAttempts
            .Where(x => x.Id == attempt.Id && x.Status == "Pending")
            .ExecuteUpdateAsync(x => x
                .SetProperty(a => a.Status, paid ? (requiresRefund ? "RefundRequired" : "Succeeded") : "Failed")
                .SetProperty(a => a.CompletedAtUtc, DateTime.UtcNow)
                .SetProperty(a => a.GatewayTransactionNo, transactionNumber));
        if (attemptUpdated != 1) return ("02", "Order already confirmed");
        await transaction.CommitAsync();
        if (requiresRefund)
            _logger.LogError("VNPay payment {TxnRef} for order {OrderId} requires a manual refund.",
                reference, attempt.OrderId);
        return ("00", "Confirm Success");
    }

    public async Task<(int? OrderId, string Result)> GetReturnResultAsync(IQueryCollection query)
    {
        if (!IsValidSignature(query)
            || !TryGet(query, "vnp_TmnCode", out var tmnCode)
            || tmnCode != _configuration["Vnpay:TmnCode"]
            || !TryGet(query, "vnp_TxnRef", out var reference))
            return (null, "invalid");
        var attempt = await _db.VnpayPaymentAttempts.AsNoTracking()
            .FirstOrDefaultAsync(x => x.TxnRef == reference);
        if (attempt == null || !TryGet(query, "vnp_Amount", out var amountText)
            || !long.TryParse(amountText, NumberStyles.None, CultureInfo.InvariantCulture, out var amount)
            || amount != attempt.Amount * 100)
            return (null, "invalid");
        var success = TryGet(query, "vnp_ResponseCode", out var response) && response == "00"
            && TryGet(query, "vnp_TransactionStatus", out var status) && status == "00";
        return (attempt.OrderId, success ? "pending" : "failed");
    }

    public string GetFrontendResultUrl(int? orderId, string result)
    {
        var uri = _configuration["Vnpay:FrontendResultUrl"]
            ?? throw new InvalidOperationException("FrontendResultUrl is missing.");
        return $"{uri}{(uri.Contains('?') ? '&' : '?')}orderId={orderId}&result={WebUtility.UrlEncode(result)}";
    }

    private static bool TryGet(IQueryCollection query, string key, out string value)
    {
        value = string.Empty;
        if (!query.TryGetValue(key, out var values) || values.Count != 1 || string.IsNullOrWhiteSpace(values[0]))
            return false;
        value = values[0]!;
        return true;
    }

    private string Sign(string data)
    {
        var key = Encoding.UTF8.GetBytes(_configuration["Vnpay:HashSecret"]!);
        return Convert.ToHexString(HMACSHA512.HashData(key, Encoding.UTF8.GetBytes(data))).ToLowerInvariant();
    }

    private static string BuildQuery(IEnumerable<KeyValuePair<string, string>> values) => string.Join("&",
        values.Select(x => $"{Encode(x.Key)}={Encode(x.Value)}"));

    private static string Encode(string value) => Uri.EscapeDataString(value).Replace("%20", "+");
}
