using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication100.DTO;
using WebApplication100.Models;
using WebApplication100.Repository.Interface;
using WebApplication100.Service;

namespace WebApplication100.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        // to get access to products repository EF methods
        private readonly IProductsRepository productsRepository;
        // to map from models to dto and vice versa
        private readonly IMapper mapper;
        // to set the time zone to the regional time
        private readonly ITimeZoneService timeZoneService;

        public ProductsController(IProductsRepository productsRepository, IMapper mapper, ITimeZoneService timeZoneService)
        {
            this.productsRepository = productsRepository;
            this.mapper = mapper;
            this.timeZoneService = timeZoneService;
        }


        /// <summary>
        /// // HTTP GET endpoint handler for the route '/GetAllProducts'
        /// </summary>
        /// <returns>
        /// Returns a list of all the products available from the database without authorization 
        /// </returns>

        [HttpGet("GetAllProducts")]
        public async Task<ActionResult<IEnumerable<Product>>> GetAllProducts()
        {
            //Retrieve all the products
            var products = await productsRepository.GetAll();
            
            // Map the retireved products to the products DTO using mapper
            var productsDTO = mapper.Map<List<ProductDTO>>(products);

            // Return OkObjectResult that returns the DTO and an HTTP status of 200
            return Ok(productsDTO); 
        }


        /// <summary>
        /// // HTTP GET endpoint handler for the route '/GetProductInformation/{productId:int}
        /// </summary>
        /// <param name="productId"></param>
        /// <returns>
        /// Gets products based on the product ID provided in the parameter
        /// </returns>

        [HttpGet("GetProductInformation/{productId:int}")]
        [ActionName("GetProduct")]
        public async Task<ActionResult<Product>> GetProduct(int productId)
        {
            var product = await productsRepository.GetAsync(productId);
            // If the product is not found, return a 404 Not Found response with a message
            if (product == null)
            {
                return NotFound("Product Not found");
            }

            // Map the retrieved product 
            var productDTO = mapper.Map<ProductDTO>(product);

            // Return OkObjectResult containint the product DTO with an OK status
            return Ok(productDTO);
        }


        /// <summary>
        /// HTTP Post endpoint to add a new product 
        /// Authorize the request based on policy named "IsAdminOrVendor"
        /// Vendors are the product owners and have the ability of adding products to the database
        /// </summary>
        /// <param name="addProduct"></param>
        /// <returns>
        /// The newly created product 
        /// </returns>
        [HttpPost("[action]")]
        [Authorize(Policy = "IsAdminOrVendor")]
        public async Task<IActionResult> AddProduct(AddProduct addProduct)
        {
            // Get the vendor's ID from the current HTTP context user claims
            var userId = UserClaims.GetUserClaimID(HttpContext);

            //Create a new product object using addProduct as the input data from the user 
            var newProduct = new Product()
            {
                ProductName = addProduct.ProductName,
                // Created By user is the product's vendor
                CreatedBy = userId,
                Description = addProduct.Description,
                CreatedTime = timeZoneService.ChangeTimeZoneToRegional(DateTime.UtcNow),
                Price = addProduct.Price,

            };

            //Link the product to the vendor for reference 
            var vendorProduct = new ProductVendor()
            {
                UserProfileId = userId
            };
            // Add the new product and vendor to the repository method 'AddAsync'
            newProduct = await productsRepository.AddAsync(newProduct, vendorProduct); 

            var productDTO = mapper.Map<ProductDTO>(newProduct);

            // Return a created at action response with the location pointing to the newly created product's URL
            return CreatedAtAction(nameof(GetProduct), new { productId = productDTO.ProductId }, productDTO);

        }

        /// <summary>
        /// HTTP Get endpoint to get all the products for the current logged in user
        /// </summary>
        /// <returns>
        /// List of all the vendor's products
        /// </returns>
        [HttpGet("GetVendorProducts")]
        public async Task<ActionResult<IEnumerable<ProductVendor>>> GetVendorProduct()
        {
            //Get the user ID from the current HTTP context's user claims
            var userId = UserClaims.GetUserClaimID(HttpContext);
            //Method to get all the vendors products using the logged in user's ID
            var products = await productsRepository.GetByVendor(userId);
            if (products == null)
            {
                return NotFound("No products found with the given id: " + userId);
            }
            // Map the dto
            var productsDTO = mapper.Map<List<ProductVendorDTO>>(products);
            // Return status of OK with the products DTO
            return Ok(productsDTO);
        }


        /// <summary>
        /// HTTP PUT Endpoint for updating a product by the ID
        /// Authorize the request based on a policy named "IsAdminOrVendor"
        /// Vendors can only update their own products
        /// Admins have access to update any product

        /// </summary>
        /// <param name="productId"></param>
        /// <param name="updateProduct"></param>
        /// <returns>
        /// The newly updated product object
        /// </returns>

        [HttpPut("UpdateProducts/{productId:int}")]
        [Authorize(Policy = "IsAdminOrVendor")]
        public async Task<ActionResult> UpdateProductsAsync([FromRoute] int productId, UpdateProduct updateProduct)
        {
            // Get the user ID from the current HTTP context's user claims
            var userId = UserClaims.GetUserClaimID(HttpContext);

            // Get the logged in user's roles from the claims
            var loggedInUserRole = User.FindFirstValue(ClaimTypes.Role);

            // Check if the current user is authorized to update this product
            var vendorProducts = await productsRepository.IsVendorProduct(userId, productId);

            if (loggedInUserRole == "vendor" && vendorProducts == false)
            {
                return Unauthorized("User unauthorized");

            }

            // Call the 'UpdateAsync' method to update the product 
            var update = await productsRepository.UpdateAsync(productId, updateProduct);

            // If the product is not found, return a 404 Not Found response with a message
            if (update == null)
            {
                return NotFound($"No products found with the productId : {productId}");
            }

            // convert domain back to DTO
            var productDTO = mapper.Map<ProductDTO>(update);
            return Ok(productDTO); 

        }



        /// <summary>
        /// HTTP DELETE endpoint to delete a product by the ID
        /// Authorization: Only vendors and admin have access to this endpoint 
        /// Vendors can only delete their own product
        /// Admins have access to delete any product
        /// </summary>
        /// <param name="productId"></param>
        /// <returns>
        /// The deleted product object
        /// </returns>
        [HttpDelete("DeleteProduct/{productId:int}")]
        [Authorize(Policy = "IsAdminOrVendor")]
        public async Task<IActionResult>DeleteProduct(int productId)
        {
            // Call the 'GetAsync' method to retrieve the product to be deleted
            var product = await productsRepository.GetAsync(productId); 
            // If product is not found, return a 404 Not Found response 
            if(product== null)
            {
                return NotFound($"Product with the ID:{productId} is not found."); 
            }

            // Get the user ID and role from the current HTTP context's user claims
            var userId = UserClaims.GetUserClaimID(HttpContext);

            // Get the logged in user's roles from the claims
            var loggedInUserRole = User.FindFirstValue(ClaimTypes.Role);
            //Check if the current user is a vendor and is authorized to delete this product
            var vendorProduct = await productsRepository.IsVendorProduct(userId, productId);
            if (loggedInUserRole == "vendor" && vendorProduct == false)
            {
                return Unauthorized("User unauthorized");
            }

            // Call the 'DeleteAsync' method to delete the requested product
            var deleteProduct = await productsRepository.DeleteAsync(productId);


            // Map the deleted 'Product' object to a 'ProductDTO' object using the 'mapper' object
            var productDTO = mapper.Map<ProductDTO>(deleteProduct);

            //Return an OK status of 200 and the deleted product object
            return Ok(productDTO); 

        }


    }
}
