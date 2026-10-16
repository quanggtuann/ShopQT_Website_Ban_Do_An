using System.Security.Claims;
using System.Net;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopAPI.Services.Customer;

namespace ShopAPI.Controllers.Customer;

[ApiController]
[Route("api/payments/vnpay")]
public class VnpayPaymentsController : ControllerBase
{
    private readonly VnpayPaymentService _payments;

    public VnpayPaymentsController(VnpayPaymentService payments) => _payments = payments;

    [AllowAnonymous]
    [HttpGet("availability")]
    public IActionResult Availability([FromServices] IConfiguration configuration,
        [FromServices] IWebHostEnvironment environment)
    {
        var available = _payments.IsConfigured;
        if (!environment.IsDevelopment()) return Ok(new { available });

        static bool Https(string? value) => Uri.TryCreate(value, UriKind.Absolute, out var uri)
            && uri.Scheme == Uri.UriSchemeHttps;
        return Ok(new
        {
            available,
            checks = new
            {
                merchantCode = configuration["Vnpay:TmnCode"]?.Length == 8,
                secret = !string.IsNullOrWhiteSpace(configuration["Vnpay:HashSecret"]),
                gatewayUrl = Https(configuration["Vnpay:BaseUrl"]),
                returnUrl = Https(configuration["Vnpay:PaymentBackReturnUrl"]),
                resultUrl = Https(configuration["Vnpay:FrontendResultUrl"])
            }
        });
    }

    [Authorize]
    [HttpPost("orders/{orderId:int}")]
    public async Task<IActionResult> Start(int orderId, [FromBody] StartPaymentRequest request)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();
        try
        {
            var parsed = IPAddress.TryParse(request?.IpAddress, out var supplied)
                ? supplied
                : HttpContext.Connection.RemoteIpAddress;
            var ipAddress = parsed == null || IPAddress.IsLoopback(parsed)
                ? IPAddress.Loopback.ToString()
                : parsed.IsIPv4MappedToIPv6 ? parsed.MapToIPv4().ToString() : parsed.ToString();
            var url = await _payments.CreatePaymentUrlAsync(userId, orderId, ipAddress);
            return Ok(new { paymentUrl = url });
        }
        catch (KeyNotFoundException) { return NotFound(new { message = "Order not found." }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [AllowAnonymous]
    [HttpGet("ipn")]
    public async Task<IActionResult> Ipn()
    {
        var result = await _payments.ProcessIpnAsync(Request.Query);
        return Ok(new VnpayIpnResponse { RspCode = result.Code, Message = result.Message });
    }

    [AllowAnonymous]
    [HttpGet("return")]
    public async Task<IActionResult> Return([FromServices] IWebHostEnvironment environment,
        [FromServices] IConfiguration configuration)
    {
        // Local sandbox testing cannot receive VNPay's server-to-server IPN. Accept the same
        // signed result through the browser return in Development only, against the sandbox host.
        if (environment.IsDevelopment()
            && Uri.TryCreate(configuration["Vnpay:BaseUrl"], UriKind.Absolute, out var gateway)
            && gateway.Host.Equals("sandbox.vnpayment.vn", StringComparison.OrdinalIgnoreCase))
            await _payments.ProcessIpnAsync(Request.Query);

        var result = await _payments.GetReturnResultAsync(Request.Query);
        return Redirect(_payments.GetFrontendResultUrl(result.OrderId, result.Result));
    }

    public sealed class StartPaymentRequest
    {
        public string? IpAddress { get; set; }
    }

    public sealed class VnpayIpnResponse
    {
        [JsonPropertyName("RspCode")]
        public string RspCode { get; set; } = string.Empty;

        [JsonPropertyName("Message")]
        public string Message { get; set; } = string.Empty;
    }
}
