using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    class Message
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Neighbor")]
        public string Text { get; set; }
        public bool SubmitButton { get; set; }
        public bool EditButton { get; set; }
        public bool DeleteButton { get; set; }
        public string NeighborId { get; set; }
        public virtual Neighbor Neighbor { get; set; }
    }
}
