using WebApplication100.Models;
using static WebApplication100.DTO.UserDTO;

namespace WebApplication100.DTO
{
    public class ProductDTO
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int? VendorId { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public string? Description { get; set; }
        public bool? Availability { get; set; }
        public decimal Price { get; set; }


        public virtual UserProfileDTO? VendorProfile { get; set; }
        public virtual ICollection<ProductVendorDTO> ProductVendors { get; set; }
    }

    public class ProductVendorDTO
    {
        public UserProfile? User { get; set; }
    }

}
