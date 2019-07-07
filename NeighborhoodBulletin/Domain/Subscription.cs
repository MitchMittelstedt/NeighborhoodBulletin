using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain
{
    public class Subscription
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Neighbor")]
        public int NeighborId { get; set; }
        public Neighbor Neighbor { get; set; }
        [ForeignKey("ShopOwner")]
        public int ShopOwnerId { get; set; }
        public ShopOwner ShopOwner { get; set; }
        [Display(Name = "Subscribe/Unsubscribegit ")]
        public bool SubscriptionStatus { get; set; } = true;
    }
}
