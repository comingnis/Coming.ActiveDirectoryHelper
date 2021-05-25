using Coming.ActiveDirectoryHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Novell.Directory.Ldap;

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
            int searchScope = LdapConnection.ScopeSub;
            string[] attributes = { "name" };

            LdapConnection ldapConn = new LdapConnection();

            await ldapConn.ConnectAsync(settings.ServerName, settings.ServerPort);

            await ldapConn.BindAsync(settings.BindDistinguishName, settings.BindPassword);

            ILdapSearchResults result = await ldapConn.SearchAsync(
                        settings.SearchBase,
                        searchScope,
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

                // Console.WriteLine(nextEntry.Dn + "\n" + nextEntry.GetAttribute("sAMAccountName") + "\n\n");
                groupNames.Add(nextEntry.GetAttribute("name").StringValue);
            }

            return groupNames;
        }

        public async Task<IEnumerable<string>> GetGroupsForUser(ADHelperUser user)
        {
            int searchScope = LdapConnection.ScopeSub;
            string searchFilter = $"(&(objectClass=group)(member={user.DistinguishedName}))";
            string[] attributes = { "name" };

            LdapConnection ldapConn = new LdapConnection();

            await ldapConn.ConnectAsync(settings.ServerName, settings.ServerPort);

            await ldapConn.BindAsync(settings.BindDistinguishName, settings.BindPassword);

            ILdapSearchResults result = await ldapConn.SearchAsync(
                        settings.SearchBase,
                        searchScope,
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
        }

        public async Task<IEnumerable<string>> GetMemberOfGroup(string groupName)
        {
            try
            {
                LdapConnection ldapConn = new LdapConnection();

                await ldapConn.ConnectAsync(settings.ServerName, settings.ServerPort);

                await ldapConn.BindAsync(settings.BindDistinguishName, settings.BindPassword);

                #region get distinguishedName of group

                string searchFilter = $"(&(objectClass=group)(name={groupName}))";

                ILdapSearchResults groupDNresult = await ldapConn.SearchAsync(
                    settings.SearchBase,
                    LdapConnection.ScopeSub,
                    searchFilter,
                    null,
                    false
                    );

                var res = groupDNresult.First();
                string groupDN = res.Dn;

                #endregion

                #region get memebers account names

                searchFilter = $"(&(objectClass=user)(memberOf={groupDN}))";
                string[] attributes = { "sAMAccountName" };

                ILdapSearchResults accountNamesresult = await ldapConn.SearchAsync(
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

                #endregion region
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<ADHelperUser> GetUserByAccountName(string sAMAccountName)
        {
            string searchFilter = $"(&(objectClass=user)(sAMAccountName={sAMAccountName}))";
            int searchScope = LdapConnection.ScopeSub;

            LdapConnection ldapConn = new LdapConnection();

            await ldapConn.ConnectAsync(settings.ServerName, settings.ServerPort);

            await ldapConn.BindAsync(settings.BindDistinguishName, settings.BindPassword);

            ILdapSearchResults result = await ldapConn.SearchAsync(
                        settings.SearchBase,
                        searchScope,
                        searchFilter,
                        null,
                        false
                    );

            try
            {
                // ici ce neko mapiranje umesto ovog dela koda

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
        }

        public async Task<bool> ValidateCredential(string distinguishedName, string password)
        {
            try
            {
                LdapConnection ldapConn = new LdapConnection();

                await ldapConn.ConnectAsync(settings.BindDistinguishName, settings.ServerPort);

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
