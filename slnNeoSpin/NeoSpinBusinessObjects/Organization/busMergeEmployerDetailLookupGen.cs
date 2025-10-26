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
	public class busMergeEmployerDetailLookupGen : busMainBase
	{

		private Collection<busMergeEmployerDetail> _iclbMergeEmployerDetail;
		public Collection<busMergeEmployerDetail> iclbMergeEmployerDetail
		{
			get
			{
				return _iclbMergeEmployerDetail;
			}
			set
			{
				_iclbMergeEmployerDetail = value;
			}
		}

		public void LoadMergeEmployerDetails(DataTable adtbSearchResult)
		{
			_iclbMergeEmployerDetail = GetCollection<busMergeEmployerDetail>(adtbSearchResult, "icdoMergeEmployerDetail");
		}
	}
}
