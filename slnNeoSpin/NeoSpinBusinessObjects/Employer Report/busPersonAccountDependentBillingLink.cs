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
	/// <summary>
	/// Class NeoSpin.BusinessObjects.busPersonAccountDependentBillingLink:
	/// Inherited from busPersonAccountDependentBillingLinkGen, the class is used to customize the business object busPersonAccountDependentBillingLinkGen.
	/// </summary>
	[Serializable]
	public class busPersonAccountDependentBillingLink : busPersonAccountDependentBillingLinkGen
	{
        public busIbsDetail ibusIBSDetail { get; set; }

        public void LoadIBSDetail()
        {
            if (ibusIBSDetail.IsNull()) ibusIBSDetail = new busIbsDetail { icdoIbsDetail = new cdoIbsDetail() };
            ibusIBSDetail.FindIbsDetail(icdoPersonAccountDependentBillingLink.ibs_detail_id);
        }

        public busEmployerPayrollDetail ibusEmployerPayrollDetail { get; set; }

        public void LoadEmployerPayrollDetail()
        {
            if (ibusEmployerPayrollDetail.IsNull()) ibusEmployerPayrollDetail = new busEmployerPayrollDetail { icdoEmployerPayrollDetail = new cdoEmployerPayrollDetail() };
            ibusEmployerPayrollDetail.FindEmployerPayrollDetail(icdoPersonAccountDependentBillingLink.employer_payroll_detail_id);
        }

        public busPersonAccountDependent ibusPADependent { get; set; }

        public void LoadPADependent()
        {
            if (ibusPADependent.IsNull()) ibusPADependent = new busPersonAccountDependent
            {
                icdoPersonAccountDependent = new cdoPersonAccountDependent(),
                icdoPersonDependent = new cdoPersonDependent()
            };
            ibusPADependent.FindPersonAccountDependent(icdoPersonAccountDependentBillingLink.person_account_dependent_id);
            ibusPADependent.FindPersonDependent(ibusPADependent.icdoPersonAccountDependent.person_dependent_id);
            ibusPADependent.LoadDependentInfo();
        }
	}
}
