using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SussexCompanions.Models.ViewModels.Common
{
    public class EditUserViewModel
    {
        public Models.User User { get; set; }
        public UserDetail UserDetail { get; set; }
    }
}