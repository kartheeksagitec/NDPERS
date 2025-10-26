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
    public class doBenefitMultiplier : doBase
    {
         
         public doBenefitMultiplier() : base()
         {
         }
		public int benefit_multiplier_id { get; set; }
		
		public int benefit_calculation_id { get; set; }
		
		public decimal pension_service_credit { get; set; }
		
		public decimal benefit_multiplier_rate { get; set; }
		
		public decimal benefit_multiplier_amount { get; set; }
		
    }
}

