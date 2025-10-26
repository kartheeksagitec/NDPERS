#region Using directives
using System;
using System.Data;
using NeoSpin.BusinessObjects;
using System.Collections.ObjectModel;
using Sagitec.BusinessObjects;
using Sagitec.CustomDataObjects;
using NeoSpin.CustomDataObjects;
using System.Linq;

#endregion

namespace NeoSpinBatch
{
    public class busFSAFlexfileout : busNeoSpinBatch
    {
        public Collection<busPersonAccountFlexComp> iclbFSAEligibleMembers { get; set; }

        public void GenerateFSAFileout()
        {
            string lstrSSN = string.Empty;
            try
            {
                DataTable ldtbResults = busBase.Select("cdoPersonAccount.FSAEligibilityFile", new object[] { });
                DataTable ldtbGetDistinctStartDate = busBase.Select("cdoPersonAccount.GetDistinctFSAStartDate", new object[] { });
                foreach (DataRow ldtrRow in ldtbGetDistinctStartDate.Rows)
                {
                    int lintYear = Convert.ToInt32(ldtrRow["START_YEAR"]);
                    DataRow[] ldtrFinal = busGlobalFunctions.FilterTable(ldtbResults, busConstant.DataType.Numeric, "START_YEAR", lintYear);
                    iclbFSAEligibleMembers = new Collection<busPersonAccountFlexComp>();
                    foreach (DataRow ldtr in ldtrFinal)
                    {
                        busPersonAccountFlexComp lobjFlexComp = new busPersonAccountFlexComp
                        {
                            icdoPersonAccount = new cdoPersonAccount(),
                            icdoPersonAccountFlexComp = new cdoPersonAccountFlexComp(),
                            ibusPerson = new busPerson { icdoPerson = new cdoPerson() },
                            ibusPersonLatestEmployment = new busPersonEmployment { icdoPersonEmployment = new cdoPersonEmployment() },
                            ibusMSRACoverage = new busPersonAccountFlexCompOption { icdoPersonAccountFlexCompOption = new cdoPersonAccountFlexCompOption() },
                            ibusDCRACoverage = new busPersonAccountFlexCompOption { icdoPersonAccountFlexCompOption = new cdoPersonAccountFlexCompOption() }
                        };
                        lobjFlexComp.icdoPersonAccount.LoadData(ldtr);
                        lobjFlexComp.icdoPersonAccountFlexComp.LoadData(ldtr);
                        lobjFlexComp.ibusPerson.icdoPerson.LoadData(ldtr);
                        lstrSSN = lobjFlexComp.ibusPerson.icdoPerson.ssn;

                        lobjFlexComp.LoadPersonAccountEmpDetailByPlan(); // PIR 12021
                        
                        lobjFlexComp.LoadFlexCompHistory();
                        var lclbMSRACurrentYear = lobjFlexComp.iclbFlexCompHistory.Where(i =>
                                                            i.icdoPersonAccountFlexCompHistory.effective_start_date.Year == lobjFlexComp.icdoPersonAccountFlexComp.start_year && // PIR 12021 - get current year's history only
                                                            i.icdoPersonAccountFlexCompHistory.level_of_coverage_value == busConstant.FlexLevelOfCoverageMedicareSpending &&
                                                            i.icdoPersonAccountFlexCompHistory.effective_start_date != i.icdoPersonAccountFlexCompHistory.effective_end_date &&
                                                            i.icdoPersonAccountFlexCompHistory.plan_participation_status_value == busConstant.PlanParticipationStatusFlexCompEnrolled).
                                                            OrderByDescending(i=>i.icdoPersonAccountFlexCompHistory.effective_start_date);
                        if (lclbMSRACurrentYear.IsNotNull())
                        {
                            var lobjRecentMSRAAmount = lclbMSRACurrentYear.Where(i => i.icdoPersonAccountFlexCompHistory.annual_pledge_amount > 0M).FirstOrDefault();
                            if (lobjRecentMSRAAmount.IsNotNull())
                                lobjFlexComp.ibusMSRACoverage.icdoPersonAccountFlexCompOption.annual_pledge_amount = lobjRecentMSRAAmount.icdoPersonAccountFlexCompHistory.annual_pledge_amount;

                            var lobjInitialStartDate = lclbMSRACurrentYear.Where(i => i.icdoPersonAccountFlexCompHistory.annual_pledge_amount > 0M).LastOrDefault();
                            if (lobjInitialStartDate.IsNotNull())
                                lobjFlexComp.ibusMSRACoverage.icdoPersonAccountFlexCompOption.effective_start_date = lobjInitialStartDate.icdoPersonAccountFlexCompHistory.effective_start_date;

                            var lobjRecentEndDate = lclbMSRACurrentYear.Where(i => i.icdoPersonAccountFlexCompHistory.effective_end_date != DateTime.MinValue &&
                                                                                   i.icdoPersonAccountFlexCompHistory.annual_pledge_amount > 0M).FirstOrDefault();

                            if (((lobjRecentEndDate.IsNotNull() && (!lclbMSRACurrentYear.Where(o => o.icdoPersonAccountFlexCompHistory.effective_end_date == DateTime.MinValue
                                                              && o.icdoPersonAccountFlexCompHistory.annual_pledge_amount > 0M).Any()))
                                                              || (lobjRecentEndDate.IsNotNull() && lobjFlexComp.iclbPersonAccountEmpDetailByPlan.Count == 0)) // PIR 12021
                                                              && (lobjFlexComp.icdoPersonAccountFlexComp.flex_comp_type_value != "CBRA")) //PIR 13401 - Term date should not be shown for COBRA participants.
                                lobjFlexComp.ibusMSRACoverage.icdoPersonAccountFlexCompOption.effective_end_date = lobjRecentEndDate.icdoPersonAccountFlexCompHistory.effective_end_date;
                        }

                        var lclbDCRACurrentYear = lobjFlexComp.iclbFlexCompHistory.Where(i =>
                                                            i.icdoPersonAccountFlexCompHistory.effective_start_date.Year == lobjFlexComp.icdoPersonAccountFlexComp.start_year && // PIR 12021 - get current year's history only
                                                            i.icdoPersonAccountFlexCompHistory.level_of_coverage_value == busConstant.FlexLevelOfCoverageDependentSpending &&
                                                            i.icdoPersonAccountFlexCompHistory.effective_start_date != i.icdoPersonAccountFlexCompHistory.effective_end_date &&
                                                            i.icdoPersonAccountFlexCompHistory.plan_participation_status_value == busConstant.PlanParticipationStatusFlexCompEnrolled).
                                                            OrderByDescending(i => i.icdoPersonAccountFlexCompHistory.effective_start_date);
                        if (lclbDCRACurrentYear.IsNotNull())
                        {
                            var lobjRecentDCRAAmount = lclbDCRACurrentYear.Where(i => i.icdoPersonAccountFlexCompHistory.annual_pledge_amount > 0M).FirstOrDefault();
                            if (lobjRecentDCRAAmount.IsNotNull())
                                lobjFlexComp.ibusDCRACoverage.icdoPersonAccountFlexCompOption.annual_pledge_amount = lobjRecentDCRAAmount.icdoPersonAccountFlexCompHistory.annual_pledge_amount;

                            var lobjInitialStartDate = lclbDCRACurrentYear.Where(i => i.icdoPersonAccountFlexCompHistory.annual_pledge_amount > 0M).LastOrDefault();
                            if (lobjInitialStartDate.IsNotNull())
                                lobjFlexComp.ibusDCRACoverage.icdoPersonAccountFlexCompOption.effective_start_date = lobjInitialStartDate.icdoPersonAccountFlexCompHistory.effective_start_date;

                            var lobjRecentEndDate = lclbDCRACurrentYear.Where(i => i.icdoPersonAccountFlexCompHistory.effective_end_date != DateTime.MinValue &&
                                                                                   i.icdoPersonAccountFlexCompHistory.annual_pledge_amount > 0M).FirstOrDefault();

                            if ((lobjRecentEndDate.IsNotNull() && (!lclbDCRACurrentYear.Where(o => o.icdoPersonAccountFlexCompHistory.effective_end_date == DateTime.MinValue
                                                              && o.icdoPersonAccountFlexCompHistory.annual_pledge_amount > 0M).Any()))
                                                              || (lobjRecentEndDate.IsNotNull() && lobjFlexComp.iclbPersonAccountEmpDetailByPlan.Count == 0)) // PIR 12021
                                lobjFlexComp.ibusDCRACoverage.icdoPersonAccountFlexCompOption.effective_end_date = lobjRecentEndDate.icdoPersonAccountFlexCompHistory.effective_end_date;
                        }

                        if (lobjFlexComp.ibusMSRACoverage.icdoPersonAccountFlexCompOption.annual_pledge_amount > 0 ||
                            lobjFlexComp.ibusDCRACoverage.icdoPersonAccountFlexCompOption.annual_pledge_amount > 0)
                        {

                            lobjFlexComp.ibusPerson.GetPersonLatestAddress(); //PIR 11867
                            lobjFlexComp.LoadLatestEmployment();

                            if (lobjFlexComp.icdoPersonAccountFlexComp.flex_comp_type_value == busConstant.FlexCompTypeValueCOBRA)
                                lobjFlexComp.ibusPersonLatestEmployment.icdoPersonEmployment.end_date = DateTime.MinValue; // PIR 10792 5)

                            // Get minimum of MSRA/DCRA start date
                            DateTime ldteHDVStartdate = new DateTime();
                            if (lobjFlexComp.ibusMSRACoverage.icdoPersonAccountFlexCompOption.effective_start_date != DateTime.MinValue ||
                                lobjFlexComp.ibusDCRACoverage.icdoPersonAccountFlexCompOption.effective_start_date != DateTime.MinValue)
                            {
                                ldteHDVStartdate = busGlobalFunctions.GetMin(
                                    ((lobjFlexComp.ibusMSRACoverage.icdoPersonAccountFlexCompOption.effective_start_date == DateTime.MinValue) ?
                                    DateTime.MaxValue : lobjFlexComp.ibusMSRACoverage.icdoPersonAccountFlexCompOption.effective_start_date),
                                    ((lobjFlexComp.ibusDCRACoverage.icdoPersonAccountFlexCompOption.effective_start_date == DateTime.MinValue) ?
                                    DateTime.MaxValue : lobjFlexComp.ibusDCRACoverage.icdoPersonAccountFlexCompOption.effective_start_date));
                            }

                            // Get minimum of MSRA/DCRA end date
                            DateTime ldteHDVEnddate = new DateTime();
                            if (lobjFlexComp.ibusPersonLatestEmployment.icdoPersonEmployment.end_date != DateTime.MinValue &&
                                (lobjFlexComp.ibusMSRACoverage.icdoPersonAccountFlexCompOption.effective_end_date != DateTime.MinValue ||
                                lobjFlexComp.ibusDCRACoverage.icdoPersonAccountFlexCompOption.effective_end_date != DateTime.MinValue))
                            {
                                ldteHDVEnddate = busGlobalFunctions.GetMin(
                                    ((lobjFlexComp.ibusMSRACoverage.icdoPersonAccountFlexCompOption.effective_end_date == DateTime.MinValue) ?
                                    DateTime.MaxValue : lobjFlexComp.ibusMSRACoverage.icdoPersonAccountFlexCompOption.effective_end_date),
                                    ((lobjFlexComp.ibusDCRACoverage.icdoPersonAccountFlexCompOption.effective_end_date == DateTime.MinValue) ?
                                    DateTime.MaxValue : lobjFlexComp.ibusDCRACoverage.icdoPersonAccountFlexCompOption.effective_end_date));
                            }

                            lobjFlexComp.LoadHDVEnrollmentDates(ldteHDVStartdate, ldteHDVEnddate);

                            if (lobjFlexComp.iclbPreviousHistory != null)
                            {
                                lobjFlexComp.iclbPreviousHistory.Clear();
                                lobjFlexComp.iclbPreviousHistory = null;
                            }
                            if (lobjFlexComp.iclbAccountEmploymentDetail != null)
                            {
                                lobjFlexComp.iclbAccountEmploymentDetail.Clear();
                                lobjFlexComp.iclbAccountEmploymentDetail = null;
                            }
                            if (lobjFlexComp.ibusPerson.icolPersonAccount != null)
                            {
                                lobjFlexComp.ibusPerson.icolPersonAccount.Clear();
                                lobjFlexComp.ibusPerson.icolPersonAccount = null;
                            }

                            iclbFSAEligibleMembers.Add(lobjFlexComp);
                        }
                    }

                    if (iclbFSAEligibleMembers.Count > 0)
                    {
                        busProcessOutboundFile lobjProcessFiles = new busProcessOutboundFile();
                        lobjProcessFiles.iobjSystemManagement = iobjSystemManagement;
                        idlgUpdateProcessLog("Creating FSA Eligibility file for the year " + lintYear.ToString(), "INFO", "FSA Eligibility File");
                        lobjProcessFiles.iarrParameters = new object[1];
                        lobjProcessFiles.iarrParameters[0] = iclbFSAEligibleMembers;
                        lobjProcessFiles.CreateOutboundFile(95);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + lstrSSN);
            }
        }
    }
}
