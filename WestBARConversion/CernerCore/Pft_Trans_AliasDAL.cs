using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WestBARConversion.CernerModel;

namespace WestBARConversion.CernerCore
{
    public class Pft_Trans_AliasDAL
    {
        public static string GetTransAlias(string aliasID)
        {
            string today = DateTime.Today.ToString("yyyyMMdd");
            int cnt = 0;
            DateTime todayDt = DateTime.Today;
            string returnVal = "";
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT  pft_trans_alias,END_EFFECTIVE_DT_TM ");
            sb.Append("FROM  BAR_PFT_TRANS_ALIAS ");
            sb.Append("WHERE pft_trans_alias_id = " + long.Parse(aliasID) + " ");
            sb.Append("AND active_ind = '1' ");
            //sb.Append("AND END_EFFECTIVE_DT_TM > '" + todayDt + "'");

            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(GlobalSettings.SqlConnectionString()))
            using (var cmd = new SqlCommand(sb.ToString(), conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.Text;
                da.Fill(dt);
            }
            foreach (DataRow row in dt.Rows)
            {
                if (GetEffDt(dt.Rows[cnt]["end_effective_dt_tm"].ToString().Trim()) > int.Parse(today))
                {
                    returnVal = dt.Rows[cnt][0].ToString();
                    break;
                }
                cnt = cnt + 1;
            }

            dt.Dispose();
            return returnVal;
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
