using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using NeoSpin.BusinessObjects;
using Sagitec.BusinessObjects;
using System.Collections.ObjectModel;
using System.Collections;
using Sagitec.CustomDataObjects;
using NeoSpin.CustomDataObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
namespace NeoSpinBatch
{
    class busEndFlexCompEnrollment : busNeoSpinBatch
    {
        public DateTime idtBatchRunDate { get; set; }
        public void EndFlexCompEnrollment()
        {
            idtBatchRunDate = iobjSystemManagement.icdoSystemManagement.batch_date;
            istrProcessName = "End Flex Comp Enrollment Batch";
            idlgUpdateProcessLog("Loading all flex comp person accounts that are not end dated", "INFO", istrProcessName);
            DataTable ldtbPersonAccountFlexComp = busBase.Select("cdoPersonAccountFlexComp.GetAllNonEndDatePersonAccountFlexCompByYear",
                new object[1] { idtBatchRunDate.Year });
            if (ldtbPersonAccountFlexComp.Rows.Count > 0)
            {
                foreach (DataRow dr in ldtbPersonAccountFlexComp.Rows)
                {

                    busPersonAccount lobjPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
                    lobjPersonAccount.icdoPersonAccount.LoadData(dr);
                    lobjPersonAccount.LoadPersonAccountFlex();
                    busPersonAccountFlexComp lobjPersonAccountFlexComp = lobjPersonAccount.ibusPersonAccountFlex;
                
                    LoadFlexCompObjects(ref lobjPersonAccountFlexComp);
                   
                    /// PREMIUM CONVERSION FOR DENTAL AND VISION 
                    ///If the member had Premium Conversion but no entry was made for Annual Enrollment 
                    /// we need to insert a new history line for the same coverage but without Premium Conversion and flag it as ANNE.
                    lobjPersonAccountFlexComp.LoadPremiumConversionForDentalVisionLife();
                 
                    if (lobjPersonAccountFlexComp.istrDentalPremiumFlag == "Yes" && lobjPersonAccountFlexComp.istrDentalPlanStatus == "Enrolled")
                    {
                        busPersonAccount lbusPersonAccount = lobjPersonAccountFlexComp.ibusPerson.icolPersonAccount.Where(i => i.icdoPersonAccount.plan_id == busConstant.PlanIdDental).FirstOrDefault();
                        lbusPersonAccount.LoadPersonAccountGHDV();
                        if (lbusPersonAccount.ibusPersonAccountGHDV != null)
                        {
                            CheckIfHistoryEndDated(lbusPersonAccount.ibusPersonAccountGHDV, "Dental");
                        }
                    }
                    if (lobjPersonAccountFlexComp.istrVisionPremiumFlag == "Yes" && lobjPersonAccountFlexComp.istrVisionPlanStatus == "Enrolled")
                    {
                        busPersonAccount lbusPersonAccount = lobjPersonAccountFlexComp.ibusPerson.icolPersonAccount.Where(i => i.icdoPersonAccount.plan_id == busConstant.PlanIdVision).FirstOrDefault();
                        lbusPersonAccount.LoadPersonAccountGHDV();
                        if (lbusPersonAccount.ibusPersonAccountGHDV != null)
                        {
                            CheckIfHistoryEndDated(lbusPersonAccount.ibusPersonAccountGHDV, "Vision");
                        }
                    }
                    if (lobjPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdFlex)
                    {
                        lobjPersonAccountFlexComp.icdoPersonAccount.history_change_date = new DateTime(idtBatchRunDate.AddYears(1).Year, 1, 1);
                        lobjPersonAccountFlexComp.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusFlexSuspended;
                        lobjPersonAccountFlexComp.icdoPersonAccount.end_date = new DateTime(idtBatchRunDate.Year, 12, 31);
                        lobjPersonAccountFlexComp.icdoPersonAccountFlexComp.reason_value = busConstant.ChangeReasonAnnualEnrollment;
                        lobjPersonAccountFlexComp.iblnEndFlexCompEnrollmentBatch = true;
                        lobjPersonAccountFlexComp.BeforeValidate(utlPageMode.All);
                        lobjPersonAccountFlexComp.ValidateHardErrors(utlPageMode.All);
                        if (lobjPersonAccountFlexComp.iarrErrors.Count == 0)
                        {
                            idlgUpdateProcessLog("Processing for Person ID : " + lobjPersonAccountFlexComp.ibusPerson.icdoPerson.person_id + ", Plan : Flex ", "INFO", istrProcessName);
                            lobjPersonAccountFlexComp.BeforePersistChanges();
                            lobjPersonAccountFlexComp.PersistChanges();
                            lobjPersonAccountFlexComp.AfterPersistChanges();
                            DateTime ldtLastDateOfYear = new DateTime(idtBatchRunDate.Year, 12, 31);                            
                            DBFunction.DBNonQuery("cdoPersonAccountFlexcompConversion.UpdateFlexCompConversionEffectiveEndDate", 
                                new object[3] { ldtLastDateOfYear, lobjPersonAccountFlexComp.icdoPersonAccount.person_account_id, iobjBatchSchedule.batch_schedule_id },
                                iobjPassInfo.iconFramework,iobjPassInfo.itrnFramework);
                            
                        }
                    }
                }
            }
         }

