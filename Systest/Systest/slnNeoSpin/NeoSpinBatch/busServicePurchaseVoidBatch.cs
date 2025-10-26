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
#endregion

namespace NeoSpinBatch
{
    class busServicePurchaseVoidBatch : busNeoSpinBatch
    {
        public busServicePurchaseVoidBatch()
        {}

        private Collection<busServicePurchaseHeader> _iclbServicePurchaseHeaderWithStatusPending;
        public Collection<busServicePurchaseHeader> iclbServicePurchaseHeaderWithStatusPending
        {
            get
            {
                return _iclbServicePurchaseHeaderWithStatusPending;
            }
            set
            {
                _iclbServicePurchaseHeaderWithStatusPending = value;
            }
        }

        public void UpdateHeaderWithExpirationDateExpiredWithStatusVoid()
        {
            DateTime ldtBatchRunDate = busGlobalFunctions.GetSysManagementBatchDate();
            ldtBatchRunDate = ldtBatchRunDate.AddDays(-60);          
            DataTable ldtbList = busNeoSpinBase.Select<cdoServicePurchaseHeader>(
                                new string[1] { "action_status_value" },
                                new object[1] { busConstant.Service_Purchase_Action_Status_Pending }, null, null);
            busBase lobjbase = new busBase();
            _iclbServicePurchaseHeaderWithStatusPending = lobjbase.GetCollection<busServicePurchaseHeader>(ldtbList, "icdoServicePurchaseHeader");
            foreach (busServicePurchaseHeader lobjServicePurchaseHeader in _iclbServicePurchaseHeaderWithStatusPending)
            {
               if (lobjServicePurchaseHeader.icdoServicePurchaseHeader.expiration_date < ldtBatchRunDate)              
                {
                    lobjServicePurchaseHeader.icdoServicePurchaseHeader.action_status_value = busConstant.Service_Purchase_Action_Status_Void;
                    lobjServicePurchaseHeader.icdoServicePurchaseHeader.Update();
                }
            }
        }
    }
}
