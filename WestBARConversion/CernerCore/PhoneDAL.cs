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
    public class PhoneDAL
    {
        public static Phone GetHomePhone(string personId)
        {
            DataTable dt = GetPhone(personId, "170");
            Phone phone = new Phone();
            if (dt.Rows.Count > 0)
                phone = LoadPhone(dt.Rows[0]);
            dt.Dispose();
            return phone;
        }
        public static Phone GetWorkPhone(string personId)
        {
            DataTable dt = GetPhone(personId, "163");
            Phone phone = new Phone();
            if (dt.Rows.Count > 0)
                phone = LoadPhone(dt.Rows[0]);
            dt.Dispose();
            return phone;
        }
        public static Phone GetWorkPhoneOrganization(string orgId)
        {
            DataTable dt = GetPhoneOrg(orgId, "163");
            Phone phone = new Phone();
            if (dt.Rows.Count > 0)
                phone = LoadPhone(dt.Rows[0]);
            dt.Dispose();
            return phone;
        }
        public static Phone GetMobilePhone(string personId)
        {
            DataTable dt = GetPhone(personId, "4149712");
            Phone phone = new Phone();
            if (dt.Rows.Count > 0)
                phone = LoadPhone(dt.Rows[0]);
            dt.Dispose();
            return phone;
        }
        //public static Phone GetOtherPhone(string personId)
        //{
        //    DataTable dt = GetPhone(personId, "4149712");
        //    Phone phone = new Phone();
        //    if (dt.Rows.Count > 0)
        //        phone = LoadPhone(dt.Rows[0]);
        //    else
        //    {
        //        dt = GetPhone(personId, "163");
        //        if (dt.Rows.Count > 0)
        //            phone = LoadPhone(dt.Rows[0]);
        //    }
        //    return phone;
        //}
        


        private static DataTable GetPhone(string personId, string phoneTypeCd)
        {
            DateTime todayDt = DateTime.Today;
            
            string today = DateTime.Today.ToString("yyyy-MM-dd");

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT  Phone_Num, phone_format_cd ");
            sb.Append("FROM  BAR_PHONE ");
            sb.Append("WHERE parent_entity_id = " + long.Parse(personId) + " ");
            sb.Append("AND parent_entity_name = 'PERSON' ");
            sb.Append("AND Phone_Type_Cd = '" + phoneTypeCd+ "' ");
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
            return dt;
        }
        private static DataTable GetPhoneOrg(string orgId, string phoneTypeCd)
        {
            DateTime todayDt = DateTime.Today;
            
            string today = DateTime.Today.ToString("yyyy-MM-dd");

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT  Phone_Num, phone_format_cd ");
            sb.Append("FROM  BAR_PHONE ");
            sb.Append("WHERE parent_entity_id = " + long.Parse(orgId) + " ");
            sb.Append("AND parent_entity_name = 'ORGANIZATION' ");
            sb.Append("AND Phone_Type_Cd = '" + phoneTypeCd + "' ");
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
            return dt;
        }
        private static Phone LoadPhone(DataRow row)
        {
            Phone phone = new Phone();
            string p = row["Phone_Num"].ToString();
            string pFormat = row["phone_format_cd"].ToString();
            if (pFormat == "873" || pFormat == "")
            {
                if (!HasLetters(p))
                    phone.Phone_Num = p;
            }
            else
            {
                phone.Phone_Num = p;
            }
            if (phone.Phone_Num != null)
            {
                if (phone.Phone_Num.Contains("(") == false)
                {
                    if (phone.Phone_Num.Length >= 10)
                        phone.Phone_Num = "(" + phone.Phone_Num.Substring(0, 3) + ")" + phone.Phone_Num.Substring(3, 3) + "-" + phone.Phone_Num.Substring(6);
                }
            }
            return phone;
        }

        private static bool HasLetters(string phoneNbr)
        {
            bool result = false;
            Char[] chars = phoneNbr.ToCharArray();
            foreach(char c in chars)
            {
                if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c < 'z'))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }
    }
}
