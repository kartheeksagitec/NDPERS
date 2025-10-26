using System;
using System.Collections.Generic;
using System.Text;
using Sagitec.BusinessObjects;
using Sagitec.CustomDataObjects;
using System.Collections.ObjectModel;
using System.Data;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPERSAuditLookup : busMainBase
    {
        private Collection<busPERSAudit> _iclbLookupResult;
        public Collection<busPERSAudit> iclbLookupResult
        {
            get
            {
                return _iclbLookupResult;
            }

            set
            {
                _iclbLookupResult = value;
            }
        }

        public void LoadSearchResult(DataTable adtbSearchResult)
        {
            _iclbLookupResult = GetCollection<busPERSAudit>(adtbSearchResult, "icdoAuditLog");
        }

        protected override void LoadOtherObjects(System.Data.DataRow adtrRow, busBase aobjBus)
        {
            busPERSAudit lobjPERSAudit = (busPERSAudit)aobjBus;
            lobjPERSAudit.ibusOrganization = new busOrganization();
            lobjPERSAudit.ibusOrganization.icdoOrganization = new NeoSpin.CustomDataObjects.cdoOrganization();
            lobjPERSAudit.ibusOrganization.icdoOrganization.LoadData(adtrRow);
            lobjPERSAudit.LoadPerson(); //PIR 15857 Audit Log
        }
    }

}
