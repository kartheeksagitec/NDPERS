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
	/// Class NeoSpin.DataObjects.doPersonContact:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPersonContact : doBase
    {
         
         public doPersonContact() : base()
         {
         }
         public int person_contact_id { get; set; }
         public int person_id { get; set; }
         public int contact_person_id { get; set; }
         public int contact_org_id { get; set; }
         public int relationship_id { get; set; }
         public string relationship_description { get; set; }
         public string relationship_value { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public string contact_name { get; set; }
         public string address_line_1 { get; set; }
         public string address_line_2 { get; set; }
         public string address_city { get; set; }
         public int address_state_id { get; set; }
         public string address_state_description { get; set; }
         public string address_state_value { get; set; }
         public int address_country_id { get; set; }
         public string address_country_description { get; set; }
         public string address_country_value { get; set; }
         public string foreign_province { get; set; }
         public string foreign_postal_code { get; set; }
         public string address_zip_code { get; set; }
         public string address_zip_4_code { get; set; }
         public string contact_phone_no { get; set; }
         public string email_address { get; set; }
         public int poa_contact_type_id { get; set; }
         public string poa_contact_type_description { get; set; }
         public string poa_contact_type_value { get; set; }
         public string peoplesoft_flag { get; set; }
         public string address_validate_flag { get; set; }
         public string address_validate_error { get; set; }
         public string same_as_member_address { get; set; }
         public int error_status_id { get; set; }
         public string error_status_description { get; set; }
         public string error_status_value { get; set; }
         public string suppress_warnings_flag { get; set; }
         public string notes { get; set; }
         public DateTime expiration_date { get; set; }
         public string exp_on_death { get; set; }

    }
    [Serializable]
    public enum enmPersonContact
    {
         person_contact_id ,
         person_id ,
         contact_person_id ,
         contact_org_id ,
         relationship_id ,
         relationship_description ,
         relationship_value ,
         status_id ,
         status_description ,
         status_value ,
         contact_name ,
         address_line_1 ,
         address_line_2 ,
         address_city ,
         address_state_id ,
         address_state_description ,
         address_state_value ,
         address_country_id ,
         address_country_description ,
         address_country_value ,
         foreign_province ,
         foreign_postal_code ,
         address_zip_code ,
         address_zip_4_code ,
         contact_phone_no ,
         email_address ,
         poa_contact_type_id ,
         poa_contact_type_description ,
         poa_contact_type_value ,
         peoplesoft_flag ,
         address_validate_flag ,
         address_validate_error ,
         same_as_member_address ,
         error_status_id ,
         error_status_description ,
         error_status_value ,
         suppress_warnings_flag ,
         notes ,
         expiration_date ,
         exp_on_death ,
    }
}

