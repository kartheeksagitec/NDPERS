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
	public partial class busJournalDetail : busExtendBase
    {
		public busJournalDetail()
		{

		} 

		private cdoJournalDetail _icdoJournalDetail;
		public cdoJournalDetail icdoJournalDetail
		{
			get
			{
				return _icdoJournalDetail;
			}

			set
			{
				_icdoJournalDetail = value;
			}
		}

		public bool FindJournalDetail(int Aintjournaldetailid)
		{
			bool lblnResult = false;
			if (_icdoJournalDetail == null)
			{
				_icdoJournalDetail = new cdoJournalDetail();
			}
			if (_icdoJournalDetail.SelectRow(new object[1] { Aintjournaldetailid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
