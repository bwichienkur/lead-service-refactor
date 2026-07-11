using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.EmsLeadEngine.Core.DataModel
{
    [Table("FB.PixelEventName")]
    public class PixelEventName
    {
        public int PixelEventNameId { get; set; }

        public int PixelId { get; set; }
        public int ConversionAPIEventId { get; set; }
        public string Name { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsEnabled { get; set; }
    }
}
