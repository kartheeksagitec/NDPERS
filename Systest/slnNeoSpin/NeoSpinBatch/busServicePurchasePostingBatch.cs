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
using System.Globalization;
using System.Linq;
using System.Collections;
#endregion

namespace NeoSpinBatch
{
    /// <summary>
    /// Batch class to post service credit as payment is allocated.
    /// </summary>
    class busServicePurchasePostingBatch : busNeoSpinBatch
    {
        public busServicePurchasePostingBatch()
        {
        }

        private Collection<busServicePurchaseHeader> _iclbApprovedServicePuchase;

        public Collection<busServicePurchaseHeader> iclbApprovedServicePuchase
        {
            get { return _iclbApprovedServicePuchase; }
            set { _iclbApprovedServicePuchase = value; }
        }

        public void PostPurchasePaymentAndServiceCredit()
        {
            istrProcessName = iobjBatchSchedule.step_name;
            busServicePurchaseHeader lbusServicePurchase = new busServicePurchaseHeader();

            DataTable ldtbList = busNeoSpinBase.Select("cdoServicePurchaseHeader.GetServicePurchaseForPosting", new object[0] { });
            _iclbApprovedServicePuchase = lbusServicePurchase.GetCollection<busServicePurchaseHeader>(ldtbList, "icdoServicePurchaseHeader");

            bool lblnInTransaction = false;
            decimal ldecPreTaxEE = 0, ldecPostTaxEE = 0, ldecPostTaxER = 0, ldecPostTaxEERHIC = 0, ldecPreTaxERRHIC = 0, ldecPreTaxER = 0, ldecEREEPickup = 0;
            decimal ldecPSCToPost = 0, ldecVSCToPost = 0;
            foreach (busServicePurchaseHeader lbusServicePurchaseToPost in _iclbApprovedServicePuchase)
            {
                try
                {
                    if (!lblnInTransaction)
                    {
                        utlPassInfo.iobjPassInfo.BeginTransaction();
                        lblnInTransaction = true;
                    }
                    lbusServicePurchaseToPost.idatRunDate = iobjSystemManagement.icdoSystemManagement.batch_date;
                    if (lbusServicePurchaseToPost.ibusPersonAccount == null)
                        lbusServicePurchaseToPost.LoadPersonAccount();
                    if (lbusServicePurchaseToPost.ibusPersonAccount.icdoPersonAccount.person_account_id == 0)
                        throw new Exception("An open person account for retirement is not found for person / plan.");
                    if (lbusServicePurchaseToPost.ibusPersonAccount.ibusPlan == null)
                        lbusServicePurchaseToPost.ibusPersonAccount.LoadPlan();
                    if (lbusServicePurchaseToPost.ibusPersonAccount.ibusPerson == null)
                        lbusServicePurchaseToPost.ibusPersonAccount.LoadPerson();
                    if (lbusServicePurchaseToPost.iclbAllocatedRemittance == null)
                        lbusServicePurchaseToPost.LoadPaymentAllocated();
                    if (lbusServicePurchaseToPost.ibusPrimaryServicePurchaseDetail == null)
                        lbusServicePurchaseToPost.LoadServicePurchaseDetail();
                    if ((lbusServicePurchaseToPost.icdoServicePurchaseHeader.grant_free_flag == "Y") || (lbusServicePurchaseToPost.IsFreeServiceOnly()))
                    {
                        ldecPreTaxEE = 0;
                        ldecPostTaxEE = 0;
                        ldecPostTaxER = 0;
                        ldecPostTaxEERHIC = 0;
                        ldecPreTaxERRHIC = 0;
                        ldecPreTaxER = 0;
                        ldecEREEPickup = 0;
                        //prod pir 4768 : need to post effective date as last month's last day
                        lbusServicePurchaseToPost.PostPersonAccountRetirement(
                            ldecPreTaxEE, ldecPostTaxEE, ldecPostTaxER, ldecPostTaxEERHIC, ldecPreTaxERRHIC, ldecPreTaxER, ldecEREEPickup, 1, ref ldecPSCToPost, ref ldecVSCToPost,
                            lbusServicePurchaseToPost.idatRunDate.AddMonths(-1).GetLastDayofMonth());
                    }
                    else
                    {
                        if (lbusServicePurchaseToPost.iclbAllocatedRemittance.Any(i => i.icdoServicePurchasePaymentAllocation.posted_flag != busConstant.Flag_Yes))
                        {
                            foreach (busServicePurchasePaymentAllocation lbusAllocatedRemittance in lbusServicePurchaseToPost.iclbAllocatedRemittance)
                            {
                                if (lbusAllocatedRemittance.icdoServicePurchasePaymentAllocation.posted_flag != "Y")
                                {
                                    lbusServicePurchaseToPost.GetDistributedRemittance(lbusAllocatedRemittance.icdoServicePurchasePaymentAllocation.service_purchase_payment_class_value,
                                        lbusAllocatedRemittance.icdoServicePurchasePaymentAllocation.applied_amount, ref ldecPreTaxEE, ref ldecPostTaxEE, ref ldecPostTaxER,
                                        ref ldecPostTaxEERHIC, ref ldecPreTaxERRHIC, ref ldecPreTaxER, ref ldecEREEPickup);
                                    //Posting GL
                                    lbusServicePurchaseToPost.PostGLAccount(ldecPreTaxEE, ldecPostTaxEE, ldecPostTaxER, ldecPostTaxEERHIC, ldecPreTaxERRHIC, ldecPreTaxER, 1);
                                    if (lbusServicePurchaseToPost.ibusPersonAccount.ibusPlan.IsDCRetirementPlan() ||
                                        lbusServicePurchaseToPost.ibusPersonAccount.ibusPlan.IsHBRetirementPlan())
                                    {
                                        //Post report to DC provider
                                        busProviderReportDataDC lbusProviderReport = new busProviderReportDataDC();
                                        lbusProviderReport.PostContribution(busConstant.SubSystemValueServicePurchase,
                                            lbusServicePurchaseToPost.icdoServicePurchaseHeader.service_purchase_header_id,
                                            lbusServicePurchaseToPost.ibusPersonAccount.ibusPerson.icdoPerson.ssn,
                                            lbusServicePurchaseToPost.ibusPersonAccount.ibusPerson.icdoPerson.person_id,
                                            lbusServicePurchaseToPost.ibusPersonAccount.ibusPerson.icdoPerson.last_name,
                                            lbusServicePurchaseToPost.ibusPersonAccount.ibusPerson.icdoPerson.first_name,
                                            lbusServicePurchaseToPost.ibusPersonAccount.icdoPersonAccount.plan_id,
                                            iobjSystemManagement.icdoSystemManagement.batch_date,
                                            ldecPostTaxER, ldecPostTaxEE, ldecPreTaxER, ldecPreTaxEE, ldecEREEPickup, 0, 0,
                                            lbusServicePurchaseToPost.ibusPersonAccount.icdoPersonAccount.person_account_id,
                                            lbusServicePurchaseToPost.ibusPersonAccount.icdoPersonAccount.provider_org_id);
                                    }

                                    //PIR 25553 effective date should be deposit date
                                    if (lbusAllocatedRemittance.icdoServicePurchasePaymentAllocation.remittance_id > 0)
                                    {
                                        object lobjDepositDate = DBFunction.DBExecuteScalar("cdoServicePurchaseHeader.GetDepositDateFromRemittanceId",
                                                                            new object[1] { lbusAllocatedRemittance.icdoServicePurchasePaymentAllocation.remittance_id },
                                                                            iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                                        DateTime ldtmDepositDate;
                                        if (lobjDepositDate.IsNotNull())
                                            ldtmDepositDate = Convert.ToDateTime(lobjDepositDate);
                                        else
                                            ldtmDepositDate = DateTime.MinValue;

                                        if (ldtmDepositDate != DateTime.MinValue)
                                        {
                                            lbusServicePurchaseToPost.PostPersonAccountRetirement(
                                                ldecPreTaxEE, ldecPostTaxEE, ldecPostTaxER, ldecPostTaxEERHIC, ldecPreTaxERRHIC, ldecPreTaxER, ldecEREEPickup, 1, ref ldecPSCToPost, ref ldecVSCToPost,
                                                ldtmDepositDate, lbusAllocatedRemittance);
                                        }
                                        else
                                        {
                                            //prod pir 4768 : need to post effective date as last month's last day
                                            lbusServicePurchaseToPost.PostPersonAccountRetirement(
                                                ldecPreTaxEE, ldecPostTaxEE, ldecPostTaxER, ldecPostTaxEERHIC, ldecPreTaxERRHIC, ldecPreTaxER, ldecEREEPickup, 1, ref ldecPSCToPost, ref ldecVSCToPost,
                                                lbusAllocatedRemittance.icdoServicePurchasePaymentAllocation.payment_date.AddMonths(-1).GetLastDayofMonth(), lbusAllocatedRemittance);
                                        }
                                    }
                                    else
                                    {
                                        //prod pir 4768 : need to post effective date as last month's last day
                                        lbusServicePurchaseToPost.PostPersonAccountRetirement(
                                            ldecPreTaxEE, ldecPostTaxEE, ldecPostTaxER, ldecPostTaxEERHIC, ldecPreTaxERRHIC, ldecPreTaxER, ldecEREEPickup, 1, ref ldecPSCToPost, ref ldecVSCToPost,
                                            lbusAllocatedRemittance.icdoServicePurchasePaymentAllocation.payment_date.AddMonths(-1).GetLastDayofMonth(), lbusAllocatedRemittance);
                                    }

                                    // Update allocated remittance status
                                    if (lbusAllocatedRemittance.icdoServicePurchasePaymentAllocation.posted_flag != "Y")
                                    {
                                        lbusAllocatedRemittance.icdoServicePurchasePaymentAllocation.prorated_psc = ldecPSCToPost;
                                        lbusAllocatedRemittance.icdoServicePurchasePaymentAllocation.prorated_vsc = ldecVSCToPost;
                                        lbusAllocatedRemittance.icdoServicePurchasePaymentAllocation.posted_flag = "Y";
                                        lbusAllocatedRemittance.icdoServicePurchasePaymentAllocation.pre_tax_ee_amount = ldecPreTaxEE;
                                        lbusAllocatedRemittance.icdoServicePurchasePaymentAllocation.pre_tax_er_amount = ldecPreTaxER;
                                        lbusAllocatedRemittance.icdoServicePurchasePaymentAllocation.post_tax_ee_amount = ldecPostTaxEE;
                                        lbusAllocatedRemittance.icdoServicePurchasePaymentAllocation.post_tax_er_amount = ldecPostTaxER;
                                        lbusAllocatedRemittance.icdoServicePurchasePaymentAllocation.ee_rhic_amount = ldecPostTaxEERHIC;
                                        lbusAllocatedRemittance.icdoServicePurchasePaymentAllocation.er_rhic_amount = ldecPreTaxERRHIC;
                                        lbusAllocatedRemittance.icdoServicePurchasePaymentAllocation.Update();
                                    }
                                }
                            }
                        }
                        //UAT PIR 726 - When Unused Sick Leave Purchase gets reduced (over paid)
                        else if (lbusServicePurchaseToPost.icdoServicePurchaseHeader.service_purchase_type_value == busConstant.Service_Purchase_Type_Unused_Sick_Leave)
                        {
                            decimal ldecTotalPaidAmount = lbusServicePurchaseToPost.icdoServicePurchaseHeader.paid_rhic_cost_amount
                                + lbusServicePurchaseToPost.icdoServicePurchaseHeader.paid_retirement_ee_cost_amount
                                + lbusServicePurchaseToPost.icdoServicePurchaseHeader.paid_retirement_er_cost_amount;
                            decimal ldecTotalPurchaseAmount = lbusServicePurchaseToPost.ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.retirement_purchase_cost +
                                lbusServicePurchaseToPost.ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.rhic_purchase_cost;
                            if (ldecTotalPaidAmount >= ldecTotalPurchaseAmount && ldecTotalPaidAmount > 0)
                            {
                                ldecPreTaxEE = 0;
                                ldecPostTaxEE = 0;
                                ldecPostTaxER = 0;
                                ldecPostTaxEERHIC = 0;
                                ldecPreTaxERRHIC = 0;
                                ldecPreTaxER = 0;
                                ldecEREEPickup = 0;
                                //prod pir 4768 : need to post effective date as last month's last day
                                lbusServicePurchaseToPost.PostPersonAccountRetirement(
                                    ldecPreTaxEE, ldecPostTaxEE, ldecPostTaxER, ldecPostTaxEERHIC, ldecPreTaxERRHIC, ldecPreTaxER, ldecEREEPickup, 1, ref ldecPSCToPost, ref ldecVSCToPost,
                                    lbusServicePurchaseToPost.idatRunDate.AddMonths(-1).GetLastDayofMonth());
                                
                                //PROD PIR ID 7482 -- The scenario where the user didn't check the ready to post flag and the batch run posts the contribution with zero PSC. 
                                // Then later when the user checks that flag the batch creates a new contribution with PSC and VSC but not updated the payment allocation.
                                var lvar = lbusServicePurchaseToPost.iclbAllocatedRemittance.Where(lobj => lobj.icdoServicePurchasePaymentAllocation.posted_flag == busConstant.Flag_Yes &&
                                     (lobj.icdoServicePurchasePaymentAllocation.prorated_vsc == 0 || lobj.icdoServicePurchasePaymentAllocation.prorated_psc == 0)).FirstOrDefault();
                                if (lvar != null && ldecPSCToPost >0 && ldecVSCToPost >0)
                                {
                                    lvar.icdoServicePurchasePaymentAllocation.prorated_psc = ldecPSCToPost;
                                    lvar.icdoServicePurchasePaymentAllocation.prorated_vsc = ldecVSCToPost;
                                    lvar.icdoServicePurchasePaymentAllocation.Update();
                                }
                            }
                        }
                    }

                    if (lblnInTransaction)
                    {
                        utlPassInfo.iobjPassInfo.Commit();
                        lblnInTransaction = false;
                    }

                    idlgUpdateProcessLog(" PERSLink ID : " + lbusServicePurchaseToPost.icdoServicePurchaseHeader.person_id + ". " +
                        " Purchase ID : " + lbusServicePurchaseToPost.icdoServicePurchaseHeader.service_purchase_header_id + ". " +
                        " Status / Action Status : " + lbusServicePurchaseToPost.icdoServicePurchaseHeader.service_purchase_status_value + " / " +
                        lbusServicePurchaseToPost.icdoServicePurchaseHeader.action_status_value,
                        "INFO", iobjBatchSchedule.step_name);
                    //PIR 17204
                    if(lbusServicePurchaseToPost.icdoServicePurchaseHeader.action_status_value == busConstant.Service_Purchase_Action_Status_Paid_In_Full)
                       GenerateCorrespondenceForServicePurchase(lbusServicePurchaseToPost);
                }
                catch (Exception lexc)
                {
                    if (lblnInTransaction)
                    {
                        utlPassInfo.iobjPassInfo.Rollback();
                        lblnInTransaction = false;
                    }
                    idlgUpdateProcessLog(" PERSLink ID : " + lbusServicePurchaseToPost.icdoServicePurchaseHeader.person_id + ". " +
                        " Purchase ID : " + lbusServicePurchaseToPost.icdoServicePurchaseHeader.service_purchase_header_id + ". " +
                        " Message : " + lexc.Message, "ERR", iobjBatchSchedule.step_name);
                }
            }
        }
		//PIR 17204
        public void GenerateCorrespondenceForServicePurchase(busServicePurchaseHeader abusServicePurchaseHeader)
        {   
            if(abusServicePurchaseHeader.iclbServicePurchaseAmortizationSchedule.IsNull())
              abusServicePurchaseHeader.LoadAmortizationSchedule();
            //ArrayList larrList = new ArrayList();
            //larrList.Add(abusServicePurchaseHeader);
            Hashtable lshtTemp = new Hashtable();
            lshtTemp.Add("FormTable", "Batch");            
            string istrFileName = CreateCorrespondence("PUR-8313", abusServicePurchaseHeader, lshtTemp);
        }
    }
}
