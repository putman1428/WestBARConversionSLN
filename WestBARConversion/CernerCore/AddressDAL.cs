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
    public class AddressDAL
    {
        public static Address GetHomeAddress(string personId)
        {
            string today = DateTime.Today.ToString("yyyyMMdd");
            int cnt = 0;
            DataTable dt = GetAddress(personId, "756");
            Address address = null;
            foreach (DataRow row in dt.Rows)
            {
                if(GetEffDt(dt.Rows[cnt]["end_effective_dt_tm"].ToString().Trim()) > int.Parse(today))
                {
                    address = LoadAddress(dt.Rows[cnt]);
                    break;
                }
                cnt = cnt + 1;
            }
            dt.Dispose();
            return address;
        }
        public static Address GetEmailAddress(string personId)
        {
            string today = DateTime.Today.ToString("yyyyMMdd");
            int cnt = 0;
            DataTable dt = GetAddress(personId, "755");
            Address address = null;
            foreach (DataRow row in dt.Rows)
            {
                if (GetEffDt(dt.Rows[cnt]["end_effective_dt_tm"].ToString().Trim()) > int.Parse(today))
                {
                    address = LoadAddress(dt.Rows[cnt]);
                    break;
                }
                cnt = cnt + 1;
            }
            dt.Dispose();
            return address;
        }
        public static Address GetAddress_Org(string orgId)
        {
            string today = DateTime.Today.ToString("yyyyMMdd");
            int cnt = 0;
            DataTable dt = GetAddress_Org(orgId, "754");
            Address address = null;
            foreach (DataRow row in dt.Rows)
            {
                if (GetEffDt(dt.Rows[cnt]["end_effective_dt_tm"].ToString().Trim()) > int.Parse(today))
                {
                    address = LoadAddress(dt.Rows[cnt]);
                    break;
                }
                cnt = cnt + 1;
            }
            dt.Dispose();
            return address;
        }
        private static DataTable GetAddress(string personId, string addressTypeCd)
        {
            string today = DateTime.Today.ToString("yyyyMMdd");
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT  street_addr, street_addr2, city, state_cd, zipcode, country_cd,end_effective_dt_tm ");
            sb.Append("FROM  BAR_ADDRESS ");
            sb.Append("WHERE parent_entity_id = " + long.Parse(personId) + " ");
            sb.Append("AND parent_entity_name = 'PERSON' ");
            sb.Append("AND Address_type_cd = '" + addressTypeCd + "' ");
            sb.Append("AND active_ind = '1' ");
            //sb.Append("AND end_effective_dt_tm > '" + today + "' ");

            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(GlobalSettings.SqlConnectionString()))
            using (var cmd = new SqlCommand(sb.ToString(), conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.Text;
                da.Fill(dt);
            }
            dt.Dispose();
            return dt;
        }
        private static DataTable GetAddress_Org(string orgId, string addressTypeCd)
        {
            string today = DateTime.Today.ToString("yyyyMMdd");

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT  street_addr, street_addr2, city, state_cd, zipcode, country_cd,end_effective_dt_tm  ");
            sb.Append("FROM  BAR_ADDRESS ");
            sb.Append("WHERE parent_entity_id = " + long.Parse(orgId) + " ");
            sb.Append("AND parent_entity_name = 'ORGANIZATION' ");
            sb.Append("AND Address_type_cd = '" + addressTypeCd + "' ");
            sb.Append("AND active_ind = '1' ");
            //sb.Append("AND end_effective_dt_tm > '" + today + "' ");

            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(GlobalSettings.SqlConnectionString()))
            using (var cmd = new SqlCommand(sb.ToString(), conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.Text;
                da.Fill(dt);
            }
            dt.Dispose();
            return dt;
        }
        private static Address LoadAddress(DataRow row)
        {
            Address address = new Address();
            address.Street_Addr = row["street_addr"].ToString();
            address.Street_Addr2 = row["street_addr2"].ToString();
            address.Street_Addr2 = row["street_addr2"].ToString();
            address.City = row["city"].ToString();
            address.Country = row["country_cd"].ToString();
            address.EndEffDt = row["end_effective_dt_tm"].ToString();

            string stateCd = CernerCommon.StripDecimalsForMap(row["state_cd"].ToString());
            address.State_Cd = CernerMapDAL.GetMap("CERNER_STATE", stateCd, stateCd);
            address.ZipCode = row["zipcode"].ToString();
            return address;
        }
        private static int GetEffDt(string dt)
        {
            string dtnum = Huron.HuronRoutines.FormatDate_YYYYMMDD(dt);
            if(Huron.HuronRoutines.IsNumeric(dtnum) == true)
                return int.Parse(dtnum);
            else
                return 0;
        }
    }
}
