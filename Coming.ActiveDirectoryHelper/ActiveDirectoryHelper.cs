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
            string userDN = user.DistinguishedName;
            string encodedUserDN = Microsoft.Security.Application.Encoder.LdapFilterEncode(user.DistinguishedName);

            string searchFilter = $"(&(objectClass=group)(member={encodedUserDN}))";
            string[] attributes = { "name" };


            return LdapHelper.ConnectionWrapper(settings, connection => {

                string[] groupsDistinguishedNames = user.MemberOf;

                IList<string> groupsNames = new List<string>();

                ILdapSearchResults result = connection.Search(
                       settings.SearchBase,
                       LdapConnection.ScopeSub,
                       searchFilter,
                       attributes,
                       false
                   );

                while (result.HasMore())
                {
                    LdapEntry nextEntry = null;
                    try
                    {
                        nextEntry = result.Next();
                        groupsNames.Add(nextEntry.GetAttribute("name").StringValue);
                    }
                    catch (LdapException e)
                    {
                        //Exception is thrown, go for next entry
                        continue;
                    }


                }

                return groupsNames;
            });
        }

        public IEnumerable<ADHelperUser> GetMemberOfGroup(string groupName)
        {
            return LdapHelper.ConnectionWrapper(settings, connection => {

                string encodedGroupName = Microsoft.Security.Application.Encoder.LdapFilterEncode(groupName);
                string searchFilter = $"(&(objectClass=group)(name={encodedGroupName}))";

                ILdapSearchResults groupDNresult = connection.Search(
                    settings.SearchBase,
                    LdapConnection.ScopeSub,
                    searchFilter,
                    null,
                    false
                    );

                var res = groupDNresult.First();
                string groupDN = Microsoft.Security.Application.Encoder.LdapFilterEncode(res.Dn);

                searchFilter = $"(&(objectClass=user)(memberOf={groupDN}))";

                ILdapSearchResults usersResult = connection.Search(
                    settings.SearchBase,
                    LdapConnection.ScopeSub,
                    searchFilter,
                    null,
                    false
                    );

                searchFilter = $"(distinguishedName={settings.SearchBase})";
                string[] attributes = { "lockoutDuration" };

                ILdapSearchResults lockoutDurationResult = connection.Search(
                    settings.SearchBase,
                    LdapConnection.ScopeSub,
                    searchFilter,
                    attributes,
                    false
                );

                var lockoutDuratiotEntry = lockoutDurationResult.First();

                List<ADHelperUser> users = new List<ADHelperUser>();

                while (usersResult.HasMore())
                {
                    LdapEntry nextEntry = null;
                    try
                    {
                        nextEntry = usersResult.Next();
                        ADHelperUser user = new ADHelperUser();
                        var attrSet = nextEntry.GetAttributeSet();

                        foreach (var attr in attrSet)
                        {
                            user[attr.Name] = attr;
                        }

                        user["lockoutDuration"] = lockoutDuratiotEntry.GetAttribute("lockoutDuration");

                        users.Add(user);
                    }
                    catch (LdapException e)
                    {
                        //Exception is thrown, go for next entry
                        continue;
                    }
                }

                return users;
            });
        } 

        public ADHelperUser GetUserByAccountName(string sAMAccountName)
        {
            string encodedsAMAccountName = Microsoft.Security.Application.Encoder.LdapFilterEncode(sAMAccountName);
            string searchFilter = $"(&(objectClass=user)(sAMAccountName={encodedsAMAccountName}))";

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

                // get domen lockoutDuration,  

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

                string encodedUserDN = Microsoft.Security.Application.Encoder.LdapFilterEncode(distinguishedName);

                ldapConn.Connect(settings.ServerName, settings.ServerPort);

                ldapConn.Bind(encodedUserDN, password);

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
