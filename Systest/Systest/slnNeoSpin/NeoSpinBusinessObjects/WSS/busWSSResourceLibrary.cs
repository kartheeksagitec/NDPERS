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
    public class busWSSResourceLibrary : busExtendBase
    {
        public string PlanName { get; set; }
        public string PlanDetails { get; set; }
        public string FileName { get; set; }

        public Collection<busWSSResourceLibrary> iclbWSSEmployerReporting { get; set; }
        public Collection<busWSSResourceLibrary> iclbWSSInsurancePlans { get; set; }
        public Collection<busWSSResourceLibrary> iclbWSSRetirementPlans { get; set; }
        public Collection<busWSSResourceLibrary> iclbWSSDeferredCompPlans { get; set; }
        public busOrganization ibusOrganization { get; set; }
        public cdoOrganization icdoOrganization { get; set; }
        private Collection<busOrgPlan> _iclbOrgPlan;
        public Collection<busOrgPlan> iclbOrgPlan
        {
            get { return _iclbOrgPlan; }
            set { _iclbOrgPlan = value; }
        }
        /// <summary>
        /// Load Plan details in collection
        /// </summary>
        /// 
        public void LoadOrganization(int aintOrgID)
        {
            if (ibusOrganization == null)
            {
                ibusOrganization = new busOrganization();
            }
            ibusOrganization.FindOrganization(aintOrgID);
        }
        public void LoadOrgPlan(int aintOrgID)
        {
            DataTable ldtbList = Select<cdoOrgPlan>(
                new string[1] { "org_id" },
                new object[1] { aintOrgID }, null, "participation_end_date,participation_start_date desc");
            _iclbOrgPlan = GetCollection<busOrgPlan>(ldtbList, "icdoOrgPlan");
        }
        public void LoadResourceLibrary(int aintOrgID, int aintContactID)
        {
            //Insurance Plans
            //ibusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
            LoadOrganization(aintOrgID);
            LoadOrgPlan(aintOrgID);
            iclbWSSInsurancePlans = new Collection<busWSSResourceLibrary>();
            iclbWSSRetirementPlans = new Collection<busWSSResourceLibrary>();
            iclbWSSDeferredCompPlans = new Collection<busWSSResourceLibrary>();
            var lenuOfferedPlans = _iclbOrgPlan.Where(i => i.icdoOrgPlan.participation_end_date == DateTime.MinValue || i.icdoOrgPlan.participation_end_date > DateTime.Now);
            foreach (busOrgPlan lobjOrgPlan in lenuOfferedPlans.AsEnumerable<busOrgPlan>())
            {
                // PIR 13998 - Title Change From 'Text' to 'Learn More'

                //Insurance Plans
                lobjOrgPlan.LoadPlanInfo(lobjOrgPlan.icdoOrgPlan.plan_id);
                if (lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdDental)
                    iclbWSSInsurancePlans.Add(new busWSSResourceLibrary { PlanName = "Dental", PlanDetails = "Learn More", FileName = "Dental Insurance Section.pdf" }); 
                if (lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdEAP)
                    iclbWSSInsurancePlans.Add(new busWSSResourceLibrary { PlanName = "EAP", PlanDetails = "Learn More", FileName = "EAP Section.pdf" }); 
                if (lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdFlex)
                    iclbWSSInsurancePlans.Add(new busWSSResourceLibrary { PlanName = "Flexcomp", PlanDetails = "Learn More", FileName = "Flexcomp Section.pdf" }); 
                if (lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdGroupHealth)
                    iclbWSSInsurancePlans.Add(new busWSSResourceLibrary { PlanName = "Health", PlanDetails = "Learn More", FileName = "Health Insurance Section.pdf" }); 
                if (lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdGroupLife)
                    iclbWSSInsurancePlans.Add(new busWSSResourceLibrary { PlanName = "Life", PlanDetails = "Learn More", FileName = "Life Insurance Section.pdf" }); 
                if (lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdLTC)
                    iclbWSSInsurancePlans.Add(new busWSSResourceLibrary { PlanName = "LTC", PlanDetails = "Learn More", FileName = "LTC Insurance Section.pdf" }); 
                if (lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdVision)
                    iclbWSSInsurancePlans.Add(new busWSSResourceLibrary { PlanName = "Vision", PlanDetails = "Learn More", FileName = "Vision Insurance Section.pdf" });

                //Retirement Plans
                var lRetirementDCPlans =iclbWSSRetirementPlans.Where(planid => planid.PlanName == "Defined Contribution"); //PIR 21382
                if ((lRetirementDCPlans != null) && (lRetirementDCPlans.Count() == 0))
                {
                    if (lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdDC || lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdDC2020 || lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdDC2025) //PIR 20232  
                        iclbWSSRetirementPlans.Add(new busWSSResourceLibrary { PlanName = "Defined Contribution", PlanDetails = "Learn More", FileName = "Defined Contribution Section.pdf" });
                }

                var lRetirementPlans = iclbWSSRetirementPlans.Where(planid => planid.PlanName == "Defined Benefit");

                if (lRetirementPlans != null)
                {
                    if (lRetirementPlans.Count() == 0)
                    {
                        if (lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdMain
                            || lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdMain2020//PIR 20232
                            || lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdLE || lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdNG
                            || lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdHP || lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdJudges
                            || lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdJobService || lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdLEWithoutPS
                            || lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdOasis || lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdJobService3rdPartyPayor
                            || lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdPriorJudges || lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdPriorService
                            || lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdBCILawEnf
                            || lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdStatePublicSafety)  //PIR 25729
                            iclbWSSRetirementPlans.Add(new busWSSResourceLibrary { PlanName = "Defined Benefit", PlanDetails = "Learn More", FileName = "Defined Benefit Section.pdf" });
                    }
                }
                //Deferred Comp Plans            
                if (lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdDeferredCompensation)
                    iclbWSSDeferredCompPlans.Add(new busWSSResourceLibrary { PlanName = "Deferred Compensation", PlanDetails = "Learn More", FileName = "Deferred Compensation Employer Guide.pdf" }); 
            }

            iclbWSSInsurancePlans.Add(new busWSSResourceLibrary { PlanName = "Wellness", PlanDetails = "Learn More", FileName = "Wellness Section.pdf" }); 


            //Employer Reporting
            iclbWSSEmployerReporting = new Collection<busWSSResourceLibrary>();
            iclbWSSEmployerReporting.Add(new busWSSResourceLibrary { PlanName = "Employer Self Service (ESS) Guide", PlanDetails = "Learn More", FileName = "Employer Guide for Employer Reporting.pdf" }); // PIR 14432 Change label 'Employer Reporting' to 'Employer Self Service (ESS) Guide'
            // PIR NO. 14043
            iclbWSSEmployerReporting.Add(new busWSSResourceLibrary { PlanName = "Creating Payroll – Quick Steps – RETIREMENT", PlanDetails = "Learn More", FileName = "Creating Payroll - RETIREMENT.pdf" }); 
            iclbWSSEmployerReporting.Add(new busWSSResourceLibrary { PlanName = "Creating Payroll – Quick Steps – Deferred Comp", PlanDetails = "Learn More", FileName = "Creating Payroll - DEFERRED COMP.pdf" }); 
            iclbWSSEmployerReporting.Add(new busWSSResourceLibrary { PlanName = "Creating Payroll – Quick Steps – Service Purchases", PlanDetails = "Learn More", FileName = "Creating Payroll - SERVICE PURCHASES.pdf" }); 
			iclbWSSEmployerReporting.Add(new busWSSResourceLibrary { PlanName = "2016-1 Payroll Reporting - Avoid Common Mistakes that can affect your Employees Retirement", PlanDetails = "Learn More", FileName = "Delayed Payroll Memo.pdf" });
			iclbWSSEmployerReporting.Add(new busWSSResourceLibrary { PlanName = "Decision Tree Retirement Contribution Reporting", PlanDetails = "Learn More", FileName = "Decision-Tree-Retirement-Contribution-Reporting.pdf" });

        }
    }
}
