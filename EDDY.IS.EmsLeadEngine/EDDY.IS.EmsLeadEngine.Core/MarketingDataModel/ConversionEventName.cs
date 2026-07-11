using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.EmsLeadEngine.Core.DataModel
{

    [Table("Google.ConversionEventName")]
    public partial class ConversionEventName
    {
        public int ConversionEventNameId { get; set; }

        public int ConversionId { get; set; }
        public int ConversionAPIEventId { get; set; }
        public string Name { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsEnabled { get; set; }
    }
}
