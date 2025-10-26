#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using NeoSpin.DataObjects;

#endregion

namespace NeoSpin.CustomDataObjects
{
    [Serializable]
	public class cdoJournalDetail : doJournalDetail
	{
		public cdoJournalDetail() : base()
		{
		}

        private string _istrAccountNo;
        public string istrAccountNo
        {
            get { return _istrAccountNo; }
            set { _istrAccountNo = value; }
        }

        private string _istrOrgCodeID;
        public string istrOrgCodeID
        {
            get { return _istrOrgCodeID; }
            set { _istrOrgCodeID = value; }
        }
    } 
} 
