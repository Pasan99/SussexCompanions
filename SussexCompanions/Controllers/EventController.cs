using SussexCompanions.Infrastructure;
using SussexCompanions.Models;
using SussexCompanions.Models.ViewModels.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SussexCompanions.Controllers
{
    public class EventController : Controller
    {
        // GET: Event
        public ActionResult Index()
        {
            using (SussexDBEntities db = new SussexDBEntities())
            {
                List<Event> events = db.Events.Where(w=> w.EventIsDeleted != true).ToList();
                return View(events);
            }
        }
        public ActionResult Bookings()
        {
            BookingViewModel viewModel = new BookingViewModel();
            SussexDBEntities db = new SussexDBEntities();
            viewModel.Events = db.Events.Where(w => w.EventIsDeleted != true).ToList();
            foreach(var eve in viewModel.Events)
            {
                foreach(var user in eve.UserEvents)
                {
                    User a = user.User;
                }
            }
            viewModel.Users = db.Users.Where(w => w.UserIsActivated).ToList();
            return View(viewModel);
        }
        public ActionResult Edit(int? Id)
        {
            EventEditViewModel viewModel = new EventEditViewModel();
            using (SussexDBEntities db = new SussexDBEntities())
            {
                viewModel.EventCategories = db.Categories.ToList();
                if (Id != null)
                {
                    viewModel.Event = db.Events.Where(w => w.EventId == Id).FirstOrDefault();
                    viewModel.MeetingSchedules = db.MeetingSchedules.Where(w => w.EventId == Id).ToList();
                    viewModel.SelectedEventCategories = db.EventCategories.Where(w => w.EventId == Id).Select(s => s.Category).ToList();
                }

            }
            return View(viewModel);
        }
        [HttpPost]
        public ActionResult Edit(EventEditViewModel viewModel)
        {
            using (SussexDBEntities db = new SussexDBEntities())
            {
                if (viewModel.Event.EventId == 0)
                {
                    db.Events.Add(viewModel.Event);
                }
                else
                {
                    Event eventToEdit = db.Events.Where(w => w.EventId == viewModel.Event.EventId).FirstOrDefault();
                    eventToEdit.EventDescription = viewModel.Event.EventDescription;
                    eventToEdit.EventTitle = viewModel.Event.EventTitle;
                    eventToEdit.EventFee = viewModel.Event.EventFee;
                    eventToEdit.EventDate = viewModel.Event.EventDate;
                    eventToEdit.EventContactNo = viewModel.Event.EventContactNo;
                    eventToEdit.EventEmail = viewModel.Event.EventEmail;
                    eventToEdit.EventRegistrationDeadline = viewModel.Event.EventRegistrationDeadline;
                }
                db.SaveChanges();

            }
            return Redirect("/Event/Edit/" + viewModel.Event.EventId);
        }

        public ActionResult AddCategoryToEvent(int EventId, int CategoryId)
        {
            using(SussexDBEntities db = new SussexDBEntities())
            {
                EventCategory eventCategory = new EventCategory();
                eventCategory.EventId = EventId;
                eventCategory.CategoryId = CategoryId;
                db.EventCategories.Add(eventCategory);
                db.SaveChanges();
            }
            return Redirect("/Event/Edit/" + EventId);
        }

        public ActionResult RemoveCategoryFromEvent(int EventId, int CategoryId)
        {
            using (SussexDBEntities db = new SussexDBEntities())
            {
                EventCategory eventCategory = db.EventCategories.Where(w => w.EventId == EventId && w.CategoryId == CategoryId).FirstOrDefault();
                db.EventCategories.Remove(eventCategory);
                db.SaveChanges();
            }
            return Redirect("/Event/Edit/" + EventId);
        }
        public ActionResult Delete(int Id)
        {
            using (SussexDBEntities db = new SussexDBEntities())
            {
                Event eventToDelete = db.Events.Where(w => w.EventId == Id).FirstOrDefault();
                eventToDelete.EventIsDeleted = true;
                db.SaveChanges();
            }
            return Redirect("/Event/");
        }
        public ActionResult AddMeetingToEvent(MeetingSchedule MeetingSchedule)
        {
            using(SussexDBEntities db = new SussexDBEntities())
            {
                db.MeetingSchedules.Add(MeetingSchedule);
                db.SaveChanges();
            }
            return Redirect("/Event/Edit/" + MeetingSchedule.EventId);
        }
        public ActionResult Book(int Id, int UserId)
        {
            //int userId = Int32.Parse(Session["UserId"].ToString());
            //int roleId = Int32.Parse(Session["RoleId"].ToString());
            int userId = 1;
            int roleId = 2;
            BookEventViewModel viewModel = new BookEventViewModel();
            using(SussexDBEntities db = new SussexDBEntities())
            {
                viewModel.Event = db.Events.Where(w => w.EventId == Id).FirstOrDefault();
                // book by user
                if (roleId == RoleTypes.CUSTOMER_ID)
                {
                    viewModel.User = db.Users.Where(w => w.UserId == userId).FirstOrDefault();
                }
                // book by Receptionist 
                else
                {
                    viewModel.User = db.Users.Where(w => w.UserId == UserId).FirstOrDefault();
                }
            }

            return View(viewModel);
        }

        public ActionResult ConfirmBooking(int UserId, int EventId, int By)
        {
            String message = "Booking Successfull";
            using(SussexDBEntities db = new SussexDBEntities())
            {
                UserEvent userEvent = db.UserEvents.Where(w => w.UserId == UserId && w.EventId == EventId).FirstOrDefault();
                if (userEvent != null)
                {
                    message = "User already has booking for this event";
                }
                else
                {
                    userEvent = new UserEvent();
                    userEvent.UserId = UserId;
                    userEvent.EventId = EventId;
                    userEvent.UserEventRegisteredDate = DateTime.Now;
                    db.UserEvents.Add(userEvent);
                    db.SaveChanges();
                }
            }
            return Redirect("/Event/BookSuccessfullMessage/" + By + "?Message=" + message + "&EventId=" + EventId + "&UserId=" + UserId);
        }

        public ActionResult BookSuccessfullMessage(int Id, int UserId, int EventId, string Message)
        {
            if (Id == RoleTypes.CUSTOMER_ID)
            {
                ViewBag.ReturnUrl = "/Home/Events";
            }
            else
            {
                ViewBag.ReturnUrl = "/Home/EventBookings";
            }
            using(SussexDBEntities db = new SussexDBEntities())
            {
                ViewBag.User = db.Users.Where(w => w.UserId == UserId).FirstOrDefault();
                ViewBag.Event = db.Events.Where(w => w.EventId == EventId).FirstOrDefault();
            }
            ViewBag.Message = Message;
            return View();
        }
    }
}