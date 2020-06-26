using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TodoApi.Models
{
    public class Favorite
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int BuildingId { get; set; }
        public int FloorId { get; set; }
        public int RoomId { get; set; }
        public int UserId { get; set; }
        [NotMapped]
        public string Key { get; set; }
    }
}
