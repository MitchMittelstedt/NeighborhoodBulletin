using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Domain
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public class ApplicationUser : IdentityUser
        {
            public virtual string Role { get; set; }
        }
        public DbSet<Neighbor> Neighbors { get; set; }
        public DbSet<ShopOwner> ShopOwners { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Update> Updates { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
    }
}
