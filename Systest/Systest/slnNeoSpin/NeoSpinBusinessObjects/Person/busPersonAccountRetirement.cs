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
using System.Linq;
using System.Globalization;
using NeoSpin.DataObjects;
using Sagitec.CustomDataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPersonAccountRetirement : busPersonAccountRetirementGen
    {
        public int countPos { get; set; }

        public int FYTDID { get { return 1; } }

        public int LTDID { get { return 2; } }
        public int iintPlanID { get; set; }

        private busPersonAccountRetirementHistory _ibusHistory;
        public busPersonAccountRetirementHistory ibusHistory
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
        public bool iblnIsEEPercentChanged { get; set; }
        public bool iblnIsEEPercentSelectedForFirstEnrollment { get; set; }

        public string istrAllowOverlapHistory { get; set; } 

        public DateTime idtInitialStartDate { get; set; }
        public bool istrIsInitialStartDateChanged { get; set; }

        #region Person overview Summary

        private decimal _idecFAS;

        public decimal idecFAS
        {
            get { return _idecFAS; }
            set { _idecFAS = value; }
        }

        private decimal _idecAccruedBenefit;

        public decimal idecAccruedBenefit
        {
            get { return _idecAccruedBenefit; }
            set { _idecAccruedBenefit = value; }
        }
        private decimal _idecDisability;

        public decimal idecDisability
        {
            get { return _idecDisability; }
            set { _idecDisability = value; }
        }
        private decimal _idecAccruedRHICBenefit;

        public decimal idecAccruedRHICBenefit
        {
            get { return _idecAccruedRHICBenefit; }
            set { _idecAccruedRHICBenefit = value; }
        }
        #endregion

        #region Contribution FYTD Properties
        private decimal _Post_Tax_Total_Contribution;

        public decimal Post_Tax_Total_Contribution
        {
            get { return _Post_Tax_Total_Contribution; }
            set { _Post_Tax_Total_Contribution = value; }
        }
        private decimal _Pre_Tax_Total_Contribution;

        public decimal Pre_Tax_Total_Contribution
        {
            get { return _Pre_Tax_Total_Contribution; }
            set { _Pre_Tax_Total_Contribution = value; }
        }

        private decimal _Pre_Tax_Employee_Contribution;
        public decimal Pre_Tax_Employee_Contribution
        {
            get { return _Pre_Tax_Employee_Contribution; }
            set { _Pre_Tax_Employee_Contribution = value; }
        }

        private decimal _Pre_Tax_Employer_Contribution;
        public decimal Pre_Tax_Employer_Contribution
        {
            get { return _Pre_Tax_Employer_Contribution; }
            set { _Pre_Tax_Employer_Contribution = value; }
        }

        private decimal _Member_Account_Balance;
        public decimal Member_Account_Balance
        {
            get { return _Member_Account_Balance; }
            set { _Member_Account_Balance = value; }
        }

        private decimal _Total_Account_Balance;
        public decimal Total_Account_Balance
        {
            get { return _Total_Account_Balance; }
            set { _Total_Account_Balance = value; }
        }

        private decimal _ee_er_pickup_amount;

        public decimal ee_er_pickup_amount
        {
            get { return _ee_er_pickup_amount; }
            set { _ee_er_pickup_amount = value; }
        }
        private decimal _post_tax_ee_amount;

        public decimal post_tax_ee_amount
        {
            get { return _post_tax_ee_amount; }
            set { _post_tax_ee_amount = value; }
        }
        private decimal _post_tax_er_amount;

        public decimal post_tax_er_amount
        {
            get { return _post_tax_er_amount; }
            set { _post_tax_er_amount = value; }
        }
        private decimal _pre_tax_er_amount;

        public decimal pre_tax_er_amount
        {
            get { return _pre_tax_er_amount; }
            set { _pre_tax_er_amount = value; }
        }
        private decimal _pre_tax_er_ser_pur_cont;
        public decimal pre_tax_er_ser_pur_cont
        {
            get { return _pre_tax_er_ser_pur_cont; }
            set { _pre_tax_er_ser_pur_cont = value; }
        }

        private decimal _pre_tax_ee_amount;

        public decimal pre_tax_ee_amount
        {
            get { return _pre_tax_ee_amount; }
            set { _pre_tax_ee_amount = value; }
        }
        private decimal _ee_rhic_amount;

        public decimal ee_rhic_amount
        {
            get { return _ee_rhic_amount; }
            set { _ee_rhic_amount = value; }
        }
        private decimal _er_rhic_amount;

        public decimal er_rhic_amount
        {
            get { return _er_rhic_amount; }
            set { _er_rhic_amount = value; }
        }
        private decimal _er_vested_amount;

        public decimal er_vested_amount
        {
            get { return _er_vested_amount; }
            set { _er_vested_amount = value; }
        }
        private decimal _interest_amount;

        public decimal interest_amount
        {
            get { return _interest_amount; }
            set { _interest_amount = value; }
        }
        private decimal _Employer_Interest;
        public decimal Employer_Interest
        {
            get { return _Employer_Interest; }
            set { _Employer_Interest = value; }
        }
        private decimal _ee_rhic_ser_pur_cont;

        public decimal ee_rhic_ser_pur_cont
        {
            get { return _ee_rhic_ser_pur_cont; }
            set { _ee_rhic_ser_pur_cont = value; }
        }
        private decimal _er_rhic_ser_pur_cont;

        public decimal er_rhic_ser_pur_cont
        {
            get { return _er_rhic_ser_pur_cont; }
            set { _er_rhic_ser_pur_cont = value; }
        }
        private decimal _post_tax_ee_ser_pur_cont;

        public decimal post_tax_ee_ser_pur_cont
        {
            get { return _post_tax_ee_ser_pur_cont; }
            set { _post_tax_ee_ser_pur_cont = value; }
        }
        private decimal _pre_tax_ee_ser_pur_cont;

        public decimal pre_tax_ee_ser_pur_cont
        {
            get { return _pre_tax_ee_ser_pur_cont; }
            set { _pre_tax_ee_ser_pur_cont = value; }
        }
        private decimal _ee_er_pickup_ser_pur_cont;

        public decimal ee_er_pickup_ser_pur_cont
        {
            get { return _ee_er_pickup_ser_pur_cont; }
            set { _ee_er_pickup_ser_pur_cont = value; }
        }
        //PIR 25920 DC 2025 changes
        public int iintAddlEEContributionPercent { get; set; }
        private decimal _adec_amount;
        public decimal adec_amount
        {
            get { return _adec_amount; }
            set { _adec_amount = value; }
        }
        private decimal _adec_amount_Total_Contribution;
        public decimal adec_amount_Total_Contribution
        {
            get { return _adec_amount_Total_Contribution; }
            set { _adec_amount_Total_Contribution = value; }
        }
        #endregion

        #region Contribution LTD Properties
        private decimal _Post_Tax_Total_Contribution_ltd;
        public decimal Post_Tax_Total_Contribution_ltd
        {
            get { return _Post_Tax_Total_Contribution_ltd; }
            set { _Post_Tax_Total_Contribution_ltd = value; }
        }
        private decimal _Pre_Tax_Total_Contribution_ltd;

        public decimal Pre_Tax_Total_Contribution_ltd
        {
            get { return _Pre_Tax_Total_Contribution_ltd; }
            set { _Pre_Tax_Total_Contribution_ltd = value; }
        }

        private decimal _Pre_Tax_Employee_Contribution_ltd;
        public decimal Pre_Tax_Employee_Contribution_ltd
        {
            get { return _Pre_Tax_Employee_Contribution_ltd; }
            set { _Pre_Tax_Employee_Contribution_ltd = value; }
        }

        private decimal _Pre_Tax_Employer_Contribution_ltd;
        public decimal Pre_Tax_Employer_Contribution_ltd
        {
            get { return _Pre_Tax_Employer_Contribution_ltd; }
            set { _Pre_Tax_Employer_Contribution_ltd = value; }
        }

        private decimal _Member_Account_Balance_ltd;
        public decimal Member_Account_Balance_ltd
        {
            get { return _Member_Account_Balance_ltd; }
            set { _Member_Account_Balance_ltd = value; }
        }

        //prod pir 6003
        public decimal idecTotalVestedContributionsForCorrs { get; set; }
       
        private decimal _Total_Account_Balance_ltd;
        public decimal Total_Account_Balance_ltd
        {
            get { return _Total_Account_Balance_ltd; }
            set { _Total_Account_Balance_ltd = value; }
        }

        private decimal _ee_er_pickup_amount_ltd;

        public decimal ee_er_pickup_amount_ltd
        {
            get { return _ee_er_pickup_amount_ltd; }
            set { _ee_er_pickup_amount_ltd = value; }
        }
        private decimal _post_tax_ee_amount_ltd;

        public decimal post_tax_ee_amount_ltd
        {
            get { return _post_tax_ee_amount_ltd; }
            set { _post_tax_ee_amount_ltd = value; }
        }
        private decimal _post_tax_er_amount_ltd;

        public decimal post_tax_er_amount_ltd
        {
            get { return _post_tax_er_amount_ltd; }
            set { _post_tax_er_amount_ltd = value; }
        }
        private decimal _pre_tax_er_amount_ltd;

        public decimal pre_tax_er_amount_ltd
        {
            get { return _pre_tax_er_amount_ltd; }
            set { _pre_tax_er_amount_ltd = value; }
        }

        private decimal _pre_tax_er_ser_pur_cont_ltd;
        public decimal pre_tax_er_ser_pur_cont_ltd
        {
            get { return _pre_tax_er_ser_pur_cont_ltd; }
            set { _pre_tax_er_ser_pur_cont_ltd = value; }
        }

        private decimal _pre_tax_ee_amount_ltd;

        public decimal pre_tax_ee_amount_ltd
        {
            get { return _pre_tax_ee_amount_ltd; }
            set { _pre_tax_ee_amount_ltd = value; }
        }
        private decimal _ee_rhic_amount_ltd;

        public decimal ee_rhic_amount_ltd
        {
            get { return _ee_rhic_amount_ltd; }
            set { _ee_rhic_amount_ltd = value; }
        }
        private decimal _er_rhic_amount_ltd;

        public decimal er_rhic_amount_ltd
        {
            get { return _er_rhic_amount_ltd; }
            set { _er_rhic_amount_ltd = value; }
        }
        private decimal _er_vested_amount_ltd;

        public decimal er_vested_amount_ltd
        {
            get { return _er_vested_amount_ltd; }
            set { _er_vested_amount_ltd = value; }
        }
        private decimal _interest_amount_ltd;

        public decimal interest_amount_ltd
        {
            get { return _interest_amount_ltd; }
            set { _interest_amount_ltd = value; }
        }

        private decimal _Employer_Interest_ltd;
        public decimal Employer_Interest_ltd
        {
            get { return _Employer_Interest_ltd; }
            set { _Employer_Interest_ltd = value; }
        }

        private decimal _ee_rhic_ser_pur_cont_ltd;

        public decimal ee_rhic_ser_pur_cont_ltd
        {
            get { return _ee_rhic_ser_pur_cont_ltd; }
            set { _ee_rhic_ser_pur_cont_ltd = value; }
        }
        private decimal _er_rhic_ser_pur_cont_ltd;

        public decimal er_rhic_ser_pur_cont_ltd
        {
            get { return _er_rhic_ser_pur_cont_ltd; }
            set { _er_rhic_ser_pur_cont_ltd = value; }
        }
        private decimal _post_tax_ee_ser_pur_cont_ltd;

        public decimal post_tax_ee_ser_pur_cont_ltd
        {
            get { return _post_tax_ee_ser_pur_cont_ltd; }
            set { _post_tax_ee_ser_pur_cont_ltd = value; }
        }
        private decimal _pre_tax_ee_ser_pur_cont_ltd;

        public decimal pre_tax_ee_ser_pur_cont_ltd
        {
            get { return _pre_tax_ee_ser_pur_cont_ltd; }
            set { _pre_tax_ee_ser_pur_cont_ltd = value; }
        }
        private decimal _ee_er_pickup_ser_pur_cont_ltd;

        public decimal ee_er_pickup_ser_pur_cont_ltd
        {
            get { return _ee_er_pickup_ser_pur_cont_ltd; }
            set { _ee_er_pickup_ser_pur_cont_ltd = value; }
        }
        //PIR 25920 DC 2025 changes
        private decimal _adec_amount_ltd;
        public decimal adec_amount_ltd
        {
            get { return _adec_amount_ltd; }
            set { _adec_amount_ltd = value; }
        }
        private decimal __adec_amount_Total_Contribution_ltd;
        public decimal adec_amount_Total_Contribution_ltd
        {
            get { return __adec_amount_Total_Contribution_ltd; }
            set { __adec_amount_Total_Contribution_ltd = value; }
        }
        #endregion

        // PIR 9115 functionality enable/disable property
        public string istrIsPIR9115Enabled
        {
            get
            {
                return busGlobalFunctions.GetData1ByCodeValue(52, "9115", iobjPassInfo);
            }
        }

        //pir 8285
        public String istrIsMutualFundFlagRequired
        {
            get
            {
                return busGlobalFunctions.GetData1ByCodeValue(52, busConstant.MutualFundWindowFlag, utlPassInfo.iobjPassInfo);
            }
        }
        private Collection<busPersonAccountRetirementDbDcTransferEstimate> _iclbDBDCTransfer;
        public Collection<busPersonAccountRetirementDbDcTransferEstimate> iclbDBDCTransfer
        {
            get { return _iclbDBDCTransfer; }
            set { _iclbDBDCTransfer = value; }
        }

        private Collection<busPersonAccountRetirementDbDbTransfer> _iclbDBDBTransfer;
        public Collection<busPersonAccountRetirementDbDbTransfer> iclbDBDBTransfer
        {
            get
            {
                return _iclbDBDBTransfer;
            }
            set
            {
                _iclbDBDBTransfer = value;
            }
        }

        //Property to contain Accrued Benefit details
        private busRetirementBenefitCalculation _ibusRetirementBenefitCalculation;
        public busRetirementBenefitCalculation ibusRetirementBenefitCalculation
        {
            get { return _ibusRetirementBenefitCalculation; }
            set { _ibusRetirementBenefitCalculation = value; }
        }

        //Property to contain Disability details
        private busRetirementBenefitCalculation _ibusDisabilityBenefitCalculation;
        public busRetirementBenefitCalculation ibusDisabilityBenefitCalculation
        {
            get { return _ibusDisabilityBenefitCalculation; }
            set { _ibusDisabilityBenefitCalculation = value; }
        }

        /* UAT PIR: 1991
         * Only display the ‘Unreduced Accrued Benefit’ if a member is currently vested.  If they are not vested; don’t display.  For this we are not projecting any service credit or contributions so only vested member are eligible for this.
         * */
        //Property to Vested Person Plan details
        private busRetirementBenefitCalculation _ibusVestedBenefitCalculation;
        public busRetirementBenefitCalculation ibusVestedBenefitCalculation
        {
            get { return _ibusVestedBenefitCalculation; }
            set { _ibusVestedBenefitCalculation = value; }
        }
        #region DC/457 TIAACREF Enrollment file
        public string filler0 { 
            get
            {
                return "0000000000000000000000000000000000000000";
            } 
        }
        public DateTime idtePayrollDate { get; set; }
        public string istrVestingPercent { get; set; }
        public string istrNewLine
        {
            get
            {
                //PIR 9122
                countPos = countPos + 1;
                if (this.ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_country_value == "0001" && (this.countPos == 3))
                {
                    return string.Empty;
                }
                else
                return Environment.NewLine;
            }
        }
        public string istrYearsOfService { get; set; }
		//PIR 14646
        public DateTime idteMainBenefitTierStartDate
        {
            get
            {
                DateTime result = DateTime.MinValue;
                if (DateTime.TryParse(busGlobalFunctions.GetData1ByCodeValue(7003, "16MT", iobjPassInfo), out result))
                    return result;
                return result;
            }
        }
        //PIR 26282
        public DateTime idteBCIBenefitTierStartDate
        {
            get
            {
                DateTime result = DateTime.MinValue;
                if (DateTime.TryParse(busGlobalFunctions.GetData1ByCodeValue(7003, "23BT", iobjPassInfo), out result))
                    return result;
                return result;
            }
        }
        #endregion

        public Collection<cdoCodeValue> iclbEligibleADECAmountValue = new Collection<cdoCodeValue>();
        //Method to load a new Retirement Benefit Calculation
        public void LoadNewRetirementBenefitCalculation()
        {
            ibusRetirementBenefitCalculation = new busRetirementBenefitCalculation();
            ibusRetirementBenefitCalculation.icdoBenefitCalculation = new cdoBenefitCalculation();

            ibusDisabilityBenefitCalculation = new busRetirementBenefitCalculation();
            ibusDisabilityBenefitCalculation.icdoBenefitCalculation = new cdoBenefitCalculation();

            /* UAT PIR: 1991
            * Only display the ‘Unreduced Accrued Benefit’ if a member is currently vested.  If they are not vested; don’t display.  For this we are not projecting any service credit or contributions so only vested member are eligible for this.
            * */
            ibusVestedBenefitCalculation = new busRetirementBenefitCalculation();
            ibusVestedBenefitCalculation.icdoBenefitCalculation = new cdoBenefitCalculation();
        }

        //Method to load cdo values for Benefit Calculation for Benefit type Retirement and Disability
        public void AssignRetirementBenefitCalculationFields(string astrCalculationType, DateTime? adteRetirementDateFromPortal = null, DateTime? adteTerminationDateFromPortal = null, DateTime? adteActEmpTermDate = null)
        {
            ibusRetirementBenefitCalculation.ibusMember = ibusPerson;
            ibusRetirementBenefitCalculation.ibusPlan = ibusPlan;
            ibusRetirementBenefitCalculation.ibusPersonAccount = ibusPersonAccount;

            ibusDisabilityBenefitCalculation.ibusMember = ibusPerson;
            ibusDisabilityBenefitCalculation.ibusPlan = ibusPlan;
            ibusDisabilityBenefitCalculation.ibusPersonAccount = ibusPersonAccount;

            ibusRetirementBenefitCalculation.icdoBenefitCalculation.benefit_calculation_id = 0;
            ibusDisabilityBenefitCalculation.icdoBenefitCalculation.benefit_calculation_id = 0;
            ibusRetirementBenefitCalculation.icdoBenefitCalculation.calculation_type_value = astrCalculationType;
            ibusDisabilityBenefitCalculation.icdoBenefitCalculation.calculation_type_value = astrCalculationType;
            ibusRetirementBenefitCalculation.icdoBenefitCalculation.created_date = DateTime.Now;
            ibusDisabilityBenefitCalculation.icdoBenefitCalculation.created_date = DateTime.Now;
            ibusRetirementBenefitCalculation.icdoBenefitCalculation.person_id = icdoPersonAccount.person_id;
            ibusDisabilityBenefitCalculation.icdoBenefitCalculation.person_id = icdoPersonAccount.person_id;
            ibusRetirementBenefitCalculation.icdoBenefitCalculation.plan_id = icdoPersonAccount.plan_id;
            ibusDisabilityBenefitCalculation.icdoBenefitCalculation.plan_id = icdoPersonAccount.plan_id;
            ibusRetirementBenefitCalculation.icdoBenefitCalculation.benefit_account_type_value = busConstant.ApplicationBenefitTypeRetirement;
            ibusDisabilityBenefitCalculation.icdoBenefitCalculation.benefit_account_type_value = busConstant.ApplicationBenefitTypeDisability;
            ibusRetirementBenefitCalculation.icdoBenefitCalculation.benefit_option_value = string.Empty;
            ibusRetirementBenefitCalculation.icdoBenefitCalculation.uniform_income_or_ssli_flag = "N";
            ibusDisabilityBenefitCalculation.icdoBenefitCalculation.benefit_option_value = string.Empty;
            ibusDisabilityBenefitCalculation.icdoBenefitCalculation.uniform_income_or_ssli_flag = "N";


            /* UAT PIR: 1991
            * Only display the ‘Unreduced Accrued Benefit’ if a member is currently vested.  If they are not vested; don’t display.  For this we are not projecting any service credit or contributions so only vested member are eligible for this.
            *  Property Set for ibusVestedBenefitCalculation
            * */
            ibusVestedBenefitCalculation.ibusMember = ibusPerson;
            ibusVestedBenefitCalculation.ibusPlan = ibusPlan;
            ibusVestedBenefitCalculation.ibusPersonAccount = ibusPersonAccount;
            ibusVestedBenefitCalculation.icdoBenefitCalculation.benefit_calculation_id = 0;
            ibusVestedBenefitCalculation.icdoBenefitCalculation.calculation_type_value = astrCalculationType;
            ibusVestedBenefitCalculation.icdoBenefitCalculation.created_date = DateTime.Now;
            ibusVestedBenefitCalculation.icdoBenefitCalculation.person_id = icdoPersonAccount.person_id;
            ibusVestedBenefitCalculation.icdoBenefitCalculation.plan_id = icdoPersonAccount.plan_id;
            ibusVestedBenefitCalculation.icdoBenefitCalculation.benefit_account_type_value = busConstant.ApplicationBenefitTypeRetirement;
            ibusVestedBenefitCalculation.icdoBenefitCalculation.benefit_option_value = string.Empty;
            ibusVestedBenefitCalculation.icdoBenefitCalculation.uniform_income_or_ssli_flag = "N";

            if (adteRetirementDateFromPortal.IsNull() || adteRetirementDateFromPortal == DateTime.MinValue)
            {
                if (iclbRetirementContributionAll != null && iclbRetirementContributionAll.Count > 0)
                {
                    //Prod PIR: 4583. The Sorting is moved to a Common place so that the value shown in person overview and batch letters invoking this method are the same.
                    iclbRetirementContributionAll = busGlobalFunctions.Sort<busPersonAccountRetirementContribution>(
                            "icdoPersonAccountRetirementContribution.PayPeriodYearMonth_no_null desc",
                             iclbRetirementContributionAll);

                    foreach (busPersonAccountRetirementContribution lobjRetirementContribution in iclbRetirementContributionAll)
                    {
                        if ((lobjRetirementContribution.icdoPersonAccountRetirementContribution.subsystem_value == busConstant.SubSystemValueEmployerReporting)
                            || (lobjRetirementContribution.icdoPersonAccountRetirementContribution.subsystem_value == busConstant.SubSystemValueConversion)
                            || (lobjRetirementContribution.icdoPersonAccountRetirementContribution.subsystem_value == busConstant.SubSystemValueConversionOpeningBalance))
                        {
                            DateTime ldtTerminationDate = DateTime.MinValue;
                            if (lobjRetirementContribution.icdoPersonAccountRetirementContribution.subsystem_value == busConstant.SubSystemValueConversionOpeningBalance)
                            {
                                //PIR: 1663
                                //For an Opening Balance record there will not be any Pay Period Month and Pay period Year.So effective date is used.
                                ldtTerminationDate = new DateTime(lobjRetirementContribution.icdoPersonAccountRetirementContribution.effective_date.Year,
                                             lobjRetirementContribution.icdoPersonAccountRetirementContribution.effective_date.Month,
                                             DateTime.DaysInMonth(lobjRetirementContribution.icdoPersonAccountRetirementContribution.effective_date.Year,
                                             lobjRetirementContribution.icdoPersonAccountRetirementContribution.effective_date.Month));
                            }
                            else
                            {
                                ldtTerminationDate = new DateTime(lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_year,
                                                lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_month,
                                                DateTime.DaysInMonth(lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_year,
                                                lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_month));
                            }
                            ibusVestedBenefitCalculation.icdoBenefitCalculation.termination_date = ldtTerminationDate;
                            //UAT PIR:1991
                            //Termination Date as the last of the Month following the last contribution date.
                            DateTime ldtActualTerminationDate = ldtTerminationDate.AddDays(1);
                            ibusRetirementBenefitCalculation.icdoBenefitCalculation.termination_date = (adteActEmpTermDate.IsNotNull() && adteActEmpTermDate != DateTime.MinValue) ? 
                                                                                                            (DateTime)adteActEmpTermDate :  ldtActualTerminationDate;
                            ibusDisabilityBenefitCalculation.icdoBenefitCalculation.termination_date = ldtActualTerminationDate;
                            ibusVestedBenefitCalculation.icdoBenefitCalculation.retirement_date = ldtActualTerminationDate;
                            if (ibusPersonAccount.ibusPlan.IsNull())
                                ibusPersonAccount.LoadPlan();

                            ibusRetirementBenefitCalculation.GetNormalRetirementDateBasedOnNormalEligibility(ibusPersonAccount.ibusPlan.icdoPlan.plan_code, null);
                            DateTime ldtRetirementdate = ibusRetirementBenefitCalculation.icdoBenefitCalculation.normal_retirement_date;

                            if (ldtRetirementdate != DateTime.MinValue)
                            {
                                ibusRetirementBenefitCalculation.icdoBenefitCalculation.retirement_date = ldtRetirementdate;
                                ibusDisabilityBenefitCalculation.icdoBenefitCalculation.retirement_date = ldtRetirementdate;
                            }
                            else
                            {
                                ibusRetirementBenefitCalculation.icdoBenefitCalculation.retirement_date = DateTime.Now;
                            }
                            break;
                        }
                    }
                }
                if (ibusRetirementBenefitCalculation.icdoBenefitCalculation.termination_date == DateTime.MinValue)
                {
                    ibusRetirementBenefitCalculation.icdoBenefitCalculation.termination_date = DateTime.Now.AddMonths(-1);
                    ibusRetirementBenefitCalculation.icdoBenefitCalculation.retirement_date = DateTime.Now;
                }
                if (ibusDisabilityBenefitCalculation.icdoBenefitCalculation.termination_date == DateTime.MinValue)
                {
                    ibusDisabilityBenefitCalculation.icdoBenefitCalculation.termination_date = DateTime.Now.AddMonths(-1);
                    ibusDisabilityBenefitCalculation.icdoBenefitCalculation.retirement_date = DateTime.Now;
                }

                if (ibusVestedBenefitCalculation.icdoBenefitCalculation.termination_date == DateTime.MinValue)
                {
                    ibusVestedBenefitCalculation.icdoBenefitCalculation.termination_date = DateTime.Now.AddMonths(-1);
                    ibusVestedBenefitCalculation.icdoBenefitCalculation.retirement_date = DateTime.Now;
                }

                if (ibusRetirementBenefitCalculation.icdoBenefitCalculation.termination_date > ibusRetirementBenefitCalculation.icdoBenefitCalculation.retirement_date)
                    ibusRetirementBenefitCalculation.icdoBenefitCalculation.retirement_date = (adteActEmpTermDate.IsNotNull() && adteActEmpTermDate != DateTime.MinValue) ? 
                                                                                                ((DateTime)adteActEmpTermDate).GetFirstDayofNextMonth() :  
                                                                                                ibusRetirementBenefitCalculation.icdoBenefitCalculation.termination_date;

                if (ibusDisabilityBenefitCalculation.icdoBenefitCalculation.termination_date > ibusDisabilityBenefitCalculation.icdoBenefitCalculation.retirement_date)
                    ibusDisabilityBenefitCalculation.icdoBenefitCalculation.retirement_date = ibusDisabilityBenefitCalculation.icdoBenefitCalculation.termination_date;




                ibusRetirementBenefitCalculation.CalculateMemberAge();
                ibusDisabilityBenefitCalculation.CalculateMemberAge();
                ibusVestedBenefitCalculation.CalculateMemberAge();
                if (!ibusRetirementBenefitCalculation.iblnConsoldatedVSCLoaded)
                    ibusRetirementBenefitCalculation.CalculateConsolidatedVSC();
                if (!ibusDisabilityBenefitCalculation.iblnConsoldatedVSCLoaded)
                    ibusDisabilityBenefitCalculation.CalculateConsolidatedVSC();
                if (!ibusVestedBenefitCalculation.iblnConsoldatedVSCLoaded)
                    ibusVestedBenefitCalculation.CalculateConsolidatedVSC();
            }
            //prod pir 6858
            else
            {
                if (ibusRetirementBenefitCalculation.icdoBenefitCalculation.termination_date == DateTime.MinValue)
                {
                    ibusRetirementBenefitCalculation.icdoBenefitCalculation.termination_date = Convert.ToDateTime(adteTerminationDateFromPortal);
                    ibusRetirementBenefitCalculation.icdoBenefitCalculation.retirement_date = Convert.ToDateTime(adteRetirementDateFromPortal);
                }
                if (ibusDisabilityBenefitCalculation.icdoBenefitCalculation.termination_date == DateTime.MinValue)
                {
                    ibusDisabilityBenefitCalculation.icdoBenefitCalculation.termination_date = Convert.ToDateTime(adteTerminationDateFromPortal);
                    ibusDisabilityBenefitCalculation.icdoBenefitCalculation.retirement_date = Convert.ToDateTime(adteRetirementDateFromPortal);
                }

                if (ibusVestedBenefitCalculation.icdoBenefitCalculation.termination_date == DateTime.MinValue)
                {
                    ibusVestedBenefitCalculation.icdoBenefitCalculation.termination_date = Convert.ToDateTime(adteTerminationDateFromPortal);
                    ibusVestedBenefitCalculation.icdoBenefitCalculation.retirement_date = Convert.ToDateTime(adteRetirementDateFromPortal);
                }
            }

                
        }

        
        public void CalculateRetirementBenefitAmount()
        {
            CalculateRetirementBenefitAmount(false);
        }
        //Method to calculate Unreduced Benefit amount for Benefit type Retirement and Disability
        public void CalculateRetirementBenefitAmount(bool iblnCheckIsPersonCurrentlyVested)
        {
            /* UAT PIR: 1991
            * Only display the ‘Unreduced Accrued Benefit’ if a member is currently vested.  If they are not vested; don’t display.  For this we are not projecting any service credit or contributions so only vested member are eligible for this.            
            * */
            bool iblnIsPersonAllowed = false;

            if (iblnCheckIsPersonCurrentlyVested)
            {
                iblnIsPersonAllowed = ibusVestedBenefitCalculation.IsPersonVested();
            }
            else
            {
                iblnIsPersonAllowed = true;
            }

            if (iblnIsPersonAllowed)
            {
                if (ibusRetirementBenefitCalculation.IsPersonEligible())
                    ibusRetirementBenefitCalculation.CalculateRetirementBenefit();
            }
            //UAT PIR: 2281. Checking whether vested or not is done only for Retirement and  Not for disability.
            if (ibusDisabilityBenefitCalculation.IsPersonEligible())
                ibusDisabilityBenefitCalculation.CalculateRetirementBenefit();

        }

        //Method to calculate FAS
        public void CalculateFAS()
        {
            ibusRetirementBenefitCalculation.CalculateFAS();
        }

        public void LoadPersonAccountDBDBTransfer(bool ablnSummaryData)
        {
            DataTable ldtbList = Select("cdoPersonAccountRetirement.LoadDBDBTransferDetails", new object[1] { icdoPersonAccount.person_account_id });
            _iclbDBDBTransfer = new Collection<busPersonAccountRetirementDbDbTransfer>();


            foreach (DataRow dr in ldtbList.Rows)
            {
                busPersonAccountRetirementDbDbTransfer lobjDBDBTransfer = new busPersonAccountRetirementDbDbTransfer
                {
                    icdoPersonAccountRetirementDbDbTransfer = new cdoPersonAccountRetirementDbDbTransfer()
                };
                lobjDBDBTransfer.icdoPersonAccountRetirementDbDbTransfer.LoadData(dr);
                if (ablnSummaryData)
                {
                    lobjDBDBTransfer.SetSummaryDataForDisplay();
                }
                iclbDBDBTransfer.Add(lobjDBDBTransfer);
            }
        }

        public void LoadFYTDSummary()
        {
            DateTime ldtCurrentDate = DateTime.Now;
            string lstrStartMonth;
            string lstrEndMonth;
            if (ldtCurrentDate.Month >= 7)
            {
                lstrStartMonth = Convert.ToString(ldtCurrentDate.Year) + "07";
                lstrEndMonth = Convert.ToString(ldtCurrentDate.Year + 1) + "06";
            }
            else
            {
                lstrStartMonth = Convert.ToString(ldtCurrentDate.Year - 1) + "07";
                lstrEndMonth = Convert.ToString(ldtCurrentDate.Year) + "06";
            }

            LoadFYTDSummary(lstrStartMonth, lstrEndMonth);
        }

        public void LoadFYTDSummary(string astrStartMonth, string astrEndMonth)
        {
            DataTable ldtbList = Select("cdoPersonAccountRetirement.LoadFYTDSummary",
                        new object[3] { icdoPersonAccount.person_account_id, astrStartMonth, astrEndMonth });
            if (ldtbList.Rows.Count > 0)
            {
                sqlFunction.LoadQueryResult(this, ldtbList.Rows[0]);
            }

            Post_Tax_Total_Contribution = post_tax_ee_amount + post_tax_ee_ser_pur_cont + ee_rhic_amount + ee_rhic_ser_pur_cont;

            Pre_Tax_Employee_Contribution = icdoPersonAccountRetirement.capital_gain + er_vested_amount +
                                            interest_amount + pre_tax_ee_amount + ee_er_pickup_amount +
                                            pre_tax_ee_ser_pur_cont;

            Member_Account_Balance = Post_Tax_Total_Contribution + Pre_Tax_Employee_Contribution;

            Pre_Tax_Employer_Contribution = er_rhic_amount + er_rhic_ser_pur_cont + pre_tax_er_amount +
                                            pre_tax_er_ser_pur_cont + Employer_Interest;

            Pre_Tax_Total_Contribution = Pre_Tax_Employee_Contribution + Pre_Tax_Employer_Contribution;

            Total_Account_Balance = Post_Tax_Total_Contribution + Pre_Tax_Total_Contribution;

            // PROD PIR 6326
            idecPreTaxEmployeeContributionToDisplay = idecCapitalGain + er_vested_amount + interest_amount + pre_tax_ee_amount + ee_er_pickup_amount + pre_tax_ee_ser_pur_cont;
            idecMemberAccountBalanceToDisplay = Post_Tax_Total_Contribution + idecPreTaxEmployeeContributionToDisplay;
            //PIR 25920 DC 2025 changes
            adec_amount_Total_Contribution = adec_amount;
        }

        public void LoadContributiomSummaryByPeriod(DateTime adtStartDate, DateTime adtEndDate, bool ablnIsTFFRTransfer)
        {
            string lstrStartMonth;
            string lstrEndMonth;
            lstrStartMonth = Convert.ToString(adtStartDate.Year) + adtStartDate.Month.ToString("00");
            lstrEndMonth = Convert.ToString(adtEndDate.Year) + adtEndDate.Month.ToString("00");
            if (ablnIsTFFRTransfer)
            {
                LoadFYTDSummaryForTFFRTransfer(adtStartDate, adtEndDate);
            }
            else
            {
                LoadFYTDSummary(lstrStartMonth, lstrEndMonth);
            }
        }
        private void LoadFYTDSummaryForTFFRTransfer(DateTime adtStartDate, DateTime adtEndDate)
        {
            DataTable ldtbList = Select("cdoPersonAccountRetirement.LoadFYTDSummaryForTFFRTransfer",
                        new object[3] { icdoPersonAccount.person_account_id, adtStartDate, adtEndDate });
            if (ldtbList.Rows.Count > 0)
            {
                sqlFunction.LoadQueryResult(this, ldtbList.Rows[0]);
            }

            Post_Tax_Total_Contribution = post_tax_ee_amount + post_tax_ee_ser_pur_cont + ee_rhic_amount + ee_rhic_ser_pur_cont;

            Pre_Tax_Employee_Contribution = icdoPersonAccountRetirement.capital_gain + er_vested_amount +
                                            interest_amount + pre_tax_ee_amount + ee_er_pickup_amount +
                                            pre_tax_ee_ser_pur_cont;

            Member_Account_Balance = Post_Tax_Total_Contribution + Pre_Tax_Employee_Contribution;

            Pre_Tax_Employer_Contribution = er_rhic_amount + er_rhic_ser_pur_cont + pre_tax_er_amount +
                                            pre_tax_er_ser_pur_cont + Employer_Interest;

            Pre_Tax_Total_Contribution = Pre_Tax_Employee_Contribution + Pre_Tax_Employer_Contribution;

            Total_Account_Balance = Post_Tax_Total_Contribution + Pre_Tax_Total_Contribution;
        }
        public bool iblnLTDSummaryLoaded = false;
        public void LoadLTDSummary()
        {
            DataTable ldtbList = Select("cdoPersonAccountRetirement.LoadLTDSummary",
                        new object[1] { icdoPersonAccount.person_account_id });
            if (ldtbList.Rows.Count > 0)
            {
                sqlFunction.LoadQueryResult(this, ldtbList.Rows[0]);
            }
            Post_Tax_Total_Contribution_ltd = post_tax_ee_amount_ltd + post_tax_ee_ser_pur_cont_ltd + ee_rhic_amount_ltd + ee_rhic_ser_pur_cont_ltd;

            Pre_Tax_Employee_Contribution_ltd = icdoPersonAccountRetirement.capital_gain + er_vested_amount_ltd +
                                                interest_amount_ltd + pre_tax_ee_amount_ltd + ee_er_pickup_amount_ltd +
                                                pre_tax_ee_ser_pur_cont_ltd;
                        
            Member_Account_Balance_ltd = Post_Tax_Total_Contribution_ltd + Pre_Tax_Employee_Contribution_ltd;

            Pre_Tax_Employer_Contribution_ltd = er_rhic_amount_ltd + er_rhic_ser_pur_cont_ltd + pre_tax_er_amount_ltd +
                                                pre_tax_er_ser_pur_cont_ltd + Employer_Interest_ltd;

            Pre_Tax_Total_Contribution_ltd = Pre_Tax_Employee_Contribution_ltd + Pre_Tax_Employer_Contribution_ltd;

            Total_Account_Balance_ltd = Post_Tax_Total_Contribution_ltd + Pre_Tax_Total_Contribution_ltd;
            iblnLTDSummaryLoaded = true;

            // PROD PIR 6326
            idecPreTaxEmployeeContributionLTDToDisplay = idecCapitalGain + er_vested_amount_ltd + interest_amount_ltd + pre_tax_ee_amount_ltd + ee_er_pickup_amount_ltd + pre_tax_ee_ser_pur_cont_ltd;
            idecMemberAccountBalanceLTDToDisplay = Post_Tax_Total_Contribution_ltd + idecPreTaxEmployeeContributionLTDToDisplay;

            //PIR 25920 DC 2025 changes
            adec_amount_Total_Contribution_ltd = adec_amount_ltd;
        }

        public void LoadLTDSummaryAsonDate(DateTime adtDate)
        {
            DataTable ldtbList = Select("cdoPersonAccountRetirement.LoadLTDSummaryAsonDate",
                        new object[2] { icdoPersonAccount.person_account_id, adtDate });
            if (ldtbList.Rows.Count > 0)
            {
                sqlFunction.LoadQueryResult(this, ldtbList.Rows[0]);
            }
            Post_Tax_Total_Contribution_ltd = post_tax_ee_amount_ltd + post_tax_ee_ser_pur_cont_ltd + ee_rhic_amount_ltd + ee_rhic_ser_pur_cont_ltd;

            Pre_Tax_Employee_Contribution_ltd = icdoPersonAccountRetirement.capital_gain + er_vested_amount_ltd +
                                                interest_amount_ltd + pre_tax_ee_amount_ltd + ee_er_pickup_amount_ltd +
                                                pre_tax_ee_ser_pur_cont_ltd;

            Member_Account_Balance_ltd = Post_Tax_Total_Contribution_ltd + Pre_Tax_Employee_Contribution_ltd;

            Pre_Tax_Employer_Contribution_ltd = er_rhic_amount_ltd + er_rhic_ser_pur_cont_ltd + pre_tax_er_amount_ltd +
                                                pre_tax_er_ser_pur_cont_ltd + Employer_Interest_ltd;

            Pre_Tax_Total_Contribution_ltd = Pre_Tax_Employee_Contribution_ltd + Pre_Tax_Employer_Contribution_ltd;

            Total_Account_Balance_ltd = Post_Tax_Total_Contribution_ltd + Pre_Tax_Total_Contribution_ltd;
        }

        //This Method is specific to Benefit Calculation. To be used only when invoked in Benefit Calculation Screen
        //This function Fetches the MemberAccountBalance based on Date parameter type set in Code id 350 and Date passed.
        //For Calculation purpose, there are some transaction types in retirement contribution table which needs to be fetched as on effective date and
        // some on Current date. 
        public void LoadLTDSummaryForCalculation(DateTime adtDate, string astrBenefitAccountType, bool ablnIsDROEstimate = false, bool ablnisRefundBenefitOption = false)
        {
            DataTable ldtbList = new DataTable();
            if (ablnIsDROEstimate)
            {
                // PROD PIR ID 1414 - Interest posted after Divorce date should not be included in calculating the benefit amount.
                ldtbList = Select("cdoPersonAccountRetirement.LoadLTDSummarForDROCalculation",
                        new object[2] { icdoPersonAccount.person_account_id, adtDate });
            }
            else
            {
                ldtbList = Select("cdoPersonAccountRetirement.LoadLTDSummaryForCalculation",
                        new object[2] { icdoPersonAccount.person_account_id, adtDate });
            }
            if (ldtbList.Rows.Count > 0)
            {
                sqlFunction.LoadQueryResult(this, ldtbList.Rows[0]);
            }

            // PROD PIR ID 6326 -- Do not include EE RHIC Amount in the non taxable benefit amount for Retirement/Disability
            if (astrBenefitAccountType == busConstant.ApplicationBenefitTypeRetirement ||
                astrBenefitAccountType == busConstant.ApplicationBenefitTypeDisability)
                Post_Tax_Total_Contribution_ltd = post_tax_ee_amount_ltd + post_tax_ee_ser_pur_cont_ltd;
            else
            {
                if(astrBenefitAccountType == busConstant.ApplicationBenefitTypePreRetirementDeath && !ablnisRefundBenefitOption)
                    Post_Tax_Total_Contribution_ltd = post_tax_ee_amount_ltd + post_tax_ee_ser_pur_cont_ltd;
                else
                    Post_Tax_Total_Contribution_ltd = post_tax_ee_amount_ltd + post_tax_ee_ser_pur_cont_ltd + ee_rhic_amount_ltd + ee_rhic_ser_pur_cont_ltd;
            }


            Pre_Tax_Employee_Contribution_ltd = icdoPersonAccountRetirement.capital_gain + er_vested_amount_ltd +
                                                interest_amount_ltd + pre_tax_ee_amount_ltd + ee_er_pickup_amount_ltd +
                                                pre_tax_ee_ser_pur_cont_ltd;

            if ((icdoPersonAccount.plan_id == busConstant.PlanIdDC || icdoPersonAccount.plan_id == busConstant.PlanIdDC2020 || icdoPersonAccount.plan_id == busConstant.PlanIdDC2025) && //PIR 20232 //PIR 25920
                 (astrBenefitAccountType == busConstant.ApplicationBenefitTypePreRetirementDeath ||
                 astrBenefitAccountType == busConstant.ApplicationBenefitTypePostRetirementDeath))
            {
                if (astrBenefitAccountType == busConstant.ApplicationBenefitTypePreRetirementDeath && !ablnisRefundBenefitOption)
                    Member_Account_Balance_ltd = 0.00M;
                else
                    Member_Account_Balance_ltd = ee_rhic_amount_ltd + ee_rhic_ser_pur_cont_ltd; // PROD PIR ID 5543
            }
            else
                Member_Account_Balance_ltd = Post_Tax_Total_Contribution_ltd + Pre_Tax_Employee_Contribution_ltd;

            Pre_Tax_Employer_Contribution_ltd = er_rhic_amount_ltd + er_rhic_ser_pur_cont_ltd + pre_tax_er_amount_ltd +
                                                pre_tax_er_ser_pur_cont_ltd + Employer_Interest_ltd;

            Pre_Tax_Total_Contribution_ltd = Pre_Tax_Employee_Contribution_ltd + Pre_Tax_Employer_Contribution_ltd;

            Total_Account_Balance_ltd = Post_Tax_Total_Contribution_ltd + Pre_Tax_Total_Contribution_ltd;
        }

        //This method is to validate whether DC Participant's Status is not Suspended and then not allow him 
        // to enroll Judges, HP, TFFR or TIAA-CREF Plans.
        public bool IsDCParticipantStatusNotSuspended()
        {
            bool lblnIsDCParticipantStatusNotSuspended = false;
            if ((ibusPlan.icdoPlan.plan_code == busConstant.Plan_Code_Judges) || (ibusPlan.icdoPlan.plan_code == busConstant.Plan_Code_Highway_Patrol)
                || (ibusPlan.icdoPlan.plan_code == busConstant.PlanCodeTFFR) || (ibusPlan.icdoPlan.plan_code == busConstant.PlanCodeTIAA))
            {
                DataTable ldtbList = new DataTable();
                ldtbList = Select<cdoPersonAccount>(new string[2] { "person_id", "plan_id" },
                                       new object[2] { icdoPersonAccount.person_id, busConstant.PlanIdDC }, null, null);
                if (ldtbList != null && ldtbList.Rows.Count == 0)
                {
                    ldtbList = Select<cdoPersonAccount>(new string[2] { "person_id", "plan_id" },
                                      new object[2] { icdoPersonAccount.person_id, busConstant.PlanIdDC2020 }, null, null); //PIR 20232
                }
                //PIR 25920
                if (ldtbList != null && ldtbList.Rows.Count == 0)
                {
                    ldtbList = Select<cdoPersonAccount>(new string[2] { "person_id", "plan_id" },
                                      new object[2] { icdoPersonAccount.person_id, busConstant.PlanIdDC2025 }, null, null); //PIR 25920
                }
                if (ldtbList.Rows.Count > 0)
                {
                    if ((ldtbList.Rows[0]["plan_participation_status_value"]).ToString() == busConstant.PlanParticipationStatusRetirementEnrolled)
                    {
                        lblnIsDCParticipantStatusNotSuspended = true;
                    }
                }
            }
            return lblnIsDCParticipantStatusNotSuspended;
        }
        //Check whether Optional DB Plan Enrollment start date is valid
        public bool ValidateOptionalDBPlanEnrollmentStartDate()
        {
            if ((ibusPersonEmploymentDetail != null) && (ibusPersonEmploymentDetail.ibusPersonEmployment != null))
            {
                if (ibusPlan.IsRetirementPlan())
                {
                    if (ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary)
                    {
                        DataTable ldtbPlanjobClass = Select<cdoPlanJobClassCrossref>(new string[3] { "plan_id", "JOB_CLASS_VALUE", "JOB_TYPE_VALUE" },
                                          new object[3] {icdoPersonAccount.plan_id,ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.job_class_value,
                                                          ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value }, null, null);
                        if (ldtbPlanjobClass.Rows.Count > 0)
                        {
                            if (icdoPersonAccount.start_date > ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.start_date.AddDays(180))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        //UAT PIR 702
        public bool IsDCPlanParticipationValid()
        {
            if (ibusPlan == null)
                LoadPlan();
            if (ibusPlan.IsDCRetirementPlan())
            {
                if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusTransferToTFFR)
                {
                    return true;
                }
            }
            return false;
        }

        //PIR: 707 If the Member has multiple active employment, if any one of the employment is eligible for DC, this method has to return true
        public bool IsDCPlanEligible()
        {
            if (iclbAccountEmploymentDetail == null)
                LoadPersonAccountEmploymentDetailsByPersonPlan();

            foreach (var lbusPAEmpDetail in iclbAccountEmploymentDetail)
            {
                if (lbusPAEmpDetail.icdoPersonAccountEmploymentDetail.election_value == busConstant.PlanOptionStatusValueEnrolled)
                {
                    if (lbusPAEmpDetail.ibusEmploymentDetail == null)
                        lbusPAEmpDetail.LoadPersonEmploymentDetail();

                    //UAT PIR 2286 : Date Overlapping Check Removed since there is chance for Future Employment Start Date too
                    if (lbusPAEmpDetail.ibusEmploymentDetail.IsDCPlanEligible())
                    {
                        return true;
                    }
                }

            }
            return false;
        }

        // PIR 11891
        public busPersonEmploymentDetail LoadPreviousEmploymentDetail(int aintPersonEmploymentId)
        {
            busPersonEmploymentDetail lobjEmploymentDtl = new busPersonEmploymentDetail { icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail() };
            DataTable ldtbEmpDtl = busBase.Select("cdoPersonEmploymentDetail.LoadPreviousEmploymentDetail", new object [1] { aintPersonEmploymentId });

            if (ldtbEmpDtl.Rows.Count > 0)
                lobjEmploymentDtl.icdoPersonEmploymentDetail.LoadData(ldtbEmpDtl.Rows[0]);

            return lobjEmploymentDtl;
        }

        //PIR 13306
        public busPersonEmploymentDetail LoadOpenPreviousEmploymentDetail(int aintPersonEmploymentID)
        {
            busPersonEmploymentDetail lobjPersonEmploymentDtl = new busPersonEmploymentDetail { icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail() };
            DataTable ldtbEmpDetail = busBase.Select("cdoPersonEmploymentDetail.LoadOpenPreviousEmploymentDetail", new object[1] { aintPersonEmploymentID });

            if (ldtbEmpDetail.Rows.Count > 0)
                lobjPersonEmploymentDtl.icdoPersonEmploymentDetail.LoadData(ldtbEmpDetail.Rows[0]);

            return lobjPersonEmploymentDtl;
        }

        // PIR 11891 - new rules for DC eligibility date population
        public bool IsPopulateDCEligibilityDate(DateTime adteEffectiveDate)
        {
            if ((icdoPersonAccount.plan_id == busConstant.PlanIdMain || icdoPersonAccount.plan_id == busConstant.PlanIdMain2020)//PIR 20232
                && icdoPersonAccount.person_employment_dtl_id > 0)
            {
                busPersonEmploymentDetail lbusPersonEmploymentDetail = new busPersonEmploymentDetail() { icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail() };
                if (lbusPersonEmploymentDetail.FindPersonEmploymentDetail(icdoPersonAccount.person_employment_dtl_id))
                {
                    lbusPersonEmploymentDetail.LoadPersonEmployment();
                    if (lbusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id == busConstant.NDSupremeCourtOrgId
                        && lbusPersonEmploymentDetail.icdoPersonEmploymentDetail.job_class_value == busConstant.PersonJobClassNonClassifiedState)
                    {
                        return false;
                    }
                }
            }
            string lstrIsEnrolledInPriorDB = string.Empty;
            string lstrIsEnrolledInPriorDC = string.Empty;
            bool lblnIsPayeeAccountExist = false; //PIR 12482
            
            busPersonEmploymentDetail lobjPreviousEmploymentDtl = new busPersonEmploymentDetail { icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail() };
            busPersonEmploymentDetail lobjCurrentEmpPreviousEmplDtl = new busPersonEmploymentDetail { icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail() };

            busPersonEmploymentDetail lobjOpenPreviousEmploymentDtl = new busPersonEmploymentDetail { icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail() };
            
            if (ibusPerson.IsNull())
                LoadPerson();
            
            if (ibusPerson.ibusCurrentEmployment.IsNull())
                ibusPerson.LoadCurrentEmployer(adteEffectiveDate);
            if (ibusPerson.ibusCurrentEmployment.ibusOrganization.IsNull())
                ibusPerson.ibusCurrentEmployment.LoadOrganization();
            if (ibusPerson.ibusCurrentEmployment.ibusLatestEmploymentDetail.IsNull())
                ibusPerson.ibusCurrentEmployment.LoadLatestPersonEmploymentDetail();
            
            if (ibusPerson.ibusPreviousEmployment.IsNull())
                ibusPerson.LoadPreviousEmployment();
            if (ibusPerson.ibusPreviousEmployment.ibusOrganization.IsNull())
                ibusPerson.ibusPreviousEmployment.LoadOrganization();

            //PIR 12482
			if (ibusPerson.icolPersonAccount.IsNull())
                ibusPerson.LoadPersonAccount();
            if (ibusPerson.iclbPayeeAccount.IsNull())
                ibusPerson.LoadPayeeAccount();

            //PIR 12482
            foreach (busPayeeAccount lobjPayeeAccount in ibusPerson.iclbPayeeAccount)
            {
                if (lobjPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement
                    || lobjPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability)
                {
                    lblnIsPayeeAccountExist = true;
                }
            }

            lobjPreviousEmploymentDtl = LoadPreviousEmploymentDetail(ibusPerson.ibusPreviousEmployment.icdoPersonEmployment.person_employment_id);
            lobjCurrentEmpPreviousEmplDtl = LoadPreviousEmploymentDetail(ibusPerson.ibusCurrentEmployment.icdoPersonEmployment.person_employment_id);
            lobjOpenPreviousEmploymentDtl = LoadOpenPreviousEmploymentDetail(ibusPerson.ibusPreviousEmployment.icdoPersonEmployment.person_employment_id);//PIR 13306

            // If member is already having an active employment
            if ((ibusPerson.ibusPreviousEmployment.icdoPersonEmployment.end_date == DateTime.MinValue)
                && (ibusPerson.ibusPreviousEmployment.icdoPersonEmployment.person_employment_id > 0))
            {
                //PIR 13306
                if (lobjOpenPreviousEmploymentDtl.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary
                    && ibusPerson.ibusCurrentEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypePermanent)
                    return true;
                else if ((ibusPerson.ibusCurrentEmployment.ibusOrganization.icdoOrganization.emp_category_value == busConstant.EmployerCategoryState)
                    && (ibusPerson.ibusPreviousEmployment.ibusOrganization.icdoOrganization.emp_category_value != busConstant.EmployerCategoryState))
                    return true;
                else
                    return false;
            }

            DataTable ldtbDBorDC = new DataTable();
            if (ibusPerson.ibusPreviousEmployment.icdoPersonEmployment.start_date == DateTime.MinValue)
            {
                if (lobjCurrentEmpPreviousEmplDtl.icdoPersonEmploymentDetail.person_employment_dtl_id > 0)
                {
                    ldtbDBorDC = busBase.Select("cdoPersonAccountRetirement.IsEnrolledInPriorDBDC", new object[2] { ibusPerson.icdoPerson.person_id, lobjCurrentEmpPreviousEmplDtl.icdoPersonEmploymentDetail.end_date });
                    lstrIsEnrolledInPriorDB = ldtbDBorDC.Rows[0]["DB_ENROLLED"].ToString();
                    lstrIsEnrolledInPriorDC = ldtbDBorDC.Rows[0]["DC_ENROLLED"].ToString();
                }
                // Scenario 1 - according to PIR 11891 attachment
                if (lstrIsEnrolledInPriorDC == busConstant.Flag_Yes)
                    return false;

                // Scenario 7
                else if ((lobjCurrentEmpPreviousEmplDtl.icdoPersonEmploymentDetail.person_employment_dtl_id > 0)
                        && (lobjCurrentEmpPreviousEmplDtl.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary)
                        && (ibusPerson.ibusCurrentEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypePermanent)
                        && (ibusPerson.ibusCurrentEmployment.ibusOrganization.icdoOrganization.emp_category_value == busConstant.EmployerCategoryState))
                    return true;

                // Scenario 4
                else if ((lstrIsEnrolledInPriorDB == busConstant.Flag_Yes) && (lobjCurrentEmpPreviousEmplDtl.IsDCPlanEligible())
                        && (ibusPerson.ibusCurrentEmployment.ibusOrganization.icdoOrganization.emp_category_value == busConstant.EmployerCategoryState)
                        && ((ibusPerson.ibusCurrentEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.job_class_value == busConstant.PersonJobClassStateElectedOfficial)
                            || (ibusPerson.ibusCurrentEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.job_class_value == busConstant.PersonJobClassStateAppointedOfficial))
                        && ((lobjCurrentEmpPreviousEmplDtl.icdoPersonEmploymentDetail.job_class_value != busConstant.PersonJobClassStateElectedOfficial)
                            && (lobjCurrentEmpPreviousEmplDtl.icdoPersonEmploymentDetail.job_class_value != busConstant.PersonJobClassStateAppointedOfficial)))
                    return true;

                // Scenario 3 and 6
                else if ((lobjCurrentEmpPreviousEmplDtl.icdoPersonEmploymentDetail.person_employment_dtl_id > 0) 
                    && (busGlobalFunctions.DateDiffInDays(lobjCurrentEmpPreviousEmplDtl.icdoPersonEmploymentDetail.end_date, ibusPerson.ibusCurrentEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.start_date) < 31)
                    && (lstrIsEnrolledInPriorDB == busConstant.Flag_Yes)
                    && (ibusPerson.ibusCurrentEmployment.ibusOrganization.icdoOrganization.emp_category_value == busConstant.EmployerCategoryState))
                    return false;

                // Scenario 2
                else if ((lobjCurrentEmpPreviousEmplDtl.icdoPersonEmploymentDetail.person_employment_dtl_id > 0)
                        && (busGlobalFunctions.DateDiffInDays(lobjCurrentEmpPreviousEmplDtl.icdoPersonEmploymentDetail.end_date, ibusPerson.ibusCurrentEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.start_date) >= 31)
                        && (lstrIsEnrolledInPriorDB == busConstant.Flag_Yes) && (lobjCurrentEmpPreviousEmplDtl.IsDCPlanEligible())
                        && (ibusPerson.ibusCurrentEmployment.ibusOrganization.icdoOrganization.emp_category_value == busConstant.EmployerCategoryState))
                    return true;

                else if ((lobjCurrentEmpPreviousEmplDtl.icdoPersonEmploymentDetail.person_employment_dtl_id == 0)
                    && (ibusPerson.ibusCurrentEmployment.ibusOrganization.icdoOrganization.emp_category_value == busConstant.EmployerCategoryState))
                    return true;
                else
                    return false;
            }
            else
            {
                ldtbDBorDC = busBase.Select("cdoPersonAccountRetirement.IsEnrolledInPriorDBDC", new object[2] { ibusPerson.icdoPerson.person_id, ibusPerson.ibusPreviousEmployment.icdoPersonEmployment.end_date });
            }

            lstrIsEnrolledInPriorDB = ldtbDBorDC.Rows[0]["DB_ENROLLED"].ToString();
            lstrIsEnrolledInPriorDC = ldtbDBorDC.Rows[0]["DC_ENROLLED"].ToString();


            // Scenario 1 - according to PIR 11891 attachment
            if (lstrIsEnrolledInPriorDC == busConstant.Flag_Yes)
                return false;

            // New scenario - PIR 12482 
            // Exclude from DC eligibility  persons who transfers employment from an Org PeopleSoft Group of Higher Ed & there exists No current Retirement Plan 1 to an Org Group State or BND
            else if (ibusPerson.ibusPreviousEmployment.ibusOrganization.icdoOrganization.peoplesoft_org_group_value == busConstant.PeopleSoftOrgGroupValueHigherEd
                && (ibusPerson.ibusCurrentEmployment.ibusOrganization.icdoOrganization.peoplesoft_org_group_value == busConstant.PeopleSoftOrgGroupValueState
                    || ibusPerson.ibusCurrentEmployment.ibusOrganization.icdoOrganization.peoplesoft_org_group_value == busConstant.PeopleSoftOrgGroupValueBND)
                && (!ibusPerson.icolPersonAccount.Where(i => i.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled).Any())
                && (busGlobalFunctions.DateDiffInDays(ibusPerson.ibusPreviousEmployment.icdoPersonEmployment.end_date, ibusPerson.ibusCurrentEmployment.icdoPersonEmployment.start_date) < 31))
                return false;

            // New scenario - PIR 12482 
            // Exclude from DC Eligibility persons with new employment but already have an existing Payee Account (benefit type of retirement or disability)
            else if (lblnIsPayeeAccountExist)
                return false;
                
            // Scenario 7
            else if (((lobjCurrentEmpPreviousEmplDtl.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary)
                    || (lobjPreviousEmploymentDtl.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary))
                    && (ibusPerson.ibusCurrentEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypePermanent)
                    && (ibusPerson.ibusCurrentEmployment.ibusOrganization.icdoOrganization.emp_category_value == busConstant.EmployerCategoryState))
                return true;

            // Scenario 4
            else if ((lstrIsEnrolledInPriorDB == busConstant.Flag_Yes) && (lobjPreviousEmploymentDtl.IsDCPlanEligible())
                    && (ibusPerson.ibusCurrentEmployment.ibusOrganization.icdoOrganization.emp_category_value == busConstant.EmployerCategoryState)
                    && ((ibusPerson.ibusCurrentEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.job_class_value == busConstant.PersonJobClassStateElectedOfficial)
                        || (ibusPerson.ibusCurrentEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.job_class_value == busConstant.PersonJobClassStateAppointedOfficial))
                    && ((lobjPreviousEmploymentDtl.icdoPersonEmploymentDetail.job_class_value != busConstant.PersonJobClassStateElectedOfficial)
                        && (lobjPreviousEmploymentDtl.icdoPersonEmploymentDetail.job_class_value != busConstant.PersonJobClassStateAppointedOfficial)))
                return true;

            // Scenario 5
            else if ((ibusPerson.ibusCurrentEmployment.ibusOrganization.icdoOrganization.emp_category_value == busConstant.EmployerCategoryState)
                && (ibusPerson.ibusPreviousEmployment.ibusOrganization.icdoOrganization.emp_category_value != busConstant.EmployerCategoryState))
                return true;

            // Scenario 3 and 6
            else if ((busGlobalFunctions.DateDiffInDays(ibusPerson.ibusPreviousEmployment.icdoPersonEmployment.end_date, ibusPerson.ibusCurrentEmployment.icdoPersonEmployment.start_date) < 31)
                    && (lstrIsEnrolledInPriorDB == busConstant.Flag_Yes)
                    && (ibusPerson.ibusCurrentEmployment.ibusOrganization.icdoOrganization.emp_category_value == busConstant.EmployerCategoryState))
                return false;

            // Scenario 2
            else if ((busGlobalFunctions.DateDiffInDays(ibusPerson.ibusPreviousEmployment.icdoPersonEmployment.end_date, ibusPerson.ibusCurrentEmployment.icdoPersonEmployment.start_date) >= 31)
                    && (lstrIsEnrolledInPriorDB == busConstant.Flag_Yes) && (lobjPreviousEmploymentDtl.IsDCPlanEligible())
                    && (ibusPerson.ibusCurrentEmployment.ibusOrganization.icdoOrganization.emp_category_value == busConstant.EmployerCategoryState))
                return true;

            return false;
        }

        //PIR 11483
        public bool IsDcEligibilityDateRequired()
        {
            DateTime ldtDateCriteria = new DateTime(2013, 10, 1);
            if (ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date < ldtDateCriteria)
            {
                int lintGetCountOfJobClass = Convert.ToInt32(DBFunction.DBExecuteScalar("cdoPlanJobClassCrossref.GetJobClassCount", new object[1] { ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.job_class_value },
                                                        iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
                if (lintGetCountOfJobClass > 0)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        ///  a former participating DB or DC employee whose plan participation is ‘Retired’ can only waive future DB or DC plan participation
        /// becoming employed by an entity different from the employer 
        /// </summary>
        /// <returns></returns>
        public bool ValidateDBorDCEmployeeDoesNotWaiveFutureDBorDC()
        {
            //This Code is no more applicable 
            //if (ibusEmployment != null)
            //{
            //    DataTable ldtbDcAccount = Select("cdoPersonAccountRetirement.GetRetiredDBorDCPlanAccounts", new object[3] { icdoPersonAccount.person_account_id,
            //                          icdoPersonAccount.person_id, icdoPersonAccount.plan_id });
            //    if (ldtbDcAccount.Rows.Count > 0)
            //    {
            //        if (Convert.ToInt32(ldtbDcAccount.Rows[0]["org_id"]) != ibusEmployment.icdoPersonEmployment.org_id)
            //        {
            //            if (icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusRetirementWaived)
            //            {
            //                return true;
            //            }
            //        }
            //    }
            //}
            return false;
        }
        public bool CheckOptionalDBorDCAccountalreadyExists()
        {
            DataTable ldtbDcAccount = Select("cdoPersonAccountRetirement.GetWithdrawnOptionalDBorDCPlanAccounts", new object[3] { icdoPersonAccount.person_id, 
                                                                              icdoPersonAccount.plan_id, icdoPersonAccount.person_account_id});
            if (ldtbDcAccount.Rows.Count > 0)
            {
                if (ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Person Employment start date should match plan participation start date for Mandatory DB Plans if not throw error
        /// </summary>
        /// <returns>bool</returns>
        public bool ValidateStartDateForMandatoryPlan()
        {
            if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled)
            {
                if ((ibusPersonEmploymentDetail != null) && (ibusPersonEmploymentDetail.ibusPersonEmployment != null))
                {
                    if (ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypePermanent)
                    {
                        if ((icdoPersonAccount.current_plan_start_date != DateTime.MinValue) && (ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueDB || ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueHB)) //PIR 25920  New DC plan
                        {
                            DataTable ldtbPlanEmp = Select("cdoPlanEmpCategoryCrossref.GetMandatoryPlanRecord", new object[2] { icdoPersonAccount.plan_id, ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.emp_category_value });
                            if (ldtbPlanEmp.Rows.Count > 0)
                            {
                                if (Convert.ToString(ldtbPlanEmp.Rows[0]["REQUIRED_FLAG_OPTION_VALUE"]) == busConstant.RequiredFlagOptionValueMandatory)
                                {
                                    //UAT PIR 1987 : System should check history change date for RTW members
                                    if (icdoPersonAccount.current_plan_start_date != ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date)
                                    {
                                        if (!icdoPersonAccountRetirement.is_from_mss) // PIR 9709
                                            return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
        public void LoadDBDCTransfer()
        {
            DataTable ldtbList = Select<cdoPersonAccountRetirementDbDcTransferEstimate>(
                new string[1] { "person_account_id" },
                new object[1] { icdoPersonAccount.person_account_id }, null, null);
            iclbDBDCTransfer = GetCollection<busPersonAccountRetirementDbDcTransferEstimate>(ldtbList, "icdoPersonAccountRetirementDbDcTransferEstimate");
            foreach (busPersonAccountRetirementDbDcTransferEstimate lbusDBDCTransfer in iclbDBDCTransfer)
            {
                //lbusEmploymentDetail.iobj.LoadPlan();
                lbusDBDCTransfer.ibusPersonAccount = this;
                lbusDBDCTransfer.idecProjectedAnnualSalary = lbusDBDCTransfer.icdoPersonAccountRetirementDbDcTransferEstimate.proj_monthly_salary_amount * 12;
                LoadPerson();
            }
        }

        public void InsertHistory()
        {
            cdoPersonAccountRetirementHistory lobjCdoRetirementHistory = new cdoPersonAccountRetirementHistory();
            lobjCdoRetirementHistory.person_account_id = icdoPersonAccount.person_account_id;
            // PIR 9115
            //PIR 17081
            //if ((icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetimentSuspended) ||
            //    (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirmentCancelled))
            //{
            //    DBFunction.DBNonQuery("cdoPersonAccountRetirementHistory.UpdateReportGeneratedFlag", new object[1] { ibusHistory.icdoPersonAccountRetirementHistory.person_account_retirement_history_id },
            //                        iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            //}
            lobjCdoRetirementHistory.plan_participation_status_value = icdoPersonAccount.plan_participation_status_value;
            lobjCdoRetirementHistory.start_date = icdoPersonAccount.history_change_date;
            lobjCdoRetirementHistory.status_value = icdoPersonAccount.status_value;
            if (icdoPersonAccount.suppress_warnings_flag == busConstant.Flag_Yes) // PROD PIR 7079
                lobjCdoRetirementHistory.suppress_warnings_by = icdoPersonAccount.suppress_warnings_by;
            lobjCdoRetirementHistory.suppress_warnings_date = icdoPersonAccount.suppress_warnings_date;
            lobjCdoRetirementHistory.suppress_warnings_flag = icdoPersonAccount.suppress_warnings_flag;
            lobjCdoRetirementHistory.from_person_account_id = icdoPersonAccount.from_person_account_id;
            lobjCdoRetirementHistory.to_person_account_id = icdoPersonAccount.to_person_account_id;
            lobjCdoRetirementHistory.dc_eligibility_date = icdoPersonAccountRetirement.dc_eligibility_date;
            lobjCdoRetirementHistory.mutual_fund_window_flag = icdoPersonAccountRetirement.mutual_fund_window_flag;
            lobjCdoRetirementHistory.capital_gain = icdoPersonAccountRetirement.capital_gain;
            lobjCdoRetirementHistory.rhic_benfit_amount = icdoPersonAccountRetirement.rhic_benfit_amount;
            lobjCdoRetirementHistory.vesting_letter_sent_flag = icdoPersonAccountRetirement.vesting_letter_sent_flag;
            lobjCdoRetirementHistory.provider_org_id = icdoPersonAccount.provider_org_id;
            if (ibusPersonEmploymentDetail.IsNull())
            {
                if (icdoPersonAccount.person_employment_dtl_id == 0) icdoPersonAccount.person_employment_dtl_id = GetEmploymentDetailID();
                LoadPersonEmploymentDetail();
            }
            //PIR 25920 when overlapping and the blank (NULL) value is used; don’t update the value in Person Account
            if (istrAllowOverlapHistory == busConstant.Flag_Yes && iobjPassInfo.istrFormName == "wfmPensionPlanMaintenance" && iintAddlEEContributionPercent == 0)
            {
                
            }
            else
            {
                if (ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary)
                    lobjCdoRetirementHistory.addl_ee_contribution_percent = icdoPersonAccount.addl_ee_contribution_percent_temp;     //PIR 25920 New Plan DC 2025
                else
                    lobjCdoRetirementHistory.addl_ee_contribution_percent = icdoPersonAccount.addl_ee_contribution_percent;     //PIR 25920 New Plan DC 2025
            }
            lobjCdoRetirementHistory.Insert();
        }

        public void InsertHistoryForDC25()
        {
            cdoPersonAccountRetirementHistory lobjCdoRetirementHistory = new cdoPersonAccountRetirementHistory();
            lobjCdoRetirementHistory.person_account_id = icdoPersonAccount.person_account_id;
            
            lobjCdoRetirementHistory.plan_participation_status_value = icdoPersonAccount.plan_participation_status_value;
            lobjCdoRetirementHistory.start_date = icdoPersonAccount.history_change_date;
            lobjCdoRetirementHistory.status_value = icdoPersonAccount.status_value;
            if (icdoPersonAccount.suppress_warnings_flag == busConstant.Flag_Yes) // PROD PIR 7079
                lobjCdoRetirementHistory.suppress_warnings_by = icdoPersonAccount.suppress_warnings_by;
            lobjCdoRetirementHistory.suppress_warnings_date = icdoPersonAccount.suppress_warnings_date;
            lobjCdoRetirementHistory.suppress_warnings_flag = icdoPersonAccount.suppress_warnings_flag;
            lobjCdoRetirementHistory.from_person_account_id = icdoPersonAccount.from_person_account_id;
            lobjCdoRetirementHistory.to_person_account_id = icdoPersonAccount.to_person_account_id;
            lobjCdoRetirementHistory.dc_eligibility_date = icdoPersonAccountRetirement.dc_eligibility_date;
            lobjCdoRetirementHistory.mutual_fund_window_flag = icdoPersonAccountRetirement.mutual_fund_window_flag;
            lobjCdoRetirementHistory.capital_gain = icdoPersonAccountRetirement.capital_gain;
            lobjCdoRetirementHistory.rhic_benfit_amount = icdoPersonAccountRetirement.rhic_benfit_amount;
            lobjCdoRetirementHistory.vesting_letter_sent_flag = icdoPersonAccountRetirement.vesting_letter_sent_flag;
            lobjCdoRetirementHistory.provider_org_id = icdoPersonAccount.provider_org_id;
            lobjCdoRetirementHistory.addl_ee_contribution_percent = 0;
            
            lobjCdoRetirementHistory.Insert();

            
                icdoPersonAccount.history_change_date = busGlobalFunctions.GetFirstDayofNextMonth(icdoPersonAccount.history_change_date);

                LoadPersonAccountRetirementHistory();
                if (icdoPersonAccount.person_employment_dtl_id > 0)
                {
                    SetPersonAccountIDInPersonAccountEmploymentDetail();
                }
                LoadAllPersonEmploymentDetails();
                LoadPreviousHistory();

                //Create New DC Account When We Set the Status to "Transferred To DC" //PIR 25920  New DC plan
                if ((ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueDB || ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueHB) &&
                    (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusTransferDC) &&
                    (ValidateWhenTransferDC() == string.Empty))
                {
                    CreateDCAccountFromDB();
                }

                // UAT PIR ID 1077 - To refresh the screen values only if the Suppress flag is On.
                if (icdoPersonAccount.suppress_warnings_flag == busConstant.Flag_Yes)
                    RefreshValues();
                LoadPersonAccountRetirementHistory();
                //PIR 25920 DC 2025 changes update the person account and person account RETE history in case additional contribution is -1
                if (iintAddlEEContributionPercent == -1)
                {
                    LoadPreviousHistory();
                    DBFunction.DBNonQuery("cdoPersonAccountRetirementHistory.UpdateAdditionalContributionToZero", new object[2] { ibusHistory.icdoPersonAccountRetirementHistory.person_account_retirement_history_id, 0 },
                                        iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                    LoadPersonAccountRetirementHistory();
                }
                //UAT PIR 2220
                if (icdoPersonAccount.status_value == busConstant.PersonAccountStatusValid && IsHistoryEntryRequired)
                    PostESSMessage();

                // Initial start date was not dispayed on screen after initial save.
                idtInitialStartDate = icdoPersonAccount.start_date;

                istrIsInitialStartDateChanged = false;
                UpdateNewHireMemberRecordReQuestDetailsManOrAutoPlanEnrollment(busConstant.RetirementCategory);

                InsertIntoEnrollmentData();

        }

        public void ProcessHistory()
        {
            Collection<busPersonAccountRetirementHistory> lclbDeletedRetrHistory = new Collection<busPersonAccountRetirementHistory>();
            if ((icdoPersonAccount.status_value == "VALD") && (IsHistoryEntryRequired))
            {
                if (iblnIsEEPercentSelectedForFirstEnrollment)
                {
                    InsertHistoryForDC25();
                }

                //Removing Overlapping history
                if (iclbOverlappingHistory != null && iclbOverlappingHistory.Count > 0)
                {
                    foreach( busPersonAccountRetirementHistory lobjHistory in iclbOverlappingHistory)
                    {
                        lclbDeletedRetrHistory.Add(lobjHistory);
                        lobjHistory.Delete();
                    }
                    //PIR 23340
                    if (IsResourceForEnhancedOverlap(busConstant.OverlapResourceEnhancedRetirement))
                    {
                        bool lblnIsPersonAccountModified = false;
                        if (icdoPersonAccount.start_date > icdoPersonAccount.history_change_date)
                        {
                            icdoPersonAccount.start_date = icdoPersonAccount.history_change_date;
                            lblnIsPersonAccountModified = true;

                        }
                        if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled &&
                            icdoPersonAccount.end_date != DateTime.MinValue)
                        {
                            icdoPersonAccount.end_date = DateTime.MinValue;
                            lblnIsPersonAccountModified = true;
                        }

                        if (lblnIsPersonAccountModified)
                            icdoPersonAccount.Update();
                    }
                }

                //if (_ibusHistory == null)
                    LoadPreviousHistory();

                if (//(ibusHistory.icdoPersonAccountRetirementHistory.end_date == DateTime.MinValue) &&
                    (ibusHistory.icdoPersonAccountRetirementHistory.person_account_retirement_history_id > 0))
                {
                    if (!lclbDeletedRetrHistory.Any(i => i.icdoPersonAccountRetirementHistory.person_account_retirement_history_id == ibusHistory.icdoPersonAccountRetirementHistory.person_account_retirement_history_id))
                    {
                        if (ibusHistory.icdoPersonAccountRetirementHistory.start_date == icdoPersonAccount.history_change_date)
                        {
                            ibusHistory.icdoPersonAccountRetirementHistory.end_date = icdoPersonAccount.history_change_date;
                            // Set flag to 'Y' so that ESS Benefit Enrollment report will ignore these records
                            //ibusHistory.icdoPersonAccountRetirementHistory.is_enrollment_report_generated = busConstant.Flag_Yes;//PIR 17081
                        }
                        else
                        {
                            ibusHistory.icdoPersonAccountRetirementHistory.end_date = icdoPersonAccount.history_change_date.AddDays(-1);
                        }
                        ibusHistory.icdoPersonAccountRetirementHistory.Update();

                        //PIR 1729 : Update the End Date with Previous History End Date if the Plan Participation Status Retired or WithDrawn
                        //Otherwise Reset the End Date with Blank Value
                        if ((icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementRetired) ||
                            (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementWithDrawn))
                        {
                            icdoPersonAccount.end_date = ibusHistory.icdoPersonAccountRetirementHistory.end_date;
                        }
                        else
                        {
                            icdoPersonAccount.end_date = DateTime.MinValue;
                        }
                        icdoPersonAccount.Update();
                    }
                }

                //Always Insert New History Whenever HistoryEntry Required Flag is Set
                InsertHistory();
            }
        }

        public bool CalculateStatusDateEndDateGreaterThanStartDate()
        {
            // if the Transferred TFFR participation status date and DB plan End Date are greater than 90 days from the Employment Start Date
            DateTime ldtHistoryChangeDate;
            DateTime ldtEndDate;
            DateTime ldtStartDate;

            if ((icdoPersonAccount.history_change_date != null) && (ibusHistory.icdoPersonAccountRetirementHistory.end_date != null) && (ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date != null))
            {
                ldtHistoryChangeDate = icdoPersonAccount.history_change_date;
                ldtEndDate = ibusHistory.icdoPersonAccountRetirementHistory.end_date;
                ldtStartDate = ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date;

                ldtStartDate = ldtStartDate.AddDays(90);

                if ((ldtHistoryChangeDate > ldtStartDate) && (ldtEndDate > ldtStartDate))
                    return true;
                else
                    return false;
            }

            return false;
        }

        //UAT PIR 1745 - Leave the Initial Plan Start Date Editable till the first history created.
        public bool IsHistoryCreated()
        {
            if (icolPersonAccountRetirementHistory == null)
                return false;

            if (icolPersonAccountRetirementHistory.Count == 0)
                return false;

            return true;
        }

        /// <summary>
        /// this validation will throw error when plan participation status TransferToDC selected and already record exist for DC
        /// And if the change effective date is greater than DC Plan Eligibility date, system will throw an error
        /// Constant Used for Validation in busXML. 1) DCEX-DC Person Account Exists 2) DCDT-change effective date is greater than DC Plan Eligibility date 3)NTEL-Not Eligible
        /// </summary>
        public string ValidateWhenTransferDC()
        {
            string lstrErrorIdentifier = string.Empty;
            if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusTransferDC)
            {
                DataTable ldtbList = new DataTable();
                ldtbList = Select<cdoPersonAccount>(
                                           new string[2] { "person_id", "plan_id" },
                                           new object[2] { icdoPersonAccount.person_id, busConstant.PlanIdDC }, null, null);
                if (ldtbList != null && ldtbList.Rows.Count == 0)
                {
                    ldtbList = Select<cdoPersonAccount>(
                                           new string[2] { "person_id", "plan_id" },
                                           new object[2] { icdoPersonAccount.person_id, busConstant.PlanIdDC2020 }, null, null); //PIR 20232
                }
                //PIR 25920
                if (ldtbList != null && ldtbList.Rows.Count == 0)
                {
                    ldtbList = Select<cdoPersonAccount>(
                                           new string[2] { "person_id", "plan_id" },
                                           new object[2] { icdoPersonAccount.person_id, busConstant.PlanIdDC2025 }, null, null); //PIR 25920
                }
                if (ldtbList.Rows.Count > 0)
                {
                    lstrErrorIdentifier = "DCEX";
                }
                else if (icdoPersonAccountRetirement.dc_eligibility_date == DateTime.MinValue)
                {
                    lstrErrorIdentifier = "NTEL";
                }
                else if ((icdoPersonAccountRetirement.dc_eligibility_date != DateTime.MinValue) &&
                    (icdoPersonAccount.history_change_date > icdoPersonAccountRetirement.dc_eligibility_date))
                {
                    lstrErrorIdentifier = "DCDT";
                }
            }
            return lstrErrorIdentifier;
        }
        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
			//PIR 25920 Droplist Refresh from DatePicker
            if (iobjPassInfo.istrFormName == "wfmPensionPlanMaintenance" && icdoPersonAccount.plan_id == busConstant.PlanIdDC2025)
            {
                GetADECAmountValuesByEffectiveDate(icdoPersonAccount.history_change_date,ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id,
                    icdoPersonAccount.person_account_id);
            }
            LoadPersonAccountRetirementHistory();
            iclbOverlappingHistory = new Collection<busPersonAccountRetirementHistory>();
            if (istrAllowOverlapHistory == busConstant.Flag_Yes && icdoPersonAccount.history_change_date != DateTime.MinValue)
            {
                Collection<busPersonAccountRetirementHistory> lclbOpenHistory = LoadOverlappingHistory();
                if (lclbOpenHistory.Count > 0)
                {
                    foreach(busPersonAccountRetirementHistory lobjHistory in lclbOpenHistory)
                    {
                        icolPersonAccountRetirementHistory.Remove(lobjHistory);
                        iclbOverlappingHistory.Add(lobjHistory);
                    }
                }
            }

            if (ibusHistory == null)
                LoadPreviousHistory();

            // PIR 2165
            LoadPlanEffectiveDate();

            //Reload the Employement Object only where person account id exists. (In New mode, we get the employment detail id from paramter)
            if (icdoPersonAccount.person_account_id > 0)
            {
                if (ibusPersonEmploymentDetail.IsNull())
                {
                    icdoPersonAccount.person_employment_dtl_id = GetEmploymentDetailID();
                    LoadPersonEmploymentDetail();
                    ibusPersonEmploymentDetail.LoadPersonEmployment();
                    ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
                }
            }
            LoadOrgPlan(idtPlanEffectiveDate);

            if (IsInitialStartDateGreaterThanHistory())
                istrIsInitialStartDateChanged = true;

            SetHistoryEntryRequiredOrNot();
            base.BeforeValidate(aenmPageMode);
        }
		//PIR 14646
        private bool IsPersonAccountsInEnrSusRetiredStatus()
        {
            return ibusPerson.iclbRetirementAccount.Count(i => (i.icdoPersonAccount.plan_id == busConstant.PlanIdMain ||
                                                i.icdoPersonAccount.plan_id == busConstant.PlanIdMain2020 ||//PIR 20232 
                                                i.icdoPersonAccount.plan_id == busConstant.PlanIdLE ||
                                                i.icdoPersonAccount.plan_id == busConstant.PlanIdLEWithoutPS ||
                                                i.icdoPersonAccount.plan_id == busConstant.PlanIdNG ||
                                                i.icdoPersonAccount.plan_id == busConstant.PlanIdBCILawEnf ||
                                                i.icdoPersonAccount.plan_id == busConstant.PlanIdStatePublicSafety || //PIR 25729
                                                i.icdoPersonAccount.plan_id == busConstant.PlanIdJudges)
                                                && (i.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled ||
                                                i.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetimentSuspended ||
                                                i.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementRetired)) > 0;

        }
        //private bool IsPersonAccountsInWithDrawnTransferOrCancelStatus()
        //{
        //    return ibusPerson.iclbRetirementAccount.Count(i => (i.icdoPersonAccount.plan_id == busConstant.PlanIdMain ||
        //                                        i.icdoPersonAccount.plan_id == busConstant.PlanIdLE ||
        //                                        i.icdoPersonAccount.plan_id == busConstant.PlanIdLEWithoutPS ||
        //                                        i.icdoPersonAccount.plan_id == busConstant.PlanIdNG ||
        //                                        i.icdoPersonAccount.plan_id == busConstant.PlanIdBCILawEnf ||
        //                                        i.icdoPersonAccount.plan_id == busConstant.PlanIdJudges)
        //                                        && (i.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementWithDrawn ||
        //                                        i.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusTransferDC ||
        //                                        i.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusTransferToTFFR ||
        //                                        i.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementTransferredTIAACREF ||
        //                                        i.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirmentCancelled)) > 0;

        //}
		//PIR 14646
        private void SetBenefitTierValue(int aintPlanID)
        {
            if (ibusPerson.IsNull())
                LoadPerson();
            if (ibusPerson.iclbRetirementAccount.IsNull())
                ibusPerson.LoadRetirementAccount();
            if(icdoPersonAccount.person_employment_dtl_id == 0)
                icdoPersonAccount.person_employment_dtl_id = GetEmploymentDetailID();
            if (ibusPersonEmploymentDetail.IsNull())
                LoadPersonEmploymentDetail();

            //PIR 26544
            DataTable ldtbResult = Select("entPersonAccountRetirement.LoadBenefitTierByPriorEnrolledHistoryDate", new object[3] { icdoPersonAccount.person_id, aintPlanID, icdoPersonAccount.plan_id == busConstant.PlanIdBCILawEnf ? icdoPersonAccount.current_plan_start_date : ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date });
            if (ldtbResult.Rows.Count > 0)
            {
                icdoPersonAccountRetirement.benefit_tier_value = ldtbResult.Rows[0]["CODE_VALUE"].ToString();
            }
        }                
        public override void BeforePersistChanges()
        {
            //icdoPersonAccount.people_soft_file_sent_flag = busConstant.Flag_No;//PIR 17081
            //PIR 14646
            if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled)
            {
                SetBenefitTierValue(icdoPersonAccount.plan_id);
            }
            if (icdoPersonAccount.plan_id == busConstant.PlanIdDC ||
                icdoPersonAccount.plan_id == busConstant.PlanIdDC2020 ||
                icdoPersonAccount.plan_id == busConstant.PlanIdDC2025) //PIR 20232 //PIR 25920 New Plan DC 2025
            {
                icdoPersonAccountRetirement.dc_file_sent_flag = busConstant.Flag_No;
                if (ibusProviderOrgPlan == null)
                {
                    LoadProviderOrgPlan();
                }
                icdoPersonAccount.provider_org_id = ibusProviderOrgPlan.icdoOrgPlan.org_id;
            }
            //PIR 25920 New Plan DC 2025 catch the exact value before insert we need to insert 0 in DB and not default value null 
            if (ibusPersonEmploymentDetail.IsNull())
                LoadPersonEmploymentDetail();
            //if (ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary)
            //    iintAddlEEContributionPercent = icdoPersonAccount.addl_ee_contribution_percent_temp;
            //else
            //    iintAddlEEContributionPercent = icdoPersonAccount.addl_ee_contribution_percent;
            //PIR 25920 when overlapping and the blank (NULL) value is used; don’t update the value in Person Account
            if (istrAllowOverlapHistory == busConstant.Flag_Yes && iobjPassInfo.istrFormName == "wfmPensionPlanMaintenance" && iintAddlEEContributionPercent == 0)
            {
                //if (ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary)
                //    icdoPersonAccount.addl_ee_contribution_percent_temp = iintAddlEEContributionPercent;
                //else
                //    icdoPersonAccount.addl_ee_contribution_percent = iintAddlEEContributionPercent;
            }
            else
            {
                if (ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary)
                    icdoPersonAccount.addl_ee_contribution_percent_temp = iintAddlEEContributionPercent;
                else
                    icdoPersonAccount.addl_ee_contribution_percent = iintAddlEEContributionPercent;
            }
            if (icdoPersonAccount.suppress_warnings_flag == "Y")
            {
                icdoPersonAccount.suppress_warnings_by = iobjPassInfo.istrUserID;
            }
            else
            {
                icdoPersonAccount.suppress_warnings_by = string.Empty;
            }

            //PIR 25920 - Check if EE percent for DC25 is changed or enrolling in the plan for the first time.
            int lintOldAddlEEContributionPercent = 0;
            if(icdoPersonAccount.ihstOldValues.Count > 0)
            { 
                if (ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary)
                    lintOldAddlEEContributionPercent = Convert.ToInt32(icdoPersonAccount.ihstOldValues["addl_ee_contribution_percent_temp"]);
                else
                    lintOldAddlEEContributionPercent = Convert.ToInt32(icdoPersonAccount.ihstOldValues["addl_ee_contribution_percent"]);
            }

            if (icdoPersonAccount.plan_id == busConstant.PlanIdDC2025 && ((icdoPersonAccount.ihstOldValues.Count > 0 && lintOldAddlEEContributionPercent != iintAddlEEContributionPercent) ||
                icdoPersonAccount.person_account_id == 0))
            {
                iblnIsEEPercentChanged = true;
            }

            if (!iblnIsFromMSSForEnrollmentData && icdoPersonAccount.plan_id == busConstant.PlanIdDC2025 && icdoPersonAccount.person_account_id == 0 && iintAddlEEContributionPercent > 0
                && ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value != busConstant.PersonJobTypeTemporary)
                iblnIsEEPercentSelectedForFirstEnrollment = true;

            if (icdoPersonAccount.ienuObjectState == ObjectState.Insert)
            {
                //UAT PIR 2373 people soft file changes
                //--Start--//
                icdoPersonAccount.ps_file_change_event_value = busConstant.NewEnrollment;
                //--End--//
                //PIR 25920 DC 25 Enrollment changes
                if (ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary &&
                        icdoPersonAccount.addl_ee_contribution_percent_temp_end_date == DateTime.MinValue)
                    icdoPersonAccount.addl_ee_contribution_percent_temp_end_date = ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date.AddDays(180);
                else if(ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypePermanent &&
                        icdoPersonAccount.addl_ee_contribution_percent_end_date == DateTime.MinValue)
                    icdoPersonAccount.addl_ee_contribution_percent_end_date = ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date.AddDays(30);
                icdoPersonAccount.history_change_date = icdoPersonAccount.start_date;
                icdoPersonAccount.Insert();
                icdoPersonAccountRetirement.vesting_letter_sent_flag = busConstant.Flag_No;
                icdoPersonAccountRetirement.person_account_id = icdoPersonAccount.person_account_id;

                //PIR 1678 : Update the Beneficiary Required Flag to YES for new enrollments
                if (ibusPerson == null)
                    LoadPerson();
                ibusPerson.icdoPerson.beneficiary_required_flag = busConstant.Flag_Yes;
                ibusPerson.icdoPerson.Update();
            }
            else
            {
                SetHistoryEntryRequiredOrNot(); //PIR 20017

                //PIR 24933
                UpdateActionStatusifDefeApplicationExists();

                //UAT PIR 2373 people soft file changes
                //--Start--//
                if (IsHistoryEntryRequired && (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirmentCancelled ||
                    icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetimentSuspended))
                {
                    SetPersonAcccountForTeminationChange();
                }
                else
                {
                    SetPersonAccountForEnrollmentChange();
                }
                //--End--//
                
                //prod pir 5546
                //--start--//
                if (icdoPersonAccount.history_change_date == DateTime.MinValue && !IsHistoryCreated())
                    icdoPersonAccount.history_change_date = icdoPersonAccount.start_date;
                //--end--//

                //PIR 25920 DC 2025 Changes allow member to make election again if plan cancelled with initial date.
                if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirmentCancelled)
                {
                    DateTime ldtAddlEEContributionPercentEndDate = ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary ? icdoPersonAccount.addl_ee_contribution_percent_temp_end_date :
                        icdoPersonAccount.addl_ee_contribution_percent_end_date;
                    int lintAllowDays = ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary ? 180 : 30;
                    if (icdoPersonAccount.history_change_date.AddDays(lintAllowDays) == ldtAddlEEContributionPercentEndDate)
                    {
                        icdoPersonAccount.addl_ee_contribution_percent_end_date = DateTime.MinValue;
                        icdoPersonAccount.addl_ee_contribution_percent_temp_end_date = DateTime.MinValue;
                    }
                }

                if (idtInitialStartDate != DateTime.MinValue)
                {
                    if (icolPersonAccountRetirementHistory == null)
                        LoadPersonAccountRetirementHistory();

                    String lstrOrderbyClause = "icdoPersonAccountRetirementHistory.start_date asc,icdoPersonAccountRetirementHistory.person_account_retirement_history_id asc";
                    icolPersonAccountRetirementHistory = busGlobalFunctions.Sort<busPersonAccountRetirementHistory>(lstrOrderbyClause, icolPersonAccountRetirementHistory);

                    Collection<busPersonAccountRetirementHistory> lclbRetirementHistory = icolPersonAccountRetirementHistory.Where(i => i.icdoPersonAccountRetirementHistory.start_date == icdoPersonAccount.start_date).ToList().ToCollection();

                    foreach (busPersonAccountRetirementHistory lobjRetirementHistory in lclbRetirementHistory)
                    {
                        DateTime ldtEndDate = DateTime.MinValue;
                        ldtEndDate = lobjRetirementHistory.icdoPersonAccountRetirementHistory.end_date;

                        if (ldtEndDate == DateTime.MinValue)
                            ldtEndDate = DateTime.MaxValue;

                        if ((idtInitialStartDate < lobjRetirementHistory.icdoPersonAccountRetirementHistory.start_date &&
                            idtInitialStartDate < ldtEndDate) ||
                            busGlobalFunctions.CheckDateOverlapping(idtInitialStartDate, lobjRetirementHistory.icdoPersonAccountRetirementHistory.start_date, ldtEndDate) ||
                            (lobjRetirementHistory.icdoPersonAccountRetirementHistory.start_date == ldtEndDate))
                        {
                                if (icdoPersonAccount.history_change_date == DateTime.MinValue)
                                    LoadPersonAccount();//Loading PersonAccount details because historychangedate is made null in FindPersonAccountRetirement method.

                                if (icdoPersonAccount.start_date == lobjRetirementHistory.icdoPersonAccountRetirementHistory.start_date)
                                {
                                    icdoPersonAccount.start_date = idtInitialStartDate;
                                    icdoPersonAccount.Update();
                                }

                                if (icdoPersonAccount.history_change_date == lobjRetirementHistory.icdoPersonAccountRetirementHistory.start_date)
                                {
                                    icdoPersonAccount.history_change_date = idtInitialStartDate;
                                    icdoPersonAccount.Update();
                                }

                                if (lobjRetirementHistory.icdoPersonAccountRetirementHistory.end_date != DateTime.MinValue)
                                {
                                    if (lobjRetirementHistory.icdoPersonAccountRetirementHistory.start_date == lobjRetirementHistory.icdoPersonAccountRetirementHistory.end_date)
                                    {
                                        lobjRetirementHistory.icdoPersonAccountRetirementHistory.end_date = idtInitialStartDate;
                                    }
                                }

                                lobjRetirementHistory.icdoPersonAccountRetirementHistory.start_date = idtInitialStartDate;
                                lobjRetirementHistory.icdoPersonAccountRetirementHistory.Update();

                                lobjRetirementHistory.icdoPersonAccountRetirementHistory.Select();
                        }
                    }
                    LoadPersonAccountRetirementHistory();//To reset the order of history records
                }

            }
            CalculateERVestedPercentage();
            base.BeforePersistChanges();
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
                if (ibusHistory.icdoPersonAccountRetirementHistory.plan_participation_status_value == busConstant.PlanParticipationStatusRetimentSuspended &&
                    icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled)
                {
                    //icdoPersonAccount.people_soft_file_sent_flag = busConstant.Flag_No;//PIR 17081
                    icdoPersonAccount.ps_file_change_event_value = busConstant.NewEnrollment;
                }
            }
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            //PIR 25920 New Plan DC 2025 catch the exact value before insert we need to insert 0 in DB and not default value null 
            //if(iintAddlEEContributionPercent == -1)
            //    icdoPersonAccount.addl_ee_contribution_percent = iintAddlEEContributionPercent;          

            ProcessHistory();
            LoadPersonAccountRetirementHistory();
            if (icdoPersonAccount.person_employment_dtl_id > 0)
            {
                SetPersonAccountIDInPersonAccountEmploymentDetail();
            }
            LoadAllPersonEmploymentDetails();
            LoadPreviousHistory();

            //Create New DC Account When We Set the Status to "Transferred To DC" //PIR 25920  New DC plan
            if ((ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueDB || ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueHB) &&
                (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusTransferDC) &&
                (ValidateWhenTransferDC() == string.Empty))
            {
                CreateDCAccountFromDB();
            }
            
            // UAT PIR ID 1077 - To refresh the screen values only if the Suppress flag is On.
            if (icdoPersonAccount.suppress_warnings_flag == busConstant.Flag_Yes)
                RefreshValues();
            LoadPersonAccountRetirementHistory();
            //PIR 25920 DC 2025 changes update the person account and person account RETE history in case additional contribution is -1
            if(iintAddlEEContributionPercent == -1)
            {
                LoadPreviousHistory();
                DBFunction.DBNonQuery("cdoPersonAccountRetirementHistory.UpdateAdditionalContributionToZero", new object[2] { ibusHistory.icdoPersonAccountRetirementHistory.person_account_retirement_history_id , 0 },
                                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                LoadPersonAccountRetirementHistory();
            }
            //UAT PIR 2220
            if (icdoPersonAccount.status_value == busConstant.PersonAccountStatusValid && IsHistoryEntryRequired)
                PostESSMessage();

            // Initial start date was not dispayed on screen after initial save.
            idtInitialStartDate = icdoPersonAccount.start_date;

            istrIsInitialStartDateChanged = false;
            UpdateNewHireMemberRecordReQuestDetailsManOrAutoPlanEnrollment(busConstant.RetirementCategory);
            if ((icdoPersonAccount.status_value == "VALD") && (IsHistoryEntryRequired) && !iblnIsFromMSSForEnrollmentData
                && icdoPersonAccount.iblnIsEmploymentEnded) //PIR 20259
                icdoPersonAccount.iblnIsEmploymentEnded = false;

                //PIR 17081 - Insert record into SGT_ENROLLMENT_DATA for PeopleSoft and Benefit Enrollment. 
                if ((icdoPersonAccount.status_value == "VALD") && (IsHistoryEntryRequired) && !iblnIsFromMSSForEnrollmentData 
                && !icdoPersonAccount.iblnIsEmploymentEnded) //PIR 20259
            {
                InsertIntoEnrollmentData();
            }
            LoadADECAmountZero();
            EvaluateInitialLoadRules();
        }

        public void InsertIntoEnrollmentData()
        {
            if (ibusPerson.IsNull())
                LoadPerson();

            if (ibusPersonEmploymentDetail.IsNull())
                LoadPersonEmploymentDetail();

            if (ibusPersonEmploymentDetail.ibusPersonEmployment.IsNull())
                ibusPersonEmploymentDetail.LoadPersonEmployment();

            //if (ibusHistory == null)
                LoadPreviousHistory();

            if (ibusProvider == null)
                LoadProvider();

            busEnrollmentData lobjEnrollmentData = new busEnrollmentData();
            lobjEnrollmentData.icdoEnrollmentData = new doEnrollmentData();

            busDailyPersonAccountPeopleSoft lobjDailyPAPeopleSoft = new busDailyPersonAccountPeopleSoft();
            lobjDailyPAPeopleSoft.ibusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
            lobjDailyPAPeopleSoft.ibusProvider = new busOrganization { icdoOrganization = new cdoOrganization() };

            if (lobjDailyPAPeopleSoft.iclbDailyPersonAccountPeopleSoft == null)
                lobjDailyPAPeopleSoft.iclbDailyPersonAccountPeopleSoft = new Collection<busDailyPersonAccountPeopleSoft>();

            lobjDailyPAPeopleSoft.ibusPersonAccount.icdoPersonAccount = icdoPersonAccount;
            lobjDailyPAPeopleSoft.ibusProvider = ibusProvider;

            lobjDailyPAPeopleSoft.iblnIsRetirementPlan = true;
            lobjDailyPAPeopleSoft.LoadPersonEmploymentForPeopleSoft();
            //PeopleSoft record should not be created if the org group is not PeopleSoft.
            lobjDailyPAPeopleSoft.LoadPeopleSoftOrgGroupValues();

            int lintCounter = 0;

			//PIR 20135 - Issue 1
            DBFunction.DBNonQuery("cdoPersonAccount.UpdatePrevPeopleSoftRecords", new object[5] { icdoPersonAccount.person_id, icdoPersonAccount.plan_id ,
                        ibusHistory.icdoPersonAccountRetirementHistory.start_date, lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id,icdoPersonAccount.person_account_id },
                        iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            //PIR 26238
            if (iclbOverlappingHistory.IsNotNull() && iclbOverlappingHistory.Count() > 0)
            {
                foreach (busPersonAccountRetirementHistory lobjDeletedHistory in iclbOverlappingHistory)
                {
                    DBFunction.DBNonQuery("cdoPersonAccount.UpdatePrevPeopleSoftNBenefitEnrlFlag", new object[2] { lobjDeletedHistory.icdoPersonAccountRetirementHistory.person_account_retirement_history_id, icdoPersonAccount.person_account_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                }
            }

            int lintAddlEEContriPercent = 0;
            if (icdoPersonAccount.plan_id == busConstant.PlanIdDC2025)
                lintAddlEEContriPercent = ibusHistory.icdoPersonAccountRetirementHistory.addl_ee_contribution_percent;

            //PeopleSoft logic will only be executed if the ORG GROUP of the current Organization is PeopleSoft. 
            if ((lobjDailyPAPeopleSoft.iclbPeopleSoftOrgGroupValue.Where(i => i.code_value == lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.peoplesoft_org_group_value && i.data2 == busConstant.Flag_Yes).Count() > 0))
            {
                if (icdoPersonAccount.plan_id == busConstant.PlanIdMain || icdoPersonAccount.plan_id == busConstant.PlanIdMain2020)//PIR 20232
                {
                    if (ibusPerson.icdoPerson.db_addl_contrib == busConstant.Flag_Yes) //DC-DB	RPERS
                    {
                        lobjDailyPAPeopleSoft.iblnBenefitFromPS = true;
                        lobjDailyPAPeopleSoft.GeneratePeoplesoftEntryForRetirementPlans(null, null, lintAddlEEContriPercent);
                    }
                    else if ((lobjDailyPAPeopleSoft.istrMemberType == busConstant.MainDPI) || (lobjDailyPAPeopleSoft.istrMemberType == busConstant.MainCareerTech))
                    {
                        lobjDailyPAPeopleSoft.iblnBenefitFromPS = true;
                        lobjDailyPAPeopleSoft.GeneratePeoplesoftEntryForRetirementPlans(null, lobjDailyPAPeopleSoft.istrMemberType, lintAddlEEContriPercent);
                    }
                    else
                        lobjDailyPAPeopleSoft.GeneratePeoplesoftEntryForRetirementPlans(lobjDailyPAPeopleSoft.istrEmpTypeValue, null, lintAddlEEContriPercent);
                }
                else if (icdoPersonAccount.plan_id == busConstant.PlanIdDC2025)//PIR 25920 - PS changes
                    lobjDailyPAPeopleSoft.GeneratePeoplesoftEntryForRetirementPlans(lobjDailyPAPeopleSoft.istrEmpTypeValue, null, lintAddlEEContriPercent);
                else
                    lobjDailyPAPeopleSoft.GeneratePeoplesoftEntryForRetirementPlans(null, null, lintAddlEEContriPercent);

                if (lobjDailyPAPeopleSoft.iclbDailyPersonAccountPeopleSoft.IsNotNull() && lobjDailyPAPeopleSoft.iclbDailyPersonAccountPeopleSoft.Count > 0)
                {
                    foreach (busDailyPersonAccountPeopleSoft lobjDailyPeopleSoft in lobjDailyPAPeopleSoft.iclbDailyPersonAccountPeopleSoft)
                    {
                        lobjEnrollmentData.icdoEnrollmentData.source_id = ibusHistory.icdoPersonAccountRetirementHistory.person_account_retirement_history_id;
                        lobjEnrollmentData.icdoEnrollmentData.plan_id = icdoPersonAccount.plan_id;
                        lobjEnrollmentData.icdoEnrollmentData.employment_type_value = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value;
                        lobjEnrollmentData.icdoEnrollmentData.ssn = ibusPerson.icdoPerson.ssn;
                        lobjEnrollmentData.icdoEnrollmentData.ndpers_member_id = ibusPerson.icdoPerson.person_id;
                        lobjEnrollmentData.icdoEnrollmentData.person_account_id = icdoPersonAccount.person_account_id;
                        lobjEnrollmentData.icdoEnrollmentData.peoplesoft_id = ibusPerson.icdoPerson.peoplesoft_id;
                        lobjEnrollmentData.icdoEnrollmentData.employer_org_id = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id;
                        lobjEnrollmentData.icdoEnrollmentData.plan_status_value = ibusHistory.icdoPersonAccountRetirementHistory.plan_participation_status_value;
                        lobjEnrollmentData.icdoEnrollmentData.change_reason_value = icdoPersonAccount.reason_value;
                        lobjEnrollmentData.icdoEnrollmentData.start_date = ibusHistory.icdoPersonAccountRetirementHistory.start_date;
                        lobjEnrollmentData.icdoEnrollmentData.end_date = ibusHistory.icdoPersonAccountRetirementHistory.end_date;

                        if ((lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date == DateTime.MinValue || (lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date != DateTime.MinValue &&
                            busGlobalFunctions.CheckDateOverlapping(icdoPersonAccount.history_change_date.AddMonths(-1), lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date,
                            new DateTime(lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date.Year, lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date.Month, 1).AddMonths(2)))) &&
                            lintCounter == 0)
                            lobjEnrollmentData.icdoEnrollmentData.is_benefit_enrollment_report_generated = busConstant.Flag_No;
                        else
                            lobjEnrollmentData.icdoEnrollmentData.is_benefit_enrollment_report_generated = busConstant.Flag_Yes;

                        lobjEnrollmentData.icdoEnrollmentData.coverage_code = lobjDailyPeopleSoft.istrCoverageCode;
                        lobjEnrollmentData.icdoEnrollmentData.benefit_plan = lobjDailyPeopleSoft.istrBenefitPlan;
                        lobjEnrollmentData.icdoEnrollmentData.deduction_begin_date = lobjDailyPeopleSoft.idtDeductionBeginDate;
                        lobjEnrollmentData.icdoEnrollmentData.coverage_begin_date = lobjDailyPeopleSoft.idtCoverageBeginDate;
                        lobjEnrollmentData.icdoEnrollmentData.election_date = lobjDailyPeopleSoft.idtElectionDate;
                        lobjEnrollmentData.icdoEnrollmentData.coverage_election_value = lobjDailyPeopleSoft.istrCoverageElection;
                        lobjEnrollmentData.icdoEnrollmentData.plan_type = lobjDailyPeopleSoft.istrPlanType;
                        lobjEnrollmentData.icdoEnrollmentData.peoplesoft_org_group_value = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.peoplesoft_org_group_value;
                        lobjEnrollmentData.icdoEnrollmentData.peoplesoft_change_reason_value = lobjDailyPAPeopleSoft.istrPSFileChangeEvent;

                        if (icdoPersonAccount.plan_id == busConstant.PlanIdDC2025)
                        {
                            if (lobjEnrollmentData.icdoEnrollmentData.employment_type_value == busConstant.PersonJobTypePermanent)
                                lobjEnrollmentData.icdoEnrollmentData.employee_employer_match = lintAddlEEContriPercent + "% Employee - " + lintAddlEEContriPercent + "% Employer Match";
                            else
                                lobjEnrollmentData.icdoEnrollmentData.employee_employer_match = lintAddlEEContriPercent + "% Employee";
                        }

                        //PIR 25920 New DC Plan
                        //if (icdoPersonAccount.plan_id == busConstant.PlanIdDC2025)
                        //{
                        //    lobjEnrollmentData.icdoEnrollmentData.pay_period_amount = icdoPersonAccount.addl_ee_contribution_percent;
                        //    lobjEnrollmentData.icdoEnrollmentData.provider_org_id = ibusProvider.icdoOrganization.org_id;
                        //}
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
            else //If ORG GROUP of the current Organization is not PeopleSoft, then 1 entry for Benefit Enrollment.
            {
                if ((lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date == DateTime.MinValue || (lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date != DateTime.MinValue &&
                busGlobalFunctions.CheckDateOverlapping(icdoPersonAccount.history_change_date.AddMonths(-1), lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date,
                new DateTime(lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date.Year, lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date.Month, 1).AddMonths(2)))))
                {
                    lobjEnrollmentData.icdoEnrollmentData.source_id = ibusHistory.icdoPersonAccountRetirementHistory.person_account_retirement_history_id;
                    lobjEnrollmentData.icdoEnrollmentData.plan_id = icdoPersonAccount.plan_id;
                    lobjEnrollmentData.icdoEnrollmentData.employment_type_value = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value;
                    lobjEnrollmentData.icdoEnrollmentData.ssn = ibusPerson.icdoPerson.ssn;
                    lobjEnrollmentData.icdoEnrollmentData.ndpers_member_id = ibusPerson.icdoPerson.person_id;
                    lobjEnrollmentData.icdoEnrollmentData.person_account_id = icdoPersonAccount.person_account_id;
                    lobjEnrollmentData.icdoEnrollmentData.peoplesoft_id = ibusPerson.icdoPerson.peoplesoft_id;
                    lobjEnrollmentData.icdoEnrollmentData.employer_org_id = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id;
                    lobjEnrollmentData.icdoEnrollmentData.plan_status_value = ibusHistory.icdoPersonAccountRetirementHistory.plan_participation_status_value;
                    lobjEnrollmentData.icdoEnrollmentData.change_reason_value = icdoPersonAccount.reason_value;
                    lobjEnrollmentData.icdoEnrollmentData.start_date = ibusHistory.icdoPersonAccountRetirementHistory.start_date;
                    lobjEnrollmentData.icdoEnrollmentData.end_date = ibusHistory.icdoPersonAccountRetirementHistory.end_date;
                    lobjEnrollmentData.icdoEnrollmentData.peoplesoft_org_group_value = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.peoplesoft_org_group_value;
                    lobjEnrollmentData.icdoEnrollmentData.is_benefit_enrollment_report_generated = busConstant.Flag_No;
                    lobjEnrollmentData.icdoEnrollmentData.peoplesoft_file_sent_flag = busConstant.Flag_Yes;

                    if (icdoPersonAccount.plan_id == busConstant.PlanIdDC2025)
                    {
                        if (lobjEnrollmentData.icdoEnrollmentData.employment_type_value == busConstant.PersonJobTypePermanent)
                            lobjEnrollmentData.icdoEnrollmentData.employee_employer_match = lintAddlEEContriPercent + "% Employee - " + lintAddlEEContriPercent + "% Employer Match";
                        else
                            lobjEnrollmentData.icdoEnrollmentData.employee_employer_match = lintAddlEEContriPercent + "% Employee";
                    }

                    //PIR 25920 New DC Plan
                    //if (icdoPersonAccount.plan_id == busConstant.PlanIdDC2025)
                    //{
                    //    lobjEnrollmentData.icdoEnrollmentData.pay_period_amount = icdoPersonAccount.addl_ee_contribution_percent;
                    //    lobjEnrollmentData.icdoEnrollmentData.provider_org_id = ibusProvider.icdoOrganization.org_id;
                    //}
                    lobjEnrollmentData.icdoEnrollmentData.Insert();
                }
            }

            //PIR 25920 - Insert Deferred comp records into Enrollment data if plan is enrolled (plan enrolled and has active provider) and EE Percent is changed. 
                if (iblnIsEEPercentChanged)
                {
                if (lintAddlEEContriPercent == 3)
                {
                    InsertIntoEnrollmentDataForMATCH3();
                    DBFunction.DBNonQuery("entPersonAccountRetirement.UpdateMatch3InsertedForBERandPS", new object[1] { icdoPersonAccount.person_id },
                                                                                                  iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                }
                else
                {
                    DataTable ldtDefCompProvider = DBFunction.DBSelect("entPersonAccount.IsEnrolledInDefCompWithActiveProvider", new object[1] { icdoPersonAccount.person_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                    if (ldtDefCompProvider.Rows.Count > 0)
                    {
                        busPersonAccountDeferredCompProvider lbusPADeferredCompProvider = new busPersonAccountDeferredCompProvider { icdoPersonAccountDeferredCompProvider = new cdoPersonAccountDeferredCompProvider() };

                        busEnrollmentData lobjEnrollmentDataDefComp = new busEnrollmentData { icdoEnrollmentData = new doEnrollmentData() };
                        busDailyPersonAccountPeopleSoft lobjDailyPAPeopleSoftDefComp = new busDailyPersonAccountPeopleSoft();

                        if (lbusPADeferredCompProvider.FindPersonAccountDeferredCompProvider(Convert.ToInt32(ldtDefCompProvider.Rows[0]["person_account_provider_id"])))
                        {
                            lbusPADeferredCompProvider.LoadPersonAccountDeferredComp();
                            lbusPADeferredCompProvider.ibusPersonAccountDeferredComp.LoadPerson();
                            lbusPADeferredCompProvider.ibusPersonAccountDeferredComp.LoadPlan();
                            lbusPADeferredCompProvider.ibusPersonAccountDeferredComp.icdoPersonAccount.person_employment_dtl_id = icdoPersonAccount.person_employment_dtl_id;
                            lbusPADeferredCompProvider.LoadPersonAccount();
                            lbusPADeferredCompProvider.ibusPersonAccountDeferredComp.LoadPersonEmploymentDetail();
                            lbusPADeferredCompProvider.ibusPersonAccountDeferredComp.ibusPersonEmploymentDetail.LoadPersonEmployment();
                            lbusPADeferredCompProvider.ibusPersonAccountDeferredComp.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
                            lbusPADeferredCompProvider.ibusPersonAccountDeferredComp.LoadOrgPlan();
                            lbusPADeferredCompProvider.ibusPersonAccountDeferredComp.LoadPreviousHistory();
                            lbusPADeferredCompProvider.InsertIntoEnrollmentData(false, busConstant.Flag_No, true, lintAddlEEContriPercent);
                        }
                    }
                }
                }              
        }

        public void InsertIntoEnrollmentDataForMATCH3()
        {
            DataTable ldtbList = DBFunction.DBSelect("entPersonAccountRetirement.CheckIfMatch3InsertedForBERorPS", new object[1] { icdoPersonAccount.person_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            if (ldtbList.Rows.Count > 0)
            {
                Collection<busEnrollmentData> lclbEnrollmentDataMatch3 = GetCollection<busEnrollmentData>(ldtbList, "icdoEnrollmentData");

                if (lclbEnrollmentDataMatch3.Count > 0)
                {
                    foreach (busEnrollmentData lobjEnrollmentDataMatch3 in lclbEnrollmentDataMatch3)
                    {
                        busEnrollmentData lobjEnrollmentDataMatch3Terminate = new busEnrollmentData { icdoEnrollmentData = new doEnrollmentData() };
                        lobjEnrollmentDataMatch3Terminate.icdoEnrollmentData.source_id = lobjEnrollmentDataMatch3.icdoEnrollmentData.source_id;
                        lobjEnrollmentDataMatch3Terminate.icdoEnrollmentData.plan_id = lobjEnrollmentDataMatch3.icdoEnrollmentData.plan_id;
                        lobjEnrollmentDataMatch3Terminate.icdoEnrollmentData.plan_type = lobjEnrollmentDataMatch3.icdoEnrollmentData.plan_type;
                        lobjEnrollmentDataMatch3Terminate.icdoEnrollmentData.ssn = lobjEnrollmentDataMatch3.icdoEnrollmentData.ssn;
                        lobjEnrollmentDataMatch3Terminate.icdoEnrollmentData.ndpers_member_id = lobjEnrollmentDataMatch3.icdoEnrollmentData.ndpers_member_id;
                        lobjEnrollmentDataMatch3Terminate.icdoEnrollmentData.person_account_id = lobjEnrollmentDataMatch3.icdoEnrollmentData.person_account_id;
                        lobjEnrollmentDataMatch3Terminate.icdoEnrollmentData.peoplesoft_id = lobjEnrollmentDataMatch3.icdoEnrollmentData.peoplesoft_id;
                        lobjEnrollmentDataMatch3Terminate.icdoEnrollmentData.employer_org_id = lobjEnrollmentDataMatch3.icdoEnrollmentData.employer_org_id;
                        lobjEnrollmentDataMatch3Terminate.icdoEnrollmentData.employment_type_value = lobjEnrollmentDataMatch3.icdoEnrollmentData.employment_type_value;
                        lobjEnrollmentDataMatch3Terminate.icdoEnrollmentData.plan_status_value = lobjEnrollmentDataMatch3.icdoEnrollmentData.plan_status_value;
                        lobjEnrollmentDataMatch3Terminate.icdoEnrollmentData.peoplesoft_change_reason_value = busConstant.DeferredCompEndDateProvider;
                        lobjEnrollmentDataMatch3Terminate.icdoEnrollmentData.coverage_election_value = "T";
                        lobjEnrollmentDataMatch3Terminate.icdoEnrollmentData.start_date = lobjEnrollmentDataMatch3.icdoEnrollmentData.start_date;
                        lobjEnrollmentDataMatch3Terminate.icdoEnrollmentData.end_date = lobjEnrollmentDataMatch3.icdoEnrollmentData.start_date;
                        lobjEnrollmentDataMatch3Terminate.icdoEnrollmentData.coverage_begin_date = lobjEnrollmentDataMatch3.icdoEnrollmentData.coverage_begin_date;
                        lobjEnrollmentDataMatch3Terminate.icdoEnrollmentData.deduction_begin_date = lobjEnrollmentDataMatch3.icdoEnrollmentData.deduction_begin_date;
                        lobjEnrollmentDataMatch3Terminate.icdoEnrollmentData.election_date = lobjEnrollmentDataMatch3.icdoEnrollmentData.election_date;
                        lobjEnrollmentDataMatch3Terminate.icdoEnrollmentData.pay_period_amount = 0.0M;
                        lobjEnrollmentDataMatch3Terminate.icdoEnrollmentData.provider_org_id = lobjEnrollmentDataMatch3.icdoEnrollmentData.provider_org_id;
                        lobjEnrollmentDataMatch3Terminate.icdoEnrollmentData.peoplesoft_org_group_value = lobjEnrollmentDataMatch3.icdoEnrollmentData.peoplesoft_org_group_value;

                        if (lobjEnrollmentDataMatch3.icdoEnrollmentData.is_benefit_enrollment_report_generated == "Y")
                            lobjEnrollmentDataMatch3Terminate.icdoEnrollmentData.is_benefit_enrollment_report_generated = "N";
                        else
                            lobjEnrollmentDataMatch3Terminate.icdoEnrollmentData.is_benefit_enrollment_report_generated = "Y";

                        if (lobjEnrollmentDataMatch3.icdoEnrollmentData.peoplesoft_file_sent_flag == "Y" && lobjEnrollmentDataMatch3.icdoEnrollmentData.peoplesoft_org_group_value == busConstant.PeopleSoftOrgGroupValueState)
                            lobjEnrollmentDataMatch3Terminate.icdoEnrollmentData.peoplesoft_file_sent_flag = "N";
                        else
                            lobjEnrollmentDataMatch3Terminate.icdoEnrollmentData.peoplesoft_file_sent_flag = "Y";

                        lobjEnrollmentDataMatch3Terminate.icdoEnrollmentData.Insert();
                    }
                }
            }
        }

        private void PostESSMessage()
        {
            if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled ||
                icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetimentSuspended) // PIR 9115 add suspended check
            {
                if (ibusPerson == null)
                    LoadPerson();
                if (ibusPlan == null)
                    LoadPlan();
                if (icdoPersonAccount.person_employment_dtl_id <= 0)
                    icdoPersonAccount.person_employment_dtl_id = GetEmploymentDetailID();
                // post message to employer
                if (ibusPersonEmploymentDetail == null)
                    LoadPersonEmploymentDetail();

                if (ibusPersonEmploymentDetail.ibusPersonEmployment.IsNull())
                    ibusPersonEmploymentDetail.LoadPersonEmployment();
                
                //this s for regula enrollment
                if (ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeRetirement)
                {
                    //for optional case
                    //regular case
                    if (ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id > 0)
                    {
                        // PIR 9115
                        if (istrIsPIR9115Enabled == busConstant.Flag_No)
                        {
                            string lstrPrioityValue = string.Empty;
                            busWSSHelper.PublishESSMessage(0, 0, string.Format(busGlobalFunctions.GetDBMessageTextByDBMessageID(30, iobjPassInfo, ref lstrPrioityValue),
                                ibusPerson.icdoPerson.FullName, ibusPerson.icdoPerson.LastFourDigitsOfSSN, ibusPlan.icdoPlan.plan_name, icdoPersonAccount.history_change_date.ToString("d", CultureInfo.CreateSpecificCulture("en-US"))), //prod pir 6294
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
            }
        }

        public void RefreshValues()
        {
            icdoPersonAccount.suppress_warnings_flag = string.Empty; // UAT PIR ID 1015
        }

        private void CreateDCAccountFromDB()
        {
            busPersonAccountRetirement lobjPersonAccountRetirement = CreateNewDCAccount();
            if (lobjPersonAccountRetirement.icdoPersonAccountRetirement.person_account_id > 0)
            {
                //Transfer the Benificary from DB to DC
                if (iclbPersonAccountBeneficiary == null)
                    LoadPersonAccountBeneficiary();

                foreach (busPersonAccountBeneficiary lobjPersonAccountBeneficiary in iclbPersonAccountBeneficiary)
                {
                    lobjPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date = icdoPersonAccount.history_change_date;
                    lobjPersonAccountBeneficiary.icdoPersonAccountBeneficiary.Update();
                    CreateNewBeneficiary(lobjPersonAccountBeneficiary, lobjPersonAccountRetirement);
                }
            }
        }

        private busPersonAccountRetirement CreateNewDCAccount()
        {
            busPersonAccountRetirement lobjPersonAccountReitirement = new busPersonAccountRetirement();
            lobjPersonAccountReitirement.icdoPersonAccount = new cdoPersonAccount();
            lobjPersonAccountReitirement.icdoPersonAccountRetirement = new cdoPersonAccountRetirement();
            lobjPersonAccountReitirement.ibusPerson = ibusPerson;
            lobjPersonAccountReitirement.ibusPersonEmploymentDetail = ibusPersonEmploymentDetail;
            lobjPersonAccountReitirement.ibusPersonEmploymentDetail.ibusPersonEmployment = ibusPersonEmploymentDetail.ibusPersonEmployment;
            if (lobjPersonAccountReitirement.ibusPersonEmploymentDetail.iclbAllPersonAccountEmpDtl == null)
                lobjPersonAccountReitirement.ibusPersonEmploymentDetail.LoadAllPersonAccountEmploymentDetails();

            //Set the Election Value as Enrolled in Person Employment Detail
            foreach (busPersonAccountEmploymentDetail lobjPAEmploymentDetail in lobjPersonAccountReitirement.ibusPersonEmploymentDetail.iclbAllPersonAccountEmpDtl)
            {
                if (lobjPAEmploymentDetail.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdDC ||
                    lobjPAEmploymentDetail.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdDC2020 ||//PIR 20232 ?code
                    lobjPAEmploymentDetail.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdDC2025)
                {
                    lobjPAEmploymentDetail.icdoPersonAccountEmploymentDetail.election_value = busConstant.PersonAccountElectionValueEnrolled;
                    lobjPAEmploymentDetail.icdoPersonAccountEmploymentDetail.Update();
                    lobjPersonAccountReitirement.icdoPersonAccount.plan_id = lobjPAEmploymentDetail.icdoPersonAccountEmploymentDetail.plan_id; //PIR 20232
                }
            }
            //lobjPersonAccountReitirement.icdoPersonAccount.plan_id = busConstant.PlanIdDC;
            lobjPersonAccountReitirement.LoadPlan();
            lobjPersonAccountReitirement.LoadOrgPlan();
            lobjPersonAccountReitirement.icdoPersonAccount.person_id = icdoPersonAccount.person_id;
            lobjPersonAccountReitirement.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusRetirementEnrolled;
            lobjPersonAccountReitirement.icdoPersonAccount.start_date = icdoPersonAccount.history_change_date;
            lobjPersonAccountReitirement.icdoPersonAccount.person_employment_dtl_id = icdoPersonAccount.person_employment_dtl_id;
            lobjPersonAccountReitirement.icdoPersonAccount.ienuObjectState = ObjectState.Insert;
            lobjPersonAccountReitirement.icdoPersonAccount.status_value = busConstant.StatusValid;
            lobjPersonAccountReitirement.icdoPersonAccountRetirement.mutual_fund_window_flag = busConstant.Flag_No;
            lobjPersonAccountReitirement.icdoPersonAccountRetirement.ienuObjectState = ObjectState.Insert;
            lobjPersonAccountReitirement.BeforeValidate(utlPageMode.New);
            lobjPersonAccountReitirement.BeforePersistChanges();
            lobjPersonAccountReitirement.iarrChangeLog.Add(lobjPersonAccountReitirement.icdoPersonAccountRetirement);
            lobjPersonAccountReitirement.PersistChanges();
            lobjPersonAccountReitirement.ValidateSoftErrors();
            lobjPersonAccountReitirement.UpdateValidateStatus();
            lobjPersonAccountReitirement.AfterPersistChanges();
            return lobjPersonAccountReitirement;
        }
        private void CreateNewBeneficiary(busPersonAccountBeneficiary lobjPersonAccountBeneficiary, busPersonAccountRetirement lobjRetirement)
        {
            cdoPersonAccountBeneficiary lcdoPersonAccountBeneficiary = new cdoPersonAccountBeneficiary();
            lcdoPersonAccountBeneficiary.beneficiary_id = lobjPersonAccountBeneficiary.icdoPersonAccountBeneficiary.beneficiary_id;
            lcdoPersonAccountBeneficiary.beneficiary_type_value = lobjPersonAccountBeneficiary.icdoPersonAccountBeneficiary.beneficiary_type_value;
            lcdoPersonAccountBeneficiary.dist_percent = lobjPersonAccountBeneficiary.icdoPersonAccountBeneficiary.dist_percent;
            lcdoPersonAccountBeneficiary.nmso_co_flag = lobjPersonAccountBeneficiary.icdoPersonAccountBeneficiary.nmso_co_flag;
            lcdoPersonAccountBeneficiary.person_account_id = lobjRetirement.icdoPersonAccount.person_account_id;
            lcdoPersonAccountBeneficiary.start_date = lobjRetirement.icdoPersonAccount.start_date.AddDays(1); ;
            lcdoPersonAccountBeneficiary.Insert();
        }

        public void LoadPersonAccount()
        {
            if (icdoPersonAccount == null)
                icdoPersonAccount = new cdoPersonAccount();

            DataTable ldtbList = Select<cdoPersonAccount>(
                                    new string[1] { "person_account_id" },
                                    new object[1] { icdoPersonAccountRetirement.person_account_id }, null, null);
            if (ldtbList.Rows.Count > 0)
                icdoPersonAccount.LoadData(ldtbList.Rows[0]);
        }

        public string GetMemberType(int aintEmployerOrgID)
        {
            string lstrReturn = "";

            if (icdoPersonAccount == null)
                LoadPersonAccount();
            if (iclbAccountEmploymentDetail == null)
                LoadPersonAccountEmploymentDetails();
            foreach (busPersonAccountEmploymentDetail lbusEmploymentDetail in iclbAccountEmploymentDetail)
            {
                if (lbusEmploymentDetail.ibusEmploymentDetail == null)
                    lbusEmploymentDetail.LoadPersonEmploymentDetail();
            }
            Collection<busPersonAccountEmploymentDetail> lclbEmploymentDetail =
                busPersonAccountEmploymentDetail.SortDescending(iclbAccountEmploymentDetail);
            foreach (busPersonAccountEmploymentDetail lbusEmploymentDetail in lclbEmploymentDetail)
            {
                if (aintEmployerOrgID == 0)
                {
                    lstrReturn = lbusEmploymentDetail.ibusEmploymentDetail.icdoPersonEmploymentDetail.derived_member_type_value;
                    break;
                }
                else
                {
                    if (lbusEmploymentDetail.ibusEmploymentDetail.ibusPersonEmployment == null)
                        lbusEmploymentDetail.ibusEmploymentDetail.LoadPersonEmployment();
                    if (lbusEmploymentDetail.ibusEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id == aintEmployerOrgID)
                    {
                        lstrReturn = lbusEmploymentDetail.ibusEmploymentDetail.icdoPersonEmploymentDetail.derived_member_type_value;
                        break;
                    }
                }
            }
            return lstrReturn;
        }
        // Retirement Contributions
        public Collection<busPersonAccountRetirementContribution> _iclbRetirementContribution;
        public Collection<busPersonAccountRetirementContribution> iclbRetirementContribution
        {
            get
            {
                return _iclbRetirementContribution;
            }
            set
            {
                _iclbRetirementContribution = value;
            }
        }
        //Collection which contain the original Retirement contribution for a given person account id
        private Collection<busPersonAccountRetirementContribution> _iclbOriginalRetirementContribution;
        public Collection<busPersonAccountRetirementContribution> iclbOriginalRetirementContribution
        {
            get { return _iclbOriginalRetirementContribution; }
            set { _iclbOriginalRetirementContribution = value; }
        }

        public void LoadRetirementContribution()
        {
            DataTable ldtbList = Select<cdoPersonAccountRetirementContribution>(
                    new string[1] { "person_account_id" },
                    new object[1] { icdoPersonAccount.person_account_id }, null, null);
            _iclbRetirementContribution = GetCollection<busPersonAccountRetirementContribution>(ldtbList, "icdoPersonAccountRetirementContribution");
            foreach (busPersonAccountRetirementContribution lbusRetirementContribution in _iclbRetirementContribution)
            {
                lbusRetirementContribution.ibusPARetirement = this;
            }
            iclbRetirementContribution = busGlobalFunctions.Sort<busPersonAccountRetirementContribution>(
                                                "icdoPersonAccountRetirementContribution.PayPeriodYearMonth desc",
                                                 iclbRetirementContribution);
        }

        public void LoadFYTDDetail()
        {
            iclbRetirementContribution = new Collection<busPersonAccountRetirementContribution>();
            DateTime ldtCurrentDate = DateTime.Now;
            string lstrStartMonth;
            string lstrEndMonth;
            if (ldtCurrentDate.Month >= 7)
            {
                lstrStartMonth = Convert.ToString(ldtCurrentDate.Year) + "07";
                lstrEndMonth = Convert.ToString(ldtCurrentDate.Year + 1) + "06";
            }
            else
            {
                lstrStartMonth = Convert.ToString(ldtCurrentDate.Year - 1 + "07");
                lstrEndMonth = Convert.ToString(ldtCurrentDate.Year) + "06";
            }
            DataTable ldtbList = Select("cdoPersonAccountRetirementContribution.LoadFYTDDetail",
                      new object[3] { icdoPersonAccount.person_account_id, lstrStartMonth, lstrEndMonth });
            foreach (DataRow dr in ldtbList.Rows)
            {
                busPersonAccountRetirementContribution lbusPAContr = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };
                lbusPAContr.icdoPersonAccountRetirementContribution.LoadData(dr);
                lbusPAContr.GetMonthName();
                lbusPAContr.ibusPersonEmploymentDetail = new busPersonEmploymentDetail { icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail() };
                lbusPAContr.ibusPersonEmploymentDetail.ibusPersonEmployment = new busPersonEmployment { icdoPersonEmployment = new cdoPersonEmployment() };
                lbusPAContr.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
                if (!Convert.IsDBNull(dr["org_code"]))
                    lbusPAContr.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.org_code = dr["org_code"].ToString();
                iclbRetirementContribution.Add(lbusPAContr);
            }
            iclbOriginalRetirementContribution = iclbRetirementContribution;
        }

        public void LoadLTDDetail()
        {
            iclbRetirementContribution = new Collection<busPersonAccountRetirementContribution>();
            DataTable ldtbList = Select("cdoPersonAccountRetirementContribution.LoadLTDDetail",
                     new object[1] { icdoPersonAccount.person_account_id });            
            foreach (DataRow dr in ldtbList.Rows)
            {
                busPersonAccountRetirementContribution lbusPAContr = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };
                lbusPAContr.icdoPersonAccountRetirementContribution.LoadData(dr);
                lbusPAContr.GetMonthName();
                lbusPAContr.ibusPersonEmploymentDetail = new busPersonEmploymentDetail { icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail() };
                lbusPAContr.ibusPersonEmploymentDetail.ibusPersonEmployment = new busPersonEmployment { icdoPersonEmployment = new cdoPersonEmployment() };
                lbusPAContr.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
                if (!Convert.IsDBNull(dr["org_code"]))
                    lbusPAContr.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.org_code = dr["org_code"].ToString();
                iclbRetirementContribution.Add(lbusPAContr);
            }
            iclbOriginalRetirementContribution = iclbRetirementContribution;
        }


        public void LoadPreviousHistory()
        {
            if (_ibusHistory == null)
            {
                _ibusHistory = new busPersonAccountRetirementHistory();
                _ibusHistory.icdoPersonAccountRetirementHistory = new cdoPersonAccountRetirementHistory();
            }

            DataTable ldtbHistory = Select("cdoPersonAccountRetirement.LatestHistoryRecord", new object[1] { icdoPersonAccount.person_account_id });
            if (ldtbHistory.Rows.Count > 0)
            {
                _ibusHistory.icdoPersonAccountRetirementHistory.LoadData(ldtbHistory.Rows[0]);
            }
        }

        public void SetHistoryEntryRequiredOrNot()
        {
            if (_ibusHistory == null)
                LoadPreviousHistory();
            //PIR 25920 
			int lintAddlContribution = 0;
            if (ibusCurrentPersonEmploymentDetail.IsNull()) LoadCurrentPersonEmploymentDetail();
            if (ibusCurrentPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary)
                lintAddlContribution = icdoPersonAccount.addl_ee_contribution_percent_temp;
            else
                lintAddlContribution = icdoPersonAccount.addl_ee_contribution_percent;
                
            if (istrIsInitialStartDateChanged)
            {
                if ((icdoPersonAccount.plan_participation_status_value != ibusHistory.icdoPersonAccountRetirementHistory.plan_participation_status_value) ||
                (icdoPersonAccount.history_change_date != DateTime.MinValue) ||
                (icdoPersonAccountRetirement.dc_eligibility_date != ibusHistory.icdoPersonAccountRetirementHistory.dc_eligibility_date) ||
                (lintAddlContribution != ibusHistory.icdoPersonAccountRetirementHistory.addl_ee_contribution_percent)||
                (icdoPersonAccountRetirement.mutual_fund_window_flag != ((ibusHistory.icdoPersonAccountRetirementHistory.mutual_fund_window_flag).IsNullOrEmpty() ? busConstant.Flag_No : ibusHistory.icdoPersonAccountRetirementHistory.mutual_fund_window_flag))) //||
                //(icdoPersonAccountRetirement.capital_gain != ibusHistory.icdoPersonAccountRetirementHistory.capital_gain) ||
                //(icdoPersonAccountRetirement.rhic_benfit_amount != ibusHistory.icdoPersonAccountRetirementHistory.rhic_benfit_amount))
                {
                    IsHistoryEntryRequired = true;
                }
                else
                {
                    IsHistoryEntryRequired = false;
                }
            }
            else
            {
                if ((icdoPersonAccount.plan_participation_status_value != ibusHistory.icdoPersonAccountRetirementHistory.plan_participation_status_value) ||
                 (icdoPersonAccount.current_plan_start_date != ibusHistory.icdoPersonAccountRetirementHistory.start_date) ||
                 (icdoPersonAccountRetirement.dc_eligibility_date != ibusHistory.icdoPersonAccountRetirementHistory.dc_eligibility_date) ||
                 (lintAddlContribution != ibusHistory.icdoPersonAccountRetirementHistory.addl_ee_contribution_percent) ||
                 (icdoPersonAccountRetirement.mutual_fund_window_flag != ibusHistory.icdoPersonAccountRetirementHistory.mutual_fund_window_flag) ||
                 (icdoPersonAccountRetirement.capital_gain != ibusHistory.icdoPersonAccountRetirementHistory.capital_gain) ||
                 (icdoPersonAccountRetirement.rhic_benfit_amount != ibusHistory.icdoPersonAccountRetirementHistory.rhic_benfit_amount))
                {
                    IsHistoryEntryRequired = true;
                }
                else
                {
                    IsHistoryEntryRequired = false;
                }
            }
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
            if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled)
            {
                if (icdoPersonAccount.current_plan_start_date_no_null > DateTime.Now)
                    idtPlanEffectiveDate = icdoPersonAccount.current_plan_start_date_no_null;
                else
                    idtPlanEffectiveDate = DateTime.Now;
            }
            else
            {
                if (icolPersonAccountRetirementHistory == null)
                    LoadPersonAccountRetirementHistory();

                //By Default the Collection sorted by latest date
                foreach (busPersonAccountRetirementHistory lbusPersonAccountRetirementHistory in icolPersonAccountRetirementHistory)
                {
                    if (lbusPersonAccountRetirementHistory.icdoPersonAccountRetirementHistory.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled)
                    {
                        if (lbusPersonAccountRetirementHistory.icdoPersonAccountRetirementHistory.end_date == DateTime.MinValue)
                        {
                            //If the Start Date is Future Date, Set it otherwise Current Date will be Start Date of Premium Calc
                            if (lbusPersonAccountRetirementHistory.icdoPersonAccountRetirementHistory.start_date > DateTime.Now)
                            {
                                idtPlanEffectiveDate = lbusPersonAccountRetirementHistory.icdoPersonAccountRetirementHistory.start_date;
                            }
                            else
                            {
                                idtPlanEffectiveDate = DateTime.Now;
                            }
                        }
                        else
                        {
                            idtPlanEffectiveDate = lbusPersonAccountRetirementHistory.icdoPersonAccountRetirementHistory.end_date;
                        }
                        break;
                    }
                }
            }
        }

        #region DB Vested / Non Vested Correspondence Properties
        public int iintMainOrLEOrNG
        {
            get
            {
                int lstrResult = 0;
                if ((icdoPersonAccount.plan_id == busConstant.PlanIdMain) ||
                   (icdoPersonAccount.plan_id == busConstant.PlanIdMain2020) || //PIR 20232  
                   (icdoPersonAccount.plan_id == busConstant.PlanIdLE) ||
                   (icdoPersonAccount.plan_id == busConstant.PlanIdNG) || 
                   (icdoPersonAccount.plan_id == busConstant.PlanIdLEWithoutPS) || //PIR-12629
                    (icdoPersonAccount.plan_id == busConstant.PlanIdBCILawEnf) || //pir 7943
                   (icdoPersonAccount.plan_id == busConstant.PlanIdStatePublicSafety)) //PIR 25729
                {
                    lstrResult = 1;
                }
                return lstrResult;
            }
        }
        public int iintBCI
        {
            get
            {
                int lstrResult = 0;
                if (icdoPersonAccount.plan_id == busConstant.PlanIdBCILawEnf)
                {
                    lstrResult = 1;
                }
                return lstrResult;
            }
        }
        public int iintJudges
        {
            get
            {
                int lstrResult = 0;
                if (icdoPersonAccount.plan_id == busConstant.PlanIdJudges)
                {
                    lstrResult = 1;
                }
                return lstrResult;
            }
        }
        public int iintHP
        {
            get
            {
                int lstrResult = 0;
                if (icdoPersonAccount.plan_id == busConstant.PlanIdHP)
                {
                    lstrResult = 1;
                }
                return lstrResult;
            }
        }

        public decimal idec20PercentageMemberAccountBalance
        {
            get
            {
                return Math.Round(Pre_Tax_Employee_Contribution_ltd * 0.2M, 2, MidpointRounding.AwayFromZero); // PIR 9179
            }
        }

        public override busBase GetCorPerson()
        {
            if (ibusPerson == null)
                LoadPerson();
            return ibusPerson;
        }

        public override busBase GetCorOrganization()
        {
            if (ibusPersonEmploymentDetail == null) LoadPersonEmploymentDetail();
            if (ibusPersonEmploymentDetail.ibusPersonEmployment == null) ibusPersonEmploymentDetail.LoadPersonEmployment();
            if (ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization == null) ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
            return ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization;
        }

        #endregion

        # region UCS-60

        //UCS -060 BR - 01
        //check wether this person is RTW member
        //and having a payee account in Suspended, complete, cancelled status
        public bool IsEligibleRTWMember()
        {
            bool lblnIsRTWMember = false;
            int lintPayeeAccountId = 0;
            if (ibusPerson.IsNull())
                LoadPerson();
            busPersonAccount lobjRTWPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
            lobjRTWPersonAccount = ibusPerson.LoadActivePersonAccountByPlan(icdoPersonAccount.plan_id);
            if (lobjRTWPersonAccount.IsNotNull())
            {
                if (lobjRTWPersonAccount.icdoPersonAccount.person_account_id != icdoPersonAccount.person_account_id)
                {
                    lblnIsRTWMember = ibusPerson.IsRTWMember(icdoPersonAccount.plan_id, busConstant.PayeeStatusForRTW.IgnoreStatus, ref lintPayeeAccountId);
                    if (((lblnIsRTWMember)) && (lintPayeeAccountId > 0))
                    {
                        busPayeeAccount lobjPayeeAccount = new busPayeeAccount();
                        lobjPayeeAccount.FindPayeeAccount(lintPayeeAccountId);
                        lobjPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();
                        if ((!lobjPayeeAccount.ibusPayeeAccountActiveStatus.IsSuspendedOrCancelledOrCompletedForDisability())
                           && (!(lobjPayeeAccount.ibusPayeeAccountActiveStatus.IsSuspendedOrCancelledOrCompletedForRetirement())))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        # endregion

        // UCS-022-26
        // UAT-PIR-ID-565
        public bool IsMemberTookRefund()
        {
            if (ibusPersonEmploymentDetail.IsNull())
                LoadPersonEmploymentDetail();
            if (ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary)
            {
                if (ibusPerson.IsNull())
                    LoadPerson();
                if (ibusPerson.iclbRetirementAccount.IsNull())
                    ibusPerson.LoadRetirementAccount();
                foreach (busPersonAccount lobjPersonAccount in ibusPerson.iclbRetirementAccount)
                {
                    if (lobjPersonAccount.icdoPersonAccount.person_account_id != icdoPersonAccount.person_account_id)
                    {
                        if (lobjPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementWithDrawn)
                        {
                            if (lobjPersonAccount.iclbAccountEmploymentDetail.IsNull())
                                lobjPersonAccount.LoadPersonAccountEmploymentDetails();
                            foreach (busPersonAccountEmploymentDetail lobjPAEmpDtl in lobjPersonAccount.iclbAccountEmploymentDetail)
                            {
                                if (lobjPAEmpDtl.ibusEmploymentDetail.IsNull())
                                    lobjPAEmpDtl.LoadPersonEmploymentDetail();
                                if (lobjPAEmpDtl.ibusEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        public busPersonAccountRetirementHistory ibusPARetirementHistory { get; set; }

        // This method will load the Previous History where the History is Enrolled. 
        // Used in HMO file.
        public void LoadPreviousEnrolledHistory()
        {
            if (ibusPARetirementHistory.IsNull())
                ibusPARetirementHistory = new busPersonAccountRetirementHistory { icdoPersonAccountRetirementHistory = new cdoPersonAccountRetirementHistory() };

            DataTable ldtbHistory = Select<cdoPersonAccountRetirementHistory>(
                                                new string[2] { "PERSON_ACCOUNT_ID", "PLAN_PARTICIPATION_STATUS_VALUE" },
                                                new object[2] { icdoPersonAccount.person_account_id, busConstant.PlanParticipationStatusRetirementEnrolled },
                                                null, "PERSON_ACCOUNT_RETIREMENT_HISTORY_ID DESC");
            if (ldtbHistory.Rows.Count > 0)
                ibusPARetirementHistory.icdoPersonAccountRetirementHistory.LoadData(ldtbHistory.Rows[0]);
        }
        //PIR 25420 Load all enrolled Retirement History and combine them into one record... Also calculates and returns contribution months 
        public int LoadPreviousEnrolledHistoryAndCalculateContribMonths()
        { 
            if (ibusPARetirementHistory.IsNull())
                ibusPARetirementHistory = new busPersonAccountRetirementHistory { icdoPersonAccountRetirementHistory = new cdoPersonAccountRetirementHistory() };
            int lintContributionMonths = 0;
            DataTable ldtbHistory = Select<cdoPersonAccountRetirementHistory>(
                                                new string[2] { "PERSON_ACCOUNT_ID", "PLAN_PARTICIPATION_STATUS_VALUE" },
                                                new object[2] { icdoPersonAccount.person_account_id, busConstant.PlanParticipationStatusRetirementEnrolled },
                                                null, "PERSON_ACCOUNT_RETIREMENT_HISTORY_ID ASC");
            Collection<busPersonAccountRetirementHistory> lclbRetHist = GetCollection<busPersonAccountRetirementHistory>(ldtbHistory, "icdoPersonAccountRetirementHistory");

            if (ldtbHistory.Rows.Count > 0)
            {
                ibusPARetirementHistory.icdoPersonAccountRetirementHistory.LoadData(ldtbHistory.Rows[0]);
                foreach (busPersonAccountRetirementHistory lobjPARetHist in lclbRetHist)
                {
                    ibusPARetirementHistory.icdoPersonAccountRetirementHistory.end_date = lobjPARetHist.icdoPersonAccountRetirementHistory.end_date;
                        if (lobjPARetHist.icdoPersonAccountRetirementHistory.end_date != DateTime.MinValue)
                        lintContributionMonths += busGlobalFunctions.DateDiffByMonth(lobjPARetHist.icdoPersonAccountRetirementHistory.start_date,
                                                                                    lobjPARetHist.icdoPersonAccountRetirementHistory.end_date);
                }
            }
            return lintContributionMonths;
        }

        public override void LoadCorresProperties(string astrTemplateName)
        {
			//the object needed to generate this correspondance gets populated in the nonpayee employment termination batch
            if (astrTemplateName != "APP-7359")
            {
                //to avoid errors when Vesting letter is generated thru batch
                if (ibusPersonAccount.IsNull())
                    LoadPersonAccount();
                ibusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
                ibusPersonAccount.icdoPersonAccount = icdoPersonAccount;
                //UAT PIR: 2351 Optimization changes.
                //Exclude the Below method if it is called for DC Remainder batch.
                if (astrTemplateName != "ENR-5300")
                {
                    LoadNewRetirementBenefitCalculation();
                    AssignRetirementBenefitCalculationFields(busConstant.CalculationTypeEstimate);
                    CalculateRetirementBenefitAmount();
                    SetVestingProperty();
                    GetLastContributionDate();
                    LoadPersonAccountForRetirementPlan();

                    //ENR-5062
                    if (ibusProvider.IsNull())
                        LoadProvider();
                }
                //prod pir 6003
                idecTotalVestedContributionsForCorrs = icdoPersonAccountRetirement.capital_gain + ee_rhic_ser_pur_cont_ltd + er_vested_amount_ltd + interest_amount_ltd;
                
            }
            if (astrTemplateName == "PER-0359")
            {
                if (ibusPlan == null)
                    LoadPlan();
                if (ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueDB)
                    iintPlanID = 1;

                if (ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueDC || ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueHB)
                    iintPlanID = 0;
            }
        }

        //PIR 20233 - added a property For check plan is main2020 or dc2020
        public int iintIsMain2020orDC2020
        {
            get
            {
                if (icdoPersonAccount == null)
                    LoadPersonAccount();
                return (icdoPersonAccount.plan_id == busConstant.PlanIdMain2020 || icdoPersonAccount.plan_id == busConstant.PlanIdDC2020) ? 1 : 0;
            }
        }
        /// PIR ID 1798 - Vested Employer Contribution Percent Calculation on the fly
        public void CalculateERVestedPercentage()
        {
            if ((icdoPersonAccount.plan_id != busConstant.PlanIdHP) &&
                (icdoPersonAccount.plan_id != busConstant.PlanIdJudges) &&
                (icdoPersonAccount.plan_id != busConstant.PlanIdJobService))
            {
                int aintReferenceID = 0;
                decimal ldecTVSC = 0M;
                if (ibusPlan.IsNull()) LoadPlan();
                if (ibusPerson.IsNull()) LoadPerson();

                // PROD PIR ID 6190 -- TVSC should be calculated as in Benefit calculations
                ldecTVSC = ibusPerson.GetTotalVSCForPerson(icdoPersonAccount.plan_id == busConstant.PlanIdJobService, DateTime.Now, true, false);
                icdoPersonAccount.vested_employer_cont_percent = GetVestedERSchedulePercentage(DateTime.Now, ibusPlan.icdoPlan.retirement_type_value, ldecTVSC, ref aintReferenceID);
            }
        }

        public DateTime idteCurrentDate
        {
            get
            {
                return DateTime.Now;
            }
        }

        public string idteCurrentDateLongDate
        {
            get
            {
                return idteCurrentDate.ToString(BusinessObjects.busConstant.DateFormatLongDate);
            }
        }


        public string istrTotalPSC
        {
            get
            {
                return Convert.ToString(Convert.ToInt32(icdoPersonAccount.Total_PSC));
            }
        }

        # region UCS 22 cor
        public string istrIsPersonVested { get; set; }

        public void SetVestingProperty()
        {
            int lintMonths = 0;
            decimal ldecMonthYear = 0.00M;
            int lintMemberAgeYear = 0;
            int lintMemberAgeMonth = 0;

            if (ibusPerson == null)
                LoadPerson();

            if (ibusPlan == null)
                LoadPlan();

            CalculateAge(ibusPerson.icdoPerson.date_of_birth, DateTime.Now, ref lintMonths, ref ldecMonthYear, 2, ref lintMemberAgeYear, ref lintMemberAgeMonth);
            istrIsPersonVested = busConstant.Flag_No;
            if (busPersonBase.CheckIsPersonVested(ibusPlan.icdoPlan.plan_id, ibusPlan.icdoPlan.plan_code, ibusPlan.icdoPlan.benefit_provision_id,
                                                    busConstant.ApplicationBenefitTypeRetirement, icdoPersonAccount.Total_VSC,
                                                    ldecMonthYear, DateTime.Now, false, DateTime.Now.AddMonths(-1).GetLastDayofMonth(), this, iobjPassInfo)) //PIR 14646
                istrIsPersonVested = busConstant.Flag_Yes;
        }

        public int iintLastContributionYear { get; set; }
        public string istrLastContributionMonthYear { get; set; }
        public void GetLastContributionDate()
        {
            int lintLastContributionYear = 0;
            istrLastContributionMonthYear = string.Empty;

            if (iclbRetirementContributionAll.IsNull())
                LoadRetirementContributionAll();
            if (iclbRetirementContributionAll.Count > 0)
            {
                busPersonAccountRetirementContribution lobjPersonAccountRetirementContribution = new busPersonAccountRetirementContribution
                {
                    icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution()
                };
                Collection<busPersonAccountRetirementContribution> lclbRetContribution = busGlobalFunctions.Sort<busPersonAccountRetirementContribution>
                                    ("icdoPersonAccountRetirementContribution.effective_date desc", iclbRetirementContributionAll); // PROD PIR ID 5124
                lobjPersonAccountRetirementContribution = lclbRetContribution.Where(lobjRet => 
                                    lobjRet.icdoPersonAccountRetirementContribution.subsystem_value != busConstant.SubSystemValueConversionOpeningBalance).FirstOrDefault();
                if (lobjPersonAccountRetirementContribution.IsNotNull())
                {
                    if(lobjPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.effective_date!=DateTime.MinValue)
                    {
                        //PIR-12347
                        istrLastContributionMonthYear = lobjPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.effective_date.ToString("MMMM yyyy");
                    }
                }
            }
        }

        //load person account
        public busPersonAccount ibusPersonAccount { get; set; }
        public void LoadPersonAccountForRetirementPlan()
        {
            if (ibusPersonAccount.IsNull())
                ibusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };

            ibusPersonAccount.icdoPersonAccount = this.icdoPersonAccount;
        }
        # endregion

        public decimal idecEarlyRetirementAge { get; set; }

        //PIR - 1567
        public string member_age_formatted
        {
            get
            {
                return Convert.ToString(idecEarlyRetirementAge);
            }
        }

        public string istrDCEligibilityEndDate
        {
            get
            {
                return icdoPersonAccountRetirement.dc_eligibility_date.ToString(busConstant.DateFormatLongDate);
            }
        }

        //PIR 1661 - PER - 0350
        public decimal idecRetirementContribution
        {
            get
            {
                return pre_tax_ee_amount_ltd + pre_tax_ee_ser_pur_cont_ltd + post_tax_ee_amount_ltd
                    + post_tax_ee_ser_pur_cont_ltd + _ee_er_pickup_amount_ltd + _ee_rhic_amount_ltd;
            }
        }

        //PIR - 1661
        public string istrQDROExists { get; set; }
        public void SetQDRO()
        {
            istrQDROExists = busConstant.Flag_No;
            if (ibusPerson.IsNull())
                LoadPerson();
            if (ibusPerson.iclbDROApplication.IsNull())
                ibusPerson.LoadDROApplications();

            var lenuMDROApp = ibusPerson.iclbDROApplication.Where(lobjDROApp => lobjDROApp.icdoBenefitDroApplication.dro_status_value == busConstant.DROApplicationStatusQualified
                && icdoPersonAccount.plan_id == lobjDROApp.icdoBenefitDroApplication.plan_id);
            if (lenuMDROApp.Count() > 0)
                istrQDROExists = busConstant.Flag_Yes;
        }

        public busPersonAccountRetirementHistory ibusHistoryAsofDate { get; set; }

        public void LoadEnrolledHistoryByMonthYear(DateTime adtEffectiveDate)
        {
            ibusHistoryAsofDate = new busPersonAccountRetirementHistory { icdoPersonAccountRetirementHistory = new cdoPersonAccountRetirementHistory() };
            /*DataTable ldtbHistory = Select<cdoPersonAccountRetirementHistory>(
                                                new string[1] { "PERSON_ACCOUNT_ID" },
                                                new object[1] { icdoPersonAccount.person_account_id },
                                                null, "PERSON_ACCOUNT_RETIREMENT_HISTORY_ID DESC");
            Collection<busPersonAccountRetirementHistory> lclbRetHistory = new Collection<busPersonAccountRetirementHistory>();
            lclbRetHistory = GetCollection<busPersonAccountRetirementHistory>(ldtbHistory, "icdoPersonAccountRetirementHistory");
            foreach (busPersonAccountRetirementHistory lobjHistory in lclbRetHistory)
            {
                if (lobjHistory.icdoPersonAccountRetirementHistory.start_date != lobjHistory.icdoPersonAccountRetirementHistory.end_date)
                {
                    if ((busGlobalFunctions.CheckDateOverlapping(adtEffectiveDate.GetFirstDayofCurrentMonth(), adtEffectiveDate.GetLastDayofMonth(),
                        lobjHistory.icdoPersonAccountRetirementHistory.start_date, lobjHistory.icdoPersonAccountRetirementHistory.end_date) ||
                        busGlobalFunctions.CheckDateOverlapping(lobjHistory.icdoPersonAccountRetirementHistory.start_date, lobjHistory.icdoPersonAccountRetirementHistory.end_date,
                        adtEffectiveDate.GetFirstDayofCurrentMonth(), adtEffectiveDate.GetLastDayofMonth())) &&
                        lobjHistory.icdoPersonAccountRetirementHistory.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled)
                    {
                        ibusHistoryAsofDate = lobjHistory;
                        break;
                    }
                }
            }*/
            //prod pir 5039 : null check added
            if (adtEffectiveDate != DateTime.MinValue && icdoPersonAccount.person_account_id > 0)
            {
                DataTable ldtbHistory = Select("cdoPersonAccountRetirementHistory.LoadHistoryForMonthYear",
                    new object[3] { icdoPersonAccount.person_account_id, adtEffectiveDate.GetFirstDayofCurrentMonth(), adtEffectiveDate.GetLastDayofMonth() });
                if (ldtbHistory.Rows.Count > 0)
                {
                    ibusHistoryAsofDate.icdoPersonAccountRetirementHistory.LoadData(ldtbHistory.Rows[0]);
                }
            }
        }

        #region PROD PIR ID 6326

        public decimal idecCapitalGain
        {
            get
            {
                if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementWithDrawn ||
                    icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirmentCancelled ||
                    icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementRetired)
                    return 0M;
                else
                    return icdoPersonAccountRetirement.capital_gain;
            }
        }

        public decimal idecPreTaxEmployeeContributionToDisplay { get; set; }

        public decimal idecPreTaxEmployeeContributionLTDToDisplay { get; set; }

        public decimal idecMemberAccountBalanceToDisplay { get; set; }

        public decimal idecMemberAccountBalanceLTDToDisplay { get; set; }

        #endregion

        //Validate Plan participation date less than employment date
        public bool IsStartDateOverlapWithEmployerOrgPlan()
        {
            //only when employment exists
            if (icdoPersonAccount.person_employment_dtl_id > 0)
            {
                if (ibusPersonEmploymentDetail == null) LoadPersonEmploymentDetail();
                if (ibusPersonEmploymentDetail.ibusPersonEmployment == null) ibusPersonEmploymentDetail.LoadPersonEmployment();
                if (ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization == null) ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
                if (ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.iclbOrgPlan == null) ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.LoadOrgPlan();

                // PROD PIR ID 6136 -- Enrollment start date cannot be earlier than plan participation start date.
                // PROD PIR ID 6788 -- Start date is editable till history record is created.
                return ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.iclbOrgPlan.Any(i => i.icdoOrgPlan.plan_id == icdoPersonAccount.plan_id &&
                                        busGlobalFunctions.CheckDateOverlapping((IsHistoryCreated() ? icdoPersonAccount.history_change_date : icdoPersonAccount.start_date), 
                                        i.icdoOrgPlan.participation_start_date, i.icdoOrgPlan.participation_end_date));
            }
            return true;
        }

        public bool IsTransferEmploymentFlag { get; set; }
        public void IsTransferEmployment()
        {
            IsTransferEmploymentFlag = false;
            if (_ibusHistory == null)
                LoadPreviousHistory();
            if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled &&
                             ibusHistory.icdoPersonAccountRetirementHistory.plan_participation_status_value == busConstant.PlanParticipationStatusRetimentSuspended)
            {
                SetHistoryEntryRequiredOrNot();
                IsTransferEmploymentFlag = true;
            }
        }

        //PIR 11901 - Visibility rule for DC Eligibility date.
        public bool VisibleRuleForDCEligibilityDate()
        {
            //PIR -13424 DC Plan Eligibility Date should not be visible to DC plans. 
            if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled )
            {
                if (((icdoPersonAccount.plan_id == busConstant.PlanIdLE || icdoPersonAccount.plan_id == busConstant.PlanIdLEWithoutPS) &&
                     ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.emp_category_value != busConstant.EmployerCategoryState) || icdoPersonAccount.plan_id == busConstant.PlanIdDC
                     || icdoPersonAccount.plan_id == busConstant.PlanIdDC2020 || icdoPersonAccount.plan_id == busConstant.PlanIdDC2025) //PIR 20232 //PIR 25920
                {
                    return false;
                }
                if ((icdoPersonAccount.plan_id == busConstant.PlanIdMain || icdoPersonAccount.plan_id == busConstant.PlanIdMain2020)//PIR 20232
                  &&
             (ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id == busConstant.NDSupremeCourtOrgId &&
                ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.job_class_value == busConstant.PersonJobClassNonClassifiedState))
                {
                    return false;
                }
                return true;
            }
            return false;
        }
        //PIR 9546 - Capital gain to be shown on pension maintenance
        public decimal idecCapitalGainContribution { get; set;  }
        public void LoadCapitalGainContribution()
        {
            DataTable ldtbList = Select<cdoPersonAccountRetirementContribution>( 
                                                                                new string[2] { "PERSON_ACCOUNT_ID", "TRANSACTION_TYPE_VALUE" }, 
                                                                                new object[2] { icdoPersonAccount.person_account_id, busConstant.TransactionTypeCapitalGain }, 
                                                                                null, null);
            if (ldtbList.Rows.Count > 0)
            {
                idecCapitalGainContribution = Convert.ToDecimal(ldtbList.Rows[0]["INTEREST_AMOUNT"] == DBNull.Value ? 0.00M : ldtbList.Rows[0]["INTEREST_AMOUNT"]);
            }
        }

        public DataTable idtbBenProvisionBenType { get; set; }
        public void CalculateAccruedBenefit(DateTime adtTerminationDate, DateTime adtRetirementDate)
        {
            busDBCacheData lobjDBCacheData = new busDBCacheData();
            lobjDBCacheData.idtbCachedBenefitProvisionEligibility = busGlobalFunctions.LoadBenefitProvisionEligibilityCacheData(iobjPassInfo);

            idtbBenProvisionBenType = busNeoSpinBase.Select("cdoBenefitProvisionBenefitType.GetAllBenefitProvision", new object[0] { });

            ibusRetirementBenefitCalculation = new busRetirementBenefitCalculation
            {
                icdoBenefitCalculation = new cdoBenefitCalculation(),
                ibusMember = new busPerson { icdoPerson = new cdoPerson() },
                ibusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount(), ibusPlan = new busPlan { icdoPlan = new cdoPlan() } },
                ibusPlan = new busPlan { icdoPlan = new cdoPlan() },
                ibusJointAnnuitant = new busPerson { icdoPerson = new cdoPerson() }

            };

            ibusRetirementBenefitCalculation.ibusMember = ibusPerson;
            ibusRetirementBenefitCalculation.LoadPersonAccount();
            ibusRetirementBenefitCalculation.LoadPlan();
            ibusRetirementBenefitCalculation.ibusPlan = ibusPlan;
            ibusRetirementBenefitCalculation.ibusPersonAccount.icdoPersonAccount = icdoPersonAccount;
            ibusRetirementBenefitCalculation.ibusPersonAccount.ibusPlan = ibusPlan;
            ibusRetirementBenefitCalculation.ibusPersonAccount.ibusPerson = ibusPerson;


            ibusRetirementBenefitCalculation.LoadJointAnnuitant();

            DataTable ldtbBenefitProvisionEligibility = busNeoSpinBase.Select<cdoBenefitProvisionEligibility>(
                          new string[0] { },
                          new object[0] { }, null, "effective_date desc");
            DataTable ldtbEligibilityForNormal = ldtbBenefitProvisionEligibility.AsEnumerable()
                                                .Where(o => o.Field<string>("benefit_account_type_value") == busConstant.ApplicationBenefitTypeRetirement
                                                            && o.Field<string>("ELIGIBILITY_TYPE_VALUE") == busConstant.BenefitProvisionEligibilityNormal)
                                                .AsDataTable();

            ibusRetirementBenefitCalculation.icdoBenefitCalculation.plan_id = ibusRetirementBenefitCalculation.ibusPlan.icdoPlan.plan_id;
            ibusRetirementBenefitCalculation.icdoBenefitCalculation.person_id = ibusRetirementBenefitCalculation.ibusMember.icdoPerson.person_id;
            ibusRetirementBenefitCalculation.icdoBenefitCalculation.rhic_option_value = busConstant.RHICOptionStandard;
            ibusRetirementBenefitCalculation.icdoBenefitCalculation.created_date = DateTime.Now;

            int lintEligibleAge = ldtbEligibilityForNormal.AsEnumerable()
                                            .Where(o => o.Field<int>("benefit_provision_id") == ibusPlan.icdoPlan.benefit_provision_id)
                                            .Select(o => o.Field<int>("age"))
                                            .FirstOrDefault();
            DateTime ldtTerminationDate = ibusPerson.icdoPerson.date_of_birth.AddYears(lintEligibleAge).GetLastDayofMonth();

            ibusRetirementBenefitCalculation.icdoBenefitCalculation.termination_date = adtTerminationDate;
            ibusRetirementBenefitCalculation.icdoBenefitCalculation.retirement_date = adtRetirementDate;
            ibusRetirementBenefitCalculation.icdoBenefitCalculation.benefit_account_type_value = busConstant.ApplicationBenefitTypeRetirement;
            
            ibusRetirementBenefitCalculation.icdoBenefitCalculation.calculation_type_value = busConstant.CalculationTypeFinal;
            
            ibusRetirementBenefitCalculation.CalculateMemberAge();
            ibusRetirementBenefitCalculation.LoadBenefitCalculationPayeeForNewMode();
            ibusRetirementBenefitCalculation.LoadBenefitProvisionBenefitType(idtbBenProvisionBenType);
            ibusRetirementBenefitCalculation.ibusPersonAccount.LoadTotalVSC();
            if (ibusRetirementBenefitCalculation.icdoBenefitCalculation.plan_id == busConstant.PlanIdDC ||
                ibusRetirementBenefitCalculation.icdoBenefitCalculation.plan_id == busConstant.PlanIdDC2020 || //PIR 20232
                ibusRetirementBenefitCalculation.icdoBenefitCalculation.plan_id == busConstant.PlanIdDC2025) //PIR 25920
            {
                ibusRetirementBenefitCalculation.icdoBenefitCalculation.benefit_account_type_value = busConstant.ApplicationBenefitTypeRetirement;
                if (!ibusRetirementBenefitCalculation.iblnConsolidatedPSCLoaded)
                    ibusRetirementBenefitCalculation.CalculateConsolidatedPSC();
                decimal ldecRHICAmount = Math.Round(ibusRetirementBenefitCalculation.icdoBenefitCalculation.consolidated_psc_in_years, 4, MidpointRounding.AwayFromZero) *
                                        ibusRetirementBenefitCalculation.ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.rhic_service_factor;
                ibusRetirementBenefitCalculation.icdoBenefitCalculation.unreduced_rhic_amount = Math.Round(ldecRHICAmount, 2, MidpointRounding.AwayFromZero);

            }
            else
            {
                ibusRetirementBenefitCalculation.icdoBenefitCalculation.benefit_account_type_value = busConstant.ApplicationBenefitTypeRetirement;
                if (!ibusRetirementBenefitCalculation.iblnConsoldatedVSCLoaded)
                    ibusRetirementBenefitCalculation.CalculateConsolidatedVSC();
                if (ibusRetirementBenefitCalculation.CheckPersonEligible(lobjDBCacheData))
                {
                    ibusRetirementBenefitCalculation.CalculateFAS();
                    ibusRetirementBenefitCalculation.CalculateRetirementBenefit();
                }
            }
        }

        //PIR 13815 - Overlap functionality
        public Collection<busPersonAccountRetirementHistory> iclbOverlappingHistory { get; set; }
        private Collection<busPersonAccountRetirementHistory> LoadOverlappingHistory()
        {
            if (icolPersonAccountRetirementHistory == null)
                LoadPersonAccountRetirementHistory();
            Collection<busPersonAccountRetirementHistory> lclbPersonAccountRetirementHistory = new Collection<busPersonAccountRetirementHistory>();
            var lenuList = icolPersonAccountRetirementHistory.Where(i => busGlobalFunctions.CheckDateOverlapping(icdoPersonAccount.history_change_date,
                i.icdoPersonAccountRetirementHistory.start_date, i.icdoPersonAccountRetirementHistory.end_date)
                || i.icdoPersonAccountRetirementHistory.start_date > icdoPersonAccount.history_change_date);

            foreach (busPersonAccountRetirementHistory lobjPARetirementHistory in lenuList)
            {
                if (lobjPARetirementHistory.icdoPersonAccountRetirementHistory.start_date >= icdoPersonAccount.history_change_date)
                {
                    lclbPersonAccountRetirementHistory.Add(lobjPARetirementHistory);
                }
                else if (lobjPARetirementHistory.icdoPersonAccountRetirementHistory.start_date == lobjPARetirementHistory.icdoPersonAccountRetirementHistory.end_date)
                {
                    lclbPersonAccountRetirementHistory.Add(lobjPARetirementHistory);
                }
                else if (lobjPARetirementHistory.icdoPersonAccountRetirementHistory.start_date != lobjPARetirementHistory.icdoPersonAccountRetirementHistory.end_date)
                {
                    break;
                }
            }
            return lclbPersonAccountRetirementHistory;

        }
        
        public bool IsMoreThanOneEnrolledInOverlapHistory()
        {
            if (istrAllowOverlapHistory == busConstant.Flag_Yes)
            {
                if (iclbOverlappingHistory != null)
                {
                    var lenuList = iclbOverlappingHistory.Where(i => i.icdoPersonAccountRetirementHistory.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled &&
                        i.icdoPersonAccountRetirementHistory.start_date != i.icdoPersonAccountRetirementHistory.end_date);
                    //PIR Enhanced Overlap - 23167, 23340, 23408
                    if (IsResourceForEnhancedOverlap(busConstant.OverlapResourceEnhancedRetirement))
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

        public bool IsResourceEligibleForInitialDate
        {
            get
            {
                bool lblnHasInitialDateRole = false;

                DataTable ldtbCount = busNeoSpinBase.Select("cdoPersonAccountRetirement.CheckResourceForLoggedInUser", new object[1] { iobjPassInfo.iintUserSerialID });
                if (ldtbCount.Rows.Count > 0)
                {
                    lblnHasInitialDateRole = true;
                }
                return lblnHasInitialDateRole;
            }

        }

        public bool IsInitialStartDateGreaterThanHistory()
        {
            bool istrResult = false;
            if (idtInitialStartDate != DateTime.MinValue)
            {
                if (icolPersonAccountRetirementHistory == null)
                    LoadPersonAccountRetirementHistory();

                if (icolPersonAccountRetirementHistory.Count() > 0)
                {
                    String lstrOrderbyClause = "icdoPersonAccountRetirementHistory.start_date asc,icdoPersonAccountRetirementHistory.person_account_retirement_history_id asc";
                    icolPersonAccountRetirementHistory = busGlobalFunctions.Sort<busPersonAccountRetirementHistory>(lstrOrderbyClause, icolPersonAccountRetirementHistory);

                    Collection<busPersonAccountRetirementHistory> lclbRetirementHistory = icolPersonAccountRetirementHistory.Where(i => i.icdoPersonAccountRetirementHistory.start_date != i.icdoPersonAccountRetirementHistory.end_date).ToList().ToCollection();
                    if(lclbRetirementHistory.Count > 0)
                    { 
                        if (lclbRetirementHistory[0].icdoPersonAccountRetirementHistory.end_date == DateTime.MinValue)
                            lclbRetirementHistory[0].icdoPersonAccountRetirementHistory.end_date = DateTime.MaxValue;

                        if (idtInitialStartDate < lclbRetirementHistory[0].icdoPersonAccountRetirementHistory.start_date &&
                            idtInitialStartDate < lclbRetirementHistory[0].icdoPersonAccountRetirementHistory.end_date)
                            istrResult = true;

                        else if (busGlobalFunctions.CheckDateOverlapping(idtInitialStartDate, lclbRetirementHistory[0].icdoPersonAccountRetirementHistory.start_date, lclbRetirementHistory[0].icdoPersonAccountRetirementHistory.end_date))
                            istrResult = true;
                    }
                    LoadPersonAccountRetirementHistory();//To reset the order of history records
                }
            }

            return istrResult;
        }

        public bool IsInitialStartDateChanged()
        {
            bool istrResult = false;
            if (idtInitialStartDate != DateTime.MinValue)
            {
                if (icolPersonAccountRetirementHistory == null)
                    LoadPersonAccountRetirementHistory();

                if (icolPersonAccountRetirementHistory.Count() > 0)
                {
                    String lstrOrderbyClause = "icdoPersonAccountRetirementHistory.start_date asc,icdoPersonAccountRetirementHistory.person_account_retirement_history_id asc";
                    icolPersonAccountRetirementHistory = busGlobalFunctions.Sort<busPersonAccountRetirementHistory>(lstrOrderbyClause, icolPersonAccountRetirementHistory);

                    Collection<busPersonAccountRetirementHistory> lclbRetirementHistory = icolPersonAccountRetirementHistory.Where(i => i.icdoPersonAccountRetirementHistory.start_date != i.icdoPersonAccountRetirementHistory.end_date).ToList().ToCollection();

                    if (lclbRetirementHistory[0].icdoPersonAccountRetirementHistory.start_date != idtInitialStartDate)
                        istrResult = true;
                    
                    LoadPersonAccountRetirementHistory();//To reset the order of history records
                }

            }
            return istrResult;
        }
        public void CalculateRetirementBenefitAmountForMSS()
        {
            busRetirementBenefitCalculation lbusRetirementBenefitCalculation = new busRetirementBenefitCalculation();
            lbusRetirementBenefitCalculation.icdoBenefitCalculation = new cdoBenefitCalculation();
            busRetirementBenefitCalculation lbusDisabilityBenefitCalculation = new busRetirementBenefitCalculation();
            lbusDisabilityBenefitCalculation.icdoBenefitCalculation = new cdoBenefitCalculation();

            lbusDisabilityBenefitCalculation.ibusMember = ibusPerson;
            lbusDisabilityBenefitCalculation.ibusPlan = ibusPlan;
            lbusDisabilityBenefitCalculation.ibusPersonAccount = ibusPersonAccount;

            busDBCacheData lobjDBCacheData = new busDBCacheData();
            lobjDBCacheData.idtbCachedBenefitProvisionEligibility = busGlobalFunctions.LoadBenefitProvisionEligibilityCacheData(iobjPassInfo);

            idtbBenProvisionBenType = busNeoSpinBase.Select("cdoBenefitProvisionBenefitType.GetAllBenefitProvision", new object[0] { });



            lbusRetirementBenefitCalculation.ibusMember = ibusPerson;
            lbusRetirementBenefitCalculation.LoadPersonAccount();
            lbusRetirementBenefitCalculation.LoadPlan();
            lbusRetirementBenefitCalculation.ibusPlan = ibusPlan;
            lbusRetirementBenefitCalculation.ibusPersonAccount.icdoPersonAccount = icdoPersonAccount;
            lbusRetirementBenefitCalculation.ibusPersonAccount.ibusPlan = ibusPlan;
            lbusRetirementBenefitCalculation.ibusPersonAccount.ibusPerson = ibusPerson;
            lbusRetirementBenefitCalculation.ibusPersonAccount.iblnLoadOnlyRequiredFieldsForVSCOrPSC = true;
            lbusRetirementBenefitCalculation.LoadJointAnnuitant();

            DataTable ldtbBenefitProvisionEligibility = busNeoSpinBase.Select<cdoBenefitProvisionEligibility>(
                          new string[0] { },
                          new object[0] { }, null, "effective_date desc");
            DataTable ldtbEligibilityForNormal = ldtbBenefitProvisionEligibility.AsEnumerable()
                                                .Where(o => o.Field<string>("benefit_account_type_value") == busConstant.ApplicationBenefitTypeRetirement
                                                            && o.Field<string>("ELIGIBILITY_TYPE_VALUE") == busConstant.BenefitProvisionEligibilityNormal)
                                                .AsDataTable();

            lbusRetirementBenefitCalculation.icdoBenefitCalculation.plan_id = lbusRetirementBenefitCalculation.ibusPlan.icdoPlan.plan_id;
            lbusRetirementBenefitCalculation.icdoBenefitCalculation.person_id = lbusRetirementBenefitCalculation.ibusMember.icdoPerson.person_id;
            lbusRetirementBenefitCalculation.icdoBenefitCalculation.rhic_option_value = busConstant.RHICOptionStandard;
            lbusRetirementBenefitCalculation.icdoBenefitCalculation.created_date = DateTime.Now;

            int lintEligibleAge = ldtbEligibilityForNormal.AsEnumerable()
                                            .Where(o => o.Field<int>("benefit_provision_id") == ibusPlan.icdoPlan.benefit_provision_id)
                                            .Select(o => o.Field<int>("age"))
                                            .FirstOrDefault();
            DateTime ldtAgeTerminationDate = ibusPerson.icdoPerson.date_of_birth.AddYears(lintEligibleAge).GetLastDayofMonth();
            lbusRetirementBenefitCalculation.icdoBenefitCalculation.benefit_account_type_value = busConstant.ApplicationBenefitTypeRetirement;
            lbusRetirementBenefitCalculation.icdoBenefitCalculation.calculation_type_value = busConstant.CalculationTypeFinal;


            lbusDisabilityBenefitCalculation.icdoBenefitCalculation.benefit_calculation_id = 0;
            lbusDisabilityBenefitCalculation.icdoBenefitCalculation.calculation_type_value = busConstant.CalculationTypeFinal;
            lbusDisabilityBenefitCalculation.icdoBenefitCalculation.created_date = DateTime.Now;
            lbusDisabilityBenefitCalculation.icdoBenefitCalculation.person_id = icdoPersonAccount.person_id;
            lbusDisabilityBenefitCalculation.icdoBenefitCalculation.plan_id = icdoPersonAccount.plan_id;
            lbusDisabilityBenefitCalculation.icdoBenefitCalculation.benefit_account_type_value = busConstant.ApplicationBenefitTypeDisability;
            lbusDisabilityBenefitCalculation.icdoBenefitCalculation.benefit_option_value = string.Empty;
            lbusDisabilityBenefitCalculation.icdoBenefitCalculation.uniform_income_or_ssli_flag = "N";


            lbusDisabilityBenefitCalculation.ibusPersonAccount.idtRetirementContributionAll = idtRetirementContributionAll;
            lbusDisabilityBenefitCalculation.ibusPersonAccount.iclbRetirementContributionAll = iclbRetirementContributionAll;

            lbusRetirementBenefitCalculation.ibusPersonAccount.idtRetirementContributionAll = idtRetirementContributionAll;
            lbusRetirementBenefitCalculation.ibusPersonAccount.iclbRetirementContributionAll = iclbRetirementContributionAll;

            ibusPerson.LoadPersonAccountByBenefitType(busConstant.PlanBenefitTypeRetirement, true, false);
            ibusPerson.icolPersonAccountByBenefitType.ForEach(a => a.iblnLoadOnlyRequiredFieldsForVSCOrPSC = true);

            lbusDisabilityBenefitCalculation.ibusMember.icolPersonAccountByBenefitType = ibusPerson.icolPersonAccountByBenefitType;
            lbusRetirementBenefitCalculation.ibusMember.icolPersonAccountByBenefitType = ibusPerson.icolPersonAccountByBenefitType;


            if (iclbRetirementContributionAll != null && iclbRetirementContributionAll.Count > 0)
            {
                iclbRetirementContributionAll = busGlobalFunctions.Sort<busPersonAccountRetirementContribution>(
                        "icdoPersonAccountRetirementContribution.PayPeriodYearMonth_no_null desc",
                         iclbRetirementContributionAll);

                foreach (busPersonAccountRetirementContribution lobjRetirementContribution in iclbRetirementContributionAll)
                {
                    if ((lobjRetirementContribution.icdoPersonAccountRetirementContribution.subsystem_value == busConstant.SubSystemValueEmployerReporting)
                        || (lobjRetirementContribution.icdoPersonAccountRetirementContribution.subsystem_value == busConstant.SubSystemValueConversion)
                        || (lobjRetirementContribution.icdoPersonAccountRetirementContribution.subsystem_value == busConstant.SubSystemValueConversionOpeningBalance))
                    {
                        DateTime ldtTerminationDate = DateTime.MinValue;
                        if (lobjRetirementContribution.icdoPersonAccountRetirementContribution.subsystem_value == busConstant.SubSystemValueConversionOpeningBalance)
                        {
                            ldtTerminationDate = new DateTime(lobjRetirementContribution.icdoPersonAccountRetirementContribution.effective_date.Year,
                                         lobjRetirementContribution.icdoPersonAccountRetirementContribution.effective_date.Month,
                                         DateTime.DaysInMonth(lobjRetirementContribution.icdoPersonAccountRetirementContribution.effective_date.Year,
                                         lobjRetirementContribution.icdoPersonAccountRetirementContribution.effective_date.Month));
                        }
                        else
                        {
                            ldtTerminationDate = new DateTime(lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_year,
                                            lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_month,
                                            DateTime.DaysInMonth(lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_year,
                                            lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_month));
                        }
                        DateTime ldtActualTerminationDate = ldtTerminationDate.AddDays(1);
                        lbusRetirementBenefitCalculation.icdoBenefitCalculation.termination_date = ldtActualTerminationDate;
                        lbusDisabilityBenefitCalculation.icdoBenefitCalculation.termination_date = ldtActualTerminationDate;
                        if (ibusPersonAccount.ibusPlan.IsNull())
                            ibusPersonAccount.LoadPlan();

                        lbusRetirementBenefitCalculation.GetNormalRetirementDateBasedOnNormalEligibility(ibusPersonAccount.ibusPlan.icdoPlan.plan_code, null);
                        DateTime ldtRetirementdate = lbusRetirementBenefitCalculation.icdoBenefitCalculation.normal_retirement_date;

                        if (ldtRetirementdate != DateTime.MinValue)
                        {
                            lbusRetirementBenefitCalculation.icdoBenefitCalculation.retirement_date = ldtRetirementdate;
                            lbusDisabilityBenefitCalculation.icdoBenefitCalculation.retirement_date = ldtRetirementdate;
                        }
                        else
                        {
                            lbusRetirementBenefitCalculation.icdoBenefitCalculation.retirement_date = DateTime.Now;
                        }
                        break;
                    }
                }
            }
            if (lbusRetirementBenefitCalculation.icdoBenefitCalculation.termination_date == DateTime.MinValue)
            {
                lbusRetirementBenefitCalculation.icdoBenefitCalculation.termination_date = DateTime.Now.AddMonths(-1);
                lbusRetirementBenefitCalculation.icdoBenefitCalculation.retirement_date = DateTime.Now;
            }
            if (lbusDisabilityBenefitCalculation.icdoBenefitCalculation.termination_date == DateTime.MinValue)
            {
                lbusDisabilityBenefitCalculation.icdoBenefitCalculation.termination_date = DateTime.Now.AddMonths(-1);
                lbusDisabilityBenefitCalculation.icdoBenefitCalculation.retirement_date = DateTime.Now;
            }
            if (lbusRetirementBenefitCalculation.icdoBenefitCalculation.termination_date > lbusRetirementBenefitCalculation.icdoBenefitCalculation.retirement_date)
                lbusRetirementBenefitCalculation.icdoBenefitCalculation.retirement_date = lbusRetirementBenefitCalculation.icdoBenefitCalculation.termination_date;

            if (lbusDisabilityBenefitCalculation.icdoBenefitCalculation.termination_date > lbusDisabilityBenefitCalculation.icdoBenefitCalculation.retirement_date)
                lbusDisabilityBenefitCalculation.icdoBenefitCalculation.retirement_date = lbusDisabilityBenefitCalculation.icdoBenefitCalculation.termination_date;

            lbusDisabilityBenefitCalculation.CalculateMemberAge();
            if (!lbusDisabilityBenefitCalculation.iblnConsoldatedVSCLoaded)
                lbusDisabilityBenefitCalculation.CalculateConsolidatedVSC();
            if (lbusDisabilityBenefitCalculation.IsPersonEligible())
                lbusDisabilityBenefitCalculation.CalculateRetirementBenefit();

            lbusRetirementBenefitCalculation.CalculateMemberAge();
            lbusRetirementBenefitCalculation.LoadBenefitCalculationPayeeForNewMode();
            lbusRetirementBenefitCalculation.LoadBenefitProvisionBenefitType(idtbBenProvisionBenType);
            lbusRetirementBenefitCalculation.ibusPersonAccount.LoadTotalVSC();
            if (lbusRetirementBenefitCalculation.icdoBenefitCalculation.plan_id == busConstant.PlanIdDC ||
                lbusRetirementBenefitCalculation.icdoBenefitCalculation.plan_id == busConstant.PlanIdDC2020 || //PIR 20232IsPersonPriorEnrolledInDC
                lbusRetirementBenefitCalculation.icdoBenefitCalculation.plan_id == busConstant.PlanIdDC2025) //PIR 25920
            {
                lbusRetirementBenefitCalculation.icdoBenefitCalculation.benefit_account_type_value = busConstant.ApplicationBenefitTypeRetirement;
                if (!lbusRetirementBenefitCalculation.iblnConsolidatedPSCLoaded)
                    lbusRetirementBenefitCalculation.CalculateConsolidatedPSC();
                decimal ldecRHICAmount = Math.Round(lbusRetirementBenefitCalculation.icdoBenefitCalculation.consolidated_psc_in_years, 4, MidpointRounding.AwayFromZero) *
                                        lbusRetirementBenefitCalculation.ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.rhic_service_factor;
                lbusRetirementBenefitCalculation.icdoBenefitCalculation.unreduced_rhic_amount = Math.Round(ldecRHICAmount, 2, MidpointRounding.AwayFromZero);

            }
            else
            {
                lbusRetirementBenefitCalculation.icdoBenefitCalculation.benefit_account_type_value = busConstant.ApplicationBenefitTypeRetirement;
                if (!lbusRetirementBenefitCalculation.iblnConsoldatedVSCLoaded)
                    lbusRetirementBenefitCalculation.CalculateConsolidatedVSC();
                if (lbusRetirementBenefitCalculation.CheckPersonEligible(lobjDBCacheData))
                {
                    lbusRetirementBenefitCalculation.CalculateFAS();
                    lbusRetirementBenefitCalculation.CalculateRetirementBenefit();
                }
            }
            idecFAS = lbusRetirementBenefitCalculation.icdoBenefitCalculation.computed_final_average_salary == 0 ?
                lbusDisabilityBenefitCalculation.icdoBenefitCalculation.computed_final_average_salary :
                lbusRetirementBenefitCalculation.icdoBenefitCalculation.computed_final_average_salary;
            idecAccruedBenefit = lbusRetirementBenefitCalculation.icdoBenefitCalculation.unreduced_benefit_amount;
            idecDisability = lbusDisabilityBenefitCalculation.icdoBenefitCalculation.unreduced_benefit_amount;
            idecAccruedRHICBenefit = lbusRetirementBenefitCalculation.idecReducedRHICAmount == 0 ?
                lbusDisabilityBenefitCalculation.idecReducedRHICAmount :
                lbusRetirementBenefitCalculation.idecReducedRHICAmount;

        }
        /// <summary>
        /// PIR 20641 - DC enrollment in prior employments for org id 463 with job class NCLS
        /// </summary>
        /// <returns></returns>
        public bool IsPersonPriorEnrolledInDC()
        {
            bool lblnpersonAlreadyEnrolledInDC = false;
            if ((icdoPersonAccount.plan_id == busConstant.PlanIdDC || icdoPersonAccount.plan_id == busConstant.PlanIdDC2020 //PIR 20232
                //|| icdoPersonAccount.plan_id == busConstant.PlanIdDC2025
                ) //PIR 25920
                && icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled
                && icdoPersonAccount.person_employment_dtl_id > 0)
            {
                busPersonEmploymentDetail lbusPersonCurrentEmpDetail = new busPersonEmploymentDetail() { icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail() };
                lbusPersonCurrentEmpDetail.FindPersonEmploymentDetail(icdoPersonAccount.person_employment_dtl_id);
                lbusPersonCurrentEmpDetail.LoadPersonEmployment();
                if (lbusPersonCurrentEmpDetail.ibusPersonEmployment.icdoPersonEmployment.org_id == busConstant.NDSupremeCourtOrgId
                    && lbusPersonCurrentEmpDetail.icdoPersonEmploymentDetail.job_class_value == busConstant.PersonJobClassNonClassifiedState)
                {
                    string lstrIsEnrolledInPriorDC = string.Empty;
                    DataTable ldtbDBorDC = busBase.Select("cdoPersonAccountRetirement.IsPersonPriorEnrolledInDC", new object[1] { ibusPerson.icdoPerson.person_id });
                    lstrIsEnrolledInPriorDC = ldtbDBorDC.Rows[0]["DC_ENROLLED"].ToString();
                    if (lstrIsEnrolledInPriorDC == "N")
                    {
                        return true;
                    }
                }
            }
            return lblnpersonAlreadyEnrolledInDC;
        }
        public string istrEligibilityCodeForFile
        {
            get
            {
                return " ";
            }
        }
        public DateTime idtmReHireDateForFile
        {
            get
            {
                return DateTime.MinValue;
            }
        }
        public DateTime idtmCheckDateForFile
        {
            get
            {
                return DateTime.Today;
            }
        }
        //PIR 25920 DC 2025 Changes get the lastest Employment Details for DC 2025 validations 
        //PIR 25920 New DC Plan ibusCurrentPersonEmploymentDetail is used only for DC 2025 validations purpose cause the list will be refreshed after date enter.
        /// <summary>
        /// Load ADEC Amount for current employment 
        /// </summary>
        /// <returns></returns>
        public void LoadCurrentPersonEmploymentDetail()
        {
			if (ibusPersonEmploymentDetail.IsNull())
            {
                if (icdoPersonAccount.person_employment_dtl_id == 0) icdoPersonAccount.person_employment_dtl_id = GetEmploymentDetailID();
                LoadPersonEmploymentDetail();
            }
            ibusCurrentPersonEmploymentDetail = ibusPersonEmploymentDetail;
            //busPersonEmploymentDetail lbusPersonEmploymentDetail = GetLatestEmploymentDetail();
            //if (lbusPersonEmploymentDetail.IsNotNull() && lbusPersonEmploymentDetail.icdoPersonEmploymentDetail.IsNotNull() &&
            //    lbusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id > 0 &&
            //    lbusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id != ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id)
            //{
            //    ibusCurrentPersonEmploymentDetail = lbusPersonEmploymentDetail;
            //}
            if (ibusCurrentPersonEmploymentDetail.ibusPersonEmployment.IsNull()) ibusCurrentPersonEmploymentDetail.LoadPersonEmployment();
            ibusCurrentPersonEmploymentDetail.LoadMemberType(icblPersonAccount);
        }
        //PIR 25920 Droplist Refresh from DatePicker
        /// <summary>
        /// Load ADEC Amount for input history change date , function called from Javascript and before validate the screen
        /// </summary>
        /// <returns></returns>
        public Collection<cdoCodeValue>  GetADECAmountValuesByEffectiveDate(DateTime astrHistoryChangeDate, int aintPersonEmploymentDtlId, int aintPersonAccountId)
        {
            if (icdoPersonAccount.plan_id == busConstant.PlanIdDC2025)
            {
                busPersonEmploymentDetail lbusPersonEmploymentDetail = new busPersonEmploymentDetail();
                lbusPersonEmploymentDetail.FindPersonEmploymentDetail(aintPersonEmploymentDtlId);
                lbusPersonEmploymentDetail.LoadPersonEmployment();
                lbusPersonEmploymentDetail.ibusPersonEmployment.LoadPersonEmploymentDetail();
                if (icdoPersonAccount.IsNull()) icdoPersonAccount = new cdoPersonAccount();
                //lbusPersonEmploymentDetail.ibusPersonEmployment.icolPersonEmploymentDetail();
                busPersonEmploymentDetail lobjPersonEmploymentdtl = lbusPersonEmploymentDetail.ibusPersonEmployment.icolPersonEmploymentDetail
                                                                           .Where(lobjDet => busGlobalFunctions.CheckDateOverlapping(astrHistoryChangeDate,
                                                                           lobjDet.icdoPersonEmploymentDetail.start_date, lobjDet.icdoPersonEmploymentDetail.end_date_no_null))
                                                                           .FirstOrDefault();
                if (lobjPersonEmploymentdtl.IsNotNull())
                {
                    icdoPersonAccount.plan_id = busConstant.PlanIdDC2025;
                    icdoPersonAccount.person_employment_dtl_id = lobjPersonEmploymentdtl.icdoPersonEmploymentDetail.person_employment_dtl_id;
                    ibusCurrentPersonEmploymentDetail = lobjPersonEmploymentdtl;
                    if (ibusCurrentPersonEmploymentDetail.ibusPersonEmployment.IsNull()) ibusCurrentPersonEmploymentDetail.LoadPersonEmployment();
                    ibusCurrentPersonEmploymentDetail.LoadMemberType(icblPersonAccount);
                    ibusPersonEmploymentDetail = ibusCurrentPersonEmploymentDetail;
                }

                LoadADECAmountValues();
                if(iobjPassInfo.istrPostBackControlID.StartsWith("btnGetADECValues"))
                    LoadADECAmountZero();
            }
            return iclbEligibleADECAmountValue;
        }

        //PIR 25920 DC 2025 Changes load zero amount 
        public void LoadADECAmountZero()
        {
            DataTable ldtPersonAccount = Select<cdoPersonAccount>(new string[1] { enmPersonAccount.person_account_id.ToString() },
                                       new object[1] { icdoPersonAccount.person_account_id }, null, null);
            if (ldtPersonAccount.IsNotNull() && ldtPersonAccount.Rows.Count > 0)
            {
                if (ibusCurrentPersonEmploymentDetail.IsNull()) LoadCurrentPersonEmploymentDetail();
                
                if (Convert.ToString(ldtPersonAccount.Rows[0][enmPersonAccount.addl_ee_contribution_percent_temp.ToString()]) == "0")
                    icdoPersonAccount.istrAddl_ee_contribution_percent_Temp_zero = Convert.ToString(ldtPersonAccount.Rows[0][enmPersonAccount.addl_ee_contribution_percent_temp.ToString()]);
                else
                    icdoPersonAccount.istrAddl_ee_contribution_percent_Temp_zero = string.Empty;
                if (Convert.ToString(ldtPersonAccount.Rows[0][enmPersonAccount.addl_ee_contribution_percent.ToString()]) == "0")
                    icdoPersonAccount.istrAddl_ee_contribution_percent_Perm_zero = Convert.ToString(ldtPersonAccount.Rows[0][enmPersonAccount.addl_ee_contribution_percent.ToString()]);
                else
                    icdoPersonAccount.istrAddl_ee_contribution_percent_Perm_zero = string.Empty;
                if (ibusCurrentPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary)
                {
                    iintAddlEEContributionPercent = icdoPersonAccount.addl_ee_contribution_percent_temp;
                    if (Convert.ToString(ldtPersonAccount.Rows[0][enmPersonAccount.addl_ee_contribution_percent_temp.ToString()]) == "0")
                    {
                        icdoPersonAccount.istrAddl_ee_contribution_percent_zero = Convert.ToString(ldtPersonAccount.Rows[0][enmPersonAccount.addl_ee_contribution_percent_temp.ToString()]);
                        iintAddlEEContributionPercent = -1;
                    }
                    if (Convert.ToString(ldtPersonAccount.Rows[0][enmPersonAccount.addl_ee_contribution_percent_temp.ToString()]) == string.Empty)
                        icdoPersonAccount.iblnIs_Addl_ee_contribution_percent_NULL = true;
                    else
                    {
                        icdoPersonAccount.iblnIs_Addl_ee_contribution_percent_NULL = false;
                        icdoPersonAccount.addl_ee_contribution_percent_temp = Convert.ToInt32(Convert.ToString(ldtPersonAccount.Rows[0][enmPersonAccount.addl_ee_contribution_percent_temp.ToString()]));
                    }
                }
                else
                {
                    iintAddlEEContributionPercent = icdoPersonAccount.addl_ee_contribution_percent;
                    if (Convert.ToString(ldtPersonAccount.Rows[0][enmPersonAccount.addl_ee_contribution_percent.ToString()]) == "0")
                    {
                        icdoPersonAccount.istrAddl_ee_contribution_percent_zero = Convert.ToString(ldtPersonAccount.Rows[0][enmPersonAccount.addl_ee_contribution_percent.ToString()]);
                        iintAddlEEContributionPercent = -1;
                    }
                    if (Convert.ToString(ldtPersonAccount.Rows[0][enmPersonAccount.addl_ee_contribution_percent.ToString()]) == string.Empty)
                        icdoPersonAccount.iblnIs_Addl_ee_contribution_percent_NULL = true;
                    else
                    {
                        icdoPersonAccount.iblnIs_Addl_ee_contribution_percent_NULL = false;
                        icdoPersonAccount.addl_ee_contribution_percent = Convert.ToInt32(Convert.ToString(ldtPersonAccount.Rows[0][enmPersonAccount.addl_ee_contribution_percent.ToString()]));
                    }
                }
            }
        }
        
        
        //PIR 25920 New DC Plan
        /// <summary>
        /// Load ADEC Amount values 0-3 or 0-6
        /// </summary>
        /// <returns></returns>
        public Collection<cdoCodeValue> LoadADECAmountValues()
        {
            decimal ldecEEPreTaxAddlAmount = 0.00m;
            string lstrMemberType = string.Empty;
            DataTable ldtbPlanRetirementRate = new DataTable();
            DataTable ldtbPersonAccountEmploymentDetail = new DataTable();
            //Collection<cdoCodeValue> lclbEligibleADECAmountValue = new Collection<cdoCodeValue>();
            if (ibusCurrentPersonEmploymentDetail.IsNull()) LoadCurrentPersonEmploymentDetail();
            iclbEligibleADECAmountValue.Clear();

            busPersonAccountEmploymentDetail lbusPersonAccountEmploymentDetail = new busPersonAccountEmploymentDetail();
            ldtbPersonAccountEmploymentDetail = Select<cdoPersonAccountEmploymentDetail>(new string[2] { "PLAN_ID", "PERSON_EMPLOYMENT_DTL_ID" }, new object[2] 
            { icdoPersonAccount.plan_id, ibusCurrentPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id }, null, "person_employment_dtl_id DESC");
            if (ldtbPersonAccountEmploymentDetail.IsNotNull() && ldtbPersonAccountEmploymentDetail.Rows.Count > 0)
            {
                lbusPersonAccountEmploymentDetail = GetCollection<busPersonAccountEmploymentDetail>(ldtbPersonAccountEmploymentDetail, "icdoPersonAccountEmploymentDetail").First();
            }
            
            if (ibusCurrentPersonEmploymentDetail.IsNotNull() && ibusCurrentPersonEmploymentDetail.icdoPersonEmploymentDetail.derived_member_type_value.IsNull()
                 && lbusPersonAccountEmploymentDetail.IsNotNull())
            {
                if (lbusPersonAccountEmploymentDetail.ibusPersonAccount.IsNull())
                {
                    lbusPersonAccountEmploymentDetail.ibusPersonAccount = new busPersonAccount();
                    lbusPersonAccountEmploymentDetail.ibusPersonAccount.icdoPersonAccount = icdoPersonAccount;
                }
                ibusCurrentPersonEmploymentDetail.LoadMemberType(ibusCurrentPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date, lbusPersonAccountEmploymentDetail);
            }
            lstrMemberType = ibusCurrentPersonEmploymentDetail.icdoPersonEmploymentDetail.derived_member_type_value.IsNull() ? string.Empty : ibusCurrentPersonEmploymentDetail.icdoPersonEmploymentDetail.derived_member_type_value;
            busPlanRetirementRate lbusPlanRetirementRate = busRateHelper.GetRatesForMemberTypeAndEffectiveDate(lstrMemberType, ibusCurrentPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date, icdoPersonAccount.plan_id);
            ldecEEPreTaxAddlAmount = lbusPlanRetirementRate.icdoPlanRetirementRate.ee_pretax_addl + lbusPlanRetirementRate.icdoPlanRetirementRate.ee_post_tax_addl;
            for (int i = 0; i <= ldecEEPreTaxAddlAmount; i++)
            {
                if(ldecEEPreTaxAddlAmount!=0)
                {
                    if (i == 0)
                        iclbEligibleADECAmountValue.Add(new cdoCodeValue { code_value = Convert.ToString(i), code_id = -1 });
                    else
                        iclbEligibleADECAmountValue.Add(new cdoCodeValue { code_value = Convert.ToString(i), code_id = i });
                }
            }
            return iclbEligibleADECAmountValue;
        }
        /// <summary>
        /// return the number of days allowed to change the additional contribution election  Temp -180, Perm- 30
        /// PIR 25920 New Plan DC 2025
        /// </summary>
        public int iintDaysDiffByEmploymentype
        {
            get
            {
                if (ibusCurrentPersonEmploymentDetail.IsNull()) LoadCurrentPersonEmploymentDetail();
                return ibusCurrentPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary ? 180 : 30;
            }
        }
        /// <summary>
        /// return true if contribution is null or employment detail start date within limit of 30 days 
        /// PIR 25920 New Plan DC 2025
        /// </summary>
        public bool iblnIsAllowAdditionalElectionChange
        {
            get
            {
                if(iobjPassInfo.istrFormName == "wfmPensionPlanMaintenance" && lblnIsVisibleADECAmountList && !IsAllowChangeADECOnEnroll())
                {
                    if (ibusCurrentPersonEmploymentDetail.IsNull()) LoadCurrentPersonEmploymentDetail();
                    bool lblnReturnTrue = false;
                    //busPersonEmploymentDetail lbusValidateDateRangePersonEmploymentDetail = new busPersonEmploymentDetail();
                    //lbusValidateDateRangePersonEmploymentDetail = ibusCurrentPersonEmploymentDetail.LoadEarliestPermEmploymentDetail(ibusCurrentPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.person_id, icdoPersonAccount.plan_id);
                    //if (lbusValidateDateRangePersonEmploymentDetail.icdoPersonEmploymentDetail.IsNull()) lbusValidateDateRangePersonEmploymentDetail.icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail();
                    //int lintEmploymentDaysDiff = busGlobalFunctions.DateDiffInDays(lbusValidateDateRangePersonEmploymentDetail.icdoPersonEmploymentDetail.start_date, DateTime.Today);
                    int lintEmploymentDaysDiff;
                    DateTime ldtAddlEEContributionEndDate = ibusCurrentPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary ? 
                        icdoPersonAccount.addl_ee_contribution_percent_temp_end_date : icdoPersonAccount.addl_ee_contribution_percent_end_date;
                    if (ldtAddlEEContributionEndDate == DateTime.MinValue)
                    {
                        lintEmploymentDaysDiff = busGlobalFunctions.DateDiffInDays(ibusCurrentPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date, busGlobalFunctions.GetSysManagementBatchDate());
                        lblnReturnTrue = lintEmploymentDaysDiff < iintDaysDiffByEmploymentype;
                    }
                    else
                    {
                        lblnReturnTrue = busGlobalFunctions.GetSysManagementBatchDate() < ldtAddlEEContributionEndDate;
                    }
                    return lblnReturnTrue;
                }
                return true;
            }
        }



        /// <summary>
        /// return true if contribution is already exist or made 
        /// PIR 25920 New Plan DC 2025
        /// </summary>
        public bool iblnIsElectionAlreadyMade
        {
            get
            {
                if (iobjPassInfo.istrFormName == "wfmPensionPlanMaintenance" && lblnIsVisibleADECAmountList && !IsAllowChangeADECOnEnroll())
                {
                    bool blnIsElectionValid = false;
                    if (ibusHistory.IsNull()) LoadPreviousHistory();
                    if (ibusCurrentPersonEmploymentDetail.IsNull())
                        LoadCurrentPersonEmploymentDetail();
                    int lintOldAddlEEContributionPercent = 0;
                    if (ibusCurrentPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary)
                        lintOldAddlEEContributionPercent = Convert.ToInt32(icdoPersonAccount.ihstOldValues["addl_ee_contribution_percent_temp"]);
                    else
                        lintOldAddlEEContributionPercent = Convert.ToInt32(icdoPersonAccount.ihstOldValues["addl_ee_contribution_percent"]);
                    if (icdoPersonAccount.person_account_id > 0 && lintOldAddlEEContributionPercent != iintAddlEEContributionPercent)
                        blnIsElectionValid = true;
                    if (blnIsElectionValid && ibusHistory.IsNotNull() && ibusHistory.icdoPersonAccountRetirementHistory.person_account_retirement_history_id > 0 
                        && !icdoPersonAccount.iblnIs_Addl_ee_contribution_percent_NULL &&
                        !(istrAllowOverlapHistory == busConstant.Flag_Yes))
                        return true;
                }
                
                return false;
            }
        }
        /// <summary>
        /// return true if contribution rates are available and ADEC fields are fetched  
        /// PIR 25920 New Plan DC 2025
        /// </summary>
        public bool lblnIsVisibleADECAmountList
        {
            get
            {
                bool lblnReturnTrue = false;
                if (ibusCurrentPersonEmploymentDetail.IsNull())
                    LoadCurrentPersonEmploymentDetail();
                if (iclbEligibleADECAmountValue.Count == 0)
                    LoadADECAmountValues();
                if (iclbEligibleADECAmountValue.IsNotNull() && iclbEligibleADECAmountValue.Count > 0)
                    lblnReturnTrue = true;
                return lblnReturnTrue;
            }

        }

        public bool IsAllowChangeADECOnEnroll()
        {
            string lstrLatestHistoryADECAmount = "0";
            bool lblnIsPrevValueNull = false;
            if (ibusHistory.IsNull()) LoadPreviousHistory();
            if (ibusCurrentPersonEmploymentDetail.IsNull())
                LoadCurrentPersonEmploymentDetail();

            DataTable ldtPersonAccount = Select<cdoPersonAccount>(new string[1] { enmPersonAccount.person_account_id.ToString() },
                               new object[1] { icdoPersonAccount.person_account_id }, null, null);
            if (ldtPersonAccount.IsNotNull() && ldtPersonAccount.Rows.Count > 0)
            {
                if (ibusCurrentPersonEmploymentDetail?.icdoPersonEmploymentDetail?.type_value == busConstant.PersonJobTypeTemporary)
                {
                    if (Convert.ToString(ldtPersonAccount.Rows[0][enmPersonAccount.addl_ee_contribution_percent_temp.ToString()]) == string.Empty)
                    {
                        lstrLatestHistoryADECAmount = "-1";
                        lblnIsPrevValueNull = true;
                    }
                    else
                        lstrLatestHistoryADECAmount = Convert.ToString(ldtPersonAccount.Rows[0][enmPersonAccount.addl_ee_contribution_percent_temp.ToString()]);
                }
                else
                {
                    if (Convert.ToString(ldtPersonAccount.Rows[0][enmPersonAccount.addl_ee_contribution_percent.ToString()]) == string.Empty)
                    {
                        lstrLatestHistoryADECAmount = "-1";
                        lblnIsPrevValueNull = true;
                    }
                    else
                        lstrLatestHistoryADECAmount = Convert.ToString(ldtPersonAccount.Rows[0][enmPersonAccount.addl_ee_contribution_percent.ToString()]);
                }
            }
            //if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled 
            //    && ibusHistory.icdoPersonAccountRetirementHistory.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled)
            //    return false;
            if (!lblnIsPrevValueNull && iintAddlEEContributionPercent != Convert.ToInt32(lstrLatestHistoryADECAmount))
                return false;
            if (icdoPersonAccount.ihstOldValues.Count > 0)
            {
                int lintOldAddlEEContributionPercent = 0;
                if (ibusCurrentPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary)
                    lintOldAddlEEContributionPercent = Convert.ToInt32(icdoPersonAccount.ihstOldValues["addl_ee_contribution_percent_temp"]);
                else
                    lintOldAddlEEContributionPercent = Convert.ToInt32(icdoPersonAccount.ihstOldValues["addl_ee_contribution_percent"]);
                if (!lblnIsPrevValueNull && iintAddlEEContributionPercent != lintOldAddlEEContributionPercent)
                    return false;
            }
            return true;
        }
        public bool IsEEContributionSelectedForInitialStart
        {
            get
            {
                if (ibusCurrentPersonEmploymentDetail.IsNull()) LoadCurrentPersonEmploymentDetail();
                if (iobjPassInfo.istrFormName == "wfmPensionPlanMaintenance" && ibusCurrentPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value != busConstant.PersonJobTypeTemporary)
                { 
                    if (ibusCurrentPersonEmploymentDetail.ibusPersonEmployment.IsNull()) ibusCurrentPersonEmploymentDetail.LoadPersonEmployment();
                    return lblnIsVisibleADECAmountList
                         && iintAddlEEContributionPercent != 0
                         && ibusCurrentPersonEmploymentDetail.ibusPersonEmployment?.icdoPersonEmployment?.start_date.GetFirstDayofNextMonth() > icdoPersonAccount.history_change_date;
                }
                return false;
            }
        }
    }
}
