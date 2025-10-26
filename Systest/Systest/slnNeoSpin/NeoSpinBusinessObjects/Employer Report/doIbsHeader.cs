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
	/// Class NeoSpin.DataObjects.doIbsHeader:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doIbsHeader : doBase
    {
         public doIbsHeader() : base()
         {
         }
         public int ibs_header_id { get; set; }
         public int report_type_id { get; set; }
         public string report_type_description { get; set; }
         public string report_type_value { get; set; }
         public DateTime billing_month_and_year { get; set; }
         public decimal total_premium_amount { get; set; }
         public int total_record_count { get; set; }
         public int report_status_id { get; set; }
         public string report_status_description { get; set; }
         public string report_status_value { get; set; }
         public string comment { get; set; }
         public int js_rhic_bill_id { get; set; }
         public DateTime run_date { get; set; }
    }
    [Serializable]
    public enum enmIbsHeader
    {
         ibs_header_id ,
         report_type_id ,
         report_type_description ,
         report_type_value ,
         billing_month_and_year ,
         total_premium_amount ,
         total_record_count ,
         report_status_id ,
         report_status_description ,
         report_status_value ,
         comment ,
         js_rhic_bill_id ,
         run_date ,
    }
}

