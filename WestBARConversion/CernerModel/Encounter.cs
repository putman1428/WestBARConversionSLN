using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WestBARConversion.CernerModel
{
    public class Encounter
    {
        public string Encounter_Id { get; set; }
        public string Person_Id { get; set; }
        public string Encounter_Type_Cd { get; set; }
        public string Encntr_type_class_cd { get; set; }
        public string Inpatient_Admit_Dt_Tm { get; set; }
        public string Reg_Dt_Tm { get; set; }
        public string Disch_Dt_Tm { get; set; }
        public string Loc_Nurse_Unit_Cd { get; set; }
        public string AttendPhysician { get; set; }
        public string AttendPhysicianId { get; set; }
        public string Reason_For_Visit { get; set; }
        public string Disch_Disposition_Cd { get; set; }
        public string Active_Ind { get; set; }
        public string ConfidentialVisit { get; set; }
        public string VIP_Cd { get; set; }
        public string Organization_Id { get; set; }
        public string Original_Encounter_Id { get; set; }
        public string Cerner_Encoutner_Type_Cd { get; set; }
        public string Cerner_Encounter_Type_Cd_Display { get; set; }
        public string Cerner_Loc_Nurse_Unit_Cd_Display { get; set; }
        public string Cerner_Disch_Disposition_Cd_Display { get; set; }
        public string Admit_Src_Cd { get; set; }
        public string Admit_Type_Cd { get; set; }
        public string BAR_Encounter_Type_Cd { get; set; }
        public string Med_Serv_Cd { get; set; }

    }
}
