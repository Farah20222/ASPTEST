//using AutoMapper;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using System.Security.Claims;
//using WebApplication100.DTO;
//using WebApplication100.Models;
//using WebApplication100.Repository.Interface;
//using WebApplication100.Service;

//namespace WebApplication100.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class ProductController : ControllerBase
//    {
//        private readonly IProductRepository productRepository;
//        private readonly IMapper mapper;
//        private readonly ITimeZoneService timeZoneService;

//        public ProductController(IProductRepository productRepository, IMapper mapper, ITimeZoneService timeZoneService)
//        {
//            this.productRepository = productRepository;
//            this.mapper = mapper;
//            this.timeZoneService = timeZoneService;
//        }

//        [HttpGet("GetProductById/{productId:int}")]
//        [ActionName("GetProductInformation")]
//        public async Task<ActionResult<Product>> GetProductInformation(int productId)
//        {
//            var product = await productRepository.GetAsync(productId);
//            if (product is null)
//            {
//                return NotFound($"Product not found with the given Id: {productId}.");
//            }

//            var productDTO = mapper.Map<ProductDTO>(product);

//            return Ok(productDTO);
//        }


//        [HttpPost("[action]")]
//        [Authorize(Policy = "IsAdminOrVendor")]
//        public async Task<IActionResult> AddProduct([FromForm] AddNewProduct newProduct)
//        {
//            var userId = UserClaims.GetUserClaimID(HttpContext);
//            var product = new Product()
//            {
//                ProductName = newProduct.ProductName,
//                VendorId = userId,
//                CreatedTime = timeZoneService.ChangeTimeZoneToRegional(DateTime.UtcNow),
//                Description = newProduct.Description,
//                Availability = true,
//                Price = newProduct.Price
//            };

//            var vendorProduct = new ProductVendor()
//            {
//                UserProfileId = userId
//            };


//            product = await productRepository.AddAsync(product, vendorProduct);
//            var productDTO = mapper.Map<ProductDTO>(product);
//            return CreatedAtAction(nameof(GetProductInformation), new { id = productDTO.ProductId }, productDTO);
//        }

//        [HttpPut("UpdateProduct/{productId:int}")]
//        [Authorize(Policy = "IsAdminOrVendor")]
//        public async Task<ActionResult> UpdateProductAsync([FromRoute] int productId, UpdateProduct updateProduct)
//        {
//            var userId = UserClaims.GetUserClaimID(HttpContext);
//            var loggedInUserRole = User.FindFirstValue(ClaimTypes.Role);
//            var IsVendorProduct = await productRepository.IsVendorProductCheck(userId, productId);
//            if (loggedInUserRole == "vendor" && IsVendorProduct == false)
//            {
//                return Unauthorized("Vendor Unauthorized to edit the requested product.");
//            }

//            var productUpdate = await productRepository.UpdateAsync(productId, updateProduct);
//            if (productUpdate == null)
//            {
//                return NotFound($"No product found with the id: {productId}.");
//            }
//            var productDTO = mapper.Map<ProductDTO>(productUpdate);
//            return Ok(productDTO);
//        }



//        [HttpPost("GetVendorProducts")]
//        public async Task<ActionResult<IEnumerable<ProductVendor>>> GetVendorProducts()
//        {
//            var userId = UserClaims.GetUserClaimID(HttpContext);
//            var products = await productRepository.GetByVendorAsync(userId);
//            if (products == null)
//            {
//                return NotFound($"No products found for the vendor with the id: {userId}");
//            }
//            var vendorProductsDTO = mapper.Map<List<ProductVendorDTO>>(products);
//            return Ok(vendorProductsDTO);
//        }

//    }
//}
