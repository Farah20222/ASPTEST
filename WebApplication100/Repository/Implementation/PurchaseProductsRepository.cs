using Microsoft.EntityFrameworkCore;
using WebApplication100.Models;
using WebApplication100.Repository.Interface;
using WebApplication100.Service;

namespace WebApplication100.Repository.Implementation
{
    public class PurchaseProductsRepository: IPurchaseProductsRepository
    {

        private readonly AssignmentDBContext assignmentDBContext;
        private readonly ITimeZoneService timeZoneService;

        public PurchaseProductsRepository(AssignmentDBContext assignmentDBContext, ITimeZoneService timeZoneService)
        {
            this.assignmentDBContext = assignmentDBContext;
            this.timeZoneService = timeZoneService;
        }
        public async Task<IEnumerable<Purchase>> GetByCustomer(int userId)
        {
            return await assignmentDBContext.Purchases.Where(x => x.UserProfileId == userId).ToListAsync();
        }
        public async Task<Purchase> AddPurchase(Purchase purchase)
        {
            purchase.PurchaseId = new int(); 
            await assignmentDBContext.AddAsync(purchase);
            await assignmentDBContext.SaveChangesAsync();
            return purchase; 
        }
    }
}
