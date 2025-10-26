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
using Sagitec.CustomDataObjects;
using System.Globalization;
using Sagitec.DataObjects;
using System.Collections.Generic;
using NeoSpin.DataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPersonAccountFlexComp : busPersonAccountFlexCompGen
    {
        private Collection<busPersonAccountFlexCompOption> _iclbFlexCompOption;
        public Collection<busPersonAccountFlexCompOption> iclbFlexCompOption
        {
            get { return _iclbFlexCompOption; }
            set { _iclbFlexCompOption = value; }
        }

        private Collection<busPersonAccountFlexCompHistory> _iclbPreviousHistory;
        public Collection<busPersonAccountFlexCompHistory> iclbPreviousHistory
        {
            get { return _iclbPreviousHistory; }
            set { _iclbPreviousHistory = value; }
        }

        private Collection<busPersonAccountFlexCompOption> _iclbFlexCompOptionModified;
        public Collection<busPersonAccountFlexCompOption> iclbFlexCompOptionModified
        {
            get { return _iclbFlexCompOptionModified; }
            set { _iclbFlexCompOptionModified = value; }
        }

        // PIR 9115
        public bool iblnIsPreviousHistoryFlagUpdated { get; set; }

        // PIR 9115 functionality enable/disable property
        public string istrIsPIR9115Enabled
        {
            get
            {
                return busGlobalFunctions.GetData1ByCodeValue(52, "9115", iobjPassInfo);
            }
        }

        public bool iblnIsFromMSS { get; set; }//pir 6407
        public busPersonAccountFlexCompOption ibusMSRACoverage { get; set; } // pir 7705
        public busPersonAccountFlexCompOption ibusDCRACoverage { get; set; } // pir 7705
        public busPersonAccountFlexCompHistory ibusHistory { get; set; }// pir 7705
        public Collection<busPersonAccountFlexCompHistory> iclbModifiedHistory { get; set; }// pir 7705
        //pir 7987
        public bool iblnEndFlexCompEnrollmentBatch { get; set; }

        public Collection<busPersonAccountFlexCompHistory> iclbOverlappingHistory { get; set; }
        public string istrAllowOverlapHistory { get; set; }
        public void LoadFlexCompOptionNew()
        {
            DataTable ldtbltc = busNeoSpinBase.Select("cdoPersonAccountFlexCompOption.LoadFlexCompOptionNew",
                                                      new object[1] { icdoPersonAccount.person_account_id });
            _iclbFlexCompOption = GetCollection<busPersonAccountFlexCompOption>(ldtbltc,
                                                                                "icdoPersonAccountFlexCompOption");
        }

       
        public void LoadFlexCompOptionUpdate()
        {
            DataTable ldtbltc = busNeoSpinBase.Select("cdoPersonAccountFlexCompOption.LoadFlexOptionUpdate",
                                                      new object[1] { icdoPersonAccount.person_account_id });
            _iclbFlexCompOption = GetCollection<busPersonAccountFlexCompOption>(ldtbltc,
                                                                                "icdoPersonAccountFlexCompOption");
        }

        //Start Date for the Selected Level of coverage must be first of month after Employer joined the plan
        public bool IsEffectiveStartDateValidComparingToOrgPlan()
        {
            if (icdoPersonAccount.person_employment_dtl_id != 0)
            {
                foreach (busPersonAccountFlexCompOption lobjFlexOption in _iclbFlexCompOption)
                {
                    if (lobjFlexOption.icdoPersonAccountFlexCompOption.effective_start_date != DateTime.MinValue)
                    {
                        DateTime ldtOrgPlanStartDate = ibusOrgPlan.icdoOrgPlan.participation_start_date;
                        DateTime ldtEffectiveDate = new DateTime(ldtOrgPlanStartDate.AddMonths(1).Year, ldtOrgPlanStartDate.AddMonths(1).Month, 1);
                        if (lobjFlexOption.icdoPersonAccountFlexCompOption.effective_start_date < ldtEffectiveDate)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public bool CheckDependentSpendingAccountIsNotPermittedforCOBRA()
        {
            if (icdoPersonAccountFlexComp.flex_comp_type_value == busConstant.FlexCompTypeValueCOBRA)
            {
                foreach (busPersonAccountFlexCompOption lobjFlexOption in _iclbFlexCompOption)
                {
                    if ((lobjFlexOption.icdoPersonAccountFlexCompOption.annual_pledge_amount > 0.0M) &&
                        (lobjFlexOption.icdoPersonAccountFlexCompOption.level_of_coverage_value == busConstant.FlexLevelOfCoverageDependentSpending))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        // pir 7705
        public bool CheckAtleastLevelOfCovergeSelected()
        {
            decimal ldecPledgeAmount = 0;
            foreach (busPersonAccountFlexCompOption lobjFlexOption in _iclbFlexCompOption)
            {
                ldecPledgeAmount = lobjFlexOption.icdoPersonAccountFlexCompOption.annual_pledge_amount;
                if (ldecPledgeAmount != 0)
                {
                    break;
                }
            }
            // In New Mode, History change date or any of the pledge amount
            if ((ldecPledgeAmount == 0) &&
                (icdoPersonAccount.history_change_date == DateTime.MinValue))
            {
                return false;
            }
            return true;
        }

        public bool IsDependentOrMedicareEnrollmentDateExceed60Days()
        {
            if (icdoPersonAccount.person_employment_dtl_id != 0)
            {
                if (icdoPersonAccount.history_change_date > ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.start_date.AddDays(60))
                {
                    return true;
                }
            }
            return false;
        }

        // PIR 17472 - Suppressible warning on Internal side for flex enrollment within 31 days
        public bool IsFlexCompEnrollmentDateExceed31Days()
        {
            if (iobjPassInfo.istrFormName.IsNotNull() && iobjPassInfo.istrFormName == "wfmFlexCompMaintenance" )
            {
                DateTime ldtdateofchange;
                if (icdoPersonAccountFlexComp.reason_value == busConstant.ReasonValueNewHire)
                {
                    ldtdateofchange = ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.start_date;
                }
                else
                {
                    ldtdateofchange = ibusPersonAccount.icdoPersonAccount.history_change_date;
                }

                if (icdoPersonAccount.person_employment_dtl_id != 0)
                {
                    if (DateTime.Today > ldtdateofchange.AddDays(31))
                    {
                        return true;
                    }
                }
                return false;
            }
            return false;
        }
             
        // BR-022-58 && BR-022-59
        public bool IsAnnualPledgeAmountExceedTheLimit()
        {
            foreach (busPersonAccountFlexCompOption lobjFlexOption in _iclbFlexCompOption)
            {
                if ((icdoPersonAccount.history_change_date != DateTime.MinValue) ||
                    (lobjFlexOption.icdoPersonAccountFlexCompOption.annual_pledge_amount != 0.00M))
                {
                    bool lblnflag = false; int lintCodeID = 0;
                    if (lobjFlexOption.icdoPersonAccountFlexCompOption.level_of_coverage_value == busConstant.FlexLevelOfCoverageMedicareSpending)
                    {
                        // UAT PIR ID 1287
                        lblnflag = true;
                        lintCodeID = 416;
                    }
                    else if(lobjFlexOption.icdoPersonAccountFlexCompOption.level_of_coverage_value==busConstant.FlexLevelOfCoverageDependentSpending)
                    {
                        // PROD PIR ID 5225
                        lblnflag = true;
                        lintCodeID = 417;
                    }
                    if (lblnflag)
                    {
                        //pir: 6407 & 6287 start
                        if (icdoPersonAccount.history_change_date == DateTime.MinValue && iblnIsFromMSS)
                            icdoPersonAccount.history_change_date = DateTime.Now;
                        //end
                        Collection<cdoCodeValue> lclbCodeValue = GetCodeValue(lintCodeID);
                        foreach (cdoCodeValue lcdoCV in lclbCodeValue)
                        {
                            if (busGlobalFunctions.CheckDateOverlapping(icdoPersonAccount.history_change_date,
                                            Convert.ToDateTime(lcdoCV.data1), Convert.ToDateTime(lcdoCV.data2)))
                            {
                                if (lobjFlexOption.icdoPersonAccountFlexCompOption.annual_pledge_amount > Convert.ToDecimal(lcdoCV.data3))
                                    return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public bool IsDependentOrMedicareEnrollmentDateValid()
        {
            if (icdoPersonAccount.person_account_id != 0)
            {
                if (iclbAccountEmploymentDetail == null)
                    LoadPersonAccountEmploymentDetails();

                // This validation works fine for a single employment. When it comes to multiple employment.
                // We are simply ignoring the check. ie. Transfer employment.
                if (iclbAccountEmploymentDetail.Count > 1)
                    return true;
            }
            if (icdoPersonAccount.person_employment_dtl_id != 0)
            {
                foreach (busPersonAccountFlexCompOption lobjFlexOption in _iclbFlexCompOption)
                {
                    if (icdoPersonAccount.history_change_date != DateTime.MinValue)
                    {
                        DateTime ldtEmploymentStartDate = ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.start_date;
                        DateTime ldtPlanEnrollmentDate = new DateTime(ldtEmploymentStartDate.AddMonths(1).Year, ldtEmploymentStartDate.AddMonths(1).Month, 1);
                        if (icdoPersonAccount.history_change_date < ldtPlanEnrollmentDate)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private void InsertHistory(busPersonAccountFlexCompOption lobjFlexOption)
        {
            cdoPersonAccountFlexCompHistory lobjcdoFlexhistory = new cdoPersonAccountFlexCompHistory();
           
            lobjcdoFlexhistory.level_of_coverage_id = lobjFlexOption.icdoPersonAccountFlexCompOption.level_of_coverage_id;
            lobjcdoFlexhistory.level_of_coverage_value = lobjFlexOption.icdoPersonAccountFlexCompOption.level_of_coverage_value;
            lobjcdoFlexhistory.effective_start_date = icdoPersonAccount.history_change_date;
            lobjcdoFlexhistory.annual_pledge_amount = lobjFlexOption.icdoPersonAccountFlexCompOption.annual_pledge_amount;
            lobjcdoFlexhistory.premium_conversion_waiver_flag = icdoPersonAccountFlexComp.premium_conversion_waiver_flag;
            lobjcdoFlexhistory.inside_mail_flag = icdoPersonAccountFlexComp.inside_mail_flag;
            lobjcdoFlexhistory.direct_deposit_flag = icdoPersonAccountFlexComp.direct_deposit_flag;
            lobjcdoFlexhistory.flex_comp_type_id = icdoPersonAccountFlexComp.flex_comp_type_id;
            lobjcdoFlexhistory.flex_comp_type_value = icdoPersonAccountFlexComp.flex_comp_type_value;
            lobjcdoFlexhistory.start_date = icdoPersonAccount.start_date;
            lobjcdoFlexhistory.plan_participation_status_id = icdoPersonAccount.plan_participation_status_id;
            // PIR 9115
            //PIR 17081
            //if ((icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusFlexSuspended) ||
            //    (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusFlexCancelled))
            //{
            //    if (!iblnIsPreviousHistoryFlagUpdated)
            //    {
            //        DBFunction.DBNonQuery("cdoPersonAccountFlexCompHistory.UpdateReportGeneratedFlag", new object[1] { icdoPersonAccount.person_account_id },
            //                            iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            //        iblnIsPreviousHistoryFlagUpdated = true;
            //    }
            //}
            lobjcdoFlexhistory.plan_participation_status_value = icdoPersonAccount.plan_participation_status_value;
            lobjcdoFlexhistory.status_id = icdoPersonAccount.status_id;
            lobjcdoFlexhistory.status_value = icdoPersonAccount.status_value;
            lobjcdoFlexhistory.person_account_id = icdoPersonAccount.person_account_id;
            lobjcdoFlexhistory.from_person_account_id = icdoPersonAccount.from_person_account_id;
            lobjcdoFlexhistory.to_person_account_id = icdoPersonAccount.to_person_account_id;
            lobjcdoFlexhistory.suppress_warnings_by = icdoPersonAccount.suppress_warnings_by;
            lobjcdoFlexhistory.suppress_warnings_date = icdoPersonAccount.suppress_warnings_date;
            lobjcdoFlexhistory.suppress_warnings_flag = icdoPersonAccount.suppress_warnings_flag;
            lobjcdoFlexhistory.provider_org_id = icdoPersonAccount.provider_org_id;
            lobjcdoFlexhistory.reason_value = icdoPersonAccountFlexComp.reason_value;
            //lobjcdoFlexhistory.ps_file_change_event_value = icdoPersonAccountFlexComp.ps_file_change_event_value;//PIR-7987
            //PIR 13811
            if (icdoPersonAccountFlexComp.reason_value == busConstant.ChangeReasonAnnualEnrollment
                    && (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusFlexCompEnrolled ||
                    icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusFlexSuspended))
            {
                if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusFlexCompEnrolled)
                {
                    lobjcdoFlexhistory.ps_file_change_event_value = icdoPersonAccount.ps_file_change_event_value;
                }
                else
                    lobjcdoFlexhistory.ps_file_change_event_value = busConstant.AnnualEnrollmentWaived;
            }
            else
            {
                lobjcdoFlexhistory.ps_file_change_event_value = icdoPersonAccount.ps_file_change_event_value;
            }
            //lobjcdoFlexhistory.people_soft_file_sent_flag = busConstant.Flag_No;//PIR 17081
            lobjcdoFlexhistory.Insert();
        }

        public void ProcessHistory()
        {
            Collection<busPersonAccountFlexCompHistory> lclbDeletedFlexCompHistory = new Collection<busPersonAccountFlexCompHistory>();
            if ((icdoPersonAccount.status_value == busConstant.PersonAccountStatusValid) && (IsHistoryEntryRequired))
            {
                //Remove the Overlapping History
                if (iclbOverlappingHistory != null && iclbOverlappingHistory.Count > 0)
                {
                    foreach (busPersonAccountFlexCompHistory lbusPAFlexCompHistory in iclbOverlappingHistory)
                    {
                        lclbDeletedFlexCompHistory.Add(lbusPAFlexCompHistory);//PIR 23148
                        lbusPAFlexCompHistory.Delete();
                    }

                    if (IsResourceForEnhancedOverlap(busConstant.OverlapResourceEnhancedFlex))
                    {
                        bool lblnIsPersonAccountModified = false;
                        if (icdoPersonAccount.start_date > icdoPersonAccount.history_change_date)
                        {
                            icdoPersonAccount.start_date = icdoPersonAccount.history_change_date;
                            lblnIsPersonAccountModified = true;

                        }
                        if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusFlexCompEnrolled &&
                            icdoPersonAccount.end_date != DateTime.MinValue)
                        {
                            icdoPersonAccount.end_date = DateTime.MinValue;
                            lblnIsPersonAccountModified = true;
                        }

                        if (lblnIsPersonAccountModified)
                            icdoPersonAccount.Update();
                    }
                }

                //if (_iclbPreviousHistory == null)
                    LoadPreviousHistory();
                ///PIR 7705 - Process History Logic change
                ///Always insert 2 records. One DCRA and one MSRA even if only one of the options has changed.
                if (_iclbFlexCompOptionModified != null)
                {
                    foreach (busPersonAccountFlexCompOption lobjOption in iclbFlexCompOption)
                    {
                        busPersonAccountFlexCompHistory lobjPreviousHistory = new busPersonAccountFlexCompHistory { icdoPersonAccountFlexCompHistory = new cdoPersonAccountFlexCompHistory() };
                           lobjPreviousHistory = GetPreviousHistoryForOption(lobjOption);
                        //7705 End Date previous history
                        if (lobjPreviousHistory.icdoPersonAccountFlexCompHistory.person_account_flex_comp_history_id > 0)
                        {
                            if (!lclbDeletedFlexCompHistory.Any(i => i.icdoPersonAccountFlexCompHistory.person_account_flex_comp_history_id == lobjPreviousHistory.icdoPersonAccountFlexCompHistory.person_account_flex_comp_history_id))
                            {
                                //Add end date only if previous history is not already end dated 
                                if (lobjPreviousHistory.icdoPersonAccountFlexCompHistory.effective_end_date == DateTime.MinValue)
                                {
                                    //If the change date and the effective start date fall under the same year. Then add end date as change date - 1
                                    if (lobjPreviousHistory.icdoPersonAccountFlexCompHistory.effective_start_date.Year == icdoPersonAccount.history_change_date.Year)
                                    {
                                        lobjPreviousHistory.icdoPersonAccountFlexCompHistory.effective_end_date = icdoPersonAccount.history_change_date.AddDays(-1);
                                        if (lobjPreviousHistory.icdoPersonAccountFlexCompHistory.effective_end_date < lobjPreviousHistory.icdoPersonAccountFlexCompHistory.effective_start_date)
                                        {
                                            lobjPreviousHistory.icdoPersonAccountFlexCompHistory.effective_end_date = lobjPreviousHistory.icdoPersonAccountFlexCompHistory.effective_start_date;
                                            // Set flag to 'Y' so that ESS Benefit Enrollment report will ignore these records
                                            //lobjPreviousHistory.icdoPersonAccountFlexCompHistory.is_enrollment_report_generated = busConstant.Flag_Yes;//PIR 17081
                                        }
                                    }
                                    else // If it is in a different year then end date the previous history according to the start date.
                                    {
                                        lobjPreviousHistory.icdoPersonAccountFlexCompHistory.effective_end_date = new DateTime(lobjPreviousHistory.icdoPersonAccountFlexCompHistory.effective_start_date.Year, 12, 31);
                                    }
                                    //if (lobjPreviousHistory.icdoPersonAccountFlexCompHistory.reason_value != icdoPersonAccountFlexComp.reason_value)
                                    //  lobjPreviousHistory.icdoPersonAccountFlexCompHistory.reason_value = icdoPersonAccountFlexComp.reason_value;
                                    lobjPreviousHistory.icdoPersonAccountFlexCompHistory.Update();
                                }
                            }
                        }
                        InsertHistory(lobjOption);
                    }
                    //PIR 10689
                    if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusFlexCompEnrolled)
                    {
                        icdoPersonAccount.end_date = DateTime.MinValue;
                    }
                    else
                    {
                        icdoPersonAccount.end_date = icdoPersonAccount.history_change_date.AddDays(-1); // PIR 10823
                    }
                }
                iblnIsPreviousHistoryFlagUpdated = false; // PIR 9115
            }
        }

        //Flex Comp Annual Enrollment issue - Updating is_enrollment_report_generated flag in history to 'N' if conversion flag is 'N'
        public void UpdateIsEnrollmentReportFlag()
        {
            if (iclbFlexCompHistory == null)
                LoadFlexCompHistory();

            var iclbHistory =  iclbFlexCompHistory.Where(i => i.icdoPersonAccountFlexCompHistory.effective_end_date == DateTime.MinValue);
            foreach (busPersonAccountFlexCompHistory lobjHistory in iclbHistory)
            {
                if (lobjHistory.icdoPersonAccountFlexCompHistory.is_enrollment_report_generated == busConstant.Flag_Yes)
                {
                    //lobjHistory.icdoPersonAccountFlexCompHistory.is_enrollment_report_generated = busConstant.Flag_No;//PIR 17081
                    lobjHistory.icdoPersonAccountFlexCompHistory.Update();
                }
            }
        }


        private void LoadLatestHistory()
        {
            ibusHistory = new busPersonAccountFlexCompHistory();
            ibusHistory.icdoPersonAccountFlexCompHistory = new cdoPersonAccountFlexCompHistory();
            ibusHistory = iclbFlexCompHistory.FirstOrDefault();
        }
      
        private DateTime GetLastEndDate()
        {
            DateTime ldtResult = DateTime.MinValue;

            foreach (busPersonAccountFlexCompOption lobjOption in iclbFlexCompOption)
            {
                if ((lobjOption.icdoPersonAccountFlexCompOption.effective_start_date != DateTime.MinValue) &&
                    (lobjOption.icdoPersonAccountFlexCompOption.effective_end_date != DateTime.MinValue))
                {
                    if (lobjOption.icdoPersonAccountFlexCompOption.effective_end_date > ldtResult)
                        ldtResult = lobjOption.icdoPersonAccountFlexCompOption.effective_end_date;
                }
            }
            return ldtResult;
        }

        public bool IsEndDateLessThanStartDate()
        {
            foreach (busPersonAccountFlexCompOption lobjFlexOption in _iclbFlexCompOption)
            {
                if ((lobjFlexOption.icdoPersonAccountFlexCompOption.effective_start_date != DateTime.MinValue) &&
                    (lobjFlexOption.icdoPersonAccountFlexCompOption.effective_end_date != DateTime.MinValue))
                {
                    if (lobjFlexOption.icdoPersonAccountFlexCompOption.effective_end_date
                        < lobjFlexOption.icdoPersonAccountFlexCompOption.effective_start_date)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            // PIR 10256
            if (!iblnIsFromTerminationPost && icdoPersonAccountFlexComp.reason_value == busConstant.AnnualEnrollment)
                icdoPersonAccount.history_change_date = Convert.ToDateTime(busGlobalFunctions.GetData3ByCodeValue(52, busConstant.AnnualEnrollment, iobjPassInfo));

            //Load the Plan Start Date Only for the First Time 
            if (icdoPersonAccount.start_date == DateTime.MinValue)
                GetPlanStartDate();

            if (iclbPreviousHistory == null)
                LoadPreviousHistory();
            if (iclbFlexCompHistory == null)
                LoadFlexCompHistory();
            if (ibusHistory == null)
                LoadLatestHistory();

            SetHistoryEntryRequiredOrNot();

            //PIR 23167, 23340, 23408
            iclbOverlappingHistory = new Collection<busPersonAccountFlexCompHistory>();
            if ((istrAllowOverlapHistory == busConstant.Flag_Yes) && (icdoPersonAccount.history_change_date != DateTime.MinValue))
            {
                Collection<busPersonAccountFlexCompHistory> lclbOpenHistory = LoadOverlappingHistory();
                if (lclbOpenHistory.Count > 0)
                {
                    foreach (busPersonAccountFlexCompHistory lbusPAFlexComp in lclbOpenHistory)
                    {
                        iclbFlexCompHistory.Remove(lbusPAFlexComp);
                        iclbOverlappingHistory.Add(lbusPAFlexComp);
                    }
                }
            }

            if (icdoPersonAccount.ienuObjectState == ObjectState.Insert)
            {
                iblnIsNewMode = true;
                if (ibusPersonEmploymentDetail != null)
                    if (ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id != 0)
                    {
                        LoadOrgPlan();
                    }
            }
            //PIR 8465 - Need to be able to remove Premium Conversion start and end dates in case they were erroneous.
            foreach (object lobj in iarrChangeLog)
            {
                if (lobj is cdoPersonAccountFlexcompConversion)
                {
                    cdoPersonAccountFlexcompConversion lobjFlexCompConversion = (cdoPersonAccountFlexcompConversion)lobj;
                    //If the record is in Update Mode and the effective start date and end date are null , delete record from DB
                    if (lobjFlexCompConversion.ienuObjectState == Sagitec.Common.ObjectState.Update)
                    {
                        if (lobjFlexCompConversion.effective_start_date == DateTime.MinValue && lobjFlexCompConversion.effective_end_date == DateTime.MinValue)
                        {
                            DBFunction.DBNonQuery("cdoPersonAccountFlexComp.DeleteFromWSSFlexCompConversion", new object[1] { lobjFlexCompConversion.person_account_flex_comp_conversion_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                            lobjFlexCompConversion.Delete();
                        }
                    }
                }
            }
            base.BeforeValidate(aenmPageMode);
        }

        public override void BeforePersistChanges()
        {
            //UAT PIR 2373 people soft file changes
            //--Start--//
            if (IsHistoryEntryRequired && (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusFlexCancelled ||
               icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusFlexSuspended))
            {
                SetPersonAcccountForTeminationChange();
                iblnUpdateModeWithHistory = true;
                SetOptionForTerminationChange();
            }
            else
            {
                SetPersonAccountForEnrollmentChange();
            }
            //--End--//
          
            base.BeforePersistChanges();
        }

        /// <summary>
        /// prod pir 4861 : method to set peoplesoft file sent flag on termination of employment
        /// </summary>
        private void SetOptionForTerminationChange()
        {
            if (iclbFlexCompOption == null)
                LoadFlexCompOptionUpdate();
            foreach (busPersonAccountFlexCompOption lobjFlexCompOption in iclbFlexCompOption)            
            {
                //PIR 17081
                //if ( lobjFlexCompOption.icdoPersonAccountFlexCompOption.annual_pledge_amount > 0.0M)//lobjFlexCompOption.icdoPersonAccountFlexCompOption.effective_start_date != DateTime.MinValue)
                //lobjFlexCompOption.icdoPersonAccountFlexCompOption.people_soft_file_sent_flag = busConstant.Flag_No;

            }
        }

        public bool iblnUpdateModeWithHistory { get; set; }

        /// <summary>
        /// uat pir 2373 : method to set the PS event value based on enrollment change
        /// </summary>
        private void SetPersonAccountForEnrollmentChange()
        {
            if (iclbPreviousHistory == null)
               LoadPreviousHistory();
            if (iclbFlexCompHistory == null)
                LoadFlexCompHistory();
            if (ibusHistory == null)
                LoadLatestHistory();
            foreach (busPersonAccountFlexCompOption lobjFlexOption in _iclbFlexCompOption)
            {
                busPersonAccountFlexCompHistory lobjPAFCH = iclbPreviousHistory
                    .Where(o => o.icdoPersonAccountFlexCompHistory.person_account_id == lobjFlexOption.icdoPersonAccountFlexCompOption.person_account_id &&
                        o.icdoPersonAccountFlexCompHistory.level_of_coverage_value == lobjFlexOption.icdoPersonAccountFlexCompOption.level_of_coverage_value)
                        .FirstOrDefault();
                if (IsHistoryEntryRequired && lobjPAFCH != null)
                {
                    //PROD PIR 4586
                    //prod pir 4861 : removing annual enrollment extraction logic
                    //PIR 7987
                    if (icdoPersonAccountFlexComp.reason_value == busConstant.ChangeReasonAnnualEnrollment
                        && (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusFlexCompEnrolled ||
                        icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusFlexSuspended))
                    {
                        //icdoPersonAccount.people_soft_file_sent_flag = busConstant.Flag_No;//PIR 17081
                        if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusFlexCompEnrolled)
                            icdoPersonAccount.ps_file_change_event_value = busConstant.AnnualEnrollment;
                        else//PIR 7987
                            icdoPersonAccount.ps_file_change_event_value = busConstant.AnnualEnrollmentWaived;
                    }
                    else if (lobjPAFCH.icdoPersonAccountFlexCompHistory.plan_participation_status_value == busConstant.PlanParticipationStatusFlexSuspended &&
                        icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusFlexCompEnrolled)
                    {
                        icdoPersonAccount.ps_file_change_event_value = busConstant.NewEnrollment;
                        //icdoPersonAccount.people_soft_file_sent_flag = busConstant.Flag_No;//PIR 17081
                        //lobjFlexOption.icdoPersonAccountFlexCompOption.people_soft_file_sent_flag = busConstant.Flag_No;//PIR 17081
                    }
                    else if (lobjPAFCH.icdoPersonAccountFlexCompHistory.effective_start_date != lobjFlexOption.icdoPersonAccountFlexCompOption.effective_start_date ||
                        lobjFlexOption.icdoPersonAccountFlexCompOption.effective_end_date != lobjPAFCH.icdoPersonAccountFlexCompHistory.effective_end_date ||
                        lobjPAFCH.icdoPersonAccountFlexCompHistory.annual_pledge_amount != lobjFlexOption.icdoPersonAccountFlexCompOption.annual_pledge_amount)
                    {
                        icdoPersonAccount.ps_file_change_event_value = busConstant.LevelOfCoverageChange;
                        //icdoPersonAccount.people_soft_file_sent_flag = busConstant.Flag_No;//PIR 17081
                        //lobjFlexOption.icdoPersonAccountFlexCompOption.people_soft_file_sent_flag = busConstant.Flag_No;//PIR 17081
                    }
                    iblnUpdateModeWithHistory = true;
                }
            }
        }

        public override int PersistChanges()
        {
            if (icdoPersonAccountFlexComp.ienuObjectState == ObjectState.Insert)
            {
                //PROD PIR 4586
                //prod pir 4861 : removal of annual enrollment logic
                // PROD PIR 7987: Added Annual enrollment logic
                //PIR 7987
                if (icdoPersonAccountFlexComp.reason_value == busConstant.ChangeReasonAnnualEnrollment
                    && (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusFlexCompEnrolled ||
                    icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusFlexSuspended))
                {
                    //icdoPersonAccount.people_soft_file_sent_flag = busConstant.Flag_No;//PIR 17081
                    if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusFlexCompEnrolled)
                        icdoPersonAccount.ps_file_change_event_value = busConstant.AnnualEnrollment;
                    else//PIR 7987
                        icdoPersonAccount.ps_file_change_event_value = busConstant.AnnualEnrollmentWaived;
                    icdoPersonAccountFlexComp.ps_file_change_event_value = icdoPersonAccount.ps_file_change_event_value;//PIR-7987
                }
                else
                {
                    //UAT PIR 2373 people soft file changes
                    //--Start--//
                    //icdoPersonAccount.people_soft_file_sent_flag = busConstant.Flag_No;//PIR 17081
                    icdoPersonAccount.ps_file_change_event_value = busConstant.NewEnrollment;
                    //--End--//
                }
                icdoPersonAccount.Insert();
                icdoPersonAccountFlexComp.person_account_id = icdoPersonAccount.person_account_id;
                icdoPersonAccountFlexComp.Insert();
                foreach (busPersonAccountFlexCompOption lobjFlexOption in _iclbFlexCompOption)
                {
                    //prod pir 4861
                    //lobjFlexOption.icdoPersonAccountFlexCompOption.people_soft_file_sent_flag = busConstant.Flag_No;//PIR 17081
                    lobjFlexOption.icdoPersonAccountFlexCompOption.person_account_id = icdoPersonAccount.person_account_id;
                    lobjFlexOption.icdoPersonAccountFlexCompOption.Insert();
                }
            }
            else
            {
                if (!iblnUpdateModeWithHistory && !iblnEndFlexCompEnrollmentBatch)
                {
                    icdoPersonAccount.ps_file_change_event_value = busConstant.NewEnrollment;
                    //icdoPersonAccount.people_soft_file_sent_flag = busConstant.Flag_No;//PIR 17081
                }

                if (iclbFlexCompHistory == null)
                    LoadFlexCompHistory();
                //if (ibusHistory.IsNull())
                //    LoadLatestHistory();
                //if (ibusHistory.IsNotNull() && ibusHistory.icdoPersonAccountFlexCompHistory.reason_value != icdoPersonAccountFlexComp.reason_value)
                //{
                //    ibusHistory.icdoPersonAccountFlexCompHistory.reason_value = icdoPersonAccountFlexComp.reason_value;
                //    ibusHistory.icdoPersonAccountFlexCompHistory.Update();
                //}
                //PIR 7987
                if (icdoPersonAccountFlexComp.reason_value == busConstant.ChangeReasonAnnualEnrollment
                    && (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusFlexCompEnrolled ||
                    icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusFlexSuspended))
                {
                    //icdoPersonAccount.people_soft_file_sent_flag = busConstant.Flag_No;//PIR 17081
                    if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusFlexCompEnrolled)
                        icdoPersonAccount.ps_file_change_event_value = busConstant.AnnualEnrollment;
                    else//PIR 7987
                        icdoPersonAccount.ps_file_change_event_value = busConstant.AnnualEnrollmentWaived;
                    icdoPersonAccountFlexComp.ps_file_change_event_value = icdoPersonAccount.ps_file_change_event_value;//PIR 13811
                }
                icdoPersonAccount.Update();
                icdoPersonAccountFlexComp.Update();
                foreach (busPersonAccountFlexCompOption lobjFlexOption in _iclbFlexCompOption)
                {
                        if (lobjFlexOption.icdoPersonAccountFlexCompOption.account_flex_comp_option_id == 0)
                        {
                            lobjFlexOption.icdoPersonAccountFlexCompOption.person_account_id = icdoPersonAccount.person_account_id; // UAT PIR ID 1287
                            lobjFlexOption.icdoPersonAccountFlexCompOption.Insert();
                        }
                        else
                        {
                            lobjFlexOption.icdoPersonAccountFlexCompOption.Update();
                        }
                }
            }
            foreach (doBase lobjBase in iarrChangeLog)
            {
                if (lobjBase is cdoPersonAccountFlexcompConversion)
                {
                    cdoPersonAccountFlexcompConversion lcdoPAFC = (cdoPersonAccountFlexcompConversion)lobjBase;
                    lcdoPAFC.person_account_id = icdoPersonAccount.person_account_id;
                    busPersonAccountFlexcompConversion lobjPAFC = new busPersonAccountFlexcompConversion();
                    lobjPAFC.icdoPersonAccountFlexcompConversion = lcdoPAFC;
                    lobjPAFC.BeforeValidate(utlPageMode.All);
                    lobjPAFC.BeforePersistChanges();
                    lobjPAFC.PersistChanges();
                    lobjPAFC.AfterPersistChanges();
                    //PIR 17081
                    //lobjPAFC.UpdateReportFlag(); //PIR 12056
                }
            }
            return 1;
        }
        public override void ValidateHardErrors(utlPageMode aenmPageMode)
        {
            base.ValidateHardErrors(aenmPageMode);
            foreach (doBase lobjBase in iarrChangeLog)
            {
                if (lobjBase is cdoPersonAccountFlexcompConversion)
                {
                    cdoPersonAccountFlexcompConversion lcdoPAFC = (cdoPersonAccountFlexcompConversion)lobjBase;
                    lcdoPAFC.person_account_id = icdoPersonAccount.person_account_id;
                    busPersonAccountFlexcompConversion lobjPAFC = new busPersonAccountFlexcompConversion();
                    lobjPAFC.icdoPersonAccountFlexcompConversion = lcdoPAFC;
                    lobjPAFC.BeforeValidate(utlPageMode.All);
                    lobjPAFC.ValidateHardErrors(utlPageMode.All);
                    foreach (utlError aobjError in lobjPAFC.iarrErrors)
                        iarrErrors.Add(aobjError);
                }
            }
        }
        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            ProcessHistory();
            if (icdoPersonAccount.person_employment_dtl_id > 0)
            {
                SetPersonAccountIDInPersonAccountEmploymentDetail();
            }
            //UTA PIR - 2043
            if ((icdoPersonAccount.status_value == busConstant.PersonAccountStatusValid) && (IsHistoryEntryRequired))
            {
                SetNPSPFlag();
            }
            
            LoadFlexCompHistory();
            LoadPreviousHistory();
            LoadModifiedFlexCompHistory();
            LoadFlexCompOptionUpdate();
            LoadFlexCompConversion();
            LoadPersonAccountFlexCompConversion();
            LoadFlexCompIndividualOptions();
            LoadLatestEmployment();

            
            //uat pir 2118 : new workflow added
            if (ibusBaseActivityInstance != null)
                // SetProcessInstanceParameters();
                SetCaseInstanceParameters();

            if ((icdoPersonAccount.status_value == busConstant.PersonAccountStatusValid) && (IsHistoryEntryRequired))
                PostESSMessage();

            //PIR 17081
            if (icdoPersonAccount.status_value == busConstant.PersonAccountStatusValid && IsHistoryEntryRequired && !iblnIsFromMSSForEnrollmentData)
            {
                InsertIntoEnrollmentData();
            }

            // UAT PIR ID 1077 - To refresh the screen values only if the Suppress flag is On.
            //Always keep RefreshValues function at the end of AfterPersist. Function is called to refresh values on screen.
            if (!iblnIsFromTerminationPost && ibusPersonLatestEmployment.icdoPersonEmployment.end_date == DateTime.MinValue && icdoPersonAccount.suppress_warnings_flag == busConstant.Flag_Yes)
                RefreshValues();
        }

        public void InsertIntoEnrollmentData()
        {
            if (iclbFlexCompHistory == null)
                LoadFlexCompHistory();
            if (iclbFlexCompOption == null)
                LoadFlexCompOptionUpdate();

            int lintPSCounter = 0;
            int lintBERCounter = 0;

            foreach (busPersonAccountFlexCompOption lobjFlexCompOption in iclbFlexCompOption)
            {
                if (iclbPreviousHistory == null)
                    LoadPreviousHistory();

                busEnrollmentData lobjEnrollmentData = new busEnrollmentData();
                lobjEnrollmentData.icdoEnrollmentData = new doEnrollmentData();

                busPersonAccountFlexCompHistory lobjHistory = LoadHistoryByDate(lobjFlexCompOption, icdoPersonAccount.history_change_date);

                busDailyPersonAccountPeopleSoft lobjDailyPAPeopleSoft = new busDailyPersonAccountPeopleSoft();
                lobjDailyPAPeopleSoft.ibusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
                lobjDailyPAPeopleSoft.ibusProvider = new busOrganization { icdoOrganization = new cdoOrganization() };
                lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                lobjDailyPAPeopleSoft.ibusPersonAccountFlexComp = new busPersonAccountFlexComp { icdoPersonAccountFlexComp = new cdoPersonAccountFlexComp() };

                lobjDailyPAPeopleSoft.ibusPersonAccount = ibusPersonAccount;
                lobjDailyPAPeopleSoft.ibusPersonAccountEAP = ibusPersonAccountEAP;

                lobjDailyPAPeopleSoft.LoadPersonEmploymentForPeopleSoft();
                lobjDailyPAPeopleSoft.LoadPeopleSoftOrgGroupValues();

                lobjDailyPAPeopleSoft.ibusProvider = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization;

                lobjDailyPAPeopleSoft.istrLevelOfCoverage = lobjHistory.icdoPersonAccountFlexCompHistory.level_of_coverage_value;

                if (iclbAccountEmploymentDetail == null)
                    LoadPersonAccountEmploymentDetails();

                lobjDailyPAPeopleSoft.lblnAllWaived = iclbAccountEmploymentDetail.Where(
                                o => o.icdoPersonAccountEmploymentDetail.election_value != busConstant.PersonAccountElectionValueWaived).Count() > 0 ? false : true;

                bool lblnAddFlexOptionToFile = false;

                if ((iclbPreviousHistory[0].icdoPersonAccountFlexCompHistory.ps_file_change_event_value == busConstant.AnnualEnrollment ||
                iclbPreviousHistory[0].icdoPersonAccountFlexCompHistory.ps_file_change_event_value == busConstant.AnnualEnrollmentWaived) &&
                (iclbPreviousHistory[0].icdoPersonAccountFlexCompHistory.reason_value == busConstant.AnnualEnrollment ||
                iclbPreviousHistory[0].icdoPersonAccountFlexCompHistory.reason_value == busConstant.AnnualEnrollmentWaived))
                {
                    lobjDailyPAPeopleSoft.iblnAnnualEnrollment = true;
                    lobjDailyPAPeopleSoft.istrHistoryPSFileChangeEventValue = iclbPreviousHistory[0].icdoPersonAccountFlexCompHistory.ps_file_change_event_value;
                }

                //PIR 20178
                if (lintPSCounter == 0)
                {
                    //PIR 20135 Issue - 1 
                    DBFunction.DBNonQuery("cdoPersonAccount.UpdatePrevPeopleSoftRecords", new object[5] { icdoPersonAccount.person_id, icdoPersonAccount.plan_id,
                        iclbPreviousHistory[0].icdoPersonAccountFlexCompHistory.effective_start_date, lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id, icdoPersonAccount.person_account_id },
                            iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                    lintPSCounter++;
                }

                //PIR 26238
                if (lintBERCounter == 0)
                {
                    if (iclbOverlappingHistory.IsNotNull() && iclbOverlappingHistory.Count() > 0)
                    {
                        foreach (busPersonAccountFlexCompHistory lobjDeletedHistory in iclbOverlappingHistory)
                        {
                            DBFunction.DBNonQuery("cdoPersonAccount.UpdatePrevPeopleSoftNBenefitEnrlFlag", new object[2] { lobjDeletedHistory.icdoPersonAccountFlexCompHistory.person_account_flex_comp_history_id, icdoPersonAccount.person_account_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                        }
                        lintBERCounter++;
                    }
                }

                //PIR 23307 - Update previous Enrollment data records if previous history records are of same start date and end date
                if (iclbFlexCompHistory != null && iclbFlexCompHistory.Count > 0)
                {
                    var lclbPAFlexCompHistory = iclbFlexCompHistory.Where(i => i.icdoPersonAccountFlexCompHistory.level_of_coverage_value == lobjFlexCompOption.icdoPersonAccountFlexCompOption.level_of_coverage_value 
                                                                    && i.icdoPersonAccountFlexCompHistory.effective_start_date == i.icdoPersonAccountFlexCompHistory.effective_end_date);

                    foreach (busPersonAccountFlexCompHistory lobjPAFlexCompHist in lclbPAFlexCompHistory)
                    {
                        //DBFunction.DBNonQuery("cdoPersonAccount.UpdatePrevBenFlagWithSameStartEndDateFlexComp", new object[2] {
                        //lobjPAFlexCompHist.icdoPersonAccountFlexCompHistory.person_account_flex_comp_history_id, icdoPersonAccount.person_account_id},
                        //iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                        DataTable ldtEnrollmentData = busNeoSpinBase.Select("cdoPersonAccount.GetEnrollmentData", new object[2] { lobjPAFlexCompHist.icdoPersonAccountFlexCompHistory.person_account_flex_comp_history_id, icdoPersonAccount.person_account_id });
                        if (ldtEnrollmentData.Rows.Count > 0)
                        {
                            busEnrollmentData lobjEnrollData = new busEnrollmentData { icdoEnrollmentData = new doEnrollmentData() };
                            lobjEnrollData.icdoEnrollmentData.LoadData(ldtEnrollmentData.Rows[0]);
                            lobjEnrollData.icdoEnrollmentData.is_benefit_enrollment_report_generated = busConstant.Flag_Yes;
                            lobjEnrollData.icdoEnrollmentData.Update();
                        }
                    }
                }
                if ((lobjDailyPAPeopleSoft.iclbPeopleSoftOrgGroupValue.Where(i => i.code_value == lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.peoplesoft_org_group_value &&
                                                                                    i.data2 == busConstant.Flag_Yes).Count() > 0))
                {
                    if (lobjDailyPAPeopleSoft.iclbDailyPersonAccountPeopleSoft == null)
                        lobjDailyPAPeopleSoft.iclbDailyPersonAccountPeopleSoft = new Collection<busDailyPersonAccountPeopleSoft>();

                    LoadPreviousHistory();
                    busPersonAccountFlexCompHistory lobjLatestHistory = iclbPreviousHistory
                        .Where(i => i.icdoPersonAccountFlexCompHistory.level_of_coverage_value == lobjFlexCompOption.icdoPersonAccountFlexCompOption.level_of_coverage_value).FirstOrDefault();

                    //Get the plan option Start Date for the fields coverage begin date,deduction begin date,election date in output file
                    lobjDailyPAPeopleSoft.idtPlanOptionStartDate = lobjLatestHistory.icdoPersonAccountFlexCompHistory.effective_start_date;

                    busPersonAccountFlexCompHistory lbusPAFlexCompHistory = iclbPreviousHistory.FirstOrDefault(o => o.icdoPersonAccountFlexCompHistory.level_of_coverage_value == lobjFlexCompOption.icdoPersonAccountFlexCompOption.level_of_coverage_value &&
                        o.icdoPersonAccountFlexCompHistory.person_account_id == lobjFlexCompOption.icdoPersonAccountFlexCompOption.person_account_id);
                    if (lbusPAFlexCompHistory != null)
                    {
                        lobjDailyPAPeopleSoft.istrHistoryPSFileChangeEventValue = lbusPAFlexCompHistory.icdoPersonAccountFlexCompHistory.ps_file_change_event_value;
                    }

                    if (lobjFlexCompOption.icdoPersonAccountFlexCompOption.annual_pledge_amount > 0.0m)
                    {
                        lblnAddFlexOptionToFile = true;
						//PIR 20135 - Issue - 3
                        if (lobjHistory.icdoPersonAccountFlexCompHistory.plan_participation_status_value != busConstant.PlanParticipationStatusFlexCompEnrolled)
                        {
                            lobjEnrollmentData.icdoEnrollmentData.coverage_election_value = "T";
                            lobjEnrollmentData.icdoEnrollmentData.coverage_amount = 0.0m;
                            lobjDailyPAPeopleSoft.idecFlatAmount = 0.0m;
                        }
                        else
                        {
                            lobjEnrollmentData.icdoEnrollmentData.coverage_election_value = "E";
                            lobjEnrollmentData.icdoEnrollmentData.coverage_amount = lobjFlexCompOption.icdoPersonAccountFlexCompOption.annual_pledge_amount;
                            lobjDailyPAPeopleSoft.idecFlatAmount = lobjFlexCompOption.icdoPersonAccountFlexCompOption.annual_pledge_amount;
                        }
                    }
                    else if (lobjFlexCompOption.icdoPersonAccountFlexCompOption.annual_pledge_amount == 0.0m)
                    {
                        busPersonAccountFlexCompHistory lobjPrevHistory = LoadHistoryByDate(lobjFlexCompOption, icdoPersonAccount.history_change_date);
                        if (lobjPrevHistory != null && lobjPrevHistory.icdoPersonAccountFlexCompHistory.annual_pledge_amount > 0)
                        {
                            lblnAddFlexOptionToFile = true;
                            lobjEnrollmentData.icdoEnrollmentData.coverage_election_value = "T";
                            lobjEnrollmentData.icdoEnrollmentData.coverage_amount = 0.0m;
                            lobjDailyPAPeopleSoft.idecFlatAmount = 0.0m;
                        }
                    }

                    if (lobjEnrollmentData.icdoEnrollmentData.coverage_election_value == "T")
                        lobjEnrollmentData.icdoEnrollmentData.direct_deposit_flag = string.Empty;
                    else
                        lobjEnrollmentData.icdoEnrollmentData.direct_deposit_flag = icdoPersonAccountFlexComp.direct_deposit_flag;

                    if (lobjEnrollmentData.icdoEnrollmentData.coverage_election_value == "T") //PIR-7987
                        lobjEnrollmentData.icdoEnrollmentData.inside_mail_flag = string.Empty;
                    else
                        lobjEnrollmentData.icdoEnrollmentData.inside_mail_flag = icdoPersonAccountFlexComp.inside_mail_flag;

                    lobjDailyPAPeopleSoft.GeneratePeoplesoftEntryForFlex();

                    if (lobjDailyPAPeopleSoft.iclbDailyPersonAccountPeopleSoft.IsNotNull() && lobjDailyPAPeopleSoft.iclbDailyPersonAccountPeopleSoft.Count > 0)
                    {
                        foreach (busDailyPersonAccountPeopleSoft lobjDailyPeopleSoft in lobjDailyPAPeopleSoft.iclbDailyPersonAccountPeopleSoft)
                        {
                            lobjEnrollmentData.icdoEnrollmentData.source_id = lobjHistory.icdoPersonAccountFlexCompHistory.person_account_flex_comp_history_id;
                            lobjEnrollmentData.icdoEnrollmentData.plan_id = icdoPersonAccount.plan_id;
                            lobjEnrollmentData.icdoEnrollmentData.employment_type_value = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value;
                            lobjEnrollmentData.icdoEnrollmentData.ssn = ibusPerson.icdoPerson.ssn;
                            lobjEnrollmentData.icdoEnrollmentData.ndpers_member_id = ibusPerson.icdoPerson.person_id;
                            lobjEnrollmentData.icdoEnrollmentData.peoplesoft_id = ibusPerson.icdoPerson.peoplesoft_id;
                            lobjEnrollmentData.icdoEnrollmentData.employer_org_id = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id;
                            lobjEnrollmentData.icdoEnrollmentData.plan_status_value = lobjHistory.icdoPersonAccountFlexCompHistory.plan_participation_status_value;
                            lobjEnrollmentData.icdoEnrollmentData.change_reason_value = lobjHistory.icdoPersonAccountFlexCompHistory.reason_value;
                            lobjEnrollmentData.icdoEnrollmentData.start_date = lobjHistory.icdoPersonAccountFlexCompHistory.effective_start_date;
                            lobjEnrollmentData.icdoEnrollmentData.end_date = lobjHistory.icdoPersonAccountFlexCompHistory.effective_end_date;
                            lobjEnrollmentData.icdoEnrollmentData.level_of_coverage_value = lobjHistory.icdoPersonAccountFlexCompHistory.level_of_coverage_value;
                            lobjEnrollmentData.icdoEnrollmentData.person_account_id = icdoPersonAccount.person_account_id;

                            if ((lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date == DateTime.MinValue || (lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date != DateTime.MinValue &&
                                busGlobalFunctions.CheckDateOverlapping(icdoPersonAccount.history_change_date.AddMonths(-1), lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date,
                                new DateTime(lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date.Year, lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date.Month, 1).AddMonths(2)))))
                                lobjEnrollmentData.icdoEnrollmentData.is_benefit_enrollment_report_generated = busConstant.Flag_No;
                            else
                                lobjEnrollmentData.icdoEnrollmentData.is_benefit_enrollment_report_generated = busConstant.Flag_Yes;

                            if (lblnAddFlexOptionToFile)
                                lobjEnrollmentData.icdoEnrollmentData.peoplesoft_file_sent_flag = busConstant.Flag_No;
                            else
                                lobjEnrollmentData.icdoEnrollmentData.peoplesoft_file_sent_flag = busConstant.Flag_Yes;

                            lobjEnrollmentData.icdoEnrollmentData.coverage_code = lobjDailyPeopleSoft.istrCoverageCode;
                            if (lobjEnrollmentData.icdoEnrollmentData.coverage_election_value.IsNullOrEmpty())
                                lobjEnrollmentData.icdoEnrollmentData.coverage_election_value = lobjDailyPeopleSoft.istrCoverageElection;

                            if (lobjEnrollmentData.icdoEnrollmentData.coverage_election_value == "T")
                                lobjEnrollmentData.icdoEnrollmentData.benefit_plan = string.Empty;
                            else
                                lobjEnrollmentData.icdoEnrollmentData.benefit_plan = lobjDailyPeopleSoft.istrBenefitPlan;
                            lobjEnrollmentData.icdoEnrollmentData.deduction_begin_date = lobjDailyPeopleSoft.idtDeductionBeginDate;
                            lobjEnrollmentData.icdoEnrollmentData.coverage_begin_date = lobjDailyPeopleSoft.idtCoverageBeginDate;
                            lobjEnrollmentData.icdoEnrollmentData.plan_type = lobjDailyPeopleSoft.istrPlanType;
                            lobjEnrollmentData.icdoEnrollmentData.election_date = lobjDailyPeopleSoft.idtElectionDate;
                            lobjEnrollmentData.icdoEnrollmentData.pretax_amount = lobjDailyPeopleSoft.idecFlatAmount;
                            lobjEnrollmentData.icdoEnrollmentData.peoplesoft_org_group_value = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.peoplesoft_org_group_value;
                            lobjEnrollmentData.icdoEnrollmentData.peoplesoft_change_reason_value = lobjDailyPAPeopleSoft.istrPSFileChangeEvent;

                            lobjEnrollmentData.icdoEnrollmentData.Insert();

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
                        lobjEnrollmentData.icdoEnrollmentData.source_id = lobjHistory.icdoPersonAccountFlexCompHistory.person_account_flex_comp_history_id;
                        lobjEnrollmentData.icdoEnrollmentData.plan_id = icdoPersonAccount.plan_id;
                        lobjEnrollmentData.icdoEnrollmentData.employment_type_value = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value;
                        lobjEnrollmentData.icdoEnrollmentData.ssn = ibusPerson.icdoPerson.ssn;
                        lobjEnrollmentData.icdoEnrollmentData.ndpers_member_id = ibusPerson.icdoPerson.person_id;
                        lobjEnrollmentData.icdoEnrollmentData.person_account_id = icdoPersonAccount.person_account_id;
                        lobjEnrollmentData.icdoEnrollmentData.peoplesoft_id = ibusPerson.icdoPerson.peoplesoft_id;
                        lobjEnrollmentData.icdoEnrollmentData.employer_org_id = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id;
                        lobjEnrollmentData.icdoEnrollmentData.plan_status_value = lobjHistory.icdoPersonAccountFlexCompHistory.plan_participation_status_value;
                        lobjEnrollmentData.icdoEnrollmentData.change_reason_value = lobjHistory.icdoPersonAccountFlexCompHistory.reason_value;
                        lobjEnrollmentData.icdoEnrollmentData.start_date = lobjHistory.icdoPersonAccountFlexCompHistory.effective_start_date;
                        lobjEnrollmentData.icdoEnrollmentData.end_date = lobjHistory.icdoPersonAccountFlexCompHistory.effective_end_date;
                        lobjEnrollmentData.icdoEnrollmentData.level_of_coverage_value = lobjHistory.icdoPersonAccountFlexCompHistory.level_of_coverage_value;
                        lobjEnrollmentData.icdoEnrollmentData.coverage_amount = lobjFlexCompOption.icdoPersonAccountFlexCompOption.annual_pledge_amount;
                        lobjEnrollmentData.icdoEnrollmentData.peoplesoft_org_group_value = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.peoplesoft_org_group_value;
                        lobjEnrollmentData.icdoEnrollmentData.is_benefit_enrollment_report_generated = busConstant.Flag_No;
                        lobjEnrollmentData.icdoEnrollmentData.peoplesoft_file_sent_flag = busConstant.Flag_Yes;
                        lobjEnrollmentData.icdoEnrollmentData.Insert();
                    }
                }

            }

        }

        public void RefreshValues()
        {
            icdoPersonAccount.suppress_warnings_flag = string.Empty; // UAT PIR ID 1015
            icdoPersonAccountFlexComp.reason_value = string.Empty;  // UAT PIR ID 1043
            icdoPersonAccount.history_change_date = DateTime.MinValue; // PIR ID 2089
        }

        private void GetPlanStartDate()
        {
            //DateTime ldtStartDate = DateTime.MinValue;

            //foreach (busPersonAccountFlexCompOption lobjFlexOption in _iclbFlexCompOption)
            //{
            //    //if (lobjFlexOption.icdoPersonAccountFlexCompOption.effective_start_date != DateTime.MinValue)
            //   // {
            //        if (ldtStartDate == DateTime.MinValue)
            //        {
            //            ldtStartDate = lobjFlexOption.icdoPersonAccountFlexCompOption.effective_start_date;
            //        }
            //        else if (lobjFlexOption.icdoPersonAccountFlexCompOption.effective_start_date < ldtStartDate)
            //        {
            //            ldtStartDate = lobjFlexOption.icdoPersonAccountFlexCompOption.effective_start_date;
            //        }
            //   // }
            //}

            //if (ldtStartDate == DateTime.MinValue)
            //    icdoPersonAccount.start_date = icdoPersonAccount.history_change_date;
            //else
            //    icdoPersonAccount.start_date = ldtStartDate;

            icdoPersonAccount.start_date = icdoPersonAccount.history_change_date;
        }

        public void LoadPreviousHistory()
        {
            if (_iclbPreviousHistory == null)
                _iclbPreviousHistory = new Collection<busPersonAccountFlexCompHistory>();

            DataTable ldtbHistory = Select("cdoPersonAccountFlexComp.GetLatestHistoryRecordByLevelOfCoverage",
                                    new object[1] { icdoPersonAccount.person_account_id });
            _iclbPreviousHistory = GetCollection<busPersonAccountFlexCompHistory>(ldtbHistory, "icdoPersonAccountFlexCompHistory");
        }

        /// <summary>
        /// Load Flex Comp History By date and Flex Comp option
        /// </summary>
        /// <param name="aobjPAFlexOption"></param>
        /// <param name="adtGivenDate"></param>
        /// <returns></returns>
        public busPersonAccountFlexCompHistory LoadHistoryByDate(busPersonAccountFlexCompOption aobjPAFlexOption, DateTime adtGivenDate)
        {
            busPersonAccountFlexCompHistory lobjPersonAccountFlexHistory = new busPersonAccountFlexCompHistory { icdoPersonAccountFlexCompHistory = new cdoPersonAccountFlexCompHistory() } ;
            if (iclbFlexCompHistory == null)
                LoadFlexCompHistory();

            foreach (busPersonAccountFlexCompHistory lobjPAFlexHistory in iclbFlexCompHistory)
            {
                if (lobjPAFlexHistory.icdoPersonAccountFlexCompHistory.level_of_coverage_value == aobjPAFlexOption.icdoPersonAccountFlexCompOption.level_of_coverage_value)
                {
                    //Ignore the Same Start Date and End Date Records
                    if (lobjPAFlexHistory.icdoPersonAccountFlexCompHistory.effective_start_date != lobjPAFlexHistory.icdoPersonAccountFlexCompHistory.effective_end_date)
                    {
                        if (busGlobalFunctions.CheckDateOverlapping(adtGivenDate, lobjPAFlexHistory.icdoPersonAccountFlexCompHistory.effective_start_date,
                            lobjPAFlexHistory.icdoPersonAccountFlexCompHistory.effective_end_date))
                        {
                            lobjPersonAccountFlexHistory = lobjPAFlexHistory;
                            break;
                        }
                    }
                }
            }
            return lobjPersonAccountFlexHistory;
        }

        private void SetHistoryEntryRequiredOrNot()
        {
            if (_iclbPreviousHistory == null)
                LoadPreviousHistory();
            IsHistoryEntryRequired = false;

            //Clear the Collection If Exists
            _iclbFlexCompOptionModified = new Collection<busPersonAccountFlexCompOption>();
            foreach (busPersonAccountFlexCompOption lobjFlexCompOption in iclbFlexCompOption)
            {
                busPersonAccountFlexCompHistory lobjPreviousOptionHistory = GetPreviousHistoryForOption(lobjFlexCompOption);
                if (lobjPreviousOptionHistory.icdoPersonAccountFlexCompHistory.person_account_flex_comp_history_id == 0)
                {
                    if (lobjFlexCompOption.icdoPersonAccountFlexCompOption.annual_pledge_amount > 0.0M)
                    {
                        IsHistoryEntryRequired = true;
                        _iclbFlexCompOptionModified.Add(lobjFlexCompOption);
                        continue;
                    }
                }

                if (IsMandatoryFieldChanged(lobjFlexCompOption, lobjPreviousOptionHistory))
                {
                    IsHistoryEntryRequired = true;
                    _iclbFlexCompOptionModified.Add(lobjFlexCompOption);
                    continue;
                }
            }
        }

        private busPersonAccountFlexCompHistory GetPreviousHistoryForOption(busPersonAccountFlexCompOption aobjFlexCompOption)
        {
            busPersonAccountFlexCompHistory lobjOptionHistory = new busPersonAccountFlexCompHistory();
            lobjOptionHistory.icdoPersonAccountFlexCompHistory = new cdoPersonAccountFlexCompHistory();
            foreach (busPersonAccountFlexCompHistory lobjFlexCompHistory in iclbPreviousHistory)
            {
                if (lobjFlexCompHistory.icdoPersonAccountFlexCompHistory.level_of_coverage_value == aobjFlexCompOption.icdoPersonAccountFlexCompOption.level_of_coverage_value)
                {
                    lobjOptionHistory = lobjFlexCompHistory;
                    break;
                }
            }
            return lobjOptionHistory;
        }

        private bool IsMandatoryFieldChanged(busPersonAccountFlexCompOption aobjOption, busPersonAccountFlexCompHistory aobjOptionHistory)
        {
            bool lblnResult = false;

            if   ((aobjOptionHistory.icdoPersonAccountFlexCompHistory.plan_participation_status_value != icdoPersonAccount.plan_participation_status_value) ||
                  (aobjOptionHistory.icdoPersonAccountFlexCompHistory.flex_comp_type_value != icdoPersonAccountFlexComp.flex_comp_type_value) ||
                  (aobjOptionHistory.icdoPersonAccountFlexCompHistory.inside_mail_flag != icdoPersonAccountFlexComp.inside_mail_flag) ||
                  (aobjOptionHistory.icdoPersonAccountFlexCompHistory.direct_deposit_flag != icdoPersonAccountFlexComp.direct_deposit_flag) ||
                  (aobjOptionHistory.icdoPersonAccountFlexCompHistory.premium_conversion_waiver_flag != icdoPersonAccountFlexComp.premium_conversion_waiver_flag) ||
                  (aobjOptionHistory.icdoPersonAccountFlexCompHistory.effective_start_date != icdoPersonAccount.history_change_date) ||
                  (aobjOptionHistory.icdoPersonAccountFlexCompHistory.annual_pledge_amount != aobjOption.icdoPersonAccountFlexCompOption.annual_pledge_amount)
                 )
            {
                lblnResult = true;
            }
            return lblnResult;
        }

        # region UAT PIR - 598

        public bool IsSameSpendingAccountEnrolledMoreThanOnce()
        {
            if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusFlexCompEnrolled)
            {
                //check already existing employment under same employer is end dated
                //and the previous plan participation status is suspended
                if (ibusPreviousEmploymentDetail.IsNull())
                    LoadPreviousEmploymentDetail();

                if (ibusPreviousEmploymentDetail.ibusPersonEmployment.IsNull())
                    ibusPreviousEmploymentDetail.LoadPersonEmployment();

                //get the latest open person employment
				//PIR 26356/26428 Load latest employment, as PA Empl Detail not linked when enrolling from MSS
                busPersonEmploymentDetail lobjPersonEmploymentDetail = new busPersonEmploymentDetail { icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail() };
                
                if (iobjPassInfo.istrFormName == "wfmViewRequestFlexCompEnrollmentMaintenanceLOB" || iobjPassInfo.istrFormName == "wfmFlexEnrollmentWizard")
                {
                    DataTable ldtPersonLatestEmploymentDtl = busBase.Select("entPersonEmploymentDetail.LoadLatestEmploymentDtl", new object[1] { icdoPersonAccount.person_id });
                    if (ldtPersonLatestEmploymentDtl.Rows.Count > 0)
                    {
                        lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.LoadData(ldtPersonLatestEmploymentDtl.Rows[0]);
                    }
                }
                else
                {
                   lobjPersonEmploymentDetail = GetLatestOpenedPersonEmploymentDetail();
                }

                if (lobjPersonEmploymentDetail.ibusPersonEmployment.IsNull())
                    lobjPersonEmploymentDetail.LoadPersonEmployment();

                //This validation should not fire when there is pledge amount on the same year for the same employment.
                //At the same time, if the member terminated, and rehired with same employer, we should throw error.
                bool lblnChangeInEmployment = false;
                if (ibusPreviousEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.person_employment_id != lobjPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.person_employment_id)
                    lblnChangeInEmployment = true;

                if (lblnChangeInEmployment && IsPersonLinkedNLatestEmpDtlGapGreaterThan31Days())
                {
                    if (ibusPreviousEmploymentDetail.icdoPersonEmploymentDetail.end_date != DateTime.MinValue)
                    {  
                      foreach (busPersonAccountFlexCompOption lobjFlexCompOption in iclbFlexCompOption)
                        {
                            if (lobjFlexCompOption.icdoPersonAccountFlexCompOption.level_of_coverage_value == busConstant.FlexLevelOfCoverageMedicareSpending)
                            {
                                if (lobjFlexCompOption.icdoPersonAccountFlexCompOption.annual_pledge_amount > 0.0M)
                                {
                                    //get the previous history 
                                    busPersonAccountFlexCompHistory lobjLastHistory = GetPreviousHistoryForOption(lobjFlexCompOption);
                                    if (lobjLastHistory.icdoPersonAccountFlexCompHistory.plan_participation_status_value == busConstant.PlanParticipationStatusFlexSuspended)
                                    {
                                        if (lobjLastHistory.icdoPersonAccountFlexCompHistory.effective_start_date.Year ==
                                            icdoPersonAccount.history_change_date.Year)
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

        # endregion

        public bool IsHistoryChangeDateLessThanLastChangeDate()
        {
            bool lblnResult = false;
            if (IsHistoryEntryRequired)
            {
                if (_iclbPreviousHistory == null)
                    LoadPreviousHistory();
                if (iclbFlexCompHistory == null)
                    LoadFlexCompHistory();
                if (ibusHistory == null)
                    LoadLatestHistory();
                if (ibusHistory.icdoPersonAccountFlexCompHistory.person_account_flex_comp_history_id > 0)
                {
                    if (ibusHistory.icdoPersonAccountFlexCompHistory.effective_end_date != DateTime.MinValue)
                    {
                        if (icdoPersonAccount.history_change_date < ibusHistory.icdoPersonAccountFlexCompHistory.effective_end_date)
                        {
                            lblnResult = true;
                        }
                    }
                    else if (icdoPersonAccount.history_change_date < ibusHistory.icdoPersonAccountFlexCompHistory.effective_start_date)
                        lblnResult = true;
                }
            }
            return lblnResult;
        }

        public bool IsEndDateLessThanPreviousHistoryDate()
        {
            bool lblnResult = false;
            if (_iclbPreviousHistory == null)
                LoadPreviousHistory();
            foreach (busPersonAccountFlexCompOption lobjOption in _iclbFlexCompOptionModified)
            {
                busPersonAccountFlexCompHistory lobjPreviousHistory = GetPreviousHistoryForOption(lobjOption);
                if (lobjPreviousHistory.icdoPersonAccountFlexCompHistory.person_account_flex_comp_history_id > 0)
                {
                    if (icdoPersonAccount.history_change_date != DateTime.MinValue && icdoPersonAccount.history_change_date < lobjPreviousHistory.icdoPersonAccountFlexCompHistory.effective_start_date)
                    {
                        lblnResult = true;
                        break;
                    }
                }
            }
            return lblnResult;
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
            if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusFlexCompEnrolled)
            {
                if (icdoPersonAccount.current_plan_start_date_no_null > DateTime.Now)
                    idtPlanEffectiveDate = icdoPersonAccount.current_plan_start_date_no_null;
                else
                    idtPlanEffectiveDate = DateTime.Now;
            }
            else
            {
                if (iclbFlexCompHistory == null)
                    LoadFlexCompHistory();

                //By Default the Collection sorted by latest date
                foreach (busPersonAccountFlexCompHistory lbusPersonAccountFlexCompHistory in iclbFlexCompHistory)
                {
                    if (lbusPersonAccountFlexCompHistory.icdoPersonAccountFlexCompHistory.plan_participation_status_value == busConstant.PlanParticipationStatusFlexCompEnrolled)
                    {
                        if (lbusPersonAccountFlexCompHistory.icdoPersonAccountFlexCompHistory.end_date == DateTime.MinValue)
                        {
                            //If the Start Date is Future Date, Set it otherwise Current Date will be Start Date of Premium Calc
                            if (lbusPersonAccountFlexCompHistory.icdoPersonAccountFlexCompHistory.start_date > DateTime.Now)
                            {
                                idtPlanEffectiveDate = lbusPersonAccountFlexCompHistory.icdoPersonAccountFlexCompHistory.start_date;
                            }
                            else
                            {
                                idtPlanEffectiveDate = DateTime.Now;
                            }
                        }
                        else
                        {
                            idtPlanEffectiveDate = lbusPersonAccountFlexCompHistory.icdoPersonAccountFlexCompHistory.end_date;
                        }
                        break;
                    }
                }
            }
        }

        public override ArrayList ValidateNew(Hashtable ahstParam)
        {
            ArrayList larrList = new ArrayList();
            busPersonAccount lobjFlexAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
            lobjFlexAccount.FindPersonAccount(Convert.ToInt32(ahstParam["aintpersonaccountid"]));
            ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            ibusPerson.FindPerson(lobjFlexAccount.icdoPersonAccount.person_id);
            ibusPerson.LoadPersonAccount();
            return larrList;
        }

        # region UCS - 022 Correspondence

        public void LoadSuspendedEndDateForFlexComp(ref DateTime adtSuspendedDate, ref DateTime adtFirstMonthOfSuspendedDate)
        {
            if (_iclbPreviousHistory.IsNull())
                LoadPreviousHistory();
            if (_iclbPreviousHistory.Count > 0)
            {
                var lFlexHistory = _iclbPreviousHistory.OrderByDescending(lobjPA => lobjPA.icdoPersonAccountFlexCompHistory.start_date);
                foreach (busPersonAccountFlexCompHistory lobjFlexHistory in lFlexHistory)
                {
                    if (lobjFlexHistory.icdoPersonAccountFlexCompHistory.plan_participation_status_value == busConstant.PlanParticipationStatusFlexSuspended)
                    {
                        adtFirstMonthOfSuspendedDate = lobjFlexHistory.icdoPersonAccountFlexCompHistory.effective_end_date;
                        idtFirstOfMonthSupendendEndDate = lobjFlexHistory.icdoPersonAccountFlexCompHistory.effective_end_date.GetFirstDayofNextMonth();
                        adtSuspendedDate = lobjFlexHistory.icdoPersonAccountFlexCompHistory.start_date;
                        break;
                    }
                }
            }
        }
        //UAT PIR - 2000
        public string istrFormattedFirstOfMonthSupendendEndDate
        {
            get
            {
                return idtFirstOfMonthSupendendEndDate.ToString(busConstant.DateFormatLongDate);
            }
        }

        public override busBase GetCorPerson()
        {
            return base.GetCorPerson();
        }

        public override busBase GetCorOrganization()
        {
            if (ibusPersonEmploymentDetail.IsNull()) LoadPersonEmploymentDetail();
            if (ibusPersonEmploymentDetail.ibusPersonEmployment.IsNull()) ibusPersonEmploymentDetail.LoadPersonEmployment();
            if (ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.IsNull()) ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
            return ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization;
        }
        public string istrSuspendedLongDate { get; set; }
        public override void LoadCorresProperties(string astrTemplateName)
        {
            if (ibusPerson.IsNull())
                LoadPerson();
            if (ibusPersonAccount.IsNull())
                LoadPersonAccount();
            if (idecMonthlyPremiumAmountByPlan == 0.00M)
                LoadMonthlyPremium();
            if (ibusPersonEmploymentDetail == null)
                LoadPersonEmploymentDetail();
            if (ibusPersonEmploymentDetail.ibusPersonEmployment == null)
                ibusPersonEmploymentDetail.LoadPersonEmployment();
            if (ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization == null)
                ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
            //ENR-5450
            DateTime ldtFirstOfMOnthSuspendedDate = DateTime.MinValue;
            DateTime ldtSuspendedDate = DateTime.MinValue;
            LoadSuspendedEndDateForFlexComp(ref ldtSuspendedDate, ref ldtFirstOfMOnthSuspendedDate);
            // idtFirstOfMonthSupendendEndDate = ldtFirstOfMOnthSuspendedDate;
            istrSuspendedLongDate = idtFirstOfMonthSupendendEndDate.ToString(busConstant.DateFormatLongDate);
        }
        public busPersonAccount ibusPersonAccount { get; set; }
        public void LoadPersonAccount()
        {
            if (ibusPersonAccount.IsNull())
                ibusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };

            ibusPersonAccount.icdoPersonAccount = this.icdoPersonAccount;
        }

        public void LoadCancelledEndDate(ref DateTime adtCancelledEffectiveDate)
        {
            if (_iclbPreviousHistory.IsNull())
                LoadPreviousHistory();
            if (_iclbPreviousHistory.Count > 0)
            {
                foreach (busPersonAccountFlexCompHistory lobjFlexHistory in _iclbPreviousHistory)
                {
                    if (lobjFlexHistory.icdoPersonAccountFlexCompHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceCancelled)
                    {
                        adtCancelledEffectiveDate = lobjFlexHistory.icdoPersonAccountFlexCompHistory.start_date;
                        break;
                    }
                }
            }
        }

        //load the monthly premium amount for medical spending
        public decimal idecMonthlyPremiumForMedicalSpending { get; set; }
        //load coverage date
        public void LoadCoverageDateForFlex()
        {
            if (idtCoverageBeginDate == DateTime.MinValue)
            {
                if (icdoPersonAccountFlexComp.flex_comp_type_value == busConstant.FlexCompTypeValueCOBRA)
                {
                    if (iclbFlexCompHistory.IsNull())
                        LoadFlexCompHistory();
                    if (iclbFlexCompHistory.Count > 0)
                    {
                        idtCoverageBeginDate = iclbFlexCompHistory.First().icdoPersonAccountFlexCompHistory.start_date;

                        DateTime ldtTempDate = iclbFlexCompHistory.First().icdoPersonAccountFlexCompHistory.start_date;
                        ldtTempDate = ldtTempDate.AddYears(1);
                        //last day of the start date year
                        idtCoverageEndDate = new DateTime(ldtTempDate.Year, 12, 31);

                        //idtCoverageEndDate = icdoPersonAccount.cobra_expiration_date;
                    }
                }
            }
        }
        public void LoadMonthlyPremium()
        {
            LoadFlexCompOptionUpdate();
            var lFlexCompOtion = iclbFlexCompOption
                .Where(lobjFlexComp => lobjFlexComp.icdoPersonAccountFlexCompOption.level_of_coverage_value == busConstant.FlexLevelOfCoverageMedicareSpending);
            if (lFlexCompOtion.Count() > 0)
                idecMonthlyPremiumAmountByPlan = (lFlexCompOtion.First().icdoPersonAccountFlexCompOption.annual_pledge_amount / 12);
          
        }
        # endregion

        public int iintNextYear
        {
            get
            {
                return DateTime.Now.AddYears(1).Year;
            }
        }
		//PIR 9946
        public int iintCurrentYear
        {
            get
            {
                return DateTime.Now.Year;
            }
        }

        #region UCS - 032

        public bool IsESSPremiumConversionTextVisible()
        {
            bool lblnResult = false;
            if (iclbFlexcompConversion != null && iclbFlexcompConversion.Count > 0)
                lblnResult = true;
            return lblnResult;
        }

        #endregion

        //UAT PIR - 2043        
        private void SetNPSPFlag()
        {
            if (ibusPersonEmploymentDetail == null)
                LoadPersonEmploymentDetail();
            if (ibusPersonEmploymentDetail.ibusPersonEmployment == null)
                ibusPersonEmploymentDetail.LoadPersonEmployment();
            if (ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization == null)
                ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();

            // PROD PIR 4624
            icdoPersonAccount.npsp_flexcomp_change_date = DateTime.Now;
            icdoPersonAccount.npsp_flexcomp_flag = busConstant.Flag_Yes;
            icdoPersonAccount.Update();
        }

        //UAT PIR 2220
        public void PostESSMessage()
        {
            string lstrPrioityValue = string.Empty;
            if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusFlexCompEnrolled ||
                icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusFlexSuspended)
            {
                if (ibusPerson.IsNull())
                    LoadPerson();
                if (icdoPersonAccount.person_employment_dtl_id <= 0)
                    icdoPersonAccount.person_employment_dtl_id = GetEmploymentDetailID();
                // post message to employer
                if (ibusPersonEmploymentDetail == null)
                    LoadPersonEmploymentDetail();

                if (ibusPersonEmploymentDetail.ibusPersonEmployment.IsNull())
                    ibusPersonEmploymentDetail.LoadPersonEmployment();

                if (iclbFlexcompConversion == null)
                    LoadFlexCompConversion();

                if (ibusPlan == null)
                    LoadPlan();

                if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusFlexCompEnrolled)
                {
                    string lstrProviderList = string.Empty;
                    foreach (busPersonAccountFlexcompConversion lobjConversion in iclbFlexcompConversion)
                    {
                        if (lobjConversion.ibusProvider.IsNull())
                            lobjConversion.LoadProvider();
                        //there is a trim command line below, so if changing the end string need to modify there too.
                        lstrProviderList += lobjConversion.ibusProvider.icdoOrganization.org_name + ", ";
                    }
                    decimal ldecMedicalSalaryPerPayPreiod = iclbFlexCompOption.Where(lobjOption => lobjOption.icdoPersonAccountFlexCompOption.level_of_coverage_value
                        == busConstant.FlexLevelOfCoverageMedicareSpending).FirstOrDefault().icdoPersonAccountFlexCompOption.annual_pledge_amount;
                    DateTime ldtMedicalEffectiveDate = icdoPersonAccount.history_change_date;//iclbFlexCompOption.Where(lobjOption => lobjOption.icdoPersonAccountFlexCompOption.level_of_coverage_value
                        //== busConstant.FlexLevelOfCoverageMedicareSpending).FirstOrDefault().icdoPersonAccountFlexCompOption.effective_start_date;

                    decimal ldecDependentSalaryPerPayPreiod = iclbFlexCompOption.Where(lobjOption => lobjOption.icdoPersonAccountFlexCompOption.level_of_coverage_value
                        == busConstant.FlexLevelOfCoverageDependentSpending).FirstOrDefault().icdoPersonAccountFlexCompOption.annual_pledge_amount;
                    DateTime ldtDependentEffectiveDate = icdoPersonAccount.history_change_date;//iclbFlexCompOption.Where(lobjOption => lobjOption.icdoPersonAccountFlexCompOption.level_of_coverage_value
                                                                                               //== busConstant.FlexLevelOfCoverageDependentSpending).FirstOrDefault().icdoPersonAccountFlexCompOption.effective_start_date;
                    string lstrMessage = string.Format(busGlobalFunctions.GetDBMessageTextByDBMessageID(21, iobjPassInfo, ref lstrPrioityValue),
                        ibusPerson.icdoPerson.FullName, ibusPerson.icdoPerson.LastFourDigitsOfSSN);
                    if (ldecDependentSalaryPerPayPreiod > 0.0M)
                    {
                        lstrMessage += string.Format(busGlobalFunctions.GetDBMessageTextByDBMessageID(22, iobjPassInfo, ref lstrPrioityValue),
                            ldecDependentSalaryPerPayPreiod, ldtDependentEffectiveDate.ToString("d", CultureInfo.CreateSpecificCulture("en-US")));
                    }
                    if (ldecMedicalSalaryPerPayPreiod > 0.0M)
                    {
                        lstrMessage += string.Format(busGlobalFunctions.GetDBMessageTextByDBMessageID(23, iobjPassInfo, ref lstrPrioityValue),
                            ldecMedicalSalaryPerPayPreiod, ldtMedicalEffectiveDate.ToString("d", CultureInfo.CreateSpecificCulture("en-US")));
                    }
                    if (!string.IsNullOrEmpty(lstrProviderList))
                    {
                        //removing the last "," and " " before storing into message
                        lstrProviderList = lstrProviderList.TrimEnd(new char[2] { ',', ' ' });
                        lstrMessage += string.Format(busGlobalFunctions.GetDBMessageTextByDBMessageID(24, iobjPassInfo, ref lstrPrioityValue), lstrProviderList);
                    }
                    if (ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id > 0)
                    {
                        // PIR 9115
                        if (istrIsPIR9115Enabled == busConstant.Flag_No)
                        {
                            busWSSHelper.PublishESSMessage(0, 0, lstrMessage,
                                lstrPrioityValue, aintPlanID: busConstant.PlanIdFlex, aintOrgID: ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id,
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
                                                                busConstant.PlanIdFlex, iobjPassInfo);
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

        #region pir 7705
        // PIR 7705
        public int iintDentalPersonAccountID  { get; set; }
        public int iintVisionPersonAccountID { get; set; }
        public int iintLifePersonAccountID { get; set; }
        public string istrDentalPremiumFlag { get; set; }
        public string istrVisionPremiumFlag { get; set; }
        public string istrLifePremiumFlag { get; set; }
        public string istrDentalPlanStatus { get; set; }
        public string istrVisionPlanStatus { get; set; }
        public string istrLifePlanStatus { get; set; }

        // PIR 9531
        public Collection<busCodeValue> iclbPretaxPremiumElection { get; set; }

        public void LoadPretaxPremiumElection()
        {
            iclbPretaxPremiumElection = new Collection<busCodeValue>();
            busCodeValue iobjbHDVCV = new busCodeValue();
            iobjbHDVCV.icdoCodeValue = new cdoCodeValue { code_serial_id = 1, data1 = "Dental", data2 = istrDentalPlanStatus, data3 = istrDentalPremiumFlag };
            iclbPretaxPremiumElection.Add(iobjbHDVCV);

            //cdoCodeValue lcdoVision = new cdoCodeValue { code_serial_id = 2, data1 = "Vision", data2 = istrVisionPlanStatus, data3 = istrVisionPremiumFlag };
            iobjbHDVCV.icdoCodeValue = new cdoCodeValue { code_serial_id = 2, data1 = "Vision", data2 = istrVisionPlanStatus, data3 = istrVisionPremiumFlag };
            iclbPretaxPremiumElection.Add(iobjbHDVCV);
            //cdoCodeValue lcdoLife = new cdoCodeValue { code_serial_id = 3, data1 = "Life", data2 = istrLifePlanStatus, data3 = istrLifePremiumFlag };
            iobjbHDVCV.icdoCodeValue = new cdoCodeValue { code_serial_id = 3, data1 = "Life", data2 = istrLifePlanStatus, data3 = istrLifePremiumFlag };
            iclbPretaxPremiumElection.Add(iobjbHDVCV);
            //iclbPretaxPremiumElection.Add(lcdoVision);
            //iclbPretaxPremiumElection.Add(lcdoLife);
        }

        /// <summary>
        /// Loads the Premium Conversion Flags for the other 3 PERS plans - Vision Dental and Life.
        /// </summary>
        public void LoadPremiumConversionForDentalVisionLife()
        {
            istrDentalPremiumFlag = busConstant.Flag_No_Value;
            istrVisionPremiumFlag = busConstant.Flag_No_Value;
            istrLifePremiumFlag = busConstant.Flag_No_Value;
            istrDentalPlanStatus = "Not Enrolled";
            istrVisionPlanStatus = "Not Enrolled";
            istrLifePlanStatus   = "Not Enrolled";

            if (ibusPerson.IsNull())
                LoadPerson();
            ibusPerson.LoadPersonAccount();
            if (ibusPerson.icolPersonAccount.Count < 0)
                return; 
            
            busPersonAccount lbusPersonAccount = ibusPerson.icolPersonAccount.Where(i => i.icdoPersonAccount.plan_id == busConstant.PlanIdDental).FirstOrDefault();
            if(lbusPersonAccount.IsNotNull())
            {
                lbusPersonAccount.LoadPersonAccountGHDV();
                if (lbusPersonAccount.ibusPersonAccountGHDV.IsNotNull())
                {
                    istrDentalPlanStatus = lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccount.plan_participation_status_description;
                    //If plan is enrolled then store the Person Account ID. Used to provide link to plan on screen
                    if (lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                    {
                        iintDentalPersonAccountID = lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccount.person_account_id;                       
                    }
                    if (lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.premium_conversion_indicator_flag == (busConstant.Flag_Yes))
                    {
                        istrDentalPremiumFlag = busConstant.Flag_Yes_Value;
                    }
                }
            }

            lbusPersonAccount = new busPersonAccount();
            lbusPersonAccount = ibusPerson.icolPersonAccount.Where(i => i.icdoPersonAccount.plan_id == busConstant.PlanIdVision).FirstOrDefault();
            if (lbusPersonAccount.IsNotNull())
            {
                lbusPersonAccount.LoadPersonAccountGHDV();
                if (lbusPersonAccount.ibusPersonAccountGHDV.IsNotNull())
                {
                    istrVisionPlanStatus = lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccount.plan_participation_status_description;
                    if (lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                    {
                        iintVisionPersonAccountID = lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccount.person_account_id;
                    }
                    if (lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.premium_conversion_indicator_flag == (busConstant.Flag_Yes))
                    {
                        istrVisionPremiumFlag = busConstant.Flag_Yes_Value;
                    }
                }
            }

            lbusPersonAccount = new busPersonAccount();
            lbusPersonAccount = ibusPerson.icolPersonAccount.Where(i => i.icdoPersonAccount.plan_id == busConstant.PlanIdGroupLife).FirstOrDefault();
            if (lbusPersonAccount.IsNotNull())
            {
                lbusPersonAccount.LoadPersonAccountLife();
                if (lbusPersonAccount.ibusPersonAccountLife.IsNotNull())
                {
                    istrLifePlanStatus = lbusPersonAccount.ibusPersonAccountLife.icdoPersonAccount.plan_participation_status_description;
                    lbusPersonAccount.ibusPersonAccountLife.LoadLifeOptionData();
                    if (lbusPersonAccount.ibusPersonAccountLife.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                    {
                        iintLifePersonAccountID = lbusPersonAccount.ibusPersonAccountLife.icdoPersonAccountLife.person_account_id;
                    }
                    //Life should only be ‘Yes’ if Supplemental is Enrolled without End Date and premium conversion flag is yes
                    if (lbusPersonAccount.ibusPersonAccountLife.iclbLifeOption.Any(i => i.icdoPersonAccountLifeOption.coverage_amount != 0.00M && i.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_Supplemental
                            && i.icdoPersonAccountLifeOption.effective_start_date != DateTime.MinValue && i.icdoPersonAccountLifeOption.effective_end_date == DateTime.MinValue)
                            && lbusPersonAccount.ibusPersonAccountLife.icdoPersonAccountLife.premium_conversion_indicator_flag == (busConstant.Flag_Yes))
                    {
                        istrLifePremiumFlag = busConstant.Flag_Yes_Value;
                    }
                }
            }      
        }

        public bool IsHealthAccountExistsDuringMSRA()
        {
            bool lblnResult = false;
            if (ibusMSRACoverage.IsNull())
                LoadFlexCompIndividualOptions();
            if (ibusMSRACoverage.IsNotNull())
            {
                //person is enrolled in msra
                if (ibusMSRACoverage.icdoPersonAccountFlexCompOption.annual_pledge_amount > 0)
                {
                    if (ibusPerson.IsNull())
                        LoadPerson();
                    if (ibusPerson.icolPersonAccount.IsNull())
                        ibusPerson.LoadPersonAccount();
                    if (ibusPerson.icolPersonAccount.Count > 0)
                    {
                        busPersonAccount lbusPersonAccount = ibusPerson.icolPersonAccount.Where(i => i.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth).FirstOrDefault();
                        if (lbusPersonAccount.IsNotNull())
                        {
                            lbusPersonAccount.LoadPersonAccountGHDV();
                            if (lbusPersonAccount.ibusPersonAccountGHDV.IsNotNull())
                            {   // person account health exists
                                if (lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.hsa_effective_date != DateTime.MinValue
                                  && lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.alternate_structure_code_value == busConstant.AlternateStructureCodeHDHP
                                  && lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled
                                    )
                                {
                                    // check if hsa effective date falls in between the current history change date and max
                                    if (busGlobalFunctions.CheckDateOverlapping(lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.hsa_effective_date,
                                        icdoPersonAccount.history_change_date, DateTime.MaxValue
                                        ))
                                    {
                                        lblnResult = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return lblnResult;
        }

        public Collection<busPersonAccountFlexcompConversion> iclbPersonAccountFlexCompConversion { get; set; }
        /// <summary>
        /// Loads the Active Flex Comp Providers
        /// </summary>
        /// <returns>Active Flex Providers</returns>
        public Collection<cdoOrganization> LoadActiveFlexCompProviders()
        {
            if (ibusPersonAccount.IsNull())
                LoadPersonAccount();
            LoadAllPersonEmploymentDetails(); //PROD PIR 8245
            ibusPersonAccount.iclbEmploymentDetail = new Collection<busPersonEmploymentDetail>();
            ibusPersonAccount.iclbEmploymentDetail = iclbEmploymentDetail;
            return ibusPersonAccount.LoadActiveProviders();
        }

        /// <summary>
        /// Loads the premium conversion grid.
        /// </summary>
        public void LoadPersonAccountFlexCompConversion()
        {
           //lclccdoOrganization Collection holds all the eligible active flex comp providers from org table
           Collection<cdoOrganization> lclccdoOrganization =  LoadActiveFlexCompProviders();

           //iclbPersonAccountFlexCompConversion Collection is the premium conversion grid
           busPersonAccountFlexcompConversion lbusPersonAccountFlexCompConversion ;
           iclbPersonAccountFlexCompConversion = new Collection<busPersonAccountFlexcompConversion>();

           //iclbFlexcompConversion Collection holds the existing providers for the current person account from database         

           //If premium conversion already exists for the person account
           if (iclbFlexcompConversion.IsNotNull())
           {
               //PIR 8303 - Should only display the current enrollment. 
               var lenumDBPremiumConversionRecords = iclbFlexcompConversion.OrderByDescending(i => i.icdoPersonAccountFlexcompConversion.effective_start_date);
               foreach (busPersonAccountFlexcompConversion lobjPAFCC in lenumDBPremiumConversionRecords)
               {
                   //Add to premium conversion grid only if the org is in lclccdoOrganization
                   if (lclccdoOrganization.Any(i => i.org_id == lobjPAFCC.icdoPersonAccountFlexcompConversion.org_id) 
                      && !iclbPersonAccountFlexCompConversion.Any(i=>i.icdoPersonAccountFlexcompConversion.org_id == lobjPAFCC.icdoPersonAccountFlexcompConversion.org_id) )//pir 8303
                   {
                       //Loading required for IsValidProvider
                       lobjPAFCC.LoadProvider();
                       lobjPAFCC.LoadPersonAccount();
                       //Check if the provider is valid and if they offer any other plans apart from Flex
                       if (lobjPAFCC.IsValidProvider())
                       {
                           // Add provider to grid only if they do not offer any other PERS plans - Vision , Dental or Life.
                           if (!lobjPAFCC.iblnHaveVisionDentalOrLifePlan)
                               iclbPersonAccountFlexCompConversion.Add(lobjPAFCC); 
                       }
                   }
               }
           }

           //Append the remaining eligible organizations to the premium conversion grid
           foreach(cdoOrganization lcdoOrganization in lclccdoOrganization)
           {
               //If the org is not already added to the Grid
               if (!iclbPersonAccountFlexCompConversion.Any(i => i.icdoPersonAccountFlexcompConversion.org_id == lcdoOrganization.org_id))
               {
                   lbusPersonAccountFlexCompConversion = new busPersonAccountFlexcompConversion { icdoPersonAccountFlexcompConversion = new cdoPersonAccountFlexcompConversion() };
                   lbusPersonAccountFlexCompConversion.icdoPersonAccountFlexcompConversion.org_id = lcdoOrganization.org_id;
                   //Loading required for IsValidProvider
                   lbusPersonAccountFlexCompConversion.LoadProvider();
                   lbusPersonAccountFlexCompConversion.LoadPersonAccount();
                   //Check if the provider is valid and if they offer any other plans apart from Flex
                   if (lbusPersonAccountFlexCompConversion.IsValidProvider())
                   {
                       // Add provider to grid only if they do not offer any other PERS plans - Vision , Dental or Life.
                       if(!lbusPersonAccountFlexCompConversion.iblnHaveVisionDentalOrLifePlan)
                            iclbPersonAccountFlexCompConversion.Add(lbusPersonAccountFlexCompConversion);
                   }
               }
           }
        }

        /// <summary>
        /// Loads MSRA , DCRA Coverage Amounts for display
        /// </summary>
        public void LoadFlexCompIndividualOptions()
        {
            if (iclbFlexCompOption.Count > 0)
            {
                ibusMSRACoverage = iclbFlexCompOption.Where(i => i.icdoPersonAccountFlexCompOption.level_of_coverage_value.Equals(busConstant.FlexLevelOfCoverageMedicareSpending)).FirstOrDefault();
                ibusDCRACoverage = iclbFlexCompOption.Where(i => i.icdoPersonAccountFlexCompOption.level_of_coverage_value.Equals(busConstant.FlexLevelOfCoverageDependentSpending)).FirstOrDefault();
                if (ibusMSRACoverage.IsNotNull())
                    icdoPersonAccountFlexComp.msra_pledge_amount = ibusMSRACoverage.icdoPersonAccountFlexCompOption.annual_pledge_amount;
                if (ibusDCRACoverage.IsNotNull())
                    icdoPersonAccountFlexComp.dcra_pledge_amount = ibusDCRACoverage.icdoPersonAccountFlexCompOption.annual_pledge_amount;
            }
        }

        #region Modified Flex Comp History Methods

        /// <summary>
        /// Method to Load Modified Flex Comp History - To Merge MSRA DCRA amounts
        /// The flex_comp_history table contains the Coverage amounts in row format .
        /// For eg , [Updated Options History Logic] 
        /// StartDate   End Date    Amt     Coverage
        /// -----------------------------------------
        /// 1/1/2010	1/31/2010	0	    DCRA
        /// 1/1/2010	1/31/2010	200	    MSRA
        /// 2/1/2010	2/1/2010	10	    DCRA
        /// 2/1/2010	2/1/2010	4000	MSRA
        /// 2/1/2010    2/28/2010   100     DCRA
        /// 2/1/2010    2/28/2010   4000    MSRA
        /// 3/1/2010                2000    DCRA
        /// 3/1/2010                100     MSRA
        /// 
        /// The Modifed history in the screen will be displayed as
        /// StartDate   End Date    MSRA    DCRA
        /// ------------------------------------
        /// 1/1/2010	1/31/2010	200	    0
        /// 2/1/2010	2/1/2010	4000	10
        /// 2/1/2010    2/28/2010   4000    100
        /// 3/1/2010                100     2000
        /// </summary>
        public void LoadModifiedFlexCompHistory()
        {
            //Load Flex Comp History from Table
            LoadFlexCompHistory();
            //Modified History to be displayed
            iclbModifiedHistory = new Collection<busPersonAccountFlexCompHistory>();
            //Holds All End Dated History Records
            IEnumerable<busPersonAccountFlexCompHistory> templclbFlexHistoryEndDated = iclbFlexCompHistory.Where(i => i.icdoPersonAccountFlexCompHistory.effective_end_date != DateTime.MinValue);
            //Hold all Non End Dated History Records
            IEnumerable<busPersonAccountFlexCompHistory> templclbFlexHistoryNonEndDated = iclbFlexCompHistory.Where(i => i.icdoPersonAccountFlexCompHistory.effective_end_date == DateTime.MinValue);

            templclbFlexHistoryEndDated = templclbFlexHistoryEndDated.OrderBy(i => i.icdoPersonAccountFlexCompHistory.person_account_flex_comp_history_id);
            templclbFlexHistoryNonEndDated = templclbFlexHistoryNonEndDated.OrderBy(i => i.icdoPersonAccountFlexCompHistory.person_account_flex_comp_history_id);
            
            //Merge End Dated History Records
            if (templclbFlexHistoryEndDated.IsNotNull())
                MergeEndDatedHistory(templclbFlexHistoryEndDated);   

            if(iclbModifiedHistory.IsNotNull())
                iclbModifiedHistory.OrderByDescending(i => i.icdoPersonAccountFlexCompHistory.effective_end_date);
            
            //Add non end dated History Records
            if (templclbFlexHistoryNonEndDated.IsNotNull())
                InsertNonEndDatedHistory(templclbFlexHistoryNonEndDated);

            if (iclbModifiedHistory.IsNotNull())
                 iclbModifiedHistory.OrderByDescending(i => i.icdoPersonAccountFlexCompHistory.person_account_flex_comp_history_id);
        }

        /// <summary>
        /// Inserts end Dated History Records
        /// </summary>
        /// <param name="aclbFlexHistoryEndDated"></param>
        private void MergeEndDatedHistory(IEnumerable<busPersonAccountFlexCompHistory> aclbFlexHistoryEndDated)
        {
            var lenumTimePeriods = aclbFlexHistoryEndDated.Select(history => new { history.icdoPersonAccountFlexCompHistory.effective_start_date, history.icdoPersonAccountFlexCompHistory.effective_end_date }).Distinct();
            foreach (var lobjEndDatedHistory in lenumTimePeriods)
            {
                //Get all History records within the Start Date and End Date - Minimum one record will exist . Maximimum unknown
                var lenumHistoryInTimePeriod = aclbFlexHistoryEndDated.Where(i => i.icdoPersonAccountFlexCompHistory.effective_start_date == lobjEndDatedHistory.effective_start_date
                                                            && i.icdoPersonAccountFlexCompHistory.effective_end_date == lobjEndDatedHistory.effective_end_date);
                lenumHistoryInTimePeriod = lenumHistoryInTimePeriod.OrderBy(i => i.icdoPersonAccountFlexCompHistory.person_account_flex_comp_history_id);

                var lenumMSRA = lenumHistoryInTimePeriod.Where(i => i.icdoPersonAccountFlexCompHistory.level_of_coverage_value == busConstant.FlexLevelOfCoverageMedicareSpending);
                var lenumDCRA = lenumHistoryInTimePeriod.Where(i => i.icdoPersonAccountFlexCompHistory.level_of_coverage_value == busConstant.FlexLevelOfCoverageDependentSpending);
                int lintMSRACount = lenumMSRA.Count();
                int lintDCRACount = lenumDCRA.Count();

                //Only MSRA or only DCRA exists - Add all history records as individual rows 
                if (lintDCRACount == 0 || lintMSRACount == 0)
                {
                    if (lintMSRACount > 0)
                        InsertSingleCoverage(lenumMSRA);
                    else if (lintDCRACount > 0)
                        InsertSingleCoverage(lenumDCRA);
                }
                //Both MSRA and DCRA exist
                else if (lintMSRACount > 0 && lintDCRACount > 0)
                {
                    //Equal Number of MSRA and DCRA records - Normal Scenario : Max 2 Records per time period. MSRA = 1, DCRA = 1
                    if (lintMSRACount == lintDCRACount)
                        MergeEqualCoverageValues(lenumHistoryInTimePeriod);
                    //Unequal Number - For legacy data - 
                    else if (lintDCRACount != lintMSRACount)
                        InsertUnequalCoverageValues(lenumHistoryInTimePeriod);
                }
            }
        }

        /// <summary>
        /// STEP 1. Determine if Non End Dated History Record lies in a time period in Modified History
        /// STEP 2. For each time period , update the modified history record's msra/dcra annual pledge amount with the non end dated history record
        /// STEP 3. If no such time period is present in Modified History , insert new Row in Modified History
        /// Actual History Data
        /// StartDate   End Date    Amt     Coverage
        /// -----------------------------------------
        /// 1/1/2010                400     DCRA
        /// 1/1/2010	1/31/2010	0	    MSRA
        /// 2/1/2010	        	200	    MSRA
        /// 
        /// Before this method. Modified History will be
        /// StartDate   End Date    MSRA    DCRA
        /// ------------------------------------
        /// 1/1/2010	1/31/2010	0       0
        /// 
        /// This method will update it as follows
        /// StartDate   End Date    MSRA    DCRA
        /// ------------------------------------
        /// 1/1/2010	1/31/2010	0       400
        /// 2/1/2010                200     400
        /// </summary>
        /// <param name="aclbFlexHistoryNonEndDated"></param>
        private void InsertNonEndDatedHistory(IEnumerable<busPersonAccountFlexCompHistory> aclbFlexHistoryNonEndDated)
        {
            foreach (busPersonAccountFlexCompHistory lobjNonEndDatedHistory in aclbFlexHistoryNonEndDated)
            {
                bool lblnInserted = false;
                
                foreach (busPersonAccountFlexCompHistory lbusModifiedHistory in iclbModifiedHistory)
                {
                    //STEP 1
                    if (lobjNonEndDatedHistory.icdoPersonAccountFlexCompHistory.effective_start_date <= lbusModifiedHistory.icdoPersonAccountFlexCompHistory.effective_start_date)
                    {
                        //STEP 2 
                        if (lobjNonEndDatedHistory.icdoPersonAccountFlexCompHistory.level_of_coverage_value == busConstant.FlexLevelOfCoverageMedicareSpending)
                        {
                            if (lbusModifiedHistory.iblnMsraAdded == false)
                            {
                                lbusModifiedHistory.idecMsraAnnualPledgeAmount = lobjNonEndDatedHistory.icdoPersonAccountFlexCompHistory.annual_pledge_amount;
                                lbusModifiedHistory.iblnMsraAdded = true;
                                lblnInserted = true;
                            }
                        }
                        else if (lobjNonEndDatedHistory.icdoPersonAccountFlexCompHistory.level_of_coverage_value == busConstant.FlexLevelOfCoverageDependentSpending)
                        {
                            if (lbusModifiedHistory.iblnDcraAdded == false)
                            {
                                lbusModifiedHistory.idecDcraAnnualPledgeAmount = lobjNonEndDatedHistory.icdoPersonAccountFlexCompHistory.annual_pledge_amount;
                                lbusModifiedHistory.iblnDcraAdded = true;
                                lblnInserted = true;
                            }
                        }
                    }
                }
                //STEP 3
                if (lblnInserted == false)
                {
                    if (lobjNonEndDatedHistory.icdoPersonAccountFlexCompHistory.level_of_coverage_value == busConstant.FlexLevelOfCoverageMedicareSpending)
                    {
                        lobjNonEndDatedHistory.idecMsraAnnualPledgeAmount = lobjNonEndDatedHistory.icdoPersonAccountFlexCompHistory.annual_pledge_amount;
                        lobjNonEndDatedHistory.iblnMsraAdded = true;
                    }
                    else if (lobjNonEndDatedHistory.icdoPersonAccountFlexCompHistory.level_of_coverage_value == busConstant.FlexLevelOfCoverageDependentSpending)
                    {
                        lobjNonEndDatedHistory.idecDcraAnnualPledgeAmount = lobjNonEndDatedHistory.icdoPersonAccountFlexCompHistory.annual_pledge_amount;
                        lobjNonEndDatedHistory.iblnDcraAdded = true;
                    }
                    iclbModifiedHistory.Add(lobjNonEndDatedHistory);
                }
            }   
        }

        /// <summary>
        /// Actual History Data
        /// StartDate   End Date    Amt     Coverage
        /// -----------------------------------------
        /// 1/1/2010	1/31/2010	10	    MSRA
        /// 2/1/2010	2/28/2010	200	    MSRA
        /// 
        /// This method will insert it as follows. No merges. Direct Insertions.
        /// StartDate   End Date    MSRA    DCRA
        /// ------------------------------------
        /// 1/1/2010	1/31/2010	10	    0
        /// 2/1/2010	2/28/2010	200	    0
        /// </summary>
        /// <param name="aenumCoverage"></param>
        private void InsertSingleCoverage(IEnumerable<busPersonAccountFlexCompHistory> aenumCoverage)
        {
            if(aenumCoverage.Any(i => i.icdoPersonAccountFlexCompHistory.level_of_coverage_value == busConstant.FlexLevelOfCoverageMedicareSpending))
            {
                foreach (busPersonAccountFlexCompHistory lobjPAFC in aenumCoverage)
                {
                    lobjPAFC.idecMsraAnnualPledgeAmount = lobjPAFC.icdoPersonAccountFlexCompHistory.annual_pledge_amount;
                    lobjPAFC.iblnMsraAdded = true;
                    lobjPAFC.iblnDcraAdded = false;
                    iclbModifiedHistory.Add(lobjPAFC);
                }
            }
            else if (aenumCoverage.Any(i => i.icdoPersonAccountFlexCompHistory.level_of_coverage_value == busConstant.FlexLevelOfCoverageDependentSpending))
            {
                foreach (busPersonAccountFlexCompHistory lobjPAFC in aenumCoverage)
                {
                    lobjPAFC.idecDcraAnnualPledgeAmount = lobjPAFC.icdoPersonAccountFlexCompHistory.annual_pledge_amount;
                    lobjPAFC.iblnDcraAdded = true;
                    lobjPAFC.iblnMsraAdded = false;
                    iclbModifiedHistory.Add(lobjPAFC);
                }
            }
        }

        /// <summary>
        /// [For history data before InsertHistory Logic Change]
        /// Actual History Data - 2 dcra , 1 msra
        /// StartDate   End Date    Amt     Coverage
        /// -----------------------------------------
        /// 1/1/2010	1/31/2010	0	    DCRA
        /// 1/1/2010	12/31/2010	200	    MSRA
        /// 2/1/2010    12/31/2010  300     DCRA
        /// 
        /// This method will merge it as follows. 
        /// StartDate   End Date    MSRA    DCRA
        /// ------------------------------------
        /// 1/1/2010	1/31/2010	200	    0
        /// 2/1/2010    12/31/2010  200     300
        /// </summary>
        /// <param name="aenumHistoryInTimePeriod"></param>
        private void InsertUnequalCoverageValues(IEnumerable<busPersonAccountFlexCompHistory> aenumHistoryInTimePeriod)
        {
            busPersonAccountFlexCompHistory lobjPrevFlexCompHistory = new busPersonAccountFlexCompHistory { icdoPersonAccountFlexCompHistory = new cdoPersonAccountFlexCompHistory() };
            foreach (busPersonAccountFlexCompHistory lbusHistory in aenumHistoryInTimePeriod)
            {
                if (lbusHistory.icdoPersonAccountFlexCompHistory.level_of_coverage_value == busConstant.FlexLevelOfCoverageMedicareSpending)
                {
                    lbusHistory.idecMsraAnnualPledgeAmount = lbusHistory.icdoPersonAccountFlexCompHistory.annual_pledge_amount;
                    lbusHistory.iblnMsraAdded = true;
                    lbusHistory.iblnDcraAdded = false;
                    //Carry over previous History record DCRA value
                    if (lobjPrevFlexCompHistory.icdoPersonAccountFlexCompHistory.level_of_coverage_value == busConstant.FlexLevelOfCoverageDependentSpending)
                    {
                        lbusHistory.idecDcraAnnualPledgeAmount = lbusHistory.icdoPersonAccountFlexCompHistory.annual_pledge_amount;
                        lbusHistory.iblnDcraAdded = true;
                    }
                    lobjPrevFlexCompHistory = lbusHistory;
                    iclbModifiedHistory.Add(lbusHistory);
                }
                else if (lbusHistory.icdoPersonAccountFlexCompHistory.level_of_coverage_value == busConstant.FlexLevelOfCoverageDependentSpending)
                {
                    lbusHistory.idecDcraAnnualPledgeAmount = lbusHistory.icdoPersonAccountFlexCompHistory.annual_pledge_amount;
                    lbusHistory.iblnDcraAdded = true;
                    lbusHistory.iblnMsraAdded = false;
                    //Carry over previous History record MSRA value
                    if (lobjPrevFlexCompHistory.icdoPersonAccountFlexCompHistory.level_of_coverage_value == busConstant.FlexLevelOfCoverageMedicareSpending)
                    {
                        lbusHistory.idecMsraAnnualPledgeAmount = lbusHistory.icdoPersonAccountFlexCompHistory.annual_pledge_amount;
                        lbusHistory.iblnMsraAdded = true;
                    }
                    lobjPrevFlexCompHistory = lbusHistory;
                    iclbModifiedHistory.Add(lbusHistory);
                }
            }
        }

        /// <summary>
        /// Normal Scenario.[After InsertHistory Logic Change]
        /// Actual History Data
        /// StartDate   End Date    Amt     Coverage
        /// -----------------------------------------
        /// 1/1/2010	1/31/2010	0	    DCRA
        /// 1/1/2010	1/31/2010	200	    MSRA
        /// 
        /// This method will merge it as follows.
        /// StartDate   End Date    MSRA    DCRA
        /// ------------------------------------
        /// 1/1/2010	1/31/2010	200	    0
        /// </summary>
        /// <param name="aenumHistoryInTimePeriod"></param>
        private void MergeEqualCoverageValues(IEnumerable<busPersonAccountFlexCompHistory> aenumHistoryInTimePeriod)
        {
            bool lblnFirstHistoryInsert = true;
            foreach (busPersonAccountFlexCompHistory lbusHistory in aenumHistoryInTimePeriod)
            {
                //First History record. Store it to merge with the next.Initialise Flags
                if (lblnFirstHistoryInsert)
                {
                    ibusHistory = lbusHistory;
                    lblnFirstHistoryInsert = false;
                    ibusHistory.iblnMsraAdded = false;
                    ibusHistory.iblnDcraAdded = false;
                }
                if (lbusHistory.icdoPersonAccountFlexCompHistory.level_of_coverage_value == busConstant.FlexLevelOfCoverageMedicareSpending)
                {
                    //If No Msra Added. Can be merged with existing history
                    if (ibusHistory.iblnMsraAdded == false)
                    {
                        ibusHistory.idecMsraAnnualPledgeAmount = lbusHistory.icdoPersonAccountFlexCompHistory.annual_pledge_amount;
                        ibusHistory.iblnMsraAdded = true;
                    }
                }
                else if (lbusHistory.icdoPersonAccountFlexCompHistory.level_of_coverage_value == busConstant.FlexLevelOfCoverageDependentSpending)
                {
                    //No Dcra Added. Can be merged with existing history
                    if (ibusHistory.iblnDcraAdded == false)
                    {
                        ibusHistory.idecDcraAnnualPledgeAmount = lbusHistory.icdoPersonAccountFlexCompHistory.annual_pledge_amount;
                        ibusHistory.iblnDcraAdded = true;
                    }
                }
                if (ibusHistory.iblnDcraAdded && ibusHistory.iblnMsraAdded)
                {
                    iclbModifiedHistory.Add(ibusHistory);
                    lblnFirstHistoryInsert = true;
                    ibusHistory = new busPersonAccountFlexCompHistory();
                }
            }
        }
        
        #endregion
       
        
        #endregion

        #region FSA File logic

        public string istrHealthHireDate { get; set; }
        public string istrHealthTermDate { get; set; }
        public string istrDentalHireDate { get; set; }
        public string istrDentalTermDate { get; set; }
        public string istrVisionHireDate { get; set; }
        public string istrVisionTermDate { get; set; }

        /// <summary>
        /// Loads the Premium Conversion Flags for the other 3 PERS plans - Vision Dental and Life.
        /// </summary>
        public void LoadHDVEnrollmentDates(DateTime adteHDVStartDate, DateTime adteHDVEndDate)
        {
            DateTime ldteDentalHireDate = new DateTime();
            DateTime ldteDentalTermDate = new DateTime();
            DateTime ldteHealthHireDate = new DateTime();
            DateTime ldteHealthTermDate = new DateTime();
            DateTime ldteVisionHireDate = new DateTime();
            DateTime ldteVisionTermDate = new DateTime();

            if (ibusPerson.IsNull()) LoadPerson();
            if(ibusPerson.icolPersonAccount.IsNull()) ibusPerson.LoadPersonAccount();
            if (ibusPerson.icolPersonAccount.Count < 0)
                return;

            busPersonAccount lbusPersonAccount = ibusPerson.icolPersonAccount.Where(i => i.icdoPersonAccount.plan_id == busConstant.PlanIdDental).FirstOrDefault();
            if (lbusPersonAccount.IsNotNull())
            {
                lbusPersonAccount.LoadPersonAccountGHDV();
                if (lbusPersonAccount.ibusPersonAccountGHDV.IsNotNull())
                {
                    lbusPersonAccount.ibusPersonAccountGHDV.LoadPersonAccountGHDVHistory();
                    int lintCounter = 0; bool lblnPrevHistoryEnrolled = false;
                    foreach (busPersonAccountGhdvHistory lobjHistory in lbusPersonAccount.ibusPersonAccountGHDV.iclbPersonAccountGHDVHistory)
                    {
                        if (lobjHistory.icdoPersonAccountGhdvHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                        {
                            ldteDentalHireDate = lobjHistory.icdoPersonAccountGhdvHistory.start_date;
                            if (lintCounter == 0)
                                ldteDentalTermDate = lobjHistory.icdoPersonAccountGhdvHistory.end_date;
                            lblnPrevHistoryEnrolled = true;
                            lintCounter++;
                        }
                        else
                        {
                            if (lblnPrevHistoryEnrolled)
                                break;
                        }
                    }
                }

                if (ldteDentalTermDate != DateTime.MinValue)
                    istrDentalTermDate = ldteDentalTermDate.ToString(busConstant.DateFormatYearMonthDay);
                if ((istrDentalTermDate.IsNotNull() && Convert.ToDateTime(istrDentalTermDate).Year >= adteHDVStartDate.Year) || Convert.ToDateTime(istrDentalTermDate) == DateTime.MinValue)
                {
                    istrDentalTermDate = (ldteDentalTermDate != DateTime.MinValue && adteHDVEndDate != DateTime.MinValue) ?
                                        busGlobalFunctions.GetMin(ldteDentalTermDate, adteHDVEndDate).ToString(busConstant.DateFormatYearMonthDay) : string.Empty;
                    istrDentalHireDate = lbusPersonAccount.icdoPersonAccount.start_date.ToString(busConstant.DateFormatYearMonthDay);
                }
                else
                {
                    istrDentalTermDate = null;
                    istrDentalHireDate = null;
                }
            }
            
            lbusPersonAccount = ibusPerson.icolPersonAccount.Where(i => i.icdoPersonAccount.plan_id == busConstant.PlanIdVision).FirstOrDefault();
            if (lbusPersonAccount.IsNotNull())
            {
                lbusPersonAccount.LoadPersonAccountGHDV();
                if (lbusPersonAccount.ibusPersonAccountGHDV.IsNotNull())
                {
                    lbusPersonAccount.ibusPersonAccountGHDV.LoadPersonAccountGHDVHistory();
                    int lintCounter = 0; bool lblnPrevHistoryEnrolled = false;
                    foreach (busPersonAccountGhdvHistory lobjHistory in lbusPersonAccount.ibusPersonAccountGHDV.iclbPersonAccountGHDVHistory)
                    {
                        if (lobjHistory.icdoPersonAccountGhdvHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                        {
                            ldteVisionHireDate = lobjHistory.icdoPersonAccountGhdvHistory.start_date;
                            if (lintCounter == 0)
                                ldteVisionTermDate = lobjHistory.icdoPersonAccountGhdvHistory.end_date;
                            lblnPrevHistoryEnrolled = true;
                            lintCounter++;
                        }
                        else
                        {
                            if (lblnPrevHistoryEnrolled)
                                break;
                        }
                    }
                }

                if (ldteVisionTermDate != DateTime.MinValue)
                    istrVisionTermDate = ldteVisionTermDate.ToString(busConstant.DateFormatYearMonthDay);
                if ((istrVisionTermDate.IsNotNull() && Convert.ToDateTime(istrVisionTermDate).Year >= adteHDVStartDate.Year) || Convert.ToDateTime(istrVisionTermDate) == DateTime.MinValue)
                {
                    istrVisionTermDate = (ldteVisionTermDate != DateTime.MinValue && adteHDVEndDate != DateTime.MinValue) ?
                                        busGlobalFunctions.GetMin(ldteVisionTermDate, adteHDVEndDate).ToString(busConstant.DateFormatYearMonthDay) : string.Empty;
                    istrVisionHireDate = lbusPersonAccount.icdoPersonAccount.start_date.ToString(busConstant.DateFormatYearMonthDay);
                }
                else
                {
                    istrVisionTermDate = null;
                    istrVisionHireDate = null;
                }
            }
            lbusPersonAccount = ibusPerson.icolPersonAccount.Where(i => i.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth).FirstOrDefault();
            if (lbusPersonAccount.IsNotNull())
            {
                lbusPersonAccount.LoadPersonAccountGHDV();
                if (lbusPersonAccount.ibusPersonAccountGHDV.IsNotNull())
                {
                    lbusPersonAccount.ibusPersonAccountGHDV.LoadPersonAccountGHDVHistory();
                    int lintCounter = 0; bool lblnPrevHistoryEnrolled = false;
                    foreach (busPersonAccountGhdvHistory lobjHistory in lbusPersonAccount.ibusPersonAccountGHDV.iclbPersonAccountGHDVHistory)
                    {
                        if (lobjHistory.icdoPersonAccountGhdvHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                        {
                            ldteHealthHireDate = lobjHistory.icdoPersonAccountGhdvHistory.start_date;
                            if (lintCounter == 0)
                                ldteHealthTermDate = lobjHistory.icdoPersonAccountGhdvHistory.end_date;
                            lblnPrevHistoryEnrolled = true;
                            lintCounter++;
                        }
                        else
                        {
                            if (lblnPrevHistoryEnrolled)
                                break;
                        }
                    }
                }

                if (ldteHealthTermDate != DateTime.MinValue)
                    istrHealthTermDate = ldteHealthTermDate.ToString(busConstant.DateFormatYearMonthDay);
                if ((istrHealthTermDate.IsNotNull() && Convert.ToDateTime(istrHealthTermDate).Year >= adteHDVStartDate.Year) || Convert.ToDateTime(istrHealthTermDate) == DateTime.MinValue)
                {
                    istrHealthTermDate = (ldteHealthTermDate != DateTime.MinValue && adteHDVEndDate != DateTime.MinValue) ?
                                        busGlobalFunctions.GetMin(ldteHealthTermDate, adteHDVEndDate).ToString(busConstant.DateFormatYearMonthDay) : string.Empty;
                    istrHealthHireDate = lbusPersonAccount.icdoPersonAccount.start_date.ToString(busConstant.DateFormatYearMonthDay);
                }
                else
                {
                    istrHealthTermDate = null;
                    istrHealthHireDate = null;
                }
            }
        }

        #endregion

        private Collection<busPersonAccountFlexCompHistory> LoadOverlappingHistory()
        {
            if (iclbFlexCompHistory == null)
                LoadFlexCompHistory();
            Collection<busPersonAccountFlexCompHistory> lclbPAGHDVHistory = new Collection<busPersonAccountFlexCompHistory>();
            var lenuList = iclbFlexCompHistory.Where(i => busGlobalFunctions.CheckDateOverlapping(icdoPersonAccount.history_change_date,
                i.icdoPersonAccountFlexCompHistory.effective_start_date, i.icdoPersonAccountFlexCompHistory.effective_end_date)
                || i.icdoPersonAccountFlexCompHistory.effective_start_date > icdoPersonAccount.history_change_date);
            foreach (busPersonAccountFlexCompHistory lobjHistory in lenuList)
            {
                if (lobjHistory.icdoPersonAccountFlexCompHistory.effective_start_date >= icdoPersonAccount.history_change_date)
                {
                    lclbPAGHDVHistory.Add(lobjHistory);
                }
                else if (lobjHistory.icdoPersonAccountFlexCompHistory.effective_start_date == lobjHistory.icdoPersonAccountFlexCompHistory.effective_end_date)
                {
                    lclbPAGHDVHistory.Add(lobjHistory);
                }
                else if (lobjHistory.icdoPersonAccountFlexCompHistory.effective_start_date != lobjHistory.icdoPersonAccountFlexCompHistory.effective_end_date)
                {
                    break;
                }

            }
            return lclbPAGHDVHistory;
        }

        //PIR 23167, 23340, 23408
        public bool IsMoreThanOneEnrolledInOverlapHistory()
        {
            if (istrAllowOverlapHistory == busConstant.Flag_Yes)
            {
                if (iclbOverlappingHistory != null)
                {
                    var lenuList = iclbOverlappingHistory.Where(i => i.icdoPersonAccountFlexCompHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled &&
                        i.icdoPersonAccountFlexCompHistory.start_date != i.icdoPersonAccountFlexCompHistory.end_date);
                    
                    if (IsResourceForEnhancedOverlap(busConstant.OverlapResourceEnhancedFlex))
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
        public bool IsMemberEligibleToEnrollInFlexInCurrentYear()
        {
            decimal ldecNewFlexMSRAAmount = 0.00m;
            ldecNewFlexMSRAAmount = Decimal.Parse(ibusMSRACoverage.IsNull() || (ibusMSRACoverage.IsNotNull() && ibusMSRACoverage.istrAnnualPledgeAmount.IsNullOrEmpty()) ? "0.00" : ibusMSRACoverage.istrAnnualPledgeAmount);
            decimal ldecNewFlexDCRAAmount = 0.00m;
            ldecNewFlexDCRAAmount = Decimal.Parse(ibusDCRACoverage.IsNull() || (ibusDCRACoverage.IsNotNull() && ibusDCRACoverage.istrAnnualPledgeAmount.IsNullOrEmpty()) ? "0.00" : ibusDCRACoverage.istrAnnualPledgeAmount);

            return icdoPersonAccount.IsNotNull() && icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusFlexCompEnrolled
                && IsMemberEligibleToEnrollInCurrentYear(icdoPersonAccount.history_change_date, icdoPersonAccountFlexComp.reason_value, adecFlexMSRAAmount: ldecNewFlexMSRAAmount, adecFlexDCRAAmount: ldecNewFlexDCRAAmount);
        }

        //PIR 26579
        public Collection<busPersonAccount> iclbPreTaxEnrollmentStatusInPreviousYears { get; set; }
        public int iintCY { get; set; }

        public void LoadPreTaxEnrollmentStatusGrid()
        {
            if (ibusPerson.IsNull())
                LoadPerson();
            //if member has a future enrollment, suppose from AE for next year the grid needs to start from the next year onwards to 4 years back
            iintCY = Convert.ToInt32(DBFunction.DBExecuteScalar("entPersonAccountFlexComp.GetLatestCY", new object[1]{ ibusPerson.icdoPerson.person_id }, iobjPassInfo.iconFramework,
                        iobjPassInfo.itrnFramework));
            //if member doesnt have any person account then the CY from the query is null/0. in that case we set the variable to be the current year.
            if (iintCY == 0)
            {
                iintCY = DateTime.Now.Year;
            }

            iclbPreTaxEnrollmentStatusInPreviousYears = new Collection<busPersonAccount>();
            //get data from new query and set the properties here
            DataTable ldtResult = DBFunction.DBSelect("entPersonAccountFlexComp.GetPreTaxEnrollmentStatusForPlans", new object[2] { ibusPerson.icdoPerson.person_id, iintCY }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            if (ldtResult.IsNotNull())
            {
                foreach (DataRow ldr in ldtResult.Rows)
                {
                    busPersonAccount lobjPA = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
                    lobjPA.icdoPersonAccount.LoadData(ldr);
                    iclbPreTaxEnrollmentStatusInPreviousYears.Add(lobjPA);
                }
            }
        }
        public bool IsMemberEligibleToPreTaxProviders()
        {
            //PIR 26356
            if (iclbPersonAccountFlexCompConversion.IsNull())
                LoadPersonAccountFlexCompConversion();
            foreach (busPersonAccountFlexcompConversion lobjPAFlexCompConv in iclbPersonAccountFlexCompConversion)
            {
                if (ibusMSRACoverage.istrAnnualPledgeAmount.IsNullOrEmpty() && ibusDCRACoverage.istrAnnualPledgeAmount.IsNullOrEmpty() &&
                    (lobjPAFlexCompConv.icdoPersonAccountFlexcompConversion.effective_start_date != DateTime.MinValue))
                {
                    return true;
                }
            }
            return false;
        }
        //PIR 26428 26356 - Check if member enrolled in flex for current year
        public bool IsFlexCompEnrolledInCurrentYear(int aintCurrentYear)
        {
            if (iclbFlexCompHistory.IsNull())
                LoadFlexCompHistory();

            return iclbFlexCompHistory.Where(i => (i.icdoPersonAccountFlexCompHistory.effective_start_date != i.icdoPersonAccountFlexCompHistory.effective_end_date &&
                            (i.icdoPersonAccountFlexCompHistory.effective_end_date == DateTime.MinValue || i.icdoPersonAccountFlexCompHistory.effective_end_date.Year == aintCurrentYear))         
                  && i.icdoPersonAccountFlexCompHistory.plan_participation_status_value == busConstant.PlanParticipationStatusFlexCompEnrolled).Any();
        }
    }
}
