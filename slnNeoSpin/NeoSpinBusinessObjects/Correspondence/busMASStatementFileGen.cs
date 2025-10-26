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
    /// Class NeoSpin.BusinessObjects.busMasStatementFileGen:
    /// Inherited from busBase, used to create new business object for main table cdoMasStatementFile and its children table. 
    /// </summary>
	[Serializable]
	public class busMasStatementFileGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busMasStatementFileGen
        /// </summary>
		public busMasStatementFileGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busMasStatementFileGen.
        /// </summary>
		public cdoMasStatementFile icdoMasStatementFile { get; set; }




        /// <summary>
        /// NeoSpin.busMasStatementFileGen.FindMasStatementFile():
        /// Finds a particular record from cdoMasStatementFile with its primary key. 
        /// </summary>
        /// <param name="aintmasstatementfileid">A primary key value of type int of cdoMasStatementFile on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindMasStatementFile(int aintmasstatementfileid)
		{
			bool lblnResult = false;
			if (icdoMasStatementFile == null)
			{
				icdoMasStatementFile = new cdoMasStatementFile();
			}
			if (icdoMasStatementFile.SelectRow(new object[1] { aintmasstatementfileid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
