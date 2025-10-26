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
	/// Class NeoSpin.DataObjects.doEmployerRemittanceAllocation:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doEmployerRemittanceAllocation : doBase
    {
         
         public doEmployerRemittanceAllocation() : base()
         {
         }
         public int employer_remittance_allocation_id { get; set; }
         public int remittance_id { get; set; }
         public int employer_payroll_header_id { get; set; }
         public decimal allocated_amount { get; set; }
         public DateTime allocated_date { get; set; }
         public int payroll_allocation_status_id { get; set; }
         public string payroll_allocation_status_description { get; set; }
         public string payroll_allocation_status_value { get; set; }
         public decimal difference_amount { get; set; }
         public int difference_type_id { get; set; }
         public string difference_type_description { get; set; }
         public string difference_type_value { get; set; }
    }
    [Serializable]
    public enum enmEmployerRemittanceAllocation
    {
         employer_remittance_allocation_id ,
         remittance_id ,
         employer_payroll_header_id ,
         allocated_amount ,
         allocated_date ,
         payroll_allocation_status_id ,
         payroll_allocation_status_description ,
         payroll_allocation_status_value ,
         difference_amount ,
         difference_type_id ,
         difference_type_description ,
         difference_type_value ,
    }
}

