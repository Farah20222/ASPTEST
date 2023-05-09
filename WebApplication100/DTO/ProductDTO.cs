using WebApplication100.Models;
using static WebApplication100.DTO.UserDTO;

namespace WebApplication100.DTO
{
    public class ProductDTO
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int? Vendor { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public string? Description { get; set; }
        public bool? Availability { get; set; }
        public decimal Price { get; set; }


        public  UserProfileDTO? VendorUser { get; set; }
        public  ICollection<ProductVendorDTO> ProductVendors { get; set; }
    }

    public class ProductVendorDTO
    {
        public UserProfile? User { get; set; }
    }

    public class AddNewProduct
    {
        public string? ProductName { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
    }

    public class UpdateProduct
    {
        public string? ProductName { get; set; }
        public string? Description { get; set; }
        public bool? Availability { get; set; }
        public decimal Price { get; set; }
    }

}
