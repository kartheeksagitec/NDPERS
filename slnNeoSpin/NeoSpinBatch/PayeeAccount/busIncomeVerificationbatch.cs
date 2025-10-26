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
using System.Linq;
#endregion

namespace NeoSpinBatch
{
    public class busIncomeVerificationbatch : busNeoSpinBatch
    {
        public void IncomeVerificationBatch()
        {
            istrProcessName = "Income Verification Batch";
            DataTable ldtbResult = busBase.Select("cdoCase.IncomeVerificationBatch", new object[1] { iobjSystemManagement.icdoSystemManagement.batch_date });
            if (ldtbResult.Rows.Count > 0)
            {
                idlgUpdateProcessLog("Processing Fetched Records", "INFO", istrProcessName);
                foreach (DataRow dr in ldtbResult.Rows)
                {
                    busCase lobjCase = new busCase { icdoCase = new cdoCase() };
                    lobjCase.ibusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                    lobjCase.icdoCase.LoadData(dr);
                    lobjCase.ibusPayeeAccount.icdoPayeeAccount.LoadData(dr);
                    lobjCase.LoadCaseDisabilityIncomeVerificationDetails();
                    lobjCase.SetComparableEarnings();
                    lobjCase.SetPayeeAccountIDAndPlan();

                    // Used in correspondence
                    if (lobjCase.icdoCase.iintPlanId == busConstant.PlanIdJobService)
                        lobjCase.idecComparableEarningPercentage = 80M;
                    else
                        lobjCase.idecComparableEarningPercentage = 70M;
                    lobjCase.idecComparateEarningAmount = lobjCase.icdoCase.comparable_earnings_amount * (lobjCase.idecComparableEarningPercentage / 100);
                    
                    //ArrayList larrList = new ArrayList();
                    //larrList.Add(lobjCase);
                    Hashtable lshtTemp = new Hashtable();
                    lshtTemp.Add("FormTable", "Batch");

                    int lintReferenceID = 0;
                    if (lobjCase.icdoCase.income_verification_date < lobjCase.icdoCase.recertification_date)
                    {
                        Collection<busCase> lclbCase = new Collection<busCase>();
                        busBase lobjBase = new busBase();
                        DataTable ldtbList = busBase.Select<cdoCase>(new string[1] { "PAYEE_ACCOUNT_ID" }, new object[1]{
                                                lobjCase.icdoCase.payee_account_id}, null, null);
                        lclbCase = lobjBase.GetCollection<busCase>(ldtbList, "icdoCase");
                        var iintCount = lclbCase.Where(o => o.icdoCase.case_type_value == busConstant.CaseStatusValuePendingMember ||
                            o.icdoCase.case_type_value == busConstant.CaseStatusValuePending3rdParty ||
                            o.icdoCase.case_type_value == busConstant.CaseStatusValuePendingNDPERS).Count();
                        if (iintCount == 0)
                        {
                            busCase lobjNewCase = new busCase { icdoCase = new cdoCase() };
                            lobjNewCase.icdoCase = lobjCase.icdoCase;
                            lobjNewCase.icdoCase.created_by = busConstant.PERSLinkBatchUser + ' ' + iobjBatchSchedule.batch_schedule_id;
                            lobjNewCase.icdoCase.modified_by = busConstant.PERSLinkBatchUser + ' ' + iobjBatchSchedule.batch_schedule_id;
                            lobjNewCase.icdoCase.created_date = iobjSystemManagement.icdoSystemManagement.batch_date;
                            lobjNewCase.icdoCase.modified_date = iobjSystemManagement.icdoSystemManagement.batch_date;
                            lobjNewCase.icdoCase.update_seq = 0;
                            lobjNewCase.icdoCase.Insert();
                            lintReferenceID = lobjNewCase.icdoCase.case_id;
                            //PIR: 1629
                            //Creating Step Details:
                            idlgUpdateProcessLog("Creating Case Detail for Case ID : " +
                            Convert.ToString(lintReferenceID), "INFO", istrProcessName);
                            lobjNewCase.LoadStepDetailsNewMode();
                            foreach (busCaseStepDetail lobjbuscasestepdetail in lobjNewCase.iclbCaseStepDetail)
                            {
                                lobjbuscasestepdetail.icdoCaseStepDetail.case_id = lobjNewCase.icdoCase.case_id;
                                lobjbuscasestepdetail.icdoCaseStepDetail.Insert();
                            }
                            lobjNewCase.SetComparableEarnings();
                            lobjNewCase.LoadIncomeVerfication();
                            lobjNewCase.InsertOrUpdateIncomeVerification();                                                                                    
                        }
                        else
                            lintReferenceID = lobjCase.icdoCase.case_id;


                        InitializeWorkFlow(busConstant.Map_Verify_Disability_Income, lobjCase.icdoCase.person_id, lintReferenceID,
                                busConstant.WorkflowProcessStatus_UnProcessed, busConstant.WorkflowProcessSource_Batch);
                    }

                    // Statement of Earnings
                    CreateCorrespondence("PAY-4256", lobjCase, lshtTemp);

                    // SFN-53157 Statement of Annual Earnings Disability Annuitants
                    CreateCorrespondence("SFN-53157", lobjCase, lshtTemp);

                    // Update the Income Verification Flag
                    lobjCase.icdoCase.income_verification_flag = busConstant.Flag_Yes;
                    lobjCase.icdoCase.Update();
                }
            }
            else
            {
                idlgUpdateProcessLog("No Records Fetched", "INFO", istrProcessName);
            }

        }        
    }
}
