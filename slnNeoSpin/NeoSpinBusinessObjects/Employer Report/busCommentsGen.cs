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
using NeoSpin.BusinessObjects;

#endregion

namespace NeoSpin
{
    /// <summary>
    /// Class NeoSpin.busCommentsGen:
    /// Inherited from busBase, used to create new business object for main table cdoComments and its children table. 
    /// </summary>
	[Serializable]
	public class busCommentsGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.busCommentsGen
        /// </summary>
		public busCommentsGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busCommentsGen.
        /// </summary>
		public cdoComments icdoComments { get; set; }




        /// <summary>
        /// NeoSpin.busCommentsGen.FindComments():
        /// Finds a particular record from cdoComments with its primary key. 
        /// </summary>
        /// <param name="aintCommentId">A primary key value of type int of cdoComments on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindComments(int aintCommentId)
		{
			bool lblnResult = false;
			if (icdoComments == null)
			{
				icdoComments = new cdoComments();
			}
			if (icdoComments.SelectRow(new object[1] { aintCommentId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
