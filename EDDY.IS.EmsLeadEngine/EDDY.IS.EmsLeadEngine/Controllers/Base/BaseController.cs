using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using EDDY.IS.Base;
using EDDY.IS.Core.Logging;
using EDDY.IS.EmsLeadEngine.Entities;

namespace EDDY.IS.EmsLeadEngine.Controllers.Base
{
    public class BaseController : ApiController
    {

        public static void CreateErrorResponseMessage(HttpResponseMessage httpResponse, Request request, Exception ex)
        {
            Response response = new Response()
            {
                TransactionId = request.TransactionId,
                Success = false,
                Message = string.Format(Constants.ERR_GENERAL,ex.Message),
                Code = (int)Constants.ResponseCode.ERROR
            };
            httpResponse.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            httpResponse.Content = new ObjectContent(response.GetType(), response, new JsonMediaTypeFormatter());
        }
    }
}