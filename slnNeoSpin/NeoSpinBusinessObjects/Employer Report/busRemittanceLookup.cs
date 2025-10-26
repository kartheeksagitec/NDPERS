#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using NeoSpin.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;

#endregion

namespace NeoSpin.BusinessObjects
{
	public partial class busRemittanceLookup
	{
        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            if (aobjBus is busRemittance)
            {
                busRemittance lobjRemittance = (busRemittance)aobjBus;
                lobjRemittance.ldclAppliedAmount = busEmployerReportHelper.GetRemittanceAllocatedAmount(lobjRemittance.icdoRemittance.remittance_id);
                lobjRemittance.ldclBalanceAmount = lobjRemittance.icdoRemittance.remittance_amount - lobjRemittance.ldclAppliedAmount - lobjRemittance.icdoRemittance.allocated_negative_deposit_amount;
                //If Refund amount is there, we need to subtract from Balance Amount
                if (!string.IsNullOrEmpty(lobjRemittance.icdoRemittance.refund_pend_by))
                    lobjRemittance.ldclBalanceAmount -= lobjRemittance.icdoRemittance.overridden_refund_amount > 0 ? lobjRemittance.icdoRemittance.overridden_refund_amount :
                                                        lobjRemittance.icdoRemittance.computed_refund_amount;
                lobjRemittance.LoadDeposit(lobjRemittance.icdoRemittance.deposit_id);
                //prod pir 6422
                if (lobjRemittance.ibusDeposit.icdoDeposit.status_value == busConstant.DepositDetailStatusInvalidated 
                    || lobjRemittance.ibusDeposit.icdoDeposit.status_value == busConstant.DepositDetailStatusNonSufficientFund)
                    lobjRemittance.ldclBalanceAmount = 0.00M;
                //prod pir 6323
                if (lobjRemittance.ldclBalanceAmount == 0 && (lobjRemittance.ibusDeposit.icdoDeposit.status_value != busConstant.DepositDetailStatusInvalidated
                    && lobjRemittance.ibusDeposit.icdoDeposit.status_value != busConstant.DepositDetailStatusNonSufficientFund))
                    lobjRemittance.icdoRemittance.allocation_status = busConstant.RemittanceLookUpAllocationStatusAllocated;
                lobjRemittance.LoadDepositTape();   // PIR ID 794 - To display Deposit Tape Date in Screen.
                lobjRemittance.LoadOrgCodeID();
                
                lobjRemittance.LoadOrganization();
                lobjRemittance.LoadPersonOrgNameByID();    //PIR 8854 - To display Person Name in Grid on Remittance Lookup         
                lobjRemittance.istrOrgCodeID = lobjRemittance.ibusOrganization.icdoOrganization.org_code;
                lobjRemittance.LoadRemittancePersonOrOrgName();    //PIR 9826
                lobjRemittance.LoadPlan();
            }
        }
	}
}
