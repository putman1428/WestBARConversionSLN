using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WestBARConversion.CernerModel
{
    public class PFT_Encntr
    {
        public string Pft_Encntr_Id { get; set; }
        public string Encntr_Id { get; set; }
        public string Acct_Id { get; set; }
        public string Pft_Encntr_Status_Cd { get; set; }
        public string Pft_Encntr_Status_Cd_Desc { get; set; }
        public string Fin_Class_Cd { get; set; }
        public string Fin_Class_Cd_Desc { get; set; }
        public string Active_Ind { get; set; }
        public DateTime? Beg_Effective_Dt_Tm { get; set; }
        public DateTime? End_Effective_Dt_Tm { get; set; }
        public string Billing_Entity_Id { get; set; }
        public decimal? Adjustment_Balance { get; set; }
        public decimal? Applied_Payment_Balance { get; set; }
        public decimal? Balance { get; set; }
        public decimal? Charge_Balance { get; set; }
        public string Collection_State_Cd { get; set; }
        public string Collection_State_Cd_Desc { get; set; }
        public DateTime? Last_Adjustment_Dt_Tm { get; set; }
        public DateTime? Last_Payment_Dt_Tm { get; set; }
        public string Recur_Ind { get; set; }
        public string Recur_Seq { get; set; }
        public string Pft_Encntr_Alias { get; set; }
        public string Bill_Status_Cd { get; set; }
        public string Bill_Status_Cd_Desc { get; set; }
        public DateTime? Last_Claim_Dt_Tm { get; set; }
        public DateTime? Last_Stmt_Dt_Tm { get; set; }
        public DateTime? Last_Patient_Pay_dt_Tm { get; set; }
        public string Statement_Cycle_Id { get; set; }
        public decimal? Pat_Bal_Fwd { get; set; }
        public string Nbr_Of_Stmts { get; set; }
        public DateTime? Bad_Debt_Dt_Tm { get; set; }
        public string Recur_Current_Year { get; set; }
        public DateTime? Zero_Balance_Dt_Tm { get; set; }
        public string Recur_Type_Cd { get; set; }
        public string Recur_Type_Desc { get; set; }
        public decimal? Bad_Debt_Balance { get; set; }
        public string Bad_Debt_Bal_Dr_Cr_Flag { get; set; }
        public string Chrg_Bal_Dr_Cr_Flag { get; set; }
        public string Dr_Cr_Flag { get; set; }

    }
}