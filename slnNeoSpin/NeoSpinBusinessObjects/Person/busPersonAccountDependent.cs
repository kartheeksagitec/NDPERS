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
using NeoSpin.DataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
	[Serializable]
	public class busPersonAccountDependent : busPersonDependent
	{
        private cdoPersonAccountDependent _icdoPersonAccountDependent;
        public cdoPersonAccountDependent icdoPersonAccountDependent
        {
            get
            {
                return _icdoPersonAccountDependent;
            }
            set
            {
                _icdoPersonAccountDependent = value;
            }
        }

        public Collection<busPersonAccountDependent> iclbCorPersonAccountDependent { get; set; } // PIR 20584
        public decimal idecMonthlyPremiumAmount { get; set; } // PIR 20584
        public string istrCorPlanName { get; set; } // PIR 20584

        private busPlan _ibusPlan;
        public busPlan ibusPlan
        {
            get { return _ibusPlan; }
            set { _ibusPlan = value; }
        }

        private busPersonAccount _ibusPersonAccount;
        public busPersonAccount ibusPersonAccount
        {
            get { return _ibusPersonAccount; }
            set { _ibusPersonAccount = value; }
        }

        private busPersonAccountGhdv _ibusPersonAccountGhdv;
        public busPersonAccountGhdv ibusPersonAccountGhdv
        {
            get { return _ibusPersonAccountGhdv; }
            set { _ibusPersonAccountGhdv = value; }
        }

        public void LoadPersonAccount()
        {
            if (_ibusPersonAccount == null)
                _ibusPersonAccount = new busPersonAccount();
            _ibusPersonAccount.FindPersonAccount(icdoPersonAccountDependent.person_account_id);
        }

        public void LoadPersonAccountGhdv()
        {
            if (_ibusPersonAccountGhdv == null)
                _ibusPersonAccountGhdv = new  busPersonAccountGhdv();
            _ibusPersonAccountGhdv.FindGHDVByPersonAccountID(icdoPersonAccountDependent.person_account_id);
        }      

        public bool FindPersonAccountDependent(int Aintpersonaccountdependentid)
        {
            bool lblnResult = false;
            if (_icdoPersonAccountDependent == null)
            {
                _icdoPersonAccountDependent = new cdoPersonAccountDependent();
            }
            if (_icdoPersonAccountDependent.SelectRow(new object[1] { Aintpersonaccountdependentid }))
            {
                lblnResult = true;
            }
            return lblnResult;
        }

        public bool FindPersonAccountDependent(int AintPersonDependentID, int AintPersonAccountID)
        {
            bool lblnResult = false;
            DataTable ldtbList = Select<cdoPersonAccountDependent>(new string[2] { "person_dependent_id", "person_account_id" },
                                                        new object[2] { AintPersonDependentID, AintPersonAccountID }, null, null);
            if (ldtbList.Rows.Count > 0)
            {
                _icdoPersonAccountDependent.LoadData(ldtbList.Rows[0]);
                lblnResult = true;
            }
            return lblnResult;
        }

        public string istrAllPlanNames { get; set; } // PIR 20584
        /// <summary>
        /// PIR 16731 - If Health dependent with relationship ‘Spouse’ is ended and spouse is also a donor in the combined RHIC – initiate WFL.  
        /// WFL should be the same activities and roles as ‘Maintain RHIC Combining’.
        /// </summary>
        public void InitiateWorkFlowMaintainRHICUncombined()
        {
            DataTable ldtbPersonDependent = Select<cdoPersonDependent>(
            new string[2] { enmPersonDependent.person_dependent_id.ToString(), enmPersonDependent.relationship_value.ToString() },
            new object[2] { this.icdoPersonAccountDependent.person_dependent_id, busConstant.DependentRelationshipSpouse }, null, null);

            if (ldtbPersonDependent.Rows.Count > 0 && ldtbPersonDependent.Rows[0]["Dependent_PersLink_id"] != DBNull.Value)
            {
                int lintDepPerslinkId = Convert.ToInt32(ldtbPersonDependent.Rows[0]["Dependent_PersLink_id"]);
                DataTable ldtbList = Select("cdoPersonAccountDependent.IsSpouseCombinedRHICDonor",
                                                                  new object[2] { lintDepPerslinkId, icdoPersonAccountDependent.end_date });
                if (ldtbList.Rows.Count > 0)
                {
                    DataTable ldtActivityInstance = Select("entSolBpmActivityInstance.LoadRunningInstancesByPersonAndProcess",
                                                                  new object[2] { lintDepPerslinkId, busConstant.Map_Maintain_Rhic_Uncombined });
                    if ((ldtActivityInstance.Rows.Count == 0))
                    {
                        busWorkflowHelper.InitiateBpmRequest(busConstant.Map_Maintain_Rhic_Uncombined, lintDepPerslinkId, 0, 0, iobjPassInfo);

                    }
                }
            }
        }

        // PIR Death Automation - 7015 Validation need to Skip From Death Notification if future end dated records going to update
        public bool iblnIsNeedToValidate = true;
    }
}
