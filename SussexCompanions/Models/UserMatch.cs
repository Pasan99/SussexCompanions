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
    
    public partial class UserMatch
    {
        public int UserMatchId { get; set; }
        public int UserMatchRequestUserId { get; set; }
        public int UserMatchRecievedUserId { get; set; }
        public System.DateTime UserMatchDate { get; set; }
        public bool UserMatchIsAccepted { get; set; }
        public bool UserMatchIsDeleted { get; set; }
    
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
    }
}
