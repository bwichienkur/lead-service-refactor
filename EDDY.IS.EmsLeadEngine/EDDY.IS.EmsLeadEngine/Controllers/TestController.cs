using EDDY.IS.EmsLeadEngine.Controllers.Base;
using EDDY.IS.EmsLeadEngine.Entities;
using EDDY.IS.EmsLeadEngine.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using EDDY.IS.EmsLeadEngine.Core;

namespace EDDY.IS.EmsLeadEngine.Controllers
{
    public class TestController : BaseController
    {
        static Core.EmsLeadEngine LeadEngine = new Core.EmsLeadEngine();


        [HttpPost]
        [Route("api/test/echo")]
       
        [AuthenticationFilter("D4484FF1-088E-4C3C-8365-B45745AE47A1", "79E209BC-666B-4660-B566-732B81FC9AA2")]
        public HttpResponseMessage Echo([FromBody] object request)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            httpResponseMessage.Content = new ObjectContent(request.GetType(), request, new JsonMediaTypeFormatter());
            LogManager.LogJournalInfo(Guid.Empty, "Echo Test method invoked", request);
            return httpResponseMessage;
        }


        
    }
}
