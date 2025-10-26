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
	public class busOrgPlanGroupHealthMedicarePartDRateStructureRefGen : busExtendBase
    {
		public busOrgPlanGroupHealthMedicarePartDRateStructureRefGen()
		{

		}

		private cdoOrgPlanGroupHealthMedicarePartDRateStructureRef _icdoOrgPlanGroupHealthMedicarePartDRateStructureRef;
		public cdoOrgPlanGroupHealthMedicarePartDRateStructureRef icdoOrgPlanGroupHealthMedicarePartDRateStructureRef
		{
			get
			{
				return _icdoOrgPlanGroupHealthMedicarePartDRateStructureRef;
			}
			set
			{
				_icdoOrgPlanGroupHealthMedicarePartDRateStructureRef = value;
			}
		}

		public bool FindOrgPlanGroupHealthMedicarePartDRateStructureRef(int Aintorgplangrouphealthmedicarepartdratestructurerefid)
		{
			bool lblnResult = false;
			if (_icdoOrgPlanGroupHealthMedicarePartDRateStructureRef == null)
			{
				_icdoOrgPlanGroupHealthMedicarePartDRateStructureRef = new cdoOrgPlanGroupHealthMedicarePartDRateStructureRef();
			}
			if (_icdoOrgPlanGroupHealthMedicarePartDRateStructureRef.SelectRow(new object[1] { Aintorgplangrouphealthmedicarepartdratestructurerefid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
