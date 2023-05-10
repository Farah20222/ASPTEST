using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication100.DTO;
using WebApplication100.Models;
using WebApplication100.Repository.Interface;
using WebApplication100.Service;

namespace WebApplication100.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseProductsController : Controller
    {
        // to access the purchase database methods
        private readonly IPurchaseProductsRepository purchaseProductsRepository;
        // used to map models to DTOs
        private readonly IMapper mapper;
        // to set the timezone to regional time
        private readonly ITimeZoneService timeZoneService;
        // used to access the products database methods
        private readonly IProductsRepository productsRepository; 

        public PurchaseProductsController(IPurchaseProductsRepository purchaseProductsRepository, IMapper mapper, ITimeZoneService timeZoneService,
            IProductsRepository productsRepository)
        {
            this.purchaseProductsRepository = purchaseProductsRepository;
            this.mapper = mapper;
            this.timeZoneService = timeZoneService;
            this.productsRepository = productsRepository;
        }

        /// <summary>
        /// HTTP POST endpoint to purchase a product
        /// Authorize header to ensure the user is logged in 
        /// </summary>
        /// <param name="newPurchase"></param>
        /// <returns>
        /// Newly created purchase
        /// </returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PurchaseProduct([FromForm] NewPurchase newPurchase)
        {
            // Get the user ID from the current HTTP context's user claims
            var userId = UserClaims.GetUserClaimID(HttpContext);

            // Get the product to be purchased by its ID
            var product = await productsRepository.GetAsync((int)newPurchase.ProductId);

            // If no product is found with the given ID, return a 404 Not Found response with a message
            if (product== null)
            {
                return NotFound("The requested product is not available.");
            }

            // Create a new 'Purchase' object
            var purchase = new Purchase()
            {
                ProductId = newPurchase.ProductId,
                UserProfileId = userId,
                PurchasedAt= timeZoneService.ChangeTimeZoneToRegional(DateTime.UtcNow), 
                PurchaseCost= product.Price
            };

            // Add the new 'Purchase' object to the 'PurchaseProducts' table in the database 
            purchase = await purchaseProductsRepository.AddPurchase(purchase);
            // map the DTO 
            var purchaseDTO = mapper.Map<PurchaseDTO>(purchase);

            return Ok(purchaseDTO); 
        }


        /// <summary>
        /// // HTTP GET endpoint for retrieving list of all purchases made by the logged in user
        /// Authorize header to ensure the user is logged in 
        /// </summary>
        /// <returns>
        /// List of all products purchased by the user
        /// </returns>
        [HttpGet("GetCustomerPurchases")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<UserProfile>>> GetCustomerPurchases()
        {
            // Get the user ID from the current HTTP context's user claims
            var userId = UserClaims.GetUserClaimID(HttpContext);

            //Get list of purchases made by the current user 
            var customerPurchases = await purchaseProductsRepository.GetByCustomer(userId);

            // If no purchases are found for the current user, return a 404 Not Found response with a message
            if (customerPurchases== null)
            {
                return NotFound("No user found with the given id: " + userId);
            }

            // Map to the DTO
            var purchaseDTO = mapper.Map<List<PurchaseDTO>>(customerPurchases);

            // Return an OK 200 response with the list of purchases 
            return Ok(purchaseDTO); 
        }
    }
}
