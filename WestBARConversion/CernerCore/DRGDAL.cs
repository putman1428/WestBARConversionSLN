using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace WestBARConversion.CernerCore
{
    public class DRGDAL
    {
        private static string today = DateTime.Today.ToString("yyyy-MM-dd");
        public static List<Drg> GetEncounterDRG(string encounterId)
        {
            DateTime todayDt = DateTime.Today;
            StringBuilder sb = new StringBuilder();
            sb.Append("Select nomenclature_id,source_identifier,source_vocabulary_cd_nomen from vDRGNomen ");
            sb.Append("where encntr_id = " + long.Parse(encounterId) + " and active_ind = '1' and DRG_PRIORITY > '0' ");
            sb.Append("AND END_EFFECTIVE_DT_TM > '" + todayDt + "'");
            sb.Append(" order by Convert(int, DRG_PRIORITY) ");

            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(GlobalSettings.SqlConnectionString()))
            using (var cmd = new SqlCommand(sb.ToString(), conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }

            List<Drg> drgs = new List<Drg>();
            foreach (DataRow row in dt.Rows)
            {
                Drg drg = LoadDRG(row);
                if (drg != null)
                {
                    drgs.Add(drg);
                }

            }
            dt.Dispose();
            return drgs;
        }

        private static Drg LoadDRG(DataRow row)
        {
            string drgCode = row["source_identifier"].ToString();
            string drgSV = row["source_vocabulary_cd_nomen"].ToString().Trim();
            Drg drg = new Drg();
            drg.DrgCode = drgCode;
            drg.DrgStatus = drg.DrgStatus;
            drg.DrgSourceVocab = drgSV;
            return drg;
        }
    }
}
