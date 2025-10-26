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
	[Serializable]
	public class busMergeEmployerDetailGen : busExtendBase
    {
		public busMergeEmployerDetailGen()
		{

		}

		private cdoMergeEmployerDetail _icdoMergeEmployerDetail;
		public cdoMergeEmployerDetail icdoMergeEmployerDetail
		{
			get
			{
				return _icdoMergeEmployerDetail;
			}
			set
			{
				_icdoMergeEmployerDetail = value;
			}
		}

		public bool FindMergeEmployerDetail(int Aintmergeemployerdetailid)
		{
			bool lblnResult = false;
			if (_icdoMergeEmployerDetail == null)
			{
				_icdoMergeEmployerDetail = new cdoMergeEmployerDetail();
			}
			if (_icdoMergeEmployerDetail.SelectRow(new object[1] { Aintmergeemployerdetailid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
