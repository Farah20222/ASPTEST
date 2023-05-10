using Microsoft.EntityFrameworkCore;

namespace WebApplication100.Models
{
    public partial class AssignmentDBContext : DbContext
    {
        public AssignmentDBContext()
        {
        }

        public AssignmentDBContext(DbContextOptions<AssignmentDBContext> options) : base(options)
        { }

        public virtual DbSet<UserProfile> UserProfiles { get; set; }
        public virtual DbSet<ProductVendor> ProductVendors { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Purchase> Purchases { get; set; }



    }
}
