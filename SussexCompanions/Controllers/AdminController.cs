using SussexCompanions.Models;
using SussexCompanions.Models.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SussexCompanions.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
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
    }
}