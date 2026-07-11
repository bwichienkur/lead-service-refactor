using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.EmsLeadEngine.Entities.Common
{
    public class ProgramStateSyncRuleState
    {
        [Key]
        public int ProgramStateSyncRuleStateID { get; set; }
        public int ProgramStateSyncRuleID { get; set; }
        public int StateID { get; set; }
        public bool IsEnabled { get; set; }
        public bool ZipLevelSyncEnabled { get; set; }
    }
}
