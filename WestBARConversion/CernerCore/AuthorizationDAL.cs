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
    public class AuthorizationDAL
    {
        public static List<Authorization> GetAuthorizations(string encounter_id, string health_plan_id, int authNbr)
        {
            DateTime todayDt = DateTime.Today;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT  auth_nbr, CERT_STATUS_CD, [AUTH_OBTAINED_DT_TM], AUTH_EXPIRE_DT_TM ");
            sb.Append("FROM  [BAR_AUTHORIZATION] ");
            sb.Append("WHERE encntr_id = " + long.Parse(encounter_id) + " AND health_plan_id = " + long.Parse(health_plan_id) + "");
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
            List<Authorization> auths = new List<Authorization>();
            foreach (DataRow row in dt.Rows)
            {
                Authorization auth = LoadAuthorization(row);
                auths.Add(auth);
            }
            
            dt.Dispose();
            return auths;
        }
        private static Authorization LoadAuthorization(DataRow row)
        {
            string authnbr = "";
            string authsts = "";
            Authorization auth = new Authorization();
            authnbr = row["auth_nbr"].ToString();
            if (authnbr.Trim() == "null" || authnbr.Trim() == "0")
                authnbr = "";

            authsts = row["CERT_STATUS_CD"].ToString();
            if (authsts.Trim() == "null" || authsts.Trim() == "0")
                authsts = "";
            auth.Authorization_Nbr = authnbr;
            auth.Authorization_EffDate = row["AUTH_OBTAINED_DT_TM"].ToString();
            auth.Authorization_ExpDate = row["AUTH_EXPIRE_DT_TM"].ToString();
            auth.Authorization_Status = authsts;


            return auth;
        }
    }
}