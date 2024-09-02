using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shopline.Controllers
{
    public class OrderController : Controller
    {
        private readonly ShoppingCartRepository _shoppingCartRepository;
        public OrderController()
        {
            _shoppingCartRepository = new ShoppingCartRepository();
        }
        // GET: Order
        public ActionResult OrderIndex()
        {
            string userName = Session["username"].ToString();
            var result = _shoppingCartRepository.GetOrder(userName);
            return View(result);
        }

        public ActionResult OrderDetail()
        {
            return View();
        }

        public ActionResult OrderEdit(ShoppingCart edit)
        {
            var result = _shoppingCartRepository.UpdateShoppingCart(edit);
            return Json(new { success = result });
        }

        public ActionResult OrderCancel(ShoppingCart edit)
        {
            var result = _shoppingCartRepository.CancelShoppingCart(edit.ID);
            return Json(new { success = result });
        }
    }
}