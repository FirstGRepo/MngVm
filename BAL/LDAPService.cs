using System;
using System.DirectoryServices;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;
using System.Threading;
using System.Globalization;
using System.Collections;
using MngVm.Models;
using Microsoft.Azure.Management.KeyVault.Fluent;
using MngVm.Constant;
using System.DirectoryServices.AccountManagement;
using System.Runtime.InteropServices.WindowsRuntime;
using System.DirectoryServices.ActiveDirectory;

namespace MngVm.BAL
{
    public class LDAPService
    {
        static string ipAddress = LDAPContants.IpAddress;
        static string domain1Name = LDAPContants.Domain1Name;
        static string domain2Name = LDAPContants.Domain2Name;
        static string userName = LDAPContants.UserName;
        static string password = LDAPContants.Password;

        DirectoryEntry _directoryEntry;
        public LDAPService()
        {
            _directoryEntry = GetDirectoryEntry();
        }

        public static void SetCultureAndIdentity()
        {
            AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
            WindowsPrincipal principal = (WindowsPrincipal)Thread.CurrentPrincipal;
            WindowsIdentity identity = (WindowsIdentity)principal.Identity;
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        }

        public static DirectoryEntry GetDirectoryEntry()
        {
            return GetDirectoryEntry($"LDAP://{ipAddress}/OU=UAT;DC={domain1Name};DC={domain2Name}");
        }

        public static DirectoryEntry GetDirectoryEntry(string Path)
        {
            DirectoryEntry de = new DirectoryEntry();
            de.Path = Path;
            //de.AuthenticationType = AuthenticationTypes.Secure;
            de.Username = $@"og\{userName}";
            de.Password = $"{password}";
            return de;
        }

        public List<User> GetAllUsers()
        {

            SearchResultCollection results;
            DirectorySearcher ds = null;

            List<User> userList = new List<User>(); ;
            string[] ouArr = LDAPContants.LDAPOU.Split(",");
            foreach (var ou in ouArr)
            {
                var _usersList = GetDirectoryEntry($"LDAP://{ipAddress}/OU={ou};DC={domain1Name};DC={domain2Name}");
                ds = new DirectorySearcher(_usersList)
                {
                    Filter = "(&(objectCategory=User)(objectClass=person))"
                };

                results = ds.FindAll();
                if (results.Count > 0)
                {
                    foreach (SearchResult sr in results)
                    {
                        User user = new User
                        {
                            FirstName = sr.Properties["givenname"].Count > 0 ? Convert.ToString(sr.Properties["givenname"][0]) : string.Empty,
                            LastName = sr.Properties["sn"].Count > 0 ? Convert.ToString(sr.Properties["sn"][0]) : string.Empty,
                            UserName = sr.Properties["userprincipalname"].Count > 0 ? Convert.ToString(sr.Properties["userprincipalname"][0]) : string.Empty,
                            Ou = ou,
                        };
                        userList.Add(user);
                    }
                }
            }
            return userList;
        }

