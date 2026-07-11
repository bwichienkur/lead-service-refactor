using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EDDY.IS.EmsLeadEngine.Core;
using EDDY.IS.EmsLeadEngine.Entities;
using EDDY.IS.EmsLeadEngine.Core.DataModel;

namespace EDDY.IS.EmsLeadEngine.Tests
{
    [TestClass]
    public class EMSLeadEngineTest
    {
        public const int EMS_INSTITUTIONID = 23;
        public const string EMS_INSTITUTION_SFTOKEN = "0012100000YGCotAAH";
        public const int EMS_PROGRAMPRODUCTID = 256667;        
        static Core.EmsLeadEngine LeadEngine = new Core.EmsLeadEngine();

        [TestMethod]
        public void TestCreateFromSalesforce()
        {
            SalesforceLeadCreateRequest req = new SalesforceLeadCreateRequest()
            {
                TransactionId = Guid.NewGuid(),
                Lead = new Entities.Common.SalesforceLead()
                {
                    Address1 = "ABC Street",
                    Address2 = "address part2",
                    City = "Wellington",
                    CountryCode = "US",
                    SalesforceId = "00Qf200001a4RQ5EAM",
                    EducationLevelId = 1,
                    FirstName = "MyFirstName",
                    LastName = "MyLastName",
                    Email = "thisisasample@email.com",
                    InstitutionSalesforceId = EMS_INSTITUTION_SFTOKEN,
                    Phone1 = "5612043299",
                    Phone2 = "5612043299",
                    PostalCode = "33414",
                    StateProvince = "FL",
                    CurrentStatus = Constants.STATUS_NEW,
                    CurrentState = Constants.STATE_OPEN
                },
                AdditionalQuestions = new System.Collections.Generic.Dictionary<string, string>()
            };

            req.AdditionalQuestions.Add("Question1", "Answer1");
            req.AdditionalQuestions.Add("Question2", "Answer2");
            req.AdditionalQuestions.Add("Question3", "Answer3");
            req.AdditionalQuestions.Add("Question4", "Answer4");

            var result = LeadEngine.CreateFromSalesforce(req);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.Code == 1);
        }

        [TestMethod]
        public void TestCreateISLead()
        {
            CacheDataService.Initialize();
            ISLeadCreateRequest createLeadRequest = new ISLeadCreateRequest();
            createLeadRequest.ISLeadIds = new System.Collections.Generic.List<int>();
            createLeadRequest.ISLeadIds.Add(36469869);
            createLeadRequest.TransactionId = Guid.NewGuid();

            var result = LeadEngine.CreateFromIS(createLeadRequest);         
        }

    }
}
