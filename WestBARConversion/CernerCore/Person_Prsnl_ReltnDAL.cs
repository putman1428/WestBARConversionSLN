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
    public class Person_Prsnl_ReltnDAL
    {
        public static string GetPrimaryCarePhysician(string personId)
        {
            DateTime todayDt = DateTime.Today;

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT  Prsnl_Person_Id, Person_Prsnl_R_Cd ");
            sb.Append("FROM  BAR_PERSON_PRSNL_RELTN ");
            sb.Append("WHERE Person_Id = " + long.Parse(personId) + " ");
            sb.Append("AND Person_Prsnl_R_Cd = '1115' ");
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
            string result = string.Empty;
            if (dt.Rows.Count > 0)
            {
                string physCode = EncntrPrsnlReltnDAL.GetAttendPhysician(dt.Rows[0]["Prsnl_Person_Id"].ToString());
                result = CernerMapDAL.GetMap("CERNER_PROVIDER", physCode, "Z.CONVPROV");

            }
            dt.Dispose();
            return result;
        }

        public static string GetPrimaryCarePhysicianName(string personId)
        {
            DateTime todayDt = DateTime.Today;

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT  Prsnl_Person_Id, Person_Prsnl_R_Cd ");
            sb.Append("FROM  BAR_PERSON_PRSNL_RELTN ");
            sb.Append("WHERE Person_Id = " + long.Parse(personId) + " ");
            sb.Append("AND Person_Prsnl_R_Cd = '1115' ");
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
            string result = string.Empty;
            if (dt.Rows.Count > 0)
            {
                //string physCode = EncntrPrsnlReltnDAL.GetAttendPhysician(dt.Rows[0]["Prsnl_Person_Id"].ToString());

                Provider provider = ProviderDAL.GetProvider(dt.Rows[0]["Prsnl_Person_Id"].ToString());
                result = provider.Name_Last + "," + provider.Name_First + " " + provider.Name_Middle;
            }
            dt.Dispose();
            return result;
        }
    }
}