using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace TodoApi.Models
{
    public class TimeSlot
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Boolean IsActive { get; set; }
    }
}
