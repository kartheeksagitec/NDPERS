#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using NeoSpin.Common;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class NeoSpin.BusinessObjects.busActivityInstanceGen:
    /// Inherited from busBase, used to create new business object for main table cdoActivityInstance and its children table. 
    /// </summary>
	[Serializable]
	public class busActivityInstanceGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busActivityInstanceGen
        /// </summary>
		public busActivityInstanceGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busActivityInstanceGen.
        /// </summary>
		public cdoActivityInstance icdoActivityInstance { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busActivity.
        /// </summary>
		public busActivity ibusActivity { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busProcessInstance.
        /// </summary>
		public busProcessInstance ibusProcessInstance { get; set; }




        /// <summary>
        /// NeoSpin.busActivityInstanceGen.FindActivityInstance():
        /// Finds a particular record from cdoActivityInstance with its primary key. 
        /// </summary>
        /// <param name="aintactivityinstanceid">A primary key value of type int of cdoActivityInstance on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindActivityInstance(int aintactivityinstanceid)
		{
			bool lblnResult = false;
			if (icdoActivityInstance == null)
			{
				icdoActivityInstance = new cdoActivityInstance();
			}
			if (icdoActivityInstance.SelectRow(new object[1] { aintactivityinstanceid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        /// NeoSpin.busActivityInstanceGen.LoadActivity():
        /// Loads non-collection object ibusActivity of type busActivity.
        /// </summary>
		public virtual void LoadActivity()
		{
			if (ibusActivity == null)
			{
				ibusActivity = new busActivity();
			}
			ibusActivity.FindActivity(icdoActivityInstance.activity_id);
		}

        /// <summary>
        /// NeoSpin.busActivityInstanceGen.LoadProcessInstance():
        /// Loads non-collection object ibusProcessInstance of type busProcessInstance.
        /// </summary>
		public virtual void LoadProcessInstance()
		{
			if (ibusProcessInstance == null)
			{
				ibusProcessInstance = new busProcessInstance();
			}
			ibusProcessInstance.FindProcessInstance(icdoActivityInstance.process_instance_id);
		}

	}
}
