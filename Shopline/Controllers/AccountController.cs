using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shopline.Controllers
{
    public class AccountController : Controller
    {
        private readonly LoginRepository _loginRepository;
        public AccountController()
        {
            _loginRepository = new LoginRepository();
        }
        // GET: Account
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult GetLogin(User model)
        {
            //成功=>Home/Index  
            if (ModelState.IsValid)
            {
                //判斷登入帳號

                User member = _loginRepository.GetLogin(model);

                if (member != null)
                {
                    //登入成功

                    //記起來
                    Session["username"] = model.UserName;
                    Session["memberId"] = member.Id;
                    Session["sellername"] = member.SellerName;

                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "帳號密碼錯誤" });
                }
            }
            return Json(new { success = false });
        }

        public ActionResult Logout(User model)
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }


    }
}