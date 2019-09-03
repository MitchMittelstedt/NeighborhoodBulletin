using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;

namespace NeighborhoodBulletin.Models
{
    public class QRCodeViewModel
    {
        public Neighbor Neigbor { get; set; }
        public MembershipRank MembershipRank { get; set; }
        public List<MembershipRank> MembershipRanks { get; set; }

    }
}
