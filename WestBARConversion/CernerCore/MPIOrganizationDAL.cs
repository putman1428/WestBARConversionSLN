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
    public class MPIOrganizationDAL
    {
        public static MPIOrganization GetOrganization(string organizationId)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT  organization_id, hcis, facility, mrnPrefix, acctPrefix, cernerPrefix ");
            //sb.Append("FROM  MPIOrganization_jan ");
            sb.Append("FROM  MPIOrganization ");
            sb.Append("WHERE organization_id = '" + organizationId + "' ");

            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(GlobalSettings.SqlConnectionString_Alt()))
            using (var cmd = new SqlCommand(sb.ToString(), conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.Text;
                da.Fill(dt);
            }
            MPIOrganization org = null;
            if (dt.Rows.Count > 0)
                org = LoadMPIOrganization(dt.Rows[0]);
            return org;
        }
        public static MPIOrganization GetOrganizationNew(string organizationId, string CernerPrefix)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT  organization_id, hcis, facility, mrnPrefix, acctPrefix, cernerPrefix ");
            //sb.Append("FROM  MPIOrganization_jan ");
            sb.Append("FROM  MPIOrganization ");
            sb.Append("WHERE organization_id = '" + organizationId + "' ");
            sb.Append("and CernerPrefix = '" + CernerPrefix + "' ");

            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(GlobalSettings.SqlConnectionString_Alt()))
            using (var cmd = new SqlCommand(sb.ToString(), conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.Text;
                da.Fill(dt);
            }
            MPIOrganization org = null;
            if (dt.Rows.Count > 0)
                org = LoadMPIOrganization(dt.Rows[0]);
            return org;
        }
        public static List<string> GetOrganizationsToProcess(string hcis, string hospFac)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT  organization_id, hcis, facility, mrnPrefix, acctPrefix, cernerPrefix ");
            //sb.Append("FROM  MPIOrganization_jan ");
            sb.Append("FROM  MPIOrganization ");
            sb.Append("WHERE hcis = '" + hcis + "' AND facility in (" + hospFac + ")");

            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(GlobalSettings.SqlConnectionString_Alt()))
            using (var cmd = new SqlCommand(sb.ToString(), conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.Text;
                da.Fill(dt);
            }

            List<string> orgs = new List<string>();
            foreach(DataRow row in dt.Rows)
            {
                orgs.Add(row["organization_id"].ToString());
            }
            return orgs;
        }
        private static MPIOrganization LoadMPIOrganization(DataRow row)
        {
            MPIOrganization org = new MPIOrganization();
            org.Organization_id = row["organization_id"].ToString();
            org.Hcis = row["hcis"].ToString();
            org.Facility= row["facility"].ToString();
            org.MRNPrefix = row["mrnPrefix"].ToString();
            org.AcctPrefix = row["acctPrefix"].ToString();
            org.CernerPrefix = row["cernerPrefix"].ToString();
            return org;
        }
    }
}