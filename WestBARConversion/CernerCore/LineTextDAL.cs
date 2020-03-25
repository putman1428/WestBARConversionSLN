using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using WestBARConversion.CernerModel;

namespace WestBARConversion.CernerCore
{
    public class LineTextDAL
    {
        private static int globalTextCntr = 0;
        private static Hashtable globalTextHash;
        public static Hashtable GetTextData(string longText_id, string parentEntity, string hcis)
        {
            char OD = '\x000d';
            char OA = '\x000a';
            char sep = (char)186;
            globalTextHash = new Hashtable();
            DateTime todayDt = DateTime.Today;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT PARENT_ENTITY_NAME,PARENT_ENTITY_ID,LONG_TEXT_ID,ACTIVE_STATUS_DT_TM ");
            sb.Append("FROM  [BAR_LONG_TEXT] ");
            sb.Append("WHERE LONG_TEXT_ID = " + long.Parse(longText_id) + " ");// " AND parentEntity = '" + parentEntity + "' ");
            sb.Append("AND active_ind = '1' ");
            //sb.Append("AND END_EFFECTIVE_DT_TM > '" + todayDt + "'");

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

                string rec = row["LONG_TEXT"].ToString().Replace(OD, sep).Replace(OA,sep);
                string tdate = CDV_DAL.GetCernerDate2(row["ACTIVE_STATUS_DT_TM"].ToString(),hcis).ToString();
                string[] recs = rec.Split(sep);
                for(int x=0;x<recs.Length;x++)
                {
                    GetText(recs[x].TrimEnd(), tdate);
                }
            }
            
            dt.Dispose();
            return globalTextHash;
        }
        public static Hashtable GetTextData(string longText_id, string parentEntity, string hcis, string tdate)
        {
            char OD = '\x000d';
            char OA = '\x000a';
            char sep = (char)186;
            globalTextHash = new Hashtable();
            DateTime todayDt = DateTime.Today;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT PARENT_ENTITY_NAME,PARENT_ENTITY_ID,LONG_TEXT_ID,ACTIVE_STATUS_DT_TM ");
            sb.Append("FROM  [BAR_LONG_TEXT] ");
            sb.Append("WHERE LONG_TEXT_ID = " + long.Parse(longText_id) + " ");// " AND parentEntity = '" + parentEntity + "' ");
            sb.Append("AND active_ind = '1' ");
            //sb.Append("AND END_EFFECTIVE_DT_TM > '" + todayDt + "'");

            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(GlobalSettings.SqlConnectionString()))
            using (var cmd = new SqlCommand(sb.ToString(), conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.Text;
                da.Fill(dt);
            }
            tdate = CDV_DAL.GetCernerDate2(tdate, hcis).ToString();
            foreach (DataRow row in dt.Rows)
            {

                string rec = row["LONG_TEXT"].ToString().Replace(OD, sep).Replace(OA, sep);
                string[] recs = rec.Split(sep);
                for (int x = 0; x < recs.Length; x++)
                {
                    GetText(recs[x].TrimEnd(), tdate);
                }
            }

            dt.Dispose();
            return globalTextHash;
        }
        public static Hashtable GetTextDataAll(string parentEntity, string hcis)
        {
            char OD = '\x000d';
            char OA = '\x000a';
            char sep = (char)186;
            globalTextHash = new Hashtable();
            DateTime todayDt = DateTime.Today;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT PARENT_ENTITY_NAME,PARENT_ENTITY_ID,LONG_TEXT,ACTIVE_STATUS_DT_TM ");
            sb.Append("FROM  [BAR_LONG_TEXT] ");
            sb.Append("WHERE PARENT_ENTITY_ID = " + long.Parse(parentEntity) + " ");// " AND parentEntity = '" + parentEntity + "' ");
            sb.Append("AND active_ind = '1' ");
            //sb.Append("AND END_EFFECTIVE_DT_TM > '" + todayDt + "'");

            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(GlobalSettings.SqlConnectionString()))
            using (var cmd = new SqlCommand(sb.ToString(), conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.Text;
                da.Fill(dt);
            }
            globalTextCntr = 0;
            foreach (DataRow row in dt.Rows)
            {

                string rec = row["LONG_TEXT"].ToString().Replace(OD, sep).Replace(OA, sep);
                string tdate = row["ACTIVE_STATUS_DT_TM"].ToString(); //CDV_DAL.GetCernerDate2(row["ACTIVE_STATUS_DT_TM"].ToString(), hcis).ToString();
                string[] recs = rec.Split(sep);
                for (int x = 0; x < recs.Length; x++)
                {
                    GetText(recs[x].TrimEnd(), tdate);
                }
            }

            dt.Dispose();
            return globalTextHash;
        }
        private static void GetText(string txt, string tdate)
        {
            string word = "";
            string[] n;
            string h = "";
            int tot = 0;
            
            if (txt.Length <= 45)
            {
                globalTextCntr++;
                globalTextHash.Add(globalTextCntr, tdate + '\t' + txt);
            }
            else
            {
                n = txt.Split(' ');
                for (int num = 0; num < n.Length; num++)
                {
                    h = n[num].ToString() + " ";
                    tot = tot + h.Length;
                    if (tot > 45)
                    {
                        globalTextCntr++;
                        globalTextHash.Add(globalTextCntr, tdate + '\t' + word);
                        word = "";
                        tot = 0;

                        h = n[num].ToString() + " ";
                        tot = tot + h.Length;
                        word = word + h;
                    }
                    else
                        word = word + h;
                }
                globalTextCntr++;
                globalTextHash.Add(globalTextCntr, tdate + '\t' + word);
            }
            
        }
    }
}