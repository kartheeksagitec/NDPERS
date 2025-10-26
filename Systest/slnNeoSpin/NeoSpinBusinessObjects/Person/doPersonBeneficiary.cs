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
	/// Class NeoSpin.DataObjects.doPersonBeneficiary:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPersonBeneficiary : doBase
    {
         
         public doPersonBeneficiary() : base()
         {
         }
         public int beneficiary_id { get; set; }
         public int person_id { get; set; }
         public int beneficiary_person_id { get; set; }
         public int benificiary_org_id { get; set; }
         public string first_name { get; set; }
         public string last_name { get; set; }
         public string middle_name { get; set; }
         public DateTime date_of_birth { get; set; }
         public int marital_status_id { get; set; }
         public string marital_status_description { get; set; }
         public string marital_status_value { get; set; }
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
         public string address_validate_flag { get; set; }
         public string address_validate_error { get; set; }
         public string same_as_member_address { get; set; }
    }
    [Serializable]
    public enum enmPersonBeneficiary
    {
         beneficiary_id ,
         person_id ,
         beneficiary_person_id ,
         benificiary_org_id ,
         first_name ,
         last_name ,
         middle_name ,
         date_of_birth ,
         marital_status_id ,
         marital_status_description ,
         marital_status_value ,
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
         address_validate_flag ,
         address_validate_error ,
         same_as_member_address ,
    }
}

