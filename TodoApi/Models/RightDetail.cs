using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoApi.Models
{
    public class RightDetail
    {
        public int Id { get; set; }
        public int RightMasterId { get; set; }
        public RightMaster RightMaster { get; set; }
        public Boolean View { get; set; }
        public Boolean List { get; set; }
        public Boolean Add { get; set; }
        public Boolean Edit { get; set; }
        public Boolean Delete { get; set; }
    }
}
