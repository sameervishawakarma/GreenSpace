using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace TodoApi.Models
{
    public class Building
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public long SiteId { get; set; }
        
        public Site Site { get; set; }
        
        [JsonProperty("items")]
        public ICollection<Floor> Floors { get; set; }
        [NotMapped]
        public string Key { get; set; }
    }
}