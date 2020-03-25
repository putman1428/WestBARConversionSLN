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
    public class EncounterInfoDAL
    {
        public static Hashtable GetEncounterInfo(string encounter_id,string hcis)
        {
            int cntr = 1;
            Hashtable hash = new Hashtable();
            DateTime todayDt = DateTime.Today;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT  LONG_TEXT_ID ");
            sb.Append("FROM  BAR_ENCNTR_INFO ");
            sb.Append("WHERE encntr_id = " + long.Parse(encounter_id) + " ");
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
                Hashtable tempHash = LineTextDAL.GetTextData(row["LONG_TEXT_ID"].ToString(), "ENCNTR_INFO", hcis);
                hash.Add(cntr, tempHash);
                cntr = cntr + 1;
            }
            
            dt.Dispose();
            return hash;
        }
        public static Hashtable GetCorspInfo(string pftEncID, string hcis)
        {
            int cntr = 1;
            Hashtable hash = new Hashtable();
            DateTime todayDt = DateTime.Today;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT  LONG_TEXT_ID,CREATED_DT_TM, CREATED_PRSNL_ID ");
            sb.Append("FROM  vCORSP_LOG_RELTN_CORSP_LOG ");
            sb.Append("WHERE PFT_ENCNTR_ID = " + long.Parse(pftEncID) + " ");
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
                Hashtable tempHash = LineTextDAL.GetTextData(row["LONG_TEXT_ID"].ToString(), "ENCNTR_INFO", hcis, row["CREATED_DT_TM"].ToString());
                hash.Add(cntr, tempHash);
                cntr = cntr + 1;
            }

            dt.Dispose();
            return hash;
        }

        public static Hashtable GetEncounterInfoV2(string encounter_id, string hcis)
        {
            Hashtable tempHash = LineTextDAL.GetTextDataAll(encounter_id, hcis);
            return tempHash;
        }
        //private static EncounterInfo LoadEncounterInfo(DataRow row)
        //{
        //    string authnbr = "";
        //    string authsts = "";
        //    EncounterInfo enc = new EncounterInfo();

        //    enc.TextData = LineTextDAL.GetTextData(row["LONG_TEXT_ID"].ToString(), "ENCNTR_INFO");
        //    //enc.TextDate = row["AUTH_OBTAINED_DT_TM"].ToString();

        //    return enc;
        //}
    }
}