        public string CreateNewUser(User user)
        {
            string retVal;
            try
            {
                if (!UserExists(user.UserName))
                {
                    //DirectoryEntry de = GetDirectoryEntry($"LDAP://{ipAddress}/OU={user.Ou};DC={domain1Name};DC={domain2Name}");
                    ///// 1. Create user account
                    //DirectoryEntries users = de.Children;
                    //DirectoryEntry newuser = users.Add("CN=" + user.FirstName, "user");
                    ///// 2. Set properties

                    //SetProperty(newuser, "givenname", user.FirstName);
                    //SetProperty(newuser, "sn", user.LastName);
                    //SetProperty(newuser, "SAMAccountName", user.UserName);
                    //SetProperty(newuser, "userPrincipalName", user.UserName);

                    //// SetProperty(newuser, "mail", email);
                    //newuser.CommitChanges();
                    ///// 3. Set password
                    //SetPassword(newuser.Path, user.Password);
                    //newuser.CommitChanges();
                    ///// 4. Enable account
                    //EnableAccount(newuser);
                    ///// 5. Add user account to groups
                    //// AddUserToGroup(de, newuser, "");

                    //newuser.Close();
                    //de.Close();

                    string stringDomainName = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
                    PrincipalContext domainContext = new PrincipalContext(ContextType.Domain, stringDomainName, "OU=" + user.Ou + ";DC=" + domain1Name + ";DC=" + domain2Name);
                    UserPrincipal newUser = new UserPrincipal(domainContext)
                    {
                        UserPrincipalName = user.UserName + "@" + user.UPNSuffix,
                        SamAccountName = user.UserName.Length > 20 ? user.UserName.Substring(0, 20) : user.UserName,
                        Name = user.FirstName,
                        GivenName = user.FirstName,
                        Surname = user.LastName,
                        PasswordNeverExpires = true,
                        Enabled = true,
                    };
                    newUser.SetPassword(user.Password);
                    newUser.Save();
                    retVal = "1|User Added Successfuly";
                }
                else
                    retVal = "2|User Already Exist";
            }
            catch (Exception ex)
            {
                retVal = "0|" + ex.Message;// Error While Adding User";
            }

            return retVal;
        }

        public void AddUserToGroup(DirectoryEntry de, DirectoryEntry deUser, string GroupName)
        {
            DirectorySearcher deSearch = new DirectorySearcher();
            deSearch.SearchRoot = de;
            deSearch.Filter = "(&(objectClass=group) (cn=" + GroupName + "))";


            SearchResultCollection results = deSearch.FindAll();
            bool isGroupMember = false;
            if (results.Count > 0)
            {
                DirectoryEntry group = GetDirectoryEntry(results[0].Path);
                object members = group.Invoke("Members", null);
                foreach (object member in (IEnumerable)members)
                {
                    DirectoryEntry x = new DirectoryEntry(member);

                    if (!x.Name.Equals(deUser.Name))
                    {
                        isGroupMember = false;
                    }
                    else
                    {
                        isGroupMember = true;
                        break;
                    }
                }
                if (!isGroupMember)
                {
                    group.Invoke("Add", new object[] { deUser.Path.ToString() });
                }
                group.Close();
            }
            return;
        }

        public void CreateNewUser(string employeeID, string name, string password, string login, string email, string group)
        {
            if (UserExists(name))
            {
                //Catalog catalog = new Catalog();
                DirectoryEntry de = GetDirectoryEntry();
                /// 1. Create user account
                DirectoryEntries users = de.Children;
                DirectoryEntry newuser = users.Add("CN=" + login, "user");
                /// 2. Set properties
                SetProperty(newuser, "employeeID", employeeID);
                SetProperty(newuser, "givenname", name);
                SetProperty(newuser, "SAMAccountName", login);
                SetProperty(newuser, "userPrincipalName", login);
                SetProperty(newuser, "mail", email);
                newuser.CommitChanges();
                /// 3. Set password
                SetPassword(newuser.Path, password);
                newuser.CommitChanges();
                /// 4. Enable account
                EnableAccount(newuser);
                /// 5. Add user account to groups
                AddUserToGroup(de, newuser, group);

                newuser.Close();
                de.Close();
            }
        }

        public void SetProperty(DirectoryEntry de, string PropertyName, string PropertyValue)
        {
            if (PropertyValue != null)
            {
                if (de.Properties.Contains(PropertyName))
                {
                    de.Properties[PropertyName][0] = PropertyValue;

                }
                else
                {
                    de.Properties[PropertyName].Add(PropertyValue);
                }
            }
        }

        public void SetPassword(string path, string passwordToSet)
        {
            DirectoryEntry usr = new DirectoryEntry();
            usr.Path = path;
            usr.AuthenticationType = AuthenticationTypes.Secure;
            object[] password = new object[] { passwordToSet };
            object ret = usr.Invoke("SetPassword", password);
            usr.CommitChanges();
            usr.Close();
        }

