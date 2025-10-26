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
    /// Class NeoSpin.busWssBenAppDisaWorkHistoryGen:
    /// Inherited from busBase, used to create new business object for main table cdoWssBenAppDisaWorkHistory and its children table. 
    /// </summary>
	[Serializable]
	public class busWssBenAppDisaWorkHistoryGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.busWssBenAppDisaWorkHistoryGen
        /// </summary>
		public busWssBenAppDisaWorkHistoryGen():base()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busWssBenAppDisaWorkHistoryGen.
        /// </summary>
		public cdoWssBenAppDisaWorkHistory icdoWssBenAppDisaWorkHistory { get; set; }




        /// <summary>
        /// NeoSpin.busWssBenAppDisaWorkHistoryGen.FindWssBenAppDisaWorkHistory():
        /// Finds a particular record from cdoWssBenAppDisaWorkHistory with its primary key. 
        /// </summary>
        /// <param name="aintDisaWorkHistoryId">A primary key value of type int of cdoWssBenAppDisaWorkHistory on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindWssBenAppDisaWorkHistory(int aintDisaWorkHistoryId)
		{
			bool lblnResult = false;
			if (icdoWssBenAppDisaWorkHistory == null)
			{
				icdoWssBenAppDisaWorkHistory = new cdoWssBenAppDisaWorkHistory();
			}
			if (icdoWssBenAppDisaWorkHistory.SelectRow(new object[1] { aintDisaWorkHistoryId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
