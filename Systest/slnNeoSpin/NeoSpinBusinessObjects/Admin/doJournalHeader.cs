#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sagitec.Common;
using Sagitec.DataObjects;
#endregion

namespace NeoSpin.DataObjects
{
    [Serializable]
    public class doJournalHeader : doBase
    {
         public doJournalHeader() : base()
         {
         }
		private int _journal_header_id;
		public int journal_header_id
		{
			get
			{
				return _journal_header_id;
			}

			set
			{
				_journal_header_id = value;
			}
		}

		private int _journal_status_id;
		public int journal_status_id
		{
			get
			{
				return _journal_status_id;
			}

			set
			{
				_journal_status_id = value;
			}
		}

		private string _journal_status_description;
		public string journal_status_description
		{
			get
			{
				return _journal_status_description;
			}

			set
			{
				_journal_status_description = value;
			}
		}

		private string _journal_status_value;
		public string journal_status_value
		{
			get
			{
				return _journal_status_value;
			}

			set
			{
				_journal_status_value = value;
			}
		}

		private DateTime _posting_date;
		public DateTime posting_date
		{
			get
			{
				return _posting_date;
			}

			set
			{
				_posting_date = value;
			}
		}

    }
}

