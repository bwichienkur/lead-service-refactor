using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EDDY.IS.EmsLeadEngine.Core
{
    public static class HtmlRemoval
    {
        private static Regex _htmlRegex = new Regex("<.*?>", RegexOptions.Compiled);

        public static string StripHtmlTags(string source)
        {
            return string.IsNullOrWhiteSpace(source) ? source : _htmlRegex.Replace(source, string.Empty);
        }
    }
}
