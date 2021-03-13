using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SussexCompanions.Models;

namespace SussexCompanions.Models.ViewModels.Event
{
    public class EventEditViewModel
    {
        public Models.Event Event { get; set; }
        public List<MeetingSchedule> MeetingSchedules { get; set; }
        public List<Category> EventCategories { get; set; }
        public List<Category> SelectedEventCategories { get; set; }

    }
}