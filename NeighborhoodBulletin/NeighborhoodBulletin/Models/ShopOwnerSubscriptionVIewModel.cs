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
        public ShopOwner ShopOwner { get; set; }
        public List<ShopOwner> ShopOwners { get; set; }
        public List<ShopOwner> NonlocalShopOwners { get; set; }
        public ShopOwner NonlocalShopOwner { get; set; }
        public Subscription Subscription { get; set; }
        public List<Subscription> Subscriptions { get; set; }
        public bool? Subscribed { get; set; }
        public List<int> ShopOwnerIds { get; set; }
        public List<int?> NonlocalShopOwnerIds { get; set; }
        public string QRCodeURL { get; set; }
    }
}
