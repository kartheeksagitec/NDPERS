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
	/// Class NeoSpin.DataObjects.doRateChangeLetterRequest:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doRateChangeLetterRequest : doBase
    {
         public doRateChangeLetterRequest() : base()
         {
         }
         public int rate_change_letter_request_id { get; set; }
         public int letter_type_id { get; set; }
         public string letter_type_description { get; set; }
         public string letter_type_value { get; set; }
         public DateTime effective_date { get; set; }
         public int provider_org_id { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public string health { get; set; }
         public string medd { get; set; }
         public string dental { get; set; }
         public string vision { get; set; }
         public string life { get; set; }
    }
    [Serializable]
    public enum enmRateChangeLetterRequest
    {
         rate_change_letter_request_id ,
         letter_type_id ,
         letter_type_description ,
         letter_type_value ,
         effective_date ,
         provider_org_id ,
         status_id ,
         status_description ,
         status_value ,
         health ,
         medd ,
         dental ,
         vision ,
         life ,
    }
}

