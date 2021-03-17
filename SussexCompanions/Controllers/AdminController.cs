using SussexCompanions.Infrastructure;
using SussexCompanions.Models;
using SussexCompanions.Models.ViewModels.Common;
using SussexCompanions.Models.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;

namespace SussexCompanions.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        [CustomAuthorize("Admin", "Customer", "Receptionist", "Client Service Agent")]
        public ActionResult Users()
        {
            SussexDBEntities db = new SussexDBEntities();
            List<User> users = db.Users.ToList();
            return View(users);
        }

        public ActionResult EditUser(int Id = 0)
        {
            EditUserViewModel viewModel = new EditUserViewModel();

            if (Id != 0)
            {
                using(SussexDBEntities db = new SussexDBEntities())
                {
                    viewModel.User = db.Users.Where(w => w.UserId == Id).FirstOrDefault();
                    if (User != null)
                    {
                        viewModel.UserDetail = db.UserDetails.Where(w => w.UserId == viewModel.User.UserId).FirstOrDefault();
                    }
                }
            }
            return View(viewModel);
        }
        [HttpGet]
        public ActionResult Login()
        {

            return View();
        }
        //Login POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserLogin login, string ReturnUrl = "/")
        {
            string message = "";
            if (ReturnUrl != "/")
            {
                string[] result = ReturnUrl.Split('/');
                ReturnUrl = "/";
                foreach (string a in result)
                {
                    if (a != result[0] && a != result[1] && a != result[2])
                    {
                        ReturnUrl += (a + "/");
                    }
                }
            }
            using (SussexDBEntities dc = new SussexDBEntities())
            {
                var v = dc.Users.Where(a => a.UserEmail == login.UserEmail).FirstOrDefault();
                if (v != null)
                {
                    if (!v.UserIsActivated)
                    {
                        message = "Invalid credential provided";
                        return View();
                    }

                    if (string.Compare(Crypto.Hash(login.UserPassword), v.UserPassword) == 0)
                    {
                        //int timeout = login.RememberMe ? 525600 : 20; // 525600 min = 1 year
                        //var ticket = new FormsAuthenticationTicket(login.UserEmail, login.RememberMe, timeout);
                        //string encrypted = FormsAuthentication.Encrypt(ticket);
                        //var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                        //cookie.Expires = DateTime.Now.AddMinutes(timeout);
                        //cookie.HttpOnly = true;
                        //Response.Cookies.Add(cookie);

                        Session["UserId"] = v.UserId;
                        Session["UserName"] = v.UserFirstName + " " + v.UserLastName;
                        Session["UserRole"] = v.Role.RoleDescription;
                        Session["RoleId"] = v.Role.RoleId;

                        if (Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Admin");
                        }
                    }
                    else
                    {
                        message = "Invalid credential provided";
                    }
                }
                else
                {
                    message = "Invalid credential provided";
                }
            }
            ViewBag.Message = message;
            return View();
        }
        public ActionResult HobbiesAndPersonalities(int Id)
        {
            SussexDBEntities db = new SussexDBEntities();
            MyProfileViewModel viewModel = new MyProfileViewModel();
            viewModel.User = db.Users.Where(w => w.UserId == Id).FirstOrDefault();
            return View(viewModel);
        }
        //Logout
        public ActionResult Logout()
        {
            Session["UserName"] = string.Empty;
            Session["UserId"] = string.Empty;
            Session["UserRole"] = string.Empty;
            Session["RoleId"] = string.Empty;
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "User");
        }
        [CustomAuthorize("Admin", "Client Service Agent")]
        public ActionResult SuccessfullMatches()
        {
            return View();
        }
        [CustomAuthorize("Admin", "Client Service Agent")]
        public ActionResult UnsuccessfullMatches()
        {
            return View();
        }
    }
}