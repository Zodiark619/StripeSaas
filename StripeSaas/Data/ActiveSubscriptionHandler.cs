using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace StripeSaas.Data
{
    public class ActiveSubscriptionHandler
    : AuthorizationHandler<ActiveSubscriptionRequirement>
    {
        private readonly ApplicationDbContext _db;

        public ActiveSubscriptionHandler(ApplicationDbContext db)
        {
            _db = db;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ActiveSubscriptionRequirement requirement)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Task.CompletedTask;

            var hasActiveSubscription = _db.Subscriptions.Any(s =>
                s.UserId == userId && s.Status == "active");

            if (hasActiveSubscription)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
    }
