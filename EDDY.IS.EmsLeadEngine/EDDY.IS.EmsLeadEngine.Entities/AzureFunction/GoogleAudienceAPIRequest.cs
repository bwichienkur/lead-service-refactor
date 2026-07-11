using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.EmsLeadEngine.Entities.AzureFunction
{
    public class GoogleAudienceAPIRequest : BaseAPIRequest
    {
        public long? ISLeadId { get; set; }
        public long? EMSLeadId { get; set; }
        public int InstitutionId { get; set; }
        public int? CampusTypeId { get; set; }
        public int? CampusId { get; set; }
        public int? ProgramId { get; set; }
        public int? StatusId { get; set; }
        public int? SubStatusId { get; set; }
        public int? ProgramLevelId { get; set; }
    }
}
