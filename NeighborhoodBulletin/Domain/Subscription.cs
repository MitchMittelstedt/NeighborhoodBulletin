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
        public int Rank { get; set; }
        [Display(Name = "Usage Count")]
        public int UsageCount { get; set; }
        [Display(Name = "Total Spent")]
        public double TotalSpent { get; set; }
        [Display(Name = "Subscribe/Unsubscribe")]
        public bool SubscriptionStatus { get; set; } = true;
    }
}
