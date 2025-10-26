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
    /// Class NeoSpin.BusinessObjects.busWssPersonAccountEnrollmentRequestAckGen:
    /// Inherited from busBase, used to create new business object for main table cdoWssPersonAccountEnrollmentRequestAck and its children table. 
    /// </summary>
	[Serializable]
	public class busWssPersonAccountEnrollmentRequestAckGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busWssPersonAccountEnrollmentRequestAckGen
        /// </summary>
		public busWssPersonAccountEnrollmentRequestAckGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busWssPersonAccountEnrollmentRequestAckGen.
        /// </summary>
		public cdoWssPersonAccountEnrollmentRequestAck icdoWssPersonAccountEnrollmentRequestAck { get; set; }




        /// <summary>
        /// NeoSpin.busWssPersonAccountEnrollmentRequestAckGen.FindWssPersonAccountEnrollmentRequestAck():
        /// Finds a particular record from cdoWssPersonAccountEnrollmentRequestAck with its primary key. 
        /// </summary>
        /// <param name="aintpersonaccountenrollmentrequestackid">A primary key value of type int of cdoWssPersonAccountEnrollmentRequestAck on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindWssPersonAccountEnrollmentRequestAck(int aintpersonaccountenrollmentrequestackid)
		{
			bool lblnResult = false;
			if (icdoWssPersonAccountEnrollmentRequestAck == null)
			{
				icdoWssPersonAccountEnrollmentRequestAck = new cdoWssPersonAccountEnrollmentRequestAck();
			}
			if (icdoWssPersonAccountEnrollmentRequestAck.SelectRow(new object[1] { aintpersonaccountenrollmentrequestackid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
