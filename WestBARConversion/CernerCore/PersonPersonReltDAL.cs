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
    public class PersonPersonReltDAL
    {
       
        public static string GetRelationship(string personId)
        {
            DateTime todayDt = DateTime.Today;
            string item = "";
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT TOP 1  related_person_reltn_cd ");
            sb.Append("FROM  BAR_PERSON_PERSON_RELTN ");
            sb.Append("WHERE Person_Id = " + long.Parse(personId) + " ");
            sb.Append("AND person_reltn_type_cd = '1158' ");
            sb.Append("AND active_ind = '1' ");
            sb.Append("AND END_EFFECTIVE_DT_TM > '" + todayDt + "'");
            sb.Append("ORDER BY beg_effective_dt_tm DESC");

            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(GlobalSettings.SqlConnectionString()))
            using (var cmd = new SqlCommand(sb.ToString(), conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.Text;
                da.Fill(dt);
            }

            if (dt.Rows.Count > 0)
                item = dt.Rows[0][0].ToString();
            dt.Dispose();
            return item;
        }
        private static List<Person> LoadPersons(DataTable dt)
        {
            List<Person> persons = new List<Person>();
            foreach(DataRow row in dt.Rows)
            {
                string personId = row["related_person_id"].ToString();
                string relationship = CernerCommon.StripDecimalsForMap(row["related_person_reltn_cd"].ToString());

                Person person = PersonDAL.GetPerson(personId);
                if (person != null)
                {
                    person.Cerner_Relationship = CodeValueDAL.GetCodeValueDisplay(relationship);
                    person.RelationShip = CernerMapDAL.GetMap("CERNER_RELATIONSHIP", relationship, relationship);
                    persons.Add(person);
                }
            }
            return persons;
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
