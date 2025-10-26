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
using System.Collections;
using Sagitec.BusinessObjects;
#endregion

namespace NeoSpinBatch
{
    class busDeferredBatchLetter : busNeoSpinBatch
    {
        busBenefitApplication lobjBenefitApplication = new busBenefitApplication();

        //PIR 25971 To send correspondence before 120 days
        private void SendLetterBefore120Days()
        {
            DateTime ldtBatchRunDate = busGlobalFunctions.GetSysManagementBatchDate();
            //DateTime ldtBatchRunDateMinus90 = ldtBatchRunDate.AddDays(-90);
            DateTime ldtBatchRunDatePlus120 = ldtBatchRunDate.AddDays(120);

            if (idtbGetBenefitApplication.IsNull())
                LoadDeferredValidBenefitApplications();

            //DataTable ldtbGetBenefitApplication = busBase.SelectWithOperator<cdoBenefitApplication>(new string[3] { "action_status_value", "status_value", "letter_sent" },
            //    new string[3] { "=", "=", "=" },
            //    new object[3] { busConstant.ApplicationActionStatusDeferred, busConstant.ApplicationStatusValid, null }, null);
            //DataTable ldtbGetBenefitApplication = busBase.SelectWithOperator<cdoBenefitApplication>(new string[5] { "retirement_date", "retirement_date", "action_status_value", "status_value", "letter_sent" },
            //    new string[5] { "<", ">", "=", "=", "=" }, new object[5] { ldtBatchRunDate, ldtBatchRunDateMinus90, busConstant.ApplicationActionStatusDeferred, busConstant.ApplicationStatusValid, null }, null);

            foreach (DataRow dr in idtbGetBenefitApplication.Rows)
            {  
                bool lblnAllowToProceed = false;

                if (dr["letter_sent"] == DBNull.Value)
                    lblnAllowToProceed = true;

                if (lblnAllowToProceed)
                {
                    int lintbenefitApplicationId = Convert.ToInt32(dr["benefit_application_id"]);
                    int lintPersonID = Convert.ToInt32(dr["member_person_id"]);
                    int lintPlanID = Convert.ToInt32(dr["plan_id"]);

                    istrProcessName = "Process Deferred Batch Letter for Person Id " + lintPersonID;

                    idlgUpdateProcessLog("Process Deferred Batch Letter for Person Id " + lintPersonID, "INFO", istrProcessName);

                    TimeSpan ts = Convert.ToDateTime(dr["retirement_date"]).Subtract(ldtBatchRunDate);

                    if ((0 < ts.Days) && (ts.Days <= 120))
                    {                                         

                        busRetirementDisabilityApplication lobjBenefitApplication = new busRetirementDisabilityApplication();
                        lobjBenefitApplication.icdoBenefitApplication = new cdoBenefitApplication();
                        lobjBenefitApplication.icdoBenefitApplication.LoadData(dr);

                   
                        if (lobjBenefitApplication.ibusPersonAccount == null)
                            lobjBenefitApplication.LoadPersonAccountByApplication();

                        if (lobjBenefitApplication.ibusPersonAccount.ibusPerson == null)
                            lobjBenefitApplication.ibusPersonAccount.LoadPerson();
                        if (lobjBenefitApplication.ibusPlan == null)
                            lobjBenefitApplication.LoadPlan();

                        idlgUpdateProcessLog("Sending Letter to member 120 days before retirement day for the Person Id " + lintPersonID, "INFO", istrProcessName);

                        lobjBenefitApplication.GetNormalRetirementDateBasedOnNormalEligibility(lobjBenefitApplication.ibusPlan.icdoPlan.plan_code);

                        GetTaxableAmount(lobjBenefitApplication);

                        idlgUpdateProcessLog("Generating benefit Estimate for member 120 days before retirement day", "INFO", istrProcessName);

                        //create a estimate for future ref -- PIR - UAT 1494
                        GenerateBenefitEstimate(lobjBenefitApplication);

                        if (lobjBenefitApplication.icdoBenefitApplication.plan_id != 7 && //pir 7753
                           lobjBenefitApplication.icdoBenefitApplication.plan_id != busConstant.PlanIdDC2020 && //PIR 20232
                           lobjBenefitApplication.icdoBenefitApplication.plan_id != busConstant.PlanIdDC2025) //PIR 25920
                        {
                            //PIR 24933 
                            if (!lobjBenefitApplication.ibusPersonAccount.IsMemberEnrolledinPlanAsofBatchDate(lobjBenefitApplication.ibusPersonAccount.icdoPersonAccount.person_id, lobjBenefitApplication.icdoBenefitApplication.plan_id, ldtBatchRunDate))
                            {
                                //Send letter 
                                CreateCorrespondence(lobjBenefitApplication);

                                //Set Sent Letter to 1               
                                lobjBenefitApplication.FindBenefitApplication(lintbenefitApplicationId);
                                lobjBenefitApplication.icdoBenefitApplication.letter_sent = 1;
                                lobjBenefitApplication.icdoBenefitApplication.Update();
                            }
                        }

                      
                    }
                }
            }
        }

