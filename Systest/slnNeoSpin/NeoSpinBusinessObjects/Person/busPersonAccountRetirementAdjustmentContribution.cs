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

#endregion

namespace NeoSpin.BusinessObjects
{
	[Serializable]
    public class busPersonAccountRetirementAdjustmentContribution : busPersonAccountRetirementAdjustmentContributionGen
	{
        // Person account
        public busPersonAccountAdjustment _ibusPersonAccountAdjustment;
        public busPersonAccountAdjustment ibusPersonAccountAdjustment
        {
            get
            {
                return _ibusPersonAccountAdjustment;
            }
            set
            {
                _ibusPersonAccountAdjustment = value;
            }
        }
        public void LoadPersonAccountAdjustment()
        {
            if (_ibusPersonAccountAdjustment == null)
            {
                _ibusPersonAccountAdjustment = new busPersonAccountAdjustment();
            }
            if (icdoRetirementAdjustmentContribution.person_account_adjustment_id > 0)
            {
                _ibusPersonAccountAdjustment.FindPersonAccountAdjustment(icdoRetirementAdjustmentContribution.person_account_adjustment_id);
                _ibusPersonAccountAdjustment.LoadPersonAccount();
            }
        }
        /// <summary>
        /// Checks if pay perios and month are valid for effetcive date.
        /// </summary>
        /// <returns></returns>
        public bool IsPayPeriodValidForEffectiveDate()
        {
            bool lblnReturn = false;

            if ((icdoRetirementAdjustmentContribution.pay_period_year > 0) ||
                (icdoRetirementAdjustmentContribution.pay_period_month > 0))
            {
                try
                {
                    DateTime ldatFirstOfMonth = new DateTime(icdoRetirementAdjustmentContribution.pay_period_year, icdoRetirementAdjustmentContribution.pay_period_month, 1);
                }
                catch (Exception lexc)
                {
                    return lblnReturn;
                }
                if ((icdoRetirementAdjustmentContribution.effective_date.Year == icdoRetirementAdjustmentContribution.pay_period_year) &&
                    (icdoRetirementAdjustmentContribution.effective_date.Month == icdoRetirementAdjustmentContribution.pay_period_month))
                {
                    lblnReturn = true;
                }
                else
                    lblnReturn = false;
            }
            else
                lblnReturn = true;
            return lblnReturn;
        }
        public Collection<busPersonAccountEmploymentDetail> GetOrganizationForPersonAccount()
        {
            busPersonAccountRetirement lbusPersonAccountRetirement = (busPersonAccountRetirement)_ibusPersonAccountAdjustment.ibusPersonAccount;
            if (lbusPersonAccountRetirement.iclbAccountEmploymentDetail == null)
                lbusPersonAccountRetirement.LoadPersonAccountEmploymentDetails();
            foreach (busPersonAccountEmploymentDetail lbusEmploymentDetail in lbusPersonAccountRetirement.iclbAccountEmploymentDetail)
            {
                if (lbusEmploymentDetail.ibusEmploymentDetail == null)
                    lbusEmploymentDetail.LoadPersonEmploymentDetail();
                if (lbusEmploymentDetail.ibusEmploymentDetail.ibusPersonEmployment == null)
                    lbusEmploymentDetail.ibusEmploymentDetail.LoadPersonEmployment();
                if (lbusEmploymentDetail.ibusEmploymentDetail.ibusPersonEmployment.ibusOrganization == null)
                    lbusEmploymentDetail.ibusEmploymentDetail.ibusPersonEmployment.LoadOrganization();
            }
            return lbusPersonAccountRetirement.iclbAccountEmploymentDetail;
        }
    }

}
