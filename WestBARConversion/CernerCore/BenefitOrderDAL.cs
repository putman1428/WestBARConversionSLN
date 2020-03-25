using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WestBARConversion.CernerModel;


namespace WestBARConversion.CernerCore
{
    public class BenefitOrderDAL
    {
        
        
        public static string GetHealthPlan(string beniD)
        {
            Hashtable hash = new Hashtable();
            string today = DateTime.Today.ToString("yyyyMMdd");
            string item = "";

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT HEALTH_PLAN_ID  ");
            sb.Append("FROM  BAR_BENEFIT_ORDER ");
            sb.Append(" where BENEFIT_ORDER_ID = '" + beniD + "' ");
            sb.Append(" AND active_ind = '1' ");
            sb.Append("AND end_effective_dt_tm > '" + today + "' ");

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
                item = row["HEALTH_PLAN_ID"].ToString().Trim();
                break;
            }

            dt.Dispose();

            return item;
        }
        private static int GetEffDt(string dt)
        {
            string dtnum = Huron.HuronRoutines.FormatDate_YYYYMMDD(dt);
            if(Huron.HuronRoutines.IsNumeric(dtnum) == true)
                return int.Parse(dtnum);
            else
                return 0;
        }
    }
}
