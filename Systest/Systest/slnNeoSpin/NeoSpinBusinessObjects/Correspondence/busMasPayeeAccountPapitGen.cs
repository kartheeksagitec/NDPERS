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
    /// Class NeoSpin.BusinessObjects.busMasPayeeAccountPapitGen:
    /// Inherited from busBase, used to create new business object for main table cdoMasPayeeAccountPapit and its children table. 
    /// </summary>
	[Serializable]
	public class busMasPayeeAccountPapitGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busMasPayeeAccountPapitGen
        /// </summary>
		public busMasPayeeAccountPapitGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busMasPayeeAccountPapitGen.
        /// </summary>
		public cdoMasPayeeAccountPapit icdoMasPayeeAccountPapit { get; set; }




        /// <summary>
        /// NeoSpin.busMasPayeeAccountPapitGen.FindMasPayeeAccountPapit():
        /// Finds a particular record from cdoMasPayeeAccountPapit with its primary key. 
        /// </summary>
        /// <param name="aintmaspayeeaccountpapitid">A primary key value of type int of cdoMasPayeeAccountPapit on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindMasPayeeAccountPapit(int aintmaspayeeaccountpapitid)
		{
			bool lblnResult = false;
			if (icdoMasPayeeAccountPapit == null)
			{
				icdoMasPayeeAccountPapit = new cdoMasPayeeAccountPapit();
			}
			if (icdoMasPayeeAccountPapit.SelectRow(new object[1] { aintmaspayeeaccountpapitid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
