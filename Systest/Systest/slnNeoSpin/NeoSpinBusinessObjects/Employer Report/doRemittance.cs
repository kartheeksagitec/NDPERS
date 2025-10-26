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
	/// Class NeoSpin.DataObjects.doRemittance:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doRemittance : doBase
    {
         
         public doRemittance() : base()
         {
         }
         public int remittance_id { get; set; }
         public int deposit_id { get; set; }
         public int person_id { get; set; }
         public int org_id { get; set; }
         public int remittance_type_id { get; set; }
         public string remittance_type_description { get; set; }
         public string remittance_type_value { get; set; }
         public decimal remittance_amount { get; set; }
         public int plan_id { get; set; }
         public DateTime applied_date { get; set; }
         public decimal computed_refund_amount { get; set; }
         public decimal overridden_refund_amount { get; set; }
         public int refund_status_id { get; set; }
         public string refund_status_description { get; set; }
         public string refund_status_value { get; set; }
         public string refund_pend_by { get; set; }
         public string refund_appr_by { get; set; }
         public int payment_history_header_id { get; set; }
         public decimal allocated_negative_deposit_amount { get; set; }
         public int refund_to_id { get; set; }
         public string refund_to_description { get; set; }
         public string refund_to_value { get; set; }
         public int refund_to_person_id { get; set; }
         public int refund_to_org_id { get; set; }
         public string notes { get; set; }
         public string refund_notes { get; set; }
    }
    [Serializable]
    public enum enmRemittance
    {
         remittance_id ,
         deposit_id ,
         person_id ,
         org_id ,
         remittance_type_id ,
         remittance_type_description ,
         remittance_type_value ,
         remittance_amount ,
         plan_id ,
         applied_date ,
         computed_refund_amount ,
         overridden_refund_amount ,
         refund_status_id ,
         refund_status_description ,
         refund_status_value ,
         refund_pend_by ,
         refund_appr_by ,
         payment_history_header_id ,
         allocated_negative_deposit_amount ,
         refund_to_id ,
         refund_to_description ,
         refund_to_value ,
         refund_to_person_id ,
         refund_to_org_id ,
         notes ,
         refund_notes ,
    }
}

