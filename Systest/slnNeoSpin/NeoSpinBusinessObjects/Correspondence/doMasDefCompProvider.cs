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
	/// Class NeoSpin.DataObjects.doMasDefCompProvider:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doMasDefCompProvider : doBase
    {
        
         public doMasDefCompProvider() : base()
         {
         }
         public int mas_def_comp_provider_id { get; set; }
         public int mas_person_id { get; set; }
         public string provider_name { get; set; }
         public decimal per_pay_period_contribution_amount { get; set; }
         public DateTime statement_effective_date { get; set; }
    }
    [Serializable]
    public enum enmMasDefCompProvider
    {
         mas_def_comp_provider_id ,
         mas_person_id ,
         provider_name ,
         per_pay_period_contribution_amount ,
         statement_effective_date ,
    }
}

