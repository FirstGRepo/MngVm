using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MngVm.Models
{
    public class RunCommandRequest
    {
        //public string ResourceName { get; set; }
        //public string VmName { get; set; }
        //public string UserName { get; set; }
        //public string SessionName { get; set; }
        //public string ID { get; set; }
        //public string State { get; set; }
        //public string IdleTime { get; set; }
        //public DateTime? LogonTime { get; set; }


        public string SessionHostName { get; set; }
        public string TenantName { get; set; }
        public string SessionId { get; set; }
        public string AdUserName { get; set; }
        public string State { get; set; }
        public DateTime? LogonTime { get; set; }

    }
}