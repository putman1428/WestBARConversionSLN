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
    public class BarAccountDAL
    {
        public static BarAccount GetBARAccount(string accountId)
        {
            DateTime todayDt = DateTime.Today;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT pft_encntr_id, encntr_id, acct_id, pft_encntr_status_cd, ");
            sb.Append("disch_dt_tm, fin_class_cd, self_pay_edit_flag, ");
            sb.Append("active_ind, active_status_cd, active_status_dt_tm, ");
            sb.Append("adjustment_balance, applied_payment_balance, balance, charge_balance, ");
            sb.Append("last_charge_dt_tm, last_adjustment_dt_tm, last_payment_dt_tm, ");
            sb.Append("pft_encntr_alias, bill_status_cd, last_claim_dt_tm, last_stmt_dt_tm, ");
            sb.Append("zero_balance_dt_tm ");
            sb.Append("From pft_encntr ");
            sb.Append("WHERE acct_id = " + long.Parse(accountId) + " ");
            sb.Append("AND active_ind = '1' ");
            
            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(GlobalSettings.SqlConnectionString()))
            using (var cmd = new SqlCommand(sb.ToString(), conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.Text;
                da.Fill(dt);
            }

            BarAccount barAccount = null;
            if (dt.Rows.Count > 0)
                barAccount = LoadBarAccount(dt.Rows[0]);
            return barAccount;
        }

        private static BarAccount LoadBarAccount(DataRow dr)
        {
            BarAccount barAccount = new BarAccount();

            return barAccount;
        }
    }
}