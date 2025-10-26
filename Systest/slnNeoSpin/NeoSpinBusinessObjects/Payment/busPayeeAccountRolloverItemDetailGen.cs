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
	public class busPayeeAccountRolloverItemDetailGen : busExtendBase
    {
		public busPayeeAccountRolloverItemDetailGen()
		{

		}

		private cdoPayeeAccountRolloverItemDetail _icdoPayeeAccountRolloverItemDetail;
		public cdoPayeeAccountRolloverItemDetail icdoPayeeAccountRolloverItemDetail
		{
			get
			{
				return _icdoPayeeAccountRolloverItemDetail;
			}
			set
			{
				_icdoPayeeAccountRolloverItemDetail = value;
			}
		}

		public bool FindPayeeAccountRolloverItemDetail(int Aintpayeeaccountrolloveritemdetailid)
		{
			bool lblnResult = false;
			if (_icdoPayeeAccountRolloverItemDetail == null)
			{
				_icdoPayeeAccountRolloverItemDetail = new cdoPayeeAccountRolloverItemDetail();
			}
			if (_icdoPayeeAccountRolloverItemDetail.SelectRow(new object[1] { Aintpayeeaccountrolloveritemdetailid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
