namespace ShopView.Areas.Admin.ViewModel
{
    public class PagedResponse<T>
    {
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public List<T> Data { get; set; } = new List<T>();
    }
}
