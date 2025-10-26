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
    public class busBenefitRhicCombineDetail : busBenefitRhicCombineDetailGen
    {
        public busBenefitApplication ibusApplication { get; set; }

        public busPayeeAccount ibusPayeeAccount { get; set; }
        public void LoadPayeeAccount()
        {
            if (ibusPayeeAccount.IsNull())
                ibusPayeeAccount = new busPayeeAccount();
            ibusPayeeAccount.FindPayeeAccount(icdoBenefitRhicCombineDetail.donar_payee_account_id);
        }
        public busBenefitRhicCombine ibusBenefitRHICCombine { get; set; }
        public void LoadRHICCombine()
        {
            if (ibusBenefitRHICCombine.IsNull())
                ibusBenefitRHICCombine = new busBenefitRhicCombine();
            ibusBenefitRHICCombine.FindBenefitRhicCombine(icdoBenefitRhicCombineDetail.benefit_rhic_combine_id);
        }

        public busConstant.donor_payee_account_type ienmDonorPayeeAccountType { get; set; }
    }
}
