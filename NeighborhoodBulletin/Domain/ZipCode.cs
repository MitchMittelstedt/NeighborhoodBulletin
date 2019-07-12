using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain
{
    public class ZipCode
    {

        [Key]
        public int Id { get; set; }
        public int NonLocalZipCode { get; set; }
        [ForeignKey("NeighborId")]
        public int NeighborId { get; set; }
        public Neighbor Neighbor { get; set; }
    }
}
