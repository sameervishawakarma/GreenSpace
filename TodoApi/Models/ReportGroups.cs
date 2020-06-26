using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TodoApi.Models
{
    public class ReportGroups
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Boolean IsActive { get; set; }
        public int ReportTemplateId { get; set; }
        public ReportTemplate ReportTemplate { get; set; }
        public int GroupId { get; set; }
        
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [NotMapped]
        public string Key { get; set; }
    }
}
