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
	/// Class NeoSpin.DataObjects.doPayeeAccountRolloverDetail:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPayeeAccountRolloverDetail : doBase
    {
         
         public doPayeeAccountRolloverDetail() : base()
         {
         }
         public int payee_account_rollover_detail_id { get; set; }
         public int payee_account_id { get; set; }
         public int rollover_org_id { get; set; }
         public int rollover_option_id { get; set; }
         public string rollover_option_description { get; set; }
         public string rollover_option_value { get; set; }
         public decimal percent_of_taxable { get; set; }
         public decimal amount_of_taxable { get; set; }
         public string account_number { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public string rollover_flag { get; set; }
         public int old_rollover_dtl_id { get; set; }
         public int rollover_type_id { get; set; }
         public string rollover_type_description { get; set; }
         public string rollover_type_value { get; set; }
         public int state_tax_option_id { get; set; }
         public string state_tax_option_description { get; set; }
         public string state_tax_option_value { get; set; }
         public string fbo { get; set; }
         public string addr_line_1 { get; set; }
         public string addr_line_2 { get; set; }
         public string city { get; set; }
         public string state_value { get; set; }
         public string zip_code { get; set; }
         public string zip_4_code { get; set; }
         public int state_id { get; set; }
         public string state_description { get; set; }
    }
    [Serializable]
    public enum enmPayeeAccountRolloverDetail
    {
         payee_account_rollover_detail_id ,
         payee_account_id ,
         rollover_org_id ,
         rollover_option_id ,
         rollover_option_description ,
         rollover_option_value ,
         percent_of_taxable ,
         amount_of_taxable ,
         account_number ,
         status_id ,
         status_description ,
         status_value ,
         rollover_flag ,
         old_rollover_dtl_id ,
         rollover_type_id ,
         rollover_type_description ,
         rollover_type_value ,
         state_tax_option_id ,
         state_tax_option_description ,
         state_tax_option_value ,
         fbo ,
         addr_line_1 ,
         addr_line_2 ,
         city ,
         state_value ,
         zip_code ,
         zip_4_code ,
         state_id ,
         state_description ,
    }
}

