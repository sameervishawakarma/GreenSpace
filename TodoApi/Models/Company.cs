using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoApi.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string DBName { get; set; }
        public string CompName { get; set; }
        public Boolean IsActive { get; set; }
    }
}
