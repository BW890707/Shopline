using Shopline.Models;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shopline.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductDetilRepository _productRepository;
        private readonly ShoppingCartRepository _shoppingCartRepository;
        public ProductController()
        {
            _productRepository = new ProductDetilRepository();
            _shoppingCartRepository = new ShoppingCartRepository();
        }
        public ActionResult Index(Product viewModel)
        {
            var response = _productRepository.GetProducts(viewModel);
            response.Product = response.Product.Skip((viewModel.page - 1) * 6).Take(6).ToList();
            response.Page = viewModel.page;
            return View("Index", response);
        }

        public ActionResult Detail(string Id)
        {
            var response = _productRepository.GetProductDetail(Id);
            return View("Detail", response);
        }
        public ActionResult Detail2()
        {
            return View();
        }

        public ActionResult Detail3()
        {
            return View();
        }

        public ActionResult Create()
        {
            CreateProduct response = new CreateProduct();
            return View(response);

        }
        public ActionResult CreateData(CreateProduct create)
        {
            create.Id = Guid.NewGuid().ToString();
            create.SellerName = Session["username"].ToString();
            if (!string.IsNullOrEmpty(create.Image))
            {
                var imgArray = create.Image.Split('\\').Length;
                var img = create.Image.Split('\\')[imgArray - 1];
                create.Image = "/img/" + img;
            }
            var response = _productRepository.CreateProduct(create);
            return Json(response);
        }

        public ActionResult Edit()
        {
            string userName = Session["username"].ToString();
            var response = _productRepository.EditFromProducts(null, userName);
            ProductResult viewResult = new ProductResult();
            viewResult.Product = response;
            return View(viewResult);
        }
        public ActionResult Edit2(string id)
        {
            var response = _productRepository.EditFromProducts(id);
            ProductResult viewResult = new ProductResult();
            viewResult.Product = response;
            return View(viewResult);
        }
        public ActionResult EditData(EditProduct data)
        {
            if (!string.IsNullOrEmpty(data.Image))
            {
                var imgArray = data.Image.Split('\\').Length;
                if (imgArray > 1)
                {
                    var img = data.Image.Split('\\')[imgArray - 1];
                    data.Image = "/img/" + img;
                }
            }
            var response = _productRepository.UpdateProduct(data);
            return Json(response);
        }
        public ActionResult Delete(string Id)
        {
            var response = _productRepository.DeleteProduct(Id);
            return RedirectToAction("Edit", "Product");
        }

        public ActionResult ShoppingCartList()
        {
            if (Session["username"] == null)
            {
                List<ShoppingCart> nullResult = new List<ShoppingCart>();
                return View(nullResult);
            }
            var userName = Session["username"].ToString();
            var response = _shoppingCartRepository.GetShoppingCart(userName);
            if (response == null)
            {
                List<ShoppingCart> nullResult = new List<ShoppingCart>();
                return View(nullResult);
            }
            return View(response);
        }
        public ActionResult CreateShoppingCart(ShoppingCart create)
        {
            create.Count = 1;
            create.BuyerName = Session["username"].ToString();
            create.Status = "未結帳";
            var response = _shoppingCartRepository.CreateShoppingCart(create);
            return Json(response);
        }

        /// <summary>
        /// 確認訂單
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public ActionResult CheckOrder(List<ShoppingCart> items)
        {
            var result = _shoppingCartRepository.InsertOrder(items);
            return RedirectToAction("ShoppingCartList", "Product");
        }

        public ActionResult DeleteShoppingCart(string id)
        {
            var response = _shoppingCartRepository.DeleteShoppingCart(id);
            return RedirectToAction("ShoppingCartList", "Product");
        }
        //ADD 20240712
        public ActionResult KeyWordSearch(Product KeyWord)
        {
            var response = _productRepository.KeyWordSearch(KeyWord);
            return View("Index", response);
        }
    }
}