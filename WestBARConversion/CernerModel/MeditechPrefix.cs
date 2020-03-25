using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WestConversion.CernerModel
{
    public class MeditechPrefix
    {
        public string MRNPrefix { get; set; }
        public string AcctPrefix { get; set; }
        public string HCIS { get; set; }
        public DateTime? ConversionDate { get; set; }
        public string ExpectedCernerPrefix { get; set; }
        public MeditechPrefix(string organizationId)
        {
            LoadObject(organizationId);
        }

        private void LoadObject(string organizationId)
        {
            #region SHAZ
            if (organizationId == "3108362.0000" || organizationId == "3110566.0000")
            {
                MRNPrefix = "VM";
                AcctPrefix = "ZV";
                HCIS = "SHAZ";
                ConversionDate = new DateTime(2020, 6, 1);
                ExpectedCernerPrefix = "15";
            }
            if (organizationId == "3108361.0000")
            {
                MRNPrefix = "LB";
                AcctPrefix = "ZQ";
                HCIS = "SHAZ";
                ConversionDate = new DateTime(2020, 6, 1);
                ExpectedCernerPrefix = "14";
            }
            if (organizationId == "3110565.0000" || organizationId == "3108359.0000" || organizationId == "3110564.0000")
            {
                MRNPrefix = "LM";
                AcctPrefix = "ZU";
                HCIS = "SHAZ";
                ConversionDate = new DateTime(2020, 6, 1);
                ExpectedCernerPrefix = "16";
            }
            if (organizationId == "3108360.0000")
            {
                MRNPrefix = "LM";
                AcctPrefix = "YK";
                HCIS = "SHAZ";
                ConversionDate = new DateTime(2020, 6, 1);
                ExpectedCernerPrefix = "17";
            }
            #endregion
        }
    }
}
