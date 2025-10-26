#region Using directives


using System;
using System.Collections;
using NeoSpin.Common;
using Sagitec.DBUtility;
using NeoSpin.CustomDataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busFedStateTaxRate : busFedStateTaxRateGen
    {
        /// <summary>
        /// This validation is to check whether the allowance amount is changed for the same effective
        /// The allowance amount should be same for all the record which are having same tax identifier,effective date
        /// </summary>
        /// <returns>bool</returns>
        public bool IsAllowanceEnteredForEffectiveDateInvalid()
        {
            decimal ldecAllowanceAmount = 0.00M;
            if ((icdoFedStateTaxRate.tax_identifier_value != null) && (icdoFedStateTaxRate.effective_date != DateTime.MinValue))
            {
                ldecAllowanceAmount = Convert.ToDecimal(DBFunction.DBExecuteScalar("cdoFedStateTaxRate.GetAllowanceRecordForEffectiveDate",
                                                          new object[3]{
                                                          icdoFedStateTaxRate.tax_identifier_value,
                                                          icdoFedStateTaxRate.effective_date,icdoFedStateTaxRate.fed_state_tax_id},
                                                          iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
                if(ldecAllowanceAmount > 0.00M)
                {
                    if (ldecAllowanceAmount != icdoFedStateTaxRate.allowance_amount)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// If the Effective date entered in the screen is less than Next benefit payment date ,throw an error
        /// </summary>
        /// <returns>bool</returns>
        public bool IsEffectiveDateInvalid()
        {
            if (icdoFedStateTaxRate.effective_date != DateTime.MinValue)
            {
                if (icdoFedStateTaxRate.effective_date < busPayeeAccountHelper.GetLastBenefitPaymentDate().AddMonths(1))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Creates a New Record which will trigger the Daily run batch to Update Taxes.
        /// </summary>
        public ArrayList UpdateTax_Click()
        {
            ArrayList larrList = new ArrayList();
            busBatchSchedule lobjBatchSchedulae = new busBatchSchedule();
            lobjBatchSchedulae.FindBatchSchedule(58);
            lobjBatchSchedulae.icdoBatchSchedule.next_run_date = DateTime.Today;
            lobjBatchSchedulae.icdoBatchSchedule.Update();
            return larrList;
        }
        public ArrayList Approve_Click()
        {
            ArrayList larrList = new ArrayList();
            icdoFedStateTaxRate.approved_flag = busConstant.Flag_Yes;
            icdoFedStateTaxRate.Update();
            larrList.Add(this);
            return larrList;
        }
        public override void BeforePersistChanges()
        {            
            icdoFedStateTaxRate.approved_flag = busConstant.Flag_Yes;
            icdoFedStateTaxRate.tax_ref = busConstant.PayeeAccountTaxRefFed22Tax;
            base.BeforePersistChanges();
        }        
    }
}
