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
    /// Class NeoSpin.BusinessObjects.busProviderReportDataMedicarePartDGen:
    /// Inherited from busBase, used to create new business object for main table cdoProviderReportDataMedicarePartD and its children table. 
    /// </summary>
	[Serializable]
	public class busProviderReportDataMedicarePartDGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busProviderReportDataMedicarePartDGen
        /// </summary>
		public busProviderReportDataMedicarePartDGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busProviderReportDataMedicarePartDGen.
        /// </summary>
        //public cdoProviderReportDataMedicarePartD icdoProviderReportDataMedicarePartD { get; set; }

        private cdoProviderReportDataMedicarePartD _icdoProviderReportDataMedicare;
        public cdoProviderReportDataMedicarePartD icdoProviderReportDataMedicare
        {
            get
            {
                return _icdoProviderReportDataMedicare;
            }
            set
            {
                _icdoProviderReportDataMedicare = value;
            }
        }


        /// <summary>
        /// NeoSpin.busProviderReportDataMedicarePartDGen.FindProviderReportDataMedicarePartD():
        /// Finds a particular record from cdoProviderReportDataMedicarePartD with its primary key. 
        /// </summary>
        /// <param name="aintProviderReportDataMedicarePartDCompId">A primary key value of type int of cdoProviderReportDataMedicarePartD on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
        //public virtual bool FindProviderReportDataMedicarePartD(int aintProviderReportDataMedicarePartDCompId)
        //{
        //    bool lblnResult = false;
        //    if (icdoProviderReportDataMedicarePartD == null)
        //    {
        //        icdoProviderReportDataMedicarePartD = new cdoProviderReportDataMedicarePartD();
        //    }
        //    if (icdoProviderReportDataMedicarePartD.SelectRow(new object[1] { aintProviderReportDataMedicarePartDCompId }))
        //    {
        //        lblnResult = true;
        //    }
        //    return lblnResult;
        //}

	}
}
