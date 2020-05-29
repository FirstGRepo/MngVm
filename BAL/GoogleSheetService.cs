using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.Ajax.Utilities;
using MngVm.Constant;
using MngVm.Models;
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
            credential = GoogleCredential.FromFile(CommonConstant.GoogleAuthFilePath)
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


        public bool UpdateCellValue(string spreadsheetId, string sheetName, string cellIndex, string value)
        {
            bool _return = false;
            try
            {
                string range = $"{ sheetName }!{cellIndex}";
                var valuesRange = new ValueRange();
                var objectList = new List<object>() { value };
                valuesRange.Values = new List<IList<object>> { objectList };

                var updateReq = service
                    .Spreadsheets.Values.Update(valuesRange, spreadsheetId, range);
                updateReq.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
                var updateRespo = updateReq.Execute();
                _return = true;
            }
            catch (Exception ex)
            {

            }


            return _return;
        }



        public VMLogSheetDetail GetVMLogDetail(string spreadsheetId, string sheetName, string vmName)
        {
            VMLogSheetDetail _return = null;
            try
            {
                AzureVMLogger azureVMLogger = new AzureVMLogger();
                int searchedRowIndex = GetVMRowIndex(spreadsheetId, sheetName, vmName);

                if (searchedRowIndex > 0)
                {
                    string _serverCell = string.Concat(azureVMLogger.ServerColumn[0], searchedRowIndex);
                    string _serverDateTimeCell = string.Concat(azureVMLogger.ServerDateTimeColumn[0], searchedRowIndex);
                    string _serverStatusCell = string.Concat(azureVMLogger.ServerStatusColumn[0], searchedRowIndex);
                    string _userActiveCell = string.Concat(azureVMLogger.UserActiveColumn[0], searchedRowIndex);
                    string _userActiveDateTimeCell = string.Concat(azureVMLogger.UserActiveDateTimeColumn[0], searchedRowIndex);

                    _return = new VMLogSheetDetail()
                    {
                        ServerName = GetCellValue(spreadsheetId, sheetName, _serverCell)?.ToString(),
                        ServerDateTime = GetCellValue(spreadsheetId, sheetName, _serverDateTimeCell)?.ToString(),
                        ServerStatus = GetCellValue(spreadsheetId, sheetName, _serverStatusCell)?.ToString(),
                        UserActiveDateTime = GetCellValue(spreadsheetId, sheetName, _userActiveDateTimeCell)?.ToString(),
                        UserActiveStatus = GetCellValue(spreadsheetId, sheetName, _userActiveCell)?.ToString()

                    };


                }

            }
            catch (Exception ex)
            {

            }


            return _return;
        }


        public int GetVMRowIndex(string spreadsheetId, string sheetName, string vmName)
        {
            int _return = 0;

            try
            {
                AzureVMLogger azureVMLogger = new AzureVMLogger();

                char ServerColRowIndex = azureVMLogger.ServerColumn[1];
                char ServerColIndex = azureVMLogger.ServerColumn[0];
                string vmCellRange = string.Concat(azureVMLogger.ServerColumn, ":", ServerColIndex);

                string range = $"{ sheetName }!{vmCellRange}";

                var getServerRequest = service.Spreadsheets.Values.Get(spreadsheetId, range);
                var getServerResponse = getServerRequest.Execute();
                if (getServerResponse.IsNotNull() && getServerResponse.Values.IsNotNull() && getServerResponse.Values.Count > 0)
                {
                    int count = Convert.ToInt32(ServerColRowIndex.ToString()) - 1;
                    foreach (var item in getServerResponse.Values)
                    {
                        count++;
                        if (item.Count > 0 && item[0].Equals(vmName))
                        {
                            _return = count;
                        }
                    }

                }


            }
            catch (Exception ex)
            {

            }

            return _return;
        }

        public object GetCellValue(string spreadsheetId, string sheetName, string cellName)
        {
            object _return = null;
            try
            {
                string range = $"{ sheetName }!{cellName}";

                var getCellRequest = service
                    .Spreadsheets.Values.Get(spreadsheetId, range);
                var getCellResponse = getCellRequest.Execute();
                if (getCellResponse.IsNotNull() && getCellResponse.Values.IsNotNull() && getCellResponse.Values.Count > 0)
                {
                    foreach (var item in getCellResponse.Values)
                    {
                        if (item.Count > 0)
                        {
                            _return = item[0];
                        }
                    }

                }
            }
            catch (Exception ex)
            {

            }


            return _return;
        }
    }
}