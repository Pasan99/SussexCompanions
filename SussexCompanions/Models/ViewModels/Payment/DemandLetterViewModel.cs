using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SussexCompanions.Models.ViewModels.Payment
{
    public class DemandLetterViewModel
    {
        public List<BillingHistory> BillingHistories { get; set; }
        public Models.User User { get; set; }
    }
}