        private static void GetTaxableAmount(busRetirementDisabilityApplication lobjBenefitApplication)
        {
            if (lobjBenefitApplication.ibusPersonAccount.ibusPersonAccountRetirement == null)
            {
                lobjBenefitApplication.ibusPersonAccount.ibusPersonAccountRetirement = new busPersonAccountRetirement();
                lobjBenefitApplication.ibusPersonAccount.ibusPersonAccountRetirement.FindPersonAccountRetirement(lobjBenefitApplication.ibusPersonAccount.icdoPersonAccount.person_account_id);
                lobjBenefitApplication.ibusPersonAccount.ibusPersonAccountRetirement.LoadLTDSummary();
            }
        }

        private void CreateCorrespondence(busRetirementDisabilityApplication lobjBenefitApplication)
        {
            //ArrayList larrlist = new ArrayList();
            //larrlist.Add(lobjBenefitApplication);
            Hashtable lhstDummyTable = new Hashtable();
            lhstDummyTable.Add("sfwCallingForm", "Batch");
            CreateCorrespondence("APP-7002", lobjBenefitApplication, lhstDummyTable);
        }

        //PIR 25971 To send correspondence before 90 days
        private void SendLetterBefore90Days()
        {
            DateTime ldtBatchRunDate = busGlobalFunctions.GetSysManagementBatchDate();
            DateTime ldtBatchRunDateMinus90 = ldtBatchRunDate.AddDays(-90);
            DateTime ldtBatchRunDatePlus90 = ldtBatchRunDate.AddDays(90);

            if (idtbGetBenefitApplication.IsNull())
                LoadDeferredValidBenefitApplications();
            ////DataTable ldtbGetBenefitApplication = busBase.SelectWithOperator<cdoBenefitApplication>(new string[5] { "retirement_date", "retirement_date", "action_status_value", "status_value", "letter_sent" },
            ////    new string[5] { "<", ">", "=", "=", "=" },
            ////    new object[5] { ldtBatchRunDate, ldtBatchRunDateMinus60, busConstant.ApplicationActionStatusDeferred, busConstant.ApplicationStatusValid, 1 }, null);
            //DataTable ldtbGetBenefitApplication = busBase.SelectWithOperator<cdoBenefitApplication>(new string[2] { "action_status_value", "status_value" },
            //    new string[2] { "=", "=" },
            //    new object[2] { busConstant.ApplicationActionStatusDeferred, busConstant.ApplicationStatusValid }, null);

            foreach (DataRow dr in idtbGetBenefitApplication.Rows)
            {
                bool lblnAllowToProceed = false;

                if (dr["letter_sent"] == DBNull.Value)
                    lblnAllowToProceed = true;
                else
                {
                    if (Convert.ToInt32(dr["letter_sent"].ToString()) == 1)
                        lblnAllowToProceed = true;
                }
                if (lblnAllowToProceed)
                {
                    int lintbenefitApplicationId = Convert.ToInt32(dr["benefit_application_id"]);
                    int lintPersonID = Convert.ToInt32(dr["member_person_id"]);
                    int lintPlanID = Convert.ToInt32(dr["plan_id"]);

                    istrProcessName = "Process Deferred Batch Letter for Person Id " + lintPersonID;

                    idlgUpdateProcessLog("Process Deferred Batch Letter for Person Id " + lintPersonID, "INFO", istrProcessName);

                    TimeSpan ts = Convert.ToDateTime(dr["retirement_date"]).Subtract(ldtBatchRunDate);

                    if ((0 < ts.Days) && (ts.Days <= 90))
                    {                      
                       
                        busRetirementDisabilityApplication lobjBenefitApplication = new busRetirementDisabilityApplication();
                        lobjBenefitApplication.icdoBenefitApplication = new cdoBenefitApplication();
                        lobjBenefitApplication.icdoBenefitApplication.LoadData(dr);
                                              
                        if (lobjBenefitApplication.ibusPersonAccount == null)
                            lobjBenefitApplication.LoadPersonAccountByApplication();

                        if (lobjBenefitApplication.ibusPersonAccount.ibusPerson == null)
                            lobjBenefitApplication.ibusPersonAccount.LoadPerson();
                        if (lobjBenefitApplication.ibusPlan == null)
                            lobjBenefitApplication.LoadPlan();

                        idlgUpdateProcessLog("Sending Letter to member 90 days before retirement day for Person Id " + lintPersonID, "INFO", istrProcessName);

                        lobjBenefitApplication.GetNormalRetirementDateBasedOnNormalEligibility(lobjBenefitApplication.ibusPlan.icdoPlan.plan_code);
                        GetTaxableAmount(lobjBenefitApplication);

                        idlgUpdateProcessLog("Generating benefit Estimate for member 90 days before retirement day", "INFO", istrProcessName);

                        //create a estimate for future ref -- PIR - UAT 1494
                        GenerateBenefitEstimate(lobjBenefitApplication);
                        if (lobjBenefitApplication.icdoBenefitApplication.plan_id != 7 && //pir 7753
                          lobjBenefitApplication.icdoBenefitApplication.plan_id != busConstant.PlanIdDC2020 && //PIR 20232
                          lobjBenefitApplication.icdoBenefitApplication.plan_id != busConstant.PlanIdDC2025) //PIR 25920
                        {
                            //PIR 24933
                            if (!lobjBenefitApplication.ibusPersonAccount.IsMemberEnrolledinPlanAsofBatchDate(lobjBenefitApplication.ibusPersonAccount.icdoPersonAccount.person_id, lobjBenefitApplication.icdoBenefitApplication.plan_id, ldtBatchRunDate))
                            {
                                //Set Sent Letter to 2                
                                lobjBenefitApplication.FindBenefitApplication(lintbenefitApplicationId);
                                lobjBenefitApplication.icdoBenefitApplication.letter_sent = 2;
                                lobjBenefitApplication.icdoBenefitApplication.Update();

                                //Send letter 
                                CreateCorrespondence(lobjBenefitApplication);
                            }
                        }
                    }
                }
            }
        }

