namespace ShopDAL.Models
{
    public class AccountFilterViewModel
    {
        public string? Keyword { get; set; }
        public bool? IsActive { get; set; }
        public string? Role { get; set; }
        public string? SortBy { get; set; } = "id";
        public string? SortOrder { get; set; } = "asc";
        public int page { get; set; } = 1;
        public int pageSize { get; set; } = 10;
    }
}