        private void EnableAccount(DirectoryEntry de)
        {
            int old_UAC = (int)de.Properties["userAccountControl"][0];
            // AD user account disable flag
            int ADS_UF_ACCOUNTDISABLE = 2;

            // To enable an ad user account, we need to clear the disable bit/flag:
            de.Properties["userAccountControl"][0] = (old_UAC & ~ADS_UF_ACCOUNTDISABLE);

            // TO set password never expires = true
            int NON_EXPIRE_FLAG = 0x10000;
            int val = (int)de.Properties["userAccountControl"].Value;
            de.Properties["userAccountControl"].Value = val | NON_EXPIRE_FLAG;
            de.CommitChanges();
        }

        public bool UserExists(string UserName)
        {

            //DirectoryEntry de = GetDirectoryEntry();
            //DirectorySearcher deSearch = new DirectorySearcher();
            //deSearch.SearchRoot = de;
            //deSearch.Filter = "(&(objectClass=user) (cn=" + UserName + "))";
            //SearchResultCollection results = deSearch.FindAll();
            //if (results.Count == 0)
            //{
            //    return false;
            //}
            //else
            //{
            //    return true;
            //}
            string stringDomainName = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
            using (var ctx = new PrincipalContext(ContextType.Domain, stringDomainName))
            {
                using (var user = UserPrincipal.FindByIdentity(ctx, IdentityType.UserPrincipalName, UserName))
                {
                    if (user != null)
                        return true;
                    else
                        return false;
                }
            }

        }

        public bool ChnagePassword(string username, string password)
        {
            //using (PrincipalContext pc = new PrincipalContext(contextType, domain))
            //{
            //    using (UserPrincipal userPrincipal = UserPrincipal.FindByIdentity(pc, username))
            //    {
            //        if (userPrincipal != null)
            //        {
            //            try { userPrincipal.ChangePassword(oldPassword, newPassword); }
            //            catch (PasswordException pe) { return null; }
            //        }
            //    }
            //}

            bool retval = false;
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);
            UserPrincipal user = UserPrincipal.FindByIdentity(ctx, username);
            if (user != null)
            {
                user.SetPassword(password);
                retval = true;
            }
            return retval;


        }

        public bool DeleteUser(string username)
        {
            bool retval = false;
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);
            UserPrincipal user = UserPrincipal.FindByIdentity(ctx, username);
            if (user != null)
            {
                user.Delete();
                retval = true;
            }
            return retval;
        }

        public bool DisableUser(string username)
        {
            bool retval = false;
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);
            UserPrincipal user = UserPrincipal.FindByIdentity(ctx, username);

            if (user != null)
            {
                user.Enabled = false;
                retval = true;
            }
            return retval;
        }

        public List<string> GetUPNSuffixes()
        {
            //add root domain
            //DirectoryEntry partitions = new DirectoryEntry($"LDAP://{ipAddress}/DC={domain1Name};DC={domain2Name}", userName, password);
            //DirectorySearcher searcher = new DirectorySearcher(partitions);
            //searcher.PropertiesToLoad.Add("uPNSuffixes");

            //List<string> suffixes = new List<string>();

            //foreach (SearchResult sr in searcher.FindAll())
            //{
            //    foreach (string pn in sr.Properties.PropertyNames)
            //    {
            //        if (pn == "upnsuffixes")
            //        {
            //            suffixes.Add(sr.Properties[pn].ToString());
            //        }
            //    }
            //}

            //return suffixes;

            List<string> suffixList = new List<string>();
            suffixList.Add(Domain.GetCurrentDomain().Name);
            DirectoryEntry rootDSE = new DirectoryEntry("LDAP://RootDSE");
            string context = rootDSE.Properties["configurationNamingContext"].Value.ToString();
            DirectoryEntry partition = new DirectoryEntry(@"LDAP://CN=Partitions," + context);

            foreach (string suffix in partition.Properties["uPNSuffixes"])
            {
                suffixList.Add(suffix);
            }

            return suffixList;
        }
    }

}
