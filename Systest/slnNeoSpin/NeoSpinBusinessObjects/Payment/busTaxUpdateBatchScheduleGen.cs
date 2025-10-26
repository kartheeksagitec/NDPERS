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
	public class busTaxUpdateBatchScheduleGen : busExtendBase
    {
		public busTaxUpdateBatchScheduleGen()
		{

		}

		private cdoTaxUpdateBatchSchedule _icdoTaxUpdateBatchSchedule;
		public cdoTaxUpdateBatchSchedule icdoTaxUpdateBatchSchedule
		{
			get
			{
				return _icdoTaxUpdateBatchSchedule;
			}
			set
			{
				_icdoTaxUpdateBatchSchedule = value;
			}
		}

		public bool FindTaxUpdateBatchSchedule(int Ainttaxupdatebatchscheduleid)
		{
			bool lblnResult = false;
			if (_icdoTaxUpdateBatchSchedule == null)
			{
				_icdoTaxUpdateBatchSchedule = new cdoTaxUpdateBatchSchedule();
			}
			if (_icdoTaxUpdateBatchSchedule.SelectRow(new object[1] { Ainttaxupdatebatchscheduleid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
