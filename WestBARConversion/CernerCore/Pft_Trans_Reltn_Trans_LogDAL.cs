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
    public class Pft_Trans_Reltn_Trans_LogDAL
    {
        /// <summary>
        /// Stored procedure called to load persion information from database
        /// </summary>
        /// <param name="personId"></param>
        /// <returns>Returns a person object to be used by conversion</returns>
        public static List<Pft_Trans_Reltn_Trans_Log> GetPft_Trans_Reltn_Trans_Records(string encounter_id)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select parent_entity_id, activity_id, parent_entity_name, bill_vrsn_nbr, amount, dr_cr_flag, benefit_order_id, ");
            sb.Append("trans_type_cd, trans_sub_type_cd,revenue_summary_id,trans_reason_cd,trans_status_cd,post_dt_tm,created_dt_tm_tl,beg_effective_dt_tm,trans_alias_id ");
            sb.Append("FROM vPft_Trans_Reltn_Trans_Log ");
            sb.Append("WHERE parent_entity_id = '" + encounter_id + "' AND  trans_type_cd in ('10982','10978') ");


            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(GlobalSettings.SqlConnectionString()))
            using (var cmd = new SqlCommand(sb.ToString(), conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.Text;
                da.Fill(dt);
            }
            List<Pft_Trans_Reltn_Trans_Log> transRecs = new List<Pft_Trans_Reltn_Trans_Log>();
            foreach (DataRow row in dt.Rows)
            {
                Pft_Trans_Reltn_Trans_Log transRec = Load_Trans_Reltn(row);
                transRecs.Add(transRec);
            }
            dt.Dispose();
            return transRecs;
        }

        private static Pft_Trans_Reltn_Trans_Log Load_Trans_Reltn(DataRow row)
        {
            Pft_Trans_Reltn_Trans_Log transRec = new Pft_Trans_Reltn_Trans_Log();
            transRec.Parent_Entity_Id = row["Parent_Entity_Id"].ToString();
            transRec.Activity_Id = row["Activity_Id"].ToString();
            transRec.Parent_Entity_Name = row["Parent_Entity_Name"].ToString();
            transRec.Bill_Vrsn_Nbr = row["Bill_Vrsn_Nbr"].ToString();
            transRec.Amount = row["Amount"].ToString();
            transRec.Dr_Cr_Flag = row["Dr_Cr_Flag"].ToString();
            transRec.Benefit_Order_Id = row["Benefit_Order_Id"].ToString();
            transRec.Trans_Type_Cd = row["Trans_Type_Cd"].ToString();
            transRec.Revenue_Summary_Id = row["Revenue_Summary_Id"].ToString();
            transRec.Trans_Sub_Type_Cd = row["trans_sub_type_cd"].ToString();
            transRec.Trans_Status_Cd = row["trans_status_cd"].ToString();
            transRec.Post_date = row["post_dt_tm"].ToString();
            transRec.Created_Dt_Tm = row["created_dt_tm_tl"].ToString();
            transRec.Beg_Eff_Dt_Tm = row["beg_effective_dt_tm"].ToString();
            transRec.Trans_Alias_ID = row["trans_alias_id"].ToString();
            return transRec;

        }
    }
}