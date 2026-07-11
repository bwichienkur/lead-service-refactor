using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.EmsLeadEngine.Core.DataModel
{

    [Table("Google.Conversion")]
    public partial class Conversion
    {
        public int ConversionId { get; set; }

        public string GoogleConversionId { get; set; }
        public string ConversionName { get; set; }

        public string ConversionLabel { get; set; }

        public string DefaultUrl { get; set; }
        public string DefaultUserAgent { get; set; }
        public int LookbackDays { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsEnabled { get; set; }
    }
}
