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
                List<Event> events = db.Events.ToList();
                return View(events);
            }
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
                db.Events.Remove(eventToDelete);
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
    }
}