# Coming.ActiveDirectoryHelper

Helper library for accessing Active Directory, using Novell in background.

## How to use:

```csharp
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
```

