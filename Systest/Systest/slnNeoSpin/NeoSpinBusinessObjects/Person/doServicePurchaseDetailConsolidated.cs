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
	/// Class NeoSpin.DataObjects.doServicePurchaseDetailConsolidated:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doServicePurchaseDetailConsolidated : doBase
    {
         
         public doServicePurchaseDetailConsolidated() : base()
         {
         }
         public int service_purchase_consolidated_detail_id { get; set; }
         public int service_purchase_detail_id { get; set; }
         public int service_credit_type_id { get; set; }
         public string service_credit_type_description { get; set; }
         public string service_credit_type_value { get; set; }
         public DateTime service_purchase_start_date { get; set; }
         public DateTime service_purchase_end_date { get; set; }
         public int time_to_purchase { get; set; }
         public int calculated_time_to_purchase { get; set; }
         public int time_to_purchase_contribution_months { get; set; }
         public decimal refund_with_interest { get; set; }
         public DateTime refund_date { get; set; }
         public string suppress_warnings_flag { get; set; }
         public int org_id { get; set; }
         public int payee_account_id { get; set; }
    }
    [Serializable]
    public enum enmServicePurchaseDetailConsolidated
    {
         service_purchase_consolidated_detail_id ,
         service_purchase_detail_id ,
         service_credit_type_id ,
         service_credit_type_description ,
         service_credit_type_value ,
         service_purchase_start_date ,
         service_purchase_end_date ,
         time_to_purchase ,
         calculated_time_to_purchase ,
         time_to_purchase_contribution_months ,
         refund_with_interest ,
         refund_date ,
         suppress_warnings_flag ,
         org_id ,
         payee_account_id ,
    }
}

