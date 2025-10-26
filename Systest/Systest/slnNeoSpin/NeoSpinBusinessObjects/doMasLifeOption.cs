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
	/// Class NeoSpin.DataObjects.doMasLifeOption:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doMasLifeOption : doBase
    {
         
         public doMasLifeOption() : base()
         {
         }
         public int mas_life_coverage_id { get; set; }
         public int mas_person_id { get; set; }
         public string level_of_coverage { get; set; }
         public decimal coverage_amount { get; set; }
    }
    [Serializable]
    public enum enmMasLifeOption
    {
         mas_life_coverage_id ,
         mas_person_id ,
         level_of_coverage ,
         coverage_amount ,
    }
}

