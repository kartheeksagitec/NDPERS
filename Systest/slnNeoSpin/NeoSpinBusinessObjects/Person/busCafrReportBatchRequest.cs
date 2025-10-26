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
using System.Linq;
using System.Linq.Expressions;

#endregion

namespace NeoSpin.BusinessObjects
{
	/// <summary>
	/// Class NeoSpin.BusinessObjects.busCafrReportBatchRequest:
	/// Inherited from busCafrReportBatchRequestGen, the class is used to customize the business object busCafrReportBatchRequestGen.
	/// </summary>
	[Serializable]
	public class busCafrReportBatchRequest : busCafrReportBatchRequestGen
	{
        /// <summary>
        /// Method to check whether same CAFR report request exists
        /// </summary>
        /// <returns>boolean value</returns>
        public bool IsSameReportRequestExists()
        {
            bool lblnResult = false;
            DataTable ldtResult = SelectWithOperator<cdoCafrReportBatchRequest>(new string[1] { "status_value" },new string[1]{"="},
                 new object[1] { busConstant.CAFRReportStatusPending }, null);
            Collection<busCafrReportBatchRequest> lclbCAFRReportRequest = GetCollection<busCafrReportBatchRequest>(ldtResult,
                "icdoCafrReportBatchRequest");
            busCafrReportBatchRequest lobjCafr = lclbCAFRReportRequest.Where
                (o => o.icdoCafrReportBatchRequest.effective_date.Month == icdoCafrReportBatchRequest.effective_date.Month &&
                    o.icdoCafrReportBatchRequest.effective_date.Year == icdoCafrReportBatchRequest.effective_date.Year).FirstOrDefault();
            if (lobjCafr != null)
                lblnResult = true;                                                                    
            return lblnResult;            
        }

        /// <summary>
        /// Method to check whether Effective date is valid (greater than or equal to today's date and should be last day of the month)
        /// </summary>
        /// <returns>boolean value</returns>
        public bool IsEffectiveDateNotValid()
        {
            bool lblnResult = false;
            if (icdoCafrReportBatchRequest.effective_date < DateTime.Today || 
                icdoCafrReportBatchRequest.effective_date.GetFirstDayofNextMonth() != icdoCafrReportBatchRequest.effective_date.AddDays(1))
                lblnResult = true;
            return lblnResult;
        }

        public override void BeforePersistChanges()
        {
            //setting the default status of batch request to pending
            icdoCafrReportBatchRequest.status_value = busConstant.CAFRReportStatusPending;
            base.BeforePersistChanges();
        }
	}
}
