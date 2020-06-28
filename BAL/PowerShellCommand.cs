using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Collections.ObjectModel;
using System.IO;
using System.Management.Automation.Runspaces;
using System.Text;
using Microsoft.Azure.Management.Monitor.Fluent.Models;
using System.Linq;
using System.Configuration;
using MngVm.Models;
using Microsoft.Ajax.Utilities;
using System.Web.Helpers;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.Web.Mvc;

namespace MngVm.BAL
{
    public class PowerShellCommand
    {
        public Collection<PSObject> AssignMachine(string hostPoolName, string userName)
        {
            string vmScriptPath = Convert.ToString(ConfigurationManager.AppSettings["PsGetVmHostPool"]);
            PowerShell ps = PowerShell.Create();
            return ps.AddScript(File.ReadAllText(vmScriptPath))
             .AddParameter("HostPoolName", hostPoolName)
             .AddParameter("UserName", userName)
             .Invoke();
        }

        public List<HostPoolVms> getHostpoolsVmList()
        {
            string jsonResult = string.Empty;

            using (PowerShell powershell = PowerShell.Create())
            {
                string vmScriptPath3 = Convert.ToString(ConfigurationManager.AppSettings["PsGetVmHostPool"]);
                powershell.AddScript(vmScriptPath3);
                powershell.AddCommand("Out-String");
                Collection<PSObject> results = powershell.Invoke();
                powershell.Streams.ClearStreams();
                powershell.Commands.Clear();
                foreach (PSObject obj in results)
                {
                    jsonResult = jsonResult + obj.ToString();
                }
            }

            List<HostPoolVms> result = new JavaScriptSerializer().Deserialize<List<HostPoolVms>>(jsonResult);

            return result;
        }

        public IEnumerable<string> getHostPools()
        {
            var lst = getHostpoolsVmList();
            return lst.Select(o => o.HostPoolName).Distinct();

        }

        public IEnumerable<string> getVmList(string hostPoolName)
        {
            var lst = getHostpoolsVmList();
            return lst.Where(x => x.HostPoolName.Equals(hostPoolName)).Select(x => x.VmName).ToList();
        }

        public List<User> getUserVmList()
        {
            List<User> result = new List<User>();
            try
            {
                string jsonResult = string.Empty;
                using (PowerShell powershell = PowerShell.Create())
                {
                    string vmScriptPath3 = Convert.ToString(ConfigurationManager.AppSettings["PsGetUsers"]);
                    powershell.AddScript(vmScriptPath3);
                    powershell.AddCommand("Out-String");
                    Collection<PSObject> results = powershell.Invoke();
                    powershell.Streams.ClearStreams();
                    powershell.Commands.Clear();
                    foreach (PSObject obj in results)
                    {
                        jsonResult = jsonResult + obj.ToString();
                    }
                    result = new JavaScriptSerializer().Deserialize<List<User>>(jsonResult);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
    }
}
