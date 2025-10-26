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
	public partial class busJsRhicBill : busExtendBase
    {
		public busJsRhicBill()
		{

		} 

		private cdoJsRhicBill _icdoJsRhicBill;
		public cdoJsRhicBill icdoJsRhicBill
		{
			get
			{
				return _icdoJsRhicBill;
			}

			set
			{
				_icdoJsRhicBill = value;
			}
		}
		private Collection<busJsRhicRemittanceAllocation> _icolJsRhicRemittanceAllocation;
		public Collection<busJsRhicRemittanceAllocation> icolJsRhicRemittanceAllocation
		{
			get
			{
				return _icolJsRhicRemittanceAllocation;
			}

			set
			{
				_icolJsRhicRemittanceAllocation = value;
			}
		}

		public bool FindJsRhicBill(int Aintjsrhicbillid)
		{
			bool lblnResult = false;
			if (_icdoJsRhicBill == null)
			{
				_icdoJsRhicBill = new cdoJsRhicBill();
			}
			if (_icdoJsRhicBill.SelectRow(new object[1] { Aintjsrhicbillid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
		public void LoadJsRhicRemittanceAllocations()
		{
			DataTable ldtbList = Select<cdoJsRhicRemittanceAllocation>(
				new string[1] { "js_rhic_bill_id" },
				new object[1] { icdoJsRhicBill.js_rhic_bill_id }, null, null);
			_icolJsRhicRemittanceAllocation = GetCollection<busJsRhicRemittanceAllocation>(ldtbList, "icdoJsRhicRemittanceAllocation");
		}
	}
}
