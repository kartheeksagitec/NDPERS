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
using NeoSpin.BusinessObjects;

#endregion

namespace NeoSpin
{
    /// <summary>
    /// Class NeoSpin.busPersonAccountPaymentElectionHistory:
    /// Inherited from busPersonAccountPaymentElectionHistoryGen, the class is used to customize the business object busPersonAccountPaymentElectionHistoryGen.
    /// </summary>
    [Serializable]
    public class busPersonAccountPaymentElectionHistory : busPersonAccountPaymentElectionHistoryGen
    {
        public busPersonAccountPaymentElection ibusPersonAccountPaymentElection { get; set; }
        public busPersonAccountPaymentElectionHistory ibusPersonAccountPaymentElectionLatestHistory { get; set; }


        public void CreatePaymentElectionHistoryObject()
        {
            if (ibusPersonAccountPaymentElection.IsNotNull())
            {
                busGlobalFunctions.CopyPropertyValues(ibusPersonAccountPaymentElection.icdoPersonAccountPaymentElection, icdoPersonAccountPaymentElectionHistory);
                icdoPersonAccountPaymentElectionHistory.history_change_date = ibusPersonAccountPaymentElection.icdoPersonAccount.history_change_date;
            }
        }

        public bool IsPaymentElectionChanged()
        {
            if (icdoPersonAccountPaymentElectionHistory.ibs_flag == ibusPersonAccountPaymentElectionLatestHistory.icdoPersonAccountPaymentElectionHistory.ibs_flag
                && icdoPersonAccountPaymentElectionHistory.payment_method_value == ibusPersonAccountPaymentElectionLatestHistory.icdoPersonAccountPaymentElectionHistory.payment_method_value
                && icdoPersonAccountPaymentElectionHistory.ibs_effective_date == ibusPersonAccountPaymentElectionLatestHistory.icdoPersonAccountPaymentElectionHistory.ibs_effective_date
                && icdoPersonAccountPaymentElectionHistory.ibs_org_id == ibusPersonAccountPaymentElectionLatestHistory.icdoPersonAccountPaymentElectionHistory.ibs_org_id
                && icdoPersonAccountPaymentElectionHistory.ibs_supplemental_flag == ibusPersonAccountPaymentElectionLatestHistory.icdoPersonAccountPaymentElectionHistory.ibs_supplemental_flag
                && icdoPersonAccountPaymentElectionHistory.ibs_supplemental_org_id == ibusPersonAccountPaymentElectionLatestHistory.icdoPersonAccountPaymentElectionHistory.ibs_supplemental_org_id
                && icdoPersonAccountPaymentElectionHistory.payee_account_id == ibusPersonAccountPaymentElectionLatestHistory.icdoPersonAccountPaymentElectionHistory.payee_account_id
                && icdoPersonAccountPaymentElectionHistory.cobra_empr_share == ibusPersonAccountPaymentElectionLatestHistory.icdoPersonAccountPaymentElectionHistory.cobra_empr_share
                )
                return false;

            return true;
        }

        public void InsertPaymentElectionHistory()
        {
            ibusPersonAccountPaymentElection.LoadPaymentElectionHistory();

            if (ibusPersonAccountPaymentElection.iclbPersonAccountPaymentElectionHistory.Count > 0)
                ibusPersonAccountPaymentElectionLatestHistory = ibusPersonAccountPaymentElection.iclbPersonAccountPaymentElectionHistory[0];

            CreatePaymentElectionHistoryObject();

            if(ibusPersonAccountPaymentElectionLatestHistory.IsNotNull())
            {
                if (IsPaymentElectionChanged())
                    icdoPersonAccountPaymentElectionHistory.Insert();
            }
            else
            {
                icdoPersonAccountPaymentElectionHistory.Insert();
            }
            
        }
    }
}
