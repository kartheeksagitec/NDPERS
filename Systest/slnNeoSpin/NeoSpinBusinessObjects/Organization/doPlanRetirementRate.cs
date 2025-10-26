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
	/// Class NeoSpin.DataObjects.doPlanRetirementRate:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPlanRetirementRate : doBase
    {
         
         public doPlanRetirementRate() : base()
         {
         }
         public int plan_rate_id { get; set; }
         public int plan_id { get; set; }
         public DateTime effective_date { get; set; }
         public int member_type_id { get; set; }
         public string member_type_description { get; set; }
         public string member_type_value { get; set; }
         public decimal ee_pre_tax { get; set; }
         public decimal ee_post_tax { get; set; }
         public decimal ee_emp_pickup { get; set; }
         public decimal ee_rhic { get; set; }
         public decimal er_post_tax { get; set; }
         public decimal er_rhic { get; set; }
         public decimal addl_ee_pre_tax { get; set; }
         public decimal addl_ee_post_tax { get; set; }
         public decimal addl_ee_emp_pickup { get; set; }
		 //PIR 25920 New Plan DC 2025
        public decimal ee_pretax_addl { get; set; }
        public decimal ee_post_tax_addl { get; set; }
        public decimal er_pretax_match { get; set; }
        public decimal adec { get; set; }
    }
    [Serializable]
    public enum enmPlanRetirementRate
    {
         plan_rate_id ,
         plan_id ,
         effective_date ,
         member_type_id ,
         member_type_description ,
         member_type_value ,
         ee_pre_tax ,
         ee_post_tax ,
         ee_emp_pickup ,
         ee_rhic ,
         er_post_tax ,
         er_rhic ,
         addl_ee_pre_tax ,
         addl_ee_post_tax ,
         addl_ee_emp_pickup ,
        ee_pretax_addl,
        ee_post_tax_addl,
        er_pretax_match,
        adec,
    }
}

