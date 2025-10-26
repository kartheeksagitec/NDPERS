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
	/// Class NeoSpin.DataObjects.doEmployerPayrollBonusDetail:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doEmployerPayrollBonusDetail : doBase
    {
         
         public doEmployerPayrollBonusDetail() : base()
         {
         }
         public int employer_payroll_bonus_detail_id { get; set; }
         public int employer_payroll_detail_id { get; set; }
         public DateTime bonus_period { get; set; }
         public decimal eligible_wages { get; set; }
         public decimal ee_contribution { get; set; }
         public decimal ee_pre_tax { get; set; }
         public decimal ee_employer_pickup { get; set; }
         public decimal er_contribution { get; set; }
         public decimal rhic_er_contribution { get; set; }
         public decimal rhic_ee_contribution { get; set; }
         public decimal member_interest { get; set; }
         public decimal employer_interest { get; set; }
         public decimal employer_rhic_interest { get; set; }
    }
    [Serializable]
    public enum enmEmployerPayrollBonusDetail
    {
         employer_payroll_bonus_detail_id ,
         employer_payroll_detail_id ,
         bonus_period ,
         eligible_wages ,
         ee_contribution ,
         ee_pre_tax ,
         ee_employer_pickup ,
         er_contribution ,
         rhic_er_contribution ,
         rhic_ee_contribution ,
         member_interest ,
         employer_interest ,
         employer_rhic_interest ,
    }
}

