namespace WebApplication100.Models
{
    public class Purchase
    {

        public int PurchaseId { get; set; }
        public int? UserProfileId { get; set; }
        public int?  ProductId { get; set; }
        public DateTime? PurchasedAt { get; set; }

        public decimal? PurchaseCost { get; set; }

    }
}
