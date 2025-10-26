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
	public class busOrgPlanGroupHealthMedicarePartDRateRefGen : busExtendBase
    {
		public busOrgPlanGroupHealthMedicarePartDRateRefGen()
		{

		} 

		private cdoOrgPlanGroupHealthMedicarePartDRateRef _icdoOrgPlanGroupHealthMedicarePartDRateRef;
		public cdoOrgPlanGroupHealthMedicarePartDRateRef icdoOrgPlanGroupHealthMedicarePartDRateRef
		{
			get
			{
				return _icdoOrgPlanGroupHealthMedicarePartDRateRef;
			}

			set
			{
				_icdoOrgPlanGroupHealthMedicarePartDRateRef = value;
			}
		}

		public bool FindOrgPlanGroupHealthMedicarePartDRateRef(int Aintorgplangrouphealthmedicarepartdraterefid)
		{
			bool lblnResult = false;
			if (_icdoOrgPlanGroupHealthMedicarePartDRateRef == null)
			{
				_icdoOrgPlanGroupHealthMedicarePartDRateRef = new cdoOrgPlanGroupHealthMedicarePartDRateRef();
			}
			if (_icdoOrgPlanGroupHealthMedicarePartDRateRef.SelectRow(new object[1] { Aintorgplangrouphealthmedicarepartdraterefid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
