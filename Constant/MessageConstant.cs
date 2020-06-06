using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MngVm.Constant
{
    public class MessageConstant
    {
        public static string NoMachineFound = "<h2 class='vmError'>No macchine found which assigned to your login, Please contact your team.</h2>";
        public static string LoginUrl = "<h2 class='vmError'>Please Login with google, to proceed click <a href='/'>here</a>. </h2>";
        public static string NotAuthorize = "<h2 class='vmError'>You are not authorized to view this page. <a href='/'>Back To Home</a></h2>";
    }
}