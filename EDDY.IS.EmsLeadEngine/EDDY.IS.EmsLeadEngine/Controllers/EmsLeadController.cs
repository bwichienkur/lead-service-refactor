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
using System.Web;

namespace EDDY.IS.EmsLeadEngine.Controllers
{
    public class EmsLeadController : BaseController
    {
        static Core.EmsLeadEngine LeadEngine = new Core.EmsLeadEngine();


        [HttpPost]
        [Route("api/emslead/createfromis")]
        [AuthenticationFilter("FF64C3AE-4776-4802-B1AA-9FCF99EA0B5E", "79E209BC-666B-4660-B566-732B81FC9AA2")]
        public HttpResponseMessage CreateFromIS([FromBody] ISLeadCreateRequest request)
         {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            try
            {
                LogManager.LogJournalInfo(request.TransactionId, "CreateFromIS Request", request);
                var Result = LeadEngine.CreateFromIS(request);//48383571
                LogManager.LogJournalInfo(request.TransactionId, "CreateFromIS Result", Result);
                httpResponseMessage.Content = new ObjectContent(Result.GetType(), Result, new JsonMediaTypeFormatter());
            }
            catch(Exception ex)
            {
                LogManager.LogJournalException(request.TransactionId, ex, request);
                CreateErrorResponseMessage(httpResponseMessage, request, ex);
            }

            return httpResponseMessage;
        }


        [HttpPost]
        [AuthenticationFilter("1E3A4C37-9AF4-407B-B226-B260D59B4DD1", "79E209BC-666B-4660-B566-732B81FC9AA2")]
        [Route("api/emslead/processfromdataexchange")]
        public HttpResponseMessage ProcessFromDataExchange([FromBody] ExchangeMultipleLeadProcessRequest request)
        {
            MultipleResponse responseList = new MultipleResponse();
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            try
            {
                LogManager.LogJournalInfo(request.TransactionId, "ProcessFromDataExchange Request", request);
                var Result = LeadEngine.ProcessFromDataExchange(request);
                LogManager.LogJournalInfo(request.TransactionId, "ProcessFromDataExchange Result", Result);
                httpResponseMessage.Content = new ObjectContent(Result.GetType(), Result, new JsonMediaTypeFormatter());
            }
            catch (Exception ex)
            {
                LogManager.LogJournalException(request.TransactionId, ex, request);
                CreateErrorResponseMessage(httpResponseMessage, request, ex);
            }

            return httpResponseMessage;
        }


