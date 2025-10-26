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
using NeoSpin.DataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class NeoSpin.BusinessObjects.busPaymentStepRefGen:
    /// Inherited from busBase, used to create new business object for main table cdoPaymentStepRef and its children table. 
    /// </summary>
	[Serializable]
	public class busPaymentStepRefGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busPaymentStepRefGen
        /// </summary>
		public busPaymentStepRefGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPaymentStepRefGen.
        /// </summary>
		public cdoPaymentStepRef icdoPaymentStepRef { get; set; }




        /// <summary>
        /// NeoSpin.BusinessObjects.busPaymentStepRefGen.FindPaymentStepRef():
        /// Finds a particular record from cdoPaymentStepRef with its primary key. 
        /// </summary>
        /// <param name="aintpaymentstepid">A primary key value of type int of cdoPaymentStepRef on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPaymentStepRef(int aintpaymentstepid)
		{
			bool lblnResult = false;
			if (icdoPaymentStepRef==null)
			{
				icdoPaymentStepRef = new cdoPaymentStepRef();
			}
			if (icdoPaymentStepRef.SelectRow(new object[1] { aintpaymentstepid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
