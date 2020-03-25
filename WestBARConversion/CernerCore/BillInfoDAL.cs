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
    public class BillInfoDAL
    {
        public static List<Bill_Record_Data> GetBillRecs(string pft_enc_id)
        {
            int cnt = 0;
            string today = DateTime.Today.ToString("yyyyMMdd");
            DateTime todayDt = DateTime.Today;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT  PFT_ENCNTR_ID,PFT_PRORATION_ID,ENCNTR_PLAN_RELTN_ID,HEALTH_PLAN_ID,BO_HP_RELTN_ID,TOTAL_BILLED_AMOUNT,TOTAL_BILLED_DR_CR_FLAG,TOTAL_PAID_AMOUNT,TOTAL_PAID_DR_CR_FLAG,TOTAL_ADJ_AMOUNT,TOTAL_ADJ_DR_CR_FLAG,BOHP_ACTIND,AMOUNT_OWED,AMOUNT_OWED_DR_CR_FLAG,BOHP_ENDEFFDATE,PRIORITY_SEQ,ORIG_AMT_DUE,ORIG_AMOUNT_DR_CR_FLAG,HIGH_AMT,HIGH_AMOUNT_DR_CR_FLAG,CURR_AMT_DUE,CURR_AMOUNT_DR_CR_FLAG,NON_COVERED_AMT,NON_COVERED_AMT_DR_CR_FLAG,TOTAL_ADJ,Proration_TotAdjDRCRFlag,Proration_EndEffDt,Proration_ActInd,BILL_RELTN_ID,CORSP_ACTIVITY_ID,BillReltnActInd,BillReltnEndEffDt,PARENT_ENTITY_ID,PARENT_ENTITY_NAME,BILL_VRSN_NBR,BILL_NBR_DISP,BILL_STATUS_CD,BILL_STATUS_REASON_CD,BillRec_ActInd,BillRec_EndEffDt,BALANCE,BILL_NBR_DISP_KEY,BILL_CLASS_CD,GEN_DT_TM,PAGE_CNT,BILL_TYPE_CD,BALANCE_DR_CR_FLAG,BALANCE_FWD,BALANCE_FWD_DR_CR_FLAG,BALANCE_DUE,BALANCE_DUE_DR_CR_FLAG,NEW_AMOUNT,NEW_AMOUNT_DR_CR_FLAG,CLAIM_STATUS_CD,SUBMIT_DT_TM,LAST_PAYMENT_DT_TM,LAST_ADJUSTMENT_DT_TM,CLAIM_SERIAL_NBR,TRANSMISSION_DT_TM,FROM_SERVICE_DT_TM,TO_SERVICE_DT_TM,STATEMENT_TO_DT_TM,STATEMENT_FROM_DT_TM ");
            sb.Append("FROM  [vBAR_BillRecords] ");
            sb.Append("WHERE PFT_ENCNTR_ID = " + long.Parse(pft_enc_id) + " ");
            
            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(GlobalSettings.SqlConnectionString()))
            using (var cmd = new SqlCommand(sb.ToString(), conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.Text;
                da.Fill(dt);
            }
            List<Bill_Record_Data> bills = new List<Bill_Record_Data>();
            foreach (DataRow row in dt.Rows)
            {
                if (row["BOHP_ACTIND"].ToString().Trim() == "1" && row["Proration_ActInd"].ToString().Trim() == "1" && row["BillReltnActInd"].ToString().Trim() == "1" && row["BillRec_ActInd"].ToString().Trim() == "1")
                {
                    if (GetEffDt(row["Proration_EndEffDt"].ToString().Trim()) > int.Parse(today) && GetEffDt(row["BOHP_ENDEFFDATE"].ToString().Trim()) > int.Parse(today) && GetEffDt(row["BillReltnEndEffDt"].ToString().Trim()) > int.Parse(today) && GetEffDt(row["BillRec_EndEffDt"].ToString().Trim()) > int.Parse(today))
                    {
                        Bill_Record_Data bill = LoadBillsData(row);
                        bills.Add(bill);
                    }
                }
            }
            
            dt.Dispose();
            return bills;
        }
        private static Bill_Record_Data LoadBillsData(DataRow row)
        {
            Bill_Record_Data bill = new Bill_Record_Data();

            bill.PFT_ENCNTR_ID = row["PFT_ENCNTR_ID"].ToString();
            bill.PFT_PRORATION_ID = row["PFT_PRORATION_ID"].ToString();
            bill.ENCNTR_PLAN_RELTN_ID = row["ENCNTR_PLAN_RELTN_ID"].ToString();
            bill.HEALTH_PLAN_ID = row["HEALTH_PLAN_ID"].ToString();
            bill.BO_HP_RELTN_ID = row["BO_HP_RELTN_ID"].ToString();
            bill.TOTAL_BILLED_AMOUNT = row["TOTAL_BILLED_AMOUNT"].ToString();
            bill.TOTAL_BILLED_DR_CR_FLAG = row["TOTAL_BILLED_DR_CR_FLAG"].ToString();
            bill.TOTAL_PAID_AMOUNT = row["TOTAL_PAID_AMOUNT"].ToString();
            bill.TOTAL_PAID_DR_CR_FLAG = row["TOTAL_PAID_DR_CR_FLAG"].ToString();
            bill.TOTAL_ADJ_AMOUNT = row["TOTAL_ADJ_AMOUNT"].ToString();
            bill.TOTAL_ADJ_DR_CR_FLAG = row["TOTAL_ADJ_DR_CR_FLAG"].ToString();
            bill.BOHP_ACTIND = row["BOHP_ACTIND"].ToString();
            bill.AMOUNT_OWED = row["AMOUNT_OWED"].ToString();
            bill.AMOUNT_OWED_DR_CR_FLAG = row["AMOUNT_OWED_DR_CR_FLAG"].ToString();
            bill.BOHP_ENDEFFDATE = row["BOHP_ENDEFFDATE"].ToString();
            bill.PRIORITY_SEQ = row["PRIORITY_SEQ"].ToString();
            bill.ORIG_AMT_DUE = row["ORIG_AMT_DUE"].ToString();
            bill.ORIG_AMOUNT_DR_CR_FLAG = row["ORIG_AMOUNT_DR_CR_FLAG"].ToString();
            bill.HIGH_AMT = row["HIGH_AMT"].ToString();
            bill.HIGH_AMOUNT_DR_CR_FLAG = row["HIGH_AMOUNT_DR_CR_FLAG"].ToString();
            bill.CURR_AMT_DUE = CheckAmt(row["CURR_AMT_DUE"].ToString());
            bill.CURR_AMOUNT_DR_CR_FLAG = row["CURR_AMOUNT_DR_CR_FLAG"].ToString();
            bill.NON_COVERED_AMT = row["NON_COVERED_AMT"].ToString();
            bill.NON_COVERED_AMT_DR_CR_FLAG = row["NON_COVERED_AMT_DR_CR_FLAG"].ToString();
            bill.TOTAL_ADJ = row["TOTAL_ADJ"].ToString();
            bill.Proration_TotAdjDRCRFlag = row["Proration_TotAdjDRCRFlag"].ToString();
            bill.Proration_EndEffDt = row["Proration_EndEffDt"].ToString();
            bill.Proration_ActInd = row["Proration_ActInd"].ToString();
            bill.BILL_RELTN_ID = row["BILL_RELTN_ID"].ToString();
            bill.CORSP_ACTIVITY_ID = row["CORSP_ACTIVITY_ID"].ToString();
            bill.BillReltnActInd = row["BillReltnActInd"].ToString();
            bill.BillReltnEndEffDt = row["BillReltnEndEffDt"].ToString();
            bill.PARENT_ENTITY_ID = row["PARENT_ENTITY_ID"].ToString();
            bill.PARENT_ENTITY_NAME = row["PARENT_ENTITY_NAME"].ToString();
            bill.BILL_VRSN_NBR = row["BILL_VRSN_NBR"].ToString();
            bill.BILL_NBR_DISP = row["BILL_NBR_DISP"].ToString();
            bill.BILL_STATUS_CD = row["BILL_STATUS_CD"].ToString();
            bill.BILL_STATUS_REASON_CD = row["BILL_STATUS_REASON_CD"].ToString();
            bill.BillRec_ActInd = row["BillRec_ActInd"].ToString();
            bill.BillRec_EndEffDt = row["BillRec_EndEffDt"].ToString();
            bill.BALANCE = row["BALANCE"].ToString();
            bill.BILL_NBR_DISP_KEY = row["BILL_NBR_DISP_KEY"].ToString();
            bill.BILL_CLASS_CD = row["BILL_CLASS_CD"].ToString();
            bill.GEN_DT_TM = row["GEN_DT_TM"].ToString();
            bill.PAGE_CNT = row["PAGE_CNT"].ToString();
            bill.BILL_TYPE_CD = row["BILL_TYPE_CD"].ToString();
            bill.BALANCE_DR_CR_FLAG = row["BALANCE_DR_CR_FLAG"].ToString();
            bill.BALANCE_FWD = row["BALANCE_FWD"].ToString();
            bill.BALANCE_FWD_DR_CR_FLAG = row["BALANCE_FWD_DR_CR_FLAG"].ToString();
            bill.BALANCE_DUE = row["BALANCE_DUE"].ToString();
            bill.BALANCE_DUE_DR_CR_FLAG = row["BALANCE_DUE_DR_CR_FLAG"].ToString();
            bill.NEW_AMOUNT = row["NEW_AMOUNT"].ToString();
            bill.NEW_AMOUNT_DR_CR_FLAG = row["NEW_AMOUNT_DR_CR_FLAG"].ToString();
            bill.CLAIM_STATUS_CD = row["CLAIM_STATUS_CD"].ToString();
            bill.SUBMIT_DT_TM = row["SUBMIT_DT_TM"].ToString();
            bill.LAST_PAYMENT_DT_TM = row["LAST_PAYMENT_DT_TM"].ToString();
            bill.LAST_ADJUSTMENT_DT_TM = row["LAST_ADJUSTMENT_DT_TM"].ToString();
            bill.CLAIM_SERIAL_NBR = row["CLAIM_SERIAL_NBR"].ToString();
            bill.TRANSMISSION_DT_TM = row["TRANSMISSION_DT_TM"].ToString();
            bill.FROM_SERVICE_DT_TM = row["FROM_SERVICE_DT_TM"].ToString();
            bill.TO_SERVICE_DT_TM = row["TO_SERVICE_DT_TM"].ToString();
            bill.STATEMENT_TO_DT_TM = row["STATEMENT_TO_DT_TM"].ToString();
            bill.STATEMENT_FROM_DT_TM = row["STATEMENT_FROM_DT_TM"].ToString();

            return bill;
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