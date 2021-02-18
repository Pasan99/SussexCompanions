using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SussexCompanions.Models.ViewModels.Event
{
    public class BookingViewModel
    {
        public List<Models.Event> Events { get; set; }
        public List<User> Users { get; set; }
    }
}