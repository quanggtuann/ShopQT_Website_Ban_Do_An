namespace ShopDAL.Models
{
    //tim kiem loc combo theo key, gia
    public class ComboFilterViewmodel
    {
        public string? KeyWord { get; set; }
        public decimal? FromPrice { get; set; }
        public decimal? ToPrice { get; set; }
        public string ShortBy { get; set; } = "name";
        public string ShortOrder { get; set; } = "asc";
        public int page { get; set; } = 1;
        public int pageSize { get; set; } = 5;
    }
}
