using EDDY.IS.EmsLeadEngine.Core.DataModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.EmsLeadEngine.Tests
{
    [TestClass]
    public class CacheDataServiceRulesFixture
    {
        [TestMethod]
        public void CacheDataServiceRules_InstitutionAllowsRoutingToSalesFource_ShouldReturnTrueForInstitutionsWithoutRules()
        {
            //Arrange
            ServiceRules _cacheDataServiceRules = new ServiceRules();
            IList<VW_ContractInstitutionsRules> contractRules = new List<VW_ContractInstitutionsRules>();
            contractRules.Add(new VW_ContractInstitutionsRules() { InstitutionId = 50, ContractEntityId = null, Value = null });
            contractRules.Add(new VW_ContractInstitutionsRules() { InstitutionId = 42, ContractEntityId = 2, Value = "Accelerated Science Courses" });
            contractRules.Add(new VW_ContractInstitutionsRules() { InstitutionId = 37, ContractEntityId = 5, Value = "1" });
            contractRules.Add(new VW_ContractInstitutionsRules() { InstitutionId = 37, ContractEntityId = null, Value = null });
            contractRules.Add(new VW_ContractInstitutionsRules() { InstitutionId = 28, ContractEntityId = 5, Value = "2" });
            VW_ISLead lead = new VW_ISLead();
            lead.InstitutionId = 20;

            //Act
            bool allowsRoutingToSF = _cacheDataServiceRules.InstitutionAllowsRoutingToSalesFource(lead);

            //Assert
            Assert.IsTrue(allowsRoutingToSF);
        }

        [TestMethod]
        public void CacheDataServiceRules_InstitutionAllowsRoutingToSalesFource_ShouldReturnTrueForInstitutionsWithNonCampusTypeRules()
        {
            //Arrange
            ServiceRules _cacheDataServiceRules = new ServiceRules();
            IList<VW_ContractInstitutionsRules> contractRules = new List<VW_ContractInstitutionsRules>();
            contractRules.Add(new VW_ContractInstitutionsRules() { InstitutionId = 50, ContractEntityId = null, Value = null });
            contractRules.Add(new VW_ContractInstitutionsRules() { InstitutionId = 42, ContractEntityId = 2, Value = "Accelerated Science Courses" });
            contractRules.Add(new VW_ContractInstitutionsRules() { InstitutionId = 37, ContractEntityId = 5, Value = "1" });
            contractRules.Add(new VW_ContractInstitutionsRules() { InstitutionId = 37, ContractEntityId = null, Value = null });
            contractRules.Add(new VW_ContractInstitutionsRules() { InstitutionId = 28, ContractEntityId = 5, Value = "2" });
            VW_ISLead lead = new VW_ISLead();
            lead.InstitutionId = 42;

            //Act
            bool allowsRoutingToSF = _cacheDataServiceRules.InstitutionAllowsRoutingToSalesFource(lead);

            //Assert
            Assert.IsTrue(allowsRoutingToSF);
        }

        [TestMethod]
        public void CacheDataServiceRules_InstitutionAllowsRoutingToSalesFource_ShouldReturnTrueForInstitutionsWithACampusTypeRuleThatMatchesInstitutionCampusTypeId()
        {
            //Arrange
            ServiceRules _cacheDataServiceRules = new ServiceRules();
            IList<VW_ContractInstitutionsRules> contractRules = new List<VW_ContractInstitutionsRules>();
            contractRules.Add(new VW_ContractInstitutionsRules() { InstitutionId = 50, ContractEntityId = null, Value = null });
            contractRules.Add(new VW_ContractInstitutionsRules() { InstitutionId = 42, ContractEntityId = 2, Value = "Accelerated Science Courses" });
            contractRules.Add(new VW_ContractInstitutionsRules() { InstitutionId = 37, ContractEntityId = 5, Value = "1" });
            contractRules.Add(new VW_ContractInstitutionsRules() { InstitutionId = 37, ContractEntityId = null, Value = null });
            contractRules.Add(new VW_ContractInstitutionsRules() { InstitutionId = 28, ContractEntityId = 5, Value = "2" });
            VW_ISLead lead = new VW_ISLead();
            lead.InstitutionId = 37;
            lead.CampusTypeId = 1;

            //Act
            bool allowsRoutingToSF = _cacheDataServiceRules.InstitutionAllowsRoutingToSalesFource(lead);

            //Assert
            Assert.IsTrue(allowsRoutingToSF);
        }

        [TestMethod]
        public void CacheDataServiceRules_InstitutionAllowsRoutingToSalesFource_ShouldReturnFalseForInstitutionsWithACampusTypeRuleThatDoesNotMatchInstitutionCampusTypeId()
        {
            //Arrange
            ServiceRules _cacheDataServiceRules = new ServiceRules();
            IList<VW_ContractInstitutionsRules> contractRules = new List<VW_ContractInstitutionsRules>();
            contractRules.Add(new VW_ContractInstitutionsRules() { InstitutionId = 50, ContractEntityId = null, Value = null });
            contractRules.Add(new VW_ContractInstitutionsRules() { InstitutionId = 42, ContractEntityId = 2, Value = "Accelerated Science Courses" });
            contractRules.Add(new VW_ContractInstitutionsRules() { InstitutionId = 10, ContractEntityId = 5, Value = "1" });
            contractRules.Add(new VW_ContractInstitutionsRules() { InstitutionId = 37, ContractEntityId = null, Value = null });
            contractRules.Add(new VW_ContractInstitutionsRules() { InstitutionId = 28, ContractEntityId = 5, Value = "1" });
            VW_ISLead lead = new VW_ISLead();
            lead.InstitutionId = 10;
            lead.CampusTypeId = 2;

            //Act
            bool allowsRoutingToSF = _cacheDataServiceRules.InstitutionAllowsRoutingToSalesFource(lead);

            //Assert
            Assert.IsFalse(allowsRoutingToSF);
        }
    }
}
