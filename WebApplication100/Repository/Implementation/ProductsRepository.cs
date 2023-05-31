using Microsoft.EntityFrameworkCore;
using WebApplication100.DTO;
using WebApplication100.Models;
using WebApplication100.Repository.Interface;
using WebApplication100.Service;

namespace WebApplication100.Repository.Implementation
{
    public class ProductsRepository: IProductsRepository
    {
        private readonly AssignmentDBContext assignmentDBContext;
        private readonly ITimeZoneService timeZoneService;

        public ProductsRepository(AssignmentDBContext assignmentDBContext, ITimeZoneService timeZoneService)
        {
            this.assignmentDBContext = assignmentDBContext;
            this.timeZoneService = timeZoneService;
        }       

        public async Task<Product> AddAsync(Product product, ProductVendor vendorProduct)
        {
            product.ProductId = new int();
            product.Availability = true;

            await assignmentDBContext.AddAsync(product);

            await assignmentDBContext.SaveChangesAsync();

            vendorProduct.ProductVendorId = new int();
            vendorProduct.ProductId = product.ProductId;

            await assignmentDBContext.AddAsync(vendorProduct);

            await assignmentDBContext.SaveChangesAsync();

            return product; 
        }

        public async Task<IEnumerable<Product>> GetAll()
        {
            return await assignmentDBContext.Products
                .Include(x => x.CreatedByUser)
                .Where(x => x.Availability == true)
                .ToListAsync(); 
        }
        public async Task<Product> GetAsync(int productId)
        {
            return await assignmentDBContext.Products
                .Include(x => x.CreatedByUser)
                .Include(x => x.ProductVendors).ThenInclude(x => x.UserProfile)
                .FirstOrDefaultAsync(x => x.ProductId == productId); 

        }

        public async Task<IEnumerable<ProductVendor>> GetByVendor(int userId)
        {
            return await assignmentDBContext.ProductVendors.Where(x => x.UserProfileId == userId)
                .ToListAsync(); 
        }
        public async Task<bool> IsVendorProduct(int userId, int productId)
        {
            var ifEducator = await assignmentDBContext.ProductVendors.FirstOrDefaultAsync(x=> x.UserProfileId == userId && x.ProductId == productId);

            if( ifEducator== null)
            {
                return false; 
            }
            else
            {
                return true; 
            }
        }
        public async Task<Product> UpdateAsync(int productId, UpdateProduct updateProduct)
        {
            var update = await assignmentDBContext.Products.FirstOrDefaultAsync(x => x.ProductId == productId); 
            if(update == null)
            {
                return null; 
            }
            if(updateProduct.ProductName!=null)
            {
                update.ProductName = updateProduct.ProductName;
            }

            if(updateProduct.Description!=null)
            {
                update.Description = updateProduct.Description; 
            }

            if( updateProduct.Price!=null)
            {
                update.Price = updateProduct.Price; 
            }

            update.UpdatedTime = timeZoneService.ChangeTimeZoneToRegional(DateTime.UtcNow);

            await assignmentDBContext.SaveChangesAsync();
            return update; 
        }

        public async Task<Product> DeleteAsync(int productId)
        {
            var delete = await assignmentDBContext.Products.FirstOrDefaultAsync(x => x.ProductId == productId);

            if( delete == null)
            {
                return null; 
            }

            assignmentDBContext.Products.Remove(delete);

            await assignmentDBContext.SaveChangesAsync();
            return delete; 
        }

    }
}
