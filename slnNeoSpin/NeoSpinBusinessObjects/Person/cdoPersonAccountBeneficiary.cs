#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using NeoSpin.DataObjects;
using Sagitec.BusinessObjects;
using NeoSpin.BusinessObjects;

#endregion

namespace NeoSpin.CustomDataObjects
{
    [Serializable]
	public class cdoPersonAccountBeneficiary : doPersonAccountBeneficiary
	{
		public cdoPersonAccountBeneficiary() : base()
		{
		}

        private int _plan_id;
        public int plan_id
        {
            get { return _plan_id; }
            set { _plan_id = value; }
        }

        private string _istrPlanName;
        public string istrPlanName
        {
            get { return _istrPlanName; }
            set { _istrPlanName = value; }
        }

        private bool _lblnValueEntered;
        public bool lblnValueEntered
        {
            get { return _lblnValueEntered; }
            set { _lblnValueEntered = value; }
        }

        private bool _IsEnteredInNewMode;
        public bool IsEnteredInNewMode
        {
            get { return _IsEnteredInNewMode; }
            set { _IsEnteredInNewMode = value; }
        }

        public DateTime end_date_no_null
        {
            get 
            {
                if (end_date == DateTime.MinValue)
                    return DateTime.MaxValue;
                else
                    return end_date;
            }
        }

        public int sort_order
        {
            get 
            {
                if ((beneficiary_type_value == busConstant.BeneficiaryMemberTypePrimary) &&
                    (busGlobalFunctions.CheckDateOverlapping(DateTime.Now, start_date, end_date_no_null)))
                    return 1;
                else if ((beneficiary_type_value == BusinessObjects.busConstant.BeneficiaryMemberTypeContingent) &&
                        (busGlobalFunctions.CheckDateOverlapping(DateTime.Now, start_date, end_date_no_null)))
                    return 2;
                else if ((beneficiary_type_value == busConstant.BeneficiaryMemberTypePrimary) &&
                        (!(busGlobalFunctions.CheckDateOverlapping(DateTime.Now, start_date, end_date_no_null))))
                    return 3;
                else if ((beneficiary_type_value == busConstant.BeneficiaryMemberTypeContingent) &&
                        (!(busGlobalFunctions.CheckDateOverlapping(DateTime.Now, start_date, end_date_no_null))))
                    return 4;
                return 0; 
            }
        }

        public string istrPlanParticipationStatus { get; set; }

        public DateTime idtInitialPlanStartDate { get; set; }

        //for ucs - 075 corrs. PAY-4261
        public string istrPrimaryBeneficiaryName { get; set; }
        public string istrBeneficiaryType { get; set; }
		//PIR 13054
        public override int Update()
        {
            DateTime ldtOldEndDate = DateTime.MaxValue;
            if (this.ihstOldValues != null && this.ihstOldValues.Count > 0)
            {
                ldtOldEndDate = Convert.ToDateTime(this.ihstOldValues["end_date"]);
            }
            int lintReturn = base.Update();
            if (this.person_account_id > 0)
            {
                busPersonAccount lbusPersonAccount = new busPersonAccount();
                if (lbusPersonAccount.FindPersonAccount(this.person_account_id) && (ldtOldEndDate == DateTime.MinValue) && (this.end_date != DateTime.MinValue) && lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupLife)
                {
                    lbusPersonAccount.icdoPersonAccount.no_bene_sent = busConstant.Flag_No;
                    lbusPersonAccount.icdoPersonAccount.Update();
                }
            }
            return lintReturn;
        }
    }
} 
