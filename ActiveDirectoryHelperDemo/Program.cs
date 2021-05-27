using Coming.ActiveDirectoryHelper;
using Coming.ActiveDirectoryHelper.Models;
using Novell.Directory.Ldap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveDirectoryHelperDemo
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            ADHelperSettings settings = new ADHelperSettings();
            settings.ServerName = "10.234.0.10";
            settings.ServerPort = 389;
            settings.BindDistinguishName = "CN=Nikola Nikolic,CN=Users,DC=glasnik,DC=local";
            settings.BindPassword = "Nikol@.123";
            settings.SearchBase = "DC=glasnik,DC=local";

            ActiveDirectoryHelper helper = new ActiveDirectoryHelper(settings);

            bool correct = helper.ValidateCredential("CN=Nikola Nikolic,CN=Users,DC=glasnik,DC=local", "Nikol@.123");
            ADHelperUser user = helper.GetUserByAccountName("jova.jokic");
            IEnumerable<string> groupsForUser = helper.GetGroupsForUser(user);
            IEnumerable<string> accountNames = helper.GetMemberOfGroup("DMS_Test");
            IEnumerable<string> groups = helper.GetAllGroups();

            foreach (var group in groups)
            {
                Console.WriteLine(group + "\n");
            }

            string accName = user.SamAccountName;
            string email = user.EmailAddress;
            string surname = user.Surname;

            bool isAccountExpired = user.IsUserExpired();

        }
    }
}
