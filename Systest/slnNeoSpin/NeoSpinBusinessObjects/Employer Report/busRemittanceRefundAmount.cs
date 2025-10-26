#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
using System.Linq;
using System.Linq.Expressions;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busRemittanceRefundAmount : busRemittance
    {
        /// <summary>
        /// Function to find the Refund amount
        /// </summary>
        public void AssignRefundAmount()
        {
            if (String.IsNullOrEmpty(icdoRemittance.refund_pend_by))
            {
                icdoRemittance.computed_refund_amount = icdoRemittance.remittance_amount -
                    busEmployerReportHelper.GetRemittanceAllocatedAmount(icdoRemittance.remittance_id);
                icdoRemittance.overridden_refund_amount = icdoRemittance.computed_refund_amount;
            }
        }

        /// <summary>
        /// Event called when Approve button is clicked, which updates the refund amount status to approved
        /// </summary>
        /// <returns>Array list</returns>
        public ArrayList ApproveClick()
        {
            ArrayList larrList = new ArrayList();
            if (icdoRemittance.overridden_refund_amount > icdoRemittance.computed_refund_amount)
            {
                utlError lobjError = new utlError { istrErrorID = "6419", istrErrorMessage = "Overridden Refund amount cannot be greater than Computed Refund amount" };
                larrList.Add(lobjError);
            }
            //prod pir 6915 : validation added
            //--start--//
            else if (string.IsNullOrEmpty(icdoRemittance.refund_to_value))
            {
                utlError lobjError = new utlError();
                lobjError = AddError(8559, string.Empty);
                larrList.Add(lobjError);
            }
            else if (icdoRemittance.refund_to_value == busConstant.RemittanceRefundNDPERS && icdoRemittance.remittance_type_value != busConstant.RemittanceTypeIBSDeposit)
            {
                utlError lobjError = new utlError();
                lobjError = AddError(8557, string.Empty);
                larrList.Add(lobjError);
            }
            else if (icdoRemittance.refund_to_value == busConstant.RemittanceRefundDifferentOrganization && string.IsNullOrEmpty(istrDifferentOrgCode))
            {
                utlError lobjError = new utlError();
                lobjError = AddError(8558, string.Empty);
                larrList.Add(lobjError);
            }
            else if (icdoRemittance.refund_to_value == busConstant.RemittanceRefundDifferentMember && iintmemberperslinkid == 0)  //PIR 16208 
            {
                utlError lobjError = new utlError();
                lobjError = AddError(10402, string.Empty);
                larrList.Add(lobjError);
            }
            else
            {
                icdoRemittance.refund_to_id = 5014;
                icdoRemittance.refund_status_value = busConstant.DepositRefundStatusApproved;
                icdoRemittance.refund_appr_by = iobjPassInfo.istrUserID;

                //prod pir 5141
                if (icdoRemittance.refund_to_value == busConstant.RemittanceRefundSameMember || icdoRemittance.refund_to_value == busConstant.RemittanceRefundEstateOfMember)
                {
                    icdoRemittance.refund_to_person_id = icdoRemittance.person_id;
                    icdoRemittance.refund_to_org_id = 0;
                }
                else if (icdoRemittance.refund_to_value == busConstant.RemittanceRefundSameOrganization)
                {
                    icdoRemittance.refund_to_org_id = icdoRemittance.org_id;
                    icdoRemittance.refund_to_person_id = 0;
                }
                else if (icdoRemittance.refund_to_value == busConstant.RemittanceRefundDifferentOrganization)
                {
                    icdoRemittance.refund_to_org_id = busGlobalFunctions.GetOrgIdFromOrgCode(istrDifferentOrgCode);
                    icdoRemittance.refund_to_person_id = 0;
                }
                else if (icdoRemittance.refund_to_value == busConstant.RemittanceRefundDifferentMember) //PIR 16208 
                {
                    icdoRemittance.refund_to_org_id = 0;
                    icdoRemittance.refund_to_person_id = iintmemberperslinkid;
                }
                else if (icdoRemittance.refund_to_value == busConstant.RemittanceRefundNDPERS)
                {
                    icdoRemittance.refund_to_person_id = 0;
                    icdoRemittance.refund_to_org_id = 0;
                }
                //--end--// prod pir 6915
                //prod pir 5142
                if (icdoRemittance.refund_to_value == busConstant.RemittanceRefundNDPERS)
                {
                    icdoRemittance.refund_status_value = busConstant.DepositRefundStatusProcessed;
                    GenerateItemLevelGL(icdoRemittance.overridden_refund_amount > 0 ? icdoRemittance.overridden_refund_amount : icdoRemittance.computed_refund_amount,
                        icdoRemittance.plan_id, icdoRemittance.person_id, 0);
                }
                if (icdoRemittance.remittance_id > 0)
                    icdoRemittance.Update();
                icdoRemittance.refund_status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2510, icdoRemittance.refund_status_value);
                
                //displaying warning message if any allocation is in pending status
                if (IsAnyAllocationPending())
                {
                    ibusSoftErrors = new busSoftErrors();
                    ibusSoftErrors.iclbError = new Collection<busError>();
                    busError lobjError = new busError();
                    lobjError.message_id = 6420;
                    lobjError.severity_description = "Warning";
                    lobjError.display_message = "Remittance Allocation is pending for this Deposit. Please Approve the same.";
                    lobjError.error_id = 1;
                    ibusSoftErrors.iclbError.Add(lobjError);
                }
                larrList.Add(this);
            }
            EvaluateInitialLoadRules();
            return larrList;
        }

        /// <summary>
        /// Event called when Pending button is clicked, which updates the refund amount status to Pending
        /// </summary>
        /// <returns>Array list</returns>
        public ArrayList PendingClick()
        {
            ArrayList larrList = new ArrayList();
           
            if (icdoRemittance.overridden_refund_amount > icdoRemittance.computed_refund_amount)
            {
                utlError lobjError = new utlError { istrErrorID = "", istrErrorMessage = "Overridden Refund amount cannot be greater than Computed Refund amount" };
                larrList.Add(lobjError);
            }
            else if (string.IsNullOrEmpty(icdoRemittance.refund_to_value))
            {
                utlError lobjError = new utlError();
                lobjError = AddError(8559, string.Empty);
                larrList.Add(lobjError);
            }
            else if (icdoRemittance.refund_to_value == busConstant.RemittanceRefundNDPERS &&  icdoRemittance.remittance_type_value != busConstant.RemittanceTypeIBSDeposit)
            {
                utlError lobjError = new utlError();
                lobjError = AddError(8557, string.Empty);
                larrList.Add(lobjError);
            }
            else if (icdoRemittance.refund_to_value == busConstant.RemittanceRefundDifferentOrganization && string.IsNullOrEmpty(istrDifferentOrgCode))
            {
                utlError lobjError = new utlError();
                lobjError = AddError(8558, string.Empty);
                larrList.Add(lobjError);
            }
            else if (icdoRemittance.refund_to_value == busConstant.RemittanceRefundDifferentMember && (iintmemberperslinkid == 0)) //PIR 16208 
            {
                utlError lobjError = new utlError();
                lobjError = AddError(10402, string.Empty);
                larrList.Add(lobjError);
            }
            else
            {
                icdoRemittance.refund_to_id = 5014;
                icdoRemittance.refund_status_value = busConstant.DepositRefundStatusPending;
                icdoRemittance.refund_pend_by = iobjPassInfo.istrUserID;

                //prod pir 5141
                if (icdoRemittance.refund_to_value == busConstant.RemittanceRefundSameMember || icdoRemittance.refund_to_value == busConstant.RemittanceRefundEstateOfMember)
                {
                    icdoRemittance.refund_to_person_id = icdoRemittance.person_id;
                    icdoRemittance.refund_to_org_id = 0;
                }
                else if (icdoRemittance.refund_to_value == busConstant.RemittanceRefundSameOrganization)
                {
                    icdoRemittance.refund_to_org_id = icdoRemittance.org_id;
                    icdoRemittance.refund_to_person_id = 0;
                }
                else if (icdoRemittance.refund_to_value == busConstant.RemittanceRefundDifferentOrganization)
                {
                    icdoRemittance.refund_to_org_id = busGlobalFunctions.GetOrgIdFromOrgCode(istrDifferentOrgCode);
                    icdoRemittance.refund_to_person_id = 0;
                }
                else if (icdoRemittance.refund_to_value == busConstant.RemittanceRefundDifferentMember) //PIR 16208 
                {
                    icdoRemittance.refund_to_org_id = 0;
                    icdoRemittance.refund_to_person_id = iintmemberperslinkid;
                }
                else if (icdoRemittance.refund_to_value == busConstant.RemittanceRefundNDPERS)
                {
                    icdoRemittance.refund_to_person_id = 0;
                    icdoRemittance.refund_to_org_id = 0;
                }

                if (icdoRemittance.remittance_id > 0)
                    icdoRemittance.Update();
                icdoRemittance.refund_status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2510, icdoRemittance.refund_status_value);
                larrList.Add(this);
            }
            EvaluateInitialLoadRules();
            return larrList;
        }

        public bool IsAnyAllocationPending()
        {
            bool lblnResult = false;
            busIbsHeader lbusIbsHeader = new busIbsHeader();
            busServicePurchaseHeader lbusServicePurchaseHeader = new busServicePurchaseHeader();
            busEmployerPayrollHeader lbusEmployerPayrollHeader = new busEmployerPayrollHeader();
            
            lbusIbsHeader.LoadIbsRemittanceAllocationsByRemittanceID(icdoRemittance.remittance_id);
            lbusIbsHeader.LoadJsRhicRemittanceAllocationsByRemittanceID(icdoRemittance.remittance_id);
            lbusEmployerPayrollHeader.LoadEmployerRemittanceAllocationByRemittanceID(icdoRemittance.remittance_id);
            lbusServicePurchaseHeader.LoadAllocatedPurchaseListByRemittanceID(icdoRemittance.remittance_id);

            if ((lbusIbsHeader.icolIbsRemittanceAllocation.Where(o => o.icdoIbsRemittanceAllocation.ibs_allocation_status_value
                    == busConstant.IBSRemittanceAllocationStatusPending).FirstOrDefault()) != null ||
                (lbusIbsHeader.icolJsRhicRemittanceAllocation.Where(o => o.icdoJsRhicRemittanceAllocation.rhic_allocation_status_value
                    == busConstant.IBSRemittanceAllocationStatusPending).FirstOrDefault()) != null ||
                (lbusEmployerPayrollHeader.iclbEmployerRemittanceAllocation.Where(o => o.icdoEmployerRemittanceAllocation.payroll_allocation_status_value
                    == busConstant.RemittanceAllocationStatusPending).FirstOrDefault()) != null)
            {
                lblnResult = true;             
            }           
            return lblnResult;
        }

        public bool Test()
        {
            return true;
        }
        
        //prod pir 5141
        public string istrDifferentOrgCode
        {
            get;
            set;
        }
        public int iintmemberperslinkid { get;set;}

        /// <summary>
        /// Method to create item level GL for refund amount
        /// </summary>
        /// <param name="adecAmortizationInterest">refund amount</param>
        /// <param name="aintPlanID">Plan ID</param>
        /// <param name="aintPersonID">Person ID</param>
        /// <param name="aintOrgID">Org ID</param>
        public void GenerateItemLevelGL(decimal adecRefundAmount, int aintPlanID, int aintPersonID, int aintOrgID)
        {
            cdoAccountReference lcdoAccountReference = new cdoAccountReference();
            lcdoAccountReference.plan_id = aintPlanID;
            lcdoAccountReference.source_type_value = busConstant.GLSourceTypeValueBenefitPayment;
            lcdoAccountReference.transaction_type_value = busConstant.TransactionTypeItemLevel;
            lcdoAccountReference.item_type_value = busConstant.PaymentItemCodeValueExcessIBSInsuranceRefund;
            lcdoAccountReference.status_transition_value = null;
            //Generating GL
            busGLHelper.GenerateGL(lcdoAccountReference, aintPersonID, aintOrgID, icdoRemittance.remittance_id,
                adecRefundAmount, DateTime.Today, DateTime.Today, iobjPassInfo);
        }

        //prod pir 6611 : to reverse cancel refund
        public ArrayList CancelRefundClick()
        {
            ArrayList larrList = new ArrayList();

            icdoRemittance.refund_status_value = null;
            icdoRemittance.refund_pend_by = null;
            icdoRemittance.refund_appr_by = null;
            icdoRemittance.refund_to_person_id = 0;
            icdoRemittance.refund_to_org_id = 0;
            icdoRemittance.refund_to_value = null;
            icdoRemittance.computed_refund_amount = 0;
            icdoRemittance.overridden_refund_amount = 0;            

            if (icdoRemittance.remittance_id > 0)
                icdoRemittance.Update();
            icdoRemittance.refund_status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2510, icdoRemittance.refund_status_value);
            larrList.Add(this);
            
            EvaluateInitialLoadRules();
            return larrList;
        }
        //pir 7006
        public ArrayList btnReissueApprove_Click()
        {
            ArrayList larrList = new ArrayList();
            if (icdoRemittance.remittance_id > 0)
            {
                // PIR 13004 Start -- PIR 6915 fixed Approve button click not Re-issue Approve click. Fixed now.
                if (string.IsNullOrEmpty(icdoRemittance.refund_to_value))
                {
                    utlError lobjError = new utlError();
                    lobjError = AddError(8559, string.Empty);
                    larrList.Add(lobjError);
                }
                else if (icdoRemittance.refund_to_value == busConstant.RemittanceRefundNDPERS && icdoRemittance.remittance_type_value != busConstant.RemittanceTypeIBSDeposit)
                {
                    utlError lobjError = new utlError();
                    lobjError = AddError(8557, string.Empty);
                    larrList.Add(lobjError);
                }
                else if (icdoRemittance.refund_to_value == busConstant.RemittanceRefundDifferentOrganization && string.IsNullOrEmpty(istrDifferentOrgCode))
                {
                    utlError lobjError = new utlError();
                    lobjError = AddError(8558, string.Empty);
                    larrList.Add(lobjError);
                }
                else if (icdoRemittance.refund_to_value == busConstant.RemittanceRefundDifferentMember && iintmemberperslinkid == 0) //PIR 16208 
                {
                    utlError lobjError = new utlError();
                    lobjError = AddError(10402, string.Empty);
                    larrList.Add(lobjError);
                }
                else
                {
                    //prod pir 5141
                    if (icdoRemittance.refund_to_value == busConstant.RemittanceRefundSameMember || icdoRemittance.refund_to_value == busConstant.RemittanceRefundEstateOfMember)
                    {
                        icdoRemittance.refund_to_person_id = icdoRemittance.person_id;
                        icdoRemittance.refund_to_org_id = 0;
                    }
                    else if (icdoRemittance.refund_to_value == busConstant.RemittanceRefundSameOrganization)
                    {
                        icdoRemittance.refund_to_org_id = icdoRemittance.org_id;
                        icdoRemittance.refund_to_person_id = 0;
                    }
                    else if (icdoRemittance.refund_to_value == busConstant.RemittanceRefundDifferentOrganization)
                    {
                        icdoRemittance.refund_to_org_id = busGlobalFunctions.GetOrgIdFromOrgCode(istrDifferentOrgCode);
                        icdoRemittance.refund_to_person_id = 0;
                    }
                    else if (icdoRemittance.refund_to_value == busConstant.RemittanceRefundDifferentMember) //PIR 16208 
                    {
                        icdoRemittance.refund_to_org_id = 0;
                        icdoRemittance.refund_to_person_id = iintmemberperslinkid;
                    }
                    else if (icdoRemittance.refund_to_value == busConstant.RemittanceRefundNDPERS)
                    {
                        icdoRemittance.refund_to_person_id = 0;
                        icdoRemittance.refund_to_org_id = 0;
                    }
                    // PIR 13004 End
                    icdoRemittance.refund_to_id = 5014;
                    icdoRemittance.refund_status_value = busConstant.ReissueApproved;
                    icdoRemittance.Update();
                    icdoRemittance.refund_status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2510, icdoRemittance.refund_status_value);
                    larrList.Add(this);
                }
            }
            EvaluateInitialLoadRules();
            return larrList;
        }
        public ArrayList btnSaveReissue_Click()
        {
            ArrayList larrList = new ArrayList();

            if (icdoRemittance.overridden_refund_amount > icdoRemittance.computed_refund_amount)
            {
                utlError lobjError = new utlError { istrErrorID = "", istrErrorMessage = "Overridden Refund amount cannot be greater than Computed Refund amount" };
                larrList.Add(lobjError);
            }
            else if (string.IsNullOrEmpty(icdoRemittance.refund_to_value))
            {
                utlError lobjError = new utlError();
                lobjError = AddError(8559, string.Empty);
                larrList.Add(lobjError);
            }
            else if (icdoRemittance.refund_to_value == busConstant.RemittanceRefundNDPERS && icdoRemittance.remittance_type_value != busConstant.RemittanceTypeIBSDeposit)
            {
                utlError lobjError = new utlError();
                lobjError = AddError(8557, string.Empty);
                larrList.Add(lobjError);
            }
            else if (icdoRemittance.refund_to_value == busConstant.RemittanceRefundDifferentOrganization && string.IsNullOrEmpty(istrDifferentOrgCode))
            {
                utlError lobjError = new utlError();
                lobjError = AddError(8558, string.Empty);
                larrList.Add(lobjError);
            }
            else if (icdoRemittance.refund_to_value == busConstant.RemittanceRefundDifferentMember && iintmemberperslinkid == 0) //PIR 16208 
            {
                utlError lobjError = new utlError();
                lobjError = AddError(10402, string.Empty);
                larrList.Add(lobjError);
            }
            else
            {
                if (icdoRemittance.refund_to_value == busConstant.RemittanceRefundSameMember || icdoRemittance.refund_to_value == busConstant.RemittanceRefundEstateOfMember)
                {
                    icdoRemittance.refund_to_person_id = icdoRemittance.person_id;
                    icdoRemittance.refund_to_org_id = 0;
                }
                else if (icdoRemittance.refund_to_value == busConstant.RemittanceRefundSameOrganization)
                {
                    icdoRemittance.refund_to_org_id = icdoRemittance.org_id;
                    icdoRemittance.refund_to_person_id = 0;
                }
                else if (icdoRemittance.refund_to_value == busConstant.RemittanceRefundDifferentOrganization)
                {
                    icdoRemittance.refund_to_org_id = busGlobalFunctions.GetOrgIdFromOrgCode(istrDifferentOrgCode);
                    icdoRemittance.refund_to_person_id = 0;
                }
                else if (icdoRemittance.refund_to_value == busConstant.RemittanceRefundDifferentMember) //PIR 16208 
                {
                    icdoRemittance.refund_to_org_id = 0;
                    icdoRemittance.refund_to_person_id = iintmemberperslinkid;
                }
                else if (icdoRemittance.refund_to_value == busConstant.RemittanceRefundNDPERS)
                {
                    icdoRemittance.refund_to_person_id = 0;
                    icdoRemittance.refund_to_org_id = 0;
                }

                if (icdoRemittance.remittance_id > 0)
                    icdoRemittance.Update();
                
                larrList.Add(this);
            }
            EvaluateInitialLoadRules();
            return larrList;
        }
    }
}
