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
	/// Class NeoSpin.DataObjects.doPersonAddress:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPersonAddress : doBase
    {
         
         public doPersonAddress() : base()
         {
         }
         public int person_address_id { get; set; }
         public int person_id { get; set; }
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
         public DateTime start_date { get; set; }
         public DateTime end_date { get; set; }
         public int address_type_id { get; set; }
         public string address_type_description { get; set; }
         public string address_type_value { get; set; }
         public string foreign_province { get; set; }
         public string foreign_postal_code { get; set; }
         public string address_validate_flag { get; set; }
         public string address_validate_error { get; set; }
         public string peoplesoft_flag { get; set; }
         public string undeliverable_address { get; set; }
         public string addr_change_letter_status_flag { get; set; }
         public string care_of { get; set; }
    }
    [Serializable]
    public enum enmPersonAddress
    {
         person_address_id ,
         person_id ,
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
         start_date ,
         end_date ,
         address_type_id ,
         address_type_description ,
         address_type_value ,
         foreign_province ,
         foreign_postal_code ,
         address_validate_flag ,
         address_validate_error ,
         peoplesoft_flag ,
         undeliverable_address ,
         addr_change_letter_status_flag ,
         care_of ,
    }
}
