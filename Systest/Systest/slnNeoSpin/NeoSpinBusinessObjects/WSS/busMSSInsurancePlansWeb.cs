using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sagitec.BusinessObjects;
using System.Data;
using System.Collections.ObjectModel;
using NeoSpin.CustomDataObjects;
using NeoSpin.DataObjects;
using Sagitec.CustomDataObjects;
using System.Collections;
using Sagitec.Common;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busMSSInsurancePlansWeb : busExtendBase
    {
        public busPerson ibusPerson { get; set; }
        public Collection<busBenefitPlanWeb> iclbBenefitEnrolledInsurancePlans { get; set; }
        public Collection<busBenefitPlanWeb> iclbBenefitEligibleInsurancePlans { get; set; }

        public void LoadPerson(int aintPersonId)
        {
            if (ibusPerson.IsNull())
                ibusPerson = new busPerson();
            ibusPerson.FindPerson(aintPersonId);
        }

        // PIR 9793 All the methods are removed by re-using the busMSSHome object methods.
        // Now both the NDPERs Plan and Insurance Plans are in sync.
        public void LoadEnrolledEligibleInsurancePlans()
        {
            iclbBenefitEnrolledInsurancePlans = new Collection<busBenefitPlanWeb>();
            iclbBenefitEligibleInsurancePlans = new Collection<busBenefitPlanWeb>();
            if (ibusPerson.icolPersonEmployment.IsNull())  ibusPerson.LoadPersonEmployment();
            if (ibusPerson.iclbWSSEnrollmentRequest.IsNull()) ibusPerson.LoadWSSEnrollmentRequest();
            busMSSHome lobjHome = new busMSSHome();
            lobjHome.ibusPerson = ibusPerson;
            lobjHome.iblnIsFromMSSActivePlan = true;
            lobjHome.LoadEnrolledAndEligiblePlans();
            foreach (busBenefitPlanWeb lobjBenefitPlan in lobjHome.iclbEnrolledPlans)
            {
                if (lobjBenefitPlan.ibusPlan.IsInsurancePlan()) iclbBenefitEnrolledInsurancePlans.Add(lobjBenefitPlan);
            }
            foreach (busBenefitPlanWeb lobjBenefitPlan in lobjHome.iclbEligiblePlans)
            {
                if (lobjBenefitPlan.ibusPlan.IsInsurancePlan()) iclbBenefitEligibleInsurancePlans.Add(lobjBenefitPlan);
            }
        }
    }
}