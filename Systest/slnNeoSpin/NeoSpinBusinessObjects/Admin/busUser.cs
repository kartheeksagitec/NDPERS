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
using System.Linq;
using System.DirectoryServices.AccountManagement;
using NeoSpin.DataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busUser : busExtendBase
    {
        public busUser()
        {
        }

        private cdoUser _icdoUser;
        public cdoUser icdoUser
        {
            get
            {
                return _icdoUser;
            }

            set
            {
                _icdoUser = value;
            }
        }
        private Collection<busUser> _iclbUser;
        public Collection<busUser> iclbUser
        {
            get
            {
                return _iclbUser;
            }

            set
            {
                _iclbUser = value;
            }
        }
        private Collection<busUserRoles> _iclbUserRoles;
        public Collection<busUserRoles> iclbUserRoles
        {
            get
            {
                return _iclbUserRoles;
            }

            set
            {
                _iclbUserRoles = value;
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
        public busUser ibusSupervisor { get; set; }
        public string User_full_name
        {
            
            get
            {
                string lstrFullName = string.Empty;
                if (!String.IsNullOrEmpty(icdoUser.first_name))
                {
                    lstrFullName = icdoUser.first_name;
                }
                if (!String.IsNullOrEmpty(icdoUser.middle_initial))
                {
                    lstrFullName += " " + icdoUser.middle_initial;
                }
                if (!String.IsNullOrEmpty(icdoUser.last_name))
                {
                    lstrFullName += " " + icdoUser.last_name;
                }

                return lstrFullName;
            }
        }

        public bool FindUser(int aintUserSerialId)
        {
            bool lblnResult = false;
            if (_icdoUser == null)
            {
                _icdoUser = new cdoUser();
            }
            if (_icdoUser.SelectRow(new object[1] { aintUserSerialId }))
            {
                lblnResult = true;
            }
            return lblnResult;
        }

        public bool FindUserByUserName(string astrUserName)
        {
            if (icdoUser == null)
            {
                icdoUser = new cdoUser();
            }
            DataTable ldtbUser = Select<cdoUser>(new string[1] { "user_id" },
                  new object[1] { astrUserName }, null, null);
            if (ldtbUser.Rows.Count > 0)
            {
                icdoUser.LoadData(ldtbUser.Rows[0]);
                return true;
            }
            return false;
        }

        public bool FindUser(string astrUserId)
        {
            DataTable ldtbPerson = busBase.Select<cdoUser>(new string[1] { "user_id" },
                new object[1] { astrUserId }, null, null);

            if (ldtbPerson.Rows.Count > 0)
            {
                this.icdoUser = new cdoUser();
                this.icdoUser.LoadData(ldtbPerson.Rows[0]);
                return true;
            }

            return false;
        }

        public void LoadUserRoles()
        {
            DataTable ldtbList = Select("cdoUserRoles.ByUser", new object[1] { _icdoUser.user_serial_id });
            _iclbUserRoles = GetCollection<busUserRoles>(ldtbList, "icdoUserRoles");
        }

        public void LoadSupervisor()
        {
            if (icdoUser.supervisor_id > 0)
            {
                if (this.ibusSupervisor == null || this.ibusSupervisor.icdoUser == null)
                {
                    this.ibusSupervisor = new busUser();
                }

                this.ibusSupervisor.FindByPrimaryKey(this.icdoUser.supervisor_id);

                // LoadObject("ibusSupervisor", enmUser.supervisor_id.ToString());
            }
        }
        public void LoadSecurity()
        {
            DataTable ldtbList =
                Select("cdoSecurity.ByUser", new object[1] { _icdoUser.user_serial_id });
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
            else if (aobjBus is busUserRoles)
            {
                busUserRoles lobjUR = (busUserRoles)aobjBus;
                lobjUR.icdoRoles = new cdoRoles();
                lobjUR.icdoRoles.role_description = adtrRow["role_description"].ToString();
            }
        }

        public bool ValidateAgainstADS()
        {
            bool iblnIsAuthenticated = false;
            //F/W Upgrade PIR 21603 - CheckUserInAD Method 
            //Implementation changed, it will not check whether a userID in active directory exists or not.
            //utlUserInfo lobjUserInfo = iobjPassInfo.isrvDBCache.CheckUserInAD(icdoUser.user_id); 
            using (var ctx = new PrincipalContext(ContextType.Domain))
            {
                using (var user = UserPrincipal.FindByIdentity(ctx, icdoUser.user_id))
                {
                    if (user != null)
                    {
                        iblnIsAuthenticated = true;
                        if (!string.IsNullOrEmpty(user.GivenName))
                            icdoUser.first_name = user.GivenName;
                        if (!string.IsNullOrEmpty(user.Surname))
                            icdoUser.last_name = user.Surname;
                    }
                }
            }
            return iblnIsAuthenticated;
        }

        public override void BeforePersistChanges()
        {
            //Fire this to obtain latest name info from active directory
            ValidateAgainstADS();
            if (_icdoUser.email_address == null)
            {
                _icdoUser.email_address = _icdoUser.user_id + "@nd.gov";
            }
        }
        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            LoadSupervisor();
        }

        //validation to check whether end date and begin date combination
        public bool EndDateBeforeBeginDate()
        {
            if ((icdoUser.end_date != DateTime.MinValue) && (icdoUser.end_date < icdoUser.begin_date))
            {
                return true;
            }
            return false;
        }
        /* To Check whether User Id is already exists*/
        public bool CheckDuplicateUserID()
        {
            DataTable ldtbList = Select<cdoUser>(new string[1] { "user_id" },
                                                 new object[1] { icdoUser.user_id }, null, null);
            iclbUser = GetCollection<busUser>(ldtbList, "icdoUser");
            string lstrUserID = _icdoUser.user_id;
            foreach (busUser lobjUser in iclbUser)
            {
                if (ldtbList.Rows[0]["user_id"].ToString() != icdoUser.user_id)
                {
                    if (lobjUser.icdoUser.user_id == lstrUserID)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool ValidateEmail()
        {
            if (!String.IsNullOrEmpty(_icdoUser.email_address))
            {
                return busGlobalFunctions.IsEmailValid(_icdoUser.email_address); //PIR-18492
            }
            return true;
        }

        public bool IsMemberActiveInRole(int aintRoleID, DateTime adtEffectiveDate)
        {
            if (iclbUserRoles == null)
                LoadUserRoles();

            busUserRoles lbusUserRole = iclbUserRoles.FirstOrDefault(i => i.icdoUserRoles.role_id == aintRoleID);
            if (lbusUserRole != null)
            {
                if (busGlobalFunctions.CheckDateOverlapping(adtEffectiveDate, lbusUserRole.icdoUserRoles.effective_start_date, lbusUserRole.icdoUserRoles.effective_end_date))
                {
                    return true;
                }
            }

            return false;
        }

    }
}
