using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeighborhoodBulletin.Models
{
    public class ShopOwnerSubscriptionViewModel
    {
        public Neighbor Neighbor { get; set; }
        public List<Hashtag> Hashtags { get; set; }
        public List<ShopOwner> ShopOwners { get; set; }
    }
}
