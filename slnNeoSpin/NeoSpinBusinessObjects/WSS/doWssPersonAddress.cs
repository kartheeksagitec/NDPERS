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
	/// Class NeoSpin.DataObjects.doWssPersonAddress:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doWssPersonAddress : doBase
    {
         
         public doWssPersonAddress() : base()
         {
         }
         public int wss_person_address_id { get; set; }
         public int member_record_request_id { get; set; }
         public int person_address_id { get; set; }
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
         public int address_type_id { get; set; }
         public string address_type_description { get; set; }
         public string address_type_value { get; set; }
         public string foreign_province { get; set; }
         public string foreign_postal_code { get; set; }
         public string address_validate_flag { get; set; }
         public string address_validate_error { get; set; }
         public DateTime addr_effective_date { get; set; }
         public string address_updated_in_ps_batch { get; set; }
    }
    [Serializable]
    public enum enmWssPersonAddress
    {
         wss_person_address_id ,
         member_record_request_id ,
         person_address_id ,
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
         address_type_id ,
         address_type_description ,
         address_type_value ,
         foreign_province ,
         foreign_postal_code ,
         address_validate_flag ,
         address_validate_error ,
         addr_effective_date ,
         address_updated_in_ps_batch ,
    }
}

