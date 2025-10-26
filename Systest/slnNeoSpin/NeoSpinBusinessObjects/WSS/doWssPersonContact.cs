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
	/// Class NeoSpin.DataObjects.doWssPersonContact:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doWssPersonContact : doBase
    {
         
         public doWssPersonContact() : base()
         {
         }
         public int wss_person_contact_id { get; set; }
         public int member_record_request_id { get; set; }
         public int person_contact_id { get; set; }
         public int contact_person_id { get; set; }
         public int contact_org_id { get; set; }
         public string contact_ssn { get; set; }
         public string contact_name { get; set; }
         public DateTime contact_date_of_birth { get; set; }
         public int contact_gender_id { get; set; }
         public string contact_gender_description { get; set; }
         public string contact_gender_value { get; set; }
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
         public int relationship_id { get; set; }
         public string relationship_description { get; set; }
         public string relationship_value { get; set; }
         public string same_as_member_address { get; set; }
    }
    [Serializable]
    public enum enmWssPersonContact
    {
         wss_person_contact_id ,
         member_record_request_id ,
         person_contact_id ,
         contact_person_id ,
         contact_org_id ,
         contact_ssn ,
         contact_name ,
         contact_date_of_birth ,
         contact_gender_id ,
         contact_gender_description ,
         contact_gender_value ,
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
         relationship_id ,
         relationship_description ,
         relationship_value ,
         same_as_member_address ,
    }
}

