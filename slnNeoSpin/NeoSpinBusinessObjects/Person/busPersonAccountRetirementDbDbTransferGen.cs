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
	public class busPersonAccountRetirementDbDbTransferGen : busExtendBase
    {
		public busPersonAccountRetirementDbDbTransferGen()
		{

		} 

		private cdoPersonAccountRetirementDbDbTransfer _icdoPersonAccountRetirementDbDbTransfer;
		public cdoPersonAccountRetirementDbDbTransfer icdoPersonAccountRetirementDbDbTransfer
		{
			get
			{
				return _icdoPersonAccountRetirementDbDbTransfer;
			}

			set
			{
				_icdoPersonAccountRetirementDbDbTransfer = value;
			}
		}
   

		public bool FindPersonAccountRetirementDbDbTransfer(int Aintdbdbtransferid)
		{
			bool lblnResult = false;
			if (_icdoPersonAccountRetirementDbDbTransfer.IsNull())
			{
				_icdoPersonAccountRetirementDbDbTransfer = new cdoPersonAccountRetirementDbDbTransfer();
			}
			if (_icdoPersonAccountRetirementDbDbTransfer.SelectRow(new object[1] { Aintdbdbtransferid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        //to load source contribution
        public Collection<busPersonAccountRetirementContribution> iclbRetirementSourceContribution { get; set; }
	}
}
