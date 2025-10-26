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
    /// Class NeoSpin.BusinessObjects.busMedicarePartDRateRefGen:
    /// Inherited from busBase, used to create new business object for main table cdoMedicarePartDRateRef and its children table. 
    /// </summary>
	[Serializable]
	public class busMedicarePartDRateRefGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busMedicarePartDRateRefGen
        /// </summary>
		public busMedicarePartDRateRefGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busMedicarePartDRateRefGen.
        /// </summary>
		public cdoMedicarePartDRateRef icdoMedicarePartDRateRef { get; set; }




        /// <summary>
        /// NeoSpin.busMedicarePartDRateRefGen.FindMedicarePartDRateRef():
        /// Finds a particular record from cdoMedicarePartDRateRef with its primary key. 
        /// </summary>
        /// <param name="aintMedicarePartDRateRefId">A primary key value of type int of cdoMedicarePartDRateRef on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindMedicarePartDRateRef(int aintMedicarePartDRateRefId)
		{
			bool lblnResult = false;
			if (icdoMedicarePartDRateRef == null)
			{
				icdoMedicarePartDRateRef = new cdoMedicarePartDRateRef();
			}
			if (icdoMedicarePartDRateRef.SelectRow(new object[1] { aintMedicarePartDRateRefId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
