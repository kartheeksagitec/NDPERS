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
	/// Class NeoSpin.DataObjects.doWssOtpActivation:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doWssOtpActivation : doBase
    {
         
         public doWssOtpActivation() : base()
         {
         }
         public int otp_activation_id { get; set; }
         public int person_id { get; set; }
         public string reference_id { get; set; }
         public int source_id { get; set; }
         public string source_description { get; set; }
         public string source_value { get; set; }
         public string activation_code { get; set; }
         public DateTime activation_code_date { get; set; }
         public string activation_code_validate { get; set; }
    }
    [Serializable]
    public enum enmWssOtpActivation
    {
         otp_activation_id ,
         person_id ,
         reference_id ,
         source_id ,
         source_description ,
         source_value ,
         activation_code ,
         activation_code_date ,
         activation_code_validate ,
    }
}

