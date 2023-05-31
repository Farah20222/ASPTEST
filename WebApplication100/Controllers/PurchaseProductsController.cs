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
        private readonly IPurchaseProductsRepository purchaseProductsRepository;
        private readonly IMapper mapper;
        private readonly ITimeZoneService timeZoneService;
        private readonly IProductsRepository productsRepository; 

        public PurchaseProductsController(IPurchaseProductsRepository purchaseProductsRepository, IMapper mapper, ITimeZoneService timeZoneService,
            IProductsRepository productsRepository)
        {
            this.purchaseProductsRepository = purchaseProductsRepository;
            this.mapper = mapper;
            this.timeZoneService = timeZoneService;
            this.productsRepository = productsRepository;
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PurchaseProduct([FromForm] NewPurchase newPurchase)
        {
            var userId = UserClaims.GetUserClaimID(HttpContext);

            var product = await productsRepository.GetAsync((int)newPurchase.ProductId);
            if (product== null)
            {
                return NotFound("The requested product is not available.");
            }

            var purchase = new Purchase()
            {
                ProductId = newPurchase.ProductId,
                UserProfileId = userId,
                PurchasedAt= timeZoneService.ChangeTimeZoneToRegional(DateTime.UtcNow), 
                PurchaseCost= product.Price
            };

            purchase = await purchaseProductsRepository.AddPurchase(purchase);
            var purchaseDTO = mapper.Map<PurchaseDTO>(purchase);

            return Ok(purchaseDTO); 
        }

        [HttpGet("GetCustomerPurchases")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<UserProfile>>> GetCustomerPurchases()
        {
            var userId = UserClaims.GetUserClaimID(HttpContext);

            var customerPurchases = await purchaseProductsRepository.GetByCustomer(userId);

            if (customerPurchases== null)
            {
                return NotFound("No user found with the given id: " + userId);
            }

            var purchaseDTO = mapper.Map<List<PurchaseDTO>>(customerPurchases);

            return Ok(purchaseDTO); 
        }
    }
}
