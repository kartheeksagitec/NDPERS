#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class NeoSpin.BusinessObjects.busWssPersonAccountEnrollmentRequestGen:
    /// Inherited from busBase, used to create new business object for main table cdoWssPersonAccountEnrollmentRequest and its children table. 
    /// </summary>
	[Serializable]
	public class busWssPersonAccountEnrollmentRequestGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busWssPersonAccountEnrollmentRequestGen
        /// </summary>
		public busWssPersonAccountEnrollmentRequestGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busWssPersonAccountEnrollmentRequestGen.
        /// </summary>
		public cdoWssPersonAccountEnrollmentRequest icdoWssPersonAccountEnrollmentRequest { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busPerson.
        /// </summary>
		public busPerson ibusPerson { get; set; }




        /// <summary>
        /// NeoSpin.busWssPersonAccountEnrollmentRequestGen.FindWssPersonAccountEnrollmentRequest():
        /// Finds a particular record from cdoWssPersonAccountEnrollmentRequest with its primary key. 
        /// </summary>
        /// <param name="aintwsspersonaccountenrollmentrequestid">A primary key value of type int of cdoWssPersonAccountEnrollmentRequest on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindWssPersonAccountEnrollmentRequest(int aintwsspersonaccountenrollmentrequestid)
		{
			bool lblnResult = false;
			if (icdoWssPersonAccountEnrollmentRequest == null)
			{
				icdoWssPersonAccountEnrollmentRequest = new cdoWssPersonAccountEnrollmentRequest();
			}
			if (icdoWssPersonAccountEnrollmentRequest.SelectRow(new object[1] { aintwsspersonaccountenrollmentrequestid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

		//Fw upgrade issues - For workflow process
        public virtual bool FindWssPersonAccountEnrollmentRequest(long aintwsspersonaccountenrollmentrequestid)
        {
            bool lblnResult = false;
            if (icdoWssPersonAccountEnrollmentRequest == null)
            {
                icdoWssPersonAccountEnrollmentRequest = new cdoWssPersonAccountEnrollmentRequest();
            }
            if (icdoWssPersonAccountEnrollmentRequest.SelectRow(new object[1] { aintwsspersonaccountenrollmentrequestid }))
            {
                lblnResult = true;
            }
            return lblnResult;
        }
        /// <summary>
        /// NeoSpin.busWssPersonAccountEnrollmentRequestGen.LoadPerson():
        /// Loads non-collection object ibusPerson of type busPerson.
        /// </summary>
		public virtual void LoadPerson()
		{
			if (ibusPerson == null)
			{
				ibusPerson = new busPerson();
			}
			ibusPerson.FindPerson(icdoWssPersonAccountEnrollmentRequest.person_id);
		}

	}
}
