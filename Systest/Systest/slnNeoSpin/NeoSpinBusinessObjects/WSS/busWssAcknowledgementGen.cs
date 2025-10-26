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
    /// Class NeoSpin.BusinessObjects.busWssAcknowledgementGen:
    /// Inherited from busBase, used to create new business object for main table cdoWssAcknowledgement and its children table. 
    /// </summary>
	[Serializable]
	public class busWssAcknowledgementGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busWssAcknowledgementGen
        /// </summary>
		public busWssAcknowledgementGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busWssAcknowledgementGen.
        /// </summary>
		public cdoWssAcknowledgement icdoWssAcknowledgement { get; set; }




        /// <summary>
        /// NeoSpin.busWssAcknowledgementGen.FindWssAcknowledgement():
        /// Finds a particular record from cdoWssAcknowledgement with its primary key. 
        /// </summary>
        /// <param name="aintacknowledgementid">A primary key value of type int of cdoWssAcknowledgement on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindWssAcknowledgement(int aintacknowledgementid)
		{
			bool lblnResult = false;
			if (icdoWssAcknowledgement == null)
			{
				icdoWssAcknowledgement = new cdoWssAcknowledgement();
			}
			if (icdoWssAcknowledgement.SelectRow(new object[1] { aintacknowledgementid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
