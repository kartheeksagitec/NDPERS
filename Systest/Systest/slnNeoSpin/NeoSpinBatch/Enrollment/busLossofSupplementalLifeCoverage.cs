#region Using directives
using System;
using System.Data;
using System.Collections.Generic;
using System.Diagnostics;
using NeoSpin.CustomDataObjects;
using NeoSpin.BusinessObjects;
using System.Collections.ObjectModel;
using Sagitec.DataObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.CustomDataObjects;
using Sagitec.BusinessObjects;
using System.Collections;
using Sagitec.CorBuilder;

#endregion

namespace NeoSpinBatch
{
    class busLossofSupplementalLifeCoverage : busNeoSpinBatch
    {
        public DateTime idtEffectiveDate = new DateTime();

        private Collection<busPersonAccountLife> _iclbMember;
        public Collection<busPersonAccountLife> iclbMember
        {
            get { return _iclbMember; }
            set { _iclbMember = value; }
        }       

        public void CreateLifeCoverageCorrespondence()
        {
            _iclbMember = new Collection<busPersonAccountLife>();
            istrProcessName = "Loss of Supplemental Life Coverage";
            idlgUpdateProcessLog("Creating Correspondence for Persons losing Supplemental Life Coverage", "INFO", istrProcessName);

            DataTable ldtbDepenents = DBFunction.DBSelect("cdoPersonAccountLife.LossofSupplementalCoverageLetter", new object[0] {  },
                            iobjPassInfo.iconFramework,
                            iobjPassInfo.itrnFramework); 
            foreach (DataRow dr in ldtbDepenents.Rows)
            {
                busPersonAccountLife lobjLife = new busPersonAccountLife();
                lobjLife.icdoPersonAccount = new cdoPersonAccount();
                lobjLife.icdoPersonAccountLife = new cdoPersonAccountLife();
                lobjLife.ibusPerson = new busPerson();
                lobjLife.ibusPerson.icdoPerson = new cdoPerson();
                lobjLife.iclbLifeOptionForCorrespondence = new Collection<busPersonAccountLifeOption>(); 
                lobjLife.icdoPersonAccountLife.LoadData(dr);
                lobjLife.icdoPersonAccount.LoadData(dr);
                lobjLife.ibusPerson.icdoPerson.LoadData(dr);
                lobjLife.LoadLifeOptionData();
                lobjLife.LoadPersonAccountLifeOptions();
                lobjLife.LoadMemberAge();                
                lobjLife.LoadPlanEffectiveDate();
                lobjLife.icdoPersonAccount.person_employment_dtl_id = lobjLife.GetEmploymentDetailID();
                lobjLife.iblnIscalledFromLossOfSuppLifeBatch = true;
                if (lobjLife.icdoPersonAccount.person_employment_dtl_id != 0)
                {
                    lobjLife.LoadPersonEmploymentDetail();
                    lobjLife.ibusPersonEmploymentDetail.LoadPersonEmployment(); 
                    lobjLife.LoadOrgPlan();
                    lobjLife.LoadProviderOrgPlan();
                }
                else
                {
                    lobjLife.LoadActiveProviderOrgPlan(lobjLife.icdoPersonAccount.current_plan_start_date_no_null);
                }
               

                // Load effective end date
                int lintTempYear = lobjLife.ibusPerson.icdoPerson.date_of_birth.AddMonths(779).Year;
                int lintTempMonth = lobjLife.ibusPerson.icdoPerson.date_of_birth.AddMonths(779).Month;
                int lintTempDays = DateTime.DaysInMonth(lintTempYear, lintTempMonth);
                idtEffectiveDate = new DateTime(lintTempYear, lintTempMonth, lintTempDays);
                //BR-022-214

                //PIR-8932.. Changed the History change date to Firstday of the month
                lobjLife.ldtEffectiveEndDate = idtEffectiveDate.AddDays(1);
                lobjLife.icdoPersonAccount.history_change_date = idtEffectiveDate.AddDays(1);

                //Checks the Overlapping History 
                lobjLife.iclbOverlappingHistory = new Collection<busPersonAccountLifeHistory>();
                if (lobjLife.icdoPersonAccount.history_change_date != DateTime.MinValue)//((lobjLife.istrAllowOverlapHistory == busConstant.Flag_Yes) && (icdoPersonAccount.history_change_date != DateTime.MinValue))
                {
                    //Reload the History Always...
                    lobjLife.LoadHistory();
                    foreach (busPersonAccountLifeOption lobjLifeOption in lobjLife.iclbPersonAccountLifeOption)
                    {
                        lobjLifeOption.iblnOverlapHistoryFound = false;
                        Collection<busPersonAccountLifeHistory> lclbOpenHistory = lobjLife.LoadOverlappingHistoryForOption(lobjLifeOption);
                        if (lclbOpenHistory.Count > 0)
                        {
                            foreach (busPersonAccountLifeHistory lbusPALifeHistory in lclbOpenHistory)
                            {
                                lobjLife.iclbPersonAccountLifeHistory.Remove(lbusPALifeHistory);
                                lobjLife.iclbOverlappingHistory.Add(lbusPALifeHistory);
                                lobjLifeOption.iblnOverlapHistoryFound = true;
                                //lblnReloadPreviousHistory = true;
                            }
                        }
                    }
                }

                
                foreach (busPersonAccountLifeOption lobjPAOption in lobjLife.iclbLifeOption)
                {
                    lobjPAOption.iintCoverageAmountForCorr = lobjPAOption.icdoPersonAccountLifeOption.coverage_amount;
                    //PIR 24076
                    if (lobjPAOption.icdoPersonAccountLifeOption.level_of_coverage_value != busConstant.LevelofCoverage_Basic)
                    {
                        lobjPAOption.icdoPersonAccountLifeOption.plan_option_status_value = busConstant.PlanOptionStatusValueWaived;
                        lobjPAOption.icdoPersonAccountLifeOption.coverage_amount = 0.0m;

                        if ((lobjPAOption.icdoPersonAccountLifeOption.effective_start_date != DateTime.MinValue) &&
                            (lobjPAOption.icdoPersonAccountLifeOption.effective_end_date == DateTime.MinValue))
                        {
                            lobjPAOption.icdoPersonAccountLifeOption.effective_end_date = idtEffectiveDate;
                            lobjLife.icdoPersonAccountLife.IsEndDatedDueToLossOfSuppLife = busConstant.Flag_Yes;
                        }
                        lobjPAOption.icdoPersonAccountLifeOption.Update();
                    }

                    //PIR-8932
                    // Monthly premium Amount Calculation
                    //If the Effective End date is Null 
                    //then calculate the Monthly premium amount considering Current Date.
                    //Else 
                    //Calculate the Monthly premium Amount considering Effective end date

                    if (lobjPAOption.icdoPersonAccountLifeOption.effective_end_date == DateTime.MinValue)
                        lobjLife.GetMonthlyPremiumAmount(DateTime.Now);
                    else
                        lobjLife.GetMonthlyPremiumAmount(lobjPAOption.icdoPersonAccountLifeOption.effective_end_date);

                    // Need to check in after confirmation with Maik
                    if ((lobjPAOption.icdoPersonAccountLifeOption.effective_end_date >= DateTime.Now && lobjPAOption.icdoPersonAccountLifeOption.level_of_coverage_value != busConstant.LevelofCoverage_Basic) ||
                       (lobjPAOption.icdoPersonAccountLifeOption.effective_end_date == DateTime.MinValue && lobjPAOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_Basic))
                        lobjLife.iclbLifeOptionForCorrespondence.Add(lobjPAOption);
                }
                //lobjLife.GetMonthlyPremiumAmount();

                if (lobjLife.iclbPreviousHistory == null)
                    lobjLife.LoadPreviousHistory();

                lobjLife.SetHistoryEntryRequiredOrNot();
                lobjLife.icdoPersonAccount.status_value = busConstant.PersonAccountStatusValid;
                lobjLife.ProcessHistory();
                lobjLife.icdoPersonAccount.Update();
                //lobjLife.GetMonthlyPremiumAmount(lobjLife.icdoPersonAccount.current_plan_start_date);

                //PIR ID 848
                foreach (busPersonAccountLifeOption lobjPALifeOption in lobjLife.iclbPersonAccountLifeOption)                
                    lobjPALifeOption.icdoPersonAccountLifeOption.coverage_amount = Math.Round(lobjPALifeOption.icdoPersonAccountLifeOption.coverage_amount, 2);
                
                //UAT PIR - 983 - set the effective date as the Life option end date + 31 days
                lobjLife.ldtEffectiveEndDate = idtEffectiveDate.AddDays(31);

                //ArrayList larrList = new ArrayList();
                //larrList.Add(lobjLife);
                Hashtable lshtTemp = new Hashtable();
                lshtTemp.Add("FormTable", "Batch");

                string lstrFileName=CreateCorrespondence("PER-0151", lobjLife, lshtTemp);                
                CreateContactTicket(lobjLife.ibusPerson.icdoPerson.person_id);
                _iclbMember.Add(lobjLife);
            }
            idlgUpdateProcessLog("Correspondence created successfully", "INFO", istrProcessName);
        }

        // Create Contact Ticket
        private void CreateContactTicket(int aintPersonID)
        {
            cdoContactTicket lobjContactTicket = new cdoContactTicket();
            CreateContactTicket(aintPersonID, busConstant.ContactTicketTypeInsuranceRetiree, lobjContactTicket);
        }        
    }
}
