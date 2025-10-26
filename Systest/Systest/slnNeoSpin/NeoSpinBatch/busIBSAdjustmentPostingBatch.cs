using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using NeoSpin.BusinessObjects;
using NeoSpin.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;

namespace NeoSpinBatch
{
    class busIBSAdjustmentPostingBatch : busNeoSpinBatch
    {
        private Collection<busIbsHeader> _iclbIBSAdjustmentHeaderToPost;

        public Collection<busIbsHeader> iclbIBSAdjustmentHeaderToPost
        {
            get { return _iclbIBSAdjustmentHeaderToPost; }
            set { _iclbIBSAdjustmentHeaderToPost = value; }
        }

        public void PostIBSAdjustmentRecords()
        {
            //Load all the IBS Header Recors which are in Ready to Post Status
            istrProcessName = "IBS Adjustment Posting";
            DataTable ldtbList = busNeoSpinBase.Select("cdoIbsHeader.LoadIBSAdjustmentForPosting", new object[0] { });
            _iclbIBSAdjustmentHeaderToPost = new busBase().GetCollection<busIbsHeader>(ldtbList, "icdoIbsHeader");

            //utlPassInfo.iobjPassInfo.idictParams[utlConstants.istrProcessAuditLogSync] = true;
            foreach (busIbsHeader lbusIBSHeader in _iclbIBSAdjustmentHeaderToPost)
            {
                try
                {
                    if (utlPassInfo.iobjPassInfo.iconFramework.ConnectionString.IsNullOrEmpty())
                        utlPassInfo.iobjPassInfo.iconFramework.ConnectionString = DBFunction.GetDBConnection().ConnectionString;
                    utlPassInfo.iobjPassInfo.BeginTransaction();
                    //Set the Batch Date
                    lbusIBSHeader.idtBatchDate = iobjSystemManagement.icdoSystemManagement.batch_date;
                    //set the gl posting date
                    lbusIBSHeader.idtGLPostingDate = iobjSystemManagement.icdoSystemManagement.batch_date;

                    //Reload the IBS Collection
                    lbusIBSHeader.LoadIbsDetails();

                    //prod pir 5389
                    //need to upate paid permium in -ve adj if any contribution got posted after enrollment change is done
                    lbusIBSHeader.UpdatePaidPremiumAmountForNegativeAdj();

                    //ucs - 038 addendum
                    //update paid premium amount col. for positive adj, if negative adjustment row have paid premium amount
                    lbusIBSHeader.UpdatePaidPremiumAmount();

                    //Set the Object State
                    lbusIBSHeader.icdoIbsHeader.ienuObjectState = ObjectState.Update;

                    //Update PAPIT entries for all plans
                    lbusIBSHeader.UpdatePAPITItems();

                    //Reload and Update the Summary Data (This will update the Latest RHIC Amount into JSBill
                    lbusIBSHeader.UpdateSummaryData(busConstant.IBSHeaderStatusPosted);

                    //Allocate the RHIC Remittance. 
                    //If the Positive Adjustment, Allocate it.
                    //If Neg Adjustment, Create a Deposit / Remittance
                    if (lbusIBSHeader.ibusJsRhicBill != null)
                    {
                        if (lbusIBSHeader.ibusJsRhicBill.icdoJsRhicBill.bill_amount > 0)
                        {
                            lbusIBSHeader.AllocateJSRHICRemittance();

                            //uat pir 2061 : gl creation moved to common gl creation code as per satya
                            /*//GL for JSRHIC
                            idlgUpdateProcessLog("GL for JS RHIC", "INFO", istrProcessName);
                            lbusIBSHeader.GenerateGLForJSRHICAllocation(lbusIBSHeader.ibusJsRhicBill.icdoJsRhicBill.org_id,
                                busConstant.ItemTypeJobSeriveHealthCredit, lbusIBSHeader.ibusJsRhicBill.icdoJsRhicBill.bill_amount);
                            idlgUpdateProcessLog("GL Generated for JS RHIC", "INFO", istrProcessName);*/
                        }
                        else if (lbusIBSHeader.ibusJsRhicBill.icdoJsRhicBill.bill_amount < 0)
                        {
                            cdoDeposit lcdoDeposit = new cdoDeposit();
                            lbusIBSHeader.CreateDepositForJSRHICNegativeAdjustment(lcdoDeposit);
                            lbusIBSHeader.CreateRemittanceForJSRHICNegativeAdjustment(lcdoDeposit);

                            //uat pir 2061 : gl creation moved to common gl creation code as per satya
                            /*//Reversel GL for JSRHIC
                            idlgUpdateProcessLog("GL for JS RHIC", "INFO", istrProcessName);
                            lbusIBSHeader.GenerateGLForReverseJSRHICAllocation(lbusIBSHeader.ibusJsRhicBill.icdoJsRhicBill.org_id,
                                busConstant.ItemTypeJobSeriveHealthCredit, lbusIBSHeader.ibusJsRhicBill.icdoJsRhicBill.bill_amount * -1M);
                            idlgUpdateProcessLog("GL Generated for JS RHIC", "INFO", istrProcessName);*/
                        }
                    }

                    idlgUpdateProcessLog("Posting Contributions", "INFO", istrProcessName);
                    lbusIBSHeader.PostIBSContributionDetails(busConstant.TransactionTypeAdjustmentIBS);
                    idlgUpdateProcessLog("Posting Contributions Process Completed", "INFO", istrProcessName);
                    /*
                    idlgUpdateProcessLog("Creating Remittance for Negative Adjustments", "INFO", istrProcessName);
                    lbusIBSHeader.CreateRemittanceForNegativeAdjustment();
                    idlgUpdateProcessLog("Creating Remittance for Negative Adjustments Generated", "INFO", istrProcessName);
                    */
                    idlgUpdateProcessLog("Creating Payment election / Remittance for Negative Adjustments", "INFO", istrProcessName);
                    lbusIBSHeader.CreateAdjustmentOrDeposit();
                    idlgUpdateProcessLog("Creating Payment election / Remittance for Negative Adjustments Completed", "INFO", istrProcessName);

                    idlgUpdateProcessLog("Posting GL", "INFO", istrProcessName);
                    lbusIBSHeader.GenerateGL();
                    idlgUpdateProcessLog("Posting Generated", "INFO", istrProcessName);

                    idlgUpdateProcessLog("Posting Provider Data", "INFO", istrProcessName);
                    lbusIBSHeader.PostProviderData();
                    idlgUpdateProcessLog("Posting Provider Data Completed", "INFO", istrProcessName);

                    utlPassInfo.iobjPassInfo.Commit();

                    idlgUpdateProcessLog(" IBS Header ID : " + lbusIBSHeader.icdoIbsHeader.ibs_header_id + " Adjustment Posted!", "INFO", iobjBatchSchedule.step_name);

                    GenerateJobServiceRHICReport(lbusIBSHeader);
                }
                catch (Exception lexc)
                {
                    utlPassInfo.iobjPassInfo.Rollback();
                    idlgUpdateProcessLog(" IBS Header ID : " + lbusIBSHeader.icdoIbsHeader.ibs_header_id + ". " +
                        " Message : " + lexc.Message, "ERR", iobjBatchSchedule.step_name);
                }
            }
            //utlPassInfo.iobjPassInfo.idictParams.Remove(utlConstants.istrProcessAuditLogSync);
        }

