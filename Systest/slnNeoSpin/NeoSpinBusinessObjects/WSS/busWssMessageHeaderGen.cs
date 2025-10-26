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
    /// Class NeoSpin.BusinessObjects.busWssMessageHeaderGen:
    /// Inherited from busBase, used to create new business object for main table cdoWssMessageHeader and its children table. 
    /// </summary>
	[Serializable]
	public class busWssMessageHeaderGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busWssMessageHeaderGen
        /// </summary>
		public busWssMessageHeaderGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busWssMessageHeaderGen.
        /// </summary>
		public cdoWssMessageHeader icdoWssMessageHeader { get; set; }




        /// <summary>
        /// NeoSpin.busWssMessageHeaderGen.FindWssMessageHeader():
        /// Finds a particular record from cdoWssMessageHeader with its primary key. 
        /// </summary>
        /// <param name="aintwssmessageid">A primary key value of type int of cdoWssMessageHeader on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindWssMessageHeader(int aintwssmessageid)
		{
			bool lblnResult = false;
			if (icdoWssMessageHeader == null)
			{
				icdoWssMessageHeader = new cdoWssMessageHeader();
			}
			if (icdoWssMessageHeader.SelectRow(new object[1] { aintwssmessageid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
