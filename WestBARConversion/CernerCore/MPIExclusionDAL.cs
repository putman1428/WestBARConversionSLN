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
    public class MPIExclusionDAL
    {
        public static List<string> GetInactivePersonIds()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT  person_id + '.0000' ");
            sb.Append("FROM  MPIExclusion ");

            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(GlobalSettings.SqlConnectionString()))
            using (var cmd = new SqlCommand(sb.ToString(), conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.Text;
                da.Fill(dt);
            }
            List<string> ids = new List<string>();
            foreach(DataRow row in dt.Rows)
            {
                ids.Add(row[0].ToString());
            }
            return ids;
        }
    }
}