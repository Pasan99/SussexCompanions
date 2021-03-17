using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SussexCompanions.Models.ViewModels.Event
{
    public class BookEventViewModel
    {
        public UserEvent UserEvent { get; set; }
        public Models.User User { get; set; }
        public Models.Event Event { get; set; }

    }
}