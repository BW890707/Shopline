using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shopline.Controllers
{
    public class ReportController : Controller
    {
        private readonly ProductDetilRepository _productRepository;
        private readonly ShoppingCartRepository _shoppingCartRepository;
        public ReportController()
        {
            _productRepository = new ProductDetilRepository();
            _shoppingCartRepository = new ShoppingCartRepository();
        }
        // GET: Report

        public ActionResult Index()
        {
            ReportViewModel request = new ReportViewModel();
            request.SellerAccount = Session["username"].ToString();
            var response = _shoppingCartRepository.Report(request);

            return View(response);
        }
        public JsonResult GetTotalOrder(ReportViewModel select)
        {
            select.SellerAccount = Session["username"].ToString();
            var response = _shoppingCartRepository.TotalOrder(select);
            ReportData result = new ReportData();
            result.TotalCount = response;
            return Json(result);
        }
        public JsonResult GetTotalPrice(ReportViewModel select)
        {
            select.SellerAccount = Session["username"].ToString();
            var response = _shoppingCartRepository.TotalPrice(select);
            ReportData result = new ReportData();
            result.TotalPrice = response;
            return Json(result);
        }

    }
}