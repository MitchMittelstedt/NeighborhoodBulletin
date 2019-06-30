using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain
{
    public class Hashtag
    {
        [Key]
        public int Id { get; set; }
        public string Text { get; set; }
        [ForeignKey("Neighbor")]
        public int NeighborId { get; set; }
        public virtual Neighbor Neighbor { get; set; }
    }
}
