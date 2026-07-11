using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.EmsLeadEngine.Entities;

namespace EDDY.IS.EmsLeadEngine.Core.Extensions
{
    public static class ValidationExtensions
    {
        public static string Merge(String source, String other)
        {
            return string.IsNullOrWhiteSpace(source) ? other : source;
        }

        public static string Update(String source, String other)
        {
            return string.IsNullOrWhiteSpace(source) ? other : (source.Trim() != other.Trim()) ? other : source;
        }

        public static bool IsTestLead(string emailAddress)
        {
            bool isTestLead = false;

            if (!String.IsNullOrWhiteSpace(emailAddress))
            {
                if (emailAddress.ToLower().Contains(Constants.TEST_LEAD_EMAIL_STRING))
                    isTestLead = true;
            }              

            return isTestLead;
        }
        
        public static string SetValueToNullIfEmpty(string value)
        {
            return value == string.Empty ? null : value;
        }
    }
}
