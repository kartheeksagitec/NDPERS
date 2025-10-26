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
	/// Class NeoSpin.DataObjects.doSeminarAttendeePaymentAllocation:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doSeminarAttendeePaymentAllocation : doBase
    {
         public doSeminarAttendeePaymentAllocation() : base()
         {
         }
         public int seminar_attendee_payment_allocation_id { get; set; }
         public int seminar_attendee_detail_id { get; set; }
         public int remittance_id { get; set; }
         public decimal applied_amount { get; set; }
         public DateTime payment_date { get; set; }
    }
    [Serializable]
    public enum enmSeminarAttendeePaymentAllocation
    {
         seminar_attendee_payment_allocation_id ,
         seminar_attendee_detail_id ,
         remittance_id ,
         applied_amount ,
         payment_date ,
    }
}

