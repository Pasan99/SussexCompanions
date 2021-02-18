using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SussexCompanions.Models.ViewModels.Payment
{
    public class NewPaymentViewModel : PaymentCard
    {
        public string ReturnUrl { get; set; }
    }
}