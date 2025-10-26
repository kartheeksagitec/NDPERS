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
	/// Class NeoSpin.DataObjects.doPaymentStatusTransitionGlRef:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPaymentStatusTransitionGlRef : doBase
    {
         
         public doPaymentStatusTransitionGlRef() : base()
         {
         }
         public int status_transition_gl_ref_id { get; set; }
         public int status_transition_type_id { get; set; }
         public string status_transition_type_description { get; set; }
         public string status_transition_type_value { get; set; }
         public string from_status { get; set; }
         public string to_status { get; set; }
         public string generate_gl_flag { get; set; }
         public string transaction_type { get; set; }
         public string status_transition_value { get; set; }
    }
    [Serializable]
    public enum enmPaymentStatusTransitionGlRef
    {
         status_transition_gl_ref_id ,
         status_transition_type_id ,
         status_transition_type_description ,
         status_transition_type_value ,
         from_status ,
         to_status ,
         generate_gl_flag ,
         transaction_type ,
         status_transition_value ,
    }
}

