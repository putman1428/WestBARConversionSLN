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
    public class CodingDAL
    {
        private static string today = DateTime.Today.ToString("yyyy-MM-dd");
        public static string GetCoding(string encounter_id)
        {
            DateTime todayDt = DateTime.Today;
            string result = "";
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT completed_dt_tm ");
            sb.Append("FROM BAR_CODING ");
            sb.Append("WHERE encntr_id = " + long.Parse(encounter_id) + " AND ACTIVE_IND = '1' ");
            sb.Append("AND END_EFFECTIVE_DT_TM > '" + todayDt + "'");

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
                result = row["completed_dt_tm"].ToString().Trim();
                break;
            }
            return result;
        }
      
    }
}