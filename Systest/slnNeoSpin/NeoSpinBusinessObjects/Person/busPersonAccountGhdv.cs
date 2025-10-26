#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using Sagitec.CustomDataObjects;
using Sagitec.ExceptionPub;

using NeoSpin.CustomDataObjects;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NeoSpin.DataObjects;
using NeoSpin.BusinessObjects;
using Sagitec.Bpm;
#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPersonAccountGhdv : busPersonAccountGhdvGen
    {
        private string _istrOrgCodeID;
        public string istrOrgCodeID
        {
            get { return _istrOrgCodeID; }
            set { _istrOrgCodeID = value; }
        }

        private Collection<busPersonAccountOtherCoverageDetail> _iclbOtherCoverageDetail;
        public Collection<busPersonAccountOtherCoverageDetail> iclbOtherCoverageDetail
        {
            get { return _iclbOtherCoverageDetail; }
            set { _iclbOtherCoverageDetail = value; }
        }        
        public bool iblnIsActiveMember { get; set; }
        public bool iblnIsCobraMember { get; set; }
        public bool iblnIsDependentCobra { get; set; }
        public bool iblnIsNewHDHPEnrollment { get; set; } //PIR 20902
        public busPaymentElectionAdjustment ibusPaymentElectionAdjustment { get; set; }

        public string CobraType
        {
            get
            {
                return String.IsNullOrEmpty(icdoPersonAccountGhdv.cobra_type_value) ? Convert.ToString(0) : Convert.ToString(1);
            }
        }

        //PIR 11192
        public string istrLowIncomeCreditFlag
        {
            get
            {
                return busGlobalFunctions.GetData1ByCodeValue(52, "LICF", iobjPassInfo);
            }
        }

        public bool iblnIsFromMSS { get; set; } //PIR 10241

        // PIR 9115 functionality enable/disable property
        public string istrIsPIR9115Enabled
        {
            get
            {
                return busGlobalFunctions.GetData1ByCodeValue(52, "9115", iobjPassInfo);
            }
        }

        public string WellnessFlag
        {
            get
            {
                string lstrWellnessFlag = "0";
                //For Retiree Enrollment, Wellness Flag should be always Zero. When the member retires, he has the option of choose COBRA
                //or Retiree. in such case, ibusOrgPlan object will be loaded. so, if he choose Retiree, we shouldnt set the wellness even
                //if org plan provides the wellness
                if (icdoPersonAccountGhdv.health_insurance_type_value != busConstant.HealthInsuranceTypeRetiree)
                {
                    if ((ibusOrgPlan != null) && (ibusOrgPlan.icdoOrgPlan != null))
                    {
                        lstrWellnessFlag = ibusOrgPlan.icdoOrgPlan.wellness_flag == busConstant.Flag_Yes ? Convert.ToString(1) : Convert.ToString(0);
                    }
                }
                return lstrWellnessFlag;
            }
        }

        public string istrAllowOverlapHistory { get; set; }
        //PIR 7705
        public string istrHSATerminationDate { get; set; }
        public string istrCoverageCodeDescription { get; set; }
        //Mail from RAJ dated on 11/18/2009 - Make Health Plan Option Read Only on screen and load only for State / NonState
        //For Retiree, it should be blank.
        public void LoadHealthPlanOption()
        {
            if (icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeRetiree)
            {
                icdoPersonAccountGhdv.plan_option_value = null;
            }
            else
            {
                if ((ibusOrgPlan != null) && (ibusOrgPlan.icdoOrgPlan != null) &&
                    ibusOrgPlan.icdoOrgPlan.org_plan_id != 0) // PROD PIR 5459
                {
                    icdoPersonAccountGhdv.plan_option_value = ibusOrgPlan.icdoOrgPlan.plan_option_value;
                }
            }
            LoadHealthPlanOptionDescription();
        }

        public void LoadHealthPlanOptionDescription()
        {
            icdoPersonAccountGhdv.plan_option_description =
                iobjPassInfo.isrvDBCache.GetCodeDescriptionString(406, icdoPersonAccountGhdv.plan_option_value);
        }

        public string LowIncome
        {
            get
            {
                if (icdoPersonAccountGhdv.low_income_credit != 0)
                    return "1";
                return "0";
            }
        }

        public string COBRAIn
        {
            get
            {
                if (!String.IsNullOrEmpty(icdoPersonAccountGhdv.cobra_type_value))
                    return "1";
                return "0";
            }
        }

        public bool IsHealthOrMedicare
        {
            get
            {
                if (ibusPlan == null)
                    LoadPlan();

                if ((ibusPlan.icdoPlan.plan_id == busConstant.PlanIdGroupHealth) || (ibusPlan.icdoPlan.plan_id == busConstant.PlanIdMedicarePartD))
                    return true;

                return false;
            }
        }

        #region COBRA Coverage Termination Batch fields

        public decimal idecCurrentMonthlyPremium { get; set; }
        public decimal idecCurrentMonthlyRhic { get; set; }
        public decimal idecCurrentNetMonthlyPrimium
        {
            get
            {
                return idecCurrentMonthlyPremium - idecCurrentMonthlyRhic;
            }
        }

        public decimal idecProjectedMonthlyPremium { get; set; }
        public decimal idecProjectedNetMonthlyPremium
        {
            get
            {
                return idecProjectedMonthlyPremium - idecCurrentMonthlyRhic;
            }
        }

        public string istrPriorMonthOfCobraExpirationDate
        {
            get
            {
                if (icdoPersonAccount.cobra_expiration_date != DateTime.MinValue)
                {
                    //prod pir 6204
                    if (icdoPersonAccount.cobra_expiration_date.Day <= 15)
                        return new DateTime(icdoPersonAccount.cobra_expiration_date.AddMonths(-1).Year,
                                                            icdoPersonAccount.cobra_expiration_date.AddMonths(-1).Month, 15).ToString("MMMM dd, yyyy");
                    else
                        return new DateTime(icdoPersonAccount.cobra_expiration_date.Year,
                                                            icdoPersonAccount.cobra_expiration_date.Month, 15).ToString("MMMM dd, yyyy");
                }
                return string.Empty;
            }
        }

        public bool IsHealthandPayee
        {
            get
            {
                bool lblnReturn = false; //PIR 8518
                if (ibusPlan.icdoPlan.plan_id == busConstant.PlanIdGroupHealth)
                {
                    if (ibusPerson.IsNull()) LoadPerson();
                    if (ibusPerson.iclbPayeeAccount.IsNull()) ibusPerson.LoadPayeeAccount(true);
                    //PROD PIR 7084
                    //as per discussion with Maik 8July2011, if person does not have a payee account or have a payee account in payments compelete,
                    //processed or cancelled then property should be true
                    if (ibusPerson.iclbPayeeAccount.Where(o => o.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 != busConstant.PayeeAccountStatusPaymentComplete &&
                        o.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 != busConstant.PayeeAccountStatusRefundProcessed &&
                        o.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 != busConstant.PayeeAccountStatusRefundCancelled &&
                        o.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 != busConstant.PayeeAccountStatusCancelled).Any())
                    {
                        lblnReturn = true;
                    }

                    //Start PIR 8518
                    //Payee Section will  be seen only when person is payee And person is a retiree cobra.
                    if (ibusPerson.iclbPayeeAccount.Count > 0 &&
                        (icdoPersonAccountGhdv.CobraTypeValue == busConstant.Retiree_18_Month_COBRA ||
                        icdoPersonAccountGhdv.CobraTypeValue == busConstant.Retiree_36_Month_COBRA ||
                        icdoPersonAccountGhdv.CobraTypeValue == busConstant.Retiree_Disability_COBRA))
                    {
                        lblnReturn = true;
                    }
                    //End PIR 8518
                }
                return lblnReturn; //PIR 8518
            }
        }

        public bool IsHealthDentalorVisionandNoPayee
        {
            get
            {
                if ((ibusPlan.icdoPlan.plan_id == busConstant.PlanIdVision) ||
                    (ibusPlan.icdoPlan.plan_id == busConstant.PlanIdDental) ||
                    (ibusPlan.icdoPlan.plan_id == busConstant.PlanIdGroupHealth))
                {
                    if (ibusPerson.IsNull()) LoadPerson();
                    if (ibusPerson.iclbPayeeAccount.IsNull()) ibusPerson.LoadPayeeAccount(true);
                    //PROD PIR 7084
                    //as per discussion with Maik 8July2011, if person does not have a payee account or have a payee account in payments compelete,
                    //processed or cancelled then property should be true
                    if (ibusPerson.iclbPayeeAccount.Count == 0 ||
                        !ibusPerson.iclbPayeeAccount.Where(o => o.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 != busConstant.PayeeAccountStatusPaymentComplete &&
                                o.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 != busConstant.PayeeAccountStatusRefundProcessed &&
                                o.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 != busConstant.PayeeAccountStatusRefundCancelled &&
                                o.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 != busConstant.PayeeAccountStatusCancelled).Any())
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        // PROD PIR 7706
        public bool IsHealthorDental
        {
            get
            {
                if (ibusPlan.icdoPlan.plan_id == busConstant.PlanIdDental ||
                    ibusPlan.icdoPlan.plan_id == busConstant.PlanIdGroupHealth)
                {
                    return true;
                }
                return false;
            }
        }

        #endregion

        private busPersonAccountGhdvHistory _ibusHistory;
        public busPersonAccountGhdvHistory ibusHistory
        {
            get
            {
                return _ibusHistory;
            }
            set
            {
                _ibusHistory = value;
            }
        }

        private busPersonAccountGhdvHistory _ibusEnrollmentHistory;
        public busPersonAccountGhdvHistory ibusEnrollmentHistory
        {
            get
            {
                return _ibusEnrollmentHistory;
            }
            set
            {
                _ibusEnrollmentHistory = value;
            }
        }

        private ObjectState lobjCurrentObjectState;
        public DataTable idtbCachedLowIncomeCredit { get; set; }
        public DataTable idtbCachedRateRef { get; set; }
        public DataTable idtbCachedRateStructureRef { get; set; }
        public DataTable idtbCachedCoverageRef { get; set; }
        public DataTable idtbCachedHealthRate { get; set; }
        public DataTable idtbCachedDentalRate { get; set; }
        public DataTable idtbCachedVisionRate { get; set; }
        public DataTable idtbCachedHmoRate { get; set; }
        public int iintOldPayeeAccountID { get; set; }
        public busPersonAccountGhdv ibusMemberGHDVForDependent { get; set; }

        public bool iblnIsFromInternal { get; set; }//PIR 17962 
        public bool iblnIsRetireeToActive { get; set; }//PIR 18155

        // Loading the Other Coverage Grid.
        public void LoadOtherCoverageDetails()
        {
            if (_iclbOtherCoverageDetail == null)
                _iclbOtherCoverageDetail = new Collection<busPersonAccountOtherCoverageDetail>();
            DataTable ldtbOtherCoverage = Select<cdoPersonAccountOtherCoverageDetail>(
                                    new string[1] { "person_account_id" },
                                    new object[1] { icdoPersonAccount.person_account_id }, null, null);
            _iclbOtherCoverageDetail = GetCollection<busPersonAccountOtherCoverageDetail>(ldtbOtherCoverage, "icdoPersonAccountOtherCoverageDetail");
            foreach (busPersonAccountOtherCoverageDetail lobjOtherCoverage in _iclbOtherCoverageDetail)
                lobjOtherCoverage.LoadProviderName();
        }

        private Collection<busPersonAccountWorkerCompensation> _iclbWorkersCompensation;
        public Collection<busPersonAccountWorkerCompensation> iclbWorkersCompensation
        {
            get { return _iclbWorkersCompensation; }
            set { _iclbWorkersCompensation = value; }
        }

        // Loading the Workers Compensation Grid.
        public void LoadWorkersCompensation()
        {
            if (_iclbWorkersCompensation == null)
                _iclbWorkersCompensation = new Collection<busPersonAccountWorkerCompensation>();
            DataTable ldtbWorkersCompensation = Select<cdoPersonAccountWorkerCompensation>(
                                                new string[1] { "person_account_id" },
                                                new object[1] { icdoPersonAccount.person_account_id }, null, null);
            _iclbWorkersCompensation = GetCollection<busPersonAccountWorkerCompensation>(ldtbWorkersCompensation, "icdoPersonAccountWorkerCompensation");
            foreach (busPersonAccountWorkerCompensation lobjWorkersCompensation in _iclbWorkersCompensation)
                lobjWorkersCompensation.LoadProviderName();
        }

        public Collection<busPersonAccountGhdvHistory> iclbOverlappingHistory { get; set; }
        private Collection<busPersonAccountGhdvHistory> LoadOverlappingHistory()
        {
            if (iclbPersonAccountGHDVHistory == null)
                LoadPersonAccountGHDVHistory();
            Collection<busPersonAccountGhdvHistory> lclbPAGHDVHistory = new Collection<busPersonAccountGhdvHistory>();
            var lenuList = iclbPersonAccountGHDVHistory.Where(i => busGlobalFunctions.CheckDateOverlapping(icdoPersonAccount.history_change_date,
                i.icdoPersonAccountGhdvHistory.start_date, i.icdoPersonAccountGhdvHistory.end_date)
                || i.icdoPersonAccountGhdvHistory.start_date > icdoPersonAccount.history_change_date);
            foreach (busPersonAccountGhdvHistory lobjHistory in lenuList)
            {
                if (lobjHistory.icdoPersonAccountGhdvHistory.start_date >= icdoPersonAccount.history_change_date)
                {
                    lclbPAGHDVHistory.Add(lobjHistory);
                }
                else if (lobjHistory.icdoPersonAccountGhdvHistory.start_date == lobjHistory.icdoPersonAccountGhdvHistory.end_date)
                {
                    lclbPAGHDVHistory.Add(lobjHistory);
                }
                else if (lobjHistory.icdoPersonAccountGhdvHistory.start_date != lobjHistory.icdoPersonAccountGhdvHistory.end_date)
                {
                    break;
                }

            }
            return lclbPAGHDVHistory;
        }
		// PIR 6919
        public string istrEnrolledPlanName { get; set; }
        public decimal idecPremiumAmount { get; set; }
        public Collection<busPersonAccountGhdv> iclbEnrolledPlanAndPremium { get; set; }
        public void LoadEnrolledPlanAndPremiumForPersonAccountGHDV()
        {
            DataTable ldtEnrolledPlanAndPremiumForPersonAccountGHDV = Select("cdoPersonAccount.LoadEnrolledPlanAndPremiumForPersonAccountGHDV", new object[2] { icdoPersonAccount.person_id, DateTime.Now });
            iclbEnrolledPlanAndPremium = new Collection<busPersonAccountGhdv>();
            if (ldtEnrolledPlanAndPremiumForPersonAccountGHDV.Rows.Count > 0)
            {

                foreach (DataRow aEnrolledPlanAndPremiumRow in ldtEnrolledPlanAndPremiumForPersonAccountGHDV.Rows)
                {
                    busPersonAccountGhdv lobjPersonAccountGhdv = new busPersonAccountGhdv
                    {
                        icdoPersonAccountGhdv = new cdoPersonAccountGhdv(),
                        icdoPersonAccount = new cdoPersonAccount()
                    };
                    lobjPersonAccountGhdv.icdoPersonAccountGhdv.LoadData(aEnrolledPlanAndPremiumRow);
                    lobjPersonAccountGhdv.icdoPersonAccount.LoadData(aEnrolledPlanAndPremiumRow);
                    lobjPersonAccountGhdv.LoadPersonAccount();
                    lobjPersonAccountGhdv.LoadPlan();
                    if ((lobjPersonAccountGhdv.istrIsPlanHealth == busConstant.Flag_Yes && lobjPersonAccountGhdv.icdoPersonAccountGhdv.health_insurance_type_value != busConstant.HealthInsuranceTypeRetiree)
                        || (lobjPersonAccountGhdv.istrIsPlanDental == busConstant.Flag_Yes && lobjPersonAccountGhdv.icdoPersonAccountGhdv.dental_insurance_type_value != busConstant.DentalInsuranceTypeRetiree)
                        || (lobjPersonAccountGhdv.istrIsPlanVision == busConstant.Flag_Yes && lobjPersonAccountGhdv.icdoPersonAccountGhdv.vision_insurance_type_value != busConstant.VisionInsuranceTypeRetiree))
                    {
                        lobjPersonAccountGhdv.LoadPlanEffectiveDate();
                        if (lobjPersonAccountGhdv.icdoPersonAccount.history_change_date != DateTime.MinValue)
                            lobjPersonAccountGhdv.idtPlanEffectiveDate = lobjPersonAccountGhdv.icdoPersonAccount.history_change_date; //PIR 20415
                        lobjPersonAccountGhdv.DetermineEnrollmentAndLoadObjects(lobjPersonAccountGhdv.idtPlanEffectiveDate, false);
                        if (lobjPersonAccountGhdv.icdoPersonAccount.plan_id == busConstant.PlanIdDental)
                            lobjPersonAccountGhdv.icdoPersonAccountGhdv.dental_insurance_type_value = busConstant.DentalInsuranceTypeCOBRA;
                        else if (lobjPersonAccountGhdv.icdoPersonAccount.plan_id == busConstant.PlanIdVision)
                            lobjPersonAccountGhdv.icdoPersonAccountGhdv.vision_insurance_type_value = busConstant.VisionInsuranceTypeCOBRA;                        
                        else
                        {
                            lobjPersonAccountGhdv.GetMonthlyPremiumAmount();
                        }
                    }
                    else
                    {
                        lobjPersonAccountGhdv.LoadActiveProviderOrgPlan(DateTime.Now);
                    }
                    if (lobjPersonAccountGhdv.IsHealthOrMedicare)
                    {                        
                        if (lobjPersonAccountGhdv.IsCoverageCodeCodeSingle())
                            lobjPersonAccountGhdv.icdoPersonAccountGhdv.coverage_code = "0004";
                        else
                            lobjPersonAccountGhdv.icdoPersonAccountGhdv.coverage_code = "0005";

                        lobjPersonAccountGhdv.icdoPersonAccountGhdv.cobra_type_value = busConstant.COBRAType18Month;                        
                        lobjPersonAccountGhdv.DetermineEnrollmentAndLoadObjects(lobjPersonAccountGhdv.icdoPersonAccount.current_plan_start_date_no_null, false);//15752
                        if (lobjPersonAccountGhdv.icdoPersonAccountGhdv.overridden_structure_code.IsNotNullOrEmpty())
                        {
                            lobjPersonAccountGhdv.LoadRateStructureForUserStructureCode();
                        }
                        else
                        {

                            lobjPersonAccountGhdv.LoadHealthParticipationDate();

                            lobjPersonAccountGhdv.LoadRateStructure();
                        }

                        lobjPersonAccountGhdv.LoadCoverageRefID();
                        lobjPersonAccountGhdv.GetMonthlyPremiumAmountByRefID();
                    }
                    else
                    {
                        lobjPersonAccountGhdv.GetMonthlyPremiumAmount();
                    }
                    switch (lobjPersonAccountGhdv.icdoPersonAccount.plan_id)
                    {
                        case busConstant.PlanIdGroupHealth:
                            busPersonAccountGhdv lbusHealthPersonAccountGhdv = new busPersonAccountGhdv();
                            lbusHealthPersonAccountGhdv.istrEnrolledPlanName = "Health";
                            lbusHealthPersonAccountGhdv.idecPremiumAmount = lobjPersonAccountGhdv.icdoPersonAccountGhdv.MonthlyPremiumAmount;

                            lobjPersonAccountGhdv.LoadEnrolledCoverageDateForGHDV();
                            lbusHealthPersonAccountGhdv.idtEnrolledCoverageEndDate = lobjPersonAccountGhdv.idtEnrolledCoverageEndDate;
                            
                            iclbEnrolledPlanAndPremium.Add(lbusHealthPersonAccountGhdv);
                            break;
                        case busConstant.PlanIdDental:
                            busPersonAccountGhdv lbusDentalPersonAccountGhdv = new busPersonAccountGhdv();
                            lbusDentalPersonAccountGhdv.istrEnrolledPlanName = "Dental";
                            lbusDentalPersonAccountGhdv.idecPremiumAmount = lobjPersonAccountGhdv.icdoPersonAccountGhdv.MonthlyPremiumAmount;

                            lobjPersonAccountGhdv.LoadEnrolledCoverageDateForGHDV();
                            lbusDentalPersonAccountGhdv.idtEnrolledCoverageEndDate = lobjPersonAccountGhdv.idtEnrolledCoverageEndDate;
                            
                            iclbEnrolledPlanAndPremium.Add(lbusDentalPersonAccountGhdv);
                            break;
                        case busConstant.PlanIdVision:
                            busPersonAccountGhdv lbusVisionPersonAccountGhdv = new busPersonAccountGhdv();
                            lbusVisionPersonAccountGhdv.istrEnrolledPlanName = "Vision";
                            lbusVisionPersonAccountGhdv.idecPremiumAmount = lobjPersonAccountGhdv.icdoPersonAccountGhdv.MonthlyPremiumAmount;

                            lobjPersonAccountGhdv.LoadEnrolledCoverageDateForGHDV();
                            lbusVisionPersonAccountGhdv.idtEnrolledCoverageEndDate = lobjPersonAccountGhdv.idtEnrolledCoverageEndDate;
                                                        
                            iclbEnrolledPlanAndPremium.Add(lbusVisionPersonAccountGhdv);
                            break;
                    }
                }
            }
        }
        public bool IsMoreThanOneEnrolledInOverlapHistory()
        {
            if (istrAllowOverlapHistory == busConstant.Flag_Yes)
            {
                if (iclbOverlappingHistory != null)
                {
                    var lenuList = iclbOverlappingHistory.Where(i => i.icdoPersonAccountGhdvHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled &&
                        i.icdoPersonAccountGhdvHistory.start_date != i.icdoPersonAccountGhdvHistory.end_date);
                    //PIR Enhanced Overlap - 23167, 23340, 23408
                    if (IsResourceForEnhancedOverlap(busConstant.OverlapResourceEnhancedGHDV))
                    {
                        if ((lenuList != null) && (lenuList.Count() > 2))
                            return true;
                    }
                    else
                    {
                        if ((lenuList != null) && (lenuList.Count() > 1))
                            return true;
                    }  
                }
            }
            return false;
        }
        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            // PIR 10256
            if (!iblnIsFromTerminationPost && icdoPersonAccountGhdv.reason_value == busConstant.AnnualEnrollment)
            {
                if (icdoPersonAccount.person_account_id == 0)
                    icdoPersonAccount.start_date = Convert.ToDateTime(busGlobalFunctions.GetData3ByCodeValue(52, busConstant.AnnualEnrollment, iobjPassInfo));
                if (icdoPersonAccount.person_account_id > 0)
                    icdoPersonAccount.history_change_date = Convert.ToDateTime(busGlobalFunctions.GetData3ByCodeValue(52, busConstant.AnnualEnrollment, iobjPassInfo));
            }
         
            lobjCurrentObjectState = icdoPersonAccountGhdv.ienuObjectState;

            //PROD Logic changes. - Delete the Previous open History lines if the Allow Overlap Sets true
            //Reload the History Always... If the user check the overlap first and some error came and then uncheck the overlap checkbox leads to issue.
            //so, we must reload the history all the time
            LoadPersonAccountGHDVHistory();
            //PIR 20902 //PIR 21077
            if (icdoPersonAccountGhdv.alternate_structure_code_value == busConstant.AlternateStructureCodeHDHP &&
                (
                  icdoPersonAccount.person_account_id == 0 ||
                 iclbPersonAccountGHDVHistory.Where(m => m.icdoPersonAccountGhdvHistory.alternate_structure_code_value == busConstant.AlternateStructureCodeHDHP).Count() == 0)
                )
                iblnIsNewHDHPEnrollment = true;
            else iblnIsNewHDHPEnrollment = false;
            iclbOverlappingHistory = new Collection<busPersonAccountGhdvHistory>();
            if ((istrAllowOverlapHistory == busConstant.Flag_Yes) && (icdoPersonAccount.history_change_date != DateTime.MinValue))
            {
                Collection<busPersonAccountGhdvHistory> lclbOpenHistory = LoadOverlappingHistory();
                if (lclbOpenHistory.Count > 0)
                {
                    foreach (busPersonAccountGhdvHistory lbusPAGhdvHistory in lclbOpenHistory)
                    {
                        iclbPersonAccountGHDVHistory.Remove(lbusPAGhdvHistory);
                        iclbOverlappingHistory.Add(lbusPAGhdvHistory);
                    }
                }
            }

            LoadPreviousHistory();

            if (icdoPersonAccount.ienuObjectState == ObjectState.Insert)
                iblnIsNewMode = true;

            if (icdoPersonAccount.suppress_warnings_flag == busConstant.Flag_Yes)
                icdoPersonAccount.suppress_warnings_by = iobjPassInfo.istrUserID;

            if (istrOrgCodeID != null)
                icdoPersonAccountGhdv.epo_org_id = busGlobalFunctions.GetOrgIdFromOrgCode(istrOrgCodeID);

            if (ibusPaymentElection == null)
                LoadPaymentElection();

            if (!String.IsNullOrEmpty(ibusPaymentElection.icdoPersonAccountPaymentElection.Billing_Organization))
            {
                ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id =
                    busGlobalFunctions.GetOrgIdFromOrgCode(ibusPaymentElection.icdoPersonAccountPaymentElection.Billing_Organization);
            }
            else
            {
                ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id = 0;
            }

            if (!String.IsNullOrEmpty(ibusPaymentElection.icdoPersonAccountPaymentElection.Supplemental_Billing_Organization))
            {
                ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_supplemental_org_id =
                    busGlobalFunctions.GetOrgIdFromOrgCode(ibusPaymentElection.icdoPersonAccountPaymentElection.Supplemental_Billing_Organization);
            }
            else
            {
                ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_supplemental_org_id = 0;
            }

            //Reload Plan Effective Date
            LoadPlanEffectiveDate();

            //Determine the Enrollment Type based on the History Change Date Given (This is useful for Future End dated Employment Records)
            //PIR 12986 -- If type is not retiree then load employer org plan and if type is retiree then load provider org plan.
            if (lobjCurrentObjectState == ObjectState.Insert)
                DetermineEnrollmentAndLoadObjects(icdoPersonAccount.current_plan_start_date_no_null, true);
            else
            {
                if ((istrIsPlanHealth == busConstant.Flag_Yes && icdoPersonAccountGhdv.health_insurance_type_value != busConstant.HealthInsuranceTypeRetiree)
                        || (istrIsPlanDental == busConstant.Flag_Yes && icdoPersonAccountGhdv.dental_insurance_type_value != busConstant.DentalInsuranceTypeRetiree)
                        || (istrIsPlanVision == busConstant.Flag_Yes && icdoPersonAccountGhdv.vision_insurance_type_value != busConstant.VisionInsuranceTypeRetiree))
                {
                    DetermineEnrollmentAndLoadObjects(icdoPersonAccount.current_plan_start_date_no_null, false);
                }
                else
                {
                    LoadActiveProviderOrgPlan(icdoPersonAccount.current_plan_start_date_no_null);
                }
            }

            //PIR 23883 - Reload function to check if member is active or retiree
            if (icdoPersonAccountGhdv.dental_insurance_type_value == busConstant.DentalInsuranceTypeRetiree ||
                icdoPersonAccountGhdv.vision_insurance_type_value == busConstant.VisionInsuranceTypeRetiree)
            {
                DetermineEnrollmentAndLoadObjects(icdoPersonAccount.current_plan_start_date_no_null, false);
            }


            //prod pir 7220
            if (icdoPersonAccountGhdv.ihstOldValues.Count > 0)
            {
                istrPreviousCOBRATypeValue = icdoPersonAccountGhdv.ihstOldValues["cobra_type_value"] == DBNull.Value ?
                    null : Convert.ToString(icdoPersonAccountGhdv.ihstOldValues["cobra_type_value"]);
            }

            // UAT PIR ID 1069 - Overridden Structure Code should be changed if the Values are changed.
            if (!string.IsNullOrEmpty(icdoPersonAccountGhdv.overridden_structure_code))
            {
                if (icdoPersonAccountGhdv.ihstOldValues.Count > 0)
                {
                    string lstrOldHealthInsuranceType = null;
                    if (icdoPersonAccountGhdv.ihstOldValues["health_insurance_type_value"] != null)
                        lstrOldHealthInsuranceType = icdoPersonAccountGhdv.ihstOldValues["health_insurance_type_value"].ToString();

                    string lstrOldPlanOptionValue = null;
                    if (icdoPersonAccountGhdv.ihstOldValues["plan_option_value"] != null)
                        lstrOldPlanOptionValue = icdoPersonAccountGhdv.ihstOldValues["plan_option_value"].ToString();

                    // PROD PIR ID 5143
                    string lstrOldCoverageCode = null;
                    if (icdoPersonAccountGhdv.ihstOldValues["coverage_code"] != null)
                        lstrOldCoverageCode = icdoPersonAccountGhdv.ihstOldValues["coverage_code"].ToString();

                    if (string.IsNullOrEmpty(icdoPersonAccountGhdv.cobra_type_value) && ((lstrOldHealthInsuranceType != icdoPersonAccountGhdv.health_insurance_type_value) ||
                        (lstrOldPlanOptionValue != icdoPersonAccountGhdv.plan_option_value) ||
                        (Convert.ToDecimal(icdoPersonAccountGhdv.ihstOldValues["low_income_credit"]) != icdoPersonAccountGhdv.low_income_credit) ||
                        (lstrOldCoverageCode != icdoPersonAccountGhdv.coverage_code)))
                        icdoPersonAccountGhdv.overridden_structure_code = string.Empty;
                }
            }

            //Load the Rate Structure
            if (IsHealthOrMedicare)
            {
                if (icdoPersonAccountGhdv.overridden_structure_code.IsNotNullOrEmpty())
                {
                    LoadRateStructureForUserStructureCode();
                }
                else
                {
                    LoadHealthParticipationDate();
                    LoadHealthPlanOption();
                    LoadRateStructure();
                }

                if (icdoPersonAccountGhdv.Rate_Ref_ID > 0)
                {
                    LoadCoverageRefID();
                }
                else
                {
                    icdoPersonAccountGhdv.Coverage_Ref_ID = 0;
                }
            }
            if (ibusHistory == null)
                LoadPreviousHistory();

            SetHistoryEntryRequiredOrNot();

            _AintValidateMemberEnrollment = ValidateMemberEnrollmentAfterLOA();

            //Function to set bool variable based on any value change in Payment election
            //SetPaymentElectionChangedOrNot();

            if ((iblnIsCobraMember) || (iblnIsDependentCobra))
            {
                //PIR 1422 : Set the Cobra Date only when cobra type gets changed / or first time cobra type selected.
                if (!String.IsNullOrEmpty(icdoPersonAccountGhdv.cobra_type_value))
                {
                    if (icdoPersonAccountGhdv.ienuObjectState == ObjectState.Insert)
                    {
                        if (!IsMedicareEligibleDataEntered)
                            SetCOBRAEndDate();
                    }
                    else if (icdoPersonAccountGhdv.ihstOldValues.Count > 0)
                    {
                        var lstrOldCobraType = (string)icdoPersonAccountGhdv.ihstOldValues["cobra_type_value"];

                        if (lstrOldCobraType != icdoPersonAccountGhdv.cobra_type_value)
                        {
                            if (!IsMedicareEligibleDataEntered)
                                SetCOBRAEndDate();
                        }
                    }
                }
            }
            
            base.BeforeValidate(aenmPageMode);
        }
        string istrOldPaymentMethod = string.Empty;
        public override void BeforePersistChanges()
        {
			//IBS Adjustment creation logic changed (Affected area enroll to enroll scenario) - logged under Backlog PIR 8022 and 8111
            icolPosNegIbsDetail = null;
            icolPosNegEmployerPayrollDtl = null;
            //Update the Provider Org ID
            if (ibusProviderOrgPlan != null)
            {
                icdoPersonAccount.provider_org_id = ibusProviderOrgPlan.icdoOrgPlan.org_id;
            }

            if ((IsHistoryEntryRequired) &&
                (icdoPersonAccount.status_value == busConstant.PersonAccountStatusValid))
            {
                // PROD PIR ID 5825 - The 834 HIPAA file needs to extract suspended plan only once, for plans other than Medicare
                if (icdoPersonAccount.plan_id != busConstant.PlanIdMedicarePartD)
                    icdoPersonAccountGhdv.is_modified_after_bcbs_file_sent_flag = busConstant.Flag_Yes;
            }

            string lstrOldPaymentMethod = null;
            if (ibusPaymentElection.icdoPersonAccountPaymentElection.ihstOldValues.Count > 0)
            {
                iintOldPayeeAccountID = Convert.ToInt32(ibusPaymentElection.icdoPersonAccountPaymentElection.ihstOldValues["payee_account_id"]);

                lstrOldPaymentMethod = (string)ibusPaymentElection.icdoPersonAccountPaymentElection.ihstOldValues["payment_method_value"];
                if (icdoPersonAccount.status_value == busConstant.StatusReview)
                {
                    if (iclbPersonAccountPaymentElectionHistory.IsNull())
                        LoadPaymentElectionHistory();
                    if (iclbPersonAccountPaymentElectionHistory.Count > 1 && iclbPersonAccountPaymentElectionHistory.Any(i => i.icdoPersonAccountPaymentElectionHistory.history_change_date == icdoPersonAccount.history_change_date))
                        istrOldPaymentMethod = iclbPersonAccountPaymentElectionHistory.Skip(1).Take(1).FirstOrDefault().icdoPersonAccountPaymentElectionHistory.payment_method_value;
                    else
                        istrOldPaymentMethod = lstrOldPaymentMethod;
                }
                else
                    istrOldPaymentMethod = lstrOldPaymentMethod;
            }

            //PIR 1676 , Flag should be reset if payment election payment method changes also.
            bool lblnResetAfterTffrFileSentFlag = false;
            if (((IsHistoryEntryRequired) && (icdoPersonAccount.status_value == busConstant.PersonAccountStatusValid)) ||
                (istrOldPaymentMethod != ibusPaymentElection.icdoPersonAccountPaymentElection.payment_method_value))
            {
                lblnResetAfterTffrFileSentFlag = true;
            }

            if (lblnResetAfterTffrFileSentFlag)
            {
                // Update the TFFR Flag
                icdoPersonAccountGhdv.modified_after_tffr_file_sent_flag = busConstant.Flag_Yes;
            }

            /// PIR ID 1707 - Reset the HMO File flag.
            if ((icdoPersonAccount.plan_id == busConstant.PlanIdHMO) && (IsHistoryEntryRequired))
                icdoPersonAccountGhdv.included_to_hmo_file_flag = busConstant.Flag_No;


            if (icdoPersonAccountGhdv.ihstOldValues.Count > 0)
            {
                istrPreviousCoverageCode = Convert.ToString(icdoPersonAccountGhdv.ihstOldValues["coverage_code"]);
                istrPreviousPlanParticipationStatus = Convert.ToString(icdoPersonAccount.ihstOldValues["plan_participation_status_value"]);
            }

            //uat pir 1344
            if (IsPlanHealthOrMedicare && ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id > 0 &&
                !string.IsNullOrEmpty(icdoPersonAccountGhdv.cobra_type_value) && ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share == 0)
            {
                ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share = 100;
            }

            //UAT PIR 2373 people soft file changes
            //--Start--//
            if (IsHistoryEntryRequired && (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceCancelled ||
                icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended))
            {
                SetPersonAcccountForTeminationChange();
            }
            else
            {
                SetPersonAccountForEnrollmentChange();
            }
            //--End--//

            //prod pir 7220
            //--start--//
            if (string.IsNullOrEmpty(istrPreviousCOBRATypeValue) && !string.IsNullOrEmpty(icdoPersonAccountGhdv.cobra_type_value))
                icdoPersonAccountGhdv.overridden_structure_code = icdoPersonAccountGhdv.rate_structure_code;
            else if (!string.IsNullOrEmpty(istrPreviousCOBRATypeValue) && string.IsNullOrEmpty(icdoPersonAccountGhdv.cobra_type_value))
                icdoPersonAccountGhdv.overridden_structure_code = null;
            //--end--//
        }

        /// <summary>
        /// uat pir 2373 : method to set the PS event value based on enrollment change
        /// </summary>
        private void SetPersonAccountForEnrollmentChange()
        {
            if (ibusHistory == null)
                LoadPreviousHistory();
            if (IsHistoryEntryRequired)
            {
                //PROD PIR 4586
                //prod pir 4861 : removal of annual enrollment logic
                // PROD PIR 7705: Added Annual enrollment logic
                if (icdoPersonAccountGhdv.reason_value == busConstant.ChangeReasonAnnualEnrollment)
                {
                    //icdoPersonAccount.people_soft_file_sent_flag = busConstant.Flag_No;//PIR 17081
                    icdoPersonAccount.ps_file_change_event_value = busConstant.AnnualEnrollment;
                }
                else if (ibusHistory.icdoPersonAccountGhdvHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended &&
                    icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                {
                    //icdoPersonAccount.people_soft_file_sent_flag = busConstant.Flag_No;//PIR 17081
                    icdoPersonAccount.ps_file_change_event_value = busConstant.NewEnrollment;
                }
                else
                {
                    icdoPersonAccount.ps_file_change_event_value = busConstant.LevelOfCoverageChange;
                    //icdoPersonAccount.people_soft_file_sent_flag = busConstant.Flag_No;//PIR 17081
                }
            }
        }

        // All Objects are inserted forcibly - No base methods for insertion or update called.
        public override int PersistChanges()
        {
            if (icdoPersonAccountGhdv.ienuObjectState == ObjectState.Insert)
            {
                //PROD PIR 4586
                //prod pir 4861 : removal of annual enrollment logic 
                //PIR 7987 : Added Annual enrollment logic
                if (icdoPersonAccountGhdv.reason_value == busConstant.ChangeReasonAnnualEnrollment
                    && (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled ||
                    icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended))
                {
                    //icdoPersonAccount.people_soft_file_sent_flag = busConstant.Flag_No;//PIR 17081
                    if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                        icdoPersonAccount.ps_file_change_event_value = busConstant.AnnualEnrollment;
                    else//PIR 7987
                        icdoPersonAccount.ps_file_change_event_value = busConstant.AnnualEnrollmentWaived;
                }
                else
                {
                    //UAT PIR 2373 people soft file changes
                    //--Start--//
                    //icdoPersonAccount.people_soft_file_sent_flag = busConstant.Flag_No;//PIR 17081
                    icdoPersonAccount.ps_file_change_event_value = busConstant.NewEnrollment;
                    //--End--//
                }
                icdoPersonAccount.history_change_date = icdoPersonAccount.start_date;
                icdoPersonAccount.Insert();
                icdoPersonAccountGhdv.person_account_id = icdoPersonAccount.person_account_id;
                icdoPersonAccountGhdv.Insert();
            }
            else
            {
                //PIR 7987
                if (icdoPersonAccountGhdv.reason_value == busConstant.ChangeReasonAnnualEnrollment
                    && (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled ||
                    icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended))
                {
                    //icdoPersonAccount.people_soft_file_sent_flag = busConstant.Flag_No;//PIR 17081
                    if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                        icdoPersonAccount.ps_file_change_event_value = busConstant.AnnualEnrollment;
                    else//PIR 7987
                        icdoPersonAccount.ps_file_change_event_value = busConstant.AnnualEnrollmentWaived;
                }
                icdoPersonAccount.Update();
                icdoPersonAccountGhdv.Update();
            }

            //Somtime Payment Election may not get inserted at the new mode time. (Conversion Record didnt put these entries)..
            //So we need insert at Update Mode too..
            if (iblnIsFromMSS) // PIR 10241
            {
                if(ibusPaymentElection == null) //PIR-10856
                LoadPaymentElection();  //PIR-10147 

                //PIR 18155
                if (iblnIsRetireeToActive)
                {
                    if (ibusPaymentElection.icdoPersonAccountPaymentElection.account_payment_election_id > 0)
                    {
                        UpdateIBSFlagForActive();
                        ibusPaymentElection.icdoPersonAccountPaymentElection.Update();
                    }
                }
            }

            if (iblnIsFromInternal)//PIR 17962
            {
                if (ibusPaymentElection.icdoPersonAccountPaymentElection.account_payment_election_id > 0)
                {
                    ibusPaymentElection.icdoPersonAccountPaymentElection.Update();
                }
                else
                {
                    ibusPaymentElection.icdoPersonAccountPaymentElection.ienuObjectState = ObjectState.Insert;
                    ibusPaymentElection.icdoPersonAccountPaymentElection.person_account_id = icdoPersonAccount.person_account_id;
                    ibusPaymentElection.icdoPersonAccountPaymentElection.Insert();
                }
                //PIR 12737 & 8565 -- added payment election history
                if (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag == busConstant.Flag_Yes)
                {
                    busPersonAccountPaymentElectionHistory lbusPersonAccountPaymentElectionHistory = new busPersonAccountPaymentElectionHistory { icdoPersonAccountPaymentElectionHistory = new cdoPersonAccountPaymentElectionHistory() };
                    lbusPersonAccountPaymentElectionHistory.ibusPersonAccountPaymentElection = ibusPaymentElection;
                    lbusPersonAccountPaymentElectionHistory.ibusPersonAccountPaymentElection.icdoPersonAccount = icdoPersonAccount;
                    lbusPersonAccountPaymentElectionHistory.InsertPaymentElectionHistory();
                }
            }
            return 1;
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();

            ProcessHistory();

            //Update the Person Account Employment Detail
            if ((lobjCurrentObjectState == ObjectState.Insert) && (icdoPersonAccount.person_employment_dtl_id > 0) && (iblnIsActiveMember))
            {
                SetPersonAccountIDInPersonAccountEmploymentDetail();
            }
            LoadPersonAccountGHDVHsa(); //19997
            LoadPersonAccountGHDVHistory();
            LoadPreviousHistory();
			//PIR 12737 & 8565 -- added payment election history
            LoadPaymentElectionHistory();

            if (IsHealthOrMedicare)
                GetMonthlyPremiumAmountByRefID();
            else
                GetMonthlyPremiumAmount();

            // creating payroll / ibs adjustment records
            if ((IsHistoryEntryRequired || IsCurrentPaymentMethodPensionCheck) && icdoPersonAccount.status_value == busConstant.PersonAccountStatusValid)
            {
                //Establish Automatic RHIC Combine before creating IBS Adjustment
                if (ibusPaymentElection == null)
                    LoadPaymentElection();

                //Only for Health and Medicare Enrollment only 
                //No Need for IBS Check here. because, member was in IBS first.. and return back to work. such cases, we need to end the old and create new without apply to flag.
                if (IsHealthOrMedicare)
                {
                    //PIR 14346 undoing the create automatic combine on health enrollment change
                    //busBenefitRhicCombine lbusBenefitRhicCombine = new busBenefitRhicCombine { icdoBenefitRhicCombine = new cdoBenefitRhicCombine() };
                    //lbusBenefitRhicCombine.icdoBenefitRhicCombine.person_id = icdoPersonAccount.person_id;
                    ////Reload the Person Account to get the latest plan participation status
                    //ibusPerson.LoadPersonAccount();
                    //lbusBenefitRhicCombine.ibusPerson = ibusPerson;
                    //lbusBenefitRhicCombine.icdoBenefitRhicCombine.start_date = icdoPersonAccount.history_change_date_no_null;
                    //lbusBenefitRhicCombine.ienmAutomaticRhicCombineTrigger = busConstant.automatic_rhic_combine_trigger.enrollment_change;
                    ////Reload Receiver Health Person Account
                    //lbusBenefitRhicCombine.ibusPerson.LoadReceiverHealthPersonAccount(lbusBenefitRhicCombine.icdoBenefitRhicCombine.start_date);
                    //if (istrAllowOverlapHistory == busConstant.Flag_Yes)
                    //    lbusBenefitRhicCombine.iblnOverlapHistory = true;
                    //lbusBenefitRhicCombine.CreateAutomaticRHICCombine();

                    ////Reload the Health Split Collection
                    //LoadBenefitRhicCombineHealthSplit();

                    ////UAT PIR 2009 - Reload this always as if user tries to update the same record on the same screen. (New and Update on the same time)
                    ////Reload Rhic Records
                    //ibusPerson.LoadBenefitRhicCombine();
                    //ibusPerson.LoadLatestBenefitRhicCombine();

                    //Reload the Rhic Amount
                    GetMonthlyPremiumAmountByRefID();
                }

                busPersonAccountGhdvHistory lbusClosedHistory = null;
                busPersonAccountGhdvHistory lbusAddedHistory = null;
                foreach (busPersonAccountGhdvHistory lbusGHDVHistory in iclbPersonAccountGHDVHistory)
                {
                    if (lbusGHDVHistory.icdoPersonAccountGhdvHistory.end_date > DateTime.MinValue)
                    {
                        lbusClosedHistory = lbusGHDVHistory;
                        break;
                    }
                    else
                        lbusAddedHistory = lbusGHDVHistory;
                }

                if ((lbusClosedHistory != null)) //PIR 2029
                {
                    //Prod pir 4142 : if adjustment done as of same day, need to pass that date itself
                    if (lbusClosedHistory.icdoPersonAccountGhdvHistory.start_date == lbusClosedHistory.icdoPersonAccountGhdvHistory.end_date)
                    {
						//IBS Adjustment creation logic changed (Affected area enroll to enroll scenario) - logged under Backlog PIR 8022 and 8111
                        CreateAdjustmentPayrollForEnrollmentHistoryClose(lbusClosedHistory.icdoPersonAccountGhdvHistory.end_date,
                           lbusClosedHistory.icdoPersonAccountGhdvHistory.start_date, lbusAddedHistory.icdoPersonAccountGhdvHistory.plan_participation_status_value);
                    }
                    else
                    {
						//IBS Adjustment creation logic changed (Affected area enroll to enroll scenario) - logged under Backlog PIR 8022 and 8111
                        CreateAdjustmentPayrollForEnrollmentHistoryClose(lbusClosedHistory.icdoPersonAccountGhdvHistory.end_date.AddMonths(1),
                            lbusClosedHistory.icdoPersonAccountGhdvHistory.start_date, lbusAddedHistory.icdoPersonAccountGhdvHistory.plan_participation_status_value);
                    }
                }
                else if(lbusClosedHistory == null)
                {
                    CreateAdjustmentPayrollForEnrollmentHistoryClose(lbusAddedHistory.icdoPersonAccountGhdvHistory.start_date,
                            lbusAddedHistory.icdoPersonAccountGhdvHistory.start_date, lbusAddedHistory.icdoPersonAccountGhdvHistory.plan_participation_status_value);
                }
                if ((lbusAddedHistory != null) &&
                    (lbusAddedHistory.icdoPersonAccountGhdvHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)) //PIR 2029
                {
                    CreateAdjustmentPayrollForEnrollmentHistoryAdd(lbusAddedHistory.icdoPersonAccountGhdvHistory.start_date);
                }
            }

            // PROD PIR ID 4735
            //PIR 7535
            if ((icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended &&
                istrPreviousPlanParticipationStatus != busConstant.PlanParticipationStatusInsuranceSuspended) ||
                (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceCancelled &&
               istrPreviousPlanParticipationStatus != busConstant.PlanParticipationStatusInsuranceCancelled))
            {
                ibusPaymentElection.EndPAPITEntriesForSuspendedAccount(icdoPersonAccount.plan_id, icdoPersonAccount.current_plan_start_date);
                // ignore if already in review IBS/ Employer payroll detail entries
                if (icdoPersonAccount.status_value == busConstant.PersonAccountStatusValid)
                {
                    DBFunction.DBNonQuery("cdoIbsDetail.UpdateAllIBSDetailStatusToIgnoreAfterPlanSusorCan", new object[2] { icdoPersonAccount.person_account_id, icdoPersonAccount.current_plan_start_date },
                                       iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                    DBFunction.DBNonQuery("entEmployerPayrollHeader.UpdateAllEPDetailStatusToIgnoreAfterPlanSusorCan", new object[3] {
                                    icdoPersonAccount.person_id, icdoPersonAccount.plan_id, icdoPersonAccount.current_plan_start_date },
                                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                }
            }
            //PAPIT entries should only be updated when the IBS Adjustment is posted. ////PIR 16393 - Wrong insurance deduction
            else if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
            {
                    //PROD PIR 5538 : need to calculate premium based on latest date
                    //Must reload Provider Org Plan based on effective date. if the enrollment change happens between 2010 and 2011 , chances for two different providers.
                    if (icdoPersonAccount.person_employment_dtl_id > 0)
                    {
                        LoadOrgPlan(GetLatestDate(icdoPersonAccount.current_plan_start_date, ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date,
                                        idtNextBenefiPaymentDate));
                        LoadProviderOrgPlan(GetLatestDate(icdoPersonAccount.current_plan_start_date, ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date,
                                        idtNextBenefiPaymentDate));
                    }
                    else
                    {
                        LoadActiveProviderOrgPlan(GetLatestDate(icdoPersonAccount.current_plan_start_date, ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date,
                                            idtNextBenefiPaymentDate));
                    }
                    CalculatePremiumAmount(GetLatestDate(icdoPersonAccount.current_plan_start_date, ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date,
                                        idtNextBenefiPaymentDate));

                    // Change 1: UAT 1452 : need to end date PAPIT entries if at all in review status as we are saving payment election
                    // Change 2: Modified by Elayaraja - PIR 1488 deduction amount should be equal to premium amount - RHIC benefit amount
                    //             *** BR-074-12 *** System must transfer insurance premium information to the Payee Account's deduction list,
                    //             when insurance premium information is updated by the enrollment
                    // Change 3: Earlier this will update only if there exists changes in Payment Election fields. Modified now to update by all time.
                    if (iintOldPayeeAccountID != 0 && iintOldPayeeAccountID != ibusPaymentElection.icdoPersonAccountPaymentElection.payee_account_id)
                    {

                        ibusPaymentElection.ManagePayeeAccountPaymentItemType(ibusPaymentElection.icdoPersonAccountPaymentElection.payee_account_id,
                                            iintOldPayeeAccountID, icdoPersonAccountGhdv.MonthlyPremiumAmount - icdoPersonAccountGhdv.total_rhic_amount, icdoPersonAccount.plan_id,
                                            GetLatestDate(icdoPersonAccount.current_plan_start_date, ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date,
                                            idtNextBenefiPaymentDate), ibusPaymentElection.icdoPersonAccountPaymentElection.payment_method_value, ibusProviderOrgPlan.icdoOrgPlan.org_id, ablnIsPayeeAccountChanged: true);
                    }
                //PROD PIR 5538 : need to calculate premium based on latest date
                RecalculatePremiumBasedOnPlanEffectiveDate();
            }
            // UAT PIR ID 1077 - To refresh the screen values only if the Suppress flag is On.
            if (icdoPersonAccount.suppress_warnings_flag == busConstant.Flag_Yes)
                RefreshValues();
            LoadAllPersonEmploymentDetails();

            //Reload Person Account Employment Details by person account id after saving in new mode.
            LoadPersonAccountEmploymentDetails();

            //uat pir 2118 : new workflow added
            if (ibusBaseActivityInstance != null)
            {
                //pir 8313 for workflow H/D/V Annual Enrollment
                busBpmActivityInstance lbusActivityInstance = (busBpmActivityInstance)ibusBaseActivityInstance;
                if (busWorkflowHelper.GetWorkflowProcessIdByBpmProcessName(lbusActivityInstance.ibusBpmProcessInstance.ibusBpmProcess.icdoBpmProcess.name) == busConstant.MAP_Process_HDV_Annual_Enrollment)
                {
                    //SetProcessInstanceParameters();
                    SetCaseInstanceParameters();
                }
                else
                {
                    if (ibusPersonAccount == null)
                        LoadPersonAccount();
                    if (ibusPersonAccount.ibusBaseActivityInstance == null)
                        ibusPersonAccount.ibusBaseActivityInstance = ibusBaseActivityInstance;
                    ibusPersonAccount.SetCaseInstanceParameters();
                }
            }
            //UAT PIR 2220
            if (icdoPersonAccount.status_value == busConstant.PersonAccountStatusValid && IsHistoryEntryRequired)
                PostESSMessage();

            if (ibusPaymentElection == null)
                LoadPaymentElection();

            //PIR 17081 
            if (icdoPersonAccount.status_value == busConstant.PersonAccountStatusValid && IsHistoryEntryRequired && !iblnIsFromMSSForEnrollmentData &&
                ((icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth && !(icdoPersonAccountGhdv.is_health_cobra || icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeRetiree))
                || (icdoPersonAccount.plan_id == busConstant.PlanIdDental && !(icdoPersonAccountGhdv.dental_insurance_type_value == busConstant.DentalInsuranceTypeCOBRA || icdoPersonAccountGhdv.dental_insurance_type_value == busConstant.DentalInsuranceTypeRetiree))
                || (icdoPersonAccount.plan_id == busConstant.PlanIdVision && !(icdoPersonAccountGhdv.vision_insurance_type_value == busConstant.VisionInsuranceTypeCOBRA || icdoPersonAccountGhdv.vision_insurance_type_value == busConstant.VisionInsuranceTypeRetiree)))
                && ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag != busConstant.Flag_Yes) //PIR 20135 - Issue - 2
            {
                InsertIntoEnrollmentData();
                if (icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth)
                {
                    //PIR 20902
                    if (iblnIsNewHDHPEnrollment && icdoPersonAccountGhdv.alternate_structure_code_value == busConstant.AlternateStructureCodeHDHP && this.ibusPersonAccountGhdvHsa.icdoPersonAccountGhdvHsa.contribution_amount == 0 &&
                        this.ibusPersonAccountGhdvHsa.icdoPersonAccountGhdvHsa.contribution_start_date == DateTime.MinValue)
                    {
                        InsertIntoEnrollmentDataDefaultEntryForHSA();
                    }
                    else  //PIR 21077 - 1. HDHP family to single or single to family, 2. Suspend HDHP, 3. HDHP to PPO
                    {
                        busPersonAccountGhdvHistory lbusPersonAccountGhdvHistory = null;
                        if (iclbPersonAccountGHDVHistory == null)
                            LoadPersonAccountGHDVHistory();
                        if (iclbPersonAccountGHDVHistory.Count >= 2)
                            lbusPersonAccountGhdvHistory = iclbPersonAccountGHDVHistory[1];
                        else
                            lbusPersonAccountGhdvHistory = iclbPersonAccountGHDVHistory.FirstOrDefault();

                        if (lbusPersonAccountGhdvHistory.IsNotNull() && (icdoPersonAccountGhdv.alternate_structure_code_value == busConstant.AlternateStructureCodeHDHP &&
                            (icdoPersonAccountGhdv.coverage_code != lbusPersonAccountGhdvHistory.icdoPersonAccountGhdvHistory.coverage_code ||
                            icdoPersonAccount.plan_participation_status_value != lbusPersonAccountGhdvHistory.icdoPersonAccountGhdvHistory.plan_participation_status_value))
                            || (icdoPersonAccountGhdv.alternate_structure_code_value != lbusPersonAccountGhdvHistory.icdoPersonAccountGhdvHistory.alternate_structure_code_value
                            && lbusPersonAccountGhdvHistory.icdoPersonAccountGhdvHistory.alternate_structure_code_value == busConstant.AlternateStructureCodeHDHP))
                        {
                            busPersonAccountGhdv lbusPersonAccountGhdv = new busPersonAccountGhdv();
                            lbusPersonAccountGhdv.FindGHDVByPersonAccountID(icdoPersonAccount.person_account_id);
                            lbusPersonAccountGhdv.icdoPersonAccount = icdoPersonAccount;
                            if (lbusPersonAccountGhdv.icdoPersonAccountGhdv.person_account_ghdv_id > 0)
                                lbusPersonAccountGhdv.InsertIntoEnrollmentDataDefaultEntryForHSA();
                        }
                    }

                    //PIR 24620 - If HDHP, end HSA records
                    LoadPersonAccount();
                    if(ibusPersonAccountGHDV == null)
                        LoadPersonAccountGHDV();

                    if (!iblnIsNewHDHPEnrollment && ibusPersonAccountGhdvHsa.IsNotNull() &&
                        ibusPersonAccountGHDV.icdoPersonAccountGhdv.alternate_structure_code_value == busConstant.AlternateStructureCodeHDHP &&
                        (iclbPersonAccountGhdvHsa.Where(o => o.icdoPersonAccountGhdvHsa.contribution_amount >= 0 &&
                         o.icdoPersonAccountGhdvHsa.contribution_start_date != DateTime.MinValue).Any()))
                    {
                        foreach (busPersonAccountGhdvHsa lobjPersonAccountGhdvHsa in iclbPersonAccountGhdvHsa)
                        {
                            if (lobjPersonAccountGhdvHsa.icdoPersonAccountGhdvHsa.contribution_end_date == DateTime.MinValue)
                            {
                                if (iblnIsFromTerminationPost)
                                {
                                    lobjPersonAccountGhdvHsa.icdoPersonAccountGhdvHsa.contribution_end_date = icdoPersonAccount.history_change_date.AddDays(-1);
                                    lobjPersonAccountGhdvHsa.icdoPersonAccountGhdvHsa.Update();
                                }
                                lobjPersonAccountGhdvHsa.icdoPersonAccount = icdoPersonAccount;
                                lobjPersonAccountGhdvHsa.icdoPersonAccountGhdv = icdoPersonAccountGhdv;
                                lobjPersonAccountGhdvHsa.LoadPersonAccount();
                                lobjPersonAccountGhdvHsa.LoadPersonAccountGHDV();
                                lobjPersonAccountGhdvHsa.LoadPersonAccountGHDVHsa();
                                lobjPersonAccountGhdvHsa.InsertIntoEnrollmentDataForHSA();
                            }
                        }
                    }
                }
            }
            if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended)
                UpdateEndDateForSuspendStatus();
        }

        //PIR 20902
        #region PIR 20902 start
        /// <summary>
        /// inserting default HSA entry in Enrollment data table  for HDHP
        /// </summary>    
        public void InsertIntoEnrollmentDataDefaultEntryForHSA()
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

            //if (ibusPersonAccountGHDV.IsNull()) //PIR 26760  - Load Person Account GHDV everytime as object was not loaded when HDHP was selected as 1st enrollment.
            LoadPersonAccountGHDV();

            if (ibusPersonAccountGHDV.ibusHistory.IsNull())
                ibusPersonAccountGHDV.LoadPreviousHistory();
            if (ibusHistory.IsNull())        //PIR 21077 - 
                ibusHistory = ibusPersonAccountGHDV.ibusHistory;
            lobjDailyPAPeopleSoft.ibusPersonAccountGhdv.icdoPersonAccountGhdv = ibusPersonAccountGHDV.icdoPersonAccountGhdv;
            LoadPersonAccountGHDVHsaForDefaultEntry();
            lobjDailyPAPeopleSoft.ibusPersonAccountGhdvHSA.icdoPersonAccountGhdvHsa = ibusPersonAccountGhdvHsa.icdoPersonAccountGhdvHsa;
            lobjDailyPAPeopleSoft.LoadPersonEmploymentForPeopleSoft();
            lobjDailyPAPeopleSoft.LoadPeopleSoftOrgGroupValues();
            ibusPersonAccountGHDV.LoadCoverageCodeDescription();
            if (iblnIsNewHDHPEnrollment) //PIR 21077
                lobjDailyPAPeopleSoft.idtHSAStartDate = ibusHistory.icdoPersonAccountGhdvHistory.start_date;
            else
                lobjDailyPAPeopleSoft.idtHSAStartDate = DateTime.MinValue;
            lobjDailyPAPeopleSoft.idtHSAEndDate = DateTime.MinValue;


            if (ibusPerson.IsNull())
                LoadPerson();

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
                            lstrWellnessFlag, ibusPersonAccountGHDV.istrCoverageCode.Trim());
                }

                #region Insert Enrollment Data
                if (lobjDailyPAPeopleSoft.iclbDailyPersonAccountPeopleSoft.IsNotNull() && lobjDailyPAPeopleSoft.iclbDailyPersonAccountPeopleSoft.Count > 0)
                {

                    foreach (busDailyPersonAccountPeopleSoft lobjDailyPeopleSoft in lobjDailyPAPeopleSoft.iclbDailyPersonAccountPeopleSoft)
                    {

                        lobjEnrollmentData.icdoEnrollmentData.source_id = ibusHistory.icdoPersonAccountGhdvHistory.person_account_ghdv_history_id;
                        lobjEnrollmentData.icdoEnrollmentData.plan_id = busConstant.PlanIdFlex;//icdoPersonAccount.plan_id;
                        lobjEnrollmentData.icdoEnrollmentData.ssn = ibusPerson.icdoPerson.ssn;
                        lobjEnrollmentData.icdoEnrollmentData.ndpers_member_id = ibusPerson.icdoPerson.person_id;
                        lobjEnrollmentData.icdoEnrollmentData.person_account_id = icdoPersonAccount.person_account_id;
                        lobjEnrollmentData.icdoEnrollmentData.peoplesoft_id = ibusPerson.icdoPerson.peoplesoft_id;
                        lobjEnrollmentData.icdoEnrollmentData.employer_org_id = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id;
                        lobjEnrollmentData.icdoEnrollmentData.employment_type_value = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value;
                        lobjEnrollmentData.icdoEnrollmentData.plan_status_value = ibusPersonAccountGHDV.ibusHistory.icdoPersonAccountGhdvHistory.plan_participation_status_value;
                        lobjEnrollmentData.icdoEnrollmentData.change_reason_value = string.Empty;
                        lobjEnrollmentData.icdoEnrollmentData.start_date = ibusHistory.icdoPersonAccountGhdvHistory.start_date;
                        lobjEnrollmentData.icdoEnrollmentData.end_date = DateTime.MinValue;
                        lobjEnrollmentData.icdoEnrollmentData.peoplesoft_change_reason_value = busConstant.HSAStart;
                        lobjEnrollmentData.icdoEnrollmentData.coverage_amount = 0.00M;
                        lobjEnrollmentData.icdoEnrollmentData.provider_org_id = ibusPersonAccountGHDV.ibusHistory.icdoPersonAccountGhdvHistory.provider_org_id;
                        lobjEnrollmentData.icdoEnrollmentData.monthly_premium = 0;
                        lobjEnrollmentData.icdoEnrollmentData.pretaxed_premiums = busConstant.Flag_Yes; //PIR  20902   icdoPersonAccountGhdv.premium_conversion_indicator_flag;
                        lobjEnrollmentData.icdoEnrollmentData.peoplesoft_org_group_value = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.peoplesoft_org_group_value;
                        //lobjEnrollmentData.icdoEnrollmentData.peoplesoft_change_reason_value = lobjDailyPAPeopleSoft.istrPSFileChangeEvent;//PIR 23674

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
                        if (lobjEnrollmentData.icdoEnrollmentData.coverage_election_value == "T")//PIR 21077
                            lobjEnrollmentData.icdoEnrollmentData.is_benefit_enrollment_report_generated = busConstant.Flag_No;
                        else
                            lobjEnrollmentData.icdoEnrollmentData.is_benefit_enrollment_report_generated = busConstant.Flag_Yes;
                        lobjEnrollmentData.icdoEnrollmentData.peoplesoft_file_sent_flag = busConstant.Flag_No;
                        lobjEnrollmentData.icdoEnrollmentData.pretax_amount = 0.00M;
                        lobjEnrollmentData.icdoEnrollmentData.Insert();
                        lintCounter++;
                    }
                }
                #endregion
            }
            else
            {
                #region update data

                lobjEnrollmentData.icdoEnrollmentData.source_id = ibusHistory.icdoPersonAccountGhdvHistory.person_account_ghdv_history_id;
                lobjEnrollmentData.icdoEnrollmentData.plan_id = busConstant.PlanIdFlex;
                lobjEnrollmentData.icdoEnrollmentData.ssn = ibusPerson.icdoPerson.ssn;
                lobjEnrollmentData.icdoEnrollmentData.ndpers_member_id = ibusPerson.icdoPerson.person_id;
                lobjEnrollmentData.icdoEnrollmentData.person_account_id = icdoPersonAccountGhdv.person_account_id;
                lobjEnrollmentData.icdoEnrollmentData.peoplesoft_id = ibusPerson.icdoPerson.peoplesoft_id;
                lobjEnrollmentData.icdoEnrollmentData.pay_period_amount = 0.00m;
                lobjEnrollmentData.icdoEnrollmentData.employer_org_id = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id;
                lobjEnrollmentData.icdoEnrollmentData.employment_type_value = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value;
                lobjEnrollmentData.icdoEnrollmentData.plan_status_value = busConstant.PlanParticipationStatusInsuranceEnrolled;
                lobjEnrollmentData.icdoEnrollmentData.change_reason_value = string.Empty;
                lobjEnrollmentData.icdoEnrollmentData.start_date = ibusHistory.icdoPersonAccountGhdvHistory.start_date;
                lobjEnrollmentData.icdoEnrollmentData.end_date = DateTime.MinValue;
                lobjEnrollmentData.icdoEnrollmentData.monthly_premium = 0.00M;
                lobjEnrollmentData.icdoEnrollmentData.pretaxed_premiums = busConstant.Flag_Yes;
                lobjEnrollmentData.icdoEnrollmentData.coverage_amount = 0.00M;

                lobjEnrollmentData.icdoEnrollmentData.plan_option_value = string.Empty;
                lobjEnrollmentData.icdoEnrollmentData.is_benefit_enrollment_report_generated = busConstant.Flag_Yes;
                lobjEnrollmentData.icdoEnrollmentData.peoplesoft_file_sent_flag = busConstant.Flag_Yes;
                lobjEnrollmentData.icdoEnrollmentData.pretax_amount = 0.00M;
                lobjEnrollmentData.icdoEnrollmentData.Insert();
                #endregion data
            }
        }

        public void LoadPersonAccountGHDVHsaForDefaultEntry()
        {
            if (ibusPersonAccountGhdvHsa == null)
            {
                ibusPersonAccountGhdvHsa = new busPersonAccountGhdvHsa();
                ibusPersonAccountGhdvHsa.icdoPersonAccountGhdvHsa = new doPersonAccountGhdvHsa();
            }

            ibusPersonAccountGhdvHsa.icdoPersonAccountGhdvHsa.contribution_amount = 0;
            ibusPersonAccountGhdvHsa.icdoPersonAccountGhdvHsa.contribution_end_date = DateTime.MinValue;
            ibusPersonAccountGhdvHsa.icdoPersonAccountGhdvHsa.contribution_start_date = ibusHistory.icdoPersonAccountGhdvHistory.start_date;
            ibusPersonAccountGhdvHsa.icdoPersonAccountGhdvHsa.person_account_ghdv_hsa_id = 0;
            ibusPersonAccountGhdvHsa.icdoPersonAccountGhdvHsa.person_account_ghdv_id = icdoPersonAccountGhdv.person_account_ghdv_id;
            ibusPersonAccountGhdvHsa.icdoPersonAccountGhdvHsa.person_account_id = icdoPersonAccountGhdv.person_account_id;
        }
        #endregion
        public void InsertIntoEnrollmentData()
        {
            busEnrollmentData lobjEnrollmentData = new busEnrollmentData();
            lobjEnrollmentData.icdoEnrollmentData = new doEnrollmentData();

            busDailyPersonAccountPeopleSoft lobjDailyPAPeopleSoft = new busDailyPersonAccountPeopleSoft();
            lobjDailyPAPeopleSoft.ibusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
            lobjDailyPAPeopleSoft.ibusProvider = new busOrganization { icdoOrganization = new cdoOrganization() };
            lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            lobjDailyPAPeopleSoft.ibusPersonAccountGhdv = new busPersonAccountGhdv { icdoPersonAccountGhdv = new cdoPersonAccountGhdv() };

            if (lobjDailyPAPeopleSoft.iclbDailyPersonAccountPeopleSoft == null)
                lobjDailyPAPeopleSoft.iclbDailyPersonAccountPeopleSoft = new Collection<busDailyPersonAccountPeopleSoft>();

            lobjDailyPAPeopleSoft.ibusPersonAccount.icdoPersonAccount = icdoPersonAccount;
            //lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail = ibusPersonEmploymentDetail;
            lobjDailyPAPeopleSoft.ibusPersonAccountGhdv.icdoPersonAccountGhdv = icdoPersonAccountGhdv;
            lobjDailyPAPeopleSoft.ibusProvider = ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization;

            lobjDailyPAPeopleSoft.LoadPersonEmploymentForPeopleSoft();
            lobjDailyPAPeopleSoft.LoadPeopleSoftOrgGroupValues();

            string lstrWellnessFlag = string.Empty;
            int lintCounter = 0;

            if ((ibusHistory.icdoPersonAccountGhdvHistory.ps_file_change_event_value == busConstant.AnnualEnrollment
                || ibusHistory.icdoPersonAccountGhdvHistory.ps_file_change_event_value == busConstant.AnnualEnrollmentWaived) &&
                (ibusHistory.icdoPersonAccountGhdvHistory.reason_value == busConstant.AnnualEnrollment
                || ibusHistory.icdoPersonAccountGhdvHistory.reason_value == busConstant.AnnualEnrollmentWaived))
            {
                lobjDailyPAPeopleSoft.iblnAnnualEnrollment = true;
                lobjDailyPAPeopleSoft.istrHistoryPSFileChangeEventValue = ibusHistory.icdoPersonAccountGhdvHistory.ps_file_change_event_value;
            }

			//PIR 20135 - Issue 1
            DBFunction.DBNonQuery("cdoPersonAccount.UpdatePrevPeopleSoftRecords", new object[5] { icdoPersonAccount.person_id, icdoPersonAccount.plan_id ,
                        ibusHistory.icdoPersonAccountGhdvHistory.start_date, lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id,icdoPersonAccount.person_account_id },
                        iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            //PIR - 21058 Same start date and end date should be removed from enrollment report
            //if (icdoPersonAccount.plan_id == busConstant.PlanIdDental || icdoPersonAccount.plan_id == busConstant.PlanIdVision)
            //{
                if (iclbPersonAccountGHDVHistory.IsNull())
                    LoadPersonAccountGHDVHistory();
                busPersonAccountGhdvHistory lbusSameEndDatedHistory = iclbPersonAccountGHDVHistory.OrderByDescending(his => his.icdoPersonAccountGhdvHistory.person_account_ghdv_history_id)
                    .FirstOrDefault(his => his.icdoPersonAccountGhdvHistory.start_date.Date == his.icdoPersonAccountGhdvHistory.end_date.Date);
                if (lbusSameEndDatedHistory.IsNotNull())
                {
                    DBFunction.DBNonQuery("cdoPersonAccount.UpdatePrevPeopleSoftNBenefitEnrlFlag", new object[2] { lbusSameEndDatedHistory.icdoPersonAccountGhdvHistory.person_account_ghdv_history_id, icdoPersonAccount.person_account_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                }
            //}
            //PIR 21392 – Outbound file and BER updates needed for HSA and HDHP elections
            if (icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth)
                CheckPlanOptionValueChangeOnSameDate();

            //PIR 26238
            if (iclbOverlappingHistory.IsNotNull() && iclbOverlappingHistory.Count() > 0)
            {
                foreach (busPersonAccountGhdvHistory lobjDeletedHistory in iclbOverlappingHistory)
                {
                    DBFunction.DBNonQuery("cdoPersonAccount.UpdatePrevPeopleSoftNBenefitEnrlFlag", new object[2] { lobjDeletedHistory.icdoPersonAccountGhdvHistory.person_account_ghdv_history_id, icdoPersonAccount.person_account_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                }
            }

            //PeopleSoft logic will only be executed if the ORG GROUP of the current Organization is PeopleSoft. 
            if ((lobjDailyPAPeopleSoft.iclbPeopleSoftOrgGroupValue.Where(i => i.code_value == lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.peoplesoft_org_group_value && i.data2 == busConstant.Flag_Yes).Count() > 0))
            {
                if (icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth)
                {
                    if (lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.iclbOrgPlan == null)
                        lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.LoadOrgPlan();

                    lobjDailyPAPeopleSoft.ibusPersonAccount.LoadOrgPlan();
                    lstrWellnessFlag = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusOrgPlan.icdoOrgPlan.wellness_flag;

                    if (lobjDailyPAPeopleSoft.istrEmpTypeValue == busConstant.PersonJobTypeTemporary)
                    {
                        lobjEnrollmentData.icdoEnrollmentData.source_id = ibusHistory.icdoPersonAccountGhdvHistory.person_account_ghdv_history_id;
                        lobjEnrollmentData.icdoEnrollmentData.plan_id = icdoPersonAccount.plan_id;
                        lobjEnrollmentData.icdoEnrollmentData.ssn = ibusPerson.icdoPerson.ssn;
                        lobjEnrollmentData.icdoEnrollmentData.ndpers_member_id = ibusPerson.icdoPerson.person_id;
                        lobjEnrollmentData.icdoEnrollmentData.person_account_id = icdoPersonAccount.person_account_id;
                        lobjEnrollmentData.icdoEnrollmentData.peoplesoft_id = ibusPerson.icdoPerson.peoplesoft_id;
                        lobjEnrollmentData.icdoEnrollmentData.employer_org_id = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id;
                        lobjEnrollmentData.icdoEnrollmentData.employment_type_value = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value;
                        lobjEnrollmentData.icdoEnrollmentData.plan_status_value = ibusHistory.icdoPersonAccountGhdvHistory.plan_participation_status_value;
                        lobjEnrollmentData.icdoEnrollmentData.change_reason_value = ibusHistory.icdoPersonAccountGhdvHistory.reason_value;
                        lobjEnrollmentData.icdoEnrollmentData.start_date = ibusHistory.icdoPersonAccountGhdvHistory.start_date;
                        lobjEnrollmentData.icdoEnrollmentData.end_date = ibusHistory.icdoPersonAccountGhdvHistory.end_date;
                        lobjEnrollmentData.icdoEnrollmentData.provider_org_id = ibusHistory.icdoPersonAccountGhdvHistory.provider_org_id;
                        lobjEnrollmentData.icdoEnrollmentData.monthly_premium = icdoPersonAccountGhdv.MonthlyPremiumAmount;
                        lobjEnrollmentData.icdoEnrollmentData.pretaxed_premiums = icdoPersonAccountGhdv.premium_conversion_indicator_flag;
                        lobjEnrollmentData.icdoEnrollmentData.peoplesoft_org_group_value = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.peoplesoft_org_group_value;
                        if (ibusHistory.icdoPersonAccountGhdvHistory.alternate_structure_code_value.IsNotNullOrEmpty())
                            lobjEnrollmentData.icdoEnrollmentData.plan_option_value = ibusHistory.icdoPersonAccountGhdvHistory.alternate_structure_code_value;
                        else
                            lobjEnrollmentData.icdoEnrollmentData.plan_option_value = ibusHistory.icdoPersonAccountGhdvHistory.plan_option_value;
                        lobjEnrollmentData.icdoEnrollmentData.coverage_code_for_health = ibusHistory.icdoPersonAccountGhdvHistory.coverage_code;
                        lobjEnrollmentData.icdoEnrollmentData.is_benefit_enrollment_report_generated = busConstant.Flag_No;
                        lobjEnrollmentData.icdoEnrollmentData.peoplesoft_file_sent_flag = busConstant.Flag_Yes;
                        lobjEnrollmentData.icdoEnrollmentData.Insert();
                    }
                    else
                    {
                        lobjDailyPAPeopleSoft.GeneratePeoplesoftEntryForGroupHealth(lobjDailyPAPeopleSoft.istrEmpTypeValue, lstrWellnessFlag);
                    }
                }
                if (icdoPersonAccount.plan_id == busConstant.PlanIdDental || icdoPersonAccount.plan_id == busConstant.PlanIdVision)
                {
                    lobjDailyPAPeopleSoft.istrLevelOfCoverage = ibusHistory.icdoPersonAccountGhdvHistory.level_of_coverage_value;
                    lobjDailyPAPeopleSoft.GeneratePeoplesoftEntryForDentalAndVision();
                }

                if (lobjDailyPAPeopleSoft.iclbDailyPersonAccountPeopleSoft.IsNotNull() && lobjDailyPAPeopleSoft.iclbDailyPersonAccountPeopleSoft.Count > 0)
                {
                    foreach (busDailyPersonAccountPeopleSoft lobjDailyPeopleSoft in lobjDailyPAPeopleSoft.iclbDailyPersonAccountPeopleSoft)
                    {
                        lobjEnrollmentData.icdoEnrollmentData.source_id = ibusHistory.icdoPersonAccountGhdvHistory.person_account_ghdv_history_id;
                        lobjEnrollmentData.icdoEnrollmentData.plan_id = icdoPersonAccount.plan_id;
                        lobjEnrollmentData.icdoEnrollmentData.ssn = ibusPerson.icdoPerson.ssn;
                        lobjEnrollmentData.icdoEnrollmentData.ndpers_member_id = ibusPerson.icdoPerson.person_id;
                        lobjEnrollmentData.icdoEnrollmentData.person_account_id = icdoPersonAccount.person_account_id;
                        lobjEnrollmentData.icdoEnrollmentData.peoplesoft_id = ibusPerson.icdoPerson.peoplesoft_id;
                        lobjEnrollmentData.icdoEnrollmentData.employer_org_id = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id;
                        lobjEnrollmentData.icdoEnrollmentData.employment_type_value = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value;
                        lobjEnrollmentData.icdoEnrollmentData.plan_status_value = ibusHistory.icdoPersonAccountGhdvHistory.plan_participation_status_value;
                        lobjEnrollmentData.icdoEnrollmentData.change_reason_value = ibusHistory.icdoPersonAccountGhdvHistory.reason_value;
                        lobjEnrollmentData.icdoEnrollmentData.start_date = ibusHistory.icdoPersonAccountGhdvHistory.start_date;
                        lobjEnrollmentData.icdoEnrollmentData.end_date = ibusHistory.icdoPersonAccountGhdvHistory.end_date;
                        lobjEnrollmentData.icdoEnrollmentData.provider_org_id = ibusHistory.icdoPersonAccountGhdvHistory.provider_org_id;
                        lobjEnrollmentData.icdoEnrollmentData.monthly_premium = icdoPersonAccountGhdv.MonthlyPremiumAmount;
                        lobjEnrollmentData.icdoEnrollmentData.pretaxed_premiums = icdoPersonAccountGhdv.premium_conversion_indicator_flag;
                        lobjEnrollmentData.icdoEnrollmentData.peoplesoft_org_group_value = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.peoplesoft_org_group_value;
                        lobjEnrollmentData.icdoEnrollmentData.peoplesoft_change_reason_value = lobjDailyPAPeopleSoft.istrPSFileChangeEvent;

                        if (ibusHistory.icdoPersonAccountGhdvHistory.alternate_structure_code_value.IsNotNullOrEmpty())
                            lobjEnrollmentData.icdoEnrollmentData.plan_option_value = ibusHistory.icdoPersonAccountGhdvHistory.alternate_structure_code_value;
                        else
                            lobjEnrollmentData.icdoEnrollmentData.plan_option_value = ibusHistory.icdoPersonAccountGhdvHistory.plan_option_value;

                        if (icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth)
                        {
                            lobjEnrollmentData.icdoEnrollmentData.coverage_code_for_health = ibusHistory.icdoPersonAccountGhdvHistory.coverage_code;
                        }
                        else
                            lobjEnrollmentData.icdoEnrollmentData.level_of_coverage_value = ibusHistory.icdoPersonAccountGhdvHistory.level_of_coverage_value;

                        lobjEnrollmentData.icdoEnrollmentData.coverage_code = lobjDailyPeopleSoft.istrCoverageCode;
                        lobjEnrollmentData.icdoEnrollmentData.benefit_plan = lobjDailyPeopleSoft.istrBenefitPlan;
                        lobjEnrollmentData.icdoEnrollmentData.deduction_begin_date = lobjDailyPeopleSoft.idtDeductionBeginDate;
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
                        lobjEnrollmentData.icdoEnrollmentData.Insert();
                        lintCounter++;

                        //PIR 23856 - Update previous PS sent flag in Enrollment Data to Y if Person Employment is changed (transfer).
                        DBFunction.DBNonQuery("cdoPersonAccount.UpdatePSSentFlagForTransfers", new object[5] { icdoPersonAccount.plan_id,icdoPersonAccount.person_id,
                                                                                                  lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id,
                                                                                                  lobjEnrollmentData.icdoEnrollmentData.deduction_begin_date,icdoPersonAccount.person_account_id },
                                                                                                  iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                    }
                }
            }
            else
            {
                if ((lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date == DateTime.MinValue || (lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date != DateTime.MinValue &&
                    busGlobalFunctions.CheckDateOverlapping(icdoPersonAccount.history_change_date.AddMonths(-1), lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date,
                    new DateTime(lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date.Year, lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date.Month, 1).AddMonths(2)))))
                {
                    lobjEnrollmentData.icdoEnrollmentData.source_id = ibusHistory.icdoPersonAccountGhdvHistory.person_account_ghdv_history_id;
                    lobjEnrollmentData.icdoEnrollmentData.plan_id = icdoPersonAccount.plan_id;
                    lobjEnrollmentData.icdoEnrollmentData.ssn = ibusPerson.icdoPerson.ssn;
                    lobjEnrollmentData.icdoEnrollmentData.ndpers_member_id = ibusPerson.icdoPerson.person_id;
                    lobjEnrollmentData.icdoEnrollmentData.person_account_id = icdoPersonAccount.person_account_id;
                    lobjEnrollmentData.icdoEnrollmentData.peoplesoft_id = ibusPerson.icdoPerson.peoplesoft_id;
                    lobjEnrollmentData.icdoEnrollmentData.employer_org_id = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id;
                    lobjEnrollmentData.icdoEnrollmentData.employment_type_value = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value;
                    lobjEnrollmentData.icdoEnrollmentData.plan_status_value = ibusHistory.icdoPersonAccountGhdvHistory.plan_participation_status_value;
                    lobjEnrollmentData.icdoEnrollmentData.change_reason_value = ibusHistory.icdoPersonAccountGhdvHistory.reason_value;
                    lobjEnrollmentData.icdoEnrollmentData.start_date = ibusHistory.icdoPersonAccountGhdvHistory.start_date;
                    lobjEnrollmentData.icdoEnrollmentData.end_date = ibusHistory.icdoPersonAccountGhdvHistory.end_date;
                    lobjEnrollmentData.icdoEnrollmentData.provider_org_id = ibusHistory.icdoPersonAccountGhdvHistory.provider_org_id;
                    lobjEnrollmentData.icdoEnrollmentData.monthly_premium = icdoPersonAccountGhdv.MonthlyPremiumAmount;
                    lobjEnrollmentData.icdoEnrollmentData.pretaxed_premiums = icdoPersonAccountGhdv.premium_conversion_indicator_flag;
                    lobjEnrollmentData.icdoEnrollmentData.peoplesoft_org_group_value = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.peoplesoft_org_group_value;


                    if (ibusHistory.icdoPersonAccountGhdvHistory.alternate_structure_code_value.IsNotNullOrEmpty())
                        lobjEnrollmentData.icdoEnrollmentData.plan_option_value = ibusHistory.icdoPersonAccountGhdvHistory.alternate_structure_code_value;
                    else
                        lobjEnrollmentData.icdoEnrollmentData.plan_option_value = ibusHistory.icdoPersonAccountGhdvHistory.plan_option_value;

                    if (icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth)
                        lobjEnrollmentData.icdoEnrollmentData.coverage_code_for_health = ibusHistory.icdoPersonAccountGhdvHistory.coverage_code;
                    else
                        lobjEnrollmentData.icdoEnrollmentData.level_of_coverage_value = ibusHistory.icdoPersonAccountGhdvHistory.level_of_coverage_value;
                    
                    lobjEnrollmentData.icdoEnrollmentData.is_benefit_enrollment_report_generated = busConstant.Flag_No;
                    lobjEnrollmentData.icdoEnrollmentData.peoplesoft_file_sent_flag = busConstant.Flag_Yes;
                    lobjEnrollmentData.icdoEnrollmentData.Insert();
                }
            }
        }
        public void CheckPlanOptionValueChangeOnSameDate()
        {
            busPersonAccountGhdvHistory lbusPersonAccountGhdvHistory = null;
            if (iclbPersonAccountGHDVHistory == null)
                LoadPersonAccountGHDVHistory();
            if (iclbPersonAccountGHDVHistory.Count >= 2)
                lbusPersonAccountGhdvHistory = iclbPersonAccountGHDVHistory[1];
            else
                lbusPersonAccountGhdvHistory = iclbPersonAccountGHDVHistory.FirstOrDefault();

            if (lbusPersonAccountGhdvHistory.IsNotNull())
            {
                if(lbusPersonAccountGhdvHistory.icdoPersonAccountGhdvHistory.alternate_structure_code_value == busConstant.AlternateStructureCodeHDHP && 
                    (string.IsNullOrEmpty(icdoPersonAccountGhdv.alternate_structure_code_value)))
                {
                    DBFunction.DBNonQuery("cdoPersonAccount.UpdatePrevRecordsSamePlanOptionValue", new object[2] {icdoPersonAccount.person_account_id, DateTime.Now.Date },
                        iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                }
            }
        }
		//PIR 19355 - APP-7013 needs to be associated with GHDV Maintenance
        public void LoadCorObjects()
        {
            ibusPayeeAccountForCor = new busPayeeAccount() { icdoPayeeAccount = new cdoPayeeAccount() };
            ibusPayeeAccountForCor.ibusPayee = ibusPerson;
        }

        //UAT PIR 2220
        private void PostESSMessage()
        {
            string lstrPrioityValue = string.Empty;
            if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled ||
                icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended)
            {
                if (ibusPerson == null)
                    LoadPerson();
                if (icdoPersonAccount.person_employment_dtl_id <= 0)
                    icdoPersonAccount.person_employment_dtl_id = GetEmploymentDetailID();
                // post message to employer
                if (ibusPersonEmploymentDetail == null)
                    LoadPersonEmploymentDetail();

                if (ibusPersonEmploymentDetail.ibusPersonEmployment.IsNull())
                    ibusPersonEmploymentDetail.LoadPersonEmployment();
                if (ibusPlan == null)
                    LoadPlan();
                if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                {
                    string lstrCoverageCodeOrLevelOfCoverage = string.Empty;
                    if (icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth
                        || icdoPersonAccount.plan_id == busConstant.PlanIdMedicarePartD)
                    {
                        LoadCoverageCodeDescription();
                        lstrCoverageCodeOrLevelOfCoverage = istrCoverageCode;
                    }
                    else
                        lstrCoverageCodeOrLevelOfCoverage = icdoPersonAccountGhdv.level_of_coverage_description;
                    if (ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id > 0)
                    {
                        // PIR 9115
                        if (istrIsPIR9115Enabled == busConstant.Flag_No)
                        {
                            busWSSHelper.PublishESSMessage(0, 0, string.Format(busGlobalFunctions.GetDBMessageTextByDBMessageID(26, iobjPassInfo, ref lstrPrioityValue),
                                ibusPerson.icdoPerson.FullName, ibusPerson.icdoPerson.LastFourDigitsOfSSN, ibusPlan.icdoPlan.plan_name, lstrCoverageCodeOrLevelOfCoverage,
                                icdoPersonAccountGhdv.MonthlyPremiumAmount, icdoPersonAccount.history_change_date.ToString("d", CultureInfo.CreateSpecificCulture("en-US"))),
                                lstrPrioityValue, aintPlanID: icdoPersonAccount.plan_id, aintOrgID: ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id,
                                astrContactRoleValue: busConstant.OrgContactRoleAuthorizedAgent);
                        }
                        else
                        {
                            DataTable ldtPersonEmploymentCount = DBFunction.DBSelect("cdoPersonEmployment.CountOfOpenEmployments",
                                                    new object[1] { ibusPerson.icdoPerson.person_id }
                                                    , iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                            // PIR 11309 - dual employment scenario
                            if (ldtPersonEmploymentCount.Rows.Count > 1)
                            {
                                int lintOrgId = 0;
                                Collection<busPersonEmployment> lclbPersonEmployment = new Collection<busPersonEmployment>();

                                lclbPersonEmployment = GetCollection<busPersonEmployment>(ldtPersonEmploymentCount, "icdoPersonEmployment");
                                lclbPersonEmployment = busGlobalFunctions.Sort<busPersonEmployment>("icdoPersonEmployment.start_date desc", lclbPersonEmployment);
                                foreach (busPersonEmployment lobjPersonEmployment in lclbPersonEmployment)
                                {
                                    if (busGlobalFunctions.CheckDateOverlapping(icdoPersonAccount.history_change_date, lobjPersonEmployment.icdoPersonEmployment.start_date,
                                        lobjPersonEmployment.icdoPersonEmployment.end_date))
                                    {
                                        lintOrgId = lobjPersonEmployment.icdoPersonEmployment.org_id;
                                        break;
                                    }
                                }
                                busGlobalFunctions.PostESSMessage(lintOrgId, icdoPersonAccount.plan_id, iobjPassInfo);
                            }
                            else
                            {
                                busGlobalFunctions.PostESSMessage(ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id,
                                                                icdoPersonAccount.plan_id, iobjPassInfo);
                            }
                        }
                    }
                }
                else
                {
                    if (ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id > 0)
                    {
                        // PIR 9115
                        if (istrIsPIR9115Enabled == busConstant.Flag_No)
                        {
                            busWSSHelper.PublishESSMessage(0, 0, string.Format(busGlobalFunctions.GetDBMessageTextByDBMessageID(25, iobjPassInfo, ref lstrPrioityValue),
                                ibusPerson.icdoPerson.FullName, ibusPerson.icdoPerson.LastFourDigitsOfSSN, ibusPlan.icdoPlan.plan_name, icdoPersonAccount.history_change_date.ToString("d", CultureInfo.CreateSpecificCulture("en-US"))),
                                lstrPrioityValue, aintPlanID: icdoPersonAccount.plan_id, aintOrgID: ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id,
                                    astrContactRoleValue: busConstant.OrgContactRoleAuthorizedAgent);
                        }
                        else
                        {
                            busGlobalFunctions.PostESSMessage(ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id,
                                                                                        icdoPersonAccount.plan_id, iobjPassInfo);
                        }
                    }
                }
            }
        }

        public void RefreshValues()
        {
            icdoPersonAccount.suppress_warnings_flag = string.Empty; // UAT PIR ID 1015
            icdoPersonAccountGhdv.reason_value = string.Empty;  // UAT PIR ID 1043
        }

        public void ProcessHistory()
        {
            Collection<busPersonAccountGhdvHistory> lclbDeletedGHDVHistory = new Collection<busPersonAccountGhdvHistory>();//PIR 23148

            if ((icdoPersonAccount.status_value == "VALD") && (IsHistoryEntryRequired || IsCurrentPaymentMethodPensionCheck)) //8022
            {
                //Remove the Overlapping History
                if (iclbOverlappingHistory != null && iclbOverlappingHistory.Count > 0)
                {
                    foreach (busPersonAccountGhdvHistory lbusPAGhdvHistory in iclbOverlappingHistory)
                    {
                        lclbDeletedGHDVHistory.Add(lbusPAGhdvHistory);//PIR 23148
                        lbusPAGhdvHistory.Delete();
                    }
                    //PIR 23167, 23340, 23408
                    if (IsResourceForEnhancedOverlap(busConstant.OverlapResourceEnhancedGHDV))
                    {
                        bool lblnIsPersonAccountModified = false;
                        if (icdoPersonAccount.start_date > icdoPersonAccount.history_change_date)
                        {
                            icdoPersonAccount.start_date = icdoPersonAccount.history_change_date;
                            lblnIsPersonAccountModified = true;
                            
                        }
                        if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled &&
                            icdoPersonAccount.end_date != DateTime.MinValue)
                        {
                            icdoPersonAccount.end_date = DateTime.MinValue;
                            lblnIsPersonAccountModified = true;
                        }

                        if (lblnIsPersonAccountModified)
                            icdoPersonAccount.Update();
                    }
                }

                if (_ibusHistory == null)
                    LoadPreviousHistory();

                //If the Current Record is Getting End Dated, We should not create New History Entry. 
                //We Just need to Update the Previous History Entry

                //If the History is already End Dated and the New Record is now removing End Date, Then 
                //We should not update the Previous History End Date. We Just need to Create the New History Record Only.
                //Alright.. Enough Comments. Lets Code it. :-)

                if (ibusHistory.icdoPersonAccountGhdvHistory.person_account_ghdv_history_id > 0)
                {
                    //PIR 23148
                    if (!lclbDeletedGHDVHistory.Any(i => i.icdoPersonAccountGhdvHistory.person_account_ghdv_history_id == ibusHistory.icdoPersonAccountGhdvHistory.person_account_ghdv_history_id))
                    {
                        if (ibusHistory.icdoPersonAccountGhdvHistory.start_date == icdoPersonAccount.history_change_date)
                        {
                            ibusHistory.icdoPersonAccountGhdvHistory.end_date = icdoPersonAccount.history_change_date;
                            // Set flag to 'Y' so that ESS Benefit Enrollment report will ignore these records
                            //ibusHistory.icdoPersonAccountGhdvHistory.is_enrollment_report_generated = busConstant.Flag_Yes;//PIR 17081
                        }
                        else
                        {
                            ibusHistory.icdoPersonAccountGhdvHistory.end_date = icdoPersonAccount.history_change_date.AddDays(-1);
                        }

                        // PIR 12193 - logged on 06/19/2014
                        // If the enrollment and suspension happens on the same day, we should send the ‘suspension’ to Benefit enrollment report 
                        // only if we already sent the ‘enrollment’ to Benefit report.
                        if ((icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended ||
                            icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceCancelled)
                            && (ibusHistory.icdoPersonAccountGhdvHistory.end_date == ibusHistory.icdoPersonAccountGhdvHistory.start_date)
                            && (ibusHistory.icdoPersonAccountGhdvHistory.is_enrollment_report_generated == busConstant.Flag_Yes))
                        {
                            //ibusHistory.icdoPersonAccountGhdvHistory.is_enrollment_report_generated = busConstant.Flag_No;//PIR 17081
                        }

                        ibusHistory.icdoPersonAccountGhdvHistory.Update();

                        //PIR 1729 : Update the End Date with Previous History End Date if the Plan Participation Status is other than Enrolled
                        //Reset the End Date with MinValue if the plan participation status is enrolled
                        if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                        {
                            icdoPersonAccount.end_date = DateTime.MinValue;
                        }
                        else
                        {
                            icdoPersonAccount.end_date = ibusHistory.icdoPersonAccountGhdvHistory.end_date;
                        }
                        icdoPersonAccount.Update();
                    }
                }

                //Always Insert New History Whenever HistoryEntry Required Flag is Set
                InsertHistory();
            }
        }

        public void LoadPreviousHistory()
        {
            if (_ibusHistory == null)
            {
                _ibusHistory = new busPersonAccountGhdvHistory();
                _ibusHistory.icdoPersonAccountGhdvHistory = new cdoPersonAccountGhdvHistory();
            }

            if (iclbPersonAccountGHDVHistory == null)
                LoadPersonAccountGHDVHistory();

            if (iclbPersonAccountGHDVHistory.Count > 0)
                ibusHistory = iclbPersonAccountGHDVHistory.First();
        }

        public void LoadPreviousHistoryExcludeCancelledPeriods()
        {
            if (_ibusHistory == null)
            {
                _ibusHistory = new busPersonAccountGhdvHistory();
                _ibusHistory.icdoPersonAccountGhdvHistory = new cdoPersonAccountGhdvHistory();
            }

            if (iclbPersonAccountGHDVHistory == null)
                LoadPersonAccountGHDVHistory();

            if (iclbPersonAccountGHDVHistory.Count > 0)
                ibusHistory = iclbPersonAccountGHDVHistory.Where(i => i.icdoPersonAccountGhdvHistory.plan_participation_status_value != busConstant.PlanParticipationStatusInsuranceCancelled).First();

        }
        // This method will load the Previous History where the History is Enrolled. 
        // Used in HMO file.
        public void LoadPreviousEnrolledHistory()
        {
            if (ibusEnrollmentHistory.IsNull())
                ibusEnrollmentHistory = new busPersonAccountGhdvHistory { icdoPersonAccountGhdvHistory = new cdoPersonAccountGhdvHistory() };

            if (iclbPersonAccountGHDVHistory == null)
                LoadPersonAccountGHDVHistory();

            if (iclbPersonAccountGHDVHistory.Any(i => i.icdoPersonAccountGhdvHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled))
            {
                ibusEnrollmentHistory = iclbPersonAccountGHDVHistory.First(i => i.icdoPersonAccountGhdvHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled);
            }
        }

        private void SetHistoryEntryRequiredOrNot()
        {
            if (_ibusHistory == null)
                LoadPreviousHistory();
            /* UAT PIR 476, Including other and JS RHIC Amount */
            /* UAT PIR 476 ends here */
            if ((icdoPersonAccount.plan_participation_status_value != ibusHistory.icdoPersonAccountGhdvHistory.plan_participation_status_value) ||
             (icdoPersonAccount.current_plan_start_date != ibusHistory.icdoPersonAccountGhdvHistory.start_date) ||
             (icdoPersonAccountGhdv.health_insurance_type_value != ibusHistory.icdoPersonAccountGhdvHistory.health_insurance_type_value) ||
             (icdoPersonAccountGhdv.dental_insurance_type_value != ibusHistory.icdoPersonAccountGhdvHistory.dental_insurance_type_value) ||
             (icdoPersonAccountGhdv.vision_insurance_type_value != ibusHistory.icdoPersonAccountGhdvHistory.vision_insurance_type_value) ||
             (icdoPersonAccountGhdv.hmo_insurance_type_value != ibusHistory.icdoPersonAccountGhdvHistory.hmo_insurance_type_value) ||
             (icdoPersonAccountGhdv.level_of_coverage_value != ibusHistory.icdoPersonAccountGhdvHistory.level_of_coverage_value) ||
             (icdoPersonAccountGhdv.coverage_code != ibusHistory.icdoPersonAccountGhdvHistory.coverage_code) ||
             (icdoPersonAccountGhdv.rate_structure_value != ibusHistory.icdoPersonAccountGhdvHistory.rate_structure_value) ||
             (icdoPersonAccountGhdv.plan_option_value != ibusHistory.icdoPersonAccountGhdvHistory.plan_option_value) ||
             (icdoPersonAccountGhdv.low_income_credit != ibusHistory.icdoPersonAccountGhdvHistory.low_income_credit) ||
             (icdoPersonAccountGhdv.cobra_type_value != ibusHistory.icdoPersonAccountGhdvHistory.cobra_type_value) ||
             (icdoPersonAccountGhdv.epo_org_id != ibusHistory.icdoPersonAccountGhdvHistory.epo_org_id) ||
             (icdoPersonAccountGhdv.medicare_claim_no != ibusHistory.icdoPersonAccountGhdvHistory.medicare_claim_no) ||
             (icdoPersonAccountGhdv.medicare_part_a_effective_date != ibusHistory.icdoPersonAccountGhdvHistory.medicare_part_a_effective_date) ||
             (icdoPersonAccountGhdv.medicare_part_b_effective_date != ibusHistory.icdoPersonAccountGhdvHistory.medicare_part_b_effective_date) ||
             (icdoPersonAccountGhdv.premium_conversion_indicator_flag != ibusHistory.icdoPersonAccountGhdvHistory.premium_conversion_indicator_flag) ||
             (icdoPersonAccountGhdv.alternate_structure_code_value != ibusHistory.icdoPersonAccountGhdvHistory.alternate_structure_code_value) //PIR 7705
             )
            {
                IsHistoryEntryRequired = true;
            }
            else
            {
                IsHistoryEntryRequired = false;
            }

            //PIR 26927 - Below scenario does not exists any more as we do not show Cancel option in MSS if plan is suspended. 
            //From LOB if Suspended plan's effective date is updated to prev date (without overlap), validation 5026 was not triggered. 

            ////PIR 12181 - no history entry for already suspended plan to suspended from MSS for insurance plans
            //if (ibusHistory.icdoPersonAccountGhdvHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended 
            //    && icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended)
            //    IsHistoryEntryRequired = false;
        }

        public void GetMonthlyPremiumAmount()
        {
            if (idtPlanEffectiveDate == DateTime.MinValue)
                LoadPlanEffectiveDate();
            GetMonthlyPremiumAmount(idtPlanEffectiveDate);
        }

        public void GetMonthlyPremiumAmount(DateTime adtGivenDate)
        {
            if (ibusHistory == null)
                LoadPreviousHistory();

            switch (ibusPlan.icdoPlan.plan_id)
            {
                case busConstant.PlanIdHMO:
                    string lstrEmpCategory = string.Empty;
                    if ((ibusPersonEmploymentDetail != null) && (ibusPersonEmploymentDetail.ibusPersonEmployment != null) &&
                        (ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization != null))
                        lstrEmpCategory = ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.emp_category_value;

                    icdoPersonAccountGhdv.MonthlyPremiumAmount =
                        busRateHelper.GetHMOPremiumAmount(ibusProviderOrgPlan.icdoOrgPlan.org_plan_id, lstrEmpCategory,
                                                          ibusHistory.icdoPersonAccountGhdvHistory.hmo_insurance_type_value,
                                                          ibusHistory.icdoPersonAccountGhdvHistory.level_of_coverage_value,
                                                          adtGivenDate, idtbCachedHmoRate, iobjPassInfo);
                    break;
                case busConstant.PlanIdDental:
                    icdoPersonAccountGhdv.MonthlyPremiumAmount =
                        busRateHelper.GetDentalPremiumAmount(ibusProviderOrgPlan.icdoOrgPlan.org_plan_id,
                        ibusHistory.icdoPersonAccountGhdvHistory.dental_insurance_type_value,
                        ibusHistory.icdoPersonAccountGhdvHistory.level_of_coverage_value,
                        adtGivenDate, idtbCachedDentalRate, iobjPassInfo);
                    break;
                case busConstant.PlanIdVision:
                    icdoPersonAccountGhdv.MonthlyPremiumAmount =
                        busRateHelper.GetVisionPremiumAmount(ibusProviderOrgPlan.icdoOrgPlan.org_plan_id,
                        ibusHistory.icdoPersonAccountGhdvHistory.vision_insurance_type_value,
                        ibusHistory.icdoPersonAccountGhdvHistory.level_of_coverage_value,
                        adtGivenDate, idtbCachedVisionRate, iobjPassInfo);
                    break;
                default:
                    break;
            }
        }

        public void GetMonthlyPremiumAmountByRefID()
        {
            if (idtPlanEffectiveDate == DateTime.MinValue)
                LoadPlanEffectiveDate();

            GetMonthlyPremiumAmountByRefID(idtPlanEffectiveDate);
        }

        public void GetMonthlyPremiumAmountByRefID(DateTime adtEffectiveDate)
        {

            decimal ldecFeeAmt = 0.00M;
            decimal ldecMedicarePartDAmount = 0.0M;
            decimal ldecHealthSavingsAccount = 0.0M;
            decimal ldecHSAVendorAmount = 0.0M;
            decimal ldecBuydownAmount = 0.0M;
            decimal ldecPremiumAmount =
                busRateHelper.GetHealthPremiumAmount(
                    icdoPersonAccountGhdv.Coverage_Ref_ID, adtEffectiveDate,
                    icdoPersonAccountGhdv.low_income_credit,
                    ref ldecFeeAmt, ref ldecBuydownAmount,
                    ref ldecMedicarePartDAmount, ref ldecHealthSavingsAccount, ref ldecHSAVendorAmount, idtbCachedHealthRate, iobjPassInfo);
            if (icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth)
            {
                icdoPersonAccountGhdv.MonthlyPremiumAmount = ldecPremiumAmount + ldecFeeAmt + ldecHealthSavingsAccount - ldecBuydownAmount + ldecMedicarePartDAmount;// PIR 14271
                icdoPersonAccountGhdv.PremiumExcludingFeeAmount = ldecPremiumAmount;
                icdoPersonAccountGhdv.BuydownAmount = ldecBuydownAmount;
                icdoPersonAccountGhdv.MedicarePartDAmount = ldecMedicarePartDAmount;
            }
            else if (icdoPersonAccount.plan_id == busConstant.PlanIdMedicarePartD)
            {
                icdoPersonAccountGhdv.MonthlyPremiumAmount = ldecMedicarePartDAmount + ldecFeeAmt - ldecBuydownAmount;// PIR 14271
                icdoPersonAccountGhdv.PremiumExcludingFeeAmount = ldecMedicarePartDAmount;
                icdoPersonAccountGhdv.BuydownAmount = ldecBuydownAmount;
                icdoPersonAccountGhdv.MedicarePartDAmount = ldecMedicarePartDAmount;
            }
            icdoPersonAccountGhdv.FeeAmount = ldecFeeAmt;
            //pir 7705
            icdoPersonAccountGhdv.idecHSAAmount = ldecHealthSavingsAccount;
            icdoPersonAccountGhdv.idecHSAVendorAmount = ldecHSAVendorAmount;
            //Calculate RHIC only for IBS Members
            if (ibusPaymentElection == null)
                LoadPaymentElection();
            if (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag == busConstant.Flag_Yes)
            {
                //Load the Rhic Amount as of effective date
                decimal ldecJsRhicAmount = 0.00M;
                decimal ldecOtherRhicAmount = 0.00M;
                icdoPersonAccountGhdv.total_rhic_amount = LoadTotalRhicAllocatedAmount(adtEffectiveDate, ref ldecJsRhicAmount, ref ldecOtherRhicAmount);
                icdoPersonAccountGhdv.js_rhic_amount = ldecJsRhicAmount;
                icdoPersonAccountGhdv.other_rhic_amount = ldecOtherRhicAmount;
            }
        }

        //Initialize Objects to Avoid Null Errors
        public void InitializeObjects()
        {
            ibusPersonEmploymentDetail = new busPersonEmploymentDetail();
            ibusPersonEmploymentDetail.icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail();

            ibusPersonEmploymentDetail.ibusPersonEmployment = new busPersonEmployment();
            ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment = new cdoPersonEmployment();

            ibusOrgPlan = new busOrgPlan();
            ibusOrgPlan.icdoOrgPlan = new cdoOrgPlan();
            ibusOrgPlan.ibusOrganization = new busOrganization();
            ibusOrgPlan.ibusOrganization.icdoOrganization = new cdoOrganization();

            ibusProviderOrgPlan = new busOrgPlan();
            ibusProviderOrgPlan.icdoOrgPlan = new cdoOrgPlan();

            ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization = new busOrganization();
            ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization = new cdoOrganization();
        }

        public void DetermineEnrollmentAndLoadObjects(DateTime adtEffectiveDate, bool ablnNewMode)
        {
            DetermineEntrollmentType(adtEffectiveDate, ablnNewMode);

            if (icdoPersonAccount.person_employment_dtl_id > 0)
            {
                //Dont Do NULL Check here because we initialize Dummy Object above so there wont be any NULL
                LoadPersonEmploymentDetail();
                ibusPersonEmploymentDetail.LoadPersonEmployment();
                ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
                LoadOrgPlan(idtPlanEffectiveDate);
                ibusOrgPlan.ibusOrganization = ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization;
                LoadProviderOrgPlan(idtPlanEffectiveDate);
            }
            else
            {
                //Loading the Provider Org Plan to Store it in Person Account Table
                LoadActiveProviderOrgPlan(adtEffectiveDate);
            }
        }

        // This method will reevaluate the Enrollment Type During Save and Load
        private void DetermineEntrollmentType(DateTime adtEffectiveDate, bool ablnNewMode)
        {
            iblnIsActiveMember = false;
            iblnIsCobraMember = false;
            iblnIsDependentCobra = false;

            if (ibusPerson == null)
                LoadPerson();

            if (ablnNewMode)
            {
                //If employment object is loaded on New Mode, it could be either Active Member Enrollment / Member COBRA Enrollment
                if (icdoPersonAccount.person_employment_dtl_id_from_screen > 0)
                {
                    icdoPersonAccount.person_employment_dtl_id = icdoPersonAccount.person_employment_dtl_id_from_screen;
                    LoadPersonEmploymentDetail();
                    ibusPersonEmploymentDetail.LoadPersonEmployment();

                    if (busGlobalFunctions.CheckDateOverlapping(adtEffectiveDate, ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date,
                        ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date))
                    {
                        iblnIsActiveMember = true;
                    }
                    else
                    {
                        iblnIsCobraMember = true;
                    }
                }
            }
            else
            {
                iblnIsActiveMember = LoadEmploymentDetailByDate(adtEffectiveDate);
                if (!iblnIsActiveMember)
                    iblnIsCobraMember = LoadEmploymentDetailByDate(adtEffectiveDate, true);
            }

            //PROD PIR : 4070 if the member is active, he can not enroll in dependent cobra
            if (!iblnIsActiveMember)
            {
                //Check This member is Eligible for Dependent COBRA
                var lcdoMemberPersonAccount = new cdoPersonAccount();
                //PIR 22945 - If 'Dependent of' is selected then load that members person account.
                iblnIsDependentCobra = ibusPerson.IsDependentCobra(icdoPersonAccount.plan_id, adtEffectiveDate, ref lcdoMemberPersonAccount, icdoPersonAccount.from_person_account_id);

                //If Dep. COBRA, load the Member Employment Detail
                if (iblnIsDependentCobra)
                {
                    // PROD PIR 5674 and 5683 and 5774 :This Validation “6680 Health Insurance Type must match with Member Enrollment Health Insurance Type!” is firing now 
                    // for Dependent COBRA scenario all the time even when changing to Retiree. Now the system will exclude this validation and enable them to change 
                    // to Retiree provided the ‘Dependent of’ is not selected. Since in IBS Billing batch and Rate change letter request, 
                    // the system will determine the enrollment as Dependent COBRA if ‘Dependent of’ is selected.
                    if (icdoPersonAccount.from_person_account_id > 0)
                    {
                        //Load Member GHDV Object
                        ibusMemberGHDVForDependent = new busPersonAccountGhdv();
                        ibusMemberGHDVForDependent.FindGHDVByPersonAccountID(lcdoMemberPersonAccount.person_account_id);
                        ibusMemberGHDVForDependent.icdoPersonAccount = lcdoMemberPersonAccount;

                        LoadEmploymentDetailByDate(adtEffectiveDate, ibusMemberGHDVForDependent, true, true);
                        icdoPersonAccountGhdv.plan_option_value = ibusMemberGHDVForDependent.icdoPersonAccountGhdv.plan_option_value; //PROD PIR 8237
                        icdoPersonAccountGhdv.plan_option_value = (icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth &&
                            icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeRetiree) ? null
                            : icdoPersonAccountGhdv.plan_option_value; //PIR 20483
                    }
                    else
                    {
                        iblnIsDependentCobra = false;
                        ibusMemberGHDVForDependent = null;
                    }
                }
            }
        }

        /// <summary>
        /// This Method will load person employment detail object based on the given date and person account object.
        /// This Method has overload for person account object, if we dont pass the Person Account object, it will load
        /// the person account object of GHDV (this). This overload will be useful when we need to load the member person account 
        /// for the dependents.
        /// </summary>
        /// <param name="adtEffectiveDate"></param>
        /// <param name="abusPersonAccount"></param>
        /// <param name="ablnCheckCOBRAEmployment"></param>
        public bool LoadEmploymentDetailByDate(DateTime adtEffectiveDate, busPersonAccountGhdv abusPersonAccountGhdv,
            bool ablnCheckCOBRAEmployment, bool ablnIsDependentCOBRA)
        {
            bool lblnResult = false;
            if (abusPersonAccountGhdv.icdoPersonAccount.person_account_id > 0)
            {
                if (abusPersonAccountGhdv.iclbAccountEmploymentDetail == null)
                    abusPersonAccountGhdv.LoadPersonAccountEmploymentDetails();

                //Filter the Records by Plan
                var lenuPAEmpDetailByPlan =
                    abusPersonAccountGhdv.iclbAccountEmploymentDetail.Where(o => o.icdoPersonAccountEmploymentDetail.plan_id == icdoPersonAccount.plan_id)
                    .OrderByDescending(lobj => lobj.icdoPersonAccountEmploymentDetail.person_employment_dtl_id); // PIR 10504

                    if (!lenuPAEmpDetailByPlan.IsNullOrEmpty())
                {
                    foreach (var lbusPAEmpDetail in lenuPAEmpDetailByPlan)
                    {
                        if (lbusPAEmpDetail.ibusEmploymentDetail == null)
                            lbusPAEmpDetail.LoadPersonEmploymentDetail();

                        if (ablnCheckCOBRAEmployment)
                        {
                            // We dont need the Date Check for Dependent COBRA Employment because
                            //Sometimes Member could be active but dependent COBRA comes in. So end_date_no_null < would not be a valid condition
                            //But, that condition needed for COBRA Member Enrollment
                            if (ablnIsDependentCOBRA)
                            {
                                lblnResult = true;
                                icdoPersonAccount.person_employment_dtl_id = lbusPAEmpDetail.ibusEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id;
                                break;
                            }
                            else
                            {
                                if (lbusPAEmpDetail.ibusEmploymentDetail.icdoPersonEmploymentDetail.end_date_no_null < adtEffectiveDate)
                                {
                                    lblnResult = true;
                                    icdoPersonAccount.person_employment_dtl_id = lbusPAEmpDetail.ibusEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            if (busGlobalFunctions.CheckDateOverlapping(adtEffectiveDate,
                                                                   lbusPAEmpDetail.ibusEmploymentDetail.icdoPersonEmploymentDetail.start_date,
                                                                   lbusPAEmpDetail.ibusEmploymentDetail.icdoPersonEmploymentDetail.end_date_no_null))
                            {
                                lblnResult = true;
                                icdoPersonAccount.person_employment_dtl_id = lbusPAEmpDetail.ibusEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id;
                                break;
                            }
                        }
                    }
                }
                //PIR 10763
                else if (icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeRetiree && icdoPersonAccount.from_person_account_id > 0
                        && ablnCheckCOBRAEmployment == true) // If ablnCheckCOBRAEmployment is true, then enrollment in Cobra dependent. 
                {
                    if (ibusPerson.iclbPersonDependentByDependent == null)
                    {
                        ibusPerson.LoadPersonDependentByDependent();
                    }
                    if (ibusPerson.iclbPersonDependentByDependent != null || ibusPerson.iclbPersonDependentByDependent.Count() > 0)
                    {
                        foreach (var lbusPersonDependent in ibusPerson.iclbPersonDependentByDependent)
                        {
                            if (lbusPersonDependent.iclbPersonAccountDependent == null)
                                lbusPersonDependent.LoadPersonAccountDependent();

                            DataTable ldtPersonAccountGHDVHistory = busNeoSpinBase.Select("cdoPersonAccountGhdv.LoadGHDVHistoryForMember",
                                new object[1] { lbusPersonDependent.icdoPersonDependent.dependent_perslink_id });

                            if (lbusPersonDependent.iclbPersonAccountDependent != null || lbusPersonDependent.iclbPersonAccountDependent.Count() > 0)
                            {
                                foreach (var lbusPADependent in lbusPersonDependent.iclbPersonAccountDependent)
                                {
                                    if (lbusPADependent.ibusPersonAccount == null)
                                        lbusPADependent.LoadPersonAccount();
                                    if (lbusPADependent.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth &&
                                        lbusPADependent.icdoPersonAccountDependent.end_date != DateTime.MinValue)
                                    {
                                        if (lbusPADependent.icdoPersonAccountDependent.start_date != lbusPADependent.icdoPersonAccountDependent.end_date)
                                        {
                                            if (icdoPersonAccount.history_change_date == busGlobalFunctions.GetFirstDayofNextMonth(lbusPADependent.icdoPersonAccountDependent.end_date))
                                            {
                                                if (ldtPersonAccountGHDVHistory.IsNotNull() && ldtPersonAccountGHDVHistory.Rows.Count > 0)
                                                {
                                                    if (icdoPersonAccountGhdv.overridden_structure_code == ldtPersonAccountGHDVHistory.Rows[0]["RATE_STRUCTURE_CODE"].ToString())
                                                        lblnResult = false;
                                                    else
                                                        lblnResult = true;
                                                }

                                            }
                                            else
                                                lblnResult = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return lblnResult;
        }

        public bool LoadEmploymentDetailByDate(DateTime adtEffectiveDate, bool ablnCheckCOBRAEmployment)
        {
            return LoadEmploymentDetailByDate(adtEffectiveDate, this, ablnCheckCOBRAEmployment, false);
        }

        private bool LoadEmploymentDetailByDate(DateTime adtEffectiveDate)
        {
            return LoadEmploymentDetailByDate(adtEffectiveDate, this, false, false);
        }

        private bool LoadEmploymentDetailByDate(DateTime adtEffectiveDate, busPersonAccountGhdv abusPersonAccountGhdv)
        {
            return LoadEmploymentDetailByDate(adtEffectiveDate, abusPersonAccountGhdv, false, false);
        }

        //Logic of Date to Get the Employment and Calculating Premium Amount has been changed and the details are mailed on 3/18/2009 after the discussion with RAJ.
        /*************************
         * 1) Member A started the Health Plan on Jan 1981 and the plan is still open.
         *      In this case, System will display the rates as of Today.
         * 2) Member A started the Health Plan on Jan 2000 and Suspended the Plan on May 2009. 
         *      In this case, system will display the rate as of End date of Latest Enrolled Status History Record. (i.e) Apr 2009. 
         * 3) Third Scenario (Future Date Scenario) might be little bit complicated. Let me know your feedback too. 
         *    If the Member starts the plan on Jan 2000 with the Single Coverage and May 2009 he wants to change to Family.
         *      Current Date is Mar 18 2009. But the latest enrolled history record is future date. 
         *      So System will display the rate as of Start Date of Latest Enrolled History Date. (i.e) of May 2009
         * *************************/

        public void LoadPlanEffectiveDate()
        {
            idtPlanEffectiveDate = DateTime.Now;

            //If the Current Participation status is enrolled, Set the Effective Date from History Change Date
            if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
            {
                if (icdoPersonAccount.current_plan_start_date_no_null > DateTime.Now)
                    idtPlanEffectiveDate = icdoPersonAccount.current_plan_start_date_no_null;
                else
                    idtPlanEffectiveDate = DateTime.Now;
            }
            else
            {
                if (iclbPersonAccountGHDVHistory == null)
                    LoadPersonAccountGHDVHistory();

                //By Default the Collection sorted by latest date
                foreach (busPersonAccountGhdvHistory lbusPersonAccountGhdvHistory in iclbPersonAccountGHDVHistory)
                {
                    if (lbusPersonAccountGhdvHistory.icdoPersonAccountGhdvHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                    {
                        if (lbusPersonAccountGhdvHistory.icdoPersonAccountGhdvHistory.end_date == DateTime.MinValue)
                        {
                            //If the Start Date is Future Date, Set it otherwise Current Date will be Start Date of Premium Calc
                            if (lbusPersonAccountGhdvHistory.icdoPersonAccountGhdvHistory.start_date > DateTime.Now)
                            {
                                idtPlanEffectiveDate = lbusPersonAccountGhdvHistory.icdoPersonAccountGhdvHistory.start_date;
                            }
                            else
                            {
                                idtPlanEffectiveDate = DateTime.Now;
                            }
                        }
                        else
                        {
                            idtPlanEffectiveDate = lbusPersonAccountGhdvHistory.icdoPersonAccountGhdvHistory.end_date;
                        }
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// For State / Non State Health Insurance Type, load the date from the employer participation start date
        /// For Retiree Health Insurance Type, Pick from Very first line of Enrolled after the SUSP entry. If there is
        /// no enrolled line after SUSP, pick up the initial plan start date
        /// For Dependent COBRA, we must loop through Member's History
        /// ********************Make Sure DetermineEnrollmentType() Called before using this method *******        
        /// Ref Mail from Satya : 5/6/2009 And also GHDV Revisited Document   
        /// We also need to Consider the Effective Date here. 
        /// For State and NonState, org plan object gets loaded based on effective date.
        /// For Retirees, we need to Consider Effective Date.
        /// </summary>        
        public void LoadHealthParticipationDate()
        {
            if (idtPlanEffectiveDate == DateTime.MinValue)
                LoadPlanEffectiveDate();

            idtHealthParticipationDate = DateTime.Now;
            if ((icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeState) ||
                (icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeNonState))
            {
                //prod pir 6846 : new field to store health participation start date
                if ((ibusOrgPlan != null) && (ibusOrgPlan.icdoOrgPlan.health_participation_start_date != DateTime.MinValue))
                    idtHealthParticipationDate = ibusOrgPlan.icdoOrgPlan.health_participation_start_date;
                else if ((ibusOrgPlan != null) && (ibusOrgPlan.icdoOrgPlan.participation_start_date != DateTime.MinValue))
                    idtHealthParticipationDate = ibusOrgPlan.icdoOrgPlan.participation_start_date;
            }
            else if (icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeRetiree)
            {
                //For Dependent COBRA , load the Member History
                if ((iblnIsDependentCobra) && (ibusMemberGHDVForDependent != null))
                {
                    ibusMemberGHDVForDependent.idtPlanEffectiveDate = idtPlanEffectiveDate;
                    idtHealthParticipationDate = ibusMemberGHDVForDependent.GetEnrollmentDateFromHistory(icdoPersonAccount.start_date);
                }
                else
                {
                    idtHealthParticipationDate = GetEnrollmentDateFromHistory();
                }
            }
        }

        private DateTime GetEnrollmentDateFromHistory(DateTime? adtDependentPlanStartDate = null)
        {
            if (iclbPersonAccountGHDVHistory == null)
                LoadPersonAccountGHDVHistory();

            busPersonAccountGhdvHistory lbusPreviousEnrolledHistory = null;
            DateTime ldtResult = icdoPersonAccount.start_date_no_null;

            foreach (var lbusHistory in iclbPersonAccountGHDVHistory)
            {
                //Ignore the Record if the Start Date and End Date is Same
                if (lbusHistory.icdoPersonAccountGhdvHistory.start_date.Date == lbusHistory.icdoPersonAccountGhdvHistory.end_date.Date) continue;

                //Ignore the Records which are greater than Effective Date
                if (lbusHistory.icdoPersonAccountGhdvHistory.start_date > idtPlanEffectiveDate) continue;

                //Dependent COBRA : we ignore the matching suspended line with dependent plan start date.
                if (adtDependentPlanStartDate != null)
                {
                    if (lbusHistory.icdoPersonAccountGhdvHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended)
                    {
                        if (lbusHistory.icdoPersonAccountGhdvHistory.start_date.Date == adtDependentPlanStartDate) continue;
                    }
                }

                if (lbusHistory.icdoPersonAccountGhdvHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended)
                {
                    //PROD PIR 4430 Always take the previous enrolled entry before the suspended line
                    if (lbusPreviousEnrolledHistory == null)
                    {
                        if (lbusHistory.icdoPersonAccountGhdvHistory.start_date.Date != icdoPersonAccount.history_change_date.Date)
                        {
                            //If no Enrolled line Before.. it will take the history change date.
                            ldtResult = icdoPersonAccount.current_plan_start_date_no_null;
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                if (lbusHistory.icdoPersonAccountGhdvHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                {
                    lbusPreviousEnrolledHistory = lbusHistory;
                    ldtResult = lbusPreviousEnrolledHistory.icdoPersonAccountGhdvHistory.start_date;
                }
            }
            return ldtResult;
        }


        private DateTime idtHealthParticipationDate { get; set; }

        public bool IsRateStructureEditable()
        {
            bool lblnFlag = false;

            if (idtPlanEffectiveDate == DateTime.MinValue)
                LoadPlanEffectiveDate();

            LoadOrgPlan(idtPlanEffectiveDate);

            if (ibusOrgPlan.icdoOrgPlan.org_plan_id == 0)
                if (ibusPaymentElection != null)
                    if (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag == busConstant.Flag_Yes)
                        lblnFlag = true;
            return lblnFlag;
        }

        public void LoadRateStructure()
        {
            if (idtPlanEffectiveDate == DateTime.MinValue)
                LoadPlanEffectiveDate();

            LoadRateStructure(idtPlanEffectiveDate);
        }

        public void LoadRateStructure(DateTime adtEffectiveDate)
        {
            if (idtHealthParticipationDate == DateTime.MinValue)
                LoadHealthParticipationDate();

            if ((!string.IsNullOrEmpty(icdoPersonAccountGhdv.health_insurance_type_value)))
            {
                if (idtbCachedRateRef == null)
                {
                    idtbCachedRateRef = busGlobalFunctions.LoadHealthRateRefCacheData(iobjPassInfo);
                }

                if (idtbCachedRateStructureRef == null)
                {
                    idtbCachedRateStructureRef = busGlobalFunctions.LoadHealthRateStructureCacheData(iobjPassInfo);
                }

                //prod pir 7029 : new rate structure added at organization level, so need to determine ratestructure based on that
                //string lstrGrandFatherRate = icdoPersonAccountGhdv.grand_father_rate_value; //PROD PIR 7705
                //if ((icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeNonState ||
                //    icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeState) &&
                //    !string.IsNullOrEmpty(lstrGrandFatherRate))
                //{
                //    if (idtPlanEffectiveDate == DateTime.MinValue)
                //        LoadPlanEffectiveDate();
                //    if (ibusOrgPlan == null)
                //        LoadOrgPlan(idtPlanEffectiveDate);

                //    lstrGrandFatherRate = ibusOrgPlan.icdoOrgPlan.grand_father_rate_value;
                //}

                string lstrGrandFatherRate = null;
                if (icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeNonState ||
                    icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeState)
                {
                    if (idtPlanEffectiveDate == DateTime.MinValue)
                        LoadPlanEffectiveDate();
                    if (ibusOrgPlan == null)
                        LoadOrgPlan(idtPlanEffectiveDate);

                    //PIR 19905
                    if (icdoPersonAccountGhdv.cobra_type_value.IsNotNull() && ibusOrgPlan.icdoOrgPlan.org_plan_id == 0)
                    {

                        LoadingRateStructureCodeForCobra();
                    }

                    lstrGrandFatherRate = ibusOrgPlan.icdoOrgPlan.alternate_structure_code_value;
                }
                else
                {
                    lstrGrandFatherRate = icdoPersonAccountGhdv.alternate_structure_code_value;
                }

                //pir 7705
                if (icdoPersonAccountGhdv.alternate_structure_code_value == busConstant.AlternateStructureCodeHDHP)
                    lstrGrandFatherRate = icdoPersonAccountGhdv.alternate_structure_code_value;


                //Get the Latest Rate Structure Value
                icdoPersonAccountGhdv.rate_structure_value = String.Empty;
                DataTable ldtbResult;

                //PIR 11192
                if (istrLowIncomeCreditFlag == busConstant.Flag_Yes)
                {
                    var lenuList = from row in idtbCachedRateStructureRef.AsEnumerable()
                                   where row.Field<DateTime>("effective_date") <= adtEffectiveDate &&
                                         busGlobalFunctions.CheckDateOverlapping(idtHealthParticipationDate,
                                                                                 row.Field<DateTime?>("enrollment_date_from"),
                                                                                 row.Field<DateTime?>("enrollment_date_to")) &&
                                         row.Field<string>("health_insurance_type_value") == icdoPersonAccountGhdv.health_insurance_type_value &&
                                         row.Field<string>("low_income") == "0" &&
                                         row.Field<string>("alternate_structure_code_value") == lstrGrandFatherRate
                                   select row;
                    ldtbResult = lenuList.AsDataTable();
                    if (ldtbResult.Rows.Count > 0)
                        icdoPersonAccountGhdv.rate_structure_value = ldtbResult.Rows[0].Field<string>("rate_structure_value");

                    //Get the Rate Ref ID and Rate Structure Value
                    icdoPersonAccountGhdv.Rate_Ref_ID = 0;
                    icdoPersonAccountGhdv.rate_structure_code = string.Empty;

                    lenuList = from row in idtbCachedRateRef.AsEnumerable()
                               where row.Field<string>("wellness") == WellnessFlag &&
                                     row.Field<string>("health_insurance_type_value") == icdoPersonAccountGhdv.health_insurance_type_value &&
                                     row.Field<string>("plan_option_value") == icdoPersonAccountGhdv.plan_option_value &&
                                     row.Field<string>("low_income") == "0" &&
                                     row.Field<string>("rate_structure_value") == icdoPersonAccountGhdv.rate_structure_value &&
                                     row.Field<string>("alternate_structure_code_value") == lstrGrandFatherRate
                               select row;

                    ldtbResult = lenuList.AsDataTable();
                }
                else
                {
                    var lenuList = from row in idtbCachedRateStructureRef.AsEnumerable()
                                   where row.Field<DateTime>("effective_date") <= adtEffectiveDate &&
                                         busGlobalFunctions.CheckDateOverlapping(idtHealthParticipationDate,
                                                                                 row.Field<DateTime?>("enrollment_date_from"),
                                                                                 row.Field<DateTime?>("enrollment_date_to")) &&
                                         row.Field<string>("health_insurance_type_value") == icdoPersonAccountGhdv.health_insurance_type_value &&
                                         row.Field<string>("low_income") == LowIncome &&
                                         row.Field<string>("alternate_structure_code_value") == lstrGrandFatherRate
                                   select row;
                    ldtbResult = lenuList.AsDataTable();
                    if (ldtbResult.Rows.Count > 0)
                        icdoPersonAccountGhdv.rate_structure_value = ldtbResult.Rows[0].Field<string>("rate_structure_value");

                    //Get the Rate Ref ID and Rate Structure Value
                    icdoPersonAccountGhdv.Rate_Ref_ID = 0;
                    icdoPersonAccountGhdv.rate_structure_code = string.Empty;

                    lenuList = from row in idtbCachedRateRef.AsEnumerable()
                               where row.Field<string>("wellness") == WellnessFlag &&
                                     row.Field<string>("health_insurance_type_value") == icdoPersonAccountGhdv.health_insurance_type_value &&
                                     row.Field<string>("plan_option_value") == icdoPersonAccountGhdv.plan_option_value &&
                                     row.Field<string>("low_income") == LowIncome &&
                                     row.Field<string>("rate_structure_value") == icdoPersonAccountGhdv.rate_structure_value &&
                                     row.Field<string>("alternate_structure_code_value") == lstrGrandFatherRate
                               select row;

                    ldtbResult = lenuList.AsDataTable();
                }
                if (ldtbResult.Rows.Count > 0)
                {
                    icdoPersonAccountGhdv.Rate_Ref_ID = ldtbResult.Rows[0].Field<int>("org_plan_group_health_medicare_part_d_rate_ref_id");
                    icdoPersonAccountGhdv.rate_structure_code = ldtbResult.Rows[0].Field<string>("rate_structure_code");
                }
            }
        }

        public void LoadRateStructureForUserStructureCode()
        {
            icdoPersonAccountGhdv.Rate_Ref_ID = 0;

            if (idtbCachedRateRef == null)
            {
                idtbCachedRateRef = busGlobalFunctions.LoadHealthRateRefCacheData(iobjPassInfo);
            }

            // PROD PIR 8914
            string lstrGrandFatherRate = null;
            if (icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeNonState ||
                icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeState)
            {
                if (idtPlanEffectiveDate == DateTime.MinValue)
                    LoadPlanEffectiveDate();
                if (ibusOrgPlan == null)
                    LoadOrgPlan(idtPlanEffectiveDate);

                //PIR 19905
                if (icdoPersonAccountGhdv.cobra_type_value.IsNotNull() && ibusOrgPlan.icdoOrgPlan.org_plan_id == 0)
                {
                    LoadingRateStructureCodeForCobra();
                }

                lstrGrandFatherRate = ibusOrgPlan.icdoOrgPlan.alternate_structure_code_value;
            }
            else
            {
                lstrGrandFatherRate = icdoPersonAccountGhdv.alternate_structure_code_value;
            }
            if (icdoPersonAccountGhdv.alternate_structure_code_value == busConstant.AlternateStructureCodeHDHP)
                lstrGrandFatherRate = icdoPersonAccountGhdv.alternate_structure_code_value;

            var lenuList = from row in idtbCachedRateRef.AsEnumerable()
                           where row.Field<string>("rate_structure_code") == icdoPersonAccountGhdv.overridden_structure_code &&
                             row.Field<string>("alternate_structure_code_value") == lstrGrandFatherRate
                           select row;

            DataTable ldtbResult = lenuList.AsDataTable();
            if (ldtbResult.Rows.Count > 0)
            {
                if (ldtbResult.Rows.Count == 1)
                {
                    icdoPersonAccountGhdv.Rate_Ref_ID = ldtbResult.Rows[0].Field<int>("org_plan_group_health_medicare_part_d_rate_ref_id");
                }
                else
                {
                    //Filter the Rows based on the Plans (Health or Medicare) (RT09 / RT12)
                    foreach (DataRow row in ldtbResult.Rows)
                    {
                        if (row.Field<string>("health_insurance_type_value") == busConstant.HealthInsuranceTypeRetiree)
                        {
                            if (icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth)
                            {
                                icdoPersonAccountGhdv.Rate_Ref_ID = row.Field<int>("org_plan_group_health_medicare_part_d_rate_ref_id");
                                break;
                            }
                        }
                        else
                        {
                            if (icdoPersonAccount.plan_id == busConstant.PlanIdMedicarePartD)
                            {
                                icdoPersonAccountGhdv.Rate_Ref_ID = row.Field<int>("org_plan_group_health_medicare_part_d_rate_ref_id");
                                break;
                            }
                        }
                    }
                }
            }
        }

        public void LoadCoverageRefID()
        {
            if ((icdoPersonAccountGhdv.Rate_Ref_ID > 0) &&
                (icdoPersonAccountGhdv.coverage_code != null))
            {
                //Except State Member Enrollment, All other Cases This should be Permanent
                string lstrEmploymentType = busConstant.PersonJobTypePermanent;

                //Sometimes Job Type could Be Null.. Such Cases pass Permanent Only
                if (icdoPersonAccountGhdv.employment_type_value.IsNotNullOrEmpty() && (icdoPersonAccountGhdv.employment_type_value == busConstant.PersonJobTypeTemporary))
                    lstrEmploymentType = icdoPersonAccountGhdv.employment_type_value;


                if (idtbCachedCoverageRef == null)
                {
                    idtbCachedCoverageRef = busGlobalFunctions.LoadHealthCoverageRefCacheData(iobjPassInfo);
                }                

                icdoPersonAccountGhdv.Coverage_Ref_ID = 0;

                var lenuList = from row in idtbCachedCoverageRef.AsEnumerable()
                               where row.Field<int>("org_plan_group_health_medicare_part_d_rate_ref_id") == icdoPersonAccountGhdv.Rate_Ref_ID &&
                                 row.Field<string>("coverage_code") == icdoPersonAccountGhdv.coverage_code &&
                                 row.Field<string>("cobra_in") == COBRAIn &&
                                 row.Field<string>("employment_type_value") == lstrEmploymentType
                               select row;

                DataTable ldtbResult = lenuList.AsDataTable();
                if (ldtbResult.Rows.Count == 1)
                {
                    icdoPersonAccountGhdv.Coverage_Ref_ID = ldtbResult.Rows[0].Field<int>("org_plan_group_health_medicare_part_d_coverage_ref_id");
                    //uat pir - 1422
                    //this property is used in correpondence PAY-4301 & PAY-4302
                    //instead of writing new method, used the same method to assign a different col. in the same row
                    icdoPersonAccountGhdv.iintMedicareIn = Convert.ToInt32(ldtbResult.Rows[0].Field<string>("medicare_in"));
                }
                // PIR 11312
                else if (ldtbResult.Rows.Count > 1)
                {
                    if (idtPlanEffectiveDate == DateTime.MinValue)
                        LoadPlanEffectiveDate();

                    if (idtbCachedHealthRate == null)
                    {
                        idtbCachedHealthRate = busGlobalFunctions.LoadHealthRateCacheData(iobjPassInfo);
                    }
                    for (int i = 0; i < ldtbResult.Rows.Count; i++)
                    {
                        int lintCoverageRefId = ldtbResult.Rows[i].Field<int>("org_plan_group_health_medicare_part_d_coverage_ref_id");

                        var lenuListNew = from row in idtbCachedHealthRate.AsEnumerable()
                                        where row.Field<int>("org_plan_group_health_medicare_part_d_coverage_ref_id") == lintCoverageRefId &&
                                         busGlobalFunctions.CheckDateOverlapping(idtPlanEffectiveDate,
                                                                                 row.Field<DateTime?>("premium_period_start_date"),
                                                                                 row.Field<DateTime?>("premium_period_end_date"))
                                        select row;

                        if (lenuListNew.Count() > 0)
                        {
                            icdoPersonAccountGhdv.Coverage_Ref_ID = lintCoverageRefId;
                            icdoPersonAccountGhdv.iintMedicareIn = Convert.ToInt32(ldtbResult.Rows[i].Field<string>("medicare_in"));
                            break;
                        }
                    }
                    
                }
            }
        }

        // PIR 11237
        public void LoadPreviousCoverageRefID()
        {
            if ((icdoPersonAccountGhdv.Rate_Ref_ID > 0) &&
                (icdoPersonAccountGhdv.coverage_code != null))
            {
                //Except State Member Enrollment, All other Cases This should be Permanent
                string lstrEmploymentType = busConstant.PersonJobTypePermanent;

                //Sometimes Job Type could Be Null.. Such Cases pass Permanent Only
                if (icdoPersonAccountGhdv.employment_type_value.IsNotNullOrEmpty() && (icdoPersonAccountGhdv.employment_type_value == busConstant.PersonJobTypeTemporary))
                    lstrEmploymentType = icdoPersonAccountGhdv.employment_type_value;


                if (idtbCachedCoverageRef == null)
                {
                    idtbCachedCoverageRef = busGlobalFunctions.LoadHealthCoverageRefCacheData(iobjPassInfo);
                }

                icdoPersonAccountGhdv.Coverage_Ref_ID = 0;

                var lenuList = from row in idtbCachedCoverageRef.AsEnumerable()
                               where row.Field<int>("org_plan_group_health_medicare_part_d_rate_ref_id") == icdoPersonAccountGhdv.Rate_Ref_ID &&
                                 row.Field<string>("coverage_code") == icdoPersonAccountGhdv.coverage_code &&
                                 row.Field<string>("cobra_in") == COBRAIn &&
                                 row.Field<string>("employment_type_value") == lstrEmploymentType
                               select row;

                DataTable ldtbResult = lenuList.AsDataTable();
                if (ldtbResult.Rows.Count > 1)
                {
                    icdoPersonAccountGhdv.Coverage_Ref_ID = ldtbResult.Rows[1].Field<int>("org_plan_group_health_medicare_part_d_coverage_ref_id");

                    icdoPersonAccountGhdv.iintMedicareIn = Convert.ToInt32(ldtbResult.Rows[1].Field<string>("medicare_in"));
                }
                else if (ldtbResult.Rows.Count == 1)
                {
                    icdoPersonAccountGhdv.Coverage_Ref_ID = ldtbResult.Rows[0].Field<int>("org_plan_group_health_medicare_part_d_coverage_ref_id");

                    icdoPersonAccountGhdv.iintMedicareIn = Convert.ToInt32(ldtbResult.Rows[0].Field<string>("medicare_in"));
                }
            }
        }


        //PIR 1758
        public Collection<cdoLowIncomeCreditRef> iclbLowIncomeCreditRef { get; set; }
        public Collection<cdoLowIncomeCreditRef> LoadLowIncomeCreditRef()
        {
            if (idtbCachedLowIncomeCredit == null)
                idtbCachedLowIncomeCredit = busGlobalFunctions.LoadLowIncomeCreditRefCacheData(iobjPassInfo);
            if (idtPlanEffectiveDate == null)
                LoadPlanEffectiveDate();
            DateTime ldtEffectiveDate = new DateTime();
            var lenumList = idtbCachedLowIncomeCredit.AsEnumerable().OrderByDescending(i => i.Field<DateTime>("effective_date"));
            foreach (DataRow dr in lenumList)
            {
                if (Convert.ToDateTime(dr["effective_date"]).Date <= idtPlanEffectiveDate.Date)
                {
                    ldtEffectiveDate = Convert.ToDateTime(dr["effective_date"]).Date;
                    break;
                }
            }
            DataTable ldtFilteredLowIncomeCredit = idtbCachedLowIncomeCredit.AsEnumerable().Where(i => i.Field<DateTime>("effective_date") == ldtEffectiveDate.Date).AsDataTable();

            iclbLowIncomeCreditRef = Sagitec.DataObjects.doBase.GetCollection<cdoLowIncomeCreditRef>(ldtFilteredLowIncomeCredit);
            iclbLowIncomeCreditRef.ForEach(i => i.display_credit = i.low_income_credit.ToString());
            ////Adding Empty Item Here since Framework has bug if you select the Last Item. Temporary Workaround
            var lcdoTempRef = new cdoLowIncomeCreditRef();
            lcdoTempRef.amount = 0;
            lcdoTempRef.low_income_credit = 0;
            lcdoTempRef.display_credit = string.Empty;
            iclbLowIncomeCreditRef.Add(lcdoTempRef);
            iclbLowIncomeCreditRef.OrderBy(i => i.low_income_credit);
            return iclbLowIncomeCreditRef;

        }
        public Collection<cdoOrgPlanGroupHealthMedicarePartDCoverageRef> iclbCoverageRef { get; set; }
        public Collection<cdoOrgPlanGroupHealthMedicarePartDCoverageRef> LoadCoverageCodeByFilter()
        {
            if (idtbCachedRateRef == null)
                idtbCachedRateRef = busGlobalFunctions.LoadHealthRateRefCacheData(iobjPassInfo);

            if (idtbCachedCoverageRef == null)
                idtbCachedCoverageRef = busGlobalFunctions.LoadHealthCoverageRefCacheData(iobjPassInfo);

            Collection<cdoOrgPlanGroupHealthMedicarePartDRateRef> lclbRateRef = new Collection<cdoOrgPlanGroupHealthMedicarePartDRateRef>();
            lclbRateRef = Sagitec.DataObjects.doBase.GetCollection<cdoOrgPlanGroupHealthMedicarePartDRateRef>(idtbCachedRateRef);

            Collection<cdoOrgPlanGroupHealthMedicarePartDCoverageRef> lclbCoverageRef = new Collection<cdoOrgPlanGroupHealthMedicarePartDCoverageRef>();
            lclbCoverageRef = Sagitec.DataObjects.doBase.GetCollection<cdoOrgPlanGroupHealthMedicarePartDCoverageRef>(idtbCachedCoverageRef);

            var lclbList = lclbCoverageRef.Join(lclbRateRef,
                                                c => c.org_plan_group_health_medicare_part_d_rate_ref_id,
                                                r => r.org_plan_group_health_medicare_part_d_rate_ref_id,
                                                (c, r) => new
                                                {
                                                    c.coverage_code,
                                                    c.description,
                                                    c.short_description,
                                                    c.org_plan_group_health_medicare_part_d_rate_ref_id,
                                                    c.org_plan_group_health_medicare_part_d_coverage_ref_id,
                                                    c.employment_type_value,
                                                    c.cobra_in,
                                                    r.rate_structure_code,
                                                    r.health_insurance_type_value,
                                                    r.rate_structure_value,
                                                    r.plan_option_value,
                                                    r.wellness,
                                                    r.low_income
                                                });

            //Filter based on the values           
            if (icdoPersonAccountGhdv.Rate_Ref_ID > 0)
                lclbList = lclbList.Where(o => o.org_plan_group_health_medicare_part_d_rate_ref_id == icdoPersonAccountGhdv.Rate_Ref_ID);

            if (icdoPersonAccountGhdv.employment_type_value != null)
                lclbList = lclbList.Where(o => o.employment_type_value == icdoPersonAccountGhdv.employment_type_value);

            lclbList = lclbList.Where(o => o.cobra_in == COBRAIn);

            if (icdoPersonAccountGhdv.overridden_structure_code.IsNullOrEmpty())
            {
                if (icdoPersonAccountGhdv.rate_structure_code.IsNotNullOrEmpty())
                    lclbList = lclbList.Where(o => o.rate_structure_code == icdoPersonAccountGhdv.rate_structure_code);

                if (icdoPersonAccountGhdv.health_insurance_type_value != null)
                    lclbList = lclbList.Where(o => o.health_insurance_type_value == icdoPersonAccountGhdv.health_insurance_type_value);

                if (icdoPersonAccountGhdv.rate_structure_value.IsNotNullOrEmpty())
                    lclbList = lclbList.Where(o => o.rate_structure_value == icdoPersonAccountGhdv.rate_structure_value);

                lclbList = lclbList.Where(o => o.plan_option_value == icdoPersonAccountGhdv.plan_option_value);

                lclbList = lclbList.Where(o => o.wellness == WellnessFlag);

                //PIR 11192
                if(istrLowIncomeCreditFlag == busConstant.Flag_Yes)
                    lclbList = lclbList.Where(o => o.low_income == "0");
                else
                    lclbList = lclbList.Where(o => o.low_income == LowIncome);
            }

            //PIR 18354
            lclbList = lclbList.OrderBy(i => i.coverage_code);

            // PIR 11312
            if (idtPlanEffectiveDate == DateTime.MinValue)
                LoadPlanEffectiveDate();

            // PIR 11312
            if (idtbCachedHealthRate == null)
                idtbCachedHealthRate = busGlobalFunctions.LoadHealthRateCacheData(iobjPassInfo);            

            iclbCoverageRef = new Collection<cdoOrgPlanGroupHealthMedicarePartDCoverageRef>();
            foreach (var result in lclbList)
            {
                cdoOrgPlanGroupHealthMedicarePartDCoverageRef lcdoRef = new cdoOrgPlanGroupHealthMedicarePartDCoverageRef();
                lcdoRef.coverage_code = result.coverage_code;
                lcdoRef.short_description = result.coverage_code + " - " + result.short_description;
                lcdoRef.description = result.coverage_code + " - " + result.description;
                // PIR 11312
                var lenuList = from row in idtbCachedHealthRate.AsEnumerable()
                               where row.Field<int>("org_plan_group_health_medicare_part_d_coverage_ref_id") == result.org_plan_group_health_medicare_part_d_coverage_ref_id &&
                                busGlobalFunctions.CheckDateOverlapping(DateTime.Now,
                                                                        row.Field<DateTime?>("premium_period_start_date"),
                                                                        row.Field<DateTime?>("premium_period_end_date"))
                               select row;
                if ((lenuList.Count() > 0)) // PIR 11312
                {
                    iclbCoverageRef.Add(lcdoRef);
                }            
            }

            //Adding Empty Item Here since Framework has bug if you select the Last Item. Temporary Workaround
            iclbCoverageRef =  iclbCoverageRef.Distinct(new cdoOrgPlanGroupHealthEquality()).ToList().ToCollection();
            var lcdoTempRef = new cdoOrgPlanGroupHealthMedicarePartDCoverageRef();
            lcdoTempRef.coverage_code = null;
            lcdoTempRef.short_description = string.Empty;
            iclbCoverageRef.Add(lcdoTempRef);
            return iclbCoverageRef;
        }

        /// <summary>
        /// Reload the Coverage Code based on the Screen Data
        /// </summary>
        /// <returns></returns>
        public ArrayList btnReloadCoverageCodeList_Click()
        {
            ArrayList larrList = new ArrayList();
            //Reload Plan Effective Date
            LoadPlanEffectiveDate();

            // UAT PIR ID 1069 - Overridden Structure Code should be changed if the Values are changed.
            if (!string.IsNullOrEmpty(icdoPersonAccountGhdv.overridden_structure_code) && string.IsNullOrEmpty(icdoPersonAccountGhdv.cobra_type_value))
            {
                if (icdoPersonAccountGhdv.ihstOldValues.Count > 0)
                {
                    if ((Convert.ToString(icdoPersonAccountGhdv.ihstOldValues["health_insurance_type_value"]) != icdoPersonAccountGhdv.health_insurance_type_value) ||
                        (Convert.ToString(icdoPersonAccountGhdv.ihstOldValues["plan_option_value"]) != icdoPersonAccountGhdv.plan_option_value) ||
                        (Convert.ToDecimal(icdoPersonAccountGhdv.ihstOldValues["low_income_credit"]) != icdoPersonAccountGhdv.low_income_credit))
                        icdoPersonAccountGhdv.overridden_structure_code = string.Empty;
                }
            }

            //Determine the Enrollment Type based on the History Change Date Given (This is useful for Future End dated Employment Records)
            if (icdoPersonAccount.ienuObjectState == ObjectState.Insert)
                DetermineEnrollmentAndLoadObjects(icdoPersonAccount.current_plan_start_date_no_null, true);
            else
                DetermineEnrollmentAndLoadObjects(icdoPersonAccount.current_plan_start_date_no_null, false);

            if (icdoPersonAccountGhdv.overridden_structure_code.IsNotNullOrEmpty())
            {
                LoadRateStructureForUserStructureCode();
            }
            else
            {
                LoadHealthParticipationDate();
                LoadHealthPlanOption();
                LoadRateStructure();
            }
            larrList.Add(this);
            return larrList;
        }

        private int _AintValidateMemberEnrollment;
        public int AintValidateMemberEnrollment
        {
            get { return _AintValidateMemberEnrollment; }
            set { _AintValidateMemberEnrollment = value; }
        }

        /// <summary>
        /// ** BR- 122 ** The System must allow an employee to re-enroll in GHDV, EAP Plans within 31 days if Premium is NOT paid during LOA.
        /// ** BR- 123 ** The System must allow an employee to re-enroll only on the first day of the month he return from active military leave.
        /// </summary>
        /// <returns></returns>
        public int ValidateMemberEnrollmentAfterLOA()
        {
            int lintFlag = 0;
            if (ibusPersonEmploymentDetail != null)
            {
                if (ibusPersonEmploymentDetail.ibusPersonEmployment.IsNull())
                    ibusPersonEmploymentDetail.LoadPersonEmployment();
                int lintPrevPersonAccountID = GetPreviousPersonAccountID(ibusPlan.icdoPlan.plan_id, ibusPerson.icdoPerson.person_id);
                int lintPrevEmploymentDetailID = GetPreviousEmploymentDetailID(lintPrevPersonAccountID);
                if (lintPrevEmploymentDetailID != 0)
                {
                    busPersonAccountGhdv lobjGHDV = new busPersonAccountGhdv();
                    lobjGHDV.icdoPersonAccount = new cdoPersonAccount();
                    lobjGHDV.icdoPersonAccount.person_employment_dtl_id = lintPrevEmploymentDetailID;
                    lobjGHDV.LoadPersonEmploymentDetail();
                    lobjGHDV.ibusPersonEmploymentDetail.LoadPersonEmployment();

                    //PROD PIR 8375
                    DateTime ldtPrevEmploymentDetailStartDate = lobjGHDV.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date;
                    DateTime ldtPrevEmploymentDetailEndDate = lobjGHDV.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date;
                    DateTime ldtEnrollmentDate = ldtPrevEmploymentDetailEndDate.GetFirstDayofNextMonth();
                    if ((ldtPrevEmploymentDetailStartDate != DateTime.MinValue) &&
                        (ldtPrevEmploymentDetailEndDate != DateTime.MinValue))
                    {
                        DateTime ldteHistoryChangeDate = icdoPersonAccount.history_change_date_no_null; //PROD PIR 8375
                        if (lobjGHDV.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOA)
                        {
                            DataTable ldtbContributions = Select("cdoPersonAccountInsuranceContribution.CheckTransactionByPersonAccountID",
                                                            new object[3]{ lintPrevPersonAccountID,
                                                                ldtPrevEmploymentDetailStartDate,
                                                                ldtPrevEmploymentDetailEndDate});
                            if (ldtbContributions.Rows.Count == 0)
                                if (ldteHistoryChangeDate.AddDays(-31) > ldtPrevEmploymentDetailEndDate &&
                                    ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id ==
                                                                                lobjGHDV.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id)
                                    lintFlag = 1;
                        }
                        if (lobjGHDV.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOAM)
                        {
                            if (ldteHistoryChangeDate != ldtEnrollmentDate && ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id ==
                                                                                lobjGHDV.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id)
                                lintFlag = 2;
                        }
                    }
                }
            }
            return lintFlag;
        }

        /// <summary>
        /// Check whether there is an employment change in the last 31 days.
        /// </summary>
        /// <returns></returns>
        public bool IsTransferEmployment()
        {
            if (ibusPerson == null)
                LoadPerson();

            if (ibusPlan == null)
                LoadPlan();

            //Get the Previous Employment
            if (ibusPreviousEmploymentDetailForTransfer.IsNull())
                LoadPreviousEmploymentDetailForTransfer();

            //Get the Current Employment
            busPersonEmploymentDetail lobjCurrentEmpDtl = GetLatestOpenedPersonEmploymentDetail();

            if ((ibusPreviousEmploymentDetailForTransfer.icdoPersonEmploymentDetail.person_employment_dtl_id > 0) &&
                (lobjCurrentEmpDtl.icdoPersonEmploymentDetail.person_employment_dtl_id > 0))
            {
                if (ibusPreviousEmploymentDetailForTransfer.icdoPersonEmploymentDetail.person_employment_dtl_id !=
                    lobjCurrentEmpDtl.icdoPersonEmploymentDetail.person_employment_dtl_id)
                {
                    if (ibusPreviousEmploymentDetailForTransfer.icdoPersonEmploymentDetail.end_date != DateTime.MinValue)
                    {
                        if (lobjCurrentEmpDtl.icdoPersonEmploymentDetail.start_date.AddDays(-31) <
                            ibusPreviousEmploymentDetailForTransfer.icdoPersonEmploymentDetail.end_date)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// **BR-124 ** Coverage should not change with previous employment enrollment.
        /// </summary>
        /// <returns></returns>
        public bool IsLevelOfCoverageChanged()
        {
            bool lblnFlag = false;
            if (IsTransferEmployment() && !icdoPersonAccount.is_from_mss) // PIR 9906
            {
                if (ibusPreviousEmploymentDetail == null)
                    LoadPreviousEmploymentDetail();
                if (iclbPersonAccountGHDVHistory == null)
                    LoadPersonAccountGHDVHistory();
                if (ibusPersonEmploymentDetail.IsNull())
                    LoadPersonEmploymentDetail();
                if (ibusPersonEmploymentDetail.ibusPersonEmployment.IsNull())
                    ibusPersonEmploymentDetail.LoadPersonEmployment();

                if (ibusPreviousEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id > 0)
                {
                    if (ibusPreviousEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id !=
                             ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id)
                    {
                        busPersonAccountGhdvHistory lobjPAGHDVHistory = LoadHistoryByDate(ibusPreviousEmploymentDetail.icdoPersonEmploymentDetail.end_date);
                        if (lobjPAGHDVHistory.icdoPersonAccountGhdvHistory.person_account_ghdv_history_id > 0)
                        {

                            if (IsHealthOrMedicare)
                            {
                                if (lobjPAGHDVHistory.icdoPersonAccountGhdvHistory.coverage_code != icdoPersonAccountGhdv.coverage_code)
                                {
                                    lblnFlag = true;
                                }
                            }
                            else
                            {
                                if (lobjPAGHDVHistory.icdoPersonAccountGhdvHistory.level_of_coverage_value != icdoPersonAccountGhdv.level_of_coverage_value)
                                {
                                    lblnFlag = true;
                                }
                            }
                        }
                    }
                }
            }
            return lblnFlag;
        }

        /// UAT PIR ID 781
        public bool IsHealthOrgPlanRestricted()
        {
            if (IsHealthOrMedicare)
            {
                if ((ibusPersonEmploymentDetail != null) && (ibusPersonEmploymentDetail.ibusPersonEmployment != null))
                {
                    if (ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization == null)
                        ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();

                    if (ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.iclbOrgPlan == null)
                        ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.LoadOrgPlan();

                    foreach (busOrgPlan lobjOrgPlan in ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.iclbOrgPlan)
                    {
                        if ((lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdGroupHealth))// Removed Medicare plan
                        {
                            if (lobjOrgPlan.icdoOrgPlan.restriction == busConstant.Flag_Yes)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public busPersonAccountGhdvHistory LoadHistoryByDate(DateTime adtGivenDate)
        {
            busPersonAccountGhdvHistory lobjPersonAccountGhdvHistory = new busPersonAccountGhdvHistory();
            lobjPersonAccountGhdvHistory.icdoPersonAccountGhdvHistory = new cdoPersonAccountGhdvHistory();

            if (iclbPersonAccountGHDVHistory == null)
                LoadPersonAccountGHDVHistory();

            foreach (busPersonAccountGhdvHistory lobjPAGHDVHistory in iclbPersonAccountGHDVHistory)
            {
                //Ignore the Same Start Date and End Date Records
                if (lobjPAGHDVHistory.icdoPersonAccountGhdvHistory.start_date != lobjPAGHDVHistory.icdoPersonAccountGhdvHistory.end_date)
                {
                    if (busGlobalFunctions.CheckDateOverlapping(adtGivenDate, lobjPAGHDVHistory.icdoPersonAccountGhdvHistory.start_date,
                        lobjPAGHDVHistory.icdoPersonAccountGhdvHistory.end_date))
                    {
                        lobjPersonAccountGhdvHistory = lobjPAGHDVHistory;
                        break;
                    }
                }
            }
            return lobjPersonAccountGhdvHistory;
        }

        /// <summary>
        /// ** BR - 078 ** The System must load allow a Member to select from the list of all EPO Providers available in the Member's Permanent address. 
        /// </summary>
        /// <returns></returns>
        public Collection<cdoOrganization> LoadEPOProviders()
        {
            Collection<cdoOrganization> _iclbEPOOrganization = new Collection<cdoOrganization>();
            DataTable ldtbEPOOrg = DBFunction.DBSelect("cdoPersonAccountGhdv.GetEPOProviders", new object[] { },
                                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            _iclbEPOOrganization = Sagitec.DataObjects.doBase.GetCollection<cdoOrganization>(ldtbEPOOrg);
            return _iclbEPOOrganization;
        }

        private int _iintPreviousEPOProviderOrgID;
        public int iintPreviousEPOProviderOrgID
        {
            get { return _iintPreviousEPOProviderOrgID; }
            set { _iintPreviousEPOProviderOrgID = value; }
        }

        public void LoadPreviousEPOProvider()
        {
            if (IsTransferEmployment())
            {
                if (ibusPreviousEmploymentDetail == null)
                    LoadPreviousEmploymentDetail();
                if (iclbPersonAccountGHDVHistory == null)
                    LoadPersonAccountGHDVHistory();

                busPersonAccountGhdvHistory lobjPAGHDVHistory =
                    LoadHistoryByDate(ibusPreviousEmploymentDetail.icdoPersonEmploymentDetail.end_date);
                if (lobjPAGHDVHistory.icdoPersonAccountGhdvHistory.person_account_ghdv_history_id > 0)
                {
                    _iintPreviousEPOProviderOrgID = lobjPAGHDVHistory.icdoPersonAccountGhdvHistory.epo_org_id;
                }
            }
        }

        /// <summary>
        /// ** BR - 126 ** Throw a warning if the member has NO Previous EPO OrgID and currently changed the address to enroll in EPO OrgID.
        /// </summary>
        /// <returns>True - Throws Warning; False - No Warning</returns>
        public bool IsEPOProviderChanged()
        {
            bool lblnFlag = false;
            if (_iintPreviousEPOProviderOrgID == 0)
                if (icdoPersonAccountGhdv.epo_org_id != 0)
                    return true;
            return lblnFlag;
        }

        /// <summary>
        /// ** BR - 127 ** Throw a warning if the member has Previous EPO OrgID and currently changed the address to have NO EPO OrgID.
        /// </summary>
        /// <returns>True - Throws Warning; False - No Warning</returns>
        public bool IsEPOProviderChangedToNull()
        {
            bool lblnFlag = false;
            if (_iintPreviousEPOProviderOrgID != 0)
                if (icdoPersonAccountGhdv.epo_org_id == 0)
                    return true;
            return lblnFlag;
        }

        /// <summary>
        /// ** BR - 128 ** Throws a warning if the member has changed the EPO OrgID in Transfer of Employment. 
        /// </summary>
        /// <returns>True - Throws Warning; False - No Warning</returns>
        public bool IsEPOProviderChangedInTransfer()
        {
            bool lblnFlag = false;
            if (IsTransferEmployment())
            {
                LoadPreviousEPOProvider();
                if (_iintPreviousEPOProviderOrgID != icdoPersonAccountGhdv.epo_org_id)
                    lblnFlag = true;
            }
            return lblnFlag;
        }

        /// <summary>
        /// ** BR - 129 ** Validate EPO Org exists for the Member's permanent address Zipcode.
        /// </summary>
        /// <returns>True - If EPO Org exists; False - If NO EPO Org exists</returns>
        public bool IsEPOProviderExists()
        {
            bool lblnFlag = false;
            if (ibusPerson.ibusPersonCurrentAddress == null)
                ibusPerson.GetPersonCurrentAddressByType(busConstant.AddressTypePermanent, DateTime.Today);
            if (ibusPerson.ibusPersonCurrentAddress != null)
            {
                if (ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress != null)
                {
                    if (ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_zip_code != null)
                    {
                        int lintCount = Convert.ToInt32(DBFunction.DBExecuteScalar("cdoPersonAccountGhdv.ValidateEPOProviderOrg", new object[2]{
                                                    ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_zip_code,
                                                    busGlobalFunctions.GetOrgCodeFromOrgId(icdoPersonAccountGhdv.epo_org_id)},
                                                        iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
                        if (lintCount > 0)
                            lblnFlag = true;
                    }
                }
            }
            return lblnFlag;
        }

        /// <summary>
        ///  ** BR - 217 ** The system must throw an error if a retiree makes an EPO election and is not 'COBRA' 'Member Type'.
        /// </summary>
        /// <returns></returns>
        public bool IsEPOEnteredNotForCOBRA()
        {
            bool lblnResult = false;
            if (ibusPerson.IsRetiree())
            {
                if (icdoPersonAccountGhdv.epo_org_id != 0)
                {
                    if (!IsCOBRAValueSelected())
                        lblnResult = true;
                }
            }
            return lblnResult;
        }

        public bool IsCOBRAValueSelected()
        {
            bool lblnFlag = true;
            //when calling from insurance inbound file, plan object is null. So need to load explicitly
            if (ibusPlan == null)
                LoadPlan();
            if (ibusPlan.icdoPlan.plan_id == busConstant.PlanIdDental)
                if (icdoPersonAccountGhdv.dental_insurance_type_value != busConstant.DentalInsuranceTypeCOBRA)
                    lblnFlag = false;
            if (ibusPlan.icdoPlan.plan_id == busConstant.PlanIdVision)
                if (icdoPersonAccountGhdv.vision_insurance_type_value != busConstant.VisionInsuranceTypeCOBRA)
                    lblnFlag = false;
            if (IsHealthOrMedicare)
                if (icdoPersonAccountGhdv.cobra_type_value == null)
                    lblnFlag = false;
            return lblnFlag;
        }

        public void SetCOBRAEndDate()
        {
            if (icdoPersonAccountGhdv.cobra_type_value != null)
            {
                DataTable ldtbList = Select<cdoCodeValue>(new string[2] { "code_id", "code_value" },
                                            new object[2] { 333, icdoPersonAccountGhdv.cobra_type_value }, null, null);

                int lintCOBRAMonths = Convert.ToInt32(ldtbList.Rows[0]["data1"]);
                DateTime ldtTempDate1 = icdoPersonAccount.current_plan_start_date_no_null.AddMonths(lintCOBRAMonths - 1);

                DateTime ldtMemberAge65 = DateTime.MinValue;

                icdoPersonAccount.cobra_expiration_date = new DateTime(ldtTempDate1.Year, ldtTempDate1.Month, DateTime.DaysInMonth(ldtTempDate1.Year, ldtTempDate1.Month));

                //PIR 18424 
                if (ibusPerson.IsNull()) LoadPerson();
                if (ibusPerson.icdoPerson.date_of_birth.Day == 1 && ldtTempDate1 > ibusPerson.icdoPerson.date_of_birth.AddYears(65) && icdoPersonAccount.current_plan_start_date_no_null < ibusPerson.icdoPerson.date_of_birth.AddYears(65))
                    {
                        ldtMemberAge65 = ibusPerson.icdoPerson.date_of_birth.AddYears(65);
                        icdoPersonAccount.cobra_expiration_date = ldtMemberAge65.AddMonths(-2).GetLastDayofMonth();
                    }
                    else if (ibusPerson.icdoPerson.date_of_birth.Day != 1 && ldtTempDate1 > ibusPerson.icdoPerson.date_of_birth.AddYears(65) && icdoPersonAccount.current_plan_start_date_no_null < ibusPerson.icdoPerson.date_of_birth.AddYears(65))
                    {
                        ldtMemberAge65 = ibusPerson.icdoPerson.date_of_birth.AddYears(65);
                        icdoPersonAccount.cobra_expiration_date = ldtMemberAge65.AddMonths(-1).GetLastDayofMonth();
                    }
                
                //  ** BR - 153 ** The system must reset the expiration date for a person whose COBRA Type is changed from 
                //                  '18 Month COBRA' to 'Disability COBRA' to be 29 months from the original 'Start Date'.
                if (icdoPersonAccountGhdv.ihstOldValues.Count > 0)
                {
                    if (((Convert.ToString(icdoPersonAccountGhdv.ihstOldValues["cobra_type_value"]) == busConstant.COBRAType18Month) &&
                        (icdoPersonAccountGhdv.cobra_type_value == busConstant.COBRATypeDisability)) ||
                        ((Convert.ToString(icdoPersonAccountGhdv.ihstOldValues["cobra_type_value"]) == busConstant.COBRATypeRetiree18Month) &&
                        (icdoPersonAccountGhdv.cobra_type_value == busConstant.COBRATypeRetireeDisability)))
                    {
                        icdoPersonAccount.cobra_expiration_date = icdoPersonAccount.history_change_date.AddMonths(lintCOBRAMonths);
                    }
                }
            }
        }

        // PIR ID 1956 - Validation added as per mail from Maik.
        public bool IsValidCOBRAEndDate()
        {
            if (!string.IsNullOrEmpty((icdoPersonAccountGhdv.cobra_type_value)))
            {
                if (icdoPersonAccount.cobra_expiration_date != DateTime.MinValue)
                {
                    //PROD PIR - 4164
                    //Validation 7119 has to allow for suspending the plan for effective the day following COBRA Expiration Date.
                    if (icdoPersonAccount.cobra_expiration_date.AddDays(1) < icdoPersonAccount.current_plan_start_date_no_null)

                        return false;
                }
            }
            return true;
        }

        /// <summary>
        ///  ** BR - 162 ** The system must throw an error if the 'COBRA Type' is other than '36 Month COBRA'.
        /// </summary>
        /// <returns></returns>
        public bool IsValidCOBRATypeForDependentCOBRA()
        {
            bool lblnFlag = true;
            if (iblnIsDependentCobra)
                if (icdoPersonAccountGhdv.cobra_type_value != busConstant.COBRAType36Month)
                    lblnFlag = false;
            return lblnFlag;
        }

        //PIR 1677 : When Member Enrolled in HMO with enrolled status, we must check that current plan start date must not overlap with
        //health enrollment date. (Enrolled Status ony)
        public bool IsMemberActiveInHealthAndOverlapWithDates()
        {
            bool lblnResult = false;

            if (ibusPerson == null)
                LoadPerson();
            if (ibusPerson.icolPersonAccount == null)
                ibusPerson.LoadPersonAccount(false);

            var lclbHealthPersonAccount = ibusPerson.icolPersonAccount.Where(o => o.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth);
            if (lclbHealthPersonAccount != null)
            {
                foreach (busPersonAccount lbusPersonAccount in lclbHealthPersonAccount)
                {
                    busPersonAccountGhdv lbusPersonAccountGhdv = new busPersonAccountGhdv();
                    lbusPersonAccountGhdv.FindGHDVByPersonAccountID(lbusPersonAccount.icdoPersonAccount.person_account_id);
                    lbusPersonAccountGhdv.icdoPersonAccount = lbusPersonAccount.icdoPersonAccount;
                    lbusPersonAccountGhdv.LoadPersonAccountGHDVHistory();
                    foreach (busPersonAccountGhdvHistory lbusPAGHDVHistory in lbusPersonAccountGhdv.iclbPersonAccountGHDVHistory)
                    {
                        if ((lbusPAGHDVHistory.icdoPersonAccountGhdvHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled) &&
                           (busGlobalFunctions.CheckDateOverlapping(icdoPersonAccount.current_plan_start_date_no_null,
                           lbusPAGHDVHistory.icdoPersonAccountGhdvHistory.start_date, lbusPAGHDVHistory.icdoPersonAccountGhdvHistory.end_date)))
                        {
                            lblnResult = true;
                            break;
                        }
                    }
                }
            }
            return lblnResult;
        }

        // UAT PIR 978 : When Member Enrolled in Health with enrolled status, we must check that current plan start date must not overlap with
        // HMO enrollment date. (Enrolled Status ony)
        public bool IsMemberActiveInHMOAndOverlapWithDates()
        {
            bool lblnResult = false;

            if (ibusPerson == null)
                LoadPerson();
            if (ibusPerson.icolPersonAccount == null)
                ibusPerson.LoadPersonAccount(false);

            var lclbHealthPersonAccount = ibusPerson.icolPersonAccount.Where(o => o.icdoPersonAccount.plan_id == busConstant.PlanIdHMO);
            if (lclbHealthPersonAccount != null)
            {
                foreach (busPersonAccount lbusPersonAccount in lclbHealthPersonAccount)
                {
                    busPersonAccountGhdv lbusPersonAccountGhdv = new busPersonAccountGhdv();
                    lbusPersonAccountGhdv.FindGHDVByPersonAccountID(lbusPersonAccount.icdoPersonAccount.person_account_id);
                    lbusPersonAccountGhdv.icdoPersonAccount = lbusPersonAccount.icdoPersonAccount;
                    lbusPersonAccountGhdv.LoadPersonAccountGHDVHistory();
                    foreach (busPersonAccountGhdvHistory lbusPAGHDVHistory in lbusPersonAccountGhdv.iclbPersonAccountGHDVHistory)
                    {
                        if ((lbusPAGHDVHistory.icdoPersonAccountGhdvHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled) &&
                           (busGlobalFunctions.CheckDateOverlapping(icdoPersonAccount.current_plan_start_date_no_null,
                           lbusPAGHDVHistory.icdoPersonAccountGhdvHistory.start_date, lbusPAGHDVHistory.icdoPersonAccountGhdvHistory.end_date)))
                        {
                            lblnResult = true;
                            break;
                        }
                    }
                }
            }
            return lblnResult;
        }

        // Inserting the Person Account GHDV History.
        public void InsertHistory()
        {
            cdoPersonAccountGhdvHistory lobjCdoPersonAccountGHDVHistory = new cdoPersonAccountGhdvHistory();
            lobjCdoPersonAccountGHDVHistory.person_account_ghdv_id = icdoPersonAccountGhdv.person_account_ghdv_id;
            lobjCdoPersonAccountGHDVHistory.start_date = icdoPersonAccount.history_change_date;
            lobjCdoPersonAccountGHDVHistory.plan_participation_status_id = icdoPersonAccount.plan_participation_status_id;

            //PIR 17081
            // PIR 9115
            //if ((icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended) ||
            //    (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceCancelled))
            //{
            //    DBFunction.DBNonQuery("cdoPersonAccountGhdvHistory.UpdateReportGeneratedFlag", new object[1] { 
            //                        ibusHistory.icdoPersonAccountGhdvHistory.person_account_ghdv_id },
            //                        iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            //}
            lobjCdoPersonAccountGHDVHistory.plan_participation_status_value = icdoPersonAccount.plan_participation_status_value;
            lobjCdoPersonAccountGHDVHistory.status_id = icdoPersonAccount.status_id;
            lobjCdoPersonAccountGHDVHistory.status_value = icdoPersonAccount.status_value;
            lobjCdoPersonAccountGHDVHistory.from_person_account_id = icdoPersonAccount.from_person_account_id;
            lobjCdoPersonAccountGHDVHistory.to_person_account_id = icdoPersonAccount.to_person_account_id;
            lobjCdoPersonAccountGHDVHistory.suppress_warnings_flag = icdoPersonAccount.suppress_warnings_flag;
            if (icdoPersonAccount.suppress_warnings_flag == busConstant.Flag_Yes) // PROD PIR 7079
                lobjCdoPersonAccountGHDVHistory.suppress_warnings_by = icdoPersonAccount.suppress_warnings_by;
            lobjCdoPersonAccountGHDVHistory.suppress_warnings_date = icdoPersonAccount.suppress_warnings_date;
            lobjCdoPersonAccountGHDVHistory.health_insurance_type_id = icdoPersonAccountGhdv.health_insurance_type_id;
            lobjCdoPersonAccountGHDVHistory.health_insurance_type_value = icdoPersonAccountGhdv.health_insurance_type_value;
            lobjCdoPersonAccountGHDVHistory.dental_insurance_type_id = icdoPersonAccountGhdv.dental_insurance_type_id;
            lobjCdoPersonAccountGHDVHistory.dental_insurance_type_value = icdoPersonAccountGhdv.dental_insurance_type_value;
            lobjCdoPersonAccountGHDVHistory.vision_insurance_type_id = icdoPersonAccountGhdv.vision_insurance_type_id;
            lobjCdoPersonAccountGHDVHistory.vision_insurance_type_value = icdoPersonAccountGhdv.vision_insurance_type_value;
            lobjCdoPersonAccountGHDVHistory.hmo_insurance_type_id = icdoPersonAccountGhdv.hmo_insurance_type_id;
            lobjCdoPersonAccountGHDVHistory.hmo_insurance_type_value = icdoPersonAccountGhdv.hmo_insurance_type_value;
            lobjCdoPersonAccountGHDVHistory.medicare_insurance_type_id = icdoPersonAccountGhdv.medicare_insurance_type_id;
            lobjCdoPersonAccountGHDVHistory.medicare_insurance_type_value = icdoPersonAccountGhdv.medicare_insurance_type_value;
            lobjCdoPersonAccountGHDVHistory.level_of_coverage_id = icdoPersonAccountGhdv.level_of_coverage_id;
            lobjCdoPersonAccountGHDVHistory.level_of_coverage_value = icdoPersonAccountGhdv.level_of_coverage_value;
            lobjCdoPersonAccountGHDVHistory.coverage_code = icdoPersonAccountGhdv.coverage_code;
            lobjCdoPersonAccountGHDVHistory.plan_option_id = icdoPersonAccountGhdv.plan_option_id;
            lobjCdoPersonAccountGHDVHistory.plan_option_value = icdoPersonAccountGhdv.plan_option_value;
            lobjCdoPersonAccountGHDVHistory.rate_structure_id = icdoPersonAccountGhdv.rate_structure_id;
            lobjCdoPersonAccountGHDVHistory.rate_structure_value = icdoPersonAccountGhdv.rate_structure_value;
            lobjCdoPersonAccountGHDVHistory.epo_org_id = icdoPersonAccountGhdv.epo_org_id;
            lobjCdoPersonAccountGHDVHistory.cobra_type_id = icdoPersonAccountGhdv.cobra_type_id;
            lobjCdoPersonAccountGHDVHistory.cobra_type_value = icdoPersonAccountGhdv.cobra_type_value;
            lobjCdoPersonAccountGHDVHistory.medicare_claim_no = icdoPersonAccountGhdv.medicare_claim_no;
            lobjCdoPersonAccountGHDVHistory.keeping_other_coverage_flag = icdoPersonAccountGhdv.keeping_other_coverage_flag;
            lobjCdoPersonAccountGHDVHistory.medicare_part_a_effective_date = icdoPersonAccountGhdv.medicare_part_a_effective_date;
            lobjCdoPersonAccountGHDVHistory.medicare_part_b_effective_date = icdoPersonAccountGhdv.medicare_part_b_effective_date;
            lobjCdoPersonAccountGHDVHistory.low_income_credit = icdoPersonAccountGhdv.low_income_credit;
            lobjCdoPersonAccountGHDVHistory.reason_id = icdoPersonAccountGhdv.reason_id;
            lobjCdoPersonAccountGHDVHistory.reason_value = icdoPersonAccountGhdv.reason_value;
            lobjCdoPersonAccountGHDVHistory.provider_org_id = icdoPersonAccount.provider_org_id;
            lobjCdoPersonAccountGHDVHistory.employer_type_id = icdoPersonAccountGhdv.employment_type_id;
            lobjCdoPersonAccountGHDVHistory.employer_type_value = icdoPersonAccountGhdv.employment_type_value;
            lobjCdoPersonAccountGHDVHistory.overridden_structure_code = icdoPersonAccountGhdv.overridden_structure_code;
            lobjCdoPersonAccountGHDVHistory.rate_structure_code = icdoPersonAccountGhdv.rate_structure_code;
            lobjCdoPersonAccountGHDVHistory.alternate_structure_code_value = icdoPersonAccountGhdv.alternate_structure_code_value;
            lobjCdoPersonAccountGHDVHistory.premium_conversion_indicator_flag = icdoPersonAccountGhdv.premium_conversion_indicator_flag;
            //PIR 12245 - ps_file_change_event_value updated in history table
			if (icdoPersonAccountGhdv.reason_value == busConstant.ChangeReasonAnnualEnrollment
                    && (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled ||
                    icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended))
            {
                if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                {
                    lobjCdoPersonAccountGHDVHistory.ps_file_change_event_value = icdoPersonAccount.ps_file_change_event_value;
                }
                else
                    lobjCdoPersonAccountGHDVHistory.ps_file_change_event_value = busConstant.AnnualEnrollmentWaived;
            }
            else
            {
                lobjCdoPersonAccountGHDVHistory.ps_file_change_event_value = busConstant.NewEnrollment;
            }
            //lobjCdoPersonAccountGHDVHistory.people_soft_file_sent_flag = busConstant.Flag_No;//PIR 17081
            lobjCdoPersonAccountGHDVHistory.Insert();
        }

        /// <summary>
        /// Function to check whether the Person is Active Dependent
        /// </summary>
        /// <returns>Returns true if the Dependent Person is Active</returns>
        public bool IsActiveDependent()
        {
            return IsActiveDependentForPlan(icdoPersonAccount.current_plan_start_date_no_null);
        }

		//IBS Adjustment creation logic changed (Affected area enroll to enroll scenario) - logged under Backlog PIR 8022 and 8111
        public void CreateAdjustmentPayrollForEnrollmentHistoryClose(DateTime adtEffectiveDate, DateTime adtEndedHistoryStartDate, string astrHistoryAddedStatus = null)
        {
            busEmployerPayrollHeader lbusPayrollHdr = null;
            busIbsHeader lbusIBSHdr = null;
            int lintOrgID = 0;
            DateTime ldatEffectedDate = adtEffectiveDate;
            ldatEffectedDate = new DateTime(ldatEffectedDate.Year, ldatEffectedDate.Month, 1);
            if (iclbInsuranceContributionAll == null)
                LoadInsuranceContributionAll();
            if (ibusPaymentElection == null)
                LoadPaymentElection();
            if (ibusPerson == null)
                LoadPerson();

            //prod pir 5831 : need to group IBS and employer reporting as separate
            var lenuContributionByMonthForIBS =
                iclbInsuranceContributionAll.Where(i => i.icdoPersonAccountInsuranceContribution.subsystem_value == busConstant.SubSystemValueIBSDeposit)
                .GroupBy(i => i.icdoPersonAccountInsuranceContribution.effective_date).Select(o => o.First());

            if (lenuContributionByMonthForIBS != null)
            {
                foreach (busPersonAccountInsuranceContribution lbusInsuranceContribution in lenuContributionByMonthForIBS)
                {
                    if (lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date >= ldatEffectedDate)
                    { //?? Need to address subsystem_value for converted data
                        if ((lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.subsystem_value == busConstant.SubSystemValueIBSDeposit) 
                            //&&
                            //((lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.due_premium_amount > 0) ||
                            // (lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.paid_premium_amount > 0) ||
                            // (lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.rhic_benefit_amount > 0))
                            //Some times full RHIC goes to premium so due premium amount is zero but we need to create nagative entries when the RHIC Changes happens
                            )
                        {
                            if (lbusIBSHdr == null)
                            {
                                lbusIBSHdr = new busIbsHeader();
                                if (!lbusIBSHdr.LoadCurrentAdjustmentIBSHeader())
                                {
                                    lbusIBSHdr.CreateAdjustmentIBSHeader();
                                    lbusIBSHdr.icolIbsDetail = new Collection<busIbsDetail>();
                                }
                                else
                                {
                                    lbusIBSHdr.icdoIbsHeader.ienuObjectState = ObjectState.Update;
                                    lbusIBSHdr.LoadIbsDetails();
                                }
                            }

                            //Getting the Coverage Code Description for Health and Medicare
                            string lstrCoverageCode = string.Empty;
                            //PIR 24918 - Commented code because getting wrong coverage code description so loaded coverage code below using rate structure code & coverage code value
                            #region Load Health /Medicare Coverage Code Description
                            //if ((icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth) || (icdoPersonAccount.plan_id == busConstant.PlanIdMedicarePartD))
                            //{
                            //    //Get the Coverage Ref ID for the History Record
                            //    busPersonAccountGhdv lbusPersonAccountGhdv = new busPersonAccountGhdv();
                            //    lbusPersonAccountGhdv.icdoPersonAccountGhdv = new cdoPersonAccountGhdv();
                            //    lbusPersonAccountGhdv.icdoPersonAccount = new cdoPersonAccount();
                            //    //Get the Start Date for Rate Calculation
                            //    lbusPersonAccountGhdv.icdoPersonAccount.start_date = icdoPersonAccount.start_date;
                            //    lbusPersonAccountGhdv.icdoPersonAccount.person_account_id = icdoPersonAccount.person_account_id;
                            //    lbusPersonAccountGhdv.icdoPersonAccount.plan_id = icdoPersonAccount.plan_id;//pir 8337
                            //    busPersonAccountGhdvHistory lbusEndedHistory = LoadHistoryByDate(adtEndedHistoryStartDate);
                            //    lbusPersonAccountGhdv = lbusEndedHistory.LoadGHDVObject(lbusPersonAccountGhdv);
                            //    lbusPersonAccountGhdv.LoadPerson();
                            //    lbusPersonAccountGhdv.LoadPlan();
                            //    //Initialize the Org Object to Avoid the NULL error
                            //    lbusPersonAccountGhdv.InitializeObjects();
                            //    lbusPersonAccountGhdv.idtPlanEffectiveDate = adtEndedHistoryStartDate;

                            //    //For Dependent COBRA, we need to load Member Employment
                            //    if (lbusPersonAccountGhdv.icdoPersonAccount.from_person_account_id > 0)
                            //    {
                            //        //Load Member GHDV Object
                            //        lbusPersonAccountGhdv.ibusMemberGHDVForDependent = new busPersonAccountGhdv();
                            //        lbusPersonAccountGhdv.ibusMemberGHDVForDependent.FindGHDVByPersonAccountID(lbusPersonAccountGhdv.icdoPersonAccount.from_person_account_id);
                            //        lbusPersonAccountGhdv.ibusMemberGHDVForDependent.FindPersonAccount(lbusPersonAccountGhdv.icdoPersonAccount.from_person_account_id);
                            //        lbusPersonAccountGhdv.iblnIsDependentCobra = true;
                            //    }

                            //    //we need org plan object for determining health participation date for IBS COBRA Members
                            //    if (lbusPersonAccountGhdv.icdoPersonAccountGhdv.cobra_type_value.IsNotNullOrEmpty())
                            //    {
                            //        //For Dependent COBRA, we need to load Member Employment
                            //        if (lbusPersonAccountGhdv.icdoPersonAccount.from_person_account_id > 0)
                            //        {
                            //            lbusPersonAccountGhdv.LoadEmploymentDetailByDate(lbusPersonAccountGhdv.idtPlanEffectiveDate, lbusPersonAccountGhdv.ibusMemberGHDVForDependent, true, true);
                            //        }
                            //        else
                            //        {
                            //            lbusPersonAccountGhdv.LoadEmploymentDetailByDate(lbusPersonAccountGhdv.idtPlanEffectiveDate, true);
                            //        }
                            //        if (lbusPersonAccountGhdv.icdoPersonAccount.person_employment_dtl_id > 0)
                            //        {
                            //            lbusPersonAccountGhdv.LoadPersonEmploymentDetail();
                            //            lbusPersonAccountGhdv.ibusPersonEmploymentDetail.LoadPersonEmployment();
                            //            lbusPersonAccountGhdv.LoadOrgPlan(lbusPersonAccountGhdv.idtPlanEffectiveDate);
                            //        }
                            //    }
                            //    else
                            //    {
                            //        lbusPersonAccountGhdv.icdoPersonAccount.person_employment_dtl_id = lbusPersonAccountGhdv.GetEmploymentDetailID(adtEndedHistoryStartDate);
                            //    }

                            //    //PROD PIR : 4444 For Negative Adjustment, we must take provider org id from Contribution Table
                            //    if (lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.provider_org_id != 0)
                            //    {
                            //        lbusPersonAccountGhdv.LoadProviderOrgPlanByProviderOrgID(lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.provider_org_id,
                            //            lbusPersonAccountGhdv.idtPlanEffectiveDate);
                            //    }
                            //    else
                            //    {
                            //        lbusPersonAccountGhdv.LoadActiveProviderOrgPlan(lbusPersonAccountGhdv.idtPlanEffectiveDate);
                            //    }

                            //    if (lbusPersonAccountGhdv.icdoPersonAccountGhdv.overridden_structure_code.IsNotNullOrEmpty())
                            //    {
                            //        lbusPersonAccountGhdv.LoadRateStructureForUserStructureCode();
                            //    }
                            //    else
                            //    {
                            //        lbusPersonAccountGhdv.LoadHealthParticipationDate();
                            //        lbusPersonAccountGhdv.LoadHealthPlanOption();
                            //        lbusPersonAccountGhdv.LoadRateStructure();
                            //    }
                            //    //Get the Coverage Ref ID
                            //    lbusPersonAccountGhdv.LoadCoverageRefID();

                            //    lstrCoverageCode =
                            //        lbusIBSHdr.GetGroupHealthCoverageCodeDescription(lbusPersonAccountGhdv.icdoPersonAccountGhdv.Coverage_Ref_ID);
                            //}
                            #endregion

                            //For Negative Adjustment Premium Amount, we should not only consider Regular.. we also need to consider other adjustment too
                            //Example Scenario : Member was in Single First and Then changed Family and then suspended. This case Single - Family Change
                            //Could have created the adjustment record which also we need to consider while creating Neg Adjustment for suspended case.
                            decimal ldecMemberPremium = 0.00M;
                            decimal ldecTotalFeeAmount = 0.00M;
                            decimal ldecTotalBuydownAmount = 0.00M;
                            decimal ldecTotalRHICBenefitAmount = 0.00M;
                            decimal ldecTotalOtherRHICAmount = 0.00M;
                            decimal ldecTotalJSRHICAmount = 0.00M;
                            decimal ldecMedicarePartDAmount = 0.00M;// PIR 14271
                            //ucs - 038 addendum, new col. for paid premium amount
                            decimal ldecPaidPremiumAmount = 0.0M;
                            //uat pir 1429:-post ghdv_history_id 
                            int lintGHDVHistoryId = 0;
                            string lstrGroupNumber = string.Empty;
                            //prod pir 6076
                            string lstrCoverageCodeValue = string.Empty, lstrRateStructureCode = string.Empty;
                            var lclbFilterdContribution = _iclbInsuranceContributionAll.Where(
                                i =>
                                i.icdoPersonAccountInsuranceContribution.subsystem_value == busConstant.SubSystemValueIBSDeposit &&
                                i.icdoPersonAccountInsuranceContribution.effective_date == lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date);
                            //ucs - 038 addendum, new col. for paid premium amount
                            var lclbPaidFilterdContribution = _iclbInsuranceContributionAll.Where(
                               i =>
                               i.icdoPersonAccountInsuranceContribution.subsystem_value == busConstant.SubSystemValueIBSPayment &&
                               i.icdoPersonAccountInsuranceContribution.effective_date == lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date);

                            if (lclbFilterdContribution != null)
                            {
                                ldecMemberPremium = lclbFilterdContribution.Sum(i => i.icdoPersonAccountInsuranceContribution.due_premium_amount);
                                ldecTotalFeeAmount = lclbFilterdContribution.Sum(i => i.icdoPersonAccountInsuranceContribution.group_health_fee_amt);
                                ldecTotalBuydownAmount = lclbFilterdContribution.Sum(i => i.icdoPersonAccountInsuranceContribution.buydown_amount);
                                ldecMedicarePartDAmount = lclbFilterdContribution.Sum(i => i.icdoPersonAccountInsuranceContribution.medicare_part_d_amt);// PIR 14271
                                ldecTotalRHICBenefitAmount = lclbFilterdContribution.Sum(i => i.icdoPersonAccountInsuranceContribution.rhic_benefit_amount);
                                ldecTotalOtherRHICAmount = lclbFilterdContribution.Sum(i => i.icdoPersonAccountInsuranceContribution.othr_rhic_amount);
                                ldecTotalJSRHICAmount = lclbFilterdContribution.Sum(i => i.icdoPersonAccountInsuranceContribution.js_rhic_amount);
                                //ucs - 038 addendum, new col. for paid premium amount
                                if (lclbPaidFilterdContribution != null)
                                    ldecPaidPremiumAmount = lclbPaidFilterdContribution.Sum(i => i.icdoPersonAccountInsuranceContribution.paid_premium_amount);
                                //prod pir 5831
                                busPersonAccountInsuranceContribution lobjContr = iclbInsuranceContributionAll
                                    .Where(i => i.icdoPersonAccountInsuranceContribution.subsystem_value == busConstant.SubSystemValueIBSDeposit &&
                                        i.icdoPersonAccountInsuranceContribution.effective_date == lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date &&
                                        i.icdoPersonAccountInsuranceContribution.due_premium_amount > 0).FirstOrDefault();
                                //uat pir 1429
                                //uat pir 2056
                                //PROD PIR 5308
                                //to get the group number for Superior Vision provider
                                if (lobjContr != null && (icdoPersonAccount.plan_id == busConstant.PlanIdMedicarePartD || icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth ||
                                    icdoPersonAccount.plan_id == busConstant.PlanIdVision || icdoPersonAccount.plan_id == busConstant.PlanIdDental))
                                {
                                    //prod pir 6076 & 6077 - Removal of person account ghdv history id
                                    //lintGHDVHistoryId = lobjContr.icdoPersonAccountInsuranceContribution.person_account_ghdv_history_id;
                                    lstrGroupNumber = lobjContr.icdoPersonAccountInsuranceContribution.group_number;
                                    //prod pir 6076
                                    lstrCoverageCodeValue = lobjContr.icdoPersonAccountInsuranceContribution.coverage_code;
                                    lstrRateStructureCode = lobjContr.icdoPersonAccountInsuranceContribution.rate_structure_code;
                                }
                                //prod pir 7839 : if full premium is created from RHIC
                                else if (lobjContr != null)
                                {
                                    busPersonAccountInsuranceContribution lobjContribution = iclbInsuranceContributionAll
                                                                       .Where(i => i.icdoPersonAccountInsuranceContribution.subsystem_value == busConstant.SubSystemValueIBSDeposit &&
                                                                           i.icdoPersonAccountInsuranceContribution.effective_date == lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date &&
                                                                           i.icdoPersonAccountInsuranceContribution.rhic_benefit_amount > 0).FirstOrDefault();
                                    if (lobjContribution != null)
                                    {
                                        //prod pir 6076 & 6077 - Removal of person account ghdv history id
                                        //lintGHDVHistoryId = lobjContr.icdoPersonAccountInsuranceContribution.person_account_ghdv_history_id;
                                        lstrGroupNumber = lobjContr.icdoPersonAccountInsuranceContribution.group_number;
                                        //prod pir 6076
                                        lstrCoverageCodeValue = lobjContr.icdoPersonAccountInsuranceContribution.coverage_code;
                                        lstrRateStructureCode = lobjContr.icdoPersonAccountInsuranceContribution.rate_structure_code;
                                    }
                                }
                            }
                            //PIR 24918
                            if((icdoPersonAccount.plan_id == busConstant.PlanIdVision || icdoPersonAccount.plan_id == busConstant.PlanIdDental) && lstrCoverageCode.IsNullOrEmpty())
                            {
                                busPersonAccountGhdvHistory lbusEndedHistory = LoadHistoryByDate(adtEndedHistoryStartDate);
                                lstrCoverageCode = lbusEndedHistory.icdoPersonAccountGhdvHistory.level_of_coverage_description;
                            }
                            else if((icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth) || (icdoPersonAccount.plan_id == busConstant.PlanIdMedicarePartD))
                            {
                                lstrCoverageCode = lbusIBSHdr.GetGroupHealthCoverageCodeDescription(lstrCoverageCodeValue, lstrRateStructureCode);
                            }
                            //uat pir 1461 --//start
                            string lstrPaymentMethod = string.Empty;
                            //uat pir : 2374 - as part of removal of ibs_effective_Date in payment election
                            //if (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date > lbusIBSHdr.icdoIbsHeader.billing_month_and_year)
                            //{
                            //    lstrPaymentMethod = busConstant.IBSModeOfPaymentPersonalCheck;
                            //}
                            //else 
                            if (ibusPaymentElection.icdoPersonAccountPaymentElection.payment_method_value == busConstant.IBSModeOfPaymentACH)
                            {
                                LoadPersonAccountAchDetail();
                                if (iclbPersonAccountAchDetail.Count == 0)
                                {
                                    lstrPaymentMethod = busConstant.IBSModeOfPaymentPersonalCheck;
                                }
                                busPersonAccountAchDetail lobjACHDetail = iclbPersonAccountAchDetail.Where(o => busGlobalFunctions.CheckDateOverlapping(lbusIBSHdr.icdoIbsHeader.billing_month_and_year,
                                    o.icdoPersonAccountAchDetail.ach_start_date, o.icdoPersonAccountAchDetail.ach_end_date == DateTime.MinValue ? DateTime.MaxValue : o.icdoPersonAccountAchDetail.ach_end_date))
                                    .FirstOrDefault();
                                if (lobjACHDetail != null && lobjACHDetail.icdoPersonAccountAchDetail.pre_note_flag == busConstant.Flag_No)
                                {
                                    lstrPaymentMethod = ibusPaymentElection.icdoPersonAccountPaymentElection.payment_method_value;
                                }
                                else
                                    lstrPaymentMethod = busConstant.IBSModeOfPaymentPersonalCheck;
                            }
                            else
                            {
                                lstrPaymentMethod = ibusPaymentElection.icdoPersonAccountPaymentElection.payment_method_value;
                            }
							
							//IBS Adjustment creation logic changed (Affected area enroll to enroll scenario) - logged under Backlog PIR 8022 and 8111
                            if (icolPosNegIbsDetail.IsNull())
                                icolPosNegIbsDetail = new Collection<busIbsDetail>();

                            //uat pir 1461 --//end
                            if ((ldecMemberPremium != 0 || ldecTotalRHICBenefitAmount != 0) &&
                                (IsAdjustmentExistsPriorToEnteredEffectiveDate(lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.provider_org_id, lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date, -1 * ldecMemberPremium, istrOldPaymentMethod, lstrCoverageCode))) //prod pir 4235
                                lbusIBSHdr.AddIBSDetailForGHDV(icdoPersonAccount.person_account_id, icdoPersonAccount.person_id, icdoPersonAccount.plan_id,
                                    lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date,
                                    istrOldPaymentMethod, lstrCoverageCode,//uat pir 1461
                                    -1M * ldecTotalFeeAmount,
                                    -1M * ldecTotalBuydownAmount,
                                    -1M * ldecMedicarePartDAmount,// PIR 14271
                                    -1M * ldecMemberPremium,
                                    -1M * ldecTotalRHICBenefitAmount,
                                    -1M * ldecTotalOtherRHICAmount,
                                    -1M * ldecTotalJSRHICAmount,
                                    -1M * (ldecMemberPremium + ldecTotalRHICBenefitAmount + ldecTotalBuydownAmount - ldecTotalFeeAmount - ldecMedicarePartDAmount), // PIR 11662 // PIR 14271-As per Maik mail, subtracting the Medicare Part D amount from the Provider Premium Amount
                                    -1M * (ldecMemberPremium + ldecTotalRHICBenefitAmount), lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.provider_org_id,
                                    aintGHDVHistoryID: lintGHDVHistoryId, astrGroupNumber: lstrGroupNumber, //uat pir 1429 : post ghdv_history_id
                                    adecPaidPremiumAmount: -1M * ldecPaidPremiumAmount, //ucs - 038 addendum
                                    astrCoverageCodeValue: lstrCoverageCodeValue,//prod pir 6076
                                    astrRateStructureCode: lstrRateStructureCode,
                                    acolNegPosIBsDetail: icolPosNegIbsDetail
                                    );
                        }
                    }
                }
            }

            //prod pir 5831 : need to group IBS and employer reporting as separate
            var lenuContributionByMonthForPayroll =
                iclbInsuranceContributionAll.Where(i => i.icdoPersonAccountInsuranceContribution.subsystem_value == busConstant.SubSystemValueEmployerReporting)
                .GroupBy(i => i.icdoPersonAccountInsuranceContribution.effective_date).Select(o => o.First());

            if (lenuContributionByMonthForPayroll != null)
            {
                foreach (busPersonAccountInsuranceContribution lbusInsuranceContribution in lenuContributionByMonthForPayroll)
                {
                    if (lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date >= ldatEffectedDate)
                    { //?? Need to address subsystem_value for converted data
                        if (lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.subsystem_value == busConstant.SubSystemValueEmployerReporting)
                        {
                            if (lbusPayrollHdr == null)
                            {
                                // As only most recent record can be ended then all these row should belong to same org id
                                busPersonEmploymentDetail lbusEmploymentDetail = new busPersonEmploymentDetail();
                                lbusEmploymentDetail.FindPersonEmploymentDetail(lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.person_employment_dtl_id);
                                lbusEmploymentDetail.LoadPersonEmployment();
                                lintOrgID = lbusEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id;

                                //There are scenario, where Regular Payroll may not come.. at that time, we need to load the employment in different wat
                                if (lintOrgID == 0)
                                {
                                    if (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id != 0)
                                        lintOrgID = ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id;
                                    else
                                    {
                                        if (ibusPersonEmploymentDetail == null)
                                        {
                                            LoadPersonEmploymentDetail();
                                        }
                                        if (ibusPersonEmploymentDetail.ibusPersonEmployment == null)
                                        {
                                            ibusPersonEmploymentDetail.LoadPersonEmployment();
                                        }
                                        lintOrgID = ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id;
                                    }
                                }
                                //prod pir 5831
                                //For COBRA members, we dont have an option to find org id, so using substem ref id, load payroll detail and then load payroll header
                                //and get the org id
                                if (lintOrgID == 0)
                                {
                                    busEmployerPayrollDetail lobjPayrollDtl = new busEmployerPayrollDetail();
                                    if (lobjPayrollDtl.FindEmployerPayrollDetail(lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.subsystem_ref_id))
                                    {
                                        lobjPayrollDtl.LoadPayrollHeader();
                                        lintOrgID = lobjPayrollDtl.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.org_id;
                                    }
                                }

                                lbusPayrollHdr = new busEmployerPayrollHeader();
                                if (!lbusPayrollHdr.LoadCurrentAdjustmentPayrollHeader(lintOrgID, busConstant.PayrollHeaderBenefitTypeInsr))
                                {
                                    lbusPayrollHdr.CreateInsuranceAdjustmentPayrollHeader(lintOrgID);
                                    lbusPayrollHdr.iclbEmployerPayrollDetail = new Collection<busEmployerPayrollDetail>();
                                }
                                else
                                {
                                    lbusPayrollHdr.icdoEmployerPayrollHeader.ienuObjectState = ObjectState.Update;
                                    lbusPayrollHdr.LoadEmployerPayrollDetail();
                                }
                            }

                            if (lintOrgID > 0)
                            {
                                //For Negative Adjustment Premium Amount, we should not only consider Regular.. we also need to consider other adjustment too
                                //Example Scenario : Member was in Single First and Then changed Family and then suspended. This case Single - Family Change
                                //Could have created the adjustment record which also we need to consider while creating Neg Adjustment for suspended case.
                                decimal ldecTotalPremium = 0.00M;
                                decimal ldecTotalFeeAmount = 0.00M;
                                decimal ldecTotalBuydownAmount = 0.00M; // PIR 11239
                                decimal ldecTotalMedicarePartDAmount = 0.00M;
                                decimal ldecTotalRHICBenefitAmount = 0.00M;
                                decimal ldecTotalOtherRHICAmount = 0.00M;
                                decimal ldecTotalJSRHICAmount = 0.00M;
                                //PIR 7705
                                decimal ldecTotalHSAAmount = 0.00M;
                                decimal ldecTotalVendorAmount = 0.00M;
                                //uat pir 1429:-post ghdv_history_id 
                                int lintGHDVHistoryId = 0;
                                string lstrGroupNumber = string.Empty;
                                //prod pir 6076
                                string lstrCoverageCodeValue = string.Empty, lstrRateStructureCode = string.Empty;
                                var lclbFilterdContribution = _iclbInsuranceContributionAll.Where(
                                    i =>
                                    i.icdoPersonAccountInsuranceContribution.subsystem_value == busConstant.SubSystemValueEmployerReporting &&
                                    i.icdoPersonAccountInsuranceContribution.effective_date == lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date);

                                if (lclbFilterdContribution != null)
                                {
                                    ldecTotalPremium = lclbFilterdContribution.Sum(i => i.icdoPersonAccountInsuranceContribution.due_premium_amount);
                                    ldecTotalFeeAmount = lclbFilterdContribution.Sum(i => i.icdoPersonAccountInsuranceContribution.group_health_fee_amt);
                                    ldecTotalBuydownAmount = lclbFilterdContribution.Sum(i => i.icdoPersonAccountInsuranceContribution.buydown_amount); //PIR 11239
                                    ldecTotalMedicarePartDAmount = lclbFilterdContribution.Sum(i => i.icdoPersonAccountInsuranceContribution.medicare_part_d_amt);//PIR 14271 
                                    ldecTotalRHICBenefitAmount = lclbFilterdContribution.Sum(i => i.icdoPersonAccountInsuranceContribution.rhic_benefit_amount);
                                    ldecTotalOtherRHICAmount = lclbFilterdContribution.Sum(i => i.icdoPersonAccountInsuranceContribution.othr_rhic_amount);
                                    ldecTotalJSRHICAmount = lclbFilterdContribution.Sum(i => i.icdoPersonAccountInsuranceContribution.js_rhic_amount);
                                    //PIR 7705
                                    ldecTotalHSAAmount = lclbFilterdContribution.Sum(i => i.icdoPersonAccountInsuranceContribution.hsa_amount);
                                    ldecTotalVendorAmount = lclbFilterdContribution.Sum(i => i.icdoPersonAccountInsuranceContribution.vendor_amount);
                                    //uat pir 1429
                                    if (icdoPersonAccount.plan_id == busConstant.PlanIdMedicarePartD || icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth)
                                    {
                                        //prod pir 6076 & 6077 - Removal of person account ghdv history id
                                        //lintGHDVHistoryId = lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.person_account_ghdv_history_id;
                                        lstrGroupNumber = lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.group_number;
                                        //prod pir 6076
                                        lstrCoverageCodeValue = lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.coverage_code;
                                        lstrRateStructureCode = lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.rate_structure_code;
                                    }
                                    //uat pir 2056
                                    else if (icdoPersonAccount.plan_id == busConstant.PlanIdVision || icdoPersonAccount.plan_id == busConstant.PlanIdDental)
                                    {
                                        //prod pir 6076 & 6077 - Removal of person account ghdv history id
                                        //lintGHDVHistoryId = lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.person_account_ghdv_history_id;
                                        //PROD PIR 5308
                                        //to get the group number for Superior Vision provider
                                        lstrGroupNumber = lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.group_number;
                                        //prod pir 6076
                                        lstrCoverageCodeValue = lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.coverage_code;
                                        lstrRateStructureCode = lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.rate_structure_code;
                                        //PIR 24918
                                        if (lstrCoverageCodeValue.IsNullOrEmpty())
                                        {
                                            busPersonAccountGhdvHistory lbusEndedHistory = LoadHistoryByDate(adtEndedHistoryStartDate);
                                            lstrCoverageCodeValue = lbusEndedHistory.icdoPersonAccountGhdvHistory.level_of_coverage_value;
                                        }
                                    }
                                }
                                //uat pir 1429 : post ghdv_history_id   
                                if (icolPosNegEmployerPayrollDtl.IsNull())
                                    icolPosNegEmployerPayrollDtl = new Collection<busEmployerPayrollDetail>();
                                if (ldecTotalPremium > 0 && IsPayrollDtlAdjustmentExistsPriorToEnteredEffectiveDate(lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.provider_org_id, 
                                    lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date, ldecTotalPremium, busConstant.PayrollDetailRecordTypeNegativeAdjustment, lstrCoverageCodeValue))
                                {
                                    lbusPayrollHdr.AddInsuranceAdjustmentPayrollDetail(icdoPersonAccount.person_id, icdoPersonAccount.plan_id,
                                                                                       ibusPerson.icdoPerson.first_name, ibusPerson.icdoPerson.last_name,
                                                                                       ibusPerson.icdoPerson.ssn, busConstant.PayrollDetailRecordTypeNegativeAdjustment,
                                                                                       lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date, ldecTotalPremium,
                                                                                       lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.provider_org_id,
                                                                                       ldecTotalFeeAmount, ldecTotalBuydownAmount ,ldecTotalMedicarePartDAmount ,ldecTotalRHICBenefitAmount, ldecTotalOtherRHICAmount, ldecTotalJSRHICAmount, ldecTotalHSAAmount, ldecTotalVendorAmount,
                                                                                       aintGHDVHistoryID: lintGHDVHistoryId, astrGroupNumber: lstrGroupNumber,
                                                                                       astrCoverageCodeValue: lstrCoverageCodeValue, astrRateStructureCode: lstrRateStructureCode, acolPosNegEmployerPayrollDtl: icolPosNegEmployerPayrollDtl);//prod pir 6076
                                }
                                else if (ldecTotalPremium < 0 && IsPayrollDtlAdjustmentExistsPriorToEnteredEffectiveDate(lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.provider_org_id, 
                                    lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date, ldecTotalPremium * -1, busConstant.PayrollDetailRecordTypePositiveAdjustment, lstrCoverageCodeValue))
                                {
                                    lbusPayrollHdr.UpdateEmployerPayrollDetailStatusReviewAndValidToIgnore(icdoPersonAccount.person_id, icdoPersonAccount.plan_id, lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date);
                                    lbusPayrollHdr.AddInsuranceAdjustmentPayrollDetail(icdoPersonAccount.person_id, icdoPersonAccount.plan_id,
                                                                                       ibusPerson.icdoPerson.first_name, ibusPerson.icdoPerson.last_name,
                                                                                       ibusPerson.icdoPerson.ssn, busConstant.PayrollDetailRecordTypePositiveAdjustment,
                                                                                       lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date,
                                                                                       ldecTotalPremium * -1, lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.provider_org_id,
                                                                                       ldecTotalFeeAmount * -1, ldecTotalBuydownAmount * -1, ldecTotalMedicarePartDAmount * -1, ldecTotalRHICBenefitAmount * -1,
                                                                                       ldecTotalOtherRHICAmount * -1, ldecTotalJSRHICAmount * -1,
                                                                                       ldecTotalHSAAmount * -1,
                                                                                       ldecTotalVendorAmount * -1,
                                                                                       aintGHDVHistoryID: lintGHDVHistoryId,
                                                                                       astrGroupNumber: lstrGroupNumber,
                                                                                       astrCoverageCodeValue: lstrCoverageCodeValue, astrRateStructureCode: lstrRateStructureCode, acolPosNegEmployerPayrollDtl: icolPosNegEmployerPayrollDtl);//prod pir 6076
                                }
                            }
                        }
                    }
                }
            }
            // Update changes
			//IBS Adjustment creation logic changed (Affected area enroll to enroll scenario) - logged under Backlog PIR 8022 and 8111
            if ((lbusIBSHdr != null) && (lbusIBSHdr.icolIbsDetail != null) && (lbusIBSHdr.icolIbsDetail.Count > 0) && astrHistoryAddedStatus != busConstant.PlanParticipationStatusInsuranceEnrolled)
            {
                lbusIBSHdr.UpdateSummaryData(busConstant.IBSHeaderStatusReview); //?? Save detail here
                foreach (busIbsDetail lbusIBSDtl in lbusIBSHdr.icolIbsDetail)
                {
                    lbusIBSDtl.icdoIbsDetail.ibs_header_id = lbusIBSHdr.icdoIbsHeader.ibs_header_id;
                    lbusIBSDtl.UpdateDataObject(lbusIBSDtl.icdoIbsDetail);
                }
            }
            if ((lbusPayrollHdr != null) && (lbusPayrollHdr.iclbEmployerPayrollDetail != null) && (lbusPayrollHdr.iclbEmployerPayrollDetail.Count > 0) && astrHistoryAddedStatus != busConstant.PlanParticipationStatusInsuranceEnrolled)
            {
                lbusPayrollHdr.UpdateDataObject(lbusPayrollHdr.icdoEmployerPayrollHeader);
                foreach (busEmployerPayrollDetail lbusPayrollDtl in lbusPayrollHdr.iclbEmployerPayrollDetail)
                {
                    lbusPayrollDtl.icdoEmployerPayrollDetail.employer_payroll_header_id = lbusPayrollHdr.icdoEmployerPayrollHeader.employer_payroll_header_id;
                    lbusPayrollDtl.UpdateDataObject(lbusPayrollDtl.icdoEmployerPayrollDetail);
                }
            }
        }

        //prod pir 933
        public DataTable idtMedicareSplitCodeValue { get; set; }

        public void CreateAdjustmentPayrollForEnrollmentHistoryAdd(DateTime adtEffectiveDate)
        {
            int lintOrgID = 0;

            if (iclbInsuranceContributionAll == null)
                LoadInsuranceContributionAll();
            if (ibusPaymentElection == null)
                LoadPaymentElection();

            if (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag == busConstant.Flag_No)
            {
                if (ibusPersonEmploymentDetail == null)
                {
                    LoadPersonEmploymentDetail();
                }
                if (ibusPersonEmploymentDetail.ibusPersonEmployment == null)
                {
                    ibusPersonEmploymentDetail.LoadPersonEmployment();
                }
                lintOrgID = ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id;
            }
            else if (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id != 0)
            {
                lintOrgID = ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id;
            }
            //prod pir 933
            idtMedicareSplitCodeValue = new DataTable();
            idtMedicareSplitCodeValue = iobjPassInfo.isrvDBCache.GetCodeValues(1922);
            CreateIBSAdjustmentForEnrollmentHistoryAdd(adtEffectiveDate);
            CreateEmployerAdjustmentForEnrollmentHistoryAdd(adtEffectiveDate);

            //PROD PIR 5538 : need to calculate premium as of idtPlanEffectiveDate
            RecalculatePremiumBasedOnPlanEffectiveDate();
        }

        public decimal idecEmprShareMonthlyPremium { get; set; }
        public decimal idecEmprSharePremium { get; set; }
        public decimal idecEmprShareFee { get; set; }
        public decimal idecEmprShareBuydownAmt { get; set; }
        public decimal idecEmprShareRHICAmt { get; set; }
        public decimal idecEmprShareOtherRHICAmt { get; set; }
        public decimal idecEmprShareJSRHICAmt { get; set; }
        public decimal idecEmprShareMedicarePartDAmt { get; set; }

        private void CalculatePremiumAmountWithCOBRAEmprShare()
        {
            idecEmprShareMonthlyPremium = idecEmprSharePremium = idecEmprShareRHICAmt = idecEmprShareOtherRHICAmt = idecEmprShareJSRHICAmt = 0.0M;
            idecEmprShareBuydownAmt = idecEmprShareMedicarePartDAmt = 0.0M;
            if (!string.IsNullOrEmpty(icdoPersonAccountGhdv.cobra_type_value) &&
                                   ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id > 0 &&
                                   ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share > 0 &&
                                   ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share < 100)
            {
                idecEmprShareMonthlyPremium = Math.Round(icdoPersonAccountGhdv.MonthlyPremiumAmount *
                                           ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);
                idecEmprSharePremium = Math.Round(icdoPersonAccountGhdv.PremiumExcludingFeeAmount *
                                           ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);
                idecEmprShareFee = Math.Round(icdoPersonAccountGhdv.FeeAmount *
                    ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);
                idecEmprShareBuydownAmt = Math.Round(icdoPersonAccountGhdv.BuydownAmount *
                    ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);
                idecEmprShareMedicarePartDAmt = Math.Round(icdoPersonAccountGhdv.MedicarePartDAmount *
                    ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);
                idecEmprShareRHICAmt = Math.Round(icdoPersonAccountGhdv.total_rhic_amount *
                    ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);
                idecEmprShareOtherRHICAmt = Math.Round(icdoPersonAccountGhdv.other_rhic_amount *
                    ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);
                idecEmprShareJSRHICAmt = Math.Round(icdoPersonAccountGhdv.js_rhic_amount *
                    ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);
            }
        }

        public int iintGHDVHistoryID { get; set; }
        public string istrGroupNumber { get; set; }
        public string istrCoverageCodeValue { get; set; }
        public string istrRateStructureCode { get; set; }

        public void CalculatePremiumAmount(DateTime adtEffectedDate)
        {
            iintGHDVHistoryID = 0;
            istrGroupNumber = null;
            istrCoverageCodeValue = string.Empty;
            istrRateStructureCode = string.Empty;
            idecEmprShareMonthlyPremium = idecEmprSharePremium = idecEmprShareRHICAmt = idecEmprShareOtherRHICAmt = idecEmprShareJSRHICAmt = 0.0M;
            //Recalculate the Premium based on the New Effective Date
            //uat pir 1429
            if (ibusPlan == null)
                LoadPlan();
            if (IsHealthOrMedicare || ibusPlan.icdoPlan.plan_id == busConstant.PlanIdDental || ibusPlan.icdoPlan.plan_id == busConstant.PlanIdVision)
            {
                busPersonAccountGhdvHistory lobjGHDVHistory = LoadHistoryByDate(adtEffectedDate);
                iintGHDVHistoryID = lobjGHDVHistory.icdoPersonAccountGhdvHistory.person_account_ghdv_history_id;
                //prod pir 6076
                istrCoverageCodeValue = lobjGHDVHistory.icdoPersonAccountGhdvHistory.coverage_code;
                istrRateStructureCode = !string.IsNullOrEmpty(lobjGHDVHistory.icdoPersonAccountGhdvHistory.overridden_structure_code) ?
                                    lobjGHDVHistory.icdoPersonAccountGhdvHistory.overridden_structure_code : lobjGHDVHistory.icdoPersonAccountGhdvHistory.rate_structure_code;
                
                if(ibusPlan.icdoPlan.plan_id == busConstant.PlanIdDental || ibusPlan.icdoPlan.plan_id == busConstant.PlanIdVision)
                {
                    istrCoverageCodeValue = lobjGHDVHistory.icdoPersonAccountGhdvHistory.level_of_coverage_value;
                }
                //uat pir 1429                
                if (icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeRetiree
                    || icdoPersonAccountGhdv.is_health_cobra || IsGroupNumber06ConditionSatisfied())
                {
                    istrGroupNumber = GetGroupNumber();
                }
                //PROD PIR 5308
                //to get the group number for Superior Vision provider
                else if (ibusPlan.icdoPlan.plan_id == busConstant.PlanIdVision)
                {
                    if (busGlobalFunctions.GetOrgCodeFromOrgId(lobjGHDVHistory.icdoPersonAccountGhdvHistory.provider_org_id) ==
                                    busGlobalFunctions.GetData1ByCodeValue(1213, busConstant.SuperiorVisionProviderCodeValue, iobjPassInfo))
                    {
                        istrGroupNumber = GetGroupNumber();

                        if (string.IsNullOrEmpty(istrGroupNumber))
                        {
                            if (ibusPersonEmploymentDetail == null)
                                LoadPersonEmploymentDetail();
                            if (ibusPersonEmploymentDetail.ibusPersonEmployment == null)
                                ibusPersonEmploymentDetail.LoadPersonEmployment();
                            if (ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization == null)
                                ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
                            istrGroupNumber = ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.org_code;
                        }
                    }
                    //6077 : removal of person account ghdv history id
                    else
                    {
                        istrGroupNumber = GetHIPAAReferenceID();
                    }
                }
                else if (ibusPlan.icdoPlan.plan_id == busConstant.PlanIdDental)
                {
                    if (busGlobalFunctions.GetOrgCodeFromOrgId(lobjGHDVHistory.icdoPersonAccountGhdvHistory.provider_org_id) ==
                                     busGlobalFunctions.GetData1ByCodeValue(1213, busConstant.DELTAProviderCodeValue, iobjPassInfo))
                    {
                        istrGroupNumber = GetGroupNumber(); // PIR 10448 - new provider
                    }
                    else
                    {
                        //6077 : removal of person account ghdv history id
                        istrGroupNumber = GetBranch();
                        istrGroupNumber += GetBenefitOptionCode();
                    }
                }
                else
                {
                    if (ibusPersonEmploymentDetail == null)
                        LoadPersonEmploymentDetail();
                    if (ibusPersonEmploymentDetail.ibusPersonEmployment == null)
                        ibusPersonEmploymentDetail.LoadPersonEmployment();
                    if (ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization == null)
                        ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
                    istrGroupNumber = ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.org_code;
                }
            }
            if (IsHealthOrMedicare)
            {
                if (icdoPersonAccountGhdv.overridden_structure_code.IsNotNullOrEmpty())
                {
                    LoadRateStructureForUserStructureCode();
                }
                else
                {
                    LoadRateStructure(adtEffectedDate);
                }
                idtPlanEffectiveDate = adtEffectedDate; // PIR 11525
                LoadCoverageRefID();
                LoadPlanEffectiveDate(); // PIR 11525

                //prod pir 6076
                istrCoverageCodeValue = icdoPersonAccountGhdv.coverage_code;
                istrRateStructureCode = !string.IsNullOrEmpty(icdoPersonAccountGhdv.overridden_structure_code) ?
                                    icdoPersonAccountGhdv.overridden_structure_code : icdoPersonAccountGhdv.rate_structure_code;

                GetMonthlyPremiumAmountByRefID(adtEffectedDate);

                CalculatePremiumAmountWithCOBRAEmprShare();
            }
            else
            {
                GetMonthlyPremiumAmount(adtEffectedDate);
            }
        }

        private void CreateIBSAdjustmentForEnrollmentHistoryAdd(DateTime adtEffectiveDate)
        {
            DateTime ldatCurrentPayPeriod = DateTime.MinValue;
            DateTime ldatEffectedDate = adtEffectiveDate;
            busIbsHeader lbusIBSHdr = null;
            //Discussion with Satya : If the new Members enrolled, there wont be any contribution record. To handle such scenario also,
            //we are now taking the current pay period from payroll header instead of last pay period from contribution table.
            //Pure IBS Member
            if (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag == busConstant.Flag_Yes &&
                (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id == 0 ||
                    (!string.IsNullOrEmpty(icdoPersonAccountGhdv.cobra_type_value) &&
                    ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id > 0 &&
                    ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share > 0 &&
                    ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share < 100)))
            {
                DataTable ldtResult = Select<cdoIbsHeader>(
                                new string[2] { "report_type_value", "REPORT_STATUS_VALUE" },
                                new object[2] { busConstant.IBSHeaderReportTypeRegular, busConstant.IBSHeaderStatusPosted }, null, "BILLING_MONTH_AND_YEAR desc");
                if ((ldtResult != null) && (ldtResult.Rows.Count > 0))
                {
                    if (!Convert.IsDBNull(ldtResult.Rows[0]["BILLING_MONTH_AND_YEAR"]))
                    {
                        ldatCurrentPayPeriod = Convert.ToDateTime(ldtResult.Rows[0]["BILLING_MONTH_AND_YEAR"]);
                    }
                }

                if (ldatCurrentPayPeriod == DateTime.MinValue)
                {
                    cdoCodeValue lcdoCodeValue = busGlobalFunctions.GetCodeValueDetails(busConstant.SystemConstantsAndVariablesCodeID, busConstant.SystemConstantsLastIBSPostingDate);
                    ldatCurrentPayPeriod = new DateTime(Convert.ToDateTime(lcdoCodeValue.data1).Year, Convert.ToDateTime(lcdoCodeValue.data1).Month, 1);
                }
            }

            while (ldatEffectedDate <= ldatCurrentPayPeriod)
            {
                //Must reload Provider Org Plan based on effective date. if the enrollment change happens between 2010 and 2011 , chances for two different providers.
                LoadActiveProviderOrgPlan(ldatEffectedDate);

                CalculatePremiumAmount(ldatEffectedDate);

                if ((ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag == busConstant.Flag_Yes) &&
                    (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id == 0 ||
                    (!string.IsNullOrEmpty(icdoPersonAccountGhdv.cobra_type_value) &&
                    ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id > 0 &&
                    ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share > 0 &&
                    ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share < 100)))
                {
                    string lstrCoverageCode = string.Empty;
                    if (lbusIBSHdr == null)
                    {
                        lbusIBSHdr = new busIbsHeader();
                        if (!lbusIBSHdr.LoadCurrentAdjustmentIBSHeader())
                        {
                            lbusIBSHdr.CreateAdjustmentIBSHeader();
                            lbusIBSHdr.icolIbsDetail = new Collection<busIbsDetail>();
                        }
                        else
                        {
                            lbusIBSHdr.icdoIbsHeader.ienuObjectState = ObjectState.Update;
                            //lbusIBSHdr.LoadIbsDetails();
                        }
                    }
                    if (IsHealthOrMedicare)
                        lstrCoverageCode = lbusIBSHdr.GetGroupHealthCoverageCodeDescription(icdoPersonAccountGhdv.Coverage_Ref_ID);
                    //PIR 24918
                    if ((icdoPersonAccount.plan_id == busConstant.PlanIdVision || icdoPersonAccount.plan_id == busConstant.PlanIdDental) && lstrCoverageCode.IsNullOrEmpty())
                        lstrCoverageCode = icdoPersonAccountGhdv.level_of_coverage_description;
                    //uat pir 1461 --//start
                    string lstrPaymentMethod = string.Empty;
                    //uat pir : 2374 - as part of removal of ibs_effective_Date in payment election
                    //if (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date > lbusIBSHdr.icdoIbsHeader.billing_month_and_year)
                    //{
                    //    lstrPaymentMethod = busConstant.IBSModeOfPaymentPersonalCheck;
                    //}
                    //else 
                    if (ibusPaymentElection.icdoPersonAccountPaymentElection.payment_method_value == busConstant.IBSModeOfPaymentACH)
                    {
                        LoadPersonAccountAchDetail();
                        if (iclbPersonAccountAchDetail.Count == 0)
                        {
                            lstrPaymentMethod = busConstant.IBSModeOfPaymentPersonalCheck;
                        }
                        busPersonAccountAchDetail lobjACHDetail = iclbPersonAccountAchDetail.Where(o => busGlobalFunctions.CheckDateOverlapping(lbusIBSHdr.icdoIbsHeader.billing_month_and_year,
                            o.icdoPersonAccountAchDetail.ach_start_date, o.icdoPersonAccountAchDetail.ach_end_date == DateTime.MinValue ? DateTime.MaxValue : o.icdoPersonAccountAchDetail.ach_end_date))
                            .FirstOrDefault();
                        if (lobjACHDetail != null && lobjACHDetail.icdoPersonAccountAchDetail.pre_note_flag == busConstant.Flag_No)
                        {
                            lstrPaymentMethod = ibusPaymentElection.icdoPersonAccountPaymentElection.payment_method_value;
                        }
                        else
                            lstrPaymentMethod = busConstant.IBSModeOfPaymentPersonalCheck;
                    }
                    else
                    {
                        lstrPaymentMethod = ibusPaymentElection.icdoPersonAccountPaymentElection.payment_method_value;
                    }
                    //uat pir 1344 
                    icdoPersonAccountGhdv.MonthlyPremiumAmount -= idecEmprShareMonthlyPremium;
                    icdoPersonAccountGhdv.PremiumExcludingFeeAmount -= idecEmprSharePremium;
                    icdoPersonAccountGhdv.FeeAmount -= idecEmprShareFee;
                    icdoPersonAccountGhdv.BuydownAmount -= idecEmprShareBuydownAmt;
                    icdoPersonAccountGhdv.total_rhic_amount -= idecEmprShareRHICAmt;
                    icdoPersonAccountGhdv.other_rhic_amount -= idecEmprShareOtherRHICAmt;
                    icdoPersonAccountGhdv.js_rhic_amount -= idecEmprShareJSRHICAmt;

					//IBS Adjustment creation logic changed (Affected area enroll to enroll scenario) - logged under Backlog PIR 8022 and 8111
                    if (icolPosNegIbsDetail.IsNull())
                        icolPosNegIbsDetail = new Collection<busIbsDetail>();

                    //uat pir 1461 --//end
                    if (IsHealthOrMedicare)
                    {
                        if (icdoPersonAccountGhdv.MonthlyPremiumAmount - icdoPersonAccountGhdv.total_rhic_amount >= 0 &&
                            icdoPersonAccountGhdv.PremiumExcludingFeeAmount > 0) // PROD PIR ID 6260
                        {
                            //PIR 24918
                            if (IsAdjustmentExistsPriorToEnteredEffectiveDate(ibusProviderOrgPlan.icdoOrgPlan.org_id, ldatEffectedDate, icdoPersonAccountGhdv.MonthlyPremiumAmount - icdoPersonAccountGhdv.total_rhic_amount, lstrPaymentMethod, lstrCoverageCode))
                            {
                                lbusIBSHdr.UpdateIBSDetailStatusReviewAndValidToIgnore(icdoPersonAccount.person_account_id, ldatEffectedDate);
                                lbusIBSHdr.AddIBSDetailForGHDV(icdoPersonAccount.person_account_id, icdoPersonAccount.person_id, icdoPersonAccount.plan_id,
                                    ldatEffectedDate, lstrPaymentMethod, lstrCoverageCode,
                                    icdoPersonAccountGhdv.FeeAmount, icdoPersonAccountGhdv.BuydownAmount, icdoPersonAccountGhdv.MedicarePartDAmount, icdoPersonAccountGhdv.MonthlyPremiumAmount - icdoPersonAccountGhdv.total_rhic_amount,
                                    icdoPersonAccountGhdv.total_rhic_amount, icdoPersonAccountGhdv.other_rhic_amount, icdoPersonAccountGhdv.js_rhic_amount,
                                    icdoPersonAccountGhdv.PremiumExcludingFeeAmount,
                                    icdoPersonAccountGhdv.MonthlyPremiumAmount, ibusProviderOrgPlan.icdoOrgPlan.org_id, aintGHDVHistoryID: iintGHDVHistoryID, astrGroupNumber: istrGroupNumber,
                                    astrCoverageCodeValue: istrCoverageCodeValue, astrRateStructureCode: istrRateStructureCode, acolNegPosIBsDetail: icolPosNegIbsDetail, aintPayeeAccountId: iPayeeAccountForPapit);//prod pir 6076
                            }
                        }
                    }
                    else
                    {
                        //PIR 24918
                        if (IsAdjustmentExistsPriorToEnteredEffectiveDate(ibusProviderOrgPlan.icdoOrgPlan.org_id, ldatEffectedDate, icdoPersonAccountGhdv.MonthlyPremiumAmount - icdoPersonAccountGhdv.total_rhic_amount, lstrPaymentMethod, lstrCoverageCode))
                        {
                            lbusIBSHdr.UpdateIBSDetailStatusReviewAndValidToIgnore(icdoPersonAccount.person_account_id, ldatEffectedDate);
                            lbusIBSHdr.AddIBSDetailForGHDV(icdoPersonAccount.person_account_id, icdoPersonAccount.person_id, icdoPersonAccount.plan_id,
                                ldatEffectedDate, lstrPaymentMethod, lstrCoverageCode,
                                icdoPersonAccountGhdv.FeeAmount, icdoPersonAccountGhdv.BuydownAmount, icdoPersonAccountGhdv.MedicarePartDAmount, icdoPersonAccountGhdv.MonthlyPremiumAmount - icdoPersonAccountGhdv.total_rhic_amount,
                                icdoPersonAccountGhdv.total_rhic_amount, icdoPersonAccountGhdv.other_rhic_amount, icdoPersonAccountGhdv.js_rhic_amount,
                                icdoPersonAccountGhdv.MonthlyPremiumAmount,
                                icdoPersonAccountGhdv.MonthlyPremiumAmount, ibusProviderOrgPlan.icdoOrgPlan.org_id, aintGHDVHistoryID: iintGHDVHistoryID, astrGroupNumber: istrGroupNumber,
                                astrCoverageCodeValue: istrCoverageCodeValue, astrRateStructureCode: istrRateStructureCode, acolNegPosIBsDetail: icolPosNegIbsDetail, aintPayeeAccountId: iPayeeAccountForPapit);//prod pir 6076
                        }
                    }
                }

                ldatEffectedDate = ldatEffectedDate.AddMonths(1);
            }

            //IBS Adjustment creation logic changed (Affected area enroll to enroll scenario) - logged under Backlog PIR 8022 and 8111
            Collection<busIbsDetail> lcolFinal =  ComparePositiveNegativeColIbsDetail();
            // Update changes
            if ((lcolFinal != null) && (lcolFinal.Count > 0))
            {
                if (lbusIBSHdr == null)
                {
                    lbusIBSHdr = new busIbsHeader();
                    if (!lbusIBSHdr.LoadCurrentAdjustmentIBSHeader())
                    {
                        lbusIBSHdr.CreateAdjustmentIBSHeader();
                    }
                    else
                    {
                        lbusIBSHdr.icdoIbsHeader.ienuObjectState = ObjectState.Update;
                    }
                }
                if (lbusIBSHdr != null)
                {
                    lbusIBSHdr.UpdateSummaryData(busConstant.IBSHeaderStatusReview); //?? Save detail here
                    foreach (busIbsDetail lbusIBSDtl in lcolFinal)
                    {
                        if (lbusIBSHdr != null && lbusIBSDtl.icdoIbsDetail.ibs_header_id == 0)
                            lbusIBSDtl.icdoIbsDetail.ibs_header_id = lbusIBSHdr.icdoIbsHeader.ibs_header_id;
                        lbusIBSDtl.UpdateDataObject(lbusIBSDtl.icdoIbsDetail);
                        //prod pir 933
                        if (idtMedicareSplitCodeValue.AsEnumerable().Where(o => o.Field<string>("data1") == lbusIBSDtl.icdoIbsDetail.coverage_code_value).Any())
                            lbusIBSDtl.InsertPersonAccountDependentBillingLink(null, lbusIBSDtl.icdoIbsDetail.person_account_id);
                    }
                }
            }
        }

        private void CreateEmployerAdjustmentForEnrollmentHistoryAdd(DateTime adtEffectiveDate)
        {
            busEmployerPayrollHeader lbusPayrollHdr = null;
            if (ibusPerson == null)
                LoadPerson();
            DateTime ldatEffectedDate = adtEffectiveDate;
            DateTime ldatCurrentPayPeriod = DateTime.MinValue;
            int lintOrgID = 0;
            //PIR 22767
            if (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag == busConstant.Flag_No || 
                ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag.IsNullOrEmpty() || (!ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag.IsNullOrEmpty() 
                && ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag.Trim() == string.Empty)) //PIR-10147
            {
                if (ibusPersonEmploymentDetail == null)
                {
                    LoadPersonEmploymentDetail();
                }
                if (ibusPersonEmploymentDetail.ibusPersonEmployment == null)
                {
                    ibusPersonEmploymentDetail.LoadPersonEmployment();
                }
                lintOrgID = ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id;
            }
            else if (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id != 0)
            {
                lintOrgID = ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id;
            }
            else if (lintOrgID == 0)
            {
                if (ibusPersonEmploymentDetail == null)
                {
                    LoadPersonEmploymentDetail();
                }
                if (ibusPersonEmploymentDetail.ibusPersonEmployment == null)
                {
                    ibusPersonEmploymentDetail.LoadPersonEmployment();
                }
                lintOrgID = ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id;
            }
            if (lintOrgID > 0)
            {
                //PIR 22767
                if (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag == busConstant.Flag_No || ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag.IsNullOrEmpty() ||
                    (!ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag.IsNullOrEmpty() && ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag.Trim() == string.Empty) ||
                    (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag == busConstant.Flag_Yes && ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id != 0)) //PIR-10147
                {
                    DataTable ldtResult = Select<cdoEmployerPayrollHeader>(
                           new string[4] { "org_id", "HEADER_TYPE_VALUE", "REPORT_TYPE_VALUE", "STATUS_VALUE" },
                           new object[4]
                            {
                                lintOrgID,busConstant.PayrollHeaderBenefitTypeInsr, 
                                busConstant.PayrollHeaderReportTypeRegular,busConstant.PayrollHeaderStatusPosted
                            }, null, "EMPLOYER_PAYROLL_HEADER_ID desc");
                    if ((ldtResult != null) && (ldtResult.Rows.Count > 0))
                    {
                        if (!Convert.IsDBNull(ldtResult.Rows[0]["PAYROLL_PAID_DATE"]))
                        {
                            ldatCurrentPayPeriod = Convert.ToDateTime(ldtResult.Rows[0]["PAYROLL_PAID_DATE"]);
                        }
                    }
                }

                if (ldatCurrentPayPeriod == DateTime.MinValue)
                {
                    cdoCodeValue lcdoCodeValue = busGlobalFunctions.GetCodeValueDetails(busConstant.SystemConstantsAndVariablesCodeID, busConstant.SystemConstantsLastEmployerPostingDate);
                    ldatCurrentPayPeriod = new DateTime(Convert.ToDateTime(lcdoCodeValue.data1).Year, Convert.ToDateTime(lcdoCodeValue.data1).Month, 1);
                }

                while (ldatEffectedDate <= ldatCurrentPayPeriod)
                {
                    if (lbusPayrollHdr == null)
                    {
                        lbusPayrollHdr = new busEmployerPayrollHeader();
                        if (!lbusPayrollHdr.LoadCurrentAdjustmentPayrollHeader(lintOrgID, busConstant.PayrollHeaderBenefitTypeInsr))
                        {
                            lbusPayrollHdr.CreateInsuranceAdjustmentPayrollHeader(lintOrgID);
                            lbusPayrollHdr.iclbEmployerPayrollDetail = new Collection<busEmployerPayrollDetail>();
                        }
                        else
                        {
                            lbusPayrollHdr.icdoEmployerPayrollHeader.ienuObjectState = ObjectState.Update;
                            lbusPayrollHdr.LoadEmployerPayrollDetail();
                        }
                    }
                    //Must reload Provider Org Plan based on effective date. if the enrollment change happens between 2010 and 2011 , chances for two different providers.
                    if (icdoPersonAccount.person_employment_dtl_id > 0)
                    {
                        LoadOrgPlan(ldatEffectedDate);
                        LoadProviderOrgPlan(ldatEffectedDate);
                    }
                    else
                    {
                        LoadActiveProviderOrgPlan(ldatEffectedDate);
                    }
                    CalculatePremiumAmount(ldatEffectedDate);

                    //uat pir 1344 
                    icdoPersonAccountGhdv.MonthlyPremiumAmount = idecEmprShareMonthlyPremium > 0 ? idecEmprShareMonthlyPremium : icdoPersonAccountGhdv.MonthlyPremiumAmount;
                    icdoPersonAccountGhdv.PremiumExcludingFeeAmount = idecEmprSharePremium > 0 ? idecEmprSharePremium : icdoPersonAccountGhdv.PremiumExcludingFeeAmount;
                    icdoPersonAccountGhdv.FeeAmount = idecEmprShareFee > 0 ? idecEmprShareFee : icdoPersonAccountGhdv.FeeAmount;
                    icdoPersonAccountGhdv.BuydownAmount = idecEmprShareBuydownAmt > 0 ? idecEmprShareBuydownAmt : icdoPersonAccountGhdv.BuydownAmount; // PIR 11239
                    icdoPersonAccountGhdv.MedicarePartDAmount = idecEmprShareMedicarePartDAmt > 0 ? idecEmprShareMedicarePartDAmt : icdoPersonAccountGhdv.MedicarePartDAmount;
                    icdoPersonAccountGhdv.total_rhic_amount = idecEmprShareRHICAmt > 0 ? idecEmprShareRHICAmt : icdoPersonAccountGhdv.total_rhic_amount;
                    icdoPersonAccountGhdv.other_rhic_amount = idecEmprShareOtherRHICAmt > 0 ? idecEmprShareOtherRHICAmt : icdoPersonAccountGhdv.other_rhic_amount;
                    icdoPersonAccountGhdv.js_rhic_amount = idecEmprShareJSRHICAmt > 0 ? idecEmprShareJSRHICAmt : icdoPersonAccountGhdv.js_rhic_amount;

                    if (icolPosNegEmployerPayrollDtl.IsNull())
                        icolPosNegEmployerPayrollDtl = new Collection<busEmployerPayrollDetail>();

                    if (IsHealthOrMedicare)
                    {
                        if (icdoPersonAccountGhdv.MonthlyPremiumAmount >= 0 && icdoPersonAccountGhdv.PremiumExcludingFeeAmount > 0 && IsPayrollDtlAdjustmentExistsPriorToEnteredEffectiveDate(ibusProviderOrgPlan.icdoOrgPlan.org_id,
                            ldatEffectedDate, icdoPersonAccountGhdv.MonthlyPremiumAmount, busConstant.PayrollDetailRecordTypePositiveAdjustment, istrCoverageCodeValue))
                        {
                            lbusPayrollHdr.UpdateEmployerPayrollDetailStatusReviewAndValidToIgnore(icdoPersonAccount.person_id, icdoPersonAccount.plan_id, ldatEffectedDate);
                            lbusPayrollHdr.AddInsuranceAdjustmentPayrollDetail(icdoPersonAccount.person_id, icdoPersonAccount.plan_id, ibusPerson.icdoPerson.first_name,
                                    ibusPerson.icdoPerson.last_name, ibusPerson.icdoPerson.ssn, busConstant.PayrollDetailRecordTypePositiveAdjustment,
                                    ldatEffectedDate, icdoPersonAccountGhdv.MonthlyPremiumAmount, ibusProviderOrgPlan.icdoOrgPlan.org_id,
                                    icdoPersonAccountGhdv.FeeAmount, icdoPersonAccountGhdv.BuydownAmount, icdoPersonAccountGhdv.MedicarePartDAmount, icdoPersonAccountGhdv.total_rhic_amount, icdoPersonAccountGhdv.other_rhic_amount, icdoPersonAccountGhdv.js_rhic_amount,
                                    icdoPersonAccountGhdv.idecHSAAmount, icdoPersonAccountGhdv.idecHSAVendorAmount,//pir 7705
                                    aintGHDVHistoryID: iintGHDVHistoryID, astrGroupNumber: istrGroupNumber,//uat pir 1429 : post ghdv_history_id        
                                    astrCoverageCodeValue: istrCoverageCodeValue, astrRateStructureCode: istrRateStructureCode, acolPosNegEmployerPayrollDtl: icolPosNegEmployerPayrollDtl);//prod pir 6076
                        }
                        else if (icdoPersonAccountGhdv.MonthlyPremiumAmount < 0 && IsPayrollDtlAdjustmentExistsPriorToEnteredEffectiveDate(ibusProviderOrgPlan.icdoOrgPlan.org_id,
                            ldatEffectedDate, icdoPersonAccountGhdv.MonthlyPremiumAmount * -1, busConstant.PayrollDetailRecordTypeNegativeAdjustment, istrCoverageCodeValue))
                        {
                            lbusPayrollHdr.AddInsuranceAdjustmentPayrollDetail(icdoPersonAccount.person_id, icdoPersonAccount.plan_id, ibusPerson.icdoPerson.first_name,
                                ibusPerson.icdoPerson.last_name, ibusPerson.icdoPerson.ssn, busConstant.PayrollDetailRecordTypeNegativeAdjustment,
                                ldatEffectedDate, icdoPersonAccountGhdv.MonthlyPremiumAmount * -1, ibusProviderOrgPlan.icdoOrgPlan.org_id,
                                icdoPersonAccountGhdv.FeeAmount * -1, icdoPersonAccountGhdv.BuydownAmount * -1, icdoPersonAccountGhdv.MedicarePartDAmount * -1, icdoPersonAccountGhdv.total_rhic_amount * -1, icdoPersonAccountGhdv.other_rhic_amount * -1, icdoPersonAccountGhdv.js_rhic_amount * -1,
                                icdoPersonAccountGhdv.idecHSAAmount * -1, icdoPersonAccountGhdv.idecHSAVendorAmount * -1,//pir 7705
                                aintGHDVHistoryID: iintGHDVHistoryID, astrGroupNumber: istrGroupNumber,//uat pir 1429 : post ghdv_history_id                        
                                astrCoverageCodeValue: istrCoverageCodeValue, astrRateStructureCode: istrRateStructureCode, acolPosNegEmployerPayrollDtl: icolPosNegEmployerPayrollDtl);//prod pir 6076
                        }
                    }
                    else
                    {
                        // PremiumExcludingFeeAmount will be calculated only for the Group Health or Medicare plans and will not be assigned for other plans.
                        if (icdoPersonAccountGhdv.MonthlyPremiumAmount >= 0 && IsPayrollDtlAdjustmentExistsPriorToEnteredEffectiveDate(ibusProviderOrgPlan.icdoOrgPlan.org_id,
                            ldatEffectedDate, icdoPersonAccountGhdv.MonthlyPremiumAmount, busConstant.PayrollDetailRecordTypePositiveAdjustment, istrCoverageCodeValue))
                        {
                            lbusPayrollHdr.UpdateEmployerPayrollDetailStatusReviewAndValidToIgnore(icdoPersonAccount.person_id, icdoPersonAccount.plan_id, ldatEffectedDate);
                            lbusPayrollHdr.AddInsuranceAdjustmentPayrollDetail(icdoPersonAccount.person_id, icdoPersonAccount.plan_id, ibusPerson.icdoPerson.first_name,
                                ibusPerson.icdoPerson.last_name, ibusPerson.icdoPerson.ssn, busConstant.PayrollDetailRecordTypePositiveAdjustment,
                                ldatEffectedDate, icdoPersonAccountGhdv.MonthlyPremiumAmount, ibusProviderOrgPlan.icdoOrgPlan.org_id,
                                icdoPersonAccountGhdv.FeeAmount, 0M, 0M, icdoPersonAccountGhdv.total_rhic_amount, icdoPersonAccountGhdv.other_rhic_amount, icdoPersonAccountGhdv.js_rhic_amount,
                                icdoPersonAccountGhdv.idecHSAAmount, icdoPersonAccountGhdv.idecHSAVendorAmount,//pir 7705
                                aintGHDVHistoryID: iintGHDVHistoryID, astrGroupNumber: istrGroupNumber,//uat pir 1429 : post ghdv_history_id        
                                astrCoverageCodeValue: istrCoverageCodeValue, astrRateStructureCode: istrRateStructureCode, acolPosNegEmployerPayrollDtl: icolPosNegEmployerPayrollDtl);//prod pir 6076
                        }
                        else if (icdoPersonAccountGhdv.MonthlyPremiumAmount < 0 && IsPayrollDtlAdjustmentExistsPriorToEnteredEffectiveDate(ibusProviderOrgPlan.icdoOrgPlan.org_id,
                            ldatEffectedDate, icdoPersonAccountGhdv.MonthlyPremiumAmount * -1, busConstant.PayrollDetailRecordTypeNegativeAdjustment, istrCoverageCodeValue))
                        {
                            lbusPayrollHdr.AddInsuranceAdjustmentPayrollDetail(icdoPersonAccount.person_id, icdoPersonAccount.plan_id, ibusPerson.icdoPerson.first_name,
                                ibusPerson.icdoPerson.last_name, ibusPerson.icdoPerson.ssn, busConstant.PayrollDetailRecordTypeNegativeAdjustment,
                                ldatEffectedDate, icdoPersonAccountGhdv.MonthlyPremiumAmount * -1, ibusProviderOrgPlan.icdoOrgPlan.org_id,
                                icdoPersonAccountGhdv.FeeAmount * -1, 0M, 0M, icdoPersonAccountGhdv.total_rhic_amount * -1, icdoPersonAccountGhdv.other_rhic_amount * -1, icdoPersonAccountGhdv.js_rhic_amount * -1,
                                icdoPersonAccountGhdv.idecHSAAmount * -1, icdoPersonAccountGhdv.idecHSAVendorAmount * -1,//pir 7705
                                aintGHDVHistoryID: iintGHDVHistoryID, astrGroupNumber: istrGroupNumber,//uat pir 1429 : post ghdv_history_id                        
                                astrCoverageCodeValue: istrCoverageCodeValue, astrRateStructureCode: istrRateStructureCode, acolPosNegEmployerPayrollDtl: icolPosNegEmployerPayrollDtl);//prod pir 6076
                        }
                    }

                    ldatEffectedDate = ldatEffectedDate.AddMonths(1);
                }
            }
            Collection<busEmployerPayrollDetail> lcolFinal = ComparePositiveNegativeColEmployerPayrollDetail();
            if ((lcolFinal != null) && (lcolFinal.Count > 0))
            {
                if (lbusPayrollHdr == null)
                {
                    lbusPayrollHdr = new busEmployerPayrollHeader();
                    if (!lbusPayrollHdr.LoadCurrentAdjustmentPayrollHeader(lintOrgID, busConstant.PayrollHeaderBenefitTypeInsr))
                    {
                        lbusPayrollHdr.CreateInsuranceAdjustmentPayrollHeader(lintOrgID);
                    }
                    else
                    {
                        lbusPayrollHdr.icdoEmployerPayrollHeader.ienuObjectState = ObjectState.Update;
                    }
                }
                if (lbusPayrollHdr != null)
                {
                    lbusPayrollHdr.UpdateDataObject(lbusPayrollHdr.icdoEmployerPayrollHeader);
                    foreach (busEmployerPayrollDetail lbusPayrollDtl in lcolFinal)
                    {
                        if (lbusPayrollHdr != null && lbusPayrollDtl.icdoEmployerPayrollDetail.employer_payroll_header_id == 0)
                            lbusPayrollDtl.icdoEmployerPayrollDetail.employer_payroll_header_id = lbusPayrollHdr.icdoEmployerPayrollHeader.employer_payroll_header_id;
                        lbusPayrollDtl.UpdateDataObject(lbusPayrollDtl.icdoEmployerPayrollDetail);
                        //prod pir 933
                        if (idtMedicareSplitCodeValue.AsEnumerable().Where(o => o.Field<string>("data1") == lbusPayrollDtl.icdoEmployerPayrollDetail.coverage_code).Any())
                            lbusPayrollDtl.InsertPersonAccountDependentBillingLink(null, icdoPersonAccountGhdv.person_account_id);
                    }
                }
            }
        }

        public bool IsInvalidCombinationForCoverageCode()
        {
            if (iclbCoverageRef == null)
                LoadCoverageCodeByFilter();

            bool lblnResult = false;
            if (iclbCoverageRef.Count == 0)
            {
                lblnResult = true;
            }
            else if ((iclbCoverageRef.Count == 1) && (iclbCoverageRef[0].coverage_code.IsNull()))
            {
                lblnResult = true;
            }
            return lblnResult;
        }

        public bool IsInvalidCoverageCodeSelected()
        {
            if (iclbCoverageRef == null)
                LoadCoverageCodeByFilter();

            bool lblnResult = true;
            foreach (var lcdoCoverageRef in iclbCoverageRef)
            {
                if (icdoPersonAccountGhdv.coverage_code == lcdoCoverageRef.coverage_code)
                {
                    lblnResult = false;
                    break;
                }
            }
            return lblnResult;
        }      
        
        public utlCollection<cdoPersonAccount> LoadFromPersonAccount()
        {
            utlCollection<cdoPersonAccount> lclbPersonAccount = new utlCollection<cdoPersonAccount>();
            if (ibusPerson.iclbPersonDependentByDependent == null)
                ibusPerson.LoadPersonDependentByDependent();

            foreach (var lbusPersonDependent in ibusPerson.iclbPersonDependentByDependent)
            {
                if (lbusPersonDependent.iclbPersonAccountDependent == null)
                    lbusPersonDependent.LoadPersonAccountDependent();

                foreach (var lbusPADependent in lbusPersonDependent.iclbPersonAccountDependent)
                {
                    if (lbusPADependent.ibusPersonAccount == null)
                        lbusPADependent.LoadPersonAccount();

                    if (lbusPADependent.ibusPersonAccount.icdoPersonAccount.plan_id == icdoPersonAccount.plan_id)
                    {
                        if (lbusPADependent.ibusPersonAccount.ibusPerson == null)
                            lbusPADependent.ibusPersonAccount.LoadPerson();

                        cdoPersonAccount lcdoPersonAccount = new cdoPersonAccount();
                        lcdoPersonAccount.person_account_id = lbusPADependent.icdoPersonAccountDependent.person_account_id;
                        lcdoPersonAccount.person_full_name_with_person_account_id =
                            lbusPADependent.ibusPersonAccount.ibusPerson.icdoPerson.person_id.ToString() + " - " +
                            lbusPADependent.ibusPersonAccount.ibusPerson.icdoPerson.FullName;
                        lclbPersonAccount.Add(lcdoPersonAccount);
                    }
                }
            }
            return lclbPersonAccount;
        }

        public string istrPreviousCoverageCode { get; set; }

        // UAT PIR ID 472
        // UCS-022-00a - Flex Premium Conversion record exists for this provider.
        public bool IsConversionExistsAndCoverageModified()
        {
            if (ibusPerson.IsNull()) LoadPerson();

            if ((icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled) ||
                (istrPreviousCoverageCode != icdoPersonAccountGhdv.coverage_code))
            {
                if (ibusPerson.IsActiveFlexPremiumConversionExists(icdoPersonAccount.plan_id))
                {
                    return true;
                }
            }
            return false;
        }

        # region 22 correspondence
        public busPersonAccount ibusPersonAccount { get; set; }
        public void LoadPersonAccount()
        {
            if (ibusPersonAccount.IsNull())
                ibusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
            ibusPersonAccount.icdoPersonAccount = icdoPersonAccount;
            ibusPersonAccount.ibusPlan = ibusPlan;
        }
        //PER - 0155
        public DateTime idtPremiumDueDate
        {
            get
            {
                DateTime ldtResultDate = DateTime.MinValue;

                return ldtResultDate;
            }
        }

        public bool IsPlanHealthOrMedicare
        {
            get
            {
                return (icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth
                    || icdoPersonAccount.plan_id == busConstant.PlanIdMedicarePartD);
            }
        }

        //ENR- 5155
        //load coverage code description
        //public string istrCoverageCode { get; set; }
        public string istrCoverageCodeGHDV { get; set; }
        public void LoadCoverageCodeDescription()
        {
            if (IsPlanHealthOrMedicare)
            {
                if (iclbCoverageRef.IsNull())
                    LoadCoverageCodeByFilter();

                var lenumCoverageCodeList = iclbCoverageRef.Where(lobjCoverageRef => lobjCoverageRef.coverage_code == icdoPersonAccountGhdv.coverage_code);

                string lstrCoverageCode = string.Empty;
                if (lenumCoverageCodeList.Count() > 0)
                {
                    char[] lsplitter = { '-' };
                    //  lintIndex = lenumCoverageCodeList.FirstOrDefault().short_description.IndexOf("-");
                    istrCoverageCode = lenumCoverageCodeList.FirstOrDefault().short_description.Split(lsplitter).Last();
                    istrCoverageCodeGHDV = lenumCoverageCodeList.FirstOrDefault().description.Split(lsplitter).Last();
                }
            }
        }

        public override void LoadCorresProperties(string astrTemplateName)
        {
            LoadCoverageCodeDescription();
            LoadPaymentElectionProperties();
            IsDependentEnrolledInLife();

            if (ibusPerson.IsNull())
                LoadPerson();
            if (ibusPerson.ibusSpouse.IsNull())
                ibusPerson.LoadSpouse();
        }

        public void LoadCancelledEndDate(ref DateTime adtCancelledEffectiveDate)
        {
            if (_ibusHistory.IsNull())
                LoadPreviousHistory();

            if (_ibusHistory.icdoPersonAccountGhdvHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceCancelled)
            {
                adtCancelledEffectiveDate = _ibusHistory.icdoPersonAccountGhdvHistory.start_date;
            }
        }


        public void LoadSuspendedEndDateForGHDV(ref DateTime adtSuspendedDate, ref DateTime adtFirstMonthOfSuspendedDate)
        {
            if (_ibusHistory.IsNull())
                LoadPreviousHistory();

            if (_ibusHistory.icdoPersonAccountGhdvHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended)
            {
                adtFirstMonthOfSuspendedDate = _ibusHistory.icdoPersonAccountGhdvHistory.end_date;
                adtSuspendedDate = _ibusHistory.icdoPersonAccountGhdvHistory.start_date;
            }
        }

        //load coverage date
        public void LoadCoverageDateForGHDV()
        {
            if (idtCoverageBeginDate == DateTime.MinValue)
            {
                bool lblnIsCOBRAAllowed = false;
                if (icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth)
                {
                    if (!string.IsNullOrEmpty(icdoPersonAccountGhdv.cobra_type_value))
                        lblnIsCOBRAAllowed = true;
                }
                else
                {
                    if (icdoPersonAccountGhdv.dental_insurance_type_value == busConstant.DentalInsuranceTypeCOBRA
                        || icdoPersonAccountGhdv.vision_insurance_type_value == busConstant.VisionInsuranceTypeCOBRA)
                        lblnIsCOBRAAllowed = true;
                }
                if (lblnIsCOBRAAllowed)
                {
                    if (iclbPersonAccountGHDVHistory.IsNull())
                        LoadPersonAccountGHDVHistory();
                    if (iclbPersonAccountGHDVHistory.Count > 0)
                    {
                        idtCoverageBeginDate = iclbPersonAccountGHDVHistory.First().icdoPersonAccountGhdvHistory.start_date;
                        DateTime ldtTempDate = iclbPersonAccountGHDVHistory.First().icdoPersonAccountGhdvHistory.start_date;
                        ldtTempDate = ldtTempDate.AddYears(1);
                        //last day of the start date year
                        idtCoverageEndDate = new DateTime(ldtTempDate.Year, 12, 31);
                    }
                }
            }
        }
        public void LoadEnrolledCoverageDateForGHDV()
        {
            if (iclbPersonAccountGHDVHistory.IsNull())
                LoadPersonAccountGHDVHistory();
            if (iclbPersonAccountGHDVHistory.Count > 0)
            {
                //PIR 22932 - latest history end_date where start_date != end_date and cobra_type_value is null and plan_participation_status_value = ENL2
               busPersonAccountGhdvHistory lbusPersonAccountGhdvHistory = iclbPersonAccountGHDVHistory.FirstOrDefault(i=>(i.icdoPersonAccountGhdvHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled 
                                        && i.icdoPersonAccountGhdvHistory.start_date.Date != i.icdoPersonAccountGhdvHistory.end_date.Date && i.icdoPersonAccountGhdvHistory.cobra_type_value.IsNullOrEmpty()));
                if(lbusPersonAccountGhdvHistory.IsNotNull())
                    idtEnrolledCoverageEndDate = lbusPersonAccountGhdvHistory.icdoPersonAccountGhdvHistory.end_date;
            }
        }
        public string istrIsPlanHealth
        {
            get
            {
                return (icdoPersonAccount.plan_id.Equals(busConstant.PlanIdGroupHealth) ? busConstant.Flag_Yes : busConstant.Flag_No);
            }
        }
        public string istrIsPlanVision
        {
            get
            {
                return (icdoPersonAccount.plan_id.Equals(busConstant.PlanIdVision) ? busConstant.Flag_Yes : busConstant.Flag_No);
            }
        }
        public string istrIsPlanDental
        {
            get
            {
                return (icdoPersonAccount.plan_id.Equals(busConstant.PlanIdDental) ? busConstant.Flag_Yes : busConstant.Flag_No);
            }
        }

        public string istrIsACH { get; set; }
        public string istrIsPremiumCheck { get; set; }
        public void LoadPaymentElectionProperties()
        {
            if (ibusPaymentElection.IsNull())
                LoadPaymentElection();

            if (ibusPaymentElection.icdoPersonAccountPaymentElection.payment_method_value == busConstant.IBSModeOfPaymentPersonalCheck)
                istrIsPremiumCheck = busConstant.Flag_Yes;
            if (ibusPaymentElection.icdoPersonAccountPaymentElection.payment_method_value == busConstant.IBSModeOfPaymentACH)
                istrIsACH = busConstant.Flag_Yes;
        }
        # endregion

        // UAT PIR ID 1083
        public bool IsMemberAge65orMore()
        {
            if (ibusPerson.IsNull())
                LoadPerson();
            if ((busGlobalFunctions.CalulateAge(ibusPerson.icdoPerson.date_of_birth, icdoPersonAccount.history_change_date)) >= 65)
                return true;
            return false;
        }

        //UAT PIR 1959 : DUAL Coverage Code 0003 should not consider as SINGLE
        public bool IsCoverageCodeCodeSingle()
        {
            return (icdoPersonAccountGhdv.coverage_code.Equals("0001")
                || (icdoPersonAccountGhdv.coverage_code.Equals("0004"))
                || (icdoPersonAccountGhdv.coverage_code.Equals("0006"))
                || (icdoPersonAccountGhdv.coverage_code.Equals("0021"))
                || (icdoPersonAccountGhdv.coverage_code.Equals("0024"))
                || (icdoPersonAccountGhdv.coverage_code.Equals("0041"))
                || (icdoPersonAccountGhdv.coverage_code.Equals("0046")));
        }

        public void ESSLoadCoverageCodeDescription()
        {
            if (IsPlanHealthOrMedicare)
            {
                if (iclbCoverageRef.IsNull())
                    LoadCoverageCodeByFilter();

                var lenumCoverageCodeList = iclbCoverageRef.Where(lobjCoverageRef => lobjCoverageRef.coverage_code == icdoPersonAccountGhdv.coverage_code);

                string lstrCoverageCode = string.Empty;
                if (lenumCoverageCodeList.Count() > 0)
                {
                    char[] lsplitter = { '-' };
                    istrCoverageCode = lenumCoverageCodeList.FirstOrDefault().short_description;
                    istrCoverageCodeDescription = lenumCoverageCodeList.FirstOrDefault().short_description.Split(lsplitter).Last();
                }
            }
        }

        public string GetGroupNumberForBCBS()
        {
            string lstrGroupNumberForBCBS = string.Empty;

            if (ibusPaymentElection.IsNull())
                LoadPaymentElection();

            if (icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth || icdoPersonAccount.plan_id == busConstant.PlanIdMedicarePartD)
            {
                if (
                    (icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeRetiree) && (icdoPersonAccountGhdv.derived_rate_structure_code != null) && //pir 7973
                    (icdoPersonAccountGhdv.derived_rate_structure_code != "0014") && (icdoPersonAccountGhdv.derived_rate_structure_code != "0015")
                    && icdoPersonAccountGhdv.cobra_type_value.IsNullOrEmpty())
                    lstrGroupNumberForBCBS = "000001";
                else if (
                    (icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeState) &&
                    (icdoPersonAccountGhdv.cobra_type_value.IsNotNullOrEmpty()))
                    lstrGroupNumberForBCBS = "000002";
                else if (
                    (icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeNonState) &&
                    (icdoPersonAccountGhdv.cobra_type_value.IsNotNullOrEmpty()))//uat pir 1430 - Removed cobra share logic here as per satya's mail dated on 9/28/2010
                    lstrGroupNumberForBCBS = "000003";
                else if (
                    (icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeRetiree) &&
                    (icdoPersonAccountGhdv.cobra_type_value.IsNotNullOrEmpty()))//uat pir 1430- Removed cobra share logic here as per satya's mail dated on 9/28/2010
                    lstrGroupNumberForBCBS = "000004";
                else if (
                    (icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeRetiree) &&
                    (icdoPersonAccountGhdv.cobra_type_value.IsNullOrEmpty()) &&
                    ((icdoPersonAccountGhdv.derived_rate_structure_code == "0014") || (icdoPersonAccountGhdv.derived_rate_structure_code == "0015")))
                    lstrGroupNumberForBCBS = "000005";
                else if (IsGroupNumber06ConditionSatisfiedforBCBS())
                {
                    lstrGroupNumberForBCBS = "000006";
                }
            }
            return lstrGroupNumberForBCBS;
        }

        public bool IsGroupNumber06ConditionSatisfiedforBCBS()
        {
            if (ibusPaymentElection == null)
                LoadPaymentElection();
            if (
                icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeNonState &&
                icdoPersonAccountGhdv.cobra_type_value.IsNullOrEmpty() &&
                ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag == busConstant.Flag_Yes &&
                ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id != 0)
                return true;
            return false;
        }

        public string GetGroupNumber()
        {
            string lstrGroupNumber = string.Empty;

            if (ibusPaymentElection.IsNull())
                LoadPaymentElection();

            if (icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth || icdoPersonAccount.plan_id == busConstant.PlanIdMedicarePartD)
            {
                if ((icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled) &&
                    (icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeRetiree) && (icdoPersonAccountGhdv.derived_rate_structure_code != null) && //pir 7973
                    (icdoPersonAccountGhdv.derived_rate_structure_code != "0014") && (icdoPersonAccountGhdv.derived_rate_structure_code != "0015")
                    && icdoPersonAccountGhdv.cobra_type_value.IsNullOrEmpty())
                    lstrGroupNumber = "000001";
                else if ((icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled) &&
                    (icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeState) &&
                    (icdoPersonAccountGhdv.cobra_type_value.IsNotNullOrEmpty()))
                    lstrGroupNumber = "000002";
                else if ((icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled) &&
                    (icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeNonState) &&
                    (icdoPersonAccountGhdv.cobra_type_value.IsNotNullOrEmpty()))//uat pir 1430 - Removed cobra share logic here as per satya's mail dated on 9/28/2010
                    lstrGroupNumber = "000003";
                else if ((icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled) &&
                    (icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeRetiree) &&
                    (icdoPersonAccountGhdv.cobra_type_value.IsNotNullOrEmpty()))//uat pir 1430- Removed cobra share logic here as per satya's mail dated on 9/28/2010
                    lstrGroupNumber = "000004";
                else if ((icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled) &&
                    (icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeRetiree) &&
                    (icdoPersonAccountGhdv.cobra_type_value.IsNullOrEmpty()) &&
                    ((icdoPersonAccountGhdv.derived_rate_structure_code == "0014") || (icdoPersonAccountGhdv.derived_rate_structure_code == "0015")))
                    lstrGroupNumber = "000005";
                else if (IsGroupNumber06ConditionSatisfied())
                {
                    lstrGroupNumber = "000006";
                }
            }
            else if (icdoPersonAccount.plan_id == busConstant.PlanIdVision)
            {
                if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                {
                    if (icdoPersonAccountGhdv.vision_insurance_type_value == busConstant.VisionInsuranceTypeRetiree)
                        lstrGroupNumber = "000001";
                    else if (icdoPersonAccountGhdv.vision_insurance_type_value == busConstant.VisionInsuranceTypeCOBRA)
                        lstrGroupNumber = "000002";
                }
            }
            // PIR 10448
            else if (icdoPersonAccount.plan_id == busConstant.PlanIdDental)
            {
                if (icdoPersonAccountGhdv.dental_insurance_type_value == busConstant.DentalInsuranceTypeActive)
                    lstrGroupNumber = "9005374820001";
                else if (icdoPersonAccountGhdv.dental_insurance_type_value == busConstant.DentalInsuranceTypeRetiree)
                    lstrGroupNumber = "9005374820002";
                else if (icdoPersonAccountGhdv.dental_insurance_type_value == busConstant.DentalInsuranceTypeCOBRA)
                    lstrGroupNumber = "9005374829272";
            }
            return lstrGroupNumber;
        }

        public bool IsGroupNumber06ConditionSatisfied()
        {
            //if (ibusPaymentElection == null) // PIR 933 - missing group number
                LoadPaymentElection();
            if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled &&
                icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeNonState &&
                icdoPersonAccountGhdv.cobra_type_value.IsNullOrEmpty() &&
                ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag == busConstant.Flag_Yes &&
                ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id != 0)
                return true;
            return false;
        }

        public bool IsMedicareEligibleDataEntered
        {
            get
            {
                if (icdoPersonAccountGhdv.medicare_claim_no.IsNotNullOrEmpty()
                    && icdoPersonAccountGhdv.medicare_part_a_effective_date != DateTime.MinValue
                    && icdoPersonAccountGhdv.medicare_part_b_effective_date != DateTime.MinValue)
                {
                    return true;
                }
                return false;
            }
        }

        //Framework 5.0 upgrade issues
        //public bool IsMemberAge65OrMore()
        //{
        //    return IsMedicareAgeAttained(icdoPersonAccount.current_plan_start_date_no_null);
        //}

        public bool IsMedicareAgeAttained(DateTime adtEffectiveDate)
        {
            if (ibusPerson == null)
                LoadPerson();

            DateTime ldtTempDate = ibusPerson.icdoPerson.date_of_birth.AddYears(65).AddMonths(-1);
            ldtTempDate = new DateTime(ldtTempDate.Year, ldtTempDate.Month, DateTime.DaysInMonth(ldtTempDate.Year, ldtTempDate.Month));

            if (ldtTempDate >= adtEffectiveDate)
            {
                return true;
            }
            return false;
        }

        #region UCS - 032

        public string istrESSProviderOrgName { get; set; }
        public int iintESSProviderOrgID { get; set; }
        public void ESSLoadProviderOrgName(int aintOrgID)
        {
            DataTable ldtOrgPlan = Select<cdoOrgPlan>(new string[2] { "org_id", "plan_id" },
                                                        new object[2] { aintOrgID, icdoPersonAccount.plan_id }, null, null);
            foreach (DataRow ldr in ldtOrgPlan.Rows)
            {
                //prod pir 6695 : load latest provider
                if (busGlobalFunctions.CheckDateOverlapping(DateTime.Today, Convert.ToDateTime(ldr["participation_start_date"]),
                    ldr["participation_end_date"] == DBNull.Value ? DateTime.MaxValue : Convert.ToDateTime(ldr["participation_end_date"])))
                {
                    DataTable ldtProvider = Select<cdoOrgPlanProvider>(new string[1] { "org_plan_id" },
                                                                        new object[1] { ldr["org_plan_id"] }, null, null);
                    if (ldtProvider.Rows.Count > 0)
                    {
                        busOrgPlanProvider lobjProvider = new busOrgPlanProvider { icdoOrgPlanProvider = new cdoOrgPlanProvider() };
                        lobjProvider.icdoOrgPlanProvider.LoadData(ldtProvider.Rows[0]);
                        lobjProvider.LoadProviderOrg();
                        istrESSProviderOrgName = lobjProvider.ibusProviderOrg.icdoOrganization.org_name;
                        iintESSProviderOrgID = lobjProvider.ibusProviderOrg.icdoOrganization.org_id;
                    }
                }
            }
        }

        #endregion

        //UAT PIR -1084
        //if medicare rate is selected then date and claim number must be entered
        public bool IsAllMedicareDetailsEntered()
        {
            if (IsPlanHealthOrMedicare)
            {
                if (iclbAccountEmploymentDetail.IsNull())
                    LoadPersonAccountEmploymentDetails();

                bool lblnFlag = true; // UAT PIR 2643 - Associated Employment should be ended to trigger the validation.
                foreach (busPersonAccountEmploymentDetail lobjPAEmpDtl in iclbAccountEmploymentDetail)
                {
                    if (lobjPAEmpDtl.ibusEmploymentDetail.IsNull()) lobjPAEmpDtl.LoadPersonEmploymentDetail();
                    if (lobjPAEmpDtl.ibusEmploymentDetail.icdoPersonEmploymentDetail.end_date == DateTime.MinValue)
                    {
                        lblnFlag = false;
                        break;
                    }
                }

                if (lblnFlag)
                {
                    if (iclbCoverageRef.IsNull())
                        LoadCoverageCodeByFilter();

                    var lenumCoverageCodeList = iclbCoverageRef.Where(lobjCoverageRef => lobjCoverageRef.coverage_code == icdoPersonAccountGhdv.coverage_code);

                    string lstrCoverageCode = string.Empty;
                    if (lenumCoverageCodeList.Count() > 0)
                    {
                        istrCoverageCode = lenumCoverageCodeList.FirstOrDefault().short_description.ToLower();
                    }

                    if (istrCoverageCode.IsNotNull())
                    {
                        if ((IsAllMedicareFieldNotEntered() && istrCoverageCode.Contains("medicare"))
                            ||
                            ((!IsAllMedicareFieldNotEntered()) && !istrCoverageCode.Contains("medicare")))
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
            return (icdoPersonAccountGhdv.medicare_claim_no.IsNullOrEmpty()
                                   && icdoPersonAccountGhdv.medicare_part_a_effective_date == DateTime.MinValue
                                   && icdoPersonAccountGhdv.medicare_part_b_effective_date == DateTime.MinValue);
        }

        public string istrEffectiveDateLongDate
        {
            get
            {
                return ibusHistory.icdoPersonAccountGhdvHistory.start_date == DateTime.MinValue ? string.Empty : ibusHistory.icdoPersonAccountGhdvHistory.start_date.ToString(busConstant.DateFormatLongDate);
            }
        }

        /// <summary>
        /// PROD pir 5538 : need to recalculate premium based on plan effective date
        /// </summary>
        public void RecalculatePremiumBasedOnPlanEffectiveDate()
        {
            //Must reload Provider Org Plan based on effective date. if the enrollment change happens between 2010 and 2011 , chances for two different providers.
            if (icdoPersonAccount.person_employment_dtl_id > 0)
            {
                LoadOrgPlan(idtPlanEffectiveDate);
                LoadProviderOrgPlan(idtPlanEffectiveDate);
            }
            else
            {
                LoadActiveProviderOrgPlan(idtPlanEffectiveDate);
            }
            //Reset the Monthly Premium based on the Screen Data
            if (IsHealthOrMedicare)
            {
                if (icdoPersonAccountGhdv.overridden_structure_code.IsNotNullOrEmpty())
                {
                    LoadRateStructureForUserStructureCode();
                }
                else
                {
                    LoadHealthParticipationDate();
                    LoadHealthPlanOption();
                    LoadRateStructure(idtPlanEffectiveDate);
                }

                LoadCoverageRefID();
                GetMonthlyPremiumAmountByRefID(idtPlanEffectiveDate);
            }
            else
            {
                //Recalculate the Premium based on the New Effective Date
                GetMonthlyPremiumAmount(idtPlanEffectiveDate);
            }
        }

        #region PROD PIR ID 933

        public Collection<cdoCodeValue> iclbMedicareSplit { get; set; }

        public Collection<cdoCodeValue> iclbNonMedicareSplit { get; set; }

        public void LoadMedicareSplit()
        {
            if (iclbMedicareSplit.IsNull()) iclbMedicareSplit = new Collection<cdoCodeValue>();
            DataTable ldtbResults = iobjPassInfo.isrvDBCache.GetCacheData("sgs_code_value", "code_id = '" + 1922.ToString() + "' ");
            iclbMedicareSplit = Sagitec.DataObjects.doBase.GetCollection<cdoCodeValue>(ldtbResults);
        }

        public void LoadNonMedicareSplit()
        {
            if (iclbNonMedicareSplit.IsNull()) iclbNonMedicareSplit = new Collection<cdoCodeValue>();
            DataTable ldtbResults = iobjPassInfo.isrvDBCache.GetCacheData("sgs_code_value", "code_id = '" + 1923.ToString() + "' ");
            iclbNonMedicareSplit = Sagitec.DataObjects.doBase.GetCollection<cdoCodeValue>(ldtbResults);
        }

        public bool IsCoverageNeedsToSplit()
        {
            if (iclbMedicareSplit.IsNull()) LoadMedicareSplit();
            return iclbMedicareSplit.Where(lobj => lobj.data1.ToString().Equals(icdoPersonAccountGhdv.coverage_code)).Any();
        }

        public void SplitCoverage(int aintTotalMedicareSplit, int aintTotalNonMedicareSplit, ref string astrMedicareCode, ref string astrNonMedicareCode)
        {
            string lstrNumberofNonMedicareSplit = string.Empty;

            if (iclbMedicareSplit.IsNull()) LoadMedicareSplit();
            if (iclbNonMedicareSplit.IsNull()) LoadNonMedicareSplit();

            //PIR 14944 - Changed 'AND' condition to 'OR' condition
            if (aintTotalMedicareSplit > 0 || aintTotalNonMedicareSplit > 0)
            {
                // Medicare Split Coverage
                var lvarMedicareSplit = iclbMedicareSplit.Where(lobj => lobj.data1.ToString().Equals(icdoPersonAccountGhdv.coverage_code) &&
                                        Convert.ToInt32(lobj.data3) == aintTotalMedicareSplit).FirstOrDefault();
                if (lvarMedicareSplit.IsNotNull())
                    astrMedicareCode = lvarMedicareSplit.data2.Substring(2, 2);

                // Non Medicare Split Coverage
                if (aintTotalNonMedicareSplit == 1)
                    lstrNumberofNonMedicareSplit = busConstant.NonMedicareSplitCountOne;
                else
                    lstrNumberofNonMedicareSplit = busConstant.NonMedicareSplitCountTwoorMore;
                var lvarNonMedicareSplit = iclbNonMedicareSplit.Where(lobj => lobj.data3 == lstrNumberofNonMedicareSplit).FirstOrDefault();
                if (lvarNonMedicareSplit.IsNotNull())
                    astrNonMedicareCode = lvarNonMedicareSplit.data1.Substring(2, 2);
            }
        }

        #endregion

        //RMR section for Vision Plan
        // Load HIPAA Reference ID from Organization Table.
        public string GetHIPAAReferenceID()
        {
            if (IsCOBRAValueSelected())
                return "00999";
            else
            {
                if (idtPlanEffectiveDate == DateTime.MinValue)
                    LoadPlanEffectiveDate();
                if (icdoPersonAccount.person_employment_dtl_id <= 0)
                    icdoPersonAccount.person_employment_dtl_id = GetEmploymentDetailID();
                // Employment is there
                if (icdoPersonAccount.person_employment_dtl_id != 0)
                {
                    DataTable ldtbResult = busBase.Select("cdoPersonEmployment.LoadEmploymentByEmpDtlID",
                                                new object[1] { icdoPersonAccount.person_employment_dtl_id });
                    if (ldtbResult.Rows.Count > 0)
                    {
                        // Employment Detail and Employment is not Loaded, If necessary we can load
                        busPersonEmploymentDetail lobjPersonEmploymentDetail = new busPersonEmploymentDetail();
                        lobjPersonEmploymentDetail.ibusPersonEmployment = new busPersonEmployment();
                        lobjPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization = new busOrganization();
                        lobjPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization = new cdoOrganization();
                        lobjPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.LoadData(ldtbResult.Rows[0]);
                        if (!string.IsNullOrEmpty(lobjPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.hipaa_reference_id))
                            return lobjPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.hipaa_reference_id;
                    }
                }
                else // No Employment 
                    return "00002";
            }

            return "00001"; // All other cases - Referred to NDPERS
        }

        //RMR section for Dental Plan
        public string GetBranch()
        {
            if (IsCOBRAValueSelected())
                return "199   ";
            else
            {
                if (idtPlanEffectiveDate == DateTime.MinValue)
                    LoadPlanEffectiveDate();
                if (icdoPersonAccount.person_employment_dtl_id <= 0)
                    icdoPersonAccount.person_employment_dtl_id = GetEmploymentDetailID();
                // Employment is there
                if (icdoPersonAccount.person_employment_dtl_id != 0)
                {
                    DataTable ldtbResult = busBase.Select("cdoPersonEmployment.LoadEmploymentByEmpDtlID",
                                                new object[1] { icdoPersonAccount.person_employment_dtl_id });
                    if (ldtbResult.Rows.Count > 0)
                    {
                        busPersonEmploymentDetail lbusPersonEmploymentDetail = new busPersonEmploymentDetail();
                        lbusPersonEmploymentDetail.ibusPersonEmployment = new busPersonEmployment();
                        lbusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization = new busOrganization();
                        lbusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization = new cdoOrganization();
                        lbusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.LoadData(ldtbResult.Rows[0]);
                        if (!string.IsNullOrEmpty(lbusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.hipaa_branch_id))
                            return lbusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.hipaa_branch_id.PadRight(6, ' ');
                    }
                }
                else // No Employment 
                    return "102   ";
            }
            return "101   "; // All other cases - Referred to NDPERS
        }

        //RMR section for Dental Plan
        public string GetBenefitOptionCode()
        {
            string lstrBOC = string.Empty;
            if (ibusPerson == null)
                LoadPerson();
            if (ibusPerson.ibusPersonCurrentAddress == null)
                ibusPerson.LoadPersonCurrentAddress();
            if (ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress != null)
                lstrBOC = ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.lstr_Benefit_Option_Code;
            return lstrBOC;
        }

        //prod pir 7220
        public string istrPreviousCOBRATypeValue { get; set; }

        //pir 7817
        public bool IsACHDetailWithNoEndDateExists()
        {
            bool lblnResult = false;
            if (iclbPersonAccountAchDetail == null)
                LoadPersonAccountAchDetail();
            lblnResult = iclbPersonAccountAchDetail.Where(i => i.icdoPersonAccountAchDetail.ach_end_date.IsNull() || i.icdoPersonAccountAchDetail.ach_end_date.Equals(DateTime.MinValue)).Any();
            return lblnResult;
        }

        //pir 7705
        public bool IsFlexAccountExistsDuringHealth()
        {
            bool lblnResult = false;
            if (icdoPersonAccountGhdv.hsa_effective_date != DateTime.MinValue)
            {
                if (ibusPerson.IsNull())
                    LoadPerson();
                if (ibusPerson.icolPersonAccount.IsNull())
                    ibusPerson.LoadPersonAccount();
                if (ibusPerson.icolPersonAccount.Count > 0)
                {
                    busPersonAccount lbusPersonAccount = ibusPerson.icolPersonAccount.Where(i => i.icdoPersonAccount.plan_id == busConstant.PlanIdFlex).FirstOrDefault();
                    if (lbusPersonAccount.IsNotNull())
                    {
                        lbusPersonAccount.LoadPersonAccountFlex();
                        if (lbusPersonAccount.ibusPersonAccountFlex.IsNotNull())
                        { //Flex account exists.
                            lbusPersonAccount.ibusPersonAccountFlex.LoadFlexCompHistory();
                            if (lbusPersonAccount.ibusPersonAccountFlex.iclbFlexCompHistory.IsNotNull())
                            {
                                IEnumerable<busPersonAccountFlexCompHistory> ienumMSRA = lbusPersonAccount.ibusPersonAccountFlex.iclbFlexCompHistory.Where(i => i.icdoPersonAccountFlexCompHistory.level_of_coverage_value == busConstant.FlexLevelOfCoverageMedicareSpending);
                                if (ienumMSRA.Count() > 0)
                                {
                                    foreach (busPersonAccountFlexCompHistory lbusPersonAccountFlexCompHistory in ienumMSRA)
                                    {
                                        // PIR 11912
                                        if (icdoPersonAccountGhdv.reason_value == busConstant.ChangeReasonAnnualEnrollment)
                                        {
                                            if (lbusPersonAccountFlexCompHistory.icdoPersonAccountFlexCompHistory.annual_pledge_amount > 0 &&
                                                (icdoPersonAccountGhdv.hsa_effective_date == lbusPersonAccountFlexCompHistory.icdoPersonAccountFlexCompHistory.effective_start_date) &&
                                                (lbusPersonAccountFlexCompHistory.icdoPersonAccountFlexCompHistory.effective_end_date == DateTime.MinValue) &&
                                                (lbusPersonAccountFlexCompHistory.icdoPersonAccountFlexCompHistory.plan_participation_status_value == busConstant.PlanParticipationStatusFlexCompEnrolled))
                                            {
                                                lblnResult = true;
                                            }
                                        }
                                        else
                                        {
                                            if (lbusPersonAccountFlexCompHistory.icdoPersonAccountFlexCompHistory.annual_pledge_amount > 0
                                                && icdoPersonAccountGhdv.hsa_effective_date.Year == lbusPersonAccountFlexCompHistory.icdoPersonAccountFlexCompHistory.effective_start_date.Year &&
                                                lbusPersonAccountFlexCompHistory.icdoPersonAccountFlexCompHistory.effective_end_date == DateTime.MinValue &&
                                                lbusPersonAccountFlexCompHistory.icdoPersonAccountFlexCompHistory.plan_participation_status_value == busConstant.PlanParticipationStatusFlexCompEnrolled)
                                            {
                                                lblnResult = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return lblnResult;
        }

        // PIR-8253
        public bool IsValidMedicareClaimNo()
        {
            if (icdoPersonAccountGhdv.medicare_claim_no != null)
            {
                Regex lobjexp = new Regex("^[a-zA-Z0-9 ]*$");
                if (!(lobjexp.IsMatch(icdoPersonAccountGhdv.medicare_claim_no)))
                    return false;
            }
            return true;
        }

        // PIR-8587
        public bool IsPremiumConversionEnrolledDentalVision()
        {
            if ((icdoPersonAccountGhdv.premium_conversion_indicator_flag == "Y") &&
                ((icdoPersonAccountGhdv.dental_insurance_type_value == busConstant.DentalInsuranceTypeRetiree) || (icdoPersonAccountGhdv.vision_insurance_type_value == busConstant.VisionInsuranceTypeRetiree)))
            {
                return true;
            }
            return false;
        }

        // PIR 8874 - Medicare Part A/Part B effective date must be first day of a month
        public bool IsMedicareEffectiveDateFirstDayOfMonth()
        {
            if ((icdoPersonAccountGhdv.medicare_part_a_effective_date != DateTime.MinValue) || (icdoPersonAccountGhdv.medicare_part_b_effective_date != DateTime.MinValue))
            {
                if ((icdoPersonAccountGhdv.medicare_part_a_effective_date.Day != 1) || (icdoPersonAccountGhdv.medicare_part_b_effective_date.Day != 1))
                {
                    return false;
                }
            }

            return true;
        }

        //PIR 10125
        public bool IsInsuranceTypeActiveAndPremiumEnrolledNotSelected()
        {
            if (icdoPersonAccountGhdv.vision_insurance_type_value == busConstant.VisionInsuranceTypeActive || icdoPersonAccountGhdv.dental_insurance_type_value == busConstant.DentalInsuranceTypeActive)
            {
                if (icdoPersonAccountGhdv.premium_conversion_indicator_flag == null && !icdoPersonAccount.is_from_mss) // PIR 10649 - Validation not needed for MSS
                {
                    return true;
                }
            }
            return false;
        }

        //PIR 14848 - Medicare Part D
        public bool IsLowIncomeCredit()
        {
            if (busGlobalFunctions.GetData1ByCodeValue(52, busConstant.LowIncomeCredit, iobjPassInfo) == busConstant.Flag_Yes)
                return true;
            return false; 
        }

        public bool IsMedicarePartDEnrollmentEnded()
        {
            if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended)
            {
                busPersonAccountMedicarePartDHistory lobjPersonAccountMedicarePartDHistory = new busPersonAccountMedicarePartDHistory();
                if (lobjPersonAccountMedicarePartDHistory.FindPersonAccount(icdoPersonAccount.person_account_id))
                {
                    if (lobjPersonAccountMedicarePartDHistory.FindMedicareByMemberPersonID(icdoPersonAccount.person_id))
                    {
                        return true;
                    }
                }
                return false;
            }
            return false;
        }
        public busPayeeAccount ibusPayeeAccountForCor { get; set; } //PIR 19355

        public void LoadingRateStructureCodeForCobra()
        {
            if (ibusPersonEmploymentDetail?.ibusPersonEmployment?.icdoPersonEmployment?.org_id > 0)
            {
                LoadOrgPlan(ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id);
            }
        }

        #region PIR-19997
        public Collection<busPersonAccountGhdvHsa> iclbPersonAccountGhdvHsa { get; set; }
        public busPersonAccountGhdvHsa ibusPersonAccountGhdvHsa { get; set; }
        public void LoadPersonAccountGHDVHsa()
        {
            if (ibusPersonAccountGhdvHsa == null)
            {
                ibusPersonAccountGhdvHsa = new busPersonAccountGhdvHsa();
                ibusPersonAccountGhdvHsa.icdoPersonAccountGhdvHsa = new doPersonAccountGhdvHsa();
            }
            DataTable ldtbList = Select<doPersonAccountGhdvHsa>(
                                                 new string[1] { "person_account_id" },
                                                 new object[1] { icdoPersonAccount.person_account_id },null,"PERSON_ACCOUNT_GHDV_HSA_ID DESC");
            iclbPersonAccountGhdvHsa = GetCollection<busPersonAccountGhdvHsa>(ldtbList, "icdoPersonAccountGhdvHsa");
        }
        public void LoadPersonAccountGHDVHsaFutureDated()
        {
            if (ibusPersonAccountGhdvHsa == null)
            {
                LoadPersonAccountGHDVHsa();
            }
            //iclbPersonAccountGhdvHsa = iclbPersonAccountGhdvHsa.Where(Hsa => Hsa.icdoPersonAccountGhdvHsa.contribution_start_date >= DateTime.Today || Hsa.icdoPersonAccountGhdvHsa.contribution_end_date >= DateTime.Today).ToList().ToCollection();            

            //The ESS contacts will have to see the current and some of the past elections to verify payroll deductions.show all records where End Date is NULL or End Date is within 1 year of GetDate()
            if (iclbPersonAccountGhdvHsa.Count > 0)
                iclbPersonAccountGhdvHsa = iclbPersonAccountGhdvHsa.Where(Hsa => Hsa.icdoPersonAccountGhdvHsa.contribution_end_date == DateTime.MinValue ||
                                            (Hsa.icdoPersonAccountGhdvHsa.contribution_end_date <= DateTime.Today ?
                                           (busGlobalFunctions.DateDiffByMonth(Hsa.icdoPersonAccountGhdvHsa.contribution_end_date, DateTime.Today) - 1 <= 12) :
                                           Hsa.icdoPersonAccountGhdvHsa.contribution_end_date >= DateTime.Today)).ToList().ToCollection();
        }
        public bool FindGHDVHsaByPersonAccountGhdvID(int AintPersonAccountGhdvID) => (Select<doPersonAccountGhdvHsa>(
                new string[1] { "person_account_ghdv_id" },
                new object[1] { AintPersonAccountGhdvID }, null, null).Rows.Count == 1);
        #endregion PIR-19997

        public bool CheckInsuranceType()
        {

            bool lblnResult = false;
            if (icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth)
            {
                if ((icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeState || 
                    icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeNonState) &&
                    string.IsNullOrEmpty(icdoPersonAccountGhdv.cobra_type_value))
                    lblnResult = true;
            }
            else if (icdoPersonAccount.plan_id == busConstant.PlanIdDental)
            {
                if (icdoPersonAccountGhdv.dental_insurance_type_value == busConstant.DentalInsuranceTypeActive ||
                    icdoPersonAccountGhdv.dental_insurance_type_value == busConstant.DentalInsuranceTypeCOBRA)
                    lblnResult = true;
            }
            else if (icdoPersonAccount.plan_id == busConstant.PlanIdVision)
            {
                if (icdoPersonAccountGhdv.vision_insurance_type_value == busConstant.VisionInsuranceTypeActive ||
                    icdoPersonAccountGhdv.vision_insurance_type_value == busConstant.VisionInsuranceTypeCOBRA)
                    lblnResult = true;
            }

            return lblnResult;
        }

        public bool CheckResourceAndInsuranceTypeForOverlap()
        {
            int lintCount = 0;

            lintCount = Convert.ToInt32(DBFunction.DBExecuteScalar("cdoPersonAccount.CheckIfLoggedInUserHasOverlapResource", new object[3] { iobjPassInfo.iintUserSerialID, 0, busConstant.OverlapResourceEnhancedGHDV },
            iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));

            if (lintCount > 0)
                return true;

            lintCount = Convert.ToInt32(DBFunction.DBExecuteScalar("cdoPersonAccount.CheckIfLoggedInUserHasOverlapResource", new object[3] { iobjPassInfo.iintUserSerialID, busConstant.OverlapResourceGHDV, 0 },
            iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));

            if (lintCount > 0 && CheckInsuranceType())
            {
                return true;
            }

            return false;
        }
        public void UpdateEndDateForSuspendStatus()
        {
            if (iclbPersonAccountDependent == null || iclbPersonAccountDependent.Count == 0)
                LoadPersonAccountDependent();
            if (iclbPersonAccountDependent.Count > 0)
            {
                busPersonAccountGhdvHistory lbusPersonAccountGhdvHistory = iclbPersonAccountGHDVHistory
                                                                                   .FirstOrDefault(history =>
                                                                                       history.icdoPersonAccountGhdvHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled &&
                                                                                       history.icdoPersonAccountGhdvHistory.start_date.Date != history.icdoPersonAccountGhdvHistory.end_date.Date);
                DateTime ldteDepEndDate = lbusPersonAccountGhdvHistory.IsNotNull() && lbusPersonAccountGhdvHistory.icdoPersonAccountGhdvHistory.end_date != DateTime.MinValue
                                                                                     ? lbusPersonAccountGhdvHistory.icdoPersonAccountGhdvHistory.end_date : icdoPersonAccount.history_change_date_no_null.AddDays(-1);
                foreach (busPersonAccountDependent lobjPersonAccountDependent in iclbPersonAccountDependent)
                {

                    if (lobjPersonAccountDependent.icdoPersonAccountDependent.end_date == DateTime.MinValue)
                    {
                        lobjPersonAccountDependent.icdoPersonAccountDependent.plan_id = this.icdoPersonAccount.plan_id;
                        lobjPersonAccountDependent.icdoPersonAccountDependent.end_date = lobjPersonAccountDependent.icdoPersonAccountDependent.start_date > icdoPersonAccount.history_change_date_no_null.AddDays(-1) ? lobjPersonAccountDependent.icdoPersonAccountDependent.start_date : ldteDepEndDate;
                        lobjPersonAccountDependent.icdoPersonAccountDependent.Update();
                    }
                    else if (istrAllowOverlapHistory == "Y" && lobjPersonAccountDependent.icdoPersonAccountDependent.end_date > icdoPersonAccount.history_change_date_no_null.AddDays(-1))
                    {
                        lobjPersonAccountDependent.icdoPersonAccountDependent.plan_id = this.icdoPersonAccount.plan_id;
                        lobjPersonAccountDependent.icdoPersonAccountDependent.end_date = lobjPersonAccountDependent.icdoPersonAccountDependent.start_date > icdoPersonAccount.history_change_date_no_null.AddDays(-1) ? lobjPersonAccountDependent.icdoPersonAccountDependent.start_date : icdoPersonAccount.history_change_date_no_null.AddDays(-1);
                        lobjPersonAccountDependent.icdoPersonAccountDependent.Update();
                    }
                }
            }
        }
        public bool RuleIsEmploymentNotActive()
        {
            if (ibusPersonEmploymentDetail.IsNull())
                LoadPersonEmploymentDetail();
            if (ibusPersonEmploymentDetail.ibusPersonEmployment.IsNull())
                ibusPersonEmploymentDetail.LoadPersonEmployment();
            if (((this.icdoPersonAccount.plan_id == busConstant.PlanIdDental && this.icdoPersonAccountGhdv.dental_insurance_type_value == busConstant.DentalInsuranceTypeActive)
                || (this.icdoPersonAccount.plan_id == busConstant.PlanIdVision && this.icdoPersonAccountGhdv.vision_insurance_type_value == busConstant.VisionInsuranceTypeActive)
                || (this.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth && (this.icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeState
                || this.icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeNonState))) && (this.icdoPersonAccountGhdv.cobra_type_value.IsNullOrEmpty()))
            {
                if (busGlobalFunctions.CheckDateOverlapping(this.icdoPersonAccount.history_change_date, this.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.start_date
                    , this.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.end_date) &&
                    this.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.end_date != DateTime.MinValue)
                {
                    return true;
                }
                return false;
            }
            return false;
        }
        public bool IsHSAPreTaxAgreementFlag
        {
            get
            {
                if ((ibusOrgPlan != null) && (ibusOrgPlan.icdoOrgPlan != null))
                {
                    return ibusOrgPlan.icdoOrgPlan.hsa_pre_tax_agreement == busConstant.Flag_Yes ? true : false;
                }
                return false;
            }
        }
		
		public string istrDependentEnrolledInLife { get; set; }

        public void IsDependentEnrolledInLife()
        {
            if (Convert.ToInt32(DBFunction.DBExecuteScalar("cdoPersonAccount.CheckIfDependentEnrolledInLife", new object[1]
                            { icdoPersonAccount.person_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework)) > 0)
            {
                istrDependentEnrolledInLife = busConstant.Flag_Yes;
            }
        }
        public void LoadPaymentElectionAdjustment()
        {
            if (ibusPaymentElectionAdjustment.IsNull())
                ibusPaymentElectionAdjustment = new busPaymentElectionAdjustment()
                {
                    icdoPaymentElectionAdjustment = new cdoPaymentElectionAdjustment(),
                    ibusPersonAccount = new busPersonAccount() { icdoPersonAccount = new cdoPersonAccount(), ibusPlan = new busPlan() { icdoPlan = new cdoPlan() }, ibusPerson= new busPerson() },
                };
            ibusPaymentElectionAdjustment.ibusPersonAccount.ibusPerson = this.ibusPerson;
            
        }

        public bool iblnIsEnrolledInMedicarePartD
        {
            get
            {
                if(ibusPersonAccount.IsNotNull())
                {
                    return (Select<doPersonAccount>(
                        new string[3] { "person_id", "plan_id", "plan_participation_status_value" },
                        new object[3] { ibusPersonAccount.icdoPersonAccount.person_id, busConstant.PlanIdMedicarePartD,
                                        busConstant.PlanParticipationStatusInsuranceEnrolled }, null, null).Rows.Count == 1);
                }
                return false;
            }
        }
        public bool IsMemberEligibleToEnrollInCurrentYear()
        {
            return icdoPersonAccount.IsNotNull() && icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled 
                    && IsMemberEligibleToEnrollInCurrentYear(icdoPersonAccount.history_change_date, icdoPersonAccountGhdv.reason_value, icdoPersonAccountGhdv.premium_conversion_indicator_flag == busConstant.Flag_Yes ? true : false);
        }

        //PIR 26356 Update exisiting validation 10307 and add a condition to it where if plan is being suspended 1st of Jan next year we skip the warning
        public bool iblnIsEffectiveDateFirstOfNextYear
        {
            get
            {
                if (icdoPersonAccount.history_change_date.Month == 1 && icdoPersonAccount.history_change_date.Day == 1 && 
                    icdoPersonAccount.history_change_date.Year > DateTime.Now.Year)
                    return true;
                return false;
            }
        }

        //PIR 26121 - Visibility for HSA contribution on LOB changed according to history
        public bool IsMemberEnrolledInHDHP()
        {
            LoadPersonAccountGHDV();

            busPersonAccountGhdvHistory lobjGHDVHistory = LoadHistoryByDate(DateTime.Now);

            if (ibusPersonAccountGHDV?.icdoPersonAccountGhdv?.alternate_structure_code_value == "HDHP" || lobjGHDVHistory?.icdoPersonAccountGhdvHistory?.alternate_structure_code_value == "HDHP")
                return true;
            else
                return false;
           
        }
    }
}
