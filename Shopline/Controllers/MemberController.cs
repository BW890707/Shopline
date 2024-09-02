using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shopline.Controllers
{
    public class MemberController : Controller
    {
        private readonly LoginRepository _db;
        public MemberController()
        {
            _db = new LoginRepository();
        }
        // GET: Member
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AJAX2(User member)
        {
            var isRepeat = _db.GetUserById(member.UserName);
            if (isRepeat)
            {
                _db.AddUser(member);
                var result = true;
                return Json(result);
            }
            else
            {
                var result = false;
                return Json(result);
            }
        }

        public ActionResult Edit()
        {
            var userName = Session["username"].ToString();
            var response = _db.EditFromLogin(userName);
            return View(response);
        }
        public ActionResult EditData(User data)
        {
            var response = _db.UpdateAccount(data);
            return Json(response);
        }

        public ActionResult DeleteAccount(string Id)
        {
            _db.DeleteAccount(Id);
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }
    }
}