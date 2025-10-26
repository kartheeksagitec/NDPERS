using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using NeoSpin.BusinessObjects;
using System.Collections.ObjectModel;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
using System.Collections;
using Sagitec.DBUtility;

namespace NeoSpinBatch
{
    class busUpdateDuesBatch : busNeoSpinBatch
    {
        public busUpdateDuesBatch()
        { }

        public void UpdateDues()
        {
            istrProcessName = "Update Dues Rate Batch";

            idlgUpdateProcessLog("Update Dues Rate Batch Started", "INFO", istrProcessName);
            idlgUpdateProcessLog("Load all the Approved Entries to Process", "INFO", istrProcessName);

            //get all request that needs to be processed
            DataTable ldtbList = busBase.Select<cdoDuesRateChangeRequest>(new string[1] { "STATUS_VALUE" },
                                                                          new object[1] { busConstant.StatusApproved }, null, null);
            if (ldtbList.Rows.Count > 0)
            {
                foreach (DataRow ldrRow in ldtbList.Rows)
                {
                    try
                    {
                        busDuesRateChangeRequest lobjDuesRateChange = new busDuesRateChangeRequest { icdoDuesRateChangeRequest = new cdoDuesRateChangeRequest() };
                        lobjDuesRateChange.icdoDuesRateChangeRequest.LoadData(ldrRow);

                        DateTime ldtEffectiveDate = lobjDuesRateChange.icdoDuesRateChangeRequest.effective_date;
                        DateTime ldtEnddate = ldtEffectiveDate.AddDays(-1);

                        idlgUpdateProcessLog(
                            "Update the Payee Account Payment Item with end date as " + ldtEnddate + " for vendor org id" +
                            lobjDuesRateChange.icdoDuesRateChangeRequest.vendor_org_id, "INFO",
                            istrProcessName);

                        DataTable ldtbPAPITList = busBase.Select("cdoDuesRateChangeRequest.LoadDuesPAPITDataByVendor",
                                                                       new object[2]
                                                                       {
                                                                           lobjDuesRateChange.icdoDuesRateChangeRequest.effective_date,
                                                                           lobjDuesRateChange.icdoDuesRateChangeRequest.vendor_org_id                                                                           
                                                                       });

                        foreach (DataRow ldrPAPITRow in ldtbPAPITList.Rows)
                        {
                            DateTime ldtTempdate = DateTime.MinValue;
                            var lobjPayeeAccountPaymentItemType = new busPayeeAccountPaymentItemType
                                                                      {
                                                                          icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType()
                                                                      };

                            lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.LoadData(ldrPAPITRow);

                            if (lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.start_date < ldtEffectiveDate)
                            {
                                if (lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.end_date == DateTime.MinValue)
                                {
                                    lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.end_date = ldtEnddate;
                                    //as per meeting with satya on Aug 13,2010
                                    lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.batch_schedule_id = busConstant.BatchScheduleIDUpdateDuesRate;
                                    lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.Update();

                                    //create  and insert into new payee account payment item type
                                    busPayeeAccountPaymentItemType lobjNewPayeeAccountPaymentItemType = new busPayeeAccountPaymentItemType { icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType() };
                                    lobjNewPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.payee_account_id = lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.payee_account_id;
                                    lobjNewPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.payment_item_type_id = lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.payment_item_type_id;
                                    lobjNewPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.vendor_org_id = lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.vendor_org_id;
                                    lobjNewPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.start_date = ldtEffectiveDate;
                                    lobjNewPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.amount = lobjDuesRateChange.icdoDuesRateChangeRequest.monthly_amount;
                                    //as per meeting with satya on Aug 13,2010
                                    lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.batch_schedule_id = busConstant.BatchScheduleIDUpdateDuesRate;
                                    lobjNewPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.Insert();
                                }
                                else if (lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.end_date >= ldtEffectiveDate)
                                {
                                    ldtTempdate = lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.end_date;
                                    lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.end_date = ldtEnddate;
                                    //as per meeting with satya on Aug 13,2010
                                    lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.batch_schedule_id = busConstant.BatchScheduleIDUpdateDuesRate;
                                    lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.Update();

                                    //create  and insert into new payee account payment item type
                                    busPayeeAccountPaymentItemType lobjNewPayeeAccountPaymentItemType = new busPayeeAccountPaymentItemType { icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType() };
                                    lobjNewPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.payee_account_id = lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.payee_account_id;
                                    lobjNewPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.payment_item_type_id = lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.payment_item_type_id;
                                    lobjNewPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.vendor_org_id = lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.vendor_org_id;
                                    lobjNewPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.start_date = ldtEffectiveDate;
                                    lobjNewPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.end_date = ldtTempdate;
                                    lobjNewPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.amount = lobjDuesRateChange.icdoDuesRateChangeRequest.monthly_amount;
                                    //as per meeting with satya on Aug 13,2010
                                    lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.batch_schedule_id = busConstant.BatchScheduleIDUpdateDuesRate;
                                    lobjNewPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.Insert();

                                }
                            }
                            else
                            {
                                lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.amount = lobjDuesRateChange.icdoDuesRateChangeRequest.monthly_amount;
                                //as per meeting with satya on Aug 13,2010
                                lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.batch_schedule_id = busConstant.BatchScheduleIDUpdateDuesRate;
                                lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.Update();
                            }
                        }

                        idlgUpdateProcessLog("Update Dues Rate Batch Ended", "INFO", istrProcessName);

                        lobjDuesRateChange.icdoDuesRateChangeRequest.status_value = busConstant.StatusProcessed;
                        lobjDuesRateChange.icdoDuesRateChangeRequest.Update();
                    }
                    catch (Exception e)
                    {
                        idlgUpdateProcessLog("Update Dues Rate Batch Failed Due to following reason" + e, "INFO", istrProcessName);
                    }
                }
            }
            else
            {
                idlgUpdateProcessLog("No requests found", "INFO", istrProcessName);
            }
        }
    }
}
