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
	public partial class busIbsDetailLookup  : busMainBase
	{

		private Collection<busIbsDetail> _icolIbsDetail;
		public Collection<busIbsDetail> icolIbsDetail
		{
			get
			{
				return _icolIbsDetail;
			}

			set
			{
				_icolIbsDetail = value;
			}
		}

		public void LoadIbsDetail(DataTable adtbSearchResult)
		{
			_icolIbsDetail = GetCollection<busIbsDetail>(adtbSearchResult, "icdoIbsDetail");
		}
	}
}
