using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace WestBARConversion.CernerModel
{
    public class MPIOrganization
    {
        public string Organization_id { get; set; }
        public string  Hcis { get; set; }
        public string Facility { get; set; }
        public string  MRNPrefix { get; set; }
        public string AcctPrefix { get; set; }
        public string CernerPrefix { get; set; }
    }
}