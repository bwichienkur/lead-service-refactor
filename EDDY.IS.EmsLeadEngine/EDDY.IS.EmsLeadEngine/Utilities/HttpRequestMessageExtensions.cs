using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;

namespace EDDY.IS.EmsLeadEngine.Utilities
{
    public static class HttpRequestMessageExtensions
    {
        public static NameValueCollection GetRequestQueryParameters(this HttpRequestMessage request)
        {
            return HttpUtility.ParseQueryString(request.RequestUri.Query);
        }

        public static NameValueCollection GetRequestJsonBodyParameters(this HttpRequestMessage request)
        {
            NameValueCollection nameValueCollection = null;
            string bodyString = request.Content.ReadAsStringAsync().Result;

            //using (var stream = new MemoryStream())
            //{
            //    var context = (HttpContextBase)request.Properties["MS_HttpContext"];
            //    context.Request.InputStream.Seek(0, SeekOrigin.Begin);
            //    context.Request.InputStream.CopyTo(stream);
            //    bodyString = Encoding.UTF8.GetString(stream.ToArray());
            //}
            //bodyString = request.Content.ReadAsStringAsync().Result;

            if (!String.IsNullOrEmpty(bodyString))
            {
                nameValueCollection = new NameValueCollection();
                Dictionary<string, object> bodyDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(bodyString);
                foreach (KeyValuePair<string, object> kvp in bodyDictionary)
                {
                    if (kvp.Value != null)
                    {
                        nameValueCollection.Add(kvp.Key.ToString(), kvp.Value.ToString());
                    }
                }

            }

            return nameValueCollection;

        }



    }
}