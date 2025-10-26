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
	/// Class NeoSpin.DataObjects.doMasFlexConversion:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doMasFlexConversion : doBase
    {
         
         public doMasFlexConversion() : base()
         {
         }
         public int mas_flex_conversion_id { get; set; }
         public int mas_person_id { get; set; }
         public string conversion_org_name { get; set; }
    }
    [Serializable]
    public enum enmMasFlexConversion
    {
         mas_flex_conversion_id ,
         mas_person_id ,
         conversion_org_name ,
    }
}

