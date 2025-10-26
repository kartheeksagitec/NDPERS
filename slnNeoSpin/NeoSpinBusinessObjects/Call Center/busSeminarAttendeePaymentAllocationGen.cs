#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using  NeoSpin.CustomDataObjects;

#endregion

namespace  NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class  NeoSpin.BusinessObjects.busSeminarAttendeePaymentAllocationGen:
    /// Inherited from busBase, used to create new business object for main table cdoSeminarAttendeePaymentAllocation and its children table. 
    /// </summary>
	[Serializable]
	public class busSeminarAttendeePaymentAllocationGen : busExtendBase
    {
        /// <summary>
        /// Constructor for  NeoSpin.BusinessObjects.busSeminarAttendeePaymentAllocationGen
        /// </summary>
		public busSeminarAttendeePaymentAllocationGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busSeminarAttendeePaymentAllocationGen.
        /// </summary>
		public cdoSeminarAttendeePaymentAllocation icdoSeminarAttendeePaymentAllocation { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busSeminarAttendeeDetail.
        /// </summary>
		public busSeminarAttendeeDetail ibusSeminarAttendeeDetail { get; set; }




        /// <summary>
        ///  NeoSpin.busSeminarAttendeePaymentAllocationGen.FindSeminarAttendeePaymentAllocation():
        /// Finds a particular record from cdoSeminarAttendeePaymentAllocation with its primary key. 
        /// </summary>
        /// <param name="aintseminarattendeepaymentallocationid">A primary key value of type int of cdoSeminarAttendeePaymentAllocation on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindSeminarAttendeePaymentAllocation(int aintseminarattendeepaymentallocationid)
		{
			bool lblnResult = false;
			if (icdoSeminarAttendeePaymentAllocation == null)
			{
				icdoSeminarAttendeePaymentAllocation = new cdoSeminarAttendeePaymentAllocation();
			}
			if (icdoSeminarAttendeePaymentAllocation.SelectRow(new object[1] { aintseminarattendeepaymentallocationid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        ///  NeoSpin.busSeminarAttendeePaymentAllocationGen.LoadSeminarAttendeeDetail():
        /// Loads non-collection object ibusSeminarAttendeeDetail of type busSeminarAttendeeDetail.
        /// </summary>
		public virtual void LoadSeminarAttendeeDetail()
		{
			if (ibusSeminarAttendeeDetail == null)
			{
				ibusSeminarAttendeeDetail = new busSeminarAttendeeDetail();
			}
			ibusSeminarAttendeeDetail.FindSeminarAttendeeDetail(icdoSeminarAttendeePaymentAllocation.seminar_attendee_detail_id);
		}

	}
}
