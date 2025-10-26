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
    /// Class NeoSpin.BusinessObjects.busWssEmploymentChangeRequestGen:
    /// Inherited from busBase, used to create new business object for main table cdoWssEmploymentChangeRequest and its children table. 
    /// </summary>
	[Serializable]
	public class busWssEmploymentChangeRequestGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busWssEmploymentChangeRequestGen
        /// </summary>
		public busWssEmploymentChangeRequestGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busWssEmploymentChangeRequestGen.
        /// </summary>
		public cdoWssEmploymentChangeRequest icdoWssEmploymentChangeRequest { get; set; }




        /// <summary>
        /// NeoSpin.busWssEmploymentChangeRequestGen.FindWssEmploymentChangeRequest():
        /// Finds a particular record from cdoWssEmploymentChangeRequest with its primary key. 
        /// </summary>
        /// <param name="aintemploymentchangerequestid">A primary key value of type int of cdoWssEmploymentChangeRequest on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindWssEmploymentChangeRequest(int aintemploymentchangerequestid)
		{
			bool lblnResult = false;
			if (icdoWssEmploymentChangeRequest == null)
			{
				icdoWssEmploymentChangeRequest = new cdoWssEmploymentChangeRequest();
			}
			if (icdoWssEmploymentChangeRequest.SelectRow(new object[1] { aintemploymentchangerequestid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
