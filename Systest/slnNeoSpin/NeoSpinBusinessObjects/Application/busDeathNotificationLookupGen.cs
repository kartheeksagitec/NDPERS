#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using NeoSpin.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;

#endregion

namespace NeoSpin.BusinessObjects
{
	[Serializable]
	public class busDeathNotificationLookupGen : busMainBase
	{

		private Collection<busDeathNotification> _iclbDeathNotification;
        public Collection<busDeathNotification> iclbDeathNotification
		{
			get
			{
                return _iclbDeathNotification;
			}
			set
			{
                _iclbDeathNotification = value;
			}
		}

		public void LoadDeathNotification(DataTable adtbSearchResult)
		{
            _iclbDeathNotification = GetCollection<busDeathNotification>(adtbSearchResult, "icdoDeathNotification");
		}
	}
}
