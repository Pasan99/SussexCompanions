using SussexCompanions.Models;
using SussexCompanions.Models.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace SussexCompanions.Infrastructure
{
    public class PaymentHelper
    {
        public static void StartMonthlySubscription(int UserId)
        {
            using (SussexDBEntities db = new SussexDBEntities())
            {
                var user = db.Users.Where(w => w.UserId == UserId).FirstOrDefault();
                // check whether the subscription is exist
                var subscription = db.BillingHistories.Where(w => w.UserId == UserId).FirstOrDefault();
                if (subscription != null || user == null)
                {
                    return;
                }

                BillingHistory billing = new BillingHistory();
                billing.BillingHistoryDate = DateTime.Now.AddMonths(1);
                billing.BillingHistoryIsPayed = false;
                billing.BillingHistoryIsOverdue = false;
                billing.UserId = UserId;

                // Currency is in Euros
                if (user.UserType == UserTypes.LOCAL)
                {
                    billing.BillingHistoryAmount = 12;
                }
                else
                {
                    billing.BillingHistoryAmount = 5;
                }
                db.BillingHistories.Add(billing);
                db.SaveChanges();
            }
        }
        public static void UpdateMonthlySubscription(int UserId)
        {
            using (SussexDBEntities db = new SussexDBEntities())
            {
                int monthlyFee = 0;
                var user = db.Users.Where(w => w.UserId == UserId).FirstOrDefault();

                if (user == null)
                {
                    return;
                }

                // Currency is in Euros
                if (user.UserType == UserTypes.LOCAL)
                {
                    monthlyFee = 12;
                }
                else
                {
                    monthlyFee = 5;
                }

                // check whether the subscription is exist
                var subscription = db.BillingHistories
                    .Where(w => w.UserId == UserId && w.BillingHistoryAmount == monthlyFee)
                    .OrderByDescending(o=> o.BillingHistoryDate)
                    .FirstOrDefault();

                DateTime nextPaymentDate;
                if (subscription != null)
                {
                    // if the next billing date is not met
                    if (subscription.BillingHistoryDate > DateTime.Now)
                    {
                        return;
                    }
                    // if payment is not recieved for last month
                    if (!subscription.BillingHistoryIsPayed && subscription.BillingHistoryDate.AddMonths(1) < DateTime.Now)
                    {
                        subscription.BillingHistoryIsOverdue = true;
                        db.SaveChanges();
                        SendOverdueEmail(user, subscription);
                    }

                    nextPaymentDate = subscription.BillingHistoryDate.AddMonths(1);

                    BillingHistory billing = new BillingHistory();
                    billing.BillingHistoryDate = nextPaymentDate;
                    billing.BillingHistoryAmount = monthlyFee;
                    billing.BillingHistoryIsPayed = false;
                    billing.BillingHistoryIsOverdue = false;
                    billing.UserId = UserId;

                    db.BillingHistories.Add(billing);
                    db.SaveChanges();
                }
            }
        }
        public static void AddEventPaymentForUser(int UserId, int EventId, bool IsPayed = true)
        {
            using(SussexDBEntities db = new SussexDBEntities())
            {
                var user = db.Users.Where(w => w.UserId == UserId).FirstOrDefault();
                var eventDetails = db.Events.Where(w => w.EventId == EventId).FirstOrDefault();
                BillingHistory billing = new BillingHistory();
                billing.BillingHistoryDate = DateTime.Now.AddMonths(1);
                billing.BillingHistoryIsPayed = IsPayed;
                billing.BillingHistoryIsOverdue = false;
                billing.BillingHistoryAmount = eventDetails.EventFee;
                billing.UserId = UserId;

                db.BillingHistories.Add(billing);
                db.SaveChanges();
                SendEventRegistrationEmail(user, eventDetails, IsPayed);
            }
        }
        public static void SendOverdueEmail(User user, BillingHistory billing)
        {
            SmtpClient client = new SmtpClient();
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            client.Host = "smtp.gmail.com";
            client.Port = 587;

            // setup Smtp authentication
            System.Net.NetworkCredential credentials =
                new System.Net.NetworkCredential("arrowprivatelimited@gmail.com", "Arrow1234#");
            client.UseDefaultCredentials = false;
            client.Credentials = credentials;

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("arrowprivatelimited@gmail.com");
            msg.To.Add(new MailAddress(user.UserEmail));

            msg.Subject = "Payment Overdue Email";
            msg.IsBodyHtml = true;
            msg.Body = string.Format("<html><head></head><body>" +
                "<b>Dear " + user.UserFirstName + ",</b>" +
                "<br><p>You have delayed your payment for " + billing.BillingHistoryDate.ToString("MMM") + " (Month). Please make your payments by loging to the system.</p>" +
                "<b>Last Deadline : " + billing.BillingHistoryDate +"</b>" + "" +
                "<hr>Thank You" + 
                "</body>"); ;

            try
            {
                client.Send(msg);
            }
            catch (Exception)
            {
            }
        }
        public static void SendEventRegistrationEmail(User user, Event eventDetails, bool IsPayed)
        {
            SmtpClient client = new SmtpClient();
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            client.Host = "smtp.gmail.com";
            client.Port = 587;

            // setup Smtp authentication
            System.Net.NetworkCredential credentials =
                new System.Net.NetworkCredential("arrowprivatelimited@gmail.com", "Arrow1234#");
            client.UseDefaultCredentials = false;
            client.Credentials = credentials;

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("arrowprivatelimited@gmail.com");
            msg.To.Add(new MailAddress(user.UserEmail));

            msg.Subject = "Payment Overdue Email";
            msg.IsBodyHtml = true;
            msg.Body = string.Format("<html><head></head><body>" +
                "<b>Dear " + user.UserFirstName + ",</b>" +
                "<br><p>You have successfully registered for the " + eventDetails.EventTitle + ".</p>" +
                "<b>Start Date : " + eventDetails.EventDate + "</b>" + "<br>" +
                "<b>Fees : £ " + eventDetails.EventFee.ToString(".00") + "</b>" + "<br>" +
                "<b>Recieved Payment : £ " + (IsPayed ? eventDetails.EventFee.ToString(".00") : "") + "</b>" + "" +
                "<hr>Thank You" +
                "</body>"); ;

            try
            {
                client.Send(msg);
            }
            catch (Exception)
            {
            }
        }
    }
}