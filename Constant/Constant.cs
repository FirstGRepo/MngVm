using System;
using System.Collections.Generic;
using System.Configuration;

namespace MngVm.Constant
{
    public class Constant
    {
        public static readonly bool IsProduction = Convert.ToBoolean(ConfigurationManager.AppSettings["IsProduction"]);
        public static readonly string AuthURL = Convert.ToString(ConfigurationManager.AppSettings["authurl"]);
        public static readonly string tenant_id = Convert.ToString(ConfigurationManager.AppSettings["tenant_id"]);
        public static readonly string grant_type = Convert.ToString(ConfigurationManager.AppSettings["grant_type"]);
        public static readonly string client_id = Convert.ToString(ConfigurationManager.AppSettings["client_id"]);
        public static readonly string client_secret = Convert.ToString(ConfigurationManager.AppSettings["client_secret"]);
        public static readonly string baseurl = Convert.ToString(ConfigurationManager.AppSettings["baseurl"]);
        public static readonly string subscriptionId = Convert.ToString(ConfigurationManager.AppSettings["subscriptionId"]);
        public static readonly string authurl = Convert.ToString(ConfigurationManager.AppSettings["authurl"]);
        public static readonly string statusurl = Convert.ToString(ConfigurationManager.AppSettings["statusurl"]);
        public static readonly string OperationStart = "start";
        public static readonly string OperationStop = "deallocate";

        public static readonly string UserSheetId = Convert.ToString(ConfigurationManager.AppSettings["gsvmSpreadSheetID"]);
        public static readonly string UserSheetName = Convert.ToString(ConfigurationManager.AppSettings["gsvmSheetName"]);
        public static readonly string UserEmailIdColumn = Convert.ToString(ConfigurationManager.AppSettings["gsvmEmailIdColumn"]);
        public static readonly string UserDefaultVMColumn = Convert.ToString(ConfigurationManager.AppSettings["gsvmDefaultVMColumn"]);
        public static readonly string UserResourceGroupColumn = Convert.ToString(ConfigurationManager.AppSettings["gsvmResourceGroupColumn"]);
        public static readonly string SplitChar = Convert.ToString(ConfigurationManager.AppSettings["gsvmSplitChar"]);

        public static string GetAuthUrl()
        {
            if (string.IsNullOrEmpty(AuthURL))
                return string.Empty;
            else
                return AuthURL.Replace("{tenant_id}", tenant_id);
        }
    }
}