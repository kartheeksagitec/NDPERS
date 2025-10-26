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
    /// Class NeoSpin.BusinessObjects.busPsAddressGen:
    /// Inherited from busBase, used to create new business object for main table cdoPsAddress and its children table. 
    /// </summary>
	[Serializable]
	public class busPsAddressGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busPsAddressGen
        /// </summary>
		public busPsAddressGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPsAddressGen.
        /// </summary>
		public cdoPsAddress icdoPsAddress { get; set; }




        /// <summary>
        /// NeoSpin.busPsAddressGen.FindPsAddress():
        /// Finds a particular record from cdoPsAddress with its primary key. 
        /// </summary>
        /// <param name="aintPsAddressId">A primary key value of type int of cdoPsAddress on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPsAddress(int aintPsAddressId)
		{
			bool lblnResult = false;
			if (icdoPsAddress == null)
			{
				icdoPsAddress = new cdoPsAddress();
			}
			if (icdoPsAddress.SelectRow(new object[1] { aintPsAddressId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
