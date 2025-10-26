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
    /// Class NeoSpin.BusinessObjects.busMasFlexOptionGen:
    /// Inherited from busBase, used to create new business object for main table cdoMasFlexOption and its children table. 
    /// </summary>
	[Serializable]
	public class busMasFlexOptionGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busMasFlexOptionGen
        /// </summary>
		public busMasFlexOptionGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busMasFlexOptionGen.
        /// </summary>
		public cdoMasFlexOption icdoMasFlexOption { get; set; }




        /// <summary>
        /// NeoSpin.busMasFlexOptionGen.FindMasFlexOption():
        /// Finds a particular record from cdoMasFlexOption with its primary key. 
        /// </summary>
        /// <param name="aintmasflexoptionid">A primary key value of type int of cdoMasFlexOption on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindMasFlexOption(int aintmasflexoptionid)
		{
			bool lblnResult = false;
			if (icdoMasFlexOption == null)
			{
				icdoMasFlexOption = new cdoMasFlexOption();
			}
			if (icdoMasFlexOption.SelectRow(new object[1] { aintmasflexoptionid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
