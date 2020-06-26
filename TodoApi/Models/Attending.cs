using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TodoApi.Models
{
    public class Attending
    {
        public long Id { get; set; }
        public string RequestorName { get; set; }
       public string RequestorEmail { get; set; }
       public int attendingNumber { get; set; }
       public bool Catering { get; set; }
       public DateTime RequestedDate { get; set; }
       public string Avrequirements { get; set; }
        [NotMapped]
        public string Key { get; set; }
    }
}
