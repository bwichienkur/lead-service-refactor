using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EDDY.IS.EmsLeadEngine.Entities.Constants;

namespace EDDY.IS.EmsLeadEngine.Entities.Common
{
    public class ProgramStateSyncRule
    {
        [Key]
        public int ProgramStateSyncRuleID { get; set; }
        public int InstitutionID { get; set; }
        public int ProgramID { get; set; }
        public int RuleTypeID { get; set; }   // DB column
        public ProgramStateSyncRuleType RuleType
            => (ProgramStateSyncRuleType)RuleTypeID;  // mapped RuleTypeId to enum
       
        public bool IsEnabled { get; set; }
    }
}
