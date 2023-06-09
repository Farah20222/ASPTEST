﻿using AutoMapper;
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
        private readonly IProductsRepository productsRepository;
        private readonly IMapper mapper;
        private readonly ITimeZoneService timeZoneService;

        public ProductsController(IProductsRepository productsRepository, IMapper mapper, ITimeZoneService timeZoneService)
        {
            this.productsRepository = productsRepository;
            this.mapper = mapper;
            this.timeZoneService = timeZoneService;
        }


        [HttpGet("GetAllProducts")]
        public async Task<ActionResult<IEnumerable<Product>>> GetAllProducts()
        {
            var products = await productsRepository.GetAll();
            
            var productsDTO = mapper.Map<List<ProductDTO>>(products);

            return Ok(productsDTO); 
        }


        [HttpGet("GetProductInformation/{productId:int}")]
        [ActionName("GetProduct")]
        public async Task<ActionResult<Product>> GetProduct(int productId)
        {
            var product = await productsRepository.GetAsync(productId);
            if (product == null)
            {
                return NotFound("Product Not found");
            }

            var productDTO = mapper.Map<ProductDTO>(product);

            return Ok(productDTO);
        }


        [HttpPost("[action]")]
        [Authorize(Policy = "IsAdminOrVendor")]
        public async Task<IActionResult> AddProduct(AddProduct addProduct)
        {
            var userId = UserClaims.GetUserClaimID(HttpContext);

            var newProduct = new Product()
            {
                ProductName = addProduct.ProductName,
                CreatedBy = userId,
                Description = addProduct.Description,
                CreatedTime = timeZoneService.ChangeTimeZoneToRegional(DateTime.UtcNow),
                Price = addProduct.Price,

            };

            var vendorProduct = new ProductVendor()
            {
                UserProfileId = userId
            };
            newProduct = await productsRepository.AddAsync(newProduct, vendorProduct); 
            var productDTO = mapper.Map<ProductDTO>(newProduct);
            return CreatedAtAction(nameof(GetProduct), new { productId = productDTO.ProductId }, productDTO);

        }
        [HttpGet("GetVendorProducts")]
        public async Task<ActionResult<IEnumerable<ProductVendor>>> GetVendorProduct()
        {
            var userId = UserClaims.GetUserClaimID(HttpContext);
            var products = await productsRepository.GetByVendor(userId);
            if (products == null)
            {
                return NotFound("No products found with the given id: " + userId);
            }
            var productsDTO = mapper.Map<List<ProductVendorDTO>>(products);
            return Ok(productsDTO);
        }


        [HttpPut("UpdateProducts/{productId:int}")]
        [Authorize(Policy = "IsAdminOrVendor")]
        public async Task<ActionResult> UpdateProductsAsync([FromRoute] int productId, UpdateProduct updateProduct)
        {
            var userId = UserClaims.GetUserClaimID(HttpContext);

            var loggedInUserRole = User.FindFirstValue(ClaimTypes.Role);

            var vendorProducts = await productsRepository.IsVendorProduct(userId, productId);

            if (loggedInUserRole == "vendor" && vendorProducts == false)
            {
                return Unauthorized("User unauthorized");

            }

            var update = await productsRepository.UpdateAsync(productId, updateProduct);

            if (update == null)
            {
                return NotFound($"No products found with the productId : {productId}");
            }

            var productDTO = mapper.Map<ProductDTO>(update);
            return Ok(productDTO); 

        }


        [HttpDelete("DeleteProduct/{productId:int}")]
        [Authorize(Policy = "IsAdminOrVendor")]
        public async Task<IActionResult>DeleteProduct(int productId)
        {
            var product = await productsRepository.GetAsync(productId); 
            if(product== null)
            {
                return NotFound($"Product with the ID:{productId} is not found."); 
            }

            var userId = UserClaims.GetUserClaimID(HttpContext);

            var loggedInUserRole = User.FindFirstValue(ClaimTypes.Role);
            var vendorProduct = await productsRepository.IsVendorProduct(userId, productId);
            if (loggedInUserRole == "vendor" && vendorProduct == false)
            {
                return Unauthorized("User unauthorized");
            }

            var deleteProduct = await productsRepository.DeleteAsync(productId);


            var productDTO = mapper.Map<ProductDTO>(deleteProduct);

            return Ok(productDTO); 

        }


    }
}