        [Route("api/emslead/updatefromsalesforce")]
        [AuthenticationFilter("D4484FF1-088E-4C3C-8365-B45745AE47A1", "79E209BC-666B-4660-B566-732B81FC9AA2")]
        [Obsolete("UpdateFromSalesforce Method is deprecated. Please use the UpdateFromSalesforceById method.", true)]
        public HttpResponseMessage UpdateFromSalesforce([FromBody] SalesforceLeadUpdateRequest request)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.Gone);
            return httpResponseMessage;
        }


        [Route("api/emslead/updatefromsalesforcebyid")]
        [AuthenticationFilter("D4484FF1-088E-4C3C-8365-B45745AE47A1", "79E209BC-666B-4660-B566-732B81FC9AA2")]
        public HttpResponseMessage UpdateFromSalesforceById([FromBody] SalesforceLeadUpdateRequest request)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            try
            {
                LogManager.LogJournalInfo(request.TransactionId, "UpdateFromSalesforceById Request", request);
                var Result = LeadEngine.UpdateFromSalesforceById(request);
                //LogManager.LogJournalInfo(request.TransactionId, "UpdateFromSalesforceById Result", Result); // Logged in async result
                httpResponseMessage.Content = new ObjectContent(Result.GetType(), Result, new JsonMediaTypeFormatter());
            }
            catch (Exception ex)
            {
                LogManager.LogJournalException(request.TransactionId, ex, request);
                CreateErrorResponseMessage(httpResponseMessage, request, ex);
            }

            return httpResponseMessage;
        }

        [Route("api/emslead/createfromsalesforce")]
        [AuthenticationFilter("2812670B-343C-41CF-988C-E1924EA4C958", "79E209BC-666B-4660-B566-732B81FC9AA2")]
        public HttpResponseMessage CreateFromSalesforce([FromBody] SalesforceLeadCreateRequest request)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            try
            {
                LogManager.LogJournalInfo(request.TransactionId, "CreateFromSalesforce Request", request);
                var Result = LeadEngine.CreateFromSalesforce(request);
                //LogManager.LogJournalInfo(request.TransactionId, "CreateFromSalesforce Result", Result); // Logged in async result
                httpResponseMessage.Content = new ObjectContent(Result.GetType(), Result, new JsonMediaTypeFormatter());
            }
            catch (Exception ex)
            {
                LogManager.LogJournalException(request.TransactionId, ex, request);
                CreateErrorResponseMessage(httpResponseMessage, request, ex);
            }

            return httpResponseMessage;
        }

        [Route("api/emslead/bulksavelegacygpleads")]
        [AuthenticationFilter("2812670B-343C-41CF-988C-E1924EA4C958", "79E209BC-666B-4660-B566-732B81FC9AA2")]
        public HttpResponseMessage BulkSavelegacyGPleads([FromBody] LegacyGPBulkLeadSaveRequest request)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            try
            { 
                LogManager.LogJournalInfo(request.TransactionId, "BulkSavelegacyGPleads Request", request);
                var Result = LeadEngine.BulkSaveFromlegacyGP(request);
                LogManager.LogJournalInfo(request.TransactionId, "BulkSavelegacyGPleads Result", Result);
                httpResponseMessage.Content = new ObjectContent(Result.GetType(), Result, new JsonMediaTypeFormatter());
            }
            catch (Exception ex)
            {
                LogManager.LogJournalException(request.TransactionId, ex, request);
                CreateErrorResponseMessage(httpResponseMessage, request, ex);
            }

            return httpResponseMessage;
        }

        [Route("api/emslead/unsubscribebyid")]
        [HttpGet]
        public HttpResponseMessage UnsubscribeBySalesforceId(string salesforceId)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            Guid transactionId = Guid.NewGuid();
            try
            {
                
                LogManager.LogJournalInfo(transactionId, "UnsubscribeBySalesforceId Request", salesforceId);
                var Result = LeadEngine.UnsubscribeBySalesforceId(transactionId, salesforceId);
                LogManager.LogJournalInfo(transactionId, "UnsubscribeBySalesforceId Result", Result);
                httpResponseMessage.Content = new ObjectContent(Result.GetType(), Result, new JsonMediaTypeFormatter());
            }
            catch (Exception ex)
            {
                LogManager.LogJournalException(transactionId, ex, salesforceId);
                httpResponseMessage.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            }

            return httpResponseMessage;
        }

        [Route("api/emslead/unsubscribe")]
        [HttpGet]
        public HttpResponseMessage Unsubscribe(string email)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            Guid transactionId = Guid.NewGuid();
            try
            {

                LogManager.LogJournalInfo(transactionId, "Unsubscribe Request", email);
                var Result = LeadEngine.Unsubscribe(transactionId, email);
                LogManager.LogJournalInfo(transactionId, "Unsubscribe Result", Result);
                httpResponseMessage.Content = new ObjectContent(Result.GetType(), Result, new JsonMediaTypeFormatter());
            }
            catch (Exception ex)
            {
                LogManager.LogJournalException(transactionId, ex, email);
                httpResponseMessage.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            }

            return httpResponseMessage;
        }

        [Route("api/emslead/unsubscribebyemsleadid")]
        [HttpGet]
        public HttpResponseMessage UnsubscribeByEMSLeadId(int emsLeadId)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            Guid transactionId = Guid.NewGuid();
            try
            {

                LogManager.LogJournalInfo(transactionId, "UnsubscribeByEMSLeadId Request", emsLeadId);
                var Result = LeadEngine.UnsubscribeByEMSLeadId(transactionId, emsLeadId);
                LogManager.LogJournalInfo(transactionId, "UnsubscribeByEMSLeadId Result", Result);
                httpResponseMessage.Content = new ObjectContent(Result.GetType(), Result, new JsonMediaTypeFormatter());
            }
            catch (Exception ex)
            {
                LogManager.LogJournalException(transactionId, ex, emsLeadId);
                httpResponseMessage.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            }

            return httpResponseMessage;
        }

        [Route("api/emslead/emailstrategyexhausted")]
        [HttpPost]
        public HttpResponseMessage EmailStrategyExhausted()
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            Guid transactionId = Guid.NewGuid();
            string salesforceId = string.Empty;
            try
            {
                string request = HttpUtility.UrlDecode(Request.Content.ReadAsStringAsync().Result);

                Dictionary<string, string> fields = request.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries)
                                                           .Select(part => part.Split('='))
                                                           .ToDictionary(split => split[0], split => split[1]);

                salesforceId = fields["contact[fields][salesforceid]"];

                LogManager.LogJournalInfo(transactionId, "EmailStrategyExhausted Request", salesforceId);
                if (!string.IsNullOrEmpty(salesforceId))
                {
                    var Result = LeadEngine.UpdateEmailStrategyExhausted(transactionId, salesforceId);
                    LogManager.LogJournalInfo(transactionId, "EmailStrategyExhausted Result", Result);
                    httpResponseMessage.Content = new ObjectContent(Result.GetType(), Result, new JsonMediaTypeFormatter());
                }
            }
            catch (Exception ex)
            {
                LogManager.LogJournalException(transactionId, ex, salesforceId);
                httpResponseMessage.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            }

            return httpResponseMessage;
        }
    }
}
