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
    /// Class NeoSpin.BusinessObjects.busUserChangeHistoryGen:
    /// Inherited from busBase, used to create new business object for main table cdoUserChangeHistory and its children table. 
    /// </summary>
	[Serializable]
	public class busUserChangeHistoryGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busUserChangeHistoryGen
        /// </summary>
		public busUserChangeHistoryGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busUserChangeHistoryGen.
        /// </summary>
		public cdoUserChangeHistory icdoUserChangeHistory { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busUser.
        /// </summary>
		public busUser ibusUser { get; set; }




        /// <summary>
        /// NeoSpin.busUserChangeHistoryGen.FindUserChangeHistory():
        /// Finds a particular record from cdoUserChangeHistory with its primary key. 
        /// </summary>
        /// <param name="aintuserchangeid">A primary key value of type int of cdoUserChangeHistory on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindUserChangeHistory(int aintuserchangeid)
		{
			bool lblnResult = false;
			if (icdoUserChangeHistory == null)
			{
				icdoUserChangeHistory = new cdoUserChangeHistory();
			}
			if (icdoUserChangeHistory.SelectRow(new object[1] { aintuserchangeid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        /// NeoSpin.busUserChangeHistoryGen.LoadUser():
        /// Loads non-collection object ibusUser of type busUser.
        /// </summary>
		public virtual void LoadUser()
		{
			if (ibusUser == null)
			{
				ibusUser = new busUser();
			}
			ibusUser.FindUser(icdoUserChangeHistory.user_serial_id);
		}

	}
}
