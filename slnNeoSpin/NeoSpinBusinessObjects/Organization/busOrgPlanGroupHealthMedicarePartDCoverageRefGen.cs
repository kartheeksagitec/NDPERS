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
	public class busOrgPlanGroupHealthMedicarePartDCoverageRefGen : busExtendBase
    {
		public busOrgPlanGroupHealthMedicarePartDCoverageRefGen()
		{

		} 

		private cdoOrgPlanGroupHealthMedicarePartDCoverageRef _icdoOrgPlanGroupHealthMedicarePartDCoverageRef;
		public cdoOrgPlanGroupHealthMedicarePartDCoverageRef icdoOrgPlanGroupHealthMedicarePartDCoverageRef
		{
			get
			{
				return _icdoOrgPlanGroupHealthMedicarePartDCoverageRef;
			}

			set
			{
				_icdoOrgPlanGroupHealthMedicarePartDCoverageRef = value;
			}
		}

		public bool FindOrgPlanGroupHealthMedicarePartDCoverageRef(int Aintorgplangrouphealthmedicarepartdcoveragerefid)
		{
			bool lblnResult = false;
			if (_icdoOrgPlanGroupHealthMedicarePartDCoverageRef == null)
			{
				_icdoOrgPlanGroupHealthMedicarePartDCoverageRef = new cdoOrgPlanGroupHealthMedicarePartDCoverageRef();
			}
			if (_icdoOrgPlanGroupHealthMedicarePartDCoverageRef.SelectRow(new object[1] { Aintorgplangrouphealthmedicarepartdcoveragerefid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
