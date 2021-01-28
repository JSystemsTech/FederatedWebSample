using FederatedIPAPIConsumer.Models;
using ServiceLayer.AuthNAuthZ;
using System.Web.Mvc;

namespace FederatedIPAPIConsumer.Controllers
{
    public class HomeController : BaseController
    {
        [FederatedAuthorize(false)]
        public ActionResult PrivacyPolicy()
        {
            return View();
        }
        [FederatedAuthorize(false)]
        public ActionResult CookiePolicy()
        {
            return View();
        }

        public ActionResult Index()
        {
            return View(FederatedApplicationSettings);
        }
        public ActionResult About()
        {
            return View();
        }
        public ActionResult Contact()
        {
            return View();
        }

        [HttpGet]
        public PartialViewResult Register()
        {
            return PartialFormView("Register", new RegisterFormVM());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public PartialViewResult Register(RegisterFormVM vm)
        {
            return PartialFormView("Register", vm);
        }
        public ActionResult BootstrapComponents()
        {
            return View();
        }
    }
}