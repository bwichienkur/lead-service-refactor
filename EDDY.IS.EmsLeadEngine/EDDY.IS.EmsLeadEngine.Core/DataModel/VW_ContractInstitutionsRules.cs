using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.EmsLeadEngine.Core.DataModel
{
    public partial class VW_ContractInstitutionsRules
    {
        [Key]
        public Int64 Id { get; set; }
        
        public int InstitutionId { get; set; }

        
        public int? ContractEntityId { get; set; }

        [StringLength(200)]
        public string Value { get; set; }
    }
}
