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
	public partial class busOrgBank : busExtendBase
    {
		public busOrgBank()
		{

		} 

		private cdoOrgBank _icdoOrgBank;
		public cdoOrgBank icdoOrgBank
		{
			get
			{
				return _icdoOrgBank;
			}

			set
			{
				_icdoOrgBank = value;
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

		public bool FindOrgBank(int Aintorgbankid)
		{
			bool lblnResult = false;
			if (_icdoOrgBank == null)
			{
				_icdoOrgBank = new cdoOrgBank();
			}
			if (_icdoOrgBank.SelectRow(new object[1] { Aintorgbankid }))
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
			_ibusOrganization.FindOrganization(_icdoOrgBank.org_id);
		}

	}
}
