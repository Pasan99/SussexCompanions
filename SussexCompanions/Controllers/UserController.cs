using SussexCompanions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SussexCompanions.Controllers
{
    public class UserController : Controller
    {
         

        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Edit()
        {
            return View();
        }
        public ActionResult Register()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult MyAccount()
        {
             int userId = 5;


            SussexDBEntities db = new SussexDBEntities();
            List<Event> events = db.Events.ToList();
            List<User> users = db.Users.ToList();  
            List<UserEvent> userEvents = db.UserEvents.ToList();

            var multibleTable = from ue in userEvents
                                join ev in events on ue.EventId equals ev.EventId into table1
                                from ev in table1.DefaultIfEmpty()
                                join u in users on ue.UserId equals u.UserId into table2
                                from u in table2.DefaultIfEmpty()  
                                where ( u.UserId == userId  && ev.EventDate < DateTime.Now)
                                select new MultibleClass {Event =ev ,userEvent = ue, user = u };



            return View(multibleTable);
        }
    }
}