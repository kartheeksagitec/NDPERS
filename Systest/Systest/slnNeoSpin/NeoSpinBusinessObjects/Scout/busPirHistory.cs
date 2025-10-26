#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
using System.Data;
using System.Data.Common;


#endregion

namespace NeoSpin.BusinessObjects
{
	[Serializable]
	public class busPirHistory : busExtendBase
    {
		public busPirHistory()
		{
		}

		private cdoPirHistory _icdoPirHistory;
		public cdoPirHistory icdoPirHistory
		{
			get
			{
				return _icdoPirHistory;
			}

			set
			{
				_icdoPirHistory = value;
			}
		}

        private busUser _ibusAssignedTo;
        public busUser ibusAssignedTo
        {
            get
            {
                return _ibusAssignedTo;
            }
            set
            {
                _ibusAssignedTo = value;
            }
        }
        
        public void LoadAssignedTo()
        {
            if (_ibusAssignedTo == null)
            {
                _ibusAssignedTo = new busUser();
            }
            _ibusAssignedTo.FindUser(icdoPirHistory.assigned_to_id);
        }     
	}
}
