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
    public class EncounterDAL
    {
        /// <summary>
        /// Method calls stored procedure that gets a list of distinct person Ids
        /// starting with the first person id > the lastPersonid proceesed
        /// Ids come from encounter so we know that the person Id is a patient
        /// </summary>
        /// <param name="lastPersonId"></param>
        /// <param name="numberOfRows"></param>
        /// <returns></returns>
        public static DataTable GetPersonIds(string lastPersonId, int numberOfRows)
        {
            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(GlobalSettings.SqlConnectionString()))
            using (var cmd = new SqlCommand("proc_encounter_person_ids", conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@LastPersonId", lastPersonId);
                cmd.Parameters.AddWithValue("@RowCountPerPage", numberOfRows);
                da.Fill(dt);
            }
            return dt;
        }
        public static List<string> GetDistinctPersonIds(List<string> organizations)
        {
            DateTime todayDt = DateTime.Today;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT DISTINCT Person_Id ");
            sb.Append("FROM BAR_ENCOUNTER ");
            sb.Append("WHERE active_ind = '1' ");
            sb.Append("AND (encntr_type_cd <> '22282402' ");   //omit clinic
            sb.Append("AND encntr_type_cd <> '20058643' ");    //omit history
            sb.Append("AND encntr_type_cd <> '309313' ");      //omit preAdmit
            sb.Append("AND encntr_type_cd <> '275211155' ");   //omit preClient
            sb.Append("AND encntr_type_cd <> '40161279' ");    //omit PreRecurring
            sb.Append("AND encntr_type_cd <> '4189852' ");     //omit PreReg
            sb.Append("AND encntr_type_cd <> '33151117') ");   //omit ZZPresched 
            sb.Append("AND END_EFFECTIVE_DT_TM > '" + todayDt + "'");

            StringBuilder sbOrgs = new StringBuilder();
            if (organizations.Count > 0)
            {
                sbOrgs.Append("AND (");
                int cnt = 0;
                foreach (string org in organizations)
                {
                    cnt++;
                    sbOrgs.Append(" Organization_id = '" + org + "' ");
                    if (cnt < organizations.Count)
                        sbOrgs.Append(" OR ");
                }
                sbOrgs.Append(") ");
                sb.Append(sbOrgs.ToString());
            }
            sb.Append("ORDER BY Person_id ");

            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(GlobalSettings.SqlConnectionString()))
            using (var cmd = new SqlCommand(sb.ToString(), conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.Text;
                da.Fill(dt);
            }

            List<string> personIds = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                personIds.Add(row[0].ToString());
            }
            return personIds;
        }

        /// <summary>
        /// This method is used by bar so we can files by their hcis 
        /// using the list of organizations
        /// </summary>
        /// <param name="organizations"></param>
        /// <returns></returns>
        public static List<string> GetDistinctEncounterIds(List<string> organizations)
        {
            DateTime todayDt = DateTime.Today;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT DISTINCT encntr_id ");
            sb.Append("FROM ENCOUNTER ");
            sb.Append("WHERE active_ind = '1.0000' ");
            sb.Append("AND (encntr_type_cd <> '22282402.0000' ");   //omit clinic
            sb.Append("AND encntr_type_cd <> '20058643.0000' ");    //omit history
            sb.Append("AND encntr_type_cd <> '309313.0000' ");      //omit preAdmit
            sb.Append("AND encntr_type_cd <> '275211155.0000' ");   //omit preClient
            sb.Append("AND encntr_type_cd <> '40161279.0000' ");    //omit PreRecurring
            sb.Append("AND encntr_type_cd <> '4189852.0000' ");     //omit PreReg
            sb.Append("AND encntr_type_cd <> '33151117.0000') ");   //omit ZZPresched 
            sb.Append("AND encntr_type_cd <> '275211141.0000') ");   //omit Client
            //sb.Append("AND END_EFFECTIVE_DT_TM > '" + todayDt + "' ");
            //sb.Append ("AND Encntr_id in (117641314,105523759,114806365)");

            StringBuilder sbOrgs = new StringBuilder();
            if (organizations.Count > 0)
            {
                sbOrgs.Append("AND (");
                int cnt = 0;
                foreach (string org in organizations)
                {
                    cnt++;
                    sbOrgs.Append(" Organization_id = '" + org + ".0000' ");
                    if (cnt < organizations.Count)
                        sbOrgs.Append(" OR ");
                }
                sbOrgs.Append(") ");
                sb.Append(sbOrgs.ToString());
            }
            sb.Append("ORDER BY encntr_id ");

            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(GlobalSettings.SqlConnectionString_Alt()))
            using (var cmd = new SqlCommand(sb.ToString(), conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 1800;
                da.Fill(dt);
            }

            List<string> personIds = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                personIds.Add(row[0].ToString());
            }
            return personIds;
        }

        public static List<string> GetDistinctEncounterIds2(List<string> organizations)
        {
            DateTime todayDt = DateTime.Today;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT DISTINCT encntr_id ");
            sb.Append("FROM BAR_ENCOUNTER ");
            sb.Append("WHERE active_ind = '1' ");
            sb.Append("AND (encntr_type_cd <> '22282402' ");   //omit clinic
            sb.Append("AND encntr_type_cd <> '20058643' ");    //omit history
            sb.Append("AND encntr_type_cd <> '309313' ");      //omit preAdmit
            sb.Append("AND encntr_type_cd <> '275211155' ");   //omit preClient
            sb.Append("AND encntr_type_cd <> '40161279' ");    //omit PreRecurring
            sb.Append("AND encntr_type_cd <> '4189852' ");     //omit PreReg
            sb.Append("AND encntr_type_cd <> '33151117') ");   //omit ZZPresched 
            sb.Append("AND END_EFFECTIVE_DT_TM > '" + todayDt + "' ");
            //sb.Append ("AND Encntr_id in (117641314,105523759,114806365)");

            StringBuilder sbOrgs = new StringBuilder();
            if (organizations.Count > 0)
            {
                sbOrgs.Append("AND (");
                int cnt = 0;
                foreach (string org in organizations)
                {
                    cnt++;
                    sbOrgs.Append(" Organization_id = '" + org + "' ");
                    if (cnt < organizations.Count)
                        sbOrgs.Append(" OR ");
                }
                sbOrgs.Append(") ");
                sb.Append(sbOrgs.ToString());
            }
            sb.Append("ORDER BY encntr_id ");

            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(GlobalSettings.SqlConnectionString()))
            using (var cmd = new SqlCommand(sb.ToString(), conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 1800;
                da.Fill(dt);
            }

            List<string> personIds = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                personIds.Add(row[0].ToString());
            }
            return personIds;
        }

        public static Encounter GetEncounter(string encounterId)
        {
            DateTime todayDt = DateTime.Today;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT encntr_id, Person_Id, Encntr_Type_Cd, Encntr_type_class_cd, Inpatient_Admit_Dt_Tm, Reg_Dt_Tm, ");
            sb.Append("Disch_Dt_tm, Loc_Nurse_Unit_Cd, Reason_For_Visit, Disch_Disposition_Cd, Active_Ind, organization_id, ");
            sb.Append("admit_type_cd, admit_src_cd, vip_cd,confid_level_cd,med_service_cd  ");
            sb.Append("FROM ENCOUNTER ");
            sb.Append("WHERE encntr_id = " + encounterId + ".0000 ");
            sb.Append("AND active_ind = '1.0000' ");
            //sb.Append("AND END_EFFECTIVE_DT_TM > '" + todayDt + "'");

            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(GlobalSettings.SqlConnectionString_Alt()))
            using (var cmd = new SqlCommand(sb.ToString(), conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.Text;
                da.Fill(dt);
            }

            Encounter encounter = null;
            if (dt.Rows.Count > 0)
                encounter = LoadEncounter(dt.Rows[0]);

            return encounter;
        }

        private static Encounter LoadEncounter(DataRow row)
        {
            string npi = "";
            Encounter encounter = new Encounter();
            encounter.Active_Ind = row["Active_Ind"].ToString().Replace(".0000","");

            string dischDisp = CernerCommon.StripDecimalsForMap(row["Disch_Disposition_Cd"].ToString());
            encounter.Disch_Disposition_Cd = CernerMapDAL.GetMap("CERNER_DISCHARGE_DISPOSITION", dischDisp, dischDisp);
            encounter.Cerner_Disch_Disposition_Cd_Display = CodeValueDAL.GetCodeValueDisplay(dischDisp);

            encounter.Disch_Dt_Tm = row["Disch_Dt_Tm"].ToString();
            encounter.Encntr_type_class_cd = row["Encntr_type_class_cd"].ToString().Replace(".0000", "");

            encounter.Original_Encounter_Id = row["Encntr_Id"].ToString().Replace(".0000", "");
            string encntrId = Encounter_AliasDAL.GetEncounterVisitNumber(row["Encntr_Id"].ToString().Replace(".0000", ""));
            encounter.Encounter_Id = encntrId;

            string encounterTypeCd = CernerCommon.StripDecimalsForMap(row["Encntr_Type_Cd"].ToString());
            encounter.Cerner_Encoutner_Type_Cd = encounterTypeCd; //used for special logic in mpi conversion record 02
            encounter.Cerner_Encounter_Type_Cd_Display = CodeValueDAL.GetCodeValueDisplay(encounterTypeCd);  //for data sheet

            encounter.Encounter_Type_Cd = CernerMapDAL.GetMap("CERNER_REGTYPE", encounterTypeCd, encounterTypeCd);
            encounter.Inpatient_Admit_Dt_Tm = row["Inpatient_Admit_Dt_Tm"].ToString();

            //From Encounter Table use field E_LOC_NURSE_UNIT_CD to retrieve the  DISPLAY_KEY (col E) in CodeSet 220.  
            //Use the DISPLAY_KEY to retrieve the Meditech location from the spreadsheet - "Approved locations"
            string loc = CodeValueDAL.GetCodeValueDisplay_Key(row["Loc_Nurse_Unit_Cd"].ToString().Replace(".0000", ""));
            encounter.Cerner_Loc_Nurse_Unit_Cd_Display = loc; //for data sheet
            encounter.Loc_Nurse_Unit_Cd = CernerMapDAL.GetMap("CERNER_LOCATION", loc, loc);

            //string attendPhys = EncntrPrsnlReltnDAL.GetAttendPhysician(row["Encntr_Id"].ToString());
            encounter.AttendPhysicianId = ""; //attendPhys;
            //MPIOrganization org = MPIOrganizationDAL.GetOrganization(CernerCommon.StripDecimalsForMap(encounter.Organization_Id));
            //npi = PrsnlAliasDAL.GetPrsnlAlias(attendPhys);
            encounter.AttendPhysician = "";//CernerMapDAL.GetMapDR(npi, facility, "Z.CONVPROV");

            encounter.Person_Id = row["Person_Id"].ToString().Replace(".0000", "");
            encounter.Reason_For_Visit = row["Reason_For_Visit"].ToString();
            encounter.Reg_Dt_Tm = row["Reg_Dt_Tm"].ToString();
            encounter.ConfidentialVisit = row["confid_level_cd"].ToString();
            encounter.VIP_Cd = row["vip_cd"].ToString().Replace(".0000", "");
            encounter.Organization_Id = row["organization_id"].ToString().Replace(".0000", "");
            encounter.Med_Serv_Cd = row["med_service_cd"].ToString().Replace(".0000", "");
            //DateTime? d = GMTtoTZ(encounter.Disch_Dt_Tm, "Mountain Standard Time");


            //for BAR
            encounter.BAR_Encounter_Type_Cd = CernerMapDAL.GetMap("CERNER_REGTYPE", encounterTypeCd, encounterTypeCd);
            encounter.Admit_Src_Cd = row["admit_src_cd"].ToString().Replace(".0000", "");
            encounter.Admit_Type_Cd = row["admit_type_cd"].ToString().Replace(".0000", "");

            return encounter;
        }

        //public static Encounter GetEncounter(string encounterId)
        //{
        //    DateTime todayDt = DateTime.Today;
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append("SELECT encntr_id, Person_Id, Encntr_Type_Cd, Encntr_type_class_cd, Inpatient_Admit_Dt_Tm, Reg_Dt_Tm, ");
        //    sb.Append("Disch_Dt_tm, Loc_Nurse_Unit_Cd, Reason_For_Visit, Disch_Disposition_Cd, Active_Ind, organization_id, ");
        //    sb.Append("admit_type_cd, admit_src_cd, vip_cd,confid_level_cd,med_service_cd  ");
        //    sb.Append("FROM BAR_ENCOUNTER ");
        //    sb.Append("WHERE encntr_id = " + long.Parse(encounterId) + " ");
        //    sb.Append("AND active_ind = '1' ");
        //    sb.Append("AND END_EFFECTIVE_DT_TM > '" + todayDt + "'");

        //    DataTable dt = new DataTable();
        //    using (var conn = new SqlConnection(GlobalSettings.SqlConnectionString()))
        //    using (var cmd = new SqlCommand(sb.ToString(), conn))
        //    using (var da = new SqlDataAdapter(cmd))
        //    {
        //        cmd.CommandType = CommandType.Text;
        //        da.Fill(dt);
        //    }

        //    Encounter encounter = null;
        //    if (dt.Rows.Count > 0)
        //        encounter = LoadEncounter(dt.Rows[0]);

        //    return encounter;
        //}

        //private static Encounter LoadEncounter(DataRow row)
        //{
        //    string npi = "";
        //    Encounter encounter = new Encounter();
        //    encounter.Active_Ind = row["Active_Ind"].ToString();

        //    string dischDisp = CernerCommon.StripDecimalsForMap(row["Disch_Disposition_Cd"].ToString());
        //    encounter.Disch_Disposition_Cd = CernerMapDAL.GetMap("CERNER_DISCHARGE_DISPOSITION", dischDisp, dischDisp);
        //    encounter.Cerner_Disch_Disposition_Cd_Display = CodeValueDAL.GetCodeValueDisplay(dischDisp);

        //    encounter.Disch_Dt_Tm = row["Disch_Dt_Tm"].ToString();
        //    encounter.Encntr_type_class_cd = row["Encntr_type_class_cd"].ToString();

        //    encounter.Original_Encounter_Id = row["Encntr_Id"].ToString();
        //    string encntrId = Encounter_AliasDAL.GetEncounterVisitNumber(row["Encntr_Id"].ToString());
        //    encounter.Encounter_Id = encntrId;

        //    string encounterTypeCd = CernerCommon.StripDecimalsForMap(row["Encntr_Type_Cd"].ToString());
        //    encounter.Cerner_Encoutner_Type_Cd = encounterTypeCd; //used for special logic in mpi conversion record 02
        //    encounter.Cerner_Encounter_Type_Cd_Display = CodeValueDAL.GetCodeValueDisplay(encounterTypeCd);  //for data sheet

        //    encounter.Encounter_Type_Cd = CernerMapDAL.GetMap("CERNER_REGTYPE", encounterTypeCd, encounterTypeCd);
        //    encounter.Inpatient_Admit_Dt_Tm = row["Inpatient_Admit_Dt_Tm"].ToString();

        //    //From Encounter Table use field E_LOC_NURSE_UNIT_CD to retrieve the  DISPLAY_KEY (col E) in CodeSet 220.  
        //    //Use the DISPLAY_KEY to retrieve the Meditech location from the spreadsheet - "Approved locations"
        //    string loc = CodeValueDAL.GetCodeValueDisplay_Key(row["Loc_Nurse_Unit_Cd"].ToString());
        //    encounter.Cerner_Loc_Nurse_Unit_Cd_Display = loc; //for data sheet
        //    encounter.Loc_Nurse_Unit_Cd = CernerMapDAL.GetMap("CERNER_LOCATION", loc, loc);

        //    //string attendPhys = EncntrPrsnlReltnDAL.GetAttendPhysician(row["Encntr_Id"].ToString());
        //    encounter.AttendPhysicianId = ""; //attendPhys;
        //    //MPIOrganization org = MPIOrganizationDAL.GetOrganization(CernerCommon.StripDecimalsForMap(encounter.Organization_Id));
        //    //npi = PrsnlAliasDAL.GetPrsnlAlias(attendPhys);
        //    encounter.AttendPhysician = "";//CernerMapDAL.GetMapDR(npi, facility, "Z.CONVPROV");

        //    encounter.Person_Id = row["Person_Id"].ToString();
        //    encounter.Reason_For_Visit = row["Reason_For_Visit"].ToString();
        //    encounter.Reg_Dt_Tm = row["Reg_Dt_Tm"].ToString();
        //    encounter.ConfidentialVisit = row["confid_level_cd"].ToString();
        //    encounter.VIP_Cd = row["vip_cd"].ToString();
        //    encounter.Organization_Id = row["organization_id"].ToString();
        //    encounter.Med_Serv_Cd = row["med_service_cd"].ToString();
        //    //DateTime? d = GMTtoTZ(encounter.Disch_Dt_Tm, "Mountain Standard Time");


        //    //for BAR
        //    encounter.BAR_Encounter_Type_Cd = CernerMapDAL.GetMap("CERNER_REGTYPE", encounterTypeCd, encounterTypeCd);
        //    encounter.Admit_Src_Cd = row["admit_src_cd"].ToString();
        //    encounter.Admit_Type_Cd = row["admit_type_cd"].ToString();

        //    return encounter;
        //}

    }
}
