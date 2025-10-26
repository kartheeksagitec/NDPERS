#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
using System.Data;
using System.Data.Common;


#endregion

namespace NeoSpin.BusinessObjects
{
	[Serializable]
	public class busRequirementHistory : busExtendBase
    {
        public busRequirementHistory()
		{
		}       
        private cdoRequirementHistory _icdoRequirementHistory;
        public cdoRequirementHistory icdoRequirementHistory
		{
			get
			{
                return _icdoRequirementHistory;
			}

			set
			{
                _icdoRequirementHistory = value;
			}
		}

        public bool FindRequirementHistory(int aintRequirementHistoryId)
		{
			bool lblnResult = false;
            if (_icdoRequirementHistory == null)
			{
                _icdoRequirementHistory = new cdoRequirementHistory();
			}
            if (_icdoRequirementHistory.SelectRow(new object[1] { aintRequirementHistoryId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        public void LoadRequirementKey()
        {
            _icdoRequirementHistory.Parent_Requirement_Key = Convert.ToString(DBFunction.DBExecuteScalar("cdoRequirementHistory.LoadRequirementKey", new object[1] { _icdoRequirementHistory.parent_requirement_id } , iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
        }
	}
}
