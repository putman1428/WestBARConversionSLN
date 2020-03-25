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
    public class Pft_Trans_Reltn_DAL
    {
        /// <summary>
        /// Stored procedure called to load persion information from database
        /// </summary>
        /// <param name="personId"></param>
        /// <returns>Returns a person object to be used by conversion</returns>
        /// 
        //Tran_Type Descr
        //code_value code_set    cdf_meaning display
        //10978.0000  18649.0000  ADJUST Adjustment
        //10979.0000  18649.0000  CHARGE Charge
        //10982.0000  18649.0000  PAYMENT Payment
        public static List<Pft_Trans_Reltn> GetPft_Trans_Reltn_Charge_Records(string encounter_id)
        {
            DateTime todayDt = DateTime.Today;

            StringBuilder sb = new StringBuilder();
            sb.Append("select parent_entity_id, activity_id, parent_entity_name, bill_vrsn_nbr, amount, dr_cr_flag, benefit_order_id, ");
            sb.Append("trans_type_cd, revenue_summary_id ");
            sb.Append("FROM BAR_pft_trans_reltn ");
            sb.Append("WHERE parent_entity_id = " + long.Parse(encounter_id) + " AND  trans_type_cd = '10979' " );
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
            List<Pft_Trans_Reltn> transRecs = new List<Pft_Trans_Reltn>();
            foreach (DataRow row in dt.Rows)
            {
                Pft_Trans_Reltn transRec = Load_Trans_Reltn(dt.Rows[0]);
                transRecs.Add(transRec);
            }
            dt.Dispose();
            return transRecs;
        }

        private static Pft_Trans_Reltn Load_Trans_Reltn(DataRow row)
        {
            Pft_Trans_Reltn transRec = new Pft_Trans_Reltn();
            transRec.Parent_Entity_Id = row["Parent_Entity_Id"].ToString();
            transRec.Activity_Id = row["Activity_Id"].ToString();
            transRec.Parent_Entity_Name = row["Parent_Entity_Name"].ToString();
            transRec.Bill_Vrsn_Nbr = row["Bill_Vrsn_Nbr"].ToString();
            transRec.Amount = row["Amount"].ToString();
            transRec.Dr_Cr_Flag = row["Dr_Cr_Flag"].ToString();
            transRec.Benefit_Order_Id = row["Benefit_Order_Id"].ToString();
            transRec.Trans_Type_Cd = row["Trans_Type_Cd"].ToString();
            transRec.Revenue_Summary_Id = row["Revenue_Summary_Id"].ToString();
            return transRec;
        }
    }
}