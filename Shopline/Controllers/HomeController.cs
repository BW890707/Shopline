using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shopline.Controllers
{
    public class HomeController : Controller
    {
        private readonly ProductDetilRepository _loginRepository;
        public HomeController()
        {
            _loginRepository = new ProductDetilRepository();
        }
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
    }
}