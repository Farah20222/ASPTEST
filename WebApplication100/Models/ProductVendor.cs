
namespace WebApplication100.Models
{
    public partial class ProductVendor
    {
        public int ProductVendorId { get; set; }
        public int? UserProfileId { get; set; }

        public int? ProductId { get; set; }

        public virtual Product? Product { get; set; }
        public virtual UserProfile? UserProfile { get; set; }  

    }
}
