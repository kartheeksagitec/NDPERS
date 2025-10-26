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
	/// Class NeoSpin.DataObjects.doWssAcknowledgement:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doWssAcknowledgement : doBase
    {
         
         public doWssAcknowledgement() : base()
         {
         }
         public int acknowledgement_id { get; set; }
         public DateTime effective_date { get; set; }
         public int screen_step_id { get; set; }
         public string screen_step_description { get; set; }
         public string screen_step_value { get; set; }
         public int display_sequence { get; set; }
         public string acknowledgement_text { get; set; }
         public string show_check_box_flag { get; set; }
    }
    [Serializable]
    public enum enmWssAcknowledgement
    {
         acknowledgement_id ,
         effective_date ,
         screen_step_id ,
         screen_step_description ,
         screen_step_value ,
         display_sequence ,
         acknowledgement_text ,
         show_check_box_flag ,
    }
}

