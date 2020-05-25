using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MngVm.BAL
{
    public class GoogleSheetService
    {

        private static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        private static readonly string ApplicationName = "Give Any Name";
        private static readonly string SpreadsheetId = "";
        private static readonly string sheet = "sheetName on bottom of sheet tabs";

        private GoogleCredential credential;

        public SheetsService service;

        public GoogleSheetService()
        {
            credential = GoogleCredential.FromFile(System.Web.Hosting.HostingEnvironment.MapPath("~/Credentials/azureauth.properties"))
                .CreateScoped(Scopes);

            service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });

        }

    }
}