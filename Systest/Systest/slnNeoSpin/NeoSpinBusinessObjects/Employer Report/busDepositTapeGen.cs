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
	public partial class busDepositTape : busExtendBase
    {
		public busDepositTape()
		{

		} 

		private cdoDepositTape _icdoDepositTape;
		public cdoDepositTape icdoDepositTape
		{
			get
			{
				return _icdoDepositTape;
			}

			set
			{
				_icdoDepositTape = value;
			}
		}
        
		public bool FindDepositTape(int Aintdeposittapeid)
		{
			bool lblnResult = false;
			if (_icdoDepositTape == null)
			{
				_icdoDepositTape = new cdoDepositTape();
			}
			if (_icdoDepositTape.SelectRow(new object[1] { Aintdeposittapeid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}        
	}
}
