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
    /// Class NeoSpin.BusinessObjects.busWssPersonAddressGen:
    /// Inherited from busBase, used to create new business object for main table cdoWssPersonAddress and its children table. 
    /// </summary>
	[Serializable]
	public class busWssPersonAddressGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busWssPersonAddressGen
        /// </summary>
		public busWssPersonAddressGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busWssPersonAddressGen.
        /// </summary>
		public cdoWssPersonAddress icdoWssPersonAddress { get; set; }




        /// <summary>
        /// NeoSpin.busWssPersonAddressGen.FindWssPersonAddress():
        /// Finds a particular record from cdoWssPersonAddress with its primary key. 
        /// </summary>
        /// <param name="aintwsspersonaddressid">A primary key value of type int of cdoWssPersonAddress on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindWssPersonAddress(int aintwsspersonaddressid)
		{
			bool lblnResult = false;
			if (icdoWssPersonAddress == null)
			{
				icdoWssPersonAddress = new cdoWssPersonAddress();
			}
			if (icdoWssPersonAddress.SelectRow(new object[1] { aintwsspersonaddressid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
