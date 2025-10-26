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
    public class doCaseDisabilityIncomeVerificationDetail : doBase
    {
         
         public doCaseDisabilityIncomeVerificationDetail() : base()
         {
         }
		public int case_disability_income_verification_detail_id { get; set; }
		
		public int case_id { get; set; }
		
		public int year { get; set; }
		
		public int month { get; set; }
		
		public decimal earnings_amount1 { get; set; }
		
		public decimal earnings_amount2 { get; set; }
		
		public decimal earnings_amount3 { get; set; }
		
		public string is_exceeds_comparable_earnings_flag { get; set; }
		
    }
}

