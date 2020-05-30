using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.IO;
using System.Web.Script.Serialization;
using System.Data;
using System.Net.Http;
using Newtonsoft.Json;
using MngVm.Constant;
using MngVm.Models;

namespace MngVm.BAL
{
    public class GoogleService
    {
        public GoogleSheetService _sheetService = new GoogleSheetService();

        public string GetToken(string code)
        {
            string poststring = "grant_type=authorization_code&code=" + code + "&client_id=" + GoogleContants.CLIENTID
                                 + "&client_secret=" + GoogleContants.SECRETE_KEY + "&redirect_uri=" +
                                 (Constant.Constant.IsProduction ? GoogleContants.REDIRECT_PROD : GoogleContants.REDIRECT_LOCAL);
            string url = "https://accounts.google.com/o/oauth2/token";

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";
            UTF8Encoding utfenc = new UTF8Encoding();
            byte[] bytes = utfenc.GetBytes(poststring);
            Stream outputstream = null;
            try
            {
                request.ContentLength = bytes.Length;
                outputstream = request.GetRequestStream();
                outputstream.Write(bytes, 0, bytes.Length);
            }
            catch { }
            var response = (HttpWebResponse)request.GetResponse();
            var streamReader = new StreamReader(response.GetResponseStream());
            string responseFromServer = streamReader.ReadToEnd();
            JavaScriptSerializer js = new JavaScriptSerializer();
            TokenModel obj = js.Deserialize<TokenModel>(responseFromServer);
            return obj.access_token;
        }

        public UserProfile userProfile(string access_token)
        {
            string url = GoogleContants.ProfileUrl + access_token + "";
            WebRequest request = WebRequest.Create(url);
            request.Credentials = CredentialCache.DefaultCredentials;
            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            response.Close();
            JavaScriptSerializer js = new JavaScriptSerializer();
            UserProfile userinfo = js.Deserialize<UserProfile>(responseFromServer);
            //imgprofile.ImageUrl = userinfo.picture;
            //lblid.Text = userinfo.id;
            //lblgender.Text = userinfo.gender;
            //lbllocale.Text = userinfo.locale;
            //lblname.Text = userinfo.name;
            //hylprofile.NavigateUrl = userinfo.link;
            //Session["username"] = userinfo.email;
            //if (string.IsNullOrEmpty(userinfo.email))
            //    return Content("Email not found");
            //else
            //{
            //    if (userinfo.email.Contains("organizedgains.com"))
            //    {
            //        return Redirect(Constant.ExternalRedirection);
            //    }
            //    return RedirectToAction("Index");
            //}
            return userinfo;
        }

    }
}