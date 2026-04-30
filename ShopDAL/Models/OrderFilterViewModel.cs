namespace ShopDAL.Models
{
    // tim kiem loc don hang theo id, ngay , tt,gia
    public class OrderFilterViewModel
    {
        public int? OrderId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Status {  get; set; }
        public string? Shortby {  get; set; }
        public string? ShotOrder { get; set; } = "asc";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public List<Order> Items { get; set; }

    }
}
