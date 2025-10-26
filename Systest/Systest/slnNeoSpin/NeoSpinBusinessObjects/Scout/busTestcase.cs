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
	public class busTestcase : busExtendBase
    {
		public busTestcase()
		{
		}

		private cdoTestcase _icdoTestcase;
		public cdoTestcase icdoTestcase
		{
			get
			{
				return _icdoTestcase;
			}

			set
			{
				_icdoTestcase = value;
			}
		}

		private Collection<busTestcaseDetail> _iclbTestcaseDetail;
		public Collection<busTestcaseDetail> iclbTestcaseDetail
		{
			get
			{
				return _iclbTestcaseDetail;
			}

			set
			{
				_iclbTestcaseDetail = value;
			}
		}

		public bool FindTestcase(int aintTestcaseId)
		{
			bool lblnResult = false;
			if (_icdoTestcase == null)
			{
				_icdoTestcase = new cdoTestcase();
			}
			if (_icdoTestcase.SelectRow(new object[1] { aintTestcaseId }))
			{
				lblnResult = true;
				LoadTestcaseDetail();
			}
			return lblnResult;
		}

		public void LoadTestcaseDetail()
		{
			DataTable ldtbList = Select<cdoTestcaseDetail>(
				new string[1] { "Testcase_id" },
				new object[1] { icdoTestcase.testcase_id}, null, null);
			_iclbTestcaseDetail = GetCollection<busTestcaseDetail>(ldtbList, "icdoTestcaseDetail");
		}

	}
}
