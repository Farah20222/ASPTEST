namespace WebApplication100.Models
{
    public class Product
    {

        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int? VendorId { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public string? Description { get; set; }
        public bool? Availability { get; set; }
        public decimal Price { get; set; }


        public virtual UserProfile? VendorProfile { get; set; }
        public virtual ICollection<ProductVendor> ProductVendors { get; set; }

    }
}
