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
    /// Class NeoSpin.BusinessObjects.busPsPersonGen:
    /// Inherited from busBase, used to create new business object for main table cdoPsPerson and its children table. 
    /// </summary>
	[Serializable]
	public class busPsPersonGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busPsPersonGen
        /// </summary>
		public busPsPersonGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPsPersonGen.
        /// </summary>
		public cdoPsPerson icdoPsPerson { get; set; }




        /// <summary>
        /// NeoSpin.busPsPersonGen.FindPsPerson():
        /// Finds a particular record from cdoPsPerson with its primary key. 
        /// </summary>
        /// <param name="aintPsPersonId">A primary key value of type int of cdoPsPerson on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPsPerson(int aintPsPersonId)
		{
			bool lblnResult = false;
			if (icdoPsPerson == null)
			{
				icdoPsPerson = new cdoPsPerson();
			}
			if (icdoPsPerson.SelectRow(new object[1] { aintPsPersonId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
