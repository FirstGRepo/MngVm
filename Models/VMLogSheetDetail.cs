using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MngVm.Models
{
    public class VMLogSheetDetail
    {
        public int rowId { get; set; }
        public string ServerName { get; set; }
        public string ServerStatus { get; set; }
        public string ServerDateTime { get; set; }
        public string UserActiveStatus { get; set; }
        public string UserActiveDateTime { get; set; }
    }

    public class VMLogAutoShut
    {
        public TimeSpan AutoShutTime { get; set; }
        public DateTime? LastUpdated { get; set; }
    }
}