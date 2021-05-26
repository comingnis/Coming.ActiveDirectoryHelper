using Coming.ActiveDirectoryHelper;
using Coming.ActiveDirectoryHelper.Models;
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

            //while (true)
            //{
            //    IEnumerable<string> groupsForUser = await helper.GetGroupsForUser(user);
            //    Console.WriteLine(groupsForUser.Count());
            //}

            //bool correct = await helper.ValidateCredential("CN=Nikola Nikolic,CN=Users,DC=glasnik,DC=local", "Nikol@.123");
            ADHelperUser user = await helper.GetUserByAccountName("monique");
            IEnumerable<string> groupsForUser = await helper.GetGroupsForUser(user);
            IEnumerable<string> accountNames = await helper.GetMemberOfGroup("DMS_Test");
            IEnumerable<string> groups = await helper.GetAllGroups();

            foreach (var group in groups)
            {
                Console.WriteLine(group + "\n");
            }
        }
    }
}
