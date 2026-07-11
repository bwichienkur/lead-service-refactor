using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.EmsLeadEngine.Core.DataModel
{
    public class VW_EmailIntegrationInstitutions
    {
        [Key]
        public int InstitutionId { get; set; }
        public string InstitutionName { get; set; }
        public int ExternalIntegrationId { get; set; }

    }
}
