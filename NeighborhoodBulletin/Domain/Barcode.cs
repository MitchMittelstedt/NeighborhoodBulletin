using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain
{
    public class Barcode
    {
        [Key]
        public int Id { get; set; }
        public string value { get; set; }
        [ForeignKey("ShopOwnerId")]
        public int ShopOwnerId { get; set; }
        public ShopOwner ShopOwner { get; set; }
    }
}
