//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SussexCompanions.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserDetail
    {
        public int UserDetailId { get; set; }
        public string UserDetailBio { get; set; }
        public string UserDetailProfession { get; set; }
        public int UserDetailAge { get; set; }
        public string UserDetailGender { get; set; }
        public int UserId { get; set; }
    
        public virtual User User { get; set; }
    }
}