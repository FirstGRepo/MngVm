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
        public static readonly string resource = Convert.ToString(ConfigurationManager.AppSettings["resource"]);
        public static readonly string baseurl = Convert.ToString(ConfigurationManager.AppSettings["baseurl"]);
        public static readonly string subscriptionId = Convert.ToString(ConfigurationManager.AppSettings["subscriptionId"]);
        public static readonly string resourceGroupName = Convert.ToString(ConfigurationManager.AppSettings["resourceGroupName"]);
        public static readonly string vmName = Convert.ToString(ConfigurationManager.AppSettings["vmName"]);
        public static readonly string authurl = Convert.ToString(ConfigurationManager.AppSettings["authurl"]);
        public static readonly string statusurl = Convert.ToString(ConfigurationManager.AppSettings["statusurl"]);
        public static readonly string OperationStart = "start";
        public static readonly string OperationStop = "deallocate";

        public static readonly string UserSheetId = Convert.ToString(ConfigurationManager.AppSettings["gsvmSpreadSheetID"]);
        public static readonly string UserSheetName = Convert.ToString(ConfigurationManager.AppSettings["gsvmSheetName"]);
        public static readonly string UserEmailIdColumn = Convert.ToString(ConfigurationManager.AppSettings["gsvmEmailIdColumn"]);
        public static readonly string UserDefaultVMColumn = Convert.ToString(ConfigurationManager.AppSettings["gsvmDefaultVMColumn"]);

        public static string GetAuthUrl()
        {
            if (string.IsNullOrEmpty(AuthURL))
                return string.Empty;
            else
                return AuthURL.Replace("{tenant_id}", tenant_id);
        }
        public static string GetBaseUrl()
        {
            string newBaseUrl = baseurl;
            if (string.IsNullOrEmpty(newBaseUrl))
                return string.Empty;
            else
            {
                //newBaseUrl = newBaseUrl.Replace("{currentdate}", DateTime.Now.ToString("yyyy-MM-dd"));
                newBaseUrl = newBaseUrl.Replace("{currentdate}", "2019-12-01");
                newBaseUrl = newBaseUrl.Replace("{subscriptionId}", subscriptionId);
                newBaseUrl = newBaseUrl.Replace("{resourceGroupName}", resourceGroupName);
                //newBaseUrl = newBaseUrl.Replace("{vmName}", vmName);
            }
            return newBaseUrl;
        }
        public static string GetStatusUrl(string machineName)
        {
            string newStatusUrl = statusurl;
            if (string.IsNullOrEmpty(newStatusUrl))
                return string.Empty;
            else
            {
                //newBaseUrl = newBaseUrl.Replace("{currentdate}", DateTime.Now.ToString("yyyy-MM-dd"));
                newStatusUrl = newStatusUrl.Replace("{currentdate}", "2019-12-01");
                newStatusUrl = newStatusUrl.Replace("{subscriptionId}", subscriptionId);
                newStatusUrl = newStatusUrl.Replace("{resourceGroupName}", resourceGroupName);
                newStatusUrl = newStatusUrl.Replace("{vmName}", machineName);
            }
            return newStatusUrl;
        }
    }
}