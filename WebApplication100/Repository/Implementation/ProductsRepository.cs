using Microsoft.EntityFrameworkCore;
using WebApplication100.DTO;
using WebApplication100.Models;
using WebApplication100.Repository.Interface;
using WebApplication100.Service;

namespace WebApplication100.Repository.Implementation
{
    public class ProductsRepository: IProductsRepository
    {
        // Context file to get access to the database
        private readonly AssignmentDBContext assignmentDBContext;
        // Used to get the time for the created and updated time
        private readonly ITimeZoneService timeZoneService;

        public ProductsRepository(AssignmentDBContext assignmentDBContext, ITimeZoneService timeZoneService)
        {
            this.assignmentDBContext = assignmentDBContext;
            this.timeZoneService = timeZoneService;
        }       

        /// <summary>
        /// Repository method to add a new product to the dabatase
        /// </summary>
        /// <param name="product"></param>
        /// <param name="vendorProduct"></param>
        /// <returns>
        /// The newly created product object
        /// </returns>
        public async Task<Product> AddAsync(Product product, ProductVendor vendorProduct)
        {
            product.ProductId = new int();
            //Set availability to true for new products
            product.Availability = true;

            //Add the product to the database
            await assignmentDBContext.AddAsync(product);
            // Save the changes to the database asynchronously

            await assignmentDBContext.SaveChangesAsync();

            vendorProduct.ProductVendorId = new int();
            vendorProduct.ProductId = product.ProductId;

            //Add the vendor-product relationship to the database
            await assignmentDBContext.AddAsync(vendorProduct);

            // Save the changes to the database asynchronously
            await assignmentDBContext.SaveChangesAsync();

            return product; 
        }


        /// <summary>
        /// Method to get a list of all availavle products from the dabatase 
        /// </summary>
        /// <returns>
        /// List of all the available products
        /// </returns>
        public async Task<IEnumerable<Product>> GetAll()
        {
            return await assignmentDBContext.Products
                .Include(x => x.CreatedByUser)
                .Where(x => x.Availability == true)
                .ToListAsync(); 
        }

        /// <summary>
        /// Method to get one product based on the product Id
        /// </summary>
        /// <param name="productId"></param>
        /// <returns>
        /// The requested product
        /// </returns>
        public async Task<Product> GetAsync(int productId)
        {
            return await assignmentDBContext.Products
                .Include(x => x.CreatedByUser)
                .Include(x => x.ProductVendors).ThenInclude(x => x.UserProfile)
                .FirstOrDefaultAsync(x => x.ProductId == productId); 

        }

        /// <summary>
        /// Method to get all the products under one vendor only 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ProductVendor>> GetByVendor(int userId)
        {
            return await assignmentDBContext.ProductVendors.Where(x => x.UserProfileId == userId)
                .ToListAsync(); 
        }

        /// <summary>
        /// A method for checking if a product belongs to a specific vendor
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        public async Task<bool> IsVendorProduct(int userId, int productId)
        {
            // Check if a row exists with the given user ID and product ID
            var ifEducator = await assignmentDBContext.ProductVendors.FirstOrDefaultAsync(x=> x.UserProfileId == userId && x.ProductId == productId);

            // If no row is ofund, return false
            if( ifEducator== null)
            {
                return false; 
            }
            // Otherwise, return true
            else
            {
                return true; 
            }
        }

        /// <summary>
        /// A method for updating a product in the database asyncronously 
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="updateProduct"></param>
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

            // Set the 'UpdatedTime' property of the 'update' object to the current time in the regional time zone 
            update.UpdatedTime = timeZoneService.ChangeTimeZoneToRegional(DateTime.UtcNow);

            // Save the changes to the database asynchronously
            await assignmentDBContext.SaveChangesAsync();
            return update; 
        }

        /// <summary>
        /// A method to delete a product from the database asynchronously using EF
        /// </summary>
        /// <param name="productId"></param>

        public async Task<Product> DeleteAsync(int productId)
        {
            // Get the product by the ID
            var delete = await assignmentDBContext.Products.FirstOrDefaultAsync(x => x.ProductId == productId);

            // If no product is found with the given ID, return null
            if( delete == null)
            {
                return null; 
            }

            // Remove the 'delete' object from the 'Products' table in the database context
            assignmentDBContext.Products.Remove(delete);

            // Save the changes to the database asynchronously
            await assignmentDBContext.SaveChangesAsync();
            return delete; 
        }

    }
}
