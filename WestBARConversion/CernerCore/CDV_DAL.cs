using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WestBARConversion.CernerModel;
using System.Collections;
using System.Globalization;

namespace WestBARConversion.CernerCore
{
    public class CDV_DAL
    {
        public static Hashtable GetExcludedUsers()
        {
            int cnt = 0;
            Hashtable result = new Hashtable();

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT person_id ");
            sb.Append("FROM  [MPIExclusion] ");

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
                //result.Add(dt.Rows[cnt]["person_id"].ToString(), "");
                result.Add(row[0].ToString(), "");
                cnt = cnt + 1;
            }
            return result;
        }

        public static DateTime? GetCernerDate(string dateValue, string hcis)
        {
            string timeZone = GetHCISTimeZone(hcis);
            DateTime? dt2 = null;

            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            DateTime? dt = GetCernerDate(dateValue);
            if (dt.HasValue)
                dt2 = TimeZoneInfo.ConvertTimeFromUtc(dt.Value, tzi);
            return dt2;
        }
        public static string GetHCISTimeZone(string hcis)
        {
            string result = string.Empty;
            if (hcis == "SHAZ" || hcis == "SHUT")
                result = "US Mountain Standard Time";
            else
                result = "Central Standard Time";

            return result;
        }
        public static DateTime? GetCernerDate(string dateValue)
        {
            string AMPM = "";
            string item = "";
            DateTime? result = null;
            if (dateValue.Trim().Length > 0 && dateValue != "null")
            {
                if (dateValue.Contains("AM") == true || dateValue.Contains("PM") == true)
                {
                    DateTime dt = DateTime.Parse(dateValue);
                    if (dateValue.Contains("AM") == true)
                        AMPM = "AM";

                    else if (dateValue.Contains("PM") == true)
                    {
                        AMPM = "PM";
                        if (dt.Hour >= 21)
                            item = item;
                    }
                    item = dt.Month.ToString().PadLeft(2, '0') + "/" + dt.Day.ToString().PadLeft(2, '0') + "/" + dt.Year.ToString() + " " + dt.Hour.ToString().PadLeft(2, '0') + ":" + dt.Minute.ToString().PadLeft(2, '0') + ":" + dt.Second.ToString().PadLeft(2, '0') + " " + AMPM;

                    result = DateTime.ParseExact(item, "MM/dd/yyyy HH:mm:ss tt", CultureInfo.CurrentCulture);
                    //result = DateTime.ParseExact(dateValue, "M/d/yyyy H:m:s tt", CultureInfo.CurrentCulture); //DateTime.ParseExact(dateValue, "M/d/yyyy HH:mm:ss tt", CultureInfo.CurrentCulture);
                }
                else if (dateValue.Contains(":") == false)
                    result = DateTime.ParseExact(dateValue, "M/d/yyyy", CultureInfo.CurrentCulture);
                else
                    result = DateTime.ParseExact(dateValue, "yyyy-MM-dd HH:mm:ss", CultureInfo.CurrentCulture);
            }
            return result;
        }
        public static string GetCernerDate2(string dateValue, string hcis)
        {
            string newdt = "";
            string timeZone = GetHCISTimeZone(hcis);
            DateTime? dt2 = null;

            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            DateTime? dt = GetCernerDate(dateValue);
            if (dt.HasValue)
            {
                dt2 = TimeZoneInfo.ConvertTimeFromUtc(dt.Value, tzi);
                if (hcis == "SHAZ" || hcis == "SHUT")
                {
                    if (dt2.Value.Hour > 22)
                        newdt = newdt;
                    DateTime dt3 = (DateTime)dt2;
                    dt3 = dt3.AddHours(3);
                    dt2 = dt3;
                }
                newdt = dt2.Value.Year.ToString() + dt2.Value.Month.ToString().PadLeft(2, '0').Trim() + dt2.Value.Day.ToString().PadLeft(2, '0').Trim();
            }
            return newdt;
        }
        public static string GetCernerTime(string dateValue, string hcis)
        {
            string newdt = "";
            string timeZone = GetHCISTimeZone(hcis);
            DateTime? dt2 = null;

            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            DateTime? dt = GetCernerDate(dateValue);
            if (dt.HasValue)
            {

                dt2 = TimeZoneInfo.ConvertTimeFromUtc(dt.Value, tzi);
                if (hcis == "SHAZ" || hcis == "SHUT")
                {
                    if (dt2.Value.Hour > 22)
                        newdt = newdt;
                    DateTime dt3 = (DateTime)dt2;
                    dt3 = dt3.AddHours(3);
                    dt2 = dt3;
                }
                newdt = dt2.Value.Hour.ToString().PadLeft(2, '0').Trim() + dt2.Value.Minute.ToString().PadLeft(2, '0').Trim();
            }
            return newdt;
        }
    }
}