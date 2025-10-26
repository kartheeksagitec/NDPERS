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
    /// Class NeoSpin.BusinessObjects.busMasFlexConversionGen:
    /// Inherited from busBase, used to create new business object for main table cdoMasFlexConversion and its children table. 
    /// </summary>
	[Serializable]
	public class busMasFlexConversionGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busMasFlexConversionGen
        /// </summary>
		public busMasFlexConversionGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busMasFlexConversionGen.
        /// </summary>
		public cdoMasFlexConversion icdoMasFlexConversion { get; set; }




        /// <summary>
        /// NeoSpin.busMasFlexConversionGen.FindMasFlexConversion():
        /// Finds a particular record from cdoMasFlexConversion with its primary key. 
        /// </summary>
        /// <param name="aintmasflexconversionid">A primary key value of type int of cdoMasFlexConversion on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindMasFlexConversion(int aintmasflexconversionid)
		{
			bool lblnResult = false;
			if (icdoMasFlexConversion == null)
			{
				icdoMasFlexConversion = new cdoMasFlexConversion();
			}
			if (icdoMasFlexConversion.SelectRow(new object[1] { aintmasflexconversionid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
