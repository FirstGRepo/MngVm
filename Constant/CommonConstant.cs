using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MngVm.Constant
{
    public class CommonConstant
    {
        public static readonly string GoogleAuthFilePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Credentials/googleauth.json");
        public static readonly string AzureAuthFilePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Credentials/azureauth.properties");

        public static readonly string DateTimeFormat = "dd-MM-yyyy HH:mm:ss";

        public static TimeSpan GetIdleTime(string value)
        {
            TimeSpan idleTime = new TimeSpan(0, 0, 0, 0, 0);

            if (value.IsNotNullOrEmpty())
            {
                if (value.Contains("+"))
                {
                    var splitString = value.Split("+", StringSplitOptions.RemoveEmptyEntries);
                    if (splitString.Length > 1)
                    {
                        idleTime = TimeSpan.Parse(string.Concat(splitString[0], ":", splitString[1], ":", 0));

                    }
                }
                else if (value.Contains(":"))
                {

                    idleTime = TimeSpan.Parse(string.Concat(value, ":", 0));

                }
                else
                {

                    idleTime = TimeSpan.Parse(string.Concat(0, ":", value, ":", 0));

                }


            }


            return idleTime;
        }
    }

    public static class ServerStatus
    {
        public static String Running { get { return "Running"; } }
        public static String Stopped { get { return "Stopped"; } }
    }

    public static class UserStatus
    {
        public static String Active { get { return "Yes"; } }
        public static String InActive { get { return "No"; } }
    }
}