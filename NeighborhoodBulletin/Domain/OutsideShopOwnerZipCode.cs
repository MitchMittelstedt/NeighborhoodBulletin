using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain
{
    public class OutsideShopOwnerZipCode
    {
        [Key]
        public int Id { get; set; }
        public int NonlocalZipCode { get; set; }
        [ForeignKey("ShopOwner")]
        public int ShopOwnerId { get; set; }
        public ShopOwner ShopOwner { get; set; }

    }
}
