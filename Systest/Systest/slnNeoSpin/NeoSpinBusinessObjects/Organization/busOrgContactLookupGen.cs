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
	public partial class busOrgContactLookup  : busMainBase
	{

		private Collection<busOrgContact> _icolOrgContact;
		public Collection<busOrgContact> icolOrgContact
		{
			get
			{
				return _icolOrgContact;
			}

			set
			{
				_icolOrgContact = value;
			}
		}

		public void LoadOrgContact(DataTable adtbSearchResult)
		{
			_icolOrgContact = GetCollection<busOrgContact>(adtbSearchResult, "icdoOrgContact");
		}
	}
}
