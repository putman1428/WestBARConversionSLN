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
    public class EncntrMotherChildDAL
    {
        public static string GetMotherAcctNumber(string encntrID, string orgID)
        {
            string result = "";
            DateTime todayDt = DateTime.Today;

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT  MOTHER_ENCNTR_ID ");
            sb.Append("FROM BAR_ENCNTR_MOTHER_CHILD_RELTN ");
            sb.Append("WHERE CHILD_ENCNTR_ID = " + long.Parse(encntrID) + " ");
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

            foreach (DataRow row in dt.Rows)
            {
                result = row["MOTHER_ENCNTR_ID"].ToString().Trim();
            }
            if (result != "")
            {
                result = Encounter_AliasDAL.GetEncounterVisitNumber(result);
                if (result != "")
                    result = orgID + result.PadLeft(10, '0');
            }
            dt.Dispose();
            return result;
        }
        
    }
}
