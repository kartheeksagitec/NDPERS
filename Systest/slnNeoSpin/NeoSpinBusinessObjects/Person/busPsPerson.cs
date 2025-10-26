#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
using NeoSpin.DataObjects;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class NeoSpin.BusinessObjects.busPsPerson:
    /// Inherited from busPsPersonGen, the class is used to customize the business object busPsPersonGen.
    /// </summary>
    [Serializable]
    public class busPsPerson : busPsPersonGen
    {
        public bool iblnMaritialchangesRequired { get; set; }
        public Collection<busPsPerson> iclbPsPersonPerson { get; set; }
        public Collection<busPsPerson> iclbProcessedPsPerson { get; set; }
        public Collection<busPsPerson> iclbUnprocessedPsPerson { get; set; }
        //public Collection<busPsPerson> iclbPersonInformativeErrors { get; set; }
        public Collection<busPsPerson> iclbErrorList { get; set; }
        public busPsPerson ibusPsPerson { get; set; }
        public ArrayList iarrbPersonErrorList { get; set; }
        public Dictionary<int, ArrayList> idictPsPersonError = new Dictionary<int, ArrayList>();
        //public ArrayList iarrErrors = new ArrayList(); 
        
        // 5 September
        public busPerson ibusPerson { get; set; }
        public busPsAddress ibusPsAddress { get; set; }
        public bool iblnEmailChanged { get; set; }
        public bool iblnPhoneNumberChanged { get; set; }
        public bool iblnDemographicChange { get; set; }
        public busPsAddress ibusPsEmployment { get; set; }
        static string istrUserId = "PERSLinkBatch";

        //PIR 12578
        public bool iblnEmploymentChangeRequestChangeTypeClassification { get; set; }
        public bool iblnEmploymentChangeRequestChangeTypeEmployment { get; set; }
        public bool iblnEmploymentChangeRequestChangeTypeLOA { get; set; }
        public bool iblnEmploymentChangeRequestChangeTypeLOAM { get; set; }
        public bool iblnEmploymentChangeRequestChangeTypeFMLA { get; set; }

        public busPsPerson()
        {
        }

        public void LoadUnprocessedPsPerson()
        {
            //DataTable ldtPsPerson = Select<cdoPsPerson>(new string[1] { enmPsPerson.processed_flag.ToString() },
                                                                       //new object[1] { busConstant.Flag_No }, null, null);
            DataTable ldtPsPerson = Select("cdoPsPerson.GetUnprocessedPsPerson", new object[] { });
            iclbPsPersonPerson = GetCollection<busPsPerson>(ldtPsPerson, "icdoPsPerson");
        }
        public void InsertPerson(ref Collection<busPsPerson> iclbPersonInformativeErrors)
        {
            iclbProcessedPsPerson = new Collection<busPsPerson>();
            iclbUnprocessedPsPerson = new Collection<busPsPerson>();
            //iclbPersonInformativeErrors = new Collection<busPsPerson>();
            ArrayList iarrbPersonErrorList = new ArrayList();
            foreach (busPsPerson lbusPsPerson in iclbPsPersonPerson)
            {
                lbusPsPerson.idictPsPersonError = new Dictionary<int, ArrayList>();
                lbusPsPerson.iarrErrors = new ArrayList();
                busWssMemberRecordRequest lbusWssMemberRecordRequest = null; 
                //busWssMemberRecordRequest lbusWssMemberRecordRequest = new busWssMemberRecordRequest() { icdoWssMemberRecordRequest = new cdoWssMemberRecordRequest() };
                //busWssMemberRecordRequest lobjPendingWssMemberRecordRequest = lbusWssMemberRecordRequest.LoadPendingMemberRecordRequestbySSN(lbusPsPerson.icdoPsPerson.ssn);

                try
                {
                     
                    //lbusWssMemberRecordRequest = new busWssMemberRecordRequest() { icdoWssMemberRecordRequest = new cdoWssMemberRecordRequest() }; //Commented on 15 October
                    //lbusPsPerson.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                    if (!lbusPsPerson.icdoPsPerson.ssn.IsNumeric())
                    {
                        utlError lobjError = null;
                        lobjError = AddError(1037, "SSN must be numeric.");
                        lbusPsPerson.iarrErrors.Add(lobjError);
                        iclbUnprocessedPsPerson.Add(lbusPsPerson);
                        if (!lbusPsPerson.idictPsPersonError.ContainsKey(lbusPsPerson.icdoPsPerson.ps_person_id))
                            lbusPsPerson.idictPsPersonError.Add(lbusPsPerson.icdoPsPerson.ps_person_id, lbusPsPerson.iarrErrors);
                        continue;
                    }

                    //PIR 26051 - Throw an error if new record PeopleSoft ID = Existing PeopleSoft ID and New record SSN != Existing SSN for the same person
                    if (Convert.ToInt32(DBFunction.DBExecuteScalar("entPsPerson.CheckIfSSNAndPeopleSoftIDMatches", new object[2]
                            { lbusPsPerson.icdoPsPerson.ssn, lbusPsPerson.icdoPsPerson.peoplesoft_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework)) > 0)
                    {
                        utlError lobjError = null;
                        lobjError = AddError(10498, "PeopleSoft ID already exists.");
                        lbusPsPerson.iarrErrors.Add(lobjError);
                        iclbUnprocessedPsPerson.Add(lbusPsPerson);
                        if (!lbusPsPerson.idictPsPersonError.ContainsKey(lbusPsPerson.icdoPsPerson.ps_person_id))
                            lbusPsPerson.idictPsPersonError.Add(lbusPsPerson.icdoPsPerson.ps_person_id, lbusPsPerson.iarrErrors);

                        continue;
                    }

                    //PIR 23927 - Record will be inserted into Error report if gender and marital status is 'UNKN', 'Null' or ' '.
                    if (lbusPsPerson.icdoPsPerson.gender_value == "UNKN" || string.IsNullOrWhiteSpace(lbusPsPerson.icdoPsPerson.gender_value))
                    {
                        utlError lobjError = null;
                        lobjError = AddError(10412, "Invalid or missing Gender.");
                        lbusPsPerson.iarrErrors.Add(lobjError);
                        iclbUnprocessedPsPerson.Add(lbusPsPerson);
                        if (!lbusPsPerson.idictPsPersonError.ContainsKey(lbusPsPerson.icdoPsPerson.ps_person_id))
                            lbusPsPerson.idictPsPersonError.Add(lbusPsPerson.icdoPsPerson.ps_person_id, lbusPsPerson.iarrErrors);
                        continue;
                    }

                    if (lbusPsPerson.icdoPsPerson.marital_status_value == "UNKN" || string.IsNullOrWhiteSpace(lbusPsPerson.icdoPsPerson.marital_status_value))
                    {
                        utlError lobjError = null;
                        lobjError = AddError(10413, "Invalid or missing Marital Status.");
                        lbusPsPerson.iarrErrors.Add(lobjError);
                        iclbUnprocessedPsPerson.Add(lbusPsPerson);
                        if (!lbusPsPerson.idictPsPersonError.ContainsKey(lbusPsPerson.icdoPsPerson.ps_person_id))
                            lbusPsPerson.idictPsPersonError.Add(lbusPsPerson.icdoPsPerson.ps_person_id, lbusPsPerson.iarrErrors);
                        continue;
                    }

                    Collection<busPsEmployment> lclbPsEmp = new Collection<busPsEmployment>();
                    lbusPsPerson.ibusPerson = LoadPersonBySSN(lbusPsPerson.icdoPsPerson.ssn);
                    //CreateMemberRecordRequest(lbusWssMemberRecordRequest, lbusPsPerson);
                    lbusPsPerson.ibusPsAddress = LoadPSPersonAddressBySSN(lbusPsPerson.icdoPsPerson.ssn);

                    lclbPsEmp = LoadPsEmploymentBySSN(lbusPsPerson.icdoPsPerson.ssn);

                    if (lbusPsPerson.ibusPerson != null) //check the primary key 
                    {
                        lbusPsPerson.ibusPerson.icdoPerson.ienuObjectState = ObjectState.Update;
                        lbusPsPerson.ibusPerson.iblnIsFromPs = true;
                        lbusPsPerson.ibusPerson.icdoPerson.temp_ssn = lbusPsPerson.icdoPsPerson.ssn;
                        lbusPsPerson.ibusPerson.icdoPerson.peoplesoft_id = lbusPsPerson.icdoPsPerson.peoplesoft_id;
                        lbusPsPerson.ibusPerson.icdoPerson.name_prefix_value = lbusPsPerson.icdoPsPerson.name_prefix_value;
                        lbusPsPerson.ibusPerson.icdoPerson.first_name = lbusPsPerson.icdoPsPerson.first_name;
                        lbusPsPerson.ibusPerson.icdoPerson.middle_name = lbusPsPerson.icdoPsPerson.middle_name;
                        lbusPsPerson.ibusPerson.icdoPerson.last_name = lbusPsPerson.icdoPsPerson.last_name;
                        lbusPsPerson.ibusPerson.icdoPerson.name_suffix_value = lbusPsPerson.icdoPsPerson.name_suffix_value;
                        lbusPsPerson.ibusPerson.icdoPerson.date_of_birth = lbusPsPerson.icdoPsPerson.date_of_birth;
                        lbusPsPerson.ibusPerson.icdoPerson.gender_value = lbusPsPerson.icdoPsPerson.gender_value;
                        // 22 September Check In 
                        //not load Marital Status and Marital Status Change Date from PS if the person exists in PERSLink unless an employment record is also sent from PS or the person does not exist.
                        //*** Commented on 12/14/2013 as per Maik's email

                        //if (lclbPsEmp != null && lclbPsEmp.Count > 0) //
                        //{
                        //    if (lbusPsPerson.icdoPsPerson.marital_status_value != lbusPsPerson.ibusPerson.icdoPerson.marital_status_value)
                        //    {
                        //        utlError lobjError = null;
                        //        lobjError = AddError(10209, "Martial Status cannot be updated.");
                        //        lbusPsPerson.iarrErrors.Add(lobjError);
                        //        iclbUnprocessedPsPerson.Add(lbusPsPerson);
                        //        iclbPersonInformativeErrors.Add(lbusPsPerson);
                        //        if (!lbusPsPerson.idictPsPersonError.ContainsKey(lbusPsPerson.icdoPsPerson.ps_person_id))
                        //            lbusPsPerson.idictPsPersonError.Add(lbusPsPerson.icdoPsPerson.ps_person_id, lbusPsPerson.iarrErrors);
                        //    }
                        //    else
                        //{
                        //PIR 24927 - Update ms_change_batch_flag to Y if there is change in martital status. Skip members with DOD.
                        if (lbusPsPerson.ibusPerson.icdoPerson.marital_status_value != lbusPsPerson.icdoPsPerson.marital_status_value 
                            && lbusPsPerson.icdoPsPerson.marital_status_value != busConstant.PersonMaritalStatusWidow )
                        {
                            busCorTracking lobjCorTracking = new busCorTracking { icdoCorTracking = new cdoCorTracking() };
                            DataTable ldtbCorTracking = Select("entCorTracking.GetCorrTrackPersonTemplate", new object[2] { lbusPsPerson.ibusPerson.icdoPerson.person_id, busConstant.TemplateID_PER0055 });

                            if (ldtbCorTracking.Rows.Count > 0)
                            {
                                lobjCorTracking.icdoCorTracking.LoadData(ldtbCorTracking.Rows[0]);

                                if (lobjCorTracking.icdoCorTracking.IsNotNull() &&
                                    busGlobalFunctions.DateDiffInDays(lobjCorTracking.icdoCorTracking.generated_date, busGlobalFunctions.GetSysManagementBatchDate()) >= 6)
                                {
                                    lbusPsPerson.ibusPerson.icdoPerson.ms_change_batch_flag = busConstant.Flag_Yes;
                                }
                            }//New member - No corr generated ever for the member, set the flag to Y. 
                            else
                                lbusPsPerson.ibusPerson.icdoPerson.ms_change_batch_flag = busConstant.Flag_Yes;
                        }

                        lbusPsPerson.ibusPerson.icdoPerson.marital_status_value = lbusPsPerson.icdoPsPerson.marital_status_value;
                        lbusPsPerson.ibusPerson.icdoPerson.ms_change_date = lbusPsPerson.icdoPsPerson.ms_change_date;
                        //    }
                        //}
                        //else
                        //{
                        //    if (lbusPsPerson.icdoPsPerson.marital_status_value != lbusPsPerson.ibusPerson.icdoPerson.marital_status_value)
                        //    {
                        //        utlError lobjError = null;
                        //        lobjError = AddError(10209, "Martial Status cannot be updated.");
                        //        lbusPsPerson.iarrErrors.Add(lobjError);
                        //        iclbUnprocessedPsPerson.Add(lbusPsPerson);
                        //        iclbPersonInformativeErrors.Add(lbusPsPerson);
                        //        if (!lbusPsPerson.idictPsPersonError.ContainsKey(lbusPsPerson.icdoPsPerson.ps_person_id))
                        //            lbusPsPerson.idictPsPersonError.Add(lbusPsPerson.icdoPsPerson.ps_person_id, lbusPsPerson.iarrErrors);
                        //    }
                        //}
                        lbusPsPerson.ibusPerson.icdoPerson.work_phone_no = lbusPsPerson.icdoPsPerson.work_phone_no;
                        lbusPsPerson.ibusPerson.icdoPerson.work_phone_ext = lbusPsPerson.icdoPsPerson.work_phone_ext;
                        lbusPsPerson.ibusPerson.icdoPerson.cell_phone_no = lbusPsPerson.icdoPsPerson.cell_phone_no;
                        lbusPsPerson.ibusPerson.icdoPerson.home_phone_no = lbusPsPerson.icdoPsPerson.home_phone_no;
                        //lbusPsPerson.ibusPerson.icdoPerson.Update();
                        lbusPsPerson.ibusPerson.ValidateHardErrors(utlPageMode.Update);
                        if (lbusPsPerson.ibusPerson.iarrErrors != null && lbusPsPerson.ibusPerson.iarrErrors.Count > 0)
                        {
                            iclbUnprocessedPsPerson.Add(lbusPsPerson);
                            if (!lbusPsPerson.idictPsPersonError.ContainsKey(lbusPsPerson.icdoPsPerson.ps_person_id))
                                lbusPsPerson.idictPsPersonError.Add(lbusPsPerson.icdoPsPerson.ps_person_id, lbusPsPerson.ibusPerson.iarrErrors);
                            if (lbusPsPerson.ibusPsAddress == null)
                                continue;
                        }
                        else
                        {
                            lbusPsPerson.ibusPerson.BeforePersistChanges();
                            lbusPsPerson.ibusPerson.PersistChanges();
                            lbusPsPerson.ibusPerson.AfterPersistChanges();
                            if (lbusPsPerson.ibusPsAddress == null)
                            {
                                if (!iclbProcessedPsPerson.Contains(lbusPsPerson))
                                    iclbProcessedPsPerson.Add(lbusPsPerson);
                                continue;
                            }
                        }

                    }
                    else if (lbusPsPerson.ibusPerson.IsNull() && lclbPsEmp.Count == 0)
                    {
                        busPerson lobjNewPerson = new busPerson { icdoPerson = new cdoPerson() };
                        lobjNewPerson.icdoPerson.ienuObjectState = ObjectState.Insert;
                        lobjNewPerson.iblnIsFromPs = true;
                        lobjNewPerson.icdoPerson.ssn = lbusPsPerson.icdoPsPerson.ssn;
                        lobjNewPerson.icdoPerson.peoplesoft_id = lbusPsPerson.icdoPsPerson.peoplesoft_id;
                        lobjNewPerson.icdoPerson.name_prefix_value = lbusPsPerson.icdoPsPerson.name_prefix_value;
                        lobjNewPerson.icdoPerson.first_name = lbusPsPerson.icdoPsPerson.first_name;
                        lobjNewPerson.icdoPerson.middle_name = lbusPsPerson.icdoPsPerson.middle_name;
                        lobjNewPerson.icdoPerson.last_name = lbusPsPerson.icdoPsPerson.last_name;
                        lobjNewPerson.icdoPerson.name_suffix_value = lbusPsPerson.icdoPsPerson.name_suffix_value;
                        lobjNewPerson.icdoPerson.date_of_birth = lbusPsPerson.icdoPsPerson.date_of_birth;
                        lobjNewPerson.icdoPerson.gender_value = lbusPsPerson.icdoPsPerson.gender_value;
                        lobjNewPerson.icdoPerson.marital_status_value = lbusPsPerson.icdoPsPerson.marital_status_value;
                        lobjNewPerson.icdoPerson.ms_change_date = lbusPsPerson.icdoPsPerson.ms_change_date;
                        lobjNewPerson.icdoPerson.work_phone_no = lbusPsPerson.icdoPsPerson.work_phone_no;
                        lobjNewPerson.icdoPerson.work_phone_ext = lbusPsPerson.icdoPsPerson.work_phone_ext;
                        lobjNewPerson.icdoPerson.cell_phone_no = lbusPsPerson.icdoPsPerson.cell_phone_no;
                        lobjNewPerson.icdoPerson.home_phone_no = lbusPsPerson.icdoPsPerson.home_phone_no;
                        lobjNewPerson.ValidateHardErrors(utlPageMode.New);
                        if (lobjNewPerson.iarrErrors != null && lobjNewPerson.iarrErrors.Count > 0)
                        {
                            iclbUnprocessedPsPerson.Add(lbusPsPerson);
                            if (!lbusPsPerson.idictPsPersonError.ContainsKey(lbusPsPerson.icdoPsPerson.ps_person_id))
                                lbusPsPerson.idictPsPersonError.Add(lbusPsPerson.icdoPsPerson.ps_person_id, lobjNewPerson.iarrErrors);
                            if (lbusPsPerson.ibusPsAddress == null)
                                continue;
                        }
                        else
                        {
                            lobjNewPerson.BeforePersistChanges();
                            lobjNewPerson.PersistChanges();
                            lobjNewPerson.AfterPersistChanges();
                            if (lbusPsPerson.ibusPsAddress == null)
                            {
                                if (!iclbProcessedPsPerson.Contains(lbusPsPerson))
                                    iclbProcessedPsPerson.Add(lbusPsPerson);
                                continue;
                            }
                            lbusPsPerson.ibusPerson = LoadPersonBySSN(lbusPsPerson.icdoPsPerson.ssn);
                        }
                    }

                    //9-11 Changes 
                    if (lbusPsPerson.ibusPsAddress != null && lbusPsPerson.ibusPsAddress.icdoPsAddress.processed_flag == busConstant.Flag_No)
                    {
                        if (lbusPsPerson.ibusPerson != null)
                        {
                            // 22 September Check In 
                            //Changed the loading logic as there is futrue dated Address End Date . Need to load the respective Current address according to the date.
                            //2 October Changes Start
                            lbusPsPerson.ibusPerson.LoadPersonCurrentAddress();
                            if (lbusPsPerson.ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress != null)
                            {
                                if (lbusPsPerson.ibusPsAddress.icdoPsAddress.addr_start_date > lbusPsPerson.ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.start_date)
                                {
                                    lbusPsPerson.ibusPerson.LoadPersonCurrentAddress(lbusPsPerson.ibusPsAddress.icdoPsAddress.addr_start_date);
                                }
                            }
                            else
                            {
                                lbusPsPerson.ibusPsAddress.iblnNewAndFirstAddress = true;
                                //*** Commented  as per Maik's email dated 03/07/2014(PIR 11030) - error should not be reported
                                //utlError lobjError = null;
                                //lobjError = AddError(10212, "Person does not have active address.");
                                //lbusPsPerson.iarrErrors.Add(lobjError);
                                //iclbUnprocessedPsPerson.Add(lbusPsPerson);
                                ////iclbPersonInformativeErrors.Add(lbusPsPerson);
                                //if (!lbusPsPerson.idictPsPersonError.ContainsKey(lbusPsPerson.icdoPsPerson.ps_person_id))
                                //    lbusPsPerson.idictPsPersonError.Add(lbusPsPerson.icdoPsPerson.ps_person_id, lbusPsPerson.iarrErrors);
                                //continue;
                            }
                            //2 October Changes End
                            if (lbusPsPerson.ibusPerson.ibusPersonCurrentAddress != null && lbusPsPerson.ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress != null && lbusPsPerson.ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.person_address_id > 0)
                            {
                                string lstrAddrCountryValue = busGlobalFunctions.GetData1ByCodeValue(lbusPsPerson.ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_country_id, lbusPsPerson.ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_country_value, iobjPassInfo);

                                if (lbusPsPerson.ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_line_1.ToUpper() != lbusPsPerson.ibusPsAddress.icdoPsAddress.addr_line_1.ToUpper() ||
                                    (lbusPsPerson.ibusPsAddress.icdoPsAddress.addr_line_2.IsNotNullOrEmpty() && lbusPsPerson.ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_line_2.IsNotNullOrEmpty() ? lbusPsPerson.ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_line_2.Trim().ToUpper() != lbusPsPerson.ibusPsAddress.icdoPsAddress.addr_line_2.Trim().ToUpper() : false) ||
                                    lbusPsPerson.ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_city.ToUpper() != lbusPsPerson.ibusPsAddress.icdoPsAddress.addr_city.ToUpper()
                                    // || lbusPsPerson.ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_zip_4_code != lbusPsPerson.ibusPsAddress.icdoPsAddress.addr_zip_4_code   -- PIR 20276 PeopleSoft Inbound File - Address update
                                    || lbusPsPerson.ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_zip_code != lbusPsPerson.ibusPsAddress.icdoPsAddress.addr_zip_code ||
                                    lstrAddrCountryValue.ToUpper() != lbusPsPerson.ibusPsAddress.icdoPsAddress.addr_country_value.ToUpper() ||
                                    //lbusPsPerson.ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.start_date != lbusPsPerson.ibusPsAddress.icdoPsAddress.addr_start_date ||
                                    lbusPsPerson.ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_state_value.ToUpper() != lbusPsPerson.ibusPsAddress.icdoPsAddress.addr_state_value.ToUpper())
                                    lbusPsPerson.ibusPsAddress.iblnAddressChanged = true;
                            }
                        }
                        if ((lbusPsPerson.ibusPsAddress.iblnAddressChanged || lbusPsPerson.ibusPsAddress.iblnNewAndFirstAddress) //PIR 11030 - as per Maik's mail dated 03/07/2014
                            && (lclbPsEmp.Count == 0)) //As per mail from Maik dated 03/20/2014, Issue#1
                        {
                            // 9/11  Changes Start
                            busPersonAddress lbusPersonAddress = new busPersonAddress { icdoPersonAddress = new cdoPersonAddress() };
                            lbusPersonAddress.icdoPersonAddress.ienuObjectState = ObjectState.Insert;
                            lbusPersonAddress.icdoPersonAddress.addr_line_1 = lbusPsPerson.ibusPsAddress.icdoPsAddress.addr_line_1;
                            lbusPersonAddress.icdoPersonAddress.addr_line_2 = lbusPsPerson.ibusPsAddress.icdoPsAddress.addr_line_2;
                            lbusPersonAddress.icdoPersonAddress.addr_city = lbusPsPerson.ibusPsAddress.icdoPsAddress.addr_city;
                            lbusPersonAddress.icdoPersonAddress.addr_zip_4_code = lbusPsPerson.ibusPsAddress.icdoPsAddress.addr_zip_4_code;
                            lbusPersonAddress.icdoPersonAddress.addr_zip_code = lbusPsPerson.ibusPsAddress.icdoPsAddress.addr_zip_code;
                            lbusPersonAddress.icdoPersonAddress.addr_country_value = busGlobalFunctions.GetCodeValueDetailsfromData1(151, lbusPsPerson.ibusPsAddress.icdoPsAddress.addr_country_value, iobjPassInfo);
                            if (lbusPsPerson.ibusPsAddress.icdoPsAddress.addr_start_date <= DateTime.Now)
                                lbusPersonAddress.icdoPersonAddress.start_date = DateTime.Now;
                            else
                                lbusPersonAddress.icdoPersonAddress.start_date = lbusPsPerson.ibusPsAddress.icdoPsAddress.addr_start_date;
                            lbusPersonAddress.icdoPersonAddress.addr_state_value = lbusPsPerson.ibusPsAddress.icdoPsAddress.addr_state_value;
                            lbusPersonAddress.icdoPersonAddress.address_type_value = busConstant.AddressTypePermanent;
                            lbusPersonAddress.icdoPersonAddress.person_id = lbusPsPerson.ibusPerson.icdoPerson.person_id;
                            lbusPersonAddress.icdoPersonAddress.peoplesoft_flag = busConstant.Flag_Yes;
                            //systest pir 2323
                            lbusPersonAddress.icdoPersonAddress.addr_state_value = lbusPsPerson.ibusPsAddress.icdoPsAddress.addr_state_value;
                            if (lbusPsPerson.ibusPsAddress.iblnAddressChanged) //PIR 11030 - as per Maik's mail dated 03/07/2014
                            {
                                if (lbusPsPerson.ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.start_date.Date == lbusPersonAddress.icdoPersonAddress.start_date.Date)
                                    lbusPsPerson.ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.end_date = lbusPersonAddress.icdoPersonAddress.start_date;
                                else
                                    lbusPsPerson.ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.end_date = lbusPersonAddress.icdoPersonAddress.start_date.AddDays(-1);
                                lbusPsPerson.ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.Update();
                            }
                            lbusPersonAddress.LoadOtherAddressses();
                            lbusPersonAddress.LoadPerson();
                            lbusPersonAddress.BeforeValidate(utlPageMode.New);
                            lbusPersonAddress.ValidateHardErrors(utlPageMode.New);
                            if (lbusPersonAddress.iarrErrors.Count > 0)
                            {
                                lbusPsPerson.ibusPsAddress.iblnAddressChanged = false;
                                iclbUnprocessedPsPerson.Add(lbusPsPerson);
                                if (!lbusPsPerson.idictPsPersonError.ContainsKey(lbusPsPerson.icdoPsPerson.ps_person_id))
                                    lbusPsPerson.idictPsPersonError.Add(lbusPsPerson.icdoPsPerson.ps_person_id, lbusPersonAddress.iarrErrors);
                                continue;
                            }
                            else
                            {
                                lbusPersonAddress.BeforePersistChanges();
                                lbusPersonAddress.PersistChanges();
                                lbusPersonAddress.AfterPersistChanges();
                            }
                            if (lclbPsEmp == null || lclbPsEmp.Count == 0)
                            {
                                if (!iclbProcessedPsPerson.Contains(lbusPsPerson))
                                    iclbProcessedPsPerson.Add(lbusPsPerson);
                                continue;
                            }
                            //9/11  Changes End
                        }
                        else
                        {
                            if (lclbPsEmp == null || lclbPsEmp.Count == 0) // Added on 2nd October
                            {
                                if (lbusPsPerson.ibusPerson != null)
                                {
                                    if (!iclbProcessedPsPerson.Contains(lbusPsPerson))
                                        iclbProcessedPsPerson.Add(lbusPsPerson);
                                    continue;
                                }
                                else // 7 October 
                                {
                                    utlError lobjError = null;
                                    lobjError = AddError(4571, "Person ID does not exist. Person has to be enroll in PERSLink System.");
                                    lbusPsPerson.iarrErrors.Add(lobjError);
                                    iclbUnprocessedPsPerson.Add(lbusPsPerson);
                                    if (!lbusPsPerson.idictPsPersonError.ContainsKey(lbusPsPerson.icdoPsPerson.ps_person_id))
                                        lbusPsPerson.idictPsPersonError.Add(lbusPsPerson.icdoPsPerson.ps_person_id, lbusPsPerson.iarrErrors);
                                    continue;
                                }
                            }
                        }

                       // lbusPsPerson.ibusPsAddress.SetWssPersonAddressInfo(lbusWssMemberRecordRequest);
                        #region Ps Employment
                        Collection<busPsEmployment> lclbPsEmployment = LoadPsEmploymentBySSN(lbusPsPerson.icdoPsPerson.ssn);
                        if (lclbPsEmployment.Count == 0)
                        {
                            utlError lobjError = null;
                            lobjError = AddError(10206, "PS Employment Record does not exist.");
                            lbusPsPerson.iarrErrors.Add(lobjError);
                            iclbUnprocessedPsPerson.Add(lbusPsPerson);
                            if (!lbusPsPerson.idictPsPersonError.ContainsKey(lbusPsPerson.icdoPsPerson.ps_person_id))
                                lbusPsPerson.idictPsPersonError.Add(lbusPsPerson.icdoPsPerson.ps_person_id, lbusPsPerson.iarrErrors);
                            continue;
                        }
                        else
                        {
                            foreach (busPsEmployment lbusPsEmployment in lclbPsEmployment)
                            {
                                    busWssPersonEmployment lbusWssPersonEmployment = new busWssPersonEmployment { icdoWssPersonEmployment = new cdoWssPersonEmployment() };
                                    busWssEmploymentChangeRequest lbusWssEmploymentChangeRequest = new busWssEmploymentChangeRequest { icdoWssEmploymentChangeRequest = new cdoWssEmploymentChangeRequest() };
                                    DataTable ldtbOrganization = Select("cdoOrganization.GetOrganizationFromOrgCode", new object[1] { lbusPsEmployment.icdoPsEmployment.org_code });
                                    if (ldtbOrganization != null)
                                        lbusPsEmployment.iintOrgId = Convert.ToInt32(ldtbOrganization.Rows[0][enmOrganization.org_id.ToString()]);
                                    busOrganization lobjOrganization = new busOrganization();
                                    lobjOrganization.FindOrganization(lbusPsEmployment.iintOrgId);

                                    //Load person 
                                    lbusPsEmployment.ibusPerson = new busPerson();
                                    lbusPsEmployment.ibusPerson = lbusPsEmployment.ibusPerson.LoadPersonBySsn(lbusPsEmployment.icdoPsEmployment.ssn);

                                    //asssign record number
                                    lbusWssPersonEmployment.icdoWssPersonEmployment.ps_empl_record_number = lbusPsEmployment.icdoPsEmployment.ps_empl_record_number;
                                    DataTable ldtbPersonEmployment = Select("cdoPersonEmployment.GetEmploymentDetail", new object[1] { lbusPsEmployment.icdoPsEmployment.ssn });
                                    Collection<busPersonEmployment> lclbPersonEmployment = GetCollection<busPersonEmployment>(ldtbPersonEmployment, "icdoPersonEmployment");


                                    if (lbusPsEmployment.icdoPsEmployment.empl_end_date == DateTime.MinValue &&
                                        (lbusPsEmployment.icdoPsEmployment.job_class_value.IsNull() || lbusPsEmployment.icdoPsEmployment.job_type_value.IsNull()
                                        || lbusPsEmployment.icdoPsEmployment.job_class_value.Trim().IsEmpty() || lbusPsEmployment.icdoPsEmployment.job_type_value.Trim().IsEmpty()))
                                    {
                                        utlError lobjError = null;
                                        lobjError = AddError(10221, "Job class/Job type is required.");
                                        lbusPsPerson.iarrErrors.Add(lobjError);
                                        iclbUnprocessedPsPerson.Add(lbusPsPerson);
                                        iclbPersonInformativeErrors.Add(lbusPsPerson);
                                        if (!lbusPsPerson.idictPsPersonError.ContainsKey(lbusPsPerson.icdoPsPerson.ps_person_id))
                                            lbusPsPerson.idictPsPersonError.Add(lbusPsPerson.icdoPsPerson.ps_person_id, lbusPsPerson.iarrErrors);
                                        continue;
                                    }
                                
                                    //As per mail from Maik dated 03/20/2014, Issue#4
                                    if ((lbusPsEmployment.ibusPerson != null) && (lbusPsEmployment.icdoPsEmployment.empl_end_date != DateTime.MinValue))
                                    {
                                        lbusPsEmployment.ibusPerson.LoadPersonEmployment();
                                        busPersonEmployment lbusPersonEmployment = lbusPsEmployment.ibusPerson.icolPersonEmployment.Where(o => o.icdoPersonEmployment.org_id == lbusPsEmployment.iintOrgId)
                                                                                    .OrderByDescending(o => o.icdoPersonEmployment.start_date).FirstOrDefault();
                                        if (lbusPersonEmployment.IsNull())
                                        {
                                            utlError lobjError = null;
                                            lobjError = AddError(4720, "Employment does not exist.");
                                            lbusPsPerson.iarrErrors.Add(lobjError);
                                            iclbUnprocessedPsPerson.Add(lbusPsPerson);
                                            iclbPersonInformativeErrors.Add(lbusPsPerson);
                                            if (!lbusPsPerson.idictPsPersonError.ContainsKey(lbusPsPerson.icdoPsPerson.ps_person_id))
                                                lbusPsPerson.idictPsPersonError.Add(lbusPsPerson.icdoPsPerson.ps_person_id, lbusPsPerson.iarrErrors);
                                            continue;
                                        }
                                    }


                                    #region Dual Employment
                                    if (lclbPersonEmployment.Count > 1)
                                    {
                                        //Dual Employment Scenario : 2 Sep
                                        busPersonEmployment lbusPersomEmp = lclbPersonEmployment.Where(lobj => lobj.icdoPersonEmployment.org_id == lbusPsEmployment.iintOrgId).FirstOrDefault();
                                        if (lbusPersomEmp != null)
                                        {
                                            lbusPersomEmp.LoadLatestPersonEmploymentDetail();
                                            if (lbusPsEmployment.icdoPsEmployment.ps_empl_record_number == (lbusPersomEmp.icdoPersonEmployment.ps_empl_record_number == null ? "0" : lbusPersomEmp.icdoPersonEmployment.ps_empl_record_number))
                                            {
                                                if (lbusPsEmployment.icdoPsEmployment.loa_date_of_return != DateTime.MinValue
                                                    && !(lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOA
                                                    || lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOAM
                                                    || lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusFMLA))
                                                {
                                                    iclbUnprocessedPsPerson.Add(lbusPsPerson);
                                                    if (lbusPsEmployment.GetEmploymentChangeRequestInReviewLOA())
                                                    {
                                                        utlError lobjError = null;
                                                        lobjError = AddError(10222, "LOA employment doesn't exist.");
                                                        lbusPsPerson.iarrErrors.Add(lobjError);
                                                        iclbPersonInformativeErrors.Add(lbusPsPerson);
                                                        if (!lbusPsPerson.idictPsPersonError.ContainsKey(lbusPsPerson.icdoPsPerson.ps_person_id))
                                                            lbusPsPerson.idictPsPersonError.Add(lbusPsPerson.icdoPsPerson.ps_person_id, lbusPsPerson.iarrErrors);
                                                    }
                                                    continue;
                                                }
                                                CreateEmploymentChangeRequest(lbusWssEmploymentChangeRequest, lbusPsEmployment, lbusPersomEmp);
                                                if (UpdateAddress(lbusPsPerson) == 0)
                                                    continue;
                                                if (lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.change_type_value.IsNull())
                                                {
                                                    if (lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.last_date_of_service == DateTime.MinValue)
                                                    {
                                                        //utlError lobjError = null;
                                                        //lobjError = AddError(10210, "Employment Record already exists.");
                                                        if (!iclbProcessedPsPerson.Contains(lbusPsPerson))
                                                            iclbProcessedPsPerson.Add(lbusPsPerson);
                                                        continue;
                                                    }
                                                }
                                                ValidatePSRecords(lbusWssEmploymentChangeRequest, lbusPsPerson, utlPageMode.New);
                                            }
                                            else //Same Organization Different Record number
                                            {
                                                if (lbusPsPerson.ibusPsAddress != null) //lbusPsEmployment.ibusPsPerson != null && lbusPsEmployment.ibusPsAddress 
                                                {
                                                    lbusWssMemberRecordRequest = new busWssMemberRecordRequest { icdoWssMemberRecordRequest = new cdoWssMemberRecordRequest() };
                                                    lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.org_id = lbusPsEmployment.iintOrgId;
                                                    CreateMemberRecordRequest(lbusWssMemberRecordRequest, lbusPsPerson);
                                                    lbusPsPerson.ibusPsAddress.SetWssPersonAddressInfo(lbusWssMemberRecordRequest);
                                                    if (lbusPsPerson.ibusPsAddress.iblnIsAddressNotValidated)
                                                    {
                                                        utlError lobjError = null;
                                                        lobjError = AddError(130, "Person Address is invalid.");
                                                        lbusPsPerson.iarrErrors.Add(lobjError);
                                                        iclbUnprocessedPsPerson.Add(lbusPsPerson);
                                                        iclbPersonInformativeErrors.Add(lbusPsPerson);
                                                    if (!lbusPsPerson.idictPsPersonError.ContainsKey(lbusPsPerson.icdoPsPerson.ps_person_id))
                                                            lbusPsPerson.idictPsPersonError.Add(lbusPsPerson.icdoPsPerson.ps_person_id, lbusPsPerson.iarrErrors);
                                                        continue;
                                                    }

                                                    lbusPsEmployment.SetWssPersonEmploymentInfo(lbusWssMemberRecordRequest, lobjOrganization);
														//25926	FILLER: Remove PS Error Report Error of "Employment exists, but Empl Record Number is Different"
                                                    //if (lbusPersomEmp.icdoPersonEmployment.ps_empl_record_number == null)
                                                    //{
                                                    //    if (lbusPsEmployment.icdoPsEmployment.empl_start_date == lbusPersomEmp.icdoPersonEmployment.start_date
                                                    //        && lbusPsEmployment.icdoPsEmployment.job_class_value == lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.job_class_value
                                                    //        && lbusPsEmployment.icdoPsEmployment.job_type_value == lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.type_value
                                                    //        && lbusPsEmployment.icdoPsEmployment.empl_status_value == lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value
                                                    //        )
                                                    //    {
                                                    //        utlError lobjError = null;
                                                    //        lobjError = AddError(10214, "Employment exists, but Empl Record Number is different.");
                                                    //        lbusPsPerson.iarrErrors.Add(lobjError);
                                                    //        iclbUnprocessedPsPerson.Add(lbusPsPerson);
                                                    //        iclbPersonInformativeErrors.Add(lbusPsPerson);
                                                    //        if (!lbusPsPerson.idictPsPersonError.ContainsKey(lbusPsPerson.icdoPsPerson.ps_person_id))
                                                    //            lbusPsPerson.idictPsPersonError.Add(lbusPsPerson.icdoPsPerson.ps_person_id, lbusPsPerson.iarrErrors);
                                                    //        continue;
                                                    //    }
                                                    //}
                                                    ValidatePSRecords(lbusWssMemberRecordRequest, lbusPsPerson, utlPageMode.New, iclbPersonInformativeErrors);
                                                }
                                                else if (lbusPsPerson.ibusPerson != null)
                                                {
                                                    lbusWssMemberRecordRequest = new busWssMemberRecordRequest { icdoWssMemberRecordRequest = new cdoWssMemberRecordRequest() };
                                                    lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.org_id = lbusPsEmployment.iintOrgId;
                                                    CreateMemberRecordRequest(lbusWssMemberRecordRequest, lbusPsPerson);
                                                    lbusPsPerson.ibusPerson.LoadPersonCurrentAddress();
                                                    if (lbusPsPerson.ibusPerson.ibusPersonCurrentAddress != null)
                                                    {
                                                        lbusPsPerson.SetWssPersonAddressInfo(lbusWssMemberRecordRequest);
                                                        lbusPsEmployment.SetWssPersonEmploymentInfo(lbusWssMemberRecordRequest, lobjOrganization);
                                                        if (lbusPersomEmp.icdoPersonEmployment.ps_empl_record_number == null)
                                                        {
															//25926	FILLER: Remove PS Error Report Error of "Employment exists, but Empl Record Number is Different"
                                                            //if (lbusPsEmployment.icdoPsEmployment.empl_start_date == lbusPersomEmp.icdoPersonEmployment.start_date)
                                                            //{
                                                            //    if (lbusPsEmployment.icdoPsEmployment.job_class_value == lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.job_class_value
                                                            //    && lbusPsEmployment.icdoPsEmployment.job_type_value == lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.type_value
                                                            //    && lbusPsEmployment.icdoPsEmployment.empl_status_value == lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value
                                                            //    )
                                                            //    {
                                                            //        utlError lobjError = null;
                                                            //        lobjError = AddError(10214, "Employment exists, but Empl Record Number is different.");
                                                            //        lbusPsPerson.iarrErrors.Add(lobjError);
                                                            //        iclbUnprocessedPsPerson.Add(lbusPsPerson);
                                                            //        iclbPersonInformativeErrors.Add(lbusPsPerson);
                                                            //        if (!lbusPsPerson.idictPsPersonError.ContainsKey(lbusPsPerson.icdoPsPerson.ps_person_id))
                                                            //            lbusPsPerson.idictPsPersonError.Add(lbusPsPerson.icdoPsPerson.ps_person_id, lbusPsPerson.iarrErrors);
                                                            //        continue;
                                                            //    }
                                                            //}
                                                            //else
                                                            //{
                                                            //    if( lbusPsEmployment.icdoPsEmployment.job_class_value == lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.job_class_value
                                                            //    && lbusPsEmployment.icdoPsEmployment.job_type_value == lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.type_value
                                                            //    && lbusPsEmployment.icdoPsEmployment.empl_status_value == lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value
                                                            //    )
                                                            //    {
                                                            //        utlError lobjError = null;
                                                            //        lobjError = AddError(10213, "Employment Record already exists with different date.");
                                                            //        lbusPsPerson.iarrErrors.Add(lobjError);
                                                            //        iclbUnprocessedPsPerson.Add(lbusPsPerson);
                                                            //        iclbPersonInformativeErrors.Add(lbusPsPerson);
                                                            //        if (!lbusPsPerson.idictPsPersonError.ContainsKey(lbusPsPerson.icdoPsPerson.ps_person_id))
                                                            //            lbusPsPerson.idictPsPersonError.Add(lbusPsPerson.icdoPsPerson.ps_person_id, lbusPsPerson.iarrErrors);
                                                            //        continue;
                                                            //    }
                                                            //}
                                                        }
                                                        ValidatePSRecords(lbusWssMemberRecordRequest, lbusPsPerson, utlPageMode.New, iclbPersonInformativeErrors);
                                                    }
                                                    else
                                                    {
                                                        utlError lobjError = null;
                                                        lobjError = AddError(10212, "Person does not have active address.");
                                                        lbusPsPerson.iarrErrors.Add(lobjError);
                                                        iclbUnprocessedPsPerson.Add(lbusPsPerson);
                                                        //iclbPersonInformativeErrors.Add(lbusPsPerson);
                                                        if (!lbusPsPerson.idictPsPersonError.ContainsKey(lbusPsPerson.icdoPsPerson.ps_person_id))
                                                            lbusPsPerson.idictPsPersonError.Add(lbusPsPerson.icdoPsPerson.ps_person_id, lbusPsPerson.iarrErrors);
                                                        continue;
                                                    }
                                                    lbusPsEmployment.SetWssPersonEmploymentInfo(lbusWssMemberRecordRequest, lobjOrganization);
                                                    ValidatePSRecords(lbusWssMemberRecordRequest, lbusPsPerson, utlPageMode.New, iclbPersonInformativeErrors);
                                                }
                                                else
                                                {
                                                    if (lbusPsEmployment.icdoPsEmployment.loa_date_of_return != DateTime.MinValue
                                                        && !(lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOA
                                                        || lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOAM
                                                        || lbusPersomEmp.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusFMLA))
                                                    {
                                                        iclbUnprocessedPsPerson.Add(lbusPsPerson);
                                                        if (lbusPsEmployment.GetEmploymentChangeRequestInReviewLOA())
                                                        {
                                                            utlError lobjError = null;
                                                            lobjError = AddError(10222, "LOA employment doesn't exist.");
                                                            lbusPsPerson.iarrErrors.Add(lobjError);
                                                            iclbPersonInformativeErrors.Add(lbusPsPerson);
                                                            if (!lbusPsPerson.idictPsPersonError.ContainsKey(lbusPsPerson.icdoPsPerson.ps_person_id))
                                                                lbusPsPerson.idictPsPersonError.Add(lbusPsPerson.icdoPsPerson.ps_person_id, lbusPsPerson.iarrErrors);
                                                        }
                                                        continue;
                                                    }
                                                    CreateEmploymentChangeRequest(lbusWssEmploymentChangeRequest, lbusPsEmployment, lbusPersomEmp);
                                                    if (UpdateAddress(lbusPsPerson) == 0)
                                                        continue;
                                                    ValidatePSRecords(lbusWssEmploymentChangeRequest, lbusPsPerson, utlPageMode.New);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            lbusWssMemberRecordRequest = new busWssMemberRecordRequest { icdoWssMemberRecordRequest = new cdoWssMemberRecordRequest() };
                                            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.org_id = lbusPsEmployment.iintOrgId;
                                            CreateMemberRecordRequest(lbusWssMemberRecordRequest, lbusPsPerson);
                                            lbusPsPerson.ibusPsAddress.SetWssPersonAddressInfo(lbusWssMemberRecordRequest);
                                            if (lbusPsPerson.ibusPsAddress.iblnIsAddressNotValidated)
                                            {
                                                utlError lobjError = null;
                                                lobjError = AddError(130, "Person Address is invalid.");
                                                lbusPsPerson.iarrErrors.Add(lobjError);
                                                iclbUnprocessedPsPerson.Add(lbusPsPerson);
                                                iclbPersonInformativeErrors.Add(lbusPsPerson);
                                            if (!lbusPsPerson.idictPsPersonError.ContainsKey(lbusPsPerson.icdoPsPerson.ps_person_id))
                                                    lbusPsPerson.idictPsPersonError.Add(lbusPsPerson.icdoPsPerson.ps_person_id, lbusPsPerson.iarrErrors);
                                                continue;
                                            }
                                            lbusPsEmployment.SetWssPersonEmploymentInfo(lbusWssMemberRecordRequest, lobjOrganization);
                                                ValidatePSRecords(lbusWssMemberRecordRequest, lbusPsPerson, utlPageMode.New, iclbPersonInformativeErrors);
                                        }
                                    }
                                    #endregion Dual Employment
                                    else if (lclbPersonEmployment.Count == 1)
                                    {

                                        foreach (busPersonEmployment lobjPersonEmployment in lclbPersonEmployment)
                                        {
                                            lobjPersonEmployment.LoadLatestPersonEmploymentDetail();
                                            if (lbusPsEmployment.iintOrgId != lobjPersonEmployment.icdoPersonEmployment.org_id)
                                            {
                                                lbusWssMemberRecordRequest = new busWssMemberRecordRequest { icdoWssMemberRecordRequest = new cdoWssMemberRecordRequest() };
                                                lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.org_id = lbusPsEmployment.iintOrgId;
                                                CreateMemberRecordRequest(lbusWssMemberRecordRequest, lbusPsPerson);
                                                lbusPsPerson.ibusPsAddress.SetWssPersonAddressInfo(lbusWssMemberRecordRequest);
                                                if (lbusPsPerson.ibusPsAddress.iblnIsAddressNotValidated)
                                                {
                                                    utlError lobjError = null;
                                                    lobjError = AddError(130, "Person Address is invalid.");
                                                    lbusPsPerson.iarrErrors.Add(lobjError);
                                                    iclbUnprocessedPsPerson.Add(lbusPsPerson);
                                                    iclbPersonInformativeErrors.Add(lbusPsPerson);
                                                     if (!lbusPsPerson.idictPsPersonError.ContainsKey(lbusPsPerson.icdoPsPerson.ps_person_id))
                                                        lbusPsPerson.idictPsPersonError.Add(lbusPsPerson.icdoPsPerson.ps_person_id, lbusPsPerson.iarrErrors);
                                                    continue;
                                                }
                                                lbusPsEmployment.SetWssPersonEmploymentInfo(lbusWssMemberRecordRequest, lobjOrganization);
                                                    ValidatePSRecords(lbusWssMemberRecordRequest, lbusPsPerson, utlPageMode.New, iclbPersonInformativeErrors);
                                            }
                                            else
                                            {
                                                //Same Organization Same Record Number
                                                if (lbusPsEmployment.icdoPsEmployment.ps_empl_record_number == (lobjPersonEmployment.icdoPersonEmployment.ps_empl_record_number == null ? "0" : lobjPersonEmployment.icdoPersonEmployment.ps_empl_record_number))
                                                {
                                                    if (lbusPsEmployment.icdoPsEmployment.loa_date_of_return != DateTime.MinValue
                                                        && !(lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOA
                                                        || lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOAM
                                                        || lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusFMLA))
                                                    {
                                                        iclbUnprocessedPsPerson.Add(lbusPsPerson);
                                                        if (lbusPsEmployment.GetEmploymentChangeRequestInReviewLOA())
                                                        {
                                                            utlError lobjError = null;
                                                            lobjError = AddError(10222, "LOA employment doesn't exist.");
                                                            lbusPsPerson.iarrErrors.Add(lobjError);

                                                            iclbPersonInformativeErrors.Add(lbusPsPerson);
                                                            if (!lbusPsPerson.idictPsPersonError.ContainsKey(lbusPsPerson.icdoPsPerson.ps_person_id))
                                                                lbusPsPerson.idictPsPersonError.Add(lbusPsPerson.icdoPsPerson.ps_person_id, lbusPsPerson.iarrErrors);
                                                        }
                                                        continue;
                                                    }
                                                    CreateEmploymentChangeRequest(lbusWssEmploymentChangeRequest, lbusPsEmployment, lobjPersonEmployment);
                                                    if (UpdateAddress(lbusPsPerson) == 0)
                                                        continue;
                                                    if (lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.change_type_value.IsNull())
                                                    {
                                                        if (lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.last_date_of_service == DateTime.MinValue)
                                                        {
                                                            //utlError lobjError = null;
                                                            //lobjError = AddError(10210, "Employment Record already exists.");
                                                            if (!iclbProcessedPsPerson.Contains(lbusPsPerson))
                                                                iclbProcessedPsPerson.Add(lbusPsPerson);
                                                            continue;
                                                        }
                                                    }
                                                    ValidatePSRecords(lbusWssEmploymentChangeRequest, lbusPsPerson, utlPageMode.New);
                                                }
                                                else //Same Organization Different Record Number 
                                                {
                                                    if (lbusPsPerson.ibusPsAddress != null)
                                                    {
                                                        lbusWssMemberRecordRequest = new busWssMemberRecordRequest { icdoWssMemberRecordRequest = new cdoWssMemberRecordRequest() };
                                                        lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.org_id = lbusPsEmployment.iintOrgId;
                                                        CreateMemberRecordRequest(lbusWssMemberRecordRequest, lbusPsPerson);
                                                        lbusPsPerson.ibusPsAddress.SetWssPersonAddressInfo(lbusWssMemberRecordRequest);
                                                        if (lbusPsPerson.ibusPsAddress.iblnIsAddressNotValidated)
                                                        {
                                                            utlError lobjError = null;
                                                            lobjError = AddError(130, "Person Address is invalid.");
                                                            lbusPsPerson.iarrErrors.Add(lobjError);
                                                            iclbUnprocessedPsPerson.Add(lbusPsPerson);
                                                            iclbPersonInformativeErrors.Add(lbusPsPerson);
                                                            if (!lbusPsPerson.idictPsPersonError.ContainsKey(lbusPsPerson.icdoPsPerson.ps_person_id))
                                                               lbusPsPerson.idictPsPersonError.Add(lbusPsPerson.icdoPsPerson.ps_person_id, lbusPsPerson.iarrErrors);
                                                            continue;
                                                        }
                                                        lbusPsEmployment.SetWssPersonEmploymentInfo(lbusWssMemberRecordRequest, lobjOrganization);
                                                        if (lobjPersonEmployment.icdoPersonEmployment.ps_empl_record_number == null)
                                                        {
															//25926	FILLER: Remove PS Error Report Error of "Employment exists, but Empl Record Number is Different"
                                                            //if (lbusPsEmployment.icdoPsEmployment.empl_start_date == lobjPersonEmployment.icdoPersonEmployment.start_date)
                                                            //{
                                                            //    if (lbusPsEmployment.icdoPsEmployment.job_class_value == lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.job_class_value
                                                            //    && lbusPsEmployment.icdoPsEmployment.job_type_value == lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.type_value
                                                            //    && lbusPsEmployment.icdoPsEmployment.empl_status_value == lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value
                                                            //    )
                                                            //    {
                                                            //        utlError lobjError = null;
                                                            //        lobjError = AddError(10214, "Employment exists, but Empl Record Number is different.");
                                                            //        lbusPsPerson.iarrErrors.Add(lobjError);
                                                            //        iclbUnprocessedPsPerson.Add(lbusPsPerson);
                                                            //        iclbPersonInformativeErrors.Add(lbusPsPerson);
                                                            //        if (!lbusPsPerson.idictPsPersonError.ContainsKey(lbusPsPerson.icdoPsPerson.ps_person_id))
                                                            //            lbusPsPerson.idictPsPersonError.Add(lbusPsPerson.icdoPsPerson.ps_person_id, lbusPsPerson.iarrErrors);
                                                            //        continue;
                                                            //    }
                                                            //}
                                                            //else
                                                            //{
                                                            //    if (lbusPsEmployment.icdoPsEmployment.job_class_value == lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.job_class_value
                                                            //    && lbusPsEmployment.icdoPsEmployment.job_type_value == lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.type_value
                                                            //    && lbusPsEmployment.icdoPsEmployment.empl_status_value == lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value
                                                            //    )
                                                            //    {
                                                            //        utlError lobjError = null;
                                                            //        lobjError = AddError(10213, "Employment Record already exists with different date.");
                                                            //        lbusPsPerson.iarrErrors.Add(lobjError);
                                                            //        iclbUnprocessedPsPerson.Add(lbusPsPerson);
                                                            //        iclbPersonInformativeErrors.Add(lbusPsPerson);
                                                            //        if (!lbusPsPerson.idictPsPersonError.ContainsKey(lbusPsPerson.icdoPsPerson.ps_person_id))
                                                            //            lbusPsPerson.idictPsPersonError.Add(lbusPsPerson.icdoPsPerson.ps_person_id, lbusPsPerson.iarrErrors);
                                                            //        continue;
                                                            //    }
                                                            //}
                                                        }
                                                        ValidatePSRecords(lbusWssMemberRecordRequest, lbusPsPerson, utlPageMode.New, iclbPersonInformativeErrors);
                                                    }
                                                    else if (lbusPsPerson.ibusPerson != null)
                                                    {
                                                        lbusWssMemberRecordRequest = new busWssMemberRecordRequest { icdoWssMemberRecordRequest = new cdoWssMemberRecordRequest() };
                                                        lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.org_id = lbusPsEmployment.iintOrgId;
                                                        CreateMemberRecordRequest(lbusWssMemberRecordRequest, lbusPsPerson);
                                                        lbusPsPerson.ibusPerson.LoadPersonCurrentAddress();
                                                        if (lbusPsPerson.ibusPerson.ibusPersonCurrentAddress != null)
                                                        {
                                                            lbusPsPerson.SetWssPersonAddressInfo(lbusWssMemberRecordRequest);
                                                            lbusPsEmployment.SetWssPersonEmploymentInfo(lbusWssMemberRecordRequest, lobjOrganization);
                                                            if (lobjPersonEmployment.icdoPersonEmployment.ps_empl_record_number == null)
                                                            {
																//25926	FILLER: Remove PS Error Report Error of "Employment exists, but Empl Record Number is Different"
                                                                //if (lbusPsEmployment.icdoPsEmployment.empl_start_date == lobjPersonEmployment.icdoPersonEmployment.start_date)
                                                                //{
                                                                //    if (lbusPsEmployment.icdoPsEmployment.job_class_value == lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.job_class_value
                                                                //    && lbusPsEmployment.icdoPsEmployment.job_type_value == lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.type_value
                                                                //    && lbusPsEmployment.icdoPsEmployment.empl_status_value == lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value
                                                                //    )
                                                                //    {
                                                                //        utlError lobjError = null;
                                                                //        lobjError = AddError(10214, "Employment exists, but Empl Record Number is different.");
                                                                //        lbusPsPerson.iarrErrors.Add(lobjError);
                                                                //        iclbUnprocessedPsPerson.Add(lbusPsPerson);
                                                                //        iclbPersonInformativeErrors.Add(lbusPsPerson);
                                                                //        if (!lbusPsPerson.idictPsPersonError.ContainsKey(lbusPsPerson.icdoPsPerson.ps_person_id))
                                                                //            lbusPsPerson.idictPsPersonError.Add(lbusPsPerson.icdoPsPerson.ps_person_id, lbusPsPerson.iarrErrors);
                                                                //        continue;
                                                                //    }
                                                                //}
                                                                //else
                                                                //{
                                                                //    if(lbusPsEmployment.icdoPsEmployment.job_class_value == lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.job_class_value
                                                                //    && lbusPsEmployment.icdoPsEmployment.job_type_value == lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.type_value
                                                                //    && lbusPsEmployment.icdoPsEmployment.empl_status_value == lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value
                                                                //    )
                                                                //    {
                                                                //        utlError lobjError = null;
                                                                //        lobjError = AddError(10213, "Employment Record already exists with different date.");
                                                                //        lbusPsPerson.iarrErrors.Add(lobjError);
                                                                //        iclbUnprocessedPsPerson.Add(lbusPsPerson);
                                                                //        iclbPersonInformativeErrors.Add(lbusPsPerson);
                                                                //        if (!lbusPsPerson.idictPsPersonError.ContainsKey(lbusPsPerson.icdoPsPerson.ps_person_id))
                                                                //            lbusPsPerson.idictPsPersonError.Add(lbusPsPerson.icdoPsPerson.ps_person_id, lbusPsPerson.iarrErrors);
                                                                //        continue;
                                                                //    }
                                                                //}
                                                            }
                                                            ValidatePSRecords(lbusWssMemberRecordRequest, lbusPsPerson, utlPageMode.New, iclbPersonInformativeErrors);
                                                        }
                                                        else
                                                        {
                                                            utlError lobjError = null;
                                                            lobjError = AddError(10212, "Person does not have active address.");
                                                            lbusPsPerson.iarrErrors.Add(lobjError);
                                                            iclbUnprocessedPsPerson.Add(lbusPsPerson);
                                                            //iclbPersonInformativeErrors.Add(lbusPsPerson);
                                                            if (!lbusPsPerson.idictPsPersonError.ContainsKey(lbusPsPerson.icdoPsPerson.ps_person_id))
                                                                lbusPsPerson.idictPsPersonError.Add(lbusPsPerson.icdoPsPerson.ps_person_id, lbusPsPerson.iarrErrors);
                                                            continue;
                                                        }
                                                        lbusPsEmployment.SetWssPersonEmploymentInfo(lbusWssMemberRecordRequest, lobjOrganization);
                                                        ValidatePSRecords(lbusWssMemberRecordRequest, lbusPsPerson, utlPageMode.New, iclbPersonInformativeErrors);
                                                    }
                                                    else
                                                    {
                                                        if (lbusPsEmployment.icdoPsEmployment.loa_date_of_return != DateTime.MinValue
                                                            && !(lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOA
                                                            || lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOAM
                                                            || lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusFMLA))
                                                        {
                                                            iclbUnprocessedPsPerson.Add(lbusPsPerson);
                                                            if (lbusPsEmployment.GetEmploymentChangeRequestInReviewLOA())
                                                            {
                                                                utlError lobjError = null;
                                                                lobjError = AddError(10222, "LOA employment doesn't exist.");
                                                                lbusPsPerson.iarrErrors.Add(lobjError);

                                                                iclbPersonInformativeErrors.Add(lbusPsPerson);
                                                                if (!lbusPsPerson.idictPsPersonError.ContainsKey(lbusPsPerson.icdoPsPerson.ps_person_id))
                                                                    lbusPsPerson.idictPsPersonError.Add(lbusPsPerson.icdoPsPerson.ps_person_id, lbusPsPerson.iarrErrors);
                                                            }
                                                            continue;
                                                        }
                                                        CreateEmploymentChangeRequest(lbusWssEmploymentChangeRequest, lbusPsEmployment, lobjPersonEmployment);
                                                        if (UpdateAddress(lbusPsPerson) == 0)
                                                            continue;
                                                        if (lobjPersonEmployment.icdoPersonEmployment.ps_empl_record_number == null)
                                                        {
															//25926	FILLER: Remove PS Error Report Error of "Employment exists, but Empl Record Number is Different"
                                                            //if (lbusPsEmployment.icdoPsEmployment.empl_start_date == lobjPersonEmployment.icdoPersonEmployment.start_date)
                                                            //{
                                                            //    if (lbusPsEmployment.icdoPsEmployment.job_class_value == lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.job_class_value
                                                            //    && lbusPsEmployment.icdoPsEmployment.job_type_value == lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.type_value
                                                            //    && lbusPsEmployment.icdoPsEmployment.empl_status_value == lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value
                                                            //    )
                                                            //    {
                                                            //        utlError lobjError = null;
                                                            //        lobjError = AddError(10214, "Employment exists, but Empl Record Number is different.");
                                                            //        lbusPsPerson.iarrErrors.Add(lobjError);
                                                            //        iclbUnprocessedPsPerson.Add(lbusPsPerson);
                                                            //        iclbPersonInformativeErrors.Add(lbusPsPerson);
                                                            //        if (!lbusPsPerson.idictPsPersonError.ContainsKey(lbusPsPerson.icdoPsPerson.ps_person_id))
                                                            //            lbusPsPerson.idictPsPersonError.Add(lbusPsPerson.icdoPsPerson.ps_person_id, lbusPsPerson.iarrErrors);
                                                            //        continue;
                                                            //    }
                                                            //}
                                                            //else
                                                            //{
                                                            //    if(lbusPsEmployment.icdoPsEmployment.job_class_value == lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.job_class_value
                                                            //        && lbusPsEmployment.icdoPsEmployment.job_type_value == lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.type_value
                                                            //        && lbusPsEmployment.icdoPsEmployment.empl_status_value == lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value
                                                            //        )
                                                            //        {
                                                            //            utlError lobjError = null;
                                                            //            lobjError = AddError(10213, "Employment Record already exists with different date.");
                                                            //            lbusPsPerson.iarrErrors.Add(lobjError);
                                                            //            iclbUnprocessedPsPerson.Add(lbusPsPerson);
                                                            //            iclbPersonInformativeErrors.Add(lbusPsPerson);
                                                            //            if (!lbusPsPerson.idictPsPersonError.ContainsKey(lbusPsPerson.icdoPsPerson.ps_person_id))
                                                            //                lbusPsPerson.idictPsPersonError.Add(lbusPsPerson.icdoPsPerson.ps_person_id, lbusPsPerson.iarrErrors);
                                                            //            continue;
                                                            //        }
                                                            //}
                                                        }
                                                        ValidatePSRecords(lbusWssEmploymentChangeRequest, lbusPsPerson, utlPageMode.New);

                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (lbusPsEmployment.icdoPsEmployment.empl_end_date != DateTime.MinValue)
                                        {
                                            utlError lobjError = null;
                                            lobjError = AddError(10211, "Terminated Employment already exists.");
                                            lbusPsEmployment.iarrErrors.Add(lobjError);
                                            iclbUnprocessedPsPerson.Add(lbusPsEmployment);
                                            iclbPersonInformativeErrors.Add(lbusPsEmployment);
                                            if (!lbusPsEmployment.idictPsEmploymentError.ContainsKey(lbusPsEmployment.icdoPsEmployment.ps_employment_id))
                                                lbusPsEmployment.idictPsEmploymentError.Add(lbusPsEmployment.icdoPsEmployment.ps_employment_id, lbusPsEmployment.iarrErrors);
                                            continue;
                                        }
                                        lbusWssMemberRecordRequest = new busWssMemberRecordRequest { icdoWssMemberRecordRequest = new cdoWssMemberRecordRequest() }; // Added on 15 October
                                        CreateMemberRecordRequest(lbusWssMemberRecordRequest, lbusPsPerson);
                                        lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.org_id = lbusPsEmployment.iintOrgId;
                                        busPsAddress lobjPsAddress = LoadPSPersonAddressBySSN(lbusPsPerson.icdoPsPerson.ssn);
                                        if (lobjPsAddress != null && lobjPsAddress.icdoPsAddress.processed_flag == busConstant.Flag_No)
                                        {
                                            lobjPsAddress.SetWssPersonAddressInfo(lbusWssMemberRecordRequest);
                                            if (lobjPsAddress.iblnIsAddressNotValidated)
                                            {
                                                utlError lobjError = null;
                                                lobjError = AddError(130, "Person Address is invalid.");
                                                lbusPsPerson.iarrErrors.Add(lobjError);
                                                iclbUnprocessedPsPerson.Add(lbusPsPerson);
                                                iclbPersonInformativeErrors.Add(lbusPsPerson);
                                                if (!lbusPsPerson.idictPsPersonError.ContainsKey(lbusPsPerson.icdoPsPerson.ps_person_id))
                                                    lbusPsPerson.idictPsPersonError.Add(lbusPsPerson.icdoPsPerson.ps_person_id, lbusPsPerson.iarrErrors);
                                                continue;
                                            }
                                            lbusPsEmployment.SetWssPersonEmploymentInfo(lbusWssMemberRecordRequest, lobjOrganization);
                                            ValidatePSRecords(lbusWssMemberRecordRequest, lbusPsPerson, utlPageMode.New, iclbPersonInformativeErrors);
                                        }
                                        else
                                        {
                                            utlError lobjError = null;
                                            lobjError = AddError(10207, "PS Address Record does not exist.");
                                            lbusPsPerson.iarrErrors.Add(lobjError);
                                            iclbUnprocessedPsPerson.Add(lbusPsPerson);
                                            if (!lbusPsPerson.idictPsPersonError.ContainsKey(lbusPsPerson.icdoPsPerson.ps_person_id))
                                                lbusPsPerson.idictPsPersonError.Add(lbusPsPerson.icdoPsPerson.ps_person_id, lbusPsPerson.iarrErrors);
                                            continue;
                                        }
                                }
                        #endregion
                            }
                        }
                    }
                    else
                    {
                        utlError lobjError = null;
                        lobjError = AddError(10207, "PS Address Record does not exist.");
                        lbusPsPerson.iarrErrors.Add(lobjError);
                        iclbUnprocessedPsPerson.Add(lbusPsPerson);
                        if (!lbusPsPerson.idictPsPersonError.ContainsKey(lbusPsPerson.icdoPsPerson.ps_person_id))
                            lbusPsPerson.idictPsPersonError.Add(lbusPsPerson.icdoPsPerson.ps_person_id, lbusPsPerson.iarrErrors);
                        continue;
                    }
                }

                catch (Exception e)
                {
                    DBFunction.StoreProcessLog(100, "An error occured while updating ps person  Id: " +
                                                               lbusPsPerson.icdoPsPerson.ps_person_id + " Index : " + iclbPsPersonPerson.IndexOf(lbusPsPerson) + " Error Message : " + e,
                                                               "ERR", "Error in peoplesoft file", istrUserId, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                }
            }
        }

        private int UpdateAddress(busPsPerson abusPsPerson)
        {
            if (abusPsPerson.ibusPsAddress.iblnAddressChanged || abusPsPerson.ibusPsAddress.iblnNewAndFirstAddress)
            {
                busPersonAddress lbusPersonAddress = new busPersonAddress { icdoPersonAddress = new cdoPersonAddress() };
                lbusPersonAddress.icdoPersonAddress.ienuObjectState = ObjectState.Insert;
                lbusPersonAddress.icdoPersonAddress.addr_line_1 = abusPsPerson.ibusPsAddress.icdoPsAddress.addr_line_1;
                lbusPersonAddress.icdoPersonAddress.addr_line_2 = abusPsPerson.ibusPsAddress.icdoPsAddress.addr_line_2;
                lbusPersonAddress.icdoPersonAddress.addr_city = abusPsPerson.ibusPsAddress.icdoPsAddress.addr_city;
                lbusPersonAddress.icdoPersonAddress.addr_zip_4_code = abusPsPerson.ibusPsAddress.icdoPsAddress.addr_zip_4_code;
                lbusPersonAddress.icdoPersonAddress.addr_zip_code = abusPsPerson.ibusPsAddress.icdoPsAddress.addr_zip_code;
                lbusPersonAddress.icdoPersonAddress.addr_country_value = busGlobalFunctions.GetCodeValueDetailsfromData1(151, abusPsPerson.ibusPsAddress.icdoPsAddress.addr_country_value, iobjPassInfo);
                if (abusPsPerson.ibusPsAddress.icdoPsAddress.addr_start_date <= DateTime.Now)
                    lbusPersonAddress.icdoPersonAddress.start_date = DateTime.Now;
                else
                    lbusPersonAddress.icdoPersonAddress.start_date = abusPsPerson.ibusPsAddress.icdoPsAddress.addr_start_date;
                lbusPersonAddress.icdoPersonAddress.addr_state_value = abusPsPerson.ibusPsAddress.icdoPsAddress.addr_state_value;
                lbusPersonAddress.icdoPersonAddress.address_type_value = busConstant.AddressTypePermanent;
                lbusPersonAddress.icdoPersonAddress.person_id = abusPsPerson.ibusPerson.icdoPerson.person_id;
                lbusPersonAddress.icdoPersonAddress.peoplesoft_flag = busConstant.Flag_Yes;

                lbusPersonAddress.icdoPersonAddress.addr_state_value = abusPsPerson.ibusPsAddress.icdoPsAddress.addr_state_value;
                if (abusPsPerson.ibusPsAddress.iblnAddressChanged)
                {
                    if (abusPsPerson.ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.start_date.Date == lbusPersonAddress.icdoPersonAddress.start_date.Date)
                        abusPsPerson.ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.end_date = lbusPersonAddress.icdoPersonAddress.start_date;
                    else
                        abusPsPerson.ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.end_date = lbusPersonAddress.icdoPersonAddress.start_date.AddDays(-1);
                    abusPsPerson.ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.Update();
                }
                lbusPersonAddress.LoadOtherAddressses();
                lbusPersonAddress.LoadPerson();
                lbusPersonAddress.BeforeValidate(utlPageMode.New);
                lbusPersonAddress.ValidateHardErrors(utlPageMode.New);
                if (lbusPersonAddress.iarrErrors.Count > 0)
                {
                    abusPsPerson.ibusPsAddress.iblnAddressChanged = false;
                    iclbUnprocessedPsPerson.Add(abusPsPerson);
                    if (!abusPsPerson.idictPsPersonError.ContainsKey(abusPsPerson.icdoPsPerson.ps_person_id))
                        abusPsPerson.idictPsPersonError.Add(abusPsPerson.icdoPsPerson.ps_person_id, lbusPersonAddress.iarrErrors);
                    return 0;
                }
                else
                {
                    lbusPersonAddress.BeforePersistChanges();
                    lbusPersonAddress.PersistChanges();
                    lbusPersonAddress.AfterPersistChanges();
                }
            }
            return 1;
        }

        //17 October 
        private void SetWssPersonAddressInfo(busWssMemberRecordRequest lbusWssMemberRecordRequest)
        {
            lbusWssMemberRecordRequest.icdoWssPersonAddress = new cdoWssPersonAddress();
            lbusWssMemberRecordRequest.icdoWssPersonAddress.addr_line_1 = ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_line_1;
            lbusWssMemberRecordRequest.icdoWssPersonAddress.addr_line_2 = ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_line_2;
            lbusWssMemberRecordRequest.icdoWssPersonAddress.addr_city = ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_city;
            lbusWssMemberRecordRequest.icdoWssPersonAddress.addr_state_value = ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_state_value;
            lbusWssMemberRecordRequest.icdoWssPersonAddress.addr_country_value = ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_country_value;
            lbusWssMemberRecordRequest.icdoWssPersonAddress.addr_zip_code = ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_zip_code;
            lbusWssMemberRecordRequest.icdoWssPersonAddress.addr_zip_4_code = ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_zip_4_code;
            lbusWssMemberRecordRequest.icdoWssPersonAddress.addr_effective_date = ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.start_date;
            //if (iblnAddressChanged) // Check if Address Updated in this batch 
            //lbusWssMemberRecordRequest.icdoWssPersonAddress.address_updated_in_ps_batch = busConstant.Flag_Yes;
        }

        

       

        /// <summary>
        /// Member record request object properties are assigned 
        /// </summary>
        /// <param name="lbusWssMemberRecordRequest"></param>
        /// <param name="lbusPsPerson"></param>


        private void CreateEmploymentChangeRequest(busWssEmploymentChangeRequest abusWssEmploymentChangeRequest, busPsEmployment lbusPsEmployment, busPersonEmployment lobjPersonEmployment)
        {

            lobjPersonEmployment.LoadLatestPersonEmploymentDetail();
            abusWssEmploymentChangeRequest.ibusPersonEmployment = lobjPersonEmployment;
            abusWssEmploymentChangeRequest.ibusPersonEmploymentDetail = lobjPersonEmployment.ibusLatestEmploymentDetail;
            abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.ienuObjectState = ObjectState.Insert;
            abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.ps_empl_record_number = lbusPsEmployment.icdoPsEmployment.ps_empl_record_number;
            abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.peoplesoft_id = lbusPsEmployment.icdoPsEmployment.peoplesoft_id;
            //Need to confirm to which filed we have to bind in sgt_wss_employment_change_request 
            if (lbusPsEmployment.icdoPsEmployment.job_class_value == busConstant.PersonJobClassStateElectedOfficial)
                abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.term_begin_date = lbusPsEmployment.icdoPsEmployment.empl_start_date;
            abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.person_id = lbusPsEmployment.ibusPerson.icdoPerson.person_id;
            abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.org_id = lobjPersonEmployment.icdoPersonEmployment.org_id;

            //*****************Need to confirm with Maik on what basis we shall assign the change_type_value 
            //*****************also need the confirmation to add job_class_change_effective_date and employment_type_change_effective_date

            if (lbusPsEmployment.icdoPsEmployment.empl_end_date == DateTime.MinValue)
            {
                //if (lbusPsEmployment.icdoPsEmployment.job_class_value.IsNotNull() && lbusPsEmployment.icdoPsEmployment.term_begin_date != DateTime.MinValue && lbusPsEmployment.icdoPsEmployment.job_class_change_effective_date != DateTime.MinValue)
                if (lbusPsEmployment.icdoPsEmployment.job_class_value != null && lbusPsEmployment.icdoPsEmployment.job_class_value.Trim().IsNotNullOrEmpty() && lbusPsEmployment.icdoPsEmployment.job_class_value != lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.job_class_value
                    //&& lbusPsEmployment.icdoPsEmployment.term_begin_date != DateTime.MinValue 
                   && lbusPsEmployment.icdoPsEmployment.empl_start_date != DateTime.MinValue)
                {
                    abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.change_type_value = busConstant.EmploymentChangeRequestChangeTypeClassification;
                    abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.job_class_change_effective_date = lbusPsEmployment.icdoPsEmployment.empl_start_date;
                    abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.employment_type_change_effective_date = lbusPsEmployment.icdoPsEmployment.empl_start_date;
                    iblnEmploymentChangeRequestChangeTypeClassification = true;
                }
                //else if (lbusPsEmployment.icdoPsEmployment.job_type_value.IsNotNull() && lbusPsEmployment.icdoPsEmployment.empl_status_value.IsNotNull() && lbusPsEmployment.icdoPsEmployment.employment_type_change_effective_date != DateTime.MinValue)
                if (((lbusPsEmployment.icdoPsEmployment.job_type_value != null && lbusPsEmployment.icdoPsEmployment.job_type_value.Trim().IsNotNullOrEmpty() && lbusPsEmployment.icdoPsEmployment.job_type_value != lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.type_value)
                     && (lbusPsEmployment.icdoPsEmployment.empl_status_value != null && lbusPsEmployment.icdoPsEmployment.empl_status_value.Trim().IsNotNullOrEmpty() && lbusPsEmployment.icdoPsEmployment.empl_start_date != DateTime.MinValue))
                    || (lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.hourly_value != lbusPsEmployment.icdoPsEmployment.hourly_value
                    || lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value != lbusPsEmployment.icdoPsEmployment.empl_status_value))
                {
                    abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.change_type_value = busConstant.EmploymentChangeRequestChangeTypeEmployment;
                    abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.employment_type_change_effective_date = lbusPsEmployment.icdoPsEmployment.empl_start_date;
                    iblnEmploymentChangeRequestChangeTypeEmployment = true;
                }
                if (lbusPsEmployment.icdoPsEmployment.loa_start_date != DateTime.MinValue && lbusPsEmployment.icdoPsEmployment.empl_status_value == busConstant.EmploymentStatusLOAM
                    || (lbusPsEmployment.icdoPsEmployment.loa_date_of_return != DateTime.MinValue && lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOAM))
                {
                    abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.change_type_value = busConstant.EmploymentChangeRequestChangeTypeLOAM;
                    iblnEmploymentChangeRequestChangeTypeLOAM = true;
                }
                if ((lbusPsEmployment.icdoPsEmployment.loa_start_date != DateTime.MinValue && lbusPsEmployment.icdoPsEmployment.empl_status_value == busConstant.EmploymentStatusLOA)
                    || (lbusPsEmployment.icdoPsEmployment.loa_date_of_return != DateTime.MinValue && lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOA))
                {
                    abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.change_type_value = busConstant.EmploymentChangeRequestChangeTypeLOA;
                    iblnEmploymentChangeRequestChangeTypeLOA = true;
                }
                if ((lbusPsEmployment.icdoPsEmployment.loa_start_date != DateTime.MinValue && lbusPsEmployment.icdoPsEmployment.empl_status_value == busConstant.EmploymentStatusFMLA)
                    || (lbusPsEmployment.icdoPsEmployment.loa_date_of_return != DateTime.MinValue && lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusFMLA))
                {
                    abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.change_type_value = busConstant.EmploymentStatusFMLA;
                    iblnEmploymentChangeRequestChangeTypeFMLA = true;
                }
            }

            //if (abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.change_type_value != busConstant.EmploymentChangeRequestChangeTypeEmployment)
            //{
            //    abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.job_class_change_effective_date = lbusPsEmployment.icdoPsEmployment.empl_start_date;
            //    abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.employment_type_change_effective_date = lbusPsEmployment.icdoPsEmployment.empl_start_date;
            //}
            //Confirm with the Vasu/Maik Again
            //lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.employment_type_change_effective_date = lbusPsEmployment.icdoPsEmployment.employment_type_change_effective_date;
            //lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.job_class_change_effective_date = lbusPsEmployment.icdoPsEmployment.job_class_change_effective_date;

            abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.hourly_value = lbusPsEmployment.icdoPsEmployment.hourly_value;
            abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.type_value = lbusPsEmployment.icdoPsEmployment.job_type_value;
            abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.job_class_value = lbusPsEmployment.icdoPsEmployment.job_class_value;
            abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.official_list_value = lbusPsEmployment.icdoPsEmployment.official_list_value;
            abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.employment_end_date = lbusPsEmployment.icdoPsEmployment.empl_end_date;
            abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.last_date_of_service = lbusPsEmployment.icdoPsEmployment.empl_end_date;
            abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.employment_status_value = lbusPsEmployment.icdoPsEmployment.empl_status_value;
            abusWssEmploymentChangeRequest.istrIsTermsAndConditionsAgreed = busConstant.Flag_Yes;

            //*********************************************************************************************************************************************************************//

            abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.loa_start_date = lbusPsEmployment.icdoPsEmployment.loa_start_date;
            abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.date_of_return = lbusPsEmployment.icdoPsEmployment.loa_date_of_return;
            abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.date_of_last_regular_paycheck = lbusPsEmployment.icdoPsEmployment.date_of_last_regular_paycheck;
            abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.last_month_on_employer_billing = lbusPsEmployment.icdoPsEmployment.last_month_on_employer_billing;
            abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.last_retirement_transmittal_of_deduction = lbusPsEmployment.icdoPsEmployment.last_retirement_transmittal_of_deduction;
            if (lbusPsEmployment.icdoPsEmployment.job_class_value == busConstant.PersonJobClassStateElectedOfficial)
                abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.term_begin_date = lbusPsEmployment.icdoPsEmployment.term_begin_date; //As Discussed with Karthik.
            abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.created_by = "PERSLinkBatch";
            abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.created_date = DateTime.Now;
            abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.modified_by = "PERSLinkBatch";
            abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.modified_date = DateTime.Now;
            abusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.status_value = busConstant.StatusReview;
        }


        private void CreateMemberRecordRequest(busWssMemberRecordRequest lbusWssMemberRecordRequest, busPsPerson lbusPsPerson)
        {
            if (lbusPsPerson.ibusPerson != null)
            {
                if (lbusPsPerson.ibusPerson.icdoPerson.cell_phone_no != lbusPsPerson.icdoPsPerson.cell_phone_no || lbusPsPerson.ibusPerson.icdoPerson.work_phone_ext != lbusPsPerson.icdoPsPerson.work_phone_ext
                    || lbusPsPerson.ibusPerson.icdoPerson.work_phone_no != lbusPsPerson.icdoPsPerson.work_phone_no || lbusPsPerson.ibusPerson.icdoPerson.home_phone_no != lbusPsPerson.icdoPsPerson.home_phone_no)
                        iblnPhoneNumberChanged = true;
            }
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.name_prefix_value = lbusPsPerson.icdoPsPerson.name_prefix_value;
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.first_name = lbusPsPerson.icdoPsPerson.first_name;
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.middle_name = lbusPsPerson.icdoPsPerson.middle_name;
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.last_name = lbusPsPerson.icdoPsPerson.last_name;
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.name_suffix_value = lbusPsPerson.icdoPsPerson.name_suffix_value;
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.date_of_birth = lbusPsPerson.icdoPsPerson.date_of_birth;
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.gender_value = lbusPsPerson.icdoPsPerson.gender_value;
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.ssn = lbusPsPerson.icdoPsPerson.ssn;
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.marital_status_value = lbusPsPerson.icdoPsPerson.marital_status_value;
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.marital_status_date = lbusPsPerson.icdoPsPerson.ms_change_date; //Need to refresh cdo 
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.work_phone_no = lbusPsPerson.icdoPsPerson.work_phone_no;
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.work_phone_ext = lbusPsPerson.icdoPsPerson.work_phone_ext;//Need to  refresh cdo
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.cell_phone_no = lbusPsPerson.icdoPsPerson.cell_phone_no;
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.home_phone_no = lbusPsPerson.icdoPsPerson.home_phone_no;
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.peoplesoft_id = Convert.ToInt32(lbusPsPerson.icdoPsPerson.peoplesoft_id); //As per mail from Maik dated 03/20/2014, Issue#3
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.status_value = busConstant.StatusReview;
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.created_by = "PERSLinkBatch";
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.created_date = DateTime.Now;
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.modified_by = "PERSLinkBatch";
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.modified_date = DateTime.Now;
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.ps_initiated_flag = busConstant.Flag_Yes;
            lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.status_value = busConstant.EmploymentChangeRequestStatusPendingAutoPosting;
                //Collection Always as it may contain multiple entries 
        }
        private busPsPerson LoadPSPersonBySSN(string astrssn)
        {
            Collection<busPsPerson> lclbPsPerson = new Collection<busPsPerson>();
            DataTable ldtbPsPerson = DBFunction.DBSelect("cdoPsPerson.GetPsPersonBySSN", new string[1] { astrssn },
                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            if (ldtbPsPerson.Rows.Count > 0)
            {
                lclbPsPerson = GetCollection<busPsPerson>(ldtbPsPerson, "icdoPsPerson");
                return lclbPsPerson[0];
            }
            return null;
        }
        public busPsAddress LoadPSPersonAddressBySSN(string astrssn)
        {
            Collection<busPsAddress> lclbPsPerson = new Collection<busPsAddress>();
            DataTable ldtbPsAddress = DBFunction.DBSelect("cdoPsAddress.GetPsAddressBySSN", new string[1] { astrssn },
                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            if (ldtbPsAddress.Rows.Count > 0)
            {
                lclbPsPerson = GetCollection<busPsAddress>(ldtbPsAddress, "icdoPsAddress");
                return lclbPsPerson[0];
            }
            return null;

        }
        public Collection<busPsEmployment> LoadPsEmploymentBySSN(string astrssn)
        {
            Collection<busPsEmployment> lclbPsEmployment = new Collection<busPsEmployment>();
            DataTable ldtbPsEmployment = DBFunction.DBSelect("cdoPsEmployment.GetPsEmploymentBySSN", new string[1] { astrssn },
                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            lclbPsEmployment = GetCollection<busPsEmployment>(ldtbPsEmployment, "icdoPsEmployment");
            return lclbPsEmployment;
        }
        /// <summary>
        /// Vaidate Member record Request from PS  
        /// </summary>
        /// <param name="aobjWssMemberRecordRequest"></param>
        /// <param name="aobjPsPerson"></param>
        /// <param name="mode"></param>
        public void ValidatePSRecords(busWssMemberRecordRequest aobjWssMemberRecordRequest, busPsPerson aobjPsPerson, utlPageMode mode, Collection<busPsPerson> iclbPersonInformativeErrors)
        {
            aobjWssMemberRecordRequest.iblnIsFromPS = true;
            
            aobjWssMemberRecordRequest.BeforeValidate(mode);
            aobjWssMemberRecordRequest.ValidateHardErrors(mode);
            if (aobjWssMemberRecordRequest.iarrErrors.Count == 1 && aobjWssMemberRecordRequest.iblnIsMemberContributing)
            {
                if (!aobjPsPerson.idictPsPersonError.ContainsKey(aobjPsPerson.icdoPsPerson.ps_person_id))
                    aobjPsPerson.idictPsPersonError.Add(aobjPsPerson.icdoPsPerson.ps_person_id, aobjWssMemberRecordRequest.iarrErrors);
                iclbUnprocessedPsPerson.Add(aobjPsPerson);
                iclbPersonInformativeErrors.Add(aobjPsPerson);
            }
            else
            {
                if (aobjWssMemberRecordRequest.iarrErrors.Count > 0)
                {
                    aobjPsPerson.icdoPsPerson.processed_flag = busConstant.Flag_No;
                    if (!aobjPsPerson.idictPsPersonError.ContainsKey(aobjPsPerson.icdoPsPerson.ps_person_id))
                        aobjPsPerson.idictPsPersonError.Add(aobjPsPerson.icdoPsPerson.ps_person_id, aobjWssMemberRecordRequest.iarrErrors);
                    aobjPsPerson.icdoPsPerson.Update();

                    if (!iclbUnprocessedPsPerson.Contains(aobjPsPerson))
                    {
                        iclbUnprocessedPsPerson.Add(aobjPsPerson);
                        if (!aobjPsPerson.idictPsPersonError.ContainsKey(aobjPsPerson.icdoPsPerson.ps_person_id))
                            aobjPsPerson.idictPsPersonError.Add(aobjPsPerson.icdoPsPerson.ps_person_id, aobjWssMemberRecordRequest.iarrErrors);
                    }
                }
                else
                {
                    DataTable ltbMemberRecordRequest = Select("cdoPsEmployment.GetMemberRecordRequestBySSN", new object[1] { aobjPsPerson.icdoPsPerson.ssn });
                    if (ltbMemberRecordRequest.Rows.Count == 0)
                    {
                    aobjPsPerson.icdoPsPerson.processed_flag = busConstant.Flag_Yes;
                    aobjWssMemberRecordRequest.BeforePersistChanges();
                    aobjWssMemberRecordRequest.PersistChanges();
                    aobjWssMemberRecordRequest.ValidateSoftErrors();
                    aobjWssMemberRecordRequest.AfterPersistChanges();
                    if (!iclbProcessedPsPerson.Contains(aobjPsPerson))
                    {
                        iclbProcessedPsPerson.Add(aobjPsPerson);
                    }
                    }
                    //iclbProcessedPsPerson.ReplaceIfExists(lbusPsPerson);
                }
            }

        }
       
        /// <summary>
        /// Validate Member record Request from PS
        /// </summary>
        /// <param name="aobjWssEmploymentChangeRequest"></param>
        /// <param name="aobjPsPerson"></param>
        /// <param name="mode"></param>
        private void ValidatePSRecords(busWssEmploymentChangeRequest aobjWssEmploymentChangeRequest, busPsPerson aobjPsPerson, utlPageMode mode)
        {
            if (mode == utlPageMode.New)
                aobjWssEmploymentChangeRequest.iblnIsNewMode = true;
            aobjWssEmploymentChangeRequest.iblnisFromPs = true;
            aobjWssEmploymentChangeRequest.BeforeValidate(mode);
            //aobjWssEmploymentChangeRequest.ValidateHardErrors(mode);
            // This code code will evaluate the group rules according to the change type value as they are triggered in the wizard for respective wizard steps
            //PIR 12578 - Boolean values checked as there can be more than one change which need to be reflected.
            if (iblnEmploymentChangeRequestChangeTypeClassification)
            {
                aobjWssEmploymentChangeRequest.ValidateGroupRules("ClassificationChange", mode);
                if (aobjWssEmploymentChangeRequest.iarrErrors.Count == 0)
                {
                    aobjWssEmploymentChangeRequest.ValidateGroupRules("EmpTypeChange", mode);
                }
            }
            if (iblnEmploymentChangeRequestChangeTypeEmployment)
            {
                aobjWssEmploymentChangeRequest.ValidateGroupRules("EmpTypeChange", mode);
            }
            if (iblnEmploymentChangeRequestChangeTypeLOA || iblnEmploymentChangeRequestChangeTypeLOAM || iblnEmploymentChangeRequestChangeTypeFMLA)
            {
                aobjWssEmploymentChangeRequest.ValidateGroupRules("LOA", mode);
            }

            if (aobjWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.last_date_of_service != DateTime.MinValue)
            {
                aobjWssEmploymentChangeRequest.ValidateGroupRules("TerminateEmployment", mode);
            }

            //if ((aobjPsPerson.ibusPsAddress.iblnAddressChanged == true || aobjPsPerson.iblnPhoneNumberChanged ==true) && aobjWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.change_type_value == null)
            //{
            //    aobjWssEmploymentChangeRequest.iarrErrors = new ArrayList();
            //    utlError lobjError = null;
            //    lobjError = AddError(10209, "This record is just for demographic change.");
            //    aobjWssEmploymentChangeRequest.iarrErrors.Add(lobjError);
            //    aobjPsPerson.iblnDemographicChange = true;
            //}

            iblnEmploymentChangeRequestChangeTypeClassification = false;
            iblnEmploymentChangeRequestChangeTypeEmployment = false;
            iblnEmploymentChangeRequestChangeTypeLOA = false;
            iblnEmploymentChangeRequestChangeTypeLOAM = false;
            iblnEmploymentChangeRequestChangeTypeFMLA = false;

            if (aobjWssEmploymentChangeRequest.iarrErrors != null && aobjWssEmploymentChangeRequest.iarrErrors.Count > 0 && !aobjPsPerson.iblnDemographicChange)
            {
                if (!iclbUnprocessedPsPerson.Contains(aobjPsPerson))
                {
                    iclbUnprocessedPsPerson.Add(aobjPsPerson);
                    if (!aobjPsPerson.idictPsPersonError.ContainsKey(aobjPsPerson.icdoPsPerson.ps_person_id))
                        aobjPsPerson.idictPsPersonError.Add(aobjPsPerson.icdoPsPerson.ps_person_id, aobjWssEmploymentChangeRequest.iarrErrors);
                }
            }
            else
            {
                if (aobjPsPerson.iblnDemographicChange)
                    aobjPsPerson.icdoPsPerson.processed_flag = busConstant.Flag_Yes;
                else
                {
                    DataTable ltbEmploymentChangeRequest = Select("cdoPsEmployment.GetEmploymentChangeRequest", new object[1] 
                                                            { busGlobalFunctions.GetPersonIDBySSN(aobjPsPerson.icdoPsPerson.ssn) });
                    if (ltbEmploymentChangeRequest.Rows.Count == 0)
                    {
                    aobjPsPerson.icdoPsPerson.processed_flag = busConstant.Flag_Yes;
                    aobjWssEmploymentChangeRequest.BeforePersistChanges();
                    aobjWssEmploymentChangeRequest.PersistChanges();
                    aobjWssEmploymentChangeRequest.ValidateSoftErrors();
                    aobjWssEmploymentChangeRequest.AfterPersistChanges();
                }
                }
                if (!iclbProcessedPsPerson.Contains(aobjPsPerson))
                {
                    iclbProcessedPsPerson.Add(aobjPsPerson);
                }
            }
        }

        public busPerson LoadPersonBySSN(string astrssn)
        {
            Collection<busPerson> lclbPerson = new Collection<busPerson>();
            DataTable ldtbPerson = DBFunction.DBSelect("cdoPerson.GetPersonBySSN", new string[1] { astrssn },
                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            if (ldtbPerson.Rows.Count > 0)
            {
                lclbPerson = GetCollection<busPerson>(ldtbPerson, "icdoPerson");
                return lclbPerson[0];
            }
            else
                return null;
        }
    }
}



