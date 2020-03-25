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
    public class EncntrPrsnlReltnDAL
    {
        public static string GetAttendPhysician(string encounterId)
        {
            DateTime todayDt = DateTime.Today;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT  PRSNL_PERSON_ID ");
            sb.Append("FROM  BAR_ENCNTR_PRSNL_RELTN ");
            sb.Append("WHERE encntr_id = " + long.Parse(encounterId) + " ");
            sb.Append("AND encntr_prsnl_r_cd = '1119' ");
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
            string result = encounterId;
            if (dt.Rows.Count > 0)
                result = dt.Rows[0][0].ToString();
            dt.Dispose();
            return result;
        }

        public static string GetGuarantor(string encounterId)
        {
            DateTime todayDt = DateTime.Today;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT  PRSNL_PERSON_ID ");
            sb.Append("FROM  BAR_ENCNTR_PRSNL_RELTN ");
            sb.Append("WHERE encntr_id = " + long.Parse(encounterId) + " ");
            sb.Append("AND encntr_prsnl_r_cd = '1155' ");
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
            string result = encounterId;
            if (dt.Rows.Count > 0)
                result = dt.Rows[0][0].ToString();
            dt.Dispose();
            return result;
        }

        public static List<Provider> GetEncounterPhysicians(string encounterId, string facility)
        {
            DateTime todayDt = DateTime.Today;
            string npi = "";
            string lic = "";
            StringBuilder sb = new StringBuilder();
            //sb.Append("SELECT epr.prsnl_person_id, epr.encntr_prsnl_r_cd, epr.encntr_id, epr.beg_effective_dt_tm,  ");
            //sb.Append("epr.end_effective_dt_tm, epr.encntr_type_cd, cv.display, cv.code_value  ");
            //sb.Append("FROM ENCNTR_PRSNL_RELTN epr  ");
            //sb.Append("JOIN CODE_VALUE cv on cv.code_value = epr.encntr_prsnl_r_cd ");
            //sb.Append("Where epr.Encntr_id = '" + encounterId + "' ");
            //sb.Append("AND(epr.encntr_prsnl_r_cd = '1116.0000' OR epr.encntr_prsnl_r_cd = '1119.0000' ");
            //sb.Append("OR epr.encntr_prsnl_r_cd = '1121.0000') ");
            sb.Append("SELECT epr.prsnl_person_id, epr.encntr_prsnl_r_cd, epr.encntr_id, epr.beg_effective_dt_tm,epr.end_effective_dt_tm, epr.encntr_type_cd  ");
            sb.Append("FROM BAR_ENCNTR_PRSNL_RELTN epr  ");
            sb.Append("Where epr.Encntr_id = " + long.Parse(encounterId) + " ");
            //sb.Append("AND(epr.encntr_prsnl_r_cd = '1116' OR epr.encntr_prsnl_r_cd = '1119' ");
            //sb.Append("OR epr.encntr_prsnl_r_cd = '1121') ");
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

            List<Provider> providers = new List<Provider>();
            foreach(DataRow row in dt.Rows)
            {
                Provider provider = LoadPhysician(row);
                if (provider != null)
                {
                    string phys = row["prsnl_person_id"].ToString();
                    string physType = row["encntr_prsnl_r_cd"].ToString();// CernerCommon.GetPhysiicanType(row["encntr_prsnl_r_cd"].ToString());

                    npi = PrsnlAliasDAL.GetPrsnlAlias(phys);
                    //lic = PrsnlAliasDAL.GetPrsnlAliasLic(phys);



                    provider.Mnemonic = CernerMapDAL.GetMapDR(npi, facility, "Z.CONVPROV");// npi;
                    provider.PhysicianType = physType;
                    provider.NPI = npi;
                    provider.License = lic;
                    providers.Add(provider);
                    
                }
            }
            dt.Dispose();
            return providers;
        }

        private static Provider LoadPhysician(DataRow row)
        {
            Provider provider = ProviderDAL.GetProvider(row["prsnl_person_id"].ToString());
            return provider;
        }
    }
}
