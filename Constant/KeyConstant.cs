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
        public string CPU1Column => ConfigurationManager.AppSettings["azvmServerCPU1Column"];
        public string CPU2Column => ConfigurationManager.AppSettings["azvmServerCPU2Column"];
        public string CPU3Column => ConfigurationManager.AppSettings["azvmServerCPU3Column"];
        public string CPU4Column => ConfigurationManager.AppSettings["azvmServerCPU4Column"];
        public string CPUPeakColumn => ConfigurationManager.AppSettings["azvmServerCPUPeakColumn"];
        public string ThisMonthCostColumn => ConfigurationManager.AppSettings["azvmServerThisMonthCostColumn"];
        public string LastMonthCostColumn => ConfigurationManager.AppSettings["azvmServerLastMonthCostColumn"];
        public string CostDiffColumn => ConfigurationManager.AppSettings["azvmServerCostDiffColumn"];
        public string ApiVersion => ConfigurationManager.AppSettings["azvmApiVersion"];
    }
}