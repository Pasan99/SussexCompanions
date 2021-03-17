using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SussexCompanions.Models.ViewModels.Event
{
    public class ScheduleItem
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Description { get; set; }
        public string Event { get; set; }
        public List<string> Categories { get; set; }
        public int EventId { get; set; }
    }
}