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
	public partial class busDepositTapeLookup  : busMainBase
	{

        private Collection<busDepositTape> _icolDepositTape;
        public Collection<busDepositTape> icolDepositTape
        {
            get { return _icolDepositTape; }
            set { _icolDepositTape = value; }
        }

        public void LoadDepositTape(DataTable adtbSearchResult)
        {
            _icolDepositTape = GetCollection<busDepositTape>(adtbSearchResult, "icdoDepositTape");
        }
	}
}
