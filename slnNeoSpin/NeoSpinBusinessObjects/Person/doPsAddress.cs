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
	/// Class NeoSpin.DataObjects.doPsAddress:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPsAddress : doBase
    {
         
         public doPsAddress() : base()
         {
         }
         public int ps_address_id { get; set; }
         public string ssn { get; set; }
         public string addr_line_1 { get; set; }
         public string addr_line_2 { get; set; }
         public string addr_city { get; set; }
         public string addr_state_value { get; set; }
         public string addr_country_value { get; set; }
         public string addr_zip_code { get; set; }
         public string addr_zip_4_code { get; set; }
         public DateTime addr_start_date { get; set; }
         public string processed_flag { get; set; }
         public string error { get; set; }
    }
    [Serializable]
    public enum enmPsAddress
    {
         ps_address_id ,
         ssn ,
         addr_line_1 ,
         addr_line_2 ,
         addr_city ,
         addr_state_value ,
         addr_country_value ,
         addr_zip_code ,
         addr_zip_4_code ,
         addr_start_date ,
         processed_flag ,
         error ,
    }
}

