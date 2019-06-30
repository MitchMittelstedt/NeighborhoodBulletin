using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Domain.ApplicationDbContext;

namespace Domain
{
    public class Neighbor
    {
        [Key]
        public int Id { get; set; }
        public int ZipCode { get; set; }
        public string Username { get; set; }
        [NotMapped]
        public List <int> ZipCodeSubscriptions { get; set; }
        [NotMapped]
        public List<string> BusinessSubscriptions { get; set; }
        [NotMapped]
        public List<string> Hashtags { get; set; }
        [ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

    }
}
