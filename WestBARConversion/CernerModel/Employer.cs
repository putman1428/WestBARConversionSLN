using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WestBARConversion.CernerModel
{
    public class Employer
    {
        public string EmployerName { get; set; }
        public Address WorkAddress { get; set; }
        public Phone EmployerPhone { get; set; }
        public Phone WorkPhone { get; set; }
        public string Occupation { get; set; }
        public string EmploymentStatus { get; set; }
        public string EmployeeID { get; set; }
        
    }
}
