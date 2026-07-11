using EDDY.IS.EmsLeadEngine.Core.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace EDDY.IS.EmsLeadEngine.Core.Extensions
{
    public class EnvironmentSettings
    {
        
        public static string SalesForceConsumerKey
        {
            get
            {
                return ConfigurationManager.AppSettings["SalesForceConsumerKey"];
            }
        }

        public static string SalesForceConsumerSecret
        {
            get
            {
                return ConfigurationManager.AppSettings["SalesForceConsumerSecret"];
            }
        }

        public static string SalesForceSecurityToken
        {
            get
            {
                return ConfigurationManager.AppSettings["SalesForceSecurityToken"];
            }
        }

        public static string SalesForceUsername
        {
            get
            {
                return ConfigurationManager.AppSettings["SalesForceUsername"];
            }
        }

        public static string SalesforcePassword
        {
            get
            {
                string hash = ConfigurationManager.AppSettings["SalesforcePassword"];
                //[ConsumerKey] [ConsumerSecret] [SecurityToken]
                hash = Security.Decrypt(hash, SalesForceConsumerKey, SalesForceConsumerSecret, SalesForceSecurityToken);
                return hash + SalesForceSecurityToken;
            }
        }

     
    }
}
