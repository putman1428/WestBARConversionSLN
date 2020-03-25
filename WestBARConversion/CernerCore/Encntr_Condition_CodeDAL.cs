using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace WestBARConversion.CernerCore
{
    public class Encntr_Condition_CodeDAL
    {
        public static List<string> GetConditionCodes(string encounterId)
        {
            DateTime todayDt = DateTime.Today;
            string[] items;
            StringBuilder sb = new StringBuilder();
            sb.Append("select condition_cd from BAR_Encntr_Condition_Code ");
            sb.Append("where encntr_Id = " + long.Parse(encounterId) + " and active_ind = '1' ");
            sb.Append("AND END_EFFECTIVE_DT_TM > '" + todayDt + "'");
            sb.Append(" order by sequence  ");
            
            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(GlobalSettings.SqlConnectionString()))
            using (var cmd = new SqlCommand(sb.ToString(), conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }

            List<string> condCodes = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                string code = row["condition_cd"].ToString();
                code = CodeValueDAL.GetCodeValueDisplay(code.Trim());
                items = code.Split(' ');
                condCodes.Add(items[0].Trim());
            }
            return condCodes;
        }
    }
}

