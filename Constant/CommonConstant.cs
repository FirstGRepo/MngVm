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
    }
}