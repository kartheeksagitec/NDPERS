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
	/// Class NeoSpin.DataObjects.doContact:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doContact : doBase
    {
         
         public doContact() : base()
         {
         }
         public int contact_id { get; set; }
         public int name_prefix_id { get; set; }
         public string name_prefix_description { get; set; }
         public string name_prefix_value { get; set; }
         public string first_name { get; set; }
         public string middle_name { get; set; }
         public string last_name { get; set; }
         public string phone_no { get; set; }
         public string fax_no { get; set; }
         public string npn_number { get; set; }
         public string email_address { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public int primary_address_id { get; set; }
         public string other_phone_number { get; set; }
         public DateTime registration_date { get; set; }
         public string phone_no_extn { get; set; }
         public string fax_no_extn { get; set; }
         public string other_phone_number_extn { get; set; }
         public string ndpers_login_id { get; set; }
         public string previous_ndpers_login_id { get; set; }
         public string exempt_from_training_flag { get; set; }
    }
    [Serializable]
    public enum enmContact
    {
         contact_id ,
         name_prefix_id ,
         name_prefix_description ,
         name_prefix_value ,
         first_name ,
         middle_name ,
         last_name ,
         phone_no ,
         fax_no ,
         npn_number ,
         email_address ,
         status_id ,
         status_description ,
         status_value ,
         primary_address_id ,
         other_phone_number ,
         registration_date ,
         phone_no_extn ,
         fax_no_extn ,
         other_phone_number_extn ,
         ndpers_login_id ,
         previous_ndpers_login_id ,
         exempt_from_training_flag ,
    }
}

