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
	/// Class NeoSpin.BusinessObjects.busUserActivityLogDetail:
	/// Inherited from busUserActivityLogDetailGen, the class is used to customize the business object busUserActivityLogDetailGen.
	/// </summary>
	[Serializable]
	public class busUserActivityLogDetail : busUserActivityLogDetailGen
    {
        #region Public Method

        /// <summary>
        /// Author                  : Framwork Team
        /// Modified By             : Gaurav Patadiya
        /// Applies To Use cases    : Security
        /// Usage                   : Load User Activity Log Queriess
        /// </summary>
        public override void LoadUserActivityLogQueriess()
        {
            DataTable ldtbUserActivityLog = busBase.Select("cdoUserActivityLogQueries.GetQueriesWithParamCount", 
                new object[1] { icdoUserActivityLogDetail.transaction_id.ToString()});
            iclbUserActivityLogQueries = GetCollection<busUserActivityLogQueries>(ldtbUserActivityLog, "icdoUserActivityLogQueries");
        }

        /// <summary>
        ///  iclbFullAuditLog of type busFullAuditLog.
        /// </summary>
        public override void LoadFullAuditLogs()
        {
            DataTable ldtbList = busBase.Select("cdoFullAuditLog.GetFullAuditLogByGuidTransactionId",
                new object[1] { icdoUserActivityLogDetail.transaction_id.ToString() });
            iclbFullAuditLog = GetCollection<busFullAuditLog>(ldtbList, "icdoFullAuditLog");
        }
       #endregion
    }
}
