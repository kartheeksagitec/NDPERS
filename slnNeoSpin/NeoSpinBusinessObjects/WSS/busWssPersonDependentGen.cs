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
    /// Class NeoSpin.BusinessObjects.busWssPersonDependentGen:
    /// Inherited from busBase, used to create new business object for main table cdoWssPersonDependent and its children table. 
    /// </summary>
	[Serializable]
	public class busWssPersonDependentGen : busPersonDependent
	{
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busWssPersonDependentGen
        /// </summary>
		public busWssPersonDependentGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busWssPersonDependentGen.
        /// </summary>
		public cdoWssPersonDependent icdoWssPersonDependent { get; set; }




        /// <summary>
        /// NeoSpin.busWssPersonDependentGen.FindWssPersonDependent():
        /// Finds a particular record from cdoWssPersonDependent with its primary key. 
        /// </summary>
        /// <param name="aintwsspersondependentid">A primary key value of type int of cdoWssPersonDependent on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindWssPersonDependent(int aintwsspersondependentid)
		{
			bool lblnResult = false;
			if (icdoWssPersonDependent == null)
			{
				icdoWssPersonDependent = new cdoWssPersonDependent();
			}
			if (icdoWssPersonDependent.SelectRow(new object[1] { aintwsspersondependentid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
