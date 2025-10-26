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
using System.Linq.Expressions;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPensionFile : busExtendBase
    {
        public cdoActuaryFilePensionDetail icdoActuaryFilePensionDetail { get; set; }
        public cdoActuaryFileRhicDetail icdoActuaryFileRhicDetail { get; set; }
        public busPerson ibusMember { get; set; }

        public busPerson ibusJointAnnuitant { get; set; }
        public void LoadPerson()
        {
            if (ibusJointAnnuitant == null)
                ibusJointAnnuitant = new busPerson();
        }
        public void LoadMember()
        {
            if (ibusMember == null)
                ibusMember = new busPerson();
            ibusMember.FindPerson(ibusPersonAccount.icdoPersonAccount.person_id);
        }
        public decimal getmemberage()
        {
            decimal ldecMemberAge = 0M;
            if (ibusMember == null)
                LoadMember();
            return CalculatePersonAge(ibusMember.icdoPerson.date_of_birth, DateTime.Today, ref ldecMemberAge, 4);
        }
        public int CalculatePersonAge(DateTime adtePersonDOB, DateTime adteDateToCompare, ref decimal adecMonthAndYear, int aintDecimallength)
        {
            int lintTotalMonths = 0;
            int lintMemberAgeYearPart = 0;
            if ((adtePersonDOB != DateTime.MinValue) && (adteDateToCompare != DateTime.MinValue))
            {
                DateTime ldteFromDate = adtePersonDOB.AddMonths(1);
                int lintMemberAgeMonthPart = 0;

                busPersonBase.CalculateAge(ldteFromDate, adteDateToCompare, ref lintTotalMonths, ref adecMonthAndYear,
                    aintDecimallength, ref lintMemberAgeYearPart, ref lintMemberAgeMonthPart);
            }
            return lintMemberAgeYearPart;
        }
        public busPlan ibusPlan { get; set; }
        public void LoadPlan()
        {
            if (ibusPlan == null)
                ibusPlan = new busPlan();
            ibusPlan.FindPlan(ibusPersonAccount.icdoPersonAccount.plan_id);
        }
        public busPersonAccount ibusPersonAccount { get; set; }

        public void LoadPersonAccount()
        {
            if (ibusPersonAccount == null)
                ibusPersonAccount = new busPersonAccount();
            ibusPersonAccount.FindPersonAccount(icdoActuaryFilePensionDetail.person_account_id);
        }
        public busPayeeAccount ibusPayeeAccount { get; set; }
        public void LoadPayeeAccount()
        {
            if (ibusPayeeAccount == null)
                ibusPayeeAccount = new busPayeeAccount();
            ibusPayeeAccount.FindPayeeAccount(icdoActuaryFilePensionDetail.payee_account_id);
        }

        public decimal GetAccruedBenefitAmount(DateTime adtEffectiveDate, DataRow adrRow, busDBCacheData abusDBCacheData,
            DataTable adtbBenProvisionBenType, DataTable adtbBenOptionFactor, DataTable adtbEligibilityForNormal)
        {
            if (ibusPersonAccount == null)
                ibusPersonAccount = new busPersonAccount() { icdoPersonAccount = new cdoPersonAccount() };
            ibusPersonAccount.icdoPersonAccount.LoadData(adrRow);

            if (ibusMember == null)
                ibusMember = new busPerson() { icdoPerson = new cdoPerson() };
            ibusMember.icdoPerson.LoadData(adrRow);

            if (ibusPlan == null)
                ibusPlan = new busPlan() { icdoPlan = new cdoPlan() };
            ibusPlan.icdoPlan.LoadData(adrRow);

            if (ibusPayeeAccount == null)
                ibusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
            ibusPayeeAccount.icdoPayeeAccount.LoadData(adrRow);

            if (ibusPayeeAccount.ibusApplication == null)
                ibusPayeeAccount.ibusApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
            ibusPayeeAccount.ibusApplication.icdoBenefitApplication.LoadData(adrRow);

            if (ibusPayeeAccount.ibusApplication.icdoBenefitApplication.joint_annuitant_perslink_id > 0 &&
                ibusPayeeAccount.ibusJointAnnuitant == null)
                ibusPayeeAccount.LoadJointAnnuitant();

            //PIR 16078 Accrued benefit amount calculated as per MAS logic
            Collection<cdoBenefitProvisionEligibility> lclbBenefitProvisionNormalEligibility = new Collection<cdoBenefitProvisionEligibility>();
            lclbBenefitProvisionNormalEligibility = busPersonBase.LoadEligibilityForPlan(ibusPlan.icdoPlan.plan_id,
                                            ibusPlan.icdoPlan.benefit_provision_id,
                                            ibusPlan.icdoPlan.benefit_type_value,
                                            busConstant.BenefitProvisionEligibilityNormal, iobjPassInfo, ibusPersonAccount?.icdoPersonAccount?.start_date, abusDBCacheData);

            busRetirementBenefitCalculation lobjRetirementCalculation = new busRetirementBenefitCalculation
            {
                icdoBenefitCalculation = new cdoBenefitCalculation(),
                ibusMember = new busPerson { icdoPerson = new cdoPerson() },
                ibusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount(), ibusPlan = new busPlan { icdoPlan = new cdoPlan() } },
                ibusPlan = new busPlan { icdoPlan = new cdoPlan() },
                ibusJointAnnuitant = new busPerson { icdoPerson = new cdoPerson() }
            };

            lobjRetirementCalculation.ibusMember = ibusMember;
            lobjRetirementCalculation.ibusPersonAccount = ibusPersonAccount;
            lobjRetirementCalculation.ibusPlan = ibusPlan;
            lobjRetirementCalculation.ibusPersonAccount.ibusPlan = ibusPlan;
            lobjRetirementCalculation.ibusPersonAccount.ibusPerson = ibusMember;
            lobjRetirementCalculation.ibusJointAnnuitant = ibusPayeeAccount.ibusJointAnnuitant;
            lobjRetirementCalculation.idtbBenOptionFactor = adtbBenOptionFactor;
            lobjRetirementCalculation.iblnUseDataTableForBenOptionFactor = true;

            lobjRetirementCalculation.icdoBenefitCalculation.plan_id = lobjRetirementCalculation.ibusPlan.icdoPlan.plan_id;
            lobjRetirementCalculation.icdoBenefitCalculation.person_id = lobjRetirementCalculation.ibusMember.icdoPerson.person_id;
            lobjRetirementCalculation.icdoBenefitCalculation.rhic_option_value = busConstant.RHICOptionStandard;
            lobjRetirementCalculation.icdoBenefitCalculation.created_date = adtEffectiveDate;

            //code to get eligible age for Normal retirement
            int lintEligibleAge = adtbEligibilityForNormal.AsEnumerable()
                                            .Where(o => o.Field<int>("benefit_provision_id") == ibusPlan.icdoPlan.benefit_provision_id)
                                            .Select(o => o.Field<int>("age"))
                                            .FirstOrDefault();
            DateTime ldtTerminationDate = ibusMember.icdoPerson.date_of_birth.AddYears(lintEligibleAge).GetLastDayofMonth();

            busPersonAccountRetirement lobjPARetirement = new busPersonAccountRetirement();
            lobjPARetirement.FindPersonAccount(ibusPersonAccount.icdoPersonAccount.person_account_id);
            lobjPARetirement.FindPersonAccountRetirement(ibusPersonAccount.icdoPersonAccount.person_account_id);
            lobjPARetirement.LoadPerson();
            lobjPARetirement.LoadPlan();
            lobjPARetirement.LoadPersonAccount();
            lobjPARetirement.LoadPersonAccountForRetirementPlan();
            lobjPARetirement.LoadRetirementContributionAll();
            lobjPARetirement.LoadNewRetirementBenefitCalculation();
            lobjPARetirement.AssignRetirementBenefitCalculationFields(busConstant.CalculationTypeFinal);

            lobjRetirementCalculation.icdoBenefitCalculation.termination_date = lobjPARetirement.ibusRetirementBenefitCalculation.icdoBenefitCalculation.termination_date;
            lobjRetirementCalculation.icdoBenefitCalculation.retirement_date = lobjPARetirement.ibusRetirementBenefitCalculation.icdoBenefitCalculation.retirement_date;

            lobjRetirementCalculation.icdoBenefitCalculation.benefit_account_type_value = busConstant.ApplicationBenefitTypeRetirement;
            
            lobjRetirementCalculation.icdoBenefitCalculation.calculation_type_value = busConstant.CalculationTypeFinal;
            lobjRetirementCalculation.CalculateMemberAge();
            lobjRetirementCalculation.LoadBenefitCalculationPayeeForNewMode();
            lobjRetirementCalculation.LoadBenefitProvisionBenefitType(adtbBenProvisionBenType);
            lobjRetirementCalculation.ibusPersonAccount.LoadTotalVSC();
            if (lobjRetirementCalculation.icdoBenefitCalculation.plan_id == busConstant.PlanIdDC ||
                lobjRetirementCalculation.icdoBenefitCalculation.plan_id == busConstant.PlanIdDC2020 || //PIR 20232
                lobjRetirementCalculation.icdoBenefitCalculation.plan_id == busConstant.PlanIdDC2025) //PIR 25920
            {
                // DC RHIC amount is only needed in report.
                lobjRetirementCalculation.icdoBenefitCalculation.benefit_account_type_value = busConstant.ApplicationBenefitTypeRetirement;
                if (!lobjRetirementCalculation.iblnConsolidatedPSCLoaded)
                    lobjRetirementCalculation.CalculateConsolidatedPSC();
                decimal ldecRHICAmount = Math.Round(lobjRetirementCalculation.icdoBenefitCalculation.consolidated_psc_in_years, 4, MidpointRounding.AwayFromZero) *
                                        lobjRetirementCalculation.ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.rhic_service_factor;
                lobjRetirementCalculation.icdoBenefitCalculation.unreduced_rhic_amount = Math.Round(ldecRHICAmount, 2, MidpointRounding.AwayFromZero);

            }
            else
            {
                /// Calculates and Inserts for the Retirement
                lobjRetirementCalculation.icdoBenefitCalculation.benefit_account_type_value = busConstant.ApplicationBenefitTypeRetirement;
                if (!lobjRetirementCalculation.iblnConsoldatedVSCLoaded)
                    lobjRetirementCalculation.CalculateConsolidatedVSC(true);//PIR 17486 - Issue 1
                if (lobjRetirementCalculation.CheckPersonEligible(abusDBCacheData))
                {
                    lobjRetirementCalculation.CalculateFAS();
                    lobjRetirementCalculation.CalculateUnRedecuedBenefitAmountForPensionFileBatch(abusDBCacheData);
                }
            }
            return lobjRetirementCalculation.icdoBenefitCalculation.unreduced_benefit_amount;

        }
    }
}