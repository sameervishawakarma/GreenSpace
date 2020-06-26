using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoApi.Models
{
    public class ReportTemplate
    {
        public int Id { get; set; }
        
        //[Newtonsoft.Json.JsonIgnore]
        //public Room Room { get; set; }
        public string Name { get; set; }
        public int TemplateId { get; set; }
        public string TemplateName { get; set; }
        public int Privacy { get; set; }
        public string GroupIds { get; set; }
        public string FieldIds { get; set; }
        public Boolean IsEnable { get; set; }
        public Boolean IsActive { get; set; }
        public Boolean IsDelete { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        [JsonProperty("RFields")]
        public ICollection<ReportFields> RFields { get; set; }
        [JsonProperty("RGroups")]
        public ICollection<ReportGroups> RGroups { get; set; }
        [JsonProperty("RParameter")]
        public ICollection<ReportParameter> RParameter { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [NotMapped]
        public string Key { get; set; }
    }
}
