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
	public class busPersonAccountRetirementDbDbTransferContributionGen : busExtendBase
    {
        public busPersonAccountRetirementDbDbTransferContributionGen()
		{

		}
        private cdoPersonAccountRetirementDbDbTransferContribution _icdoRetirementDbDbTransferContribution;
        public cdoPersonAccountRetirementDbDbTransferContribution icdoRetirementDbDbTransferContribution
		{
			get
			{
                return _icdoRetirementDbDbTransferContribution;
			}
			set
			{
                _icdoRetirementDbDbTransferContribution = value;
			}
		}

        public bool FindRetirementDbDbTransferContribution(int aintRetirmentDbDbTransferContribution)
		{
			bool lblnResult = false;
			if (_icdoRetirementDbDbTransferContribution == null)
			{
                _icdoRetirementDbDbTransferContribution = new cdoPersonAccountRetirementDbDbTransferContribution();
			}
            if (_icdoRetirementDbDbTransferContribution.SelectRow(new object[1] { aintRetirmentDbDbTransferContribution }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
