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
    [Serializable]
    public partial class busOrgContact : busExtendBase
    {
        public busOrgContact()
        {

        }

        private cdoOrgContact _icdoOrgContact;
        public cdoOrgContact icdoOrgContact
        {
            get
            {
                return _icdoOrgContact;
            }

            set
            {
                _icdoOrgContact = value;
            }
        }

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

        private busContact _ibusContact;
        public busContact ibusContact
        {
            get { return _ibusContact; }
            set { _ibusContact = value; }
        }

        public bool FindOrgContact(int Aintorgcontactid)
        {
            bool lblnResult = false;
            if (_icdoOrgContact == null)
            {
                _icdoOrgContact = new cdoOrgContact();
            }
            if (_icdoOrgContact.SelectRow(new object[1] { Aintorgcontactid }))
            {
                lblnResult = true;
            }
            return lblnResult;
        }

        public void LoadOrganization()
        {
            if (_ibusOrganization == null)
            {
                _ibusOrganization = new busOrganization();
            }            
            _ibusOrganization.FindOrganization(_icdoOrgContact.org_id);
        }
        public void LoadContact()
        {
            if (_ibusContact == null)
            {
                _ibusContact = new busContact();
            }            
            _ibusContact.FindContact(_icdoOrgContact.contact_id);
        }

    }
}
