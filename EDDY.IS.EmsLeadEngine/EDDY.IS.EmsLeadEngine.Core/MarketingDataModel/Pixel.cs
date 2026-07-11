using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.EmsLeadEngine.Core.DataModel
{
    [Table("FB.Pixel")]
    public class Pixel
    {
        public int PixelId { get; set; }

        [StringLength(20)]
        public string FBPixelId { get; set; }
        public string PixelName { get; set; }

        public int TokenId { get; set; }

        [StringLength(50)]
        public string PixelTestEventCode { get; set; }
        public bool UseTestEventCode { get; set; }
        public string DefaultUrl { get; set; }
        public string DefaultUserAgent { get; set; }
        public int LookbackDays { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsEnabled { get; set; }
        public string LimitedDataUsageParam { get; set; }
    }
}
