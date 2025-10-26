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
	public class busPersonAccountMissedDeposit : busPersonAccountMissedDepositGen
	{
        public busPersonAccountRetirement ibusPersonAccountRetirement { get; set; }
        public void LoadPersonAccount()
        {
            if (ibusPersonAccountRetirement == null)
                ibusPersonAccountRetirement = new busPersonAccountRetirement();
            ibusPersonAccountRetirement.FindPersonAccountRetirement(icdoPersonAccountMissedDeposit.person_account_id);
        }
	}
}
