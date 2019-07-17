using Domain;
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
        public Message Message { get; set; }
    }
}
