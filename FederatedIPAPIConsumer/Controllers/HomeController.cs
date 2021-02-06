using FederatedAuthNAuthZ.Attributes;
using FederatedIPAPIConsumer.Models;
using System.Web.Mvc;

namespace FederatedIPAPIConsumer.Controllers
{
    public class HomeController : BaseController
    {
        [FederatedApplication(false)]
        public ActionResult PrivacyPolicy()
        {
            return View();
        }
        [FederatedApplication(false)]
        public PartialViewResult PrivacyPolicyPartial()
        {
            return PartialView();
        }
        [FederatedApplication(false)]
        public ActionResult CookiePolicy()
        {
            return View();
        }
        [FederatedApplication(false)]
        public PartialViewResult CookiePolicyPartial()
        {
            return PartialView();
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