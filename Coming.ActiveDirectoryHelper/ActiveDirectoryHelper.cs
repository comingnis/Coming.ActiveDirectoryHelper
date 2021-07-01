using Coming.ActiveDirectoryHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Novell.Directory.Ldap;
using Coming.ActiveDirectoryHelper.Helpers;

namespace Coming.ActiveDirectoryHelper
{
    public class ActiveDirectoryHelper
    {
        private readonly ADHelperSettings settings;

        public ActiveDirectoryHelper(ADHelperSettings settings)
        {
            this.settings = settings;
        }

        public IEnumerable<string> GetAllGroups()
        {
            string searchFilter = "(objectClass=group)";
            string[] attributes = { "name" };

            return LdapHelper.ConnectionWrapper(settings, connection => {
                ILdapSearchResults result = connection.Search(
                    settings.SearchBase,
                    LdapConnection.ScopeSub,
                    searchFilter,
                    attributes,
                    false
                );

                List<string> groupNames = new List<string>();

                while (result.HasMore())
                {
                    LdapEntry nextEntry = null;
                    try
                    {
                        nextEntry = result.Next();
                    }
                    catch (LdapException e)
                    {
                        //Exception is thrown, go for next entry
                        continue;
                    }

                    groupNames.Add(nextEntry.GetAttribute("name").StringValue);
                }

                return groupNames;
            });
        }

        public IEnumerable<string> GetGroupsForUser(ADHelperUser user)
        {
            return LdapHelper.ConnectionWrapper(settings, connection => {

                string[] attributes = { "name" };
                string[] groupsDistinguishedNames = user.MemberOf;

                IList<string> groupsNames = new List<string>();

                for (int i = 0; i < groupsDistinguishedNames.Length; i++)
                {
                    string searchFilter = $"(&(objectClass=group)(distinguishedName={groupsDistinguishedNames[i]}))";

                    ILdapSearchResults result = connection.Search(
                           settings.SearchBase,
                           LdapConnection.ScopeSub,
                           searchFilter,
                           attributes,
                           false
                       );

                    try
                    {
                        var name = result.First();
                        groupsNames.Add(name.GetAttribute("name").StringValue);
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }

                }

                return groupsNames;
            });
        }

        public IEnumerable<string> GetMemberOfGroup(string groupName)
        {
            return LdapHelper.ConnectionWrapper(settings, connection => {

                string searchFilter = $"(&(objectClass=group)(name={groupName}))";

                ILdapSearchResults groupDNresult = connection.Search(
                    settings.SearchBase,
                    LdapConnection.ScopeSub,
                    searchFilter,
                    null,
                    false
                    );

                var res = groupDNresult.First();
                string groupDN = res.Dn;

                searchFilter = $"(&(objectClass=user)(memberOf={groupDN}))";
                string[] attributes = { "sAMAccountName" };

                ILdapSearchResults accountNamesresult = connection.Search(
                    settings.SearchBase,
                    LdapConnection.ScopeSub,
                    searchFilter,
                    attributes,
                    false
                    );

                List<string> accountNames = new List<string>();

                while (accountNamesresult.HasMore())
                {
                    LdapEntry nextEntry = null;
                    try
                    {
                        nextEntry = accountNamesresult.Next();
                    }
                    catch (LdapException e)
                    {
                        //Exception is thrown, go for next entry
                        continue;
                    }

                    accountNames.Add(nextEntry.GetAttribute("sAMAccountName").StringValue);
                }

                return accountNames;
            });
        }

        public ADHelperUser GetUserByAccountName(string sAMAccountName)
        {
            string searchFilter = $"(&(objectClass=user)(sAMAccountName={sAMAccountName}))";

            return LdapHelper.ConnectionWrapper(settings, connection => {

                // get user from AD, by sAMAccountName
                ILdapSearchResults userResult = connection.Search(
                        settings.SearchBase,
                        LdapConnection.ScopeSub,
                        searchFilter,
                        null,
                        false
                    );

                searchFilter = $"(distinguishedName={settings.SearchBase})";
                string[] attributes = { "lockoutDuration" };

                // get domnen lockoutDuration,  

                ILdapSearchResults lockoutDurationResult = connection.Search(
                        settings.SearchBase,
                        LdapConnection.ScopeSub,
                        searchFilter,
                        attributes,
                        false
                    );

                try
                {
                    ADHelperUser user = new ADHelperUser();
                    var firstUser = userResult.First();
                    var attrSet = firstUser.GetAttributeSet();

                    foreach (var attr in attrSet)
                    {
                        user[attr.Name] = attr;
                    }

                    var firstLockoutDuratiotEntry = lockoutDurationResult.First();
                    user["lockoutDuration"] = firstLockoutDuratiotEntry.GetAttribute("lockoutDuration");

                    return user;
                }
                catch (Exception ex)
                {
                    return null;
                }
            });
        }

        public bool ValidateCredential(string distinguishedName, string password)
        {
            try
            {
                LdapConnection ldapConn = new LdapConnection();

                ldapConn.Connect(settings.ServerName, settings.ServerPort);

                ldapConn.Bind(distinguishedName, password);

                ldapConn.Disconnect();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
