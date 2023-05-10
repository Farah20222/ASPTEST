using WebApplication100.Models;

namespace WebApplication100.Repository.Interface
{
    public interface IPurchaseProductsRepository
    {
        Task<Purchase> AddPurchase(Purchase purchase);
        Task<IEnumerable<Purchase>> GetByCustomer(int userId); 
    }
}
