using NeoSpin.BusinessObjects;
using NeoSpin.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.ExceptionPub;
using System;
using System.Collections;
using System.Data;
using Sagitec.CustomDataObjects;
using System.Collections.ObjectModel;
using System.Linq;
using NeoSpin.DataObjects;
using Microsoft.VisualBasic;
using Sagitec.DBUtility;


namespace NeoSpinBatch
{
    class busHB1040ComparisonLetter : busNeoSpinBatch
    {
        DateTime idtTerminationDate;
        DateTime idtRetirementDate;
        bool iblnIsCorGenerated = false;
        public Collection<busPersonAccountRetirementContribution> _iclbRetirementContributionAll;
        public Collection<busPersonAccountRetirementContribution> iclbRetirementContributionAll
        {
            get
            {
                return _iclbRetirementContributionAll;
            }
            set
            {
                _iclbRetirementContributionAll = value;
            }
        }
        decimal idecDBBeginingBalanceAsOf2025 = 0.00m;
        public void ProcessHB1040ComparisonLetter()
        {
            istrProcessName = "HB1040 Comparison Letter";
            idlgUpdateProcessLog("Loading All Eligible Members HB1040 Comparison Letter Batch.", "INFO", istrProcessName);
            DataTable ldtbHB1040ComparisonMembers = busBase.Select("entHB1040Communication.GetPersonsToSendCorrespondence", new object[0] { });
            foreach (DataRow ldrRow in ldtbHB1040ComparisonMembers.Rows)
            {
                try
                {
                    busHb1040Communication lbusHb1040Communication = new busHb1040Communication { icdoHb1040Communication = new doHb1040Communication() };
                    lbusHb1040Communication.icdoHb1040Communication.LoadData(ldrRow);
                    DateTime ldtNRD = new DateTime();
                    decimal ldecFAS = 0;
                    //decimal ldecProjectedMonthlySalaryAmount = 0;

                    bool lblnIsUpdateRequired = false;

                    busPersonAccountRetirement lbusPersonAccountRetirement = new busPersonAccountRetirement { icdoPersonAccount = new cdoPersonAccount() };
                    lbusPersonAccountRetirement.FindPersonAccount(lbusHb1040Communication.icdoHb1040Communication.person_account_id);
                    lbusPersonAccountRetirement.FindPersonAccountRetirement(lbusHb1040Communication.icdoHb1040Communication.person_account_id);
                    lbusPersonAccountRetirement.LoadPerson();
                    //if person account is already exist in table just update the values
                    if (lbusHb1040Communication.FindHB1040Communication(lbusPersonAccountRetirement.icdoPersonAccount.person_account_id) &&
                        lbusPersonAccountRetirement.FindPersonAccount(lbusHb1040Communication.icdoHb1040Communication.person_account_id) &&
                        lbusHb1040Communication.icdoHb1040Communication.person_id == lbusPersonAccountRetirement.icdoPersonAccount.person_id)
                    {
                        lbusPersonAccountRetirement.LoadPlan();
                        lbusPersonAccountRetirement.LoadPerson();
                        lbusPersonAccountRetirement.LoadPersonAccount();
                        lbusPersonAccountRetirement.LoadPersonAccountForRetirementPlan();

                        lbusPersonAccountRetirement.ibusPersonAccount.LoadPerson();
                        lbusPersonAccountRetirement.ibusPersonAccount.LoadPlan();
                        lbusPersonAccountRetirement.LoadHb1040Communication();

                        if (lbusHb1040Communication.icdoHb1040Communication.letter_generated_1.IsNullOrEmpty() ||
                            lbusHb1040Communication.icdoHb1040Communication.letter_generated_1 == busConstant.Flag_No)
                        {
                            GetTerminationDate(lbusPersonAccountRetirement.ibusPersonAccount);
                            GetRetirementDate();
                            //GetBenefitValues(lbusPersonAccount, lbusHb1040Communication);                        
                            if (lbusHb1040Communication.icdoHb1040Communication.nrd == DateTime.MinValue)
                            {
                                ldtNRD = GetNRD(lbusPersonAccountRetirement.ibusPersonAccount);
                                decimal ldecAgeAtNRD = 0.00m;
                                decimal ldecAgeAt2025 = 0.00m;
                                ldecAgeAt2025 = GetAgeAtNRD(lbusPersonAccountRetirement.ibusPerson.icdoPerson.date_of_birth, new DateTime(2025, 01, 01));
                                ldecAgeAtNRD = GetAgeAtNRD(lbusPersonAccountRetirement.ibusPerson.icdoPerson.date_of_birth, ldtNRD);
                                if (ldecAgeAtNRD < ldecAgeAt2025)
                                    ldecAgeAtNRD = ldecAgeAt2025;

                                if (ldtNRD < new DateTime(2025, 01, 01))
                                    ldtNRD = new DateTime(2025, 02, 01);
                                                                
                                lbusHb1040Communication.icdoHb1040Communication.nrd = ldtNRD;

                                lbusHb1040Communication.icdoHb1040Communication.age_at_nrd = ldecAgeAtNRD;//GetAgeAtNRD(lbusPersonAccountRetirement.ibusPerson.icdoPerson.date_of_birth, ldtNRD);
                                if (lbusHb1040Communication.icdoHb1040Communication.nrd != DateTime.MinValue) lblnIsUpdateRequired = true;
                            }
                            else
                            {
                                ldtNRD = lbusHb1040Communication.icdoHb1040Communication.nrd;
                                if (ldtNRD < new DateTime(2025, 01, 01))
                                    ldtNRD = new DateTime(2025, 02, 01);
                            }
                            if (lbusHb1040Communication.icdoHb1040Communication.fas == 0.00m)
                            {
                                ldecFAS = GetFAS(lbusPersonAccountRetirement.ibusPersonAccount, ldtNRD, true);
                                lbusHb1040Communication.icdoHb1040Communication.fas = ldecFAS;
                                if (lbusHb1040Communication.icdoHb1040Communication.fas != 0.00m) lblnIsUpdateRequired = true;
                            }
                            else
                            {
                                ldecFAS = lbusHb1040Communication.icdoHb1040Communication.fas;
                            }
                            if (lbusHb1040Communication.icdoHb1040Communication.accrued_benefit_amt == 0.00m)
                            {
                                lbusPersonAccountRetirement = GetAccruedBenefitAmt(lbusPersonAccountRetirement, lbusHb1040Communication.icdoHb1040Communication.nrd);
                                lbusHb1040Communication.icdoHb1040Communication.accrued_benefit_amt = lbusPersonAccountRetirement.ibusRetirementBenefitCalculation.icdoBenefitCalculation.unreduced_benefit_amount;
                                if (lbusHb1040Communication.icdoHb1040Communication.accrued_benefit_amt != 0.00m) lblnIsUpdateRequired = true;
                            }
                            if (lbusHb1040Communication.icdoHb1040Communication.db_account_balance == 0.00m)
                            {
                                lbusHb1040Communication.icdoHb1040Communication.db_account_balance = GetDBAccountBalance(lbusPersonAccountRetirement);
                                if (lbusHb1040Communication.icdoHb1040Communication.db_account_balance != 0.00m) lblnIsUpdateRequired = true;
                            }
                            if (lbusHb1040Communication.icdoHb1040Communication.db_dc_transfer_amt == 0.00m)
                            {
                                //if(ldecFAS == 0)
                                //    ldecFAS = GetFAS(lbusPersonAccount, ldtNRD);
                                lbusHb1040Communication.icdoHb1040Communication.db_dc_transfer_amt = GetDBDCTransferAmountByCalculation(lbusPersonAccountRetirement.ibusPersonAccount, lbusHb1040Communication.icdoHb1040Communication.age_at_nrd,
                                    lbusHb1040Communication.icdoHb1040Communication.accrued_benefit_amt, lbusHb1040Communication.icdoHb1040Communication.db_account_balance);
                                if (lbusHb1040Communication.icdoHb1040Communication.db_dc_transfer_amt != 0.00m) lblnIsUpdateRequired = true;
                            }
                            if (lbusHb1040Communication.icdoHb1040Communication.member_dob == DateTime.MinValue)
                                lbusHb1040Communication.icdoHb1040Communication.member_dob = lbusPersonAccountRetirement.ibusPerson.icdoPerson.date_of_birth;

                            if (lblnIsUpdateRequired)
                            {
                                idlgUpdateProcessLog("Updating the member to generate the correspondence : " + lbusPersonAccountRetirement.icdoPersonAccount.person_id, "INFO", istrProcessName);
                                lbusHb1040Communication.icdoHb1040Communication.letter_generated_1 = busConstant.Flag_No;
                                lbusHb1040Communication.icdoHb1040Communication.Update();
                            }
                        }
                    }//end commumication table have a records
                    else
                    {
                        idlgUpdateProcessLog("Person Id not found or does not matched with Peron Account ID : " + lbusPersonAccountRetirement.icdoPersonAccount.person_id, "INFO", istrProcessName);

                    }//end commumication table don't have a records


                }//close try block
                catch (Exception _exc)
                {
                    idlgUpdateProcessLog("ERROR:" + _exc.Message, "INFO", istrProcessName);
                }
            }//close loop eligible query
            GenerateCorrespondence();
            if (iblnIsCorGenerated)
                idlgUpdateProcessLog("Correspondence generated successfully", "INFO", istrProcessName);
        }
        public void GetTerminationDate(busPersonAccount abusPersonAccount)
        {
            idtTerminationDate = DateTime.MinValue;
        }
        public void GetRetirementDate()
        {
            idtRetirementDate = DateTime.MinValue;
        }

