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
using System.Linq;

#endregion

namespace NeoSpin.BusinessObjects
{
	[Serializable]
    public partial class busOrganization : busExtendBase
	{
		public busOrganization()
		{

		} 

		private cdoOrganization _icdoOrganization;
		public cdoOrganization icdoOrganization
		{
			get
			{
				return _icdoOrganization;
			}

			set
			{
				_icdoOrganization = value;
			}
		}

        public bool iblnIsOrgOfferingAnyRetPlan { get {
                if (this.iclbOrgPlan.IsNull())
                    LoadOrgPlan();
                return this.iclbOrgPlan.Any(i => busGlobalFunctions.CheckDateOverlapping(busGlobalFunctions.GetSysManagementBatchDate(), i.icdoOrgPlan.participation_start_date, i.icdoOrgPlan.participation_end_date) 
                                            && i.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeRetirement);
            } }

        public bool FindOrganization(int Aintorgid)
		{
			bool lblnResult = false;
			if (_icdoOrganization == null)
			{
				_icdoOrganization = new cdoOrganization();
			}
			if (_icdoOrganization.SelectRow(new object[1] { Aintorgid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
