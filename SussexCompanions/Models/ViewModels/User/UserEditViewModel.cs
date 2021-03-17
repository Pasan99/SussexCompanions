using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SussexCompanions.Models.ViewModels.User
{
    public class UserEditViewModel
    {
        public string Bio { get; set; }
        public string Profession { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ContactNo { get; set; }
        public string UserType { get; set; }
        public int UserId { get; set; }
        public int UserDetailId { get; set; }

    }
}