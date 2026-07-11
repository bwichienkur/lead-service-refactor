using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EDDY.IS.EmsLeadEngine.Core
{
    public static class DataUtils
    {
        public static string ParsePhone(string phone)
        {
            string phoneReturn = null;

            if (!String.IsNullOrWhiteSpace(phone))
            {
                phoneReturn = new String(phone.Where(Char.IsDigit).ToArray());

                //Added fix for cases when phone contains no digits
                if (phoneReturn.Any() && phoneReturn.Substring(0, 1) == "1")
                    phoneReturn = phoneReturn.Substring(1);
            }             

            return phoneReturn;
        }

        public static Dictionary<string, string> ParseAdditionalFieldsXml(string xml)
        {

            Dictionary<string, string> result = null;

            try
            {
                var fields = new Dictionary<string, string>();
                var xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(xml);

                foreach (XmlNode node in xmlDocument.DocumentElement.FirstChild.ChildNodes)
                {
                    var fieldName = node.Attributes["FieldName"].Value;
                    var fieldValue = node.Attributes["Value"].Value;
                    fields.Add(fieldName, fieldValue);
                }

                result = fields;
            }
            catch (Exception)
            {
                result = new Dictionary<string, string>();
            }


            return result;
        }
    }
}
