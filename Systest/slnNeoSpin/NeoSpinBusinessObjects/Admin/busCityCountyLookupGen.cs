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
	public class busCityCountyLookupGen : busMainBase
	{

		private Collection<busCountyRef> _icolCountyRef;
		public Collection<busCountyRef> icolCountyRef
		{
			get
			{
				return _icolCountyRef;
			}

			set
			{
				_icolCountyRef = value;
			}
		}

		public void LoadCountyRef(DataTable adtbSearchResult)
		{
			_icolCountyRef = GetCollection<busCountyRef>(adtbSearchResult, "icdoCountyRef");
		}
	}
}
