namespace StripeSaas.Models
{
    public class Subscription
    {
        public int Id { get; set; }
        public string UserId { get; set; }

        public string StripeCustomerId { get; set; }
        public string StripeSubscriptionId { get; set; }

        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