        private void LoadFlexCompObjects(ref busPersonAccountFlexComp aobjPersonAccountFlexComp)
        {
            aobjPersonAccountFlexComp.LoadPerson();
            aobjPersonAccountFlexComp.LoadPlan();
            aobjPersonAccountFlexComp.LoadPlanEffectiveDate();

            aobjPersonAccountFlexComp.icdoPersonAccount.person_employment_dtl_id = aobjPersonAccountFlexComp.GetEmploymentDetailID();
            if (aobjPersonAccountFlexComp.icdoPersonAccount.person_employment_dtl_id != 0)
            {
                aobjPersonAccountFlexComp.LoadPersonEmploymentDetail();
                aobjPersonAccountFlexComp.ibusPersonEmploymentDetail.LoadPersonEmployment();
                aobjPersonAccountFlexComp.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
                aobjPersonAccountFlexComp.LoadOrgPlan(aobjPersonAccountFlexComp.idtPlanEffectiveDate);
            }
            aobjPersonAccountFlexComp.LoadFlexCompOptionUpdate();
            foreach (busPersonAccountFlexCompOption lobjPAFCOption in aobjPersonAccountFlexComp.iclbFlexCompOption)
            {
                lobjPAFCOption.icdoPersonAccountFlexCompOption.annual_pledge_amount = 0;
            }

            aobjPersonAccountFlexComp.LoadFlexCompConversion();
            aobjPersonAccountFlexComp.LoadFlexCompHistory();
            aobjPersonAccountFlexComp.LoadPreviousHistory();
            aobjPersonAccountFlexComp.LoadAllPersonEmploymentDetails();

            aobjPersonAccountFlexComp.LoadPersonAccount();
        }

       

        /// <summary>
        /// loads the latest history
        /// PIR 10570 - ibusHistory object will never be null, so the primary key value check condition added
        /// for issue 1 - maik mail dated 10/30/2015
        /// PIR 10570 - Loading the history as of last day of the year rather than batch run date for 
        /// issue 2 - maik mail dated 10/30/2015
        /// </summary>
        /// <param name="abusPersonAccountGhdv"></param>
        private void CheckIfHistoryEndDated(busPersonAccountGhdv abusPersonAccountGhdv, string astrPlanName)
        {
            DateTime ldtHistoryAsOfDate = new DateTime(idtBatchRunDate.Year, 12, 31); //Maik's mail dated 10/30/2015
            abusPersonAccountGhdv.ibusHistory = abusPersonAccountGhdv.LoadHistoryByDate(ldtHistoryAsOfDate);
            //Check if history is end dated
            if (abusPersonAccountGhdv.ibusHistory != null && abusPersonAccountGhdv.ibusHistory.icdoPersonAccountGhdvHistory.end_date == DateTime.MinValue
                && abusPersonAccountGhdv.ibusHistory.icdoPersonAccountGhdvHistory.person_account_ghdv_history_id > 0) //Maik's mail dated 10/30/2015
            {
                //insert history into dental
                EndGHDVPremiumConversion(abusPersonAccountGhdv, astrPlanName);
            }
        }


