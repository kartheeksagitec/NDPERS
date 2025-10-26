#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using NeoSpin.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;

#endregion

namespace NeoSpin.BusinessObjects
{
	[Serializable]
	public class busRoles : busExtendBase
    {
		public busRoles()
		{
		}

		private string _istrSecurityValue;
		public string istrSecurityValue
		{
			get
			{
				return _istrSecurityValue;
			}

			set
			{
				_istrSecurityValue = value;
			}
		}

		private cdoRoles _icdoRoles;
		public cdoRoles icdoRoles
		{
			get
			{
				return _icdoRoles;
			}

			set
			{
				_icdoRoles = value;
			}
		}

		private Collection<busSecurity> _iclbSecurity;
		public Collection<busSecurity> iclbSecurity
		{
			get
			{
				return _iclbSecurity;
			}

			set
			{
				_iclbSecurity = value;
			}
		}

		public bool FindRole(int aintRoleId)
		{
			bool lblnResult = false;
			if (_icdoRoles == null)
			{
				_icdoRoles = new cdoRoles();
			}
			if (_icdoRoles.SelectRow(new object[1] { aintRoleId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

		public void LoadSecurity()
		{
			DataTable ldtbList =
				Select("cdoSecurity.ByRole", new object[1] { _icdoRoles.role_id });
			_iclbSecurity = GetCollection<busSecurity>(ldtbList, "icdoSecurity");
		}

		protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
		{
			if (aobjBus is busSecurity)
			{
				busSecurity lobjSec = (busSecurity)aobjBus;
				lobjSec.ibusResources = new busResources();
				lobjSec.ibusResources.icdoResources = new cdoResources();
				sqlFunction.LoadQueryResult(lobjSec.ibusResources.icdoResources, adtrRow);
			}
		}

        public override void AfterPersistChanges()
        {
            if ((_iclbSecurity == null) || (_iclbSecurity.Count == 0))
                LoadSecurity();
        }
	}
}
