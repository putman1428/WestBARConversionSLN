using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace WestBARConversion
{
    public class NomenclatureDAL
    {
        public static string GetNomenclature_SourceIdentifier(string nomenclatureId)
        {
            DateTime todayDt = DateTime.Today;
            StringBuilder sb = new StringBuilder();
            sb.Append("Select source_identifier ");
            sb.Append("FROM BAR_Nomenclature ");
            sb.Append("where nomenclature_id = " + long.Parse(nomenclatureId) + " and active_ind = '1' ");
            sb.Append("AND END_EFFECTIVE_DT_TM > '" + todayDt + "'");

            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(GlobalSettings.SqlConnectionString()))
            using (var cmd = new SqlCommand(sb.ToString(), conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }
            string result = string.Empty;
            if (dt.Rows.Count > 0)
                result = dt.Rows[0][0].ToString();
            dt.Dispose();
            return result;
        }
    }
}