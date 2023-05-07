using Microsoft.EntityFrameworkCore;
using WebApplication100.DTO;
using WebApplication100.Models;
using WebApplication100.Repository.Interface;
using WebApplication100.Service;

namespace WebApplication100.Repository.Implementation
{
    public class ProductRepository: IProductRepository
    {

        private readonly AssignmentDBContext assignmentDBContext;
        private readonly ITimeZoneService timeZoneService; 

        public ProductRepository(AssignmentDBContext assignmentDBContext, ITimeZoneService timeZoneService)
        {
            this.assignmentDBContext = assignmentDBContext;
            this.timeZoneService = timeZoneService;
        }   


        public async Task<Product> AddAsync(Product product,  ProductVendor productVendor)
        {
            product.ProductId = new int();
            product.Availability = true;
            await assignmentDBContext.AddAsync(product);
            await assignmentDBContext.SaveChangesAsync();

            productVendor.ProductVendorId = new int();
            productVendor.ProductId = product.ProductId;
            await assignmentDBContext.AddAsync(product);
            await assignmentDBContext.SaveChangesAsync(); 

            return product;

        }


        public async Task<IEnumerable<Product>>GetAll()
        {
            return await assignmentDBContext.Products.ToListAsync(); 
        }

        public async Task<Product>GetAsync(int id)
        {
            return await assignmentDBContext.Products
                 .Include(x => x.VendorProfile)
                 .FirstOrDefaultAsync(x => x.ProductId == id); 
        }

        public async Task<IEnumerable<ProductVendor>> GetByVendorAsync(int userId)
        {
            return await assignmentDBContext.ProductVendors.Where(x => x.UserProfileId == userId)
                .ToListAsync(); 
        }

      

        public async Task<bool> IsVendorProductCheck(int userId , int productId)
        {
            var isVendor = await assignmentDBContext.ProductVendors.FirstOrDefaultAsync(x => x.UserProfileId == userId && x.ProductId == productId); 
            if(isVendor == null)
            {
                return false; 
            }

            else
            {
                return true; 
            }

        }


        public async Task<Product> UpdateAsync(int id, UpdateProduct updateProduct) 
        {
            var update = await assignmentDBContext.Products.FirstOrDefaultAsync(x => x.ProductId == id); 
            if(update == null)
            {
                return null; 
            }

            if(updateProduct.ProductName!=null)
            {
                update.ProductName = updateProduct.ProductName.Trim();
            }

            if(updateProduct.Description!=null)
            {
                update.Description = updateProduct.Description; 
            }

            if (updateProduct.Price != null)
            {
                update.Price = updateProduct.Price;
            }

            update.UpdatedTime = timeZoneService.ChangeTimeZoneToRegional(DateTime.UtcNow);
            await assignmentDBContext.SaveChangesAsync();
            return update;


        }





    }
}
