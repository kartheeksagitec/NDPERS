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
    public class doCaseFinancialHardshipProviderDetail : doBase
    {
         
         public doCaseFinancialHardshipProviderDetail() : base()
         {
         }
		public int case_financial_hardship_provider_detail_id { get; set; }
		
		public int case_id { get; set; }
		
		public int person_account_provider_id { get; set; }
		
		public decimal account_balance_amount { get; set; }
		
		public DateTime account_balance_date { get; set; }

        public string istrProviderName { get; set; }
    }
}

