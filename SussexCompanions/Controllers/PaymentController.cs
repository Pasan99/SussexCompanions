using SussexCompanions.Models;
using SussexCompanions.Models.ViewModels.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SussexCompanions.Controllers
{
    public class PaymentController : Controller
    {
        // GET: Payment
        [HttpPost]
        public ActionResult Edit(NewPaymentViewModel viewModel)
        {
            PaymentCard paymentCard = new PaymentCard();
            using(SussexDBEntities db = new SussexDBEntities())
            {
                if (viewModel.PaymentCardId != 0)
                {
                    paymentCard = db.PaymentCards.Where(w => w.PaymentCardId == viewModel.PaymentCardId).FirstOrDefault();
                }
                paymentCard.PaymentCardIsDefault = true;
                paymentCard.PaymentCardHolderName = viewModel.PaymentCardHolderName;
                paymentCard.PaymentCardNumber = viewModel.PaymentCardNumber;
                paymentCard.PaymentCardExpireDate = viewModel.PaymentCardExpireDate;
                viewModel.PaymentCardSecurityCode = viewModel.PaymentCardSecurityCode;
                viewModel.UserId = viewModel.UserId;

                if (viewModel.PaymentCardId != 0)
                {
                    db.PaymentCards.Add(paymentCard);
                }
                db.SaveChanges();
            }
            return Redirect(viewModel.ReturnUrl);
        }
    }
}