        public DateTime GetNRD(busPersonAccount abusPersonAccount)
        {
            DateTime ldtNRD = new DateTime();
            busRetirementBenefitCalculation lobjRetirementBenefitCalculation = new busRetirementBenefitCalculation { icdoBenefitCalculation = new cdoBenefitCalculation() };
            decimal ldecConsolidatedServiceCredit = 0.0M;
            lobjRetirementBenefitCalculation.icdoBenefitCalculation.person_id = abusPersonAccount.icdoPersonAccount.person_id;
            lobjRetirementBenefitCalculation.icdoBenefitCalculation.plan_id = abusPersonAccount.icdoPersonAccount.plan_id;
            lobjRetirementBenefitCalculation.icdoBenefitCalculation.calculation_type_value = busConstant.CalculationTypeEstimate;
            ldecConsolidatedServiceCredit = lobjRetirementBenefitCalculation.GetConsolidatedExtraServiceCredit();

            if (abusPersonAccount.icdoPersonAccount.Total_VSC == 0.00M)
                abusPersonAccount.LoadTotalVSC();

            ldtNRD = busPersonBase.GetNormalRetirementDateBasedOnNormalEligibility(abusPersonAccount.icdoPersonAccount.plan_id, abusPersonAccount.ibusPlan.icdoPlan.plan_code,
                    abusPersonAccount.ibusPlan.icdoPlan.benefit_provision_id, busConstant.ApplicationBenefitTypeRetirement,
                    abusPersonAccount.ibusPerson.icdoPerson.date_of_birth, abusPersonAccount.icdoPersonAccount.Total_VSC,
                    0, iobjPassInfo, idtTerminationDate,
                    abusPersonAccount.icdoPersonAccount.person_account_id, true,
                    ldecConsolidatedServiceCredit, idtRetirementDate, null, abusPersonAccount); //PIR 14646

            return ldtNRD;
        }

        public decimal GetAgeAtNRD(DateTime adtPersonDOB, DateTime adtNDRDate)
        {
            decimal ldecAgeAtNRD = 0.00m;
            int lintMembersAgeInMonthsAsOnRetirementDate = 0;
            decimal ldecMemberAgeBasedOnRetirementDate = 0.00M;
            int lintMemberAgeMonthPart = 0;
            int lintMemberAgeYearPart = 0;

            DateTime ldtFrom = adtPersonDOB;
            DateTime ldtTo = adtNDRDate.AddMonths(-1);

            busPersonBase.CalculateAge(ldtFrom, ldtTo, ref lintMembersAgeInMonthsAsOnRetirementDate, ref ldecMemberAgeBasedOnRetirementDate, 4,
                            ref lintMemberAgeYearPart, ref lintMemberAgeMonthPart);

            ldecAgeAtNRD = ldecMemberAgeBasedOnRetirementDate;
            return ldecAgeAtNRD;
        }

