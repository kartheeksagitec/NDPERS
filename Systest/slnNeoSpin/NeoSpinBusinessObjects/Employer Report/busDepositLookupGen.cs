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
	[Serializable]
	public partial class busDepositLookup  : busMainBase
	{
        private Collection<busDeposit> _icolDeposit;
        public Collection<busDeposit> icolDeposit
        {
            get { return _icolDeposit; }
            set { _icolDeposit = value; }
        }

        public void LoadDeposit(DataTable adtbSearchResult)
        {
            _icolDeposit = GetCollection<busDeposit>(adtbSearchResult, "icdoDeposit");
        }

        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            if (aobjBus is busDeposit)
            {
                busDeposit lobjDeposit = new busDeposit();
                lobjDeposit.icdoDeposit = new cdoDeposit();
                lobjDeposit = (busDeposit)aobjBus;
                lobjDeposit.LoadOrgCodeID();
                lobjDeposit.LoadDepositTape();
                lobjDeposit.LoadBenfitTypeValue();
            }
        }
	}
}
