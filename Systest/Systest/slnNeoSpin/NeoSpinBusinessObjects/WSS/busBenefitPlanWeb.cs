using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sagitec.BusinessObjects;
using Sagitec.CustomDataObjects;
using System.Data;
using System.Collections.ObjectModel;
using NeoSpin.CustomDataObjects;
using NeoSpin.DataObjects;
using Sagitec.Common;
using System.Collections;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busBenefitPlanWeb : busExtendBase
    {
        public int iintOrgId { get; set; }
        public int iintPlanId { get; set; }
        public int iintEnrollmentRequestId { get; set; }
        public string istrEnrollmentRequestStatus { get; set; }
        public string istrProviderName { get; set; } //PIR -23093 Provider_Name
        //**** RETIREMENT
        public string istrPlanNameNewDBRetirement { get; set; }
        public string istrPlanNameNewDBOptionalRetirement { get; set; }
        public string istrPlanNameNewDBElectedOfficial { get; set; }
        public string istrPlanNameNewDCRetirement { get; set; }
        public string istrPlanNameNewDCOptionalRetirement { get; set; }
        //**** INSURANCE
        public string istrPlanNameNewGHDVInsurance { get; set; }
        public string istrPlanNameNewLifeInsurance { get; set; }
        public string istrPlanNameNewLTCInsurance { get; set; }
        public string istrPlanNameNewEAPInsurnace { get; set; }
        public string istrPlanNameNewFlexCompInsurance { get; set; }
        public string istrPlanNameNewHMOInsurance { get; set; }
        //**** DEFERRED COMP
        public string istrPlanNameNewDeferredComp { get; set; }
        public string istrPlanNameOpenEligibleDeferredComp { get; set; }
        public string benefit_tier_description_display { get; set; }
        //*********
        public string istrPlanNameUpdate { get; set; }
        public string istrElectionValue { get; set; }
        public string istrElectionDescription { get; set; }
        public int iintPersonAccountId { get; set; }
        public int iintPersonEmploymentDetailid { get; set; }
        public string istrScreenDifferentiator { get; set; }
        public string istrOrganizationName { get; set; }
        public string istrJobClassValue { get; set; }
        public DateTime idtStartDate { get; set; }
        //**** RETIREMENT
        public bool iblnEligibleDBRetirementEnrollment;
        public bool iblnEligibleDBRetirementOptionalEnrollment;
        public bool iblnEligibleDBRetirementElectedOffcialEnrollment;
        public bool iblnEligibleDCRetirementEnrollment;
        public bool iblnEligibleDCRetirementOptionalEnrollment;
        //**** INSURANCE
        public string istrPlanName { get; set; }
        public bool iblnEligibleGHDVInsuranceEnrollment;
        public bool iblnEligibleLifeInsuranceEnrollment;
        public bool iblnEligibleLTCInsuranceEnrollment;
        public bool iblnEligibleEAPInsuranceEnrollment;
        public bool iblnOpenEmploymentEAP; //PIR 10269
        public bool iblnEligibleFlexCompInsuranceEnrollment;
        public bool iblnEligibleHMOInsuranceEnrollment;
        //**** DEFF COMP
        public bool iblnEligibleDeferredCompEnrollment;
        public bool iblnOpenEligibleDeferredCompEnrollment;

        public string istrPlanVideoLink
        {
            get
            {
                if(iintPlanId == busConstant.PlanIdOther457 || iintPlanId == busConstant.PlanIdLTC) //PIR 9794
                    return "";
                return "Video";
            }
        }

        public string istrPlanVideoLinkLTC //PIR 13051
        {
            get
            {
                if (iintPlanId == busConstant.PlanIdLTC)
                    return "Video Not Available";
                else
                    return "";
            }
        }

        public string istrPlanTextLink
        {
            get
            {
                if (iintPlanId != busConstant.PlanIdOther457) //PIR 9794
                    return "Learn More"; // PIR 13998
                return "";
            }
        }

        public int iintMSSPlanEligibleValue
        {
            get
            {
                int lintReturn = 0;
                switch (iintPlanId)
                {
                    case 1: //for retirement plans                   
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 20:
                    case 26:
                    case 29: //PIR 25729
                    case 27://PIR 20232
                    case 30:	//PIR 25920 New Plan DC 2025
                        if (iblnEligibleDBRetirementEnrollment)
                            lintReturn = 104;
                        else if (iblnEligibleDBRetirementOptionalEnrollment)
                            lintReturn = 105;
                        else if (iblnEligibleDBRetirementElectedOffcialEnrollment)
                            lintReturn = 110;
                        else if (iintPersonAccountId > 0)
                            lintReturn = 106;
                        else
                            lintReturn = 104;
                        break;
                    case 7:
                    case 28: //PIR 20232
                        if (iblnEligibleDCRetirementEnrollment)
                            lintReturn = 107;
                        else if (iblnEligibleDCRetirementOptionalEnrollment)
                            lintReturn = 108;
                        else if (iintPersonAccountId > 0)
                            lintReturn = 106;
                        else
                            lintReturn = 104;
                        break;
                    case 9: //for insurance plans
                    case 12:
                    case 13:
                    case 14:
                    case 15:
                    case 16:
                    case 17:
                    case 18:
                        if (iblnEligibleGHDVInsuranceEnrollment)
                            if (iintPersonAccountId > 0)
                                lintReturn = 111;
                            else
                                lintReturn = 116;
                        else if (iblnEligibleLifeInsuranceEnrollment)
                            if (iintPersonAccountId > 0)
                                lintReturn = 112;
                            else
                                lintReturn = 121;
                        else if (iblnEligibleLTCInsuranceEnrollment)
                            if (iintPersonAccountId > 0)
                                lintReturn = 113;
                            else
                                lintReturn = 117;
                        else if (iblnEligibleEAPInsuranceEnrollment)
                             if (!iblnOpenEmploymentEAP )// PIR 10269
                                lintReturn = 122;
                            else
                                lintReturn = 118;
                        else if (iblnEligibleFlexCompInsuranceEnrollment)
                            if (iintPersonAccountId > 0)
                                lintReturn = 115;
                            else
                                lintReturn = 119;
                        else
                            lintReturn = 120;
                        break;
                    case 19: //for def comp and Other 457                        
                        if (iblnEligibleDeferredCompEnrollment)
                        {
                            if (iintPersonAccountId == 0)
                                lintReturn = 103;                          
                            else
                                lintReturn = 102;
                        }
                        if(iblnOpenEligibleDeferredCompEnrollment)
                            lintReturn = 103;        
                        break;
                    case 8:
                        lintReturn = 102;
                        break;
                    default:
                        lintReturn = iintPlanId;
                        break;
                }
                return lintReturn;
            }
        }

        // PROD PIR ID 6320
        public busPlan ibusPlan { get; set; }

        public void LoadPlan()
        {
            if (ibusPlan.IsNull()) ibusPlan = new busPlan { icdoPlan = new cdoPlan() };
            ibusPlan.FindPlan(iintPlanId);
        }

        public busPersonAccount ibusPersonAccount { get; set; }
        public bool iblnIsBenefitPlanCobraorRetiree { get;  set; }
        public string istrEmpTypeValue { get; set; }
        public bool iblnDontShowInANNEGrid { get;  set; }
        public bool iblnEnrollInHealthAsTemporary { get;  set; }

        public void LoadPersonAccount(int aintPersonAccountId)
        {
            ibusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
            ibusPersonAccount.FindPersonAccount(aintPersonAccountId);
        }
        public bool iblnIsLearnMoreLinkVisible { get; set; }

        public bool iblnIsVideoLinkVisible { get; set; }

    }
}

