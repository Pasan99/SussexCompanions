﻿using SussexCompanions.Infrastructure;
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

        public ActionResult UserPayments()
        {
            SussexDBEntities db = new SussexDBEntities();
            UserPaymentsViewModel viewModel = new UserPaymentsViewModel();
            viewModel.BillingHistories = db.BillingHistories.Where(w => !w.BillingHistoryIsPayed).ToList();
            return View(viewModel);
        }

        public ActionResult DemandLetter(int Id)
        {
            SussexDBEntities db = new SussexDBEntities();
            DemandLetterViewModel viewModel = new DemandLetterViewModel();
            viewModel.User = db.Users.Where(w => w.UserId == Id).FirstOrDefault();
            DateTime temp = DateTime.Now.AddMonths(-2);
            viewModel.BillingHistories = db.BillingHistories
                .Where(w => w.UserId == Id && w.BillingHistoryDate < temp)
                .ToList();
            foreach (var bill in viewModel.BillingHistories) {
                PaymentHelper.SendOverdueEmail(viewModel.User, bill);
            }
            return View(viewModel);
        }
    }
}