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
	public partial class busChartOfAccountLookup 
	{
        private Collection<busChartOfAccount> _iclbChartOfAccount;
        public Collection<busChartOfAccount> iclbChartOfAccount
        {
            get { return _iclbChartOfAccount; }
            set { _iclbChartOfAccount = value; }
        }

        public void LoadChartOfAccount(DataTable adtbSearchResult)
        {
            _iclbChartOfAccount = GetCollection<busChartOfAccount>(adtbSearchResult, "icdoChartOfAccount");
        }

	}
}
