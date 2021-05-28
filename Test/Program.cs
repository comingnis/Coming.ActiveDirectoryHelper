using Coming.ActiveDirectoryHelper;
using Coming.ActiveDirectoryHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public class Program
    {
        static void Main(string[] args)
        {
            ADHelperSettings settings = new ADHelperSettings();

            settings.BindDistinguishName = "Some DN which will be used for accessing AD.";
            settings.BindPassword = "password";
            settings.SearchBase = "Search base DN";
            settings.ServerName = "IP address or server name";
            settings.ServerPort = 389; // Server port. Default is 389.

            ActiveDirectoryHelper helper = new ActiveDirectoryHelper(settings);

            bool correct = helper.ValidateCredential("DN", "password");
            ADHelperUser user = helper.GetUserByAccountName("john.smith");
            IEnumerable<string> groupsForUser = helper.GetGroupsForUser(user);
            IEnumerable<string> accountNames = helper.GetMemberOfGroup("GroupName");
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
