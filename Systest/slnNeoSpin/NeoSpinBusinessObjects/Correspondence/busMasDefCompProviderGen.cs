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
    /// Class NeoSpin.BusinessObjects.busMasDefCompProviderGen:
    /// Inherited from busBase, used to create new business object for main table cdoMasDefCompProvider and its children table. 
    /// </summary>
	[Serializable]
	public class busMasDefCompProviderGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busMasDefCompProviderGen
        /// </summary>
		public busMasDefCompProviderGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busMasDefCompProviderGen.
        /// </summary>
		public cdoMasDefCompProvider icdoMasDefCompProvider { get; set; }




        /// <summary>
        /// NeoSpin.busMasDefCompProviderGen.FindMasDefCompProvider():
        /// Finds a particular record from cdoMasDefCompProvider with its primary key. 
        /// </summary>
        /// <param name="aintmasdefcompproviderid">A primary key value of type int of cdoMasDefCompProvider on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindMasDefCompProvider(int aintmasdefcompproviderid)
		{
			bool lblnResult = false;
			if (icdoMasDefCompProvider == null)
			{
				icdoMasDefCompProvider = new cdoMasDefCompProvider();
			}
			if (icdoMasDefCompProvider.SelectRow(new object[1] { aintmasdefcompproviderid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
