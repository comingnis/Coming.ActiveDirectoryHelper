﻿using Novell.Directory.Ldap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coming.ActiveDirectoryHelper.Models
{
    public class ADHelperUser
    {
        public ADHelperUser()
        {
            Attributes = new Dictionary<string, LdapAttribute>();
        }

        private Dictionary<string, LdapAttribute> Attributes;
        public string Name
        {
            get
            {
                return this["name"]?.StringValue;
            }
        }
        public string DistinguishedName
        {
            get
            {
                return this["distinguishedName"]?.StringValue;
            }
        }
        public string SamAccountName
        {
            get
            {
                return this["sAMAccountName"]?.StringValue;
            }
        }
        public string DisplayName
        {
            get
            {
                return this["displayName"]?.StringValue;
            }
        }
        public string GivenName
        {
            get
            {
                return this["givenName"]?.StringValue;
            }
        }
        public string Surname
        {
            get
            {
                return this["sn"]?.StringValue;
            }
        }
        public string EmailAddress
        {
            get
            {
                return this["mail"]?.StringValue;
            }
        }

        public LdapAttribute this[string attributeName]
        {
            get => GetAttributeValueByAttributeName(attributeName);
            set => SetAttributeValueByAttributeName(attributeName, value);
        }

        private void SetAttributeValueByAttributeName(string attributeName, LdapAttribute attribute)
        {
            Attributes.Add(attributeName, attribute);
        }

        private LdapAttribute GetAttributeValueByAttributeName(string attributeName)
        {
            LdapAttribute attributeValue = null;
            Attributes.TryGetValue(attributeName, out attributeValue);

            return attributeValue;
        }

        public bool IsUserExpired()
        {
            var attrPasswordNeverExpires = this["userAccountControl"];

            if (attrPasswordNeverExpires != null)
            {
                if (attrPasswordNeverExpires.StringValueArray.Contains("66048"))
                {
                    return false;
                }
            }

            var attrAccountExpirationDate = this["accountExpires"];

            if (attrAccountExpirationDate != null)
            {
                long ticks = long.Parse(attrAccountExpirationDate.StringValue);
                if (ticks == 0 || ticks == 9223372036854775807)
                {
                    return false;
                }

                var offset = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow);
                System.DateTime dtDateTime = new DateTime(1601, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                dtDateTime = dtDateTime + offset;

                long maxDateTimeTicks = DateTime.MaxValue.Ticks - dtDateTime.Ticks;
                if (maxDateTimeTicks < ticks)
                {
                    return false;
                }

                var expirationDate = dtDateTime.AddTicks(ticks);

                if (expirationDate < DateTime.Today)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
