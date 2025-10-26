#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using NeoSpin.CustomDataObjects;
using Sagitec.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;

#endregion

namespace NeoSpin.BusinessObjects
{
	[Serializable]
	public class busUserRoles : busExtendBase
    {
		public busUserRoles()
		{
		}

		private cdoUserRoles _icdoUserRoles;
		public cdoUserRoles icdoUserRoles
		{
			get
			{
				return _icdoUserRoles;
			}

			set
			{
				_icdoUserRoles = value;
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

		private busUser _ibusUser;
		public busUser ibusUser
		{
			get
			{
				return _ibusUser;
			}

			set
			{
				_ibusUser = value;
			}
		}

		public bool FindUserRoles(int aintUserSerialId, int aintRoleId)
		{
			bool lblnResult = false;
			if (_icdoUserRoles == null)
			{
				_icdoUserRoles = new cdoUserRoles();
			}
			if (_icdoUserRoles.SelectRow(new object[2] { aintUserSerialId, aintRoleId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

		public void LoadUser()
		{
			if (_ibusUser == null)
			{
				_ibusUser = new busUser();
			}
			_ibusUser.FindUser(_icdoUserRoles.user_serial_id);
		}

        public void LoadRoles()
        {
            if (icdoRoles == null)
            {
                icdoRoles = new cdoRoles();
                icdoRoles.SelectRow(new object[1] { icdoUserRoles.role_id });
            }
        }

        public override void AddToResponse(utlResponseData aobjResponseData)
        {
            if (icdoUserRoles.role_id > 0)
            {
                aobjResponseData.KeysData.Add("PrimaryKey", icdoUserRoles.role_id.ToString());
            }
            base.AddToResponse(aobjResponseData);
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            LoadRoles();
        }
        ////checking the user status 
        //public bool CheckStatus()
        //{
        //    DataTable ldtbList = busNeoSpinBase.Select("cdoUserRoles.GetUserWithActiveStatus", new object[1] { _icdoUserRoles.user_serial_id });
        //    if (ldtbList.Rows.Count >= 1)
        //    {
        //        return true;
        //    }
        //    return false;

        //}     
       
	}
}
