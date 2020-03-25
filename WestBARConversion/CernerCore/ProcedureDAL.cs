using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace WestBARConversion.CernerCore
{
    public class ProcedureDAL
    {
        public static List<Procedure_Obj> GetEncounterProcedures(string encounterId, string facility)
        {
            DateTime todayDt = DateTime.Today;
            StringBuilder sb = new StringBuilder();
            sb.Append("Select nomenclature_id, source_identifier, proc_dt_tm, procedure_id, source_vocabulary_cd,proc_priority from vProceduresNomen ");
            sb.Append("where encntr_id = " + long.Parse(encounterId) + " and active_ind = '1' ");
            sb.Append(" and active_ind_nomen = '1' ");
            sb.Append("AND END_EFFECTIVE_DT_TM > '" + todayDt + "'");
            sb.Append(" order by Convert(int, proc_priority) ");


            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(GlobalSettings.SqlConnectionString()))
            using (var cmd = new SqlCommand(sb.ToString(), conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }

            List<Procedure_Obj> procs = new List<Procedure_Obj>();
            foreach (DataRow row in dt.Rows)
            {
                Procedure_Obj proc = LoadProcedure(row, facility);
                if (proc != null)
                {
                    if(proc.PriorityCd != "0")
                        procs.Add(proc);
                }

            }
            dt.Dispose();
            return procs;
        }

        private static Procedure_Obj LoadProcedure(DataRow row, string facility)
        {
            //string procCode = NomenclatureDAL.GetNomenclature_SourceIdentifier(row["nomenclature_id"].ToString());
            string[] phyItems;
            string phys = "";
            string npi = "";
            Procedure_Obj proc = new Procedure_Obj();
            proc.ProcedureId = row["procedure_id"].ToString();
            proc.ProcCode = row["source_identifier"].ToString();
            proc.GrouperVersion = string.Empty;
            proc.ProcedureDateTime = row["proc_dt_tm"].ToString();
            proc.SourceVocabCd = row["source_vocabulary_cd"].ToString();
            proc.NomenID = row["nomenclature_id"].ToString();
            proc.PriorityCd = row["proc_priority"].ToString();
            phys = ProcPrsnlReltn.GetSurgicalPhysician(proc.ProcedureId);
            phyItems = phys.Split('|');
            if (phyItems[0].Trim() != "")
            {
                npi = PrsnlAliasDAL.GetPrsnlAlias(phyItems[0]);
                proc.ProcedurePhy = CernerMapDAL.GetMapDR(npi, facility, "Z.CONVPROV");
            }
            if (phyItems[1].Trim() != "")
            {
                npi = PrsnlAliasDAL.GetPrsnlAlias(phyItems[1]);
                proc.ProcedurePhyAsst = CernerMapDAL.GetMapDR(npi, facility, "Z.CONVPROV");
            }
            //proc.ProcedureBy = CernerMapDAL.GetMap("CERNER_PROVIDER", procPersonId, "Z.CONVPROV");

            return proc;
        }

        
    }
}
