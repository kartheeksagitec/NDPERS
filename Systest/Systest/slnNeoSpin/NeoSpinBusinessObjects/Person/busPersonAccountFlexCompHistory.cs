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
	public class busPersonAccountFlexCompHistory : busPersonAccountFlexCompHistoryGen
	{
        public decimal idecMsraAnnualPledgeAmount { get; set; }
        public decimal idecDcraAnnualPledgeAmount { get; set; }
        public bool iblnMsraAdded { get; set; }
        public bool iblnDcraAdded { get; set; }
	}
}
