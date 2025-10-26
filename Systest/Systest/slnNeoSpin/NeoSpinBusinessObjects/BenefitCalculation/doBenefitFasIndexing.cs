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
	/// Class NeoSpin.DataObjects.doBenefitFasIndexing:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doBenefitFasIndexing : doBase
    {
         public doBenefitFasIndexing() : base()
         {
         }
         public int benefit_hp_fas_id { get; set; }
         public int benefit_calculation_id { get; set; }
         public DateTime benefit_begin_date { get; set; }
         public DateTime benefit_end_date { get; set; }
         public decimal average_increase_percentage { get; set; }
         public decimal year_average_increase_factor { get; set; }
         public decimal salary_factor { get; set; }
    }
    [Serializable]
    public enum enmBenefitFasIndexing
    {
         benefit_hp_fas_id ,
         benefit_calculation_id ,
         benefit_begin_date ,
         benefit_end_date ,
         average_increase_percentage ,
         year_average_increase_factor ,
         salary_factor ,
    }
}

