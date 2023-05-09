using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication100.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public string? Description { get; set; }

        public decimal? Price { get; set; }
        public bool? Availability { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual UserProfile? CreatedByUser { get; set; }

        public virtual ICollection<ProductVendor>? ProductVendors {get; set;}


    }
}
