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
	/// Class NeoSpin.DataObjects.doIbsPersonSummary:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doIbsPersonSummary : doBase
    {
         public doIbsPersonSummary() : base()
         {
         }
         public int ibs_person_summary_id { get; set; }
         public int ibs_header_id { get; set; }
         public int person_id { get; set; }
         public decimal balance_forward { get; set; }
         public decimal remittance_balance_forward { get; set; }
         public decimal adjustment_amount { get; set; }
         public decimal member_premium_amount { get; set; }
    }
    [Serializable]
    public enum enmIbsPersonSummary
    {
         ibs_person_summary_id ,
         ibs_header_id ,
         person_id ,
         balance_forward ,
         remittance_balance_forward ,
         adjustment_amount ,
         member_premium_amount ,
    }
}

