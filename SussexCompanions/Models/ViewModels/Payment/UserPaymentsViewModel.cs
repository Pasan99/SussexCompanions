using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SussexCompanions.Models.ViewModels.Payment
{
    public class UserPaymentsViewModel
    {
        public List<BillingHistory> BillingHistories { get; set; }
    }
}