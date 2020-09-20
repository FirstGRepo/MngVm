using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MngVm.Models
{
    public class AzureVMCPU
    {
        public string VmName { get; set; }
        public double Average5min { get; set; }
        public double Average1Hr { get; set; }
        public double Average6Hr { get; set; }
        public double Average24Hr { get; set; }
        public double Peak24Hr { get; set; }
    }


    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);  
    public class Name
    {
        public string value { get; set; }
        public string localizedValue { get; set; }
    }

    public class Datum
    {
        public DateTime timeStamp { get; set; }
        public double average { get; set; }
    }

    public class Timeseries
    {
        public List<object> metadatavalues { get; set; }
        public List<Datum> data { get; set; }
    }

    public class Value
    {
        public string id { get; set; }
        public string type { get; set; }
        public Name name { get; set; }
        public string displayDescription { get; set; }
        public string unit { get; set; }
        public List<Timeseries> timeseries { get; set; }
        public string errorCode { get; set; }
    }

    public class AzureCPUUsage
    {
        public int cost { get; set; }
        public string timespan { get; set; }
        public string interval { get; set; }
        public List<Value> value { get; set; }
        public string @namespace { get; set; }
        public string resourceregion { get; set; }
    }


}