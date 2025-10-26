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
    /// Class NeoSpin.BusinessObjects.busMasPersonCalculationGen:
    /// Inherited from busBase, used to create new business object for main table cdoMasPersonCalculation and its children table. 
    /// </summary>
	[Serializable]
	public class busMasPersonCalculationGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busMasPersonCalculationGen
        /// </summary>
		public busMasPersonCalculationGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busMasPersonCalculationGen.
        /// </summary>
		public cdoMasPersonCalculation icdoMasPersonCalculation { get; set; }




        /// <summary>
        /// NeoSpin.busMasPersonCalculationGen.FindMasPersonCalculation():
        /// Finds a particular record from cdoMasPersonCalculation with its primary key. 
        /// </summary>
        /// <param name="aintmaspersoncalculationid">A primary key value of type int of cdoMasPersonCalculation on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindMasPersonCalculation(int aintmaspersoncalculationid)
		{
			bool lblnResult = false;
			if (icdoMasPersonCalculation == null)
			{
				icdoMasPersonCalculation = new cdoMasPersonCalculation();
			}
			if (icdoMasPersonCalculation.SelectRow(new object[1] { aintmaspersoncalculationid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
