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
	public class busTestcaseDetail : busExtendBase
    {
		public busTestcaseDetail()
		{
		}

		private cdoTestcaseDetail _icdoTestcaseDetail;
		public cdoTestcaseDetail icdoTestcaseDetail
		{
			get
			{
				return _icdoTestcaseDetail;
			}

			set
			{
				_icdoTestcaseDetail = value;
			}
		}

		private busTestcase _ibusTestcase;
		public busTestcase ibusTestcase
		{
			get
			{
				return _ibusTestcase;
			}
			set
			{
				_ibusTestcase = value;
			}
		}

		public bool FindTestcaseDetail(int aintTestcaseDtlId)
		{
			bool lblnResult = false;
			if (_icdoTestcaseDetail == null)
			{
				_icdoTestcaseDetail = new cdoTestcaseDetail();
			}
			if (_icdoTestcaseDetail.SelectRow(new object[1] { aintTestcaseDtlId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

		public void LoadTestcase()
		{
			if (_ibusTestcase == null)
			{
				_ibusTestcase = new busTestcase();
			}
			_ibusTestcase.FindTestcase(_icdoTestcaseDetail.testcase_id);
		}

	}
}
