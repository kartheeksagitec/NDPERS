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
	/// Class NeoSpin.BusinessObjects.busPayment1099rRequest:
	/// Inherited from busPayment1099rRequestGen, the class is used to customize the business object busPayment1099rRequestGen.
	/// </summary>
	[Serializable]
	public class busPayment1099rRequest : busPayment1099rRequestGen
	{
        /// <summary>
        /// method called when new button is clicke
        /// </summary>
        /// <returns></returns>
        public ArrayList btn_New_Click()
        {
            ArrayList larrReturn = new ArrayList();
            utlError lobjError = new utlError();
            this.ValidateHardErrors(utlPageMode.All);
            if (iarrErrors.Count <= 0)
            {
                icdoPayment1099rRequest.status_value = busConstant.BatchRequest1099rStatusApproved;
                icdoPayment1099rRequest.Update();
                icdoPayment1099rRequest.status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(3101, busConstant.BatchRequest1099rStatusApproved);

                this.EvaluateInitialLoadRules();
                larrReturn.Add(this);
            }
            else
            {
                foreach (utlError lobjErr in iarrErrors)
                    larrReturn.Add(lobjErr);
            }
            return larrReturn;
        }

        /// <summary>
        /// method called when cancel button is clicke
        /// </summary>
        /// <returns></returns>
        public ArrayList btn_Cancel_Click()
        {
            ArrayList larrReturn = new ArrayList();
            utlError lobjError = new utlError();
            this.ValidateHardErrors(utlPageMode.All);
            if (iarrErrors.Count <= 0)
            {
                icdoPayment1099rRequest.status_value = busConstant.BatchRequest1099rStatusCancelled;
                icdoPayment1099rRequest.Update();
                icdoPayment1099rRequest.status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(3101, busConstant.BatchRequest1099rStatusCancelled);

                this.EvaluateInitialLoadRules();
                larrReturn.Add(this);
            }
            else
            {
                foreach (utlError lobjErr in iarrErrors)
                    larrReturn.Add(lobjErr);
            }
            return larrReturn;
        }

        public override void BeforePersistChanges()
        {
            if (icdoPayment1099rRequest.ienuObjectState == ObjectState.Insert)
            {
                //by default only annual batch requests are created
                if (icdoPayment1099rRequest.request_type_value != busConstant.Monthly1009rRequestBatch)
                    icdoPayment1099rRequest.request_type_value = busConstant.BatchRequest1099rTypeAnnual;
                else
                    icdoPayment1099rRequest.request_type_value = busConstant.Monthly1009rRequestBatch;
                //setting status to Pending
                icdoPayment1099rRequest.status_value = busConstant.BatchRequest1099rStatusPending;
            }
            base.BeforePersistChanges();
        }

        /// <summary>
        /// Method to check whether Pending or Approve 1099r request is available for entered Tax year
        /// </summary>
        /// <returns></returns>
        public bool IsAnnual1099rBatchRequestNotValid()
        {
            bool lblnResult = false;
            DataTable ldtPending1099rRequests = Select<cdoPayment1099rRequest>
                (new string[1] { enmPayment1099rRequest.tax_year.ToString() },
                new object[1] { icdoPayment1099rRequest.tax_year }, null, null);
            DataTable ldt1099rRequest = ldtPending1099rRequests.AsEnumerable()
                                                                .Where(o => (o.Field<string>("status_value") == busConstant.BatchRequest1099rStatusApproved ||
                                                                        o.Field<string>("status_value") == busConstant.BatchRequest1099rStatusPending) &&
                                                                        o.Field<int>("request_id") != icdoPayment1099rRequest.request_id &&
                                                                        o.Field<string>("request_type_value") == busConstant.BatchRequest1099rTypeAnnual)
                                                                .AsDataTable();
            if (ldt1099rRequest.Rows.Count > 0)
                lblnResult = true;
            return lblnResult;
        }

        //PIR-8946 Start
        /// <summary>
        /// Method to check whether Pending or Approve 1099r request is available for entered Tax year and Tax month 
        /// </summary>
        /// <returns></returns>
        public bool IsMonthly1099rBatchRequestNotValid()
        {
            bool lblnResult = false;
            DataTable ldtPending1099rRequests = Select<cdoPayment1099rRequest>
                (new string[2] { enmPayment1099rRequest.tax_year.ToString(), enmPayment1099rRequest.tax_month.ToString()},
                new object[2] { icdoPayment1099rRequest.tax_year, icdoPayment1099rRequest.tax_month }, null, null);
            DataTable ldt1099rRequest = ldtPending1099rRequests.AsEnumerable()
                                                                .Where(o => (o.Field<string>("status_value") == busConstant.BatchRequest1099rStatusApproved ||
                                                                        o.Field<string>("status_value") == busConstant.BatchRequest1099rStatusPending) &&
                                                                        o.Field<int>("request_id") != icdoPayment1099rRequest.request_id)
                                                                .AsDataTable();
            if (ldt1099rRequest.Rows.Count > 0)
                lblnResult = true;
            return lblnResult;
        }
        //PIR-8946 ENd        
	}
}
