using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TodoApi.Models
{
    public class AVDevices
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DeviceType DeviceType { get; set; }
        public ApproveDeviceStatus Approved { get; set; }
        [NotMapped]
        public string Key { get; set; }
    }

    public enum ApproveDeviceStatus
    {
        Pending,
        Approved,
        Disapproved
    }

    public enum DeviceType
    {
        None,
        Audio,
        Video,
        Both
    }
}
