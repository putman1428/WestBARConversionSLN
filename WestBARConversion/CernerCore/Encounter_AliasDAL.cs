using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WestBARConversion.CernerCore
{
    public class Encounter_AliasDAL
    {
        private static string today = DateTime.Today.ToString("yyyy-MM-dd");
        public static string GetEncounterVisitNumber(string encounterId)
        {
            DateTime todayDt = DateTime.Today;

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT  alias ");
            sb.Append("FROM  BAR_ENCNTR_ALIAS ");
            sb.Append("WHERE encntr_id = " + long.Parse(encounterId) + " ");
            sb.Append("AND encntr_alias_type_cd  = '1077' ");
            sb.Append("AND active_ind = '1' ");
            sb.Append("AND END_EFFECTIVE_DT_TM > '" + todayDt + "'");

            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(GlobalSettings.SqlConnectionString()))
            using (var cmd = new SqlCommand(sb.ToString(), conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.Text;
                da.Fill(dt);
            }
            string result = encounterId;
            if (dt.Rows.Count > 0)
                result = dt.Rows[0][0].ToString();
            else
                result = string.Empty;
            dt.Dispose();
            return result;
        }

    }
}
