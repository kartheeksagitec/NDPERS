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
    /// Class NeoSpin.BusinessObjects.busWssPersonContactGen:
    /// Inherited from busBase, used to create new business object for main table cdoWssPersonContact and its children table. 
    /// </summary>
	[Serializable]
	public class busWssPersonContactGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busWssPersonContactGen
        /// </summary>
		public busWssPersonContactGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busWssPersonContactGen.
        /// </summary>
		public cdoWssPersonContact icdoWssPersonContact { get; set; }




        /// <summary>
        /// NeoSpin.busWssPersonContactGen.FindWssPersonContact():
        /// Finds a particular record from cdoWssPersonContact with its primary key. 
        /// </summary>
        /// <param name="aintwsspersoncontactid">A primary key value of type int of cdoWssPersonContact on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindWssPersonContact(int aintwsspersoncontactid)
		{
			bool lblnResult = false;
			if (icdoWssPersonContact == null)
			{
				icdoWssPersonContact = new cdoWssPersonContact();
			}
			if (icdoWssPersonContact.SelectRow(new object[1] { aintwsspersoncontactid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
