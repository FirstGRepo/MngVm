using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MngVm.Constant
{
    public class MessageConstant
    {
        public static string NoMachineFound = "No macchine found which assigned to your login, Please contact your team. <a href='/'>Login Here</a>.";
        public static string LoginUrl = "Please Login with google, to proceed click <a href='/'>here</a>. ";
        public static string InsufficientRequestParameter = "Insufficient Request Parameter";
        public static string SchedulerSuccess = "Scheduler Run Successfull.";
        public static string NotAuthorize = "<h2 class='vmError'>You are not authorized to view this page. <a href='/'>Back To Home</a></h2>";
    }
}