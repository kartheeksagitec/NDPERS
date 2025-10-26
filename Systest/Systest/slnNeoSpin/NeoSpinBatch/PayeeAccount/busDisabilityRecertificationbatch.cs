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
using Sagitec.ExceptionPub;
using System.Linq;
#endregion

namespace NeoSpinBatch
{
    public class busDisabilityRecertificationbatch : busNeoSpinBatch
    {
        public void DisabilityRecertificationBatch()
        {
            istrProcessName = iobjBatchSchedule.step_name;
            idlgUpdateProcessLog("Disability Recertification Batch Started", "INFO", istrProcessName);
            idlgUpdateProcessLog("Step:1 Creating Disability Workflow Process and Case Started", "INFO", istrProcessName);
            DataTable ldbtResult = busBase.Select("cdoCase.DisabilityRecertificationBatch",
                                        new object[1] { iobjSystemManagement.icdoSystemManagement.batch_date });
            bool lblnTransaction = false;
            // Task 1 : Create Case and send Recertification notices
            idlgUpdateProcessLog("Total Records Fetched : " + Convert.ToString(ldbtResult.Rows.Count), "INFO", istrProcessName);            
            foreach (DataRow ldr in ldbtResult.Rows)
            {
                try
                {
                    if (!lblnTransaction)
                    {
                        utlPassInfo.iobjPassInfo.BeginTransaction();
                        lblnTransaction = true;
                    }
                    // 1.Creating a Case
                    int lintReferenceID = 0;
                    busPayeeAccount lobjDisabilityPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                    lobjDisabilityPayeeAccount.icdoPayeeAccount.LoadData(ldr);
                    idlgUpdateProcessLog("Creating Case for Payee Account ID : " +
                        Convert.ToString(lobjDisabilityPayeeAccount.icdoPayeeAccount.payee_account_id), "INFO", istrProcessName);
                    busCase lobjCase = new busCase { icdoCase = new cdoCase() };

                    Collection<busCase> lclbCase = new Collection<busCase>();
                    busBase lobjBase = new busBase();
                    DataTable ldtbList = busBase.Select<cdoCase>(new string[1] { "PAYEE_ACCOUNT_ID" }, new object[1]{
                                                lobjDisabilityPayeeAccount.icdoPayeeAccount.payee_account_id}, null, "CREATED_DATE DESC");
                    lclbCase = lobjBase.GetCollection<busCase>(ldtbList, "icdoCase");
                    var iintCount = lclbCase.Where(o => o.icdoCase.case_type_value == busConstant.CaseStatusValuePendingMember ||
                            o.icdoCase.case_type_value == busConstant.CaseStatusValuePending3rdParty ||
                            o.icdoCase.case_type_value == busConstant.CaseStatusValuePendingNDPERS).Count();
                    if (iintCount == 0)
                    {
                        lobjCase.icdoCase.person_id = lobjDisabilityPayeeAccount.icdoPayeeAccount.payee_perslink_id;
                        if (lobjDisabilityPayeeAccount.icdoPayeeAccount.benefit_begin_date < busConstant.Pre1991Disability)
                        {
                            lobjCase.icdoCase.case_type_value = busConstant.CaseTypePre1991DisabilityRecertification;
                        }
                        else
                        {
                            lobjCase.icdoCase.case_type_value = busConstant.CaseTypeDisabilityRecertification;
                            lobjCase.icdoCase.income_verification_date = lobjDisabilityPayeeAccount.icdoPayeeAccount.recertification_date;
                        }
                        lobjCase.icdoCase.case_status_value = busConstant.CaseStatusValuePendingMember;
                        lobjCase.icdoCase.payee_account_id = lobjDisabilityPayeeAccount.icdoPayeeAccount.payee_account_id;
                        lobjCase.icdoCase.recertification_date = lobjDisabilityPayeeAccount.icdoPayeeAccount.recertification_date;
                        if (lobjDisabilityPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                            lobjDisabilityPayeeAccount.LoadNexBenefitPaymentDate();
                        lobjCase.icdoCase.next_recertification_date = lobjDisabilityPayeeAccount.idtNextBenefitPaymentDate.AddMonths(18); // UAT PIR ID 1362
                        lobjCase.icdoCase.Insert();
                        lintReferenceID = lobjCase.icdoCase.case_id;
                    }
                    else
                    {
                        lintReferenceID = lclbCase[0].icdoCase.case_id;
                    }

                    // 2. Creating Case Details
                    idlgUpdateProcessLog("Creating Case Detail for Case ID : " +
                            Convert.ToString(lobjCase.icdoCase.case_id), "INFO", istrProcessName);
                    lobjCase.LoadStepDetailsNewMode();
                    foreach (busCaseStepDetail lobjbuscasestepdetail in lobjCase.iclbCaseStepDetail)
                    {
                        lobjbuscasestepdetail.icdoCaseStepDetail.case_id = lobjCase.icdoCase.case_id;
                        lobjbuscasestepdetail.icdoCaseStepDetail.Insert();
                    }
                    if (lobjCase.icdoCase.case_type_value == busConstant.CaseTypeDisabilityRecertification)
                    {
                        lobjCase.SetComparableEarnings();
                        lobjCase.LoadIncomeVerfication();
                        lobjCase.InsertOrUpdateIncomeVerification();
                    }

                    // 3.Create First notification Letter
                    lobjCase.iintNotificationIdentifier = 1;
                    idlgUpdateProcessLog("Generating Correspondence for Case ID : " +
                                    Convert.ToString(lobjCase.icdoCase.case_id), "INFO", istrProcessName);
                    GenerateCorrespondence(lobjCase);

                    // 4.Create Recertify WFL
                    idlgUpdateProcessLog("Initializing Workflow for Case ID : " +
                                Convert.ToString(lobjCase.icdoCase.case_id), "INFO", istrProcessName);
                    if (lobjCase.icdoCase.case_type_value == busConstant.CaseTypePre1991DisabilityRecertification)
                    {
                        InitializeWorkFlow(busConstant.Map_Recertify_Pre1991_Disability, lobjCase.icdoCase.person_id, lintReferenceID,
                                    busConstant.WorkflowProcessStatus_UnProcessed, busConstant.WorkflowProcessSource_Batch);
                    }
                    else if (lobjCase.icdoCase.case_type_value == busConstant.CaseTypeDisabilityRecertification)
                    {
                        InitializeWorkFlow(busConstant.Map_Recertify_Disability, lobjCase.icdoCase.person_id, lintReferenceID,
                                    busConstant.WorkflowProcessStatus_UnProcessed, busConstant.WorkflowProcessSource_Batch);
                    }

                    // 5. Update batch letter sent flag
                    lobjDisabilityPayeeAccount.icdoPayeeAccount.is_disability_batch_letter_sent_flag = busConstant.Flag_Yes;
                    lobjDisabilityPayeeAccount.icdoPayeeAccount.Update();
                    if (lblnTransaction)
                    {
                        utlPassInfo.iobjPassInfo.Commit();
                        lblnTransaction = false;
                    }
                }
                catch (Exception _ex)
                {
                    ExceptionManager.Publish(_ex);
                    if (lblnTransaction)
                    {
                        utlPassInfo.iobjPassInfo.Rollback();
                        lblnTransaction = false;
                    }
                    idlgUpdateProcessLog("Message:" + _ex.Message, "ERR", istrProcessName);
                }
            }
            idlgUpdateProcessLog("Step:1 Creating Disability Workflow Process and Case Ended", "INFO", istrProcessName);
            idlgUpdateProcessLog("Step:2 Sending First,Second,Third Disability Recertification Notification letters started", "INFO", istrProcessName);
            // Task 2 : Update Notification flags            
            DataTable ldtb = busBase.Select("cdoCase.DisabilityRecertificationBatchGetIdentifier",
                                        new object[1] { iobjSystemManagement.icdoSystemManagement.batch_date });
            idlgUpdateProcessLog("Total Records Fetched : " + Convert.ToString(ldtb.Rows.Count), "INFO", istrProcessName);
            foreach (DataRow ldr in ldtb.Rows)
            {
                try
                {
                    if (!lblnTransaction)
                    {
                        utlPassInfo.iobjPassInfo.BeginTransaction();
                        lblnTransaction = true;
                    }
                    int lintIdentifier = Convert.ToInt32(ldr["RowIdentifier"]);
                    busCase lobjCase = new busCase { icdoCase = new cdoCase() };
                    lobjCase.icdoCase.LoadData(ldr);

                    switch (lintIdentifier)
                    {
                        case 1:
                            // 1 Month before Recertification Date
                            // Update Third Notification Flag
                            lobjCase.icdoCase.third_notification_flag = busConstant.Flag_Yes;
                            idlgUpdateProcessLog("Set Third notification flag for Person ID :"
                                    + lobjCase.icdoCase.person_id, "INFO", istrProcessName);
                            lobjCase.iintNotificationIdentifier = 3;
                            // Initiate WFL to Suspend Disability Benefits
                            idlgUpdateProcessLog("Initialize Suspension Disability Benefit Workflow for Person ID :"
                                                            + lobjCase.icdoCase.person_id, "INFO", istrProcessName);
                            InitializeWorkFlow(busConstant.Map_Suspend_Disability_Benefits,
                                                lobjCase.icdoCase.person_id,lobjCase.icdoCase.case_id,
                                                busConstant.WorkflowProcessStatus_UnProcessed,busConstant.WorkflowProcessSource_Batch);
                            break;
                        case 2:
                            // 3 Months before Recertification Date
                            // Update Second Notification Flag
                            lobjCase.icdoCase.second_notification_flag = busConstant.Flag_Yes;
                            idlgUpdateProcessLog("Set Second notification flag for Person ID :"
                                    + lobjCase.icdoCase.person_id, "INFO", istrProcessName);
                            lobjCase.iintNotificationIdentifier = 2;
                            break;
                        case 3:
                            // 5 Months before Recertification Date
                            // Update First Notification Flag
                            lobjCase.icdoCase.first_notification_flag = busConstant.Flag_Yes;
                            idlgUpdateProcessLog("Set First notification flag for Person ID :"
                                    + lobjCase.icdoCase.person_id, "INFO", istrProcessName);
                            lobjCase.iintNotificationIdentifier = 1;
                            break;
                        default:
                            break;
                    }
                    //PIR 17268 - to avoid generation of duplicate letters.
                    if(lobjCase.iintNotificationIdentifier != 1)
                     GenerateCorrespondence(lobjCase);
                    lobjCase.icdoCase.Update();
                    if (lblnTransaction)
                    {
                        utlPassInfo.iobjPassInfo.Commit();
                        lblnTransaction = false;
                    }
                }
                catch (Exception _ex)
                {
                    ExceptionManager.Publish(_ex);
                    if (lblnTransaction)
                    {
                        utlPassInfo.iobjPassInfo.Rollback();
                        lblnTransaction = false;
                    }
                    idlgUpdateProcessLog("Message:" + _ex.Message, "ERR", istrProcessName);
                }
            }
            idlgUpdateProcessLog("Step:2 Sending First,Second,Third Disability Recertification Notification letters Ended", "INFO", istrProcessName);
            idlgUpdateProcessLog("Disability Recertification Batch Ended.", "INFO", istrProcessName);            
        }

        private void GenerateCorrespondence(busCase aobjCase)
        {
            //ArrayList larrList = new ArrayList();
            //larrList.Add(aobjCase);
            Hashtable lshtTemp = new Hashtable();
            lshtTemp.Add("FormTable", "Batch");
            CreateCorrespondence("PAY-4253", aobjCase, lshtTemp);
        }
    }
}
