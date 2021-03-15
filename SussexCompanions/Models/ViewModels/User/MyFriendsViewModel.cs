using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SussexCompanions.Models.ViewModels.User
{
    public class MyFriendsViewModel
    {
        public List<Models.User> Friends { get; set; }
        public List<Models.User> Suggetions { get; set; }
        public List<UserMatch> FriendRequests { get; set; }
        public List<Models.User> OnGoingRequests { get; set; }
    }
}