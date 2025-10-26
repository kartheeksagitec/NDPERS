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
    /// Class NeoSpin.BusinessObjects.busWssPersonAccountWorkerCompensationGen:
    /// Inherited from busBase, used to create new business object for main table cdoWssPersonAccountWorkerCompensation and its children table. 
    /// </summary>
	[Serializable]
	public class busWssPersonAccountWorkerCompensationGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busWssPersonAccountWorkerCompensationGen
        /// </summary>
		public busWssPersonAccountWorkerCompensationGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busWssPersonAccountWorkerCompensationGen.
        /// </summary>
		public cdoWssPersonAccountWorkerCompensation icdoWssPersonAccountWorkerCompensation { get; set; }




        /// <summary>
        /// NeoSpin.busWssPersonAccountWorkerCompensationGen.FindWssPersonAccountWorkerCompensation():
        /// Finds a particular record from cdoWssPersonAccountWorkerCompensation with its primary key. 
        /// </summary>
        /// <param name="aintwsspersonaccountworkercompensationid">A primary key value of type int of cdoWssPersonAccountWorkerCompensation on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindWssPersonAccountWorkerCompensation(int aintwsspersonaccountworkercompensationid)
		{
			bool lblnResult = false;
			if (icdoWssPersonAccountWorkerCompensation == null)
			{
				icdoWssPersonAccountWorkerCompensation = new cdoWssPersonAccountWorkerCompensation();
			}
			if (icdoWssPersonAccountWorkerCompensation.SelectRow(new object[1] { aintwsspersonaccountworkercompensationid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
