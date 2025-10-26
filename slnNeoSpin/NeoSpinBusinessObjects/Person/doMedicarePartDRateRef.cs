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
	/// Class NeoSpin.DataObjects.doMedicarePartDRateRef:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doMedicarePartDRateRef : doBase
    {
        
         public doMedicarePartDRateRef() : base()
         {
         }
         public int medicare_part_d_rate_ref_id { get; set; }
         public DateTime effective_date { get; set; }
         public decimal medicare_part_d_rate { get; set; }
    }
    [Serializable]
    public enum enmMedicarePartDRateRef
    {
         medicare_part_d_rate_ref_id ,
         effective_date ,
         medicare_part_d_rate ,
    }
}

