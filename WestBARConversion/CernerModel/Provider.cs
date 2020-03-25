using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WestBARConversion.CernerModel
{
    public class Provider : Person
    {
        public string License { get; set; }
        public string NPI { get; set; }
        public string PhysicianType { get; set; }
        public string Mnemonic { get; set; }
    }
}