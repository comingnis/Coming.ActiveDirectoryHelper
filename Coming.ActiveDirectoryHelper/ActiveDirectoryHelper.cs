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

        public async Task<IEnumerable<string>> GetAllGroups()
        {
            string searchFilter = "(objectClass=group)";
            string[] attributes = { "name" };


            return await LdapHelper.ConnectionWrapper(settings, async connection => {
                ILdapSearchResults result = await connection.SearchAsync(
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
            }).Result;
        }

        public async Task<IEnumerable<string>> GetGroupsForUser(ADHelperUser user)
        {
            string searchFilter = $"(&(objectClass=group)(member={user.DistinguishedName}))";
            string[] attributes = { "name" };

            return await LdapHelper.ConnectionWrapper(settings, async connection => {

                ILdapSearchResults result = await connection.SearchAsync(
                       settings.SearchBase,
                       LdapConnection.ScopeSub,
                       searchFilter,
                       attributes,
                       false
                   );

                IList<string> groupNames = new List<string>();

                while (result.HasMore())
                {
                    LdapEntry nextEntry = null;
                    try
                    {
                        //result.
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
            }).Result;
        }

        public async Task<IEnumerable<string>> GetMemberOfGroup(string groupName)
        {
            return await LdapHelper.ConnectionWrapper(settings, async connection => {

                string searchFilter = $"(&(objectClass=group)(name={groupName}))";

                ILdapSearchResults groupDNresult = await connection.SearchAsync(
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

                ILdapSearchResults accountNamesresult = await connection.SearchAsync(
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
            }).Result;

        }

        public async Task<ADHelperUser> GetUserByAccountName(string sAMAccountName)
        {
            string searchFilter = $"(&(objectClass=user)(sAMAccountName={sAMAccountName}))";

            return await LdapHelper.ConnectionWrapper(settings, async connection => {
                ILdapSearchResults result = await connection.SearchAsync(
                        settings.SearchBase,
                        LdapConnection.ScopeSub,
                        searchFilter,
                        null,
                        false
                    );
                try
                {
                    ADHelperUser user = new ADHelperUser();
                    var fres = result.First();

                    user.Name = fres.GetAttribute("name").StringValue;
                    user.DistinguishedName = fres.Dn;
                    user.SamAccountName = fres.GetAttribute("sAMAccountName").StringValue;
                    user.DisplayName = fres.GetAttribute("displayName").StringValue;

                    return user;
                }
                catch (Exception ex)
                {
                    return null;
                }

            }).Result;
        }

        public async Task<bool> ValidateCredential(string distinguishedName, string password)
        {
            try
            {
                LdapConnection ldapConn = new LdapConnection();

                await ldapConn.ConnectAsync(settings.ServerName, settings.ServerPort);

                await ldapConn.BindAsync(distinguishedName, password);

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
