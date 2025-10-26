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
	/// Class NeoSpin.DataObjects.doIbsRemittanceAllocation:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doIbsRemittanceAllocation : doBase
    {
         
         public doIbsRemittanceAllocation() : base()
         {
         }
         public int ibs_remittance_allocation_id { get; set; }
         public int remittance_id { get; set; }
         public decimal allocated_amount { get; set; }
         public int ibs_allocation_status_id { get; set; }
         public string ibs_allocation_status_description { get; set; }
         public string ibs_allocation_status_value { get; set; }
         public int person_account_id { get; set; }
         public DateTime effective_date { get; set; }
    }
    [Serializable]
    public enum enmIbsRemittanceAllocation
    {
         ibs_remittance_allocation_id ,
         remittance_id ,
         allocated_amount ,
         ibs_allocation_status_id ,
         ibs_allocation_status_description ,
         ibs_allocation_status_value ,
         person_account_id ,
         effective_date ,
    }
}

