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
using Sagitec.CustomDataObjects;
using System.Linq;
using NeoSpin.DataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busDeductionCalculation : busDeductionCalculationGen
    {
        #region Properties Used
        private busPerson _ibusPerson;
        public busPerson ibusPerson
        {
            get
            {
                return _ibusPerson;
            }
            set
            {
                _ibusPerson = value;
            }
        }

        private busPlan _ibusPlan;
        public busPlan ibusPlan
        {
            get
            {
                return _ibusPlan;
            }
            set
            {
                _ibusPlan = value;
            }
        }

        private busPersonAccount _ibusPersonAccount;
        public busPersonAccount ibusPersonAccount
        {
            get
            {
                return ibusPersonAccount;
            }
            set
            {
                _ibusPersonAccount = value;
            }
        }

        private busBenefitApplication _ibusBenefitApplication;
        public busBenefitApplication ibusBenefitApplication
        {
            get { return _ibusBenefitApplication; }
            set { _ibusBenefitApplication = value; }
        }

        private busBenefitGhdvDeduction _ibusBenefitHealthDeduction;
        public busBenefitGhdvDeduction ibusBenefitHealthDeduction
        {
            get
            {
                return _ibusBenefitHealthDeduction;
            }
            set
            {
                _ibusBenefitHealthDeduction = value;
            }
        }

        private busBenefitGhdvDeduction _ibusBenefitDentalDeduction;
        public busBenefitGhdvDeduction ibusBenefitDentalDeduction
        {
            get
            {
                return _ibusBenefitDentalDeduction;
            }
            set
            {
                _ibusBenefitDentalDeduction = value;
            }
        }

        private busBenefitGhdvDeduction _ibusBenefitVisionDeduction;
        public busBenefitGhdvDeduction ibusBenefitVisionDeduction
        {
            get
            {
                return _ibusBenefitVisionDeduction;
            }
            set
            {
                _ibusBenefitVisionDeduction = value;
            }
        }

        private busBenefitPayeeTaxWithholding _ibusBenefitPayeeStateTaxWithholding;
        public busBenefitPayeeTaxWithholding ibusBenefitPayeeStateTaxWithholding
        {
            get
            {
                return _ibusBenefitPayeeStateTaxWithholding;
            }
            set
            {
                _ibusBenefitPayeeStateTaxWithholding = value;
            }
        }

        private busBenefitPayeeTaxWithholding _ibusBenefitPayeeFedTaxWithholding;
        public busBenefitPayeeTaxWithholding ibusBenefitPayeeFedTaxWithholding
        {
            get
            {
                return _ibusBenefitPayeeFedTaxWithholding;
            }
            set
            {
                _ibusBenefitPayeeFedTaxWithholding = value;
            }
        }

        private busBenefitDeductionSummary _ibusBenefitDeductionSummary;
        public busBenefitDeductionSummary ibusBenefitDeductionSummary
        {
            get { return _ibusBenefitDeductionSummary; }
            set { _ibusBenefitDeductionSummary = value; }
        }

        private busBenefitCalculationOptions _ibusBenefitCalculationOptions;
        public busBenefitCalculationOptions ibusBenefitCalculationOptions
        {
            get { return _ibusBenefitCalculationOptions; }
            set { _ibusBenefitCalculationOptions = value; }
        }

        private string _istrLifeInsuranceTypeValue;
        public string istrLifeInsuranceTypeValue
        {
            get
            {
                return _istrLifeInsuranceTypeValue;
            }

            set
            {
                _istrLifeInsuranceTypeValue = value;
            }
        }

        private decimal _idecTotalLifePremium;
        public decimal idecTotalLifePremium
        {
            get
            {
                return _idecTotalLifePremium;
            }
            set
            {
                _idecTotalLifePremium = value;
            }
        }

        private decimal _idecTotalLtcPremium;
        public decimal idecTotalLtcPremium
        {
            get
            {
                return _idecTotalLtcPremium;
            }
            set
            {
                _idecTotalLtcPremium = value;
            }
        }

        private decimal _idecMiscellaneousTax;
        public decimal idecMiscellaneousTax
        {
            get { return _idecMiscellaneousTax; }
            set { _idecMiscellaneousTax = value; }
        }

        private decimal _idecMisellaneousDeduction;
        public decimal idecMisellaneousDeduction
        {
            get { return _idecMisellaneousDeduction; }
            set { _idecMisellaneousDeduction = value; }
        }

        private decimal _idecNetMonthlyPensionBenefit;
        public decimal idecNetMonthlyPensionBenefit
        {
            get { return _idecNetMonthlyPensionBenefit; }
            set { _idecNetMonthlyPensionBenefit = value; }
        }

        private decimal _idecGrossMonthlyBenefit;
        public decimal idecGrossMonthlyBenefit
        {
            get { return _idecGrossMonthlyBenefit; }
            set { _idecGrossMonthlyBenefit = value; }
        }

        private int _iintSpousePersonid;
        public int iintSpousePersonid
        {
            get { return _iintSpousePersonid; }
            set { _iintSpousePersonid = value; }
        }

        private int _iintSpouseAge;
        public int iintSpouseAge
        {
            get { return _iintSpouseAge; }
            set { _iintSpouseAge = value; }
        }

        private int _iintPersonAge;
        public int iintPersonAge
        {
            get { return _iintPersonAge; }
            set { _iintPersonAge = value; }
        }

        private int _iintOrgPlanidHealth;
        public int iintOrgPlanidHealth
        {
            get { return _iintOrgPlanidHealth; }
            set { _iintOrgPlanidHealth = value; }
        }

        private int _iintOrgPlanidDental;
        public int iintOrgPlanidDental
        {
            get { return _iintOrgPlanidDental; }
            set { _iintOrgPlanidDental = value; }
        }

        private int _iintOrgPlanidVision;
        public int iintOrgPlanidVision
        {
            get { return _iintOrgPlanidVision; }
            set { _iintOrgPlanidVision = value; }
        }

        private int _iintOrgPlanidLTC;
        public int iintOrgPlanidLTC
        {
            get { return _iintOrgPlanidLTC; }
            set { _iintOrgPlanidLTC = value; }
        }

        private int _iintOrgPlanidLife;
        public int iintOrgPlanidLife
        {
            get { return _iintOrgPlanidLife; }
            set { _iintOrgPlanidLife = value; }
        }

        private Collection<busBenefitGhdvDeduction> _iclbBenefitGHDVDeduction;
        public Collection<busBenefitGhdvDeduction> iclbBenefitGHDVDeduction
        {
            get
            {
                return _iclbBenefitGHDVDeduction;
            }
            set
            {
                _iclbBenefitGHDVDeduction = value;
            }
        }

        private Collection<busBenefitLifeDeduction> _iclbBenefitLifeDeduction;
        public Collection<busBenefitLifeDeduction> iclbBenefitLifeDeduction
        {
            get
            {
                return _iclbBenefitLifeDeduction;
            }
            set
            {
                _iclbBenefitLifeDeduction = value;
            }
        }

        public Collection<busBenefitLtcDeduction> iclbLTCBenefitDeductions { get; set; }

        private Collection<busBenefitLtcDeduction> _iclbBenefitLtcMemberDeduction;
        public Collection<busBenefitLtcDeduction> iclbBenefitLtcMemberDeduction
        {
            get
            {
                return _iclbBenefitLtcMemberDeduction;
            }
            set
            {
                _iclbBenefitLtcMemberDeduction = value;
            }
        }

        private Collection<busBenefitLtcDeduction> _iclbBenefitLtcSpouseDeduction;
        public Collection<busBenefitLtcDeduction> iclbBenefitLtcSpouseDeduction
        {
            get
            {
                return _iclbBenefitLtcSpouseDeduction;
            }
            set
            {
                _iclbBenefitLtcSpouseDeduction = value;
            }
        }

        private Collection<busBenefitPayeeTaxWithholding> _iclbBenefitPayeeTaxWithholding;
        public Collection<busBenefitPayeeTaxWithholding> iclbBenefitPayeeTaxWithholding
        {
            get
            {
                return _iclbBenefitPayeeTaxWithholding;
            }
            set
            {
                _iclbBenefitPayeeTaxWithholding = value;
            }
        }

        #endregion

        #region Load Business Objects
        public void LoadPerson()
        {
            if (_ibusPerson == null)
            {
                _ibusPerson = new busPerson();
            }
            _ibusPerson.FindPerson(icdoBenefitCalculation.person_id);
        }


        public void LoadPlan()
        {
            if (_ibusPlan == null)
            {
                _ibusPlan = new busPlan();
            }
            _ibusPlan.FindPlan(icdoBenefitCalculation.plan_id);
        }

        public void LoadBenefitApplication()
        {
            if (_ibusBenefitApplication == null)
            {
                _ibusBenefitApplication = new busBenefitApplication();
            }
            _ibusBenefitApplication.FindBenefitApplication(icdoBenefitCalculation.benefit_application_id);
        }

        public void LoadPersonAccount()
        {
            if (_ibusPersonAccount == null)
                _ibusPersonAccount = new busPersonAccount();
            if (ibusPerson == null)
                LoadPerson();
            if (ibusPlan == null)
                LoadPlan();
            _ibusPersonAccount = ibusPerson.LoadActivePersonAccountByPlan(_ibusPlan.icdoPlan.plan_id);
        }


        public busPersonAccount LoadPersonAccount(int aintPlanid)
        {
            if (ibusPerson == null)
                LoadPerson();
            return ibusPerson.LoadActivePersonAccountByPlan(aintPlanid);
        }

        public void LoadBenefitGHDVDeductions()
        {
            DataTable ldtbList = Select<cdoBenefitGhdvDeduction>(
                           new string[1] { "benefit_calculation_id" },
                           new object[1] { icdoBenefitCalculation.benefit_calculation_id }, null, null);
            _iclbBenefitGHDVDeduction = GetCollection<busBenefitGhdvDeduction>(ldtbList, "icdoBenefitGhdvDeduction");
        }

        public void LoadBenefitHealthDeduction()
        {
            int lintFlag = 0;
            if (_iclbBenefitGHDVDeduction == null)
                LoadBenefitGHDVDeductions();
            _ibusBenefitHealthDeduction = new busBenefitGhdvDeduction();
            _ibusBenefitHealthDeduction.icdoBenefitGhdvDeduction = new cdoBenefitGhdvDeduction();

            foreach (busBenefitGhdvDeduction lobjGhdvDeduction in _iclbBenefitGHDVDeduction)
            {
                if (lobjGhdvDeduction.icdoBenefitGhdvDeduction.benefit_deduction_ghdv_value == busConstant.BenefitHealthDeduction)
                {
                    lintFlag = 1;
                    _ibusBenefitHealthDeduction.icdoBenefitGhdvDeduction = lobjGhdvDeduction.icdoBenefitGhdvDeduction;
                    break;
                }
            }
            if (lintFlag == 0)
            {
                _ibusBenefitHealthDeduction = AssignDefaultGhdvValues(busConstant.PlanIdGroupHealth);
                LoadHealthPlanOption();
            }
            ibusBenefitHealthDeduction.LoadGHDVObjectFromDeduction();
            ibusBenefitHealthDeduction.ibusPersonAccountGHDV.LoadRateStructure(icdoBenefitCalculation.created_date);
            ibusBenefitHealthDeduction.ibusPersonAccountGHDV.LoadCoverageRefID();
        }

        public void LoadBenefitDentalDeduction()
        {
            int lintFlag = 0;
            if (_iclbBenefitGHDVDeduction == null)
                LoadBenefitGHDVDeductions();
            _ibusBenefitDentalDeduction = new busBenefitGhdvDeduction();
            _ibusBenefitDentalDeduction.icdoBenefitGhdvDeduction = new cdoBenefitGhdvDeduction();
            foreach (busBenefitGhdvDeduction lobjGhdvDeduction in _iclbBenefitGHDVDeduction)
                if (lobjGhdvDeduction.icdoBenefitGhdvDeduction.benefit_deduction_ghdv_value == busConstant.BenefitDentalDeduction)
                {
                    lintFlag = 1;
                    _ibusBenefitDentalDeduction.icdoBenefitGhdvDeduction = lobjGhdvDeduction.icdoBenefitGhdvDeduction;
                    break;
                }
            if (lintFlag == 0)
                _ibusBenefitDentalDeduction = AssignDefaultGhdvValues(busConstant.PlanIdDental);
            iintOrgPlanidDental = ibusPerson.LoadDefaultOrgPlanIdByPlanId(busConstant.PlanIdDental);
        }

        public void LoadBenefitVisionDeduction()
        {
            int lintFlag = 0;
            if (_iclbBenefitGHDVDeduction == null)
                LoadBenefitGHDVDeductions();
            _ibusBenefitVisionDeduction = new busBenefitGhdvDeduction();
            _ibusBenefitVisionDeduction.icdoBenefitGhdvDeduction = new cdoBenefitGhdvDeduction();
            foreach (busBenefitGhdvDeduction lobjGhdvDeduction in _iclbBenefitGHDVDeduction)
                if (lobjGhdvDeduction.icdoBenefitGhdvDeduction.benefit_deduction_ghdv_value == busConstant.BenefitVisionDeduction)
                {
                    lintFlag = 1;
                    _ibusBenefitVisionDeduction.icdoBenefitGhdvDeduction = lobjGhdvDeduction.icdoBenefitGhdvDeduction;
                    break;
                }
            if (lintFlag == 0)
                _ibusBenefitVisionDeduction = AssignDefaultGhdvValues(busConstant.PlanIdVision);
            iintOrgPlanidVision = ibusPerson.LoadDefaultOrgPlanIdByPlanId(busConstant.PlanIdVision);
        }

        public void LoadBenefitLifeDeductions()
        {
            if (_iclbBenefitLifeDeduction == null)
                LoadBenefitLifeNew();
            DataTable ldtbList = Select<cdoBenefitLifeDeduction>(
                           new string[1] { "benefit_calculation_id" },
                           new object[1] { icdoBenefitCalculation.benefit_calculation_id }, null, null);
            foreach (busBenefitLifeDeduction lobjLifeDeduction in iclbBenefitLifeDeduction)
            {
                foreach (DataRow dr in ldtbList.Rows)
                {
                    if (dr["level_of_coverage_value"].ToString() == lobjLifeDeduction.icdoBenefitLifeDeduction.level_of_coverage_value)
                    {
                        lobjLifeDeduction.icdoBenefitLifeDeduction.LoadData(dr);
                        istrLifeInsuranceTypeValue = lobjLifeDeduction.icdoBenefitLifeDeduction.life_insurance_type_value;
                        idecTotalLifePremium += lobjLifeDeduction.icdoBenefitLifeDeduction.computed_premium_amount;
                        break;
                    }
                }
            }
            iintOrgPlanidLife = ibusPerson.LoadDefaultOrgPlanIdByPlanId(busConstant.PlanIdGroupLife);
        }

        public void LoadBenefitLtcMemberDeductions()
        {
            if (_iclbBenefitLtcMemberDeduction == null)
                LoadBenefitLTCMemberNew();
            DataTable ldtbList = Select<cdoBenefitLtcDeduction>(
                           new string[1] { "benefit_calculation_id" },
                           new object[1] { icdoBenefitCalculation.benefit_calculation_id }, null, null);

            foreach (busBenefitLtcDeduction lobjBenefitLtcDeduction in _iclbBenefitLtcMemberDeduction)
            {
                if (ldtbList.Rows.Count > 0)
                {
                    foreach (DataRow dr in ldtbList.Rows)
                    {
                        if ((dr["level_of_coverage_value"].ToString() == lobjBenefitLtcDeduction.icdoBenefitLtcDeduction.level_of_coverage_value) &&
                            (dr["ltc_relationship_value"].ToString() == busConstant.PersonAccountLtcRelationShipMember))
                        {
                            lobjBenefitLtcDeduction.icdoBenefitLtcDeduction.LoadData(dr);
                            idecTotalLtcPremium += lobjBenefitLtcDeduction.icdoBenefitLtcDeduction.computed_premium_amount;
                            break;
                        }
                    }
                }
            }
            iintOrgPlanidLTC = ibusPerson.LoadDefaultOrgPlanIdByPlanId(busConstant.PlanIdLTC);
        }

        public void LoadBenefitLtcSpouseDeductions()
        {
            if (_iclbBenefitLtcSpouseDeduction == null)
                LoadBenefitLTCSpouseNew();
            DataTable ldtbList = Select<cdoBenefitLtcDeduction>(
                           new string[1] { "benefit_calculation_id" },
                           new object[1] { icdoBenefitCalculation.benefit_calculation_id }, null, null);
            DataTable ldtbSpouse = busNeoSpinBase.Select<cdoPersonContact>(new string[2] { "person_id", "relationship_value" },
                                       new object[2] { ibusPerson.icdoPerson.person_id, busConstant.PersonContactTypeSpouse }, null, null);

            if (ldtbSpouse.Rows.Count > 0 && ldtbSpouse.Rows[0]["contact_person_id"] != DBNull.Value)
                iintSpousePersonid = Convert.ToInt32(ldtbSpouse.Rows[0]["contact_person_id"]); // PIR 9298 person_id was taken instead contact_person_id

            foreach (busBenefitLtcDeduction lobjBenefitLtcDeduction in _iclbBenefitLtcSpouseDeduction)
            {
                if (ldtbList.Rows.Count > 0)
                {
                    foreach (DataRow dr in ldtbList.Rows)
                    {
                        if ((dr["level_of_coverage_value"].ToString() == lobjBenefitLtcDeduction.icdoBenefitLtcDeduction.level_of_coverage_value) &&
                            (dr["ltc_relationship_value"].ToString() == busConstant.PersonAccountLtcRelationShipSpouse))
                        {
                            lobjBenefitLtcDeduction.icdoBenefitLtcDeduction.LoadData(dr);
                            idecTotalLtcPremium += lobjBenefitLtcDeduction.icdoBenefitLtcDeduction.computed_premium_amount;
                            break;
                        }
                    }
                }
            }
        }

        public void LoadBenefitPayeeTaxWithholding()
        {
            DataTable ldtbList = Select<cdoBenefitPayeeTaxWithholding>(
                                      new string[1] { "benefit_calculation_id" },
                                      new object[1] { icdoBenefitCalculation.benefit_calculation_id }, null, null);
            _iclbBenefitPayeeTaxWithholding = GetCollection<busBenefitPayeeTaxWithholding>(ldtbList, "icdoBenefitPayeeTaxWithholding");
        }

        public void LoadBenefitPayeeFedTaxWithholding()
        {
            int lintFlag = 0;
            if (_iclbBenefitPayeeTaxWithholding == null)
                LoadBenefitPayeeTaxWithholding();
            _ibusBenefitPayeeFedTaxWithholding = new busBenefitPayeeTaxWithholding();
            _ibusBenefitPayeeFedTaxWithholding.icdoBenefitPayeeTaxWithholding = new cdoBenefitPayeeTaxWithholding();
            foreach (busBenefitPayeeTaxWithholding lobjTaxWithholding in iclbBenefitPayeeTaxWithholding)
                if (lobjTaxWithholding.icdoBenefitPayeeTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierFedTax)
                {
                    lintFlag = 1;
                    _ibusBenefitPayeeFedTaxWithholding.icdoBenefitPayeeTaxWithholding = lobjTaxWithholding.icdoBenefitPayeeTaxWithholding;
                    break;
                }
            if (lintFlag == 0)
                _ibusBenefitPayeeFedTaxWithholding = AssignDefaultTaxValues(busConstant.PayeeAccountTaxIdentifierFedTax);

        }

        public void LoadBenefitPayeeStateTaxWithholding()
        {
            int lintFlag = 0;
            if (_iclbBenefitPayeeTaxWithholding == null)
                LoadBenefitPayeeTaxWithholding();
            _ibusBenefitPayeeStateTaxWithholding = new busBenefitPayeeTaxWithholding();
            _ibusBenefitPayeeStateTaxWithholding.icdoBenefitPayeeTaxWithholding = new cdoBenefitPayeeTaxWithholding();
            foreach (busBenefitPayeeTaxWithholding lobjTaxWithholding in iclbBenefitPayeeTaxWithholding)
                if (lobjTaxWithholding.icdoBenefitPayeeTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierStateTax)
                {
                    lintFlag = 1;
                    _ibusBenefitPayeeStateTaxWithholding.icdoBenefitPayeeTaxWithholding = lobjTaxWithholding.icdoBenefitPayeeTaxWithholding;
                    break;
                }
            if (lintFlag == 0)
                _ibusBenefitPayeeStateTaxWithholding = AssignDefaultTaxValues(busConstant.PayeeAccountTaxIdentifierStateTax);
        }

        public void LoadBenefitDeductionSummary()
        {
            if (_ibusBenefitDeductionSummary == null)
                _ibusBenefitDeductionSummary = new busBenefitDeductionSummary();
            if (!_ibusBenefitDeductionSummary.FindBenefitDeductionSummaryByCalId(icdoBenefitCalculation.benefit_calculation_id))
                _ibusBenefitDeductionSummary = AssignDefaultDeductionSummaryValues();
        }

        #endregion

        #region Function to Initialize Collection
        /// <summary>
        /// Function to initialize the LTC member collection
        /// </summary>
        public void LoadBenefitLTCMemberNew()
        {
            _iclbBenefitLtcMemberDeduction = new Collection<busBenefitLtcDeduction>();
            DataTable ldtbNewList = Select("cdoBenefitLtcDeduction.LoadLtcOptionNewMember", new object[0]);
            _iclbBenefitLtcMemberDeduction = GetCollection<busBenefitLtcDeduction>(ldtbNewList, "icdoBenefitLtcDeduction");
        }

        /// <summary>
        /// Function to initialize the LTC spouse collection
        /// </summary>
        public void LoadBenefitLTCSpouseNew()
        {
            _iclbBenefitLtcSpouseDeduction = new Collection<busBenefitLtcDeduction>();
            DataTable ldtbNewList = Select("cdoBenefitLtcDeduction.LoadLtcOptionNewSpouse", new object[0]);
            _iclbBenefitLtcSpouseDeduction = GetCollection<busBenefitLtcDeduction>(ldtbNewList, "icdoBenefitLtcDeduction");
        }

        public void LoadBenefitLTCNew()
        {
            iclbLTCBenefitDeductions = new Collection<busBenefitLtcDeduction>();

            busBenefitLtcDeduction lobjMember = new busBenefitLtcDeduction { icdoBenefitLtcDeduction = new cdoBenefitLtcDeduction() };
            lobjMember.icdoBenefitLtcDeduction.ltc_relationship_id = 338;
            lobjMember.icdoBenefitLtcDeduction.ltc_relationship_value = busConstant.PersonAccountLtcRelationShipMember;
            lobjMember.icdoBenefitLtcDeduction.level_of_coverage_id = 408;
            lobjMember.icdoBenefitLtcDeduction.level_of_coverage_value = busConstant.LTCLevelOfCoverage3YRS;
            lobjMember.icdoBenefitLtcDeduction.level_of_coverage_description = busGlobalFunctions.GetDescriptionByCodeValue(408, busConstant.LTCLevelOfCoverage3YRS, iobjPassInfo);
            lobjMember.icdoBenefitLtcDeduction.ltc_insurance_type_id = 339;
            iclbLTCBenefitDeductions.Add(lobjMember);

            busBenefitLtcDeduction lobjSpouse = new busBenefitLtcDeduction { icdoBenefitLtcDeduction = new cdoBenefitLtcDeduction() };
            lobjSpouse.icdoBenefitLtcDeduction.ltc_relationship_id = 338;
            lobjSpouse.icdoBenefitLtcDeduction.ltc_relationship_value = busConstant.PersonAccountLtcRelationShipSpouse;
            lobjSpouse.icdoBenefitLtcDeduction.level_of_coverage_id = 408;
            lobjSpouse.icdoBenefitLtcDeduction.level_of_coverage_value = busConstant.LTCLevelOfCoverage5YRS;
            lobjSpouse.icdoBenefitLtcDeduction.level_of_coverage_description = busGlobalFunctions.GetDescriptionByCodeValue(408, busConstant.LTCLevelOfCoverage5YRS, iobjPassInfo);
            lobjSpouse.icdoBenefitLtcDeduction.ltc_insurance_type_id = 339;
            iclbLTCBenefitDeductions.Add(lobjSpouse);
        }

        public void LoadLTCDeductions()
        {
            if (iclbLTCBenefitDeductions.IsNotNull())
            {
                foreach (busBenefitLtcDeduction lobjDeduction in iclbLTCBenefitDeductions)
                {
                    foreach (busBenefitLtcDeduction lobjMember in _iclbBenefitLtcMemberDeduction)
                    {
                        if (lobjMember.icdoBenefitLtcDeduction.ltc_relationship_value == lobjDeduction.icdoBenefitLtcDeduction.ltc_relationship_value &&
                            lobjMember.icdoBenefitLtcDeduction.level_of_coverage_value == lobjDeduction.icdoBenefitLtcDeduction.level_of_coverage_value)
                        {
                            lobjMember.icdoBenefitLtcDeduction.ltc_insurance_type_value = lobjDeduction.icdoBenefitLtcDeduction.member_insurance_type;
                        }
                    }

                    foreach (busBenefitLtcDeduction lobjSpouse in _iclbBenefitLtcSpouseDeduction)
                    {
                        if (lobjSpouse.icdoBenefitLtcDeduction.ltc_relationship_value == lobjDeduction.icdoBenefitLtcDeduction.ltc_relationship_value &&
                            lobjSpouse.icdoBenefitLtcDeduction.level_of_coverage_value == lobjDeduction.icdoBenefitLtcDeduction.level_of_coverage_value)
                        {
                            lobjSpouse.icdoBenefitLtcDeduction.ltc_insurance_type_value = lobjDeduction.icdoBenefitLtcDeduction.spouse_insurance_type;
                        }
                    }
                    lobjDeduction.icdoBenefitLtcDeduction.ltc_insurance_type_description =
                                                        busGlobalFunctions.GetDescriptionByCodeValue(339, lobjDeduction.icdoBenefitLtcDeduction.ltc_insurance_type_value, iobjPassInfo);
                }
            }
        }

        /// <summary>
        /// Loading Group Life Option Grid objects in new mode.
        /// </summary>
        public void LoadBenefitLifeNew()
        {
            _iclbBenefitLifeDeduction = new Collection<busBenefitLifeDeduction>();
            istrLifeInsuranceTypeValue = busConstant.LifeInsuranceTypeRetireeMember;

            busBenefitLifeDeduction lobjBasicLifeOption = new busBenefitLifeDeduction();
            lobjBasicLifeOption.icdoBenefitLifeDeduction = new cdoBenefitLifeDeduction();
            lobjBasicLifeOption.icdoBenefitLifeDeduction.benefit_life_deduction_id = -1;
            lobjBasicLifeOption.icdoBenefitLifeDeduction.level_of_coverage_value = busConstant.LevelofCoverage_Basic;
            lobjBasicLifeOption.icdoBenefitLifeDeduction.level_of_coverage_description =
                iobjPassInfo.isrvDBCache.GetCodeDescriptionString(busConstant.LevelofCoverage_CodeID, busConstant.LevelofCoverage_Basic);
            lobjBasicLifeOption.icdoBenefitLifeDeduction.life_insurance_type_value = istrLifeInsuranceTypeValue;
            lobjBasicLifeOption.icdoBenefitLifeDeduction.ienuObjectState = ObjectState.Insert;
            _iclbBenefitLifeDeduction.Add(lobjBasicLifeOption);

            busBenefitLifeDeduction lobjSupplementalLifeOption = new busBenefitLifeDeduction();
            lobjSupplementalLifeOption.icdoBenefitLifeDeduction = new cdoBenefitLifeDeduction();
            lobjSupplementalLifeOption.icdoBenefitLifeDeduction.benefit_life_deduction_id = -2;
            lobjSupplementalLifeOption.icdoBenefitLifeDeduction.level_of_coverage_value = busConstant.LevelofCoverage_Supplemental;
            lobjSupplementalLifeOption.icdoBenefitLifeDeduction.level_of_coverage_description =
                iobjPassInfo.isrvDBCache.GetCodeDescriptionString(busConstant.LevelofCoverage_CodeID, busConstant.LevelofCoverage_Supplemental);
            lobjSupplementalLifeOption.icdoBenefitLifeDeduction.life_insurance_type_value = istrLifeInsuranceTypeValue;
            lobjSupplementalLifeOption.icdoBenefitLifeDeduction.ienuObjectState = ObjectState.Insert;
            _iclbBenefitLifeDeduction.Add(lobjSupplementalLifeOption);

            busBenefitLifeDeduction lobjDependentLifeOption = new busBenefitLifeDeduction();
            lobjDependentLifeOption.icdoBenefitLifeDeduction = new cdoBenefitLifeDeduction();
            lobjDependentLifeOption.icdoBenefitLifeDeduction.benefit_life_deduction_id = -3;
            lobjDependentLifeOption.icdoBenefitLifeDeduction.level_of_coverage_value = busConstant.LevelofCoverage_DependentSupplemental;
            lobjDependentLifeOption.icdoBenefitLifeDeduction.level_of_coverage_description =
                iobjPassInfo.isrvDBCache.GetCodeDescriptionString(busConstant.LevelofCoverage_CodeID, busConstant.LevelofCoverage_DependentSupplemental);
            lobjDependentLifeOption.icdoBenefitLifeDeduction.life_insurance_type_value = istrLifeInsuranceTypeValue;
            lobjDependentLifeOption.icdoBenefitLifeDeduction.ienuObjectState = ObjectState.Insert;
            _iclbBenefitLifeDeduction.Add(lobjDependentLifeOption);

            busBenefitLifeDeduction lobjSpouseLifeOption = new busBenefitLifeDeduction();
            lobjSpouseLifeOption.icdoBenefitLifeDeduction = new cdoBenefitLifeDeduction();
            lobjSpouseLifeOption.icdoBenefitLifeDeduction.benefit_life_deduction_id = -4;
            lobjSpouseLifeOption.icdoBenefitLifeDeduction.level_of_coverage_value = busConstant.LevelofCoverage_SpouseSupplemental;
            lobjSpouseLifeOption.icdoBenefitLifeDeduction.level_of_coverage_description =
                iobjPassInfo.isrvDBCache.GetCodeDescriptionString(busConstant.LevelofCoverage_CodeID, busConstant.LevelofCoverage_SpouseSupplemental);
            lobjSpouseLifeOption.icdoBenefitLifeDeduction.life_insurance_type_value = istrLifeInsuranceTypeValue;
            lobjSpouseLifeOption.icdoBenefitLifeDeduction.ienuObjectState = ObjectState.Insert;
            _iclbBenefitLifeDeduction.Add(lobjSpouseLifeOption);
        }

        //PIR 1730 - start - Display amounts from SGT_PERSON_ACCOUNT_LIFE_OPTION where plan_option is ENLD in appropriate Life options for deductions
        public Collection<busPersonAccountLifeOption> iclbCurrentCoverageLifeOptions { get; set; }
        public void LoadCurrentCoverageLifeOptions()
        {
            busBase lbusBase = new busBase();
            iclbCurrentCoverageLifeOptions = new Collection<busPersonAccountLifeOption>();
            if (ibusPerson.icolPersonAccountByPlan.IsNull())
                ibusPerson.LoadPersonAccountByPlan(busConstant.PlanIdGroupLife);
            busPersonAccount lbusPersonAccount = ibusPerson.icolPersonAccountByPlan
                                                            .Where(i => i.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusInsuranceCancelled)
                                                            .FirstOrDefault();
            if (lbusPersonAccount.IsNotNull())
            {
                DataTable ldtbLifeOptions = Select<cdoPersonAccountLifeOption>(new string[2] { enmPersonAccountLifeOption.plan_option_status_value.ToString(), enmPersonAccountLifeOption.person_account_id.ToString() },
                                                    new object[2] { busConstant.PlanOptionStatusValueEnrolled, lbusPersonAccount.icdoPersonAccount.person_account_id }, null, null);
                iclbCurrentCoverageLifeOptions = lbusBase.GetCollection<busPersonAccountLifeOption>(ldtbLifeOptions);
            }
        }
        //PIR 1730 - end - Display amounts from SGT_PERSON_ACCOUNT_LIFE_OPTION where plan_option is ENLD in appropriate Life options for deductions

        /// <summary>
        /// Function to load the tax collection object with default values
        /// </summary>
        /// <param name="astrTaxIdentifier">Tax Identifier(Fed or State Tax)</param>
        /// <returns></returns>
        private busBenefitPayeeTaxWithholding AssignDefaultTaxValues(string astrTaxIdentifier)
        {
            busBenefitPayeeTaxWithholding lobjTaxWithholding = new busBenefitPayeeTaxWithholding();
            lobjTaxWithholding.icdoBenefitPayeeTaxWithholding = new cdoBenefitPayeeTaxWithholding();
            lobjTaxWithholding.icdoBenefitPayeeTaxWithholding.benefit_payee_tax_withholding_id = -1;
            lobjTaxWithholding.icdoBenefitPayeeTaxWithholding.tax_identifier_value = astrTaxIdentifier;
            //if (astrTaxIdentifier == busConstant.PayeeAccountTaxIdentifierFedTax)
            //    lobjTaxWithholding.icdoBenefitPayeeTaxWithholding.tax_option_value = busConstant.NoFedTax;
            //else if (astrTaxIdentifier == busConstant.PayeeAccountTaxIdentifierStateTax)
            //    lobjTaxWithholding.icdoBenefitPayeeTaxWithholding.tax_option_value = busConstant.NoStateTax;
            lobjTaxWithholding.icdoBenefitPayeeTaxWithholding.marital_status_value = busConstant.PersonMaritalStatusMarried;
            lobjTaxWithholding.icdoBenefitPayeeTaxWithholding.tax_allowance = 3;
            lobjTaxWithholding.icdoBenefitPayeeTaxWithholding.additional_tax_amount = 0;
            lobjTaxWithholding.icdoBenefitPayeeTaxWithholding.ienuObjectState = ObjectState.Insert;
            return lobjTaxWithholding;
        }

        /// <summary>
        /// Function to load Ghdv business object with default values
        /// </summary>
        /// <param name="aintPlanid">Plan id(Health,Dental or Vision)</param>
        /// <returns></returns>
        public busBenefitGhdvDeduction AssignDefaultGhdvValues(int aintPlanid)
        {
            busBenefitGhdvDeduction lobjBenefitGhdvDeduction = new busBenefitGhdvDeduction();
            lobjBenefitGhdvDeduction.icdoBenefitGhdvDeduction = new cdoBenefitGhdvDeduction();

            lobjBenefitGhdvDeduction.icdoBenefitGhdvDeduction.benefit_ghdv_deduction_id = -1;
            if (aintPlanid == busConstant.PlanIdGroupHealth)
                lobjBenefitGhdvDeduction.icdoBenefitGhdvDeduction.health_insurance_type_value = busConstant.HealthInsuranceTypeRetiree;
            else if (aintPlanid == busConstant.PlanIdDental)
                lobjBenefitGhdvDeduction.icdoBenefitGhdvDeduction.dental_insurance_type_value = busConstant.DentalInsuranceTypeRetiree;
            else if (aintPlanid == busConstant.PlanIdVision)
                lobjBenefitGhdvDeduction.icdoBenefitGhdvDeduction.vision_insurance_type_value = busConstant.VisionInsuranceTypeRetiree;
            lobjBenefitGhdvDeduction.icdoBenefitGhdvDeduction.ienuObjectState = ObjectState.Insert;
            return lobjBenefitGhdvDeduction;
        }

        /// <summary>
        /// Function to load Deduction Summary business object with values
        /// </summary>
        public busBenefitDeductionSummary AssignDefaultDeductionSummaryValues()
        {
            busBenefitDeductionSummary lobjDedSummary = new busBenefitDeductionSummary();
            lobjDedSummary.icdoBenefitDeductionSummary = new cdoBenefitDeductionSummary();
            lobjDedSummary.icdoBenefitDeductionSummary.benefit_deduction_summary_id = -1;
            lobjDedSummary.icdoBenefitDeductionSummary.gross_monthly_benefit_amount = 0.0M; //need to calculate as said in UID,Pg 18
            lobjDedSummary.icdoBenefitDeductionSummary.rhic_overridden_amount = 0.0M;//need to bring in the rhic amt           
            return lobjDedSummary;
        }
        #endregion

        #region Overridden Functions
        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            CheckValueEnteredForLife();
            base.BeforeValidate(aenmPageMode);
        }

        public override void BeforePersistChanges()
        {
            ProcessHealth();
            ProcessDental();
            ProcessVision();
            ProcessLTCMember();
            ProcessLTCSpouse();
            ProcessLife();
            if (ibusBenefitPayeeFedTaxWithholding.icdoBenefitPayeeTaxWithholding.tax_option_value == "FTWH") // UAT PIR ID 1992
                ProcessFedTax();
            else
                ibusBenefitPayeeFedTaxWithholding.icdoBenefitPayeeTaxWithholding.computed_tax_amount = 0M;
            if (ibusBenefitPayeeStateTaxWithholding.icdoBenefitPayeeTaxWithholding.tax_option_value == "STWH") // UAT PIR ID 1992
                ProcessStateTax();
            else
                ibusBenefitPayeeStateTaxWithholding.icdoBenefitPayeeTaxWithholding.computed_tax_amount = 0M;
            //if (idecTotalLifePremium == 0.00M)
            //{
            //    iclbBenefitLifeDeduction = new Collection<busBenefitLifeDeduction>();
            //    LoadBenefitLifeDeductions();
            //}
            ProcessDeductionSummary();
            base.BeforePersistChanges();
        }

        public override void AfterPersistChanges()
        {
            CalculateNetMonthlyPensionBenefit();
            base.AfterPersistChanges();
            LoadBenefitDeductionSummary();
        }
        #endregion

        #region Helper Functions
        /// <summary>
        /// Fucntion to calculate Net Monthly Pension Benefit in Dedution Summary Tab
        /// </summary>
        public void CalculateNetMonthlyPensionBenefit()
        {

            ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.net_monthly_pension_benefit_amount =
                ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.gross_monthly_benefit_amount -
                               (ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.fit_overridden_amount +
                               ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.ndit_overridden_amount +
                               ((ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.net_health_insurance_premium_amount > 0) ?
                                ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.net_health_insurance_premium_amount : 0) +//health overridden amount - rhic amount
                               ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.vision_overridden_amount +
                               ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.dental_overridden_amount +
                               ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.life_overridden_amount +
                               ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.ltc_overridden_amount +
                               ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.miscellaneous_deduction_amount);
        }

        /// <summary>
        /// Function to check any entry is made by user for life option
        /// </summary>
        public void CheckValueEnteredForLife()
        {
            foreach (busBenefitLifeDeduction lobjLifeDeduction in iclbBenefitLifeDeduction)
            {
                if (istrLifeInsuranceTypeValue != null && lobjLifeDeduction.icdoBenefitLifeDeduction.coverage_amount != 0.0M)
                {
                    lobjLifeDeduction.icdoBenefitLifeDeduction.idtCalculationDate = icdoBenefitCalculation.created_date;
                    lobjLifeDeduction.icdoBenefitLifeDeduction.idtDateofBirth = ibusPerson.icdoPerson.date_of_birth;
                    lobjLifeDeduction.icdoBenefitLifeDeduction.iblnValueEntered = true;
                    lobjLifeDeduction.icdoBenefitLifeDeduction.life_insurance_type_value = istrLifeInsuranceTypeValue;
                }
            }
        }

        private bool IsHealthCriteriaModified()
        {
            if (ibusBenefitHealthDeduction.icdoBenefitGhdvDeduction.ihstOldValues.Count > 0)
            {
                if (Convert.ToString(ibusBenefitHealthDeduction.icdoBenefitGhdvDeduction.ihstOldValues["health_insurance_type_value"]) !=
                    ibusBenefitHealthDeduction.icdoBenefitGhdvDeduction.health_insurance_type_value)
                    return true;
                if (Convert.ToString(ibusBenefitHealthDeduction.icdoBenefitGhdvDeduction.ihstOldValues["coverage_code"]) !=
                    ibusBenefitHealthDeduction.icdoBenefitGhdvDeduction.coverage_code)
                    return true;
                if (Convert.ToString(ibusBenefitHealthDeduction.icdoBenefitGhdvDeduction.ihstOldValues["plan_option_value"]) !=
                    ibusBenefitHealthDeduction.icdoBenefitGhdvDeduction.plan_option_value)
                    return true;
                if (Convert.ToDecimal(ibusBenefitHealthDeduction.icdoBenefitGhdvDeduction.ihstOldValues["low_income_credit"]) !=
                    ibusBenefitHealthDeduction.icdoBenefitGhdvDeduction.low_income_credit)
                    return true;
                if (Convert.ToString(ibusBenefitHealthDeduction.icdoBenefitGhdvDeduction.ihstOldValues["cobra_type_value"]) !=
                    ibusBenefitHealthDeduction.icdoBenefitGhdvDeduction.cobra_type_value)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Function called Beforepersistchange to calculate premium and set Object state for Health
        /// </summary>
        public void ProcessHealth()
        {
            if (!String.IsNullOrEmpty(ibusBenefitHealthDeduction.icdoBenefitGhdvDeduction.coverage_code))
            {
                LoadHealthPlanOption();
                ibusBenefitHealthDeduction.LoadGHDVObjectFromDeduction();
                ibusBenefitHealthDeduction.ibusPersonAccountGHDV.LoadRateStructure(icdoBenefitCalculation.created_date);
                ibusBenefitHealthDeduction.ibusPersonAccountGHDV.LoadCoverageRefID();
                ibusBenefitHealthDeduction.ibusPersonAccountGHDV.GetMonthlyPremiumAmountByRefID(icdoBenefitCalculation.created_date);
                ibusBenefitHealthDeduction.icdoBenefitGhdvDeduction.computed_premium_amount = ibusBenefitHealthDeduction.ibusPersonAccountGHDV.icdoPersonAccountGhdv.MonthlyPremiumAmount;

                ibusBenefitHealthDeduction.icdoBenefitGhdvDeduction.benefit_calculation_id = icdoBenefitCalculation.benefit_calculation_id;
                ibusBenefitHealthDeduction.icdoBenefitGhdvDeduction.benefit_deduction_ghdv_value = busConstant.BenefitHealthDeduction;
                ibusBenefitHealthDeduction.icdoBenefitGhdvDeduction.ienuObjectState = ObjectState.Insert;
                if (ibusBenefitHealthDeduction.icdoBenefitGhdvDeduction.benefit_ghdv_deduction_id > 0)
                    ibusBenefitHealthDeduction.icdoBenefitGhdvDeduction.ienuObjectState = ObjectState.Update;
                this.iarrChangeLog.Add(ibusBenefitHealthDeduction.icdoBenefitGhdvDeduction);
            }
        }

        private bool IsDentalCriteriaModified()
        {
            if (ibusBenefitDentalDeduction.icdoBenefitGhdvDeduction.ihstOldValues.Count > 0)
            {
                if (Convert.ToString(ibusBenefitDentalDeduction.icdoBenefitGhdvDeduction.ihstOldValues["dental_insurance_type_value"]) !=
                    ibusBenefitDentalDeduction.icdoBenefitGhdvDeduction.dental_insurance_type_value)
                    return true;
                if (Convert.ToString(ibusBenefitDentalDeduction.icdoBenefitGhdvDeduction.ihstOldValues["level_of_coverage_value"]) !=
                    ibusBenefitDentalDeduction.icdoBenefitGhdvDeduction.level_of_coverage_value)
                    return true;
                if (Convert.ToString(ibusBenefitDentalDeduction.icdoBenefitGhdvDeduction.ihstOldValues["cobra_type_value"]) !=
                    ibusBenefitDentalDeduction.icdoBenefitGhdvDeduction.cobra_type_value)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Function called Beforepersistchange to calculate premium and set Object state for Dental
        /// </summary>
        public void ProcessDental()
        {
            if (!String.IsNullOrEmpty(ibusBenefitDentalDeduction.icdoBenefitGhdvDeduction.level_of_coverage_value))
            {
                ibusBenefitDentalDeduction.icdoBenefitGhdvDeduction.benefit_calculation_id = icdoBenefitCalculation.benefit_calculation_id;
                ibusBenefitDentalDeduction.icdoBenefitGhdvDeduction.computed_premium_amount =
                    busRateHelper.GetDentalVisionPremiumAmount("cdoPersonAccountGhdv.GetDentalPremiumAmount"
                            , ibusBenefitDentalDeduction.icdoBenefitGhdvDeduction.dental_insurance_type_value ?? string.Empty,
                            ibusBenefitDentalDeduction.icdoBenefitGhdvDeduction.level_of_coverage_value ?? string.Empty,
                            iintOrgPlanidDental/*org plan id*/, icdoBenefitCalculation.created_date);
                ibusBenefitDentalDeduction.icdoBenefitGhdvDeduction.benefit_deduction_ghdv_value = busConstant.BenefitDentalDeduction;
                if (ibusBenefitDentalDeduction.icdoBenefitGhdvDeduction.benefit_ghdv_deduction_id > 0)
                    ibusBenefitDentalDeduction.icdoBenefitGhdvDeduction.ienuObjectState = ObjectState.Update;
                this.iarrChangeLog.Add(ibusBenefitDentalDeduction.icdoBenefitGhdvDeduction);
            }
        }

        private bool IsVisionCriteriaModified()
        {
            if (ibusBenefitVisionDeduction.icdoBenefitGhdvDeduction.ihstOldValues.Count > 0)
            {
                if (Convert.ToString(ibusBenefitVisionDeduction.icdoBenefitGhdvDeduction.ihstOldValues["vision_insurance_type_value"]) !=
                    ibusBenefitVisionDeduction.icdoBenefitGhdvDeduction.vision_insurance_type_value)
                    return true;
                if (Convert.ToString(ibusBenefitVisionDeduction.icdoBenefitGhdvDeduction.ihstOldValues["level_of_coverage_value"]) !=
                    ibusBenefitVisionDeduction.icdoBenefitGhdvDeduction.level_of_coverage_value)
                    return true;
                if (Convert.ToString(ibusBenefitVisionDeduction.icdoBenefitGhdvDeduction.ihstOldValues["cobra_type_value"]) !=
                    ibusBenefitVisionDeduction.icdoBenefitGhdvDeduction.cobra_type_value)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Function called Beforepersistchange to calculate premium and set Object state for Vision
        /// </summary>
        public void ProcessVision()
        {
            if (!String.IsNullOrEmpty(ibusBenefitVisionDeduction.icdoBenefitGhdvDeduction.level_of_coverage_value))
            {
                ibusBenefitVisionDeduction.icdoBenefitGhdvDeduction.benefit_calculation_id = icdoBenefitCalculation.benefit_calculation_id;
                ibusBenefitVisionDeduction.icdoBenefitGhdvDeduction.computed_premium_amount =
                    busRateHelper.GetDentalVisionPremiumAmount("cdoPersonAccountGhdv.GetVisionPremiumAmount",
                            ibusBenefitVisionDeduction.icdoBenefitGhdvDeduction.vision_insurance_type_value ?? string.Empty,
                            ibusBenefitVisionDeduction.icdoBenefitGhdvDeduction.level_of_coverage_value ?? string.Empty,
                            iintOrgPlanidVision/*org plan id*/, icdoBenefitCalculation.created_date);
                ibusBenefitVisionDeduction.icdoBenefitGhdvDeduction.benefit_deduction_ghdv_value = busConstant.BenefitVisionDeduction;
                ibusBenefitVisionDeduction.icdoBenefitGhdvDeduction.ienuObjectState = ObjectState.Insert;
                if (ibusBenefitVisionDeduction.icdoBenefitGhdvDeduction.benefit_ghdv_deduction_id > 0)
                    ibusBenefitVisionDeduction.icdoBenefitGhdvDeduction.ienuObjectState = ObjectState.Update;
                this.iarrChangeLog.Add(ibusBenefitVisionDeduction.icdoBenefitGhdvDeduction);
            }
        }

        private bool IsLTCCriteriaModified()
        {
            foreach (busBenefitLtcDeduction lobjLtcDeduction in iclbBenefitLtcMemberDeduction)
            {
                if (lobjLtcDeduction.icdoBenefitLtcDeduction.ihstOldValues.Count > 0)
                {
                    //if (!string.IsNullOrEmpty(Convert.ToString(lobjLtcDeduction.icdoBenefitLtcDeduction.ihstOldValues["ltc_insurance_type_value"])))
                    //{
                    if (lobjLtcDeduction.icdoBenefitLtcDeduction.ihstOldValues["ltc_insurance_type_value"] != DBNull.Value)
                    {
                        if (Convert.ToString(lobjLtcDeduction.icdoBenefitLtcDeduction.ihstOldValues["ltc_insurance_type_value"]) !=
                                lobjLtcDeduction.icdoBenefitLtcDeduction.ltc_insurance_type_value)
                            return true;
                    }
                    //}
                }
            }
            foreach (busBenefitLtcDeduction lobjLtcDeduction in iclbBenefitLtcSpouseDeduction)
            {
                if (lobjLtcDeduction.icdoBenefitLtcDeduction.ihstOldValues.Count > 0)
                {
                    //if (!string.IsNullOrEmpty(Convert.ToString(lobjLtcDeduction.icdoBenefitLtcDeduction.ihstOldValues["ltc_insurance_type_value"])))
                    if (lobjLtcDeduction.icdoBenefitLtcDeduction.ihstOldValues["ltc_insurance_type_value"] != DBNull.Value)
                    {
                        if (Convert.ToString(lobjLtcDeduction.icdoBenefitLtcDeduction.ihstOldValues["ltc_insurance_type_value"]) !=
                                lobjLtcDeduction.icdoBenefitLtcDeduction.ltc_insurance_type_value)
                            return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Function called Beforepersistchange to calculate premium and set Object state for LTC Member
        /// </summary>
        public void ProcessLTCMember()
        {
            //UAT PIR:1733. Age Calculated As of Retirement Date.
            DateTime ldtPrevYearLastDay = new DateTime(icdoBenefitCalculation.retirement_date.Year - 1, 12, 31);
            iintPersonAge = busGlobalFunctions.CalulateAge(ibusPerson.icdoPerson.date_of_birth, ldtPrevYearLastDay);
            if (iintOrgPlanidLTC == 0)
                iintOrgPlanidLTC = ibusPerson.LoadDefaultOrgPlanIdByPlanId(busConstant.PlanIdLTC);
            foreach (busBenefitLtcDeduction lobjLtcDeduction in iclbBenefitLtcMemberDeduction)
            {
                if (!String.IsNullOrEmpty(lobjLtcDeduction.icdoBenefitLtcDeduction.ltc_insurance_type_value))
                {
                    lobjLtcDeduction.icdoBenefitLtcDeduction.benefit_calculation_id = icdoBenefitCalculation.benefit_calculation_id;
                    lobjLtcDeduction.icdoBenefitLtcDeduction.computed_premium_amount =
                        busRateHelper.GetLTCPremiumAmount(iintOrgPlanidLTC,
                                                          lobjLtcDeduction.icdoBenefitLtcDeduction.ltc_insurance_type_value ?? string.Empty,
                                                          lobjLtcDeduction.icdoBenefitLtcDeduction.level_of_coverage_value ?? string.Empty,
                                                          icdoBenefitCalculation.created_date, iintPersonAge, null, iobjPassInfo);
                    idecTotalLtcPremium += lobjLtcDeduction.icdoBenefitLtcDeduction.computed_premium_amount;
                    if (lobjLtcDeduction.icdoBenefitLtcDeduction.benefit_ltc_deduction_id > 0)
                        lobjLtcDeduction.icdoBenefitLtcDeduction.ienuObjectState = ObjectState.Update;
                    else
                        lobjLtcDeduction.icdoBenefitLtcDeduction.ienuObjectState = ObjectState.Insert;
                }
            }
        }

        /// <summary>
        /// Function called Beforepersistchange to calculate premium and set Object state for LTC Spouse
        /// </summary>
        public void ProcessLTCSpouse()
        {
            busPerson lobjPerson = new busPerson();
            lobjPerson.icdoPerson = new cdoPerson();
            lobjPerson.FindPerson(iintSpousePersonid);
            int lintSpouseAge = busGlobalFunctions.CalulateAge(lobjPerson.icdoPerson.date_of_birth, new DateTime(icdoBenefitCalculation.created_date.Year - 1, 12, 31));
            foreach (busBenefitLtcDeduction lobjLtcDeduction in iclbBenefitLtcSpouseDeduction)
            {
                if (!String.IsNullOrEmpty(lobjLtcDeduction.icdoBenefitLtcDeduction.ltc_insurance_type_value))
                {
                    lobjLtcDeduction.icdoBenefitLtcDeduction.benefit_calculation_id = icdoBenefitCalculation.benefit_calculation_id;
                    lobjLtcDeduction.icdoBenefitLtcDeduction.computed_premium_amount =
                        busRateHelper.GetLTCPremiumAmount(iintOrgPlanidLTC,
                                                          lobjLtcDeduction.icdoBenefitLtcDeduction.ltc_insurance_type_value ?? string.Empty,
                                                          lobjLtcDeduction.icdoBenefitLtcDeduction.level_of_coverage_value ?? string.Empty,
                                                          icdoBenefitCalculation.created_date, lintSpouseAge, null, iobjPassInfo);
                    idecTotalLtcPremium += lobjLtcDeduction.icdoBenefitLtcDeduction.computed_premium_amount;
                    if (lobjLtcDeduction.icdoBenefitLtcDeduction.benefit_ltc_deduction_id > 0)
                        lobjLtcDeduction.icdoBenefitLtcDeduction.ienuObjectState = ObjectState.Update;
                    else
                        lobjLtcDeduction.icdoBenefitLtcDeduction.ienuObjectState = ObjectState.Insert;
                }
            }
        }

        private bool IsLifeCriteriaModified()
        {
            foreach (busBenefitLifeDeduction lobjLifeDeduction in iclbBenefitLifeDeduction)
            {
                if (lobjLifeDeduction.icdoBenefitLifeDeduction.ihstOldValues.Count > 0)
                {
                    if (Convert.ToDecimal(lobjLifeDeduction.icdoBenefitLifeDeduction.ihstOldValues["coverage_amount"]) !=
                         Convert.ToDecimal(lobjLifeDeduction.icdoBenefitLifeDeduction.coverage_amount))
                        return true;
                    if (Convert.ToString(lobjLifeDeduction.icdoBenefitLifeDeduction.ihstOldValues["life_insurance_type_value"]) !=
                        Convert.ToString(lobjLifeDeduction.icdoBenefitLifeDeduction.life_insurance_type_value))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Function called Beforepersistchange to calculate premium and set Object state for Life
        /// </summary>
        public void ProcessLife()
        {
            decimal ldecADAndDBasicRate = 0.0000M;
            decimal ldecADAndDSupplementalRate = 0.0000M;
            idecTotalLifePremium = 0.0M;
            if (iintOrgPlanidLife == 0)
                iintOrgPlanidLife = ibusPerson.LoadDefaultOrgPlanIdByPlanId(busConstant.PlanIdGroupLife);
            foreach (busBenefitLifeDeduction lobjLifeDeduction in iclbBenefitLifeDeduction)
            {
                if (lobjLifeDeduction.icdoBenefitLifeDeduction.coverage_amount != 0.00M)
                {
                    decimal ldecEmployerPremiumAmount = 0.0M;
                    lobjLifeDeduction.icdoBenefitLifeDeduction.benefit_calculation_id = icdoBenefitCalculation.benefit_calculation_id;
                    lobjLifeDeduction.icdoBenefitLifeDeduction.computed_premium_amount =
                        busRateHelper.GetLifePremiumAmount(lobjLifeDeduction.icdoBenefitLifeDeduction.life_insurance_type_value ?? string.Empty,
                            lobjLifeDeduction.icdoBenefitLifeDeduction.level_of_coverage_value ?? string.Empty,
                            lobjLifeDeduction.icdoBenefitLifeDeduction.coverage_amount,
                            iintPersonAge/*age*/,
                            iintOrgPlanidLife/*org plan id*/, icdoBenefitCalculation.created_date, ref ldecEmployerPremiumAmount, null, iobjPassInfo,
                            ref ldecADAndDBasicRate, ref ldecADAndDSupplementalRate);
                    idecTotalLifePremium += lobjLifeDeduction.icdoBenefitLifeDeduction.computed_premium_amount + ldecEmployerPremiumAmount; // UAT PIR ID 1081
                    if (lobjLifeDeduction.icdoBenefitLifeDeduction.benefit_life_deduction_id > 0)
                        lobjLifeDeduction.icdoBenefitLifeDeduction.ienuObjectState = ObjectState.Update;
                    else
                        lobjLifeDeduction.icdoBenefitLifeDeduction.ienuObjectState = ObjectState.Insert;

                    this.iarrChangeLog.Add(lobjLifeDeduction.icdoBenefitLifeDeduction);
                }
            }
        }

        private bool IsFedTaxCriteriaModified()
        {
            if (ibusBenefitPayeeFedTaxWithholding.icdoBenefitPayeeTaxWithholding.ihstOldValues.Count > 0)
            {
                if (!string.IsNullOrEmpty(ibusBenefitPayeeFedTaxWithholding.icdoBenefitPayeeTaxWithholding.tax_option_value) &&
                    (Convert.ToString(ibusBenefitPayeeFedTaxWithholding.icdoBenefitPayeeTaxWithholding.ihstOldValues["tax_option_value"]) !=
                    ibusBenefitPayeeFedTaxWithholding.icdoBenefitPayeeTaxWithholding.tax_option_value))
                    return true;
                if (Convert.ToString(ibusBenefitPayeeFedTaxWithholding.icdoBenefitPayeeTaxWithholding.ihstOldValues["marital_status_value"]) !=
                    ibusBenefitPayeeFedTaxWithholding.icdoBenefitPayeeTaxWithholding.marital_status_value)
                    return true;
                if (Convert.ToInt32(ibusBenefitPayeeFedTaxWithholding.icdoBenefitPayeeTaxWithholding.ihstOldValues["tax_allowance"]) !=
                    ibusBenefitPayeeFedTaxWithholding.icdoBenefitPayeeTaxWithholding.tax_allowance)
                    return true;
                if (Convert.ToDecimal(ibusBenefitPayeeFedTaxWithholding.icdoBenefitPayeeTaxWithholding.ihstOldValues["additional_tax_amount"]) !=
                    ibusBenefitPayeeFedTaxWithholding.icdoBenefitPayeeTaxWithholding.additional_tax_amount)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Function called Beforepersistchange to calculate tax and set Object state for Fed Tax
        /// </summary>
        public void ProcessFedTax()
        {
            // UAT PIR ID 1993
            decimal ldecTaxableAmount = ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.gross_monthly_benefit_amount;
            if (ibusBenefitCalculationOptions.IsNotNull())
                ldecTaxableAmount = ibusBenefitCalculationOptions.icdoBenefitCalculationOptions.taxable_amount;

            if (ibusBenefitApplication == null)
                LoadBenefitApplication();
            ibusBenefitPayeeFedTaxWithholding.icdoBenefitPayeeTaxWithholding.benefit_calculation_id = icdoBenefitCalculation.benefit_calculation_id;
            if (ibusBenefitCalculationOptions.ibusBenefitProvisionBenefitOption.IsNull())
                ibusBenefitCalculationOptions.LoadBenefitProvisionOption();
            if (ibusBenefitCalculationOptions.ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.benefit_option_value == busConstant.BenefitOptionRefund &&
                (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimate ||
                icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimateSubsequent)) //PIR 19594
            {
                if (ibusBenefitCalculationOptions.ibusBenefitCalculationPayee == null)
                    ibusBenefitCalculationOptions.LoadBenefitCalculationPayee();
                ibusBenefitPayeeFedTaxWithholding.icdoBenefitPayeeTaxWithholding.computed_tax_amount =
                    busPayeeAccountHelper.CalculateFlatTax(ldecTaxableAmount,
                                   icdoBenefitCalculation.created_date,
                                   ibusBenefitPayeeFedTaxWithholding.icdoBenefitPayeeTaxWithholding.tax_identifier_value,
                                   busConstant.PayeeAccountTaxRefFed22Tax,
                                   busConstant.Flag_No);
            }
            else
            {
                if (icdoBenefitCalculation.plso_requested_flag != busConstant.Flag_Yes)
                {
                    busPayeeAccountTaxWithholding lbusPayeeAccountTaxWithholding = new busPayeeAccountTaxWithholding() { icdoPayeeAccountTaxWithholding = new cdoPayeeAccountTaxWithholding() };
                    lbusPayeeAccountTaxWithholding.iblnSkipSave = true;
                    lbusPayeeAccountTaxWithholding.ibusPayeeAccount = new busPayeeAccount() { icdoPayeeAccount = new cdoPayeeAccount() { payee_account_id = -1 } };
                    lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.additional_tax_amount = ibusBenefitPayeeFedTaxWithholding.icdoBenefitPayeeTaxWithholding.additional_tax_amount;
                    lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_identifier_value = busConstant.PayeeAccountTaxIdentifierFedTax;
                    lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_allowance = ibusBenefitPayeeFedTaxWithholding.icdoBenefitPayeeTaxWithholding.tax_allowance;
                    lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.marital_status_value = ibusBenefitPayeeFedTaxWithholding.icdoBenefitPayeeTaxWithholding.marital_status_value == busConstant.MaritalStatusMarriedWithholdAtSingleRate
                                        ? busConstant.PersonMaritalStatusSingle : ibusBenefitPayeeFedTaxWithholding.icdoBenefitPayeeTaxWithholding.marital_status_value;
                    lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.benefit_distribution_type_value = busConstant.BenefitDistributionMonthlyBenefit;
                    ibusBenefitPayeeFedTaxWithholding.icdoBenefitPayeeTaxWithholding.computed_tax_amount = lbusPayeeAccountTaxWithholding.CalculateW4PTaxBridgeWithholdingAmount(ldecTaxableAmount, icdoBenefitCalculation.created_date);
                }
                else
                {
                    if (ibusBenefitCalculationOptions.ibusBenefitCalculationPayee == null)
                        ibusBenefitCalculationOptions.LoadBenefitCalculationPayee();
                    ibusBenefitPayeeFedTaxWithholding.icdoBenefitPayeeTaxWithholding.computed_tax_amount =
                        busPayeeAccountHelper.CalculateFlatTax(ldecTaxableAmount,
                                       icdoBenefitCalculation.created_date/*payment date*/,
                                       ibusBenefitPayeeFedTaxWithholding.icdoBenefitPayeeTaxWithholding.tax_identifier_value,
                                       busConstant.PayeeAccountTaxRefFed22Tax,
                                   busConstant.Flag_No);
                }
            }
            ibusBenefitPayeeFedTaxWithholding.icdoBenefitPayeeTaxWithholding.ienuObjectState = ObjectState.Insert;
            if (ibusBenefitPayeeFedTaxWithholding.icdoBenefitPayeeTaxWithholding.benefit_payee_tax_withholding_id > 0)
                ibusBenefitPayeeFedTaxWithholding.icdoBenefitPayeeTaxWithholding.ienuObjectState = ObjectState.Update;
            this.iarrChangeLog.Add(ibusBenefitPayeeFedTaxWithholding.icdoBenefitPayeeTaxWithholding);
        }

        private bool IsStateTaxCriteriaModified()
        {
            if (ibusBenefitPayeeStateTaxWithholding.icdoBenefitPayeeTaxWithholding.ihstOldValues.Count > 0)
            {
                if (!string.IsNullOrEmpty(ibusBenefitPayeeStateTaxWithholding.icdoBenefitPayeeTaxWithholding.tax_option_value) &&
                    (Convert.ToString(ibusBenefitPayeeStateTaxWithholding.icdoBenefitPayeeTaxWithholding.ihstOldValues["tax_option_value"]) !=
                    ibusBenefitPayeeStateTaxWithholding.icdoBenefitPayeeTaxWithholding.tax_option_value))
                    return true;
                if (Convert.ToString(ibusBenefitPayeeStateTaxWithholding.icdoBenefitPayeeTaxWithholding.ihstOldValues["marital_status_value"]) !=
                    ibusBenefitPayeeStateTaxWithholding.icdoBenefitPayeeTaxWithholding.marital_status_value)
                    return true;
                if (Convert.ToInt32(ibusBenefitPayeeStateTaxWithholding.icdoBenefitPayeeTaxWithholding.ihstOldValues["tax_allowance"]) !=
                    ibusBenefitPayeeStateTaxWithholding.icdoBenefitPayeeTaxWithholding.tax_allowance)
                    return true;
                if (Convert.ToDecimal(ibusBenefitPayeeStateTaxWithholding.icdoBenefitPayeeTaxWithholding.ihstOldValues["additional_tax_amount"]) !=
                    ibusBenefitPayeeStateTaxWithholding.icdoBenefitPayeeTaxWithholding.additional_tax_amount)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Function called Beforepersistchange to calculate tax and set Object state for State Tax
        /// </summary>
        public void ProcessStateTax()
        {
            // UAT PIR ID 1993
            decimal ldecTaxableAmount = ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.gross_monthly_benefit_amount;
            if (ibusBenefitCalculationOptions.IsNotNull())
                ldecTaxableAmount = ibusBenefitCalculationOptions.icdoBenefitCalculationOptions.taxable_amount;

            if (ibusBenefitApplication == null)
                LoadBenefitApplication();
            ibusBenefitPayeeStateTaxWithholding.icdoBenefitPayeeTaxWithholding.benefit_calculation_id = icdoBenefitCalculation.benefit_calculation_id;
            if (ibusBenefitCalculationOptions.ibusBenefitProvisionBenefitOption.IsNull())
                ibusBenefitCalculationOptions.LoadBenefitProvisionOption();
            if (ibusBenefitCalculationOptions.ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.benefit_option_value == busConstant.BenefitOptionRefund &&
                (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimate ||
                icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimateSubsequent)) //PIR 19594
            {
                if (ibusBenefitCalculationOptions.ibusBenefitCalculationPayee == null)
                    ibusBenefitCalculationOptions.LoadBenefitCalculationPayee();
                ibusBenefitPayeeStateTaxWithholding.icdoBenefitPayeeTaxWithholding.computed_tax_amount =
                   busPayeeAccountHelper.CalculateFlatTax(ldecTaxableAmount,
                                       icdoBenefitCalculation.created_date/*payment date*/,
                                       ibusBenefitPayeeStateTaxWithholding.icdoBenefitPayeeTaxWithholding.tax_identifier_value,
                                       busConstant.PayeeAccountTaxRefState22Tax, busConstant.Flag_No);
            }
            else
            {
                if (icdoBenefitCalculation.plso_requested_flag != busConstant.Flag_Yes)
                {
                    //need to give the taxable amount
                    ibusBenefitPayeeStateTaxWithholding.icdoBenefitPayeeTaxWithholding.computed_tax_amount =
                        busPayeeAccountHelper.CalculateFedOrStateTax(ldecTaxableAmount,
                                        ibusBenefitPayeeStateTaxWithholding.icdoBenefitPayeeTaxWithholding.tax_allowance,
                                        icdoBenefitCalculation.created_date,
                                        ibusBenefitPayeeStateTaxWithholding.icdoBenefitPayeeTaxWithholding.marital_status_value == busConstant.MaritalStatusMarriedWithholdAtSingleRate
                                        ? busConstant.PersonMaritalStatusSingle : ibusBenefitPayeeStateTaxWithholding.icdoBenefitPayeeTaxWithholding.marital_status_value,
                                        ibusBenefitPayeeStateTaxWithholding.icdoBenefitPayeeTaxWithholding.tax_identifier_value,
                                        ibusBenefitPayeeStateTaxWithholding.icdoBenefitPayeeTaxWithholding.additional_tax_amount);
                }
                else
                {
                    if (ibusBenefitCalculationOptions.ibusBenefitCalculationPayee == null)
                        ibusBenefitCalculationOptions.LoadBenefitCalculationPayee();
                    ibusBenefitPayeeStateTaxWithholding.icdoBenefitPayeeTaxWithholding.computed_tax_amount =
                        busPayeeAccountHelper.CalculateFlatTax(ldecTaxableAmount,
                                       icdoBenefitCalculation.created_date/*payment date*/,
                                       ibusBenefitPayeeStateTaxWithholding.icdoBenefitPayeeTaxWithholding.tax_identifier_value,
                                       busConstant.PayeeAccountTaxRefState22Tax,
                                       busConstant.Flag_No);
                }
            }
            ibusBenefitPayeeStateTaxWithholding.icdoBenefitPayeeTaxWithholding.ienuObjectState = ObjectState.Insert;
            if (ibusBenefitPayeeStateTaxWithholding.icdoBenefitPayeeTaxWithholding.benefit_payee_tax_withholding_id > 0)
                ibusBenefitPayeeStateTaxWithholding.icdoBenefitPayeeTaxWithholding.ienuObjectState = ObjectState.Update;
            this.iarrChangeLog.Add(ibusBenefitPayeeStateTaxWithholding.icdoBenefitPayeeTaxWithholding);
        }

        /// <summary>
        /// Function called Beforepersistchange to set Object state for deduction Summary
        /// </summary>
        public void ProcessDeductionSummary()
        {
            ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.benefit_calculation_id = icdoBenefitCalculation.benefit_calculation_id;
            if (ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.benefit_deduction_summary_id > 0)
                ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.ienuObjectState = ObjectState.Update;
            else
            {
                ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.ienuObjectState = ObjectState.Insert;
            }
            //PIR 15721 Change Health premium logic = Health Premium + (Medicare Premium * Medicare Part D)
            decimal ldecMedicarePremium = busRateHelper.GetMedicarePartDPremiumAmountFromRef(icdoBenefitCalculation.created_date, null, iobjPassInfo);
            if (ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.ihstOldValues.Count == 0)
            {
                ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.health_overridden_amount =
                    (ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.health_overridden_amount == 0 ?
                    ibusBenefitHealthDeduction.icdoBenefitGhdvDeduction.computed_premium_amount :
                    ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.health_overridden_amount) +
                    (ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.medicare * ldecMedicarePremium);//PIR 15721
                ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.dental_overridden_amount =
                    ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.dental_overridden_amount == 0 ?
                    ibusBenefitDentalDeduction.icdoBenefitGhdvDeduction.computed_premium_amount :
                    ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.dental_overridden_amount;
                ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.vision_overridden_amount =
                    ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.vision_overridden_amount == 0 ?
                    ibusBenefitVisionDeduction.icdoBenefitGhdvDeduction.computed_premium_amount :
                    ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.vision_overridden_amount;
                ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.fit_overridden_amount =
                    ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.fit_overridden_amount == 0 ?
                    ibusBenefitPayeeFedTaxWithholding.icdoBenefitPayeeTaxWithholding.computed_tax_amount :
                    ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.fit_overridden_amount;
                ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.ndit_overridden_amount =
                    ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.ndit_overridden_amount == 0 ?
                    ibusBenefitPayeeStateTaxWithholding.icdoBenefitPayeeTaxWithholding.computed_tax_amount :
                    ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.ndit_overridden_amount;
                ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.ltc_overridden_amount =
                    ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.ltc_overridden_amount == 0 ?
                    idecTotalLtcPremium : ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.ltc_overridden_amount;
                ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.life_overridden_amount =
                    ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.life_overridden_amount == 0 ?
                    idecTotalLifePremium : ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.life_overridden_amount;
            }
            else
            {
                if (IsMedicareCriteriaModified() || IsHealthCriteriaModified()) //PIR 15721
                {
                    if (Convert.ToDecimal(ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.ihstOldValues["health_overridden_amount"]) ==
                                        ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.health_overridden_amount)
                    {
                        ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.health_overridden_amount =
                            ibusBenefitHealthDeduction.icdoBenefitGhdvDeduction.computed_premium_amount + (ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.medicare * ldecMedicarePremium);//PIR 15721
                    }
                }
                if (IsDentalCriteriaModified())
                {
                    if (Convert.ToDecimal(ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.ihstOldValues["dental_overridden_amount"]) ==
                                        ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.dental_overridden_amount)
                    {
                        ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.dental_overridden_amount =
                                        ibusBenefitDentalDeduction.icdoBenefitGhdvDeduction.computed_premium_amount;
                    }
                }
                if (IsVisionCriteriaModified())
                {
                    if (Convert.ToDecimal(ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.ihstOldValues["vision_overridden_amount"]) ==
                                        ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.vision_overridden_amount)
                    {
                        ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.vision_overridden_amount =
                                        ibusBenefitVisionDeduction.icdoBenefitGhdvDeduction.computed_premium_amount;
                    }
                }
                if (IsFedTaxCriteriaModified())
                {
                    if (Convert.ToDecimal(ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.ihstOldValues["fit_overridden_amount"]) ==
                                        ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.fit_overridden_amount)
                    {
                        ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.fit_overridden_amount =
                                        ibusBenefitPayeeFedTaxWithholding.icdoBenefitPayeeTaxWithholding.computed_tax_amount;
                    }
                }
                if (IsStateTaxCriteriaModified())
                {
                    if (Convert.ToDecimal(ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.ihstOldValues["ndit_overridden_amount"]) ==
                                        ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.ndit_overridden_amount)
                    {
                        ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.ndit_overridden_amount =
                                        ibusBenefitPayeeStateTaxWithholding.icdoBenefitPayeeTaxWithholding.computed_tax_amount;
                    }
                }
                if (IsLTCCriteriaModified())
                {
                    if (Convert.ToDecimal(ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.ihstOldValues["ltc_overridden_amount"]) ==
                                        ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.ltc_overridden_amount)
                    {
                        ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.ltc_overridden_amount = idecTotalLtcPremium;
                    }
                }
                if (IsLifeCriteriaModified())
                {
                    if (Convert.ToDecimal(ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.ihstOldValues["life_overridden_amount"]) ==
                                        ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.life_overridden_amount)
                    {
                        ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.life_overridden_amount = idecTotalLifePremium;
                    }
                }
            }

            ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.net_health_insurance_premium_amount =
                ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.health_overridden_amount;
            // PIR 15271 and 20269 : Removing rhic_overridden_amount    
            //- ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.rhic_overridden_amount;

            ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.net_monthly_pension_benefit_amount =
                    ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.gross_monthly_benefit_amount -
                   (ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.fit_overridden_amount +
                   ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.ndit_overridden_amount +
                   ((ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.net_health_insurance_premium_amount > 0) ?
                   ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.net_health_insurance_premium_amount : 0) +//health overridden amount - rhic amount
                   ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.vision_overridden_amount +
                   ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.dental_overridden_amount +
                   ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.life_overridden_amount +
                   ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.ltc_overridden_amount +
                   ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.miscellaneous_deduction_amount);
            this.iarrChangeLog.Add(ibusBenefitDeductionSummary.icdoBenefitDeductionSummary);
        }

        //PIR 15721
        private bool IsMedicareCriteriaModified() => ((ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.ihstOldValues.Count > 0) && (Convert.ToInt32(ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.ihstOldValues["medicare"]) !=
                    ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.medicare));

        public decimal idecTotalDeductions
        {
            get
            {
                if (ibusBenefitDeductionSummary.IsNotNull())
                    return ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.fit_overridden_amount +
                            ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.ndit_overridden_amount +
                           ((ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.net_health_insurance_premium_amount > 0) ?
                           ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.net_health_insurance_premium_amount : 0) +//health overridden amount - rhic amount
                            ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.vision_overridden_amount +
                            ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.dental_overridden_amount +
                            ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.life_overridden_amount +
                            ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.ltc_overridden_amount +
                            ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.miscellaneous_deduction_amount;
                return 0;
            }
        }

        #endregion

        #region Rules

        public bool IsActiveOnlyInBasicCoverage()
        {
            if (_iclbBenefitLifeDeduction == null)
                LoadBenefitLifeDeductions();
            bool lblnResult = true;
            foreach (busBenefitLifeDeduction lobjLife in _iclbBenefitLifeDeduction)
            {
                if ((lobjLife.icdoBenefitLifeDeduction.level_of_coverage_value != busConstant.LevelofCoverage_Basic) &&
                    (lobjLife.icdoBenefitLifeDeduction.coverage_amount != 0.0M))
                {
                    lblnResult = false;
                    break;
                }
            }
            return lblnResult;
        }

        public string IsValidCoverageAmount()
        {
            busPersonAccountLife lobjPersonLife = new busPersonAccountLife();
            foreach (busBenefitLifeDeduction lobjLife in iclbBenefitLifeDeduction)
            {
                if (lobjLife.icdoBenefitLifeDeduction.iblnValueEntered)
                {
                    if (lobjLife.icdoBenefitLifeDeduction.level_of_coverage_value == busConstant.LevelofCoverage_Basic &&
                        !lobjPersonLife.IsValidCoverageAmount(lobjLife.icdoBenefitLifeDeduction.level_of_coverage_value, istrLifeInsuranceTypeValue,
                            lobjLife.icdoBenefitLifeDeduction.coverage_amount, DateTime.Today))
                        return "B";
                    else if (lobjLife.icdoBenefitLifeDeduction.level_of_coverage_value == busConstant.LevelofCoverage_DependentSupplemental &&
                            !lobjPersonLife.IsValidCoverageAmount(lobjLife.icdoBenefitLifeDeduction.level_of_coverage_value, istrLifeInsuranceTypeValue,
                            lobjLife.icdoBenefitLifeDeduction.coverage_amount, DateTime.Today))
                        return "DS";
                    else if (lobjLife.icdoBenefitLifeDeduction.level_of_coverage_value == busConstant.LevelofCoverage_SpouseSupplemental &&
                            !lobjPersonLife.IsValidCoverageAmount(lobjLife.icdoBenefitLifeDeduction.level_of_coverage_value, istrLifeInsuranceTypeValue,
                            lobjLife.icdoBenefitLifeDeduction.coverage_amount, DateTime.Today))
                        return "SS";
                    else if (lobjLife.icdoBenefitLifeDeduction.level_of_coverage_value == busConstant.LevelofCoverage_Supplemental &&
                            !lobjPersonLife.IsValidCoverageAmount(lobjLife.icdoBenefitLifeDeduction.level_of_coverage_value, istrLifeInsuranceTypeValue,
                            lobjLife.icdoBenefitLifeDeduction.coverage_amount, DateTime.Today))
                        return "S";
                }
            }
            return "Y";
        }

        public bool IsRetireeAttainedAge65()
        {
            if (_iclbBenefitLifeDeduction == null)
                LoadBenefitLifeDeductions();
            foreach (busBenefitLifeDeduction lobjLife in _iclbBenefitLifeDeduction)
            {
                if (lobjLife.icdoBenefitLifeDeduction.iblnValueEntered)
                {
                    if (istrLifeInsuranceTypeValue == busConstant.LifeInsuranceTypeRetireeMember)
                    {
                        if (busGlobalFunctions.CalulateAge(ibusPerson.icdoPerson.date_of_birth, icdoBenefitCalculation.created_date) >= 65)
                            return true;
                        else
                            return false;
                    }
                }
            }
            return false;
        }

        public bool IsValidDeductionForRetirement()
        {
            if (icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement)
            {
                if (ibusBenefitCalculationOptions != null)
                {
                    ibusBenefitCalculationOptions.LoadBenefitCalculationPayee();
                    if (ibusBenefitCalculationOptions.ibusBenefitCalculationPayee.icdoBenefitCalculationPayee.account_relationship_value ==
                        busConstant.AccountRelationshipMember)
                        return true;
                    return false;
                }
            }
            return true;
        }

        public bool IsValidDeductionForDeath()
        {
            if (icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath)
            {
                if (ibusBenefitCalculationOptions != null)
                {
                    ibusBenefitCalculationOptions.LoadBenefitCalculationPayee();
                    if (ibusBenefitCalculationOptions.ibusBenefitCalculationPayee.icdoBenefitCalculationPayee.family_relationship_value ==
                        busConstant.PersonContactTypeSpouse)
                        return true;
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region Method to populate DDL
        public Collection<cdoOrgPlanGroupHealthMedicarePartDCoverageRef> LoadCoverageCodeByFilter()
        {
            return ibusBenefitHealthDeduction.ibusPersonAccountGHDV.LoadCoverageCodeByFilter();
        }

        public Collection<cdoCodeValue> LoadLevelOfCoverageDental()
        {
            DataTable ldtbList = iobjPassInfo.isrvDBCache.GetCodeValues(408, busConstant.PlanIdDental.ToString(), null, null);
            Collection<cdoCodeValue> lclcCodeValue = Sagitec.DataObjects.doBase.GetCollection<cdoCodeValue>(ldtbList);
            return lclcCodeValue;
        }

        public Collection<cdoCodeValue> LoadLevelOfCoverageVision()
        {
            DataTable ldtbList = iobjPassInfo.isrvDBCache.GetCodeValues(408, busConstant.PlanIdVision.ToString(), null, null);
            Collection<cdoCodeValue> lclcCodeValue = Sagitec.DataObjects.doBase.GetCollection<cdoCodeValue>(ldtbList);
            return lclcCodeValue;
        }

        public Collection<cdoCodeValue> LoadHealthInsuranceTypeByPlan()
        {
            DataTable ldtbList = iobjPassInfo.isrvDBCache.GetCodeValues(345, busConstant.PlanIdGroupHealth.ToString(), null, null);
            Collection<cdoCodeValue> lclcCodeValue = Sagitec.DataObjects.doBase.GetCollection<cdoCodeValue>(ldtbList);
            return lclcCodeValue;
        }

        public Collection<cdoCodeValue> LoadTaxOptionForFedTax()
        {
            Collection<cdoCodeValue> lclbTaxOption = new Collection<cdoCodeValue>();
            DataTable ldtbTaxOptions = iobjPassInfo.isrvDBCache.GetCodeValues(2218);
            foreach (DataRow dr in ldtbTaxOptions.Rows)
            {
                cdoCodeValue lobjCodeValue = new cdoCodeValue();
                if (dr["data2"].ToString() == ibusBenefitPayeeFedTaxWithholding.icdoBenefitPayeeTaxWithholding.tax_identifier_value)
                {
                    if ((icdoBenefitCalculation.plso_requested_flag == busConstant.Flag_Yes)
                    && (dr["data1"].ToString() == busConstant.Flag_Yes))
                    {
                        lobjCodeValue.LoadData(dr);
                        lclbTaxOption.Add(lobjCodeValue);
                    }
                    else if (icdoBenefitCalculation.plso_requested_flag != busConstant.Flag_Yes)
                    {
                        if (dr["data1"].ToString() == busConstant.Flag_No)
                        {
                            lobjCodeValue.LoadData(dr);
                            lclbTaxOption.Add(lobjCodeValue);
                        }
                    }
                }
            }
            return lclbTaxOption;
        }

        public Collection<cdoCodeValue> LoadTaxOptionForStateTax()
        {
            Collection<cdoCodeValue> lclbTaxOption = new Collection<cdoCodeValue>();
            DataTable ldtbTaxOptions = iobjPassInfo.isrvDBCache.GetCodeValues(2218);
            foreach (DataRow dr in ldtbTaxOptions.Rows)
            {
                cdoCodeValue lobjCodeValue = new cdoCodeValue();
                if (dr["data2"].ToString() == ibusBenefitPayeeStateTaxWithholding.icdoBenefitPayeeTaxWithholding.tax_identifier_value)
                {
                    if ((icdoBenefitCalculation.plso_requested_flag == busConstant.Flag_Yes)
                    && (dr["data1"].ToString() == busConstant.Flag_Yes))
                    {
                        lobjCodeValue.LoadData(dr);
                        lclbTaxOption.Add(lobjCodeValue);
                    }
                    else if (icdoBenefitCalculation.plso_requested_flag != busConstant.Flag_Yes)
                    {
                        if (dr["data1"].ToString() == busConstant.Flag_No)
                        {
                            lobjCodeValue.LoadData(dr);
                            lclbTaxOption.Add(lobjCodeValue);
                        }
                    }
                }
            }
            return lclbTaxOption;
        }
        #endregion

        #region Properties for Correspondence

        private busBenefitCalculation _ibusBenefitCalculation;
        public busBenefitCalculation ibusBenefitCalculation
        {
            get { return _ibusBenefitCalculation; }
            set { _ibusBenefitCalculation = value; }
        }

        public int iintCheckMiscAmtAvailable
        {
            get
            {
                if (ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.miscellaneous_deduction_amount != 0.0M)
                    return 1;
                else
                    return 0;
            }
        }

        public string istrRHICOptionName
        {
            get
            {
                if (ibusBenefitCalculation.IsNotNull())
                {
                    if (ibusBenefitCalculation.icdoBenefitCalculation.rhic_option_value.IsNotNullOrEmpty())
                    {
                        return ibusBenefitCalculation.icdoBenefitCalculation.rhic_option_description;
                    }
                    else
                    {
                        return (iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1905, busConstant.RHICOptionStandard));
                    }
                }
                return string.Empty;
            }
        }

        #endregion

        #region Methods for Correspondence

        public override busBase GetCorPerson()
        {
            return ibusPerson;
        }

        public void LoadBenefitCalculation()
        {
            if (_ibusBenefitCalculation == null)
                _ibusBenefitCalculation = new busBenefitCalculation();
            _ibusBenefitCalculation.FindBenefitCalculation(icdoBenefitCalculation.benefit_calculation_id);
        }

        #endregion

        /// <summary>
        /// Reload the Coverage Code based on the Screen Data
        /// </summary>
        /// <returns></returns>
        public ArrayList btnRefreshCoverageCodeList_Click()
        {
            ArrayList larrList = new ArrayList();
            LoadHealthPlanOption();
            ibusBenefitHealthDeduction.LoadGHDVObjectFromDeduction();
            ibusBenefitHealthDeduction.ibusPersonAccountGHDV.LoadRateStructure(icdoBenefitCalculation.created_date);
            ibusBenefitHealthDeduction.ibusPersonAccountGHDV.LoadCoverageRefID();
            larrList.Add(this);
            return larrList;
        }

        public int iintRetirementOrgID { get; set; }

        public void LoadHealthPlanOption()
        {
            if (ibusBenefitHealthDeduction.icdoBenefitGhdvDeduction.health_insurance_type_value == busConstant.HealthInsuranceTypeRetiree)
                ibusBenefitHealthDeduction.icdoBenefitGhdvDeduction.plan_option_value = null;
            else
            {
                if (ibusBenefitCalculation.IsNull()) LoadBenefitCalculation();
                if (ibusBenefitCalculation.ibusPersonAccount.IsNull()) ibusBenefitCalculation.LoadPersonAccount();
                DateTime ldteTempDate = new DateTime();
                iintRetirementOrgID = ibusBenefitCalculation.GetOrgIdAsLatestEmploymentOrgId(
                    ibusBenefitCalculation.ibusPersonAccount.icdoPersonAccount.person_account_id,
                    ibusBenefitCalculation.icdoBenefitCalculation.benefit_account_type_value, ref ldteTempDate);

                DataTable ldtbResult = Select<cdoOrgPlan>(new string[2] { "ORG_ID", "PLAN_ID" },
                                        new object[2] { iintRetirementOrgID, busConstant.PlanIdGroupHealth }, null, "PARTICIPATION_START_DATE DESC");
                if (ldtbResult.Rows.Count > 0)
                {
                    ibusBenefitHealthDeduction.ibusOrgPlan = new busOrgPlan { icdoOrgPlan = new cdoOrgPlan() };
                    ibusBenefitHealthDeduction.ibusOrgPlan.icdoOrgPlan.LoadData(ldtbResult.Rows[0]);
                    ibusBenefitHealthDeduction.icdoBenefitGhdvDeduction.plan_option_value = ibusBenefitHealthDeduction.ibusOrgPlan.icdoOrgPlan.plan_option_value;
                    ibusBenefitHealthDeduction.icdoBenefitGhdvDeduction.plan_option_description =
                            iobjPassInfo.isrvDBCache.GetCodeDescriptionString(406, ibusBenefitHealthDeduction.ibusOrgPlan.icdoOrgPlan.plan_option_value);
                }
            }
        }
    }
}