        //Generate Job service rhic report
        public void GenerateJobServiceRHICReport(busIbsHeader abusIBSHeader)
        {
            idlgUpdateProcessLog("Generate Job Service RHIC Report", "INFO", istrProcessName);
            abusIBSHeader.idtJSRHICReportTable = new DataTable();
            abusIBSHeader.idtJSRHICReportTable = abusIBSHeader.CreateNewDataTableForJSRHICReport();
            foreach (busIbsDetail lobjIbsDetail in abusIBSHeader.icolIbsDetail)
            {
                if (lobjIbsDetail.icdoIbsDetail.js_rhic_amount > 0.0m)
                {
                    if (lobjIbsDetail.ibusPerson.IsNull())
                        lobjIbsDetail.LoadPerson();
                    abusIBSHeader.AddToNewDataRow(lobjIbsDetail);
                }
            }
            if (abusIBSHeader.idtJSRHICReportTable.Rows.Count > 0)
            {
                //prod pir 5762 : order by last name
                abusIBSHeader.idtJSRHICReportTable = abusIBSHeader.idtJSRHICReportTable.AsEnumerable()
                                                        .OrderBy(o => o.Field<string>("LastName"))
                                                        .ThenBy(o => o.Field<string>("FirstName")).AsDataTable();
                //create report for JobService Rhic
                CreateReport("rptJobServiceRHICReport.rpt", abusIBSHeader.idtJSRHICReportTable);

                idlgUpdateProcessLog("Job Service RHIC Report generated succesfully", "INFO", istrProcessName);
            }
            else
            {
                idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
            }
        }

        private void SetDeductionIdentifier(busPerson aobjPerson, busIbsDetail aobjIbsDetail)
        {
            if (aobjIbsDetail.icdoIbsDetail.mode_of_payment_value == busConstant.IBSModeOfPaymentACH)
            {
                aobjPerson.DeductionMethodIdentifier = "3";
            }
            else if (aobjIbsDetail.icdoIbsDetail.mode_of_payment_value == busConstant.IBSModeOfPaymentPensionCheck)
            {
                aobjPerson.DeductionMethodIdentifier = "1";
            }
            else if (aobjIbsDetail.icdoIbsDetail.mode_of_payment_value == busConstant.IBSModeOfPaymentPersonalCheck)
            {
                //lblnPaymentModePersonalCheck = true;
                aobjPerson.DeductionMethodIdentifier = "2";
            }
        }
    }
}
