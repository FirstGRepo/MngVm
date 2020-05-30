using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MngVm.Models
{
    public class VMLogSheetDetail
    {
        public string ServerName { get; set; }
        public string ServerStatus { get; set; }
        public string ServerDateTime { get; set; }
        public string UserActiveStatus { get; set; }
        public string UserActiveDateTime { get; set; }
    }
}