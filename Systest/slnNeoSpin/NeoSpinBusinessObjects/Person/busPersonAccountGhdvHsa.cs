#region Using directives
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.DataObjects;
using NeoSpin.CustomDataObjects;
using NeoSpin.BusinessObjects;
using Sagitec.CustomDataObjects;
using System.Linq;
#endregion
namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// partial Class NeoSpin.busPersonAccountGhdvHsa:
    /// </summary>
    [Serializable]
    public partial class busPersonAccountGhdvHsa : busPersonAccountGhdv
    {
        /// <summary>
        /// Constructor for NeoSpin.busPersonAccountGhdvHsa
        /// </summary>
        public busPersonAccountGhdvHsa()
        {
        }
        public bool iblnIsFromMSS { get; set; }
        public bool iblnIsFromMSSForACK { get; set; }//PIR 26812
        public string istrAcknowledgementText { get; set; }
        public int iintPersonAccountId { get; set; }
        public int iintPersonAccountGhdvId { get; set; }
        public int iintPersonAccountGhdvHSAId { get; set; }
        public bool iblnIsSaveClick { get; set; }
        public bool iblnIsFromMSSForEnrollmentData { get; set; }

        //busPersonAccountGhdv ibusPersonAccountGhdv = new busPersonAccountGhdv();// code line is not needed
        public Collection<busPersonAccountGhdvHsa> iclbPersonAccountGhdvHsaDetail { get; set; }
        busBase ibusbase = new busBase();
        public string istrMSSAcknowledge { get; set; }
        public string istrConfirmationTextForHSA { get; set; }
        public decimal idecMaxContibutionamount { get; set; }
        public override void ValidateHardErrors(utlPageMode aenmPageMode)
        {
            base.ValidateHardErrors(aenmPageMode);            
            if(this.iarrErrors.Count == 0 && iblnIsFromMSS && icdoPersonAccountGhdvHsa.ienuObjectState != ObjectState.Insert)
            {
                icdoPersonAccountGhdvHsa.ienuObjectState = ObjectState.Insert;
            }
        }        
        public void LoadGHDVHsaByPersonAccountGhdvID()
        {
            if (ibusPersonAccountGhdvHsa == null)
            {
                ibusPersonAccountGhdvHsa = new busPersonAccountGhdvHsa();
                ibusPersonAccountGhdvHsa.icdoPersonAccountGhdvHsa = new doPersonAccountGhdvHsa();
            }
            DataTable ldtResult = Select("cdoPersonAccountGhdvHsa.LoadPersonAccountGHDVHSA", new object[1] { icdoPersonAccountGhdv.person_account_ghdv_id });
            iclbPersonAccountGhdvHsaDetail = ibusbase.GetCollection<busPersonAccountGhdvHsa>(ldtResult, "icdoPersonAccountGhdvHsa");
            if (iclbPersonAccountGhdvHsaDetail != null && iintPersonAccountGhdvHSAId > 0)
            {
                ibusPersonAccountGhdvHsa = iclbPersonAccountGhdvHsaDetail.Where(i => i.icdoPersonAccountGhdvHsa.person_account_ghdv_hsa_id == iintPersonAccountGhdvHSAId).FirstOrDefault();
                icdoPersonAccountGhdvHsa.person_account_ghdv_hsa_id = iintPersonAccountGhdvHSAId;
                icdoPersonAccountGhdvHsa.contribution_start_date = ibusPersonAccountGhdvHsa.icdoPersonAccountGhdvHsa.contribution_start_date;
                icdoPersonAccountGhdvHsa.contribution_amount = ibusPersonAccountGhdvHsa.icdoPersonAccountGhdvHsa.contribution_amount;
            }
        }
        public override int PersistChanges()
        {
             if(iclbPersonAccountGhdvHsaDetail.IsNullOrEmpty())
                LoadGHDVHsaByPersonAccountGhdvID();

            if (ibusPersonAccountGHDV.IsNull())
                LoadPersonAccountGHDV();

            busPersonAccountGhdvHistory lobjGHDVHistory = ibusPersonAccountGHDV?.LoadHistoryByDate(icdoPersonAccountGhdvHsa.contribution_start_date);

            if (iblnIsFromMSS)
            {
                if (icdoPersonAccountGhdvHsa.ienuObjectState == ObjectState.Insert)
                {
                    if (icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth && lobjGHDVHistory?.icdoPersonAccountGhdvHistory?.alternate_structure_code_value == busConstant.AlternateStructureCodeHDHP)
                    {
                        var lenuList = iclbPersonAccountGhdvHsaDetail.Where(i => i.icdoPersonAccountGhdvHsa.contribution_end_date == DateTime.MinValue);
                        if (lenuList.Count() > 0)
                        {
                            foreach (busPersonAccountGhdvHsa lbusPersonAccountGhdvHsa in lenuList)
                            {
                                if (icdoPersonAccountGhdvHsa.contribution_start_date == lbusPersonAccountGhdvHsa.icdoPersonAccountGhdvHsa.contribution_start_date)
                                        lbusPersonAccountGhdvHsa.icdoPersonAccountGhdvHsa.contribution_end_date = icdoPersonAccountGhdvHsa.contribution_start_date;
                                else
                                    lbusPersonAccountGhdvHsa.icdoPersonAccountGhdvHsa.contribution_end_date = icdoPersonAccountGhdvHsa.contribution_start_date.AddDays(-1);
                                lbusPersonAccountGhdvHsa.icdoPersonAccountGhdvHsa.Update();
                            }
                        }
                        icdoPersonAccountGhdvHsa.person_account_id = icdoPersonAccount.person_account_id;
                        icdoPersonAccountGhdvHsa.person_account_ghdv_id = icdoPersonAccountGhdv.person_account_ghdv_id;
                        icdoPersonAccountGhdvHsa.Insert();                        
                    }
                }
                else
                {
                    if (icdoPersonAccountGhdvHsa.person_account_ghdv_hsa_id != 0 && icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth && lobjGHDVHistory?.icdoPersonAccountGhdvHistory?.alternate_structure_code_value == busConstant.AlternateStructureCodeHDHP)
                    {
                        icdoPersonAccountGhdvHsa.person_account_id = icdoPersonAccount.person_account_id;
                        icdoPersonAccountGhdvHsa.person_account_ghdv_id = icdoPersonAccountGhdv.person_account_ghdv_id;
                        icdoPersonAccountGhdvHsa.Update();
                    }
                    if (icdoPersonAccountGhdvHsa.person_account_ghdv_hsa_id == 0 && icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth && lobjGHDVHistory?.icdoPersonAccountGhdvHistory?.alternate_structure_code_value == busConstant.AlternateStructureCodeHDHP)
                    {
                        var lenuList = iclbPersonAccountGhdvHsaDetail.Where(i => i.icdoPersonAccountGhdvHsa.contribution_end_date == DateTime.MinValue);
                        if (lenuList.Count() > 0)
                        {
                            foreach (busPersonAccountGhdvHsa lbusPersonAccountGhdvHsa in lenuList)
                            {
                                lbusPersonAccountGhdvHsa.icdoPersonAccountGhdvHsa.contribution_end_date = icdoPersonAccountGhdvHsa.contribution_start_date.AddDays(-1);
                                lbusPersonAccountGhdvHsa.icdoPersonAccountGhdvHsa.Update();
                            }
                        }
                        icdoPersonAccountGhdvHsa.person_account_id = icdoPersonAccount.person_account_id;
                        icdoPersonAccountGhdvHsa.person_account_ghdv_id = icdoPersonAccountGhdv.person_account_ghdv_id;
                        icdoPersonAccountGhdvHsa.Insert();
                    }
                }
                iblnIsSaveClick = true;
                //PIR 20816 Due to amount and date are reflects old so from here assign newly inserted id to variable
                iintPersonAccountGhdvHSAId = icdoPersonAccountGhdvHsa.person_account_ghdv_hsa_id;
            }
            else
            {
                if (icdoPersonAccountGhdvHsa.ienuObjectState == ObjectState.Insert)
                {
                    icdoPersonAccountGhdvHsa.person_account_id = icdoPersonAccount.person_account_id;
                    icdoPersonAccountGhdvHsa.person_account_ghdv_id = icdoPersonAccountGhdv.person_account_ghdv_id;
                    icdoPersonAccountGhdvHsa.Insert();
                }
                else
                {
                    icdoPersonAccountGhdvHsa.person_account_id = icdoPersonAccount.person_account_id;
                    icdoPersonAccountGhdvHsa.person_account_ghdv_id = icdoPersonAccountGhdv.person_account_ghdv_id;
                    icdoPersonAccountGhdvHsa.Update();
                }
            }
            return 1;
        }
        public override void AfterPersistChanges()
        {
            LoadGHDVHsaByPersonAccountGhdvID();

            if(!iblnIsFromMSSForEnrollmentData)
                InsertIntoEnrollmentDataForHSA();
        }
        //1.	‘Contribution Amount cannot exceed [Amount from Code ID 418, Code Value where Contribution Start Date between Data1 and Data2]’
        public bool IsContributionAmountExceedsExpectedAmount()
        {
                DataTable ldtResult = Select<cdoCodeValue>(new string[1] { "CODE_ID"},
                                                            new object[1] { 418}, null, null);
                if (ldtResult.Rows.Count > 0)
                {
                    foreach (DataRow ldrCodeGroup in ldtResult.Rows)
                    {
                        if (busGlobalFunctions.CheckDateOverlapping(icdoPersonAccountGhdvHsa.contribution_start_date,
                                Convert.ToDateTime(ldrCodeGroup["DATA1"]),
                                (Convert.IsDBNull(ldrCodeGroup["DATA2"]) ? DateTime.MaxValue : Convert.ToDateTime(ldrCodeGroup["DATA2"])))
                                && (icdoPersonAccountGhdvHsa.contribution_amount > Convert.ToDecimal(ldrCodeGroup["DATA3"]))
                            )
                        {
                            idecMaxContibutionamount = Convert.ToDecimal(ldrCodeGroup["DATA3"]);
                            return true;
                        }
                    }
                }
            return false;
        }
                
        //3.	Date must be the first of the month(it always be 1st on month
        public bool IsContributionStartDateIsFirstDayOfMonth()
        {
                return (icdoPersonAccountGhdvHsa.contribution_start_date != DateTime.MinValue && icdoPersonAccountGhdvHsa.contribution_start_date != icdoPersonAccountGhdvHsa.contribution_start_date.GetFirstDayofCurrentMonth()) ? true : false;
        }
        //4.	Dates cannot overlap with existing records
        public bool IsDateOverlap()
        {
            bool lstrResult = false;
            if (icdoPersonAccountGhdvHsa.contribution_start_date != DateTime.MinValue)
            {
                if (iclbPersonAccountGhdvHsaDetail.IsNullOrEmpty())
                    LoadGHDVHsaByPersonAccountGhdvID();

                //cause via MSS End date might be min value and it will going to auto filled by start date -1 hence seprate block for MSS
                if (iblnIsFromMSS && iclbPersonAccountGhdvHsaDetail.Any(hsa => hsa.icdoPersonAccountGhdvHsa.contribution_start_date > icdoPersonAccountGhdvHsa.contribution_start_date))
                    return true;

                foreach (busPersonAccountGhdvHsa lbusPersonAccountGhdvHsa in iclbPersonAccountGhdvHsaDetail)
                {
                    if (lbusPersonAccountGhdvHsa.icdoPersonAccountGhdvHsa.person_account_ghdv_hsa_id != icdoPersonAccountGhdvHsa.person_account_ghdv_hsa_id
                        && icdoPersonAccountGhdvHsa.contribution_start_date != DateTime.MinValue
                        && lbusPersonAccountGhdvHsa.icdoPersonAccountGhdvHsa.contribution_end_date != DateTime.MinValue
                        && lbusPersonAccountGhdvHsa.icdoPersonAccountGhdvHsa.contribution_start_date != lbusPersonAccountGhdvHsa.icdoPersonAccountGhdvHsa.contribution_end_date
                        && busGlobalFunctions.CheckDateOverlapping(icdoPersonAccountGhdvHsa.contribution_start_date, lbusPersonAccountGhdvHsa.icdoPersonAccountGhdvHsa.contribution_start_date, lbusPersonAccountGhdvHsa.icdoPersonAccountGhdvHsa.contribution_end_date))
                    {
                        lstrResult = true;
                        break;
                    }
                }
            }
            return lstrResult;
        }
        //5.	‘Member Contribution Start Date cannot be prior to Employer HSA Effective Date’
        public bool IsContributionStartDatePriorToHSAEffectiveDate()
        {
            if (!iblnIsFromMSS)
            {
                if (icdoPersonAccountGhdvHsa.contribution_start_date != DateTime.MinValue && icdoPersonAccountGhdvHsa.contribution_start_date < icdoPersonAccountGhdv.hsa_effective_date)
                    return true;
            }
            return false;
        }
        //6.	‘Contribution Start Date’ cannot be earlier then the High Deductible Health Plan start date. 
        //PIR 26567 - Skip validation if only Contribution end date is updated
        public bool IsContributionStartDateEarlierToHighDeductibleHealthPlanStartDate()
        {
            bool lblnResult = false;
            if(!iblnIsFromMSS)
            { 
               if (icdoPersonAccountGhdvHsa.ihstOldValues.Count > 0 && Convert.ToDateTime(icdoPersonAccountGhdvHsa.ihstOldValues["contribution_end_date"]) == icdoPersonAccountGhdvHsa.contribution_end_date)
                {
                    DataTable ldtResult = Select<cdoCodeValue>(new string[2] { "CODE_ID", "CODE_VALUE" },
                                                                new object[2] { 418, "HSA1" }, null, null);
                    if (iclbPersonAccountGHDVHistory == null)
                        LoadPersonAccountGHDVHistory();
                    busPersonAccountGhdvHistory lbusPersonAccountGhdvHistory = iclbPersonAccountGHDVHistory
                        .Where(his => his.icdoPersonAccountGhdvHistory.start_date != his.icdoPersonAccountGhdvHistory.end_date)
                        .OrderByDescending(his => his.icdoPersonAccountGhdvHistory.start_date)
                        .FirstOrDefault(his => his.icdoPersonAccountGhdvHistory.alternate_structure_code_value == busConstant.AlternateStructureCodeHDHP);

                    return icdoPersonAccountGhdvHsa.contribution_start_date != DateTime.MinValue && icdoPersonAccountGhdvHsa.contribution_start_date < lbusPersonAccountGhdvHistory.icdoPersonAccountGhdvHistory.start_date;
                }
            }
            return lblnResult;
        }

        //7.	SGT_ORG_PLAN – HSA_PRE_TAX_AGREEMENT has to be Y – ‘Employer does not have HSA Pre Tax Agreement on file.’
        public bool IsHSAPreTaxAgreementOnFile()
        {
            return (!iblnIsFromMSS && (icdoPersonAccountGhdvHsa.contribution_amount >= 0 || icdoPersonAccountGhdvHsa.contribution_start_date != DateTime.MinValue)
                && (ibusOrgPlan.icdoOrgPlan.hsa_pre_tax_agreement == busConstant.Flag_No)) ? true : false;
        }
        //8.	Is start date prior to existing latest record
        public bool IsStartDatePriorToExsitingRecord()
        {
            if (iclbPersonAccountGhdvHsaDetail.IsNull())
                LoadGHDVHsaByPersonAccountGhdvID();            
            if (iclbPersonAccountGhdvHsaDetail.IsNull() || iclbPersonAccountGhdvHsaDetail.Count == 0)
                return false;
            return iblnIsFromMSS && iintPersonAccountGhdvHSAId > 0 && iclbPersonAccountGhdvHsaDetail.Any(hsa => hsa.icdoPersonAccountGhdvHsa.contribution_start_date > icdoPersonAccountGhdvHsa.contribution_start_date);
        }
        public bool IsHsaExistsWIthoutEndDate()
        {
            if (!iblnIsFromMSS)
            {
                if (icdoPersonAccountGhdvHsa.ienuObjectState == ObjectState.Insert)
                {
                    if (iclbPersonAccountGhdvHsaDetail.IsNullOrEmpty())
                        LoadGHDVHsaByPersonAccountGhdvID();
                    return (iclbPersonAccountGhdvHsaDetail.Count() > 0 && (iclbPersonAccountGhdvHsaDetail.Where(i => i.icdoPersonAccountGhdvHsa.contribution_end_date == DateTime.MinValue).Count() > 0)) ? true : false;
                }
            }
            return false;
        }
        public bool FindGHDVHsaByPersonAccountGhdvHsaID(int AintPersonAccountGhdvHsaID)
        {        
            bool lblnResult = false;
            if (icdoPersonAccountGhdvHsa == null)
            {
                icdoPersonAccountGhdvHsa = new doPersonAccountGhdvHsa();
            }
            if (icdoPersonAccountGhdvHsa.SelectRow(new object[1] { AintPersonAccountGhdvHsaID }))
            {
                lblnResult = true;
            }
            return lblnResult;
        }

        public void InsertIntoEnrollmentDataForHSA()
        {
            string lstrWellnessFlag = string.Empty;
            int lintCounter = 0;
            busEnrollmentData lobjEnrollmentData = new busEnrollmentData { icdoEnrollmentData = new doEnrollmentData() };

            busDailyPersonAccountPeopleSoft lobjDailyPAPeopleSoft = new busDailyPersonAccountPeopleSoft();
            lobjDailyPAPeopleSoft.ibusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
            lobjDailyPAPeopleSoft.ibusProvider = new busOrganization { icdoOrganization = new cdoOrganization() };
            lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            lobjDailyPAPeopleSoft.ibusPersonAccountGhdv = new busPersonAccountGhdv { icdoPersonAccountGhdv = new cdoPersonAccountGhdv() };
            lobjDailyPAPeopleSoft.ibusPersonAccountGhdvHSA = new busPersonAccountGhdvHsa { icdoPersonAccountGhdvHsa = new doPersonAccountGhdvHsa() };

            if (lobjDailyPAPeopleSoft.iclbDailyPersonAccountPeopleSoft == null)
                lobjDailyPAPeopleSoft.iclbDailyPersonAccountPeopleSoft = new Collection<busDailyPersonAccountPeopleSoft>();
            
            lobjDailyPAPeopleSoft.ibusPersonAccount.icdoPersonAccount = icdoPersonAccount;

            if(ibusPersonAccountGHDV.IsNull())
                LoadPersonAccountGHDV();

            if (ibusPersonAccountGHDV.ibusHistory.IsNull())
                ibusPersonAccountGHDV.LoadPreviousHistory();
            
            lobjDailyPAPeopleSoft.ibusPersonAccountGhdv.icdoPersonAccountGhdv = ibusPersonAccountGHDV.icdoPersonAccountGhdv;
            lobjDailyPAPeopleSoft.ibusPersonAccountGhdvHSA.icdoPersonAccountGhdvHsa = this.icdoPersonAccountGhdvHsa;

            lobjDailyPAPeopleSoft.LoadPersonEmploymentForPeopleSoft();
            lobjDailyPAPeopleSoft.LoadPeopleSoftOrgGroupValues();
            ibusPersonAccountGHDV.LoadCoverageCodeDescription();
            lobjDailyPAPeopleSoft.idtHSAStartDate = this.icdoPersonAccountGhdvHsa.contribution_start_date;
            lobjDailyPAPeopleSoft.idtHSAEndDate = this.icdoPersonAccountGhdvHsa.contribution_end_date;
            

            if (ibusPerson.IsNull())
                LoadPerson();
            UpdateDefaultEnrollmentDataIfExist();

            //PIR 20481 : hsa health
            //PeopleSoft logic will only be executed if the ORG GROUP of the current Organization is PeopleSoft. 
            if ((lobjDailyPAPeopleSoft.iclbPeopleSoftOrgGroupValue.Where(i => i.code_value == lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.peoplesoft_org_group_value && i.data2 == busConstant.Flag_Yes).Count() > 0))
            {
                if (icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth)
                {
                    if (lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.iclbOrgPlan == null)
                        lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.LoadOrgPlan();

                    lobjDailyPAPeopleSoft.ibusPersonAccount.LoadOrgPlan();
                    lstrWellnessFlag = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusOrgPlan.icdoOrgPlan.wellness_flag;

                    if (lobjDailyPAPeopleSoft.istrEmpTypeValue != busConstant.PersonJobTypeTemporary)

                        lobjDailyPAPeopleSoft.GeneratePeoplesoftEntryForGroupHealthForHSA(lobjDailyPAPeopleSoft.istrEmpTypeValue, ibusPersonAccountGHDV.icdoPersonAccountGhdv.plan_option_value,
                            lstrWellnessFlag, ibusPersonAccountGHDV.istrCoverageCode.Trim() );
                }

                #region Insert Enrollment Data
                if (lobjDailyPAPeopleSoft.iclbDailyPersonAccountPeopleSoft.IsNotNull() && lobjDailyPAPeopleSoft.iclbDailyPersonAccountPeopleSoft.Count > 0)
                {

                    //PIR 20816 update previous records and set flag value to Y
                    UpdatePreviousEnrollmentData();

                    DBFunction.DBNonQuery("cdoPersonAccount.UpdatePrevBenefitEnrollmentRecordHSA", new object[3]
                    { ibusPerson.icdoPerson.person_id, icdoPersonAccountGhdvHsa.person_account_id, icdoPersonAccountGhdvHsa.contribution_start_date},
                                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                    foreach (busDailyPersonAccountPeopleSoft lobjDailyPeopleSoft in lobjDailyPAPeopleSoft.iclbDailyPersonAccountPeopleSoft)
                    {

                        lobjEnrollmentData.icdoEnrollmentData.source_id = icdoPersonAccountGhdvHsa.person_account_ghdv_hsa_id; // PIR 20902
                        lobjEnrollmentData.icdoEnrollmentData.plan_id = busConstant.PlanIdFlex;//icdoPersonAccount.plan_id;
                        lobjEnrollmentData.icdoEnrollmentData.ssn = ibusPerson.icdoPerson.ssn;
                        lobjEnrollmentData.icdoEnrollmentData.ndpers_member_id = ibusPerson.icdoPerson.person_id;
                        lobjEnrollmentData.icdoEnrollmentData.person_account_id = icdoPersonAccount.person_account_id;
                        lobjEnrollmentData.icdoEnrollmentData.peoplesoft_id = ibusPerson.icdoPerson.peoplesoft_id;
                        lobjEnrollmentData.icdoEnrollmentData.employer_org_id = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id;
                        lobjEnrollmentData.icdoEnrollmentData.employment_type_value = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value;
                        lobjEnrollmentData.icdoEnrollmentData.plan_status_value = ibusPersonAccountGHDV.ibusHistory.icdoPersonAccountGhdvHistory.plan_participation_status_value;
                        //PIr 20481-start
						lobjEnrollmentData.icdoEnrollmentData.change_reason_value = string.Empty; 
                        lobjEnrollmentData.icdoEnrollmentData.start_date = this.icdoPersonAccountGhdvHsa.contribution_start_date ;
                        lobjEnrollmentData.icdoEnrollmentData.end_date = this.icdoPersonAccountGhdvHsa.contribution_end_date;
                        if(this.icdoPersonAccountGhdvHsa.contribution_end_date == DateTime.MinValue) //PIr 20481
                            lobjEnrollmentData.icdoEnrollmentData.peoplesoft_change_reason_value = busConstant.HSAStart;
                        else
                            lobjEnrollmentData.icdoEnrollmentData.peoplesoft_change_reason_value = busConstant.HSAEnd;
						
						lobjEnrollmentData.icdoEnrollmentData.coverage_amount = icdoPersonAccountGhdvHsa.contribution_amount;                       
						//PIr 20481-end
						lobjEnrollmentData.icdoEnrollmentData.provider_org_id = ibusPersonAccountGHDV.ibusHistory.icdoPersonAccountGhdvHistory.provider_org_id;
                        lobjEnrollmentData.icdoEnrollmentData.monthly_premium = icdoPersonAccountGhdv.MonthlyPremiumAmount;
                        lobjEnrollmentData.icdoEnrollmentData.pretaxed_premiums = busConstant.Flag_Yes; //PIR  20902   icdoPersonAccountGhdv.premium_conversion_indicator_flag;
                        lobjEnrollmentData.icdoEnrollmentData.peoplesoft_org_group_value = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.peoplesoft_org_group_value;
                        //lobjEnrollmentData.icdoEnrollmentData.peoplesoft_change_reason_value = lobjDailyPAPeopleSoft.istrPSFileChangeEvent; //PIR 23674

                        if (ibusPersonAccountGHDV.ibusHistory.icdoPersonAccountGhdvHistory.alternate_structure_code_value.IsNotNullOrEmpty())
                            lobjEnrollmentData.icdoEnrollmentData.plan_option_value = ibusPersonAccountGHDV.ibusHistory.icdoPersonAccountGhdvHistory.alternate_structure_code_value;
                        else
                            lobjEnrollmentData.icdoEnrollmentData.plan_option_value = ibusPersonAccountGHDV.ibusHistory.icdoPersonAccountGhdvHistory.plan_option_value;

                        if (icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth)
                        {
                            lobjEnrollmentData.icdoEnrollmentData.coverage_code_for_health = ibusPersonAccountGHDV.ibusHistory.icdoPersonAccountGhdvHistory.coverage_code;
                        }
                        else
                            lobjEnrollmentData.icdoEnrollmentData.level_of_coverage_value = ibusPersonAccountGHDV.ibusHistory.icdoPersonAccountGhdvHistory.level_of_coverage_value;

                        lobjEnrollmentData.icdoEnrollmentData.coverage_code = lobjDailyPeopleSoft.istrCoverageCode;
                        lobjEnrollmentData.icdoEnrollmentData.benefit_plan = lobjDailyPeopleSoft.istrBenefitPlan;
                        lobjEnrollmentData.icdoEnrollmentData.deduction_begin_date = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.start_date > lobjDailyPeopleSoft.idtDeductionBeginDate ? lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.start_date : lobjDailyPeopleSoft.idtDeductionBeginDate;// PIR 21267
                        lobjEnrollmentData.icdoEnrollmentData.coverage_begin_date = lobjDailyPeopleSoft.idtCoverageBeginDate;
                        lobjEnrollmentData.icdoEnrollmentData.plan_type = lobjDailyPeopleSoft.istrPlanType;
                        lobjEnrollmentData.icdoEnrollmentData.coverage_election_value = lobjDailyPeopleSoft.istrCoverageElection;
                        lobjEnrollmentData.icdoEnrollmentData.election_date = lobjDailyPeopleSoft.idtElectionDate;
                        lobjEnrollmentData.icdoEnrollmentData.pretax_amount = lobjDailyPeopleSoft.idecFlatAmount;

                        if (((lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date == DateTime.MinValue || (lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date != DateTime.MinValue &&
                            busGlobalFunctions.CheckDateOverlapping(icdoPersonAccount.history_change_date.AddMonths(-1), lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date,
                            new DateTime(lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date.Year, lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date.Month, 1).AddMonths(2)))))
                            && lintCounter == 0)
                            lobjEnrollmentData.icdoEnrollmentData.is_benefit_enrollment_report_generated = busConstant.Flag_No;
                        else
                            lobjEnrollmentData.icdoEnrollmentData.is_benefit_enrollment_report_generated = busConstant.Flag_Yes;

                        lobjEnrollmentData.icdoEnrollmentData.peoplesoft_file_sent_flag = busConstant.Flag_No;
                        //PIR 20902
                        lobjEnrollmentData.icdoEnrollmentData.pretax_amount = icdoPersonAccountGhdvHsa.contribution_amount;
                        lobjEnrollmentData.icdoEnrollmentData.Insert();
                        lintCounter++;
                    }
                }
                #endregion
            }
            else
            {
                #region update data
                UpdatePreviousEnrollmentData();

                DBFunction.DBNonQuery("cdoPersonAccount.UpdatePrevBenefitEnrollmentRecordHSA", new object[3]
                { ibusPerson.icdoPerson.person_id, icdoPersonAccountGhdvHsa.person_account_id, icdoPersonAccountGhdvHsa.contribution_start_date},
                            iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                
                lobjEnrollmentData.icdoEnrollmentData.source_id = icdoPersonAccountGhdvHsa.person_account_ghdv_hsa_id;
                lobjEnrollmentData.icdoEnrollmentData.plan_id = busConstant.PlanIdFlex;
                lobjEnrollmentData.icdoEnrollmentData.ssn = ibusPerson.icdoPerson.ssn;
                lobjEnrollmentData.icdoEnrollmentData.ndpers_member_id = ibusPerson.icdoPerson.person_id;
                lobjEnrollmentData.icdoEnrollmentData.person_account_id = icdoPersonAccountGhdvHsa.person_account_id;
                lobjEnrollmentData.icdoEnrollmentData.peoplesoft_id = ibusPerson.icdoPerson.peoplesoft_id;
                lobjEnrollmentData.icdoEnrollmentData.pay_period_amount = 0.00m;
                lobjEnrollmentData.icdoEnrollmentData.employer_org_id = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id;
                lobjEnrollmentData.icdoEnrollmentData.employment_type_value = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value;
                lobjEnrollmentData.icdoEnrollmentData.plan_status_value = busConstant.PlanParticipationStatusInsuranceEnrolled;
                lobjEnrollmentData.icdoEnrollmentData.change_reason_value = string.Empty;
                lobjEnrollmentData.icdoEnrollmentData.start_date = icdoPersonAccountGhdvHsa.contribution_start_date;
                lobjEnrollmentData.icdoEnrollmentData.end_date = icdoPersonAccountGhdvHsa.contribution_end_date;
                lobjEnrollmentData.icdoEnrollmentData.monthly_premium = 0.00M;
                lobjEnrollmentData.icdoEnrollmentData.pretaxed_premiums = busConstant.Flag_Yes;
                lobjEnrollmentData.icdoEnrollmentData.coverage_amount = icdoPersonAccountGhdvHsa.contribution_amount;
                lobjEnrollmentData.icdoEnrollmentData.plan_option_value = string.Empty;
                lobjEnrollmentData.icdoEnrollmentData.is_benefit_enrollment_report_generated = busConstant.Flag_No;
                lobjEnrollmentData.icdoEnrollmentData.peoplesoft_file_sent_flag = busConstant.Flag_Yes;//PIR 20481
                //PIR20902
                lobjEnrollmentData.icdoEnrollmentData.pretax_amount = icdoPersonAccountGhdvHsa.contribution_amount;
                lobjEnrollmentData.icdoEnrollmentData.Insert();
                #endregion data
            }
        }

        public void UpdatePreviousEnrollmentData()
        {
            Collection<busEnrollmentData> lclbPreviousEnrollmentData = new Collection<busEnrollmentData>();

            DataTable ldtbPreviousEnrollmentData = Select("cdoPersonAccount.LoadPreviousEnrollmentData", new object[2] 
                    { ibusPerson.icdoPerson.person_id, icdoPersonAccountGhdvHsa.person_account_id });

            lclbPreviousEnrollmentData = ibusbase.GetCollection<busEnrollmentData>(ldtbPreviousEnrollmentData, "icdoEnrollmentData");

            if (lclbPreviousEnrollmentData.Count > 0)
            {
                foreach(busEnrollmentData lobjPrevEnrollmentData in lclbPreviousEnrollmentData)
                {
                    if (icdoPersonAccountGhdvHsa.contribution_amount == 0.0m && lobjPrevEnrollmentData.icdoEnrollmentData.coverage_amount == 0.0m)
                        lobjPrevEnrollmentData.icdoEnrollmentData.is_benefit_enrollment_report_generated = busConstant.Flag_Yes;

                    if (lobjPrevEnrollmentData.icdoEnrollmentData.end_date == DateTime.MinValue)
                    {
                        if (lobjPrevEnrollmentData.icdoEnrollmentData.start_date == icdoPersonAccountGhdvHsa.contribution_start_date)
                            lobjPrevEnrollmentData.icdoEnrollmentData.end_date = icdoPersonAccountGhdvHsa.contribution_start_date;
                        else
                            lobjPrevEnrollmentData.icdoEnrollmentData.end_date = icdoPersonAccountGhdvHsa.contribution_start_date.AddDays(-1);
                    }
                    //PIR 20816 check if Date combination already exist but only one is Y and one is N, update N to Y and insert new
                    if (busGlobalFunctions.CheckDateOverlapping(icdoPersonAccountGhdvHsa.contribution_start_date,
                        lobjPrevEnrollmentData.icdoEnrollmentData.start_date,
                        Convert.IsDBNull(lobjPrevEnrollmentData.icdoEnrollmentData.end_date) || lobjPrevEnrollmentData.icdoEnrollmentData.end_date == DateTime.MinValue ? DateTime.MaxValue : lobjPrevEnrollmentData.icdoEnrollmentData.end_date))
                    {
                        lobjPrevEnrollmentData.icdoEnrollmentData.is_benefit_enrollment_report_generated = busConstant.Flag_Yes;
                        lobjPrevEnrollmentData.icdoEnrollmentData.peoplesoft_file_sent_flag = busConstant.Flag_Yes;
                    }
                    lobjPrevEnrollmentData.icdoEnrollmentData.Update();

                }
            }
        }
        public void UpdateDefaultEnrollmentDataIfExist()
        {
            if (icdoPersonAccountGhdvHsa.contribution_end_date == DateTime.MinValue)
            {
                DBFunction.DBNonQuery("cdoPersonAccount.UpdateDefaultHSAEnrollmentDataEntry", new object[3]
                            { ibusPerson.icdoPerson.person_id, icdoPersonAccountGhdvHsa.person_account_id, icdoPersonAccountGhdvHsa.contribution_start_date},
                                                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            }
        }
        public bool IsANNEEnrollInHDHPWithoutHSA()
        {
            DateTime ldtmAnnualEnrollmentDate = Convert.ToDateTime(busGlobalFunctions.GetData3ByCodeValue(52, busConstant.AnnualEnrollment, iobjPassInfo));

            if (ibusPersonAccount.IsNull())
                LoadPersonAccount();
            if (ibusPersonAccount.icdoPersonAccount.person_account_id > 0 && ibusPersonAccountGHDV.IsNull())
                LoadPersonAccountGHDV();
            if (ibusPersonAccountGHDV.iclbPersonAccountGHDVHistory.IsNullOrEmpty())
                ibusPersonAccountGHDV.LoadPersonAccountGHDVHistory();

            busPersonAccountGhdvHistory lobjPAGhdvLatestHistory = new busPersonAccountGhdvHistory { icdoPersonAccountGhdvHistory = new cdoPersonAccountGhdvHistory() };
            busPersonAccountGhdvHistory lobjPAGhdvHistory = new busPersonAccountGhdvHistory { icdoPersonAccountGhdvHistory = new cdoPersonAccountGhdvHistory() };

            if(ibusPersonAccountGHDV.iclbPersonAccountGHDVHistory.IsNotNull() && ibusPersonAccountGHDV.iclbPersonAccountGHDVHistory.Count > 0)
            {
                lobjPAGhdvLatestHistory = ibusPersonAccountGHDV.iclbPersonAccountGHDVHistory.Where(i => i.icdoPersonAccountGhdvHistory.start_date >= ldtmAnnualEnrollmentDate &&
                                                                                                    i.icdoPersonAccountGhdvHistory.reason_value == busConstant.AnnualEnrollment).FirstOrDefault();
                lobjPAGhdvHistory = ibusPersonAccountGHDV.iclbPersonAccountGHDVHistory.Where(i => i.icdoPersonAccountGhdvHistory.start_date < ldtmAnnualEnrollmentDate).FirstOrDefault();
            }

            if (lobjPAGhdvLatestHistory.IsNotNull() && lobjPAGhdvHistory.IsNotNull() &&
                lobjPAGhdvLatestHistory.icdoPersonAccountGhdvHistory.alternate_structure_code_value == busConstant.AlternateStructureCodeHDHP &&
                lobjPAGhdvHistory.icdoPersonAccountGhdvHistory.alternate_structure_code_value.IsNull() &&
                icdoPersonAccountGhdvHsa.contribution_start_date < ibusPersonAccountGHDV.icdoPersonAccountGhdv.hsa_effective_date.AddMonths(1))
                return true;
            return false;
        }

        //PIR 26750
        public bool IsHDHPEnrolledForEffectiveDate()
        {
            if (icdoPersonAccountGhdvHsa.contribution_start_date != DateTime.MinValue)
            {
                if (ibusPersonAccount.IsNull())
                    LoadPersonAccount();
                if (ibusPersonAccount.icdoPersonAccount.person_account_id > 0 && ibusPersonAccountGHDV.IsNull())
                    LoadPersonAccountGHDV();
                if (ibusPersonAccountGHDV.iclbPersonAccountGHDVHistory == null)
                    ibusPersonAccountGHDV.LoadPersonAccountGHDVHistory();

                if (ibusPersonAccountGHDV.iclbPersonAccountGHDVHistory.IsNotNull() && ibusPersonAccountGHDV.iclbPersonAccountGHDVHistory.Count > 0)
                {
                    return !ibusPersonAccountGHDV.iclbPersonAccountGHDVHistory.Where(i => busGlobalFunctions.CheckDateOverlapping(icdoPersonAccountGhdvHsa.contribution_start_date,
                        i.icdoPersonAccountGhdvHistory.start_date, i.icdoPersonAccountGhdvHistory.end_date) && i.icdoPersonAccountGhdvHistory.alternate_structure_code_value == busConstant.AlternateStructureCodeHDHP).Any();
                }
            }
            return false;
        }
    }
}
