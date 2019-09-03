using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain
{
    public class MembershipRank
    {
        [Key]
        public int Id { get; set; }
        public string Rank { get; set; }
        public double TotalSpent { get; set; }
        public int Count { get; set; }
        [ForeignKey("NeighborId")]
        public int NeighborId { get; set; }
        public Neighbor Neighbor { get; set; }
        [ForeignKey("ShopOwnerId")]
        public int ShopOwnerId { get; set; }
        public ShopOwner ShopOwner { get; set; }
    }
}
