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
	public class busSecurity : busExtendBase
    {
		public busSecurity()
		{
		}

		private busResources _ibusResources;
		public busResources ibusResources
		{
			get
			{
				return _ibusResources;
			}

			set
			{
				_ibusResources = value;
			}
		}

		private busRoles _ibusRoles;
		public busRoles ibusRoles
		{
			get
			{
				return _ibusRoles;
			}

			set
			{
				_ibusRoles = value;
			}
		}

		private cdoSecurity _icdoSecurity;
		public cdoSecurity icdoSecurity
		{
			get
			{
				return _icdoSecurity;
			}

			set
			{
				_icdoSecurity = value;
			}
		}

		public bool FindSecurity(int aintResourceId, int aintRoleId)
		{
			bool lblnResult = false;
			if (_icdoSecurity == null)
			{
				_icdoSecurity = new cdoSecurity();
			}
			if (_icdoSecurity.SelectRow(new object[2] { aintResourceId, aintRoleId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

		public void LoadResource()
		{
			if (_ibusResources == null)
			{
				_ibusResources = new busResources();
			}
			_ibusResources.FindResource(_icdoSecurity.resource_id);
		}

		public void LoadRole()
		{
			if (_ibusRoles == null)
			{
				_ibusRoles = new busRoles();
			}
			_ibusRoles.FindRole(_icdoSecurity.role_id);
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

	}
}
