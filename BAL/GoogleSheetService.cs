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

        public bool UpdateCellValueWithColorRed(string spreadsheetId, string sheetName, string cellIndex, string value, bool isRed)
        {
            bool _return = false;
            try
            {
                string range = $"{ sheetName }!{cellIndex}";
                //var valuesRange = new ValueRange();
                //var objectList = new List<object>() { value };
                //valuesRange.Values = new List<IList<object>> { objectList };



                //get sheet id by sheet name
                SpreadsheetsResource.GetRequest spresRequest = service.Spreadsheets.Get(spreadsheetId);
                spresRequest.Ranges = new List<string>() { range };
                spresRequest.IncludeGridData = true;

                Spreadsheet spr = spresRequest.Execute();
                Sheet sh = spr.Sheets.Where(s => s.Properties.Title == sheetName).FirstOrDefault();
                int sheetId = (int)sh.Properties.SheetId;

                var _dataInCell = sh.Data?[0];
                //define cell color 
                var _cellFormat = new CellFormat()
                {
                    BackgroundColor = new Color()
                    {
                        Blue = 1,
                        Red = 1,
                        Green = 1,
                        Alpha = 0
                    },
                    TextFormat = new TextFormat()
                    {
                        Bold = true,
                        ForegroundColor = new Color()
                        {
                            Blue = 0,
                            Red = isRed ? 1 : 0,
                            Green = 0,
                            Alpha = 0
                        },
                    }
                };


                BatchUpdateSpreadsheetRequest bussr = new BatchUpdateSpreadsheetRequest();
                //create the update request for cells from the first row
                var updateCellsRequest = new Request()
                {
                    RepeatCell = new RepeatCellRequest()
                    {

                        Range = new GridRange()
                        {
                            SheetId = sheetId,
                            StartColumnIndex = _dataInCell?.StartColumn,
                            StartRowIndex = _dataInCell?.StartRow,
                            EndColumnIndex = _dataInCell?.StartColumn + 1,
                            EndRowIndex = _dataInCell?.StartRow + 1,
                        },
                        Cell = new CellData()
                        {
                            
                            FormattedValue = value,
                            UserEnteredValue = new ExtendedValue() { StringValue = value},
                            UserEnteredFormat = _cellFormat
                        },
                        //Fields = "UserEnteredFormat(BackgroundColor,TextFormat)"
                        Fields = "UserEnteredValue,UserEnteredFormat(BackgroundColor,TextFormat)"
                    }
                };
                bussr.Requests = new List<Request>();
                bussr.Requests.Add(updateCellsRequest);
                var updateReq = service.Spreadsheets.BatchUpdate(bussr, spreadsheetId);
                //bur.Execute();


                //var updateReq = service
                //    .Spreadsheets.Values.Update(valuesRange, spreadsheetId, range);
                //updateReq.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;

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
                    string _resourceGroupCell = string.Concat(azureVMLogger.ResourceGroupColumn[0], searchedRowIndex);
                    string _serverCell = string.Concat(azureVMLogger.ServerColumn[0], searchedRowIndex);
                    string _serverDateTimeCell = string.Concat(azureVMLogger.ServerDateTimeColumn[0], searchedRowIndex);
                    string _serverStatusCell = string.Concat(azureVMLogger.ServerStatusColumn[0], searchedRowIndex);
                    string _userActiveCell = string.Concat(azureVMLogger.UserActiveColumn[0], searchedRowIndex);
                    string _userActiveDateTimeCell = string.Concat(azureVMLogger.UserActiveDateTimeColumn[0], searchedRowIndex);

                    string _serverDateTimeRawValue = GetCellValue(spreadsheetId, sheetName, _serverDateTimeCell)?.ToString();
                    string _userActiveDateTimeRawValue = GetCellValue(spreadsheetId, sheetName, _userActiveDateTimeCell)?.ToString();
                    _return = new VMLogSheetDetail()
                    {
                        rowId = searchedRowIndex,
                        ResourceGroupName = GetCellValue(spreadsheetId, sheetName, _resourceGroupCell)?.ToString(),
                        ServerName = GetCellValue(spreadsheetId, sheetName, _serverCell)?.ToString(),
                        ServerDateTime = _serverDateTimeRawValue.IsNullOrEmpty() ? DateTime.Now : Convert.ToDateTime(_serverDateTimeRawValue),
                        ServerStatus = GetCellValue(spreadsheetId, sheetName, _serverStatusCell)?.ToString(),
                        UserActiveDateTime = _userActiveDateTimeRawValue.IsNullOrEmpty() ? DateTime.Now : Convert.ToDateTime(_userActiveDateTimeRawValue),
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

        public int GetUserEmailRowIndex(string spreadsheetId, string sheetName, string emailID)
        {
            int _return = 0;

            try
            {

                char UserEmailIDColRowIndex = Constant.Constant.UserEmailIdColumn[1];
                char UserEmailIdColIndex = Constant.Constant.UserEmailIdColumn[0];
                string vmCellRange = string.Concat(Constant.Constant.UserEmailIdColumn, ":", UserEmailIdColIndex);

                string range = $"{ sheetName }!{vmCellRange}";

                var getServerRequest = service.Spreadsheets.Values.Get(spreadsheetId, range);
                var getServerResponse = getServerRequest.Execute();
                if (getServerResponse.IsNotNull() && getServerResponse.Values.IsNotNull() && getServerResponse.Values.Count > 0)
                {
                    int count = Convert.ToInt32(UserEmailIDColRowIndex.ToString()) - 1;
                    foreach (var item in getServerResponse.Values)
                    {
                        count++;
                        if (item.Count > 0 && item[0].Equals(emailID))
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

        public UserProfile GetUserVMDetail(string emailId)
        {
            UserProfile _return = null;

            try
            {
                string spreadsheetId = Constant.Constant.UserSheetId;
                string sheetName = Constant.Constant.UserSheetName;
                string UserDefaultVmCol = Constant.Constant.UserDefaultVMColumn;
                char UserDefaultVmColRowIndex = Constant.Constant.UserDefaultVMColumn[1];
                char UserDefaultVmColIndex = Constant.Constant.UserDefaultVMColumn[0];
                char UserResourceGroupColIndex = Constant.Constant.UserResourceGroupColumn[0];
                int searchedRowIndex = GetUserEmailRowIndex(spreadsheetId, sheetName, emailId);

                if (searchedRowIndex > 0)
                {
                    List<MachineInfo> lstMachineInfo = new List<MachineInfo>();

                    for (char colIndex = UserDefaultVmColIndex; colIndex <= 'Z'; colIndex++)
                    {
                        MachineInfo machine = new MachineInfo();
                        string cellName = string.Concat(colIndex, searchedRowIndex);
                        string vmName = Convert.ToString(GetCellValue(spreadsheetId, sheetName, cellName));
                        if (vmName.Contains(Constant.Constant.SplitChar))
                        {
                            string[] machineInfo = Convert.ToString(vmName).Split(Constant.Constant.SplitChar.ToChar());
                            machine.MachineName = machineInfo[0].Trim();
                            machine.Resource_Group = machineInfo[1].Trim();
                            lstMachineInfo.Add(machine);
                            if (vmName.IsNull())
                                break;
                        }
                    }
                    _return = new UserProfile()
                    {
                        email = emailId,
                        machineInfo = lstMachineInfo
                    };
                }
            }
            catch (Exception ex)
            {

            }
            return _return;
        }

        public VMLogAutoShut GetVMAutoShutDetail()
        {
            VMLogAutoShut _return = null;
            try
            {
                AzureVMLogger azureVMLogger = new AzureVMLogger();

                if (azureVMLogger.VMLogSpreadSheetID.IsNotNullOrEmpty() && azureVMLogger.VMLogSheetName.IsNotNullOrEmpty())
                {
                    string spreadsheetId = azureVMLogger.VMLogSpreadSheetID;
                    string sheetName = azureVMLogger.VMLogSheetName;

                    string _lastUpdatedCell = azureVMLogger.LastSheetUpdated;
                    string _autoShutTimeCell = azureVMLogger.AutoShutTime;

                    string _lastUpdatedCellValue = GetCellValue(spreadsheetId, sheetName, _lastUpdatedCell)?.ToString();
                    string _autoShutTimeCellValue = GetCellValue(spreadsheetId, sheetName, _autoShutTimeCell)?.ToString();

                    _return = new VMLogAutoShut()
                    {

                        AutoShutTime = _autoShutTimeCellValue.IsNullOrEmpty() ? default(TimeSpan) : TimeSpan.Parse(_autoShutTimeCellValue),
                        LastUpdated = _lastUpdatedCellValue.IsNullOrEmpty() ? default(DateTime) : Convert.ToDateTime(_lastUpdatedCellValue),

                    };


                }

            }
            catch (Exception ex)
            {

            }


            return _return;
        }

        public bool UpdateVmLogUserStatus(int rowID, string value, bool IsUpdateDate)
        {
            bool _return = false;

            try
            {
                AzureVMLogger azureVMLogger = new AzureVMLogger();

                if (rowID > 0 && azureVMLogger.VMLogSpreadSheetID.IsNotNullOrEmpty() && azureVMLogger.VMLogSheetName.IsNotNullOrEmpty())
                {
                    string spreadsheetId = azureVMLogger.VMLogSpreadSheetID;
                    string sheetName = azureVMLogger.VMLogSheetName;

                    string _UserActiveCol = azureVMLogger.UserActiveColumn;
                    char _UserActiveColRowIndex = azureVMLogger.UserActiveColumn[1];
                    char _UserActiveColIndex = azureVMLogger.UserActiveColumn[0];

                    string _UserActiveDateTimeCol = azureVMLogger.UserActiveDateTimeColumn;
                    char _UserActiveDateTimeColRowIndex = azureVMLogger.UserActiveDateTimeColumn[1];
                    char _UserActiveDateTimeColIndex = azureVMLogger.UserActiveDateTimeColumn[0];

                    string UserActiveCellName = string.Concat(_UserActiveColIndex, rowID);
                    string UserActiveDateTimeCellName = string.Concat(_UserActiveDateTimeColIndex, rowID);

                    if (value.IsNotNullOrEmpty())
                    {

                        _return = UpdateCellValue(spreadsheetId, sheetName, UserActiveCellName, value);
                        if (_return && IsUpdateDate)
                        {
                            string currentDateTime = DateTime.Now.ToString(CommonConstant.DateTimeFormat);
                            UpdateCellValue(spreadsheetId, sheetName, UserActiveDateTimeCellName, currentDateTime);
                        }

                    }

                }

            }
            catch (Exception ex)
            {

            }


            return _return;
        }

        public bool UpdateVmLogServerStatus(int rowID, string value, bool IsUpdateDate)
        {
            bool _return = false;

            try
            {
                AzureVMLogger azureVMLogger = new AzureVMLogger();

                if (rowID > 0 && azureVMLogger.VMLogSpreadSheetID.IsNotNullOrEmpty() && azureVMLogger.VMLogSheetName.IsNotNullOrEmpty())
                {
                    string spreadsheetId = azureVMLogger.VMLogSpreadSheetID;
                    string sheetName = azureVMLogger.VMLogSheetName;

                    string _ServerStatusCol = azureVMLogger.ServerStatusColumn;
                    char _ServerStatusColRowIndex = azureVMLogger.ServerStatusColumn[1];
                    char _ServerStatusColIndex = azureVMLogger.ServerStatusColumn[0];

                    string _ServerDateTimeCol = azureVMLogger.ServerDateTimeColumn;
                    char _ServerDateTimeColRowIndex = azureVMLogger.ServerDateTimeColumn[1];
                    char _ServerDateTimeColIndex = azureVMLogger.ServerDateTimeColumn[0];

                    string ServerStatusCellName = string.Concat(_ServerStatusColIndex, rowID);
                    string ServerDateTimeCellName = string.Concat(_ServerDateTimeColIndex, rowID);

                    if (value.IsNotNullOrEmpty())
                    {

                        _return = UpdateCellValue(spreadsheetId, sheetName, ServerStatusCellName, value);
                        if (_return && IsUpdateDate)
                        {
                            string currentDateTime = DateTime.Now.ToString(CommonConstant.DateTimeFormat);
                            UpdateCellValue(spreadsheetId, sheetName, ServerDateTimeCellName, currentDateTime);
                        }

                    }

                }

            }
            catch (Exception ex)
            {

            }


            return _return;
        }


        public bool UpdateVmLogCPUAverage(int rowID, string value, int indxCPUCol)
        {
            bool _return = false;

            try
            {
                AzureVMLogger azureVMLogger = new AzureVMLogger();

                if (rowID > 0 && azureVMLogger.VMLogSpreadSheetID.IsNotNullOrEmpty() && azureVMLogger.VMLogSheetName.IsNotNullOrEmpty())
                {
                    string spreadsheetId = azureVMLogger.VMLogSpreadSheetID;
                    string sheetName = azureVMLogger.VMLogSheetName;

                    string _CPUAvgCol = string.Empty;
                    char _CPUAvgColRowIndex = char.MinValue;
                    char _CPUAvgColIndex = char.MinValue;

                    switch (indxCPUCol)
                    {
                        case 1:
                            _CPUAvgCol = azureVMLogger.CPU1Column;
                            _CPUAvgColRowIndex = azureVMLogger.CPU1Column[1];
                            _CPUAvgColIndex = azureVMLogger.CPU1Column[0];

                            break;
                        case 2:
                            _CPUAvgCol = azureVMLogger.CPU2Column;
                            _CPUAvgColRowIndex = azureVMLogger.CPU2Column[1];
                            _CPUAvgColIndex = azureVMLogger.CPU2Column[0];
                            break;
                        case 3:
                            _CPUAvgCol = azureVMLogger.CPU3Column;
                            _CPUAvgColRowIndex = azureVMLogger.CPU3Column[1];
                            _CPUAvgColIndex = azureVMLogger.CPU3Column[0];
                            break;
                        case 4:
                            _CPUAvgCol = azureVMLogger.CPU4Column;
                            _CPUAvgColRowIndex = azureVMLogger.CPU4Column[1];
                            _CPUAvgColIndex = azureVMLogger.CPU4Column[0];
                            break;
                        case 5:
                            _CPUAvgCol = azureVMLogger.CPUPeakColumn;
                            _CPUAvgColRowIndex = azureVMLogger.CPUPeakColumn[1];
                            _CPUAvgColIndex = azureVMLogger.CPUPeakColumn[0];
                            break;
                    }

                    string CPUAvgCellName = string.Concat(_CPUAvgColIndex, rowID);

                    if (_CPUAvgCol.IsNotNullOrEmpty() && value.IsNotNullOrEmpty())
                    {
                        _return = UpdateCellValue(spreadsheetId, sheetName, CPUAvgCellName, value);
                    }

                }

            }
            catch (Exception ex)
            {
                _return = false;
            }


            return _return;
        }

        /// <summary> 
        ///  
        /// Row ID : 0 for Last Month 
        ///         1 for Current Month 
        ///  
        /// </summary> 
        /// <param name="rowID"></param> 
        /// <param name="value"></param> 
        /// <param name="costIndex"></param> 
        /// <returns></returns> 
        public bool UpdateVmLogLastMonthCost(int rowID, string value, int monthCostColIndex = 0)
        {
            bool _return = false;

            try
            {
                AzureVMLogger azureVMLogger = new AzureVMLogger();

                if (rowID > 0 && azureVMLogger.VMLogSpreadSheetID.IsNotNullOrEmpty() && azureVMLogger.VMLogSheetName.IsNotNullOrEmpty())
                {
                    string spreadsheetId = azureVMLogger.VMLogSpreadSheetID;
                    string sheetName = azureVMLogger.VMLogSheetName;

                    string _MonthCostCol = string.Empty;
                    char _MonthCostColRowIndex = char.MinValue;
                    char _MonthCostColIndex = char.MinValue;

                    switch (monthCostColIndex)
                    {
                        case 0:
                            _MonthCostCol = azureVMLogger.LastMonthCostColumn;
                            _MonthCostColRowIndex = azureVMLogger.LastMonthCostColumn[1];
                            _MonthCostColIndex = azureVMLogger.LastMonthCostColumn[0];

                            break;
                        case 1:
                            _MonthCostCol = azureVMLogger.ThisMonthCostColumn;
                            _MonthCostColRowIndex = azureVMLogger.ThisMonthCostColumn[1];
                            _MonthCostColIndex = azureVMLogger.ThisMonthCostColumn[0];
                            break;
                    }


                    string _MonthCostCellName = string.Concat(_MonthCostColIndex, rowID);

                    if (_MonthCostCol.IsNotNullOrEmpty() && value.IsNotNullOrEmpty())
                    {
                        _return = UpdateCellValue(spreadsheetId, sheetName, _MonthCostCellName, value);
                    }

                }

            }
            catch (Exception ex)
            {
                _return = false;
            }


            return _return;
        }

        public bool UpdateVmLogDiffCost(int rowID, string value, bool isExceed = false)
        {
            bool _return = false;

            try
            {
                AzureVMLogger azureVMLogger = new AzureVMLogger();

                if (rowID > 0 && azureVMLogger.VMLogSpreadSheetID.IsNotNullOrEmpty() && azureVMLogger.VMLogSheetName.IsNotNullOrEmpty())
                {
                    string spreadsheetId = azureVMLogger.VMLogSpreadSheetID;
                    string sheetName = azureVMLogger.VMLogSheetName;

                    string _CostDiffCol = azureVMLogger.CostDiffColumn;
                    char _CostDiffColRowIndex = azureVMLogger.CostDiffColumn[1];
                    char _CostDiffColIndex = azureVMLogger.CostDiffColumn[0];


                    string _CostDiffCellName = string.Concat(_CostDiffColIndex, rowID);

                    if (_CostDiffCol.IsNotNullOrEmpty() && value.IsNotNullOrEmpty())
                    {
                        _return = UpdateCellValueWithColorRed(spreadsheetId, sheetName, _CostDiffCellName, value, isExceed);
                    }

                }

            }
            catch (Exception ex)
            {
                _return = false;
            }


            return _return;
        }


        public IList<VMLogSheetDetail> GetAllVMLogDetail(string spreadsheetId, string sheetName)
        {
            IList<VMLogSheetDetail> _return = null;
            try
            {
                AzureVMLogger azureVMLogger = new AzureVMLogger();
                int initialRowIndex = int.Parse(azureVMLogger.RowIndex);

                if (initialRowIndex > 0)
                {
                    _return = new List<VMLogSheetDetail>();

                    for (int _rowIndex = initialRowIndex; true; _rowIndex++)
                    {

                        string _resourceGroupCell = string.Concat(azureVMLogger.ResourceGroupColumn[0], _rowIndex);
                        string _serverCell = string.Concat(azureVMLogger.ServerColumn[0], _rowIndex);
                        string _serverDateTimeCell = string.Concat(azureVMLogger.ServerDateTimeColumn[0], _rowIndex);
                        string _serverStatusCell = string.Concat(azureVMLogger.ServerStatusColumn[0], _rowIndex);
                        string _userActiveCell = string.Concat(azureVMLogger.UserActiveColumn[0], _rowIndex);
                        string _userActiveDateTimeCell = string.Concat(azureVMLogger.UserActiveDateTimeColumn[0], _rowIndex);

                        string _serverDateTimeRawValue = GetCellValue(spreadsheetId, sheetName, _serverDateTimeCell)?.ToString();
                        string _userActiveDateTimeRawValue = GetCellValue(spreadsheetId, sheetName, _userActiveDateTimeCell)?.ToString();

                        //If vmName not exist then break the loop
                        if (GetCellValue(spreadsheetId, sheetName, _serverCell).IsNull())
                        {
                            break;
                        }

                        _return.Add(new VMLogSheetDetail()
                        {
                            rowId = _rowIndex,
                            ResourceGroupName = GetCellValue(spreadsheetId, sheetName, _resourceGroupCell)?.ToString(),
                            ServerName = GetCellValue(spreadsheetId, sheetName, _serverCell)?.ToString(),
                            ServerDateTime = _serverDateTimeRawValue.IsNullOrEmpty() ? DateTime.Now : Convert.ToDateTime(_serverDateTimeRawValue),
                            ServerStatus = GetCellValue(spreadsheetId, sheetName, _serverStatusCell)?.ToString(),
                            UserActiveDateTime = _userActiveDateTimeRawValue.IsNullOrEmpty() ? DateTime.Now : Convert.ToDateTime(_userActiveDateTimeRawValue),
                            UserActiveStatus = GetCellValue(spreadsheetId, sheetName, _userActiveCell)?.ToString()

                        });

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