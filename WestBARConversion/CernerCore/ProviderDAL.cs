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
    public class ProviderDAL
    {
        public static Provider GetProvider(string person_id)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT  person_id, name_last, name_first ");
            sb.Append("FROM  BAR_PRSNL ");
            sb.Append("WHERE person_id = " + long.Parse(person_id) + " ");
            sb.Append("AND active_ind = '1' ");

            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(GlobalSettings.SqlConnectionString()))
            using (var cmd = new SqlCommand(sb.ToString(), conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.Text;
                da.Fill(dt);
            }
            Provider provider = null;

            if (dt.Rows.Count > 0)
                provider = LoadProvider(dt.Rows[0]);
            dt.Dispose();
            return provider;
        }

        private static Provider LoadProvider(DataRow row)
        {
            Provider provider = new Provider();
            provider.PersonId = row["person_id"].ToString();
            provider.Name_Last = row["name_last"].ToString();
            provider.Name_First = row["name_first"].ToString();
            provider.Name_Middle = string.Empty;

            return provider;
        }
    }
}