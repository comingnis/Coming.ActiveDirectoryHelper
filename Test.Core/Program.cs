using Coming.ActiveDirectoryHelper;
using Coming.ActiveDirectoryHelper.Models;
using System;
using System.Collections.Generic;

namespace Test.Core
{
    class Program
    {
        static void Main(string[] args)
        {
            ADHelperSettings settings = new ADHelperSettings();

            settings.ServerName = "10.234.0.10";
            settings.ServerPort = 389;
            settings.BindDistinguishName = "CN=Nikola Nikolic,CN=Users,DC=glasnik,DC=local";
            settings.BindPassword = "Nikol@.123";
            settings.SearchBase = "DC=glasnik,DC=local";

            ActiveDirectoryHelper helper = new ActiveDirectoryHelper(settings);

            //ADHelperUser user = helper.GetUserByAccountName("comingbk");
            //bool isAccountExpired = user.IsUserExpired();
            //bool isLO = user.IsAccountLockedOut();

            var usersFromGroup = helper.GetMemberOfGroup("DMS_Test");
        }
    }
}
