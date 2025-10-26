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
	public class busDeathNotificationGen : busExtendBase
	{
		public busDeathNotificationGen()
		{

		}

        private string _istrRealtionship;

        public string istrRealtionship
        {
            get { return _istrRealtionship; }
            set { _istrRealtionship = value; }
        }
	

		private cdoDeathNotification _icdoDeathNotification;
		public cdoDeathNotification icdoDeathNotification
		{
			get
			{
				return _icdoDeathNotification;
			}
			set
			{
				_icdoDeathNotification = value;
			}
		}
        public bool iblnIsPersonDependentAddressInValid;
        public bool iblnIsPersonBeneficiaryAddressInValid;
        private busPerson _ibusPerson;
		public busPerson ibusPerson
		{
			get
			{
				return _ibusPerson;
			}
			set
			{
				_ibusPerson = value;
			}
		}

		public bool FindDeathNotification(int Aintdeathnotificationid)
		{
			bool lblnResult = false;
			if (_icdoDeathNotification == null)
			{
				_icdoDeathNotification = new cdoDeathNotification();
			}
			if (_icdoDeathNotification.SelectRow(new object[1] { Aintdeathnotificationid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

		public void LoadPerson()
		{
			if (_ibusPerson == null)
			{
				_ibusPerson = new busPerson();
			}
			
			_ibusPerson.FindPerson(_icdoDeathNotification.person_id);
		}

	}
}
