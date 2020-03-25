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
    public class CernerMapDAL
    {
        private static string GlobalResult = "|||||||||||||||||||||||||";
        public static string GetMap(string tableName, string cernerCode, string defaultValue)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT MeditechCode FROM " + tableName + " ");
            sb.Append("WHERE CernerCode = '" + cernerCode + "'");

            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(GlobalSettings.SqlConnectionString_Alt()))
            using (var cmd = new SqlCommand(sb.ToString(), conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }

            string result = defaultValue;
            if (dt.Rows.Count > 0)
                result = dt.Rows[0][0].ToString();
            return result;
        }

        public static string GetMapDR(string npi, string facility, string defaultValue)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT MTMnemonic FROM CERNER_PROVIDER_MAP ");
            sb.Append("WHERE ProviderNPI = '" + npi + "' AND Facility = '" + facility + "'");

            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(GlobalSettings.SqlConnectionString_Alt()))
            using (var cmd = new SqlCommand(sb.ToString(), conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }

            string result = defaultValue;
            if (dt.Rows.Count > 0)
                result = dt.Rows[0][0].ToString();
            return result;
        }
        public static Hashtable GetMapAll(string tableName)
        {
            string ccode = "";
            string mcode = "";
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT CernerCode, MeditechCode FROM " + tableName);

            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(GlobalSettings.SqlConnectionString_Alt()))
            using (var cmd = new SqlCommand(sb.ToString(), conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }
            Hashtable mapHash = new Hashtable();
            foreach (DataRow row in dt.Rows)
            {
                ccode = row["CernerCode"].ToString();
                mcode = row["MeditechCode"].ToString();
                mapHash.Add(ccode, mcode);
            }
            return mapHash;
        }
        //Mapping------------------------------------------------------------------------------------------------------------------------
        public static Hashtable GetCERNER_DISCHARGE_DISPOSITION()
        {
            Hashtable hash = new Hashtable();
            int cnt = 0;

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT CernerCode,MeditechCode ");
            sb.Append("FROM  [CERNER_DISCHARGE_DISPOSITION] ");

            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(GlobalSettings.SqlConnectionString_Alt()))
            using (var cmd = new SqlCommand(sb.ToString(), conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.Text;
                da.Fill(dt);
            }
            foreach (DataRow row in dt.Rows)
            {
                if (hash.ContainsKey("DD_" + dt.Rows[cnt]["CernerCode"].ToString().Trim()) == false)
                {
                    hash.Add("DD_" + dt.Rows[cnt]["CernerCode"].ToString().Trim(), dt.Rows[cnt]["MeditechCode"].ToString().Trim());
                }
                cnt = cnt + 1;
            }

            return hash;
        }
        public static Hashtable GetCERNER_GENDER()
        {
            Hashtable hash = new Hashtable();
            int cnt = 0;

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT CernerCode,MeditechCode ");
            sb.Append("FROM  [CERNER_GENDER] ");

            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(GlobalSettings.SqlConnectionString_Alt()))
            using (var cmd = new SqlCommand(sb.ToString(), conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.Text;
                da.Fill(dt);
            }
            foreach (DataRow row in dt.Rows)
            {
                if (hash.ContainsKey("SX_" + dt.Rows[cnt]["CernerCode"].ToString().Trim()) == false)
                {
                    hash.Add("SX_" + dt.Rows[cnt]["CernerCode"].ToString().Trim(), dt.Rows[cnt]["MeditechCode"].ToString().Trim());
                }
                cnt = cnt + 1;
            }

            return hash;
        }
        public static Hashtable GetCERNER_LOCATION()
        {
            Hashtable hash = new Hashtable();
            int cnt = 0;

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT CernerCode,MeditechCode ");
            sb.Append("FROM  [CERNER_LOCATION] ");

            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(GlobalSettings.SqlConnectionString_Alt()))
            using (var cmd = new SqlCommand(sb.ToString(), conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.Text;
                da.Fill(dt);
            }
            foreach (DataRow row in dt.Rows)
            {
                if (hash.ContainsKey("LC_" + dt.Rows[cnt]["CernerCode"].ToString().Trim()) == false)
                {
                    hash.Add("LC_" + dt.Rows[cnt]["CernerCode"].ToString().Trim(), dt.Rows[cnt]["MeditechCode"].ToString().Trim());
                }
                cnt = cnt + 1;
            }

            return hash;
        }
        public static Hashtable GetCERNER_MARITALSTATUS()
        {
            Hashtable hash = new Hashtable();
            int cnt = 0;

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT CernerCode,MeditechCode ");
            sb.Append("FROM  [CERNER_MARITALSTATUS] ");

            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(GlobalSettings.SqlConnectionString_Alt()))
            using (var cmd = new SqlCommand(sb.ToString(), conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.Text;
                da.Fill(dt);
            }
            foreach (DataRow row in dt.Rows)
            {
                if (hash.ContainsKey("MS_" + dt.Rows[cnt]["CernerCode"].ToString().Trim()) == false)
                {
                    hash.Add("MS_" + dt.Rows[cnt]["CernerCode"].ToString().Trim(), dt.Rows[cnt]["MeditechCode"].ToString().Trim());
                }
                cnt = cnt + 1;
            }

            return hash;
        }
        public static Hashtable GetCERNER_PROVIDER_MAP()
        {
            Hashtable hash = new Hashtable();
            int cnt = 0;

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT Facility,ProviderNPI,MTMnemonic ");
            sb.Append("FROM  [CERNER_PROVIDER_MAP] ");

            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(GlobalSettings.SqlConnectionString_Alt()))
            using (var cmd = new SqlCommand(sb.ToString(), conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.Text;
                da.Fill(dt);
            }
            foreach (DataRow row in dt.Rows)
            {
                if (hash.ContainsKey("DR_" + dt.Rows[cnt]["Facility"].ToString().Trim() + "_" + dt.Rows[cnt]["ProviderNPI"].ToString().Trim()) == false)
                {
                    hash.Add("DR_" + dt.Rows[cnt]["Facility"].ToString().Trim() + "_" + dt.Rows[cnt]["ProviderNPI"].ToString().Trim(), dt.Rows[cnt]["MTMnemonic"].ToString().Trim());
                }
                cnt = cnt + 1;
            }

            return hash;
        }
        public static Hashtable GetCERNER_RACE()
        {
            Hashtable hash = new Hashtable();
            int cnt = 0;

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT CernerCode,MeditechCode ");
            sb.Append("FROM  [CERNER_RACE] ");

            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(GlobalSettings.SqlConnectionString_Alt()))
            using (var cmd = new SqlCommand(sb.ToString(), conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.Text;
                da.Fill(dt);
            }
            foreach (DataRow row in dt.Rows)
            {
                if (hash.ContainsKey("RC_" + dt.Rows[cnt]["CernerCode"].ToString().Trim()) == false)
                {
                    hash.Add("RC_" + dt.Rows[cnt]["CernerCode"].ToString().Trim(), dt.Rows[cnt]["MeditechCode"].ToString().Trim());
                }
                cnt = cnt + 1;
            }

            return hash;
        }
        public static Hashtable GetCERNER_REGTYPE()
        {
            Hashtable hash = new Hashtable();
            int cnt = 0;

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT CernerCode,MeditechCode ");
            sb.Append("FROM  [CERNER_REGTYPE] ");

            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(GlobalSettings.SqlConnectionString_Alt()))
            using (var cmd = new SqlCommand(sb.ToString(), conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.Text;
                da.Fill(dt);
            }
            foreach (DataRow row in dt.Rows)
            {
                if (hash.ContainsKey("PS_" + dt.Rows[cnt]["CernerCode"].ToString().Trim()) == false)
                {
                    hash.Add("PS_" + dt.Rows[cnt]["CernerCode"].ToString().Trim(), dt.Rows[cnt]["MeditechCode"].ToString().Trim());
                }
                cnt = cnt + 1;
            }

            return hash;
        }
        public static Hashtable GetCERNER_STATE()
        {
            Hashtable hash = new Hashtable();
            int cnt = 0;

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT CernerCode,MeditechCode ");
            sb.Append("FROM  [CERNER_STATE] ");

            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(GlobalSettings.SqlConnectionString_Alt()))
            using (var cmd = new SqlCommand(sb.ToString(), conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.Text;
                da.Fill(dt);
            }
            foreach (DataRow row in dt.Rows)
            {
                if (hash.ContainsKey("PS_" + dt.Rows[cnt]["CernerCode"].ToString().Trim()) == false)
                {
                    hash.Add("ST_" + dt.Rows[cnt]["CernerCode"].ToString().Trim(), dt.Rows[cnt]["MeditechCode"].ToString().Trim());
                }
                cnt = cnt + 1;
            }

            return hash;
        }
        public static string GetPRSNL_ALIAS(string prsnlID)
        {
            //ENCNTR_ALIAS:  ENCNTR_ID,ENCNTR_ALIAS_TYPE_CD,ALIAS,ACTIVE_IND,END_EFFECTIVE_DT_TM
            DateTime todayDt = DateTime.Today;
            string result = "";
            string activeInd = "1";
            string aliasTypeCd = "26026547";

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT ALIAS,END_EFFECTIVE_DT_TM ");
            sb.Append("FROM  [BAR_PRSNL_ALIAS] ");
            sb.Append("WHERE PERSON_ID = " + long.Parse(prsnlID) + " AND ACTIVE_IND = '" + activeInd + "' AND alias_pool_cd = '" + aliasTypeCd + "' ");
            sb.Append("AND END_EFFECTIVE_DT_TM > '" + todayDt + "'");

            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(GlobalSettings.SqlConnectionString()))
            using (var cmd = new SqlCommand(sb.ToString(), conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.Text;
                da.Fill(dt);
            }
            result = GlobalResult;
            foreach (DataRow row in dt.Rows)
            {
                result = dt.Rows[0]["ALIAS"].ToString();
                break;
            }
            return result;
        }
    }
}
