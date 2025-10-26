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
    public class busPersonAccountGhdvHistoryGen : busExtendBase
    {
		public busPersonAccountGhdvHistoryGen()
		{

		}

		private cdoPersonAccountGhdvHistory _icdoPersonAccountGhdvHistory;
		public cdoPersonAccountGhdvHistory icdoPersonAccountGhdvHistory
		{
			get
			{
				return _icdoPersonAccountGhdvHistory;
			}
			set
			{
				_icdoPersonAccountGhdvHistory = value;
			}
		}

		public bool FindPersonAccountGhdvHistory(int Aintpersonaccountghdvhistoryid)
		{
			bool lblnResult = false;
			if (_icdoPersonAccountGhdvHistory == null)
			{
				_icdoPersonAccountGhdvHistory = new cdoPersonAccountGhdvHistory();
			}
			if (_icdoPersonAccountGhdvHistory.SelectRow(new object[1] { Aintpersonaccountghdvhistoryid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
        private busOrganization _ibusEPOProvider;

        public busOrganization ibusEPOProvider
        {
            get { return _ibusEPOProvider; }
            set { _ibusEPOProvider = value; }
        }
        public void LoadEPOProvider()
        {
            if (_ibusEPOProvider == null)
            {
                _ibusEPOProvider = new busOrganization();
            }
            _ibusEPOProvider.FindOrganization(icdoPersonAccountGhdvHistory.epo_org_id);
        }
	}
}
