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
	/// Class NeoSpin.DataObjects.doServicePurchaseDetailUserra:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doServicePurchaseDetailUserra : doBase
    {
         
         public doServicePurchaseDetailUserra() : base()
         {
         }
         public int service_purchase_userra_detail_id { get; set; }
         public int service_purchase_detail_id { get; set; }
         public DateTime missed_salary_month { get; set; }
         public decimal missed_salary_amount { get; set; }
         public decimal employee_contribution { get; set; }
         public decimal employee_pickup { get; set; }
         public decimal employer_contribution { get; set; }
         public decimal rhic_contribution { get; set; }
         public string posted_flag { get; set; }
         public int service_purchase_payment_allocation_id { get; set; }
    }
    [Serializable]
    public enum enmServicePurchaseDetailUserra
    {
         service_purchase_userra_detail_id ,
         service_purchase_detail_id ,
         missed_salary_month ,
         missed_salary_amount ,
         employee_contribution ,
         employee_pickup ,
         employer_contribution ,
         rhic_contribution ,
         posted_flag ,
         service_purchase_payment_allocation_id ,
    }
}

