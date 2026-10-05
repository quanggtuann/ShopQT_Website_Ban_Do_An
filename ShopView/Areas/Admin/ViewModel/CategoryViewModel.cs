using System.ComponentModel.DataAnnotations;

namespace ShopView.Areas.Admin.Models
{
    public class CategoryViewModel
    {
        public int CategoryId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }
    }

    public class CategoryCreateViewModel
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }
    }
}
