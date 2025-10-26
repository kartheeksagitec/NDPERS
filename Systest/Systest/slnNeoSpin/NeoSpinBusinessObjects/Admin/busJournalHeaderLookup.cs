#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using NeoSpin.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;

#endregion

namespace NeoSpin.BusinessObjects
{
	public partial class busJournalHeaderLookup 
	{
        private Collection<busJournalHeader> _iclbJournalHeader;
        public Collection<busJournalHeader> iclbJournalHeader
        {
            get { return _iclbJournalHeader; }
            set { _iclbJournalHeader = value; }
        }

        public void LoadJournalHeader(DataTable adtbSearchResult)
        {
            _iclbJournalHeader = GetCollection<busJournalHeader>(adtbSearchResult, "icdoJournalHeader");
        }
	}
}
