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
	public partial class busJournalHeader : busExtendBase
    {
		public busJournalHeader()
		{

		} 

		private cdoJournalHeader _icdoJournalHeader;
		public cdoJournalHeader icdoJournalHeader
		{
			get
			{
				return _icdoJournalHeader;
			}

			set
			{
				_icdoJournalHeader = value;
			}
		}

		private Collection<busJournalDetail> _icolJournalDetail;
		public Collection<busJournalDetail> icolJournalDetail
		{
			get
			{
				return _icolJournalDetail;
			}

			set
			{
				_icolJournalDetail = value;
			}
		}

		public bool FindJournalHeader(int Aintjournalheaderid)
		{
			bool lblnResult = false;
			if (_icdoJournalHeader == null)
			{
				_icdoJournalHeader = new cdoJournalHeader();
			}
			if (_icdoJournalHeader.SelectRow(new object[1] { Aintjournalheaderid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

		public void LoadJournalDetails()
		{
			DataTable ldtbList = Select<cdoJournalDetail>(
				new string[1] { "journal_header_id" },
				new object[1] { icdoJournalHeader.journal_header_id }, null, null);
			_icolJournalDetail = GetCollection<busJournalDetail>(ldtbList, "icdoJournalDetail");
		}
	}
}
