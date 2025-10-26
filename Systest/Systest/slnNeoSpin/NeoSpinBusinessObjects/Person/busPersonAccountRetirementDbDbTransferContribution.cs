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
    public class busPersonAccountRetirementDbDbTransferContribution : busPersonAccountRetirementDbDbTransferContributionGen
	{
        // Person account
        public busPersonAccountRetirementDbDbTransfer _ibusRetirementDbDbTransfer;
        public busPersonAccountRetirementDbDbTransfer ibusRetirementDbDbTransfer
        {
            get
            {
                return _ibusRetirementDbDbTransfer;
            }
            set
            {
                _ibusRetirementDbDbTransfer = value;
            }
        }
        public void LoadPersonAccountRetirementDbDbTransfer()
        {
            if (_ibusRetirementDbDbTransfer == null)
            {
                _ibusRetirementDbDbTransfer = new busPersonAccountRetirementDbDbTransfer();
            }
            if (icdoRetirementDbDbTransferContribution.db_db_transfer_id > 0)
            {
                _ibusRetirementDbDbTransfer.FindPersonAccountRetirementDbDbTransfer(icdoRetirementDbDbTransferContribution.db_db_transfer_id);
                _ibusRetirementDbDbTransfer.LoadFromPersonAccountRetirement();
            }
        }
        public busPersonAccountRetirementContribution _ibusRetirementContribution;
        public busPersonAccountRetirementContribution ibusRetirementContribution
        {
            get
            {
                return _ibusRetirementContribution;
            }
            set
            {
                _ibusRetirementContribution = value;
            }
        }
    }

}
