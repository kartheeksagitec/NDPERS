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
    /// Class NeoSpin.BusinessObjects.busPayment1099rGen:
    /// Inherited from busBase, used to create new business object for main table cdoPayment1099r and its children table. 
    /// </summary>
	[Serializable]
	public class busPayment1099rGen : busExtendBase
	{
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busPayment1099rGen
        /// </summary>
		public busPayment1099rGen()
		{
           
		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPayment1099rGen.
        /// </summary>
		public cdoPayment1099r icdoPayment1099r { get; set; }

        public busPayeeAccount ibusPayeeAccount { get; set; }
        


        /// <summary>
        /// NeoSpin.busPayment1099rGen.FindPayment1099r():
        /// Finds a particular record from cdoPayment1099r with its primary key. 
        /// </summary>
        /// <param name="aintpayment1099rid">A primary key value of type int of cdoPayment1099r on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPayment1099r(int aintpayment1099rid)
		{
			bool lblnResult = false;
			if (icdoPayment1099r == null)
			{
				icdoPayment1099r = new cdoPayment1099r();
			}
			if (icdoPayment1099r.SelectRow(new object[1] { aintpayment1099rid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
