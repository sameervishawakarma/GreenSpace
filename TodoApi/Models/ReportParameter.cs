using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoApi.Models
{
    public class ReportParameter
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DataType { get; set; }
        public string Caption { get; set; }
        public Boolean IsVisible { get; set; }
        public int ReportTemplateId { get; set; }
        public ReportTemplate ReportTemplate { get; set; }
        [NotMapped]
        public string Key { get; set; }
    }
}
