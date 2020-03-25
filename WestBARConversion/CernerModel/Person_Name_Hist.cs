using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WestBARConversion.CernerModel
{
    public class Person_Name_Hist
    {
        public string Name_Full { get; set; }
        public string Name_Type_Cd { get; set; }
        public string Name_Last { get; set; }
        public string Name_First { get; set; }
        public string Name_Middle { get; set; }
        public string Name_Suffix { get; set; }
        public string FullName()
        {
            string result = Name_Last.Trim().ToUpper() + "," + Name_First.ToUpper() + " " + Name_Middle.ToUpper();
            return result.Trim();
        }
    }
}
