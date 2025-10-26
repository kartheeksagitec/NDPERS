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
    public partial class busPersonContact : busExtendBase
	{
		public busPersonContact()
		{

		} 

		private cdoPersonContact _icdoPersonContact;
		public cdoPersonContact icdoPersonContact
		{
			get
			{
				return _icdoPersonContact;
			}

			set
			{
				_icdoPersonContact = value;
			}
		}

		public bool FindPersonContact(int Aintpersoncontactid)
		{
			bool lblnResult = false;
			if (_icdoPersonContact == null)
			{
				_icdoPersonContact = new cdoPersonContact();
			}
			if (_icdoPersonContact.SelectRow(new object[1] { Aintpersoncontactid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
