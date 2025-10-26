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
	public class busPersonAccountRetirementAdjustmentContributionGen : busExtendBase
    {
        public busPersonAccountRetirementAdjustmentContributionGen()
		{

		}
        private cdoPersonAccountRetirementAdjustmentContribution _icdoRetirementAdjustmentContribution;
        public cdoPersonAccountRetirementAdjustmentContribution icdoRetirementAdjustmentContribution
		{
			get
			{
                return _icdoRetirementAdjustmentContribution;
			}
			set
			{
                _icdoRetirementAdjustmentContribution = value;
			}
		}

        public bool FindRetirementAdjustmentContribution(int aintRetirementAdjustmentContribution)
		{
			bool lblnResult = false;
			if (_icdoRetirementAdjustmentContribution == null)
			{
                _icdoRetirementAdjustmentContribution = new cdoPersonAccountRetirementAdjustmentContribution();
			}
            if (_icdoRetirementAdjustmentContribution.SelectRow(new object[1] { aintRetirementAdjustmentContribution }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
