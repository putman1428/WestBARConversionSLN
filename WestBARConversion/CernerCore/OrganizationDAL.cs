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
    public class OrganizationDAL
    {
        public static string GetOrganizationName(string orgId)
        {
            DateTime todayDt = DateTime.Today;
            string today = DateTime.Today.ToString("yyyy-MM-dd");
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT org_name,organization_id , org_name_key, org_status_cd, org_class_cd ");
            sb.Append("FROM BAR_ORGANIZATION ");
            sb.Append("WHERE organization_id = " + long.Parse(orgId) + " ");
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
            dt.Dispose();
            return result;
        }
    }
}
