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
	/// Class NeoSpin.DataObjects.doPaymentMonthlyBenefitSummary:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPaymentMonthlyBenefitSummary : doBase
    {
         
         public doPaymentMonthlyBenefitSummary() : base()
         {
         }
         public int payment_monthly_benefit_summary_id { get; set; }
         public int payment_schedule_id { get; set; }
         public DateTime payment_date { get; set; }
         public decimal gross_amount { get; set; }
         public decimal rhic_benefit { get; set; }
         public decimal federal_tax { get; set; }
         public decimal state_tax { get; set; }
         public decimal insurance { get; set; }
         public decimal other_deductions { get; set; }
         public decimal net_amount { get; set; }
    }
    [Serializable]
    public enum enmPaymentMonthlyBenefitSummary
    {
         payment_monthly_benefit_summary_id ,
         payment_schedule_id ,
         payment_date ,
         gross_amount ,
         rhic_benefit ,
         federal_tax ,
         state_tax ,
         insurance ,
         other_deductions ,
         net_amount ,
    }
}

