using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MngVm.Models
{
    public class VMLogSheetDetail
    {
        public int rowId { get; set; }
        public string ResourceGroupName { get; set; }
        public string ServerName { get; set; }
        public string ServerStatus { get; set; }
        public DateTime? ServerDateTime { get; set; }
        public string UserActiveStatus { get; set; }
        public DateTime? UserActiveDateTime { get; set; }
    }

    public class VMLogAutoShut
    {
        public TimeSpan AutoShutTime { get; set; }
        public DateTime? LastUpdated { get; set; }
    }
}