using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace WestBARConversion.CernerCore
{
    public class DiagnosisDAL
    {
        public static List<Diagnosis> GetAllEncounterDxs(string encounterId)
        {
            DateTime todayDt = DateTime.Today;
            StringBuilder sb = new StringBuilder();
            sb.Append("Select encntr_id, nomenclature_id, diag_type_cd, diag_priority, beg_effective_dt_tm, ");
            sb.Append("active_ind, present_on_admit_cd ");
            sb.Append("FROM BAR_DIAGNOSIS ");
            sb.Append("where encntr_Id = " + long.Parse(encounterId) + " and active_ind = '1' and diag_type_cd in ('87','89') ");
            sb.Append("AND END_EFFECTIVE_DT_TM >'" + todayDt + "'");
            sb.Append("order by CONVERT(int, diag_priority) ");

            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(GlobalSettings.SqlConnectionString()))
            using (var cmd = new SqlCommand(sb.ToString(), conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }

            List<Diagnosis> dxs = new List<Diagnosis>();
            foreach (DataRow row in dt.Rows)
            {
                Diagnosis dx = LoadDiagnosis(row);
                if (dx != null)
                {
                    if (dx.priority.Trim() != "")
                        dxs.Add(dx);
                }

            }
            dt.Dispose();
            return dxs;
        }
        public static List<Diagnosis> GetEncounterAdmitDxs(string encounterId)
        {
            DateTime todayDt = DateTime.Today;
            StringBuilder sb = new StringBuilder();
            sb.Append("Select encntr_id, nomenclature_id, diag_type_cd, diag_priority, beg_effective_dt_tm, ");
            sb.Append("active_ind, present_on_admit_cd ");
            sb.Append("FROM BAR_DIAGNOSIS ");
            sb.Append("where encntr_Id = " + long.Parse(encounterId) + " and active_ind = '1' and diag_type_cd = '87' ");
            sb.Append("AND END_EFFECTIVE_DT_TM >'" + todayDt + "'");
            sb.Append("order by CONVERT(int, diag_priority) ");

            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(GlobalSettings.SqlConnectionString()))
            using (var cmd = new SqlCommand(sb.ToString(), conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }

            List<Diagnosis> dxs = new List<Diagnosis>();
            foreach(DataRow row in dt.Rows)
            {
                Diagnosis dx = LoadDiagnosis(row);
                if (dx != null)
                {
                    if (dx.priority.Trim() != "")
                        dxs.Add(dx);
                }

            }
            dt.Dispose();
            return dxs;
        }

        public static List<Diagnosis> GetEncounterFinalDxs(string encounterId)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Select encntr_id, nomenclature_id, diag_type_cd, diag_priority, beg_effective_dt_tm, ");
            sb.Append("active_ind, present_on_admit_cd ");
            sb.Append("FROM BAR_DIAGNOSIS ");
            sb.Append("where encntr_Id = " + long.Parse(encounterId) + " and active_ind = '1' and diag_type_cd = '89' ");
            sb.Append("order by CONVERT(int, diag_priority) ");

            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(GlobalSettings.SqlConnectionString()))
            using (var cmd = new SqlCommand(sb.ToString(), conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }

            List<Diagnosis> dxs = new List<Diagnosis>();
            foreach (DataRow row in dt.Rows)
            {
                Diagnosis dx = LoadDiagnosis(row);
                if (dx != null)
                {
                    if(dx.priority.Trim() != "")
                        dxs.Add(dx);
                }

            }
            dt.Dispose();
            return dxs;
        }

        private static Diagnosis LoadDiagnosis(DataRow row)
        {
            string diagCode = NomenclatureDAL.GetNomenclature_SourceIdentifier(row["nomenclature_id"].ToString());
            string poa = CodeValueDAL.GetCodeValueDisplay(row["present_on_admit_cd"].ToString());
            string pri = row["diag_priority"].ToString().Trim();
            if (poa == "0")
                poa = string.Empty;

            Diagnosis dx = new Diagnosis();
            dx.DxCode = diagCode;
            dx.GrouperVersion = string.Empty;
            dx.POA = poa;
            if(pri == "0" || pri =="")
                dx.priority = string.Empty;
            else
                dx.priority = pri;
            dx.DxType = row["diag_type_cd"].ToString().Trim();
            return dx;
        }
    }
}