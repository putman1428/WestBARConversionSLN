using System;

namespace WestBARConversion
{
    public class PftProration
    {
        public string PftEncntrID { get; set; }
        public string HealthPlanID { get; set; }
        public decimal? CurrAmtDue { get; set; }
        public string PrioritySeq { get; set; }
        public string CurrAmtDueFlg { get; set; }
    }
}