        public decimal GetFAS(busPersonAccount abusPersonAccount, DateTime adtNRD, bool ablnIsProjected = false)
        {
            decimal ldecFAS = 0.00m;
            busBenefitCalculation lbusBenefitCalculation = new busBenefitCalculation { icdoBenefitCalculation = new cdoBenefitCalculation() };
            //decimal ldecComputedFAS = 0M, ldecCalculationFAS = 0M;
            lbusBenefitCalculation.icdoBenefitCalculation.plan_id = abusPersonAccount.icdoPersonAccount.plan_id;
            lbusBenefitCalculation.icdoBenefitCalculation.person_id = abusPersonAccount.icdoPersonAccount.person_id;
            lbusBenefitCalculation.ibusPersonAccount = abusPersonAccount;
            lbusBenefitCalculation.ibusPersonAccount.icdoPersonAccount = abusPersonAccount.icdoPersonAccount;
            lbusBenefitCalculation.ibusMember = abusPersonAccount.ibusPerson;
            lbusBenefitCalculation.ibusMember.icdoPerson = abusPersonAccount.ibusPerson.icdoPerson;
            lbusBenefitCalculation.icdoBenefitCalculation.calculation_type_value = busConstant.CalculationTypeEstimate;
            lbusBenefitCalculation.icdoBenefitCalculation.benefit_account_type_value = busConstant.ApplicationBenefitTypeRetirement;
            if (ablnIsProjected)
            {
                lbusBenefitCalculation.icdoBenefitCalculation.percentage_salary_increase = busConstant.AnnualSalaryIncreaseRate;
                lbusBenefitCalculation.icdoBenefitCalculation.salary_month_increase = busConstant.AnnualSalaryIncreaseMonth;
            }
            lbusBenefitCalculation.icdoBenefitCalculation.termination_date = adtNRD == DateTime.MinValue ? adtNRD : adtNRD.AddDays(-1);
            if (idtRetirementDate == DateTime.MinValue) idtRetirementDate = adtNRD;
            //if (idtTerminationDate == DateTime.MinValue) idtTerminationDate = idtRetirementDate;
            lbusBenefitCalculation.icdoBenefitCalculation.retirement_date = idtRetirementDate;
            lbusBenefitCalculation.icdoBenefitCalculation.fas_termination_date = lbusBenefitCalculation.icdoBenefitCalculation.termination_date;
            lbusBenefitCalculation.LoadBenefitProvisionBenefitType();
            lbusBenefitCalculation.ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.fas_formula_value = busConstant.FASHighestAverage;

            lbusBenefitCalculation.CalculateFAS();

            return lbusBenefitCalculation.icdoBenefitCalculation.calculation_final_average_salary;
        }
        public busPersonAccountRetirement GetAccruedBenefitAmt(busPersonAccountRetirement abusPersonAccountRetirement, DateTime adtRetirementDate)
        {
            abusPersonAccountRetirement.LoadRetirementContributionAll();
            abusPersonAccountRetirement.LoadNewRetirementBenefitCalculation();
            abusPersonAccountRetirement.AssignRetirementBenefitCalculationFields(busConstant.CalculationTypeEstimate);
            abusPersonAccountRetirement.ibusRetirementBenefitCalculation.icdoBenefitCalculation.termination_date = new DateTime(2025, 01, 01).AddMonths(-1);
            abusPersonAccountRetirement.ibusRetirementBenefitCalculation.icdoBenefitCalculation.retirement_date = adtRetirementDate;
            abusPersonAccountRetirement.ibusRetirementBenefitCalculation.icdoBenefitCalculation.fas_termination_date = abusPersonAccountRetirement.ibusRetirementBenefitCalculation.icdoBenefitCalculation.termination_date;
            abusPersonAccountRetirement.ibusRetirementBenefitCalculation.icdoBenefitCalculation.percentage_salary_increase = busConstant.AnnualSalaryIncreaseRate;
            abusPersonAccountRetirement.ibusRetirementBenefitCalculation.icdoBenefitCalculation.salary_month_increase = busConstant.AnnualSalaryIncreaseMonth;

            abusPersonAccountRetirement.ibusRetirementBenefitCalculation.CalculateConsolidatedPSC();
            if (abusPersonAccountRetirement.ibusRetirementBenefitCalculation.idteLastContributedDate == DateTime.MinValue)
                abusPersonAccountRetirement.ibusRetirementBenefitCalculation.LoadLastContributedDate();
            int lintMonthGap = busGlobalFunctions.DateDiffByMonth(abusPersonAccountRetirement.ibusRetirementBenefitCalculation.idteLastContributedDate.AddMonths(1), new DateTime(2024, 12, 1));
            abusPersonAccountRetirement.ibusRetirementBenefitCalculation.ibusPersonAccount.icdoPersonAccount.Total_PSC += lintMonthGap;

            abusPersonAccountRetirement.ibusRetirementBenefitCalculation.idteLastContributedDate = new DateTime(2025, 01, 01).AddDays(-1);
            abusPersonAccountRetirement.ibusRetirementBenefitCalculation.CalculateRetirementBenefit();
            abusPersonAccountRetirement.ibusRetirementBenefitCalculation.iblnConsolidatedPSCLoaded = false;

            if (idtTerminationDate == DateTime.MinValue) idtTerminationDate = abusPersonAccountRetirement.ibusRetirementBenefitCalculation.icdoBenefitCalculation.termination_date;
            if (idtRetirementDate == DateTime.MinValue) idtRetirementDate = abusPersonAccountRetirement.ibusRetirementBenefitCalculation.icdoBenefitCalculation.retirement_date;

            return abusPersonAccountRetirement;
        }
        public decimal GetDBAccountBalance(busPersonAccountRetirement abusPersonAccountRetirement)
        {
            decimal ldecDB_ACCOUNT_BALANCE = 0.00m;
            abusPersonAccountRetirement.LoadLTDSummary();
            ldecDB_ACCOUNT_BALANCE = abusPersonAccountRetirement.Member_Account_Balance_ltd;
            return ldecDB_ACCOUNT_BALANCE;
        }
        public decimal GetDBDCTransferAmount(busPersonAccountRetirement abusPersonAccountRetirement, decimal adecProjMonthlySalaryAmount)
        {
            decimal ldecDB_DC_TRANSFER_AMT = 0.00m;

            busPersonAccountRetirementDbDcTransferEstimate lbusPersonAccountRetirementDbDcTransferEstimate = new busPersonAccountRetirementDbDcTransferEstimate();
            lbusPersonAccountRetirementDbDcTransferEstimate.icdoPersonAccountRetirementDbDcTransferEstimate = new cdoPersonAccountRetirementDbDcTransferEstimate();
            lbusPersonAccountRetirementDbDcTransferEstimate.icdoPersonAccountRetirementDbDcTransferEstimate.person_account_id = abusPersonAccountRetirement.icdoPersonAccount.person_account_id;

            lbusPersonAccountRetirementDbDcTransferEstimate.icdoPersonAccountRetirementDbDcTransferEstimate.proj_month_no = busConstant.AnnualSalaryIncreaseMonth;
            lbusPersonAccountRetirementDbDcTransferEstimate.icdoPersonAccountRetirementDbDcTransferEstimate.proj_monthly_salary_amount = adecProjMonthlySalaryAmount;

            decimal ldecRate = 0;
            cdoPlanRetirementRate lcdoPlanRetirementRate =
                busGlobalFunctions.GetRetirementRateForPlanDateCombination(
                    abusPersonAccountRetirement.icdoPersonAccount.plan_id,
                    DateTime.Now, abusPersonAccountRetirement.GetMemberType(0));

            ldecRate = lcdoPlanRetirementRate.ee_pre_tax + lcdoPlanRetirementRate.ee_post_tax + lcdoPlanRetirementRate.ee_emp_pickup;

            lbusPersonAccountRetirementDbDcTransferEstimate.CalculateContributionsAndInterest(ldecRate);
            lbusPersonAccountRetirementDbDcTransferEstimate.LoadTotalEmployeeContributions();
            ldecRate = lcdoPlanRetirementRate.er_post_tax;
            lbusPersonAccountRetirementDbDcTransferEstimate.CalculateContributionsAndInterest(ldecRate);
            lbusPersonAccountRetirementDbDcTransferEstimate.LoadTotalEmployerContributions();

            lbusPersonAccountRetirementDbDcTransferEstimate.idecTransferAmt = lbusPersonAccountRetirementDbDcTransferEstimate.idecTotalEmployerContribution +
                lbusPersonAccountRetirementDbDcTransferEstimate.idecTotalEmployeeContribution;

            ldecDB_DC_TRANSFER_AMT = lbusPersonAccountRetirementDbDcTransferEstimate.idecTransferAmt;
            return ldecDB_DC_TRANSFER_AMT;
        }
        public decimal GetDBDCTransferAmountByCalculation(busPersonAccount abusPersonAccount, decimal adtAgeAtNRD, decimal ldecAccrudBenefitAmount, decimal ldecDBAccountBalance)
        {
            //DB DC Transfer factor 
            decimal ldecDCTransferFactor = GetDCTransferFactor(GetAgeAtNRD(abusPersonAccount.ibusPerson.icdoPerson.date_of_birth, new DateTime(2025, 01, 01)), adtAgeAtNRD);
            decimal ldecDBDCTransferAmount = ldecAccrudBenefitAmount * 12 * ldecDCTransferFactor;
            return ldecDBDCTransferAmount;
        }
        public double getValue()
        {
            int iNumberOfPayments = 120; //This will be input from below
            double dLoanAmount = 129276.38; //This will be the Projected DC Account Balance
            return PMT(0.0036748094, iNumberOfPayments, dLoanAmount) * -1;
        }
        public static double PMT(double yearlyInterestRate, double totalNumberOfMonths, double loanAmount)
        {
            var rate = (double)yearlyInterestRate;      // / 100 / 12;
            var denominator = Math.Pow((1 + rate), totalNumberOfMonths) - 1;
            if (denominator == 0)
                return 0;
            return (rate + (rate / denominator)) * loanAmount;
        }
        private void GenerateCorrespondence()
        {
            idlgUpdateProcessLog("Loading All Eligible Members To Generate the Correspondence", "INFO", istrProcessName);
            DataTable ldtbEligibleMembersForCor = busBase.Select("entHB1040Communication.GetPersonsToSendCorrespondence", new object[0] { });
            foreach (DataRow ldrRow in ldtbEligibleMembersForCor.Rows)
            {
                try
                {
                    busHb1040Communication lbusHb1040Communication = new busHb1040Communication { icdoHb1040Communication = new doHb1040Communication() };
                    lbusHb1040Communication.icdoHb1040Communication.LoadData(ldrRow);

                    busPersonAccountRetirement lbusPersonAccountRetirement = new busPersonAccountRetirement { icdoPersonAccount = new cdoPersonAccount() };
                    lbusPersonAccountRetirement.FindPersonAccount(lbusHb1040Communication.icdoHb1040Communication.person_account_id);
                    lbusPersonAccountRetirement.FindPersonAccountRetirement(lbusHb1040Communication.icdoHb1040Communication.person_account_id);
                    lbusPersonAccountRetirement.LoadPerson();

                    lbusPersonAccountRetirement.LoadPersonAccount();
                    lbusPersonAccountRetirement.LoadPersonAccountForRetirementPlan();
                    lbusPersonAccountRetirement.LoadHb1040Communication();
                    lbusPersonAccountRetirement.LoadPlan();
                    lbusPersonAccountRetirement.LoadPerson();
                    lbusPersonAccountRetirement.ibusPersonAccount.LoadPlan();
                    lbusPersonAccountRetirement.ibusPersonAccount.LoadPerson();
                    lbusPersonAccountRetirement.ibusPersonAccount.LoadTotalPSC();

                    busPersonEmploymentDetail lobjPersonEmploymentDetail = new busPersonEmploymentDetail { icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail() };
                    lobjPersonEmploymentDetail = lbusPersonAccountRetirement.GetLatestEmploymentDetail();
                    lbusPersonAccountRetirement.ibusPersonEmploymentDetail = lobjPersonEmploymentDetail;
                    lbusPersonAccountRetirement.icdoPersonAccount.person_employment_dtl_id = lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id;
                    lbusPersonAccountRetirement.LoadPersonEmploymentDetail();

                    lbusPersonAccountRetirement.ibusPersonEmploymentDetail.LoadPersonEmployment();

                    lbusPersonAccountRetirement.LoadNewRetirementBenefitCalculation();
                    lbusPersonAccountRetirement.AssignRetirementBenefitCalculationFields(busConstant.CalculationTypeEstimate);
                    //lbusPersonAccountRetirement.CalculateRetirementBenefitAmount();

                    lbusPersonAccountRetirement.ibusRetirementBenefitCalculation.icdoBenefitCalculation.retirement_date = lbusPersonAccountRetirement.ibusHb1040Communication.icdoHb1040Communication.nrd;
                    lbusPersonAccountRetirement.ibusRetirementBenefitCalculation.icdoBenefitCalculation.termination_date = lbusPersonAccountRetirement.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date == DateTime.MinValue ?
                                                                                         lbusPersonAccountRetirement.ibusRetirementBenefitCalculation.icdoBenefitCalculation.retirement_date == DateTime.MinValue ? lbusPersonAccountRetirement.ibusRetirementBenefitCalculation.icdoBenefitCalculation.retirement_date : lbusPersonAccountRetirement.ibusRetirementBenefitCalculation.icdoBenefitCalculation.retirement_date.AddMonths(-1) :
                                                                                        lbusPersonAccountRetirement.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date;
                    lbusPersonAccountRetirement.ibusRetirementBenefitCalculation.icdoBenefitCalculation.fas_termination_date = lbusPersonAccountRetirement.ibusRetirementBenefitCalculation.icdoBenefitCalculation.termination_date;
                    lbusPersonAccountRetirement.ibusRetirementBenefitCalculation.icdoBenefitCalculation.percentage_salary_increase = busConstant.AnnualSalaryIncreaseRate;
                    lbusPersonAccountRetirement.ibusRetirementBenefitCalculation.icdoBenefitCalculation.salary_month_increase = busConstant.AnnualSalaryIncreaseMonth;
                    lbusPersonAccountRetirement.ibusRetirementBenefitCalculation.CalculateRetirementBenefit();
                    LoadOtherCorrProperties(lbusPersonAccountRetirement);
                    idlgUpdateProcessLog("Creating correspondence for the PERSLink ID : " + lbusPersonAccountRetirement.ibusHb1040Communication.icdoHb1040Communication.person_id, "INFO", istrProcessName);

                    //PIR 25920 update back to table dbDBTranser amount and also print on document so whichever is greater from projection year Jan 2025 and actual DBDCTransfer amount
                    lbusHb1040Communication.icdoHb1040Communication.jan_25_DB_Acct_Bal = idecDBBeginingBalanceAsOf2025.RoundToTwoDecimalPoints();
                    lbusHb1040Communication.icdoHb1040Communication.db_dc_transfer_amt =
                      lbusHb1040Communication.icdoHb1040Communication.db_dc_transfer_amt.RoundToTwoDecimalPoints() > idecDBBeginingBalanceAsOf2025.RoundToTwoDecimalPoints()
                    ? lbusHb1040Communication.icdoHb1040Communication.db_dc_transfer_amt.RoundToTwoDecimalPoints() : idecDBBeginingBalanceAsOf2025.RoundToTwoDecimalPoints();

                    //Assign before generate Correspondence
                    lbusPersonAccountRetirement.ibusHb1040Communication.icdoHb1040Communication.db_dc_transfer_amt = lbusHb1040Communication.icdoHb1040Communication.db_dc_transfer_amt;
                    lbusPersonAccountRetirement.ibusHb1040Communication.icdoHb1040Communication.jan_25_DB_Acct_Bal = idecDBBeginingBalanceAsOf2025.RoundToTwoDecimalPoints();

                    CreateCorrespondence(lbusPersonAccountRetirement);

                    lbusHb1040Communication.icdoHb1040Communication.letter_generated_1 = busConstant.Flag_Yes;
                    lbusHb1040Communication.icdoHb1040Communication.letter_generated_date_1 = DateTime.Now;
                    lbusHb1040Communication.icdoHb1040Communication.Update();
                    iblnIsCorGenerated = true;
                }
                catch (Exception _exc)
                {
                    idlgUpdateProcessLog("ERROR:" + _exc.Message, "INFO", istrProcessName);
                    //iblnIsCorGenerated = false;
                }
            }
            //updating the cor status to Ready for Imaging 
            //busBase.Select("entCorTracking.UpdateCorStatusToImagingFromCommunicationBatch", new object[0] { });
            DBFunction.DBNonQuery("entCorTracking.UpdateCorStatusToImagingFromCommunicationBatch", new object[0] { },
                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
        }
        public void LoadOtherCorrProperties(busPersonAccountRetirement abusPersonAccountRetirement)
        {
            decimal ldecLifeExpenctancy = GetLifeExpectancy(Convert.ToDecimal(Math.Round(Convert.ToDouble(abusPersonAccountRetirement.ibusHb1040Communication.icdoHb1040Communication.age_at_nrd) / 5.0) * 5), abusPersonAccountRetirement.ibusPerson.icdoPerson.gender_value);

            //DB DC Transfer factor 
            decimal ldecDCTransferFactor = GetDCTransferFactor(GetAgeAtNRD(abusPersonAccountRetirement.ibusPerson.icdoPerson.date_of_birth, new DateTime(2025, 01, 01)), abusPersonAccountRetirement.ibusHb1040Communication.icdoHb1040Communication.age_at_nrd);
            //Employee and Employer Contribution Rate
            SetRetirementPlanRateEEAndER(abusPersonAccountRetirement);

            //Current Salary 
            abusPersonAccountRetirement.ibusHb1040Communication.idecCurrentSalary = GetFAS(abusPersonAccountRetirement.ibusPersonAccount, abusPersonAccountRetirement.ibusHb1040Communication.icdoHb1040Communication.nrd, false) * 12;

            //Projected Yearly FAS at Retirement 
            abusPersonAccountRetirement.ibusHb1040Communication.idecProjectedYearlyFASatRetirement = abusPersonAccountRetirement.ibusHb1040Communication.icdoHb1040Communication.fas * 12;

            //LifeExpDCMonthlyBenefit 
            abusPersonAccountRetirement.ibusHb1040Communication.idecLifeExpDBBenefit = ldecLifeExpenctancy;

            //PSC at Retirement 
            abusPersonAccountRetirement.ibusHb1040Communication.idecPSCAtRetirement = GetPSCAtRetirement(abusPersonAccountRetirement);

            //Current PSC - No projection
            abusPersonAccountRetirement.ibusPersonAccount.LoadTotalPSC();
            abusPersonAccountRetirement.ibusHb1040Communication.idecPSC = abusPersonAccountRetirement.ibusPersonAccount.icdoPersonAccount.Total_PSC;

            //Current plan tier name
            abusPersonAccountRetirement.ibusHb1040Communication.istrCurrentPlanTierName = abusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_description_display;

            //future plan tier name for DC - Hard coded after discuss with Maik on 10/30/2024
            abusPersonAccountRetirement.ibusHb1040Communication.istrFuturePlanTierName = abusPersonAccountRetirement.icdoPersonAccount.plan_id == busConstant.PlanIdMain ?
                                                                                                "Tier 1 DC" : "Tier 2 DC 2020";
            //DataTable ldtbResult = busBase.Select("entPersonAccountRetirement.LoadBenefitTierByPriorEnrolledHistoryDate", new object[3] { abusPersonAccountRetirement.ibusPersonAccount.icdoPersonAccount.person_id, abusPersonAccountRetirement.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMain ? busConstant.PlanIdDC : busConstant.PlanIdDC2020, abusPersonAccountRetirement.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date });
            //if (ldtbResult.Rows.Count > 0)
            //{
            //    abusPersonAccountRetirement.ibusHb1040Communication.istrFuturePlanTierName = busGlobalFunctions.GetDescriptionByCodeValue(7003, ldtbResult.Rows[0]["CODE_VALUE"].ToString(), iobjPassInfo);  
            //}

            //DB Account Balance
            abusPersonAccountRetirement.ibusHb1040Communication.idecProjectedDBAccountBalance = GetProjectedDBAccountBalance(abusPersonAccountRetirement);

            //Monthly DB Benefit Amount 
            abusPersonAccountRetirement.ibusHb1040Communication.idecMonthlyDBBenefitAmount = abusPersonAccountRetirement.ibusRetirementBenefitCalculation.icdoBenefitCalculation.unreduced_benefit_amount;
            //Benefit as % of FAS
            //taking precaustion while divide by zero values
            if (abusPersonAccountRetirement.ibusHb1040Communication.idecProjectedYearlyFASatRetirement > 0)
                abusPersonAccountRetirement.ibusHb1040Communication.idecBenefitASPercentileOfFAS = Convert.ToDecimal(abusPersonAccountRetirement.ibusHb1040Communication.idecMonthlyDBBenefitAmount * 12 / abusPersonAccountRetirement.ibusHb1040Communication.idecProjectedYearlyFASatRetirement * 100).RoundToTwoDecimalPoints();
            else
                abusPersonAccountRetirement.ibusHb1040Communication.idecBenefitASPercentileOfFAS = 0;
            //DB Life Time benefit
            abusPersonAccountRetirement.ibusHb1040Communication.idec10YearDBLifeTimeBenefit = Convert.ToDecimal(abusPersonAccountRetirement.ibusHb1040Communication.idecMonthlyDBBenefitAmount * 12 * 10).RoundToTwoDecimalPoints();
            abusPersonAccountRetirement.ibusHb1040Communication.idec15YearDBLifeTimeBenefit = Convert.ToDecimal(abusPersonAccountRetirement.ibusHb1040Communication.idecMonthlyDBBenefitAmount * 12 * 15).RoundToTwoDecimalPoints();
            abusPersonAccountRetirement.ibusHb1040Communication.idec20YearDBLifeTimeBenefit = Convert.ToDecimal(abusPersonAccountRetirement.ibusHb1040Communication.idecMonthlyDBBenefitAmount * 12 * 20).RoundToTwoDecimalPoints();
            abusPersonAccountRetirement.ibusHb1040Communication.idec25YearDBLifeTimeBenefit = Convert.ToDecimal(abusPersonAccountRetirement.ibusHb1040Communication.idecMonthlyDBBenefitAmount * 12 * 25).RoundToTwoDecimalPoints();
            abusPersonAccountRetirement.ibusHb1040Communication.idec30YearDBLifeTimeBenefit = Convert.ToDecimal(abusPersonAccountRetirement.ibusHb1040Communication.idecMonthlyDBBenefitAmount * 12 * 30).RoundToTwoDecimalPoints();
            abusPersonAccountRetirement.ibusHb1040Communication.idec35YearDBLifeTimeBenefit = Convert.ToDecimal(abusPersonAccountRetirement.ibusHb1040Communication.idecMonthlyDBBenefitAmount * 12 * 35).RoundToTwoDecimalPoints();
            abusPersonAccountRetirement.ibusHb1040Communication.idec40YearDBLifeTimeBenefit = Convert.ToDecimal(abusPersonAccountRetirement.ibusHb1040Communication.idecMonthlyDBBenefitAmount * 12 * 40).RoundToTwoDecimalPoints();

            abusPersonAccountRetirement.ibusHb1040Communication.idecLifeExpDBLifeTimeBenefit = Convert.ToDecimal(abusPersonAccountRetirement.ibusHb1040Communication.idecMonthlyDBBenefitAmount * 12 * ldecLifeExpenctancy).RoundToTwoDecimalPoints();



            //DC Account balance
            decimal ldecProjectedDCAccountBalance = GetProjectedDCAccountBalance(abusPersonAccountRetirement);
            abusPersonAccountRetirement.ibusHb1040Communication.idecProjectedDCAccountBalance = ldecProjectedDCAccountBalance.RoundToTwoDecimalPoints();

            //DC Monthly Benefit
            abusPersonAccountRetirement.ibusHb1040Communication.idec10YearDCMonthlyBenefit = Convert.ToDecimal(PMT(0.0036748094, 120, Convert.ToDouble(ldecProjectedDCAccountBalance))).RoundToTwoDecimalPoints();
            abusPersonAccountRetirement.ibusHb1040Communication.idec15YearDCMonthlyBenefit = Convert.ToDecimal(PMT(0.0036748094, 180, Convert.ToDouble(ldecProjectedDCAccountBalance))).RoundToTwoDecimalPoints();
            abusPersonAccountRetirement.ibusHb1040Communication.idec20YearDCMonthlyBenefit = Convert.ToDecimal(PMT(0.0036748094, 240, Convert.ToDouble(ldecProjectedDCAccountBalance))).RoundToTwoDecimalPoints();
            abusPersonAccountRetirement.ibusHb1040Communication.idec25YearDCMonthlyBenefit = Convert.ToDecimal(PMT(0.0036748094, 300, Convert.ToDouble(ldecProjectedDCAccountBalance))).RoundToTwoDecimalPoints();
            abusPersonAccountRetirement.ibusHb1040Communication.idec30YearDCMonthlyBenefit = Convert.ToDecimal(PMT(0.0036748094, 360, Convert.ToDouble(ldecProjectedDCAccountBalance))).RoundToTwoDecimalPoints();
            abusPersonAccountRetirement.ibusHb1040Communication.idec35YearDCMonthlyBenefit = Convert.ToDecimal(PMT(0.0036748094, 420, Convert.ToDouble(ldecProjectedDCAccountBalance))).RoundToTwoDecimalPoints();
            abusPersonAccountRetirement.ibusHb1040Communication.idec40YearDCMonthlyBenefit = Convert.ToDecimal(PMT(0.0036748094, 480, Convert.ToDouble(ldecProjectedDCAccountBalance))).RoundToTwoDecimalPoints();

            abusPersonAccountRetirement.ibusHb1040Communication.idecLifeExpDCMonthlyBenefit = Convert.ToDecimal(PMT(0.0036748094, (12 * Convert.ToDouble(ldecLifeExpenctancy)), Convert.ToDouble(ldecProjectedDCAccountBalance))).RoundToTwoDecimalPoints();
            //DC Life Time benefit
            abusPersonAccountRetirement.ibusHb1040Communication.idec10YearDCLifeTimeBenefit = Convert.ToDecimal(abusPersonAccountRetirement.ibusHb1040Communication.idec10YearDCMonthlyBenefit * 12 * 10).RoundToTwoDecimalPoints();
            abusPersonAccountRetirement.ibusHb1040Communication.idec15YearDCLifeTimeBenefit = Convert.ToDecimal(abusPersonAccountRetirement.ibusHb1040Communication.idec15YearDCMonthlyBenefit * 12 * 15).RoundToTwoDecimalPoints();
            abusPersonAccountRetirement.ibusHb1040Communication.idec20YearDCLifeTimeBenefit = Convert.ToDecimal(abusPersonAccountRetirement.ibusHb1040Communication.idec20YearDCMonthlyBenefit * 12 * 20).RoundToTwoDecimalPoints();
            abusPersonAccountRetirement.ibusHb1040Communication.idec25YearDCLifeTimeBenefit = Convert.ToDecimal(abusPersonAccountRetirement.ibusHb1040Communication.idec25YearDCMonthlyBenefit * 12 * 25).RoundToTwoDecimalPoints();
            abusPersonAccountRetirement.ibusHb1040Communication.idec30YearDCLifeTimeBenefit = Convert.ToDecimal(abusPersonAccountRetirement.ibusHb1040Communication.idec30YearDCMonthlyBenefit * 12 * 30).RoundToTwoDecimalPoints();
            abusPersonAccountRetirement.ibusHb1040Communication.idec35YearDCLifeTimeBenefit = Convert.ToDecimal(abusPersonAccountRetirement.ibusHb1040Communication.idec35YearDCMonthlyBenefit * 12 * 35).RoundToTwoDecimalPoints();
            abusPersonAccountRetirement.ibusHb1040Communication.idec40YearDCLifeTimeBenefit = Convert.ToDecimal(abusPersonAccountRetirement.ibusHb1040Communication.idec40YearDCMonthlyBenefit * 12 * 40).RoundToTwoDecimalPoints();

            abusPersonAccountRetirement.ibusHb1040Communication.idecLifeExpDCLifeTimeBenefit = Convert.ToDecimal(abusPersonAccountRetirement.ibusHb1040Communication.idecLifeExpDCMonthlyBenefit * 12 * ldecLifeExpenctancy).RoundToTwoDecimalPoints();


        }

        public decimal GetPSCAtRetirement(busPersonAccountRetirement abusPersonAccountRetirement)
        {
            busRetirementBenefitCalculation lobjRetirementCalculation = new busRetirementBenefitCalculation
            {
                icdoBenefitCalculation = new cdoBenefitCalculation(),
                ibusMember = new busPerson { icdoPerson = new cdoPerson() },
                ibusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount(), ibusPlan = new busPlan { icdoPlan = new cdoPlan() } },
                ibusPlan = new busPlan { icdoPlan = new cdoPlan() },
                ibusJointAnnuitant = new busPerson { icdoPerson = new cdoPerson() }
            };
            lobjRetirementCalculation.ibusMember = abusPersonAccountRetirement.ibusPerson;
            lobjRetirementCalculation.ibusPersonAccount = abusPersonAccountRetirement.ibusPersonAccount;
            lobjRetirementCalculation.ibusPlan = abusPersonAccountRetirement.ibusPersonAccount.ibusPlan;
            lobjRetirementCalculation.ibusPersonAccount.ibusPlan = abusPersonAccountRetirement.ibusPersonAccount.ibusPlan;
            lobjRetirementCalculation.icdoBenefitCalculation.plan_id = lobjRetirementCalculation.ibusPlan.icdoPlan.plan_id;
            lobjRetirementCalculation.icdoBenefitCalculation.person_id = lobjRetirementCalculation.ibusMember.icdoPerson.person_id;
            lobjRetirementCalculation.icdoBenefitCalculation.retirement_date = abusPersonAccountRetirement.ibusHb1040Communication.icdoHb1040Communication.nrd;
            lobjRetirementCalculation.icdoBenefitCalculation.termination_date = abusPersonAccountRetirement.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date == DateTime.MinValue ?
                                                                                 lobjRetirementCalculation.icdoBenefitCalculation.retirement_date == DateTime.MinValue ? lobjRetirementCalculation.icdoBenefitCalculation.retirement_date : lobjRetirementCalculation.icdoBenefitCalculation.retirement_date.AddMonths(-1) :
                                                                                abusPersonAccountRetirement.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date;

            lobjRetirementCalculation.iblnConsolidatedPSCLoaded = false;
            lobjRetirementCalculation.icdoBenefitCalculation.benefit_account_type_value = busConstant.ApplicationBenefitTypeRetirement;
            if (lobjRetirementCalculation.iclbBenefitCalculationPersonAccount.IsNull())
                lobjRetirementCalculation.LoadPersonPlanAccounts();
            DateTime ldteActualEmployeeTerminationDate = new DateTime();
            lobjRetirementCalculation.GetOrgIdAsLatestEmploymentOrgId(
                                abusPersonAccountRetirement.ibusPersonAccount,
                                lobjRetirementCalculation.icdoBenefitCalculation.benefit_account_type_value,
                                ref ldteActualEmployeeTerminationDate);
            lobjRetirementCalculation.icdoBenefitCalculation.calculation_type_value = busConstant.CalculationTypeEstimate;
            if (ldteActualEmployeeTerminationDate != DateTime.MinValue)
            {
                lobjRetirementCalculation.icdoBenefitCalculation.termination_date = ldteActualEmployeeTerminationDate;
                if (!(lobjRetirementCalculation.IsMemberDual()))
                    lobjRetirementCalculation.icdoBenefitCalculation.calculation_type_value = busConstant.CalculationTypeFinal;
            }
            if (!lobjRetirementCalculation.iblnConsolidatedPSCLoaded)
                lobjRetirementCalculation.CalculateConsolidatedPSC();

            return lobjRetirementCalculation.icdoBenefitCalculation.credited_psc;
        }

