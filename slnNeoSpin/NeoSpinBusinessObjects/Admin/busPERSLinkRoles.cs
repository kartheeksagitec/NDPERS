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
    public class busPERSLinkRoles : busRoles
    {
        public busPERSLinkRoles()
        {
        }

        private Collection<busUser> _iclbUsers;
        public Collection<busUser> iclbUsers
        {
            get
            {
                return _iclbUsers;
            }

            set
            {
                _iclbUsers = value;
            }
        }

        public void LoadUsers()
        {
            DataTable ldtbList = Select("cdoUser.ListOfUsersByRole", new object[1] { icdoRoles.role_id });
            _iclbUsers = GetCollection<busUser>(ldtbList, "icdoUser");
        }
    }
}
