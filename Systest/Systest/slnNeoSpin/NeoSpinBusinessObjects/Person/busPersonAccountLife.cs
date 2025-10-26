#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.CustomDataObjects;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
using System.Globalization;
using NeoSpin.DataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPersonAccountLife : busPersonAccountLifeGen
    {
        private Collection<busPersonAccountLifeOption> _iclbLifeOption;      
        public Collection<busPersonAccountLifeOption> iclbLifeOption
        {
            get { return _iclbLifeOption; }
            set { _iclbLifeOption = value; }
        }

        private Collection<busPersonAccountLifeHistory> _iclbLifeHistory;


        public Collection<busPersonAccountLifeHistory> iclbLifeHistory 
        {
            get {return _iclbLifeHistory;} 
            set{ _iclbLifeHistory = value; }
        }
       

        private decimal _idecTotalEEPremium;
        public decimal idecTotalEEPremium
        {
            get { return _idecTotalEEPremium; }
            set { _idecTotalEEPremium = value; }
        }

        private decimal _idecTotalERPremium;
        public decimal idecTotalERPremium
        {
            get { return _idecTotalERPremium; }
            set { _idecTotalERPremium = value; }
        }

        private decimal _idecTotalMonthlyPremium;
        public decimal idecTotalMonthlyPremium
        {
            get { return _idecTotalMonthlyPremium; }
            set { _idecTotalMonthlyPremium = value; }
        }

        private decimal _idecLifeBasicPremiumAmt;
        public decimal idecLifeBasicPremiumAmt
        {
            get { return _idecLifeBasicPremiumAmt; }
            set { _idecLifeBasicPremiumAmt = value; }
        }
        private decimal _idecLifeSupplementalPremiumAmount;
        public decimal idecLifeSupplementalPremiumAmount
        {
            get { return _idecLifeSupplementalPremiumAmount; }
            set { _idecLifeSupplementalPremiumAmount = value; }
        }

        private decimal _idecDependentSupplementalPremiumAmt;
        public decimal idecDependentSupplementalPremiumAmt
        {
            get { return _idecDependentSupplementalPremiumAmt; }
            set { _idecDependentSupplementalPremiumAmt = value; }
        }

        private decimal _idecSpouseSupplementalPremiumAmt;
        public decimal idecSpouseSupplementalPremiumAmt
        {
            get { return _idecSpouseSupplementalPremiumAmt; }
            set { _idecSpouseSupplementalPremiumAmt = value; }
        }

        private Collection<busPersonAccountLifeHistory> _iclbPreviousHistory;
        public Collection<busPersonAccountLifeHistory> iclbPreviousHistory
        {
            get { return _iclbPreviousHistory; }
            set { _iclbPreviousHistory = value; }
        }

        private Collection<busPersonAccountLifeOption> _iclbLifeOptionModified;
        public Collection<busPersonAccountLifeOption> iclbLifeOptionModified
        {
            get { return _iclbLifeOptionModified; }
            set { _iclbLifeOptionModified = value; }
        }

        //PIR-8932
        private Collection<busPersonAccountLifeOption> _iclbLifeOptionForCorrespondence;
        public Collection<busPersonAccountLifeOption> iclbLifeOptionForCorrespondence
        {
            get { return _iclbLifeOptionForCorrespondence; }
            set { _iclbLifeOptionForCorrespondence = value; }
        }

        // PIR 9115 functionality enable/disable property
        public string istrIsPIR9115Enabled
        {
            get
            {
                return busGlobalFunctions.GetData1ByCodeValue(52, "9115", iobjPassInfo);
            }
        }

        //PIR 10422
        private Collection<busPersonAccountLifeOption> _iclbPersonAccountLifeOption;
        public Collection<busPersonAccountLifeOption> iclbPersonAccountLifeOption
        {
            get { return _iclbPersonAccountLifeOption; }
            set { _iclbPersonAccountLifeOption = value; }
        }

        public DataTable idtbCachedLifeRate { get; set; }
        public int iintOldPayeeAccountID { get; set; }

        public decimal idecADAndDBasicRate { get; set; }
        public decimal idecADAndDSupplementalRate { get; set; }
        public decimal idecBasicCoverageAmount { get; set; }
        public decimal idecSuppCoverageAmount { get; set; }
        public decimal idecSpouseSuppCoverageAmount { get; set; }
        public decimal idecDepSuppCoverageAmount { get; set; }
        public bool iblnIscalledFromLossOfSuppLifeBatch { get; set; }
        public bool iblnIsFromMSS { get; set; } //PIR 10422

        public DateTime idtCoverageEndDate { get; set; }
        public DateTime idtCoverageEndDatePlus31Days { get; set; }

        public bool iblnIsHistoryInserted { get; set; }
        

        public string istrAllowOverlapHistory { get; set; }

        public void GetMonthlyPremiumAmount(DateTime adtEffectiveDate)
        {
            GetMonthlyPremiumAmount(adtEffectiveDate, ibusProviderOrgPlan.icdoOrgPlan.org_plan_id);
        }

        public void GetMonthlyPremiumAmount(DateTime adtEffectiveDate, int aintOrgPlanId)
        {
            _idecTotalMonthlyPremium = 0;
            _idecLifeBasicPremiumAmt = 0;
            _idecLifeSupplementalPremiumAmount = 0;
            _idecSpouseSupplementalPremiumAmt = 0;
            _idecDependentSupplementalPremiumAmt = 0;
            //prod pir 4260
            idecADAndDBasicRate = 0.0000M;
            idecADAndDSupplementalRate = 0.0000M;
            idecBasicCoverageAmount = 0.0M;
            idecSuppCoverageAmount = 0.0M;
            idecSpouseSuppCoverageAmount = 0.0M;
            idecDepSuppCoverageAmount = 0.0M;

            decimal ldecADAndDBasicRate = 0.0000M;
            decimal ldecADAndDSupplementalRate = 0.0000M;

            LoadMemberAge(adtEffectiveDate);

            foreach (busPersonAccountLifeOption lobjLifeOption in iclbLifeOption)
            {
                // Refresh Coverage Premiums
                lobjLifeOption.icdoPersonAccountLifeOption.Employee_Premium_Amount = 0M;
                lobjLifeOption.icdoPersonAccountLifeOption.Employer_Premium_Amount = 0M;

                //Load the History Object for the Given Life Option and the Date
                busPersonAccountLifeHistory lobjPALifeHistory = LoadHistoryByDate(lobjLifeOption, adtEffectiveDate);
                //PROD PIR 4114 : Dont Calculate Premium for Waived Plans
                if ((lobjPALifeHistory.icdoPersonAccountLifeHistory.person_account_life_history_id > 0) &&
                    (lobjPALifeHistory.icdoPersonAccountLifeHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled) &&
                    (lobjPALifeHistory.icdoPersonAccountLifeHistory.plan_option_status_value == busConstant.PlanOptionStatusValueEnrolled) &&
                    (lobjPALifeHistory.icdoPersonAccountLifeHistory.coverage_amount != 0.00M))
                {
                    // Load Monthly Premium only if the Premium waiver flag is NOT checked.
                    if (lobjPALifeHistory.icdoPersonAccountLifeHistory.premium_waiver_flag != busConstant.Flag_Yes)
                    {
                        decimal ldecEmployerPremiumAmount = 0.0M;
                        lobjLifeOption.icdoPersonAccountLifeOption.Employee_Premium_Amount = busRateHelper.GetLifePremiumAmount(
                            lobjPALifeHistory.icdoPersonAccountLifeHistory.life_insurance_type_value,
                            lobjPALifeHistory.icdoPersonAccountLifeHistory.level_of_coverage_value,
                            lobjPALifeHistory.icdoPersonAccountLifeHistory.coverage_amount,
                            icdoPersonAccountLife.Life_Insurance_Age, aintOrgPlanId,
                            adtEffectiveDate, ref ldecEmployerPremiumAmount, idtbCachedLifeRate, iobjPassInfo, ref ldecADAndDBasicRate, ref ldecADAndDSupplementalRate);

                        lobjLifeOption.icdoPersonAccountLifeOption.Employer_Premium_Amount = ldecEmployerPremiumAmount;

                        //Assigning the Total Premium Properties
                        _idecTotalMonthlyPremium += lobjLifeOption.icdoPersonAccountLifeOption.Monthly_Premium;
                        _idecTotalEEPremium += lobjLifeOption.icdoPersonAccountLifeOption.Employee_Premium_Amount;
                        _idecTotalERPremium += lobjLifeOption.icdoPersonAccountLifeOption.Employer_Premium_Amount;

                        //Assign the Property By Life Option
                        if (lobjLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_Basic)
                        {
                            _idecLifeBasicPremiumAmt = lobjLifeOption.icdoPersonAccountLifeOption.Monthly_Premium;
                            idecBasicCoverageAmount = lobjLifeOption.icdoPersonAccountLifeOption.coverage_amount;
                            idecADAndDBasicRate = ldecADAndDBasicRate;
                        }
                        else if (lobjLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_Supplemental)
                        {
                            _idecLifeSupplementalPremiumAmount = lobjLifeOption.icdoPersonAccountLifeOption.Monthly_Premium;
                            idecSuppCoverageAmount = lobjLifeOption.icdoPersonAccountLifeOption.coverage_amount;
                            idecADAndDSupplementalRate = ldecADAndDSupplementalRate;
                        }
                        else if (lobjLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_SpouseSupplemental)
                        {
                            _idecSpouseSupplementalPremiumAmt = lobjLifeOption.icdoPersonAccountLifeOption.Monthly_Premium;
                            idecSpouseSuppCoverageAmount = lobjLifeOption.icdoPersonAccountLifeOption.coverage_amount;
                        }
                        else if (lobjLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_DependentSupplemental)
                        {
                            _idecDependentSupplementalPremiumAmt = lobjLifeOption.icdoPersonAccountLifeOption.Monthly_Premium;
                            idecDepSuppCoverageAmount = lobjLifeOption.icdoPersonAccountLifeOption.coverage_amount;
                        }
                    }
                }
            }
        }

        public void GetMonthlyPremiumAmount()
        {
            if (idtPlanEffectiveDate == DateTime.MinValue)
                LoadPlanEffectiveDate();
            GetMonthlyPremiumAmount(idtPlanEffectiveDate);
        }

        //PIR 10422
        public void LoadPersonAccountLifeOptions()
        {
            if (iclbPersonAccountLifeOption == null)
                iclbPersonAccountLifeOption = new Collection<busPersonAccountLifeOption>();
            if (icdoPersonAccount.person_account_id != 0)
            {
                DataTable ldtbList = Select<cdoPersonAccountLifeOption>(
                    new string[1] { "person_account_id" },
                    new object[1] { icdoPersonAccount.person_account_id }, null, null);
                iclbPersonAccountLifeOption = GetCollection<busPersonAccountLifeOption>(ldtbList, "icdoPersonAccountLifeOption");
            }
        }

        /// <summary>
        /// Loading Group Life Option Grid objects in new mode.
        /// </summary>
        public void LoadLifeOption()
        {
            if (_iclbLifeOption == null)
                _iclbLifeOption = new Collection<busPersonAccountLifeOption>();
            //PIR-7987 Start
            int lintEnrollmentRequestId= Convert.ToInt32(DBFunction.DBExecuteScalar("cdoWssPersonAccountEnrollmentRequest.GetWssPersonAccountEnrollmentRequestId",
                            new object[1] { icdoPersonAccount.person_account_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework)); 
            
            this.LoadWSSEnrollmentRequestUpdate(lintEnrollmentRequestId);
            ibusWSSPersonAccountEnrollmentRequest.LoadMSSLifeOptions();
            //PIR-7987 End

            busPersonAccountLifeOption lobjBasicLifeOption = new busPersonAccountLifeOption();
            lobjBasicLifeOption.icdoPersonAccountLifeOption = new cdoPersonAccountLifeOption();
            lobjBasicLifeOption.icdoPersonAccountLifeOption.Sequence_ID = 1;
            lobjBasicLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value = busConstant.LevelofCoverage_Basic;
            lobjBasicLifeOption.icdoPersonAccountLifeOption.level_of_coverage_description =
                iobjPassInfo.isrvDBCache.GetCodeDescriptionString(busConstant.LevelofCoverage_CodeID, busConstant.LevelofCoverage_Basic);
            lobjBasicLifeOption.icdoPersonAccountLifeOption.plan_option_status_value = busConstant.PlanOptionStatusValueEnrolled;
            //prod pir 6947
            ibusWSSPersonAccountEnrollmentRequest.idecBasicCoverageAmount = ibusWSSPersonAccountEnrollmentRequest.GetCoverageAmountDetails(busConstant.LevelofCoverage_Basic);
            lobjBasicLifeOption.icdoPersonAccountLifeOption.coverage_amount = ibusWSSPersonAccountEnrollmentRequest.idecBasicCoverageAmount;
            _iclbLifeOption.Add(lobjBasicLifeOption);


            if (iblnIsFromMSS == true)
            {
                LoadPersonAccountLifeOptions();
                if (iclbPersonAccountLifeOption.IsNotNull())
                {
                    foreach (busPersonAccountLifeOption lobjPersonAccountLifeOption in iclbPersonAccountLifeOption)
                    {
                        busPersonAccountLifeOption lobjSupplementalLifeOption = new busPersonAccountLifeOption();
                        lobjSupplementalLifeOption.icdoPersonAccountLifeOption = new cdoPersonAccountLifeOption();
                        lobjSupplementalLifeOption.icdoPersonAccountLifeOption.Sequence_ID = 2;
                        lobjSupplementalLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value = busConstant.LevelofCoverage_Supplemental;
                        lobjSupplementalLifeOption.icdoPersonAccountLifeOption.level_of_coverage_description =
                            iobjPassInfo.isrvDBCache.GetCodeDescriptionString(busConstant.LevelofCoverage_CodeID, busConstant.LevelofCoverage_Supplemental);
                        if (ibusWSSPersonAccountEnrollmentRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_waiver_flag == busConstant.Flag_No)
                        {
                            lobjSupplementalLifeOption.icdoPersonAccountLifeOption.plan_option_status_value = busConstant.PlanOptionStatusValueEnrolled;
                        }
                        else
                            lobjSupplementalLifeOption.icdoPersonAccountLifeOption.plan_option_status_value = busConstant.PlanOptionStatusValueWaived; //PIR-7987 + 

                        if (lobjPersonAccountLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_Supplemental &&
                            lobjPersonAccountLifeOption.icdoPersonAccountLifeOption.plan_option_status_value == busConstant.PlanOptionStatusValueEnrolled)
                        {
                            _iclbLifeOption.Add(lobjSupplementalLifeOption);
                        }

                        busPersonAccountLifeOption lobjDependentLifeOption = new busPersonAccountLifeOption();
                        lobjDependentLifeOption.icdoPersonAccountLifeOption = new cdoPersonAccountLifeOption();
                        lobjDependentLifeOption.icdoPersonAccountLifeOption.Sequence_ID = 3;
                        lobjDependentLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value = busConstant.LevelofCoverage_DependentSupplemental;
                        lobjDependentLifeOption.icdoPersonAccountLifeOption.level_of_coverage_description =
                            iobjPassInfo.isrvDBCache.GetCodeDescriptionString(busConstant.LevelofCoverage_CodeID, busConstant.LevelofCoverage_DependentSupplemental);
                        if (ibusWSSPersonAccountEnrollmentRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_waiver_flag == busConstant.Flag_No)
                        {
                            lobjDependentLifeOption.icdoPersonAccountLifeOption.plan_option_status_value = busConstant.PlanOptionStatusValueEnrolled;
                        }
                        else
                            lobjDependentLifeOption.icdoPersonAccountLifeOption.plan_option_status_value = busConstant.PlanOptionStatusValueWaived;//PIR-7987 + 


                        if (lobjPersonAccountLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_DependentSupplemental &&
                            lobjPersonAccountLifeOption.icdoPersonAccountLifeOption.plan_option_status_value == busConstant.PlanOptionStatusValueEnrolled)
                        {
                            _iclbLifeOption.Add(lobjDependentLifeOption);
                        }

                        busPersonAccountLifeOption lobjSpouseLifeOption = new busPersonAccountLifeOption();
                        lobjSpouseLifeOption.icdoPersonAccountLifeOption = new cdoPersonAccountLifeOption();
                        lobjSpouseLifeOption.icdoPersonAccountLifeOption.Sequence_ID = 4;
                        lobjSpouseLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value = busConstant.LevelofCoverage_SpouseSupplemental;
                        lobjSpouseLifeOption.icdoPersonAccountLifeOption.level_of_coverage_description =
                            iobjPassInfo.isrvDBCache.GetCodeDescriptionString(busConstant.LevelofCoverage_CodeID, busConstant.LevelofCoverage_SpouseSupplemental);
                        if (ibusWSSPersonAccountEnrollmentRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_waiver_flag == busConstant.Flag_No)
                        {
                            lobjSpouseLifeOption.icdoPersonAccountLifeOption.plan_option_status_value = busConstant.PlanOptionStatusValueEnrolled;
                        }
                        else
                            lobjSpouseLifeOption.icdoPersonAccountLifeOption.plan_option_status_value = busConstant.PlanOptionStatusValueWaived;//PIR-7987 + 


                        if (lobjPersonAccountLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_SpouseSupplemental &&
                            lobjPersonAccountLifeOption.icdoPersonAccountLifeOption.plan_option_status_value == busConstant.PlanOptionStatusValueEnrolled)
                        {
                            _iclbLifeOption.Add(lobjSpouseLifeOption);
                        }
                    }
                }
            }
            else
            {
                busPersonAccountLifeOption lobjSupplementalLifeOption = new busPersonAccountLifeOption();
                lobjSupplementalLifeOption.icdoPersonAccountLifeOption = new cdoPersonAccountLifeOption();
                lobjSupplementalLifeOption.icdoPersonAccountLifeOption.Sequence_ID = 2;
                lobjSupplementalLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value = busConstant.LevelofCoverage_Supplemental;
                lobjSupplementalLifeOption.icdoPersonAccountLifeOption.level_of_coverage_description =
                    iobjPassInfo.isrvDBCache.GetCodeDescriptionString(busConstant.LevelofCoverage_CodeID, busConstant.LevelofCoverage_Supplemental);
                if (ibusWSSPersonAccountEnrollmentRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_waiver_flag == busConstant.Flag_No)
                {
                    lobjSupplementalLifeOption.icdoPersonAccountLifeOption.plan_option_status_value = busConstant.PlanOptionStatusValueEnrolled;
                }
                else
                    lobjSupplementalLifeOption.icdoPersonAccountLifeOption.plan_option_status_value = busConstant.PlanOptionStatusValueWaived; //PIR-7987 + 
                _iclbLifeOption.Add(lobjSupplementalLifeOption);
                
                busPersonAccountLifeOption lobjDependentLifeOption = new busPersonAccountLifeOption();
                lobjDependentLifeOption.icdoPersonAccountLifeOption = new cdoPersonAccountLifeOption();
                lobjDependentLifeOption.icdoPersonAccountLifeOption.Sequence_ID = 3;
                lobjDependentLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value = busConstant.LevelofCoverage_DependentSupplemental;
                lobjDependentLifeOption.icdoPersonAccountLifeOption.level_of_coverage_description =
                    iobjPassInfo.isrvDBCache.GetCodeDescriptionString(busConstant.LevelofCoverage_CodeID, busConstant.LevelofCoverage_DependentSupplemental);
                if (ibusWSSPersonAccountEnrollmentRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_waiver_flag == busConstant.Flag_No)
                {
                    lobjDependentLifeOption.icdoPersonAccountLifeOption.plan_option_status_value = busConstant.PlanOptionStatusValueEnrolled;
                }
                else
                    lobjDependentLifeOption.icdoPersonAccountLifeOption.plan_option_status_value = busConstant.PlanOptionStatusValueWaived;//PIR-7987 + 
                 _iclbLifeOption.Add(lobjDependentLifeOption);

                busPersonAccountLifeOption lobjSpouseLifeOption = new busPersonAccountLifeOption();
                lobjSpouseLifeOption.icdoPersonAccountLifeOption = new cdoPersonAccountLifeOption();
                lobjSpouseLifeOption.icdoPersonAccountLifeOption.Sequence_ID = 4;
                lobjSpouseLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value = busConstant.LevelofCoverage_SpouseSupplemental;
                lobjSpouseLifeOption.icdoPersonAccountLifeOption.level_of_coverage_description =
                    iobjPassInfo.isrvDBCache.GetCodeDescriptionString(busConstant.LevelofCoverage_CodeID, busConstant.LevelofCoverage_SpouseSupplemental);
                if (ibusWSSPersonAccountEnrollmentRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_waiver_flag == busConstant.Flag_No)
                {
                    lobjSpouseLifeOption.icdoPersonAccountLifeOption.plan_option_status_value = busConstant.PlanOptionStatusValueEnrolled;
                }
                else
                    lobjSpouseLifeOption.icdoPersonAccountLifeOption.plan_option_status_value = busConstant.PlanOptionStatusValueWaived;//PIR-7987 + 
                _iclbLifeOption.Add(lobjSpouseLifeOption);
            }
        }

        /// <summary>
        /// Loading Group Life Option objects with Data in update Mode
        /// </summary>
        /// <param name="AintPersonAccountID"></param>
        public void LoadLifeOptionData()
        {
            if (_iclbLifeOption == null)
                LoadLifeOption();

            if (icdoPersonAccount.person_account_id > 0)
            {
                DataTable ldtbOptions = Select<cdoPersonAccountLifeOption>(
                                        new string[1] { "person_account_id" },
                                        new object[1] { icdoPersonAccount.person_account_id }, null, null);
                foreach (DataRow dr in ldtbOptions.Rows)
                {
                    foreach (busPersonAccountLifeOption lobjLifeOption in _iclbLifeOption)
                    {
                        if (Convert.ToString(dr["LEVEL_OF_COVERAGE_VALUE"]) == lobjLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value)
                        {
                            lobjLifeOption.icdoPersonAccountLifeOption.LoadData(dr);
                            break;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Goup Life plan loading from history table
        /// </summary>
        public void LoadLifeHistoryData(int aintOrgID)
        {
            if (_iclbLifeHistory == null)
                _iclbLifeHistory = new Collection<busPersonAccountLifeHistory>();

            if (icdoPersonAccount.person_account_id > 0)
            {
                //PIR 26324 
                DataTable ldtbLifeHistoryFiltered = Select("entPersonAccountLife.LoadLifeHistoryForESS", new object[2] { icdoPersonAccount.person_account_id, aintOrgID });

                _iclbLifeHistory = GetCollection<busPersonAccountLifeHistory>(ldtbLifeHistoryFiltered, "icdoPersonAccountLifeHistory");
            }
        }
        public void GetMonthlyPremiumAmount_ForLifeHistory()
        {

            if (idtPlanEffectiveDate == DateTime.MinValue)
            { LoadPlanEffectiveDate(); }


            _idecTotalMonthlyPremium = 0;
            _idecLifeBasicPremiumAmt = 0;
            _idecLifeSupplementalPremiumAmount = 0;
            _idecSpouseSupplementalPremiumAmt = 0;
            _idecDependentSupplementalPremiumAmt = 0;
            //prod pir 4260
            idecADAndDBasicRate = 0.0000M;
            idecADAndDSupplementalRate = 0.0000M;
            idecBasicCoverageAmount = 0.0M;
            idecSuppCoverageAmount = 0.0M;
            idecSpouseSuppCoverageAmount = 0.0M;
            idecDepSuppCoverageAmount = 0.0M;

            decimal ldecADAndDBasicRate = 0.0000M;
            decimal ldecADAndDSupplementalRate = 0.0000M;

            LoadMemberAge(idtPlanEffectiveDate);

            foreach (busPersonAccountLifeHistory lobjLifeHistory in iclbLifeHistory)
            {
                // Refresh Coverage Premiums
                lobjLifeHistory.icdoPersonAccountLifeHistory.Employee_Premium_Amount = 0M;
                lobjLifeHistory.icdoPersonAccountLifeHistory.Employer_Premium_Amount = 0M;              

                if ((lobjLifeHistory.icdoPersonAccountLifeHistory.person_account_life_history_id > 0) &&
                    (lobjLifeHistory.icdoPersonAccountLifeHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled) &&
                    (lobjLifeHistory.icdoPersonAccountLifeHistory.plan_option_status_value == busConstant.PlanOptionStatusValueEnrolled) &&
                    (lobjLifeHistory.icdoPersonAccountLifeHistory.coverage_amount != 0.00M))
                {
                    // Load Monthly Premium only if the Premium waiver flag is NOT checked.
                    if (lobjLifeHistory.icdoPersonAccountLifeHistory.premium_waiver_flag != busConstant.Flag_Yes)
                    {
                        decimal ldecEmployerPremiumAmount = 0.0M;
                        lobjLifeHistory.icdoPersonAccountLifeHistory.Employee_Premium_Amount = busRateHelper.GetLifePremiumAmount(
                            lobjLifeHistory.icdoPersonAccountLifeHistory.life_insurance_type_value,
                            lobjLifeHistory.icdoPersonAccountLifeHistory.level_of_coverage_value,
                            lobjLifeHistory.icdoPersonAccountLifeHistory.coverage_amount,
                            icdoPersonAccountLife.Life_Insurance_Age, ibusProviderOrgPlan.icdoOrgPlan.org_plan_id,
                            idtPlanEffectiveDate, ref ldecEmployerPremiumAmount, idtbCachedLifeRate, iobjPassInfo, ref ldecADAndDBasicRate, ref ldecADAndDSupplementalRate);

                        lobjLifeHistory.icdoPersonAccountLifeHistory.Employer_Premium_Amount = ldecEmployerPremiumAmount;

                        //Assigning the Total Premium Properties
                        _idecTotalMonthlyPremium += lobjLifeHistory.icdoPersonAccountLifeHistory.Monthly_Premium;
                        _idecTotalEEPremium += lobjLifeHistory.icdoPersonAccountLifeHistory.Employee_Premium_Amount;
                        _idecTotalERPremium += lobjLifeHistory.icdoPersonAccountLifeHistory.Employer_Premium_Amount;

                        //Assign the Property By Life Option
                        if (lobjLifeHistory.icdoPersonAccountLifeHistory.level_of_coverage_value == busConstant.LevelofCoverage_Basic)
                        {
                            _idecLifeBasicPremiumAmt = lobjLifeHistory.icdoPersonAccountLifeHistory.Monthly_Premium;
                            idecBasicCoverageAmount = lobjLifeHistory.icdoPersonAccountLifeHistory.coverage_amount;
                            idecADAndDBasicRate = ldecADAndDBasicRate;
                        }
                        else if (lobjLifeHistory.icdoPersonAccountLifeHistory.level_of_coverage_value == busConstant.LevelofCoverage_Supplemental)
                        {
                            _idecLifeSupplementalPremiumAmount = lobjLifeHistory.icdoPersonAccountLifeHistory.Monthly_Premium;
                            idecSuppCoverageAmount = lobjLifeHistory.icdoPersonAccountLifeHistory.coverage_amount;
                            idecADAndDSupplementalRate = ldecADAndDSupplementalRate;
                        }
                        else if (lobjLifeHistory.icdoPersonAccountLifeHistory.level_of_coverage_value == busConstant.LevelofCoverage_SpouseSupplemental)
                        {
                            _idecSpouseSupplementalPremiumAmt = lobjLifeHistory.icdoPersonAccountLifeHistory.Monthly_Premium;
                            idecSpouseSuppCoverageAmount = lobjLifeHistory.icdoPersonAccountLifeHistory.coverage_amount;
                        }
                        else if (lobjLifeHistory.icdoPersonAccountLifeHistory.level_of_coverage_value == busConstant.LevelofCoverage_DependentSupplemental)
                        {
                            _idecDependentSupplementalPremiumAmt = lobjLifeHistory.icdoPersonAccountLifeHistory.Monthly_Premium;
                            idecDepSuppCoverageAmount = lobjLifeHistory.icdoPersonAccountLifeHistory.coverage_amount;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Loading Group Life Option objects with Data in update Mode
        /// </summary>
        /// <param name="AintPersonAccountID"></param>
        public void LoadLifeOptionDataFromHistory(DataRow[] aarrRows)
        {
            if (_iclbLifeOption == null)
                LoadLifeOption();

         
            foreach (DataRow dr in aarrRows)
            {
                foreach (busPersonAccountLifeOption lobjLifeOption in _iclbLifeOption)
                {
                    if (Convert.ToString(dr["LEVEL_OF_COVERAGE_VALUE"]) == lobjLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value)
                    {
                        busPersonAccountLifeHistory lbusPALifeHistory = new busPersonAccountLifeHistory { icdoPersonAccountLifeHistory = new cdoPersonAccountLifeHistory() };
                        lbusPALifeHistory.icdoPersonAccountLifeHistory.LoadData(dr);
                       
                            lobjLifeOption.icdoPersonAccountLifeOption.person_account_id = lbusPALifeHistory.icdoPersonAccountLifeHistory.person_account_id;
                            lobjLifeOption.icdoPersonAccountLifeOption.level_of_coverage_id = lbusPALifeHistory.icdoPersonAccountLifeHistory.level_of_coverage_id;
                            lobjLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value = lbusPALifeHistory.icdoPersonAccountLifeHistory.level_of_coverage_value;
                            lobjLifeOption.icdoPersonAccountLifeOption.effective_start_date = lbusPALifeHistory.icdoPersonAccountLifeHistory.effective_start_date;
                            lobjLifeOption.icdoPersonAccountLifeOption.effective_end_date = lbusPALifeHistory.icdoPersonAccountLifeHistory.effective_end_date;
                            lobjLifeOption.icdoPersonAccountLifeOption.plan_option_status_id = lbusPALifeHistory.icdoPersonAccountLifeHistory.plan_option_status_id;
                            lobjLifeOption.icdoPersonAccountLifeOption.plan_option_status_value = lbusPALifeHistory.icdoPersonAccountLifeHistory.plan_option_status_value;
                            lobjLifeOption.icdoPersonAccountLifeOption.coverage_amount = lbusPALifeHistory.icdoPersonAccountLifeHistory.coverage_amount;
                     
                            break;                      
                       
                    }
                }
            }
        }

        /// <summary>
        /// Loading Group Life Option objects with Data in update Mode
        /// </summary>
        /// <param name="AintPersonAccountID"></param>
        public void LoadLifeOptionDataFromHistory(IEnumerable<cdoPersonAccountLifeHistory> aclcPALH)
        {
            if (_iclbLifeOption == null)
                LoadLifeOption();

            foreach (cdoPersonAccountLifeHistory lobjCDO in aclcPALH)
            {
                var lobjLifeOption = _iclbLifeOption.Where(d => d.icdoPersonAccountLifeOption.level_of_coverage_value == lobjCDO.level_of_coverage_value).FirstOrDefault();

                busPersonAccountLifeHistory lbusPALifeHistory = new busPersonAccountLifeHistory();
                lbusPALifeHistory.icdoPersonAccountLifeHistory = lobjCDO;
                lobjLifeOption.icdoPersonAccountLifeOption.person_account_id = lbusPALifeHistory.icdoPersonAccountLifeHistory.person_account_id;
                lobjLifeOption.icdoPersonAccountLifeOption.level_of_coverage_id = lbusPALifeHistory.icdoPersonAccountLifeHistory.level_of_coverage_id;
                lobjLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value = lbusPALifeHistory.icdoPersonAccountLifeHistory.level_of_coverage_value;
                lobjLifeOption.icdoPersonAccountLifeOption.effective_start_date = lbusPALifeHistory.icdoPersonAccountLifeHistory.effective_start_date;
                lobjLifeOption.icdoPersonAccountLifeOption.effective_end_date = lbusPALifeHistory.icdoPersonAccountLifeHistory.effective_end_date;
                lobjLifeOption.icdoPersonAccountLifeOption.plan_option_status_id = lbusPALifeHistory.icdoPersonAccountLifeHistory.plan_option_status_id;
                lobjLifeOption.icdoPersonAccountLifeOption.plan_option_status_value = lbusPALifeHistory.icdoPersonAccountLifeHistory.plan_option_status_value;
                lobjLifeOption.icdoPersonAccountLifeOption.coverage_amount = lbusPALifeHistory.icdoPersonAccountLifeHistory.coverage_amount;
            }
        }

        /// <summary>
        /// Loading Group Life Option objects with Data in update Mode
        /// </summary>
        /// <param name="AintPersonAccountID"></param>
        public void LoadLifeOptionDataFromHistory(DateTime adtEffectiveDate)
        {
            if (_iclbLifeOption == null)
                LoadLifeOption();

            if (iclbPersonAccountLifeHistory == null)
                LoadHistory();

            foreach (busPersonAccountLifeHistory lbusHistory in iclbPersonAccountLifeHistory)
            {
                foreach (busPersonAccountLifeOption lobjLifeOption in _iclbLifeOption)
                {
                    if ((lbusHistory.icdoPersonAccountLifeHistory.level_of_coverage_value == lobjLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value) &&
                        (busGlobalFunctions.CheckDateOverlapping(adtEffectiveDate, lbusHistory.icdoPersonAccountLifeHistory.effective_start_date,
                        lbusHistory.icdoPersonAccountLifeHistory.effective_end_date)) &&
                        lbusHistory.icdoPersonAccountLifeHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled &&
                        lbusHistory.icdoPersonAccountLifeHistory.plan_option_status_value == busConstant.PlanOptionStatusValueEnrolled &&
                        lbusHistory.icdoPersonAccountLifeHistory.effective_start_date != lbusHistory.icdoPersonAccountLifeHistory.effective_end_date) //ignore same dated records
                    {
                        lobjLifeOption.icdoPersonAccountLifeOption.person_account_id = lbusHistory.icdoPersonAccountLifeHistory.person_account_id;
                        lobjLifeOption.icdoPersonAccountLifeOption.level_of_coverage_id = lbusHistory.icdoPersonAccountLifeHistory.level_of_coverage_id;
                        lobjLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value = lbusHistory.icdoPersonAccountLifeHistory.level_of_coverage_value;
                        lobjLifeOption.icdoPersonAccountLifeOption.effective_start_date = lbusHistory.icdoPersonAccountLifeHistory.effective_start_date;
                        lobjLifeOption.icdoPersonAccountLifeOption.effective_end_date = lbusHistory.icdoPersonAccountLifeHistory.effective_end_date;
                        lobjLifeOption.icdoPersonAccountLifeOption.plan_option_status_id = lbusHistory.icdoPersonAccountLifeHistory.plan_option_status_id;
                        lobjLifeOption.icdoPersonAccountLifeOption.plan_option_status_value = lbusHistory.icdoPersonAccountLifeHistory.plan_option_status_value;
                        lobjLifeOption.icdoPersonAccountLifeOption.coverage_amount = lbusHistory.icdoPersonAccountLifeHistory.coverage_amount;
                        break;
                    }
                }
            }
        }

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            //PROD Logic changes. - Delete the Previous open History lines if the Allow Overlap Sets true
            bool lblnReloadPreviousHistory = false;
            iclbOverlappingHistory = new Collection<busPersonAccountLifeHistory>();
            if ((istrAllowOverlapHistory == busConstant.Flag_Yes) && (icdoPersonAccount.history_change_date != DateTime.MinValue))
            {
                //Reload the History Always...
                LoadHistory();
                foreach (busPersonAccountLifeOption lobjLifeOption in _iclbLifeOption)
                {
                    lobjLifeOption.iblnOverlapHistoryFound = false;
                    Collection<busPersonAccountLifeHistory> lclbOpenHistory = LoadOverlappingHistoryForOption(lobjLifeOption);
                    if (lclbOpenHistory.Count > 0)
                    {
                        foreach (busPersonAccountLifeHistory lbusPALifeHistory in lclbOpenHistory)
                        {
                            iclbPersonAccountLifeHistory.Remove(lbusPALifeHistory);
                            iclbOverlappingHistory.Add(lbusPALifeHistory);
                            lobjLifeOption.iblnOverlapHistoryFound = true;
                            lblnReloadPreviousHistory = true;
                        }
                    }
                }
            }

            if (lblnReloadPreviousHistory)
            {
                LoadPreviousHistory();
            }

            if (icdoPersonAccount.ihstOldValues.Count > 0)
            {
                // BR -226 is a Warning/Soft Error and hence the Old Participation Status needs to persist before the object gets updated.
                istrOldPlanParticipationStatus = Convert.ToString(icdoPersonAccount.ihstOldValues["plan_participation_status_value"]);
            }

            LoadPlanEffectiveDate();

            foreach (busPersonAccountLifeOption lobjLifeOption in _iclbLifeOption)
            {
                //PROD FIX:  based on new flow in Life screen, Getting the value Effective End Date Concept has been removed. But, Raj does not wants to remove the data from DB..
                //So, this is the workaround to avoid any issue just by clearing it before any validation
                lobjLifeOption.icdoPersonAccountLifeOption.effective_end_date = DateTime.MinValue;

                /// Turn the flag ON if any value entered in Life Option Grid.
                if ((lobjLifeOption.icdoPersonAccountLifeOption.coverage_amount != 0.0M) || lobjLifeOption.icdoPersonAccountLifeOption.plan_option_status_value == busConstant.PlanOptionStatusValueEnrolled ||
                    (lobjLifeOption.icdoPersonAccountLifeOption.plan_option_status_value == busConstant.PlanOptionStatusValueWaived))
                {
                    lobjLifeOption.icdoPersonAccountLifeOption.lblnValueEntered = true;
                }
                else
                {
                    lobjLifeOption.icdoPersonAccountLifeOption.lblnValueEntered = false;
                }

                if (lobjLifeOption.icdoPersonAccountLifeOption.ihstOldValues.Count > 0)
                {
                    if (Convert.ToDecimal(lobjLifeOption.icdoPersonAccountLifeOption.ihstOldValues["coverage_amount"]) > 0M &&
                        lobjLifeOption.icdoPersonAccountLifeOption.coverage_amount == 0M)
                        lobjLifeOption.icdoPersonAccountLifeOption.lblnValueRemoved = true;
                }

                if (lobjLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_Basic)
                {
                    icdoPersonAccount.start_date = lobjLifeOption.icdoPersonAccountLifeOption.effective_start_date;
                    //PIR 1729 : Update the End Date if the Basic Life Option is end dated. 
                    if ((icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusInsuranceEnrolled)
                        && (icdoPersonAccount.history_change_date != DateTime.MinValue))
                        icdoPersonAccount.end_date = icdoPersonAccount.history_change_date.AddDays(-1);
                    else
                        icdoPersonAccount.end_date = DateTime.MinValue;
                }
                //PIR 25250 When history is overlapped with earlier date then effective start date, Option and History were not updating.
                if(istrAllowOverlapHistory == busConstant.Flag_Yes && icdoPersonAccount.start_date != DateTime.MinValue 
                    && icdoPersonAccount.history_change_date != DateTime.MinValue && icdoPersonAccount.start_date > icdoPersonAccount.history_change_date)
                {
                    lobjLifeOption.icdoPersonAccountLifeOption.effective_start_date = icdoPersonAccount.history_change_date;
                    lobjLifeOption.icdoPersonAccountLifeOption.Update();
                }
            }

            _AintGroupLifeOptionValidateFlag = ValidateGroupLifeOption();

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

            if (iclbPreviousHistory == null)
                LoadPreviousHistory();

            SetHistoryEntryRequiredOrNot();

            // In case of Negative adjustment posting after the member has retired, the system has to load his active employment.
            // This needs to be reload every time except for new mode.
			//PIR 15544
			
            if (icdoPersonAccount.person_account_id > 0 && !icdoPersonAccount.is_from_mss)
                icdoPersonAccount.person_employment_dtl_id = GetEmploymentDetailID();
            if (icdoPersonAccount.person_employment_dtl_id != 0)
            {
                LoadPersonEmploymentDetail();
                ibusPersonEmploymentDetail.LoadPersonEmployment();
            }

            if (icdoPersonAccount.ienuObjectState == ObjectState.Insert)
            {
                if (ibusPersonEmploymentDetail != null)
                    if (ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id != 0)
                    {
                        LoadOrgPlan();
                        LoadProviderOrgPlan();
                    }
                iblnIsNewMode = true;
            }
            //Function to set bool variable based on any value change in Payment election
            //SetPaymentElectionChangedOrNot();
            base.BeforeValidate(aenmPageMode);
        }
        string istrOldPaymentMethod = string.Empty;
        public override void BeforePersistChanges()
        {
            icolPosNegEmployerPayrollDtl = null;
            icolPosNegIbsDetail = null;
            if (ibusProviderOrgPlan != null)
            {
                icdoPersonAccount.provider_org_id = ibusProviderOrgPlan.icdoOrgPlan.org_id;
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
                icdoPersonAccountLife.modified_after_tffr_file_sent_flag = busConstant.Flag_Yes;
            }
            if (icdoPersonAccount.ihstOldValues.Count > 0)
                istrPreviousPlanParticipationStatus = Convert.ToString(icdoPersonAccount.ihstOldValues["plan_participation_status_value"]);

            //UAT PIR 2373 people soft file changes
            //--Start--//
            if (icdoPersonAccount.ihstOldValues.Count > 0
                && Convert.ToString(icdoPersonAccount.ihstOldValues["plan_participation_status_value"]) != icdoPersonAccount.plan_participation_status_value
                && (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceCancelled ||
                icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended))
            {
                SetPersonAcccountForTeminationChange();
                SetLifeOptionForTerminationChange();
            }
            else
            {
                SetPersonAccountForEnrollmentChange();
            }
            //--End--//
        }

        /// <summary>
        /// prod pir 4861 : to set peoplesoft flag when employment is terminated
        /// </summary>
        private void SetLifeOptionForTerminationChange()
        {
            if (iclbLifeOption == null)
                LoadLifeOptionData();

            foreach (busPersonAccountLifeOption lobjLifeOption in iclbLifeOption)
            {
                if (lobjLifeOption.icdoPersonAccountLifeOption.coverage_amount > 0)
                {
                    //lobjLifeOption.icdoPersonAccountLifeOption.people_soft_file_sent_flag = busConstant.Flag_No;
                    lobjLifeOption.icdoPersonAccountLifeOption.lblnValueEntered = true;
                }
            }
        }

        /// <summary>
        /// uat pir 2373 : method to set the PS event value based on enrollment change
        /// </summary>
        private void SetPersonAccountForEnrollmentChange()
        {
            if (iclbPreviousHistory == null)
                LoadPreviousHistory();
            foreach (busPersonAccountLifeOption lobjLifeOption in _iclbLifeOption)
            {
                busPersonAccountLifeHistory lobjPALH = iclbPreviousHistory
                    .Where(o => o.icdoPersonAccountLifeHistory.person_account_id == lobjLifeOption.icdoPersonAccountLifeOption.person_account_id &&
                        o.icdoPersonAccountLifeHistory.level_of_coverage_value == lobjLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value)
                        .FirstOrDefault();
                if (IsHistoryEntryRequired && lobjPALH != null)
                {
                    //PROD PIR 4586
                    //prod pir 4861 : removal of annual enrollment logic
                    // PROD PIR 7705: Added Annual enrollment logic
                    if (icdoPersonAccountLife.reason_value == busConstant.ChangeReasonAnnualEnrollment
                   && (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled ||
                   icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended))
                    {
                        //icdoPersonAccount.people_soft_file_sent_flag = busConstant.Flag_No;//PIR 17081
                        if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                            icdoPersonAccount.ps_file_change_event_value = busConstant.AnnualEnrollment;
                        else//PIR 7987
                            icdoPersonAccount.ps_file_change_event_value = busConstant.AnnualEnrollmentWaived;
                        break;
                    }
                    else if (lobjPALH.icdoPersonAccountLifeHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended &&
                        icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                    {
                        icdoPersonAccount.ps_file_change_event_value = busConstant.NewEnrollment;
                        //icdoPersonAccount.people_soft_file_sent_flag = busConstant.Flag_No;//PIR 17081
                        //lobjLifeOption.icdoPersonAccountLifeOption.people_soft_file_sent_flag = busConstant.Flag_No;//PIR 17081
                    }
                    else if (lobjLifeOption.icdoPersonAccountLifeOption.coverage_amount != lobjPALH.icdoPersonAccountLifeHistory.coverage_amount && IsHistoryEntryRequired ||
                        lobjPALH.icdoPersonAccountLifeHistory.effective_start_date != lobjLifeOption.icdoPersonAccountLifeOption.effective_start_date ||
                        lobjPALH.icdoPersonAccountLifeHistory.plan_option_status_value != lobjLifeOption.icdoPersonAccountLifeOption.plan_option_status_value)
                    {
                        icdoPersonAccount.ps_file_change_event_value = busConstant.LevelOfCoverageChange;
                        //icdoPersonAccount.people_soft_file_sent_flag = busConstant.Flag_No;//PIR 17081
                        //lobjLifeOption.icdoPersonAccountLifeOption.people_soft_file_sent_flag = busConstant.Flag_No;//PIR 17081
                    }
                }
            }
        }

        public override int PersistChanges()
        {
            if (icdoPersonAccount.ienuObjectState == ObjectState.Insert)
            {
                //PROD PIR 4586
                //prod pir 4861 : removal of annual enrollment logic
                // PROD PIR 7987: Added Annual enrollment logic

               
                if (icdoPersonAccountLife.reason_value == busConstant.ChangeReasonAnnualEnrollment
                     && (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled ||
                     icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended))
                {
                    //icdoPersonAccount.people_soft_file_sent_flag = busConstant.Flag_No;//PIR 17081
                    if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                        icdoPersonAccount.ps_file_change_event_value = busConstant.AnnualEnrollment;
                    else//PIR 7987
                        icdoPersonAccount.ps_file_change_event_value = busConstant.AnnualEnrollmentWaived;
                    icdoPersonAccountLife.ps_file_change_event_value = icdoPersonAccount.ps_file_change_event_value;

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
                icdoPersonAccountLife.person_account_id = icdoPersonAccount.person_account_id;
                icdoPersonAccountLife.disability_letter_sent_flag = busConstant.Flag_No;
                icdoPersonAccountLife.Insert();
                foreach (busPersonAccountLifeOption lobjLifeOption in _iclbLifeOption)
                {
                    if (lobjLifeOption.icdoPersonAccountLifeOption.lblnValueEntered)
                    {
                            //prod pir 4861
                            //lobjLifeOption.icdoPersonAccountLifeOption.people_soft_file_sent_flag = busConstant.Flag_No;//PIR 17081
                            lobjLifeOption.icdoPersonAccountLifeOption.person_account_id = icdoPersonAccount.person_account_id;
                            lobjLifeOption.icdoPersonAccountLifeOption.Insert();
                    }
                }
            }
            else
            {
                foreach (busPersonAccountLifeOption lobjLifeOption in _iclbLifeOption)
                {
                    if (lobjLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_Basic)
                    {
                        icdoPersonAccount.start_date = lobjLifeOption.icdoPersonAccountLifeOption.effective_start_date;
                        break;
                    }
                }

                //PIR 7987
                if (icdoPersonAccountLife.reason_value == busConstant.ChangeReasonAnnualEnrollment
                    && (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled ||
                    icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended))
                {
                    //icdoPersonAccount.people_soft_file_sent_flag = busConstant.Flag_No;//PIR 17081
                    if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                        icdoPersonAccount.ps_file_change_event_value = busConstant.AnnualEnrollment;                        
                    else//PIR 7987
                        icdoPersonAccount.ps_file_change_event_value = busConstant.AnnualEnrollmentWaived;
                    icdoPersonAccountLife.ps_file_change_event_value = icdoPersonAccount.ps_file_change_event_value;

                }
                

                foreach (busPersonAccountLifeOption lobjLifeOption in _iclbLifeOption)
                {
                    if (lobjLifeOption.icdoPersonAccountLifeOption.lblnValueEntered ||
                        lobjLifeOption.icdoPersonAccountLifeOption.lblnValueRemoved)
                    {
                        if (IsLifeOptionExists(lobjLifeOption.icdoPersonAccountLifeOption.account_life_option_id))
                            lobjLifeOption.icdoPersonAccountLifeOption.Update();
                        else
                        {
                            lobjLifeOption.icdoPersonAccountLifeOption.person_account_id = icdoPersonAccount.person_account_id;
                            lobjLifeOption.icdoPersonAccountLifeOption.Insert();
                        }
                    }
                }
            }

            icdoPersonAccount.Update();
            icdoPersonAccountLife.Update();

            //Payment election may not be inserted in New mode, so we need to insert in update mode too.
            if (ibusPaymentElection.icdoPersonAccountPaymentElection.account_payment_election_id > 0)
                ibusPaymentElection.icdoPersonAccountPaymentElection.Update();
            else
            {
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

            return 1;
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();

            ProcessHistory();
            LoadHistory();
            LoadProviderName();
            GetMonthlyPremiumAmount();
            if (icdoPersonAccount.person_employment_dtl_id > 0)
            {
                SetPersonAccountIDInPersonAccountEmploymentDetail();
            }
            LoadAllPersonEmploymentDetails(true);
            LoadPreviousHistory();
			//PIR 12737 & 8565 -- added payment election history
            LoadPaymentElectionHistory();
            //PIR 24918
            if (ibusPaymentElection == null)
                LoadPaymentElection();

            //Creating Payroll / IBS Adjustment Record If Enrollment Changes Occurs
            if ((IsHistoryEntryRequired || IsCurrentPaymentMethodPensionCheck) && icdoPersonAccount.status_value == busConstant.PersonAccountStatusValid) //PIR-19416 
            {
                CreateAdjustmentPayrollForEnrollmentHistoryClose();
                //PIR : 2029 Do not create Positive Adjustment for Suspended Status
                if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                    CreateAdjustmentPayrollForEnrollmentHistoryAdd();
            }

            // PROD PIR ID 4735
            //PIR 7535
            if ((icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended &&
                istrPreviousPlanParticipationStatus != busConstant.PlanParticipationStatusInsuranceSuspended) ||
                (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceCancelled &&
               istrPreviousPlanParticipationStatus != busConstant.PlanParticipationStatusInsuranceCancelled))
            {
                ibusPaymentElection.EndPAPITEntriesForSuspendedAccount(icdoPersonAccount.plan_id, icdoPersonAccount.current_plan_start_date);
                // ignore if already in review IBS entries
                if (icdoPersonAccount.status_value == busConstant.PersonAccountStatusValid)
                {
                    DBFunction.DBNonQuery("cdoIbsDetail.UpdateAllIBSDetailStatusToIgnoreAfterPlanSusorCan", new object[2] { icdoPersonAccount.person_account_id, icdoPersonAccount.current_plan_start_date },
                                       iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                    DBFunction.DBNonQuery("entEmployerPayrollHeader.UpdateAllEPDetailStatusToIgnoreAfterPlanSusorCan", new object[3] {
                                    icdoPersonAccount.person_id, icdoPersonAccount.plan_id, icdoPersonAccount.current_plan_start_date },
                                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                }
            }
            //PAPIT entries should only be updated when the IBS Adjustment is posted. //PIR 16393 - Wrong insurance deduction
            else if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
            {
                    //prod pir 5538 : need to recalculate premium as on latest date
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
                    //Recalculate the Premium Based on the New Effective Date
                    GetMonthlyPremiumAmount(GetLatestDate(icdoPersonAccount.current_plan_start_date, ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date,
                                        idtNextBenefiPaymentDate));

                    // Change 1: UAT 1452 : need to end date PAPIT entries if at all in review status as we are saving payment election
                    // Change 2: Modified by Elayaraja - PIR 1488 deduction amount should be equal to premium amount - RHIC benefit amount
                    //             *** BR-074-12 *** System must transfer insurance premium information to the Payee Account's deduction list,
                    //             when insurance premium information is updated by the enrollment
                    // Change 3: Earlier this will update only if there exists changes in Payment Election fields. Modified now to update by all time.
                    if (iintOldPayeeAccountID != 0 && iintOldPayeeAccountID != ibusPaymentElection.icdoPersonAccountPaymentElection.payee_account_id)
                    {
                        ibusPaymentElection.ManagePayeeAccountPaymentItemType(ibusPaymentElection.icdoPersonAccountPaymentElection.payee_account_id,
                                            iintOldPayeeAccountID, idecTotalMonthlyPremium, icdoPersonAccount.plan_id,
                                            GetLatestDate(icdoPersonAccount.current_plan_start_date, ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date,
                                            idtNextBenefiPaymentDate), ibusPaymentElection.icdoPersonAccountPaymentElection.payment_method_value, ibusProviderOrgPlan.icdoOrgPlan.org_id, ablnIsPayeeAccountChanged: true);
                    }
                //prod pir 5538 : need to recalculate premium as on latest date
                RecalculatePremiumBasedOnPlanEffectiveDate();
            }
            // UAT PIR ID 1077 - To refresh the screen values only if the Suppress flag is On.
            if (icdoPersonAccount.suppress_warnings_flag == busConstant.Flag_Yes)
                RefreshValues();
            //UAT PIR 2220
            if (icdoPersonAccount.status_value == busConstant.PersonAccountStatusValid && IsHistoryEntryRequired)
                PostESSMessage();

            LoadLifeOptionData(); // PROD PIR ID 6384
            foreach (busPersonAccountLifeOption lobjOption in iclbLifeOption)
                lobjOption.EvaluateInitialLoadRules();
            //pir 8313
            if (ibusBaseActivityInstance != null)
                //SetProcessInstanceParameters();
                SetCaseInstanceParameters();

            if (ibusPaymentElection == null)
                LoadPaymentElection();

            if(icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended)
                EndBeneficiaryAndOptionOnLifeSuspend();
            UpdateNewHireMemberRecordReQuestDetailsManOrAutoPlanEnrollment(busConstant.LifeCategory);
            //PIR 17081
            if (iblnIsHistoryInserted && !iblnIsFromMSSForEnrollmentData && icdoPersonAccountLife.life_insurance_type_value != busConstant.LifeInsuranceTypeRetireeMember 
                && ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag != busConstant.Flag_Yes) //PIR 20135 - Issue 2
            {
                InsertIntoEnrollmentData();
            }
        }
        public void EndBeneficiaryAndOptionOnLifeSuspend()
        {            
            if (iclbPersonAccountBeneficiary == null || iclbPersonAccountBeneficiary.Count == 0)
                LoadPersonAccountBeneficiary();
            if (iclbPersonAccountBeneficiary.Count > 0)
            {
                busPersonAccountLifeHistory lbusPersonAccountLifeHistory = iclbPersonAccountLifeHistory
                                                                                   .FirstOrDefault(history =>
                                                                                       history.icdoPersonAccountLifeHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled &&
                                                                                       history.icdoPersonAccountLifeHistory.start_date.Date != history.icdoPersonAccountLifeHistory.end_date.Date);
                DateTime ldteBenEndDate = lbusPersonAccountLifeHistory.IsNotNull() && lbusPersonAccountLifeHistory.icdoPersonAccountLifeHistory.end_date != DateTime.MinValue
                                                                                     ? lbusPersonAccountLifeHistory.icdoPersonAccountLifeHistory.end_date : icdoPersonAccount.history_change_date_no_null.AddDays(-1);
                foreach (busPersonAccountBeneficiary lobjPersonAccountBeneficiary in iclbPersonAccountBeneficiary)
                {

                    if (lobjPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date == DateTime.MinValue)
                    {
                        lobjPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date = lobjPersonAccountBeneficiary.icdoPersonAccountBeneficiary.start_date > icdoPersonAccount.history_change_date_no_null.AddDays(-1) ? lobjPersonAccountBeneficiary.icdoPersonAccountBeneficiary.start_date : ldteBenEndDate;
                        lobjPersonAccountBeneficiary.icdoPersonAccountBeneficiary.Update();
                    }
                    else if (istrAllowOverlapHistory == "Y" && lobjPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date > icdoPersonAccount.history_change_date_no_null.AddDays(-1))
                    {
                        lobjPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date = lobjPersonAccountBeneficiary.icdoPersonAccountBeneficiary.start_date > icdoPersonAccount.history_change_date_no_null.AddDays(-1) ? lobjPersonAccountBeneficiary.icdoPersonAccountBeneficiary.start_date : icdoPersonAccount.history_change_date_no_null.AddDays(-1);
                        lobjPersonAccountBeneficiary.icdoPersonAccountBeneficiary.Update();
                    }
                }
            }

            //if (iclbLifeOption == null || iclbLifeOption.Count == 0)
            //    LoadLifeOption();
            //if (iclbLifeOption.Count > 0)
            //{
            //    foreach (busPersonAccountLifeOption lobjPersonAccountLifeOption in iclbLifeOption)
            //    {
            //        lobjPersonAccountLifeOption.icdoPersonAccountLifeOption.plan_option_status_value = busConstant.PlanOptionStatusValueWaived;
            //        lobjPersonAccountLifeOption.icdoPersonAccountLifeOption.coverage_amount = 0.00M;
            //        lobjPersonAccountLifeOption.icdoPersonAccountLifeOption.Update();
            //    }
            //}
            //if (iclbLifeHistory == null || iclbLifeHistory.Count == 0)
            //    LoadLifeHistoryData();
            //if (iclbLifeHistory.Count > 0)
            //{
            //    foreach (busPersonAccountLifeHistory lobjPersonAccountLifeHistory in iclbLifeHistory)
            //    {
            //        if (lobjPersonAccountLifeHistory.icdoPersonAccountLifeHistory.plan_option_status_value == busConstant.PlanOptionStatusValueEnrolled &&
            //            lobjPersonAccountLifeHistory.icdoPersonAccountLifeHistory.effective_start_date != DateTime.MinValue &&
            //            lobjPersonAccountLifeHistory.icdoPersonAccountLifeHistory.effective_end_date == DateTime.MinValue)
            //        {
            //            lobjPersonAccountLifeHistory.icdoPersonAccountLifeHistory.plan_option_status_value = busConstant.PlanOptionStatusValueWaived;
            //            lobjPersonAccountLifeHistory.icdoPersonAccountLifeHistory.coverage_amount = 0.0M;
            //            lobjPersonAccountLifeHistory.icdoPersonAccountLifeHistory.Update();
            //        }
            //    }
            //}
        }

        public void InsertIntoEnrollmentData()
        {
            int lintPSCounter = 0;
            int lintBERCounter = 0;

            if (iclbPersonAccountLifeHistory == null)
                LoadHistory();

            if (iclbLifeOption == null)
                LoadLifeOptionData();

            // PIR 23527 Adding Basic Coverage to Supplemental Coverage amount for Peoplesoft
            decimal ldecBasicSuppAmount = 0M;
            decimal ldecBasicAmount = 0m;
            if (iclbLifeOption.Count > 0)
            {
                foreach (busPersonAccountLifeOption lobjOption in iclbLifeOption)
                {
                    if (lobjOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_Basic)
                    {
                        ldecBasicSuppAmount = lobjOption.icdoPersonAccountLifeOption.coverage_amount;
                        ldecBasicAmount = lobjOption.icdoPersonAccountLifeOption.coverage_amount;
                    }
                    if (lobjOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_Supplemental &&
                        lobjOption.icdoPersonAccountLifeOption.coverage_amount > 0M)
                        ldecBasicSuppAmount += lobjOption.icdoPersonAccountLifeOption.coverage_amount;
                }
            }

            if (iclbLifeOption.Count > 0)
            {
                foreach (busPersonAccountLifeOption lobjLifeOption in iclbLifeOption)
                {
                    busEnrollmentData lobjEnrollmentData = new busEnrollmentData();
                    lobjEnrollmentData.icdoEnrollmentData = new doEnrollmentData();

                    busPersonAccountLifeHistory lobjLifeHistory = LoadHistoryByDate(lobjLifeOption, icdoPersonAccount.history_change_date);

                    if (lobjLifeHistory.icdoPersonAccountLifeHistory.person_account_life_history_id > 0)
                    {

                        if (ibusPersonEmploymentDetail == null)
                            LoadPersonEmploymentDetail();

                        if (ibusPersonEmploymentDetail.ibusPersonEmployment.IsNull())
                            ibusPersonEmploymentDetail.LoadPersonEmployment();

                        if (ibusProvider == null)
                            LoadProvider();

                        busDailyPersonAccountPeopleSoft lobjDailyPAPeopleSoft = new busDailyPersonAccountPeopleSoft();
                        lobjDailyPAPeopleSoft.ibusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
                        lobjDailyPAPeopleSoft.ibusProvider = new busOrganization { icdoOrganization = new cdoOrganization() };
                        lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                        lobjDailyPAPeopleSoft.ibusPersonAccountLife = new busPersonAccountLife { icdoPersonAccountLife = new cdoPersonAccountLife() };
                        lobjDailyPAPeopleSoft.ibusPersonAccountLife.ibusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };

                        decimal ldecEmployerPremiumAmount = 0.0M;
                        decimal ldecEmployeePremiumAmount = 0.0M;
                        decimal ldecADAndDBasicRate = 0.0000M;
                        decimal ldecADAndDSupplementalRate = 0.0000M;

                        if (lobjDailyPAPeopleSoft.iclbDailyPersonAccountPeopleSoft == null)
                            lobjDailyPAPeopleSoft.iclbDailyPersonAccountPeopleSoft = new Collection<busDailyPersonAccountPeopleSoft>();

                        lobjDailyPAPeopleSoft.ibusPersonAccount.icdoPersonAccount = icdoPersonAccount;
                        lobjDailyPAPeopleSoft.ibusPersonAccountLife.icdoPersonAccountLife = icdoPersonAccountLife;
                        lobjDailyPAPeopleSoft.ibusPersonAccountLife.ibusPersonAccount.icdoPersonAccount = icdoPersonAccount;

                        lobjDailyPAPeopleSoft.LoadPersonEmploymentForPeopleSoft();
                        lobjDailyPAPeopleSoft.LoadPeopleSoftOrgGroupValues();

                        lobjDailyPAPeopleSoft.ibusProvider = ibusProvider;

                        if (lobjDailyPAPeopleSoft.ibusPersonAccountLife == null)
                            lobjDailyPAPeopleSoft.ibusPersonAccountLife.FindPersonAccountLife(lobjDailyPAPeopleSoft.ibusPersonAccount.icdoPersonAccount.person_account_id);

                        lobjDailyPAPeopleSoft.ibusPersonAccountLife.icdoPersonAccount = lobjDailyPAPeopleSoft.ibusPersonAccount.icdoPersonAccount;
                        lobjDailyPAPeopleSoft.ibusPersonAccountLife.LoadLifeOptionData();

                        LoadPlanEffectiveDate();
                        LoadMemberAge(idtPlanEffectiveDate);
                        LoadOrgPlan(idtPlanEffectiveDate, lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id);
                        LoadProviderOrgPlan(idtPlanEffectiveDate);
                        ldecEmployeePremiumAmount = busRateHelper.GetLifePremiumAmount(lobjLifeHistory.icdoPersonAccountLifeHistory.life_insurance_type_value,
                                        lobjLifeHistory.icdoPersonAccountLifeHistory.level_of_coverage_value, lobjLifeHistory.icdoPersonAccountLifeHistory.coverage_amount,
                                        icdoPersonAccountLife.Life_Insurance_Age,
                                        ibusProviderOrgPlan.icdoOrgPlan.org_plan_id, idtPlanEffectiveDate, ref ldecEmployerPremiumAmount,
                                        idtbCachedLifeRate, iobjPassInfo, ref ldecADAndDBasicRate, ref ldecADAndDSupplementalRate);

                        int lintCounter = 0;

                        //PIR - 20885 ESS Benefit Enrollment Report - Annual Enrollment - Life Plan - If a member made a change in Life options and selected to Waive one of the levels they previously had, we incorrectly write the data to Enrollment Data table
                        if ((lobjLifeHistory.icdoPersonAccountLifeHistory.reason_value == busConstant.AnnualEnrollment
                            || lobjLifeHistory.icdoPersonAccountLifeHistory.reason_value == busConstant.AnnualEnrollmentWaived) &&
                            (lobjLifeHistory.icdoPersonAccountLifeHistory.ps_file_change_event_value == busConstant.AnnualEnrollment ||
                            lobjLifeHistory.icdoPersonAccountLifeHistory.ps_file_change_event_value == busConstant.AnnualEnrollmentWaived))
                        {
                            lobjDailyPAPeopleSoft.iblnAnnualEnrollment = true;
                            lobjDailyPAPeopleSoft.istrHistoryPSFileChangeEventValue = lobjLifeHistory.icdoPersonAccountLifeHistory.ps_file_change_event_value;
                        }

                        //PIR 20178
                        if (lintPSCounter == 0)
                        {
                            //PIR 20135 - Issue 1

                            //PIR 24581
                            DBFunction.DBNonQuery("cdoPersonAccount.UpdatePrevPeopleSoftRecords", new object[5] { icdoPersonAccount.person_id, icdoPersonAccount.plan_id ,
                            iclbPersonAccountLifeHistory[0].icdoPersonAccountLifeHistory.effective_start_date, lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id,icdoPersonAccount.person_account_id },
                            iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                            
                             DBFunction.DBNonQuery("cdoPersonAccount.UpdatePrevBenefitEnrollmentRecords", new object[5] { icdoPersonAccount.person_id, icdoPersonAccount.plan_id ,
                             iclbPersonAccountLifeHistory[0].icdoPersonAccountLifeHistory.effective_start_date, lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id,icdoPersonAccount.person_account_id },
                             iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                       

                            lintPSCounter++;
                        }

                        //PIR 26238
                        if (lintBERCounter == 0)
                        {
                            if (iclbOverlappingHistory.IsNotNull() && iclbOverlappingHistory.Count() > 0)
                            {
                                foreach (busPersonAccountLifeHistory lobjDeletedHistory in iclbOverlappingHistory)
                                {
                                    DBFunction.DBNonQuery("cdoPersonAccount.UpdatePrevPeopleSoftNBenefitEnrlFlag", new object[2] { lobjDeletedHistory.icdoPersonAccountLifeHistory.person_account_life_history_id, icdoPersonAccount.person_account_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                                }
                                lintBERCounter++;
                            }
                        }

                        //PeopleSoft logic will only be executed if the ORG GROUP of the current Organization is PeopleSoft. 
                        if ((lobjDailyPAPeopleSoft.iclbPeopleSoftOrgGroupValue.Where(i => i.code_value == lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.peoplesoft_org_group_value && i.data2 == busConstant.Flag_Yes).Count() > 0))
                        {
                            if ((lobjLifeOption.icdoPersonAccountLifeOption.effective_start_date != DateTime.MinValue) &&
                                        (lobjLifeOption.icdoPersonAccountLifeOption.effective_end_date_no_null > DateTime.Today))
                            {
                                if (lobjLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_Basic)
                                {
                                    lobjDailyPAPeopleSoft.idecFlatAmount = idecLifeBasicPremiumAmt;
                                }
                                else if (lobjLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_Supplemental)
                                {
                                    lobjDailyPAPeopleSoft.idecSuppPremiumAmount = idecLifeSupplementalPremiumAmount;
                                    lobjDailyPAPeopleSoft.idecSuppCoverageAmount = lobjLifeOption.icdoPersonAccountLifeOption.coverage_amount;
                                }
                                else if (lobjLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_DependentSupplemental)
                                {
                                    lobjDailyPAPeopleSoft.idecDependentSuppCoverageAmount = lobjLifeOption.icdoPersonAccountLifeOption.coverage_amount;
                                    lobjDailyPAPeopleSoft.idecFlatAmount = idecDependentSupplementalPremiumAmt;
                                }
                                else if (lobjLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_SpouseSupplemental)
                                {
                                    lobjDailyPAPeopleSoft.idecSpouseSuppCoverageAmount = lobjLifeOption.icdoPersonAccountLifeOption.coverage_amount;
                                    lobjDailyPAPeopleSoft.idecFlatAmount = idecSpouseSupplementalPremiumAmt;
                                }
                                lobjDailyPAPeopleSoft.idtPlanOptionStartDate = lobjLifeOption.icdoPersonAccountLifeOption.effective_start_date;
                                lobjDailyPAPeopleSoft.istrLevelOfCoverage = lobjLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value;

                                if (lobjLifeHistory.icdoPersonAccountLifeHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended ||
                                        lobjLifeHistory.icdoPersonAccountLifeHistory.plan_option_status_value == busConstant.PlanOptionStatusValueWaived)
                                {
                                    lobjEnrollmentData.icdoEnrollmentData.coverage_election_value = "T";
                                    lobjEnrollmentData.icdoEnrollmentData.pay_period_amount = 0.0m;
                                    lobjDailyPAPeopleSoft.iblnIsPlanOptionSuspended = true;
                                }

                                lobjDailyPAPeopleSoft.idecBasicSuppCoverageAmount = ldecBasicSuppAmount;//PIR 23527

                                lobjDailyPAPeopleSoft.istrInsuranceTypeValue =  lobjLifeHistory.icdoPersonAccountLifeHistory.life_insurance_type_value;
                                lobjDailyPAPeopleSoft.idtHistoryChangeDate = icdoPersonAccount.history_change_date;
                                lobjDailyPAPeopleSoft.idecBasicCoverageAmount = ldecBasicAmount;

                                lobjDailyPAPeopleSoft.GeneratePeoplesoftEntryForLife(lobjDailyPAPeopleSoft.istrEmpTypeValue);

                                if (lobjDailyPAPeopleSoft.iclbDailyPersonAccountPeopleSoft.IsNotNull() && lobjDailyPAPeopleSoft.iclbDailyPersonAccountPeopleSoft.Count > 0)
                                {
                                    foreach (busDailyPersonAccountPeopleSoft lobjDailyPeopleSoft in lobjDailyPAPeopleSoft.iclbDailyPersonAccountPeopleSoft)
                                    {
                                        lobjEnrollmentData.icdoEnrollmentData.source_id = lobjLifeHistory.icdoPersonAccountLifeHistory.person_account_life_history_id;
                                        lobjEnrollmentData.icdoEnrollmentData.plan_id = icdoPersonAccount.plan_id;
                                        lobjEnrollmentData.icdoEnrollmentData.employment_type_value = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value;
                                        lobjEnrollmentData.icdoEnrollmentData.ssn = ibusPerson.icdoPerson.ssn;
                                        lobjEnrollmentData.icdoEnrollmentData.ndpers_member_id = ibusPerson.icdoPerson.person_id;
                                        lobjEnrollmentData.icdoEnrollmentData.person_account_id = icdoPersonAccount.person_account_id;
                                        lobjEnrollmentData.icdoEnrollmentData.peoplesoft_id = ibusPerson.icdoPerson.peoplesoft_id;
                                        lobjEnrollmentData.icdoEnrollmentData.employer_org_id = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id;
                                        lobjEnrollmentData.icdoEnrollmentData.plan_status_value = lobjLifeHistory.icdoPersonAccountLifeHistory.plan_participation_status_value;
                                        lobjEnrollmentData.icdoEnrollmentData.change_reason_value = lobjLifeHistory.icdoPersonAccountLifeHistory.reason_value;
                                        lobjEnrollmentData.icdoEnrollmentData.start_date = lobjLifeHistory.icdoPersonAccountLifeHistory.effective_start_date;
                                        lobjEnrollmentData.icdoEnrollmentData.end_date = lobjLifeHistory.icdoPersonAccountLifeHistory.effective_end_date;
                                        lobjEnrollmentData.icdoEnrollmentData.level_of_coverage_value = lobjLifeHistory.icdoPersonAccountLifeHistory.level_of_coverage_value;
                                        lobjEnrollmentData.icdoEnrollmentData.coverage_amount = lobjLifeHistory.icdoPersonAccountLifeHistory.coverage_amount;
                                        lobjEnrollmentData.icdoEnrollmentData.provider_org_id = ibusProvider.icdoOrganization.org_id;

                                        lobjEnrollmentData.icdoEnrollmentData.coverage_code = lobjDailyPeopleSoft.istrCoverageCode;
                                        lobjEnrollmentData.icdoEnrollmentData.deduction_begin_date = lobjDailyPeopleSoft.idtDeductionBeginDate;
                                        lobjEnrollmentData.icdoEnrollmentData.coverage_begin_date = lobjDailyPeopleSoft.idtCoverageBeginDate;
                                        lobjEnrollmentData.icdoEnrollmentData.plan_type = lobjDailyPeopleSoft.istrPlanType;

                                        if (lobjEnrollmentData.icdoEnrollmentData.plan_status_value != busConstant.PlanParticipationStatusInsuranceEnrolled ||
                                            lobjLifeHistory.icdoPersonAccountLifeHistory.plan_option_status_value == busConstant.PlanOptionStatusValueWaived)
                                        {
                                            if (lobjEnrollmentData.icdoEnrollmentData.coverage_election_value.IsNullOrEmpty())
                                                lobjEnrollmentData.icdoEnrollmentData.coverage_election_value = lobjDailyPeopleSoft.istrCoverageElection;

                                            if (lobjEnrollmentData.icdoEnrollmentData.coverage_election_value == "T")
                                            {
                                                lobjEnrollmentData.icdoEnrollmentData.benefit_plan = string.Empty;
                                                lobjEnrollmentData.icdoEnrollmentData.pretax_amount = 0.0m;
                                            }
                                            else
                                                lobjEnrollmentData.icdoEnrollmentData.benefit_plan = lobjDailyPeopleSoft.istrBenefitPlan;
                                        }
                                        else
                                        {
                                            lobjEnrollmentData.icdoEnrollmentData.coverage_election_value = lobjDailyPeopleSoft.istrCoverageElection;
                                            lobjEnrollmentData.icdoEnrollmentData.benefit_plan = lobjDailyPeopleSoft.istrBenefitPlan;
                                            lobjEnrollmentData.icdoEnrollmentData.pretax_amount = lobjDailyPeopleSoft.idecFlatAmount;
                                        }
                                        lobjEnrollmentData.icdoEnrollmentData.election_date = lobjDailyPeopleSoft.idtElectionDate;

                                        lobjEnrollmentData.icdoEnrollmentData.monthly_premium = ldecEmployeePremiumAmount + ldecEmployerPremiumAmount;
                                        lobjEnrollmentData.icdoEnrollmentData.peoplesoft_org_group_value = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.peoplesoft_org_group_value;
                                        lobjEnrollmentData.icdoEnrollmentData.peoplesoft_change_reason_value = lobjDailyPAPeopleSoft.istrPSFileChangeEvent;


                                        if (lobjEnrollmentData.icdoEnrollmentData.level_of_coverage_value != busConstant.LevelofCoverage_Supplemental)
                                        {
                                            if (((lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date == DateTime.MinValue || (lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date != DateTime.MinValue &&
                                                 busGlobalFunctions.CheckDateOverlapping(icdoPersonAccount.history_change_date.AddMonths(-1), lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date,
                                                 new DateTime(lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date.Year, lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date.Month, 1).AddMonths(2))))) &&
                                                 lintCounter == 0)
                                                lobjEnrollmentData.icdoEnrollmentData.is_benefit_enrollment_report_generated = busConstant.Flag_No;
                                            else
                                                lobjEnrollmentData.icdoEnrollmentData.is_benefit_enrollment_report_generated = busConstant.Flag_Yes;
                                            lintCounter++;
                                        }
                                        else
                                        {   //Benefit Plan FlxLif should not shown on Benefit enrollment report
                                            if (lobjEnrollmentData.icdoEnrollmentData.benefit_plan == busConstant.PeopleSoftFileBenefitPlanFlxLif && lobjDailyPAPeopleSoft.iclbDailyPersonAccountPeopleSoft.Count > 1)
                                                lobjEnrollmentData.icdoEnrollmentData.is_benefit_enrollment_report_generated = busConstant.Flag_Yes;
                                            else if ((lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date == DateTime.MinValue || (lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date != DateTime.MinValue &&
                                                 busGlobalFunctions.CheckDateOverlapping(icdoPersonAccount.history_change_date.AddMonths(-1), lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date,
                                                 new DateTime(lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date.Year, lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date.Month, 1).AddMonths(2)))))
                                                lobjEnrollmentData.icdoEnrollmentData.is_benefit_enrollment_report_generated = busConstant.Flag_No;
                                            else
                                                lobjEnrollmentData.icdoEnrollmentData.is_benefit_enrollment_report_generated = busConstant.Flag_Yes;
                                            lobjEnrollmentData.icdoEnrollmentData.pretaxed_premiums = lobjLifeHistory.icdoPersonAccountLifeHistory.premium_conversion_indicator_flag; //Pretaxed premium is only for Supp 
                                        }
                                        lobjEnrollmentData.icdoEnrollmentData.peoplesoft_file_sent_flag = busConstant.Flag_No;
                                        lobjEnrollmentData.icdoEnrollmentData.Insert();

                                        //PIR 23856 - Update previous PS sent flag in Enrollment Data to Y if Person Employment is changed (transfer).
                                        DBFunction.DBNonQuery("cdoPersonAccount.UpdatePSSentFlagForTransfers", new object[5] { icdoPersonAccount.plan_id,icdoPersonAccount.person_id,
                                                                                                  lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id,
                                                                                                  lobjEnrollmentData.icdoEnrollmentData.deduction_begin_date,icdoPersonAccount.person_account_id },
                                                                                                                  iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                                    }
                                }
                            }
                        }
                        else //If ORG GROUP of the current Organization is not PeopleSoft, then 1 entry for Benefit Enrollment.
                        {
                            if ((lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date == DateTime.MinValue || (lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date != DateTime.MinValue &&
                                 busGlobalFunctions.CheckDateOverlapping(icdoPersonAccount.history_change_date.AddMonths(-1), lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date,
                                new DateTime(lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date.Year, lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date.Month, 1).AddMonths(2)))))
                            {
                                lobjEnrollmentData.icdoEnrollmentData.source_id = lobjLifeHistory.icdoPersonAccountLifeHistory.person_account_life_history_id;
                                lobjEnrollmentData.icdoEnrollmentData.plan_id = icdoPersonAccount.plan_id;
                                lobjEnrollmentData.icdoEnrollmentData.employment_type_value = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value;
                                lobjEnrollmentData.icdoEnrollmentData.ssn = ibusPerson.icdoPerson.ssn;
                                lobjEnrollmentData.icdoEnrollmentData.ndpers_member_id = ibusPerson.icdoPerson.person_id;
                                lobjEnrollmentData.icdoEnrollmentData.person_account_id = icdoPersonAccount.person_account_id;
                                lobjEnrollmentData.icdoEnrollmentData.peoplesoft_id = ibusPerson.icdoPerson.peoplesoft_id;
                                lobjEnrollmentData.icdoEnrollmentData.employer_org_id = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id;
                                lobjEnrollmentData.icdoEnrollmentData.plan_status_value = lobjLifeHistory.icdoPersonAccountLifeHistory.plan_participation_status_value;
                                lobjEnrollmentData.icdoEnrollmentData.change_reason_value = lobjLifeHistory.icdoPersonAccountLifeHistory.reason_value;
                                lobjEnrollmentData.icdoEnrollmentData.provider_org_id = ibusProvider.icdoOrganization.org_id;
                                lobjEnrollmentData.icdoEnrollmentData.start_date = lobjLifeHistory.icdoPersonAccountLifeHistory.effective_start_date;
                                lobjEnrollmentData.icdoEnrollmentData.end_date = lobjLifeHistory.icdoPersonAccountLifeHistory.effective_end_date;
                                lobjEnrollmentData.icdoEnrollmentData.level_of_coverage_value = lobjLifeHistory.icdoPersonAccountLifeHistory.level_of_coverage_value;
                                lobjEnrollmentData.icdoEnrollmentData.coverage_amount = lobjLifeHistory.icdoPersonAccountLifeHistory.coverage_amount;
                                lobjEnrollmentData.icdoEnrollmentData.monthly_premium = ldecEmployeePremiumAmount + ldecEmployerPremiumAmount;
                                lobjEnrollmentData.icdoEnrollmentData.peoplesoft_org_group_value = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.peoplesoft_org_group_value;

                                if (lobjEnrollmentData.icdoEnrollmentData.level_of_coverage_value == busConstant.LevelofCoverage_Supplemental)
                                    lobjEnrollmentData.icdoEnrollmentData.pretaxed_premiums = lobjLifeHistory.icdoPersonAccountLifeHistory.premium_conversion_indicator_flag;
                                lobjEnrollmentData.icdoEnrollmentData.is_benefit_enrollment_report_generated = busConstant.Flag_No;
                                lobjEnrollmentData.icdoEnrollmentData.peoplesoft_file_sent_flag = busConstant.Flag_Yes;
                                lobjEnrollmentData.icdoEnrollmentData.Insert();
                            }
                        }
                    }
                }
            }
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
                    GetMonthlyPremiumAmount();
                    string lstrMessage = string.Format(busGlobalFunctions.GetDBMessageTextByDBMessageID(27, iobjPassInfo, ref lstrPrioityValue),
                        ibusPerson.icdoPerson.FullName, ibusPerson.icdoPerson.LastFourDigitsOfSSN);
                    if (idecLifeBasicPremiumAmt > 0.0M || (idecLifeSupplementalPremiumAmount + idecDependentSupplementalPremiumAmt + idecSpouseSupplementalPremiumAmt) > 0.0M)
                        lstrMessage += string.Format(busGlobalFunctions.GetDBMessageTextByDBMessageID(28, iobjPassInfo, ref lstrPrioityValue),
                             idecLifeBasicPremiumAmt, (idecLifeSupplementalPremiumAmount + idecDependentSupplementalPremiumAmt + idecSpouseSupplementalPremiumAmt),
                            icdoPersonAccount.history_change_date.ToString("d", CultureInfo.CreateSpecificCulture("en-US")));
                    if (ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id > 0)
                    {
                        // PIR 9115
                        if (istrIsPIR9115Enabled == busConstant.Flag_No)
                        {
                            busWSSHelper.PublishESSMessage(0, 0, lstrMessage, lstrPrioityValue, aintPlanID: busConstant.PlanIdGroupLife, aintOrgID: ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id,
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
                                                                busConstant.PlanIdGroupLife, iobjPassInfo);
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
            icdoPersonAccountLife.reason_value = string.Empty;  // UAT PIR ID 1043
        }

        private void CreateAdjustmentPayrollForEnrollmentHistoryClose()
        {
            busEmployerPayrollHeader lbusPayrollHdr = null;
            busIbsHeader lbusIBSHdr = null;
            int lintOrgID = 0;
            DateTime ldatEffectedDate = icdoPersonAccount.history_change_date;
            DateTime ldatEndDate = icdoPersonAccount.history_change_date.AddDays(-1);
            decimal ldecOldPremium = 0.00M;
            decimal ldecOldBasicPremium = 0.00M;
            decimal ldecOldSuppPremium = 0.00M;
            decimal ldecOldSpouseSuppPremium = 0.00M;
            decimal ldecOldDepSuppPremium = 0.00M;
            //ucs - 038 addendum, new col. for paid premium amount
            decimal ldecPaidPremiumAmount = 0.0M;
            //PROD pir 4260
            decimal ldecADAndDBasicRate = 0.0000M;
            decimal ldecADAndDSuppRate = 0.0000M;
            decimal ldecBasicCoverageAmount = 0.00M;
            decimal ldecSuppCoverageAmount = 0.00M;
            decimal ldecSpouSuppCoverageAmount = 0.00M;
            decimal ldecDepSuppCoverageAmount = 0.00M;

            if (iclbInsuranceContributionAll == null)
                LoadInsuranceContributionAll();
            if (ibusPaymentElection == null)
                LoadPaymentElection();
            //prod pir 5831 : need to group IBS and employer reporting as separate
            var lenuIBSContributionByMonth =
                iclbInsuranceContributionAll.Where(
                    i =>
                    i.icdoPersonAccountInsuranceContribution.subsystem_value == busConstant.SubSystemValueIBSDeposit).
                    GroupBy(i => i.icdoPersonAccountInsuranceContribution.effective_date).Select(o => o.First());
           
            if (lenuIBSContributionByMonth != null)
            {
                foreach (busPersonAccountInsuranceContribution lbusInsuranceContribution in lenuIBSContributionByMonth)
                {
                    if (lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date >= ldatEffectedDate)
                    {
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

                            //For Negative Adjustment Premoum Amount, we should not only consider Regular.. we also need to consider other adjustment too
                            //Example Scenario : Member was in Single First and Then changed Family and then suspended. This case Single - Family Change
                            //Could have created the adjustment record which also we need to consider while creating Neg Adjustment for suspended case.
                            var lclbFilterdIBSContribution = _iclbInsuranceContributionAll.Where(
                                i =>
                                i.icdoPersonAccountInsuranceContribution.subsystem_value == busConstant.SubSystemValueIBSDeposit &&
                                i.icdoPersonAccountInsuranceContribution.effective_date == lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date);
                            //ucs - 038 addendum, new col. for paid premium amount
                            var lclbFilterdPaidIBSContribution = _iclbInsuranceContributionAll.Where(
                                i =>
                                i.icdoPersonAccountInsuranceContribution.subsystem_value == busConstant.SubSystemValueIBSPayment &&
                                i.icdoPersonAccountInsuranceContribution.effective_date == lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date);

                            ldecOldBasicPremium = lclbFilterdIBSContribution.Sum(i => i.icdoPersonAccountInsuranceContribution.life_basic_premium_amount);
                            ldecOldSuppPremium = lclbFilterdIBSContribution.Sum(i => i.icdoPersonAccountInsuranceContribution.life_supp_premium_amount);
                            ldecOldSpouseSuppPremium = lclbFilterdIBSContribution.Sum(i => i.icdoPersonAccountInsuranceContribution.life_spouse_supp_premium_amount);
                            ldecOldDepSuppPremium = lclbFilterdIBSContribution.Sum(i => i.icdoPersonAccountInsuranceContribution.life_dep_supp_premium_amount);
                            ldecOldPremium = ldecOldBasicPremium + ldecOldSuppPremium + ldecOldSpouseSuppPremium + ldecOldDepSuppPremium;
                            ldecPaidPremiumAmount = 0.0M;
                            ldecADAndDBasicRate = lclbFilterdIBSContribution.Where(i => i.icdoPersonAccountInsuranceContribution.ad_and_d_basic_premium_rate > 0)
                                                                            .Select(i => i.icdoPersonAccountInsuranceContribution.ad_and_d_basic_premium_rate).FirstOrDefault();
                            ldecADAndDSuppRate = lclbFilterdIBSContribution.Where(i => i.icdoPersonAccountInsuranceContribution.ad_and_d_supplemental_premium_rate > 0)
                                                                            .Select(i => i.icdoPersonAccountInsuranceContribution.ad_and_d_supplemental_premium_rate).FirstOrDefault();
                            ldecBasicCoverageAmount = lclbFilterdIBSContribution.Where(i => i.icdoPersonAccountInsuranceContribution.life_basic_coverage_amount > 0)
                                                                            .Select(i => i.icdoPersonAccountInsuranceContribution.life_basic_coverage_amount).FirstOrDefault();
                            ldecSuppCoverageAmount = lclbFilterdIBSContribution.Where(i => i.icdoPersonAccountInsuranceContribution.life_supp_coverage_amount > 0)
                                                                            .Select(i => i.icdoPersonAccountInsuranceContribution.life_supp_coverage_amount).FirstOrDefault();
                            ldecSpouSuppCoverageAmount = lclbFilterdIBSContribution.Where(i => i.icdoPersonAccountInsuranceContribution.life_spouse_supp_coverage_amount > 0)
                                                                            .Select(i => i.icdoPersonAccountInsuranceContribution.life_spouse_supp_coverage_amount).FirstOrDefault();
                            ldecDepSuppCoverageAmount = lclbFilterdIBSContribution.Where(i => i.icdoPersonAccountInsuranceContribution.life_dep_supp_coverage_amount > 0)
                                                                            .Select(i => i.icdoPersonAccountInsuranceContribution.life_dep_supp_coverage_amount).FirstOrDefault();
                            //ucs - 038 addendum, new col. for paid premium amount
                            if (lclbFilterdPaidIBSContribution != null)
                                ldecPaidPremiumAmount = lclbFilterdPaidIBSContribution.Sum(o => o.icdoPersonAccountInsuranceContribution.paid_premium_amount);
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
                            if (ldecOldPremium != 0 &&
                               (IsAdjustmentExistsPriorToEnteredEffectiveDate(lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.provider_org_id, lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date, -1M * ldecOldPremium, istrOldPaymentMethod)))
                                lbusIBSHdr.AddIBSDetailForLife(icdoPersonAccount.person_account_id, icdoPersonAccount.person_id,
                                    icdoPersonAccount.plan_id, lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date,
                                    istrOldPaymentMethod,
                                -1M * ldecOldPremium,
                                -1M * ldecOldPremium,
                                -1M * ldecOldPremium,
                                -1M * ldecOldBasicPremium,
                                -1M * ldecOldSuppPremium,
                                -1M * ldecOldSpouseSuppPremium,
                                -1M * ldecOldDepSuppPremium,
                                -1M * ldecADAndDBasicRate,
                                -1M * ldecADAndDSuppRate,
                                -1M * ldecBasicCoverageAmount,
                                -1M * ldecSuppCoverageAmount,
                                -1M * ldecSpouSuppCoverageAmount,
                                -1M * ldecDepSuppCoverageAmount, lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.provider_org_id,
                                adecPaidPremiumAmount: -1M * ldecPaidPremiumAmount, acolNegPosIBsDetail: icolPosNegIbsDetail);//ucs - 038 addendum : added new col. paid premium amount
                        }
                    }
                }
            }
            //prod pir 5831 : need to group IBS and employer reporting as separate
            var lenuPayrollContributionByMonth =
               iclbInsuranceContributionAll.Where(
                   i =>
                   i.icdoPersonAccountInsuranceContribution.subsystem_value == busConstant.SubSystemValueEmployerReporting).
                   GroupBy(i => i.icdoPersonAccountInsuranceContribution.effective_date).Select(o => o.First());

            if (lenuPayrollContributionByMonth != null)
            {
                foreach (busPersonAccountInsuranceContribution lbusInsuranceContribution in lenuPayrollContributionByMonth)
                {
                    if (lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date >= ldatEffectedDate)
                    {
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
                                var lclbFilterdPayrollContribution = _iclbInsuranceContributionAll.Where(
                                    i =>
                                    i.icdoPersonAccountInsuranceContribution.subsystem_value == busConstant.SubSystemValueEmployerReporting &&
                                    i.icdoPersonAccountInsuranceContribution.effective_date == lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date);

                                ldecOldBasicPremium = lclbFilterdPayrollContribution.Sum(i => i.icdoPersonAccountInsuranceContribution.life_basic_premium_amount);
                                ldecOldSuppPremium = lclbFilterdPayrollContribution.Sum(i => i.icdoPersonAccountInsuranceContribution.life_supp_premium_amount);
                                ldecOldSpouseSuppPremium = lclbFilterdPayrollContribution.Sum(i => i.icdoPersonAccountInsuranceContribution.life_spouse_supp_premium_amount);
                                ldecOldDepSuppPremium = lclbFilterdPayrollContribution.Sum(i => i.icdoPersonAccountInsuranceContribution.life_dep_supp_premium_amount);
                                ldecOldPremium = ldecOldBasicPremium + ldecOldSuppPremium + ldecOldSpouseSuppPremium + ldecOldDepSuppPremium;
                                ldecADAndDBasicRate = lclbFilterdPayrollContribution.Where(i => i.icdoPersonAccountInsuranceContribution.ad_and_d_basic_premium_rate > 0)
                                                                            .Select(i => i.icdoPersonAccountInsuranceContribution.ad_and_d_basic_premium_rate).FirstOrDefault();
                                ldecADAndDSuppRate = lclbFilterdPayrollContribution.Where(i => i.icdoPersonAccountInsuranceContribution.ad_and_d_supplemental_premium_rate > 0)
                                                                                .Select(i => i.icdoPersonAccountInsuranceContribution.ad_and_d_supplemental_premium_rate).FirstOrDefault();
                                ldecBasicCoverageAmount = lclbFilterdPayrollContribution.Where(i => i.icdoPersonAccountInsuranceContribution.life_basic_coverage_amount > 0)
                                                                                .Select(i => i.icdoPersonAccountInsuranceContribution.life_basic_coverage_amount).FirstOrDefault();
                                ldecSuppCoverageAmount = lclbFilterdPayrollContribution.Where(i => i.icdoPersonAccountInsuranceContribution.life_supp_coverage_amount > 0)
                                                                                .Select(i => i.icdoPersonAccountInsuranceContribution.life_supp_coverage_amount).FirstOrDefault();
                                ldecSpouSuppCoverageAmount = lclbFilterdPayrollContribution.Where(i => i.icdoPersonAccountInsuranceContribution.life_spouse_supp_coverage_amount > 0)
                                                                                .Select(i => i.icdoPersonAccountInsuranceContribution.life_spouse_supp_coverage_amount).FirstOrDefault();
                                ldecDepSuppCoverageAmount = lclbFilterdPayrollContribution.Where(i => i.icdoPersonAccountInsuranceContribution.life_dep_supp_coverage_amount > 0)
                                                                                .Select(i => i.icdoPersonAccountInsuranceContribution.life_dep_supp_coverage_amount).FirstOrDefault();
                                if (icolPosNegEmployerPayrollDtl.IsNull())
                                    icolPosNegEmployerPayrollDtl = new Collection<busEmployerPayrollDetail>();
                                if (ldecOldPremium > 0 && IsPayrollDtlAdjustmentExistsPriorToEnteredEffectiveDate(lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.provider_org_id, 
                                    lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date, ldecOldPremium, busConstant.PayrollDetailRecordTypeNegativeAdjustment))
                                {
                                    lbusPayrollHdr.AddInsuranceAdjustmentPayrollDetail(icdoPersonAccount.person_id, icdoPersonAccount.plan_id, ibusPerson.icdoPerson.first_name,
                                        ibusPerson.icdoPerson.last_name, ibusPerson.icdoPerson.ssn, busConstant.PayrollDetailRecordTypeNegativeAdjustment,
                                        lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date, ldecOldPremium,
                                        lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.provider_org_id,
                                        adecBasicPremium: ldecOldBasicPremium, adecSuppPremium: ldecOldSuppPremium,
                                        adecSpouseSuppPremium: ldecOldSpouseSuppPremium, adecDepSuppPremium: ldecOldDepSuppPremium, adecADAndDBasiceRate: ldecADAndDBasicRate,
                            adecADAndDSuppRate: ldecADAndDSuppRate, adecBasicCoverageAmount: ldecBasicCoverageAmount, adecSuppCoverageAmount: ldecSuppCoverageAmount,
                            adecSpouSuppCoverageAmount: ldecSpouSuppCoverageAmount, adecDepSuppCoverageAmount: ldecDepSuppCoverageAmount, acolPosNegEmployerPayrollDtl: icolPosNegEmployerPayrollDtl);
                                }
                                else if (ldecOldPremium < 0 && IsPayrollDtlAdjustmentExistsPriorToEnteredEffectiveDate(lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.provider_org_id,
                                    lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date, ldecOldPremium * -1, busConstant.PayrollDetailRecordTypePositiveAdjustment))
                                {
                                    lbusPayrollHdr.UpdateEmployerPayrollDetailStatusReviewAndValidToIgnore(icdoPersonAccount.person_id, icdoPersonAccount.plan_id, lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date);
                                    lbusPayrollHdr.AddInsuranceAdjustmentPayrollDetail(icdoPersonAccount.person_id, icdoPersonAccount.plan_id, ibusPerson.icdoPerson.first_name,
                                        ibusPerson.icdoPerson.last_name, ibusPerson.icdoPerson.ssn, busConstant.PayrollDetailRecordTypePositiveAdjustment,
                                        lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date, ldecOldPremium * -1,
                                        lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.provider_org_id,
                                        adecBasicPremium: ldecOldBasicPremium * -1, adecSuppPremium: ldecOldSuppPremium * -1,
                                        adecSpouseSuppPremium: ldecOldSpouseSuppPremium * -1, adecDepSuppPremium: ldecOldDepSuppPremium * -1, adecADAndDBasiceRate: ldecADAndDBasicRate * -1,
                            adecADAndDSuppRate: ldecADAndDSuppRate * -1, adecBasicCoverageAmount: ldecBasicCoverageAmount * -1, adecSuppCoverageAmount: ldecSuppCoverageAmount * -1,
                            adecSpouSuppCoverageAmount: ldecSpouSuppCoverageAmount * -1, adecDepSuppCoverageAmount: ldecDepSuppCoverageAmount * -1, acolPosNegEmployerPayrollDtl: icolPosNegEmployerPayrollDtl);
                                }
                            }
                        }
                    }
                }
            }
			//IBS Adjustment creation logic changed (Affected area enroll to enroll scenario) - logged under Backlog PIR 8022 and 8111
            // Update changes
            if ((lbusIBSHdr != null) && (lbusIBSHdr.icolIbsDetail != null) && (lbusIBSHdr.icolIbsDetail.Count > 0)
                && (icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusInsuranceEnrolled
                || istrOldPlanParticipationStatus != busConstant.PlanParticipationStatusInsuranceEnrolled))
            {
                lbusIBSHdr.UpdateSummaryData(busConstant.IBSHeaderStatusReview); //?? Save detail here
                foreach (busIbsDetail lbusIBSDtl in lbusIBSHdr.icolIbsDetail)
                {
                    lbusIBSDtl.icdoIbsDetail.ibs_header_id = lbusIBSHdr.icdoIbsHeader.ibs_header_id;
                    lbusIBSDtl.UpdateDataObject(lbusIBSDtl.icdoIbsDetail);
                }
            }
            if ((lbusPayrollHdr != null) && (lbusPayrollHdr.iclbEmployerPayrollDetail != null) && (lbusPayrollHdr.iclbEmployerPayrollDetail.Count > 0)
                && (icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusInsuranceEnrolled
                || istrOldPlanParticipationStatus != busConstant.PlanParticipationStatusInsuranceEnrolled))
            {
                lbusPayrollHdr.UpdateDataObject(lbusPayrollHdr.icdoEmployerPayrollHeader);
                foreach (busEmployerPayrollDetail lbusPayrollDtl in lbusPayrollHdr.iclbEmployerPayrollDetail)
                {
                    lbusPayrollDtl.icdoEmployerPayrollDetail.employer_payroll_header_id = lbusPayrollHdr.icdoEmployerPayrollHeader.employer_payroll_header_id;
                    lbusPayrollDtl.UpdateDataObject(lbusPayrollDtl.icdoEmployerPayrollDetail);
                }
            }
        }

        private void CreateAdjustmentPayrollForEnrollmentHistoryAdd()
        {
            busEmployerPayrollHeader lbusPayrollHdr = null;
            busIbsHeader lbusIBSHdr = null;
            int lintOrgID = 0;
            DateTime ldatEffectedDate = icdoPersonAccount.history_change_date;
            DateTime ldatCurrentIBSPayPeriod = DateTime.MinValue;
            DateTime ldatCurrentEmployerPayPeriod = DateTime.MinValue;
            decimal ldecOldPremium = 0.00M;
            decimal ldecOldBasicPremium = 0.00M;
            decimal ldecOldSuppPremium = 0.00M;
            decimal ldecOldSpouseSuppPremium = 0.00M;
            decimal ldecOldDepSuppPremium = 0.00M;
            if (iclbInsuranceContributionAll == null)
                LoadInsuranceContributionAll();
            if (ibusPaymentElection == null)
                LoadPaymentElection();

            if (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag == busConstant.Flag_Yes)
            {
                // PROD PIR ID 6259 -- Effective date should be taken from Employer payroll header or IBS header, not from Insurance Contribution.
                //var lenuIBSFilter =
                //_iclbInsuranceContributionAll.Where(
                //    i =>
                //    i.icdoPersonAccountInsuranceContribution.transaction_type_value == busConstant.TransactionTypeRegularIBS).
                //    OrderByDescending(i => i.icdoPersonAccountInsuranceContribution.effective_date).FirstOrDefault();

                //if (lenuIBSFilter != null)
                //    ldatCurrentIBSPayPeriod = lenuIBSFilter.icdoPersonAccountInsuranceContribution.effective_date;

                if (ldatCurrentIBSPayPeriod == DateTime.MinValue)
                {
                    DataTable ldtResult = Select<cdoIbsHeader>(
                                        new string[2] { "report_type_value", "REPORT_STATUS_VALUE" },
                                        new object[2] { busConstant.IBSHeaderReportTypeRegular, busConstant.IBSHeaderStatusPosted }, null, "BILLING_MONTH_AND_YEAR desc");
                    if ((ldtResult != null) && (ldtResult.Rows.Count > 0))
                    {
                        if (!Convert.IsDBNull(ldtResult.Rows[0]["BILLING_MONTH_AND_YEAR"]))
                        {
                            ldatCurrentIBSPayPeriod = Convert.ToDateTime(ldtResult.Rows[0]["BILLING_MONTH_AND_YEAR"]);
                        }
                    }
                }
            }

            if ((ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag != busConstant.Flag_Yes) ||
                (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id != 0) ||
                (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_supplemental_org_id != 0))
            {
                // PROD PIR ID 6259 -- Effective date should be taken from Employer payroll header or IBS header, not from Insurance Contribution.
                //var lenuPayrollFilter =
                //_iclbInsuranceContributionAll.Where(
                //    i =>
                //    i.icdoPersonAccountInsuranceContribution.transaction_type_value == busConstant.TransactionTypeRegularPayroll).
                //    OrderByDescending(i => i.icdoPersonAccountInsuranceContribution.effective_date).FirstOrDefault();

                //if (lenuPayrollFilter != null)
                //{
                //    ldatCurrentEmployerPayPeriod = lenuPayrollFilter.icdoPersonAccountInsuranceContribution.effective_date;
                //}

                if (ldatCurrentEmployerPayPeriod == DateTime.MinValue)
                {
                    //Reload the Employment based on the Effective Date. (There will be a scenario employment will be null on load but adjustment will have the employment)
                    if (icdoPersonAccount.person_employment_dtl_id == 0) // PIR 10816
                        icdoPersonAccount.person_employment_dtl_id = GetEmploymentDetailID(ldatEffectedDate, true);
                    if (icdoPersonAccount.person_employment_dtl_id > 0)
                    {
                        LoadPersonEmploymentDetail();
                        ibusPersonEmploymentDetail.LoadPersonEmployment();
                        lintOrgID = ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id;
                    }

                    if (lintOrgID == 0)
                    {
                        if (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id > 0)
                            lintOrgID = ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id;
                        else if (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_supplemental_org_id > 0)
                            lintOrgID = ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_supplemental_org_id;
                    }

                    if (lintOrgID > 0)
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
                                ldatCurrentEmployerPayPeriod = Convert.ToDateTime(ldtResult.Rows[0]["PAYROLL_PAID_DATE"]);
                            }
                        }
                    }
                }
            }

            if (ldatCurrentEmployerPayPeriod == DateTime.MinValue)
            {
                cdoCodeValue lcdoCodeValue = busGlobalFunctions.GetCodeValueDetails(busConstant.SystemConstantsAndVariablesCodeID, busConstant.SystemConstantsLastEmployerPostingDate);
                ldatCurrentEmployerPayPeriod = new DateTime(Convert.ToDateTime(lcdoCodeValue.data1).Year, Convert.ToDateTime(lcdoCodeValue.data1).Month, 1);
            }
            if (ldatCurrentIBSPayPeriod == DateTime.MinValue)
            {
                cdoCodeValue lcdoIBSCodeValue = busGlobalFunctions.GetCodeValueDetails(busConstant.SystemConstantsAndVariablesCodeID, busConstant.SystemConstantsLastIBSPostingDate);
                ldatCurrentIBSPayPeriod = new DateTime(Convert.ToDateTime(lcdoIBSCodeValue.data1).Year, Convert.ToDateTime(lcdoIBSCodeValue.data1).Month, 1);
            }

            /*****************************IBS Member Pay ***************************************/
            if (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag == busConstant.Flag_Yes)
            {
                while (ldatEffectedDate <= ldatCurrentIBSPayPeriod)
                {
                    //Reload the Employment based on the Effective Date. (There will be a scenario employment will be null on load but adjustment will have the employment)
                    if (icdoPersonAccount.person_employment_dtl_id == 0) // PIR 10816
                        icdoPersonAccount.person_employment_dtl_id = GetEmploymentDetailID(ldatEffectedDate, true);
                    if (icdoPersonAccount.person_employment_dtl_id > 0)
                    {
                        LoadPersonEmploymentDetail();
                        ibusPersonEmploymentDetail.LoadPersonEmployment();
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
                    //Recalculate the Premium Based on the New Effective Date
                    GetMonthlyPremiumAmount(ldatEffectedDate);

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

                    //Get the Premium Amount based on the Payment Election
                    ldecOldPremium = 0.00M;
                    ldecOldBasicPremium = 0.00M;
                    ldecOldSuppPremium = 0.00M;
                    ldecOldSpouseSuppPremium = 0.00M;
                    ldecOldDepSuppPremium = 0.00M;

                    if (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id == 0)
                    {
                        GetPremiumSplitForHistoryAdd(ref ldecOldPremium,
                                                ref ldecOldBasicPremium, ref ldecOldSuppPremium,
                                                ref ldecOldSpouseSuppPremium, ref ldecOldDepSuppPremium, true, false, false, false);
                    }

                    if (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_supplemental_org_id == 0)
                    {
                        GetPremiumSplitForHistoryAdd(ref ldecOldPremium,
                                                 ref ldecOldBasicPremium, ref ldecOldSuppPremium,
                                                 ref ldecOldSpouseSuppPremium, ref ldecOldDepSuppPremium, false, true, false, false);
                    }

                    GetPremiumSplitForHistoryAdd(ref ldecOldPremium,
                                                 ref ldecOldBasicPremium, ref ldecOldSuppPremium,
                                                 ref ldecOldSpouseSuppPremium, ref ldecOldDepSuppPremium, false, false, true, false);
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
                    if (ldecOldPremium != 0)
                    {
                        //PIR 24918
                        if (IsAdjustmentExistsPriorToEnteredEffectiveDate(ibusProviderOrgPlan.icdoOrgPlan.org_id, ldatEffectedDate, ldecOldPremium, lstrPaymentMethod))
                        {
                            lbusIBSHdr.UpdateIBSDetailStatusReviewAndValidToIgnore(icdoPersonAccount.person_account_id, ldatEffectedDate);
                            lbusIBSHdr.AddIBSDetailForLife(icdoPersonAccount.person_account_id, icdoPersonAccount.person_id, icdoPersonAccount.plan_id,
                                    ldatEffectedDate, lstrPaymentMethod,
                                    ldecOldPremium, ldecOldPremium, ldecOldPremium, ldecOldBasicPremium,
                                    ldecOldSuppPremium, ldecOldSpouseSuppPremium, ldecOldDepSuppPremium, idecADAndDBasicRate, idecADAndDSupplementalRate,
                                    idecBasicCoverageAmount, idecSuppCoverageAmount, idecSpouseSuppCoverageAmount, idecDepSuppCoverageAmount, ibusProviderOrgPlan.icdoOrgPlan.org_id, acolNegPosIBsDetail: icolPosNegIbsDetail, aintPayeeAccountId: iPayeeAccountForPapit);

                        }
                    }
					ldatEffectedDate = ldatEffectedDate.AddMonths(1);
                }

                // Update changes
                //IBS Adjustment creation logic changed (Affected area enroll to enroll scenario) - logged under Backlog PIR 8022 and 8111
                Collection<busIbsDetail> lcolFinal =  ComparePositiveNegativeColIbsDetail();
                // Update changes
                if (lbusIBSHdr != null && (lcolFinal != null) && (lcolFinal.Count > 0))
                {
                    if (lbusIBSHdr != null)
                        lbusIBSHdr.UpdateSummaryData(busConstant.IBSHeaderStatusReview); //?? Save detail here
                    foreach (busIbsDetail lbusIBSDtl in lcolFinal)
                    {
                        if (lbusIBSHdr != null && lbusIBSDtl.icdoIbsDetail.ibs_header_id == 0)
                            lbusIBSDtl.icdoIbsDetail.ibs_header_id = lbusIBSHdr.icdoIbsHeader.ibs_header_id;
                        lbusIBSDtl.UpdateDataObject(lbusIBSDtl.icdoIbsDetail);
                    }
                }
            }

            if ((ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag != busConstant.Flag_Yes) ||
                (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id != 0) ||
                (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_supplemental_org_id != 0))
            {
                ldatEffectedDate = icdoPersonAccount.history_change_date;
                while (ldatEffectedDate <= ldatCurrentEmployerPayPeriod)
                {
                    //Reload the Employment based on the Effective Date. (There will be a scenario employment will be null on load but adjustment will have the employment)
                    if (icdoPersonAccount.person_employment_dtl_id == 0) // PIR 10816
                        icdoPersonAccount.person_employment_dtl_id = GetEmploymentDetailID(ldatEffectedDate, true);
                    if (icdoPersonAccount.person_employment_dtl_id > 0)
                    {
                        LoadPersonEmploymentDetail();
                        ibusPersonEmploymentDetail.LoadPersonEmployment();
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
                    //Recalculate the Premium Based on the New Effective Date
                    GetMonthlyPremiumAmount(ldatEffectedDate);

                    /*****************************Org To Bill Pay ***************************************/
                    //Organization to Bill for Basic Coverage
                    if (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id != 0)
                    {
                        lintOrgID = ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id;
                        lbusPayrollHdr = GetCurrentPayrollAdjustmentHeader(lintOrgID);

                        ldecOldPremium = 0.00M;
                        ldecOldBasicPremium = 0.00M;
                        ldecOldSuppPremium = 0.00M;
                        ldecOldSpouseSuppPremium = 0.00M;
                        ldecOldDepSuppPremium = 0.00M;
                        GetPremiumSplitForHistoryAdd(ref ldecOldPremium,
                                                     ref ldecOldBasicPremium, ref ldecOldSuppPremium,
                                                     ref ldecOldSpouseSuppPremium, ref ldecOldDepSuppPremium, true, false, false, false);
                        if (icolPosNegEmployerPayrollDtl.IsNull())
                            icolPosNegEmployerPayrollDtl = new Collection<busEmployerPayrollDetail>();
                        if (ldecOldPremium > 0 && IsPayrollDtlAdjustmentExistsPriorToEnteredEffectiveDate(ibusProviderOrgPlan.icdoOrgPlan.org_id, ldatEffectedDate, ldecOldPremium, busConstant.PayrollDetailRecordTypePositiveAdjustment))
                        {
                            lbusPayrollHdr.UpdateEmployerPayrollDetailStatusReviewAndValidToIgnore(icdoPersonAccount.person_id, icdoPersonAccount.plan_id, ldatEffectedDate);
                            lbusPayrollHdr.AddInsuranceAdjustmentPayrollDetail(icdoPersonAccount.person_id, icdoPersonAccount.plan_id, ibusPerson.icdoPerson.first_name,
                            ibusPerson.icdoPerson.last_name, ibusPerson.icdoPerson.ssn, busConstant.PayrollDetailRecordTypePositiveAdjustment,
                            ldatEffectedDate, ldecOldPremium, ibusProviderOrgPlan.icdoOrgPlan.org_id,
                            adecBasicPremium: ldecOldBasicPremium, adecADAndDBasiceRate: idecADAndDBasicRate,
                            adecADAndDSuppRate: idecADAndDSupplementalRate, adecBasicCoverageAmount: idecBasicCoverageAmount, adecSuppCoverageAmount: idecSuppCoverageAmount,
                            adecSpouSuppCoverageAmount: idecSpouseSuppCoverageAmount, adecDepSuppCoverageAmount: idecDepSuppCoverageAmount, acolPosNegEmployerPayrollDtl: icolPosNegEmployerPayrollDtl);
                        }
                        else if (ldecOldPremium < 0 && IsPayrollDtlAdjustmentExistsPriorToEnteredEffectiveDate(ibusProviderOrgPlan.icdoOrgPlan.org_id, ldatEffectedDate, ldecOldPremium * -1, busConstant.PayrollDetailRecordTypeNegativeAdjustment))
                        {
                            lbusPayrollHdr.AddInsuranceAdjustmentPayrollDetail(icdoPersonAccount.person_id, icdoPersonAccount.plan_id, ibusPerson.icdoPerson.first_name,
                            ibusPerson.icdoPerson.last_name, ibusPerson.icdoPerson.ssn, busConstant.PayrollDetailRecordTypeNegativeAdjustment,
                            ldatEffectedDate, ldecOldPremium * -1, ibusProviderOrgPlan.icdoOrgPlan.org_id,
                            adecBasicPremium: ldecOldBasicPremium * -1, adecADAndDBasiceRate: idecADAndDBasicRate * -1,
                            adecADAndDSuppRate: idecADAndDSupplementalRate * -1, adecBasicCoverageAmount: idecBasicCoverageAmount * -1, adecSuppCoverageAmount: idecSuppCoverageAmount * -1,
                            adecSpouSuppCoverageAmount: idecSpouseSuppCoverageAmount * -1, adecDepSuppCoverageAmount: idecDepSuppCoverageAmount * -1, acolPosNegEmployerPayrollDtl: icolPosNegEmployerPayrollDtl);
                        }

                        //Collection<busEmployerPayrollDetail> lcolFinal = ComparePositiveNegativeColEmployerPayrollDetail();
                        //if (lbusPayrollHdr != null &&(lcolFinal != null) && (lcolFinal.Count > 0))
                        //{
                        //    lbusPayrollHdr.UpdateDataObject(lbusPayrollHdr.icdoEmployerPayrollHeader);
                        //    foreach (busEmployerPayrollDetail lbusPayrollDtl in lcolFinal)
                        //    {
                        //        lbusPayrollDtl.icdoEmployerPayrollDetail.employer_payroll_header_id = lbusPayrollHdr.icdoEmployerPayrollHeader.employer_payroll_header_id;
                        //        lbusPayrollDtl.UpdateDataObject(lbusPayrollDtl.icdoEmployerPayrollDetail);
                        //    }
                        //}
                    }

                    /*****************************Org To Bill Pay ***************************************/
                    if (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_supplemental_org_id != 0)
                    {
                        lintOrgID = ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_supplemental_org_id;
                        lbusPayrollHdr = GetCurrentPayrollAdjustmentHeader(lintOrgID);

                        ldecOldPremium = 0.00M;
                        ldecOldBasicPremium = 0.00M;
                        ldecOldSuppPremium = 0.00M;
                        ldecOldSpouseSuppPremium = 0.00M;
                        ldecOldDepSuppPremium = 0.00M;
                        GetPremiumSplitForHistoryAdd(ref ldecOldPremium,
                                                     ref ldecOldBasicPremium, ref ldecOldSuppPremium,
                                                     ref ldecOldSpouseSuppPremium, ref ldecOldDepSuppPremium, false, true, false, false);
                        if (icolPosNegEmployerPayrollDtl.IsNull())
                            icolPosNegEmployerPayrollDtl = new Collection<busEmployerPayrollDetail>();
                        if (ldecOldPremium > 0 && IsPayrollDtlAdjustmentExistsPriorToEnteredEffectiveDate(ibusProviderOrgPlan.icdoOrgPlan.org_id, ldatEffectedDate, ldecOldPremium, busConstant.PayrollDetailRecordTypePositiveAdjustment))
                        {
                            lbusPayrollHdr.UpdateEmployerPayrollDetailStatusReviewAndValidToIgnore(icdoPersonAccount.person_id, icdoPersonAccount.plan_id, ldatEffectedDate);
                            lbusPayrollHdr.AddInsuranceAdjustmentPayrollDetail(icdoPersonAccount.person_id, icdoPersonAccount.plan_id, ibusPerson.icdoPerson.first_name,
                            ibusPerson.icdoPerson.last_name, ibusPerson.icdoPerson.ssn, busConstant.PayrollDetailRecordTypePositiveAdjustment,
                            ldatEffectedDate, ldecOldPremium, ibusProviderOrgPlan.icdoOrgPlan.org_id,
                            adecSuppPremium: ldecOldSuppPremium, adecADAndDBasiceRate: idecADAndDBasicRate,
                            adecADAndDSuppRate: idecADAndDSupplementalRate, adecBasicCoverageAmount: idecBasicCoverageAmount, adecSuppCoverageAmount: idecSuppCoverageAmount,
                            adecSpouSuppCoverageAmount: idecSpouseSuppCoverageAmount, adecDepSuppCoverageAmount: idecDepSuppCoverageAmount, acolPosNegEmployerPayrollDtl: icolPosNegEmployerPayrollDtl);
                        }
                        else if (ldecOldPremium < 0 && IsPayrollDtlAdjustmentExistsPriorToEnteredEffectiveDate(ibusProviderOrgPlan.icdoOrgPlan.org_id, ldatEffectedDate, ldecOldPremium * -1, busConstant.PayrollDetailRecordTypeNegativeAdjustment))
                        {
                            lbusPayrollHdr.AddInsuranceAdjustmentPayrollDetail(icdoPersonAccount.person_id, icdoPersonAccount.plan_id, ibusPerson.icdoPerson.first_name,
                            ibusPerson.icdoPerson.last_name, ibusPerson.icdoPerson.ssn, busConstant.PayrollDetailRecordTypeNegativeAdjustment,
                            ldatEffectedDate, ldecOldPremium * -1, ibusProviderOrgPlan.icdoOrgPlan.org_id,
                            adecSuppPremium: ldecOldSuppPremium * -1, adecADAndDBasiceRate: idecADAndDBasicRate * -1,
                            adecADAndDSuppRate: idecADAndDSupplementalRate * -1, adecBasicCoverageAmount: idecBasicCoverageAmount * -1, adecSuppCoverageAmount: idecSuppCoverageAmount * -1,
                            adecSpouSuppCoverageAmount: idecSpouseSuppCoverageAmount * -1, adecDepSuppCoverageAmount: idecDepSuppCoverageAmount * -1, acolPosNegEmployerPayrollDtl: icolPosNegEmployerPayrollDtl);
                        }
                        //Collection<busEmployerPayrollDetail> lcolFinal = ComparePositiveNegativeColEmployerPayrollDetail();
                        //if (lbusPayrollHdr != null && (lcolFinal != null) && (lcolFinal.Count > 0))
                        //{
                        //    lbusPayrollHdr.UpdateDataObject(lbusPayrollHdr.icdoEmployerPayrollHeader);
                        //    foreach (busEmployerPayrollDetail lbusPayrollDtl in lcolFinal)
                        //    {
                        //        lbusPayrollDtl.icdoEmployerPayrollDetail.employer_payroll_header_id = lbusPayrollHdr.icdoEmployerPayrollHeader.employer_payroll_header_id;
                        //        lbusPayrollDtl.UpdateDataObject(lbusPayrollDtl.icdoEmployerPayrollDetail);
                        //    }
                        //}
                    }

                    /*****************************Member (Employment) Pay ***************************************/
                    if ((ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag != busConstant.Flag_Yes) &&
                        (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id == 0) &&
                        (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_supplemental_org_id == 0))
                    {
                        //Reload the Employment based on the Effective Date. (There will be a scenario employment will be null on load but adjustment will have the employment)
                        if (icdoPersonAccount.person_employment_dtl_id == 0) // PIR 10816
                            icdoPersonAccount.person_employment_dtl_id = GetEmploymentDetailID(ldatEffectedDate, true);
                        lintOrgID = 0;
                        if (icdoPersonAccount.person_employment_dtl_id > 0)
                        {
                            LoadPersonEmploymentDetail();
                            ibusPersonEmploymentDetail.LoadPersonEmployment();
                            lintOrgID = ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id;
                        }

                        if (lintOrgID > 0)
                        {
                            lbusPayrollHdr = GetCurrentPayrollAdjustmentHeader(lintOrgID);

                            ldecOldPremium = 0.00M;
                            ldecOldBasicPremium = 0.00M;
                            ldecOldSuppPremium = 0.00M;
                            ldecOldSpouseSuppPremium = 0.00M;
                            ldecOldDepSuppPremium = 0.00M;
                            GetPremiumSplitForHistoryAdd(ref ldecOldPremium,
                                                         ref ldecOldBasicPremium, ref ldecOldSuppPremium,
                                                         ref ldecOldSpouseSuppPremium, ref ldecOldDepSuppPremium, false, false, false, true);
                            if (icolPosNegEmployerPayrollDtl.IsNull())
                                icolPosNegEmployerPayrollDtl = new Collection<busEmployerPayrollDetail>();
                            if (ldecOldPremium > 0 && IsPayrollDtlAdjustmentExistsPriorToEnteredEffectiveDate(ibusProviderOrgPlan.icdoOrgPlan.org_id, ldatEffectedDate, ldecOldPremium, busConstant.PayrollDetailRecordTypePositiveAdjustment))
                            {
                                lbusPayrollHdr.UpdateEmployerPayrollDetailStatusReviewAndValidToIgnore(icdoPersonAccount.person_id, icdoPersonAccount.plan_id, ldatEffectedDate);
                                lbusPayrollHdr.AddInsuranceAdjustmentPayrollDetail(icdoPersonAccount.person_id, icdoPersonAccount.plan_id, ibusPerson.icdoPerson.first_name,
                                ibusPerson.icdoPerson.last_name, ibusPerson.icdoPerson.ssn, busConstant.PayrollDetailRecordTypePositiveAdjustment,
                                ldatEffectedDate, ldecOldPremium, ibusProviderOrgPlan.icdoOrgPlan.org_id,
                                adecBasicPremium: ldecOldBasicPremium, adecSuppPremium: ldecOldSuppPremium,
                                adecSpouseSuppPremium: ldecOldSpouseSuppPremium, adecDepSuppPremium: ldecOldDepSuppPremium, adecADAndDBasiceRate: idecADAndDBasicRate,
                            adecADAndDSuppRate: idecADAndDSupplementalRate, adecBasicCoverageAmount: idecBasicCoverageAmount, adecSuppCoverageAmount: idecSuppCoverageAmount,
                            adecSpouSuppCoverageAmount: idecSpouseSuppCoverageAmount, adecDepSuppCoverageAmount: idecDepSuppCoverageAmount, acolPosNegEmployerPayrollDtl: icolPosNegEmployerPayrollDtl);
                            }
                            else if (ldecOldPremium < 0 && IsPayrollDtlAdjustmentExistsPriorToEnteredEffectiveDate(ibusProviderOrgPlan.icdoOrgPlan.org_id, ldatEffectedDate, ldecOldPremium * -1, busConstant.PayrollDetailRecordTypeNegativeAdjustment))
                            {
                                lbusPayrollHdr.AddInsuranceAdjustmentPayrollDetail(icdoPersonAccount.person_id, icdoPersonAccount.plan_id, ibusPerson.icdoPerson.first_name,
                                ibusPerson.icdoPerson.last_name, ibusPerson.icdoPerson.ssn, busConstant.PayrollDetailRecordTypeNegativeAdjustment, ldatEffectedDate, ldecOldPremium * -1,
                                ibusProviderOrgPlan.icdoOrgPlan.org_id, adecBasicPremium: ldecOldBasicPremium * -1, adecSuppPremium: ldecOldSuppPremium * -1,
                                adecSpouseSuppPremium: ldecOldSpouseSuppPremium * -1, adecDepSuppPremium: ldecOldDepSuppPremium * -1, adecADAndDBasiceRate: idecADAndDBasicRate * -1,
                            adecADAndDSuppRate: idecADAndDSupplementalRate * -1, adecBasicCoverageAmount: idecBasicCoverageAmount * -1, adecSuppCoverageAmount: idecSuppCoverageAmount * -1,
                            adecSpouSuppCoverageAmount: idecSpouseSuppCoverageAmount * -1, adecDepSuppCoverageAmount: idecDepSuppCoverageAmount * -1, acolPosNegEmployerPayrollDtl: icolPosNegEmployerPayrollDtl);
                            }                            
                        }
                    }
                    ldatEffectedDate = ldatEffectedDate.AddMonths(1);
                }

                Collection<busEmployerPayrollDetail> lcolFinal = ComparePositiveNegativeColEmployerPayrollDetail();
                if (lbusPayrollHdr != null && (lcolFinal != null) && (lcolFinal.Count > 0))
                {
                    if(lbusPayrollHdr != null)
                        lbusPayrollHdr.UpdateDataObject(lbusPayrollHdr.icdoEmployerPayrollHeader);
                    foreach (busEmployerPayrollDetail lbusPayrollDtl in lcolFinal)
                    {
                        if (lbusPayrollHdr != null && lbusPayrollDtl.icdoEmployerPayrollDetail.employer_payroll_header_id == 0)
                            lbusPayrollDtl.icdoEmployerPayrollDetail.employer_payroll_header_id = lbusPayrollHdr.icdoEmployerPayrollHeader.employer_payroll_header_id;
                        lbusPayrollDtl.UpdateDataObject(lbusPayrollDtl.icdoEmployerPayrollDetail);
                    }
                }
            }
            //PROD PIR 5538 : need to calculate premium as of idtPlanEffectiveDate
            RecalculatePremiumBasedOnPlanEffectiveDate();            
        }

        private void GetPremiumSplitForHistoryAdd(ref decimal adecTotalPremium, ref decimal adecBasicPremium, ref decimal adecSuppPremium,
                                                  ref decimal adecSpouseSuppPremium, ref decimal adecDepSuppPremium,
                                                  bool ablnOnlyBasic, bool ablnOnlySupp, bool ablnOnlySpouseAndDepSupp, bool ablnAll)
        {
            if (ablnAll || ablnOnlyBasic)
            {
                adecBasicPremium = idecLifeBasicPremiumAmt;
                adecTotalPremium += adecBasicPremium;
            }

            if (ablnAll || ablnOnlySupp)
            {
                adecSuppPremium = idecLifeSupplementalPremiumAmount;
                adecTotalPremium += adecSuppPremium;
            }

            if (ablnAll || ablnOnlySpouseAndDepSupp)
            {
                adecSpouseSuppPremium = idecSpouseSupplementalPremiumAmt;
                adecTotalPremium += adecSpouseSuppPremium;
            }

            if (ablnAll || ablnOnlySpouseAndDepSupp)
            {
                adecDepSuppPremium = idecDependentSupplementalPremiumAmt;
                adecTotalPremium += adecDepSuppPremium;
            }
        }

        private busEmployerPayrollHeader GetCurrentPayrollAdjustmentHeader(int aintOrgID)
        {
            busEmployerPayrollHeader lbusPayrollHdr = new busEmployerPayrollHeader();
            if (!lbusPayrollHdr.LoadCurrentAdjustmentPayrollHeader(aintOrgID, busConstant.PayrollHeaderBenefitTypeInsr))
            {
                lbusPayrollHdr.CreateInsuranceAdjustmentPayrollHeader(aintOrgID);
                lbusPayrollHdr.iclbEmployerPayrollDetail = new Collection<busEmployerPayrollDetail>();
            }
            else
            {
                lbusPayrollHdr.icdoEmployerPayrollHeader.ienuObjectState = ObjectState.Update;
                lbusPayrollHdr.LoadEmployerPayrollDetail();
            }
            return lbusPayrollHdr;
        }

        public bool IsLifeOptionExists(int AintAccountLifeID)
        {
            bool lblnResult = false;
            cdoPersonAccountLifeOption icdoLifeOption = new cdoPersonAccountLifeOption();
            if (icdoLifeOption.SelectRow(new object[1] { AintAccountLifeID }))
                lblnResult = true;
            return lblnResult;
        }

        public bool IsMemberHasSpouse()
        {
            bool lblnflag = false;
            if (ibusPerson == null)
                LoadPerson();
            if (ibusPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusMarried)
            {
                if (ibusPerson.icolPersonContact == null)
                    ibusPerson.LoadContacts();
                foreach (busPersonContact lobjContact in ibusPerson.icolPersonContact)
                {
                    if ((lobjContact.icdoPersonContact.relationship_value == busConstant.PersonTypeSpouse) &&
                        (lobjContact.icdoPersonContact.status_value.Trim() == busConstant.PersonContactStatusActive.Trim()))
                    {
                        lblnflag = true;
                        break;
                    }
                }
            }
            return lblnflag;
        }

        private int _AintGroupLifeOptionValidateFlag;
        public int AintGroupLifeOptionValidateFlag
        {
            get { return _AintGroupLifeOptionValidateFlag; }
            set { _AintGroupLifeOptionValidateFlag = value; }
        }

        /// <summary>
        /// Validates the Person Account Life Option
        /// </summary>
        /// <returns>Returns the corresponding error no.</returns>
        public int ValidateGroupLifeOption()
        {
            bool lblnBasicEntered = false;
            bool lblnSupplementalEntered = false;
            bool lblnDependentSupplementalEntered = false;
            bool lblnBasicEnrolled = false;
            bool lblnSupplementalEnrolled = false;
            bool lblnDependentSupplementalEnrolled = false;

            decimal ldclBasicCoverageAmount = 0.0M;
            decimal ldclSupplementalCoverageAmount = 0.0M;

            DateTime ldteBasicStartDate = DateTime.MinValue;
            DateTime ldteSupplementalStartDate = DateTime.MinValue;
            DateTime ldteDependentStartDate = DateTime.MinValue;
            DateTime ldteSpouseStartDate = DateTime.MinValue;

            //prod pir 6947 : added effective date to coverage amoutn retrival
            DateTime ldtEffectiveDate = DateTime.MinValue;
            if (icdoPersonAccount.history_change_date != DateTime.MinValue)
                ldtEffectiveDate = icdoPersonAccount.history_change_date;
            else if (icdoPersonAccount.start_date != DateTime.MinValue && icdoPersonAccount.ienuObjectState == ObjectState.Insert)
                ldtEffectiveDate = icdoPersonAccount.start_date;
            else
                ldtEffectiveDate = DateTime.Today;

            bool lblnIsContributedDuringLOA = true;
            
            /// Need to check contribution amount only in update mode
            if (icdoPersonAccount.ienuObjectState != ObjectState.Insert)
            {
                //Prod PIR:4430
                //This validation should fire only if any supplemental / spouse supp / Dep Supp in enrolled status
                if (iclbLifeOption.Where(i => i.icdoPersonAccountLifeOption.level_of_coverage_value != busConstant.LevelofCoverage_Basic)
                                 .Any(i => i.icdoPersonAccountLifeOption.lblnValueEntered && i.icdoPersonAccountLifeOption.plan_option_status_value == busConstant.PlanOptionStatusValueEnrolled))
                {
                    lblnIsContributedDuringLOA = IsContributedDuringLOA();
                }
            }            
           
            if ((_iclbLifeOption != null) &&
                (icdoPersonAccountLife.life_insurance_type_value != null))
            { 
                foreach (busPersonAccountLifeOption lobjLifeOption in _iclbLifeOption)
                { 
                    if (lobjLifeOption.icdoPersonAccountLifeOption.plan_option_status_value == busConstant.PlanOptionStatusValueWaived)
                    {
                        if (lobjLifeOption.icdoPersonAccountLifeOption.coverage_amount != 0.0M)
                        {
                            return 33;
                        }
                    }
                    //PIR-7987 + Start
                    //if (lobjLifeOption.icdoPersonAccountLifeOption.plan_option_status_value == busConstant.PlanOptionStatusValueEnrolled)
                    //{
                    //    if (lobjLifeOption.icdoPersonAccountLifeOption.coverage_amount != 0 && lobjLifeOption.icdoPersonAccountLifeOption.effective_start_date == DateTime.MinValue)
                    //    {
                    //        return 35;
                    //    }
                    //    else if (lobjLifeOption.icdoPersonAccountLifeOption.coverage_amount == 0)
                    //    {
                    //        return 36;
                    //    }
                    //}
                    //PIR-7987 + End                    
                    
                    if (icdoPersonAccount.history_change_date != DateTime.MinValue && lobjLifeOption.icdoPersonAccountLifeOption.effective_start_date != DateTime.MinValue)
                    {
                        // The Validation is applicable for newly entered life option
                        if (lobjLifeOption.icdoPersonAccountLifeOption.ihstOldValues.Count == 0 && 
                            lobjLifeOption.icdoPersonAccountLifeOption.effective_start_date < icdoPersonAccount.history_change_date)
                        {
                            return 34; // PROD PIR ID 6384
                        }
                    }

                    if (lobjLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_Basic)
                    {
                        if (lobjLifeOption.icdoPersonAccountLifeOption.lblnValueEntered)
                        {
                            if (lobjLifeOption.icdoPersonAccountLifeOption.coverage_amount != 0.0M)
                            {
                                if (!IsValidCoverageAmount(busConstant.LevelofCoverage_Basic,
                                    icdoPersonAccountLife.life_insurance_type_value,
                                    lobjLifeOption.icdoPersonAccountLifeOption.coverage_amount, ldtEffectiveDate))
                                {
                                    return 7;   // Basic Coverage amount is Invalid.
                                }
                                ldclBasicCoverageAmount = lobjLifeOption.icdoPersonAccountLifeOption.coverage_amount;
                            }
                            else
                            {
                                if (lobjLifeOption.icdoPersonAccountLifeOption.plan_option_status_value != busConstant.PlanOptionStatusValueWaived)
                                    return 12;    // Basic Coverage amount should not be $0.00
                            }

                            if (lobjLifeOption.icdoPersonAccountLifeOption.plan_option_status_value != null)
                            {
                                if (lobjLifeOption.icdoPersonAccountLifeOption.plan_option_status_value == busConstant.PlanOptionStatusValueEnrolled)
                                    lblnBasicEnrolled = true;
                            }
                            else
                            {
                                return 13;  // Basic Plan option status cannot be null.
                            }


                            if (lobjLifeOption.icdoPersonAccountLifeOption.effective_start_date == DateTime.MinValue)
                            {
                                return 11;  // Basic Start date cannot be null.
                            }
                            else
                            {
                                ldteBasicStartDate = lobjLifeOption.icdoPersonAccountLifeOption.effective_start_date;
                            }
                            //PIR-7987 + Start
                            if (lobjLifeOption.icdoPersonAccountLifeOption.plan_option_status_value == busConstant.PlanOptionStatusValueEnrolled)
                            {
                                if (lobjLifeOption.icdoPersonAccountLifeOption.coverage_amount != 0 && lobjLifeOption.icdoPersonAccountLifeOption.effective_start_date == DateTime.MinValue)
                                {
                                    return 35;
                                }
                                else if (lobjLifeOption.icdoPersonAccountLifeOption.coverage_amount == 0)
                                {
                                    return 36;
                                }
                            }
                            //PIR-7987 + End             
                            lblnBasicEntered = true;

                        }
                        else
                        {
                            return 1;   // Level of coverage Basic is mandatory.
                        }
                    }

                    if ((lobjLifeOption.icdoPersonAccountLifeOption.lblnValueEntered) &&
                        (lobjLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_Supplemental))
                    {
                        if (!lblnBasicEntered)
                        {
                            return 14;  // Supplemental cannot be entered without entering Basic.
                        }

                        if (!lblnIsContributedDuringLOA)
                        {
                            return 26;
                        }

                        if (lobjLifeOption.icdoPersonAccountLifeOption.coverage_amount != 0.0M)
                        {
                            if (!IsValidCoverageAmount(busConstant.LevelofCoverage_Supplemental,
                                icdoPersonAccountLife.life_insurance_type_value,
                                lobjLifeOption.icdoPersonAccountLifeOption.coverage_amount, ldtEffectiveDate))
                            {
                                return 8;   // Supplemental coverage amount not valid.
                            }
                            ldclSupplementalCoverageAmount = lobjLifeOption.icdoPersonAccountLifeOption.coverage_amount;
                        }

                        if (lobjLifeOption.icdoPersonAccountLifeOption.plan_option_status_value != null)
                        {
                            if (lobjLifeOption.icdoPersonAccountLifeOption.plan_option_status_value == busConstant.PlanOptionStatusValueEnrolled)
                            {
                                if (!lblnBasicEnrolled)
                                {
                                    return 4; // Supplemental can enrolled iff the Basic is Enrolled.
                                }
                                //PIR-7987 + Start
                                if (lobjLifeOption.icdoPersonAccountLifeOption.plan_option_status_value == busConstant.PlanOptionStatusValueEnrolled)
                                {
                                    if (lobjLifeOption.icdoPersonAccountLifeOption.coverage_amount != 0 && lobjLifeOption.icdoPersonAccountLifeOption.effective_start_date == DateTime.MinValue)
                                    {
                                        return 35;
                                    }
                                    else if (lobjLifeOption.icdoPersonAccountLifeOption.coverage_amount == 0)
                                    {
                                        return 36;
                                    }
                                }
                                //PIR-7987 + End             
                                lblnSupplementalEnrolled = true;
                            }
                        }
                        else
                        {
                            return 16;  // Supplemental Plan option status is mandatory.
                        }

                        if (lobjLifeOption.icdoPersonAccountLifeOption.plan_option_status_value == busConstant.PlanOptionStatusValueEnrolled &&  //Added Extra Condition PIR-7987 +
                            lobjLifeOption.icdoPersonAccountLifeOption.effective_start_date == DateTime.MinValue)
                        {
                            return 17;  // Supplemental Start date is mandatory.
                        }
                        else
                        {
                            ldteSupplementalStartDate = lobjLifeOption.icdoPersonAccountLifeOption.effective_start_date;
                        }

                        if ((lobjLifeOption.icdoPersonAccountLifeOption.coverage_amount == 0.0M) &&
                            (lobjLifeOption.icdoPersonAccountLifeOption.plan_option_status_value != busConstant.PlanOptionStatusValueWaived))
                        {
                            return 15;  // Supplemental coverage amount cannot be $0.00
                        }

                        lblnSupplementalEntered = true;

                        if ((lobjLifeOption.icdoPersonAccountLifeOption.ihstOldValues.Count > 0) &&
                            (Convert.ToInt32(lobjLifeOption.icdoPersonAccountLifeOption.ihstOldValues["coverage_amount"]) !=
                            lobjLifeOption.icdoPersonAccountLifeOption.coverage_amount))
                            iblnIsSupplementalCoverageModifed = true;
                    }

                    if ((lobjLifeOption.icdoPersonAccountLifeOption.lblnValueEntered) &&
                        (lobjLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_DependentSupplemental))
                    {
                        if (!lblnSupplementalEntered)
                        {
                            return 2;   // Dependent Supplemental cannot be entered without entering Supplemental.
                        }

                        if (!lblnIsContributedDuringLOA)
                        {
                            return 26;
                        }

                        if (lobjLifeOption.icdoPersonAccountLifeOption.coverage_amount != 0.0M)
                        {
                            if (!IsValidCoverageAmount(busConstant.LevelofCoverage_DependentSupplemental,
                                icdoPersonAccountLife.life_insurance_type_value,
                                lobjLifeOption.icdoPersonAccountLifeOption.coverage_amount, ldtEffectiveDate))
                            {
                                return 9;   // Dependental Supplemental coverage amount is not valid.
                            }
                        }

                        if (lobjLifeOption.icdoPersonAccountLifeOption.plan_option_status_value != null)
                        {
                            if (lobjLifeOption.icdoPersonAccountLifeOption.plan_option_status_value == busConstant.PlanOptionStatusValueEnrolled)
                            {
                                lblnDependentSupplementalEnrolled = true;
                                if (!lblnSupplementalEnrolled)
                                {
                                    return 5;   // Dependent Supplemental can be Enrolled iff the Supplemental is Enrolled.
                                }
                            }
                        }
                        else
                        {
                            return 19;  // Dependent Supplemental Plan option status is mandatory.
                        }

                        if (lobjLifeOption.icdoPersonAccountLifeOption.plan_option_status_value == busConstant.PlanOptionStatusValueEnrolled && //Added Extra Condition PIR-7987 +
                            lobjLifeOption.icdoPersonAccountLifeOption.effective_start_date == DateTime.MinValue)
                        {
                            return 20;  // Dependent Supplemental Effective Start date is mandatory.
                        }
                        else
                        {
                            ldteDependentStartDate = lobjLifeOption.icdoPersonAccountLifeOption.effective_start_date;
                        }

                        if ((lobjLifeOption.icdoPersonAccountLifeOption.coverage_amount == 0.0M) &&
                            (lobjLifeOption.icdoPersonAccountLifeOption.plan_option_status_value != busConstant.PlanOptionStatusValueWaived)) 
                        {
                            return 18;  // Dependent Supplemental coverage amount cannot be $0.00
                        }
                        //PIR-7987 + Start
                        if (lobjLifeOption.icdoPersonAccountLifeOption.plan_option_status_value == busConstant.PlanOptionStatusValueEnrolled)
                        {
                            if (lobjLifeOption.icdoPersonAccountLifeOption.coverage_amount != 0 && lobjLifeOption.icdoPersonAccountLifeOption.effective_start_date == DateTime.MinValue)
                            {
                                return 35;
                            }
                            else if (lobjLifeOption.icdoPersonAccountLifeOption.coverage_amount == 0)
                            {
                                return 36;
                            }
                        }
                        //PIR-7987 + End             

                        lblnDependentSupplementalEntered = true;
                    }

                    if ((lobjLifeOption.icdoPersonAccountLifeOption.lblnValueEntered) &&
                        (lobjLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_SpouseSupplemental))
                    {
                        if (!lblnDependentSupplementalEntered)
                        {
                            return 3;   // Spouse Supplemental cannot be entered without entering Dependent Supplemental.
                        }

                        if (!lblnIsContributedDuringLOA)
                        {
                            return 26;
                        }

                        if (lobjLifeOption.icdoPersonAccountLifeOption.plan_option_status_value != null)
                        {
                            if (lobjLifeOption.icdoPersonAccountLifeOption.plan_option_status_value == busConstant.PlanOptionStatusValueEnrolled)
                            {
                                if (!lblnDependentSupplementalEnrolled)
                                {
                                    return 6;   // Spouse Supplemental cannot be Enrolled without Enrolling Dependent Supplemental.
                                }
                            }
                        }
                        else
                        {
                            return 22;  // Spouse Supplemental Plan option status is mandatory.
                        }

                        if (lobjLifeOption.icdoPersonAccountLifeOption.plan_option_status_value == busConstant.PlanOptionStatusValueEnrolled && //Added Extra Condition PIR-7987 +
                            lobjLifeOption.icdoPersonAccountLifeOption.effective_start_date == DateTime.MinValue)
                        {
                            return 23;  // Spouse Supplemental Start Date is mandatory.
                        }
                        else
                        {
                            ldteSpouseStartDate = lobjLifeOption.icdoPersonAccountLifeOption.effective_start_date;
                        }

                        if ((lobjLifeOption.icdoPersonAccountLifeOption.coverage_amount == 0.0M) &&
                            (lobjLifeOption.icdoPersonAccountLifeOption.plan_option_status_value != busConstant.PlanOptionStatusValueWaived))
                        {
                            return 21;    // Spouse Supplemental coverage amount cannot be $0.00.
                        }

                        if (lobjLifeOption.icdoPersonAccountLifeOption.coverage_amount != 0.0M)
                        {
                            if (!IsValidCoverageAmount(busConstant.LevelofCoverage_SpouseSupplemental,
                                icdoPersonAccountLife.life_insurance_type_value,
                                lobjLifeOption.icdoPersonAccountLifeOption.coverage_amount, ldtEffectiveDate))
                            {
                                return 10;  // Spouse Supplemental coverage amount is Invalid.
                            }
                            //PROD PIR : 4040 Only if not end dated, this validation should fire.
                            if ((icdoPersonAccountLife.life_insurance_type_value != busConstant.LifeInsuranceTypeRetireeMember)
                                && (lobjLifeOption.icdoPersonAccountLifeOption.coverage_amount > ((ldclSupplementalCoverageAmount + ldclBasicCoverageAmount) / 2)))
                            {
                                return 24;  // Spouse Supplemental cannot exceed 50% of Supplemental coverage amount.
                            }
                        }
                        //PIR-7987 + Start
                        if (lobjLifeOption.icdoPersonAccountLifeOption.plan_option_status_value == busConstant.PlanOptionStatusValueEnrolled)
                        {
                            if (lobjLifeOption.icdoPersonAccountLifeOption.coverage_amount != 0 && lobjLifeOption.icdoPersonAccountLifeOption.effective_start_date == DateTime.MinValue)
                            {
                                return 35;
                            }
                            else if (lobjLifeOption.icdoPersonAccountLifeOption.coverage_amount == 0)
                            {
                                return 36;
                            }
                        }
                        //PIR-7987 + End             
                    }
                }

                /// Start Date Validations
                if (ldteSupplementalStartDate != DateTime.MinValue)
                {
                    if (ldteSupplementalStartDate < ldteBasicStartDate)
                        return 30;
                }
                if (ldteDependentStartDate != DateTime.MinValue) //PIR 2057
                {
                    if (ldteDependentStartDate < ldteSupplementalStartDate)
                        return 31;
                }
                if (ldteSpouseStartDate != DateTime.MinValue) //PIR 2057
                {
                    if (ldteSpouseStartDate < ldteDependentStartDate)
                        return 32;
                }
            }
            return 0;
        }

        public bool IsSuplementalSpouseCoverageAmountGreaterThan50000()
        {
            foreach (busPersonAccountLifeOption lobjOption in iclbLifeOption)
            {
                if (lobjOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_SpouseSupplemental)
                {
                    if (lobjOption.icdoPersonAccountLifeOption.coverage_amount > 50000)
                        return true;
                }
            }
            return false;
        }

        public bool IsContributedDuringLOA()
        {
            if (ibusPersonEmploymentDetail != null && icdoPersonAccount.history_change_date != DateTime.MinValue)
            {
                // Get latest LOA Person Employment Detail
                DataTable ldtbLists = Select("cdoPersonEmploymentDetail.GetLOAEmploymentDetailID",
                            new object[2] { ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_id , icdoPersonAccount.history_change_date });//PIR 25319
                if (ldtbLists.Rows.Count > 0)
                {
                    DateTime ldteGoLiveDate = Convert.ToDateTime(busGlobalFunctions.GetData1ByCodeValue(52, "PGLD", iobjPassInfo)); // UAT PIR ID 994
                    busPersonEmploymentDetail lobjEmploymentDetail = new busPersonEmploymentDetail { icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail() };
                    lobjEmploymentDetail.icdoPersonEmploymentDetail.LoadData(ldtbLists.Rows[0]);
                    // Check Insurance contributions over the given period.
                    /***************************************************************/
                    //Prod PIR: 4430
                    //The Start Date should be the First of the Month
                    //The End Date should be the last Day of the Month.
                    /***************************************************************/
                    DateTime ldtStartDate = lobjEmploymentDetail.icdoPersonEmploymentDetail.start_date.GetFirstDayofCurrentMonth();
                    DateTime ldtEndDate = lobjEmploymentDetail.icdoPersonEmploymentDetail.end_date_no_null.GetLastDayofMonth();
                    /***************************************************************/
                    DataTable ldtbContributions = Select("cdoPersonAccountInsuranceContribution.CheckTransactionByPersonAccountID",
                                new object[3]{icdoPersonAccount.person_account_id,
                                                            ldtStartDate,
                                                            ldtEndDate});
                    if ((ldtbContributions.Rows.Count == 0) && (lobjEmploymentDetail.icdoPersonEmploymentDetail.start_date >= ldteGoLiveDate))
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Method to check whether the Level of coverage changed during Transfer.
        /// </summary>
        /// <returns>True - If Level of coverage changed in transfer</returns>
        public bool IsLifeOptionChangedinTransfer()
        {
            bool lblnFlag = false;
            if (!icdoPersonAccount.is_from_mss) // PIR 9702
            {
                if (ibusPerson == null)
                    LoadPerson();
                if (ibusPlan == null)
                    LoadPlan();

                //Load the Previous Employment
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
                        if (ibusPreviousEmploymentDetailForTransfer.ibusPersonEmployment.icdoPersonEmployment.end_date != DateTime.MinValue)
                        {
                            if (lobjCurrentEmpDtl.ibusPersonEmployment.icdoPersonEmployment.start_date.AddDays(-31) <
                                ibusPreviousEmploymentDetailForTransfer.ibusPersonEmployment.icdoPersonEmployment.end_date)
                            {
                                if (iclbPersonAccountLifeHistory == null)
                                    LoadHistory();

                                if (IsLevelOfCoverageChanged())
                                    lblnFlag = true;
                            }
                        }
                    }
                }
            }
            return lblnFlag;
        }

        private bool IsLevelOfCoverageChanged()
        {
            bool lblnResult = false;
            if (ibusPreviousEmploymentDetail == null)
                LoadPreviousEmploymentDetail();
            //Only If Employment Exists, do the Validation
            if (ibusPreviousEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id > 0)
            {
                foreach (busPersonAccountLifeOption lobjOption in _iclbLifeOption)
                {
                    //UAT PIR : 589
                    if (lobjOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_Supplemental)
                    {
                        busPersonAccountLifeHistory lobjPALifeHistory = LoadHistoryByDate(lobjOption, ibusPreviousEmploymentDetail.icdoPersonEmploymentDetail.end_date);
                        if (lobjPALifeHistory.icdoPersonAccountLifeHistory.person_account_life_history_id > 0)
                        {
                            if ((lobjPALifeHistory.icdoPersonAccountLifeHistory.coverage_amount != lobjOption.icdoPersonAccountLifeOption.coverage_amount) ||
                                (lobjPALifeHistory.icdoPersonAccountLifeHistory.plan_option_status_value != lobjOption.icdoPersonAccountLifeOption.plan_option_status_value))
                            {
                                lblnResult = true;
                                break;
                            }
                        }
                        else //check if new coverage entered
                        {
                            if ((lobjOption.icdoPersonAccountLifeOption.coverage_amount > 0)
                                && (lobjOption.icdoPersonAccountLifeOption.effective_start_date != DateTime.MinValue))
                            {
                                lblnResult = true;
                                break;
                            }
                        }
                    }
                }
            }
            return lblnResult;
        }

        /// <summary>
        /// ** BR - 223 ** The system must not allow continuation of employee supplemental, dependent supplemental, and spouse supplemental Life Insurance 
        ///  if the Member’ age is equal or greater than attained age 65 during month of enrollment.
        /// </summary>
        /// <returns></returns>
        public int IsRetireeAttainedAge65()
        {
            if (icdoPersonAccountLife.life_insurance_type_value == busConstant.LifeInsuranceTypeRetireeMember)
            {
                DateTime ldtLastDay = new DateTime(icdoPersonAccount.current_plan_start_date.Year, icdoPersonAccount.current_plan_start_date.Month,
                    DateTime.DaysInMonth(icdoPersonAccount.current_plan_start_date.Year, icdoPersonAccount.current_plan_start_date.Month));
                if (busGlobalFunctions.CalulateAge(ibusPerson.icdoPerson.date_of_birth, ldtLastDay) >= 65)
                {
                    // Age >=65 and Actively covered in Level of coverages other than Basic. 
                    // UAT PIR ID: 2398
                    if (_iclbLifeOption.Where(lobj => lobj.icdoPersonAccountLifeOption.effective_start_date != DateTime.MinValue &&
                                                      lobj.icdoPersonAccountLifeOption.level_of_coverage_value != busConstant.LevelofCoverage_Basic &&
                                                      lobj.icdoPersonAccountLifeOption.coverage_amount != 0).Any())
                        return 1;
                }
                else
                {
                    // Age < 65 and Level of coverage changed in Retirement.
                    if (IsLevelOfCoverageChangedForRetiree())
                        return 2;
                }
            }
            return 0;
        }

        public int IsRetireeAttainedAge65(DateTime adtRetirementDate)
        {
            DateTime idtDateAfter65 = ibusPerson.icdoPerson.date_of_birth.AddYears(65);
            if (adtRetirementDate.Year > idtDateAfter65.Year)
                return 1;
            if (adtRetirementDate.Year == idtDateAfter65.Year)
            {
                if (adtRetirementDate.Month >= idtDateAfter65.Month)
                {
                    return 1;
                }
            }
            return 0;
        }
        /// <summary>
        /// PIR 25806
        /// </summary>
        /// <returns></returns>
        private bool IsLevelOfCoverageChangedForRetiree()
        {
            //1. If only Basic has Plan Option Status Value = ENLD and all other options are WAVD – skip this validation; else validate below
            if (iclbLifeOption.Any(lobjOption => lobjOption.icdoPersonAccountLifeOption.level_of_coverage_value != busConstant.LevelofCoverage_Basic && 
                lobjOption.icdoPersonAccountLifeOption.plan_option_status_value == busConstant.PlanOptionStatusValueEnrolled))
            {
                //2.	Option SPML – Compare Options Coverage Amount (sum Basic Coverage Amt + SPML Coverage Amt) to prior History (Change Effective Date between History Start & End Date) Coverage Amount 
                //     (sum Basic Coverage Amt + SPML Coverage Amt) – If Options Amt (sum Basic Coverage Amt + SPML Coverage Amt) is > History Amt – throw error; if not, check DSPL
                if (iclbLifeOption.Any(lobjOption => lobjOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_Supplemental  &&
                lobjOption.icdoPersonAccountLifeOption.plan_option_status_value == busConstant.PlanOptionStatusValueEnrolled))
                {
                    decimal ldecCurrentSumBasicCovAmtAndSuplCovAmt = iclbLifeOption.Where(lobjOption => lobjOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_Basic ||
                                                                                                lobjOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_Supplemental)
                                                                            .Sum(lobjOption => lobjOption.icdoPersonAccountLifeOption.coverage_amount);
                    busPersonAccountLifeHistory lbusPrevBasicPersonAccountLifeHistory = iclbPreviousHistory.FirstOrDefault(lobjHistory => 
                                                                                                                                        lobjHistory.icdoPersonAccountLifeHistory.level_of_coverage_value == busConstant.LevelofCoverage_Basic &&
                                                                                                                                        busGlobalFunctions.CheckDateOverlapping(icdoPersonAccount.history_change_date,
                                                                                                                                          lobjHistory.icdoPersonAccountLifeHistory.start_date, lobjHistory.icdoPersonAccountLifeHistory.end_date));
                    busPersonAccountLifeHistory lbusPrevSuplPersonAccountLifeHistory = iclbPreviousHistory.FirstOrDefault(lobjHistory => lobjHistory.icdoPersonAccountLifeHistory.level_of_coverage_value == busConstant.LevelofCoverage_Supplemental &&
                                                                                                                                        busGlobalFunctions.CheckDateOverlapping(icdoPersonAccount.history_change_date,
                                                                                                                                          lobjHistory.icdoPersonAccountLifeHistory.start_date, lobjHistory.icdoPersonAccountLifeHistory.end_date));
                    if (lbusPrevBasicPersonAccountLifeHistory.IsNotNull() && lbusPrevSuplPersonAccountLifeHistory.IsNotNull())
                    {
                        decimal ldecPreviousSumBasicAmtAndSuplCovAmt = lbusPrevBasicPersonAccountLifeHistory.icdoPersonAccountLifeHistory.coverage_amount + lbusPrevSuplPersonAccountLifeHistory.icdoPersonAccountLifeHistory.coverage_amount;
                        if (ldecCurrentSumBasicCovAmtAndSuplCovAmt > ldecPreviousSumBasicAmtAndSuplCovAmt) return true;
                    }
                }
                //3.	Option DSPL if exists in ENLD Plan Option Status Value – If not ENLD; skip remaining validation; If ENLD, check condition a. below
                //      a.Compare Option Coverage Amount to prior History Coverage Amount – If Options Amt for DSPL is > History Amt – throw error; if not, check SPSL
                if (iclbLifeOption.Any(lobjOption => lobjOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_DependentSupplemental &&
                lobjOption.icdoPersonAccountLifeOption.plan_option_status_value == busConstant.PlanOptionStatusValueEnrolled))
                {
                    decimal ldecCurrentDepSuplCovAmt = iclbLifeOption.Where(lobjOption => lobjOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_DependentSupplemental)
                                                                            .Sum(lobjOption => lobjOption.icdoPersonAccountLifeOption.coverage_amount);
                    busPersonAccountLifeHistory lbusPrevDepSuplPersonAccountLifeHistory = iclbPreviousHistory.FirstOrDefault(lobjHistory => lobjHistory.icdoPersonAccountLifeHistory.level_of_coverage_value == busConstant.LevelofCoverage_DependentSupplemental &&
                                                                                                                                        busGlobalFunctions.CheckDateOverlapping(icdoPersonAccount.history_change_date,
                                                                                                                                          lobjHistory.icdoPersonAccountLifeHistory.start_date, lobjHistory.icdoPersonAccountLifeHistory.end_date));
                    if (lbusPrevDepSuplPersonAccountLifeHistory.IsNotNull())
                    {
                        decimal ldecPreviousDepSuplCovAmt = lbusPrevDepSuplPersonAccountLifeHistory.icdoPersonAccountLifeHistory.coverage_amount;
                        if (ldecCurrentDepSuplCovAmt > ldecPreviousDepSuplCovAmt) return true;
                    }
                }
                //4.Option SPSL if exists in ENLD Plan Option Status Value – If not ENLD; skip remaining validation; If ENLD, check condition a. below
                //a.Compare Option Coverage Amount to prior History Coverage Amount – If Options Amt for SPSL is > History Amt – throw error; if not, no validation
                if (iclbLifeOption.Any(lobjOption => lobjOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_SpouseSupplemental &&
                lobjOption.icdoPersonAccountLifeOption.plan_option_status_value == busConstant.PlanOptionStatusValueEnrolled))
                {
                    decimal ldecCurrentSpouseSuplCovAmt = iclbLifeOption.Where(lobjOption => lobjOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_SpouseSupplemental)
                                                    .Sum(lobjOption => lobjOption.icdoPersonAccountLifeOption.coverage_amount);
                    busPersonAccountLifeHistory lbusPrevSpouseSuplPersonAccountLifeHistory = iclbPreviousHistory.FirstOrDefault(lobjHistory => lobjHistory.icdoPersonAccountLifeHistory.level_of_coverage_value == busConstant.LevelofCoverage_SpouseSupplemental &&
                                                                                                                                    busGlobalFunctions.CheckDateOverlapping(icdoPersonAccount.history_change_date,
                                                                                                                                      lobjHistory.icdoPersonAccountLifeHistory.start_date, lobjHistory.icdoPersonAccountLifeHistory.end_date));
                    if (lbusPrevSpouseSuplPersonAccountLifeHistory.IsNotNull())
                    {
                        decimal ldecPreviousSpouseSuplCovAmt = lbusPrevSpouseSuplPersonAccountLifeHistory.icdoPersonAccountLifeHistory.coverage_amount;
                        if (ldecCurrentSpouseSuplCovAmt > ldecPreviousSpouseSuplCovAmt) return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// ** BR - 224 ** The system must not allow new or continued spouse or dependent coverage 
        /// if a premium waiver has been approved for Group Life Basic and Supplement premiums.  
        /// </summary>
        /// <returns></returns>
        public bool IsActalPremiumWaiverDateValid()
        {
            bool lblnResult = true;
            if (icdoPersonAccountLife.premium_waiver_flag == busConstant.Flag_Yes)
            {
                foreach (busPersonAccountLifeOption lobjOption in _iclbLifeOption)
                {
                    if ((lobjOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_DependentSupplemental) ||
                        (lobjOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_SpouseSupplemental))
                    {
                        if ((lobjOption.icdoPersonAccountLifeOption.effective_start_date != DateTime.MinValue)  && // UAT PIR ID 585
                            (lobjOption.icdoPersonAccountLifeOption.coverage_amount !=0) &&//PIR 7671
                            (lobjOption.icdoPersonAccountLifeOption.plan_option_status_value == busConstant.PlanOptionStatusValueEnrolled) &&//PIR 7671
                            (icdoPersonAccountLife.actual_premium_waiver_date != DateTime.MinValue))
                        {
                            if (icdoPersonAccountLife.actual_premium_waiver_date >= lobjOption.icdoPersonAccountLifeOption.effective_start_date)
                                lblnResult = false;
                        }
                    }
                }
            }
            return lblnResult;
        }

        private string istrOldPlanParticipationStatus = string.Empty;

        /// <summary>
        /// ** BR - 226 ** The system must throw a warning message if a retiree is re-enrolled in basic, supplemental, dependent, or spouse supplemental 
        /// Group Life if any Life coverage was cancelled after retirement.  
        /// </summary>
        /// <returns></returns>
        public bool IsRetireeReEnrollingCoverages()
        {
            if (icdoPersonAccountLife.life_insurance_type_value == busConstant.LifeInsuranceTypeRetireeMember)
            {
                if ((istrOldPlanParticipationStatus == busConstant.PlanParticipationStatusInsuranceSuspended) &&
                    (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled))
                    return true;

                if (ibusPreviousEmploymentDetail == null)
                    LoadPreviousEmploymentDetail();
                foreach (busPersonAccountLifeOption lobjOption in _iclbLifeOption)
                {
                    busPersonAccountLifeHistory lobjPrevHistory = new busPersonAccountLifeHistory { icdoPersonAccountLifeHistory = new cdoPersonAccountLifeHistory() };
                    lobjPrevHistory = GetPreviousHistoryForOption(lobjOption);
                    if ((lobjPrevHistory.icdoPersonAccountLifeHistory.effective_end_date != DateTime.MinValue) &&
                        (lobjOption.icdoPersonAccountLifeOption.coverage_amount > 0) &&
                        (lobjPrevHistory.icdoPersonAccountLifeHistory.effective_end_date >= ibusPreviousEmploymentDetail.icdoPersonEmploymentDetail.end_date))
                        return true;
                }
            }
            return false;
        }

        // UAT PIR ID 992
        // Raise a Soft error if the Level of coverage is increased or Coverage amount increased.
        public bool IsCoverageAmountIncreased()
        {
            foreach (busPersonAccountLifeOption lobjOption in _iclbLifeOption)
            {
                if (lobjOption.icdoPersonAccountLifeOption.level_of_coverage_value != busConstant.LevelofCoverage_Basic)
                {
                    busPersonAccountLifeHistory lobjPrevHistory = new busPersonAccountLifeHistory { icdoPersonAccountLifeHistory = new cdoPersonAccountLifeHistory() };
                    lobjPrevHistory = GetPreviousHistoryForOption(lobjOption);
                    if (lobjPrevHistory.icdoPersonAccountLifeHistory.coverage_amount > 0)
                    {
                        // Coverage Amount increased
                        //prod pir 7388
                        if (lobjOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_Supplemental)
                        {
                            busPersonAccountLifeOption lobjBasic = 
                                iclbLifeOption.Where(o => o.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_Basic).FirstOrDefault();
                            if (lobjBasic != null)
                            {
                                busPersonAccountLifeHistory lobjPABasicHistory = GetPreviousHistoryForOption(lobjBasic);
                                if (lobjPABasicHistory != null)
                                {
                                    if ((lobjOption.icdoPersonAccountLifeOption.coverage_amount + lobjBasic.icdoPersonAccountLifeOption.coverage_amount) >
                                        (lobjPrevHistory.icdoPersonAccountLifeHistory.coverage_amount + lobjPABasicHistory.icdoPersonAccountLifeHistory.coverage_amount))
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (lobjOption.icdoPersonAccountLifeOption.coverage_amount > lobjPrevHistory.icdoPersonAccountLifeHistory.coverage_amount)
                                return true;
                        }
                        // New Coverage added
                        if ((lobjPrevHistory.icdoPersonAccountLifeHistory.person_account_life_history_id == 0) &&
                            (lobjOption.icdoPersonAccountLifeOption.coverage_amount > 0))
                            return true;
                    }
                }
            }
            return false;
        }

        //PIR - 585
        //check if latest closed employment detail.
        public bool IsPremiumWaiverEffectiveDateValid()
        {
            bool lblnFlag = true;
            if ((icdoPersonAccountLife.projected_premium_waiver_date != DateTime.MinValue)
                && (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled))
            {
                busPersonEmploymentDetail lobjPersonEmploymentDetail = GetLatestClosedPersonEmploymentDetail(false);

                if (lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date != DateTime.MinValue)
                {
                    DateTime ldtTempDate = lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date;
                    DateTime ldtEffectiveDate = new DateTime(ldtTempDate.AddMonths(2).Year, ldtTempDate.AddMonths(2).Month, 1);
                    if (ldtEffectiveDate != icdoPersonAccountLife.projected_premium_waiver_date)
                    {
                        lblnFlag = false;
                    }
                }
            }
            return lblnFlag;
        }

        /// <summary>
        /// Check if the employment is end dated for the plan / person account combination.
        /// </summary>
        /// <returns></returns>
        public bool IsEmploymentEndDated()
        {
            bool lblnResult = false;

            if (iclbAccountEmploymentDetail == null)
                LoadPersonAccountEmploymentDetails();
            if (iclbAccountEmploymentDetail.Count == 0) return true;
            foreach (busPersonAccountEmploymentDetail lobjPersonEmploymentDetail in iclbAccountEmploymentDetail)
            {
                if ((lobjPersonEmploymentDetail.icdoPersonAccountEmploymentDetail.plan_id == icdoPersonAccount.plan_id) &&
                    (lobjPersonEmploymentDetail.icdoPersonAccountEmploymentDetail.election_value == busConstant.PersonAccountElectionValueEnrolled))
                {
                    if (lobjPersonEmploymentDetail.ibusEmploymentDetail == null)
                        lobjPersonEmploymentDetail.LoadPersonEmploymentDetail();

                    if (lobjPersonEmploymentDetail.ibusEmploymentDetail.ibusPersonEmployment == null)
                        lobjPersonEmploymentDetail.ibusEmploymentDetail.LoadPersonEmployment();
                    if (lobjPersonEmploymentDetail.ibusEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.end_date != DateTime.MinValue)
                    {
                        lblnResult = true;
                        break;
                    }
                }
            }
            return lblnResult;
        }

        public bool IsSpouseSupplementalEntered()
        {
            bool lblnResult = false;
            if (_iclbLifeOption != null)

            {
                foreach (busPersonAccountLifeOption lobjLifeOption in _iclbLifeOption)
                {
                    if ((lobjLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_SpouseSupplemental) &&
                        (lobjLifeOption.icdoPersonAccountLifeOption.effective_start_date != DateTime.MinValue) &&
                        (lobjLifeOption.icdoPersonAccountLifeOption.coverage_amount != 0.00M))
                    {
                        lblnResult = true;
                    }
                }
            }
            return lblnResult;
        }

        public bool IsSupplementalEntered()
        {
            if (_iclbLifeOption != null)
            {
                foreach (busPersonAccountLifeOption lobjLifeOption in _iclbLifeOption)
                {
                    if ((lobjLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_Supplemental) &&
                        (lobjLifeOption.icdoPersonAccountLifeOption.effective_start_date != DateTime.MinValue) &&
                        (lobjLifeOption.icdoPersonAccountLifeOption.coverage_amount != 0.00M))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsDependentSupplementalEntered()
        {
            if (_iclbLifeOption != null)
            {
                foreach (busPersonAccountLifeOption lobjLifeOption in _iclbLifeOption)
                {
                    if ((lobjLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_DependentSupplemental) &&
                        (lobjLifeOption.icdoPersonAccountLifeOption.effective_start_date != DateTime.MinValue) &&
                        (lobjLifeOption.icdoPersonAccountLifeOption.coverage_amount != 0.00M))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsValidCoverageAmount(string astrLevelOfCoverage, string astrMemberType, decimal adclCoverageAmount, DateTime adtEffectiveDate)
        {
            bool lblnFlag = false;
            if (astrMemberType != null && astrLevelOfCoverage != null && adtEffectiveDate != DateTime.MinValue)
            {
                //prod pir 6947
                DataTable ldtbValidCoverageAmounts = Select("cdoPersonAccountLife.CheckCoverageAmountExists", 
                    new object[3] { astrLevelOfCoverage, astrMemberType, adtEffectiveDate });
                foreach (DataRow drCoverageAmount in ldtbValidCoverageAmounts.Rows)
                {
                    decimal ldclCoverageAmount = Convert.ToDecimal(drCoverageAmount["FULL_COVERAGE_AMT"]);
                    if (ldclCoverageAmount == adclCoverageAmount)
                    {
                        lblnFlag = true;
                        break;
                    }
                }
            }
            return lblnFlag;
        }

        public void InsertHistory(busPersonAccountLifeOption lobjGroupLifeOption, bool ablnIsOptionEnteredFirstTime)
        {
            cdoPersonAccountLifeHistory lobjLifeHistory = new cdoPersonAccountLifeHistory();
            lobjLifeHistory.person_account_id = icdoPersonAccount.person_account_id;
            lobjLifeHistory.start_date = icdoPersonAccount.history_change_date;

            //PIR 17081
            //// PIR 9115
            //if ((icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended) ||
            //    (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceCancelled))
            //{
            //    DBFunction.DBNonQuery("cdoPersonAccountLifeHistory.UpdateReportGeneratedFlag", new object[2] { icdoPersonAccount.person_account_id , 
            //                        lobjGroupLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value },
            //                        iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            //}
            lobjLifeHistory.plan_participation_status_value = icdoPersonAccount.plan_participation_status_value;
            lobjLifeHistory.status_id = icdoPersonAccount.status_id;
            lobjLifeHistory.status_value = icdoPersonAccount.status_value;
            lobjLifeHistory.from_person_account_id = icdoPersonAccount.from_person_account_id;
            lobjLifeHistory.to_person_account_id = icdoPersonAccount.to_person_account_id;
            lobjLifeHistory.suppress_warnings_by = icdoPersonAccount.suppress_warnings_by;
            lobjLifeHistory.suppress_warnings_date = icdoPersonAccount.suppress_warnings_date;
            lobjLifeHistory.suppress_warnings_flag = icdoPersonAccount.suppress_warnings_flag;
            lobjLifeHistory.life_insurance_type_id = icdoPersonAccountLife.life_insurance_type_id;
            lobjLifeHistory.life_insurance_type_value = icdoPersonAccountLife.life_insurance_type_value;
            lobjLifeHistory.premium_waiver_flag = icdoPersonAccountLife.premium_waiver_flag;
            lobjLifeHistory.projected_premium_waiver_date = icdoPersonAccountLife.projected_premium_waiver_date;
            lobjLifeHistory.actual_premium_waiver_date = icdoPersonAccountLife.actual_premium_waiver_date;
            lobjLifeHistory.premium_waiver_provider_org_id = icdoPersonAccountLife.premium_waiver_provider_org_id;
            lobjLifeHistory.waived_amount = icdoPersonAccountLife.waived_amount;
            lobjLifeHistory.spouse_waived_amount = icdoPersonAccountLife.spouse_waived_amount;
            lobjLifeHistory.dependent_waived_amount = icdoPersonAccountLife.dependent_waived_amount;
            lobjLifeHistory.level_of_coverage_id = lobjGroupLifeOption.icdoPersonAccountLifeOption.level_of_coverage_id;
            lobjLifeHistory.level_of_coverage_value = lobjGroupLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value;
            //PIR-7987
            if (lobjGroupLifeOption.icdoPersonAccountLifeOption.coverage_amount == 0.0M ||
                lobjGroupLifeOption.icdoPersonAccountLifeOption.plan_option_status_value == busConstant.PlanOptionStatusValueWaived)
                lobjLifeHistory.ps_file_change_event_value = busConstant.AnnualEnrollmentWaived;
            else
            lobjLifeHistory.ps_file_change_event_value = busConstant.AnnualEnrollment;

            //lobjLifeHistory.people_soft_file_sent_flag = busConstant.Flag_No;//PIR 17081
            if (ablnIsOptionEnteredFirstTime)
            {
                lobjLifeHistory.effective_start_date = lobjGroupLifeOption.icdoPersonAccountLifeOption.effective_start_date;
            }
            else
            {
             
                lobjLifeHistory.effective_start_date = icdoPersonAccount.history_change_date;
                //PIR-8932 //PIR 24076
                //if (lobjGroupLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value != busConstant.LevelofCoverage_Basic && iblnIscalledFromLossOfSuppLifeBatch)
                //    lobjLifeHistory.plan_participation_status_value = busConstant.PlanParticipationStatusInsuranceSuspended;
            }

            //PIR 24076
            if (lobjGroupLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value != busConstant.LevelofCoverage_Basic && iblnIscalledFromLossOfSuppLifeBatch)
            {
                lobjLifeHistory.plan_option_status_value = busConstant.PlanOptionStatusValueWaived;
                lobjLifeHistory.coverage_amount = 0.0m;
            }
            else
            {
                lobjLifeHistory.plan_option_status_value = lobjGroupLifeOption.icdoPersonAccountLifeOption.plan_option_status_value;
                lobjLifeHistory.coverage_amount = lobjGroupLifeOption.icdoPersonAccountLifeOption.coverage_amount;
            }
            lobjLifeHistory.provider_name = icdoPersonAccountLife.Provider_Name;
            lobjLifeHistory.disability_letter_sent_flag = icdoPersonAccountLife.disability_letter_sent_flag;
            lobjLifeHistory.provider_org_id = icdoPersonAccount.provider_org_id;
            lobjLifeHistory.reason_id = icdoPersonAccountLife.reason_id;
            lobjLifeHistory.reason_value = icdoPersonAccountLife.reason_value;
            lobjLifeHistory.premium_conversion_indicator_flag = icdoPersonAccountLife.premium_conversion_indicator_flag;
            lobjLifeHistory.Insert();
        }

        public void ProcessHistory()
        {
            Collection<busPersonAccountLifeHistory> lclbDeletedLifeHistory = new Collection<busPersonAccountLifeHistory>();
            if ((icdoPersonAccount.status_value == busConstant.PersonAccountStatusValid) && (IsHistoryEntryRequired))
            {
                //Remove the Overlapping History
                if (iclbOverlappingHistory != null && iclbOverlappingHistory.Count > 0)
                {
                    foreach (busPersonAccountLifeHistory lbusPALifeHistory in iclbOverlappingHistory)
                    {
                        lclbDeletedLifeHistory.Add(lbusPALifeHistory);
                        lbusPALifeHistory.Delete();
                    }
                    //PIR 23340
                    if (IsResourceForEnhancedOverlap(busConstant.OverlapResourceEnhancedLife))
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

                //If the Current Record is Getting End Dated, We should not create New History Entry. 
                //We Just need to Update the Previous History Entry

                //If the History is already End Dated and the New Record is now removing End Date, Then 
                //We should not update the Previous History End Date. We Just need to Create the New History Record Only.
                if (_iclbPreviousHistory == null)
                    LoadPreviousHistory();
                if (_iclbLifeOptionModified != null)
                {
                    foreach (busPersonAccountLifeOption lobjOption in _iclbLifeOptionModified)
                    {
                        busPersonAccountLifeHistory lobjHistory = GetPreviousHistoryForOption(lobjOption);
                        if (lobjHistory.icdoPersonAccountLifeHistory.person_account_life_history_id > 0)
                        {
                            if (!lclbDeletedLifeHistory.Any(i => i.icdoPersonAccountLifeHistory.person_account_life_history_id == lobjHistory.icdoPersonAccountLifeHistory.person_account_life_history_id))
                            {
                                //PIR 26324
                                //if ((lobjHistory.icdoPersonAccountLifeHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended &&
                                //    lobjOption.icdoPersonAccountLifeOption.coverage_amount > 0M) // PROD PIR ID 6384
                                //|| (lobjHistory.icdoPersonAccountLifeHistory.plan_participation_status_value != busConstant.PlanParticipationStatusInsuranceSuspended))
                                //{
                                    if (lobjHistory.icdoPersonAccountLifeHistory.effective_start_date == icdoPersonAccount.history_change_date)
                                    {
                                        lobjHistory.icdoPersonAccountLifeHistory.effective_end_date = icdoPersonAccount.history_change_date;
                                        if(icdoPersonAccountLife.IsEndDatedDueToLossOfSuppLife == busConstant.Flag_Yes)
                                            lobjHistory.icdoPersonAccountLifeHistory.is_end_dated_due_to_loss_of_supp_life = busConstant.Flag_Yes;
                                        // Set flag to 'Y' so that ESS Benefit Enrollment report will ignore these records
                                        //lobjHistory.icdoPersonAccountLifeHistory.is_enrollment_report_generated = busConstant.Flag_Yes;//PIR 17081
                                        lobjHistory.icdoPersonAccountLifeHistory.Update();
                                    }
                                    //PIR-8900 -- > THe Effective end date should be updated if 
                                    //1. Effective End date is greater than History Change date minus one OR
                                    //2. Effective start date is less than or Equal to History change date minus one.
                                    else if (lobjHistory.icdoPersonAccountLifeHistory.effective_end_date == DateTime.MinValue
                                        || (lobjHistory.icdoPersonAccountLifeHistory.effective_end_date > icdoPersonAccount.history_change_date.AddDays(-1)
                                        && lobjHistory.icdoPersonAccountLifeHistory.effective_start_date <= icdoPersonAccount.history_change_date.AddDays(-1))) // PROD PIR 8635 :The end-date should not be modified again.
                                    {
                                        lobjHistory.icdoPersonAccountLifeHistory.effective_end_date = icdoPersonAccount.history_change_date.AddDays(-1);
                                        if (icdoPersonAccountLife.IsEndDatedDueToLossOfSuppLife == busConstant.Flag_Yes)
                                            lobjHistory.icdoPersonAccountLifeHistory.is_end_dated_due_to_loss_of_supp_life = busConstant.Flag_Yes;
                                        lobjHistory.icdoPersonAccountLifeHistory.Update();
                                    }
                                //}
                            }
                        }
                       
                        //Insert New History only when the Coverage Amount is Set
                        //Maik wants to insert the history for waived entry with coverage amount zero.
                        if ((lobjOption.icdoPersonAccountLifeOption.coverage_amount > 0) ||
                            (lobjOption.icdoPersonAccountLifeOption.plan_option_status_value == busConstant.PlanOptionStatusValueWaived))
                        {
                            bool lblnIsOptionEnteredFirstTime = false;
                            if (lobjHistory.icdoPersonAccountLifeHistory.person_account_life_history_id == 0)
                                lblnIsOptionEnteredFirstTime = true;                            
                            InsertHistory(lobjOption, lblnIsOptionEnteredFirstTime);
                            iblnIsHistoryInserted = true;
                        }
                    }
                }
            }
        }

        public void LoadMemberAge()
        {
            LoadMemberAge(DateTime.Now);
        }

        public void LoadMemberAge(DateTime adtEffectiveDate)
        {
            if (ibusPerson == null)
                LoadPerson();
            DateTime ldtPrevYearLastDay = new DateTime(adtEffectiveDate.Year - 1, 12, 31);
            icdoPersonAccountLife.Life_Insurance_Age = busGlobalFunctions.CalulateAge(ibusPerson.icdoPerson.date_of_birth, ldtPrevYearLastDay);
        }

        public void LoadProviderName()
        {
            if (ibusProviderOrgPlan != null)
                icdoPersonAccountLife.Provider_Name = busGlobalFunctions.GetOrgNameByOrgID(ibusProviderOrgPlan.icdoOrgPlan.org_id);
        }

        public void LoadPreviousHistory()
        {
            _iclbPreviousHistory = new Collection<busPersonAccountLifeHistory>();

            if (iclbPersonAccountLifeHistory == null)
                LoadHistory();

            bool lblnBasicFound = false;
            bool lblnSuppFound = false;
            bool lblnDepSuppFound = false;
            bool lblnSpouseSuppFound = false;

            foreach (busPersonAccountLifeHistory lbusPAHistory in iclbPersonAccountLifeHistory)
            {
                if (lbusPAHistory.icdoPersonAccountLifeHistory.level_of_coverage_value == busConstant.LevelofCoverage_Basic)
                {
                    if (!lblnBasicFound)
                    {
                        iclbPreviousHistory.Add(lbusPAHistory);
                        lblnBasicFound = true;
                    }
                }
                else if (lbusPAHistory.icdoPersonAccountLifeHistory.level_of_coverage_value == busConstant.LevelofCoverage_Supplemental)
                {
                    if (!lblnSuppFound)
                    {
                        iclbPreviousHistory.Add(lbusPAHistory);
                        lblnSuppFound = true;
                    }
                }
                else if (lbusPAHistory.icdoPersonAccountLifeHistory.level_of_coverage_value == busConstant.LevelofCoverage_DependentSupplemental)
                {
                    if (!lblnDepSuppFound)
                    {
                        iclbPreviousHistory.Add(lbusPAHistory);
                        lblnDepSuppFound = true;
                    }
                }
                else if (lbusPAHistory.icdoPersonAccountLifeHistory.level_of_coverage_value == busConstant.LevelofCoverage_SpouseSupplemental)
                {
                    if (!lblnSpouseSuppFound)
                    {
                        iclbPreviousHistory.Add(lbusPAHistory);
                        lblnSpouseSuppFound = true;
                    }
                }

                if (lblnBasicFound && lblnSuppFound && lblnDepSuppFound && lblnSpouseSuppFound) break;
            }
        }

        public void LoadPreviousHistoryForAnnualEnrollment(DateTime adteANNEDate)
        {
            _iclbPreviousHistory = new Collection<busPersonAccountLifeHistory>();
            if (iclbPersonAccountLifeHistory == null)
                LoadHistory();
            
            bool lblnBasicFound = false;
            bool lblnSuppFound = false;
            bool lblnDepSuppFound = false;
            bool lblnSpouseSuppFound = false;

            foreach (busPersonAccountLifeHistory lbusPAHistory in iclbPersonAccountLifeHistory)
            {
                if (lbusPAHistory.icdoPersonAccountLifeHistory.effective_start_date != adteANNEDate &&
                   lbusPAHistory.icdoPersonAccountLifeHistory.reason_value != busConstant.AnnualEnrollment)
                {
                    if (lbusPAHistory.icdoPersonAccountLifeHistory.level_of_coverage_value == busConstant.LevelofCoverage_Basic)
                    {
                        if (!lblnBasicFound)
                        {
                            lbusPAHistory.icdoPersonAccountLifeHistory.Sequence_ID = 1; //PIR 10422
                            iclbPreviousHistory.Add(lbusPAHistory);
                            lblnBasicFound = true;
                        }
                    }
                    else if (lbusPAHistory.icdoPersonAccountLifeHistory.level_of_coverage_value == busConstant.LevelofCoverage_Supplemental)
                    {
                        if (!lblnSuppFound)
                        {
                            lbusPAHistory.icdoPersonAccountLifeHistory.Sequence_ID = 2;//PIR 10422
                            iclbPreviousHistory.Add(lbusPAHistory);
                            lblnSuppFound = true;
                        }
                    }
                    else if (lbusPAHistory.icdoPersonAccountLifeHistory.level_of_coverage_value == busConstant.LevelofCoverage_DependentSupplemental)
                    {
                        if (!lblnDepSuppFound)
                        {
                            lbusPAHistory.icdoPersonAccountLifeHistory.Sequence_ID = 3;//PIR 10422
                            iclbPreviousHistory.Add(lbusPAHistory);
                            lblnDepSuppFound = true;
                        }
                    }
                    else if (lbusPAHistory.icdoPersonAccountLifeHistory.level_of_coverage_value == busConstant.LevelofCoverage_SpouseSupplemental)
                    {
                        if (!lblnSpouseSuppFound)
                        {
                            lbusPAHistory.icdoPersonAccountLifeHistory.Sequence_ID = 4;//PIR 10422
                            iclbPreviousHistory.Add(lbusPAHistory);
                            lblnSpouseSuppFound = true;
                        }
                    }
                }

                if (lblnBasicFound && lblnSuppFound && lblnDepSuppFound && lblnSpouseSuppFound) break;
            }
        }

        public void SetHistoryEntryRequiredOrNot()
        {
            if (_iclbPreviousHistory == null)
                LoadPreviousHistory();

            IsHistoryEntryRequired = false;

            //Clear the Collection If Exists
            _iclbLifeOptionModified = new Collection<busPersonAccountLifeOption>();
            foreach (busPersonAccountLifeOption lobjOption in iclbLifeOption)
            {
                //If Data Entered
                if (lobjOption.icdoPersonAccountLifeOption.effective_start_date != DateTime.MinValue)
                {
                    if (lobjOption.iblnOverlapHistoryFound)
                    {
                        IsHistoryEntryRequired = true;
                        _iclbLifeOptionModified.Add(lobjOption);
                        continue;
                    }
                    
                    //Check If the History Exists for this Option
                    busPersonAccountLifeHistory lobjPreviousOptionHistory = GetPreviousHistoryForOption(lobjOption);
                    if (lobjPreviousOptionHistory.icdoPersonAccountLifeHistory.effective_start_date != icdoPersonAccount.history_change_date)//pir 8708
                    {
                        IsHistoryEntryRequired = true;
                        _iclbLifeOptionModified.Add(lobjOption);
                        continue;
                    }

                    if (IsMandatoryFieldChanged(lobjOption, lobjPreviousOptionHistory))
                    {
                        IsHistoryEntryRequired = true;
                        _iclbLifeOptionModified.Add(lobjOption);
                        continue;
                    }
                }
            }
        }

        public busPersonAccountLifeHistory GetPreviousHistoryForOption(busPersonAccountLifeOption aobjOption)
        {
            busPersonAccountLifeHistory lobjOptionHistory = new busPersonAccountLifeHistory();
            lobjOptionHistory.icdoPersonAccountLifeHistory = new cdoPersonAccountLifeHistory();

            foreach (busPersonAccountLifeHistory lobjHistory in iclbPreviousHistory)
            {
                if (lobjHistory.icdoPersonAccountLifeHistory.level_of_coverage_value == aobjOption.icdoPersonAccountLifeOption.level_of_coverage_value)
                {
                    lobjOptionHistory = lobjHistory;
                    break;
                }
            }
            return lobjOptionHistory;
        }

        public Collection<busPersonAccountLifeHistory> iclbOverlappingHistory { get; set; }
        public Collection<busPersonAccountLifeHistory> LoadOverlappingHistoryForOption(busPersonAccountLifeOption aobjOption)
        {
            if (iclbPersonAccountLifeHistory == null)
                LoadHistory();
            Collection<busPersonAccountLifeHistory> lclbPALifeHistory = new Collection<busPersonAccountLifeHistory>();
            var lenuList = iclbPersonAccountLifeHistory.Where(i => busGlobalFunctions.CheckDateOverlapping(icdoPersonAccount.history_change_date,
                i.icdoPersonAccountLifeHistory.effective_start_date, i.icdoPersonAccountLifeHistory.effective_end_date)
                || i.icdoPersonAccountLifeHistory.effective_start_date > icdoPersonAccount.history_change_date);
            foreach (busPersonAccountLifeHistory lobjHistory in lenuList)
            {
                if (lobjHistory.icdoPersonAccountLifeHistory.level_of_coverage_value == aobjOption.icdoPersonAccountLifeOption.level_of_coverage_value)
                {
                    if (lobjHistory.icdoPersonAccountLifeHistory.effective_start_date >= icdoPersonAccount.history_change_date)
                    {
                        lclbPALifeHistory.Add(lobjHistory);
                    }
                    else if (lobjHistory.icdoPersonAccountLifeHistory.effective_start_date == lobjHistory.icdoPersonAccountLifeHistory.effective_end_date)
                    {
                        lclbPALifeHistory.Add(lobjHistory);
                    }
                    else if (lobjHistory.icdoPersonAccountLifeHistory.effective_start_date != lobjHistory.icdoPersonAccountLifeHistory.effective_end_date)
                    {
                        break;
                    }
                }
            }
            return lclbPALifeHistory;
        }

        public bool IsMoreThanOneEnrolledInOverlapHistory()
        {
            if (istrAllowOverlapHistory == busConstant.Flag_Yes)
            {
                if (iclbOverlappingHistory != null)
                {
                    foreach (busPersonAccountLifeOption lbusOption in iclbLifeOption)
                    {
                        var lenuList = iclbOverlappingHistory.Where(i => i.icdoPersonAccountLifeHistory.level_of_coverage_value == lbusOption.icdoPersonAccountLifeOption.level_of_coverage_value &&
                            i.icdoPersonAccountLifeHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled &&
                            i.icdoPersonAccountLifeHistory.start_date != i.icdoPersonAccountLifeHistory.end_date);
                        //PIR Enhanced Overlap - 23167, 23340, 23408
                        if (IsResourceForEnhancedOverlap(busConstant.OverlapResourceEnhancedLife))
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
            }
            return false;
        }

        private bool IsMandatoryFieldChanged(busPersonAccountLifeOption aobjOption, busPersonAccountLifeHistory aobjHistory)
        {
            bool lblnResult = false;
            if ((aobjHistory.icdoPersonAccountLifeHistory.plan_participation_status_value != icdoPersonAccount.plan_participation_status_value) ||
                (aobjHistory.icdoPersonAccountLifeHistory.life_insurance_type_value != icdoPersonAccountLife.life_insurance_type_value) ||
                (aobjHistory.icdoPersonAccountLifeHistory.plan_option_status_value != aobjOption.icdoPersonAccountLifeOption.plan_option_status_value) ||
                (aobjHistory.icdoPersonAccountLifeHistory.coverage_amount != aobjOption.icdoPersonAccountLifeOption.coverage_amount) ||
                (aobjHistory.icdoPersonAccountLifeHistory.premium_waiver_flag != icdoPersonAccountLife.premium_waiver_flag) ||
                (aobjHistory.icdoPersonAccountLifeHistory.projected_premium_waiver_date != icdoPersonAccountLife.projected_premium_waiver_date) ||
                (aobjHistory.icdoPersonAccountLifeHistory.waived_amount != icdoPersonAccountLife.waived_amount) ||
                (aobjHistory.icdoPersonAccountLifeHistory.actual_premium_waiver_date != icdoPersonAccountLife.actual_premium_waiver_date) ||
                (aobjHistory.icdoPersonAccountLifeHistory.premium_conversion_indicator_flag != icdoPersonAccountLife.premium_conversion_indicator_flag)
                )
            {
                lblnResult = true;
            }
            return lblnResult;
        }

        public bool IsHistoryChangeDateLessThanLastChangeDate()
        {
            bool lblnResult = false;

            if (IsHistoryEntryRequired)
            {
                if (_iclbPreviousHistory == null)
                    LoadPreviousHistory();

                foreach (busPersonAccountLifeOption lobjOption in _iclbLifeOptionModified)
                {
                    //Ignore Ended records
                    if (lobjOption.icdoPersonAccountLifeOption.coverage_amount > 0.00M)
                    {
                        busPersonAccountLifeHistory lobjPreviousHistory = GetPreviousHistoryForOption(lobjOption);
                        if (lobjPreviousHistory.icdoPersonAccountLifeHistory.person_account_life_history_id > 0)
                        {
                            if (lobjPreviousHistory.icdoPersonAccountLifeHistory.effective_end_date != DateTime.MinValue)
                            {
                                if (icdoPersonAccount.history_change_date < lobjPreviousHistory.icdoPersonAccountLifeHistory.effective_end_date)
                                {
                                    lblnResult = true;
                                    break;
                                }
                            }
                            else if (icdoPersonAccount.history_change_date < lobjPreviousHistory.icdoPersonAccountLifeHistory.effective_start_date)
                            {
                                lblnResult = true;
                                break;
                            }
                        }
                    }
                }
            }
            return lblnResult;
        }

        public busPersonAccountLifeHistory LoadHistoryByDate(busPersonAccountLifeOption aobjPALifeOption, DateTime adtGivenDate)
        {
            busPersonAccountLifeHistory lobjPersonAccountLifeHistory = new busPersonAccountLifeHistory();
            lobjPersonAccountLifeHistory.icdoPersonAccountLifeHistory = new cdoPersonAccountLifeHistory();

            if (iclbPersonAccountLifeHistory == null)
                LoadHistory();

            foreach (busPersonAccountLifeHistory lobjPALifeHistory in iclbPersonAccountLifeHistory)
            {
                if (lobjPALifeHistory.icdoPersonAccountLifeHistory.level_of_coverage_value == aobjPALifeOption.icdoPersonAccountLifeOption.level_of_coverage_value)
                {
                    //Ignore the Same Start Date and End Date Records
                    if (lobjPALifeHistory.icdoPersonAccountLifeHistory.effective_start_date != lobjPALifeHistory.icdoPersonAccountLifeHistory.effective_end_date)
                    {
                        if (busGlobalFunctions.CheckDateOverlapping(adtGivenDate, lobjPALifeHistory.icdoPersonAccountLifeHistory.effective_start_date,
                            lobjPALifeHistory.icdoPersonAccountLifeHistory.effective_end_date))
                        {
                            lobjPersonAccountLifeHistory = lobjPALifeHistory;
                            break;
                        }
                    }
                }
            }
            return lobjPersonAccountLifeHistory;
        }

        //This methoed i used to get the provider org id
        public busPersonAccountLifeHistory LoadHistoryByDateWithOutOption(DateTime adtGivenDate)
        {
            busPersonAccountLifeHistory lobjPersonAccountLifeHistory = new busPersonAccountLifeHistory();
            lobjPersonAccountLifeHistory.icdoPersonAccountLifeHistory = new cdoPersonAccountLifeHistory();

            if (iclbPersonAccountLifeHistory == null)
                LoadHistory();

            foreach (busPersonAccountLifeHistory lobjPALifeHistory in iclbPersonAccountLifeHistory)
            {
                if (lobjPALifeHistory.icdoPersonAccountLifeHistory.effective_start_date != DateTime.MinValue)
                {
                    //Ignore the Same Start Date and End Date Records
                    if (lobjPALifeHistory.icdoPersonAccountLifeHistory.effective_start_date != lobjPALifeHistory.icdoPersonAccountLifeHistory.effective_end_date)
                    {
                        if (busGlobalFunctions.CheckDateOverlapping(adtGivenDate, lobjPALifeHistory.icdoPersonAccountLifeHistory.effective_start_date,
                            lobjPALifeHistory.icdoPersonAccountLifeHistory.effective_end_date))
                        {
                            lobjPersonAccountLifeHistory = lobjPALifeHistory;
                            break;
                        }
                    }
                }
            }
            return lobjPersonAccountLifeHistory;
        }

        //Logic of Date to Calculate Premium Amount has been changed and the details are mailed on 3/18/2009 after the discussion with RAJ.
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
                if (iclbPersonAccountLifeHistory == null)
                    LoadHistory();

                //By Default the Collection sorted by latest date
                foreach (busPersonAccountLifeHistory lbusPersonAccountLifeHistory in iclbPersonAccountLifeHistory)
                {
                    if (lbusPersonAccountLifeHistory.icdoPersonAccountLifeHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                    {
                        if (lbusPersonAccountLifeHistory.icdoPersonAccountLifeHistory.level_of_coverage_value == busConstant.LevelofCoverage_Basic)
                        {
                            idtPlanEffectiveDate = LoadPlanEffectiveDate(lbusPersonAccountLifeHistory);
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This will be usefule when you need to find out the premium amount for specific History Records.
        /// As of now it used in TFFR File Generation
        /// </summary>
        /// <param name="abusPersonAccountLifeHistory"></param>
        public DateTime LoadPlanEffectiveDate(busPersonAccountLifeHistory abusPersonAccountLifeHistory)
        {
            DateTime ldtReturnDate = DateTime.Now;
            if (abusPersonAccountLifeHistory == null) return ldtReturnDate;
            if (abusPersonAccountLifeHistory.icdoPersonAccountLifeHistory.end_date == DateTime.MinValue)
            {
                //If the Start Date is Future Date, Set it otherwise Current Date will be Start Date of Premium Calc
                if (abusPersonAccountLifeHistory.icdoPersonAccountLifeHistory.start_date > DateTime.Now)
                {
                    ldtReturnDate = abusPersonAccountLifeHistory.icdoPersonAccountLifeHistory.start_date;
                }
                else
                {
                    ldtReturnDate = DateTime.Now;
                }
            }
            else
            {
                ldtReturnDate = abusPersonAccountLifeHistory.icdoPersonAccountLifeHistory.end_date;
            }

            return ldtReturnDate;
        }

        #region Correspondence
        public busPersonAccount ibusPersonAccount { get; set; }
        public void LoadPersonAccount()
        {
            if (ibusPersonAccount.IsNull())
                ibusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
            ibusPersonAccount.icdoPersonAccount = icdoPersonAccount;
            ibusPersonAccount.ibusPlan = ibusPlan;
        }
        /// Used in Correspondence
        private decimal _ldclBasicPremiumAmount;
        public decimal ldclBasicPremiumAmount
        {
            get
            {
                if (iclbLifeOption == null)
                    LoadLifeOptionData();
                foreach (busPersonAccountLifeOption lobjPALifeOption in iclbLifeOption)
                {
                    if (lobjPALifeOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_Basic)
                        _ldclBasicPremiumAmount = lobjPALifeOption.icdoPersonAccountLifeOption.Monthly_Premium;
                }
                return Math.Round(_ldclBasicPremiumAmount, 2);
            }
        }

        /// EndDate + 31 Days - Used in Correspondence
        private DateTime _ldtEffectiveEndDate;
        public DateTime ldtEffectiveEndDate
        {
            get { return _ldtEffectiveEndDate; }
            set { _ldtEffectiveEndDate = value; }
        }

        public string istrEffectiveEndDate
        {
            get
            {
                if (ldtEffectiveEndDate != DateTime.MinValue)
                    return ldtEffectiveEndDate.ToString(busConstant.DateFormatLongDate);
                else
                    return string.Empty;
            }
        }

        private string _lstrProjectedPremiumEffectiveDate;
        public string lstrProjectedPremiumEffectiveDate
        {
            get { return _lstrProjectedPremiumEffectiveDate; }
            set { _lstrProjectedPremiumEffectiveDate = value; }
        }


        //UCS 40
        public decimal idecLifeCoverageAmount { get; set; }
        public string istrLevelOfCoverage { get; set; }
        public string istrLevelOfCoverageValue { get; set; }
        public decimal idecCurrentIBSPremium { get; set; }
        public decimal idecNewIBSPremium { get; set; }
        #endregion

        public bool iblnIsSupplementalCoverageModifed { get; set; }

        // UAT PIR ID 472
        // UCS-022-00a - Flex Premium Conversion record exists for this provider.
        public bool IsConversionExistsAndCoverageModified()
        {
            if (ibusPerson.IsNull()) LoadPerson();

            if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled ||
                iblnIsSupplementalCoverageModifed)
            {
                if (ibusPerson.IsActiveFlexPremiumConversionExists(icdoPersonAccount.plan_id))
                {
                    return true;
                }
            }
            return false;
        }

        public void LoadCancelledEndDate(ref DateTime adtCancelledEffectiveDate)
        {
            if (_iclbPreviousHistory.IsNull())
                LoadPreviousHistory();
            if (_iclbPreviousHistory.Count > 0)
            {
                foreach (busPersonAccountLifeHistory lobjLifeHistory in _iclbPreviousHistory)
                {
                    if (lobjLifeHistory.icdoPersonAccountLifeHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceCancelled)
                    {
                        adtCancelledEffectiveDate = lobjLifeHistory.icdoPersonAccountLifeHistory.start_date;
                        break;
                    }
                }
            }
        }
        public override busBase GetCorPerson()
        {
            return base.GetCorPerson();
        }

        /// <summary>
        /// PROD pir 5538 : need to recalculate premium based on plan effective date
        /// </summary>
        private void RecalculatePremiumBasedOnPlanEffectiveDate()
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
            //Recalculate the Premium with Default History Change Date
            GetMonthlyPremiumAmount(idtPlanEffectiveDate);
        }

        #region UCS - 032

        public string istrESSProviderOrgName { get; set; }
        public int iintESSProviderOrgID { get; set; }
        public void ESSLoadProviderOrgName(int aintOrgID)
        {
            DataTable ldtOrgPlan = Select<cdoOrgPlan>(new string[2] { "org_id", "plan_id" },
                                                        new object[2] { aintOrgID, icdoPersonAccount.plan_id }, null, null);
            foreach(DataRow ldr in ldtOrgPlan.Rows)
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
        public override void LoadCorresProperties(string astrTemplateName)
        {
            if (ibusProvider.IsNull())
            {
                if (iclbPersonAccountLifeHistory.IsNull())
                    LoadHistory();
                if (iclbPersonAccountLifeHistory.Count > 0)
                {
                    foreach (busPersonAccountLifeHistory lobjPersonAccountHistory in iclbPersonAccountLifeHistory)
                    {
                        if (lobjPersonAccountHistory.icdoPersonAccountLifeHistory.provider_org_id != 0)
                        {
                            ibusProvider = new busOrganization();
                            ibusProvider.FindOrganization(iclbPersonAccountLifeHistory[0].icdoPersonAccountLifeHistory.provider_org_id);
                            break;
                        }
                    }
                }

            }
        }
        //pir 7817
        public bool IsACHDetailWithNoEndDateExists()
        {
            bool lblnResult = false;
            if (iclbPersonAccountAchDetail == null)
                LoadPersonAccountAchDetail();

            lblnResult = iclbPersonAccountAchDetail.Where(i => i.icdoPersonAccountAchDetail.ach_end_date.IsNull() || i.icdoPersonAccountAchDetail.ach_end_date.Equals(DateTime.MinValue)).Any();
            return lblnResult;
        }
        //pir 7987
        public bool IsPremiumConversionEnrolledButNoSupplemental()
        {
            bool lblnResult = false;
            if (icdoPersonAccountLife.premium_conversion_indicator_flag == busConstant.Flag_Yes && !IsSupplementalEntered())
            {
                lblnResult = true;
            }
            return lblnResult;
        }

        //PIR 10125
        public bool IsLifeInsuranceTypeActiveAndPremiumEnrolledNotSelected()
        {
            if (icdoPersonAccountLife.life_insurance_type_value == busConstant.LifeInsuranceTypeActiveMember)
            {
                if (icdoPersonAccountLife.premium_conversion_indicator_flag == null && !icdoPersonAccount.is_from_mss)// PIR 10649 - Validation not needed for MSS
                    return true;
            }
            return false;
        }

        public DataTable idtEmploymentDetails { get; set; }

        public void LoadPersonEmployment()
        {
            idtEmploymentDetails = busBase.Select("cdoPersonEmployment.LoadPersonEmploymentForDailyPeopleSoft", new object[1] { icdoPersonAccount.person_id });

            DataRow[] ldarrFilteredEmploymentDetails =
                idtEmploymentDetails.FilterTable(busConstant.DataType.Numeric, "person_account_id", icdoPersonAccount.person_account_id);
            if (ldarrFilteredEmploymentDetails.Count() > 0)
            {
                idtPlanEffectiveDate = ldarrFilteredEmploymentDetails[0]["h_end_date"] == DBNull.Value ?
                    DateTime.MinValue : Convert.ToDateTime(ldarrFilteredEmploymentDetails[0]["h_end_date"]);

                ibusPersonEmploymentDetail = new busPersonEmploymentDetail { icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail() };
                ibusPersonEmploymentDetail.ibusPersonEmployment = new busPersonEmployment { icdoPersonEmployment = new cdoPersonEmployment() };
                ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };

                ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.LoadData(ldarrFilteredEmploymentDetails[0]);
                ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date = ldarrFilteredEmploymentDetails[0]["d_start_date"] == DBNull.Value ?
                    DateTime.MinValue : Convert.ToDateTime(ldarrFilteredEmploymentDetails[0]["d_start_date"]);
                ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date = ldarrFilteredEmploymentDetails[0]["d_end_date"] == DBNull.Value ?
                    DateTime.MinValue : Convert.ToDateTime(ldarrFilteredEmploymentDetails[0]["d_end_date"]);

                ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.LoadData(ldarrFilteredEmploymentDetails[0]);
                ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.start_date = ldarrFilteredEmploymentDetails[0]["h_start_date"] == DBNull.Value ?
                    DateTime.MinValue : Convert.ToDateTime(ldarrFilteredEmploymentDetails[0]["h_start_date"]);
                ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.end_date = ldarrFilteredEmploymentDetails[0]["h_end_date"] == DBNull.Value ?
                    DateTime.MinValue : Convert.ToDateTime(ldarrFilteredEmploymentDetails[0]["h_end_date"]);
                ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.LoadData(ldarrFilteredEmploymentDetails[0]);
            }
            else
            {
                icdoPersonAccount.person_employment_dtl_id = GetEmploymentDetailID();
                LoadPersonEmploymentDetail();
                ibusPersonEmploymentDetail.LoadPersonEmployment();
                ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
            }
        }

        public bool CheckInsuranceType()
        {
            return icdoPersonAccountLife.life_insurance_type_value == busConstant.LifeInsuranceTypeActiveMember;
        }

        public bool CheckResourceAndInsuranceTypeForOverlap()
        {
            int lintCount = 0;

            lintCount = Convert.ToInt32(DBFunction.DBExecuteScalar("cdoPersonAccount.CheckIfLoggedInUserHasOverlapResource", new object[3] { iobjPassInfo.iintUserSerialID, 0, busConstant.OverlapResourceEnhancedLife },
            iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));

            if (lintCount > 0)
                return true;

            lintCount = Convert.ToInt32(DBFunction.DBExecuteScalar("cdoPersonAccount.CheckIfLoggedInUserHasOverlapResource", new object[3] { iobjPassInfo.iintUserSerialID, busConstant.OverlapResourceLife, 0 },
            iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));

            if (lintCount > 0 && CheckInsuranceType())
            {
                return true;
            }

            return false;
        }
        public bool IsMemberEligibleToEnrollInCurrentYear()
        {
            return icdoPersonAccount.IsNotNull() && icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled &&
                   IsMemberEligibleToEnrollInCurrentYear(icdoPersonAccount.history_change_date, icdoPersonAccountLife.reason_value, icdoPersonAccountLife.premium_conversion_indicator_flag == busConstant.Flag_Yes ? true : false);
        }
    }
}
