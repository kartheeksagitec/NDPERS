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
	public class busPersonAccountRetirementDbDcTransferEstimateGen : busExtendBase
    {
		public busPersonAccountRetirementDbDcTransferEstimateGen()
		{

		} 

		private cdoPersonAccountRetirementDbDcTransferEstimate _icdoPersonAccountRetirementDbDcTransferEstimate;
		public cdoPersonAccountRetirementDbDcTransferEstimate icdoPersonAccountRetirementDbDcTransferEstimate
		{
			get
			{
				return _icdoPersonAccountRetirementDbDcTransferEstimate;
			}

			set
			{
				_icdoPersonAccountRetirementDbDcTransferEstimate = value;
			}
		}

		public bool FindPersonAccountRetirementDbDcTransferEstimate(int Aintdbdctransferestimateid)
		{
			bool lblnResult = false;
			if (_icdoPersonAccountRetirementDbDcTransferEstimate == null)
			{
				_icdoPersonAccountRetirementDbDcTransferEstimate = new cdoPersonAccountRetirementDbDcTransferEstimate();
			}
			if (_icdoPersonAccountRetirementDbDcTransferEstimate.SelectRow(new object[1] { Aintdbdctransferestimateid }))
			{
				lblnResult = true;
			}            
			return lblnResult;
		}
	}
}
