using System.ComponentModel.DataAnnotations;

namespace ShopDAL.Models;

public class VnpayPaymentAttempt
{
    public int Id { get; set; }
    [MaxLength(100)]
    public string TxnRef { get; set; } = string.Empty;
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public decimal Amount { get; set; }
    [MaxLength(20)]
    public string Status { get; set; } = "Pending";
    [MaxLength(50)]
    public string? GatewayTransactionNo { get; set; }
    [MaxLength(2048)]
    public string PaymentUrl { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAtUtc { get; set; }
}
