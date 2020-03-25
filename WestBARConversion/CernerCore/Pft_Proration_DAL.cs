using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace WestBARConversion.CernerCore
{
    public class Pft_Proration_DAL
    {
        private static string today = DateTime.Today.ToString("yyyy-MM-dd");
        public static List<PftProration> GetPFTProration(string pftEncID)
        {
            DateTime todayDt = DateTime.Today;
            StringBuilder sb = new StringBuilder();
            sb.Append("Select health_plan_id,pft_encntr_id,priority_seq,curr_amt_due,curr_amount_dr_cr_flag from BAR_PFT_PRORATION ");
            sb.Append("where pft_encntr_id = " + long.Parse(pftEncID) + " and active_ind = '1' ");
            sb.Append("AND END_EFFECTIVE_DT_TM > '" + todayDt + "'");

            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(GlobalSettings.SqlConnectionString()))
            using (var cmd = new SqlCommand(sb.ToString(), conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }

            List<PftProration> prorations = new List<PftProration>();
            foreach (DataRow row in dt.Rows)
            {
                PftProration proration = LoadProration(row);
                if (proration != null)
                {
                    prorations.Add(proration);
                }

            }
            dt.Dispose();
            return prorations;
        }

        private static PftProration LoadProration(DataRow row)
        {
            PftProration proration = new PftProration();
            proration.CurrAmtDue = CernerCommon.ConvertDecimalString(row["curr_amt_due"].ToString());
            proration.CurrAmtDueFlg = row["curr_amount_dr_cr_flag"].ToString();
            proration.HealthPlanID = row["health_plan_id"].ToString();
            proration.PftEncntrID = row["pft_encntr_id"].ToString();
            proration.PrioritySeq = row["priority_seq"].ToString();
            return proration;
        }
        private static int GetEffDt(string dt)
        {
            string dtnum = Huron.HuronRoutines.FormatDate_YYYYMMDD(dt);
            if (Huron.HuronRoutines.IsNumeric(dtnum) == true)
                return int.Parse(dtnum);
            else
                return 0;
        }
    }
}
