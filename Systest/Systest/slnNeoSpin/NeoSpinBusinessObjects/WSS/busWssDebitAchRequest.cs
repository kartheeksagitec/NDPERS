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
    /// Class NeoSpin.BusinessObjects.busWssDebitAchRequest:
    /// Inherited from busWssDebitAchRequestGen, the class is used to customize the business object busWssDebitAchRequestGen.
    /// </summary>
    [Serializable]
    public class busWssDebitAchRequest : busWssDebitAchRequestGen
    {
        /// <summary>
        /// Method to create debit ach request details with deposit ids
        /// </summary>
        /// <param name="aintDepositId"></param>
        public void CreateDebitACHRequestDetails(int aintDepositId)
        {
            busWssDebitAchRequestDetail lobjDetail = new busWssDebitAchRequestDetail { icdoWssDebitAchRequestDetail = new cdoWssDebitAchRequestDetail() };

            lobjDetail.icdoWssDebitAchRequestDetail.debit_ach_request_id = icdoWssDebitAchRequest.debit_ach_request_id;
            lobjDetail.icdoWssDebitAchRequestDetail.deposit_id = aintDepositId;

            lobjDetail.icdoWssDebitAchRequestDetail.Insert();
        }

        public Collection<busWssDebitAchRequestDetail> iclbDebitAchRequestDetail { get; set; }
        public void LoadDebitACHRequestDetail()
        {
            DataTable ldtbACHRequestDetail = Select<cdoWssDebitAchRequestDetail>(new string[1] { "debit_ach_request_id" },
                new object[1] { icdoWssDebitAchRequest.debit_ach_request_id }, null, null);
            iclbDebitAchRequestDetail = GetCollection<busWssDebitAchRequestDetail>(ldtbACHRequestDetail, "icdoWssDebitAchRequestDetail");
        }
    }
}