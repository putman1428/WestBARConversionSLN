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
    public class PftEncntrDAL
    {
        public static List<PFT_Encntr> GetPFT_Encntrs(string encounterId)
        {
            int cnt = 0;
            string today = DateTime.Today.ToString("yyyyMMdd");
            StringBuilder sb = new StringBuilder();
            sb.Append("select pft_encntr_id, encntr_Id, acct_id, pft_encntr_status_cd, fin_class_cd, active_ind, ");
            sb.Append("beg_effective_dt_tm, end_effective_dt_tm, billing_entity_id, adjustment_balance, adj_bal_dr_cr_flag, ");
            sb.Append("applied_payment_balance, balance, charge_balance, chrg_bal_dr_cr_flag, collection_state_cd, ");
            sb.Append("dr_cr_flag, last_charge_dt_tm, last_payment_dt_tm, recur_ind, recur_seq, pft_encntr_alias, ");
            sb.Append("bill_status_cd, last_adjustment_dt_tm, last_claim_dt_tm, last_stmt_dt_tm, last_patient_pay_dt_tm, ");
            sb.Append("statement_cycle_id, pat_bal_fwd, nbr_of_stmts, bad_debt_dt_tm, recur_current_year, ");
            sb.Append("zero_balance_dt_tm, recur_type_cd,bad_debt_balance,bad_debt_bal_dr_cr_flag  ");
            sb.Append("from BAR_pft_encntr "  );
            sb.Append("where encntr_id = " + long.Parse(encounterId) + " AND active_ind = '1'" );

            //sb.Append("and balance = '0'");
  
            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(GlobalSettings.SqlConnectionString()))
            using (var cmd = new SqlCommand(sb.ToString(), conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.Text;
                da.Fill(dt);
            }

            List<PFT_Encntr> pfts = new List<PFT_Encntr>();
            foreach(DataRow row in dt.Rows)
            {
                if (GetEffDt(dt.Rows[cnt]["end_effective_dt_tm"].ToString().Trim()) > int.Parse(today))
                {
                    PFT_Encntr pft = LoadPFTEncntr(row);
                    pfts.Add(pft);
                }
                cnt = cnt + 1;
            }
            dt.Dispose();
            return pfts;
        }

        private static PFT_Encntr LoadPFTEncntr(DataRow row)
        {
            PFT_Encntr pft = new PFT_Encntr();
            pft.Pft_Encntr_Id = row["pft_encntr_id"].ToString();
            pft.Encntr_Id = row["encntr_id"].ToString();
            pft.Acct_Id = row["acct_id"].ToString();
            pft.Pft_Encntr_Status_Cd = row["pft_encntr_status_cd"].ToString();
            pft.Pft_Encntr_Status_Cd_Desc = row["pft_encntr_status_cd"].ToString();
            pft.Fin_Class_Cd = row["fin_class_cd"].ToString();
            pft.Fin_Class_Cd_Desc = row["fin_class_cd"].ToString();
            pft.Active_Ind = row["active_ind"].ToString();
            pft.Beg_Effective_Dt_Tm = CernerCommon.ConvertDateString(row["beg_effective_dt_tm"].ToString());
            pft.End_Effective_Dt_Tm = CernerCommon.ConvertDateString(row["end_effective_dt_tm"].ToString());
            pft.Billing_Entity_Id = row["billing_entity_id"].ToString();
            pft.Adjustment_Balance = CernerCommon.ConvertDecimalString(row["Adjustment_Balance"].ToString());
            pft.Applied_Payment_Balance = CernerCommon.ConvertDecimalString(row["Applied_Payment_Balance"].ToString());
            pft.Balance = CernerCommon.ConvertDecimalString(row["balance"].ToString());
            pft.Charge_Balance = CernerCommon.ConvertDecimalString(CheckAmt(row["charge_balance"].ToString()));
            pft.Collection_State_Cd = row["collection_state_cd"].ToString();
            pft.Collection_State_Cd_Desc = row["collection_state_cd"].ToString();
            pft.Last_Adjustment_Dt_Tm = CernerCommon.ConvertDateString(row["last_adjustment_dt_tm"].ToString());
            pft.Last_Payment_Dt_Tm = CernerCommon.ConvertDateString(row["last_payment_dt_tm"].ToString());
            pft.Recur_Ind = row["recur_ind"].ToString();
            pft.Recur_Seq = row["recur_seq"].ToString();
            pft.Pft_Encntr_Alias = row["pft_encntr_alias"].ToString();
            pft.Bill_Status_Cd = row["bill_status_cd"].ToString();
            pft.Bill_Status_Cd_Desc = row["bill_status_cd"].ToString();
            pft.Last_Claim_Dt_Tm = CernerCommon.ConvertDateString(row["last_claim_dt_tm"].ToString());
            pft.Last_Stmt_Dt_Tm = CernerCommon.ConvertDateString(row["last_stmt_dt_tm"].ToString());
            pft.Last_Patient_Pay_dt_Tm = CernerCommon.ConvertDateString(row["last_patient_pay_dt_tm"].ToString());
            pft.Statement_Cycle_Id = row["statement_cycle_id"].ToString();
            pft.Pat_Bal_Fwd = CernerCommon.ConvertDecimalString(row["pat_bal_fwd"].ToString());
            pft.Nbr_Of_Stmts = row["Nbr_Of_Stmts"].ToString();
            pft.Bad_Debt_Dt_Tm = CernerCommon.ConvertDateString(row["bad_debt_dt_tm"].ToString());
            pft.Recur_Current_Year = row["recur_current_year"].ToString();
            pft.Zero_Balance_Dt_Tm = CernerCommon.ConvertDateString(row["zero_balance_dt_tm"].ToString());
            pft.Recur_Type_Cd = row["recur_type_cd"].ToString();
            pft.Recur_Type_Desc = row["recur_type_cd"].ToString();
            pft.Bad_Debt_Balance = CernerCommon.ConvertDecimalString(CheckAmt(row["bad_debt_balance"].ToString()));
            pft.Bad_Debt_Bal_Dr_Cr_Flag = row["bad_debt_bal_dr_cr_flag"].ToString();
            pft.Dr_Cr_Flag = row["dr_cr_flag"].ToString();
            pft.Chrg_Bal_Dr_Cr_Flag = row["chrg_bal_dr_cr_flag"].ToString();
            return pft;
        }
        private static int GetEffDt(string dt)
        {
            string dtnum = Huron.HuronRoutines.FormatDate_YYYYMMDD(dt);
            if (Huron.HuronRoutines.IsNumeric(dtnum) == true)
                return int.Parse(dtnum);
            else
                return 0;
        }
        private static string CheckAmt(string amt)
        {
            string val1 = "";
            string val2 = "";
            string[] item = amt.Split('.');
            if (item.Length > 1)
            {
                val2 = item[1];
                if (val2.Length > 2)
                {
                    val2 = val2.Substring(0, 2);
                    amt = val1 + "." + val2;
                }
            }
            return amt;
        }
    }
}