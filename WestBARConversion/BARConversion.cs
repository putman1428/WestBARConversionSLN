using System;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeditechLibrary.BAR._61X_V2;
using WestBARConversion.CernerCore;
using WestBARConversion.CernerModel;

namespace WestBARConversion
{
    public class BARConversion
    {
        private static BARForm frm;
        private string _hcis = string.Empty;
        private string _hospFac = string.Empty;
        private string _barStatus = string.Empty;
        private string _grouperVersion = string.Empty;
        private decimal? _globalFBBalance = 0;
        private decimal? _globalBDBalance = 0;
        private BAR mtBAR = null;
        private Hashtable _globalInsHash;
        private Hashtable global_Map;
        private Hashtable CDMHash;
        private Hashtable _globalPhyTypeHash;
        private DataIO fLog;
        public void Convert(BARForm f,string barStatus, string hospName, string outputPath, string hcis)
        {
            string timein = "";
            string timeout = "";
            string encntrPrefix = "";
            int acctCnt = 0;
            int encntrCnt = 0;
            int notFound = 0;

            frm = f;
            _hcis = hcis;
            _barStatus = barStatus;
            timein = DateTime.Now.ToString();
            fLog = new DataIO();
            fLog.OpenFile(@"E:\Huron\Output\BAR\TimeBARLog.txt", DataIO.IO.OUTPUT);

            LoadMapping();

            List<string> organizations = GetOrganizations(_hcis,hospName);
            Hashtable hashInvalidPersonID = CDV_DAL.GetExcludedUsers();

            mtBAR = new BAR(hospName, outputPath, _hcis, _barStatus);
            List<string> encounterIds = EncounterDAL.GetDistinctEncounterIds(organizations);
            //encounterIds.Insert(0, "126182873");
            
            foreach (string encounterId in encounterIds)
            {
                
                List<PFT_Encntr> pft_encntrs = PftEncntrDAL.GetPFT_Encntrs(CernerCommon.StripDecimalsForMap(encounterId));
                if (pft_encntrs.Count > 0)
                {
                    Encounter encounter = EncounterDAL.GetEncounter(encounterId);
                    //Person person = PersonDAL.GetPerson(encounter.Person_Id);
                    encntrPrefix = encounter.Encounter_Id.Trim().PadRight(15, ' ').Substring(0,2);
                    MPIOrganization org = MPIOrganizationDAL.GetOrganizationNew(CernerCommon.StripDecimalsForMap(encounter.Organization_Id), encntrPrefix);

                    if (hashInvalidPersonID.ContainsKey(encounter.Person_Id.Replace(".0000", "")) == false)
                    {
                        string mrn = org.MRNPrefix + CernerCommon.StripDecimalsForMap(encounter.Person_Id).PadLeft(8, char.Parse("0"));
                        string facility = org.Facility;

                        foreach (PFT_Encntr pft_Encntr in pft_encntrs)
                        {
                            if (pft_Encntr.Recur_Seq == "0")//Remove
                            {

                                if (barStatus == "BD")
                                {
                                    if (pft_Encntr.Bad_Debt_Dt_Tm != null)
                                    {
                                        List<Bill_Record_Data> bills = BillInfoDAL.GetBillRecs(pft_Encntr.Pft_Encntr_Id);
                                        Person person = PersonDAL.GetPerson(encounter.Person_Id);
                                        ProcessAccount(pft_Encntr, encounter, person, org, mrn, facility, bills);
                                        acctCnt++;
                                    }
                                }
                                else if (barStatus == "FB" || barStatus == "UB" || barStatus == "ZERO")
                                {

                                    if (pft_Encntr.Bad_Debt_Dt_Tm == null)
                                    {
                                        List<Bill_Record_Data> bills = BillInfoDAL.GetBillRecs(pft_Encntr.Pft_Encntr_Id);
                                        if (bills.Count == 0 && barStatus == "UB" && pft_Encntr.Zero_Balance_Dt_Tm == null)
                                        {
                                            Person personUB = PersonDAL.GetPerson(encounter.Person_Id);
                                            ProcessAccount(pft_Encntr, encounter, personUB, org, mrn, facility, bills);
                                            acctCnt++;
                                        }
                                        else if (pft_Encntr.Balance != 0 && barStatus == "FB")
                                        {
                                            Person personFB = PersonDAL.GetPerson(encounter.Person_Id);
                                            ProcessAccount(pft_Encntr, encounter, personFB, org, mrn, facility, bills);
                                            acctCnt++;
                                        }
                                        else if (pft_Encntr.Balance == 0 && barStatus == "ZERO")
                                        {
                                            Person personFB = PersonDAL.GetPerson(encounter.Person_Id);
                                            ProcessAccount(pft_Encntr, encounter, personFB, org, mrn, facility, bills);
                                            acctCnt++;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                encntrCnt++;
                if (encntrCnt % 10 == 0)
                    frm.DisplayResults(hcis + " - " + acctCnt.ToString() + " / " + encntrCnt.ToString() + "  Start Time: " + timein);
                if (acctCnt > 4470)
                {

                    if (_barStatus == "FB" || _barStatus == "UB")
                        mtBAR.SetEndingBalance(_globalFBBalance, _barStatus);
                    else if (_barStatus == "BD")
                        mtBAR.SetEndingBalance(_globalBDBalance, _barStatus);
                    mtBAR.CreateFileBreak();
                    _globalFBBalance = 0;
                    _globalBDBalance = 0;
                    acctCnt = 0;
                }
            }
            if(_barStatus == "FB" || _barStatus == "UB")
                mtBAR.SetEndingBalance(_globalFBBalance,_barStatus);
            else if (_barStatus == "BD")
                mtBAR.SetEndingBalance(_globalBDBalance, _barStatus);
            mtBAR.UnInit();
            timeout = DateTime.Now.ToString();
            frm.DisplayResults(hcis + " - " + acctCnt.ToString() + " / " + encntrCnt.ToString() + "  Start Time: " + timein + "  End Time: " + timeout);
        }

        private void ProcessAccount(PFT_Encntr pft_Encnter, Encounter encounter, Person person, MPIOrganization org, string mrn, string facility, List<Bill_Record_Data> bills)
        {
            string acctNbr = "";
            string gtrPersonId = "";
            string gtrRel = "";
            List<Encntr_Person_Reltn> eprs = EncntrPersonReltDAL.GetGuarantor(encounter.Original_Encounter_Id);
            foreach (Encntr_Person_Reltn epr in eprs)
            {
                gtrPersonId = epr.guarPersonID;
                gtrRel = epr.relationship;
            }
            if (gtrPersonId == "")
                gtrPersonId = person.PersonId;
            fLog.WriteRecordNoCriteria("Start 1a:" + DateTime.UtcNow.ToString("yyyyMMdd HH:mm:ss.FFF tt"));
            Person guarantor = PersonDAL.GetPerson(gtrPersonId);
            Employer patientEmp = PersonOrgReltDAL.GetEmployer(person.PersonId);
            fLog.WriteRecordNoCriteria("Start 2a:" + DateTime.UtcNow.ToString("yyyyMMdd HH:mm:ss.FFF tt"));
            Employer guarEmp = PersonOrgReltDAL.GetEmployer(gtrPersonId);
            fLog.WriteRecordNoCriteria("Start 3a:" + DateTime.UtcNow.ToString("yyyyMMdd HH:mm:ss.FFF tt"));
            List<Diagnosis> allDxs = DiagnosisDAL.GetAllEncounterDxs(encounter.Original_Encounter_Id); //remember - encounterdal strips this value - use the orig encoutner id property if you need that 
            //List<Diagnosis> admitDxs = DiagnosisDAL.GetEncounterAdmitDxs(encounter.Original_Encounter_Id); //remember - encounterdal strips this value - use the orig encoutner id property if you need that
            List<Drg> drgs = DRGDAL.GetEncounterDRG(encounter.Original_Encounter_Id);
            //List<Diagnosis> finalDxs = DiagnosisDAL.GetEncounterFinalDxs(encounter.Original_Encounter_Id);
            fLog.WriteRecordNoCriteria("Start 4a:" + DateTime.UtcNow.ToString("yyyyMMdd HH:mm:ss.FFF tt"));
            List<Procedure_Obj> procedures = ProcedureDAL.GetEncounterProcedures(encounter.Original_Encounter_Id, facility);
            fLog.WriteRecordNoCriteria("Start 5a:" + DateTime.UtcNow.ToString("yyyyMMdd HH:mm:ss.FFF tt"));
            List<Provider> providers = EncntrPrsnlReltnDAL.GetEncounterPhysicians(encounter.Original_Encounter_Id, facility);
            fLog.WriteRecordNoCriteria("Start 6a:" + DateTime.UtcNow.ToString("yyyyMMdd HH:mm:ss.FFF tt"));
            List<string> conditionCodes = Encntr_Condition_CodeDAL.GetConditionCodes(encounter.Original_Encounter_Id);
            List<Occurrence_Code> occurrenceCodes = Encntr_Occurrence_CodeDAL.GetOccurrence_Codes(encounter.Original_Encounter_Id);
            List<Value_Code> valueCodes = Encntr_Value_CodeDAL.GetEncounterValueCodes(encounter.Original_Encounter_Id);
            fLog.WriteRecordNoCriteria("Start 7a:" + DateTime.UtcNow.ToString("yyyyMMdd HH:mm:ss.FFF tt"));
            List<Encntr_Plan_Reltn> encPlans = Encntr_Plan_ReltnDAL.GetEncounterPlans(encounter.Original_Encounter_Id);
            fLog.WriteRecordNoCriteria("Start 8a:" + DateTime.UtcNow.ToString("yyyyMMdd HH:mm:ss.FFF tt"));
            List<PftProration> pftProrate = Pft_Proration_DAL.GetPFTProration(pft_Encnter.Pft_Encntr_Id);
            //List<Bill_Record_Data> bills = BillInfoDAL.GetBillRecs(pft_Encnter.Pft_Encntr_Id);

            mtBAR.InitPatient();
            acctNbr = encounter.Encounter_Id.Trim();
            if (acctNbr.Length > 2)
                acctNbr = acctNbr.Substring(2);

            mtBAR.CurrentAccountNumber = org.AcctPrefix + acctNbr.Trim().PadLeft(10,'0');
            mtBAR.CurrentMRN = mrn;

            fLog.WriteRecordNoCriteria("Start 1:" + DateTime.UtcNow.ToString("yyyyMMdd HH:mm:ss.FFF tt"));
            PatientDemogrpahics(person, encounter, facility);
            PatientEmployer(patientEmp);
            fLog.WriteRecordNoCriteria("Start 2:" + DateTime.UtcNow.ToString("yyyyMMdd HH:mm:ss.FFF tt"));
            AccountInfo(pft_Encnter, encounter, org.AcctPrefix);
            Providers(providers);
            fLog.WriteRecordNoCriteria("Start 3:" + DateTime.UtcNow.ToString("yyyyMMdd HH:mm:ss.FFF tt"));
            //PatientDiagnosis(admitDxs, finalDxs);
            PatientDiagnosis(allDxs, allDxs);
            PatientConditionCodes(conditionCodes);
            PatientOccurenceAndSpanCodes(occurrenceCodes);
            PatientValueCodes(valueCodes);
            fLog.WriteRecordNoCriteria("Start 4:" + DateTime.UtcNow.ToString("yyyyMMdd HH:mm:ss.FFF tt"));
            InpatientCodes(drgs, encounter.Original_Encounter_Id.Replace(".0000",""));
            PatientProcedures(procedures);
            OutpatientAPCHCPCSCodes(procedures);
            OutpatientCPTCodes(procedures);
            fLog.WriteRecordNoCriteria("Start 5:" + DateTime.UtcNow.ToString("yyyyMMdd HH:mm:ss.FFF tt"));
            PatientGuarantor(guarantor, gtrRel);
            PatientGuarantorEmployer(guarEmp);
            _globalInsHash = new Hashtable();
            PatientInsurances(encPlans);
            //PatientQueries();
            fLog.WriteRecordNoCriteria("Start 6:" + DateTime.UtcNow.ToString("yyyyMMdd HH:mm:ss.FFF tt"));
            CurrentBalance(pft_Encnter, pftProrate);
            
            bool printBills = true;
            if (_barStatus != "UB")
            {
                printBills = ProcessBills(bills, pftProrate, pft_Encnter.Pat_Bal_Fwd);
                if (printBills == true)
                    ProcessBillsDetail(bills, pftProrate, pft_Encnter.Pat_Bal_Fwd);
            }
            else
                printBills = false;
            fLog.WriteRecordNoCriteria("Start 7:" + DateTime.UtcNow.ToString("yyyyMMdd HH:mm:ss.FFF tt"));
            ProcessChargesTransactions(pft_Encnter, facility);
            fLog.WriteRecordNoCriteria("Start 8:" + DateTime.UtcNow.ToString("yyyyMMdd HH:mm:ss.FFF tt"));
            //ProcessComments

            mtBAR.Print(printBills);
            fLog.WriteRecordNoCriteria("Start 9:" + DateTime.UtcNow.ToString("yyyyMMdd HH:mm:ss.FFF tt"));
        }

        private void PatientDemogrpahics(Person person, Encounter encounter, string facility)
        {
            mtBAR.PatientDemo.AccountNumber = mtBAR.CurrentAccountNumber;
            mtBAR.PatientDemo.Name = person.FullName();
            mtBAR.PatientDemo.MedicalRecordNumber = mtBAR.CurrentMRN;
            mtBAR.PatientDemo.SocialSecurityNumber = CernerCommon.GetSSN(person.SocialSecurityNumber);
            if (person.HomeAddress != null)
            {
                mtBAR.PatientDemo.AddressLine1 = person.HomeAddress.Street_Addr;
                mtBAR.PatientDemo.AddressLine2 = CheckCode(person.HomeAddress.Street_Addr2);
                mtBAR.PatientDemo.City = person.HomeAddress.City;
                mtBAR.PatientDemo.State = person.HomeAddress.State_Cd;
                mtBAR.PatientDemo.ZipCode = person.HomeAddress.ZipCode;
            }
            else
            {
                mtBAR.PatientDemo.AddressLine1 = string.Empty; 
                mtBAR.PatientDemo.AddressLine2 = string.Empty;
                mtBAR.PatientDemo.City = string.Empty;
                mtBAR.PatientDemo.State = string.Empty;
                mtBAR.PatientDemo.ZipCode = string.Empty;
            }
            if (person.HomePhone != null)
                mtBAR.PatientDemo.HomePhone = person.HomePhone.Phone_Num;
            else
                mtBAR.PatientDemo.HomePhone = string.Empty;
            mtBAR.PatientDemo.Race = person.Race_Cd;
            mtBAR.PatientDemo.Sex = person.Sex_Cd;
            mtBAR.PatientDemo.BirthDate = CernerCommon.GetCernerDate(person.Birth_Dt_Tm);
            mtBAR.PatientDemo.MaritalStatus = person.Marital_Type_Cd;
            mtBAR.PatientDemo.Facility = facility;
            mtBAR.PatientDemo.AlternateAddress = string.Empty;
            if (encounter.ConfidentialVisit == "0" || encounter.ConfidentialVisit == "null" || encounter.ConfidentialVisit == "" || encounter.ConfidentialVisit == ".0000")
                mtBAR.PatientDemo.Confidential = "";
            else
                mtBAR.PatientDemo.Confidential = "Y";
            if (encounter.VIP_Cd == "0" || encounter.VIP_Cd == "null" || encounter.VIP_Cd == "" || encounter.VIP_Cd == ".0000")
                mtBAR.PatientDemo.VIP = "";
            else
                mtBAR.PatientDemo.VIP = "Y";
        }
        
        private void PatientEmployer(Employer patientEmp)
        {
            string addr1 = "";
            string addr2 = "";
            string city = "";
            string state = "";
            string zip = "";
            string phone = "";
            if(patientEmp.WorkAddress != null)
            {
                addr1 = patientEmp.WorkAddress.Street_Addr;
                addr2 = patientEmp.WorkAddress.Street_Addr2;
                city = patientEmp.WorkAddress.City;
                state = patientEmp.WorkAddress.State_Cd;
                zip = patientEmp.WorkAddress.ZipCode;
            }
            if (patientEmp.WorkPhone != null)
            { 
                phone = patientEmp.WorkPhone.Phone_Num;
            }
            if(patientEmp.EmployerName != null)
                mtBAR.UpdatePatientEmployer(patientEmp.EmployerName, addr1, addr2, city, state, zip, phone,  "",patientEmp.Occupation, patientEmp.EmploymentStatus, "");
        }
        private void AccountInfo(PFT_Encntr pft_Encnter, Encounter encounter, string orgPrefix)
        {
            mtBAR.AccountInfo.AccountNumber = mtBAR.PatientDemo.AccountNumber;
            mtBAR.AccountInfo.ARStatus = _barStatus;
            mtBAR.AccountInfo.PatientStatus = encounter.BAR_Encounter_Type_Cd; //using bar encounter type
            mtBAR.AccountInfo.PatientService = "";// encounter.Med_Serv_Cd; //using mpi map
            mtBAR.AccountInfo.PatientLocation = encounter.Loc_Nurse_Unit_Cd; //using mpi map
            mtBAR.AccountInfo.AdmitDate = CernerCommon.GetCernerDate(encounter.Reg_Dt_Tm, _hcis); //have to convert string date and time zone to get datetype
            mtBAR.AccountInfo.AdmitTime = CernerCommon.GetCernerTime(encounter.Reg_Dt_Tm, _hcis);
            mtBAR.AccountInfo.AdmitPriority = CheckCode(encounter.Admit_Type_Cd);  //need to get map
            mtBAR.AccountInfo.AdmitSource = CheckCode(encounter.Admit_Src_Cd); //need to get map
            mtBAR.AccountInfo.DischargeDate = CernerCommon.GetCernerDate(encounter.Disch_Dt_Tm, _hcis); //have to convert string date and time zone to get datetype
            mtBAR.AccountInfo.DischargeTime = CernerCommon.GetCernerTime(encounter.Disch_Dt_Tm, _hcis);
            mtBAR.AccountInfo.DischargeDisposition = encounter.Disch_Disposition_Cd; //mapped in encounterDAL object

            if (pft_Encnter.Zero_Balance_Dt_Tm.HasValue)
                mtBAR.AccountInfo.ZeroDate = CernerCommon.GetCernerDate(pft_Encnter.Zero_Balance_Dt_Tm.ToString(),_hcis);
            mtBAR.AccountInfo.TotalCharges = GetBalance(pft_Encnter.Charge_Balance,pft_Encnter.Chrg_Bal_Dr_Cr_Flag);

            if (encounter.Admit_Type_Cd == "670841")
            {
                mtBAR.AccountInfo.MotherAccountNumber = EncntrMotherChildDAL.GetMotherAcctNumber(encounter.Encounter_Id, orgPrefix);
                mtBAR.AccountInfo.NewbornAdmitSource = encounter.Admit_Src_Cd;
            }
            mtBAR.AccountInfo.AbstractStatus = string.Empty;
            SetGrouperVersion(CernerCommon.GetCernerDate(encounter.Disch_Dt_Tm, _hcis));
        }
        private void Providers(List<Provider> providers)
        {
            foreach(Provider provider in providers)
            {
                string physMnemonic = provider.Mnemonic;
                string name = provider.FullName();
                string addr1 = string.Empty;
                string addr2 = string.Empty;
                string city = string.Empty;
                string state = string.Empty;
                string zipCode = string.Empty;
                string homePhone = string.Empty;
                string npi = provider.NPI;
                string licenseNbr = provider.License;
                string physType = GetPhyTypeMap(provider.PhysicianType);

                if (provider.HomeAddress != null)
                {
                    addr1 = provider.HomeAddress.Street_Addr;
                    addr2 = provider.HomeAddress.Street_Addr2;
                    city = provider.HomeAddress.Street_Addr;
                    state = provider.HomeAddress.Street_Addr;
                    zipCode = provider.HomeAddress.Street_Addr;
                }
                if (provider.HomePhone != null)
                {
                    homePhone = provider.HomePhone.Phone_Num;
                }
                mtBAR.AddProvider(physMnemonic, name, addr1, addr2, city, state, zipCode, homePhone, npi, licenseNbr, physType);
            }
        }

        private void PatientDiagnosis(List<Diagnosis> admitDxs, List<Diagnosis> finalDxs)
        {
            int cnt_a = 0;
            string admitType = "A"; //defualt to Admit - set in finalDx where cnt = 1,P else 'S'
            foreach (Diagnosis Dx in admitDxs)
            {
                if (Dx.DxType == "87")
                {
                    Diagnosis admitDx = admitDxs[cnt_a];
                    mtBAR.AddDiagnosis(admitDx.DxCode, _grouperVersion, admitDx.POA, admitType);
                    break;
                }
                cnt_a++;
            }
            //if (admitDxs.Count > 0)
            //{
            //    Diagnosis admitDx = admitDxs[0];
            //    mtBAR.AddDiagnosis(admitDx.DxCode, _grouperVersion, admitDx.POA, admitType);
            //}
            
            int cnt = 0;
            foreach(Diagnosis finalDx in finalDxs)
            {
                if (finalDx.DxType == "89")
                {
                    cnt++;
                    if (cnt == 1)
                        admitType = "P";
                    else
                        admitType = "S";

                    mtBAR.AddDiagnosis(finalDx.DxCode, _grouperVersion, finalDx.POA, admitType);
                }
            }

            
        }
        private void PatientConditionCodes(List<string> condCodes)
        {
            foreach(string code in condCodes)
            {
                mtBAR.AddConditionCode(code);
            }
        }

        private void PatientOccurenceAndSpanCodes(List<Occurrence_Code> occurCodes)
        {
            foreach(Occurrence_Code code in occurCodes)
            {
                mtBAR.AddOccurenceCode(code.OccurrenceCode, code.OccurrenceDate);
            }
        }

        private void PatientValueCodes(List<Value_Code> valueCodes)
        {
            foreach(Value_Code code in valueCodes)
            {
                mtBAR.AddValue(code.ValueCode, code.ValueAmount);
            }
            
        }
        private void InpatientCodes(List<Drg> drgs, string encID)
        {
            int cnt = 0;
            string compDt = "";
            string sts = "";
            foreach (Drg drg in drgs)
            {
                if(cnt == 0)
                    compDt = CodingDAL.GetCoding(encID);
                cnt++;
                if (compDt == "")
                    sts = "I";
                else
                    sts = "F";
                mtBAR.UpdateImpatientCodes(drg.DrgCode, sts, "","","","",null,null);
            }
        }
        private void PatientProcedures(List<Procedure_Obj> procedures)
        {
            
            foreach(Procedure_Obj proc in procedures)
            {
                if (proc.SourceVocabCd.Trim() == "19350130")
                {
                    DateTime? procDate = CernerCommon.GetCernerDate(proc.ProcedureDateTime,_hcis);
                    mtBAR.AddInpatientProcedure(proc.ProcCode, _grouperVersion, procDate, proc.ProcedurePhy);
                }
            }
        }
        private void OutpatientAPCHCPCSCodes(List<Procedure_Obj> procedures)
        {
            string hcpcs = "";
            foreach (Procedure_Obj proc in procedures)
            {
                if (proc.SourceVocabCd.Trim() == "1222")
                {
                    hcpcs = hcpcs + "|" + proc.ProcCode;
                }
            }
            if (hcpcs.Length > 0)
                hcpcs = hcpcs.Substring(1);
            mtBAR.AddOutPatientHCPCS(hcpcs);
        }
        private void OutpatientCPTCodes(List<Procedure_Obj> procedures)
        {
            foreach (Procedure_Obj proc in procedures)
            {
                if (proc.SourceVocabCd.Trim() == "1217")
                {
                    DateTime? procDate = CernerCommon.ConvertDateString(proc.ProcedureDateTime);
                    mtBAR.AddOutPatientCPT(proc.ProcCode, procDate, "", proc.ProcedurePhy, proc.ProcedurePhyAsst);
                }
            }
        }
        private void PatientGuarantor(Person guarantor, string guarRel)
        {
            
            string name = guarantor.FullName();
            string addr1 = string.Empty;
            string addr2 = string.Empty;
            string city = string.Empty;
            string state = string.Empty;
            string zipCode = string.Empty;
            string homePhone = string.Empty;
            string workPhone = string.Empty;

            if (guarantor.HomeAddress != null)
            {
                addr1 = guarantor.HomeAddress.Street_Addr;
                addr2 = CheckCode(guarantor.HomeAddress.Street_Addr2);
                city = guarantor.HomeAddress.City;
                state = guarantor.HomeAddress.State_Cd;
                zipCode = guarantor.HomeAddress.ZipCode;
            }
            if (guarantor.HomePhone != null)
                homePhone = guarantor.HomePhone.Phone_Num;
            if (guarantor.WorkPhone != null)
                workPhone = guarantor.WorkPhone.Phone_Num;

            string ssn = CernerCommon.GetSSN(guarantor.SocialSecurityNumber);
            string gtrNbr = ssn.Replace("-","");
            string relToPatient = guarRel;
            string mostRecentVisit = string.Empty;
            string institutionGuarantor = string.Empty;

            mtBAR.UpdateGuarantor(gtrNbr, name, addr1, addr2, city, state, zipCode, homePhone, workPhone, ssn, relToPatient, mostRecentVisit, institutionGuarantor);
        }
        private void PatientGuarantorEmployer(Employer guarEmp)
        {
            string addr1 = "";
            string addr2 = "";
            string city = "";
            string state = "";
            string zip = "";
            string phone = "";
            if (guarEmp.WorkAddress != null)
            {
                addr1 = guarEmp.WorkAddress.Street_Addr;
                addr2 = guarEmp.WorkAddress.Street_Addr2;
                city = guarEmp.WorkAddress.City;
                state = guarEmp.WorkAddress.State_Cd;
                zip = guarEmp.WorkAddress.ZipCode;
            }
            if (guarEmp.WorkPhone != null)
            {
                phone = guarEmp.WorkPhone.Phone_Num;
            }
            if (guarEmp.EmployerName != null)
                mtBAR.UpdateGuarantorEmployer(guarEmp.EmployerName, addr1, addr2, city, state, zip, phone, "", guarEmp.Occupation, guarEmp.EmploymentStatus, "");
        }
        
        private void PatientInsurances(List<Encntr_Plan_Reltn> encPlans)
        {
            int cnt = 0;
            string addr1 = string.Empty;
            string addr2 = string.Empty;
            string city = string.Empty;
            string state = string.Empty;
            string zipCode = string.Empty;
            string businessPhone = string.Empty;
            string workPhone = string.Empty;
            string country = string.Empty;
            string emailAddr = string.Empty;

            foreach (Encntr_Plan_Reltn encPlan in encPlans)
            {
                cnt = cnt + 1;

                if (_globalInsHash.Contains(cnt) == false)
                    _globalInsHash.Add(cnt, encPlan.InsuranceCode);

                if (encPlan.Address != null)
                {
                    addr1 = encPlan.Address.Street_Addr;
                    addr2 = CheckCode(encPlan.Address.Street_Addr2);
                    city = encPlan.Address.City;
                    state = encPlan.Address.State_Cd;
                    zipCode = encPlan.Address.ZipCode;
                }
                else
                {
                    addr1 = string.Empty;
                    addr2 = string.Empty;
                    city = string.Empty;
                    state = string.Empty;
                    zipCode = string.Empty;
                }
                if (encPlan.InsurancePhone != null)
                    businessPhone = encPlan.InsurancePhone.Phone_Num;
                else
                    businessPhone = string.Empty;
                string polNbr = encPlan.PolicyNbr;
                if (polNbr.Trim() == "null")
                    polNbr = "";
                mtBAR.AddInsurance(cnt, encPlan.InsuranceCode, encPlan.InsuranceName, addr1, addr2, city, state, zipCode, businessPhone,
                                   CernerCommon.GetCernerDate(encPlan.EffectiveDate,_hcis), 0, 0, 0, "", CernerCommon.GetCernerDate(encPlan.StatusDate,_hcis), CernerCommon.GetCernerDate(encPlan.InsuranceExpdate,_hcis), polNbr, CheckCode(encPlan.GroupNbr), CheckCode(encPlan.GroupName), encPlan.CoverageNbr);

                if (encPlan.SubscriberPerson != null)
                {
                    if (encPlan.SubscriberAddress != null)
                    {
                        addr1 = encPlan.SubscriberAddress.Street_Addr;
                        addr2 = CheckCode(encPlan.SubscriberAddress.Street_Addr2);
                        city = encPlan.SubscriberAddress.City;
                        state = encPlan.SubscriberAddress.State_Cd;
                        zipCode = encPlan.SubscriberAddress.ZipCode;
                        country = encPlan.SubscriberAddress.Country;
                    }
                    else
                    {
                        addr1 = string.Empty;
                        addr2 = string.Empty;
                        city = string.Empty;
                        state = string.Empty;
                        zipCode = string.Empty;
                        country = string.Empty;
                    }
                    if (encPlan.SubscriberPhone != null)
                        businessPhone = encPlan.SubscriberPhone.Phone_Num;
                    else
                        businessPhone = string.Empty;
                    if (encPlan.SubscriberEmailAddress != null)
                        emailAddr = encPlan.SubscriberEmailAddress.Street_Addr;
                    else
                        emailAddr = string.Empty;
                    mtBAR.UpdateInsuranceSubscriber(cnt, "", encPlan.SubscriberPerson.FullName(), encPlan.SubscriberPerson.SocialSecurityNumber, encPlan.SubscriberPerson.Sex_Cd, CernerCommon.GetCernerDate(encPlan.SubscriberPerson.Birth_Dt_Tm,_hcis), encPlan.Relationship, addr1,
                                   addr2, city, state, zipCode, country, businessPhone, emailAddr, encPlan.SubscriberPerson.Race_Cd, encPlan.SubscriberPerson.Marital_Type_Cd);
                }
                string item = "";
                if (encPlan.Authorizations != null)
                {
                    foreach (Authorization authorization in encPlan.Authorizations)
                    {
                        string authnbr = authorization.Authorization_Nbr;
                        string authsts = authorization.Authorization_Status;
                        //if (authnbr == null)
                        //    authnbr = "";
                        //if (authsts == null)
                        //    authsts = "";
                        //if (authnbr == "null" || authnbr == "0")
                        //    authnbr = "";
                        //if (authsts == "null" || authsts == "0")
                        //    authsts = "";
                        if (authnbr.Trim() == "" && authsts.Trim() == "")
                            item = "N";
                        else
                            mtBAR.AddInsuranceAuthorizations(cnt, authnbr, CernerCommon.GetCernerDate(authorization.Authorization_EffDate,_hcis), CernerCommon.GetCernerDate(authorization.Authorization_ExpDate,_hcis), authsts, authorization.Authorization_ReferalNbr);
                    }
                }
            }


            //mtBAR.AddInsurance(1, "Ins1", "name1,Last", "Address1", "Address2", "City1", "SC", "29601", "111-111-1111", DateTime.Today, 1.1M, 1.2M, 1.3M, "Status1", DateTime.Today, DateTime.Today,
            //    "policy1", "GroupNbr1", "GroupName1");
            //mtBAR.AddInsurance(2, "Ins2", "name2,Last", "Address2", "Address22", "City2", "SC", "29602", "222-111-1111", DateTime.Today, 2.1M, 2.2M, 2.3M, "Status2", DateTime.Today, DateTime.Today,
            //    "policy2", "GroupNbr2", "GroupName2");
            //mtBAR.AddInsurance(3, "Ins3", "name3,Last", "Address3", "Address23", "City3", "SC", "29603", "333-111-1111", DateTime.Today, 3.1M, 3.2M, 3.3M, "Status3", DateTime.Today, DateTime.Today,
            //    "policy3", "GroupNbr3", "GroupName3");

            //mtBAR.UpdateInsuranceSubscriber(2, "unr2", "LastName,2", "222-11-1111", "M", DateTime.Today, "R2", "Address2", "Address2", "City2", "SC", "29602", "Country2", "222-111-1111", "Email2.com", "R2", "M2");
            //mtBAR.UpdateInsuranceSubscriber(3, "unr3", "LastName,3", "333-11-1111", "M", DateTime.Today, "R3", "Address3", "Address3", "City3", "SC", "29603", "Country3", "333-111-1111", "Email3.com", "R3", "M3");

            
            //mtBAR.AddInsuranceAuthorizations(2, "Auth2.1", DateTime.Today, DateTime.Today, "Status2.1", "Referal2.1");
            //mtBAR.AddInsuranceAuthorizations(2, "Auth3", DateTime.Today, DateTime.Today, "Status3", "Referal3");
            //mtBAR.AddInsuranceAuthorizations(2, "Auth4.1", DateTime.Today, DateTime.Today, "Status4.1", "Referal4.1");

            //mtBAR.AddInsuranceQuery(3, "Query1", "Response1");
            //mtBAR.AddInsuranceQuery(3, "Query2", "Response2");
            //mtBAR.AddInsuranceQuery(3, "Query3", "Response3");
            //mtBAR.AddInsuranceQuery(3, "Query4", "Response4");
            //mtBAR.AddInsuranceQuery(3, "Query5", "Response5");

        }

        private void PatientQueries()
        {
            mtBAR.AddPatientQuery("Query1", "Response1");
            mtBAR.AddPatientQuery("Query2", "Response2");
            mtBAR.AddPatientQuery("Query3", "Response3");
            mtBAR.AddPatientQuery("Query4", "Response4");
            mtBAR.AddPatientQuery("Query5", "Response5");
            mtBAR.AddPatientQuery("Query6", "Response5");

        }

        private void CurrentBalance(PFT_Encntr pft_Encnter, List<PftProration> proRateList)
        {
            string ins = "";
            mtBAR.BalanceRecord.PatientAccountNumber = mtBAR.CurrentAccountNumber;
            if (_barStatus == "FB" || _barStatus == "UB")
            {
                mtBAR.BalanceRecord.AccountBalance = GetBalance(pft_Encnter.Balance, pft_Encnter.Dr_Cr_Flag);
                _globalFBBalance = _globalFBBalance + mtBAR.BalanceRecord.AccountBalance;
            }
            foreach (PftProration pro in proRateList)
            {
                if (_globalInsHash.ContainsKey(1) == true)
                {
                    ins = _globalInsHash[1].ToString();
                    if (ins == pro.HealthPlanID.Trim())
                    {
                        mtBAR.BalanceRecord.InsuranceBalance1 = GetBalance(pro.CurrAmtDue, pro.CurrAmtDueFlg);
                        mtBAR.BalanceRecord.InsuranceReceipts1 = 999;
                    }
                }
                else if (_globalInsHash.ContainsKey(2) == true)
                {
                    ins = _globalInsHash[2].ToString();
                    if (ins == pro.HealthPlanID.Trim())
                    {
                        mtBAR.BalanceRecord.InsuranceBalance2 = GetBalance(pro.CurrAmtDue, pro.CurrAmtDueFlg);
                        mtBAR.BalanceRecord.InsuranceReceipts2 = 999;
                    }
                }
                else if (_globalInsHash.ContainsKey(3) == true)
                {
                    ins = _globalInsHash[3].ToString();
                    if (ins == pro.HealthPlanID.Trim())
                    {
                        mtBAR.BalanceRecord.InsuranceBalance3 = GetBalance(pro.CurrAmtDue, pro.CurrAmtDueFlg);
                        mtBAR.BalanceRecord.InsuranceReceipts3 = 999;
                    }
                }
                else if (_globalInsHash.ContainsKey(4) == true)
                {
                    ins = _globalInsHash[4].ToString();
                    if (ins == pro.HealthPlanID.Trim())
                    {
                        mtBAR.BalanceRecord.InsuranceBalance4 = GetBalance(pro.CurrAmtDue, pro.CurrAmtDueFlg);
                        mtBAR.BalanceRecord.InsuranceReceipts4 = 999;
                    }
                }
            }
            mtBAR.BalanceRecord.SelfPayBalance = pft_Encnter.Pat_Bal_Fwd;
            //if (_barStatus == "FB" || _barStatus == "UB")
            if (_barStatus == "UB")
                mtBAR.BalanceRecord.URBalance = GetBalance(pft_Encnter.Balance, pft_Encnter.Dr_Cr_Flag);
            if (_barStatus == "BD")
            {
                if (mtBAR.BalanceRecord.BadDebtBalance == 0)
                {
                    mtBAR.BalanceRecord.BadDebtBalance = GetBalance(pft_Encnter.Balance, pft_Encnter.Bad_Debt_Bal_Dr_Cr_Flag);
                    _globalBDBalance = _globalBDBalance + mtBAR.BalanceRecord.AccountBalance;
                }
                else
                {
                    mtBAR.BalanceRecord.BadDebtBalance = GetBalance(pft_Encnter.Bad_Debt_Balance, pft_Encnter.Bad_Debt_Bal_Dr_Cr_Flag);
                    _globalBDBalance = _globalBDBalance + mtBAR.BalanceRecord.BadDebtBalance;
                }
            }
        }
        private bool ProcessBills(List<Bill_Record_Data> bills, List<PftProration> proRateList, decimal? spBalance)
        {
            string ins = "";
            int cnt = 1;
            decimal? totChgs = 0;
            decimal? totBal = 0;
            bool found = false;
            mtBAR.BillsRec.PatientAccountNumber = mtBAR.CurrentAccountNumber;
            mtBAR.BillsRec.BillNumber = "1";

            if (_globalInsHash.Count > 0)
            {
                foreach (PftProration pro in proRateList)
                {
                    if (_globalInsHash.ContainsKey(1) == true)
                    {
                        mtBAR.BillsRec.BillInsurance1 = _globalInsHash[1].ToString();
                        ins = mtBAR.BillsRec.BillInsurance1;
                        if (ins == pro.HealthPlanID.Trim())
                        {
                            mtBAR.BillsRec.BillInsBal1 = GetBalance(pro.CurrAmtDue, pro.CurrAmtDueFlg);
                        }
                    }
                    else if (_globalInsHash.ContainsKey(2) == true)
                    {
                        mtBAR.BillsRec.BillInsurance2 = _globalInsHash[2].ToString();
                        ins = mtBAR.BillsRec.BillInsurance2;
                        if (ins == pro.HealthPlanID.Trim())
                        {
                            mtBAR.BillsRec.BillInsBal2 = GetBalance(pro.CurrAmtDue, pro.CurrAmtDueFlg);
                        }
                    }
                    else if (_globalInsHash.ContainsKey(3) == true)
                    {
                        mtBAR.BillsRec.BillInsurance3 = _globalInsHash[3].ToString();
                        ins = mtBAR.BillsRec.BillInsurance3;
                        if (ins == pro.HealthPlanID.Trim())
                        {
                            mtBAR.BillsRec.BillInsBal3 = GetBalance(pro.CurrAmtDue, pro.CurrAmtDueFlg);
                        }
                    }
                    else if (_globalInsHash.ContainsKey(4) == true)
                    {
                        mtBAR.BillsRec.BillInsurance4 = _globalInsHash[4].ToString();
                        ins = mtBAR.BillsRec.BillInsurance4;
                        if (ins == pro.HealthPlanID.Trim())
                        {
                            mtBAR.BillsRec.BillInsBal4 = GetBalance(pro.CurrAmtDue, pro.CurrAmtDueFlg);
                        }
                    }
                }
                // mtBAR.BillsRec.BillTotCharges = "";
                
                foreach (Bill_Record_Data bill in bills)
                {
                    if (bill.BILL_STATUS_CD == "685131" || bill.BILL_STATUS_CD == "685130")
                    {
                        mtBAR.BillsRec.BillFromDate = CernerCommon.GetCernerDate(bill.STATEMENT_FROM_DT_TM,_hcis);
                        mtBAR.BillsRec.BillThruDate = CernerCommon.GetCernerDate(bill.STATEMENT_TO_DT_TM,_hcis);
                        //mtBAR.BillsRec.BillAgeDate = GetDate(bill.STATEMENT_TO_DT_TM);
                        if (cnt == 1)
                        {
                            mtBAR.BillsRec.BillTotCharges = GetBalance(decimal.Parse(bill.TOTAL_BILLED_AMOUNT), bill.TOTAL_BILLED_DR_CR_FLAG);
                            mtBAR.BillsRec.BillPostDate = CernerCommon.GetCernerDate(bill.SUBMIT_DT_TM,_hcis);
                        }
                        
                        decimal? billTotBal = GetBalance(decimal.Parse(CheckAmt(bill.CURR_AMT_DUE.Trim())), bill.CURR_AMOUNT_DR_CR_FLAG);
                        totBal = totBal + billTotBal;
                        cnt = cnt + 1;
                        found = true;
                    }
                }
                mtBAR.BillsRec.BillBalance = totBal;
                mtBAR.BillsRec.BillSPBal = spBalance;
                mtBAR.BillsRec.BillType = "FINAL";
                mtBAR.BillsRec.BillStatus = "POSTED";
               
            }
            return true;
        }
        private string CheckAmt(string amt)
        {
            string val1 = "";
            string val2 = "";
            string[] item = amt.Split('.');
            if(item.Length > 1)
            {
                val2 = item[1];
                if(val2.Length > 2)
                {
                    val2 = val2.Substring(0, 2);
                    amt = val1 + "." + val2;
                }
            }
            return amt;
        }
        private void ProcessBillsDetail(List<Bill_Record_Data> bills, List<PftProration> proRateList, decimal? spBalance)
        {
            Hashtable hash = new Hashtable();
            string ins = "";
            string PatientAccountNumber = "";
            string billNumber = "";
            string billIns ="";
            string[] recs;
            decimal? billInsAmt = 0;
            decimal? billProAmt = 0;
            decimal? billTotAdj = 0;
            decimal? billTotPay = 0;
            decimal? billTotBal = 0;

            decimal? billInsAmtSP = 0;
            decimal? billProAmtSP = 0;
            decimal? billTotAdjSP = 0;
            decimal? billTotPaySP = 0;
            decimal? billTotBalSP = 0;
            int cnt = 1;
           

            ArrayList list = new ArrayList();

            for(int x =1; x<= _globalInsHash.Count; x++ )
            {
                list.Add(_globalInsHash[x].ToString());
            }
            
            foreach (Bill_Record_Data bill in bills)
            {
                ins = bill.HEALTH_PLAN_ID;
                if (list.Contains(ins) == true)
                {
                    if (ins != "0")
                    {
                        if (bill.BILL_STATUS_CD == "685131" || bill.BILL_STATUS_CD == "685130")
                        {
                            if (hash.Contains(ins) == true)
                            {
                                recs = hash[ins].ToString().Split('\t');
                                PatientAccountNumber = recs[0];
                                billNumber = recs[1];
                                billIns = recs[2];
                                billInsAmt = decimal.Parse(recs[3]) + GetBalance(decimal.Parse(bill.BALANCE_DUE), bill.BALANCE_DUE_DR_CR_FLAG);
                                billProAmt = decimal.Parse(recs[4]) + GetBalance(decimal.Parse(bill.CURR_AMT_DUE), bill.CURR_AMOUNT_DR_CR_FLAG);
                                billTotAdj = decimal.Parse(recs[5]) + GetBalance(decimal.Parse(bill.TOTAL_ADJ), bill.TOTAL_ADJ_DR_CR_FLAG);
                                billTotPay = decimal.Parse(recs[6]) + GetBalance(decimal.Parse(bill.TOTAL_PAID_AMOUNT), bill.TOTAL_PAID_DR_CR_FLAG);
                                billTotBal = decimal.Parse(recs[7]) + GetBalance(decimal.Parse(bill.CURR_AMT_DUE), bill.CURR_AMOUNT_DR_CR_FLAG);
                                hash.Remove(ins);
                                hash.Add(ins, PatientAccountNumber + '\t' + billNumber + '\t' + billIns + '\t' + billInsAmt.ToString() + '\t' + billProAmt.ToString() + '\t' + billTotAdj.ToString() + '\t' + billTotPay.ToString() + '\t' + billTotBal.ToString());
                            }
                            else
                            {
                                PatientAccountNumber = mtBAR.CurrentAccountNumber;
                                billNumber = "1";
                                billIns = bill.HEALTH_PLAN_ID;
                                billInsAmt = GetBalance(decimal.Parse(bill.BALANCE_DUE), bill.BALANCE_DUE_DR_CR_FLAG);
                                billProAmt = GetBalance(decimal.Parse(bill.CURR_AMT_DUE), bill.CURR_AMOUNT_DR_CR_FLAG);
                                billTotAdj = GetBalance(decimal.Parse(bill.TOTAL_ADJ), bill.TOTAL_ADJ_DR_CR_FLAG);
                                billTotPay = GetBalance(decimal.Parse(bill.TOTAL_PAID_AMOUNT), bill.TOTAL_PAID_DR_CR_FLAG);
                                billTotBal = GetBalance(decimal.Parse(bill.CURR_AMT_DUE), bill.CURR_AMOUNT_DR_CR_FLAG);
                                hash.Add(ins, PatientAccountNumber + '\t' + billNumber + '\t' + billIns + '\t' + billInsAmt.ToString() + '\t' + billProAmt.ToString() + '\t' + billTotAdj.ToString() + '\t' + billTotPay.ToString() + '\t' + billTotBal.ToString());
                            }
                        }
                    }
                }
                
            }
            foreach(string text in hash.Keys)
            {
                string record = hash[text].ToString();
                recs = record.Split('\t');
                PatientAccountNumber = recs[0];
                billNumber = recs[1];
                billIns = recs[2];
                billInsAmt = decimal.Parse(recs[3]);
                billProAmt = decimal.Parse(recs[4]);
                billTotAdj = decimal.Parse(recs[5]);
                billTotPay = decimal.Parse(recs[6]);
                billTotBal = decimal.Parse(recs[7]);
                mtBAR.AddBillRecDetail(billNumber, billIns, billInsAmt, billProAmt, billTotPay,0, billTotAdj,0,0,0,0,0,999,0);
            }
            mtBAR.AddBillRecDetail("1", "3670808", spBalance, 0, 0, 0, 0, 0, 0, 0, 0, 0, 999, 0);
        }
        private void ProcessChargesTransactions(PFT_Encntr pft_Encnter, string facility)
        {
            string amt = "";
            string reversingFlag = string.Empty;
            decimal? amount = 0;
            List <Charge> charges = ChargeDAL.GetCharges(pft_Encnter.Encntr_Id, facility);
            foreach(Charge charge in charges)
            {
                amt = "";
                reversingFlag = "";
                amount = 0;

                string transactionType = "CHG";
                DateTime? serviceDate = CernerCommon.GetCernerDate(charge.Service_Dt_Tm,_hcis);
                string chargeCode = GMVCDM(charge.BILL_ITEM_ID, charge.Charge_Item_Id);
                int? count = int.Parse(Math.Round(Double.Parse(charge.Quantity),MidpointRounding.AwayFromZero).ToString());
                amt = charge.Item_Extended_Price;
                if (amt.Contains("-") == true)
                {
                    amt = amt.Replace("-", "");
                    amount = CernerCommon.ConvertDecimalString(amt);
                    reversingFlag = "1";
                }
                else
                {
                    amount = CernerCommon.ConvertDecimalString(amt);
                    reversingFlag = "";
                }
                string billNumber = string.Empty;
                string comment = string.Empty;
                string insurance = "";// charge.Payor_Id;
                string perfPhys = charge.Perf_Phy_Id;

                string orderPhys = charge.Ord_Phy_Id;
                string revenueSite = string.Empty;
                string nonChargeCode = string.Empty;
                string user = charge.User_Id;
                string systemComment = string.Empty;

                mtBAR.AddTransactionRecord(transactionType, serviceDate, chargeCode, count, amount, billNumber,
                    reversingFlag, comment, insurance, perfPhys, orderPhys, revenueSite, nonChargeCode, user, systemComment);
            }

            List<Pft_Trans_Reltn_Trans_Log> pftTransReltns = Pft_Trans_Reltn_Trans_LogDAL.GetPft_Trans_Reltn_Trans_Records(pft_Encnter.Pft_Encntr_Id);
            foreach (Pft_Trans_Reltn_Trans_Log pftTranReltn in pftTransReltns)
            {
                string transactionType = pftTranReltn.Trans_Type_Cd;
                if (transactionType == "10978")
                    transactionType = "ADJ";
                else if (transactionType == "10982")
                    transactionType = "PAY";
                DateTime? serviceDate = CernerCommon.GetCernerDate(pftTranReltn.Beg_Eff_Dt_Tm,_hcis);
                string chargeCode = string.Empty;
                int? count = 0;

                amt = GetBalance(decimal.Parse(pftTranReltn.Amount), pftTranReltn.Dr_Cr_Flag).ToString();
                if (amt.Contains("-") == true)
                {
                    amt = amt.Replace("-", "");
                    amount = CernerCommon.ConvertDecimalString(amt);
                    reversingFlag = "1";
                }
                else
                {
                    amount = CernerCommon.ConvertDecimalString(amt);
                    reversingFlag = "";
                }

                string billNumber = string.Empty;
                string comment = string.Empty;
                string insurance = "";
                if (pftTranReltn.Benefit_Order_Id.Trim() != "")
                {
                    insurance = BenefitOrderDAL.GetHealthPlan(pftTranReltn.Benefit_Order_Id.Trim());
                }
                string perfPhys = string.Empty;
                string orderPhys = string.Empty;
                string revenueSite = string.Empty;
                string nonChargeCode = Pft_Trans_AliasDAL.GetTransAlias(pftTranReltn.Trans_Alias_ID.Trim());
                string user = string.Empty;
                string systemComment = string.Empty;

                mtBAR.AddTransactionRecord(transactionType, serviceDate, chargeCode, count, amount, billNumber,
                    reversingFlag, comment, insurance, perfPhys, orderPhys, revenueSite, nonChargeCode, user, systemComment);
            }

            Hashtable notes = EncounterInfoDAL.GetEncounterInfo(pft_Encnter.Encntr_Id, facility);
            for (int y = 1; y <= notes.Count; y++)
            {
                Hashtable tHash = (Hashtable)notes[y];
                for (int x = 1; x <= tHash.Count; x++)
                {
                    string note_rec = tHash[x].ToString();
                    string[] note_data = note_rec.Split('\t');
                    string note_dt = note_data[0].Trim();
                    string note_line = note_data[1].Trim();

                    string transactionType = "NTE";
                    DateTime? serviceDate = CernerCommon.GetCernerDate(note_dt, _hcis);
                    int? count = 1;

                    if (note_line.Trim() != "")
                    {
                        mtBAR.AddTransactionRecord(transactionType, serviceDate, "", count, amount, "",
                            "", note_line, "", "", "", "", "", "", "");
                    }
                }
            }
            Hashtable cnotes = EncounterInfoDAL.GetEncounterInfo(pft_Encnter.Encntr_Id, facility);
            for (int y = 1; y <= cnotes.Count; y++)
            {
                Hashtable tHash = (Hashtable)cnotes[y];
                for (int x = 1; x <= tHash.Count; x++)
                {
                    string note_rec = tHash[x].ToString();
                    string[] note_data = note_rec.Split('\t');
                    string note_dt = note_data[0].Trim();
                    string note_line = note_data[1].Trim();

                    string transactionType = "NTE";
                    DateTime? serviceDate = CernerCommon.GetCernerDate(note_dt, _hcis);
                    int? count = 1;

                    if (note_line.Trim() != "")
                    {
                        mtBAR.AddTransactionRecord(transactionType, serviceDate, "", count, amount, "",
                            "", note_line, "", "", "", "", "", "", "");
                    }
                }
            }
            //Hashtable notes = EncounterInfoDAL.GetEncounterInfoV2(pft_Encnter.Encntr_Id, facility);
            //for (int y = 1; y <= notes.Count; y++)
            //{

            //    string note_rec = notes[y].ToString();
            //    string[] note_data = note_rec.Split('\t');
            //    string note_dt = note_data[0].Trim();
            //    string note_line = note_data[1].Trim();

            //    string transactionType = "NTE";
            //    DateTime? serviceDate = CernerCommon.GetCernerDate(note_dt, _hcis);
            //    int? count = 1;

            //    if (note_line.Trim() != "")
            //    {
            //        mtBAR.AddTransactionRecord(transactionType, serviceDate, "", count, 0, "",
            //            "", note_line, "", "", "", "", "", "", "");
            //    }

            //}
        }


        private List<string> GetOrganizations(string hcis, string hospFac)
        {
            List<string> orgs = new List<string>();
            orgs = MPIOrganizationDAL.GetOrganizationsToProcess(hcis, hospFac);
            return orgs;
        }

        private DateTime? ConvertDate(string dt)
        {
            DateTime? newDt;
            if (IsDate(dt) == true)
                newDt = DateTime.Parse(dt);
            else
                newDt = null;
            return newDt;
        }
        public static bool IsDate(string dt)
        {
            try
            {
                DateTime testDt = DateTime.Parse(dt);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static string FormatDT(string dt, string returnVal)
        {
            if (dt.Trim() == "")
                return "";
            else if (IsDate(dt) == true)
            {
                DateTime dateTime = DateTime.Parse(dt);
                if (returnVal == "date")
                {
                    string day = string.Format("{0:00}", dateTime.Day);
                    string month = string.Format("{0:00}", dateTime.Month);
                    string year = string.Format("{0:0000}", dateTime.Year);
                    return year + month + day;
                }
                else
                {
                    string time = dateTime.ToString("HH:mm");
                    return time;
                }
            }
            else
                return "";
        }
        public static string FormatDT(DateTime dateTime, string returnVal)
        {
            //if (dt.Trim() == "")
            //    return "";
            
            //DateTime dateTime = DateTime.Parse(dt);
            if (returnVal == "date")
            {
                string day = string.Format("{0:00}", dateTime.Day);
                string month = string.Format("{0:00}", dateTime.Month);
                string year = string.Format("{0:0000}", dateTime.Year);
                return year + month + day;
            }
            else
            {
                string time = dateTime.ToString("HH:mm");
                return time;
            }
            
        }
        private void SetGrouperVersion(DateTime? dischargeDate)
        {
            if (dischargeDate >= new DateTime(2015, 10, 01) && dischargeDate <= new DateTime(2016, 9, 30))
                _grouperVersion = "33";
            else if (dischargeDate >= new DateTime(2016, 10, 1) && dischargeDate <= new DateTime(2017, 9, 30))
                _grouperVersion = "34";
            else if (dischargeDate >= new DateTime(2017, 10, 1) && dischargeDate <= new DateTime(2018, 9, 30))
                _grouperVersion = "35";
            else if (dischargeDate >= new DateTime(2018, 10, 1) && dischargeDate <= new DateTime(2019, 9, 30))
                _grouperVersion = "36";
            else if (dischargeDate >= new DateTime(2019, 10, 1) && dischargeDate <= new DateTime(2020, 9, 30))
                _grouperVersion = "37";
        }

        private decimal? GetBalance(decimal? bal, string flg)
        {
            if (flg.Trim() == "2")
                bal = bal * -1;
            return bal;
        }
        private static string CheckSSN(string ssn)
        {
            ssn = ssn.Replace("-", "");
            if (ssn.Distinct().Count() > 1 && ssn != "999999999")
                ssn.PadLeft(9, '0');
            else
                ssn = "";
            return ssn;
        }
        public void LoadMapping()
        {
            global_Map = new Hashtable();
            CDMHash = new Hashtable();
            Hashtable returnHash = new Hashtable();

            returnHash = CernerMapDAL.GetCERNER_DISCHARGE_DISPOSITION();
            foreach (string x in returnHash.Keys)
            {
                global_Map.Add(x, returnHash[x].ToString());
            }

            returnHash = CernerMapDAL.GetCERNER_GENDER();
            foreach (string x in returnHash.Keys)
            {
                global_Map.Add(x, returnHash[x].ToString());
            }

            returnHash = CernerMapDAL.GetCERNER_LOCATION();
            foreach (string x in returnHash.Keys)
            {
                global_Map.Add(x, returnHash[x].ToString());
            }

            returnHash = CernerMapDAL.GetCERNER_MARITALSTATUS();
            foreach (string x in returnHash.Keys)
            {
                global_Map.Add(x, returnHash[x].ToString());
            }

            returnHash = CernerMapDAL.GetCERNER_PROVIDER_MAP();
            foreach (string x in returnHash.Keys)
            {
                global_Map.Add(x, returnHash[x].ToString());
            }

            returnHash = CernerMapDAL.GetCERNER_RACE();
            foreach (string x in returnHash.Keys)
            {
                global_Map.Add(x, returnHash[x].ToString());
            }

            returnHash = CernerMapDAL.GetCERNER_STATE();
            foreach (string x in returnHash.Keys)
            {
                global_Map.Add(x, returnHash[x].ToString());
            }

            returnHash = CernerMapDAL.GetCERNER_REGTYPE();
            foreach (string x in returnHash.Keys)
            {
                global_Map.Add(x, returnHash[x].ToString());
            }

            CDMHash = BillItemModDAL.GetBillItemCDM();
            _globalPhyTypeHash = SetPhyTypeMap();
        }
        public string GMV(string mapType, string cernerCd, string defaultVal)
        {
            if (global_Map.ContainsKey(mapType + "_" + cernerCd) == true)
                return global_Map[mapType + "_" + cernerCd].ToString();
            else
            {
                if (defaultVal == "CernerCode")
                    return cernerCd;
                else
                    return defaultVal;
            }
        }
        public string GMV_PROV(string mapType, string fac, string prsnlID, string defaultVal)
        {
            string npi = "";
            if (prsnlID == "" || prsnlID == "0" || prsnlID == "0.00")
                return "";
            npi = CernerMapDAL.GetPRSNL_ALIAS(prsnlID);
            if (npi != "")
            {
                if (global_Map.ContainsKey(mapType + "_" + fac + "_" + npi) == true)
                    return global_Map[mapType + "_" + fac + "_" + npi].ToString();
                else
                    return defaultVal;
            }
            return "";
        }
        private DateTime? GetDate(string date)
        {
            if(Huron.HuronRoutines.IsDate(date) == true)
            {
                return DateTime.Parse(date);
            }
            return null;
        }
        public string GMVCDM(string tranCd, string chargeItemID)
        {
            string cd = "";
            if (CDMHash.ContainsKey(tranCd) == true)
                return CDMHash[tranCd.Trim()].ToString();
            else
            {
                cd = BillItemModDAL.GetChargeMod(chargeItemID.Trim());
                if (cd == "")
                    return tranCd;
                else
                    return cd;
            }
        }
        public string CheckCode(string cd)
        {
            if (cd.Trim() == "0" || cd.Trim().ToUpper() == "NULL")
                return "";
            else
                return cd;

        }

        private Hashtable SetPhyTypeMap ()
        {
            bool eof = false;
            string rec = "";
            string[] recs;
            DataIO fI1 = new DataIO();
            Hashtable hash = new Hashtable();
            
            fI1.OpenFile(@"E:\Huron\Program\BAR_Conversion\WestBARConversion\WestBARConversionSLN\WestBARConversion\CernerCore\ZPhyTypeMap.txt", DataIO.IO.INPUT);
            
            while (eof == false)
            {
                fI1.ReadRecord();
                rec = fI1.RecordData;
                if (rec == "END OF FILE")
                    break;
                else
                {
                    recs = rec.Split('\t');
                    if (hash.ContainsKey(recs[0].Trim()) == false)
                    {
                        hash.Add(recs[0].Trim(),recs[1].Trim());
                    }

                }
            }
            return hash;
        }

        public string GetPhyTypeMap(string ptCode)
        {
            ptCode = ptCode.Trim();
            if (_globalPhyTypeHash.ContainsKey(ptCode) == true)
                return _globalPhyTypeHash[ptCode].ToString();
            else
                return ptCode;
        }
    }
}
