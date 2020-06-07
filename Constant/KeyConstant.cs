using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace MngVm.Constant
{
    public class KeyConstant
    {

    }

    public class AzureVMLogger
    {
        public string VMLogSpreadSheetID => ConfigurationManager.AppSettings["azvmVMLogSpreadSheetID"];
        public string VMLogSheetName => ConfigurationManager.AppSettings["azvmVMLogSheetName"];
        public string AutoShutTime => ConfigurationManager.AppSettings["azvmAutoShutTime"];
        public string LastSheetUpdated => ConfigurationManager.AppSettings["azvmLastSheetUpdated"];

        public string ResourceGroupColumn => ConfigurationManager.AppSettings["azvmResourceGroupColumn"];
        public string UserActiveColumn => ConfigurationManager.AppSettings["azvmUserActiveColumn"];
        public string UserActiveDateTimeColumn => ConfigurationManager.AppSettings["azvmUserActiveDateTimeColumn"];
        public string ServerColumn => ConfigurationManager.AppSettings["azvmServerColumn"];
        public string ServerStatusColumn => ConfigurationManager.AppSettings["azvmServerStatusColumn"];
        public string ServerDateTimeColumn => ConfigurationManager.AppSettings["azvmServerDateTimeColumn"];
        public string RowIndex => ConfigurationManager.AppSettings["azvmRowIndex"];
    }
}