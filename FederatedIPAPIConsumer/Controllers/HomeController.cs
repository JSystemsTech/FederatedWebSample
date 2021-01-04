using FederatedIPAPIConsumer.Models;
using FederatedIPAuthenticationService.Attributes.Common;
using FederatedIPAuthenticationService.Extensions;
using System;
using System.Web.Mvc;

namespace FederatedIPAPIConsumer.Controllers
{
    public class HomeController : BaseController
    {

        
        public ActionResult Index()
        {
            return View(SiteMeta);
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
    }
}