using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;

namespace MngVm.Constant
{
    public class GoogleContants
    {
        public static readonly string OAUTHURL = Convert.ToString(ConfigurationManager.AppSettings["googleoauthurl"]);
        public static readonly string API_KEY = Convert.ToString(ConfigurationManager.AppSettings["api_key"]);
        public static readonly string VALIDURL = Convert.ToString(ConfigurationManager.AppSettings["validurl"]);
        //public static readonly string SCOPE = "https://www.googleapis.com/auth/userinfo.profile https://www.googleapis.com/auth/userinfo.email";
        public static readonly string SCOPE = Convert.ToString(ConfigurationManager.AppSettings["scope"]);
        public static readonly string CLIENTID = Convert.ToString(ConfigurationManager.AppSettings["clientid"]);
        public static readonly string REDIRECT_PROD = Convert.ToString(ConfigurationManager.AppSettings["redirectProd"]);
        public static readonly string REDIRECT_LOCAL = Convert.ToString(ConfigurationManager.AppSettings["redirectLocal"]);
        public static readonly string SECRETE_KEY = Convert.ToString(ConfigurationManager.AppSettings["secrete_key"]);
        public static readonly string ExternalRedirection = Convert.ToString(ConfigurationManager.AppSettings["externalRedirection"]);
        public static readonly string ProfileUrl = Convert.ToString(ConfigurationManager.AppSettings["profileUrl"]) + "&access_token=";
        public static readonly string SheetId = Convert.ToString(ConfigurationManager.AppSettings["sheetId"]);
        public static string redirectToGoogleLogin()
        {
            StringBuilder UrlBuilder = new StringBuilder(GoogleContants.OAUTHURL);
            UrlBuilder.Append("client_id=" + GoogleContants.CLIENTID);
            UrlBuilder.Append("&redirect_uri=" + (Constant.IsProduction ? GoogleContants.REDIRECT_PROD : GoogleContants.REDIRECT_LOCAL));
            UrlBuilder.Append("&response_type=" + "code");
            UrlBuilder.Append("&scope=" + GoogleContants.SCOPE);
            UrlBuilder.Append("&access_type=" + "offline");
            UrlBuilder.Append("&state=" + "test"); //setting the user id in state
            return UrlBuilder.ToString();
        }
    }
}