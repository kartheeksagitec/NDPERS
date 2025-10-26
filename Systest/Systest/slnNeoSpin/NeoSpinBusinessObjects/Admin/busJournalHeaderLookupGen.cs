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
	[Serializable]
	public partial class busJournalHeaderLookup : busMainBase
	{
        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            if (aobjBus is busJournalHeader)
            {
                busJournalHeader lobjJournalHeader = (busJournalHeader)aobjBus;
                lobjJournalHeader.LoadTotalDebitandCredit();
            }
        }
    }
}
