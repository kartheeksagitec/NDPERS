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
    /// Class NeoSpin.BusinessObjects.busMasLifeOptionGen:
    /// Inherited from busBase, used to create new business object for main table cdoMasLifeOption and its children table. 
    /// </summary>
	[Serializable]
	public class busMasLifeOptionGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busMasLifeOptionGen
        /// </summary>
		public busMasLifeOptionGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busMasLifeOptionGen.
        /// </summary>
		public cdoMasLifeOption icdoMasLifeOption { get; set; }




        /// <summary>
        /// NeoSpin.busMasLifeOptionGen.FindMasLifeOption():
        /// Finds a particular record from cdoMasLifeOption with its primary key. 
        /// </summary>
        /// <param name="aintmaslifecoverageid">A primary key value of type int of cdoMasLifeOption on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindMasLifeOption(int aintmaslifecoverageid)
		{
			bool lblnResult = false;
			if (icdoMasLifeOption == null)
			{
				icdoMasLifeOption = new cdoMasLifeOption();
			}
			if (icdoMasLifeOption.SelectRow(new object[1] { aintmaslifecoverageid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
