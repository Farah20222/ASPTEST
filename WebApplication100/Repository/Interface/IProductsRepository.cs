using WebApplication100.DTO;
using WebApplication100.Models;

namespace WebApplication100.Repository.Interface
{
    public interface IProductsRepository
    {
        Task<Product> AddAsync(Product product, ProductVendor vendorProduct);
        Task<Product> GetAsync(int productId);
        Task<IEnumerable<ProductVendor>> GetByVendor(int userId);
        Task<bool> IsVendorProduct(int userId, int productId);
        Task<Product> UpdateAsync(int productId, UpdateProduct updateProduct);
        Task<IEnumerable<Product>> GetAll();
        Task<Product> DeleteAsync(int productId); 
    }
}
