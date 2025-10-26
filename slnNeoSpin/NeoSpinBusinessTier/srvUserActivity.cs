#region Using directives

using System;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using Sagitec.BusinessObjects;
using Sagitec.CustomDataObjects;
using Sagitec.Common;
using NeoSpin.CustomDataObjects;
using NeoSpin.BusinessObjects;
using Sagitec.DBUtility;
#endregion

namespace NeoSpin.BusinessTier
{
    public class srvUserActivity : srvNeoSpin
    {
        public srvUserActivity()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public busUserActivityLog FindUserActivityLog(int aintUserActivityLogId, int aintUserActivityLogDetailId)
        {
            busUserActivityLog lobjUserActivityLog = new busUserActivityLog();
            if (lobjUserActivityLog.FindUserActivityLog(aintUserActivityLogId))
            {
                lobjUserActivityLog.LoadUserActivityLogDetails();
            }

            return lobjUserActivityLog;
        }

        public busUserActivityLogLookup LoadUserActivityLogs(DataTable adtbSearchResult)
        {
            busUserActivityLogLookup lobjUserActivityLogLookup = new busUserActivityLogLookup();
            lobjUserActivityLogLookup.LoadUserActivityLogs(adtbSearchResult);
            return lobjUserActivityLogLookup;
        }

        public busUserActivityLogDetail FindUserActivityLogDetails(int aintUserActivityLogDetailId, int aintAuditLogCount)
        {
            busUserActivityLogDetail lobjUserActivityLogDetail = new busUserActivityLogDetail();
            if (lobjUserActivityLogDetail.FindUserActivityLogDetail(aintUserActivityLogDetailId))
            {
                if (aintAuditLogCount == -1)
                {
                    object lobjResult = DBFunction.DBExecuteScalar("cdoFullAuditLog.CountByTransactionID", new object[1] { lobjUserActivityLogDetail.icdoUserActivityLogDetail.transaction_id.ToString() },
                        iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                    if (lobjResult != null && lobjResult is int)
                        lobjUserActivityLogDetail.icdoUserActivityLogDetail.audit_count = Convert.ToString(lobjResult);
                }
                else
                {
                    lobjUserActivityLogDetail.icdoUserActivityLogDetail.audit_count = aintAuditLogCount.ToString();
                }
                lobjUserActivityLogDetail.LoadUserActivityLogParameterss();
                lobjUserActivityLogDetail.LoadUserActivityLogQueriess();
                lobjUserActivityLogDetail.iclbFullAuditLog = new Collection<BusinessObjects.busFullAuditLog>();
            }

            return lobjUserActivityLogDetail;
        }

        public busUserActivityLogQueryLookup LoadUserActivityLogQuerys(DataTable adtbSearchResult)
        {
            busUserActivityLogQueryLookup lobjUserActivityLogQueryLookup = new busUserActivityLogQueryLookup();
            lobjUserActivityLogQueryLookup.LoadUserActivityLogQueriess(adtbSearchResult);
            return lobjUserActivityLogQueryLookup;
        }
    }
}
