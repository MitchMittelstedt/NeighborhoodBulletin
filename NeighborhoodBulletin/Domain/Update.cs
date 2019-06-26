﻿using System;
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
        public virtual ShopOwner ShopOwner { get; set; }
        public string Text { get; set; }
        public bool SubmitButton { get; set; }
        public bool EditButton { get; set; }
        public bool DeleteButton { get; set; }
        public bool Like { get; set; }
        public bool Reply { get; set; }

    }
}
