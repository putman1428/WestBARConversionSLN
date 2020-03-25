using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WestBARConversion.CernerModel
{
    public class Encntr_Plan_Reltn
    {
        public string SequenceNbr { get; set; }
        public string InsuranceCode { get; set; }
        public string InsuranceName { get; set; }
        public Address Address { get; set; }
        public Phone InsurancePhone { get; set; }
        public string EffectiveDate { get; set; }
        public string Deductible { get; set; }
        public string CoInsuranceAmt { get; set; }
        public string CoPay { get; set; }
        public string InsEligibilitySts { get; set; }
        public string StatusDate { get; set; }
        public string InsuranceExpdate { get; set; }
        public string PolicyNbr { get; set; }
        public string InsEffectiveDate { get; set; }
        public string CoverageNbr { get; set; }
        public string GroupNbr { get; set; }
        public string GroupName { get; set; }
        public Person SubscriberPerson { get; set; }
        public Address SubscriberAddress { get; set; }
        public Phone SubscriberPhone { get; set; }
        public Address SubscriberEmailAddress { get; set; }
        public string SubPolicyNbr { get; set; }
        public List<Authorization> Authorizations { get; set; }

        //public Authorization Authorization2 { get; set; }
        //public Authorization Authorization3 { get; set; }
        public string Relationship { get; set; }
    }
}
