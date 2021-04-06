using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace MngVm.Constant
{
    public class LDAPContants
    {
        public static readonly string IpAddress = Convert.ToString(ConfigurationManager.AppSettings["ldpIpAddress"]);
        public static readonly string LDAPOU = Convert.ToString(ConfigurationManager.AppSettings["ldpOU"]);
        public static readonly string Domain1Name = Convert.ToString(ConfigurationManager.AppSettings["ldpdomain1"]);
        public static readonly string Domain2Name = Convert.ToString(ConfigurationManager.AppSettings["ldpdomain2"]);
        public static readonly string UserName = Convert.ToString(ConfigurationManager.AppSettings["ldpUsername"]);
        public static readonly string Password = Convert.ToString(ConfigurationManager.AppSettings["ldpPassword"]);
        public static readonly string SubAdminGroupName = Convert.ToString(ConfigurationManager.AppSettings["ldpSubAdminGroup"]);
        public static readonly string SubAdminGroupCN = Convert.ToString(ConfigurationManager.AppSettings["ldpSubAdminGroupCN"]);
    }
}