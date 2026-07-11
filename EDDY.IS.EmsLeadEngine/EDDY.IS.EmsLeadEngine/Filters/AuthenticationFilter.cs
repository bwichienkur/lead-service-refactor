using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Net.Http.Formatting;
using System.Net;
using EDDY.IS.EmsLeadEngine.Entities;
using System.Collections.Specialized;
using EDDY.IS.EmsLeadEngine.Utilities;
using EDDY.IS.EmsLeadEngine.Core;
using System.Text;
using Newtonsoft.Json;

namespace EDDY.IS.EmsLeadEngine.Filters
{
    public class AuthenticationFilter : ActionFilterAttribute
    {
        private List<string> AuthenticationTokenList { get; set; }
        public AuthenticationFilter(params string[] Tokens)
        {
            AuthenticationTokenList = new List<string>(Tokens);
            AuthenticationTokenList = AuthenticationTokenList.ConvertAll(i => i.ToUpper());
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            bool AuthenticationError = true;
            NameValueCollection requestValues = null;

            if (actionContext.Request.Method.Method == "POST" && actionContext.ActionArguments.Count >= 1 && isRequestCompressed(actionContext))
            {
                if (actionContext.ActionArguments.ContainsKey("request"))
                {
                    actionContext.Request.Content.Headers.Clear();
                    actionContext.Request.Content = new StringContent(JsonConvert.SerializeObject(actionContext.ActionArguments["request"]), Encoding.UTF8, "application/json");
                }
            }

            switch (actionContext.Request.Method.Method)
            {
                case "GET":
                    requestValues = actionContext.Request.GetRequestQueryParameters();
                    break;
                case "POST":
                    requestValues = actionContext.Request.GetRequestJsonBodyParameters();
                    break;
            }
            string Token = string.Empty;

            //if requestvalues is null. the content was serialized and read. Lets get it from the action arguments.
            if (requestValues == null)
            {
                if (actionContext.ActionArguments.Count >= 1)
                {
                    if (actionContext.ActionArguments.ContainsKey("request"))
                    {
                        Request requestActionArguments = actionContext.ActionArguments["request"] as Request;
                        if (requestActionArguments != null)
                        {
                            Token = requestActionArguments.AuthenticationToken.ToString();
                        }
                    }
                }
            }
            else
            {
                Token = requestValues["AuthenticationToken"];
            }


            if (!string.IsNullOrEmpty(Token) && AuthenticationTokenList.FindIndex(i => i == Token.ToUpper()) > -1)
            {
                AuthenticationError = false;
            }

            if (AuthenticationError)
            {
                HttpResponseMessage invalidHttpResponseMessage = new HttpResponseMessage(HttpStatusCode.Unauthorized);

                Response response = new Response();
                response.Code = (int)Constants.ResponseCode.AUTHENTICATION_ERROR;
                response.Message = Constants.ERR_AUTHENTICATION;
                response.Success = false;
                Guid TransactionId;
                if (Guid.TryParse(requestValues["TransactionId"], out TransactionId))
                {
                    response.TransactionId = TransactionId;
                }
                invalidHttpResponseMessage.Content = new ObjectContent(response.GetType(), response, new JsonMediaTypeFormatter());
                actionContext.Response = invalidHttpResponseMessage;

                LogManager.LogJournalException(TransactionId, new Exception(response.Message), Token);
            }

        }



        private bool isRequestCompressed(HttpActionContext message)
        {
            foreach (var encoding in message.Request.Content.Headers.ContentEncoding)
            {
                if (encoding.Equals("gzip", StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }
    }
}