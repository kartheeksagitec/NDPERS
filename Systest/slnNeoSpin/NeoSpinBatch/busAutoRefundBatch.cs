#region Using directives
using System;
using System.Data;
using NeoSpin.CustomDataObjects;
using NeoSpin.BusinessObjects;
using System.Collections.ObjectModel;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using System.Collections;
using NeoSpin.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using Sagitec.Bpm;

#endregion
namespace NeoSpinBatch
{
    class busAutoRefundBatch : busNeoSpinBatch
    {

        private Collection<busDeathNotification> _iclbListOfDeceased;

        public Collection<busDeathNotification> iclbListOfDeceased
        {
            get { return _iclbListOfDeceased; }
            set { _iclbListOfDeceased = value; }
        }

        public void AutoRefund()
        {
            istrProcessName = "Auto Refund Batch ";
            GenerateCancelAutoRefundLetter(true);
            GenerateConfirmAutoRefundLetter(true);
            GenerateCancelAutoRefundLetter(false);
            GenerateConfirmAutoRefundLetterForRefund();
        }

        public void GenerateConfirmAutoRefundLetter(bool ablnIsDeathAutoRefund)
        {
            istrProcessName = " Auto Refund Confirmation Batch";

            idlgUpdateProcessLog("Auto Refund Batch for benefit type Pre Retirement Death", "INFO", istrProcessName);

            _iclbListOfDeceased = new Collection<busDeathNotification>();
            DataTable ldtbGetCertifiedDeceasedList = busBase.Select("cdoDeathNotification.GetListOfPersonForAutoRefundBatch", new object[] { });
            busBase lobjBase = new busBase();
            _iclbListOfDeceased = lobjBase.GetCollection<busDeathNotification>(ldtbGetCertifiedDeceasedList, "icdoDeathNotification");

            foreach (busDeathNotification lobjdeathNotification in _iclbListOfDeceased)
            {

                decimal ldecMemberAgeAsOnDateOfDeath = 0.00M;
                int lintMonths = 0;
                decimal ldecMonthAndYear = 0.00M;
                int lintMemberAgeYear = 0;
                int lintMemberAgemonths = 0;
                DateTime ldtDateOfDeath = lobjdeathNotification.icdoDeathNotification.date_of_death;
                lobjdeathNotification.LoadPerson();
                lobjdeathNotification.ibusPerson.LoadBeneficiary();

                //get member age based on date of death                             
                busPersonBase.CalculateAge(lobjdeathNotification.ibusPerson.icdoPerson.date_of_birth.AddMonths(1),
                     lobjdeathNotification.ibusPerson.icdoPerson.date_of_death, ref lintMonths, ref ldecMonthAndYear, 2, ref lintMemberAgeYear,
                     ref lintMemberAgemonths);

                ldecMemberAgeAsOnDateOfDeath = ldecMonthAndYear;

                //get date of death plus 60 days
                DateTime ldtDateOfDeathPlus6Months = ldtDateOfDeath.AddMonths(6);
                if (lobjdeathNotification.ibusPerson.iclbPersonBeneficiary.Count > 0)
                {
                    foreach (busPersonBeneficiary lobjPersonBeneficiary in lobjdeathNotification.ibusPerson.iclbPersonBeneficiary)
                    {
                        if (lobjPersonBeneficiary.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.person_account_beneficiary_id > 0)
                        {
                            busPersonAccountBeneficiary lobjPerBenAcc = lobjPersonBeneficiary.ibusPersonAccountBeneficiary;
                            if (lobjPerBenAcc.icdoPersonAccountBeneficiary.end_date == DateTime.MinValue)
                            {
                                if (lobjPersonBeneficiary.ibusPerson.IsNull())
                                    lobjPersonBeneficiary.ibusPerson = lobjdeathNotification.ibusPerson;

                                if (lobjPersonBeneficiary.ibusBeneficiaryPerson.IsNull())
                                    lobjPersonBeneficiary.LoadBeneficiaryPerson();

                                if (lobjPerBenAcc.ibusPersonAccount.IsNull())
                                    lobjPerBenAcc.LoadPersonAccount();

                                if (lobjPerBenAcc.ibusPersonAccount.ibusPlan.IsNull())
                                    lobjPerBenAcc.ibusPersonAccount.LoadPlan();

                                // UAT PIR ID 1342 - Application should be created if the Plan Participation status is in Enrolled/Suspended.
                                if ((lobjPerBenAcc.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled) ||
                                    (lobjPerBenAcc.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetimentSuspended))
                                {
                                    //check is plan JOb service
                                    bool lblnPlanJobService = false;
                                    if (lobjPerBenAcc.ibusPersonAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdJobService)
                                        lblnPlanJobService = true;

                                    //calculate TVSC
                                    decimal ldecTVSC = lobjdeathNotification.ibusPerson.GetTotalVSCForPerson(lblnPlanJobService, lobjdeathNotification.icdoDeathNotification.date_of_death);

                                    //get member account balance
                                    GetMemberAccountBalance(lobjPerBenAcc.ibusPersonAccount);

                                    //boolean to check to continue or discontinue
                                    bool lblnContinue = true;
                                    //check if any application recieved from the beneficiary within these 60 days
                                    DataTable ldtbApplications = busBase.Select("cdoBenefitApplication.LoadPreRetirementDeathApplications",
                                                                       new object[4] { lobjPersonBeneficiary.icdoPersonBeneficiary.person_id, 
                                                                                 lobjPersonBeneficiary.icdoPersonBeneficiary.beneficiary_person_id ,
                                                                                 busConstant.ApplicationBenefitTypePreRetirementDeath,
                                                                                 lobjPerBenAcc.ibusPersonAccount.icdoPersonAccount.plan_id});
                                    if (ldtbApplications.Rows.Count > 0)
                                    {
                                        lblnContinue = false;
                                        var lintCountOfApplications = ldtbApplications.AsEnumerable().Where(ldr => ldr.Field<DateTime>("received_date") < ldtDateOfDeathPlus6Months
                                                                                              && ldr.Field<DateTime>("received_date") > ldtDateOfDeath);
                                        if (lintCountOfApplications.AsDataTable().Rows.Count == 0)
                                            lblnContinue = true;
                                    }

                                    //check if the person is not vested.
                                    // then only continue the batch
                                    if (lblnContinue)
                                    {
                                        lblnContinue = false;
                                        bool lblnIsPersonVested = busPersonBase.CheckIsPersonVested(lobjPerBenAcc.ibusPersonAccount.ibusPlan.icdoPlan.plan_id,
                                                                    lobjPerBenAcc.ibusPersonAccount.ibusPlan.icdoPlan.plan_code,
                                                                    lobjPerBenAcc.ibusPersonAccount.ibusPlan.icdoPlan.benefit_provision_id,
                                                                    busConstant.ApplicationBenefitTypePreRetirementDeath, ldecTVSC, ldecMemberAgeAsOnDateOfDeath,
                                                                    lobjdeathNotification.icdoDeathNotification.date_of_death.GetFirstDayofNextMonth(), false, lobjdeathNotification.icdoDeathNotification.date_of_death, lobjPerBenAcc.ibusPersonAccount, iobjPassInfo); //PIR 14646 - Vesting logic changes
                                        if (!lblnIsPersonVested)
                                            lblnContinue = true;
                                    }

                                    if (lblnContinue)
                                    {
                                        lblnContinue = false;
                                        //if the account balance is less than 1000
                                        //then only continue the batch
                                        if (lobjPerBenAcc.ibusPersonAccount.ibusPersonAccountRetirement.Member_Account_Balance_ltd < 1000.00M)
                                        {
                                            lblnContinue = true;
                                        }

                                        if (lblnContinue)
                                        {
                                            if (!IsAutoRefundApplicationExists(lobjdeathNotification.ibusPerson,
                                                lobjPersonBeneficiary.icdoPersonBeneficiary.beneficiary_person_id, lobjPerBenAcc.ibusPersonAccount.ibusPlan.icdoPlan.plan_id))
                                            {

                                                idlgUpdateProcessLog("Generating Pre Retirement Death auto refund confirmation letter for " + lobjPersonBeneficiary.icdoPersonBeneficiary.beneficiary_person_id.ToString() + " ", "INFO", istrProcessName);

                                                CreateCorrespondence(lobjPersonBeneficiary, "PAY-4002");

                                                //create a pre retirement death application 
                                                int lintBenefitApplicationId = CreateNewAutoRefundApplication(lobjdeathNotification.ibusPerson.icdoPerson,
                                                    lobjPersonBeneficiary.icdoPersonBeneficiary.beneficiary_person_id, lobjPerBenAcc.ibusPersonAccount.icdoPersonAccount.plan_id,
                                                    lobjPersonBeneficiary.icdoPersonBeneficiary.relationship_value);

                                                //initiate workflow batch - Process beneficiary Auto Refund
                                                InitializeWorkFlow(lobjPersonBeneficiary.icdoPersonBeneficiary.person_id, ablnIsDeathAutoRefund, lintBenefitApplicationId, lobjPersonBeneficiary.icdoPersonBeneficiary.beneficiary_person_id);

                                                //suspend workflow for 30 days. then resume
                                                SuspendWorkflowForDeathApplication(lintBenefitApplicationId);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {//PIR 1343 fix
                    lobjdeathNotification.ibusPerson.LoadPersonAccountByBenefitType(busConstant.PlanBenefitTypeRetirement);
                    foreach (busPersonAccount lbusPersonAccount in lobjdeathNotification.ibusPerson.icolPersonAccountByBenefitType.Where(o =>
                        o.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled ||
                        o.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetimentSuspended))
                    {
                        lbusPersonAccount.LoadLatestEmployment();
                        GetMemberAccountBalance(lbusPersonAccount);
                        if (lbusPersonAccount.ibusPersonAccountRetirement.Member_Account_Balance_ltd < 1000.00M)
                        {
                            GenerateConfirmAutoRefundLetterForRefund(lbusPersonAccount, lbusPersonAccount.ibusPersonLatestEmployment.icdoPersonEmployment.org_id,
                                lbusPersonAccount.ibusPersonLatestEmployment.icdoPersonEmployment.end_date);

                        }
                    }
                }
            }
        }
        private void GenerateCancelAutoRefundLetter(bool ablnIsDeathApplication)
        {
            DataTable ldtbApplication = new DataTable();
            istrProcessName = " Auto Refund Cancellation Batch";
            if (ablnIsDeathApplication)
            {
                idlgUpdateProcessLog("Generate Auto Refund cancel letter For Pre Retirement Death", "INFO", istrProcessName);
                ldtbApplication = busBase.Select<cdoBenefitApplication>(new string[4] { "benefit_account_type_value", "benefit_option_value", "action_status_value", "letter_sent" },
                                                                                           new object[4] {busConstant.ApplicationBenefitTypePreRetirementDeath,busConstant.BenefitOptionAutoRefund, busConstant.ApplicationActionStatusPending,                                                                                  
                                                                                 0}, null, null);
            }
            else
            {
                idlgUpdateProcessLog("Generate Auto Refund  cancel letter For Refund", "INFO", istrProcessName);
                ldtbApplication = busBase.Select<cdoBenefitApplication>(new string[4] { "benefit_account_type_value", "benefit_option_value", "action_status_value", "letter_sent" },
                                                                                           new object[4] {busConstant.ApplicationBenefitTypeRefund,busConstant.BenefitOptionAutoRefund,
                                                                                           busConstant.ApplicationActionStatusPending,0}, null, null);
            }
            if (ldtbApplication.Rows.Count > 0)
            {
                foreach (DataRow dr in ldtbApplication.Rows)
                {
                    if ((dr["status_value"].ToString() == busConstant.ApplicationStatusReview)
                        || (dr["status_value"].ToString() == busConstant.ApplicationStatusValid))
                    {
                        busBenefitApplication lobjBenefitApplication = new busBenefitApplication();
                        lobjBenefitApplication.icdoBenefitApplication = new cdoBenefitApplication();
                        lobjBenefitApplication.icdoBenefitApplication.LoadData(dr);

                        lobjBenefitApplication.LoadPersonAccount();
                        lobjBenefitApplication.LoadRecipient();

                        GetMemberAccountBalance(lobjBenefitApplication.ibusPersonAccount);

                        decimal ldecAccountBalance = lobjBenefitApplication.ibusPersonAccount.ibusPersonAccountRetirement.Member_Account_Balance_ltd;

                        if (ldecAccountBalance >= 1000.00M)
                        {
                            idlgUpdateProcessLog("Generating Auto Refund cancel letter for " + lobjBenefitApplication.icdoBenefitApplication.member_person_id.ToString() + " ", "INFO", istrProcessName);

                            if (ablnIsDeathApplication)
                            {
                                CreateCancelledLetterForDeathApplication(lobjBenefitApplication);
                            }
                            else
                            {
                                CreateCancelledLetterForRefundApplication(lobjBenefitApplication);
                            }
                            //update the applicationa s cancelled
                            lobjBenefitApplication.icdoBenefitApplication.letter_sent = 1;
                            lobjBenefitApplication.icdoBenefitApplication.action_status_effective_date = DateTime.Now;
                            lobjBenefitApplication.icdoBenefitApplication.action_status_value = busConstant.ApplicationActionStatusCancelled;
                            lobjBenefitApplication.icdoBenefitApplication.Update();
                        }
                    }
                }
            }
        }
        private void CreateCancelledLetterForRefundApplication(busBenefitApplication lobjBenefitApplication)
        {

            CreateCorrespondence(lobjBenefitApplication, "APP-7302");

            //update the status of workflow as cancelled.
            // get instance id and check if greater than 0
            // update the instance status as cancelled
            // //get activity instance id for the workflow id and ref id.

            //DataTable ldtbActInstance = busNeoSpinBase.Select("cdoBenefitApplication.LoadRunningInstanceByApplication", new object[2] 
            //                                                { 
            //                                                    busConstant.Map_Initialize_Process_Auto_Refund_Workflow, 
            //                                                    lobjBenefitApplication.icdoBenefitApplication.benefit_application_id 
            //                                                });
            //if (ldtbActInstance != null && ldtbActInstance.Rows.Count > 0)
            //{
            //    busBpmActivityInstance lbusActivityInstance = busWorkflowHelper.GetActivityInstance(Convert.ToInt32(ldtbActInstance.Rows[0]["activity_instance_id"]));
            //    busWorkflowHelper.UpdateWorkflowActivityByEvent(lbusActivityInstance, enmNextAction.Cancel, busConstant.ActivityStatusCancelled, iobjPassInfo);
            //}
        }
        private void CreateCancelledLetterForDeathApplication(busBenefitApplication lobjBenefitApplication)
        {
            CreateCorrespondence(lobjBenefitApplication, "PAY-4003");

            //update the status of workflow as cancelled.
            // get instance id and check if greater than 0
            // update the instance status as cancelled
            // //get activity instance id for the workflow id and ref id.
            //venkat check query
            //DataTable ldtbActInstance = busNeoSpinBase.Select("cdoBenefitApplication.LoadRunningInstanceByApplication", new object[2] 
            //                                                { 
            //                                                    busConstant.Map_Initialize_Process_Beneficiary_Auto_Refund_Workflow, 
            //                                                    lobjBenefitApplication.icdoBenefitApplication.benefit_application_id
            //                                                });

            //if (ldtbActInstance != null && ldtbActInstance.Rows.Count > 0)
            //{
            //    busBpmActivityInstance lbusActivityInstance = busWorkflowHelper.GetActivityInstance(Convert.ToInt32(ldtbActInstance.Rows[0]["activity_instance_id"]));

            //    busWorkflowHelper.UpdateWorkflowActivityByEvent(lbusActivityInstance, enmNextAction.Cancel, busConstant.ActivityStatusCancelled, iobjPassInfo);
            //}
        }

        private void SuspendWorkflowForDeathApplication(int aintBenefitApplicationId)
        {
            //update the status of workflow as suspend.
            // get instance id and check if greater than 0
            // update the instance status as suspend
            // //get activity instance id for the workflow id and ref id.
            DataTable ldtbActInstance = busNeoSpinBase.Select("cdoBenefitApplication.LoadRunningInstanceByApplication", new object[2] 
                                                            { 
                                                                busConstant.Map_Initialize_Process_Beneficiary_Auto_Refund_Workflow, 
                                                                aintBenefitApplicationId
                                                            });

            if (ldtbActInstance != null && ldtbActInstance.Rows.Count > 0)
            {
                //venkat check query
                busBpmActivityInstance lbusActivityInstance = busWorkflowHelper.GetActivityInstance(Convert.ToInt32(ldtbActInstance.Rows[0]["activity_instance_id"]));

                busWorkflowHelper.UpdateWorkflowActivityByStatus(busConstant.ActivityStatusInProcess, lbusActivityInstance, utlPassInfo.iobjPassInfo);
                // PROD PIR ID 4924
                lbusActivityInstance.icdoBpmActivityInstance.suspension_end_date = busGlobalFunctions.GetSysManagementBatchDate().AddDays(15);
                busWorkflowHelper.UpdateWorkflowActivityByStatus(busConstant.ActivityStatusSuspended, lbusActivityInstance, iobjPassInfo);
            }
        }

        private void GetMemberAccountBalance(busPersonAccount abusPersonAccount)
        {
            abusPersonAccount.ibusPersonAccountRetirement = new busPersonAccountRetirement();
            abusPersonAccount.ibusPersonAccountRetirement.FindPersonAccountRetirement(abusPersonAccount.icdoPersonAccount.person_account_id);
            abusPersonAccount.ibusPersonAccountRetirement.LoadLTDSummary();
        }

        private void CreateCorrespondence(busPersonBeneficiary aobjPersonBeneficiary, string astrCorName)
        {
            //ArrayList larrlist = new ArrayList();
            //larrlist.Add(aobjPersonBeneficiary);
            Hashtable lhstDummyTable = new Hashtable();
            lhstDummyTable.Add("sfwCallingForm", "Batch");
            CreateCorrespondence(astrCorName, aobjPersonBeneficiary, lhstDummyTable);
        }
        private void CreateCorrespondence(busBenefitApplication aobjBenefitApplication, string astrCorName)
        {
            //ArrayList larrlist = new ArrayList();
            //larrlist.Add(aobjBenefitApplication);
            Hashtable lhstDummyTable = new Hashtable();
            lhstDummyTable.Add("sfwCallingForm", "Batch");
            CreateCorrespondence(astrCorName, aobjBenefitApplication, lhstDummyTable);
        }
        private int CreateNewAutoRefundApplication(cdoPerson acdoPerson, int aintBeneficiaryPersonID, int aintPlanId, string astrRelationShipvalue)
        {
            busBenefitApplication lobjBenefitApplication = new busBenefitApplication();
            lobjBenefitApplication.icdoBenefitApplication = new cdoBenefitApplication();
            lobjBenefitApplication.icdoBenefitApplication.received_date = DateTime.Now;
            lobjBenefitApplication.icdoBenefitApplication.plan_id = aintPlanId;
            lobjBenefitApplication.icdoBenefitApplication.member_person_id = acdoPerson.person_id;
            lobjBenefitApplication.icdoBenefitApplication.recipient_person_id = aintBeneficiaryPersonID;
            lobjBenefitApplication.icdoBenefitApplication.benefit_account_type_value = busConstant.ApplicationBenefitTypePreRetirementDeath;
            lobjBenefitApplication.icdoBenefitApplication.benefit_option_value = busConstant.BenefitOptionAutoRefund;
            lobjBenefitApplication.icdoBenefitApplication.action_status_value = busConstant.ApplicationActionStatusPending;
            lobjBenefitApplication.icdoBenefitApplication.status_value = busConstant.ApplicationStatusReview;
            DateTime ldtTempDate = acdoPerson.date_of_death.AddMonths(1);
            lobjBenefitApplication.icdoBenefitApplication.retirement_date = new DateTime(ldtTempDate.Year, ldtTempDate.Month, 1);
            lobjBenefitApplication.icdoBenefitApplication.dnro_flag = busConstant.Flag_No;
            lobjBenefitApplication.icdoBenefitApplication.early_reduction_waived_flag = busConstant.Flag_No;
            lobjBenefitApplication.icdoBenefitApplication.account_relationship_value = busConstant.AccountRelationshipBeneficiary;
            lobjBenefitApplication.icdoBenefitApplication.family_relationship_value = astrRelationShipvalue;
            lobjBenefitApplication.icdoBenefitApplication.Insert();
            lobjBenefitApplication.LoadPersonAccount();
            lobjBenefitApplication.CreateBenefitApplicationPersonAccount();

            return lobjBenefitApplication.icdoBenefitApplication.benefit_application_id;
        }

        private void InitializeWorkFlow(int aintPersonID, bool ablnIsDeathAutoRefund, int aintApplicationID, int aintBeneficiaryID)
        {
            int process_id = 0;
            if (ablnIsDeathAutoRefund)
            {
                process_id = busConstant.Map_Initialize_Process_Beneficiary_Auto_Refund_Workflow;
            }
            else
            {
                process_id = busConstant.Map_Initialize_Process_Auto_Refund_Workflow;
            }
            Dictionary<string, object> ldctParameters = new Dictionary<string, object>();
            ldctParameters["additional_parameter1"] = aintBeneficiaryID;
            busWorkflowHelper.InitiateBpmRequest(process_id, aintPersonID, 0, aintApplicationID, iobjPassInfo, adictInstanceParameters: ldctParameters, astrSource: BpmRequestSource.Batch);
        }

        public void GenerateConfirmAutoRefundLetterForRefund()
        {
            istrProcessName = " Auto Refund Confirmation Batch";

            idlgUpdateProcessLog("Auto Refund Batch for benefit type Refund", "INFO", istrProcessName);
            DataTable ldtbEmpRec = busNeoSpinBase.Select("cdoPerson.LoadMemberAndPAEmpDetailsForAutoRefundBatch", new object[1] { iobjSystemManagement.icdoSystemManagement.batch_date });
            foreach (DataRow drEmpRec in ldtbEmpRec.Rows)
            {
                bool lblnSkipRecord = false;
                busPerson lobjPerson = new busPerson();
                lobjPerson.icdoPerson = new cdoPerson();
                lobjPerson.icdoPerson.LoadData(drEmpRec);
                busPersonEmployment lobjPersonEmployment = new busPersonEmployment();
                lobjPersonEmployment.icdoPersonEmployment = new cdoPersonEmployment();
                lobjPersonEmployment.icdoPersonEmployment.LoadData(drEmpRec);
                //pir 6690 : application end date is getting set as null
                if (drEmpRec["pe_start_date"] != DBNull.Value && drEmpRec["pe_end_date"] != DBNull.Value && drEmpRec["pe_org_id"] != DBNull.Value)
                {
                    lobjPersonEmployment.icdoPersonEmployment.org_id = Convert.ToInt32(drEmpRec["pe_org_id"]);
                    lobjPersonEmployment.icdoPersonEmployment.start_date = Convert.ToDateTime(drEmpRec["pe_start_date"]);
                    lobjPersonEmployment.icdoPersonEmployment.end_date = Convert.ToDateTime(drEmpRec["pe_end_date"]);
                }
                busPersonAccount lobjPersonAccount = new busPersonAccount();
                lobjPersonAccount.icdoPersonAccount = new cdoPersonAccount();
                lobjPersonAccount.icdoPersonAccount.LoadData(drEmpRec);
                if (lobjPersonAccount.ibusPerson == null)
                    lobjPersonAccount.ibusPerson = lobjPerson;
                GetMemberAccountBalance(lobjPersonAccount);
                GenerateConfirmAutoRefundLetterForRefund(lobjPersonAccount, lobjPersonEmployment.icdoPersonEmployment.org_id, lobjPersonEmployment.icdoPersonEmployment.end_date);                
            }
        }
        public void GenerateConfirmAutoRefundLetterForRefund(busPersonAccount aobjBusPersonAccount, int aintOrgID, DateTime adtEmploymentEndDate)
        {
            bool lblnSkipRecord = false;
            if (aobjBusPersonAccount.ibusPerson == null)
                aobjBusPersonAccount.LoadPerson();
            if (aobjBusPersonAccount.ibusPerson.iclbBenefitApplication == null)
                aobjBusPersonAccount.ibusPerson.LoadBenefitApplication();
            foreach (busBenefitApplication lobjBenefitApplication in aobjBusPersonAccount.ibusPerson.iclbBenefitApplication)
            {
                
                if (lobjBenefitApplication.icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeRefund)
                {
                    if (lobjBenefitApplication.ibusPersonAccount == null)
                    {
                        lobjBenefitApplication.ibusPersonAccount = aobjBusPersonAccount;
                    }
                    TimeSpan ltsDiff = lobjBenefitApplication.icdoBenefitApplication.received_date.Subtract(adtEmploymentEndDate);
                    if (ltsDiff.Days < 30)
                    {
                        lblnSkipRecord = true;
                    }
                    if (lobjBenefitApplication.icdoBenefitApplication.action_status_value != busConstant.ApplicationActionStatusCancelled)
                    {
                        lblnSkipRecord = true;
                    }
                    if (lobjBenefitApplication.IsPersonVestedForRefund())
                    {
                        lblnSkipRecord = true;
                    }
                }
                else if (lobjBenefitApplication.icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement)
                {
                    lblnSkipRecord = true;
                }
            }
            //PIR-9140 
            DataTable ldtbBenefitapp =  busBase.Select("cdoBenefitApplication.GetBenefitApplicationforAutorefund", new object[1] { aobjBusPersonAccount.icdoPersonAccount.person_account_id });
            DataTable ldtbOpenPersonEmployment = busBase.Select("cdoPersonEmployment.LoadOpenPersonEmployment", new object[1] { aobjBusPersonAccount.icdoPersonAccount.person_id });

            if (ldtbBenefitapp.Rows.Count > 0)
                lblnSkipRecord = true;
            else
            {
                if (ldtbOpenPersonEmployment.Rows.Count == 0)
                {
                    lblnSkipRecord = false;
                }
                else
                {
                    lblnSkipRecord = true;
                }                        
            }

            
            if (!lblnSkipRecord)
            {
                int lintBenefitApplicationID = 0;
                //ArrayList larrlist = new ArrayList();
                //larrlist.Add(aobjBusPersonAccount);
                Hashtable lhstDummyTable = new Hashtable();
                lhstDummyTable.Add("sfwCallingForm", "Batch");

                idlgUpdateProcessLog("Generating Refund Confirmation letter for PERSLink ID " + aobjBusPersonAccount.icdoPersonAccount.person_id.ToString() + " ", "INFO", istrProcessName);

                CreateCorrespondence("APP-7310", aobjBusPersonAccount, lhstDummyTable);

                CreateNewAutoRefundApplication(aobjBusPersonAccount, adtEmploymentEndDate, adtEmploymentEndDate.AddMonths(1), aintOrgID, ref lintBenefitApplicationID);
                //uat pir - 1343
                //CheckConditionAndIntiateWorkflow(aobjBusPersonAccount, lintBenefitApplicationID);
                InitializeWorkFlow(aobjBusPersonAccount.icdoPersonAccount.person_id, false, lintBenefitApplicationID, 0);
            }
        }
        //uat pir 1343
        private void CheckConditionAndIntiateWorkflow(busPersonAccount aobjBusPersonAccount, int aintBenefitApplicationID)
        {
            //    if (aobjBusPersonAccount.ibusPerson == null)
            //        aobjBusPersonAccount.LoadPerson();
            //    if (aobjBusPersonAccount.ibusPerson.iclbPersonBeneficiary == null)
            //        aobjBusPersonAccount.ibusPerson.LoadBeneficiary();
            //    if (aobjBusPersonAccount.ibusPerson.icdoPerson.date_of_death != DateTime.MinValue &&
            //        aobjBusPersonAccount.ibusPerson.iclbPersonBeneficiary
            //        .Where(o => o.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.person_account_id == aobjBusPersonAccount.icdoPersonAccount.person_account_id &&
            //            o.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.start_date <= aobjBusPersonAccount.ibusPerson.icdoPerson.date_of_death &&
            //            (o.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date == DateTime.MinValue || o.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date >= aobjBusPersonAccount.ibusPerson.icdoPerson.date_of_death))
            //        .Any())
            //    {
            //        InitializeWorkFlow(aobjBusPersonAccount.ibusPerson.icdoPerson.person_id, true, aintBenefitApplicationID);
            //    }
            //    else
            if (aobjBusPersonAccount.ibusPerson.icdoPerson.date_of_death == DateTime.MinValue ||
                (aobjBusPersonAccount.ibusPerson.icdoPerson.date_of_death != DateTime.MinValue &&
                !aobjBusPersonAccount.ibusPerson.iclbPersonBeneficiary
                .Where(o => o.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.person_account_id == aobjBusPersonAccount.icdoPersonAccount.person_account_id &&
                    o.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.start_date <= aobjBusPersonAccount.ibusPerson.icdoPerson.date_of_death &&
                    (o.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date == DateTime.MinValue || o.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date >= aobjBusPersonAccount.ibusPerson.icdoPerson.date_of_death))
                .Any()))
            {
                InitializeWorkFlow(aobjBusPersonAccount.ibusPerson.icdoPerson.person_id, false, aintBenefitApplicationID, 0);
            }
        }

        private void CreateNewAutoRefundApplication(busPersonAccount aobjPersonAccount, DateTime adtTerminationDate, DateTime adtRecievedDate, int aintRetirementOrg, ref int aintBenefitApplicationId)
        {
            busBenefitApplication lobjBenefitApplication = new busBenefitApplication();
            lobjBenefitApplication.icdoBenefitApplication = new cdoBenefitApplication();
            lobjBenefitApplication.icdoBenefitApplication.account_relationship_value = busConstant.AccountRelationshipMember;
            lobjBenefitApplication.icdoBenefitApplication.family_relationship_value = busConstant.AccountRelationshipMember;
            lobjBenefitApplication.icdoBenefitApplication.benefit_option_value = busConstant.BenefitOptionAutoRefund;
            lobjBenefitApplication.icdoBenefitApplication.benefit_account_type_value = busConstant.ApplicationBenefitTypeRefund;
            lobjBenefitApplication.icdoBenefitApplication.benefit_sub_type_value = busConstant.BenefitOptionAutoRefund;
            lobjBenefitApplication.icdoBenefitApplication.action_status_value = busConstant.ApplicationActionStatusPending;
            lobjBenefitApplication.icdoBenefitApplication.action_status_effective_date = DateTime.Now;
            lobjBenefitApplication.icdoBenefitApplication.dnro_flag = busConstant.Flag_No;
            lobjBenefitApplication.icdoBenefitApplication.early_reduction_waived_flag = busConstant.Flag_No;
            lobjBenefitApplication.icdoBenefitApplication.payment_date = iobjSystemManagement.icdoSystemManagement.batch_date.GetFirstDayofNextMonth().GetFirstDayofNextMonth(); // PROD PIR ID 5105
            lobjBenefitApplication.icdoBenefitApplication.member_person_id = aobjPersonAccount.icdoPersonAccount.person_id;
            lobjBenefitApplication.icdoBenefitApplication.plan_id = aobjPersonAccount.icdoPersonAccount.plan_id;
            lobjBenefitApplication.icdoBenefitApplication.plso_requested_flag = busConstant.Flag_No;
            lobjBenefitApplication.icdoBenefitApplication.received_date = DateTime.Now;
            lobjBenefitApplication.icdoBenefitApplication.termination_date = adtTerminationDate;
            lobjBenefitApplication.icdoBenefitApplication.sick_leave_purchase_indicated_flag = busConstant.Flag_No;
            lobjBenefitApplication.icdoBenefitApplication.status_value = busConstant.StatusReview;
            lobjBenefitApplication.icdoBenefitApplication.uniform_income_flag = busConstant.Flag_No;
            lobjBenefitApplication.icdoBenefitApplication.suppress_warnings_flag = busConstant.Flag_No;
            lobjBenefitApplication.icdoBenefitApplication.recipient_person_id = aobjPersonAccount.icdoPersonAccount.person_id;
            lobjBenefitApplication.icdoBenefitApplication.retirement_org_id = aintRetirementOrg;
            lobjBenefitApplication.icdoBenefitApplication.rhic_option_value = string.Empty;
            lobjBenefitApplication.icdoBenefitApplication.Insert();
            aintBenefitApplicationId = lobjBenefitApplication.icdoBenefitApplication.benefit_application_id;
            lobjBenefitApplication.ibusPersonAccount = aobjPersonAccount;
            lobjBenefitApplication.CreateBenefitApplicationPersonAccount();
        }

        //PIR - 1725
        //'Check if already there exists ny  application for the same person and 
        //int aintPersonId, int aintBenePersonID, int aintPlanid
        private bool IsAutoRefundApplicationExists(busPerson aobjPerson, int aintBenePersonID, int aintPlanid)
        {
            if (aobjPerson.iclbBenefitApplication.IsNull())
                aobjPerson.LoadBenefitApplication();

            var lenum = aobjPerson.iclbBenefitApplication.Where(lobj => lobj.icdoBenefitApplication.recipient_person_id == aintBenePersonID
                && lobj.icdoBenefitApplication.plan_id == aintPlanid
                && lobj.icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath
                && lobj.icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionAutoRefund);
            if (lenum.Count() > 0)
                return true;
            return false;
        }
    }
}
