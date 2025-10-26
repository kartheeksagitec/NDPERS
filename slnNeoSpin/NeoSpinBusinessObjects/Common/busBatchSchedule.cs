#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using NeoSpin.CustomDataObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
	[Serializable]
	public class busBatchSchedule : busExtendBase
    {
        public busBatchSchedule()
		{
		}

        private cdoBatchSchedule _icdoBatchSchedule;
        public cdoBatchSchedule icdoBatchSchedule
		{
			get
			{
                return _icdoBatchSchedule;
			}

			set
			{
                _icdoBatchSchedule = value;
			}
		}

        public bool FindBatchSchedule(int aintBatchScheduleId)
		{
            bool lblnResult = false;
            if (_icdoBatchSchedule == null)
			{
                _icdoBatchSchedule = new cdoBatchSchedule();
			}
            if (_icdoBatchSchedule.SelectRow(new object[1] { aintBatchScheduleId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
