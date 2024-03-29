﻿using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class Message
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Neighbor")]
        public int NeighborId { get; set; }
        public virtual Neighbor Neighbor { get; set; }
        public int ZipCode { get; set; }
        [Display(Name = "User Locality")]
        public int NeighborZipCode { get; set; }
        public string Username { get; set; }
        public string Text { get; set; }
        public string Image { get; set; }
        [NotMapped]
        public List<string> Hashtags { get; set; }
        [Display(Name = "Date Posted")]
        public DateTime DateTime { get; set; }


    }
}
