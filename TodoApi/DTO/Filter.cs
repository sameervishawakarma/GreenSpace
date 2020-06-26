using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoApi.DTO
{
    public class Filter
    {
        public List<FieldCondition> Fields { get; set; }

        public List<long> RegionIds { get; set; }

        public List<long> SiteIds { get; set; }

        public List<long> BuildingIds { get; set; }

        public List<long> FloorIds { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [NotMapped]
        public string Key { get; set; }

    }
}