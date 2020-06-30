using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoApi.Models
{
    public class RightMaster
    {
        public int Id { get; set; }
        public string PageName { get; set; }
        public string Module { get; set; }
        public string SubModule { get; set; }
        public Boolean IsActive { get; set; }
        [JsonProperty("DFields")]
        public ICollection<RightDetail> DFields { get; set; }
        [JsonProperty("PermissionFields")]
        public ICollection<RolePriviligae> PermissionFields { get; set; }
    }
}
