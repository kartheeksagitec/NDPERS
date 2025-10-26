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
	/// <summary>
	/// Class NeoSpin.DataObjects.doOrgPlanHealthMedicarePartDRate:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doOrgPlanHealthMedicarePartDRate : doBase
    {
        
         public doOrgPlanHealthMedicarePartDRate() : base()
         {
         }
         public int org_plan_health_medicare_part_d_rate_id { get; set; }
         public int org_plan_id { get; set; }
         public DateTime effective_date { get; set; }
         public DateTime premium_period_start_date { get; set; }
         public DateTime premium_period_end_date { get; set; }
         public int org_plan_group_health_medicare_part_d_coverage_ref_id { get; set; }
         public decimal provider_premium_amt { get; set; }
         public decimal fee_amt { get; set; }
         public decimal medicare_part_d_amt { get; set; }
         public Decimal health_savings_amount { get; set; }
         public Decimal health_savings_vendor_amount { get; set; }
         public decimal buydown_amount { get; set; }
    }
    [Serializable]
    public enum enmOrgPlanHealthMedicarePartDRate
    {
         org_plan_health_medicare_part_d_rate_id ,
         org_plan_id ,
         effective_date ,
         premium_period_start_date ,
         premium_period_end_date ,
         org_plan_group_health_medicare_part_d_coverage_ref_id ,
         provider_premium_amt ,
         fee_amt ,
         medicare_part_d_amt ,
         health_savings_amount ,
         health_savings_vendor_amount ,
         buydown_amount ,
    }
}

