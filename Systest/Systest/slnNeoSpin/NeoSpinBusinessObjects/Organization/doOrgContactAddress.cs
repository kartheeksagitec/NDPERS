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
	/// Class NeoSpin.DataObjects.doOrgContactAddress:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doOrgContactAddress : doBase
    {
         
         public doOrgContactAddress() : base()
         {
         }
         public int contact_org_address_id { get; set; }
         public int org_id { get; set; }
         public int contact_id { get; set; }
         public string addr_line_1 { get; set; }
         public string addr_line_2 { get; set; }
         public string city { get; set; }
         public int state_id { get; set; }
         public string state_description { get; set; }
         public string state_value { get; set; }
         public int country_id { get; set; }
         public string country_description { get; set; }
         public string country_value { get; set; }
         public string foreign_province { get; set; }
         public string foreign_postal_code { get; set; }
         public string zip_code { get; set; }
         public string zip_4_code { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public string address_validate_flag { get; set; }
         public string address_validate_error { get; set; }
    }
    [Serializable]
    public enum enmOrgContactAddress
    {
         contact_org_address_id ,
         org_id ,
         contact_id ,
         addr_line_1 ,
         addr_line_2 ,
         city ,
         state_id ,
         state_description ,
         state_value ,
         country_id ,
         country_description ,
         country_value ,
         foreign_province ,
         foreign_postal_code ,
         zip_code ,
         zip_4_code ,
         status_id ,
         status_description ,
         status_value ,
         address_validate_flag ,
         address_validate_error ,
    }
}

