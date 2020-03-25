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
    public class EncntrPersonReltDAL
    {
        public static string GetInsuredReltn(string encntr_id)
        {
            string result = "";
            DateTime todayDt = DateTime.Today;

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT  related_person_reltn_cd ");
            sb.Append("FROM BAR_ENCNTR_PERSON_RELTN ");
            sb.Append("WHERE encntr_Id = " + long.Parse(encntr_id) + " ");
            sb.Append("AND person_reltn_type_cd = '1158' ");
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

            foreach (DataRow row in dt.Rows)
            {
                result = row["related_person_reltn_cd"].ToString().Trim();
            }
            dt.Dispose();
            return result;
        }
        public static List<Encntr_Person_Reltn> GetGuarantor(string encounterId)
        {
            DateTime todayDt = DateTime.Today;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT  related_person_id,related_person_reltn_cd ");
            sb.Append("FROM  BAR_ENCNTR_PERSON_RELTN ");
            sb.Append("WHERE encntr_id = " + long.Parse(encounterId) + " ");
            sb.Append("AND person_reltn_type_cd = '1150' ");
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
            List<Encntr_Person_Reltn> prs = new List<Encntr_Person_Reltn>();
            foreach (DataRow row in dt.Rows)
            {
                Encntr_Person_Reltn pr = LoadPersonReltn(row);
                prs.Add(pr);
            }

            //string result = personId;
            //foreach (DataRow row in dt.Rows)
            //{
            //    result = row["related_person_id"].ToString().Trim();
            //}
            //if (dt.Rows.Count > 0)
            //    result = dt.Rows[0][0].ToString();
            dt.Dispose();
            return prs;
        }
        private static Encntr_Person_Reltn LoadPersonReltn(DataRow row)
        {
            string relCd = "";
            Encntr_Person_Reltn reltn = new Encntr_Person_Reltn();
            relCd = row["related_person_reltn_cd"].ToString();
            reltn.guarPersonID = row["related_person_id"].ToString();
            reltn.relationship = CernerMapDAL.GetMap("CERNER_RELATIONSHIP", relCd, relCd);

            return reltn;
        }
    }
}
