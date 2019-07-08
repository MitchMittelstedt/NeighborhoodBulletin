using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain
{
    public class MessageHashtag
    {
        [Key]
        public int Id { get; set; }
        public string Text { get; set; }
        [ForeignKey("Message")]
        public int MessageId { get; set; }
        public Message Message { get; set; }

    }
}
