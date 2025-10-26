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
	public partial class busContactTicketHistory : busExtendBase
    {        
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
        public void LoadUser()
        {
            if (_ibusUser == null)
            {
                _ibusUser = new busUser();
            }
            _ibusUser.FindUser(this.icdoContactTicketHistory.assign_to_user_id);
        }              
	}
}
