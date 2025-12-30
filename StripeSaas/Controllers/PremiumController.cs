using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StripeSaas.Controllers
{
    [Authorize(Policy = "ActiveSubscription")]  
    public class PremiumController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
