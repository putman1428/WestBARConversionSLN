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
    public class Encntr_Plan_ReltnDAL
    {
        public static List<Encntr_Plan_Reltn> GetEncounterPlans(string encounterId)
        {
            int cnt = 0;
            string today = DateTime.Today.ToString("yyyyMMdd");
            StringBuilder sb = new StringBuilder();
            sb.Append("Select encntr_id, person_id, person_plan_reltn_id, health_plan_id, organization_id,person_org_reltn_id,Beg_Effective_Dt_Tm,group_name,group_nbr, ");
            sb.Append("priority_seq,member_nbr, deduct_amt,verify_status_cd, verify_dt_tm,policy_nbr,member_nbr,subs_member_nbr,end_effective_dt_tm ");
            sb.Append("FROM BAR_Encntr_plan_reltn ");
            sb.Append("where encntr_Id = " + long.Parse(encounterId) + " and active_ind = '1' ");
            //sb.Append("AND END_EFFECTIVE_DT_TM > '" + todayDt + "'");
            sb.Append(" order by priority_seq ");

            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(GlobalSettings.SqlConnectionString()))
            using (var cmd = new SqlCommand(sb.ToString(), conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }

            List<Encntr_Plan_Reltn> plans = new List<Encntr_Plan_Reltn>();
            foreach (DataRow row in dt.Rows)
            {
                if (GetEffDt(dt.Rows[cnt]["end_effective_dt_tm"].ToString().Trim()) > int.Parse(today))
                {
                    Encntr_Plan_Reltn plan = LoadEncPlans(row);
                    if (plan != null)
                    {
                        plans.Add(plan);
                    }
                }
                cnt = cnt + 1;
            }
            return plans;
        }
        private static Encntr_Plan_Reltn LoadEncPlans(DataRow row)
        {
            Encntr_Plan_Reltn plan = new Encntr_Plan_Reltn();
            plan.InsuranceCode = row["health_plan_id"].ToString().Trim();
            plan.InsuranceName = Health_PlanDAL.HealthPlanName(row["health_plan_id"].ToString().Trim()).Trim();
            plan.Address = AddressDAL.GetAddress_Org(row["organization_id"].ToString().Trim());
            plan.InsurancePhone = PhoneDAL.GetWorkPhoneOrganization(row["organization_id"].ToString().Trim());
            plan.GroupName = row["group_name"].ToString().Trim();
            plan.GroupNbr = row["group_nbr"].ToString().Trim();
            plan.SequenceNbr = row["priority_seq"].ToString().Trim();
            plan.PolicyNbr = row["policy_nbr"].ToString().Trim();
            plan.SubscriberPhone = PhoneDAL.GetHomePhone(row["person_id"].ToString().Trim());
            plan.SubscriberAddress = AddressDAL.GetHomeAddress(row["person_id"].ToString().Trim());
            plan.EffectiveDate = row["Beg_Effective_Dt_Tm"].ToString().Trim();
            plan.SubscriberPerson = PersonDAL.GetPerson(row["person_id"].ToString().Trim());
            plan.SubPolicyNbr = "";// row["subs_member_nbr"].ToString().Trim();
            plan.SubscriberEmailAddress = AddressDAL.GetEmailAddress(row["person_id"].ToString().Trim());
            plan.StatusDate = row["verify_dt_tm"].ToString().Trim();
            plan.InsEligibilitySts = row["verify_status_cd"].ToString().Trim();
            plan.Relationship = PersonPersonReltDAL.GetRelationship(row["person_id"].ToString().Trim());
            plan.CoverageNbr = row["member_nbr"].ToString().Trim();
            plan.Authorizations = AuthorizationDAL.GetAuthorizations(row["encntr_id"].ToString().Trim(), row["health_plan_id"].ToString().Trim(),1);
            return plan;
        }
            private static int GetEffDt(string dt)
            {
                string dtnum = Huron.HuronRoutines.FormatDate_YYYYMMDD(dt);
                if (Huron.HuronRoutines.IsNumeric(dtnum) == true)
                    return int.Parse(dtnum);
                else
                    return 0;
            }
        }
}