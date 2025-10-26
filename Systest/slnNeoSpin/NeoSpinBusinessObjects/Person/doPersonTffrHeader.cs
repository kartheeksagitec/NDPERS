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
	/// Class NeoSpin.DataObjects.doPersonTffrHeader:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPersonTffrHeader : doBase
    {
         public doPersonTffrHeader() : base()
         {
         }
         public int person_tffr_header_id { get; set; }
         public int upload_id { get; set; }
         public int person_id { get; set; }
         public DateTime upload_date { get; set; }
         public string notes { get; set; }
    }
    [Serializable]
    public enum enmPersonTffrHeader
    {
         person_tffr_header_id ,
         upload_id ,
         person_id ,
         upload_date ,
         notes ,
    }
}
