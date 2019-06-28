using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class ApplicationUser : IdentityUser
    {
        //public ApplicationUser() : base() { }
        public virtual string Role { get; set; }
    }
}