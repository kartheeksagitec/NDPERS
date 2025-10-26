using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoSpin.BusinessObjects;
using System.IO;
using Sagitec.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.DataObjects;
using Sagitec.DBUtility;
using NeoSpin.CustomDataObjects;
using Sagitec.ExceptionPub;
using System.Linq.Expressions;
using System.Data;

namespace NeoSpinBatch
{
    public class busWssMessageBatch : busNeoSpinBatch
    {
        public void PublishMessageToSelectedAudiencePerWssRequest()
        {
            DataTable ldtResult = busBase.Select<cdoWssMessageHeader>(
                                                                    new string[1] { "IS_MESSAGE_SENT" },
                                                                    new object[1] { busConstant.Flag_No }, null, null);
            if (ldtResult.Rows.Count > 0)
            {
                busWssMessageHeader lbusCurrentRequest = new busWssMessageHeader { icdoWssMessageHeader = new cdoWssMessageHeader() };
                foreach (DataRow dr in ldtResult.Rows)
                {
                    lbusCurrentRequest.icdoWssMessageHeader.LoadData(dr);
                    idlgUpdateProcessLog("Processing " + lbusCurrentRequest.icdoWssMessageHeader.member_type_description + " Batch Request : Batch Request ID - "
                       + Convert.ToString(lbusCurrentRequest.icdoWssMessageHeader.wss_message_id), "INFO", iobjBatchSchedule.step_name);
                    bool lblnTransaction = false;
                    int lintrtn = 0;
                    try
                    {
                        idlgUpdateProcessLog("Inserting Member's Data into WSS Message Detail Table", "INFO", iobjBatchSchedule.step_name);
                        if (lbusCurrentRequest.icdoWssMessageHeader.is_message_sent == busConstant.Flag_No)
                        {
                            if (!lblnTransaction)
                            {
                                utlPassInfo.iobjPassInfo.BeginTransaction();
                                lblnTransaction = true;
                            }
                            if (lbusCurrentRequest.icdoWssMessageHeader.member_type_value == busConstant.MemberTypeActive)
                            {
                                lintrtn = DBFunction.DBNonQuery("cdoWssMessageDetail.InsertActiveMemberWSSBatch", new object[6] { 
                                    lbusCurrentRequest.icdoWssMessageHeader.wss_message_id ,
                                    iobjSystemManagement.icdoSystemManagement.batch_date,
                                    lbusCurrentRequest.icdoWssMessageHeader.plan_id ,
                                    lbusCurrentRequest.icdoWssMessageHeader.job_class_value,
                                    lbusCurrentRequest.icdoWssMessageHeader.type_value,
                                    iobjBatchSchedule.batch_schedule_id,
                                    }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                            }
                            if (lbusCurrentRequest.icdoWssMessageHeader.member_type_value == busConstant.MemberTypeRetire)
                            {
                                lintrtn = DBFunction.DBNonQuery("cdoWssMessageDetail.InsertForRetireInWSSBatch", new object[8] {
                                      lbusCurrentRequest.icdoWssMessageHeader.wss_message_id,
                                    iobjSystemManagement.icdoSystemManagement.batch_date,
                                    lbusCurrentRequest.icdoWssMessageHeader.benefit_type_value,
                                    lbusCurrentRequest.icdoWssMessageHeader.benefit_option_value,
                                    lbusCurrentRequest.icdoWssMessageHeader.plan_id,
                                    iobjBatchSchedule.batch_schedule_id,
                                    (lbusCurrentRequest.icdoWssMessageHeader.benefit_begin_date_from == DateTime.MinValue? (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue : lbusCurrentRequest.icdoWssMessageHeader.benefit_begin_date_from),
                                    (lbusCurrentRequest.icdoWssMessageHeader.benefit_begin_date_to == DateTime.MinValue? DateTime.Now : lbusCurrentRequest.icdoWssMessageHeader.benefit_begin_date_from),                                    
                                }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                            }
                            if (lbusCurrentRequest.icdoWssMessageHeader.member_type_value == busConstant.MemberTypeEmployer)
                            {
                                lintrtn = DBFunction.DBNonQuery("cdoWssMessageDetail.InsertForEmployerInWSSBatch", new object[9] {
                                    lbusCurrentRequest.icdoWssMessageHeader.wss_message_id,
                                    iobjSystemManagement.icdoSystemManagement.batch_date,
                                    lbusCurrentRequest.icdoWssMessageHeader.plan_id,
                                    lbusCurrentRequest.icdoWssMessageHeader.emp_category_value,
                                    lbusCurrentRequest.icdoWssMessageHeader.central_payroll_flag,
                                    lbusCurrentRequest.icdoWssMessageHeader.contact_role_value, 
                                    lbusCurrentRequest.icdoWssMessageHeader.org_id,
                                    lbusCurrentRequest.icdoWssMessageHeader.peoplesoft_org_group_value,
                                    iobjBatchSchedule.batch_schedule_id
                                }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                            }
                            //updating flag to NO once the insertion is successfull. 
                            lbusCurrentRequest.icdoWssMessageHeader.is_message_sent = busConstant.Flag_Yes;
                            lbusCurrentRequest.icdoWssMessageHeader.Update();

                            if (lblnTransaction)
                            {
                                utlPassInfo.iobjPassInfo.Commit();
                                lblnTransaction = false;
                            }
                            idlgUpdateProcessLog("Inserting " + lintrtn + " Member's Data into WSS Message Detail Tables is successfully completed.", "INFO", iobjBatchSchedule.step_name);
                        }
                    }
                    catch (Exception Ex)
                    {
                        ExceptionManager.Publish(Ex);
                        if (lblnTransaction)
                        {
                            utlPassInfo.iobjPassInfo.Rollback();
                            lblnTransaction = false;
                        }
                        else
                        {
                            /// Change the Batch Request status to Failed
                            lbusCurrentRequest.icdoWssMessageHeader.is_message_sent = busConstant.Flag_No;
                            lbusCurrentRequest.icdoWssMessageHeader.Update();
                        }
                        idlgUpdateProcessLog("Message" + Ex.Message, "ERR", istrProcessName);
                    }
                    lbusCurrentRequest.icdoWssMessageHeader.Reset();
                }
            }
            else
            {
                idlgUpdateProcessLog("No WSS Message Requests Found To Process. ", "INFO", iobjBatchSchedule.step_name);
            }
        }
    }
}
