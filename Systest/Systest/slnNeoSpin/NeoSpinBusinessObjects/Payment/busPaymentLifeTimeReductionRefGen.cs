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
    /// Class NeoSpin.BusinessObjects.busPaymentLifeTimeReductionRefGen:
    /// Inherited from busBase, used to create new business object for main table cdoPaymentLifeTimeReductionRef and its children table. 
    /// </summary>
	[Serializable]
	public class busPaymentLifeTimeReductionRefGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busPaymentLifeTimeReductionRefGen
        /// </summary>
		public busPaymentLifeTimeReductionRefGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPaymentLifeTimeReductionRefGen.
        /// </summary>
		public cdoPaymentLifeTimeReductionRef icdoPaymentLifeTimeReductionRef { get; set; }




        /// <summary>
        /// NeoSpin.busPaymentLifeTimeReductionRefGen.FindPaymentLifeTimeReductionRef():
        /// Finds a particular record from cdoPaymentLifeTimeReductionRef with its primary key. 
        /// </summary>
        /// <param name="aintlifetimereductionrefid">A primary key value of type int of cdoPaymentLifeTimeReductionRef on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPaymentLifeTimeReductionRef(int aintlifetimereductionrefid)
		{
			bool lblnResult = false;
			if (icdoPaymentLifeTimeReductionRef == null)
			{
				icdoPaymentLifeTimeReductionRef = new cdoPaymentLifeTimeReductionRef();
			}
			if (icdoPaymentLifeTimeReductionRef.SelectRow(new object[1] { aintlifetimereductionrefid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
