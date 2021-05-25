using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coming.ActiveDirectoryHelper.Models
{
    public class ADHelperUser
    {
        public string Name { get; set; }
        public string DistinguishedName { get; set; }
        public string SamAccountName { get; set; }
        public string DisplayName { get; set; }
    }
}
