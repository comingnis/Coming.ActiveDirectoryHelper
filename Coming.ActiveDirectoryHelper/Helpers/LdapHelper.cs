using Coming.ActiveDirectoryHelper.Models;
using Novell.Directory.Ldap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Coming.ActiveDirectoryHelper.Helpers
{
    internal static class LdapHelper
    {
        public static T ConnectionWrapper<T>(ADHelperSettings aDHelperSettings, Func<LdapConnection, T> func)
        {
            LdapConnection ldapConn = new LdapConnection();

            try
            {
                ldapConn.Connect(aDHelperSettings.ServerName, aDHelperSettings.ServerPort);

                ldapConn.Bind(aDHelperSettings.BindDistinguishName, aDHelperSettings.BindPassword);

                var result = func(ldapConn);

                return result;
            }
            catch (Exception ex)
            {
                ldapConn.Disconnect();
                ldapConn.Dispose();
                Console.WriteLine(ex);
                return default;
            }
            finally
            {
                ldapConn.Disconnect();
                ldapConn.Dispose();
            }
        }

        public static void ConnectionWrapper(ADHelperSettings aDHelperSettings, Action<LdapConnection> func)
        {
            LdapConnection ldapConn = new LdapConnection();

            try
            {
                ldapConn.Connect(aDHelperSettings.ServerName, aDHelperSettings.ServerPort);

                ldapConn.Bind(aDHelperSettings.BindDistinguishName, aDHelperSettings.BindPassword);

               func(ldapConn);
            }
            catch (Exception ex)
            {
                ldapConn.Disconnect();
                ldapConn.Dispose();
                Console.WriteLine(ex);
            }
            finally
            {
                ldapConn.Disconnect();
                ldapConn.Dispose();
            }
        }
    }
}