        public decimal GetMonthlyDBBenefitAmount(busPersonAccountRetirement abusPersonAccountRetirement)
        {
            decimal ldecMonthlyDBBenefitAmount = 0.00m;

            abusPersonAccountRetirement.LoadRetirementContributionAll();
            abusPersonAccountRetirement.LoadNewRetirementBenefitCalculation();
            abusPersonAccountRetirement.AssignRetirementBenefitCalculationFields(busConstant.CalculationTypeEstimate);

            abusPersonAccountRetirement.CalculateAccruedBenefit(abusPersonAccountRetirement.ibusRetirementBenefitCalculation.icdoBenefitCalculation.termination_date, abusPersonAccountRetirement.ibusRetirementBenefitCalculation.icdoBenefitCalculation.retirement_date);
            ldecMonthlyDBBenefitAmount = abusPersonAccountRetirement.ibusRetirementBenefitCalculation.icdoBenefitCalculation.unreduced_benefit_amount;
            return ldecMonthlyDBBenefitAmount;
        }
        public decimal GetProjectedDBAccountBalance(busPersonAccountRetirement abusPersonAccountRetirement)
        {
            decimal ldecProjectedDB_ACCOUNT_BALANCE = 0.00m;
            decimal ldecContribution = 0.00m;
            decimal ldecCurrentWagesAmount = 0.000m;
            decimal ldecDBAccountBalance = 0.00m;
            int lintPlanID = abusPersonAccountRetirement.ibusPersonAccount.icdoPersonAccount.plan_id;
            decimal ldecContributionRate = 0.00m;
            DateTime ldtNRD = DateTime.MinValue;
            decimal ldecMemberInterest = 0.00M;
            DateTime ldtLastContributionDate;

            decimal ldecTotalContributionRetirementRate = 1m;
            abusPersonAccountRetirement.ibusPersonAccount.LoadLastContributionDateByRegularPayroll();
            ldtLastContributionDate = abusPersonAccountRetirement.ibusPersonAccount.idtLastContributionDate;
            ldtLastContributionDate = ldtLastContributionDate.AddMonths(1);
            busPlanRetirementRate lobjPlanRetirement = new busPlanRetirementRate();
            ldecCurrentWagesAmount = abusPersonAccountRetirement.ibusPersonAccount.idecLastContributionWages;

            if (ldecCurrentWagesAmount == 0.00m) ldecCurrentWagesAmount = 1;

            ldecDBAccountBalance = abusPersonAccountRetirement.ibusHb1040Communication.icdoHb1040Communication.db_account_balance;
            ldtNRD = abusPersonAccountRetirement.ibusHb1040Communication.icdoHb1040Communication.nrd;
            ldecContributionRate = lintPlanID == busConstant.PlanIdMain2020 ? busConstant.ContributionRateMain2020 : busConstant.ContributionRateMain;
            ldecTotalContributionRetirementRate = busConstant.ContributionRetirementRate;

            idecDBBeginingBalanceAsOf2025 = 0.00m;
			idecDBBeginingBalanceAsOf2025 = ldecDBAccountBalance;
            int lintCounter = 0;
            while (ldtLastContributionDate < ldtNRD)
            {
                if (busConstant.AnnualSalaryIncreaseMonth == ldtLastContributionDate.Month)
                {
                    ldecCurrentWagesAmount += (ldecCurrentWagesAmount.RoundToTwoDecimalPoints() * busConstant.AnnualSalaryIncreaseRate) / 100;
                    //ldecContribution += (ldecContribution * (ldecContributionRate / 100));
                }

                ldecContribution = (ldecCurrentWagesAmount.RoundToTwoDecimalPoints() * ((ldecContributionRate.RoundToTwoDecimalPoints()) / 100));

                ldecDBAccountBalance += ldecContribution.RoundToTwoDecimalPoints() + (ldecDBAccountBalance * ((ldecTotalContributionRetirementRate.RoundToTwoDecimalPoints()) / 100));

                ldecDBAccountBalance = ldecDBAccountBalance.RoundToTwoDecimalPoints();

                if (ldtLastContributionDate.Year == 2025 && ldtLastContributionDate.Month == 1)
                    idecDBBeginingBalanceAsOf2025 = ldecDBAccountBalance;

                if (ldtLastContributionDate.Day == 1)
                    ldtLastContributionDate = ldtLastContributionDate.AddMonths(1);
                else
                    ldtLastContributionDate = ldtLastContributionDate.GetFirstDayofNextMonth();

                //if (ldtLastContributionDate.Year == 2025 && ldtLastContributionDate.Month == 1)
                //    idecDBBeginingBalanceAsOf2025 = ldecDBAccountBalance;

                //if (idecDBBeginingBalanceAsOf2025 == 0.00m)
                //    idecDBBeginingBalanceAsOf2025 = ldecDBAccountBalance;

                lintCounter++;
            }

            ldecProjectedDB_ACCOUNT_BALANCE = ldecDBAccountBalance;

            return ldecProjectedDB_ACCOUNT_BALANCE;
        }
        public decimal GetProjectedDCAccountBalance(busPersonAccountRetirement abusPersonAccountRetirement)
        {
            decimal ldecProjectedDC_ACCOUNT_BALANCE = 0.00m;
            decimal ldecContribution = 0.00m;
            decimal ldecCurrentWagesAmount = 0.000m;
            decimal ldecDBAccountBalance = 0.00m;
            decimal ldecDBDCTransferAmount = 0.00m;
            int lintPlanID = abusPersonAccountRetirement.ibusPersonAccount.icdoPersonAccount.plan_id;
            decimal ldecContributionRate = 0.00m;
            DateTime ldtNRD = DateTime.MinValue;
            decimal ldecMemberInterest = 0.00M;
            DateTime ldtLastContributionDate;
            decimal ldecTotalContributionRetirementRate = 1m;
            abusPersonAccountRetirement.ibusPersonAccount.LoadLastContributionDateByRegularPayroll();
            ldtLastContributionDate = abusPersonAccountRetirement.ibusPersonAccount.idtLastContributionDate;
            busPlanRetirementRate lobjPlanRetirement = new busPlanRetirementRate();
            ldecDBAccountBalance = abusPersonAccountRetirement.ibusHb1040Communication.icdoHb1040Communication.db_account_balance;
            ldecDBDCTransferAmount = abusPersonAccountRetirement.ibusHb1040Communication.icdoHb1040Communication.db_dc_transfer_amt;
            ldtNRD = abusPersonAccountRetirement.ibusHb1040Communication.icdoHb1040Communication.nrd;

            ldecContributionRate = lintPlanID == busConstant.PlanIdMain2020 ? busConstant.ContributionRateDC2020 : busConstant.ContributionRateDC;
            ldecTotalContributionRetirementRate = busConstant.ContributionRetirementRate;
            DateTime ldtLastPostedInterestDate = getLastInrestPostingDate(abusPersonAccountRetirement.ibusPersonAccount);

            ldecDBDCTransferAmount = ldecDBDCTransferAmount.RoundToTwoDecimalPoints() > idecDBBeginingBalanceAsOf2025.RoundToTwoDecimalPoints()
                ? ldecDBDCTransferAmount.RoundToTwoDecimalPoints() : idecDBBeginingBalanceAsOf2025.RoundToTwoDecimalPoints();

            ldecCurrentWagesAmount = abusPersonAccountRetirement.ibusPersonAccount.idecLastContributionWages;
            if (ldecCurrentWagesAmount == 0.00m) ldecCurrentWagesAmount = 1;

            if (ldtLastContributionDate.Month < busConstant.AnnualSalaryIncreaseMonth)
                ldecCurrentWagesAmount += (ldecCurrentWagesAmount * busConstant.AnnualSalaryIncreaseRate) / 100;
            ldtLastContributionDate = new DateTime(2025, 01, 01);
            int lintCounter = 0;
            while (ldtLastContributionDate < ldtNRD)
            {
                if (busConstant.AnnualSalaryIncreaseMonth == ldtLastContributionDate.Month && lintCounter > 0)
                {
                    ldecCurrentWagesAmount += ldecCurrentWagesAmount.RoundToTwoDecimalPoints() * (busConstant.AnnualSalaryIncreaseRate / 100);
                }
                ldecContribution = (ldecCurrentWagesAmount.RoundToTwoDecimalPoints() * ((ldecContributionRate.RoundToTwoDecimalPoints()) / 100));

                ldecDBDCTransferAmount += ldecContribution.RoundToTwoDecimalPoints() + (ldecDBDCTransferAmount * ((ldecTotalContributionRetirementRate.RoundToTwoDecimalPoints()) / 100)).RoundToTwoDecimalPoints();
                if (ldtLastContributionDate.Month == 1 && (ldtLastContributionDate.Year == 2026 || ldtLastContributionDate.Year == 2027 || ldtLastContributionDate.Year == 2028))
                {
                    ldecDBDCTransferAmount += busConstant.AnnualSalaryIncentiveAmount;
                }
                if (ldtLastContributionDate.Day == 1)
                    ldtLastContributionDate = ldtLastContributionDate.AddMonths(1);
                else
                    ldtLastContributionDate = ldtLastContributionDate.GetFirstDayofNextMonth();
                ldecDBDCTransferAmount = ldecDBDCTransferAmount.RoundToTwoDecimalPoints();

                lintCounter++;
            }

            ldecProjectedDC_ACCOUNT_BALANCE = ldecDBDCTransferAmount;

            return ldecProjectedDC_ACCOUNT_BALANCE;
        }
        public DateTime getLastInrestPostingDate(busPersonAccount abusPersonAccount)
        {
            DateTime ldtLastPostedInterestDate;
            ldtLastPostedInterestDate = DateTime.MinValue;
            //if (ibusEmployerPayrollHeader.idtLastInterestPostingDate == DateTime.MinValue)
            {
                //ibusEmployerPayrollHeader.LoadLastInterestBatchDate();
                DataTable ldtbEffectiveAndTransDates = busNeoSpinBase.Select("cdoPersonAccountRetirementContribution.GetEffectiveAndTransactionDates",
                              new object[1] { abusPersonAccount.icdoPersonAccount.person_account_id });
                if (ldtbEffectiveAndTransDates.Rows.Count > 0)
                {
                    ldtLastPostedInterestDate = Convert.ToDateTime(ldtbEffectiveAndTransDates.Rows[0]["effective_date"]);
                }
            }
            //DateTime ldtLastPostedInterestDate = ibusEmployerPayrollHeader.idtLastInterestPostingDate;
            //There is a chance for Min Value when we run the system for the First Time.
            //Such cases, get the next run date for Interest Posting batch Date and Substract two months
            if (ldtLastPostedInterestDate == DateTime.MinValue)
            {
                busBatchSchedule lbusBatchSchedule = new busBatchSchedule();
                lbusBatchSchedule.FindBatchSchedule(busConstant.PostingInterestBatchStep);
                ldtLastPostedInterestDate = lbusBatchSchedule.icdoBatchSchedule.next_run_date;
                if (ldtLastPostedInterestDate != DateTime.MinValue)
                {
                    ldtLastPostedInterestDate = ldtLastPostedInterestDate.AddMonths(-2);
                    ldtLastPostedInterestDate = new DateTime(ldtLastPostedInterestDate.Year, ldtLastPostedInterestDate.Month, 1).AddMonths(1).AddDays(-1);
                }
            }
            return ldtLastPostedInterestDate;
        }
        public decimal GetDCTransferFactor(decimal adecAgeAt2025, decimal adecAgeAtNRD)
        {
            busHb1040DcTransferFactors lbusHb1040DcTransferFactors = new busHb1040DcTransferFactors();
            if (lbusHb1040DcTransferFactors.FindDcTransferFactors(adecAgeAt2025.RoundToTwoDecimalPoints(), adecAgeAtNRD.RoundToTwoDecimalPoints()))
            {
                return lbusHb1040DcTransferFactors.icdoHb1040DcTransferFactors.factor;
            }
            return 0.00m;
        }
        public decimal GetLifeExpectancy(decimal adecAgeAtNRD, string astrGender)
        {
            busHb1040LifeExpectancy lbusHb1040LifeExpectancy = new busHb1040LifeExpectancy();
            adecAgeAtNRD = Convert.ToDecimal(adecAgeAtNRD + 0.00m).RoundToTwoDecimalPoints();
            if (lbusHb1040LifeExpectancy.FindLifeExpectancy(adecAgeAtNRD))
            {
                return astrGender == busConstant.GenderTypeMale ? lbusHb1040LifeExpectancy.icdoHb1040LifeExpectancy.male : lbusHb1040LifeExpectancy.icdoHb1040LifeExpectancy.female;
            }
            return 0.00m;
        }
        public decimal GetContributionRate(int aintPlanID)
        {
            decimal ldecContributionRate = 0.00m;
            switch (aintPlanID)
            {
                case busConstant.PlanIdMain:
                    return busConstant.ContributionRateMain;
                case busConstant.PlanIdMain2020:
                    return busConstant.ContributionRateMain2020;
                case busConstant.PlanIdDC:
                    return busConstant.ContributionRateDC;
                case busConstant.PlanIdDC2020:
                    return busConstant.ContributionRateDC2020;
                default:
                    return ldecContributionRate;
            }
        }
        public void SetRetirementPlanRateEEAndER(busPersonAccountRetirement abusPersonAccountRetirement)
        {
            int lintPlanID = abusPersonAccountRetirement.icdoPersonAccount.plan_id;
            abusPersonAccountRetirement.ibusHb1040Communication.idecEEContributionRateDB = busConstant.EEContributionRateDB;
            abusPersonAccountRetirement.ibusHb1040Communication.idecEEContributionRateDC = busConstant.EEContributionRateDC;

            if (lintPlanID == busConstant.PlanIdMain || lintPlanID == busConstant.PlanIdDC)
            {
                abusPersonAccountRetirement.ibusHb1040Communication.idecERContributionRateDB = busConstant.ERContributionRateDB;
                abusPersonAccountRetirement.ibusHb1040Communication.idecERContributionRateDC = busConstant.ERContributionRateDC;
            }
            if (lintPlanID == busConstant.PlanIdMain2020 || lintPlanID == busConstant.PlanIdDC2020)
            {
                abusPersonAccountRetirement.ibusHb1040Communication.idecERContributionRateDB = busConstant.ERContributionRateDB2020;
                abusPersonAccountRetirement.ibusHb1040Communication.idecERContributionRateDC = busConstant.ERContributionRateDC2020;
            }
        }
        private void CreateCorrespondence(busPersonAccountRetirement abusPersonAccountRetirement)
        {
            // Generate Correspondence
            //ArrayList larrlist = new ArrayList();
            //larrlist.Add(abusPerson);
            Hashtable lhstDummyTable = new Hashtable();
            lhstDummyTable.Add("sfwCallingForm", "Batch");
            CreateCorrespondence("ENR-5305", abusPersonAccountRetirement, lhstDummyTable);
        }

    }
}
