using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MngVm.Models
{

    public class Statuses
    {
        public string code { get; set; }
        public string level { get; set; }
        public string displayStatus { get; set; }
        public string message { get; set; }
        public DateTime time { get; set; }

    }
    public class Status
    {
        public string code { get; set; }
        public string level { get; set; }
        public string displayStatus { get; set; }

    }
    public class ExtensionHandlers
    {
        public string type { get; set; }
        public string typeHandlerVersion { get; set; }
        public Status status { get; set; }

    }
    public class VmAgent
    {
        public string vmAgentVersion { get; set; }
        public IList<Statuses> statuses { get; set; }
        public IList<ExtensionHandlers> extensionHandlers { get; set; }

    }

    public class Disks
    {
        public string name { get; set; }
        public IList<Statuses> statuses { get; set; }

    }
    public class BootDiagnostics
    {
        public string consoleScreenshotBlobUri { get; set; }
        public string serialConsoleLogBlobUri { get; set; }

    }

    public class Extensions
    {
        public string name { get; set; }
        public string type { get; set; }
        public string typeHandlerVersion { get; set; }
        public IList<Statuses> statuses { get; set; }

    }

    public class Application
    {
        public int platformUpdateDomain { get; set; }
        public int platformFaultDomain { get; set; }
        public string computerName { get; set; }
        public string osName { get; set; }
        public string osVersion { get; set; }
        public VmAgent vmAgent { get; set; }
        public IList<Disks> disks { get; set; }
        public BootDiagnostics bootDiagnostics { get; set; }
        public IList<Extensions> extensions { get; set; }
        public string hyperVGeneration { get; set; }
        public IList<Statuses> statuses { get; set; }

    }
}
