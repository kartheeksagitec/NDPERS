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
	public class busPersonAccountFlexCompOption : busPersonAccountFlexCompOptionGen
	{
        public string istrStartDateFSA
        {
            get
            {
                if (icdoPersonAccountFlexCompOption.effective_start_date == DateTime.MinValue)
                {
                    if (ibusPersonAccountFlexComp.IsNotNull() && icdoPersonAccountFlexCompOption.effective_end_date != DateTime.MinValue)
                    {
                        if (ibusPersonAccountFlexComp.ibusPersonAccount.IsNull()) ibusPersonAccountFlexComp.LoadPersonAccount();
                        return ibusPersonAccountFlexComp.ibusPersonAccount.icdoPersonAccount.history_change_date.ToString(busConstant.DateFormatYearMonthDay);
                    }
                    return string.Empty;
                }
                else
                    return icdoPersonAccountFlexCompOption.effective_start_date.ToString(busConstant.DateFormatYearMonthDay);
            }
        }

        public string istrEndDateFSA
        {
            get
            {
                if (icdoPersonAccountFlexCompOption.effective_end_date == DateTime.MinValue)
                    return string.Empty;
                else
                    return icdoPersonAccountFlexCompOption.effective_end_date.ToString(busConstant.DateFormatYearMonthDay);
            }
        }

        public string istrAnnualPledgeAmount
        {
            get
            {
                if (icdoPersonAccountFlexCompOption.annual_pledge_amount > 0M)
                    return icdoPersonAccountFlexCompOption.annual_pledge_amount.ToString();
                else
                    return string.Empty;
            }
        }
	}
}
