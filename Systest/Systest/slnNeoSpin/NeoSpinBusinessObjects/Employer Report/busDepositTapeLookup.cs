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
	public partial class busDepositTapeLookup
	{        
        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            if (aobjBus is busDepositTape)
            {
                busDepositTape lobjDepositTape = (busDepositTape)aobjBus;
                lobjDepositTape.LoadDeposits();
                lobjDepositTape.LoadDepositsCountAndTotalAmount();
                lobjDepositTape.icdoDepositTape.DepositsCount = lobjDepositTape.icdoDepositTape.DepositsCount;
            }
        }
	}
}
