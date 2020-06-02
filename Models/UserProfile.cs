using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MngVm.Models
{
    public class UserProfile
    {

        public string id
        {
            get;
            set;
        }
        public string email
        {
            get;
            set;
        }
        public string name
        {
            get;
            set;
        }
        public string given_name
        {
            get;
            set;
        }
        public string family_name
        {
            get;
            set;
        }
        public string link
        {
            get;
            set;
        }
        public string picture
        {
            get;
            set;
        }
        public string gender
        {
            get;
            set;
        }
        public string locale
        {
            get;
            set;
        }

        public int rowId
        {
            get;
            set;
        }
        public List<MachineInfo> machineInfo
        {
            get;
            set;
        }
    }
}