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
    /// Class NeoSpin.BusinessObjects.busMASSelectionGen:
    /// Inherited from busBase, used to create new business object for main table cdoMasSelection and its children table. 
    /// </summary>
	[Serializable]
	public class busMASSelectionGen : busMAS
	{
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busMASSelectionGen
        /// </summary>
		public busMASSelectionGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busMASSelectionGen.
        /// </summary>
		public cdoMasSelection icdoMasSelection { get; set; }




        /// <summary>
        /// NeoSpin.busMASSelectionGen.FindMASSelection():
        /// Finds a particular record from cdoMasSelection with its primary key. 
        /// </summary>
        /// <param name="aintannualstatementbatchrequestdetailid">A primary key value of type int of cdoMasSelection on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindMASSelection(int aintannualstatementbatchrequestdetailid)
		{
			bool lblnResult = false;
			if (icdoMasSelection == null)
			{
				icdoMasSelection = new cdoMasSelection();
			}
			if (icdoMasSelection.SelectRow(new object[1] { aintannualstatementbatchrequestdetailid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
