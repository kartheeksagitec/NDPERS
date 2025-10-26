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
	public partial class busSeminarAttendeeDetail : busExtendBase
	{
		public busSeminarAttendeeDetail()
		{

		} 

		private cdoSeminarAttendeeDetail _icdoSeminarAttendeeDetail;
		public cdoSeminarAttendeeDetail icdoSeminarAttendeeDetail
		{
			get
			{
				return _icdoSeminarAttendeeDetail;
			}

			set
			{
				_icdoSeminarAttendeeDetail = value;
			}
		}

		public bool FindSeminarAttendeeDetail(int Aintseminarattendeedetailid)
		{
			bool lblnResult = false;
			if (_icdoSeminarAttendeeDetail == null)
			{
				_icdoSeminarAttendeeDetail = new cdoSeminarAttendeeDetail();
			}
			if (_icdoSeminarAttendeeDetail.SelectRow(new object[1] { Aintseminarattendeedetailid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
