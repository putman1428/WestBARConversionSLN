using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WestBARConversion.CernerModel
{
    class Person_Prsnl_Reltn
    {
        public string Person_Prsnl_Reltn_Id { get; set; }
        public string Person_Id { get; set; }
        public string Person_Prsnl_R_Cd { get; set; }
        public string Prsnl_Person_Id { get; set; }
        public string Active_Ind { get; set; }
        public string Beg_Effective_Dt_Tm { get; set; }
        public string End_Effective_Dt_Tm { get; set; }
        public string Ft_prsnl_name { get; set; }
        public string Priority_seq { get; set; }
        public string Internal_seq { get; set; }
    }
}
