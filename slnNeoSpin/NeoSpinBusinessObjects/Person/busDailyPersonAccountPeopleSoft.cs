using NeoSpin.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.CustomDataObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoSpin.BusinessObjects
{
    class busDailyPersonAccountPeopleSoft : busExtendBase
    {
        public busPersonAccount ibusPersonAccount { get; set; }
        public busOrganization ibusProvider { get; set; }
        public busPersonAccountGhdv ibusPersonAccountGhdv { get; set; }
        public busPersonAccountGhdvHsa ibusPersonAccountGhdvHSA { get; set; } //PIR 20481
        public busPersonAccountEAP ibusPersonAccountEAP { get; set; }
        public busPersonAccountFlexComp ibusPersonAccountFlexComp { get; set; }
        public busPersonAccountLife ibusPersonAccountLife { get; set; }

        public string istrBenefitPlan { get; set; }
        public bool iblnTerminatedRecord { get; set; }
        public string istrCoverageElection { get; set; }
        public string istrHistoryPSFileChangeEventValue { get; set; }
        public DateTime idtEffectiveDate { get; set; }
        public string istrOrgGroupValue { get; set; }
        public string istrLevelOfCoverage { get; set; }
        public decimal idecSuppCoverageAmount { get; set; }
        public decimal idecBasicSuppCoverageAmount { get; set; }
        public decimal idecBasicCoverageAmount { get; set; }

        public string istrInsuranceTypeValue { get; set; }
        public DateTime idtHistoryChangeDate { get; set; }
        public decimal idecSuppPremiumAmount { get; set; }
        public decimal idecFlexCompAnnualPledgeAmount { get; set; }
        public decimal idecSpouseSuppCoverageAmount { get; set; }
        public decimal idecDependentSuppCoverageAmount { get; set; }
        public decimal idecFlatAmount { get; set; }
        public string istrPlanType { get; set; }
        public DateTime idtCoverageBeginDate { get; set; }
        public DateTime idtDeductionBeginDate { get; set; }
        public DateTime idtElectionDate { get; set; }
        public string istrCoverageCode { get; set; }
        public string istrDirectDeposit { get; set; }
        public string istrInsideMail { get; set; }
        public string istrCompany { get; set; }
        public string istrCalculationRoutine { get; set; }
        public bool lblnAllWaived = false;
        public string istrMemberType { get; set; }
        public string istrBenefitPlanforRetirement { get; set; }
        public string istrRHICBenefitPlanforRetirement { get; set; }
        public bool iblnIsRetirementPlan { get; set; }
        public bool iblnBenefitFromPS { get; set; }
        public string istrEmpTypeValue { get; set; }
        public DateTime idtPlanOptionStartDate { get; set; } //Used to get fields coverage begin date,deduction begin date,election date 
        public bool iblnIsPlanOptionSuspended { get; set; }
        public DateTime idtDeductionStartDate { get; set; }
        public DateTime idtDeductionEndDate { get; set; }
        public string istrPSFileChangeEvent { get; set; }
        public bool iblnAnnualEnrollment { get; set; }
        public DateTime idtHSAStartDate { get; set; }
        public DateTime idtHSAEndDate { get; set; }
        public bool iblnIsFromDeferredComp { get; set; } //PIR 20702
        public int iintPersonEmpId { get; set; } //PIR 20702
        public Collection<busPeoplesoftPlanCrossRef> iclbPeopleSoftPlanCrossRef { get; set; }
        public Collection<busPeoplesoftPlanOrgCrossRef> iclbPeoplesoftPlanOrgCrossRef { get; set; }
        public Collection<busOrgPlanGroupHealthMedicarePartDCoverageRef> iclbOrgPlanGroupHealthMedicarePartDCoverageRef { get; set; }
        public Collection<cdoCodeValue> iclbPeoplesoftPlanTypeCodeValues { get; set; }
        public DataTable idtEmploymentDetails { get; set; }
        public DataTable idtbPlanCacheData { get; set; }
        public Collection<busDailyPersonAccountPeopleSoft> iclbDailyPersonAccountPeopleSoft { get; set; }
        public Collection<cdoCodeValue> iclbPeopleSoftOrgGroupValue { get; set; }

        busBase lobjBase = new busBase();
        public busDBCacheData ibusDBCacheData { get; set; }

        public void LoadPlanCacheData()
        {
            idtbPlanCacheData = iobjPassInfo.isrvDBCache.GetCacheData("sgt_plan", null);
        }

        public void LoadPeopleSoftPlanCrossRef()
        {
            DataTable ldtbList = busBase.Select<cdoPeoplesoftPlanCrossRef>(null, null, null, null);
            iclbPeopleSoftPlanCrossRef = lobjBase.GetCollection<busPeoplesoftPlanCrossRef>(ldtbList, "icdoPeoplesoftPlanCrossRef");
        }

        public void LoadPeoplePlanOrgCrossRef()
        {
            DataTable ldtbList = busBase.Select<cdoPeoplesoftPlanOrgCrossRef>(null, null, null, null);
            iclbPeoplesoftPlanOrgCrossRef = lobjBase.GetCollection<busPeoplesoftPlanOrgCrossRef>(ldtbList, "icdoPeoplesoftPlanOrgCrossRef");
        }

        public void LoadHealthCoverageRefCacheData()
        {
            if (ibusDBCacheData.IsNull())
                ibusDBCacheData = new busDBCacheData();

            ibusDBCacheData.idtbCachedCoverageRef = busGlobalFunctions.LoadHealthCoverageRefCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedLifeRate = busGlobalFunctions.LoadLifeRateCacheData(iobjPassInfo);
            iclbOrgPlanGroupHealthMedicarePartDCoverageRef =
                lobjBase.GetCollection<busOrgPlanGroupHealthMedicarePartDCoverageRef>(ibusDBCacheData.idtbCachedCoverageRef,
                                                                                    "icdoOrgPlanGroupHealthMedicarePartDCoverageRef");
        }

        public void LoadPeoplesoftPlanTypeValues()
        {
            DataTable ltdbCodeValue = iobjPassInfo.isrvDBCache.GetCodeValues(363);
            iclbPeoplesoftPlanTypeCodeValues = Sagitec.DataObjects.doBase.GetCollection<cdoCodeValue>(ltdbCodeValue);
        }

        public void LoadPeopleSoftOrgGroupValues()
        {
            DataTable ltdbCodeValue = iobjPassInfo.isrvDBCache.GetCodeValues(364);
            iclbPeopleSoftOrgGroupValue = Sagitec.DataObjects.doBase.GetCollection<cdoCodeValue>(ltdbCodeValue);
        }

        private busPeoplesoftPlanCrossRef GetPeoplesoftPlanCrossRefRecord(Collection<busPeoplesoftPlanCrossRef> aclbPeoplesoftPlanCrossRef)
        {
            switch (ibusPersonAccount.icdoPersonAccount.plan_id)
            {
                case busConstant.PlanIdDental:
                    if (CheckPremiumFlagChecked())
                        return aclbPeoplesoftPlanCrossRef.Where(o => o.icdoPeoplesoftPlanCrossRef.benefit_plan == busConstant.PeopleSoftFileBenefitPlanDenFlx).FirstOrDefault();
                    else
                        return aclbPeoplesoftPlanCrossRef.Where(o => o.icdoPeoplesoftPlanCrossRef.benefit_plan == busConstant.PeopleSoftFileBenefitPlanDental).FirstOrDefault();

                case busConstant.PlanIdVision:
                    if (CheckPremiumFlagChecked())
                        return aclbPeoplesoftPlanCrossRef.Where(o => o.icdoPeoplesoftPlanCrossRef.benefit_plan == busConstant.PeopleSoftFileBenefitPlanVisFlx).FirstOrDefault();
                    else
                        return aclbPeoplesoftPlanCrossRef.Where(o => o.icdoPeoplesoftPlanCrossRef.benefit_plan == busConstant.PeopleSoftFileBenefitPlanVision).FirstOrDefault();

                case busConstant.PlanIdGroupLife:
                    //Spouse Supplemental
                    if (istrLevelOfCoverage == busConstant.LevelofCoverage_SpouseSupplemental)
                    {
                        idecFlatAmount = idecSuppCoverageAmount;
                        return aclbPeoplesoftPlanCrossRef.Where(o => o.icdoPeoplesoftPlanCrossRef.benefit_plan == busConstant.PeopleSoftFileBenefitPlanLifeSpouSuppl ||
                        o.icdoPeoplesoftPlanCrossRef.benefit_plan == busConstant.PeopleSoftBenefitPlanLifeSpouseSuppTemp).FirstOrDefault(); //PIR 20178
                    }
                    //Basic
                    else if (istrLevelOfCoverage == busConstant.LevelofCoverage_Basic)
                    {
                        return aclbPeoplesoftPlanCrossRef.Where(o => o.icdoPeoplesoftPlanCrossRef.benefit_plan == busConstant.PeopleSoftFileBenefitPlanLifeBasic
                        || o.icdoPeoplesoftPlanCrossRef.benefit_plan == busConstant.PeopleSoftBenefitPlanLifeBasicTemp).FirstOrDefault(); //PIR 20178
                    }
                    //Supplemental
                    else if (istrLevelOfCoverage == busConstant.LevelofCoverage_Supplemental)
                        return aclbPeoplesoftPlanCrossRef.Where(o => o.icdoPeoplesoftPlanCrossRef.benefit_plan == busConstant.PeopleSoftFileBenefitPlanLifeSuppl ||
                        o.icdoPeoplesoftPlanCrossRef.benefit_plan == busConstant.PeopleSoftBenefitPlanLifeSuppTemp).FirstOrDefault(); //PIR 20178
                    //Dependent Supplemental
                    else if (istrLevelOfCoverage == busConstant.LevelofCoverage_DependentSupplemental && idecDependentSuppCoverageAmount > 0.0m)
                        return aclbPeoplesoftPlanCrossRef.Where(o => o.icdoPeoplesoftPlanCrossRef.coverage_amount == idecDependentSuppCoverageAmount).FirstOrDefault();
                    break;

                case busConstant.PlanIdFlex:
                    //DCRA
                    if (istrLevelOfCoverage == busConstant.FlexLevelOfCoverageDependentSpending)
                        return aclbPeoplesoftPlanCrossRef.Where(o => o.icdoPeoplesoftPlanCrossRef.benefit_plan == busConstant.PeopleSoftFileBenefitPlanFlexDCRA).FirstOrDefault();
                    //MSRA
                    else if (istrLevelOfCoverage == busConstant.FlexLevelOfCoverageMedicareSpending)
                        return aclbPeoplesoftPlanCrossRef.Where(o => o.icdoPeoplesoftPlanCrossRef.benefit_plan == busConstant.PeopleSoftFileBenefitPlanFlexMSRA).FirstOrDefault();
                    break;
                case busConstant.PlanIdGroupHealth:
                    //If HDHP
                    if (ibusPersonAccountGhdv.icdoPersonAccountGhdv.alternate_structure_code_value == busConstant.AlternateStructureCodeHDHP)
                    {
                        if (ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.peoplesoft_org_group_value == busConstant.PeopleSoftOrgGroupValueBND)
                            return aclbPeoplesoftPlanCrossRef.Where(o => o.icdoPeoplesoftPlanCrossRef.provider_org_code.IsNotNull() && o.icdoPeoplesoftPlanCrossRef.provider_org_code == busGlobalFunctions.GetData1ByCodeValue(52, busConstant.HSAProvider, iobjPassInfo)
                                && o.icdoPeoplesoftPlanCrossRef.benefit_plan == busConstant.PeopleSoftFileBenefitPlanHealthBND).FirstOrDefault();
                        else
                            return aclbPeoplesoftPlanCrossRef.Where(o => o.icdoPeoplesoftPlanCrossRef.provider_org_code.IsNotNull() && o.icdoPeoplesoftPlanCrossRef.provider_org_code == busGlobalFunctions.GetData1ByCodeValue(52, busConstant.HSAProvider, iobjPassInfo)).FirstOrDefault();
                    }
                    else
                        return aclbPeoplesoftPlanCrossRef.Where(o => o.icdoPeoplesoftPlanCrossRef.provider_org_code.IsNull() || o.icdoPeoplesoftPlanCrossRef.provider_org_code != busGlobalFunctions.GetData1ByCodeValue(52, busConstant.HSAProvider, iobjPassInfo)).FirstOrDefault();
                default:
                    //For all other plans
                    return aclbPeoplesoftPlanCrossRef.Where(o => o.icdoPeoplesoftPlanCrossRef.check_flex_comp_plan == busConstant.Flag_No).FirstOrDefault();
            }
            return null;
        }

        //Retirement plans
        public void GeneratePeoplesoftEntryForRetirementPlans(string astrEmpType, string astrMemberType, int aintEEContriPercent)
        {
            if (iclbPeopleSoftPlanCrossRef == null)
                LoadPeopleSoftPlanCrossRef();

            Collection<busPeoplesoftPlanCrossRef> lclbPeopleCrossRef = new Collection<busPeoplesoftPlanCrossRef>();
            foreach (busPeoplesoftPlanCrossRef lobjPeopleSoftPlanCrossRef in iclbPeopleSoftPlanCrossRef)
            {
                if ((lobjPeopleSoftPlanCrossRef.icdoPeoplesoftPlanCrossRef.plan_id == ibusPersonAccount.icdoPersonAccount.plan_id &&
                   lobjPeopleSoftPlanCrossRef.icdoPeoplesoftPlanCrossRef.emp_type_value == astrEmpType &&
                   lobjPeopleSoftPlanCrossRef.icdoPeoplesoftPlanCrossRef.member_type_value == astrMemberType && 
                   (ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMain || ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMain2020)) 
                   ||
                   (!(ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMain || ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMain2020
                   || ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDC2025) 
                   && lobjPeopleSoftPlanCrossRef.icdoPeoplesoftPlanCrossRef.plan_id == ibusPersonAccount.icdoPersonAccount.plan_id)
                   ||
                   (ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDC2025 && lobjPeopleSoftPlanCrossRef.icdoPeoplesoftPlanCrossRef.emp_type_value == astrEmpType
                   && lobjPeopleSoftPlanCrossRef.icdoPeoplesoftPlanCrossRef.plan_id == ibusPersonAccount.icdoPersonAccount.plan_id
                   && lobjPeopleSoftPlanCrossRef.icdoPeoplesoftPlanCrossRef.addl_ee_contribution_percent == aintEEContriPercent)
                   ) //PIR 20232
                {
                    lclbPeopleCrossRef.Add(lobjPeopleSoftPlanCrossRef);
                }
            }

            if (lclbPeopleCrossRef.Count > 1)
            {
                busPeoplesoftPlanCrossRef lobjPeoplesoftPlanCrossRef = GetPeoplesoftPlanCrossRefRecord(lclbPeopleCrossRef);

                if (lobjPeoplesoftPlanCrossRef != null)
                {
                    CreateFileRecordsAndAddToCollection(lobjPeoplesoftPlanCrossRef, ibusProvider.icdoOrganization.org_code);
                    //To skip RHIC records for Main2020 and DC2025
                    if (!(ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMain2020 || ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDC2025)) //PIR 20232
                    {
                        CreateFileRecordsAndAddToCollection(lobjPeoplesoftPlanCrossRef, ibusProvider.icdoOrganization.org_code, true);
                    }
                }
            }
            else if (lclbPeopleCrossRef.Count > 0)
            {
                busPeoplesoftPlanCrossRef lobjPeoplesoftPlanCrossRef = lclbPeopleCrossRef[0];

                CreateFileRecordsAndAddToCollection(lobjPeoplesoftPlanCrossRef, ibusProvider.icdoOrganization.org_code);
                //To skip RHIC records for Main2020
                if (!(ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMain2020 || ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDC2025)) //PIR 20232
                {
                    CreateFileRecordsAndAddToCollection(lobjPeoplesoftPlanCrossRef, ibusProvider.icdoOrganization.org_code, true);
                }
            }
        }

        //Deferred Comp
        public void GeneratePeoplesoftEntryForDeferredComp(string astrProviderOrgcode, int aintEEContriPercent = 0, bool ablnIsFromDC25 = false, string astrEmpType = null)
        {
            if (iclbPeopleSoftPlanCrossRef == null)
                LoadPeopleSoftPlanCrossRef();
            Collection<busPeoplesoftPlanCrossRef> lclbPeopleCrossRef = new Collection<busPeoplesoftPlanCrossRef>();

            foreach (busPeoplesoftPlanCrossRef lobjPeopleSoftPlanCrossRef in iclbPeopleSoftPlanCrossRef)
            {
                if ((lobjPeopleSoftPlanCrossRef.icdoPeoplesoftPlanCrossRef.plan_id == ibusPersonAccount.icdoPersonAccount.plan_id) &&
                    ((!ablnIsFromDC25 && lobjPeopleSoftPlanCrossRef.icdoPeoplesoftPlanCrossRef.provider_org_code == astrProviderOrgcode) || 
                    (ablnIsFromDC25 && lobjPeopleSoftPlanCrossRef.icdoPeoplesoftPlanCrossRef.addl_ee_contribution_percent == aintEEContriPercent && astrEmpType == lobjPeopleSoftPlanCrossRef.icdoPeoplesoftPlanCrossRef.emp_type_value
                    && lobjPeopleSoftPlanCrossRef.icdoPeoplesoftPlanCrossRef.provider_org_code.IsNullOrEmpty())))
                {
                    lclbPeopleCrossRef.Add(lobjPeopleSoftPlanCrossRef);
                }
            }
            if (lclbPeopleCrossRef.Count > 1)
            {
                busPeoplesoftPlanCrossRef lobjPeoplesoftPlanCrossRef = GetPeoplesoftPlanCrossRefRecord(lclbPeopleCrossRef);
                if (lobjPeoplesoftPlanCrossRef != null)
                    CreateFileRecordsAndAddToCollection(lobjPeoplesoftPlanCrossRef, ibusProvider.icdoOrganization.org_code);
            }
            else if (lclbPeopleCrossRef.Count > 0)
            {
                busPeoplesoftPlanCrossRef lobjPeoplesoftPlanCrossRef = lclbPeopleCrossRef[0];
                CreateFileRecordsAndAddToCollection(lobjPeoplesoftPlanCrossRef, ibusProvider.icdoOrganization.org_code);
            }
        }
        //PIR 20481 : hsa health
        public void GeneratePeoplesoftEntryForGroupHealthForHSA(string astrEmpType, string astrPlanOption, string astrWellnessFlag, string astrCoverageCodeValue)
        {
            if (iclbPeopleSoftPlanCrossRef == null)
                LoadPeopleSoftPlanCrossRef();

            Collection<busPeoplesoftPlanCrossRef> lclbPeopleCrossRef = new Collection<busPeoplesoftPlanCrossRef>();

            foreach (busPeoplesoftPlanCrossRef lobjPeopleSoftPlanCrossRef in iclbPeopleSoftPlanCrossRef)
            {
                if ((lobjPeopleSoftPlanCrossRef.icdoPeoplesoftPlanCrossRef.plan_id == ibusPersonAccount.icdoPersonAccount.plan_id) &&
                  (lobjPeopleSoftPlanCrossRef.icdoPeoplesoftPlanCrossRef.emp_type_value == astrEmpType) &&
                    (lobjPeopleSoftPlanCrossRef.icdoPeoplesoftPlanCrossRef.plan_option_value == astrPlanOption) &&
                    (lobjPeopleSoftPlanCrossRef.icdoPeoplesoftPlanCrossRef.wellness_flag == astrWellnessFlag) &&
                    (lobjPeopleSoftPlanCrossRef.icdoPeoplesoftPlanCrossRef.covergae_code_required == "Y") &&
                    lobjPeopleSoftPlanCrossRef.icdoPeoplesoftPlanCrossRef.benefit_plan != busConstant.PeopleSoftFileBenefitPlanHealthDAKHDH &&
                    lobjPeopleSoftPlanCrossRef.icdoPeoplesoftPlanCrossRef.benefit_plan != busConstant.PeopleSoftFileBenefitPlanHealthHDHP &&
                    lobjPeopleSoftPlanCrossRef.icdoPeoplesoftPlanCrossRef.benefit_plan != busConstant.PeopleSoftFileBenefitPlanHealthBND)
                {
                    lclbPeopleCrossRef.Add(lobjPeopleSoftPlanCrossRef);
                }
            }
            if (lclbPeopleCrossRef.Count > 1)
            {
                if (ibusPersonAccountGhdv.icdoPersonAccountGhdv.alternate_structure_code_value == busConstant.AlternateStructureCodeHDHP)
                {
                    busPeoplesoftPlanCrossRef lobjPeoplesoftPlanCrossRefHealth = null;
                    if (astrCoverageCodeValue == busConstant.CoverageSingle)
                    {
                        if (astrWellnessFlag == busConstant.Flag_Yes)
                            lobjPeoplesoftPlanCrossRefHealth = lclbPeopleCrossRef.Where(o => o.icdoPeoplesoftPlanCrossRef.benefit_plan == busConstant.PeopleSoftFileBenefitPlanHealthHSAWS).FirstOrDefault();
                        else if (astrWellnessFlag == busConstant.Flag_No)
                            lobjPeoplesoftPlanCrossRefHealth = lclbPeopleCrossRef.Where(o => o.icdoPeoplesoftPlanCrossRef.benefit_plan == busConstant.PeopleSoftFileBenefitPlanHealthHSANWS).FirstOrDefault();
                    }
                    if (astrCoverageCodeValue == busConstant.CoverageFamily)
                    {
                        if (astrWellnessFlag == busConstant.Flag_Yes)
                            lobjPeoplesoftPlanCrossRefHealth = lclbPeopleCrossRef.Where(o => o.icdoPeoplesoftPlanCrossRef.benefit_plan == busConstant.PeopleSoftFileBenefitPlanHealthHSAWF).FirstOrDefault();
                        else if (astrWellnessFlag == busConstant.Flag_No)
                            lobjPeoplesoftPlanCrossRefHealth = lclbPeopleCrossRef.Where(o => o.icdoPeoplesoftPlanCrossRef.benefit_plan == busConstant.PeopleSoftFileBenefitPlanHealthHSANWF).FirstOrDefault();
                    }
                    if (lobjPeoplesoftPlanCrossRefHealth != null)
                    {
                        CreateFileRecordsAndAddToCollection(lobjPeoplesoftPlanCrossRefHealth, ibusProvider.icdoOrganization.org_code);
                    }
                }

                //busPeoplesoftPlanCrossRef lobjPeoplesoftPlanCrossRef = GetPeoplesoftPlanCrossRefRecord(lclbPeopleCrossRef);
                //if (lobjPeoplesoftPlanCrossRef != null)
                //{
                //    CreateFileRecordsAndAddToCollection(lobjPeoplesoftPlanCrossRef, ibusProvider.icdoOrganization.org_code);
                //}

                ibusPersonAccountGhdv.LoadPersonAccountGHDVHistory();
                busPersonAccountGhdvHistory lbusPersonAccountGhdvHistory = null;
                if (ibusPersonAccountGhdv.iclbPersonAccountGHDVHistory.Count >= 2)
                    lbusPersonAccountGhdvHistory = ibusPersonAccountGhdv.iclbPersonAccountGHDVHistory.Skip(1).FirstOrDefault();
                busPersonAccountGhdvHistory lbusPersonAccountGhdvHistoryLatest = ibusPersonAccountGhdv.iclbPersonAccountGHDVHistory.FirstOrDefault();

                if (lbusPersonAccountGhdvHistory != null && lbusPersonAccountGhdvHistoryLatest != null)
                {
                    if (lbusPersonAccountGhdvHistory.icdoPersonAccountGhdvHistory.alternate_structure_code_value == busConstant.AlternateStructureCodeHDHP && lbusPersonAccountGhdvHistoryLatest.icdoPersonAccountGhdvHistory.alternate_structure_code_value.IsNullOrEmpty())
                    {
                        busPeoplesoftPlanCrossRef lobjPeoplesoftPlanCrossRefHealth = lclbPeopleCrossRef.Where(o => o.icdoPeoplesoftPlanCrossRef.provider_org_code.IsNotNull() && o.icdoPeoplesoftPlanCrossRef.provider_org_code == busGlobalFunctions.GetData1ByCodeValue(52, busConstant.HSAProvider, iobjPassInfo)).FirstOrDefault();
                        if (lobjPeoplesoftPlanCrossRefHealth != null)
                        {
                            istrBenefitPlan = string.Empty;
                            iblnTerminatedRecord = true;
                            istrCoverageElection = "T";

                            CreateFileRecordsAndAddToCollection(lobjPeoplesoftPlanCrossRefHealth, ibusProvider.icdoOrganization.org_code);
                            iblnTerminatedRecord = false;
                        }
                    }
                }
            }
            else if (lclbPeopleCrossRef.Count > 0)
            {
                busPeoplesoftPlanCrossRef lobjPeoplesoftPlanCrossRef = lclbPeopleCrossRef[0];

                CreateFileRecordsAndAddToCollection(lobjPeoplesoftPlanCrossRef, ibusProvider.icdoOrganization.org_code);
            }
        }

        //Health
        public void GeneratePeoplesoftEntryForGroupHealth(string astrEmpType, string astrWellnessFlag)
        {
            if (iclbPeopleSoftPlanCrossRef == null)
                LoadPeopleSoftPlanCrossRef();

            Collection<busPeoplesoftPlanCrossRef> lclbPeopleCrossRef = new Collection<busPeoplesoftPlanCrossRef>();

            foreach (busPeoplesoftPlanCrossRef lobjPeopleSoftPlanCrossRef in iclbPeopleSoftPlanCrossRef)
            {
                if ((lobjPeopleSoftPlanCrossRef.icdoPeoplesoftPlanCrossRef.plan_id == ibusPersonAccount.icdoPersonAccount.plan_id) &&
                  (lobjPeopleSoftPlanCrossRef.icdoPeoplesoftPlanCrossRef.emp_type_value == astrEmpType) &&
                    (lobjPeopleSoftPlanCrossRef.icdoPeoplesoftPlanCrossRef.plan_option_value == ibusPersonAccountGhdv.icdoPersonAccountGhdv.plan_option_value) &&
                    (lobjPeopleSoftPlanCrossRef.icdoPeoplesoftPlanCrossRef.wellness_flag == astrWellnessFlag ) &&
                    (lobjPeopleSoftPlanCrossRef.icdoPeoplesoftPlanCrossRef.benefit_plan != busConstant.PeopleSoftFileBenefitPlanHealthHSAWF &&
                    lobjPeopleSoftPlanCrossRef.icdoPeoplesoftPlanCrossRef.benefit_plan != busConstant.PeopleSoftFileBenefitPlanHealthHSAWS &&
                    lobjPeopleSoftPlanCrossRef.icdoPeoplesoftPlanCrossRef.benefit_plan != busConstant.PeopleSoftFileBenefitPlanHealthHSANWF &&
                    lobjPeopleSoftPlanCrossRef.icdoPeoplesoftPlanCrossRef.benefit_plan != busConstant.PeopleSoftFileBenefitPlanHealthHSANWS &&
                    lobjPeopleSoftPlanCrossRef.icdoPeoplesoftPlanCrossRef.benefit_plan != busConstant.PeopleSoftFileBenefitPlanHealthBND)

                    )
                {
                    lclbPeopleCrossRef.Add(lobjPeopleSoftPlanCrossRef);
                }
            }
            if (lclbPeopleCrossRef.Count > 1)
            {
                if (ibusPersonAccountGhdv.icdoPersonAccountGhdv.alternate_structure_code_value == busConstant.AlternateStructureCodeHDHP)
                {
                    busPeoplesoftPlanCrossRef lobjPeoplesoftPlanCrossRefHealth = null;
                    if (astrWellnessFlag == busConstant.Flag_Yes)
                        lobjPeoplesoftPlanCrossRefHealth = lclbPeopleCrossRef.Where(o => o.icdoPeoplesoftPlanCrossRef.benefit_plan == busConstant.PeopleSoftFileBenefitPlanHealthDAKHDH).FirstOrDefault();
                    else if (astrWellnessFlag == busConstant.Flag_No)
                        lobjPeoplesoftPlanCrossRefHealth = lclbPeopleCrossRef.Where(o => o.icdoPeoplesoftPlanCrossRef.benefit_plan == busConstant.PeopleSoftFileBenefitPlanHealthHDHP).FirstOrDefault();

                    if (lobjPeoplesoftPlanCrossRefHealth != null)
                    {
                        CreateFileRecordsAndAddToCollection(lobjPeoplesoftPlanCrossRefHealth, ibusProvider.icdoOrganization.org_code);
                    }
                }
                
                
                    busPeoplesoftPlanCrossRef lobjPeoplesoftPlanCrossRef = GetPeoplesoftPlanCrossRefRecord(lclbPeopleCrossRef);
                    if (lobjPeoplesoftPlanCrossRef != null)
                    {
                        CreateFileRecordsAndAddToCollection(lobjPeoplesoftPlanCrossRef, ibusProvider.icdoOrganization.org_code);
                    }
                

                ibusPersonAccountGhdv.LoadPersonAccountGHDVHistory();
                busPersonAccountGhdvHistory lbusPersonAccountGhdvHistory = ibusPersonAccountGhdv.iclbPersonAccountGHDVHistory.Skip(1).Take(1).FirstOrDefault();
                busPersonAccountGhdvHistory lbusPersonAccountGhdvHistoryLatest = ibusPersonAccountGhdv.iclbPersonAccountGHDVHistory.Take(1).FirstOrDefault();

                if (lbusPersonAccountGhdvHistory != null && lbusPersonAccountGhdvHistoryLatest != null)
                {
                    if (lbusPersonAccountGhdvHistory.icdoPersonAccountGhdvHistory.alternate_structure_code_value == busConstant.AlternateStructureCodeHDHP && lbusPersonAccountGhdvHistoryLatest.icdoPersonAccountGhdvHistory.alternate_structure_code_value.IsNullOrEmpty())
                    {
                        busPeoplesoftPlanCrossRef lobjPeoplesoftPlanCrossRefHealth = lclbPeopleCrossRef.Where(o => o.icdoPeoplesoftPlanCrossRef.provider_org_code.IsNotNull() && o.icdoPeoplesoftPlanCrossRef.provider_org_code == busGlobalFunctions.GetData1ByCodeValue(52, busConstant.HSAProvider, iobjPassInfo)).FirstOrDefault();
                        if (lobjPeoplesoftPlanCrossRefHealth != null)
                        {
                            istrBenefitPlan = string.Empty;
                            iblnTerminatedRecord = true;
                            istrCoverageElection = "T";

                            CreateFileRecordsAndAddToCollection(lobjPeoplesoftPlanCrossRefHealth, ibusProvider.icdoOrganization.org_code);
                            iblnTerminatedRecord = false;
                        }
                    }
                }
            }
            else if (lclbPeopleCrossRef.Count > 0)
            {
                busPeoplesoftPlanCrossRef lobjPeoplesoftPlanCrossRef = lclbPeopleCrossRef[0];

                CreateFileRecordsAndAddToCollection(lobjPeoplesoftPlanCrossRef, ibusProvider.icdoOrganization.org_code);
            }
        }

        //Dental and Vision
        public void GeneratePeoplesoftEntryForDentalAndVision()
        {
            if (iclbPeopleSoftPlanCrossRef == null)
                LoadPeopleSoftPlanCrossRef();

            Collection<busPeoplesoftPlanCrossRef> lclbPeopleCrossRef = new Collection<busPeoplesoftPlanCrossRef>();

            foreach (busPeoplesoftPlanCrossRef lobjPeopleSoftPlanCrossRef in iclbPeopleSoftPlanCrossRef)
            {
                if (lobjPeopleSoftPlanCrossRef.icdoPeoplesoftPlanCrossRef.plan_id == ibusPersonAccount.icdoPersonAccount.plan_id)
                {
                    lclbPeopleCrossRef.Add(lobjPeopleSoftPlanCrossRef);
                }
            }
            if (lclbPeopleCrossRef.Count > 1)
            {
                busPeoplesoftPlanCrossRef lobjPeoplesoftPlanCrossRef = GetPeoplesoftPlanCrossRefRecord(lclbPeopleCrossRef);
                if (lobjPeoplesoftPlanCrossRef != null)
                {
                    CreateFileRecordsAndAddToCollection(lobjPeoplesoftPlanCrossRef, string.Empty);
                }
            }
            else if (lclbPeopleCrossRef.Count > 0)
            {
                busPeoplesoftPlanCrossRef lobjPeoplesoftPlanCrossRef = lclbPeopleCrossRef[0];
                CreateFileRecordsAndAddToCollection(lobjPeoplesoftPlanCrossRef, string.Empty);
            }
        }

        //EAP
        public void GeneratePeoplesoftEntryForEAP()
        {
            if (iclbPeopleSoftPlanCrossRef == null)
                LoadPeopleSoftPlanCrossRef();

            Collection<busPeoplesoftPlanCrossRef> lclbPeopleCrossRef = new Collection<busPeoplesoftPlanCrossRef>();
            foreach (busPeoplesoftPlanCrossRef lobjPeopleSoftPlanCrossRef in iclbPeopleSoftPlanCrossRef)
            {
                if ((lobjPeopleSoftPlanCrossRef.icdoPeoplesoftPlanCrossRef.plan_id == ibusPersonAccount.icdoPersonAccount.plan_id) &&
                    (lobjPeopleSoftPlanCrossRef.icdoPeoplesoftPlanCrossRef.provider_org_code == ibusProvider.icdoOrganization.org_code))
                {
                    lclbPeopleCrossRef.Add(lobjPeopleSoftPlanCrossRef);
                }
            }
            if (lclbPeopleCrossRef.Count > 1)
            {
                busPeoplesoftPlanCrossRef lobjPeoplesoftPlanCrossRef = GetPeoplesoftPlanCrossRefRecord(lclbPeopleCrossRef);
                if (lobjPeoplesoftPlanCrossRef != null)
                    CreateFileRecordsAndAddToCollection(lobjPeoplesoftPlanCrossRef, ibusProvider.icdoOrganization.org_code);
            }
            else if (lclbPeopleCrossRef.Count > 0)
            {
                busPeoplesoftPlanCrossRef lobjPeoplesoftPlanCrossRef = lclbPeopleCrossRef[0];
                CreateFileRecordsAndAddToCollection(lobjPeoplesoftPlanCrossRef, ibusProvider.icdoOrganization.org_code);
            }
        }

        //Life
        public void GeneratePeoplesoftEntryForLife(string astrEmpTypeValue)
        {
            if (iclbPeopleSoftPlanCrossRef == null)
                LoadPeopleSoftPlanCrossRef();

            Collection<busPeoplesoftPlanCrossRef> lclbPeopleCrossRef = new Collection<busPeoplesoftPlanCrossRef>();

            foreach (busPeoplesoftPlanCrossRef lobjPeopleSoftPlanCrossRef in iclbPeopleSoftPlanCrossRef)
            {
                if ((lobjPeopleSoftPlanCrossRef.icdoPeoplesoftPlanCrossRef.plan_id == ibusPersonAccount.icdoPersonAccount.plan_id) &&
                    (lobjPeopleSoftPlanCrossRef.icdoPeoplesoftPlanCrossRef.level_of_coverage_value == istrLevelOfCoverage) &&
                    (lobjPeopleSoftPlanCrossRef.icdoPeoplesoftPlanCrossRef.emp_type_value == astrEmpTypeValue))
                {
                    lclbPeopleCrossRef.Add(lobjPeopleSoftPlanCrossRef);
                }
            }
            GetPeopleSoftPlanRecordsForLife(lclbPeopleCrossRef);
        }

        //Flex Comp
        public void GeneratePeoplesoftEntryForFlex()
        {
            if (iclbPeopleSoftPlanCrossRef == null)
                LoadPeopleSoftPlanCrossRef();

            Collection<busPeoplesoftPlanCrossRef> lclbPeopleCrossRef = new Collection<busPeoplesoftPlanCrossRef>();
            if (ibusPersonAccount.icdoPersonAccount.plan_id != busConstant.PlanIdGroupLife)
            {
                foreach (busPeoplesoftPlanCrossRef lobjPeopleSoftPlanCrossRef in iclbPeopleSoftPlanCrossRef)
                {
                    if ((lobjPeopleSoftPlanCrossRef.icdoPeoplesoftPlanCrossRef.plan_id == ibusPersonAccount.icdoPersonAccount.plan_id) &&
                       (lobjPeopleSoftPlanCrossRef.icdoPeoplesoftPlanCrossRef.level_of_coverage_value == istrLevelOfCoverage)
                        )
                    {
                        lclbPeopleCrossRef.Add(lobjPeopleSoftPlanCrossRef);
                    }
                }
            }
            if (lclbPeopleCrossRef.Count > 1)
            {
                busPeoplesoftPlanCrossRef lobjPeoplesoftPlanCrossRef = GetPeoplesoftPlanCrossRefRecord(lclbPeopleCrossRef);
                if (lobjPeoplesoftPlanCrossRef != null)
                {
                    CreateFileRecordsAndAddToCollection(lobjPeoplesoftPlanCrossRef, ibusProvider.icdoOrganization.org_code);
                }
            }
            else if (lclbPeopleCrossRef.Count > 0)
            {
                busPeoplesoftPlanCrossRef lobjPeoplesoftPlanCrossRef = lclbPeopleCrossRef[0];
                CreateFileRecordsAndAddToCollection(lobjPeoplesoftPlanCrossRef,ibusProvider.icdoOrganization.org_code);
            }
        }

        private bool CheckPremiumFlagChecked()
        {
            if (iblnAnnualEnrollment) 
            {
                DateTime ldtAnnualEnrollment = Convert.ToDateTime(busGlobalFunctions.GetData3ByCodeValue(52, busConstant.AnnualEnrollment, iobjPassInfo));
                if (ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDental
                    || ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdVision)
                {
                    busPersonAccountGhdvHistory lobjGhdvHistory = ibusPersonAccountGhdv.LoadHistoryByDate(ldtAnnualEnrollment);

                    if (lobjGhdvHistory.IsNotNull() && (lobjGhdvHistory.icdoPersonAccountGhdvHistory.ps_file_change_event_value == busConstant.AnnualEnrollment
                        || lobjGhdvHistory.icdoPersonAccountGhdvHistory.ps_file_change_event_value == busConstant.AnnualEnrollmentWaived))
                    {
                        if (lobjGhdvHistory.IsNotNull() && lobjGhdvHistory.icdoPersonAccountGhdvHistory.premium_conversion_indicator_flag == busConstant.Flag_Yes && lobjGhdvHistory.icdoPersonAccountGhdvHistory.reason_value == busConstant.AnnualEnrollment)
                            return true;
                    }
                }
                else if (ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupLife)
                {
                    busPersonAccountLifeHistory lobjPALifeHistory = new busPersonAccountLifeHistory();
                    lobjPALifeHistory.icdoPersonAccountLifeHistory = new cdoPersonAccountLifeHistory();
                    busPersonAccountLifeOption lobjPALifeOption = new busPersonAccountLifeOption { icdoPersonAccountLifeOption = new cdoPersonAccountLifeOption() };
                    lobjPALifeOption = ibusPersonAccountLife.iclbLifeOption.Where(i => i.icdoPersonAccountLifeOption.level_of_coverage_value == istrLevelOfCoverage).FirstOrDefault();
                    lobjPALifeHistory = ibusPersonAccountLife.LoadHistoryByDate(lobjPALifeOption, ldtAnnualEnrollment);

                    if (lobjPALifeHistory.IsNotNull() && (lobjPALifeHistory.icdoPersonAccountLifeHistory.ps_file_change_event_value == busConstant.AnnualEnrollment
                        || lobjPALifeHistory.icdoPersonAccountLifeHistory.ps_file_change_event_value == busConstant.AnnualEnrollmentWaived))
                    {
                        if (lobjPALifeHistory.IsNotNull() && lobjPALifeHistory.icdoPersonAccountLifeHistory.premium_conversion_indicator_flag == busConstant.Flag_Yes && lobjPALifeHistory.icdoPersonAccountLifeHistory.reason_value == busConstant.AnnualEnrollment)
                            return true;
                    }
                }
            }
            else
            {
                if (idtEffectiveDate == DateTime.MinValue)
                    idtEffectiveDate = ibusPersonAccount.icdoPersonAccount.history_change_date;

                if (ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDental
                      || ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdVision)
                {
                    busPersonAccountGhdvHistory lobjGhdvHistory = ibusPersonAccountGhdv.LoadHistoryByDate(idtEffectiveDate);
                    if (lobjGhdvHistory.IsNotNull() && lobjGhdvHistory.icdoPersonAccountGhdvHistory.premium_conversion_indicator_flag == busConstant.Flag_Yes)
                        return true;
                }
                else if (ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupLife)
                {
                    busPersonAccountLifeHistory lobjPALifeHistory = new busPersonAccountLifeHistory();
                    lobjPALifeHistory.icdoPersonAccountLifeHistory = new cdoPersonAccountLifeHistory();
                    busPersonAccountLifeOption lobjPALifeOption = new busPersonAccountLifeOption { icdoPersonAccountLifeOption = new cdoPersonAccountLifeOption() };
                    lobjPALifeOption = ibusPersonAccountLife.iclbLifeOption.Where(i => i.icdoPersonAccountLifeOption.level_of_coverage_value == istrLevelOfCoverage).FirstOrDefault();
                    lobjPALifeHistory = ibusPersonAccountLife.LoadHistoryByDate(lobjPALifeOption, idtEffectiveDate);
                    if (lobjPALifeHistory.IsNotNull() && lobjPALifeHistory.icdoPersonAccountLifeHistory.premium_conversion_indicator_flag == busConstant.Flag_Yes)
                        return true;
                }
            }
            return false;
        }

        //Get The Org Group Value from the Employer Details and Org Group Value is used in getting the Coverage Begin Date,Election Date,Deduction Begin Date
        public void GetOrgGroupValue(busOrganization aobjOrganization)
        {
            if (aobjOrganization.icdoOrganization.peoplesoft_org_group_value == busConstant.PeopleSoftOrgGroupValueState)
                istrOrgGroupValue = busConstant.PeopleSoftOrgGroupValueState;

            else if (aobjOrganization.icdoOrganization.peoplesoft_org_group_value == busConstant.PeopleSoftOrgGroupValueBND)
                istrOrgGroupValue = busConstant.PeopleSoftOrgGroupValueBND;

            else if (aobjOrganization.icdoOrganization.peoplesoft_org_group_value == busConstant.PeopleSoftOrgGroupValueHigherEd)
                istrOrgGroupValue = busConstant.PeopleSoftOrgGroupValueHigherEd;

            else if (aobjOrganization.icdoOrganization.peoplesoft_org_group_value == busConstant.PeopleSoftOrgGroupValueNonPSParoll)
                istrOrgGroupValue = busConstant.PeopleSoftOrgGroupValueNonPSParoll;
        }

        private void CreateFileRecordsAndAddToCollection(busPeoplesoftPlanCrossRef aobjPeoplesoftPlanCrossRef, string astrProviderOrgcode, bool ablnRhicEntry = false)
        {
            busDailyPersonAccountPeopleSoft lobjDailyPAPeopleSoft = new busDailyPersonAccountPeopleSoft();
            Collection<busPeoplesoftPlanOrgCrossRef> lclbPeoplesoftPlanOrgCrossRef = new Collection<busPeoplesoftPlanOrgCrossRef>();

            if (iclbDailyPersonAccountPeopleSoft == null)
                iclbDailyPersonAccountPeopleSoft = new Collection<busDailyPersonAccountPeopleSoft>();

            if (iclbPeoplesoftPlanOrgCrossRef == null)
                LoadPeoplePlanOrgCrossRef();
            busOrganization lbusOrganization = ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization;
            var lclbFilteredSourceData = new List<busPeoplesoftPlanOrgCrossRef>();
            bool lblnRecordFound = iclbPeoplesoftPlanOrgCrossRef.Any(i => i.icdoPeoplesoftPlanOrgCrossRef.org_code == lbusOrganization.icdoOrganization.org_code);
            if (lblnRecordFound)
                lclbFilteredSourceData = iclbPeoplesoftPlanOrgCrossRef.Where(i => i.icdoPeoplesoftPlanOrgCrossRef.org_code == lbusOrganization.icdoOrganization.org_code).ToList();
            else
                lclbFilteredSourceData = iclbPeoplesoftPlanOrgCrossRef.Where(i => i.icdoPeoplesoftPlanOrgCrossRef.org_code == null).ToList();

            GetOrgGroupValue(lbusOrganization);

            foreach (busPeoplesoftPlanOrgCrossRef lobjPeoplesoftPlanOrgCrossRef in lclbFilteredSourceData)
            {
                if (lbusOrganization.icdoOrganization.peoplesoft_org_group_value == busConstant.PeopleSoftOrgGroupValueNonPSParoll &&
                   !(istrLevelOfCoverage == busConstant.FlexLevelOfCoverageDependentSpending || istrLevelOfCoverage == busConstant.FlexLevelOfCoverageMedicareSpending))
                    continue;

                if (iblnAnnualEnrollment)
                {
                     istrPSFileChangeEvent = istrHistoryPSFileChangeEventValue;
                }
                else
                {
                    if (ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDeferredCompensation)
                    {
                        if (idtDeductionEndDate != DateTime.MinValue)
                            istrPSFileChangeEvent = busConstant.DeferredCompEndDateProvider;
                        else
                            istrPSFileChangeEvent = busConstant.DeferredCompStartDateProvider;
                    }
                    else if (ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdFlex)
                        istrPSFileChangeEvent = istrHistoryPSFileChangeEventValue;
                    else if (ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth &&  idtHSAStartDate != DateTime.MinValue )
                    {
                        if (idtHSAStartDate != DateTime.MinValue && idtHSAEndDate == DateTime.MinValue)
                            istrPSFileChangeEvent = busConstant.HSAStart;
                        else if (idtHSAEndDate != DateTime.MinValue)
                            istrPSFileChangeEvent = busConstant.HSAEnd;
                    }
                    else if (ibusPersonAccount.icdoPersonAccount.ps_file_change_event_value == null)
                        istrPSFileChangeEvent = busConstant.NewEnrollment;
                    else
                        istrPSFileChangeEvent = ibusPersonAccount.icdoPersonAccount.ps_file_change_event_value;

                }

                if ((lobjPeoplesoftPlanOrgCrossRef.icdoPeoplesoftPlanOrgCrossRef.plan_id == ibusPersonAccount.icdoPersonAccount.plan_id) &&
                (lobjPeoplesoftPlanOrgCrossRef.icdoPeoplesoftPlanOrgCrossRef.people_soft_org_group_value == istrOrgGroupValue) &&
                lobjPeoplesoftPlanOrgCrossRef.icdoPeoplesoftPlanOrgCrossRef.ps_file_change_event_value == istrPSFileChangeEvent)
                {
                    lclbPeoplesoftPlanOrgCrossRef.Add(lobjPeoplesoftPlanOrgCrossRef);
                    if ((!lblnAllWaived && (!iblnTerminatedRecord && !iblnIsPlanOptionSuspended))
                        || (ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupLife && (!iblnIsPlanOptionSuspended && !iblnTerminatedRecord)))
                        lobjDailyPAPeopleSoft.istrCoverageElection = lobjPeoplesoftPlanOrgCrossRef.icdoPeoplesoftPlanOrgCrossRef.ps_coverage_election_value;
                }
            }

            if (lclbPeoplesoftPlanOrgCrossRef.Count > 0)
            {
                #region Plan Type
                if (ibusPersonAccount.icdoPersonAccount.plan_id != busConstant.PlanIdDeferredCompensation)
                {
                    if (ablnRhicEntry)
                        lobjDailyPAPeopleSoft.istrPlanType = aobjPeoplesoftPlanCrossRef.icdoPeoplesoftPlanCrossRef.rhic_plan_type;
                    else
                        lobjDailyPAPeopleSoft.istrPlanType = aobjPeoplesoftPlanCrossRef.icdoPeoplesoftPlanCrossRef.plan_type;
                }
                else
                    lobjDailyPAPeopleSoft.istrPlanType =
                        aobjPeoplesoftPlanCrossRef.icdoPeoplesoftPlanCrossRef.plan_type;
                #endregion

                #region Benefit Plan
                if (aobjPeoplesoftPlanCrossRef.icdoPeoplesoftPlanCrossRef.benefit_plan_required == busConstant.Flag_Yes)
                {
                    if (iblnTerminatedRecord)
                        lobjDailyPAPeopleSoft.istrCoverageElection = istrCoverageElection;

                    if (iblnIsRetirementPlan && !iblnBenefitFromPS && ibusPersonAccount.icdoPersonAccount.plan_id != busConstant.PlanIdDC2025)
                    {
                        if (ablnRhicEntry && lobjDailyPAPeopleSoft.istrCoverageElection != "T")
                            lobjDailyPAPeopleSoft.istrBenefitPlan = istrRHICBenefitPlanforRetirement;
                        else if (lobjDailyPAPeopleSoft.istrCoverageElection != "T")
                            lobjDailyPAPeopleSoft.istrBenefitPlan = istrBenefitPlanforRetirement;
                    }
                    else if (ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDC2025)
                    {
                        if (lobjDailyPAPeopleSoft.istrCoverageElection != "T")
                            lobjDailyPAPeopleSoft.istrBenefitPlan = aobjPeoplesoftPlanCrossRef.icdoPeoplesoftPlanCrossRef.benefit_plan;
                    }
                    else
                    {
                        if (ablnRhicEntry && lobjDailyPAPeopleSoft.istrCoverageElection != "T")
                            lobjDailyPAPeopleSoft.istrBenefitPlan = aobjPeoplesoftPlanCrossRef.icdoPeoplesoftPlanCrossRef.rhic_benefit_plan;
                        else if (lobjDailyPAPeopleSoft.istrCoverageElection != "T")
                            lobjDailyPAPeopleSoft.istrBenefitPlan = aobjPeoplesoftPlanCrossRef.icdoPeoplesoftPlanCrossRef.benefit_plan;
                    }
                }
                if (lobjDailyPAPeopleSoft.istrCoverageElection == "T" && ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdFlex)
                    lobjDailyPAPeopleSoft.istrBenefitPlan = "";
                #endregion

                #region Coverage Code
                if (aobjPeoplesoftPlanCrossRef.icdoPeoplesoftPlanCrossRef.covergae_code_required == busConstant.Flag_Yes)
                {
                    if (ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth && lobjDailyPAPeopleSoft.istrCoverageElection == "T")
                        lobjDailyPAPeopleSoft.istrCoverageCode = string.Empty;
                    else
                        lobjDailyPAPeopleSoft.istrCoverageCode = GetCoverageCode(lobjDailyPAPeopleSoft.istrBenefitPlan, istrLevelOfCoverage, astrProviderOrgcode);
                }
                #endregion

                #region Coverage Election
                //Logic changed a bit.. If Coverage election flag is 'N', then set Coverage Election as null.
                if (aobjPeoplesoftPlanCrossRef.icdoPeoplesoftPlanCrossRef.coverage_election_required == busConstant.Flag_No)
                {
                    lobjDailyPAPeopleSoft.istrCoverageElection = string.Empty;
                }
                #endregion

                #region Flat Amount
                if (aobjPeoplesoftPlanCrossRef.icdoPeoplesoftPlanCrossRef.flat_amount_required == busConstant.Flag_Yes)
                    lobjDailyPAPeopleSoft.idecFlatAmount = idecFlatAmount;
                else
                    lobjDailyPAPeopleSoft.idecFlatAmount = 0.0m;
                #endregion

                #region Calculation Routine
                if (aobjPeoplesoftPlanCrossRef.icdoPeoplesoftPlanCrossRef.calculation_routine_required == busConstant.Flag_Yes)
                {
                    lobjDailyPAPeopleSoft.istrCalculationRoutine = "A";
                }
                #endregion

                #region Coverage begin Date, Deduction begin date, Election Date
                foreach (busPeoplesoftPlanOrgCrossRef lobjPeoplesoftPlanOrgCrossRef in lclbPeoplesoftPlanOrgCrossRef)
                {
                    if (lobjPeoplesoftPlanOrgCrossRef.icdoPeoplesoftPlanOrgCrossRef.people_soft_file_date_type_value == busConstant.PeopleSoftCoverageBeginDate)
                    {
                        lobjDailyPAPeopleSoft.idtCoverageBeginDate =
                            GetDateFields(lobjPeoplesoftPlanOrgCrossRef.icdoPeoplesoftPlanOrgCrossRef.people_soft_file_date_value);
                    }
                    else if (lobjPeoplesoftPlanOrgCrossRef.icdoPeoplesoftPlanOrgCrossRef.people_soft_file_date_type_value == busConstant.PeopleSoftDeductionBeginDate)
                    {
                        lobjDailyPAPeopleSoft.idtDeductionBeginDate =
                            GetDateFields(lobjPeoplesoftPlanOrgCrossRef.icdoPeoplesoftPlanOrgCrossRef.people_soft_file_date_value);
                    }
                    else if (lobjPeoplesoftPlanOrgCrossRef.icdoPeoplesoftPlanOrgCrossRef.people_soft_file_date_type_value == busConstant.PeopleSoftElectionDate)
                    {
                        lobjDailyPAPeopleSoft.idtElectionDate =
                            GetDateFields(lobjPeoplesoftPlanOrgCrossRef.icdoPeoplesoftPlanOrgCrossRef.people_soft_file_date_value);
                    }
                }
                #endregion

                if (istrBenefitPlan == busConstant.PeopleSoftFileBenefitPlanLifeSuppl && idecSuppCoverageAmount == 0.0m)
                {
                    lobjDailyPAPeopleSoft.istrCoverageElection = "T";
                    lobjDailyPAPeopleSoft.istrBenefitPlan = string.Empty;
                }
               iclbDailyPersonAccountPeopleSoft.Add(lobjDailyPAPeopleSoft);
            }
        }

        private string GetCoverageCode(string astrBenefitPlan, string astrLoc, string astrOrgcode)
        {
            if (iclbOrgPlanGroupHealthMedicarePartDCoverageRef == null)
                LoadHealthCoverageRefCacheData();

            if (iclbPeoplesoftPlanTypeCodeValues == null)
                LoadPeoplesoftPlanTypeValues();

            string lstrCoverageCode = null;
            if (astrBenefitPlan == null)
                astrBenefitPlan = string.Empty;
            if (astrLoc == null)
                astrLoc = string.Empty;
            if (astrOrgcode == null)
                astrOrgcode = string.Empty;

            if (ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth)
            {
                string lstrHealthLoc = iclbOrgPlanGroupHealthMedicarePartDCoverageRef.Where(o =>
                    o.icdoOrgPlanGroupHealthMedicarePartDCoverageRef.coverage_code ==
                    ibusPersonAccountGhdv.icdoPersonAccountGhdv.coverage_code).Select(o =>
                        o.icdoOrgPlanGroupHealthMedicarePartDCoverageRef.short_description).FirstOrDefault() ?? string.Empty;

                if (lstrHealthLoc.ToLower() == busConstant.GroupHealthLevelofCoverageSingle.ToLower())
                    lstrCoverageCode = "A";

                else if (lstrHealthLoc.Length > 10 && lstrHealthLoc.Substring(0, 9).ToLower() == busConstant.GroupHealthLevelofCoverage1Medicare.ToLower())
                    lstrCoverageCode = "A";
                else
                    lstrCoverageCode = "B";
            }
            else
            {
                foreach (cdoCodeValue lcdoPlanType in iclbPeoplesoftPlanTypeCodeValues)
                {
                    if ((lcdoPlanType.description.Substring(0, 6).Trim().ToLower() == astrBenefitPlan.ToLower()) && (lcdoPlanType.data2 == astrLoc) && (lcdoPlanType.data3 == astrOrgcode))
                    {
                        lstrCoverageCode = lcdoPlanType.data1;
                        break;
                    }
                }
            }
            return lstrCoverageCode;
        }

        private DateTime GetDateFields(string strDateValue)
        {
            DataTable idtEmploymentDetails = null;
            if (iblnIsFromDeferredComp)//PIR 20702
                idtEmploymentDetails = busBase.Select("cdoPersonEmployment.LoadPersonEmploymentForDailyPeopleSoftByEmpId", new object[2] { ibusPersonAccount.icdoPersonAccount.person_id, iintPersonEmpId });
            else
                idtEmploymentDetails = busBase.Select("cdoPersonEmployment.LoadPersonEmploymentForDailyPeopleSoft", new object[1] { ibusPersonAccount.icdoPersonAccount.person_id });

            DateTime ldtdate = DateTime.MinValue;
            string lstrDate = string.Empty;
            DataRow[] ldarrFilteredEmploymentDetails =
                idtEmploymentDetails.FilterTable(busConstant.DataType.Numeric, "person_account_id", ibusPersonAccount.icdoPersonAccount.person_account_id);
            switch (strDateValue)
            {
                case busConstant.PeopleSoftDateBlank:
                    break;
                case busConstant.PeopleSoftDateDateFileGenerated:
                    ldtdate = DateTime.Today;
                    break;
                case busConstant.PeopleSoftHistoryChangeDate:
                    ldtdate = ibusPersonAccount.icdoPersonAccount.history_change_date;
                    break;
                case busConstant.PeopleSoftTemproaryEmploymentStartDate:
                    foreach (DataRow dr in ldarrFilteredEmploymentDetails)
                    {
                        if (dr["type_value"] != DBNull.Value && dr["d_end_date"] == DBNull.Value && dr["d_start_date"] == DBNull.Value &&
                            Convert.ToString(dr["type_value"]) == busConstant.PersonJobTypeTemporary &&
                            ldarrFilteredEmploymentDetails[0]["d_start_date"] != DBNull.Value &&
                            Convert.ToDateTime(ldarrFilteredEmploymentDetails[0]["d_start_date"]) != DateTime.MinValue)
                        {
                            ldtdate = Convert.ToDateTime(ldarrFilteredEmploymentDetails[0]["d_start_date"]);
                        }
                    }
                    break;
                case busConstant.PeopleSoftEmploymentStartDate:
                    if (ldarrFilteredEmploymentDetails.Count() > 0)
                    {
                        ldtdate = ldarrFilteredEmploymentDetails[0]["h_start_date"] != DBNull.Value ?
                            Convert.ToDateTime(ldarrFilteredEmploymentDetails[0]["h_start_date"]) : DateTime.MinValue;
                    }
                    break;
                case busConstant.PeopleSoftDatePlanParticipationStartDate:

                    ldtdate = ibusPersonAccount.icdoPersonAccount.start_date;
                    break;
                case busConstant.PeopleSoftDateDeductionBeginDate:
                    ldtdate = idtDeductionStartDate;
                    break;
                case busConstant.PeopleSoftDateFirstDayPriorToPlanParticipationStartDate:
                    if (ibusPersonAccount.icdoPersonAccount.start_date != DateTime.MinValue)
                    {
                        ldtdate = new DateTime(ibusPersonAccount.icdoPersonAccount.start_date.AddMonths(-1).Year,
                                                    ibusPersonAccount.icdoPersonAccount.start_date.AddMonths(-1).Month, 1);
                    }
                    break;
                case busConstant.PeopleSoftDatePlanOptionStartDate:

                    ldtdate = idtPlanOptionStartDate;
                    break;
                case busConstant.PeopleSoftDateFirstDayPriorToPlanOptionStartDate:
                    if (idtPlanOptionStartDate != DateTime.MinValue)
                    {
                        ldtdate = new DateTime(idtPlanOptionStartDate.AddMonths(-1).Year,
                                                    idtPlanOptionStartDate.AddMonths(-1).Month, 1);
                    }
                    break;
                case busConstant.PeopleSoftFirstDayoftheNextmonthofEmploymentHeaderEndDate:
                    if (ldarrFilteredEmploymentDetails.Count() > 0)
                    {
                        ldtdate = ldarrFilteredEmploymentDetails[0]["h_end_date"] != DBNull.Value ?
                            Convert.ToDateTime(ldarrFilteredEmploymentDetails[0]["h_end_date"]).GetFirstDayofNextMonth() : DateTime.MinValue;
                    }
                    break;
                case busConstant.PeopleSoftFirstDayoftheMonthPriortoHistoryChangeDate:
                    if (ibusPersonAccount.icdoPersonAccount.history_change_date != DateTime.MinValue)
                    {
                        if (ldarrFilteredEmploymentDetails.Count() > 0)
                        {
                            //PIR 20690 :  More Outbound File Updates for Temp to Permanent Employees
                            if ((ldarrFilteredEmploymentDetails.Count() > 1) &&
                               (ldarrFilteredEmploymentDetails[0]["type_value"].ToString() == busConstant.PersonJobTypePermanent &&
                                    ldarrFilteredEmploymentDetails[1]["type_value"].ToString() == busConstant.PersonJobTypeTemporary))
                            {
                                ldtdate = Convert.ToDateTime(ldarrFilteredEmploymentDetails[0]["start_date"]);
                            }
                            else
                            {
                                if (ibusPersonAccount.icdoPersonAccount.history_change_date.AddMonths(-1).GetFirstDayofCurrentMonth() < Convert.ToDateTime(ldarrFilteredEmploymentDetails[0]["h_start_date"]))
                                    ldtdate = Convert.ToDateTime(ldarrFilteredEmploymentDetails[0]["h_start_date"]);
                                else
                                    ldtdate = ibusPersonAccount.icdoPersonAccount.history_change_date.AddMonths(-1).GetFirstDayofCurrentMonth();
                            }
                        }
                    }
                    break;
                case busConstant.PeopleSoftFirstDayofthemonthofEmploymentHeaderEndDate:
                    if (ldarrFilteredEmploymentDetails.Count() > 0)
                    {
                        ldtdate = ldarrFilteredEmploymentDetails[0]["h_end_date"] != DBNull.Value ?
                            Convert.ToDateTime(ldarrFilteredEmploymentDetails[0]["h_end_date"]).GetFirstDayofCurrentMonth() : DateTime.MinValue;
                    }
                    break;
                case busConstant.PeopleSoftFirstDayoftheMonthofPermanentEmploymentEndDate:
                    foreach (DataRow dr in ldarrFilteredEmploymentDetails)
                    {
                        if (dr["type_value"] != DBNull.Value && dr["d_end_date"] != DBNull.Value &&
                            Convert.ToString(dr["type_value"]) == busConstant.PersonJobTypePermanent &&
                            ldarrFilteredEmploymentDetails[0]["d_end_date"] != DBNull.Value &&
                            Convert.ToDateTime(ldarrFilteredEmploymentDetails[0]["d_end_date"]) != DateTime.MinValue)
                        {
                            ldtdate = Convert.ToDateTime(ldarrFilteredEmploymentDetails[0]["d_end_date"]).GetFirstDayofCurrentMonth();
                        }
                    }
                    break;
                case busConstant.PeopleSoftFirstDayoftheMonthFollowingHistoryChangeDate:
                    ldtdate = ibusPersonAccount.icdoPersonAccount.history_change_date.GetFirstDayofNextMonth();
                    break;
                case busConstant.PeopleSoftFirstDayoftheMonthFollowingTemproaryEmploymentStartDate:
                    foreach (DataRow dr in ldarrFilteredEmploymentDetails)
                    {
                        if (dr["type_value"] != DBNull.Value && dr["d_end_date"] == DBNull.Value && dr["d_start_date"] == DBNull.Value &&
                            Convert.ToString(dr["type_value"]) == busConstant.PersonJobTypeTemporary &&
                            ldarrFilteredEmploymentDetails[0]["d_start_date"] != DBNull.Value &&
                            Convert.ToDateTime(ldarrFilteredEmploymentDetails[0]["d_start_date"]) != DateTime.MinValue)
                        {
                            ldtdate = Convert.ToDateTime(ldarrFilteredEmploymentDetails[0]["d_start_date"]).GetFirstDayofNextMonth();
                        }
                    }
                    break;
                case busConstant.PeopleSoftAnnualEnrollmentCoverageBeginDate:
                    lstrDate = busGlobalFunctions.GetData1ByCodeValue(362, busConstant.PeopleSoftAnnualEnrollmentCoverageBeginDate, iobjPassInfo);
                    if (!string.IsNullOrEmpty(lstrDate))
                        ldtdate = Convert.ToDateTime(lstrDate);
                    break;
                case busConstant.PeopleSoftAnnualEnrollmentDeductionBeginDate:
                    lstrDate = busGlobalFunctions.GetData1ByCodeValue(362, busConstant.PeopleSoftAnnualEnrollmentDeductionBeginDate, iobjPassInfo);
                    if (!string.IsNullOrEmpty(lstrDate))
                        ldtdate = Convert.ToDateTime(lstrDate);
                    break;
                //Deferred comp dates
                case busConstant.DeductionEndDateDefCompProvider:
                    if (idtDeductionEndDate != DateTime.MinValue)
                    {
                        if (ldarrFilteredEmploymentDetails.Count() > 0)
                        {
                            //PIR 21398 - to make DEDP populate the 1st of the month following the date 
                            if (idtDeductionEndDate < Convert.ToDateTime(ldarrFilteredEmploymentDetails[0]["h_start_date"]))
                                ldtdate = Convert.ToDateTime(ldarrFilteredEmploymentDetails[0]["h_start_date"]).GetFirstDayofNextMonth();
                            else if (idtDeductionStartDate == idtDeductionEndDate)//PIR 26897
                                ldtdate = idtDeductionEndDate;
                            else
                                ldtdate = idtDeductionEndDate.GetFirstDayofNextMonth();
                        }
                    }
                    break;
                case busConstant.DeductionStartDateMonthPriorDefCompProvider:
                    if (idtDeductionStartDate != DateTime.MinValue)
                    {
                        if (ldarrFilteredEmploymentDetails.Count() > 0)
                        {
                            if (idtDeductionStartDate.AddMonths(-1).GetFirstDayofCurrentMonth() < Convert.ToDateTime(ldarrFilteredEmploymentDetails[0]["h_start_date"]))
                                ldtdate = Convert.ToDateTime(ldarrFilteredEmploymentDetails[0]["h_start_date"]);
                            else
                                ldtdate = idtDeductionStartDate.AddMonths(-1).GetFirstDayofCurrentMonth();
                        }
                    }
                    break;
                case busConstant.DeductionEndDateMonthpriorDefCompProvider:
                    if (idtDeductionEndDate != DateTime.MinValue)
                    {
                        if (ldarrFilteredEmploymentDetails.Count() > 0)
                        {
                            if (idtDeductionEndDate.GetFirstDayofCurrentMonth() < Convert.ToDateTime(ldarrFilteredEmploymentDetails[0]["h_start_date"]))
                                ldtdate = Convert.ToDateTime(ldarrFilteredEmploymentDetails[0]["h_start_date"]);
                            else
                                ldtdate = idtDeductionEndDate.GetFirstDayofCurrentMonth();
                        }
                    }
                    break;
                    //PIR 20481 : start
                case busConstant.PeopleSoftDateFirstDayMonthPriorToHSAStartDate:
                    //First Day of the Month Prior to HSA Start Date – S / b Latest of 1st of month prior to SGT_PERSON_ACCOUNT_GHDV_HSA.CONTRIBUTION_START_DATE or Employment Start Date
                    if (ibusPersonAccountGhdvHSA.icdoPersonAccountGhdvHsa.contribution_start_date != DateTime.MinValue)
                    {
                        ldtdate = new DateTime(ibusPersonAccountGhdvHSA.icdoPersonAccountGhdvHsa .contribution_start_date.AddMonths(-1).Year,
                                                    ibusPersonAccountGhdvHSA.icdoPersonAccountGhdvHsa.contribution_start_date.AddMonths(-1).Month, 1);
                    }
                    break;
                case busConstant.PeopleSoftDateHSARecordStartDate:
                    //HSA record Start Date – S / b SGT_PERSON_ACCOUNT_GHDV_HSA.CONTRIBUTION_START_DATE
                    if (ibusPersonAccountGhdvHSA.icdoPersonAccountGhdvHsa.contribution_start_date != DateTime.MinValue)
                        ldtdate = ibusPersonAccountGhdvHSA.icdoPersonAccountGhdvHsa.contribution_start_date;
                        break;
                case busConstant.PeopleSoftDateFirstDayMonthPriorToHSAEndDate:
                    //First Day of the Month Prior to HSA End Date – S / b 1st of month prior to SGT_PERSON_ACCOUNT_GHDV_HSA.CONTRIBUTION_END_DATE
                    if (ibusPersonAccountGhdvHSA.icdoPersonAccountGhdvHsa.contribution_end_date != DateTime.MinValue)
                    {
                        ldtdate = new DateTime(ibusPersonAccountGhdvHSA.icdoPersonAccountGhdvHsa.contribution_end_date.AddMonths(-1).Year,
                                                    ibusPersonAccountGhdvHSA.icdoPersonAccountGhdvHsa.contribution_end_date.AddMonths(-1).Month, 1);
                    }
                        break;
                case busConstant.PeopleSoftDateHSARecordEndDate:
                    //HSA record End Date – S / b SGT_PERSON_ACCOUNT_GHDV_HSA.CONTRIBUTION_END_DATE
                    if (ibusPersonAccountGhdvHSA.icdoPersonAccountGhdvHsa.contribution_end_date != DateTime.MinValue)
                        ldtdate = ibusPersonAccountGhdvHSA.icdoPersonAccountGhdvHsa.contribution_end_date;
                    break;
                    //PIR 20481 : end
            }
            return ldtdate;
        }

        public void LoadPersonEmploymentForPeopleSoft()
        {
            if (iblnIsFromDeferredComp)//PIR 20702
                idtEmploymentDetails = busBase.Select("cdoPersonEmployment.LoadPersonEmploymentForDailyPeopleSoftByEmpId", new object[2] { ibusPersonAccount.icdoPersonAccount.person_id, iintPersonEmpId });
            else
                idtEmploymentDetails = busBase.Select("cdoPersonEmployment.LoadPersonEmploymentForDailyPeopleSoft", new object[1] { ibusPersonAccount.icdoPersonAccount.person_id });

            DataRow[] ldarrFilteredEmploymentDetails =
                idtEmploymentDetails.FilterTable(busConstant.DataType.Numeric, "person_account_id", ibusPersonAccount.icdoPersonAccount.person_account_id);
            if (ldarrFilteredEmploymentDetails.Count() > 0)
            {
                ibusPersonAccount.idtPlanEffectiveDate = ldarrFilteredEmploymentDetails[0]["h_end_date"] == DBNull.Value ?
                    DateTime.MinValue : Convert.ToDateTime(ldarrFilteredEmploymentDetails[0]["h_end_date"]);

                ibusPersonAccount.ibusPersonEmploymentDetail = new busPersonEmploymentDetail { icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail() };
                ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment = new busPersonEmployment { icdoPersonEmployment = new cdoPersonEmployment() };
                ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };

                ibusPersonAccount.idtbPlanCacheData = idtbPlanCacheData;
                ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.LoadData(ldarrFilteredEmploymentDetails[0]);
                ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date = ldarrFilteredEmploymentDetails[0]["d_start_date"] == DBNull.Value ?
                    DateTime.MinValue : Convert.ToDateTime(ldarrFilteredEmploymentDetails[0]["d_start_date"]);
                ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date = ldarrFilteredEmploymentDetails[0]["d_end_date"] == DBNull.Value ?
                    DateTime.MinValue : Convert.ToDateTime(ldarrFilteredEmploymentDetails[0]["d_end_date"]);
                ibusPersonAccount.ibusPersonEmploymentDetail.idtbPlanCacheData = idtbPlanCacheData;
                
                ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.LoadData(ldarrFilteredEmploymentDetails[0]);
                ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.start_date = ldarrFilteredEmploymentDetails[0]["h_start_date"] == DBNull.Value ?
                    DateTime.MinValue : Convert.ToDateTime(ldarrFilteredEmploymentDetails[0]["h_start_date"]);
                ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.end_date = ldarrFilteredEmploymentDetails[0]["h_end_date"] == DBNull.Value ?
                    DateTime.MinValue : Convert.ToDateTime(ldarrFilteredEmploymentDetails[0]["h_end_date"]);
                ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.LoadData(ldarrFilteredEmploymentDetails[0]);

                //PIR 25729 - Member type, Benefit plan and RHIC Benefit plan should be loaded for all retirement plans.
                if (iblnIsRetirementPlan)
                { 
                    ibusPersonAccount.LoadOrgPlan(ibusPersonAccount.icdoPersonAccount.current_plan_start_date_no_null, 
                                                  ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.org_id);
                    ibusPersonAccount.ibusPersonEmploymentDetail.LoadMemberType(ibusPersonAccount.icdoPersonAccount.current_plan_start_date_no_null);//PIR 26208
                    istrMemberType = ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.derived_member_type_value;
                    istrBenefitPlanforRetirement = ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.istrBenefitPlanForRetr;
                    istrRHICBenefitPlanforRetirement = ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.istrRHICBenefitPlanForRetr;
                }
            }
            else
            {
                ibusPersonAccount.idtbPlanCacheData = idtbPlanCacheData;
                ibusPersonAccount.icdoPersonAccount.person_employment_dtl_id = ibusPersonAccount.GetEmploymentDetailID();
                ibusPersonAccount.LoadPersonEmploymentDetail();
                ibusPersonAccount.ibusPersonEmploymentDetail.LoadPersonEmployment();
                ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
            }

            istrEmpTypeValue = ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value;
        }

        private void GetPeopleSoftPlanRecordsForLife(Collection<busPeoplesoftPlanCrossRef> aclbPeopleCrossRef)
        {
            if (aclbPeopleCrossRef.Count > 0)
            {
                //PIR 23527
                decimal ldecSuppCoverageAmountLimit = Convert.ToDecimal(busGlobalFunctions.GetData1ByCodeValue(7020, busConstant.SupplementalCoverageAmountLimit, iobjPassInfo));

                if (istrLevelOfCoverage != busConstant.LevelofCoverage_Supplemental)
                {
                    if (istrLevelOfCoverage == busConstant.LevelofCoverage_DependentSupplemental && idecDependentSuppCoverageAmount >= 0.0m)
                    {
                        busPeoplesoftPlanCrossRef lobjPeoplesoftPlanCrossRef = aclbPeopleCrossRef.Where(o =>
                            o.icdoPeoplesoftPlanCrossRef.coverage_amount == idecDependentSuppCoverageAmount).FirstOrDefault();
                        if (lobjPeoplesoftPlanCrossRef != null)
                            CreateFileRecordsAndAddToCollection(lobjPeoplesoftPlanCrossRef, ibusProvider.icdoOrganization.org_code);
                    }
                    else
                    {
                        busPeoplesoftPlanCrossRef lobjPeoplesoftPlanCrossRef = GetPeoplesoftPlanCrossRefRecord(aclbPeopleCrossRef);
                        if (lobjPeoplesoftPlanCrossRef != null)
                        {
                            if (istrLevelOfCoverage == busConstant.LevelofCoverage_SpouseSupplemental && !iblnIsPlanOptionSuspended)
                                idecFlatAmount = idecSpouseSuppCoverageAmount;

                            CreateFileRecordsAndAddToCollection(lobjPeoplesoftPlanCrossRef, ibusProvider.icdoOrganization.org_code);
                        }
                    }
                }
                //For Life Plan and Level of Coverage Supplemental ,the two records in the collection should be sent to output file                   
                else
                {
                    if (idecBasicSuppCoverageAmount > ldecSuppCoverageAmountLimit && CheckPremiumFlagChecked())
                    {
                        decimal ldecSuppPreTaxLimit = 0.0m;
                        ibusPersonAccountLife.LoadHistory();
                        busPersonAccountLifeHistory lbusPersonAccountLifeHistory = ibusPersonAccountLife.iclbPersonAccountLifeHistory.Where
                                                                                    (o => o.icdoPersonAccountLifeHistory.level_of_coverage_value == busConstant.LevelofCoverage_Supplemental && o.icdoPersonAccountLifeHistory.effective_end_date != DateTime.MinValue).
                                                                                        OrderByDescending(o => o.icdoPersonAccountLifeHistory.effective_start_date).FirstOrDefault();

                        DataTable ldtbCovergeAmountSupp = Select("cdoWssPersonAccountEnrollmentRequest.GetValidCoverageAmountForDisplay", new object[3]
                                                    { busConstant.LevelofCoverage_Supplemental, istrInsuranceTypeValue, idtHistoryChangeDate });
                        if (ldtbCovergeAmountSupp.Rows.Count > 0)
                        {
                            ldecSuppPreTaxLimit = Convert.ToDecimal(ldtbCovergeAmountSupp.Rows[0]["PRE_TAX_LIMIT"]);
                        }

                        if (lbusPersonAccountLifeHistory != null)
                        {
                            busPeoplesoftPlanCrossRef lobjPeoplesoftPlanCrossRefLifeFLXLIF = aclbPeopleCrossRef.Where(o => o.icdoPeoplesoftPlanCrossRef.benefit_plan == busConstant.PeopleSoftFileBenefitPlanFlxLif).FirstOrDefault();
                            if (lobjPeoplesoftPlanCrossRefLifeFLXLIF != null)
                            {
                                if (iblnIsPlanOptionSuspended)
                                    idecFlatAmount = 0.0m;
                                else
                                    idecFlatAmount = ldecSuppPreTaxLimit - idecBasicCoverageAmount; 
                                idecSuppCoverageAmount -= ldecSuppPreTaxLimit - idecBasicCoverageAmount;
                                CreateFileRecordsAndAddToCollection(lobjPeoplesoftPlanCrossRefLifeFLXLIF, ibusProvider.icdoOrganization.org_code);
                            }
                        }
                        else
                        {

                            busPeoplesoftPlanCrossRef lobjPeoplesoftPlanCrossRefLifeFLXLIF = aclbPeopleCrossRef.Where(o => o.icdoPeoplesoftPlanCrossRef.benefit_plan == busConstant.PeopleSoftFileBenefitPlanFlxLif).FirstOrDefault();
                            if (lobjPeoplesoftPlanCrossRefLifeFLXLIF != null)
                            {
                                idecFlatAmount = ldecSuppPreTaxLimit - idecBasicCoverageAmount;
                                idecSuppCoverageAmount -= ldecSuppPreTaxLimit - idecBasicCoverageAmount;
                                CreateFileRecordsAndAddToCollection(lobjPeoplesoftPlanCrossRefLifeFLXLIF, ibusProvider.icdoOrganization.org_code);
                            }
                        }
                        busPeoplesoftPlanCrossRef lobjPeoplesoftPlanCrossRef = GetPeoplesoftPlanCrossRefRecord(aclbPeopleCrossRef);

                        if (lbusPersonAccountLifeHistory != null)
                        {
                            if (lobjPeoplesoftPlanCrossRef != null && idecSuppCoverageAmount >= 0.0m)
                            {
                                idecFlatAmount = idecSuppCoverageAmount;
                                CreateFileRecordsAndAddToCollection(lobjPeoplesoftPlanCrossRef, ibusProvider.icdoOrganization.org_code);
                            }
                        }
                        else
                        {
                            if (lobjPeoplesoftPlanCrossRef != null && idecSuppCoverageAmount > 0.0m)
                            {
                                idecFlatAmount = idecSuppCoverageAmount;
                                CreateFileRecordsAndAddToCollection( lobjPeoplesoftPlanCrossRef, ibusProvider.icdoOrganization.org_code);
                            }
                        }

                    }
                    else if (idecBasicSuppCoverageAmount > ldecSuppCoverageAmountLimit && !CheckPremiumFlagChecked())
                    {
                        ibusPersonAccountLife.LoadHistory();
                        busPersonAccountLifeHistory lbusPersonAccountLifeHistory = ibusPersonAccountLife.iclbPersonAccountLifeHistory.Where
                                                                                    (o => o.icdoPersonAccountLifeHistory.level_of_coverage_value == busConstant.LevelofCoverage_Supplemental && o.icdoPersonAccountLifeHistory.effective_end_date != DateTime.MinValue).
                                                                                        OrderByDescending(o => o.icdoPersonAccountLifeHistory.effective_start_date).FirstOrDefault();
                        if (lbusPersonAccountLifeHistory != null)
                        {
                            if (lbusPersonAccountLifeHistory.icdoPersonAccountLifeHistory.premium_conversion_indicator_flag == busConstant.Flag_Yes)
                            {
                                busPeoplesoftPlanCrossRef lobjPeoplesoftPlanCrossRefLifeFLXLIF = aclbPeopleCrossRef.Where(o => o.icdoPeoplesoftPlanCrossRef.benefit_plan == busConstant.PeopleSoftFileBenefitPlanFlxLif).FirstOrDefault();
                                if (lobjPeoplesoftPlanCrossRefLifeFLXLIF != null)
                                {
                                    idecFlatAmount = 0.0m;
                                    iblnTerminatedRecord = true;
                                    istrCoverageElection = "T";

                                    CreateFileRecordsAndAddToCollection(lobjPeoplesoftPlanCrossRefLifeFLXLIF, ibusProvider.icdoOrganization.org_code);
                                    iblnTerminatedRecord = false;
                                }
                            }
                        }
                        busPeoplesoftPlanCrossRef lobjPeoplesoftPlanCrossRef = GetPeoplesoftPlanCrossRefRecord(aclbPeopleCrossRef);
                        if (lobjPeoplesoftPlanCrossRef != null)
                        {
                            idecFlatAmount = idecSuppCoverageAmount;
                            CreateFileRecordsAndAddToCollection(lobjPeoplesoftPlanCrossRef, ibusProvider.icdoOrganization.org_code);
                        }
                    }
                    else if (idecBasicSuppCoverageAmount <= ldecSuppCoverageAmountLimit && CheckPremiumFlagChecked())
                    {
                        ibusPersonAccountLife.LoadHistory();
                        busPersonAccountLifeHistory lbusPersonAccountLifeHistory = ibusPersonAccountLife.iclbPersonAccountLifeHistory.FirstOrDefault
                                                                                   (o => o.icdoPersonAccountLifeHistory.level_of_coverage_value == busConstant.LevelofCoverage_Supplemental
                                                                                   && o.icdoPersonAccountLifeHistory.effective_end_date != DateTime.MinValue
                                                                                   && o.icdoPersonAccountLifeHistory.effective_start_date != o.icdoPersonAccountLifeHistory.effective_end_date
                                                                                   && o.icdoPersonAccountLifeHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled
                                                                                   && o.icdoPersonAccountLifeHistory.plan_option_status_value == busConstant.PlanOptionStatusValueEnrolled);

                        if (lbusPersonAccountLifeHistory != null)
                        {
                            busPeoplesoftPlanCrossRef lobjPeoplesoftPlanCrossRefLifeFLXLIF = aclbPeopleCrossRef.Where(o => o.icdoPeoplesoftPlanCrossRef.benefit_plan == busConstant.PeopleSoftFileBenefitPlanFlxLif).FirstOrDefault();
                            if (lobjPeoplesoftPlanCrossRefLifeFLXLIF != null)
                            {
                                if (iblnIsPlanOptionSuspended) 
                                    idecFlatAmount = 0.0m;
                                else
                                    idecFlatAmount = idecSuppCoverageAmount;
                                CreateFileRecordsAndAddToCollection(lobjPeoplesoftPlanCrossRefLifeFLXLIF, ibusProvider.icdoOrganization.org_code);
                            }
                            busPeoplesoftPlanCrossRef lobjPeoplesoftPlanCrossRef1 = GetPeoplesoftPlanCrossRefRecord(aclbPeopleCrossRef);
                            if (lobjPeoplesoftPlanCrossRef1 != null && (lbusPersonAccountLifeHistory.icdoPersonAccountLifeHistory.premium_conversion_indicator_flag == busConstant.Flag_No
                                || (lbusPersonAccountLifeHistory.icdoPersonAccountLifeHistory.premium_conversion_indicator_flag == busConstant.Flag_Yes && lbusPersonAccountLifeHistory.icdoPersonAccountLifeHistory.coverage_amount > 50000)))
                            {
                                idecFlatAmount = 0.0m;
                                iblnTerminatedRecord = true;
                                istrCoverageElection = "T";

                                CreateFileRecordsAndAddToCollection(lobjPeoplesoftPlanCrossRef1, ibusProvider.icdoOrganization.org_code);
                                iblnTerminatedRecord = false;
                            }
                            else if (lobjPeoplesoftPlanCrossRef1?.icdoPeoplesoftPlanCrossRef?.benefit_plan == busConstant.PeopleSoftBenefitPlanLifeSuppTemp)//PIR 22949
                            {
                                if (iblnIsPlanOptionSuspended)
                                    idecFlatAmount = 0.0m;
                                else
                                    idecFlatAmount = idecSuppCoverageAmount;

                                CreateFileRecordsAndAddToCollection(lobjPeoplesoftPlanCrossRef1, ibusProvider.icdoOrganization.org_code);

                            }
                        }
                        else
                        {
                            busPersonAccountLifeHistory lbusLifeHistory = ibusPersonAccountLife.iclbPersonAccountLifeHistory.Where(o => o.icdoPersonAccountLifeHistory.level_of_coverage_value == busConstant.LevelofCoverage_Supplemental).OrderByDescending(o => o.icdoPersonAccountLifeHistory.effective_start_date).FirstOrDefault();
                            if (lbusLifeHistory != null && lbusLifeHistory.icdoPersonAccountLifeHistory.premium_conversion_indicator_flag == busConstant.Flag_Yes)
                            {
                                busPeoplesoftPlanCrossRef lobjPeoplesoftPlanCrossRefLifeFLXLIF = aclbPeopleCrossRef.Where(o => o.icdoPeoplesoftPlanCrossRef.benefit_plan == busConstant.PeopleSoftFileBenefitPlanFlxLif).FirstOrDefault();
                                if (lobjPeoplesoftPlanCrossRefLifeFLXLIF != null)
                                {
                                    if (iblnIsPlanOptionSuspended) 
                                        idecFlatAmount = 0.0m;
                                    else
                                        idecFlatAmount = idecSuppCoverageAmount;
                                    CreateFileRecordsAndAddToCollection( lobjPeoplesoftPlanCrossRefLifeFLXLIF, ibusProvider.icdoOrganization.org_code);
                                }
                                else //PIR 22949
                                {
                                    busPeoplesoftPlanCrossRef lobjPeoplesoftPlanCrossRef = GetPeoplesoftPlanCrossRefRecord(aclbPeopleCrossRef);
                                    if (lobjPeoplesoftPlanCrossRef != null)
                                    {
                                        if (iblnIsPlanOptionSuspended)
                                            idecFlatAmount = 0.0m;
                                        else
                                            idecFlatAmount = idecSuppCoverageAmount;

                                        CreateFileRecordsAndAddToCollection(lobjPeoplesoftPlanCrossRef, ibusProvider.icdoOrganization.org_code);
                                    }
                                }
                            }
                            else
                            {
                                busPeoplesoftPlanCrossRef lobjPeoplesoftPlanCrossRef = GetPeoplesoftPlanCrossRefRecord(aclbPeopleCrossRef);
                                if (lobjPeoplesoftPlanCrossRef != null)
                                {
                                    if (iblnIsPlanOptionSuspended) 
                                        idecFlatAmount = 0.0m;
                                    else
                                        idecFlatAmount = idecSuppCoverageAmount;

                                    CreateFileRecordsAndAddToCollection( lobjPeoplesoftPlanCrossRef, ibusProvider.icdoOrganization.org_code);
                                }
                            }
                        }
                    }
                    else if (idecBasicSuppCoverageAmount <= ldecSuppCoverageAmountLimit && !CheckPremiumFlagChecked()) //PIR 24922 & 24885
                    {
                        ibusPersonAccountLife.LoadHistory();
                        busPersonAccountLifeHistory lbusPersonAccountLifeHistory = ibusPersonAccountLife.iclbPersonAccountLifeHistory.Where
                                                                                    (o => o.icdoPersonAccountLifeHistory.level_of_coverage_value == busConstant.LevelofCoverage_Supplemental && o.icdoPersonAccountLifeHistory.effective_end_date != DateTime.MinValue).
                                                                                        OrderByDescending(o => o.icdoPersonAccountLifeHistory.effective_start_date).FirstOrDefault();

                        if (lbusPersonAccountLifeHistory != null)
                        {
                            if (lbusPersonAccountLifeHistory.icdoPersonAccountLifeHistory.premium_conversion_indicator_flag == busConstant.Flag_Yes)
                            {
                                busPeoplesoftPlanCrossRef lobjPeoplesoftPlanCrossRefLifeFLXLIF = aclbPeopleCrossRef.Where(o => o.icdoPeoplesoftPlanCrossRef.benefit_plan == busConstant.PeopleSoftFileBenefitPlanFlxLif).FirstOrDefault();
                                if (lobjPeoplesoftPlanCrossRefLifeFLXLIF != null)
                                {
                                    idecFlatAmount = 0.0m;
                                    iblnTerminatedRecord = true;
                                    istrCoverageElection= "T";
                                    CreateFileRecordsAndAddToCollection( lobjPeoplesoftPlanCrossRefLifeFLXLIF, ibusProvider.icdoOrganization.org_code);
                                    iblnTerminatedRecord = false;
                                }
                            }
                            else //If Previous premium indicator flag is No
                            {
                                busPeoplesoftPlanCrossRef lobjPeoplesoftPlanCrossRef1 = GetPeoplesoftPlanCrossRefRecord(aclbPeopleCrossRef);
                                if (iblnIsPlanOptionSuspended)
                                    idecFlatAmount = 0.0m;
                                else
                                    idecFlatAmount = idecSuppCoverageAmount;
                                CreateFileRecordsAndAddToCollection( lobjPeoplesoftPlanCrossRef1, ibusProvider.icdoOrganization.org_code);
                            }
                        }
                        busPeoplesoftPlanCrossRef lobjPeoplesoftPlanCrossRef = GetPeoplesoftPlanCrossRefRecord(aclbPeopleCrossRef);
                        if (lobjPeoplesoftPlanCrossRef != null)
                        {
                            if ((lbusPersonAccountLifeHistory != null
                            && lbusPersonAccountLifeHistory.icdoPersonAccountLifeHistory.premium_conversion_indicator_flag == busConstant.Flag_Yes) || lbusPersonAccountLifeHistory == null)
                            {
                                if (iblnIsPlanOptionSuspended) 
                                   idecFlatAmount = 0.0m;
                                else
                                    idecFlatAmount = idecSuppCoverageAmount;
                                CreateFileRecordsAndAddToCollection( lobjPeoplesoftPlanCrossRef, ibusProvider.icdoOrganization.org_code);
                            }
                        }
                    }
                }
            }
        }
    }
}
