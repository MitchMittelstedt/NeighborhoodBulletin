using Domain;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeighborhoodBulletin.Models
{
    public class UpdateIndexViewModel
    {
        public List<Update> Updates { get; set; }
        public Update Update { get; set; }
        public List<Update> AllUpdates { get; set; }
        public List<Update> ScheduledUpdates { get; set; }
        public List<Message> Messages { get; set; }
        public List<Message> MessagesOutsideZipCode { get; set; }
        public Message Message { get; set; }
        public ShopOwner ShopOwner { get; set; }
        public List<int> SubscriberZipCodes { get; set; }
        public int[] ZipCodes { get; set; }
        public int[] Frequencies { get; set; }
    }
}
