using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TodoApi.Models
{
    public class ReportFields
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DataType { get; set; }
        public string GroupsFilter { get; set; }
        public string FormsFilter { get; set; }
        public int ParentType { get; set; }
        public string DisplayText { get; set; }
        public int ReportTemplateId { get; set; }
        public long FieldId { get; set; }
        public ReportTemplate ReportTemplate { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [NotMapped]
        public string Key { get; set; }
    }
}
