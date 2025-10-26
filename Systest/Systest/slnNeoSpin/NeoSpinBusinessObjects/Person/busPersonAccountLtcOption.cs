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

using System.Linq;
#endregion

namespace NeoSpin.BusinessObjects
{
	[Serializable]
	public class busPersonAccountLtcOption : busPersonAccountLtcOptionGen
	{
        public void GetMonthlyPremiumForLTC()
        {
        }

        public busPersonAccountLtc ibusPersonAccountLTC { get; set; }
        public bool iblnOverlapHistoryFound { get; set; }
        public void LoadPersonAccountLTC()
        {
            if (ibusPersonAccountLTC.IsNull())
                ibusPersonAccountLTC = new busPersonAccountLtc { icdoPersonAccount = new cdoPersonAccount() };
            ibusPersonAccountLTC.FindPersonAccount(icdoPersonAccountLtcOption.person_account_id);
        }

        // UAT PIR ID 1042
        public bool IsMemberGridStartDateEditable()
        {
            if (ibusPersonAccountLTC.IsNull())
                LoadPersonAccountLTC();
            if (ibusPersonAccountLTC.iclbLtcOptionMember.IsNull())
                ibusPersonAccountLTC.LoadLtcOptionUpdateMember();

            // If any Member option is enrolled, Start Date is not editable.
            return ibusPersonAccountLTC.iclbLtcOptionMember.Where(lobj => lobj.icdoPersonAccountLtcOption.effective_start_date != DateTime.MinValue).Any();
        }

        // UAT PIR ID 1042
        public bool IsSpouseGridStartDateEditable()
        {
            if (ibusPersonAccountLTC.IsNull())
                LoadPersonAccountLTC();
            if (ibusPersonAccountLTC.iclbLtcOptionSpouse.IsNull())
                ibusPersonAccountLTC.LoadLtcOptionUpdateSpouse();

            // If any Member option is enrolled, Start Date is not editable.
            return ibusPersonAccountLTC.iclbLtcOptionSpouse.Where(lobj => lobj.icdoPersonAccountLtcOption.effective_start_date != DateTime.MinValue).Any();
        }
    }
}
