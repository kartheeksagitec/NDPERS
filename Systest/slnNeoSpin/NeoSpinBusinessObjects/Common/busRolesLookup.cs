#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;

#endregion

namespace NeoSpin.BusinessObjects
{
	[Serializable]
	public class busRolesLookup : busMainBase
	{
		private Collection<busRoles> _iclbLookupResult;
		public Collection<busRoles> iclbLookupResult
		{
			get
			{
				return _iclbLookupResult;
			}

			set
			{
				_iclbLookupResult = value;
			}
		}

		public void LoadRoles(DataTable adtbSearchResult)
		{
			_iclbLookupResult = GetCollection<busRoles>(adtbSearchResult, "icdoRoles");
		}
	}
}
