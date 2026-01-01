using Microsoft.AspNetCore.Mvc;
using Stripe;
using StripeSaas.Data;
using StripeSaas.Models;

namespace StripeSaas.Controllers
{

    [ApiController]
    [Route("webhook/stripe")]
    public class WebhookController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _db;

        public WebhookController(IConfiguration config,ApplicationDbContext dbContext)
        {
            _config = config;
            _db = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> Stripe()
        {
            var json = await new StreamReader(Request.Body).ReadToEndAsync();
            var signature = Request.Headers["Stripe-Signature"];

            Event stripeEvent;

            try
            {
                stripeEvent = EventUtility.ConstructEvent(
                    json,
                    signature,
                    _config["Stripe:WebhookSecret"]
                );
            }
            catch
            {
                return BadRequest();
            }

            // 🔁 Prevent duplicate processing
            if (_db.ProcessedEvents.Any(e => e.EventId == stripeEvent.Id))
                return Ok();

            _db.ProcessedEvents.Add(new ProcessedEvent
            {
                EventId = stripeEvent.Id,
                CreatedAt = DateTime.UtcNow
            });

            // ✅ Subscription created
            if (stripeEvent.Type == "checkout.session.completed")
            {
                var session = stripeEvent.Data.Object as Stripe.Checkout.Session;

                if (!_db.Subscriptions.Any(s => s.StripeSubscriptionId == session.SubscriptionId))
                {
                    _db.Subscriptions.Add(new Models.Subscription
                    {
                        UserId = session.Metadata["UserId"],
                        StripeCustomerId = session.CustomerId,
                        StripeSubscriptionId = session.SubscriptionId,
                        Status = "active",
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            // ❌ Payment failed → grace period
            if (stripeEvent.Type == "invoice.payment_failed")
            {
                var invoice = stripeEvent.Data.Object as Stripe.Invoice;
                var subscriptionId = invoice.Lines.Data
    .FirstOrDefault()?.SubscriptionId;
                var subscription = _db.Subscriptions
                    .FirstOrDefault(s => s.StripeSubscriptionId == subscriptionId);

                if (subscription != null)
                    subscription.Status = "past_due";
            }

            // ✅ Payment recovered
            if (stripeEvent.Type == "invoice.payment_succeeded")
            {
                var invoice = stripeEvent.Data.Object as Stripe.Invoice;
                var subscriptionId = invoice.Lines.Data
   .FirstOrDefault()?.SubscriptionId;
                var subscription = _db.Subscriptions
                    .FirstOrDefault(s => s.StripeSubscriptionId == subscriptionId);

                if (subscription != null)
                    subscription.Status = "active";
            }

            // ❌ Subscription ended
            if (stripeEvent.Type == "customer.subscription.deleted")
            {
                var stripeSubscription = stripeEvent.Data.Object as Stripe.Subscription;

                var subscription = _db.Subscriptions
                    .FirstOrDefault(s => s.StripeSubscriptionId == stripeSubscription.Id);

                if (subscription != null)
                    subscription.Status = "inactive";
            }
            if (stripeEvent.Type == "customer.subscription.updated")
            {
                var stripeSubscription = stripeEvent.Data.Object as Stripe.Subscription;

                var subscription = _db.Subscriptions
                    .FirstOrDefault(s => s.StripeSubscriptionId == stripeSubscription.Id);

                if (subscription != null)
                {
                    if (stripeSubscription.CancelAtPeriodEnd)
                    {
                        subscription.Status = "canceling"; // optional
                    }
                    else if (stripeSubscription.Status == "active")
                    {
                        subscription.Status = "active";
                    }
                }
            }
            await _db.SaveChangesAsync();
            return Ok();
        }

    }
}
