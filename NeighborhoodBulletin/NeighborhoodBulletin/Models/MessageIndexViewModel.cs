using Domain;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeighborhoodBulletin.Models
{
    public class MessageIndexViewModel
    {
        public Neighbor Neighbor { get; set; }
        public List<Message> Messages { get; set; }
        public Message Message { get; set; }
        public ShopOwner ShopOwner { get; set; }
        public List<ShopOwner> ShopOwners { get; set; }
        public List<Update> Updates { get; set; }
        public Update Update { get; set; }
        public Location Location { get; set; }
        public JArray ShopOwnersArray { get; set; }
        public string Url { get; set; }
        public Subscription Subscription { get; set; }
        public List<ZipCode> ZipCodes { get; set; }
        public List<Message> MessagesOutsideZipCode { get; set; }
        public List<Update> UpdatesOutsideZipCode { get; set; }
        public Dictionary<string, double>[] LatLngs { get; set; }
    }
}
