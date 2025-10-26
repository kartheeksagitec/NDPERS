using NeoSpin.BusinessObjects;
using NeoSpin.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.ExceptionPub;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoSpinBatch
{
    public class busEmailNotificationBatch : busNeoSpinBatch
    {
        public void SendEmailNotication()
        {
            DataTable ldtbResult = busBase.Select("cdoWssMessageDetail.LoadPersonForEmailNotification", new object[0] { });
            string lstrEmailFrom =  Convert.ToString(Sagitec.Common.SystemSettings.Instance["WSSRetrieveMailFrom"]);
            string lstrMessage = Convert.ToString(Sagitec.Common.SystemSettings.Instance["WSSEmailNotificationMsg"]);
            string lstrEmailSubject = Convert.ToString(Sagitec.Common.SystemSettings.Instance["WSSEmailNotificationSubject"]);
            string lstrEmailChangeEmailMsgSignature = Convert.ToString(Sagitec.Common.SystemSettings.Instance["WSSMailBodySignature"]);
            //string lstrUrl = "https://perslink.nd.gov/PERSLink/wfmLoginME.aspx ";
            string lstrMemberName = string.Empty;

            string lstrEmailMessage = string.Empty;
            //lstrMessage = string.Format(lstrMessage, "<a href = \"" + lstrUrl + "\" >", "</a>");
            if (ldtbResult.Rows.Count > 0)
            {
                List<int> llstPersonsEmailSent = new List<int>();
                foreach (DataRow ldrwDataRow in ldtbResult.Rows)
                {
                    lstrMemberName = Convert.ToString(ldrwDataRow["first_name"])+ " " + Convert.ToString(ldrwDataRow["Last_Name"]); 
                    lstrEmailMessage = string.Format(lstrMessage + lstrEmailChangeEmailMsgSignature, lstrMemberName);
                    int lintPersonId = 0;
                    int lintWssDetailId = 0;
                    try
                    {
                        lintPersonId = !Convert.IsDBNull(ldrwDataRow["PERSON_ID"]) ? Convert.ToInt32(ldrwDataRow["PERSON_ID"]) : 0;
                        lintWssDetailId = !Convert.IsDBNull(ldrwDataRow["WSS_MESSAGE_DETAIL_ID"]) ? Convert.ToInt32(ldrwDataRow["WSS_MESSAGE_DETAIL_ID"]) : 0;
                        if (lintPersonId > 0 && lintWssDetailId > 0 && !llstPersonsEmailSent.Contains(lintPersonId))
                        {
                            string lstrEmailAddress = Convert.ToString(ldrwDataRow["EMAIL_ADDRESS"]);
                            lstrEmailAddress = !string.IsNullOrEmpty(lstrEmailAddress) ? lstrEmailAddress.Trim() : lstrEmailAddress;

                            if (!string.IsNullOrEmpty(lstrEmailAddress) && busGlobalFunctions.IsValidEmail(lstrEmailAddress))
                            {
                                busGlobalFunctions.SendMail(lstrEmailFrom, lstrEmailAddress, lstrEmailSubject, lstrEmailMessage, true, true);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        idlgUpdateProcessLog($"Email cannot be sent to PERSLinkId {Convert.ToString(lintPersonId)} due to {ex.Message}", "ERR", iobjBatchSchedule.step_name);
                    }
                    finally
                    {
                        int lintQueryResult = 0;
                        lintQueryResult = DBFunction.DBNonQuery("cdoWssMessageDetail.UpdateEmailSentFlag", new object[2] { lintWssDetailId, iobjBatchSchedule.batch_schedule_id },
                                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                        if (lintQueryResult > 0 && !llstPersonsEmailSent.Contains(lintPersonId))
                            llstPersonsEmailSent.Add(lintPersonId);
                    }
                }
            }
            else
            {
                idlgUpdateProcessLog("No persons found to be E-mail notified.", "INFO", iobjBatchSchedule.step_name);
            }
        }
        public void SendESSEmailNotification()
        {
            DataTable ldtbResultESS = busBase.Select("cdoWssMessageDetail.LoadEmployerForEmailNotification", new object[0] { });
            string lstrEmailFromESS = Convert.ToString(Sagitec.Common.SystemSettings.Instance["ESSRetrieveMailFrom"]);
            string lstrMessageESS = Convert.ToString(Sagitec.Common.SystemSettings.Instance["ESSEmailNotificationMsg"]);
            string lstrEmailSubjectESS = Convert.ToString(Sagitec.Common.SystemSettings.Instance["ESSEmailNotificationSubject"]);
            string lstrEmailChangeEmailMsgSignatureESS = Convert.ToString(Sagitec.Common.SystemSettings.Instance["ESSMailBodySignature"]);
            string lstrEmployerNameESS = string.Empty;
            string lstrEmailMessageESS = string.Empty;
            if (ldtbResultESS.Rows.Count > 0)
            {
                List<int> llstOrgEmailSent = new List<int>();
                foreach (DataRow ldrwDataRow in ldtbResultESS.Rows)
                {
                    lstrEmployerNameESS = Convert.ToString(ldrwDataRow["FIRST_NAME"]) + " " + Convert.ToString(ldrwDataRow["LAST_NAME"]); ;
                    lstrEmailMessageESS = string.Format(lstrMessageESS + lstrEmailChangeEmailMsgSignatureESS, lstrEmployerNameESS);
                    int lintContactId = 0;
                    int lintWssDetailId = 0;
                    try
                    {
                        lintContactId = !Convert.IsDBNull(ldrwDataRow["CONTACT_ID"]) ? Convert.ToInt32(ldrwDataRow["CONTACT_ID"]) : 0;
                        lintWssDetailId = !Convert.IsDBNull(ldrwDataRow["WSS_MESSAGE_DETAIL_ID"]) ? Convert.ToInt32(ldrwDataRow["WSS_MESSAGE_DETAIL_ID"]) : 0;
                        if (lintContactId > 0 && lintWssDetailId > 0 && !llstOrgEmailSent.Contains(lintContactId))
                        {
                            string lstrEmailAddress = Convert.ToString(ldrwDataRow["EMAIL_ADDRESS"]);
                            lstrEmailAddress = !string.IsNullOrEmpty(lstrEmailAddress) ? lstrEmailAddress.Trim() : lstrEmailAddress;

                            if (!string.IsNullOrEmpty(lstrEmailAddress) && busGlobalFunctions.IsValidEmail(lstrEmailAddress))
                            {
                                busGlobalFunctions.SendMail(lstrEmailFromESS, lstrEmailAddress, lstrEmailSubjectESS, lstrEmailMessageESS, true, true);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        idlgUpdateProcessLog($"Email cannot be sent to Employer Contact {Convert.ToString(lintContactId)} due to {ex.Message}", "ERR", iobjBatchSchedule.step_name);
                    }
                    finally
                    {
                        int lintQueryResult = 0;
                        lintQueryResult = DBFunction.DBNonQuery("cdoWssMessageDetail.UpdateESSEmailSentFlag", new object[2] { lintWssDetailId,iobjBatchSchedule.batch_schedule_id },
                                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                        if (lintQueryResult > 0 && !llstOrgEmailSent.Contains(lintContactId))
                            llstOrgEmailSent.Add(lintContactId);
                    }
                }
            }
            else
            {
                idlgUpdateProcessLog("No Employer Contact found to be E-mail notified.", "INFO", iobjBatchSchedule.step_name);
            }
        }
    }
}
