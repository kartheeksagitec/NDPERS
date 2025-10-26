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
    public class busMSSPersonAccountInsuranceWeb : busPersonAccount
    { 
        public int iintEnrollmentRequestId { get; set; }
        public Collection<busPersonEmploymentDetail> iclbMSSPersonEmploymentDetail { get; set; }      

        // Load Premium YTD
        public Collection<busPersonAccountInsuranceContribution> iclbMSSInsurancePremiumDetails { get; set; }
        public void LoadMSSInsuranceDetails()
        {
            if (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag == busConstant.Flag_Yes)
            {
                DateTime CYTDStartDate = ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date;
                DateTime CYTDEndDDate = DateTime.MaxValue;
                DataTable ldtbList = Select("cdoPersonAccount.LoadInsuranceYTD",
                        new object[3] { CYTDStartDate, CYTDEndDDate, icdoPersonAccount.person_account_id });
                iclbMSSInsurancePremiumDetails = GetCollection<busPersonAccountInsuranceContribution>(ldtbList, "icdoPersonAccountInsuranceContribution");
            }
        }
        public Collection<busPersonDependent> iclbMSSPersonDependent { get; set; }
        public void LoadMSSDependent()
        {
            iclbMSSPersonDependent = new Collection<busPersonDependent>();

            if (ibusPerson.IsNull())
                LoadPerson();

            if (ibusPerson.iclbPersonDependent.IsNull())
                ibusPerson.LoadDependent();

            foreach (busPersonDependent lobjPersonDependent in ibusPerson.iclbPersonDependent)
            {
                if (lobjPersonDependent.ibusPeronAccountDependent.ibusPersonAccount.IsNull())
                    lobjPersonDependent.ibusPeronAccountDependent.LoadPersonAccount();
            }

            var lenumDependentList = ibusPerson.iclbPersonDependent.Where(lobjPerDep => lobjPerDep.ibusPeronAccountDependent.ibusPersonAccount.icdoPersonAccount.plan_id == icdoPersonAccount.plan_id
                && (lobjPerDep.ibusPeronAccountDependent.icdoPersonAccountDependent.end_date > DateTime.Now
                || lobjPerDep.ibusPeronAccountDependent.icdoPersonAccountDependent.end_date == DateTime.MinValue));

            foreach (busPersonDependent lobjPersonDependent in lenumDependentList)
            {
                lobjPersonDependent.LoadDependentInfo();
                iclbMSSPersonDependent.Add(lobjPersonDependent);
            }
        }

        //load only contributing employers
        public void LoadMSSContributingEmployers()
        {
            if (iclbMSSPersonEmploymentDetail.IsNull())
                iclbMSSPersonEmploymentDetail = new Collection<busPersonEmploymentDetail>();

            if (iclbEmploymentDetail.IsNull())
                LoadAllPersonEmploymentDetails();

            var lContributingEmployers = iclbEmploymentDetail.Where(lobjPersonEmploymentDTL => lobjPersonEmploymentDTL.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusContributing);

            foreach (busPersonEmploymentDetail lobjPersonEmploymentdtl in lContributingEmployers)
            {
                iclbMSSPersonEmploymentDetail.Add(lobjPersonEmploymentdtl);
            }
        }               
    }
}
