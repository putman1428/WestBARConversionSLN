using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WestBARConversion
{
    public class GlobalSettings
    {
        public static string TextFileImportPath = @"\\appdata\etl\CernerFiles_MT_Migration\Cerna CSV Outputs";
        public static string MPIDestinationPath = @"E:\WestConversion\MPIOutput";
        public static string MPILogFilePath = @"E:\WestConversion\LogFiles";
        public static string InputPath = @"\\appdata\etl\CernerFiles_MT_Migration\Cerna CSV Outputs";
        public static string BARTextFileImportPath = @"\\appdata\etl\CernerFiles_MT_Migration\Huron_Extracts";
        public static string MPIInputPath = @"E:\WestConversion\MPIInput"; //this is for the person id tag files for the conversion to run off
        public static string SqlConnectionString()
        {
            SqlConnectionStringBuilder b = new SqlConnectionStringBuilder();
            b.DataSource = "SHCDSDBP01";
            b.IntegratedSecurity = true;
            b.InitialCatalog = "WestMedHostDB";
            return b.ConnectionString;
           
        }
        public static string SqlConnectionString_Alt()
        {
            SqlConnectionStringBuilder b = new SqlConnectionStringBuilder();
            b.DataSource = "SHCDSDBP01";
            b.IntegratedSecurity = true;
            b.InitialCatalog = "WestConversionDB";
            return b.ConnectionString;

        }
    }
}
