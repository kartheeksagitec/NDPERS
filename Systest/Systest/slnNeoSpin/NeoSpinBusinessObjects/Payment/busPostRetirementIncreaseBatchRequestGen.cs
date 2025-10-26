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
    /// Class .busPostRetirementIncreaseBatchRequestGen:
    /// Inherited from busBase, used to create new business object for main table cdoPostRetirementIncreaseBatchRequest and its children table. 
    /// </summary>
	[Serializable]
	public class busPostRetirementIncreaseBatchRequestGen : busExtendBase
	{
        /// <summary>
        /// Constructor for .busPostRetirementIncreaseBatchRequestGen
        /// </summary>
		public busPostRetirementIncreaseBatchRequestGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPostRetirementIncreaseBatchRequestGen.
        /// </summary>
		public cdoPostRetirementIncreaseBatchRequest icdoPostRetirementIncreaseBatchRequest { get; set; }

        /// <summary>
        /// .busPostRetirementIncreaseBatchRequestGen.FindPostRetirementIncreaseBatchRequest():
        /// Finds a particular record from cdoPostRetirementIncreaseBatchRequest with its primary key. 
        /// </summary>
        /// <param name="aintpostretirementincreasebatchrequestid">A primary key value of type int of cdoPostRetirementIncreaseBatchRequest on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPostRetirementIncreaseBatchRequest(int aintpostretirementincreasebatchrequestid)
		{
			bool lblnResult = false;
			if (icdoPostRetirementIncreaseBatchRequest == null)
			{
				icdoPostRetirementIncreaseBatchRequest = new cdoPostRetirementIncreaseBatchRequest();
			}
			if (icdoPostRetirementIncreaseBatchRequest.SelectRow(new object[1] { aintpostretirementincreasebatchrequestid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        public Collection<cdoPostRetirementIncreaseBatchRequestDetail> iclbPorRetirementIncreaseBatchRequestDetail { get; set; }

        public void LoadPostRetirementIncreaseBatchRequestDetail()
        {
            DataTable ldtbList = Select<cdoPostRetirementIncreaseBatchRequestDetail>(new string[1] { "POST_RETIREMENT_INCREASE_BATCH_REQUEST_ID" }, 
                new object[1]{icdoPostRetirementIncreaseBatchRequest.post_retirement_increase_batch_request_id},null,null);
            iclbPorRetirementIncreaseBatchRequestDetail = cdoPostRetirementIncreaseBatchRequestDetail.GetCollection<cdoPostRetirementIncreaseBatchRequestDetail>(ldtbList);
        }

        //this is used only for batch correspondence
        public busPayeeAccount ibusPayeeAccount { get; set; }
	}
}
