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
using Sagitec.CustomDataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
	[Serializable]
	public partial class busPersonTffrDetail : busNeoSpinBase
	{
        public busPerson ibusPerson { get; set; }
        public int iintFileRecordType { get; set; }
        public string istrPersonSSNFromFile { get; set; }
        public string istrIsServiceCredit { get; set; }
        public string istrTffrWage { get; set; }

    }
}
