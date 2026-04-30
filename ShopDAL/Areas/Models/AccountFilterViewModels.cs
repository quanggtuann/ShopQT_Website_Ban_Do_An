using ShopDAL.Models;



namespace ShopDAL.Areas.Models

{

    public class AccountFilterViewModels

    {

        public string KeyWord {  get; set; }

        public bool? IsActive { get; set; }

        public string Role {  get; set; }

        public string SortBy {  get; set; }

        public string SortOrder { get; set; }

        public int Page {  get; set; } =1;

        public int PageSize { get; set; } = 10;

        public List<User> Accounts { get; set; }

    }

}