        /// <summary>
        /// End Premium Conversion For Dental And Vision
        /// </summary>
        /// <param name="aobjPersonAccountGhdv"></param>
        private void EndGHDVPremiumConversion(busPersonAccountGhdv aobjPersonAccountGhdv, string astrPlanName)
        {

            LoadGHDVObjects(aobjPersonAccountGhdv);

            DateTime ldtNextYearFirstDay = new DateTime(idtBatchRunDate.AddYears(1).Year, 1, 1);
            aobjPersonAccountGhdv.icdoPersonAccount.history_change_date = ldtNextYearFirstDay;
            aobjPersonAccountGhdv.icdoPersonAccountGhdv.reason_value = busConstant.AnnualEnrollment;
            aobjPersonAccountGhdv.icdoPersonAccountGhdv.premium_conversion_indicator_flag = "N";
            

            aobjPersonAccountGhdv.BeforeValidate(utlPageMode.All);
            aobjPersonAccountGhdv.ValidateHardErrors(utlPageMode.All);
            if (aobjPersonAccountGhdv.iarrErrors.Count == 0)
            {
                idlgUpdateProcessLog("Premium Conversion Ended for Person ID : " + aobjPersonAccountGhdv.icdoPersonAccount.person_id + ", Plan : " + astrPlanName, "INFO", istrProcessName);
                aobjPersonAccountGhdv.BeforePersistChanges();
                aobjPersonAccountGhdv.PersistChanges();
                aobjPersonAccountGhdv.AfterPersistChanges();
            }
            else
            {
                idlgUpdateProcessLog("Error in processing Person ID : " + aobjPersonAccountGhdv.icdoPersonAccount.person_id + ", Plan : " + astrPlanName, "INFO", istrProcessName);
            }

        }

        /// <summary>
        /// Load GHDV Objects
        /// </summary>
        /// <param name="aobjPersonAccountGhdv"></param>
        private void LoadGHDVObjects(busPersonAccountGhdv aobjPersonAccountGhdv)
        {
            aobjPersonAccountGhdv.LoadPerson();
            aobjPersonAccountGhdv.LoadPlan();
            aobjPersonAccountGhdv.LoadWorkersCompensation();
            aobjPersonAccountGhdv.LoadOtherCoverageDetails();
            aobjPersonAccountGhdv.LoadPaymentElection();
            aobjPersonAccountGhdv.LoadBillingOrganization();
            aobjPersonAccountGhdv.LoadPersonAccountGHDVHistory();
            aobjPersonAccountGhdv.LoadInsuranceYTD();

            //Initialize the Org Object to Avoid the NULL error
            aobjPersonAccountGhdv.InitializeObjects();

            aobjPersonAccountGhdv.LoadPlanEffectiveDate();
            aobjPersonAccountGhdv.DetermineEnrollmentAndLoadObjects(aobjPersonAccountGhdv.idtPlanEffectiveDate, false);

            if (aobjPersonAccountGhdv.IsHealthOrMedicare)
            {
                if (aobjPersonAccountGhdv.icdoPersonAccountGhdv.overridden_structure_code.IsNotNullOrEmpty())
                {
                    aobjPersonAccountGhdv.LoadRateStructureForUserStructureCode();
                }
                else
                {
                    //Load the Health Plan Participation Date (based on effective Date)
                    aobjPersonAccountGhdv.LoadHealthParticipationDate();
                    //To Get the Rate Structure Code (Derived Field)
                    aobjPersonAccountGhdv.LoadRateStructure();
                }
                //Get the Coverage Ref ID
                aobjPersonAccountGhdv.LoadCoverageRefID();

                aobjPersonAccountGhdv.GetMonthlyPremiumAmountByRefID();
            }
            else
            {
                aobjPersonAccountGhdv.GetMonthlyPremiumAmount();
            }

            aobjPersonAccountGhdv.LoadAllPersonEmploymentDetails();
            aobjPersonAccountGhdv.iintPreviousEPOProviderOrgID = aobjPersonAccountGhdv.icdoPersonAccountGhdv.epo_org_id;

            aobjPersonAccountGhdv.LoadPersonAccountAchDetail();
            aobjPersonAccountGhdv.LoadPersonAccountInsuranceTransfer();
        }       
    }
}
