using SussexCompanions.Models;
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

        public ActionResult Edit()
        {
            return View();
        }
        public ActionResult Register()
        {
            return View();
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
            using (SussexDBEntities dc = new SussexDBEntities())
            {
                var v = dc.Users.Where(a => a.UserEmail == login.UserEmail).FirstOrDefault();
                if (v != null)
                {

                    if (string.Compare(Crypto.Hash(login.UserPassword), v.UserPassword) == 0)
                    {
                        int timeout = login.RememberMe ? 525600 : 20; // 525600 min = 1 year
                        var ticket = new FormsAuthenticationTicket(login.UserEmail, login.RememberMe, timeout);
                        string encrypted = FormsAuthentication.Encrypt(ticket);
                        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                        cookie.Expires = DateTime.Now.AddMinutes(timeout);
                        cookie.HttpOnly = true;
                        Response.Cookies.Add(cookie);

                        Session["UserId"] = v.UserId;
                        Session["UserName"] = v.UserFirstName + " " + v.UserLastName;
                        Session["UserRole"] = v.Role.RoleDescription;

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
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "User");
        }

        public ActionResult MyProfile()
        {
            //int userId = Int32.Parse(Session["UserId"].ToString());
            //int roleId = Int32.Parse(Session["RoleId"].ToString());
            int userId = 2;
            //int roleId = 2;

            SussexDBEntities db = new SussexDBEntities();
            MyProfileViewModel viewModel = new MyProfileViewModel();
            viewModel.User = db.Users.Where(w => w.UserId == userId).FirstOrDefault();
            return View(viewModel);
        }

        public ActionResult MyFriends()
        {
            //int userId = Int32.Parse(Session["UserId"].ToString());
            //int roleId = Int32.Parse(Session["RoleId"].ToString());
            int userId = 2;
            //int roleId = 2;

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
                            w.UserHobbies.Where(w2=> w2.HobbyId == hobby.HobbyId).ToList().Count) > 0)
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

        public ActionResult AddFriend(int Id)
        {
            //int userId = Int32.Parse(Session["UserId"].ToString());
            int userId = 2;
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
        public ActionResult AcceptRequest(int Id)
        {
            //int userId = Int32.Parse(Session["UserId"].ToString());
            int userId = 2;
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

        public ActionResult AddHobby(int Id, bool IsDelete = false)
        {
            //int userId = Int32.Parse(Session["UserId"].ToString());
            int userId = 2;
            using (SussexDBEntities db = new SussexDBEntities())
            {
                if (IsDelete)
                {
                    UserHobby userHobby = db.UserHobbies.Where(w => w.UserId == userId && w.HobbyId == Id).FirstOrDefault();
                    db.UserHobbies.Remove(userHobby);
                    db.SaveChanges();
                }
                else
                {
                    UserHobby hobby = new UserHobby();
                    hobby.UserId = userId;
                    hobby.HobbyId = Id;
                    db.UserHobbies.Add(hobby);
                    db.SaveChanges();
                }
                
            }
            return Redirect("/User/MyProfile");
        }

        public ActionResult AddPersonality(int Id, bool IsDelete = false)
        {
            //int userId = Int32.Parse(Session["UserId"].ToString());
            int userId = 2;
            using (SussexDBEntities db = new SussexDBEntities())
            {
                if (IsDelete)
                {
                    UserPersonality userPersonality = db.UserPersonalities.Where(w => w.UserId == userId && w.PersonalityId == Id).FirstOrDefault();
                    db.UserPersonalities.Remove(userPersonality);
                    db.SaveChanges();
                }
                else
                {
                    UserPersonality personality = new UserPersonality();
                    personality.UserId = userId;
                    personality.PersonalityId = Id;
                    db.UserPersonalities.Add(personality);
                    db.SaveChanges();
                }

            }
            return Redirect("/User/MyProfile");
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