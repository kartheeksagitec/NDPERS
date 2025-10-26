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
	/// Class NeoSpin.DataObjects.doPersonAccount457LimitRef:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPersonAccount457LimitRef : doBase
    {
         
         public doPersonAccount457LimitRef() : base()
         {
         }
         public int person_account_457_limit_id { get; set; }
         public int limit_457_id { get; set; }
         public string limit_457_description { get; set; }
         public string limit_457_value { get; set; }
         public DateTime start_date { get; set; }
         public DateTime end_date { get; set; }
         public decimal amount { get; set; }
    }
    [Serializable]
    public enum enmPersonAccount457LimitRef
    {
         person_account_457_limit_id ,
         limit_457_id ,
         limit_457_description ,
         limit_457_value ,
         start_date ,
         end_date ,
         amount ,
    }
}

