using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Web;

namespace Domain
{
    public class SingleUseCoupon
    {
        [Key]
        public int Id { get; set; }
        public int Value { get; set; }
        public int? LastSpent { get; set; }
    }
}
