using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MngVm.Models
{
    public class User
    {
        public int id { get; set; }
        public string FirstName { get; set; } //givenName 
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string UPNName { get; set; }
        public string UPNSuffix { get; set; }
        public string Password { get; set; }
        public bool PassNverExpire { get; set; }
        public string name { get; set; }
        public string objectCategory { get; set; }
        public string objectClass { get; set; }
        public string profilePath { get; set; }
        public string sAMAccountName { get; set; }
        public string title { get; set; }
        public string c { get; set; }
        public string Ou { get; set; }
        public string HostPoolName { get; set; }
        public string VmName { get; set; }
    }
}