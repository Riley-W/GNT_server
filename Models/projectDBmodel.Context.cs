﻿//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace GNT_server.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class projectDBEntities : DbContext
    {
        public projectDBEntities()
            : base("name=projectDBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AdminInfo> AdminInfo { get; set; }
        public virtual DbSet<MemberFavorite> MemberFavorite { get; set; }
        public virtual DbSet<MemberInfo> MemberInfo { get; set; }
        public virtual DbSet<Route> Route { get; set; }
        public virtual DbSet<ShopInfo> ShopInfo { get; set; }
        public virtual DbSet<ShopReview> ShopReview { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<Tag> Tag { get; set; }
        public virtual DbSet<WebsiteReview> WebsiteReview { get; set; }
    }
}
