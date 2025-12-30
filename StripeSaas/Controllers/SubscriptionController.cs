using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace StripeSaas.Controllers
{
    [Authorize]
    public class SubscriptionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create()
        {
            var options = new SessionCreateOptions
            {
                Mode = "subscription",
                PaymentMethodTypes = new List<string> { "card" },
                CustomerEmail = User.Identity.Name,
                Metadata = new Dictionary<string, string>
                {
                    ["UserId"] = User.FindFirstValue(ClaimTypes.NameIdentifier)
                },
                LineItems = new List<SessionLineItemOptions>
            {
                new()
                {
                    Price = "price_1Sjw0e6xMtJuZDI42O98YBMM", // Stripe price ID
                    Quantity = 1
                }
            },
                SuccessUrl = "https://localhost:7134/subscription/success",
                CancelUrl = "https://localhost:7134/subscription/cancel"
            };

            var service = new SessionService();
            var session = service.Create(options);

            return Redirect(session.Url);
        }

        public IActionResult Success() => View();
        public IActionResult Cancel() => View();
    }
}
