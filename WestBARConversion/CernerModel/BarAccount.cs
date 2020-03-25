using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WestBARConversion.CernerModel
{
    public class BarAccount
    {
        public string pft_encntr_id { get; set; }
        public string enctr_id { get; set; }
        public string acct_id { get; set; }
        public string AccountNumber { get; set; }
        public string AccountRecivableStatus { get; set; }
        public string PatientStatus { get; set; }
        public string PatientService { get; set; }
        public string PatientLocation { get; set; }
        public DateTime? AdmitDate { get; set; }
        public string AdmitPriority { get; set; }
        public string AdmitSource { get; set; }
        public DateTime? DischargeDate { get; set; }
        public string DischargeDisposition { get; set; }
        public DateTime? ZeroDate { get; set; }
        public decimal? TotalCharges { get; set; }
        public string MotherAccountNumber { get; set; }
        public string NewbornAdmitSource { get; set; }
        public string AbstractStatus { get; set; }
        public decimal? AccountBalance { get; set; }
        public decimal? SelfPayBalance { get; set; }
        public decimal? URBalance { get; set; }
        public decimal? BadDebtBalance { get; set; }

    }
}