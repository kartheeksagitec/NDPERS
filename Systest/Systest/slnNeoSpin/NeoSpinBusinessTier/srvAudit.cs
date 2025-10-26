#region Using directives

using System;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using NeoSpin.BusinessObjects;
#endregion

namespace NeoSpin.BusinessTier
{
    public class srvAudit : srvNeoSpin
    {
        public srvAudit()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        //public busAudit FindAuditLog(int aintAuditLogID)
        //{
        //    busAudit lobjAudit = new busAudit();
        //    if (lobjAudit.FindAuditLog(aintAuditLogID))
        //    {
        //        lobjAudit.LoadAuditLogDetail();
        //    }
        //    return lobjAudit;
        //}

        //PIR 15857 Audit Log
        public busPERSAuditLookup LoadSearchResult(DataTable adtbSearchResult)
        {
            busPERSAuditLookup lobjAuditLookup = new busPERSAuditLookup();
            lobjAuditLookup.LoadSearchResult(adtbSearchResult);
            return lobjAuditLookup;
        }
        
        //public busAuditLookup GetAuditDetails(int aintPrimaryKey, string astrFormName)
        //{
        //    busAuditLookup lobjAuditLookup = new busAuditLookup();
        //    lobjAuditLookup.GetAuditDetails(aintPrimaryKey, astrFormName);
        //    return lobjAuditLookup;
        //}
    }
}
