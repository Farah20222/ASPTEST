namespace WebApplication100.DTO
{
    public class PurchaseDTO
    {
        public int PurchaseId { get; set; }
        public int? UserProfileId { get; set; }
        public int? ProductId { get; set; }
        public DateTime? PurchasedAt { get; set; }
        public decimal? PurchaseCost { get; set; }
    }


    public class NewPurchase
    {
        public int? ProductId { get; set; }

    }
}
