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
	/// Class NeoSpin.DataObjects.doScreenNotes:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doScreenNotes : doBase
    {
         public doScreenNotes() : base()
         {
         }
         public int screen_notes_id { get; set; }
         public int screen_id { get; set; }
         public string screen_description { get; set; }
         public string screen_value { get; set; }
         public int screen_primary_id { get; set; }
         public string notes { get; set; }
    }
    [Serializable]
    public enum enmScreenNotes
    {
         screen_notes_id ,
         screen_id ,
         screen_description ,
         screen_value ,
         screen_primary_id ,
         notes ,
    }
}
