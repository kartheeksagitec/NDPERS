using System;
using System.Collections.Generic;
using System.Text;
using Sagitec.BusinessObjects;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPERSAudit : busAuditLog
    {
        public int iintOrgId { get; set; }
        public string istrOrgName { get; set; }
        public string istrOrgCode { get; set; }
        public busPerson ibusPerson { get; set; } //PIR 15857 Audit Log
        private busOrganization _ibusOrganization;
        public busOrganization ibusOrganization
        {
            get
            {
                return _ibusOrganization;
            }

            set
            {
                _ibusOrganization = value;
            }
        }

        public void LoadOrganization()
        {
            if (_ibusOrganization == null)
            {
                _ibusOrganization = new busOrganization();
            }
            _ibusOrganization.FindOrganization(icdoAuditLog.org_id);
        }
        //PIR 15857 Audit Log
        public void LoadPerson()
        {
            if (ibusPerson == null) ibusPerson = new busPerson();
            ibusPerson.FindPerson(icdoAuditLog.person_id);
        }
    }
}
