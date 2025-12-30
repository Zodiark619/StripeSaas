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

            // ✅ Subscription created
            if (stripeEvent.Type == "checkout.session.completed")
            {
                var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
                var userId = session.Metadata["UserId"];
                var customerId = session.CustomerId;
                var subscriptionId = session.SubscriptionId;
                var subscription = new Models.Subscription
                {
                    UserId = userId,
                    StripeCustomerId = customerId,
                    StripeSubscriptionId = subscriptionId,
                    Status = "active",
                    CreatedAt = DateTime.UtcNow
                };

                _db.Subscriptions.Add(subscription);
                await _db.SaveChangesAsync();
                // 👉 Save to DB here
            }

            // ❌ Subscription canceled
            if (stripeEvent.Type == "customer.subscription.deleted")
            {
                var subscription = stripeEvent.Data.Object as Stripe.Subscription;
                // update DB status
            }

            return Ok();
        }
    }
    }
