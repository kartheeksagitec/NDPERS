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
	/// Class NeoSpin.DataObjects.doCustomCorrespondence:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doCustomCorrespondence : doBase
    {
         public doCustomCorrespondence() : base()
         {
         }
         public int cor_id { get; set; }
    }
    [Serializable]
    public enum enmCustomCorrespondence
    {
         cor_id ,
    }
}
