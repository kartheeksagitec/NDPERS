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
	/// Class NeoSpin.DataObjects.doMasFlexOption:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doMasFlexOption : doBase
    {
         
         public doMasFlexOption() : base()
         {
         }
         public int mas_flex_option_id { get; set; }
         public int mas_person_id { get; set; }
         public string coverage_description { get; set; }
         public decimal annual_pledge_amount { get; set; }
    }
    [Serializable]
    public enum enmMasFlexOption
    {
         mas_flex_option_id ,
         mas_person_id ,
         coverage_description ,
         annual_pledge_amount ,
    }
}

