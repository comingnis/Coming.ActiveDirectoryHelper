using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coming.ActiveDirectoryHelper.Models
{
    public class ADHelperSettings
    {
        public string ServerName { get; set; }
        public int ServerPort { get; set; } = 389;
        public string SearchBase { get; set; }
        public string BindDistinguishName { get; set; }
        public string BindPassword { get; set; }
    }
}
