﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class SussexDBEntities : DbContext
    {
        public SussexDBEntities()
            : base("name=SussexDBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<BillingHistory> BillingHistories { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<EventCategory> EventCategories { get; set; }
        public virtual DbSet<Hobby> Hobbies { get; set; }
        public virtual DbSet<MeetingSchedule> MeetingSchedules { get; set; }
        public virtual DbSet<PaymentCard> PaymentCards { get; set; }
        public virtual DbSet<Personality> Personalities { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserDetail> UserDetails { get; set; }
        public virtual DbSet<UserEvent> UserEvents { get; set; }
        public virtual DbSet<UserHobby> UserHobbies { get; set; }
        public virtual DbSet<UserMatch> UserMatches { get; set; }
        public virtual DbSet<UserPersonality> UserPersonalities { get; set; }
    }
}