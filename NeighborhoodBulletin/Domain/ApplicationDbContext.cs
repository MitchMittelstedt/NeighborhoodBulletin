using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Domain
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole,string>
    {

        public DbSet<Neighbor> Neighbors { get; set; }
        public DbSet<ShopOwner> ShopOwners { get; set; }
        public DbSet<MembershipRank> MembershipRanks { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Update> Updates { get; set; }
        public DbSet<Hashtag> Hashtags { get; set; }
        public DbSet<ShopHashtag> ShopHashtags { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<MessageHashtag> MessageHashtags { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<ZipCode> ZipCodes { get; set; }
        public DbSet<OutsideShopOwnerZipCode> OutsideShopOwnerZipCodes { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
    }
}
