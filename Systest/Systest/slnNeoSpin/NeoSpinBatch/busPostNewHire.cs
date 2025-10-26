using NeoSpin.BusinessObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using System;
using System.Collections;
using System.Data;

namespace NeoSpinBatch
{
    class busPostNewHire : busNeoSpinBatch
    {
        busWssMemberRecordRequest ibusWssMemberRecordRequest = new busWssMemberRecordRequest() { icdoWssMemberRecordRequest = new NeoSpin.CustomDataObjects.cdoWssMemberRecordRequest() };
        public void ProcessNewHire()
        {
            istrProcessName = "New Hire Auto-Plan Enrollment";
            idlgUpdateProcessLog("New Hire Auto-Plan Enrollment Batch Started.", "INFO", istrProcessName);
            DataTable ldtbPendingNewHireMembers = busBase.Select("entWssMemberRecordRequest.LoadPendingMemberRecordRequest", new object[0] { });
            foreach (DataRow ldrRow in ldtbPendingNewHireMembers.Rows)
            {
                if (ibusWssMemberRecordRequest.FindWssMemberRecordRequest(Convert.ToInt32(ldrRow[NeoSpin.DataObjects.enmWssMemberRecordRequest.member_record_request_id.ToString()])))
                {
                    if ((ibusWssMemberRecordRequest.icdoWssPersonEmploymentDetail.employment_status_value == busConstant.EmploymentStatusContributing && 
                        ibusWssMemberRecordRequest.icdoWssMemberRecordRequest.first_month_for_retirement_contribution >= ibusWssMemberRecordRequest.icdoWssPersonEmployment.start_date && 
                        ibusWssMemberRecordRequest.icdoWssMemberRecordRequest.first_month_for_retirement_contribution < ibusWssMemberRecordRequest.icdoWssPersonEmployment.start_date.AddDays(45)) || 
                        (ibusWssMemberRecordRequest.icdoWssPersonEmploymentDetail.employment_status_value != busConstant.EmploymentStatusContributing))
                    {
                        try
                        {
                            if (!iobjPassInfo.iblnInTransaction) iobjPassInfo.BeginTransaction();
                            if (ibusWssMemberRecordRequest?.icdoWssPersonEmploymentDetail?.type_value == busConstant.PersonJobTypeTemporary)
                            {
                                ibusWssMemberRecordRequest.InitiateBPM();
                            }
                            else
                            {
                                ArrayList larrReturn = ibusWssMemberRecordRequest.Post_click(true);
                                if (larrReturn.Count > 0 && larrReturn[0] is utlError)
                                {
                                    ibusWssMemberRecordRequest.InitiateBPM();
                                    if (iobjPassInfo.iblnInTransaction) iobjPassInfo.Commit();
                                    continue;
                                }
                                if (ibusWssMemberRecordRequest.icdoWssPersonEmploymentDetail.person_employment_dtl_id > 0)
                                {
                                    ArrayList larlMandatoryAutoPlanEnrollmentResult = ibusWssMemberRecordRequest.AutoEnrollPersonInMandatoryPlans();
                                    if (larlMandatoryAutoPlanEnrollmentResult.Count > 0 && larlMandatoryAutoPlanEnrollmentResult[0] is utlError)
                                    {
                                        idlgUpdateProcessLog(String.Format("We Could Not Enroll The Member In The Mandatory Plans For New Hire, Please Enroll The Member with ID {0} Manually.", ibusWssMemberRecordRequest.icdoWssMemberRecordRequest.person_id), "ERR", istrProcessName);
                                    }
                                    else
                                    {
                                        ibusWssMemberRecordRequest.UpdateRequestStatus();
                                        idlgUpdateProcessLog(String.Format("The Mandatory Plans Enrollment Successfully Completed For Member with ID {0}. ", ibusWssMemberRecordRequest.icdoWssMemberRecordRequest.person_id), "INFO", istrProcessName);
                                    }
                                }
                            }
                            if (iobjPassInfo.iblnInTransaction) iobjPassInfo.Commit();
                        }
                        catch (Exception ex)
                        {
                            if (iobjPassInfo.iblnInTransaction) iobjPassInfo.Rollback();
                            idlgUpdateProcessLog($"Error Occurred While Auto-Posting {ibusWssMemberRecordRequest.icdoWssMemberRecordRequest.person_id}'s Member Record Request.", "ERR", istrProcessName);
                            if (ibusWssMemberRecordRequest.icdoWssMemberRecordRequest.status_value != busConstant.EmploymentChangeRequestStatusReview)
                            {
                                if (!iobjPassInfo.iblnInTransaction) iobjPassInfo.BeginTransaction();
                                ibusWssMemberRecordRequest.icdoWssMemberRecordRequest.status_value = busConstant.EmploymentChangeRequestStatusReview;
                                ibusWssMemberRecordRequest.icdoWssMemberRecordRequest.Update();
                                if (iobjPassInfo.iblnInTransaction) iobjPassInfo.Commit();
                            }
                        }
                    }
                    else
                    {
                        ibusWssMemberRecordRequest.InitiateBPM();
                    }
                }
            }
            idlgUpdateProcessLog("New Hire Auto-Plan Enrollment Batch Ended.", "INFO", istrProcessName);
        }
    }
}