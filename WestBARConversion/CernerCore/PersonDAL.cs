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
    public class PersonDAL
    {
        /// <summary>
        /// Stored procedure called to load persion information from database
        /// </summary>
        /// <param name="personId"></param>
        /// <returns>Returns a person object to be used by conversion</returns>
        public static Person GetPerson(string personId)
        {
            DateTime todayDt = DateTime.Today;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT Person_Id, Name_Full_Formatted, Birth_Dt_Tm, Sex_Cd, Mother_Maiden_Name, Vip_Cd, ");
            sb.Append("Deceased_Dt_Tm, Deceased_Tz, Religion_Cd, Marital_Type_Cd, Race_Cd, Language_Cd, ");
            sb.Append("Name_Last, Name_First, Name_Middle, ethnic_grp_cd ");
            sb.Append("FROM BAR_PERSON ");
            sb.Append("WHERE Person_Id = " + int.Parse(personId) + " ");
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

            Person person = null;
            if (dt.Rows.Count > 0)
                person = LoadPerson(dt.Rows[0]);
            dt.Dispose();
            return person;
        }

        private static Person LoadPerson(DataRow row)
        {
            Person person = new Person();
            person.PersonId = row["Person_Id"].ToString();
            person.Birth_Dt_Tm = row["Birth_Dt_Tm"].ToString();
            person.Deceased_Dt_Tm = row["Deceased_Dt_Tm"].ToString();
            person.Deceased_Tz = row["Deceased_Tz"].ToString();

            string languageCd = CernerCommon.StripDecimalsForMap(row["Language_Cd"].ToString());
            person.Language_Cd = CernerMapDAL.GetMap("CERNER_LANGUAGE",languageCd, languageCd);

            string maritalStatus = CernerCommon.StripDecimalsForMap(row["Marital_Type_Cd"].ToString());
            person.Marital_Type_Cd = CernerMapDAL.GetMap("CERNER_MARITALSTATUS", maritalStatus, maritalStatus);


            person.Mother_Maiden_Name = row["Mother_Maiden_Name"].ToString();
            person.Name_Full_Formatted = row["Name_Full_Formatted"].ToString();

            string raceCd = CernerCommon.StripDecimalsForMap(row["Race_Cd"].ToString());
            person.Race_Cd = CernerMapDAL.GetMap("CERNER_RACE",raceCd, raceCd);

            string religionCd = CernerCommon.StripDecimalsForMap(row["Religion_Cd"].ToString());
            person.Religion_Cd = CernerMapDAL.GetMap("CERNER_RELIGION",religionCd, religionCd);

            string sexCd = CernerCommon.StripDecimalsForMap(row["Sex_Cd"].ToString());
            person.Sex_Cd = CernerMapDAL.GetMap("CERNER_GENDER",sexCd, sexCd);

            person.Vip_Cd = CernerCommon.StripDecimalsForMap(row["Vip_Cd"].ToString());

            person.SocialSecurityNumber = PersonAliasDAL.GetPersonAlias_SSN(person.PersonId);

            person.HomeAddress = AddressDAL.GetHomeAddress(person.PersonId);
            person.EmailAddress = AddressDAL.GetEmailAddress(person.PersonId);

            person.HomePhone = PhoneDAL.GetHomePhone(person.PersonId);
            //person.OtherPhone = PhoneDAL.GetOtherPhone(person.PersonId);
            person.MobilePhone = PhoneDAL.GetMobilePhone(person.PersonId);
            person.WorkPhone = PhoneDAL.GetWorkPhone(person.PersonId);

            person.Name_First = row["Name_First"].ToString().ToUpper();
            person.Name_Last = row["Name_Last"].ToString().ToUpper();
            person.Name_Middle = CheckCode(row["Name_Middle"].ToString().ToUpper());

            string ethnicGroupCd = CernerCommon.StripDecimalsForMap(row["ethnic_grp_cd"].ToString());
            person.Ethnic_Group_Cd = CernerMapDAL.GetMap("CERNER_ETHNIC_GROUP_CD", ethnicGroupCd, ethnicGroupCd);

            person.PrimaryCarePhysician = "";// Person_Prsnl_ReltnDAL.GetPrimaryCarePhysician(person.PersonId);
            person.PrimaryCarePhysicianName = "";// Person_Prsnl_ReltnDAL.GetPrimaryCarePhysicianName(person.PersonId);
            return person;
        }
        public static string CheckCode(string cd)
        {
            if (cd.Trim() == "0" || cd.Trim().ToUpper() == "NULL")
                return "";
            else
                return cd;

        }
    }
}
