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
            //ADHelperSettings settings = new ADHelperSettings();

            //settings.BindDistinguishName = "CN=Maja Mitrovic,OU=Users_BEout,DC=bgdel,DC=local";
            //settings.BindPassword = "4hNMxjr";
            //settings.SearchBase = "DC=bgdel,DC=local";
            //settings.ServerName = "10.1.50.110";
            //settings.ServerPort = 389;

            //ActiveDirectoryHelper helper = new ActiveDirectoryHelper(settings);

            //ADHelperUser user = helper.GetUserByAccountName("jasmina.micunovic");
            //var groups = helper.GetGroupsForUser(user);
            //var result = helper.GetMemberOfGroup("BE_Sava.06a HPV (video nadzor)");
            //OU = BE_Sava.06a HPV(video nadzor),OU = BE_Sava,OU = korisnici_,DC = bgdel,DC = local
            //BE_Sava.06a HPV(video nadzor)
            //var testSaZagradama = helper.TestZagrada("OU = BE_Sava.06a HPV(video nadzor),OU = BE_Sava,OU = korisnici_,DC = bgdel,DC = local");

            //helper.ValidateCredential("CN=Maja Mitrovic,OU=Users_BEout,DC=bgdel,DC=local", "4hNMxjr");

            ADHelperSettings settings = new ADHelperSettings();

            settings.ServerName = "10.234.0.10";
            settings.ServerPort = 389;
            settings.BindDistinguishName = "CN=Nikola Nikolic,CN=Users,DC=glasnik,DC=local";
            settings.BindPassword = "Nikol@.123";
            settings.SearchBase = "DC=glasnik,DC=local";

            ActiveDirectoryHelper helper = new ActiveDirectoryHelper(settings);

            ADHelperUser user = helper.GetUserByAccountName("predragg()");
            //bool isAccountExpired = user.IsUserExpired();
            //bool isLO = user.IsAccountLockedOut();

            //var result = helper.GetMemberOfGroup("Zagrade(Test)");

            //var groups = helper.GetGroupsForUser(user);

            //foreach(var g in groups)
            //{
            //    var members = helper.GetMemberOfGroup(g);
            //}

        }
    }
}
