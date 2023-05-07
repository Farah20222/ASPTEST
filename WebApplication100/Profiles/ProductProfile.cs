using AutoMapper;
using WebApplication100.DTO;
using WebApplication100.Models;
using static WebApplication100.DTO.UserDTO;

namespace WebApplication100.Profiles
{
    public class ProductProfile: Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductDTO>().ReverseMap();
            CreateMap<UserProfile, UserProfileDTO>().ReverseMap();
            CreateMap<ProductVendor, ProductVendorDTO>().ReverseMap();


        }
    }
}
