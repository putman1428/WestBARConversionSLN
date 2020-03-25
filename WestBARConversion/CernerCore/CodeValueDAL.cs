using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace WestBARConversion.CernerCore
{
    public class CodeValueDAL
    {
        public static string GetCodeValueDisplay(string codeValue)
        {
            if (codeValue.Trim() == "")
                return "";
            DateTime todayDt = DateTime.Today;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT Display FROM BAR_CODE_VALUE ");
            sb.Append("WHERE Code_Value = " + long.Parse(codeValue) + " ");
            sb.Append("AND active_ind = '1' ");
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
            return result;
        }

        public static string GetCodeValueDisplay_Key(string codeValue)
        {
            DateTime todayDt = DateTime.Today;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT Display_Key FROM BAR_CODE_VALUE ");
            sb.Append("WHERE Code_Value = " + long.Parse(codeValue) + " ");
            sb.Append("AND active_ind = '1' ");
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
            return result;
        }
    }
}
