using System.Collections.Generic;

namespace EDDY.IS.EmsLeadEngine.Core.Extensions
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Merges right dictionary into left, returns a bool if left was modified
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool Merge(this Dictionary<string,string> left, Dictionary<string,string> right)
        {
            bool ChangedResult = false;

            foreach(var item in right)
            {
                if(!left.ContainsKey(item.Key))
                {
                    left.Add(item.Key, item.Value);
                    ChangedResult = true;
                }
                else
                {
                    left[item.Key] = item.Value;
                    if(left[item.Key]!= item.Value)
                    {
                        ChangedResult = true;
                    }
                }
            }

            return ChangedResult;
        }
    }
}
