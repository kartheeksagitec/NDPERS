using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sagitec.BusinessObjects;
using System.Data;
using System.Collections.ObjectModel;
using NeoSpin.CustomDataObjects;
using NeoSpin.DataObjects;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busMSSMedicarePartDWeb : busPersonAccountMedicarePartDHistory
    {

        public Collection<busPersonAccountInsuranceContribution> iclbMSSInsurancePremiumDetails { get; set; }

        public void LoadMSSInsuranceDetailsMedicare(int aintMemberPersonID)
        {
            Collection<busPersonAccountInsuranceContribution> iclbTemp = new Collection<busPersonAccountInsuranceContribution>();
            if (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag == busConstant.Flag_Yes)
            {
                DateTime CYTDStartDate = ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date;
                DateTime CYTDEndDDate = DateTime.MaxValue;
                DataTable ldtbList = Select("cdoPersonAccount.LoadInsuranceYTDMedicare",
                        new object[3] { CYTDStartDate, CYTDEndDDate, aintMemberPersonID });
                iclbMSSInsurancePremiumDetails = GetCollection<busPersonAccountInsuranceContribution>(ldtbList, "icdoPersonAccountInsuranceContribution");
            }
        }
    }
}
