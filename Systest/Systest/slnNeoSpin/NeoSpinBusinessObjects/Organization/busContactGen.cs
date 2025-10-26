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
    public partial class busContact : busExtendBase
	{
		public busContact()
		{

		} 

		private cdoContact _icdoContact;
		public cdoContact icdoContact
		{
			get
			{
				return _icdoContact;
			}

			set
			{
				_icdoContact = value;
			}
		}

        private Collection<busOrgContactAddress> _iclbContactAddress;
        public Collection<busOrgContactAddress> iclbContactAddress
		{
			get
			{
                return _iclbContactAddress;
			}

			set
			{
                _iclbContactAddress = value;
			}
		}

		public bool FindContact(int Aintcontactid)
		{
			bool lblnResult = false;
			if (_icdoContact == null)
			{
				_icdoContact = new cdoContact();
			}
			if (_icdoContact.SelectRow(new object[1] { Aintcontactid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

		public void LoadContactAddress()
		{
			DataTable ldtbList = Select<cdoOrgContactAddress>(
				new string[1] { "contact_id" },
				new object[1] { icdoContact.contact_id }, null, null);
            _iclbContactAddress = GetCollection<busOrgContactAddress>(ldtbList, "icdoOrgContactAddress");
            foreach (busOrgContactAddress lobjTempContactAddress in _iclbContactAddress)
            {
                if (lobjTempContactAddress.icdoOrgContactAddress.contact_org_address_id == _icdoContact.primary_address_id)
                {
                    //Load the Contact Property in Org Contact Address to fill the Primary Address Flag
                    lobjTempContactAddress.LoadContact();

                    lobjTempContactAddress.primary_address_flag = "Y";
                }
                else
                {
                    lobjTempContactAddress.primary_address_flag = "N";
                }
            }
		}
	}
}
