using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.EmsLeadEngine.Entities.Common
{
    public class ProgramStateSyncRuleStateZip
    {
        [Key]
        public int ProgramStateSyncRuleStateID { get; set; }
        public string ZipCode { get; set; }
        public bool IsEnabled { get; set; }
    }
}
