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
    public class BillItemModDAL
    {
        
        public static Hashtable GetBillItemCDM()
        {
            Hashtable hash = new Hashtable();
            string today = DateTime.Today.ToString("yyyyMMdd");

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT bill_item_id, key6, key1_id  ");
            sb.Append("FROM  BAR_BILL_ITEM_MODIFIER ");
            sb.Append(" where active_ind = '1' ");
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
                if(row["key1_id"].ToString().Trim() == "667687")
                {
                    if(hash.ContainsKey(row["bill_item_id"].ToString().Trim()) == false)
                        hash.Add(row["bill_item_id"].ToString().Trim(), row["key6"].ToString().Trim());
                }

            }

            dt.Dispose();

            return hash;
        }
        public static string GetChargeMod(string chgItem)
        {
            Hashtable hash = new Hashtable();
            string today = DateTime.Today.ToString("yyyyMMdd");
            string item = "";

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT FIELD6  ");
            sb.Append("FROM  BAR_CHARGE_MOD_B ");
            sb.Append(" where CHARGE_ITEM_ID = '" + chgItem + "' ");
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
                item = row["FIELD6"].ToString().Trim();
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
