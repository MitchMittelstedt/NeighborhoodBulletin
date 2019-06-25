using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Domain.ApplicationDbContext;

namespace Domain
{
    public class ShopOwner
    {
        [Key]
        public int Id { get; set; }
        public int ZipCode { get; set; }
        public string BusinessName { get; set; }
        [NotMapped]
        public List<int> CouponZipCodes { get; set; }
        [NotMapped]
        public List<string> DiscussionSubscriptions { get; set; }
        [NotMapped]
        public List<int> ZipCodeSubscriptions { get; set; }
        [ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
