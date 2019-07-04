using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain
{
    public class ShopHashtag
    {
        [Key]
        public int Id { get; set; }
        public string Text { get; set; }
        [ForeignKey("ShopOwner)")]
        public int ShopOwnerId { get; set; }
        public virtual ShopOwner ShopOwner { get; set; }
        public int ZipCode { get; set; }
    }
}
