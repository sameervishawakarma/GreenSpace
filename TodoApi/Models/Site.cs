using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace TodoApi.Models
{
    public class Site
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public long RegionId { get; set; }

       public Region Region { get; set; }

        [JsonProperty("items")]
        public ICollection<Building> Buildings { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [NotMapped]
        public string Key { get; set; }
    }
}