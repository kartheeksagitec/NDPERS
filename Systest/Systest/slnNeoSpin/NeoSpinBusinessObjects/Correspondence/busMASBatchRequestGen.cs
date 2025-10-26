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
    /// Class NeoSpin.BusinessObjects.busMASBatchRequestGen:
    /// Inherited from busBase, used to create new business object for main table cdoMasBatchRequest and its children table. 
    /// </summary>
	[Serializable]
	public class busMASBatchRequestGen : busMAS
	{
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busMASBatchRequestGen
        /// </summary>
		public busMASBatchRequestGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busMASBatchRequestGen.
        /// </summary>
		public cdoMasBatchRequest icdoMasBatchRequest { get; set; }




        /// <summary>
        /// NeoSpin.busMASBatchRequestGen.FindMASBatchRequest():
        /// Finds a particular record from cdoMasBatchRequest with its primary key. 
        /// </summary>
        /// <param name="aintMASBatchRequestID">A primary key value of type int of cdoMasBatchRequest on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
        public virtual bool FindMASBatchRequest(int aintMASBatchRequestID)
		{
			bool lblnResult = false;
			if (icdoMasBatchRequest == null)
			{
				icdoMasBatchRequest = new cdoMasBatchRequest();
			}
            if (icdoMasBatchRequest.SelectRow(new object[1] { aintMASBatchRequestID }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
