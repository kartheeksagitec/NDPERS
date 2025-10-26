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
	/// Class NeoSpin.DataObjects.doWssBenAppRolloverDetail:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doWssBenAppRolloverDetail : doBase
    {
         
         public doWssBenAppRolloverDetail() : base()
         {
         }
         public int wss_ben_app_rollover_detail_id { get; set; }
         public int wss_ben_app_id { get; set; }
         public string rollover_institution_name { get; set; }
         public string addr_line_1 { get; set; }
         public string addr_line_2 { get; set; }
         public string addr_city { get; set; }
         public int addr_state_id { get; set; }
         public string addr_state_description { get; set; }
         public string addr_state_value { get; set; }
         public int addr_country_id { get; set; }
         public string addr_country_description { get; set; }
         public string addr_country_value { get; set; }
         public string addr_zip_code { get; set; }
         public string addr_zip_4_code { get; set; }
         public int rollover_option_id { get; set; }
         public string rollover_option_description { get; set; }
         public string rollover_option_value { get; set; }
         public decimal percent_of_taxable { get; set; }
         public decimal amount_of_taxable { get; set; }
         public string rollover_account_number { get; set; }
         public string rollover_flag { get; set; }
         public int rollover_type_id { get; set; }
         public string rollover_type_description { get; set; }
         public string rollover_type_value { get; set; }
         public int state_tax_option_id { get; set; }
         public string state_tax_option_description { get; set; }
         public string state_tax_option_value { get; set; }
         public string foreign_province { get; set; }
         public string foreign_postal_code { get; set; }
    }
    [Serializable]
    public enum enmWssBenAppRolloverDetail
    {
         wss_ben_app_rollover_detail_id ,
         wss_ben_app_id ,
         rollover_institution_name ,
         addr_line_1 ,
         addr_line_2 ,
         addr_city ,
         addr_state_id ,
         addr_state_description ,
         addr_state_value ,
         addr_country_id ,
         addr_country_description ,
         addr_country_value ,
         addr_zip_code ,
         addr_zip_4_code ,
         rollover_option_id ,
         rollover_option_description ,
         rollover_option_value ,
         percent_of_taxable ,
         amount_of_taxable ,
         rollover_account_number ,
         rollover_flag ,
         rollover_type_id ,
         rollover_type_description ,
         rollover_type_value ,
         state_tax_option_id ,
         state_tax_option_description ,
         state_tax_option_value ,
         foreign_province,
         foreign_postal_code,
    }
}

