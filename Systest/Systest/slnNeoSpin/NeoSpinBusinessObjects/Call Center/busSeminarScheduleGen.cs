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
    public partial class busSeminarSchedule : busExtendBase
	{
		public busSeminarSchedule()
		{

		} 

		private cdoSeminarSchedule _icdoSeminarSchedule;
		public cdoSeminarSchedule icdoSeminarSchedule
		{
			get
			{
				return _icdoSeminarSchedule;
			}

			set
			{
				_icdoSeminarSchedule = value;
			}
		}

		public bool FindSeminarSchedule(int Aintseminarsceduleid)
		{
			bool lblnResult = false;
			if (_icdoSeminarSchedule == null)
			{
				_icdoSeminarSchedule = new cdoSeminarSchedule();
			}
			if (_icdoSeminarSchedule.SelectRow(new object[1] { Aintseminarsceduleid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
