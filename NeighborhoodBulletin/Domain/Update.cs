using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class Update
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("ShopOwner")]
        public int ShopOwnerId { get; set; }
        public int ZipCode { get; set; }
        public int ShopOwnerZipCode { get; set; }
        public virtual ShopOwner ShopOwner { get; set; }
        [Display(Name = "Business Name")]
        public string BusinessName { get; set; }
        public string Text { get; set; }
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }
        [Display(Name = "Add barcode?")]
        public bool HasBarcode { get; set; }
        public string BarcodeValue { get; set; }
        public bool Valid { get; set; }


    }
}
