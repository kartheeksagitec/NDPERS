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
    /// Class NeoSpin.BusinessObjects.busWssMessageDetailGen:
    /// Inherited from busBase, used to create new business object for main table cdoWssMessageDetail and its children table. 
    /// </summary>
	[Serializable]
	public class busWssMessageDetailGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busWssMessageDetailGen
        /// </summary>
		public busWssMessageDetailGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busWssMessageDetailGen.
        /// </summary>
		public cdoWssMessageDetail icdoWssMessageDetail { get; set; }




        /// <summary>
        /// NeoSpin.busWssMessageDetailGen.FindWssMessageDetail():
        /// Finds a particular record from cdoWssMessageDetail with its primary key. 
        /// </summary>
        /// <param name="aintwssmessageid">A primary key value of type int of cdoWssMessageDetail on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindWssMessageDetail(int aintwssmessageid)
		{
			bool lblnResult = false;
			if (icdoWssMessageDetail == null)
			{
				icdoWssMessageDetail = new cdoWssMessageDetail();
			}
			if (icdoWssMessageDetail.SelectRow(new object[1] { aintwssmessageid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
