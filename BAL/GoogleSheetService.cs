using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MngVm.BAL
{
    public class GoogleSheetService
    {

        private static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        private static readonly string ApplicationName = "ManageVM";

        private GoogleCredential credential;

        public SheetsService service;

        public GoogleSheetService()
        {
            credential = GoogleCredential.FromFile(System.Web.Hosting.HostingEnvironment.MapPath("~/Credentials/googleauth.json"))
                .CreateScoped(Scopes);

            service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });

        }

        public void UpdateEntity(string spreadsheetId, string sheetName)
        {
            string range = $"{ sheetName }!D5";
            var valuesRange = new ValueRange();
            var objectList = new List<object>() { "Running" };
            valuesRange.Values = new List<IList<object>> { objectList };

            var updateReq = service
                .Spreadsheets.Values.Update(valuesRange, spreadsheetId, range);
            updateReq.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            var updateRespo = updateReq.Execute();
        }

    }
}