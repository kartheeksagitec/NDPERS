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
	/// Class NeoSpin.DataObjects.doPersonInvalidSsnRef:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPersonInvalidSsnRef : doBase
    {
         
         public doPersonInvalidSsnRef() : base()
         {
         }
         public int person_invalid_ssn_ref_id { get; set; }
         public string ssn { get; set; }
    }
    [Serializable]
    public enum enmPersonInvalidSsnRef
    {
         person_invalid_ssn_ref_id ,
         ssn ,
    }
}

