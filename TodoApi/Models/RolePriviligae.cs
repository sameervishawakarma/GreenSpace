using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoApi.Models
{
    public class RolePriviligae
    {
        public int Id { get; set; }
        public UserRoles UserRole { get; set; }
        public int PageId { get; set; }
        public Boolean View { get; set; }
        public Boolean List { get; set; }
        public Boolean Add { get; set; }
        public Boolean Edit { get; set; }
        public Boolean Delete { get; set; }
    }
}
