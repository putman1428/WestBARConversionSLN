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
    public class PrsnlAliasDAL
    {
        //public static List<Prsnl_Alias> GetPrsnlAliases(string personId)
        //{
        //    if (personId.Contains(".0000") == false)
        //        personId = personId + ".0000";
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append("SELECT  person_alias_type_cd, alias_pool_Cd, alias ");
        //    sb.Append("FROM  PERSON_ALIAS ");
        //    sb.Append("WHERE person_id = '" + personId + "' ");
        //    sb.Append("AND prsnl_alias_type_cd  <> '4038127' ");
        //    sb.Append("AND active_ind = '1' ");

        //    DataTable dt = new DataTable();
        //    using (var conn = new SqlConnection(GlobalSettings.SqlConnectionString()))
        //    using (var cmd = new SqlCommand(sb.ToString(), conn))
        //    using (var da = new SqlDataAdapter(cmd))
        //    {
        //        cmd.CommandType = CommandType.Text;
        //        da.Fill(dt);
        //    }
        //    List<Prsnl_Alias> aliases = new List<Prsnl_Alias>();
        //    foreach(DataRow row in dt.Rows)
        //    {
        //        Prsnl_Alias pa = LoadPrsnlAlias(row);
        //        aliases.Add(pa);
        //    }
        //    dt.Dispose();
        //    return aliases;
        //}

        public static string GetPrsnlAlias(string personId)
        {
            string returnval = "";
            DateTime todayDt = DateTime.Today;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT  prsnl_alias_type_cd, alias_pool_Cd, alias ");
            sb.Append("FROM  BAR_PRSNL_ALIAS ");
            sb.Append("WHERE person_id = " + long.Parse(personId) + " ");
            sb.Append("AND PRSNL_ALIAS_TYPE_CD  = '4038127' ");
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
            List<Prsnl_Alias> aliases = new List<Prsnl_Alias>();
            foreach (DataRow row in dt.Rows)
            {
                Prsnl_Alias pa = LoadPrsnlAlias(row);
                //aliases.Add(pa);
                returnval = pa.Alias;
                break;
            }
            dt.Dispose();
            return returnval;
        }

        public static string GetPrsnlAliasLic(string personId)
        {
            string returnval = "";
            DateTime todayDt = DateTime.Today;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT  prsnl_alias_type_cd, alias_pool_Cd, alias ");
            sb.Append("FROM  BAR_PRSNL_ALIAS ");
            sb.Append("WHERE person_id = " + long.Parse(personId) + " ");
            sb.Append("AND alias_pool_cd  = '275985665' ");
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
            List<Prsnl_Alias> aliases = new List<Prsnl_Alias>();
            foreach (DataRow row in dt.Rows)
            {
                Prsnl_Alias pa = LoadPrsnlAlias(row);
                //aliases.Add(pa);
                returnval = pa.Alias;
                break;
            }
            dt.Dispose();
            return returnval;
        }

        private static Prsnl_Alias LoadPrsnlAlias(DataRow row)
        {
            Prsnl_Alias alias = new Prsnl_Alias();
            alias.Aleas_Pool = CodeValueDAL.GetCodeValueDisplay(row["alias_pool_Cd"].ToString());
            alias.Alias_Type = CodeValueDAL.GetCodeValueDisplay(row["prsnl_alias_type_cd"].ToString());
            alias.Alias = row["alias"].ToString();

            return alias;
        }
        
    }
}