        public void SendLetterToDeferredApplicationMembers()
        {
            SendLetterBefore90Days();
            SendLetterBefore120Days();
        }       

        //Create a Benefit Estimate -- PIR - 1494
        private void GenerateBenefitEstimate(busRetirementDisabilityApplication abusBenefitApplication)
        {
            busRetirementBenefitCalculation lobjBenefitEstimateCalculation = new busRetirementBenefitCalculation { icdoBenefitCalculation = new cdoBenefitCalculation() };
            lobjBenefitEstimateCalculation.icdoBenefitCalculation.ienuObjectState = ObjectState.Insert;
            lobjBenefitEstimateCalculation.icdoBenefitCalculation.calculation_type_value = busConstant.CalculationTypeEstimate;
            lobjBenefitEstimateCalculation.icdoBenefitCalculation.created_date = DateTime.Now;

            lobjBenefitEstimateCalculation.ibusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
            lobjBenefitEstimateCalculation.ibusPersonAccount.icdoPersonAccount = abusBenefitApplication.ibusPersonAccount.icdoPersonAccount;

            lobjBenefitEstimateCalculation.GetCalculationByApplication(abusBenefitApplication);
            //setting benefit application id = 0 for calculation type Estimate
            lobjBenefitEstimateCalculation.icdoBenefitCalculation.benefit_application_id = 0;
            //temporary solution for setting Termination date
            if (lobjBenefitEstimateCalculation.icdoBenefitCalculation.termination_date == DateTime.MinValue)
            {
                lobjBenefitEstimateCalculation.icdoBenefitCalculation.termination_date = busGlobalFunctions.GetLastDayOfMonth(abusBenefitApplication.icdoBenefitApplication.retirement_date.AddMonths(-1));
            }
            lobjBenefitEstimateCalculation.LoadBenefitCalculationPayeeForNewMode(); //internal finding while testing PIR 1730
            lobjBenefitEstimateCalculation.BeforeValidate(utlPageMode.New);
            // lobjBenefitEstimateCalculation.ValidateHardErrors(utlPageMode.New);          

            lobjBenefitEstimateCalculation.BeforePersistChanges();
            lobjBenefitEstimateCalculation.PersistChanges();
            lobjBenefitEstimateCalculation.AfterPersistChanges();
            lobjBenefitEstimateCalculation.ValidateSoftErrors();
            lobjBenefitEstimateCalculation.UpdateValidateStatus();

            abusBenefitApplication.idecTaxableAmount = lobjBenefitEstimateCalculation.icdoBenefitCalculation.taxable_amount;
        }

        DataTable idtbGetBenefitApplication;
        private void LoadDeferredValidBenefitApplications()
        {
            idtbGetBenefitApplication = busBase.SelectWithOperator<cdoBenefitApplication>(new string[3] { "action_status_value", "status_value", "benefit_account_type_value" },
                        new string[3] { "=", "=", "=" },
                        new object[3] { busConstant.ApplicationActionStatusDeferred, busConstant.ApplicationStatusValid, busConstant.ApplicationBenefitTypeRetirement }, null);
        }
    }
}
