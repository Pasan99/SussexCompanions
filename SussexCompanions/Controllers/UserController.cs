using SussexCompanions.Infrastructure;
using SussexCompanions.Models;
using SussexCompanions.Models.Constants;
using SussexCompanions.Models.ViewModels.Payment;
using SussexCompanions.Models.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;

namespace SussexCompanions.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        //[CustomAuthorize("Admin", "Customer", "Receptionist", "Client Service Agent")]
        public ActionResult EditUser(int? Id)
        {
            UserEditViewModel viewModel = new UserEditViewModel();
            int roleId = 0;
            if (Session["RoleId"] != null && Session["RoleId"].ToString() != "")
            {
                roleId = Int32.Parse(Session["RoleId"].ToString());
            }
            if (roleId == RoleTypes.CUSTOMER_ID)
            {
                viewModel.UserType = UserTypes.ONLINE;
                if (Id == null)
                {
                    return Redirect("/");
                }
            }
            else
            {
                viewModel.UserType = UserTypes.LOCAL;
            }
            if (roleId == 0)
            {
                viewModel.UserType = UserTypes.ONLINE;
            }

            if (Id == null)
            {
                return View(viewModel);
            }
            using (SussexDBEntities db = new SussexDBEntities())
            {
                User user = db.Users.Where(w => w.UserId == Id).FirstOrDefault();
                if (user == null)
                {
                    return View(viewModel);
                }
                UserDetail userDetail = db.UserDetails.Where(w => w.UserId == Id).FirstOrDefault();
                if (userDetail != null)
                {
                    viewModel.Bio = userDetail.UserDetailBio;
                    viewModel.Profession = userDetail.UserDetailProfession;
                    viewModel.Age = userDetail.UserDetailAge;
                    viewModel.Gender = userDetail.UserDetailGender;
                    viewModel.UserDetailId = userDetail.UserDetailId;
                }
                viewModel.FirstName = user.UserFirstName;
                viewModel.LastName = user.UserLastName;
                viewModel.Email = user.UserEmail;
                viewModel.Password = user.UserPassword;
                viewModel.ContactNo = user.UserContactNo;
                viewModel.UserType = user.UserType;
                viewModel.UserId = user.UserId;

            }
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[CustomAuthorize("Admin", "Customer", "Receptionist", "Client Service Agent")]
        public ActionResult EditUser(UserEditViewModel viewModel)
        {
            int roleId = 0;
            if (Session["RoleId"] != null && Session["RoleId"].ToString() != "")
            {
                roleId = Int32.Parse(Session["RoleId"].ToString());
            }
            SussexDBEntities db = new SussexDBEntities();
            User user = null;
            if (viewModel.UserId != 0)
            {
                user = db.Users.Where(w => w.UserId == viewModel.UserId).FirstOrDefault();
            }
            // new user
            if (user == null)
            {
                user = new User();
                user.UserType = viewModel.UserType;
                user.UserJoinedDate = DateTime.Now;
                user.UserIsActivated = true;
                user.UserEmail = viewModel.Email;
            }
            
            if (user.RoleId == roleId)
            {
                user.RoleId = roleId;
            }
            else
            {
                user.RoleId = RoleTypes.CUSTOMER_ID;
            }
            if (roleId == 0)
            {
                user.RoleId = RoleTypes.CUSTOMER_ID;
            }
            user.UserFirstName = viewModel.FirstName;
            user.UserLastName = viewModel.LastName;
            user.UserContactNo = viewModel.ContactNo;
            if (user.UserId == 0)
            {
                user.UserPassword = Crypto.Hash(viewModel.Password);
                db.Users.Add(user);
            }
            db.SaveChanges();
            PaymentHelper.StartMonthlySubscription(user.UserId);

            UserDetail userDetail = null;
            userDetail = db.UserDetails.Where(w => w.UserDetailId == viewModel.UserDetailId).FirstOrDefault();

            if (userDetail == null)
            {
                userDetail = new UserDetail();
            }
            userDetail.UserDetailBio = viewModel.Bio;
            userDetail.UserDetailProfession = viewModel.Profession;
            userDetail.UserDetailAge = viewModel.Age;
            userDetail.UserDetailGender = viewModel.Gender;
            userDetail.UserId = user.UserId;

            if (userDetail.UserDetailId == 0)
            {
                db.UserDetails.Add(userDetail);
            }
            db.SaveChanges();
            if (roleId == RoleTypes.CUSTOMER_ID)
            {
                return Redirect("/User/MyProfile");
            }
            return Redirect("/");
        }
        [HttpGet]
        public ActionResult Login()
        {

            return View();
        }
        //Login POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserLogin login, string ReturnUrl = "/")
        {
            string message = "";
            if (ReturnUrl != "/")
            {
                string[] result = ReturnUrl.Split('/');
                ReturnUrl = "/";
                foreach(string a in result)
                {
                    if (a != result[0] && a != result[1] && a != result[2])
                    {
                        ReturnUrl += (a + "/");
                    }
                }
            }
            using (SussexDBEntities dc = new SussexDBEntities())
            {
                var v = dc.Users.Where(a => a.UserEmail == login.UserEmail).FirstOrDefault();
                if (v != null)
                {
                    if (!v.UserIsActivated)
                    {
                        message = "Invalid credential provided";
                        return View();
                    }

                    if (string.Compare(Crypto.Hash(login.UserPassword), v.UserPassword) == 0)
                    {
                        //int timeout = login.RememberMe ? 525600 : 20; // 525600 min = 1 year
                        //var ticket = new FormsAuthenticationTicket(login.UserEmail, login.RememberMe, timeout);
                        //string encrypted = FormsAuthentication.Encrypt(ticket);
                        //var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                        //cookie.Expires = DateTime.Now.AddMinutes(timeout);
                        //cookie.HttpOnly = true;
                        //Response.Cookies.Add(cookie);

                        Session["UserId"] = v.UserId;
                        Session["UserName"] = v.UserFirstName + " " + v.UserLastName;
                        Session["UserRole"] = v.Role.RoleDescription;
                        Session["RoleId"] = v.Role.RoleId;

                        if (Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        message = "Invalid credential provided";
                    }
                }
                else
                {
                    message = "Invalid credential provided";
                }
            }
            ViewBag.Message = message;
            return View();
        }
        //Logout
        public ActionResult Logout()
        {
            Session["UserName"] = string.Empty;
            Session["UserId"] = string.Empty;
            Session["UserRole"] = string.Empty;
            Session["RoleId"] = string.Empty;
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "User");
        }
        [CustomAuthorize("Admin", "Customer")]
        public ActionResult MyProfile()
        {
            int userId = Int32.Parse(Session["UserId"].ToString());

            SussexDBEntities db = new SussexDBEntities();
            MyProfileViewModel viewModel = new MyProfileViewModel();
            viewModel.User = db.Users.Where(w => w.UserId == userId).FirstOrDefault();
            return View(viewModel);
        }
        [CustomAuthorize("Admin", "Customer")]
        public ActionResult MyFriends()
        {
            int userId = Int32.Parse(Session["UserId"].ToString());

            SussexDBEntities db = new SussexDBEntities();
            var user = db.Users.Where(w => w.UserId == userId).FirstOrDefault();

            MyFriendsViewModel viewModel = new MyFriendsViewModel();
            viewModel.Suggetions = new List<User>();

            // if user has hobbies find user's who has similar hobbies
            if (user.UserHobbies != null && user.UserHobbies.Count > 0)
            {
                // for each hobby add people who have that hobby
                foreach (var hobby in user.UserHobbies)
                {
                    viewModel.Suggetions.AddRange(db.Users
                        .Where(w => (
                            w.UserHobbies.Where(w2 => w2.HobbyId == hobby.HobbyId).ToList().Count) > 0)
                        .ToList());
                }
            }

            // if user has personalities find user's who has similar personalities
            if (user.UserPersonalities != null && user.UserPersonalities.Count > 0)
            {
                // for each personality add people who have that personality
                foreach (var personality in user.UserPersonalities)
                {
                    viewModel.Suggetions.AddRange(db.Users
                        .Where(w => (
                            w.UserPersonalities.Where(w2 => w2.PersonalityId == personality.PersonalityId).ToList().Count) > 0)
                        .ToList());
                }
            }

            viewModel.Friends = GetFriendsByUser(userId);
            viewModel.OnGoingRequests = GetFriendRequestsByUser(userId);

            // find friend requests
            viewModel.FriendRequests = db.UserMatches
                .Where(w => (w.UserMatchRecievedUserId == userId) && !w.UserMatchIsAccepted).ToList();

            var suggetionsRemoveDuplicates = viewModel.Suggetions.Distinct().ToList();

            // remove my account from suggetions
            if (suggetionsRemoveDuplicates != null)
            {
                var meInList = suggetionsRemoveDuplicates
                    .Where(w => w.UserId == userId).FirstOrDefault();
                suggetionsRemoveDuplicates.Remove(meInList);
            }
            // remove my friends from suggetions
            if (suggetionsRemoveDuplicates != null)
            {
                foreach (var f in viewModel.Friends)
                {
                    var friend = suggetionsRemoveDuplicates
                        .Where(w => w.UserId == f.UserId)
                        .FirstOrDefault();
                    suggetionsRemoveDuplicates.Remove(friend);
                }
            }


            viewModel.Suggetions = suggetionsRemoveDuplicates;

            return View(viewModel);
        }
        [CustomAuthorize("Admin", "Customer")]
        public ActionResult AddFriend(int Id)
        {
            int userId = Int32.Parse(Session["UserId"].ToString());
            using (SussexDBEntities db = new SussexDBEntities())
            {
                UserMatch match = new UserMatch();
                match.UserMatchRequestUserId = userId;
                match.UserMatchRecievedUserId = Id;
                match.UserMatchDate = DateTime.Now;
                match.UserMatchIsAccepted = false;
                match.UserMatchIsDeleted = false;

                db.UserMatches.Add(match);
                db.SaveChanges();
            }
            return Redirect("/User/MyFriends");
        }
        [CustomAuthorize("Admin", "Customer")]
        public ActionResult AcceptRequest(int Id)
        {
            int userId = Int32.Parse(Session["UserId"].ToString());
            using (SussexDBEntities db = new SussexDBEntities())
            {
                UserMatch match = db.UserMatches
                    .Where(w => w.UserMatchRequestUserId == Id
                        && w.UserMatchRecievedUserId == userId).FirstOrDefault();
                match.UserMatchIsAccepted = true;
                db.SaveChanges();
            }
            return Redirect("/User/MyFriends");
        }
        [CustomAuthorize("Admin", "Customer")]
        public ActionResult AddHobby(int Id, int? UserId, bool IsDelete = false)
        {
            if (UserId == null)
            {
                UserId = Int32.Parse(Session["UserId"].ToString());
            }
            int roleId = Int32.Parse(Session["RoleId"].ToString());
            using (SussexDBEntities db = new SussexDBEntities())
            {
                if (IsDelete)
                {
                    UserHobby userHobby = db.UserHobbies.Where(w => w.UserId == UserId && w.HobbyId == Id).FirstOrDefault();
                    db.UserHobbies.Remove(userHobby);
                    db.SaveChanges();
                }
                else
                {
                    UserHobby hobby = new UserHobby();
                    hobby.UserId = (int) UserId;
                    hobby.HobbyId = Id;
                    db.UserHobbies.Add(hobby);
                    db.SaveChanges();
                }

            }
            if (roleId == RoleTypes.CUSTOMER_ID)
            {
                return Redirect("/User/MyProfile");
            }
            return Redirect("/Admin/HobbiesAndPersonalities/" + UserId);
        }
        [CustomAuthorize("Admin", "Customer")]
        public ActionResult AddPersonality(int Id, int? UserId, bool IsDelete = false)
        {
            if (UserId == null) {
                UserId = Int32.Parse(Session["UserId"].ToString());
            }
            int roleId = Int32.Parse(Session["RoleId"].ToString());
            using (SussexDBEntities db = new SussexDBEntities())
            {
                if (IsDelete)
                {
                    UserPersonality userPersonality = db.UserPersonalities.Where(w => w.UserId == UserId && w.PersonalityId == Id).FirstOrDefault();
                    db.UserPersonalities.Remove(userPersonality);
                    db.SaveChanges();
                }
                else
                {
                    UserPersonality personality = new UserPersonality();
                    personality.UserId = (int) UserId;
                    personality.PersonalityId = Id;
                    db.UserPersonalities.Add(personality);
                    db.SaveChanges();
                }

            }
            if (roleId == RoleTypes.CUSTOMER_ID)
            {
                return Redirect("/User/MyProfile");
            }
            return Redirect("/Admin/HobbiesAndPersonalities/" + UserId);
        }
        [CustomAuthorize("Admin", "Customer")]
        public ActionResult Subscription()
        {
            int userId = Int32.Parse(Session["UserId"].ToString());
            SussexDBEntities db = new SussexDBEntities();
            UserPaymentsViewModel viewModel = new UserPaymentsViewModel();
            viewModel.BillingHistories = db.BillingHistories.Where(w => w.UserId == userId).ToList();
            return View(viewModel);
        }
        [CustomAuthorize("Admin", "Customer", "Finance Manager")]
        public ActionResult Payment(int Id)
        {
            SussexDBEntities db = new SussexDBEntities();
            BillingHistory billing = db.BillingHistories.Where(w => w.BillingHistoryId == Id).FirstOrDefault();
            return View(billing);
        }

        [CustomAuthorize("Admin", "Customer", "Finance Manager")]
        public ActionResult ConfirmPayment(int Id)
        {
            int roleId = Int32.Parse(Session["RoleId"].ToString());
            SussexDBEntities db = new SussexDBEntities();
            BillingHistory billing = db.BillingHistories.Where(w => w.BillingHistoryId == Id).FirstOrDefault();

            if (billing != null)
            {
                billing.BillingHistoryIsPayed = true;
                db.SaveChanges();
            }

            if (roleId == RoleTypes.CUSTOMER_ID)
            {
                return Redirect("/User/Subscription");
            }
            return Redirect("/Payment/UserPayments");
        }

        private List<User> GetFriendsByUser(int userId)
        {
            SussexDBEntities db = new SussexDBEntities();
            // find friends
            var Friends = new List<User>();
            var userFriendMatches = db.UserMatches
                .Where(w => (w.UserMatchRequestUserId == userId || w.UserMatchRecievedUserId == userId) && w.UserMatchIsAccepted)
                .ToList();
            List<int> userFriendMatchIds = new List<int>();
            foreach (var item in userFriendMatches)
            {
                if (item.UserMatchRequestUserId == userId)
                {
                    userFriendMatchIds.Add(item.UserMatchRecievedUserId);
                }
                else
                {
                    userFriendMatchIds.Add(item.UserMatchRequestUserId);
                }
            }

            // remove duplicates
            var friendsRemoveDuplicateIds = userFriendMatchIds.Distinct().ToList();
            // add
            if (friendsRemoveDuplicateIds != null)
            {
                foreach (int id in friendsRemoveDuplicateIds)
                {
                    Friends.Add(db.Users.Where(w => w.UserId == id).FirstOrDefault());
                }
            }
            return Friends;
        }
        private List<User> GetFriendRequestsByUser(int userId)
        {
            SussexDBEntities db = new SussexDBEntities();
            // find friends
            var Friends = new List<User>();
            var userFriendMatches = db.UserMatches
                .Where(w => (w.UserMatchRequestUserId == userId || w.UserMatchRecievedUserId == userId) && !w.UserMatchIsAccepted)
                .ToList();
            List<int> userFriendMatchIds = new List<int>();
            foreach (var item in userFriendMatches)
            {
                if (item.UserMatchRequestUserId == userId)
                {
                    userFriendMatchIds.Add(item.UserMatchRecievedUserId);
                }
                else
                {
                    userFriendMatchIds.Add(item.UserMatchRequestUserId);
                }
            }

            // remove duplicates
            var friendsRemoveDuplicateIds = userFriendMatchIds.Distinct().ToList();
            // add
            if (friendsRemoveDuplicateIds != null)
            {
                foreach (int id in friendsRemoveDuplicateIds)
                {
                    Friends.Add(db.Users.Where(w => w.UserId == id).FirstOrDefault());
                }
            }
            return Friends;
        }
    }
}