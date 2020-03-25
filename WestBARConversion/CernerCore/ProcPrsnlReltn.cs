using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WestBARConversion.CernerCore
{
    public class ProcPrsnlReltn
    {
        public static string GetSurgicalPhysician(string procedureId)
        {
            DateTime todayDt = DateTime.Today;
            string result = "";
            string procPrsnlCd = "";
            string phy = "";
            string asstPhy = "";
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT  PRSNL_PERSON_ID, proc_prsnl_reltn_cd ");
            sb.Append("FROM  BAR_PROC_PRSNL_RELTN ");
            sb.Append("WHERE procedure_id = " + long.Parse(procedureId) + " ");
            sb.Append("AND proc_prsnl_reltn_cd = '1209' ");
            sb.Append(" OR proc_prsnl_reltn_cd = '1347951279'");
            sb.Append(" AND END_EFFECTIVE_DT_TM > '" + todayDt + "'");

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
                procPrsnlCd = row["proc_prsnl_reltn_cd"].ToString();
                if(procPrsnlCd.Trim() == "1209")
                    phy = row["PRSNL_PERSON_ID"].ToString();
                else if(procPrsnlCd.Trim() == "1347951279")
                    asstPhy = row["PRSNL_PERSON_ID"].ToString();
            }
            result = phy + "|" + asstPhy;
            dt.Dispose();
            return result;
        }
    }
}