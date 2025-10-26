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
using Sagitec.DataObjects;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPersonDependent : busPersonDependentGen
    {
        private busPerson _ibusPerson;
        public busPerson ibusPerson
        {
            get { return _ibusPerson; }
            set { _ibusPerson = value; }
        }
        public bool iblnIsRecordSelectToGenerateCor { get; set; }

        private busPerson _ibusDependentPerson;
        public busPerson ibusDependentPerson
        {
            get { return _ibusDependentPerson; }
            set { _ibusDependentPerson = value; }
        }

        public void LoadPerson()
        {
            if (_ibusPerson == null)
                _ibusPerson = new busPerson();
            _ibusPerson.FindPerson(icdoPersonDependent.person_id);
        }

        //This method is should be called  only when DEPENDENT PERSLINK ID > 0
        //Dependent PERSLink ID Could be Zero. At that time it loads the data that entered on the screen.
        //Ideally, when you want to load dependent person , Call the LoadDependentInfo() Method
        public void LoadDependentPerson()
        {
            if (_ibusDependentPerson == null)
                _ibusDependentPerson = new busPerson();

            if (icdoPersonDependent.dependent_perslink_id > 0)
                _ibusDependentPerson.FindPerson(icdoPersonDependent.dependent_perslink_id);
        }
        public bool iblnIsFromDeathNotification { get; set; } = false; // BPM Death Automation
        public void LoadDependentInfo()
        {
            if (icdoPersonDependent.dependent_perslink_id != 0)
            {
                if (ibusDependentPerson == null)
                    LoadDependentPerson();
                icdoPersonDependent.dependent_first_name = busGlobalFunctions.ToTitleCase(ibusDependentPerson.icdoPerson.first_name);
                icdoPersonDependent.dependent_last_name = busGlobalFunctions.ToTitleCase(ibusDependentPerson.icdoPerson.last_name);
                icdoPersonDependent.dependent_middle_name = ibusDependentPerson.icdoPerson.middle_name;
                icdoPersonDependent.dependent_name = busGlobalFunctions.ToTitleCase(ibusDependentPerson.icdoPerson.FullName);
                icdoPersonDependent.istrDependentNameForCorrs = busGlobalFunctions.ToTitleCase(ibusDependentPerson.icdoPerson.FullName);// PROD PIR ID 6199
                icdoPersonDependent.dependent_DOB = ibusDependentPerson.icdoPerson.date_of_birth;
                icdoPersonDependent.dependent_Contact_No = ibusDependentPerson.icdoPerson.home_phone_no;
                icdoPersonDependent.dependent_Email = ibusDependentPerson.icdoPerson.email_address;
                icdoPersonDependent.dependent_ssn = ibusDependentPerson.icdoPerson.ssn;
                icdoPersonDependent.dependent_last_4_digit_ssn = ibusDependentPerson.icdoPerson.LastFourDigitsOfSSN;
                icdoPersonDependent.dependent_marital_status = ibusDependentPerson.icdoPerson.marital_status_value;
                icdoPersonDependent.dependent_gender = ibusDependentPerson.icdoPerson.gender_value;
                //prod pir 7791
                icdoPersonDependent.dependent_marital_status_description = ibusDependentPerson.icdoPerson.marital_status_description;
                icdoPersonDependent.dependent_gender_description = ibusDependentPerson.icdoPerson.gender_description;
                icdoPersonDependent.dependent_prefix_name = ibusDependentPerson.icdoPerson.name_prefix_description;
                icdoPersonDependent.dependent_suffix_name = ibusDependentPerson.icdoPerson.name_suffix_description;
                icdoPersonDependent.dependent_date_of_death = ibusDependentPerson.icdoPerson.date_of_death; // PROD PIR ID 6590
                icdoPersonDependent.dependent_ms_change_date = ibusDependentPerson.icdoPerson.ms_change_date;
            }
            else
            {
                //PIR - UAT 975
                string lstrFullName = icdoPersonDependent.first_name + " " + icdoPersonDependent.middle_name_no_null + " " + icdoPersonDependent.last_name; // PROD PIR ID 6199
                string lstrFirstMiddleLastName = string.Empty;
                if (!String.IsNullOrEmpty(icdoPersonDependent.first_name))
                {
                    lstrFirstMiddleLastName = icdoPersonDependent.first_name.Trim();
                }
                if (!String.IsNullOrEmpty(icdoPersonDependent.middle_name))
                {
                    lstrFirstMiddleLastName += " " + icdoPersonDependent.middle_name.Trim();
                }
                if (!String.IsNullOrEmpty(icdoPersonDependent.last_name))
                    lstrFirstMiddleLastName += " " + icdoPersonDependent.last_name.Trim();

                icdoPersonDependent.dependent_first_name = busGlobalFunctions.ToTitleCase(icdoPersonDependent.first_name);
                icdoPersonDependent.dependent_last_name = busGlobalFunctions.ToTitleCase(icdoPersonDependent.last_name);
                icdoPersonDependent.dependent_middle_name = icdoPersonDependent.middle_name;
                icdoPersonDependent.dependent_name = busGlobalFunctions.ToTitleCase(lstrFullName);
                icdoPersonDependent.istrDependentNameForCorrs = busGlobalFunctions.ToTitleCase(lstrFirstMiddleLastName);
                icdoPersonDependent.dependent_DOB = icdoPersonDependent.date_of_birth; ;
                icdoPersonDependent.dependent_Contact_No = icdoPersonDependent.contact_phone_no;
                icdoPersonDependent.dependent_Email = icdoPersonDependent.email_address;
                icdoPersonDependent.dependent_marital_status = icdoPersonDependent.marital_status_value;
                icdoPersonDependent.dependent_gender = icdoPersonDependent.gender_value;
                //prod pir 7791
                icdoPersonDependent.dependent_marital_status_description = icdoPersonDependent.marital_status_description;
                icdoPersonDependent.dependent_gender_description = icdoPersonDependent.gender_description;
            }
        }

        private Collection<busPersonDependent> _iclbOtherDependents;
        public Collection<busPersonDependent> iclbOtherDependents
        {
            get { return _iclbOtherDependents; }
            set { _iclbOtherDependents = value; }
        }

        public void LoadOtherDependents()
        {
            DataTable ldtbOtherDependents = Select("cdoPersonDependent.LoadOtherDependents", new object[2] {
                                                icdoPersonDependent.person_id, icdoPersonDependent.person_dependent_id });
            _iclbOtherDependents = new Collection<busPersonDependent>();
            foreach (DataRow dr in ldtbOtherDependents.Rows)
            {
                busPersonDependent lobjDependent = new busPersonDependent();
                lobjDependent.ibusPeronAccountDependent = new busPersonAccountDependent();
                lobjDependent.icdoPersonDependent = new cdoPersonDependent();
                lobjDependent.ibusPeronAccountDependent.icdoPersonAccountDependent = new cdoPersonAccountDependent();
                lobjDependent.icdoPersonDependent.LoadData(dr);
                lobjDependent.ibusPeronAccountDependent.icdoPersonAccountDependent.LoadData(dr);
                lobjDependent.LoadDependentInfo();
                _iclbOtherDependents.Add(lobjDependent);
            }
        }

        private Collection<busPersonAccountDependent> _iclbPersonAccountDependent;
        public Collection<busPersonAccountDependent> iclbPersonAccountDependent
        {
            get { return _iclbPersonAccountDependent; }
            set { _iclbPersonAccountDependent = value; }
        }

        public void LoadPersonAccountDependent()
        {
            DataTable ldtbList = Select<cdoPersonAccountDependent>(
                new string[1] { "person_dependent_id" },
                new object[1] { icdoPersonDependent.person_dependent_id }, null, null);
            _iclbPersonAccountDependent = GetCollection<busPersonAccountDependent>(ldtbList, "icdoPersonAccountDependent");
        }

        //In New Mode, It loads all the Plans that are enrolled by the member
        public void LoadPersonAccountDependentNew()
        {
            /// Load Grid Data in new mode.
            DataTable ldtbPersonAccount = Select("cdoPersonDependent.LoadPlanEnrolledByPerson", new object[1] { icdoPersonDependent.person_id });
            _iclbPersonAccountDependent = GetCollection<busPersonAccountDependent>(ldtbPersonAccount, "icdoPersonAccountDependent");
        }

        public void LoadPersonAccountDependentUpdate()
        {
            if (_iclbPersonAccountDependent == null)
                LoadPersonAccountDependentNew();

            /// Load Grid Data in update mode with data.
            foreach (busPersonAccountDependent lobjPersonAccount in _iclbPersonAccountDependent)
            {
                lobjPersonAccount.FindPersonAccountDependent(icdoPersonDependent.person_dependent_id, lobjPersonAccount.icdoPersonAccountDependent.person_account_id);
            }
        }

        public bool iblnIsMedicareInfoModified { get; set; }

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            foreach (busPersonAccountDependent lobjPersonAccount in _iclbPersonAccountDependent)
            {
                if (lobjPersonAccount.icdoPersonAccountDependent.start_date != DateTime.MinValue || icdoPersonDependent.iblnNewResult) //PIR 11841
                    lobjPersonAccount.icdoPersonAccountDependent.lblnValueEntered = true;
                //prod pir 7613
                if (lobjPersonAccount.icdoPersonAccountDependent.ihstOldValues.Count > 0 && lobjPersonAccount.icdoPersonAccountDependent.plan_id == busConstant.PlanIdGroupHealth)
                {
                    if ((lobjPersonAccount.icdoPersonAccountDependent.ihstOldValues["start_date"] != null &&
                        Convert.ToDateTime(Convert.ToDateTime(lobjPersonAccount.icdoPersonAccountDependent.ihstOldValues["start_date"]).ToString(busConstant.DateFormatMMddyyyy))
                            != lobjPersonAccount.icdoPersonAccountDependent.start_date) ||
                        (lobjPersonAccount.icdoPersonAccountDependent.ihstOldValues["end_date"] != null &&
                        Convert.ToDateTime(Convert.ToDateTime(lobjPersonAccount.icdoPersonAccountDependent.ihstOldValues["end_date"]).ToString(busConstant.DateFormatMMddyyyy))
                            != lobjPersonAccount.icdoPersonAccountDependent.end_date) ||
                        ((lobjPersonAccount.icdoPersonAccountDependent.ihstOldValues["start_date"] == null || Convert.ToDateTime(lobjPersonAccount.icdoPersonAccountDependent.ihstOldValues["start_date"]) == DateTime.MinValue)
                            && lobjPersonAccount.icdoPersonAccountDependent.start_date != DateTime.MinValue) ||
                        ((lobjPersonAccount.icdoPersonAccountDependent.ihstOldValues["end_date"] == null || Convert.ToDateTime(lobjPersonAccount.icdoPersonAccountDependent.ihstOldValues["end_date"]) == DateTime.MinValue)
                            && lobjPersonAccount.icdoPersonAccountDependent.end_date != DateTime.MinValue))
                    {
                        lobjPersonAccount.icdoPersonAccountDependent.iblnNeedToCreateAutomaticRHIC = true;
                    }
                }
            }
            LoadDependentInfo();

            /// Validate address only if Person ID not entered and Address not same as Member address.(as per mail from Maik)
            /// Load Dependent Person  Address if dependent perslink id is entered.
            if (icdoPersonDependent.dependent_perslink_id != 0)
            {
                //Reload
                LoadDependentPerson();
                _ibusDependentPerson.LoadPersonCurrentAddress();
            }

            //validate address using USPS
            //if dependent perslink id is entered and address fields not 
            //entered and same as member address is not checked and dependent person doesnt have address.
            if (((icdoPersonDependent.dependent_perslink_id == 0) &&
               (icdoPersonDependent.same_as_member_address != busConstant.Flag_Yes))
                ||
                ((icdoPersonDependent.dependent_perslink_id != 0)
                && (icdoPersonDependent.same_as_member_address != busConstant.Flag_Yes)
                && (_ibusDependentPerson.ibusPersonCurrentAddress.icdoPersonAddress.person_address_id == 0)))
                VerifyAddressUsingUSPS();

            _AintValidateDependent = ValidateDependent();
            _AintValidatedAddress = ValidateAddress();

            base.BeforeValidate(aenmPageMode);
        }

        public override void ValidateHardErrors(utlPageMode aenmPageMode)
        {
            base.ValidateHardErrors(aenmPageMode);
        }

        public override void BeforePersistChanges()
        {
            iblnIsMedicareInfoModified = IsMedicareInfoModified();
            // pir 7232 Trim the leading and trailing spaces before saving
            //PIR 11071
            if (!icdoPersonDependent.first_name.IsNullOrEmpty())
                icdoPersonDependent.first_name = icdoPersonDependent.first_name.Trim();
            if (!icdoPersonDependent.last_name.IsNullOrEmpty())
                icdoPersonDependent.last_name = icdoPersonDependent.last_name.Trim();
            if (!icdoPersonDependent.middle_name.IsNullOrEmpty())
                icdoPersonDependent.middle_name = icdoPersonDependent.middle_name.Trim();

            //PIR 23774 - if dependent relationship is Spouse and Spouse is a Receiver of Approved RHIC Combine record - trigger ‘Maintain RHIC Combining’ WFL
            if (ibusDependentPerson.IsNull()) LoadDependentPerson();
            if (ibusDependentPerson?.icdoPerson?.person_id > 0 && icdoPersonDependent.relationship_value == busConstant.DependentRelationshipSpouse &&
                !busWorkflowHelper.IsActiveInstanceAvailable(ibusDependentPerson.icdoPerson.person_id, busConstant.Map_Maintain_Rhic))
            {
                if (ibusDependentPerson.ibusLatestBenefitRhicCombine == null)
                    ibusDependentPerson.LoadLatestBenefitRhicCombine();

                if (ibusDependentPerson.ibusLatestBenefitRhicCombine != null && ibusDependentPerson.ibusLatestBenefitRhicCombine.icdoBenefitRhicCombine.benefit_rhic_combine_id > 0)
                {
                    ibusDependentPerson.ibusLatestBenefitRhicCombine.InitiateRHICCombineWorkflow();
                }
            }

            base.BeforePersistChanges();
        }

        public override int PersistChanges()
        {
            if (ibusPerson.IsNull()) LoadPerson();

            if (icdoPersonDependent.ienuObjectState == ObjectState.Insert)
            {
                icdoPersonDependent.Insert();
                foreach (busPersonAccountDependent lobjPersonAccount in _iclbPersonAccountDependent)
                {
                    if (lobjPersonAccount.ibusPersonAccount.IsNull()) lobjPersonAccount.LoadPersonAccount();
                    if (lobjPersonAccount.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth)
                    {
                        if (lobjPersonAccount.icdoPersonAccountDependent.start_date != DateTime.MinValue)
                            lobjPersonAccount.icdoPersonAccountDependent.is_modified_after_bcbs_file_sent_flag = busConstant.Flag_Yes;
                    }

                    lobjPersonAccount.icdoPersonAccountDependent.person_dependent_id = icdoPersonDependent.person_dependent_id;
                    if (lobjPersonAccount.icdoPersonAccountDependent.lblnValueEntered)
                    {
                        if (lobjPersonAccount.icdoPersonAccountDependent.end_date == DateTime.MinValue)
                            lobjPersonAccount.icdoPersonAccountDependent.is_drop_dependent_letter_sent_flag = null;
                        //Backlog PIR 8015 (PIR 16462, PIR 11040)
                        else if (lobjPersonAccount.icdoPersonAccountDependent.end_date != DateTime.MinValue &&
                                 lobjPersonAccount.icdoPersonAccountDependent.is_drop_dependent_letter_sent_flag != busConstant.Flag_Yes)
                            lobjPersonAccount.icdoPersonAccountDependent.is_drop_dependent_letter_sent_flag = busConstant.Flag_No;
                        lobjPersonAccount.icdoPersonAccountDependent.Insert();
                        //PIR 14346 Commented
                        //UAT PIR 1969 - Rhic combining Trigger should be here too as per David Comments
                        //If Combine Records available for this person, then we create AR otherwise We dont create AR.
                        //if (lobjPersonAccount.icdoPersonAccountDependent.plan_id == busConstant.PlanIdGroupHealth && 
                        //    lobjPersonAccount.icdoPersonAccountDependent.iblnNeedToCreateAutomaticRHIC &&
                        //    ibusPerson.icdoPerson.date_of_death == DateTime.MinValue) //PROD PIR 7709
                        //{
                        //    CreateAutomaticRHICCombine(busGlobalFunctions.GetMax(lobjPersonAccount.icdoPersonAccountDependent.start_date,
                        //        lobjPersonAccount.icdoPersonAccountDependent.end_date == DateTime.MinValue ? DateTime.MinValue : lobjPersonAccount.icdoPersonAccountDependent.end_date.GetFirstDayofNextMonth()));
                        //}
                    }
                }
            }
            else
            {
                icdoPersonDependent.Update();
                foreach (busPersonAccountDependent lobjPersonAccount in _iclbPersonAccountDependent)
                {
                    if (lobjPersonAccount.ibusPersonAccount.IsNull()) lobjPersonAccount.LoadPersonAccount();
                    if (lobjPersonAccount.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth)
                    {
                        // UAT PIR ID 2170
                        if (lobjPersonAccount.icdoPersonAccountDependent.ihstOldValues.Count > 0)
                        {
                            DateTime ldtePrevStartDate = Convert.ToDateTime(lobjPersonAccount.icdoPersonAccountDependent.ihstOldValues["start_date"]);
                            DateTime ldtePrevEndDate = Convert.ToDateTime(lobjPersonAccount.icdoPersonAccountDependent.ihstOldValues["end_date"]);
                            if (ldtePrevStartDate.Date != lobjPersonAccount.icdoPersonAccountDependent.start_date.Date ||
                                ldtePrevEndDate.Date != lobjPersonAccount.icdoPersonAccountDependent.end_date.Date ||
                                iblnIsMedicareInfoModified)
                                lobjPersonAccount.icdoPersonAccountDependent.is_modified_after_bcbs_file_sent_flag = busConstant.Flag_Yes;
                        }
                    }

                    // SYS PIR ID 2614
                    if ((lobjPersonAccount.icdoPersonAccountDependent.is_bcbs_file_sent_flag == busConstant.Flag_Yes) &&
                        (lobjPersonAccount.icdoPersonAccountDependent.end_date == DateTime.MinValue))
                        lobjPersonAccount.icdoPersonAccountDependent.is_bcbs_file_sent_flag = busConstant.Flag_No;

                    if (lobjPersonAccount.icdoPersonAccountDependent.lblnValueEntered)
                    {
                        lobjPersonAccount.icdoPersonAccountDependent.person_dependent_id = icdoPersonDependent.person_dependent_id;
                        //if (_iclbPersonAccountDependent.Where(o => o.icdoPersonAccountDependent.person_account_id == lobjPersonAccount.icdoPersonAccountDependent.person_account_id &&
                        //            o.icdoPersonAccountDependent.person_dependent_id == icdoPersonDependent.person_dependent_id).Any())

                        //PIR 6918
                        if (lobjPersonAccount.icdoPersonAccountDependent.end_date == DateTime.MinValue)
                            lobjPersonAccount.icdoPersonAccountDependent.is_drop_dependent_letter_sent_flag = null;
                        //Backlog PIR 8015 (PIR 16462, PIR 11040)
                        else if (lobjPersonAccount.icdoPersonAccountDependent.end_date != DateTime.MinValue &&
                                 lobjPersonAccount.icdoPersonAccountDependent.is_drop_dependent_letter_sent_flag != busConstant.Flag_Yes)
                            lobjPersonAccount.icdoPersonAccountDependent.is_drop_dependent_letter_sent_flag = busConstant.Flag_No;
                        //PIR 6918 End
                        if (lobjPersonAccount.icdoPersonAccountDependent.person_account_dependent_id > 0)
                            lobjPersonAccount.icdoPersonAccountDependent.Update();
                        else
                            lobjPersonAccount.icdoPersonAccountDependent.Insert();
                        //PIR 14346 - Commented
                        //UAT PIR 1969 - Rhic combining Trigger should be here too as per David Comments
                        //If Combine Records available for this person, then we create AR otherwise We dont create AR.
                        //if (lobjPersonAccount.icdoPersonAccountDependent.plan_id == busConstant.PlanIdGroupHealth &&
                        //    lobjPersonAccount.icdoPersonAccountDependent.iblnNeedToCreateAutomaticRHIC &&
                        //    ibusPerson.icdoPerson.date_of_death == DateTime.MinValue) //PROD PIR 7709
                        //{
                        //    CreateAutomaticRHICCombine(busGlobalFunctions.GetMax(lobjPersonAccount.icdoPersonAccountDependent.start_date,
                        //        lobjPersonAccount.icdoPersonAccountDependent.end_date == DateTime.MinValue ? DateTime.MinValue : lobjPersonAccount.icdoPersonAccountDependent.end_date.GetFirstDayofNextMonth()));
                        //}
                    }
                }
            }
            return 1;
        }
        //PIR 14346 - Commented
        //private void CreateAutomaticRHICCombine(DateTime adtStartDate)
        //{
        //    if (ibusPerson == null)
        //        LoadPerson();

        //    busBenefitRhicCombine lbusBenefitRhicCombine = new busBenefitRhicCombine { icdoBenefitRhicCombine = new cdoBenefitRhicCombine() };
        //    lbusBenefitRhicCombine.icdoBenefitRhicCombine.person_id = icdoPersonDependent.person_id;
        //    //Reload the Person Account to get the latest plan participation status
        //    ibusPerson.LoadPersonAccount();
        //    lbusBenefitRhicCombine.ibusPerson = ibusPerson;
        //    if (ibusPerson.ibusHealthPersonAccount == null)
        //        ibusPerson.LoadHealthPersonAccount();
        //    lbusBenefitRhicCombine.icdoBenefitRhicCombine.start_date = GetRHICCombineStartDate(adtStartDate);
        //    //prod pir 7613 : dont create automatic RHIC combine if not able to determine start date
        //    if (lbusBenefitRhicCombine.icdoBenefitRhicCombine.start_date != DateTime.MinValue)
        //    {
        //        lbusBenefitRhicCombine.ienmAutomaticRhicCombineTrigger = busConstant.automatic_rhic_combine_trigger.enrollment_change;
        //        //Reload Receiver Health Person Account
        //        lbusBenefitRhicCombine.ibusPerson.LoadReceiverHealthPersonAccount(lbusBenefitRhicCombine.icdoBenefitRhicCombine.start_date);
        //        lbusBenefitRhicCombine.CreateAutomaticRHICCombine();

        //        //Reload the Latest Rhic Combine After AR
        //        ibusPerson.LoadBenefitRhicCombine();
        //        ibusPerson.LoadLatestBenefitRhicCombine();
        //    }
        //}

        // PROD PIR ID 5867
        public DateTime GetRHICCombineStartDate(DateTime adtStartDate)
        {
            // Email from Maik - Wed 2/16/2011
            // Instead of using the Retirement Date as the RHIC Effective Start Date, use the most recent date of the following: 
            // Retirement Date, latest Health enrollment, current RHIC rate effective date, or RHIC combining (combined with Spouse) – modification to point 3.
            if (ibusPerson.IsNull()) LoadPerson();
            if (ibusPerson.iclbPayeeAccount.IsNull()) ibusPerson.LoadPayeeAccount(true);
            foreach (busPayeeAccount lobjPayeeAccount in ibusPerson.iclbPayeeAccount) // PROD PIR ID 6762
            {
                if (!lobjPayeeAccount.ibusPayeeAccountActiveStatus.IsCancelledOrCompletedForRetirement())
                {
                    if (lobjPayeeAccount.ibusLatestBenefitRhicCombine.IsNull())
                        lobjPayeeAccount.LoadLatestBenefitRhicCombine();
                }
            }
            var lbusPayeeAccount = ibusPerson.iclbPayeeAccount.Where(i => !i.ibusPayeeAccountActiveStatus.IsCancelledOrCompletedForRetirement() &&
                        i.ibusLatestBenefitRhicCombine.IsNotNull()).OrderByDescending(i => i.ibusLatestBenefitRhicCombine.icdoBenefitRhicCombine.start_date).FirstOrDefault();
            //prod pir 7613 : if not rhic combine exists, use payee account and derieve the start date
            var lbusRetrPayeeAccount = ibusPerson.iclbPayeeAccount.Where(i => !i.ibusPayeeAccountActiveStatus.IsCancelledOrCompletedForRetirement()).FirstOrDefault();
            if (lbusPayeeAccount.IsNotNull())
            {
                lbusPayeeAccount.idtDependentChangeDate = adtStartDate;
                return lbusPayeeAccount.GetRHICCombineStartDate();
            }
            //prod pir 7613 : if not rhic combine exists, use payee account and derieve the start date
            else if (lbusRetrPayeeAccount.IsNotNull())
            {
                bool lblnCreateRhicCombine = false;
                if (lbusRetrPayeeAccount.iclbBenefitRHICCombineDetail == null)
                    lbusRetrPayeeAccount.LoadBenefitRhicCombineDetail();
                if (lbusRetrPayeeAccount.iclbBenefitRHICCombineDetail.Count == 0)
                {
                    if (lbusRetrPayeeAccount.ibusPayee == null)
                        lbusRetrPayeeAccount.LoadPayee();

                    if (lbusRetrPayeeAccount.ibusPayee.iclbBenefitRhicCombine == null)
                        lbusRetrPayeeAccount.ibusPayee.LoadBenefitRhicCombine();

                    if (lbusRetrPayeeAccount.ibusPayee.iclbBenefitRhicCombine.Count > 0)
                        lblnCreateRhicCombine = true;
                }
                else
                    lblnCreateRhicCombine = true;
                if (lblnCreateRhicCombine)
                {
                    lbusRetrPayeeAccount.idtDependentChangeDate = adtStartDate;
                    return lbusRetrPayeeAccount.GetRHICCombineStartDate();
                }
            }
            return DateTime.MinValue;
        }

        private int _AintValidateDependent;
        public int AintValidateDependent
        {
            get { return _AintValidateDependent; }
            set { _AintValidateDependent = value; }
        }

        public int ValidateDependent()
        {
            bool lblnIsPlanEnteredForDependent = false;
            bool lblnIsEndDated = false;

            foreach (busPersonAccountDependent lobjPersonAccount in _iclbPersonAccountDependent)
            {
                lblnIsEndDated = false;
                if (lobjPersonAccount.ibusPersonAccount.IsNull())
                    lobjPersonAccount.LoadPersonAccount();

                // Check whether Dependent person is already enrolled for a Member
                if (icdoPersonDependent.dependent_perslink_id != 0)
                {
                    DataTable ldtbLists = new DataTable();
                    if (icdoPersonDependent.ienuObjectState == ObjectState.Insert)
                    {
                        ldtbLists = Select<cdoPersonDependent>(new string[1] { "dependent_perslink_id" },
                            new object[1] { icdoPersonDependent.dependent_perslink_id }, null, null);
                    }
                    else
                    {
                        ldtbLists = SelectWithOperator<cdoPersonDependent>(new string[2] { "dependent_perslink_id", "person_dependent_id" },
                            new string[2] { "=", "<>" },
                            new object[2] { icdoPersonDependent.dependent_perslink_id, icdoPersonDependent.person_dependent_id }, null);
                    }
                    foreach (DataRow ldr in ldtbLists.Rows)
                    {
                        busPersonDependent lobjPersonDependent = new busPersonDependent { icdoPersonDependent = new cdoPersonDependent() };
                        lobjPersonDependent.icdoPersonDependent.LoadData(ldr);
                        lobjPersonDependent.LoadPersonAccountDependent();
                        foreach (busPersonAccountDependent lobjPADependent in lobjPersonDependent.iclbPersonAccountDependent)
                        {
                            if (lobjPADependent.ibusPersonAccount.IsNull())
                                lobjPADependent.LoadPersonAccount();
                            /* PIR ID 1706 - Dependent Person check should consider the Plan and dates too. */
                            if ((lobjPADependent.ibusPersonAccount.icdoPersonAccount.plan_id == lobjPersonAccount.ibusPersonAccount.icdoPersonAccount.plan_id) ||
                                ((lobjPADependent.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth) &&
                                    (lobjPersonAccount.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdHMO)) ||
                                ((lobjPADependent.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdHMO) &&
                                    (lobjPersonAccount.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth)))
                            {
                                if ((lobjPADependent.icdoPersonAccountDependent.start_date != DateTime.MinValue) &&
                                    (lobjPersonAccount.icdoPersonAccountDependent.start_date != DateTime.MinValue))
                                {
                                    if (busGlobalFunctions.CheckDateOverlapping(lobjPADependent.icdoPersonAccountDependent.start_date,
                                                    lobjPADependent.icdoPersonAccountDependent.end_date,
                                                    lobjPersonAccount.icdoPersonAccountDependent.start_date,
                                                    lobjPersonAccount.icdoPersonAccountDependent.end_date))
                                        return 13;
                                }
                            }
                        }
                    }
                }

                if ((lobjPersonAccount.icdoPersonAccountDependent.start_date == DateTime.MinValue) &&
                    ((lobjPersonAccount.icdoPersonAccountDependent.nmso_co_flag == busConstant.Flag_Yes) ||
                    (lobjPersonAccount.icdoPersonAccountDependent.out_area_flag == busConstant.Flag_Yes)))
                {
                    // Start Date is Mandatory.
                    return 1;
                }
                if (lobjPersonAccount.icdoPersonAccountDependent.lblnValueEntered)
                {
                    if (lobjPersonAccount.icdoPersonAccountDependent.start_date != DateTime.MinValue)
                    {
                        icdoPersonDependent.dependent_age = busGlobalFunctions.CalulateAge(
                                                                icdoPersonDependent.dependent_DOB,
                                                                lobjPersonAccount.icdoPersonAccountDependent.start_date);
                        if (!IsDateOverlapping(lobjPersonAccount.icdoPersonAccountDependent.person_account_id,
                                                lobjPersonAccount.icdoPersonAccountDependent.start_date))
                        {
                            // Start Date cannot be earlier than Plan Participation Start Date
                            return 8;
                        }
                    }

                    if (lobjPersonAccount.icdoPersonAccountDependent.end_date != DateTime.MinValue)
                    {
                        if (lobjPersonAccount.iblnIsNeedToValidate && !IsDateOverlapping(lobjPersonAccount.icdoPersonAccountDependent.person_account_id,
                                                lobjPersonAccount.icdoPersonAccountDependent.end_date))
                        {
                            // Dependent coverage End Date should be between Plan Participation Start Date and End Date
                            return 9;
                        }
                        if (lobjPersonAccount.icdoPersonAccountDependent.ihstOldValues.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(lobjPersonAccount.icdoPersonAccountDependent.ihstOldValues["end_date"].ToString()))
                            {
                                if (Convert.ToDateTime(lobjPersonAccount.icdoPersonAccountDependent.ihstOldValues["end_date"]) == DateTime.MinValue)
                                    lblnIsEndDated = true;
                            }
                        }
                    }

                    if (lobjPersonAccount.icdoPersonAccountDependent.start_date != DateTime.MinValue &&
                        lobjPersonAccount.icdoPersonAccountDependent.end_date != DateTime.MinValue &&
                        lobjPersonAccount.icdoPersonAccountDependent.start_date > lobjPersonAccount.icdoPersonAccountDependent.end_date)
                        return 15;

                    // ******* START PROD PIR ID 5509 *******
                    // Validation related to Full time flag are made inactive in business xml too.
                    //prod pir 6262 : should not do the below validation for relationship spouse
                    if (icdoPersonDependent.relationship_value != busConstant.DependentRelationshipDisabledChild &&
                        icdoPersonDependent.relationship_value != busConstant.DependentRelationshipSpouse &&
                        icdoPersonDependent.relationship_value != busConstant.DependentRelationshipExSpouse)
                    {
                        if (icdoPersonDependent.dependent_age >= 26) //PIR 11905
                            return 3;
                    }
                    //if ((icdoPersonDependent.relationship_value == busConstant.DependentRelationshipAdoptiveChild) ||
                    //    (icdoPersonDependent.relationship_value == busConstant.DependentRelationshipChild) ||
                    //    (icdoPersonDependent.relationship_value == busConstant.DependentRelationshipGrandChild) ||
                    //    (icdoPersonDependent.relationship_value == busConstant.DependentRelationshipStepChild) ||
                    //    (icdoPersonDependent.relationship_value == busConstant.DependentRelationshipDisabledChild))
                    //{
                    //    if (icdoPersonDependent.marital_status_value == busConstant.PersonMaritalStatusMarried)
                    //    {
                    //        // Married person cannot be enrolled as dependent.
                    //        return 2;
                    //    }
                    //    if (icdoPersonDependent.dependent_age >= 23)
                    //    {
                    //        if (icdoPersonDependent.relationship_value != busConstant.DependentRelationshipDisabledChild)
                    //        {
                    //            // Dependent 'Full Time Student' can be added if the Dependent Age is less than 26.
                    //            if ((icdoPersonDependent.full_time_student_flag != busConstant.Flag_Yes) &&
                    //                (icdoPersonDependent.dependent_age <= 26))
                    //                return 4;
                    //            // Only Disabled child can only be selected if the Dependent Age exceeds 26.
                    //            if (icdoPersonDependent.dependent_age > 26)
                    //                return 3;
                    //        }
                    //    }
                    //}
                    // ******* END PROD PIR ID 5509 *******

                    if (icdoPersonDependent.dependent_perslink_id != 0)
                    {
                        if (!lblnIsEndDated)
                        {
                            busPerson lobjPerson = new busPerson();
                            if (lobjPerson.FindPerson(icdoPersonDependent.dependent_perslink_id))
                            {
                                // 7059 Dependent Person is already enrolled for the same plan with his Employer.
                                lobjPerson.LoadPersonAccount();
                                foreach (busPersonAccount lobjDependentPersonAccount in lobjPerson.icolPersonAccount)
                                {
                                    if (iblnFromPortal)
                                    {
                                        if ((lobjDependentPersonAccount.icdoPersonAccount.plan_id == icdoPersonDependent.iintPlanId) &&
                                                                   (busGlobalFunctions.CheckDateOverlapping(icdoPersonDependent.start_date,
                                                                                                            DateTime.MinValue,
                                                                                                            lobjDependentPersonAccount.icdoPersonAccount.start_date,
                                                                                                            lobjDependentPersonAccount.icdoPersonAccount.end_date_no_null)) &&
                                                                   (lobjDependentPersonAccount.icdoPersonAccount.plan_participation_status_value !=
                                                                                                             busConstant.PlanParticipationStatusInsuranceSuspended))
                                        {
                                            return 7;
                                        }
                                    }
                                    else if ((lobjDependentPersonAccount.icdoPersonAccount.plan_id == lobjPersonAccount.icdoPersonAccountDependent.plan_id) &&
                                        (busGlobalFunctions.CheckDateOverlapping(lobjPersonAccount.icdoPersonAccountDependent.start_date,
                                                                                 lobjPersonAccount.icdoPersonAccountDependent.end_date_no_null,
                                                                                 lobjDependentPersonAccount.icdoPersonAccount.start_date,
                                                                                 lobjDependentPersonAccount.icdoPersonAccount.end_date_no_null)) &&
                                        (lobjDependentPersonAccount.icdoPersonAccount.plan_participation_status_value !=
                                                                                  busConstant.PlanParticipationStatusInsuranceSuspended))
                                    {
                                        return 7;
                                    }

                                    // PROD PIR 5132
                                    // 7016 Dependent Person has overlapping State Employment associated to Health Plan.
                                    if (lobjDependentPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth &&
                                        lobjPersonAccount.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth)
                                    {
                                        if (lobjDependentPersonAccount.iclbAccountEmploymentDetail.IsNull()) lobjDependentPersonAccount.LoadPersonAccountEmploymentDetails();
                                        foreach (busPersonAccountEmploymentDetail lobjPAEmpDtl in lobjDependentPersonAccount.iclbAccountEmploymentDetail)
                                        {
                                            if (lobjPAEmpDtl.ibusEmploymentDetail.IsNull()) lobjPAEmpDtl.LoadPersonEmploymentDetail(DateTime.Now, false);
                                            if (lobjPAEmpDtl.ibusEmploymentDetail.ibusPersonEmployment.IsNull()) lobjPAEmpDtl.ibusEmploymentDetail.LoadPersonEmployment();
                                            if (lobjPAEmpDtl.ibusEmploymentDetail.ibusPersonEmployment.ibusOrganization.IsNull())
                                                lobjPAEmpDtl.ibusEmploymentDetail.ibusPersonEmployment.LoadOrganization();
                                            if (lobjPersonAccount.ibusPersonAccountGhdv.IsNull()) lobjPersonAccount.LoadPersonAccountGhdv();
                                            if (lobjPersonAccount.ibusPersonAccountGhdv.icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeState &&
                                                lobjPAEmpDtl.ibusEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.emp_category_value == busConstant.EmployerCategoryState &&
                                                busGlobalFunctions.CheckDateOverlapping(lobjPAEmpDtl.ibusEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.start_date,
                                                lobjPAEmpDtl.ibusEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.end_date,
                                                lobjPersonAccount.icdoPersonAccountDependent.start_date,
                                                lobjPersonAccount.icdoPersonAccountDependent.end_date))
                                            {
                                                //prod pir 7175 : need to check the enrollment for dependent person account and if its Enrolled, throw error
                                                if (lobjDependentPersonAccount.ibusPersonAccountGHDV == null)
                                                    lobjDependentPersonAccount.LoadPersonAccountGHDV();
                                                busPersonAccountGhdvHistory lobjPAHistory = lobjDependentPersonAccount.ibusPersonAccountGHDV.LoadHistoryByDate(lobjPersonAccount.icdoPersonAccountDependent.start_date);
                                                if (lobjPAHistory.icdoPersonAccountGhdvHistory.person_account_ghdv_history_id > 0)
                                                {
                                                    busPersonAccountGhdv lobjPAGHDV = new busPersonAccountGhdv { icdoPersonAccount = new cdoPersonAccount(), icdoPersonAccountGhdv = new cdoPersonAccountGhdv() };
                                                    lobjPAGHDV = lobjPAHistory.LoadGHDVObject(lobjPAGHDV);
                                                    if (lobjPAGHDV.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                                                        return 10;
                                                }
                                            }
                                        }
                                    }
                                }
                                //// Check whether the Dependent's Employer is also providing the same plan.
                                //lobjPerson.LoadPersonEmployment();
                                //foreach (busPersonEmployment lobjEmployment in lobjPerson.icolPersonEmployment)
                                //{
                                //    if (busGlobalFunctions.CheckDateOverlapping(
                                //                            lobjPersonAccount.icdoPersonAccountDependent.start_date_no_null,
                                //                            lobjPersonAccount.icdoPersonAccountDependent.end_date_no_null,
                                //                            lobjEmployment.icdoPersonEmployment.start_date,
                                //                            lobjEmployment.icdoPersonEmployment.end_date_no_null))
                                //    {
                                //        busOrganization lobjOrganization = new busOrganization();
                                //        lobjOrganization.FindOrganization(lobjEmployment.icdoPersonEmployment.org_id);
                                //        lobjOrganization.LoadOrganizationOfferedPlans();
                                //        //prod pir - 4086
                                //        //check need to be done only for Health plan, as discussed with Maik, 18 Oct 2010
                                //        var lobjOrgPlan = lobjOrganization.iclbOrganizationOfferedPlans.Where(o => o.icdoOrgPlan.plan_id == busConstant.PlanIdGroupHealth ||
                                //            o.icdoOrgPlan.plan_id == busConstant.PlanIdMedicarePartD).FirstOrDefault();
                                //        if(lobjOrgPlan != null)
                                //        {
                                //            if ((busGlobalFunctions.CheckDateOverlapping(lobjPersonAccount.icdoPersonAccountDependent.start_date,
                                //                                                        lobjPersonAccount.icdoPersonAccountDependent.end_date_no_null,
                                //                                                        lobjOrgPlan.icdoOrgPlan.participation_start_date,
                                //                                                        lobjOrgPlan.icdoOrgPlan.end_date_no_null)) &&
                                //                (lobjOrgPlan.icdoOrgPlan.plan_id == lobjPersonAccount.icdoPersonAccountDependent.plan_id))
                                //            {
                                //                if (lobjPersonAccount.ibusPersonAccount == null)
                                //                    lobjPersonAccount.LoadPersonAccount();
                                //                if (lobjPersonAccount.ibusPersonAccount.iclbAccountEmploymentDetail == null)
                                //                    lobjPersonAccount.ibusPersonAccount.LoadPersonAccountEmploymentDetails();
                                //                foreach (busPersonAccountEmploymentDetail lobjPAEmpDtl in lobjPersonAccount.ibusPersonAccount.iclbAccountEmploymentDetail)
                                //                {
                                //                    if (lobjPAEmpDtl.ibusEmploymentDetail == null)
                                //                        lobjPAEmpDtl.LoadPersonEmploymentDetail();
                                //                    if (busGlobalFunctions.CheckDateOverlapping(lobjPersonAccount.icdoPersonAccountDependent.start_date,
                                //                                                                lobjPersonAccount.icdoPersonAccountDependent.end_date_no_null,
                                //                                                                lobjPAEmpDtl.ibusEmploymentDetail.icdoPersonEmploymentDetail.start_date,
                                //                                                                lobjPAEmpDtl.ibusEmploymentDetail.icdoPersonEmploymentDetail.end_date_no_null))
                                //                    {
                                //                        if (lobjPAEmpDtl.ibusEmploymentDetail.ibusPersonEmployment == null)
                                //                            lobjPAEmpDtl.ibusEmploymentDetail.LoadPersonEmployment();

                                //                        if (lobjPAEmpDtl.ibusEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.start_date >
                                //                            lobjEmployment.icdoPersonEmployment.start_date)
                                //                        {
                                //                            return 10;
                                //                        }
                                //                    }
                                //                }                                                
                                //            }
                                //        }
                                //    }
                                //}
                            }
                        }
                    }
                    lblnIsPlanEnteredForDependent = true;
                }
            }

            //PIR 11841 - In case of new dependent
            if (iblnFromPortal)
            {

                busPerson lobjPerson1 = new busPerson();
                if (lobjPerson1.FindPerson(icdoPersonDependent.dependent_perslink_id))
                {

                    // 7059 Dependent Person is already enrolled for the same plan with his Employer.
                    lobjPerson1.LoadPersonAccount();
                    foreach (busPersonAccount lobjDependentPersonAccount in lobjPerson1.icolPersonAccount)
                    {
                        if ((lobjDependentPersonAccount.icdoPersonAccount.plan_id == icdoPersonDependent.iintPlanId) &&
                            (busGlobalFunctions.CheckDateOverlapping(icdoPersonDependent.start_date,
                                                                     DateTime.MinValue,
                                                                     lobjDependentPersonAccount.icdoPersonAccount.start_date,
                                                                     lobjDependentPersonAccount.icdoPersonAccount.end_date_no_null)) &&
                            (lobjDependentPersonAccount.icdoPersonAccount.plan_participation_status_value !=
                                                                      busConstant.PlanParticipationStatusInsuranceSuspended))
                        {
                            return 7;
                        }
                    }
                }


            }

            // No Plans entered for the Dependent
            if (!lblnIsPlanEnteredForDependent && !iblnFromPortal)
                return 11;

            if (icdoPersonDependent.relationship_value == busConstant.DependentRelationshipSpouse)
            {
                DataTable ldtbLists = new DataTable();
                int lintPersonDependentID = 0;
                if (iblnFromPortal)
                    lintPersonDependentID = icdoPersonDependent.mss_person_dependent_id;
                else
                    lintPersonDependentID = icdoPersonDependent.person_dependent_id;
                if (icdoPersonDependent.ienuObjectState == ObjectState.Insert)
                {
                    ldtbLists = Select<cdoPersonDependent>(new string[2] { "person_id", "relationship_value" },
                                    new object[2] { icdoPersonDependent.person_id, busConstant.DependentRelationshipSpouse }, null, null);
                }
                else
                {
                    ldtbLists = SelectWithOperator<cdoPersonDependent>(new string[3] { "person_id", "relationship_value", "person_dependent_id" },
                                    new string[3] { "=", "=", "<>" }, new object[3] { icdoPersonDependent.person_id,
                                        busConstant.DependentRelationshipSpouse,lintPersonDependentID }, null);
                }
                // Dependent of relationship 'Spouse' already exists.
                if (ldtbLists.Rows.Count > 0)
                    return 12;

                busPerson lobjPerson = new busPerson();
                lobjPerson.FindPerson(icdoPersonDependent.dependent_perslink_id);
                if (icdoPersonDependent.dependent_perslink_id != 0)
                {
                    if (!(lblnIsEndDated))
                    {
                        if ((lobjPerson.icdoPerson.marital_status_value != busConstant.PersonMaritalStatusMarried) ||
                            (ibusPerson.icdoPerson.marital_status_value != busConstant.PersonMaritalStatusMarried))
                        {
                            // If Relationship is Spouse, both the Person and Dependent Person should be of Maritial Status 'Married'.
                            return 5;
                        }
                    }
                }
                //PIR 10073
                if (icdoPersonDependent.relationship_value == busConstant.DependentRelationshipSpouse)
                {
                    if (((icdoPersonDependent.gender_value == busConstant.GenderTypeFemale) &&
                                            (ibusPerson.icdoPerson.gender_value == busConstant.GenderTypeFemale)) ||
                                            ((icdoPersonDependent.gender_value == busConstant.GenderTypeMale) &&
                                            (ibusPerson.icdoPerson.gender_value == busConstant.GenderTypeMale)))
                    {
                        return 6;
                    }
                }
            }
            if (icdoPersonDependent.ihstOldValues.Count > 0)
            {
                // Relationship cannot be changed to 'Others'.
                if ((Convert.ToString(icdoPersonDependent.ihstOldValues["relationship_value"]) != null) &&
                    (Convert.ToString(icdoPersonDependent.ihstOldValues["relationship_value"]) != busConstant.DependentRelationshipOthers) &&
                    (icdoPersonDependent.relationship_value == busConstant.DependentRelationshipOthers))
                    return 14;
            }
            return 0;
        }

        public bool IsDateOverlapping(int AintPersonAccountID, DateTime AdtGivenDate)
        {
            bool lblnFlag = false;
            busPersonAccount lobjPersonAccount = new busPersonAccount();
            if (lobjPersonAccount.FindPersonAccount(AintPersonAccountID))
            {
                lblnFlag = busGlobalFunctions.CheckDateOverlapping(AdtGivenDate,
                                    lobjPersonAccount.icdoPersonAccount.start_date,
                                    lobjPersonAccount.icdoPersonAccount.end_date);
            }
            return lblnFlag;
        }

        private int _AintValidatedAddress;
        public int AintValidatedAddress
        {
            get { return _AintValidatedAddress; }
            set { _AintValidatedAddress = value; }
        }

        public bool iblnFromPortal { get; set; }

        public int ValidateAddress()
        {
            /// Is Member has Valid Address
            bool IsMemberHasValidAddress = false;
            if (ibusPerson == null)
                LoadPerson();
            ibusPerson.LoadPersonCurrentAddress();
            if (ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.person_address_id != 0)
                IsMemberHasValidAddress = true;

            if (icdoPersonDependent.dependent_perslink_id == 0)
            {
                if ((icdoPersonDependent.date_of_birth == DateTime.MinValue) ||
                    (icdoPersonDependent.first_name == null) ||
                    (icdoPersonDependent.last_name == null) ||
                    (icdoPersonDependent.gender_value == null))
                    return 1;

                if (!IsMemberHasValidAddress && !iblnFromPortal)
                {
                    if (icdoPersonDependent.same_as_member_address != busConstant.Flag_Yes)
                    {
                        if ((icdoPersonDependent.address_line_1 == null) ||
                            (icdoPersonDependent.address_city == null) ||
                            (icdoPersonDependent.address_state_value == null) ||
                            (icdoPersonDependent.address_country_value == null) ||
                            (icdoPersonDependent.address_zip_code == null))
                            return 2; /// Address is info is mandatory.
                    }
                    else
                    {
                        return 5; // Member doesnt have valid address
                    }
                }
                else if (icdoPersonDependent.address_country_value != null && !iblnFromPortal)
                {
                    if (icdoPersonDependent.address_country_value != busConstant.US_Code_ID)
                        if ((icdoPersonDependent.foreign_postal_code == null) || (icdoPersonDependent.foreign_province == null))
                            return 3;
                }

            }
            else
            {
                if (!IsMemberHasValidAddress && !iblnFromPortal)
                    return 5;

                if ((icdoPersonDependent.first_name != null) ||
                    (icdoPersonDependent.last_name != null) ||
                    (icdoPersonDependent.date_of_birth != DateTime.MinValue))
                    return 6;

                if (icdoPersonDependent.same_as_member_address == busConstant.Flag_Yes)
                {
                    if ((icdoPersonDependent.address_line_1 != null) ||
                        (icdoPersonDependent.address_city != null) ||
                        (icdoPersonDependent.address_state_value != null) ||
                        (icdoPersonDependent.address_zip_4_code != null) ||
                        (icdoPersonDependent.address_zip_code != null))
                        return 4;   /// Contact info should not be entered.
                }

                if (icdoPersonDependent.same_as_member_address != busConstant.Flag_Yes && !iblnFromPortal)
                {
                    if (((icdoPersonDependent.address_line_1 == null) ||
                        (icdoPersonDependent.address_city == null) ||
                        (icdoPersonDependent.address_state_value == null) ||
                        (icdoPersonDependent.address_country_value == null) ||
                        (icdoPersonDependent.address_zip_code == null))
                       &&
                        (_ibusDependentPerson.ibusPersonCurrentAddress.icdoPersonAddress.person_address_id == 0))
                        return 2; /// Address is info is mandatory.
                }
            }
            return 0;
        }

        // Used in Correspondence

        private busPersonAccountDependent _ibusPersonAccountDependent;
        public busPersonAccountDependent ibusPeronAccountDependent
        {
            get { return _ibusPersonAccountDependent; }
            set { _ibusPersonAccountDependent = value; }
        }

        public override busBase GetCorPerson()
        {
            if (ibusPerson == null)
                LoadPerson();
            return ibusPerson;
        }

        /// <summary>
        /// Verify Address using Web Service and save after validation.
        /// </summary>
        /// <returns></returns>
        public void VerifyAddressUsingUSPS()
        {
            //If Suppress Warning Flag is Checked, we can skip the Web Service Validation
            if ((icdoPersonDependent.Suppress_Warning == busConstant.Flag_Yes) ||
                (icdoPersonDependent.same_as_member_address == busConstant.Flag_Yes))
            {
                icdoPersonDependent.address_validate_flag = busConstant.Flag_Yes;
                return;
            }
            else
            {
                cdoWebServiceAddress lcdoWebServiceAddress = new cdoWebServiceAddress();
                lcdoWebServiceAddress.addr_line_1 = icdoPersonDependent.address_line_1;
                lcdoWebServiceAddress.addr_line_2 = icdoPersonDependent.address_line_2;
                lcdoWebServiceAddress.addr_city = icdoPersonDependent.address_city;
                lcdoWebServiceAddress.addr_state_value = icdoPersonDependent.address_state_value;
                lcdoWebServiceAddress.addr_zip_code = icdoPersonDependent.address_zip_code;
                lcdoWebServiceAddress.addr_zip_4_code = icdoPersonDependent.address_zip_4_code;

                cdoWebServiceAddress lcdoWebServiceAddressResult = busGlobalFunctions.ValidateWebServiceAddress(lcdoWebServiceAddress);
                if (lcdoWebServiceAddressResult.address_validate_flag != busConstant.Flag_No)
                {
                    icdoPersonDependent.address_line_1 = lcdoWebServiceAddressResult.addr_line_1;
                    icdoPersonDependent.address_line_2 = lcdoWebServiceAddressResult.addr_line_2;
                    icdoPersonDependent.address_city = lcdoWebServiceAddressResult.addr_city;
                    icdoPersonDependent.address_state_value = lcdoWebServiceAddressResult.addr_state_value;
                    icdoPersonDependent.address_zip_code = lcdoWebServiceAddressResult.addr_zip_code;
                    icdoPersonDependent.address_zip_4_code = lcdoWebServiceAddressResult.addr_zip_4_code;
                }
                icdoPersonDependent.address_validate_error = lcdoWebServiceAddressResult.address_validate_error;
                icdoPersonDependent.address_validate_flag = lcdoWebServiceAddressResult.address_validate_flag;
            }
        }

        //This method called when we click Hand Icon. That will reload the object and display the details
        public ArrayList ReloadHandButtonData()
        {
            ArrayList larrList = new ArrayList();
            LoadDependentInfo();
            larrList.Add(this);
            return larrList;
        }

        //this rule is based on 53 ucs
        // to check if the dependent person is deceased
        public bool IsDependentPersonDeceased()
        {
            if (icdoPersonDependent.dependent_perslink_id != 0)
            {
                busDeathNotification lobjDeathNotification = new busDeathNotification();
                if (lobjDeathNotification.FindDeathNotificationByPersonId(icdoPersonDependent.dependent_perslink_id))
                {
                    if (iclbPersonAccountDependent == null)
                        LoadPersonAccountDependent();
                    foreach (busPersonAccountDependent lobjPersonAccountDependent in iclbPersonAccountDependent)
                    {
                        if (busGlobalFunctions.CheckDateOverlapping(lobjDeathNotification.icdoDeathNotification.date_of_death, lobjPersonAccountDependent.icdoPersonAccountDependent.start_date,
                            lobjPersonAccountDependent.icdoPersonAccountDependent.end_date))
                            return true;
                    }
                }
            }
            return false;
        }

        // Systest PIR - 2062
        // check if the gender for spouse entered correctly        
        public bool IsValidGenderSelected()
        {
            if (icdoPersonDependent.gender_value != null)
            {
                if (icdoPersonDependent.relationship_value == busConstant.PersonBeneficiaryRelationshipSpouse)
                {
                    if (icdoPersonDependent.dependent_perslink_id == 0)
                    {
                        if (ibusPerson.icdoPerson.gender_value == icdoPersonDependent.gender_value)
                            return false;
                    }
                    else
                    {
                        if (ibusDependentPerson.IsNull())
                            LoadDependentPerson();
                        if (ibusPerson.icdoPerson.gender_value == ibusDependentPerson.icdoPerson.gender_value)
                            return false;
                    }
                }
            }
            return true;
        }
        /*Prod PIR:4422
         * Similar to validation 10041 it should also warn in PersonDependentMaintenance when a record is linked to a Health enrollment with a Medicare rate: 
         * 'Member is enrolled with a Medicare rate.  If this dependent is Medicare eligible, enter the Medicare Claim Number and Effective Dates.'
         * */
        public bool IsMemberEnrolledInHealthwithMedicare()
        {
            if (ibusPerson.IsNull())
                LoadPerson();

            if (ibusPerson.ibusHealthPersonAccount.IsNull())
                ibusPerson.LoadHealthPersonAccount();

            if (ibusPerson.ibusHealthPersonAccount.IsNotNull())
            {
                if (ibusPerson.ibusHealthPersonAccount.ibusPersonAccountGHDV.IsNull())
                    ibusPerson.ibusHealthPersonAccount.LoadPersonAccountGHDV();

                if (ibusPerson.ibusHealthPersonAccount.ibusPersonAccountGHDV.IsNotNull())
                {
                    if (ibusPerson.ibusHealthPersonAccount.ibusPersonAccountGHDV.iclbCoverageRef.IsNull())
                        ibusPerson.ibusHealthPersonAccount.ibusPersonAccountGHDV.LoadCoverageCodeByFilter();

                    var lenumCoverageCodeList = ibusPerson.ibusHealthPersonAccount.ibusPersonAccountGHDV.iclbCoverageRef.Where(lobjCoverageRef => lobjCoverageRef.coverage_code == ibusPerson.ibusHealthPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.coverage_code);

                    string lstrCoverageCode = string.Empty;
                    if (lenumCoverageCodeList.Count() > 0)
                    {
                        lstrCoverageCode = lenumCoverageCodeList.FirstOrDefault().short_description.ToLower();
                    }

                    if (lstrCoverageCode.IsNotNull())
                    {
                        if (lstrCoverageCode.Contains("medicare") && IsAllMedicareFieldNotEntered())
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool IsAllMedicareFieldNotEntered()
        {
            return (icdoPersonDependent.medicare_claim_no.IsNullOrEmpty()
                                   && icdoPersonDependent.medicare_part_a_effective_date == DateTime.MinValue
                                   && icdoPersonDependent.medicare_part_b_effective_date == DateTime.MinValue);
        }

        //check if the relationship is spouse and both the member is married marital status
        public bool IsRelationshipMarriedSelected()
        {
            if ((icdoPersonDependent.gender_value != null)
                && (icdoPersonDependent.marital_status_value != null))
            {
                if (icdoPersonDependent.dependent_perslink_id == 0)
                {
                    if ((icdoPersonDependent.relationship_value == busConstant.PersonBeneficiaryRelationshipSpouse) &&
                        ((icdoPersonDependent.marital_status_value != busConstant.PersonMaritalStatusMarried)
                        || (ibusPerson.icdoPerson.marital_status_value != busConstant.PersonMaritalStatusMarried)))

                        return true;
                }
            }
            return false;
        }

        /// BCBS file need modified Medicare Info
        private bool IsMedicareInfoModified()
        {
            string lstrOldMedicareClaimNo = null;
            if (icdoPersonDependent.ihstOldValues.Count > 0)
            {
                if (icdoPersonDependent.ihstOldValues["medicare_claim_no"] != null)
                    lstrOldMedicareClaimNo = icdoPersonDependent.ihstOldValues["medicare_claim_no"].ToString();
                DateTime ldtOldPartAEffectiveDate = DateTime.MinValue;
                if (icdoPersonDependent.ihstOldValues["medicare_part_a_effective_date"] != null)
                    ldtOldPartAEffectiveDate = Convert.ToDateTime(icdoPersonDependent.ihstOldValues["medicare_part_a_effective_date"]);

                DateTime ldtOldPartBEffectiveDate = DateTime.MinValue;
                if (icdoPersonDependent.ihstOldValues["medicare_part_b_effective_date"] != null)
                    ldtOldPartBEffectiveDate = Convert.ToDateTime(icdoPersonDependent.ihstOldValues["medicare_part_b_effective_date"]);

                if (icdoPersonDependent.medicare_claim_no != lstrOldMedicareClaimNo ||
                    icdoPersonDependent.medicare_part_a_effective_date != ldtOldPartAEffectiveDate ||
                    icdoPersonDependent.medicare_part_b_effective_date != ldtOldPartBEffectiveDate)
                    return true;
            }
            return false;
        }

        // PROD PIR ID 5660
        public override int Delete()
        {
            // Load all PA Beneficiary
            Collection<busPersonAccountDependent> lclbPADependent = new Collection<busPersonAccountDependent>();
            DataTable ldtbPABene = Select<cdoPersonAccountDependent>(new string[1] { "PERSON_DEPENDENT_ID" },
                                    new object[1] { icdoPersonDependent.person_dependent_id }, null, null);
            lclbPADependent = GetCollection<busPersonAccountDependent>(ldtbPABene, "icdoPersonAccountDependent");

            // Delete Beneficiary
            foreach (busPersonAccountDependent lobjPADependent in lclbPADependent)
                lobjPADependent.icdoPersonAccountDependent.Delete();

            DataTable ldtbDependent = Select<cdoPersonDependent>(new string[1] { "PERSON_DEPENDENT_ID" },
                                    new object[1] { icdoPersonDependent.person_dependent_id }, null, null);
            if (ldtbDependent.Rows.Count > 0)
            {
                busPersonDependent lobjDependent = new busPersonDependent { icdoPersonDependent = new cdoPersonDependent() };
                lobjDependent.icdoPersonDependent.LoadData(ldtbDependent.Rows[0]);
                lobjDependent.icdoPersonDependent.Delete();
            }
            return 1;
        }

        // PIR-8253
        public bool IsValidMedicareClaimNo()
        {
            if (icdoPersonDependent.medicare_claim_no != null)
            {
                Regex lobjexp = new Regex("^[a-zA-Z0-9 ]*$");
                if (!(lobjexp.IsMatch(icdoPersonDependent.medicare_claim_no)))
                    return false;
            }
            return true;
        }

        // PIR 8874 - Medicare Part A/Part B effective date must be first day of a month
        public bool IsMedicareEffectiveDateFirstDayOfMonth()
        {
            if ((icdoPersonDependent.medicare_part_a_effective_date != DateTime.MinValue) || (icdoPersonDependent.medicare_part_b_effective_date != DateTime.MinValue))
            {
                if ((icdoPersonDependent.medicare_part_a_effective_date.Day != 1) || (icdoPersonDependent.medicare_part_b_effective_date.Day != 1))
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsChildDependent()
        {
            if (icdoPersonDependent.IsNotNull() &&
                (icdoPersonDependent.relationship_value == busConstant.DependentRelationshipAdoptiveChild ||
                icdoPersonDependent.relationship_value == busConstant.DependentRelationshipChild ||
                icdoPersonDependent.relationship_value == busConstant.DependentRelationshipDisabledChild ||
                icdoPersonDependent.relationship_value == busConstant.DependentRelationshipGrandChild ||
                icdoPersonDependent.relationship_value == busConstant.DependentRelationshipLegalGuardian ||
                icdoPersonDependent.relationship_value == busConstant.DependentRelationshipStepChild))
                return true;
            return false;
        }

        public bool IsMedicarePartDEnrollmentEnded()
        {
            if (iclbPersonAccountDependent == null)
                LoadPersonAccountDependent();

            foreach (busPersonAccountDependent lobjPersonAcctDependent in iclbPersonAccountDependent)
            {
                if (lobjPersonAcctDependent.icdoPersonAccountDependent.end_date != DateTime.MinValue && lobjPersonAcctDependent.icdoPersonAccountDependent.plan_id == busConstant.PlanIdGroupHealth)
                {
                    busPersonAccountMedicarePartDHistory lobjPersonAcctMedicare = new busPersonAccountMedicarePartDHistory();
                    if (lobjPersonAcctMedicare.FindMedicareByDependentID(icdoPersonDependent.dependent_perslink_id))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /** To Validate Email - PIR-18492**/
        public bool ValidateEmail()
        {
            if (!string.IsNullOrEmpty(icdoPersonDependent.email_address))
            {
                return busGlobalFunctions.IsEmailValid(icdoPersonDependent.email_address);
            }
            return true;
        }
        public bool IsPersonAddressExistOnDependent()
        {
            if (ibusDependentPerson.IsNull())
                LoadDependentPerson();

            busPersonAddress lbusPersonCureentAddress = new busPersonAddress();
            if (ibusPerson.icolPersonAddress.IsNull())
                ibusPerson.LoadAddresses();

            if (ibusPerson.icolPersonAddress.Count > 0)
            {
                lbusPersonCureentAddress = ibusPerson.icolPersonAddress.Where(i => i.icdoPersonAddress.end_date == DateTime.MinValue && i.icdoPersonAddress.undeliverable_address != busConstant.Flag_Yes).FirstOrDefault();
            }
            if (icdoPersonDependent.same_as_member_address == busConstant.Flag_Yes)
            {
                return true;
            }
            else if (icdoPersonDependent.address_line_1.IsNotNullOrEmpty() && icdoPersonDependent.address_city.IsNotNullOrEmpty()
                && icdoPersonDependent.address_state_value.IsNotNullOrEmpty() && icdoPersonDependent.address_country_value.IsNotNullOrEmpty()
                && icdoPersonDependent.address_zip_code.IsNotEmpty())
            {
                return true;
            }
            else if (lbusPersonCureentAddress.IsNotNull() && lbusPersonCureentAddress.icdoPersonAddress.IsNotNull() &&
                lbusPersonCureentAddress.icdoPersonAddress.addr_line_1.IsNotNullOrEmpty() &&
                lbusPersonCureentAddress.icdoPersonAddress.addr_city.IsNotNullOrEmpty() &&
                lbusPersonCureentAddress.icdoPersonAddress.addr_state_value.IsNotNullOrEmpty() &&
                lbusPersonCureentAddress.icdoPersonAddress.addr_country_value.IsNotNullOrEmpty() &&
                lbusPersonCureentAddress.icdoPersonAddress.addr_zip_code.IsNotNullOrEmpty() &&
                lbusPersonCureentAddress.icdoPersonAddress.end_date == DateTime.MinValue &&
                lbusPersonCureentAddress.icdoPersonAddress.undeliverable_address != busConstant.Flag_Yes)
            {
                return true;
            }
            return false;
        }
    }
}
