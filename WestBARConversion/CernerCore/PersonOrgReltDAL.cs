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
    public class PersonOrgReltDAL
    {

        public static Employer GetEmployer(string personId)
        {
            DateTime todayDt = DateTime.Today;

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT  PERSON_ORG_RELTN_ID, PERSON_ID, PERSON_ORG_RELTN_CD, ORGANIZATION_ID, EMPL_STATUS_CD, PERSON_ORG_NBR, EMPL_OCCUPATION_TEXT, ");
            sb.Append("EMPL_POSITION, PRIORITY_SEQ, PERSON_ORG_ALIAS ");
            sb.Append("FROM BAR_PERSON_ORG_RELTN  ");
            sb.Append("WHERE Person_Id = " + int.Parse(personId) + " ");
            sb.Append("AND PERSON_ORG_RELTN_CD = '1136' ");
            sb.Append("AND active_ind = '1' ");
            sb.Append("AND END_EFFECTIVE_DT_TM > '" + todayDt + "'");
            sb.Append(" ORDER BY PRIORITY_SEQ");

            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(GlobalSettings.SqlConnectionString()))
            using (var cmd = new SqlCommand(sb.ToString(), conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.Text;
                da.Fill(dt);
            }

            Employer emp = LoadEmpData(dt);
            return emp;
        }

        private static Employer LoadEmpData(DataTable dt)
        {
            Employer emp = new Employer();
            foreach(DataRow row in dt.Rows)
            {
                string orgId = row["Organization_ID"].ToString();
                string workpos = row["EMPL_OCCUPATION_TEXT"].ToString();
                if (workpos == "null") workpos = "";
                emp.EmploymentStatus = row["EMPL_STATUS_CD"].ToString();
                emp.Occupation = workpos;
                emp.EmployerName = OrganizationDAL.GetOrganizationName(orgId);
                emp.WorkAddress = AddressDAL.GetAddress_Org(orgId);
                emp.EmployerPhone = PhoneDAL.GetWorkPhoneOrganization(orgId);
                emp.EmployeeID = "";
            }
            return emp;
        }

        private static Person LoadPersons(DataRow row)
        {
            string personId = row["related_person_id"].ToString();
            string relationship = CernerCommon.StripDecimalsForMap(row["related_person_reltn_cd"].ToString());

            Person person = PersonDAL.GetPerson(personId);
            if (person != null)
            {
                person.Cerner_Relationship = CodeValueDAL.GetCodeValueDisplay(relationship);
                person.RelationShip = CernerMapDAL.GetMap("CERNER_RELATIONSHIP", relationship, relationship);
            }
            return person;
        }
    }
}
