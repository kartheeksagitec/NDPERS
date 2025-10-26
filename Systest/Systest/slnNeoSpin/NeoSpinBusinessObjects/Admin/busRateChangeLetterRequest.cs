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
using Sagitec.ExceptionPub;
using Sagitec.CustomDataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class NeoSpin.BusinessObjects.busRateChangeLetterRequest:
    /// Inherited from busRateChangeLetterRequestGen, the class is used to customize the business object busRateChangeLetterRequestGen.
    /// </summary>
    [Serializable]
    public class busRateChangeLetterRequest : busRateChangeLetterRequestGen
    {
        # region Properties
        public Collection<busOrgPlan> iclbProviderOrgPlan { get; set; }
        public busDBCacheData ibusDBCacheData { get; set; }

        public DateTime adtNewEffectiveDate { get; set; }
        public DateTime adtCurrentEffectiveDate { get; set; }

        public decimal idecCurrentBenefitFactor { get; set; }
        public decimal idecNewBenefitFactor { get; set; }

        public DataTable idtbOrgToBillGHDVHistory { get; set; }
        public DataTable idtbOrgToBillEAPHistory { get; set; }
        public DataTable idtbOrgToBillLTCHistory { get; set; }
        public DataTable idtbOrgToBillLIFEHistory { get; set; }
        public DataTable idtbOrgToBillLIFEOption { get; set; }

        public DataTable idtbLifeAgeEmployerLetterLifeOptionHistory { get; set; }
        public DataTable idtbLifeAgeEmployerLetterLifeHistory { get; set; }
        public DataTable idtbLifeAgeEmployerLetterLifeMembers { get; set; }

        public DataTable idtbTFFRPensionCheckGHDVHistory { get; set; }
        public DataTable idtbTFFRPensionCheckEAPHistory { get; set; }
        public DataTable idtbTFFRPensionCheckLTCHistory { get; set; }
        public DataTable idtbTFFRPensionCheckLIFEHistory { get; set; }
        public DataTable idtbTFFRPensionCheckLIFEOption { get; set; }

        public DataTable idtbIBSMembersGHDVHistory { get; set; }
        public DataTable idtbIBSMembersLTCHistory { get; set; }
        public DataTable idtbIBSMembersLIFEHistory { get; set; }
        public DataTable idtbIBSMembersLIFEOption { get; set; }
		//PIR 15683
        public DataTable idtbIBSMembersMedicarePartDHistory { get; set; }

        public DataTable idtbNetCheckAmountForPenionCheckMembers { get; set; }

        public DataTable idtbRHICApprovedCombine { get; set; }
        public bool iblnIsInsuranceRateChangeLetterBatch { get; set; }
        # endregion

        #region Correspondence Properties
        public bool iblnIsLetterTypeRHIC
        {
            get
            {
                if (icdoRateChangeLetterRequest.letter_type_value == busConstant.LetterTypeValueRHIC)
                    return true;
                return false;
            }
        }

        public bool iblnIsRHICBenefitIncrease
        {
            get
            {
                if (idecCurrentBenefitFactor - idecNewBenefitFactor != 0)
                    return true;
                return false;
            }
        }
        public bool iblnIsLetterTypeRHICOrHealth
        {
            get
            {
                if (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdGroupHealth)
                    return true;
                return false;
            }
        }

        public bool iblnIsLetterTypeRHICOrMedicare
        {
            get
            {
                if ((icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdMedicarePartD) ||
                    (icdoRateChangeLetterRequest.letter_type_value == busConstant.LetterTypeValueRHIC))
                    return true;
                return false;
            }
        }

        public bool iblnIsLetterTypeEAP
        {
            get
            {
                if (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdEAP)
                    return true;
                return false;
            }
        }

        public bool iblnIsLetterTypeDental
        {
            get
            {
                if (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdDental)
                    return true;
                return false;
            }
        }

        public bool iblnIsLetterTypeVision
        {
            get
            {
                if (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdVision)
                    return true;
                return false;
            }
        }

        public bool iblnIsLetterTypeLife
        {
            get
            {
                if (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdGroupLife)
                    return true;
                return false;
            }
        }

        public bool iblnIsLetterTypeLTC
        {
            get
            {
                if (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdLTC)
                    return true;
                return false;
            }
        }
        //PIR 6933
        public DateTime NewEffectiveDate
        {
            get
            {
                return icdoRateChangeLetterRequest.effective_date;
            }
        }
        //PIR 6933
        public DateTime CurrentEffectiveDate
        {
            get
            {
                if (icdoRateChangeLetterRequest.effective_date != DateTime.MinValue) return icdoRateChangeLetterRequest.effective_date.GetLastDayofMonth().AddMonths(-1);
                return DateTime.Now;
            }
        }


        public int EffectiveYear
        {
            get
            {
                return icdoRateChangeLetterRequest.effective_date.Year;
            }
        }

        public bool iblnIsMemberEnrolledInMedicare { get; set; }

        #endregion

        # region Business Rules

        //check whether selected Provider is valid provider for the selected letter type
        public bool IsValidProvider()
        {
            DataTable ldtbListCount = Select("cdoRateChangeLetterRequest.GetCountOfOrgPlanProvider",
                                        new object[2] { icdoRateChangeLetterRequest.iintPlanId, icdoRateChangeLetterRequest.provider_org_id });
            if (ldtbListCount.Rows.Count == 0)
                return false;
            return true;
        }

        //check only one pending record must exists for the letter type and provider with effective date
        public bool IsRecordDuplicated()
        {
            DataTable ldtbRecordList = Select<cdoRateChangeLetterRequest>(new string[4] { "LETTER_TYPE_VALUE", "EFFECTIVE_DATE", "PROVIDER_ORG_ID", "STATUS_VALUE" },
                                        new object[4]{icdoRateChangeLetterRequest.letter_type_value,icdoRateChangeLetterRequest.effective_date,
                                        icdoRateChangeLetterRequest.provider_org_id,busConstant.LetterStatuValuePending}, null, null);
            if (ldtbRecordList.Rows.Count >= 1)
            {
                if (icdoRateChangeLetterRequest.rate_change_letter_request_id != Convert.ToInt32(ldtbRecordList.Rows[0]["RATE_CHANGE_LETTER_REQUEST_ID"]))
                    return false;
            }
            return true;
        }

        # endregion

        # region Overriden Methods

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            //Reload the Org
            if (!String.IsNullOrEmpty(icdoRateChangeLetterRequest.istrOrgCodeId))
            {
                if (ibusProviderOrganization.FindOrganizationByOrgCode(icdoRateChangeLetterRequest.istrOrgCodeId))
                {
                    icdoRateChangeLetterRequest.provider_org_id = ibusProviderOrganization.icdoOrganization.org_id;
                }
            }
            else
            {
                ibusProviderOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
                icdoRateChangeLetterRequest.provider_org_id = 0;
            }

            //Load the Plan ID
            if (icdoRateChangeLetterRequest.letter_type_value.IsNotNullOrEmpty())
            {
                GetPlanIDFromLetterType();
            }
            else
            {
                icdoRateChangeLetterRequest.iintPlanId = 0;
            }

            base.BeforeValidate(aenmPageMode);
        }

        # endregion

        # region Load/Get Methods

        public void GetPlanIDFromLetterType()
        {
            icdoRateChangeLetterRequest.iintPlanId = Convert.ToInt32(busGlobalFunctions.GetData1ByCodeValue(4000,
                                            icdoRateChangeLetterRequest.letter_type_value, iobjPassInfo));
        }

        public void LoadPlan()
        {
            if (ibusPlan.IsNull())
                ibusPlan = new busPlan();
            ibusPlan.FindPlan(icdoRateChangeLetterRequest.iintPlanId);
        }

        /// <summary>
        /// Loading the Provider Org Plan Object for the Given Provider Org ID
        /// </summary>
        public void LoadProviderOrgPlan()
        {
            if (ibusProviderOrgPlan.IsNull())
                ibusProviderOrgPlan = new busOrgPlan { icdoOrgPlan = new cdoOrgPlan() };
            DataTable ldtbProvierOrgPlan = Select("cdoRateChangeLetterRequest.LoadProviderOrgPlan", new object[3] {icdoRateChangeLetterRequest.provider_org_id,
                                        icdoRateChangeLetterRequest.effective_date,icdoRateChangeLetterRequest.iintPlanId });
            if (ldtbProvierOrgPlan.Rows.Count > 0)
            {
                ibusProviderOrgPlan.icdoOrgPlan.LoadData(ldtbProvierOrgPlan.Rows[0]);

                ibusProviderOrgPlan.ibusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
                ibusProviderOrgPlan.ibusOrganization.icdoOrganization.LoadData(ldtbProvierOrgPlan.Rows[0]);
            }
        }

        public void LoadAllEmployerOrgPlans()
        {
            iclbOrgPlan = new System.Collections.Generic.List<busOrgPlan>();
            DataTable ldtbList = Select("cdoRateChangeLetterRequest.LoadAllEmployerOrgPlanByProvider",
                                                        new object[3] {icdoRateChangeLetterRequest.provider_org_id,
                                                        icdoRateChangeLetterRequest.effective_date,icdoRateChangeLetterRequest.iintPlanId});

            foreach (DataRow dr in ldtbList.Rows)
            {
                busOrgPlan lobjOrgPlan = new busOrgPlan { icdoOrgPlan = new cdoOrgPlan() };
                lobjOrgPlan.icdoOrgPlan.LoadData(dr);

                lobjOrgPlan.ibusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
                lobjOrgPlan.ibusOrganization.icdoOrganization.LoadData(dr);

                iclbOrgPlan.Add(lobjOrgPlan);
            }
        }

        public void LoadAllProviderOrgPlans()
        {
            DataTable ldtbAllProviderOrgPlans = busNeoSpinBase.Select("cdoRateChangeLetterRequest.LoadAllProviderOrgPlans", new object[] { });
            iclbProviderOrgPlan = new busBase().GetCollection<busOrgPlan>(ldtbAllProviderOrgPlans, "icdoOrgPlan");
        }

        public busOrgPlan LoadProviderOrgPlanByProviderOrgId(int aintProviderOrgId, int aintPlanId, DateTime adtEffectiveDate)
        {
            busOrgPlan lbusOrgPlanToReturn = new busOrgPlan { icdoOrgPlan = new cdoOrgPlan() };
            foreach (var lbusOrgPlan in iclbProviderOrgPlan)
            {
                if ((lbusOrgPlan.icdoOrgPlan.org_id == aintProviderOrgId) &&
                   (lbusOrgPlan.icdoOrgPlan.plan_id == aintPlanId))
                {
                    if (busGlobalFunctions.CheckDateOverlapping(adtEffectiveDate,
                        lbusOrgPlan.icdoOrgPlan.participation_start_date,
                        lbusOrgPlan.icdoOrgPlan.participation_end_date))
                    {
                        lbusOrgPlanToReturn = lbusOrgPlan;
                        break;
                    }
                }
            }
            return lbusOrgPlanToReturn;
        }

        public void LoadDBCacheData()
        {
            if (ibusDBCacheData == null)
                ibusDBCacheData = new busDBCacheData();
            ibusDBCacheData.idtbCachedRateRef = busGlobalFunctions.LoadHealthRateRefCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedRateStructureRef = busGlobalFunctions.LoadHealthRateStructureCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedCoverageRef = busGlobalFunctions.LoadHealthCoverageRefCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedHealthRate = busGlobalFunctions.LoadHealthRateCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedLifeRate = busGlobalFunctions.LoadLifeRateCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedDentalRate = busGlobalFunctions.LoadDentalRateCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedHMORate = busGlobalFunctions.LoadHMORateCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedLtcRate = busGlobalFunctions.LoadLTCRateCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedVisionRate = busGlobalFunctions.LoadVisionRateCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedEapRate = busGlobalFunctions.LoadEAPRateCacheData(iobjPassInfo);
            //PIR 15683
            ibusDBCacheData.idtbCachedMedicarePartDRate = busGlobalFunctions.LoadMedicarePartDRateCacheData(iobjPassInfo);
        }

        //TO GET NEW / CURRENT EFFECTIVE DATE BASED ON PLAN
        public void LoadCurrentAndNewRateEffectiveDate()
        {
            DataTable adtbListTobeFiltered = new DataTable();
            adtNewEffectiveDate = DateTime.MinValue;
            adtCurrentEffectiveDate = DateTime.MinValue;

            if (icdoRateChangeLetterRequest.letter_type_value == busConstant.LetterTypeValueRHIC)
            {
                adtNewEffectiveDate = new DateTime(icdoRateChangeLetterRequest.effective_date.Year, icdoRateChangeLetterRequest.effective_date.Month, 1);
                adtCurrentEffectiveDate = adtNewEffectiveDate.AddDays(-1);
            }
            else
            {
                switch (icdoRateChangeLetterRequest.iintPlanId)
                {
                    case busConstant.PlanIdGroupHealth:
                        adtbListTobeFiltered = ibusDBCacheData.idtbCachedHealthRate;
                        break;
                    case busConstant.PlanIdDental:
                        adtbListTobeFiltered = ibusDBCacheData.idtbCachedDentalRate;
                        break;
                    case busConstant.PlanIdEAP:
                        adtbListTobeFiltered = ibusDBCacheData.idtbCachedEapRate;
                        break;
                    case busConstant.PlanIdVision:
                        adtbListTobeFiltered = ibusDBCacheData.idtbCachedVisionRate;
                        break;
                    case busConstant.PlanIdGroupLife:
                        adtbListTobeFiltered = ibusDBCacheData.idtbCachedLifeRate;
                        break;
                    case busConstant.PlanIdLTC:
                        adtbListTobeFiltered = ibusDBCacheData.idtbCachedLtcRate;
                        break;
                    //PIR 15683
                    case busConstant.PlanIdMedicarePartD:
                        adtbListTobeFiltered = ibusDBCacheData.idtbCachedMedicarePartDRate;
                        break;
                    default:
                        break;
                }

                var ldtbNewFilteredDentalRate = adtbListTobeFiltered.AsEnumerable()
                                                        .Where(ldr => ldr.Field<DateTime>("effective_date") <= icdoRateChangeLetterRequest.effective_date)
                                                        .OrderByDescending(ldr => ldr.Field<DateTime>("effective_date"));

                if (ldtbNewFilteredDentalRate.Count() > 0)
                    adtNewEffectiveDate = Convert.ToDateTime(ldtbNewFilteredDentalRate.AsDataTable().Rows[0]["effective_date"]);

                if (adtNewEffectiveDate != DateTime.MinValue)
                {
                    //Satya Logic : Set the Current Effective Date as of Today
                    adtCurrentEffectiveDate = DateTime.Now;

                    //var ldtbCurrentFilteredDentalRate = adtbListTobeFiltered.AsEnumerable()
                    //                                         .Where(ldr => ldr.Field<DateTime>("effective_date") <= adtNewEffectiveDate.AddMonths(-1))
                    //                                         .OrderByDescending(ldr => ldr.Field<DateTime>("effective_date"));

                    //if (ldtbCurrentFilteredDentalRate.Count() > 0)
                    //    adtCurrentEffectiveDate = Convert.ToDateTime(ldtbCurrentFilteredDentalRate.AsDataTable().Rows[0]["effective_date"]);
                }
            }
        }

        public void LoadCurrentAndNewRHICBenefitFactor()
        {
            DataTable ldtbBenefitProvisionType = iobjPassInfo.isrvDBCache.GetCacheData("sgt_benefit_provision_benefit_type", null);
            DateTime ldtLastEffectiveDate = DateTime.MinValue;

            var lenumNewList = ldtbBenefitProvisionType.AsEnumerable()
                                           .Where(ldr => ldr.Field<DateTime>("effective_date") <= icdoRateChangeLetterRequest.effective_date)
                                           .OrderByDescending(ldr => ldr.Field<DateTime>("effective_date"));

            if (lenumNewList != null && lenumNewList.Count() > 0)
            {
                ldtLastEffectiveDate = Convert.ToDateTime(lenumNewList.AsDataTable().Rows[0]["effective_date"]);
                idecNewBenefitFactor = lenumNewList.FirstOrDefault().Field<decimal>("RHIC_SERVICE_FACTOR");
            }

            if (ldtLastEffectiveDate != DateTime.MinValue)
            {
                var lenumCurrentList = ldtbBenefitProvisionType.AsEnumerable()
                                                     .Where(ldr => ldr.Field<DateTime>("effective_date") <= ldtLastEffectiveDate.AddMonths(-1))
                                                     .OrderByDescending(ldr => ldr.Field<DateTime>("effective_date"));

                if (lenumCurrentList != null && lenumCurrentList.Count() > 0)
                {
                    idecCurrentBenefitFactor = lenumCurrentList.FirstOrDefault().Field<decimal>("RHIC_SERVICE_FACTOR");
                }
            }
        }

        # endregion

        #region Employer Letter Private Methods
        private void PopulateHealthMedicareEmployerLetter(busOrgPlan abusOrgPlan)
        {
            //prod pir 6846 : new field to store health participation start date
            DateTime ldtEffectiveDate = abusOrgPlan.icdoOrgPlan.health_participation_start_date == DateTime.MinValue ?
                abusOrgPlan.icdoOrgPlan.participation_start_date : abusOrgPlan.icdoOrgPlan.health_participation_start_date;
            var lenumFirstFilter =
                        ibusDBCacheData.idtbCachedRateStructureRef.AsEnumerable().Where(
                            row => row.Field<string>("health_insurance_type_value") == abusOrgPlan.HealthInsuranceType &&
                                    row.Field<DateTime>("EFFECTIVE_DATE") <= icdoRateChangeLetterRequest.effective_date &&
                                    busGlobalFunctions.CheckDateOverlapping(ldtEffectiveDate,//prod pir 6846 : new field to store health participation start date
                                        row.Field<DateTime?>("ENROLLMENT_DATE_FROM"), row.Field<DateTime?>("ENROLLMENT_DATE_TO"))
                                    && row.Field<string>("LOW_INCOME") == "0");
            if (lenumFirstFilter.Count() > 0)
            {
                //Generating Active Rates With / Without Wellness
                string lstrRateStructureValue = null;
                if (lenumFirstFilter.IsNotNull())
                    lstrRateStructureValue = lenumFirstFilter.FirstOrDefault()["RATE_STRUCTURE_VALUE"].ToString();

                if (lstrRateStructureValue.IsNotNullOrEmpty())
                {
                    //Populate Active Health Rate With Wellness
                    Collection<busInsurancePremium> lclbResult = GetHealthMedicarePremium(abusOrgPlan, lstrRateStructureValue, true, true);
                    foreach (var lbusInsurancePremium in lclbResult)
                    {
                        lbusInsurancePremium.idecWithWellnessPremium = lbusInsurancePremium.idecTempHealthMedicarePremium;
                        abusOrgPlan.iclbHealthPremium.Add(lbusInsurancePremium);
                    }

                    //Populate Active Health Rate Without Wellness
                    lclbResult = GetHealthMedicarePremium(abusOrgPlan, lstrRateStructureValue, true, false);
                    foreach (var lbusInsurancePremium in lclbResult)
                    {
                        busInsurancePremium lbusOrginalInsurancePremium =
                            abusOrgPlan.iclbHealthPremium.Where(i => i.istrCoverageCode == lbusInsurancePremium.istrCoverageCode).FirstOrDefault();

                        if (lbusOrginalInsurancePremium == null)
                        {
                            busInsurancePremium lbusNewInsurancePremium = new busInsurancePremium();
                            lbusNewInsurancePremium.istrClientHealthDescription = lbusInsurancePremium.istrClientHealthDescription;
                            lbusNewInsurancePremium.iintRateRefID = lbusInsurancePremium.iintRateRefID;
                            lbusNewInsurancePremium.istrCoverageCode = lbusInsurancePremium.istrCoverageCode;
                            lbusNewInsurancePremium.idecWithoutWellnessPremium = lbusInsurancePremium.idecTempHealthMedicarePremium;
                            abusOrgPlan.iclbHealthPremium.Add(lbusNewInsurancePremium);
                        }
                        else
                        {
                            lbusOrginalInsurancePremium.idecWithoutWellnessPremium = lbusInsurancePremium.idecTempHealthMedicarePremium;
                        }
                    }

                    //Populate COBRA Health Rate With Wellness
                    lclbResult = GetHealthMedicarePremium(abusOrgPlan, lstrRateStructureValue, false, true);
                    foreach (var lbusInsurancePremium in lclbResult)
                    {
                        lbusInsurancePremium.idecWithWellnessPremium = lbusInsurancePremium.idecTempHealthMedicarePremium;
                        abusOrgPlan.iclbHealthCOBRAPremium.Add(lbusInsurancePremium);
                    }

                    //Populate COBRA Health Rate Without Wellness
                    lclbResult = GetHealthMedicarePremium(abusOrgPlan, lstrRateStructureValue, false, false);
                    foreach (var lbusInsurancePremium in lclbResult)
                    {
                        busInsurancePremium lbusOrginalInsurancePremium =
                            abusOrgPlan.iclbHealthCOBRAPremium.Where(i => i.istrCoverageCode == lbusInsurancePremium.istrCoverageCode).FirstOrDefault();

                        if (lbusOrginalInsurancePremium == null)
                        {
                            busInsurancePremium lbusNewInsurancePremium = new busInsurancePremium();
                            lbusNewInsurancePremium.istrClientHealthDescription = lbusInsurancePremium.istrClientHealthDescription;
                            lbusNewInsurancePremium.iintRateRefID = lbusInsurancePremium.iintRateRefID;
                            lbusNewInsurancePremium.istrCoverageCode = lbusInsurancePremium.istrCoverageCode;
                            lbusNewInsurancePremium.idecWithoutWellnessPremium = lbusInsurancePremium.idecTempHealthMedicarePremium;
                            abusOrgPlan.iclbHealthCOBRAPremium.Add(lbusNewInsurancePremium);
                        }
                        else
                        {
                            lbusOrginalInsurancePremium.idecWithoutWellnessPremium = lbusInsurancePremium.idecTempHealthMedicarePremium;
                        }
                    }
                }
            }
        }

        private Collection<busInsurancePremium> GetHealthMedicarePremium(busOrgPlan abusOrgPlan, string astrRateStructureValue, bool ablnActiveRate, bool ablnWellness)
        {
            Collection<busInsurancePremium> lclbResult = new Collection<busInsurancePremium>();

            string lstrCobraFlag = "1";
            if (ablnActiveRate)
                lstrCobraFlag = "0";

            string lstrWellness = "0";
            if (ablnWellness)
                lstrWellness = "1";

            var lenumSecondFilter = ibusDBCacheData.idtbCachedRateRef.AsEnumerable().Where
                (row => row.Field<string>("health_insurance_type_value") == abusOrgPlan.HealthInsuranceType
                        && row.Field<string>("rate_structure_value") == astrRateStructureValue
                        && row.Field<string>("plan_option_value") == abusOrgPlan.icdoOrgPlan.plan_option_value
                        && row.Field<string>("low_income") == "0"
                        && row.Field<string>("alternate_structure_code_value") == null
                        && row.Field<string>("wellness") == lstrWellness);

            if (lenumSecondFilter.IsNotNull())
            {
                DataTable ldtSecondFileterRows = lenumSecondFilter.AsDataTable();
                if (ldtSecondFileterRows.Rows.Count > 0)
                {
                    DataRow ldrSecondFilterRow = ldtSecondFileterRows.Rows[0];
                    int lintRateRefId = (Int32)ldrSecondFilterRow["org_plan_group_health_medicare_part_d_rate_ref_id"];
                    //Load All Possible Coverage for each Rate Ref Id and Populate the Premium too
                    var lenumThirdFilter = ibusDBCacheData.idtbCachedCoverageRef.AsEnumerable().Where(row =>
                            row.Field<int>("org_plan_group_health_medicare_part_d_rate_ref_id") == lintRateRefId
                                //&& row.Field<string>("employment_type_value") == "PERM"
                            && row.Field<string>("cobra_in") == lstrCobraFlag);

                    if (lenumThirdFilter.IsNotNull())
                    {
                        foreach (DataRow ldrRow in lenumThirdFilter.AsDataTable().Rows)
                        {
                            int lintCoverageRefId = Convert.ToInt32(ldrRow["ORG_PLAN_GROUP_HEALTH_MEDICARE_PART_D_COVERAGE_REF_ID"].ToString());

                            busInsurancePremium lobjInsurancePremium = new busInsurancePremium();
                            //prod pir 6982 : need to display only client description
                            lobjInsurancePremium.istrClientHealthDescription = ldrRow["CLIENT_DESCRIPTION"] == DBNull.Value ? string.Empty : ldrRow["CLIENT_DESCRIPTION"].ToString();
                            lobjInsurancePremium.iintRateRefID = lintRateRefId;
                            lobjInsurancePremium.istrCoverageCode = ldrRow["COVERAGE_CODE"] == DBNull.Value ? string.Empty : ldrRow["COVERAGE_CODE"].ToString();
                            //Get the Premium Amount
                            decimal ldecFeeAmt = 0;
                            decimal ldecMedicarePartDAmount = 0;
                            //pir 7705
                            decimal ldecHealthSavingsAmount = 0;
                            decimal ldecHSAVendorAmt = 0;
                            decimal ldecBuydownAmount = 0;
                            decimal ldecHealthPremiumAmount =
                                busRateHelper.GetHealthPremiumAmount(lintCoverageRefId,
                                                                     icdoRateChangeLetterRequest.effective_date, 0.00M,
                                                                     ref ldecFeeAmt, ref ldecBuydownAmount,
                                                                     ref ldecMedicarePartDAmount,ref ldecHealthSavingsAmount, ref ldecHSAVendorAmt,
                                                                     ibusDBCacheData.idtbCachedHealthRate, iobjPassInfo);

                            if (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdGroupHealth)
                            {
                                lobjInsurancePremium.idecTempHealthMedicarePremium = ldecHealthPremiumAmount + ldecFeeAmt - ldecBuydownAmount + ldecMedicarePartDAmount; //PIR 14271
                            }
                            else
                            {
                                lobjInsurancePremium.idecTempHealthMedicarePremium = ldecMedicarePartDAmount + ldecFeeAmt - ldecBuydownAmount ;
                            }
                            lclbResult.Add(lobjInsurancePremium);
                        }
                    }
                }
            }
            return lclbResult;
        }

        private void PopulateDentalEmployerLetter(busOrgPlan abusOrgPlan)
        {
            if (adtNewEffectiveDate != DateTime.MinValue)
            {
                var ldtbfiltereddentalrate = ibusDBCacheData.idtbCachedDentalRate.AsEnumerable()
                                                        .Where(ldr => ldr.Field<DateTime>("effective_date") == adtNewEffectiveDate
                                                        && ldr.Field<string>("dental_insurance_type_value") == busConstant.DentalInsuranceTypeActive
                                                        && ldr.Field<int>("org_plan_id") == ibusProviderOrgPlan.icdoOrgPlan.org_plan_id)
                                                        .AsDataTable();

                foreach (DataRow drDental in ldtbfiltereddentalrate.Rows)
                {
                    busInsurancePremium lobjInsurancePremium = new busInsurancePremium();
                    lobjInsurancePremium.idecNewDentalPremium = Convert.ToDecimal(drDental["premium_amt"]);
                    lobjInsurancePremium.istrClientDentalDescription = drDental["CLIENT_DESCRIPTION"].ToString();
                    lobjInsurancePremium.istrDentalInsuranceTypeValue = drDental["DENTAL_INSURANCE_TYPE_VALUE"].ToString();
                    lobjInsurancePremium.istrDentalLevelOfCoverage = drDental["LEVEL_OF_COVERAGE_VALUE"].ToString();
                    abusOrgPlan.iclbDentalPremium.Add(lobjInsurancePremium);
                }
            }

            if (adtCurrentEffectiveDate != DateTime.MinValue)
            {
                //Set the New premium amount in the same populated collection
                foreach (busInsurancePremium lobjInsurancePremium in abusOrgPlan.iclbDentalPremium)
                {
                    var ldtbfilteredCurrentdentalrate = ibusDBCacheData.idtbCachedDentalRate.AsEnumerable()
                                        .Where(ldr => ldr.Field<string>("LEVEL_OF_COVERAGE_VALUE") == lobjInsurancePremium.istrDentalLevelOfCoverage
                                        && ldr.Field<string>("DENTAL_INSURANCE_TYPE_VALUE") == lobjInsurancePremium.istrDentalInsuranceTypeValue
                                        && ldr.Field<DateTime>("effective_date") <= adtCurrentEffectiveDate //prod pir 6972
                                        && ldr.Field<int>("org_plan_id") == ibusProviderOrgPlan.icdoOrgPlan.org_plan_id)
                                        .AsDataTable();

                    if (ldtbfilteredCurrentdentalrate.Rows.Count > 0)
                        lobjInsurancePremium.idecCurrentDentalPremium = Convert.ToDecimal(ldtbfilteredCurrentdentalrate.Rows[0]["premium_amt"]);
                }
            }
        }

        private void PopulateVisionEmployerLetter(busOrgPlan abusOrgPlan)
        {
            if (adtNewEffectiveDate != DateTime.MinValue)
            {
                var ldtbfilteredVisionrate = ibusDBCacheData.idtbCachedVisionRate.AsEnumerable()
                                        .Where(ldr => ldr.Field<DateTime>("effective_date") == adtNewEffectiveDate
                                        && ldr.Field<string>("vision_insurance_type_value") == busConstant.VisionInsuranceTypeActive
                                        && ldr.Field<int>("org_plan_id") == ibusProviderOrgPlan.icdoOrgPlan.org_plan_id)
                                        .AsDataTable();

                foreach (DataRow ldrRow in ldtbfilteredVisionrate.Rows)
                {
                    busInsurancePremium lobjInsurancePremium = new busInsurancePremium();
                    lobjInsurancePremium.idecNewVisionPremium = Convert.ToDecimal(ldrRow["premium_amt"]);
                    lobjInsurancePremium.istrClientVisionDescription = ldrRow["CLIENT_DESCRIPTION"].ToString();
                    lobjInsurancePremium.istrVisionInsuranceTypeValue = ldrRow["VISION_INSURANCE_TYPE_VALUE"].ToString();
                    lobjInsurancePremium.istrVisionLevelOfCoverage = ldrRow["LEVEL_OF_COVERAGE_VALUE"].ToString();
                    abusOrgPlan.iclbVisionPremium.Add(lobjInsurancePremium);
                }

                if (adtCurrentEffectiveDate != DateTime.MinValue)
                {
                    //map the New premium amount
                    foreach (busInsurancePremium lobjInsurancePremium in abusOrgPlan.iclbVisionPremium)
                    {
                        var ldtbfilteredCurrentVisionrate = ibusDBCacheData.idtbCachedVisionRate.AsEnumerable()
                                            .Where(ldr => ldr.Field<string>("level_of_coverage_value") == lobjInsurancePremium.istrVisionLevelOfCoverage
                                            && ldr.Field<string>("VISION_INSURANCE_TYPE_VALUE") == lobjInsurancePremium.istrVisionInsuranceTypeValue
                                            && ldr.Field<DateTime>("effective_date") <= adtCurrentEffectiveDate //prod pir 6972
                                            && ldr.Field<int>("org_plan_id") == ibusProviderOrgPlan.icdoOrgPlan.org_plan_id)
                                            .AsDataTable();

                        if (ldtbfilteredCurrentVisionrate.Rows.Count > 0)
                            lobjInsurancePremium.idecCurrentVisionPremium = Convert.ToDecimal(ldtbfilteredCurrentVisionrate.Rows[0]["premium_amt"]);
                    }
                }
            }
        }

        private void PopulateEAPEmployerLetter(busOrgPlan abusOrgPlan)
        {
            if (adtNewEffectiveDate != DateTime.MinValue)
            {
                var ldtbEAPData = ibusDBCacheData.idtbCachedEapRate.AsEnumerable()
                                        .Where(ldr => ldr.Field<DateTime>("effective_date") == adtNewEffectiveDate
                                         && ldr.Field<int>("org_plan_id") == ibusProviderOrgPlan.icdoOrgPlan.org_plan_id)
                                        .AsDataTable();

                foreach (DataRow ldrRow in ldtbEAPData.Rows)
                {
                    busInsurancePremium lobjInsurancePremium = new busInsurancePremium();
                    lobjInsurancePremium.idecNewEAPPremium = Convert.ToDecimal(ldrRow["premium_amt"]);
                    lobjInsurancePremium.istrEAPInsuranceValue = ldrRow["eap_insurance_type_value"].ToString();
                    lobjInsurancePremium.istrClientEAPDescription = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(344, lobjInsurancePremium.istrEAPInsuranceValue);
                    abusOrgPlan.iclbEAPPremium.Add(lobjInsurancePremium);
                }

                if (adtCurrentEffectiveDate != DateTime.MinValue)
                {
                    //map the New premium amount
                    foreach (busInsurancePremium lobjInsurancePremium in abusOrgPlan.iclbEAPPremium)
                    {
                        var ldtbEAPFilterData = ibusDBCacheData.idtbCachedEapRate.AsEnumerable()
                                            .Where(ldr => ldr.Field<string>("eap_insurance_type_value") == lobjInsurancePremium.istrEAPInsuranceValue
                                            && ldr.Field<DateTime>("effective_date") <= adtCurrentEffectiveDate //prod pir 6972
                                            && ldr.Field<int>("org_plan_id") == ibusProviderOrgPlan.icdoOrgPlan.org_plan_id)
                                            .AsDataTable();

                        if (ldtbEAPFilterData.Rows.Count > 0)
                            lobjInsurancePremium.idecCurrentEAPPremium = Convert.ToDecimal(ldtbEAPFilterData.Rows[0]["premium_amt"]);
                    }
                }
            }
        }

        #endregion

        public void GenerateEmployerRateChangeLetter(busOrgPlan abusOrgPlan)
        {
            if ((icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdGroupHealth) ||
                (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdMedicarePartD))
            {
                PopulateHealthMedicareEmployerLetter(abusOrgPlan);
            }
            else if (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdDental)
            {
                PopulateDentalEmployerLetter(abusOrgPlan);
            }
            else if (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdVision)
            {
                PopulateVisionEmployerLetter(abusOrgPlan);
            }
            else if (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdEAP)
            {
                PopulateEAPEmployerLetter(abusOrgPlan);
            }
            else if (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdGroupLife)
            {
                //Do Nothing
            }
            else if (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdLTC)
            {
                //Do Nothing
            }
        }

        public void GenerateEmployerLifeAgeChangeLetter(busOrgPlan abusOrgPlan)
        {
            DataRow[] larrRow = idtbLifeAgeEmployerLetterLifeMembers.FilterTable(busConstant.DataType.Numeric,
                                                                            "org_id",
                                                                            abusOrgPlan.ibusOrganization.icdoOrganization.org_id);

            foreach (DataRow ldrRow in larrRow)
            {
                busInsurancePremium lobjInsurancePremium = LoadInsurancePremium(ldrRow, null,
                                                                                null,
                                                                                idtbLifeAgeEmployerLetterLifeHistory,
                                                                                idtbLifeAgeEmployerLetterLifeOptionHistory,
                                                                                null, false, true);

                //Collection inside Collection not possible in Framework Corr Bookmark.
                foreach (var lbusInsurancePremium in lobjInsurancePremium.iclbCoverageLevelLifePremium)
                {
                    //PROD PIR : 4776
                    if (lbusInsurancePremium.idecCurrentPremium != lbusInsurancePremium.idecNewPremium)
                        abusOrgPlan.iclbLifeAgeEmployerPremium.Add(lbusInsurancePremium);
                }
            }

        }

        public void GenerateEmployerLetterForOrgToBill(busOrgPlan abusOrgPlan, bool ablnLoadCoverageLevelPremium, int aintExcludeOnlyBasicMembers = 0)
        {
            DataTable ldtbInsuranceData;
            string lstrQuery = string.Empty;

            if ((icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdDental)
                || (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdVision)
                || (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdGroupHealth)
                || (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdMedicarePartD))
            {
                lstrQuery = "cdoRateChangeLetterRequest.LoadOrgToBillGHDVMembers";
            }
            else if (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdEAP)
            {
                lstrQuery = "cdoRateChangeLetterRequest.LoadOrgToBillEAPMembers";
            }
            else if (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdGroupLife)
            {
                lstrQuery = "cdoRateChangeLetterRequest.LoadOrgToBillLifeMembers";
            }
            else if (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdLTC)
            {
                lstrQuery = "cdoRateChangeLetterRequest.LoadOrgToBillLTCMembers";
            }

            if ((icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdDental)
                || (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdVision)
                || (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdGroupHealth)
                || (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdMedicarePartD))
            {
                ldtbInsuranceData = busBase.Select(lstrQuery, new object[3] { abusOrgPlan.icdoOrgPlan.org_id, 
                                                                              icdoRateChangeLetterRequest.iintPlanId,
                                                                              icdoRateChangeLetterRequest.effective_date});
            }
            else if (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdGroupLife)
            {
                ldtbInsuranceData = busBase.Select(lstrQuery, new object[3] { abusOrgPlan.icdoOrgPlan.org_id,
                                                                              icdoRateChangeLetterRequest.effective_date,
                                                                              aintExcludeOnlyBasicMembers});
            }
            else
            {
                ldtbInsuranceData = busBase.Select(lstrQuery, new object[2] { abusOrgPlan.icdoOrgPlan.org_id,
                                                                              icdoRateChangeLetterRequest.effective_date});
            }

            foreach (DataRow ldrRow in ldtbInsuranceData.Rows)
            {
                busInsurancePremium lobjInsurancePremium = LoadInsurancePremium(ldrRow, idtbOrgToBillGHDVHistory,
                                                                                idtbOrgToBillEAPHistory,
                                                                                idtbOrgToBillLIFEHistory,
                                                                                idtbOrgToBillLIFEOption,
                                                                                idtbOrgToBillLTCHistory, true, ablnLoadCoverageLevelPremium);
                //This Check will be useful for Employer Life Age Change Letter 
                if (ablnLoadCoverageLevelPremium)
                {
                    foreach (var lbusInsurancePremium in lobjInsurancePremium.iclbCoverageLevelLifePremium)
                    {
                        //PROD PIR : 4776
                        if (lbusInsurancePremium.idecCurrentPremium != lbusInsurancePremium.idecNewPremium)
                            abusOrgPlan.iclbOrgToBillPremium.Add(lbusInsurancePremium);
                    }
                }
                else
                {
                    abusOrgPlan.iclbOrgToBillPremium.Add(lobjInsurancePremium);
                }
            }
        }

        public void GenerateEmployerLetterForTFFRPensionCheck(busOrgPlan abusOrgPlan, bool ablnLoadCoverageLevelPremium, int aintExcludeOnlyBasicMembers = 0)
        {
            DataTable ldtbInsuranceData;
            string lstrQuery = string.Empty;

            if ((icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdDental)
                || (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdVision)
                || (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdGroupHealth)
                || (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdMedicarePartD))
            {
                lstrQuery = "cdoRateChangeLetterRequest.LoadTFFRPensionCheckGHDVMembers";
            }
            else if (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdEAP)
            {
                lstrQuery = "cdoRateChangeLetterRequest.LoadTFFRPensionCheckEAPMembers";
            }
            else if (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdGroupLife)
            {
                lstrQuery = "cdoRateChangeLetterRequest.LoadTFFRPensionCheckLifeMembers";
            }
            else if (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdLTC)
            {
                lstrQuery = "cdoRateChangeLetterRequest.LoadTFFRPensionCheckLTCMembers";
            }

            if ((icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdDental)
               || (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdVision)
               || (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdGroupHealth)
               || (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdMedicarePartD))
            {
                ldtbInsuranceData = busBase.Select(lstrQuery, new object[2] { icdoRateChangeLetterRequest.iintPlanId, icdoRateChangeLetterRequest.effective_date });
            }
            else if (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdGroupLife)
            {
                ldtbInsuranceData = busBase.Select(lstrQuery, new object[2] { icdoRateChangeLetterRequest.effective_date, aintExcludeOnlyBasicMembers });
            }
            else
            {
                ldtbInsuranceData = busBase.Select(lstrQuery, new object[1] { icdoRateChangeLetterRequest.effective_date });
            }

            foreach (DataRow ldrRow in ldtbInsuranceData.Rows)
            {
                busInsurancePremium lobjInsurancePremium = LoadInsurancePremium(ldrRow, idtbTFFRPensionCheckGHDVHistory,
                                                                                idtbTFFRPensionCheckEAPHistory,
                                                                                idtbTFFRPensionCheckLIFEHistory,
                                                                                idtbTFFRPensionCheckLIFEOption,
                                                                                idtbTFFRPensionCheckLTCHistory, false, ablnLoadCoverageLevelPremium);

                //This Check will be useful for Employer Life Age Change Letter 
                if (ablnLoadCoverageLevelPremium)
                {
                    foreach (var lbusInsurancePremium in lobjInsurancePremium.iclbCoverageLevelLifePremium)
                    {
                        //PROD PIR : 4776
                        if (lbusInsurancePremium.idecCurrentPremium != lbusInsurancePremium.idecNewPremium)
                            abusOrgPlan.iclbTFFRPensionCheckPremium.Add(lbusInsurancePremium);
                    }
                }
                else
                {
                    abusOrgPlan.iclbTFFRPensionCheckPremium.Add(lobjInsurancePremium);
                }
            }
        }

        public DataTable LoadIBSMembers(int aintExcludeOnlyBasicMembers = 0)
        {
            DataTable ldtbIBSMembers = null;
            string lstrQuery = string.Empty;
			
			//PIR 15347 - Medicare Part D bookmarks
            if ((icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdDental)
                || (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdVision)
                || (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdGroupHealth))
            {
                lstrQuery = "cdoRateChangeLetterRequest.LoadIBSGHDVMembers";
            }
            else if (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdMedicarePartD)
            {
                lstrQuery = "cdoRateChangeLetterRequest.LoadIBSMedicareMembers";
            }
            else if (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdGroupLife)
            {
                lstrQuery = "cdoRateChangeLetterRequest.LoadIBSLifeMembers";
            }
            else if (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdLTC)
            {
                lstrQuery = "cdoRateChangeLetterRequest.LoadIBSLTCMembers";
            }

			//PIR 15347 - Medicare Part D bookmarks
            if ((icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdDental)
                || (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdVision)
                || (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdGroupHealth))
            {
                ldtbIBSMembers = busBase.Select(lstrQuery, new object[2] { icdoRateChangeLetterRequest.effective_date, icdoRateChangeLetterRequest.iintPlanId });
            }
            else
            {
                ldtbIBSMembers = busBase.Select(lstrQuery, new object[1] { icdoRateChangeLetterRequest.effective_date });
            }

            return ldtbIBSMembers;
        }

        public busInsurancePremium ProcessInsurancePremiumForIBSMember(DataRow adrRow)
        {
			//PIR 15683
            return LoadInsurancePremium(adrRow, idtbIBSMembersGHDVHistory, null, idtbIBSMembersLIFEHistory,
                                        idtbIBSMembersLIFEOption, idtbIBSMembersLTCHistory, false, true, idtbIBSMembersMedicarePartDHistory);
        }
		
		//PIR 15683
        public busInsurancePremium LoadInsurancePremium(DataRow adrRow, DataTable adtbGHDVHistory,
            DataTable adtbEAPHistory, DataTable adtbLifeHistory, DataTable adtbLifeOptionHistory, DataTable adtbLTCHistory, bool ablnOrgToBill, bool ablnLoadCoverageLevelPremium, DataTable adtbMedicareHistory = null)
        {
            busInsurancePremium lobjInsurancePremium = new busInsurancePremium();
			if(iblnIsInsuranceRateChangeLetterBatch)
            {
                adtNewEffectiveDate = NewEffectiveDate;
                adtCurrentEffectiveDate = CurrentEffectiveDate;
            }
            //Initialize the Collection to Avoid Null Exception
            lobjInsurancePremium.iclbCoverageLevelLTCMemberPremium = new Collection<busInsurancePremium>();
            lobjInsurancePremium.iclbCoverageLevelLTCSpousePremium = new Collection<busInsurancePremium>();
            lobjInsurancePremium.iclbCoverageLevelLifePremium = new Collection<busInsurancePremium>();

            if ((icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdDental)
                || (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdVision)
                || (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdGroupHealth))
            {
                var lobjPersonAccountGHDV = new busPersonAccountGhdv { icdoPersonAccountGhdv = new cdoPersonAccountGhdv() };
                lobjPersonAccountGHDV.icdoPersonAccountGhdv.LoadData(adrRow);

                lobjPersonAccountGHDV.icdoPersonAccount = new cdoPersonAccount();
                lobjPersonAccountGHDV.icdoPersonAccount.LoadData(adrRow);

                lobjPersonAccountGHDV.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                lobjPersonAccountGHDV.ibusPerson.icdoPerson.LoadData(adrRow);

                lobjInsurancePremium.istrIBSMemberFullName = lobjPersonAccountGHDV.ibusPerson.icdoPerson.FullName;
                lobjInsurancePremium.ibusPerson = lobjPersonAccountGHDV.ibusPerson;
                lobjPersonAccountGHDV.ibusPaymentElection = new busPersonAccountPaymentElection
                {
                    icdoPersonAccountPaymentElection = new cdoPersonAccountPaymentElection()
                };

                lobjPersonAccountGHDV.ibusPaymentElection.icdoPersonAccountPaymentElection.LoadData(adrRow);

                if (lobjPersonAccountGHDV.ibusPaymentElection.icdoPersonAccountPaymentElection.payment_method_value == busConstant.IBSModeOfPaymentPensionCheck)
                {
                    lobjInsurancePremium.istrIsPERSPensionPayment = busConstant.Flag_Yes;
                    lobjInsurancePremium.iintPayeeAccountID = lobjPersonAccountGHDV.ibusPaymentElection.icdoPersonAccountPaymentElection.payee_account_id;

                }
                if(iblnIsInsuranceRateChangeLetterBatch)
                {
                    if (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdGroupHealth && idtbIBSMembersGHealthHistory.IsNotNull() && idtbIBSMembersGHealthHistory.Rows.Count > 0)
                    {
                        DataRow[] larrRow = idtbIBSMembersGHealthHistory.FilterTable(busConstant.DataType.Numeric,
                                                                       "person_account_ghdv_id",
                                                                       lobjPersonAccountGHDV.icdoPersonAccountGhdv.person_account_ghdv_id);
                        lobjPersonAccountGHDV.iclbPersonAccountGHDVHistory =
                           GetCollection<busPersonAccountGhdvHistory>(larrRow, "icdoPersonAccountGhdvHistory");
                    }
                    else if (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdDental && idtbIBSMembersDentalHistory.IsNotNull() && idtbIBSMembersDentalHistory.Rows.Count > 0)
                    {
                        DataRow[] larrRow = idtbIBSMembersDentalHistory.FilterTable(busConstant.DataType.Numeric,
                                                                       "person_account_ghdv_id",
                                                                       lobjPersonAccountGHDV.icdoPersonAccountGhdv.person_account_ghdv_id);
                        lobjPersonAccountGHDV.iclbPersonAccountGHDVHistory =
                           GetCollection<busPersonAccountGhdvHistory>(larrRow, "icdoPersonAccountGhdvHistory");
                    }
                    else if (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdVision && idtbIBSMembersVisionHistory.IsNotNull() && idtbIBSMembersVisionHistory.Rows.Count > 0)
                    {
                        DataRow[] larrRow = idtbIBSMembersVisionHistory.FilterTable(busConstant.DataType.Numeric,
                                                                       "person_account_ghdv_id",
                                                                       lobjPersonAccountGHDV.icdoPersonAccountGhdv.person_account_ghdv_id);
                        lobjPersonAccountGHDV.iclbPersonAccountGHDVHistory =
                           GetCollection<busPersonAccountGhdvHistory>(larrRow, "icdoPersonAccountGhdvHistory");
                    }
                    
                }
                //Load History From DataTable (Optimization)
                else if (adtbGHDVHistory != null && adtbGHDVHistory.Rows.Count > 0)
                {
                    DataRow[] larrRow = adtbGHDVHistory.FilterTable(busConstant.DataType.Numeric,
                                                                       "person_account_ghdv_id",
                                                                       lobjPersonAccountGHDV.icdoPersonAccountGhdv.person_account_ghdv_id);

                    lobjPersonAccountGHDV.iclbPersonAccountGHDVHistory =
                       GetCollection<busPersonAccountGhdvHistory>(larrRow, "icdoPersonAccountGhdvHistory");
                }
                //PIR - 14346 Commented this block
                // The RHIC Combine records needs to be created for the new premium amount.
                // This is applicable only for Insurance rate change letter batch 
                //if (iblnIsInsuranceRateChangeLetterBatch)
                //{
                //    LoadGHDVPremiumByGivenDate(lobjPersonAccountGHDV, true, lobjInsurancePremium);
                //    if (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdGroupHealth ||
                //        icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdMedicarePartD)
                //        UpdateRHICCombine(lobjInsurancePremium.ibusPerson, lobjInsurancePremium.idecNewPremium);

                //    // PIR 11301 - Reload Rhic Combine Health split
                //    lobjPersonAccountGHDV.LoadBenefitRhicCombineHealthSplit();
                //}

                //Load the New Premium
                LoadGHDVPremiumByGivenDate(lobjPersonAccountGHDV, true, lobjInsurancePremium);

                //Load the Old Premium
                LoadGHDVPremiumByGivenDate(lobjPersonAccountGHDV, false, lobjInsurancePremium);

                ////Load the Medicare Premium Also For the Letter Type RHIC
                ////PIR 15347 - Medicare Part D bookmarks
                //if (icdoRateChangeLetterRequest.letter_type_value == busConstant.LetterTypeValueRHIC || icdoRateChangeLetterRequest.letter_type_value == busConstant.LetterTypeValueMedicare)
                //{
                //    if (lobjPersonAccountGHDV.ibusPerson.IsMemberEnrolledInPlan(busConstant.PlanIdMedicarePartD))
                //    {
                //        iblnIsMemberEnrolledInMedicare = true;
                //        busInsurancePremium lbusMedicareInsurancePremium = LoadMedicarePremium(lobjPersonAccountGHDV.ibusPerson);
                //        lobjInsurancePremium.idecNewMedicarePartDPremium = lbusMedicareInsurancePremium.idecNewPremium;
                //        lobjInsurancePremium.idecCurrentMedicarePartDPremium = lbusMedicareInsurancePremium.idecCurrentPremium;
                //    }
                //}
            }
			//PIR 15683
            else if (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdMedicarePartD)
            {

                var lobjPersonAccountMedicareHistory = new busPersonAccountMedicarePartDHistory { icdoPersonAccountMedicarePartDHistory = new cdoPersonAccountMedicarePartDHistory() };
                lobjPersonAccountMedicareHistory.icdoPersonAccountMedicarePartDHistory.LoadData(adrRow);

                lobjPersonAccountMedicareHistory.icdoPersonAccount = new cdoPersonAccount();
                lobjPersonAccountMedicareHistory.icdoPersonAccount.LoadData(adrRow);

                lobjPersonAccountMedicareHistory.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                lobjPersonAccountMedicareHistory.ibusPerson.icdoPerson.LoadData(adrRow);

                lobjPersonAccountMedicareHistory.ibusPaymentElection = new busPersonAccountPaymentElection
                {
                    icdoPersonAccountPaymentElection = new cdoPersonAccountPaymentElection()
                };

                lobjPersonAccountMedicareHistory.ibusPaymentElection.icdoPersonAccountPaymentElection.LoadData(adrRow);
                //Load the New Premium
               lobjPersonAccountMedicareHistory.GetTotalPremiumAmountForMedicareInsuranceRateChanage(adtNewEffectiveDate);
               lobjInsurancePremium.idecNewMedicarePartDPremium = lobjPersonAccountMedicareHistory.TotalMonthlyPremiumAmount;

               Collection<busPersonAccountMedicarePartDHistory> lclbPersonAccountMedicarePartDHistory = new Collection<busPersonAccountMedicarePartDHistory>();
               //Load History From DataTable (Optimization)
               if (adtbMedicareHistory != null && adtbMedicareHistory.Rows.Count > 0)
               {
                   DataRow[] larrRow = adtbMedicareHistory.FilterTable(busConstant.DataType.Numeric,
                                                                      "member_person_id",
                                                                      lobjPersonAccountMedicareHistory.icdoPersonAccountMedicarePartDHistory.member_person_id);

                   lclbPersonAccountMedicarePartDHistory =
                     GetCollection<busPersonAccountMedicarePartDHistory>(larrRow, "icdoPersonAccountMedicarePartDHistory");
               }

                //Load the Old Premium
               busPersonAccountMedicarePartDHistory lbusPersonAccountMedicarePartDHistory = lclbPersonAccountMedicarePartDHistory.Where(i => busGlobalFunctions.CheckDateOverlapping(adtCurrentEffectiveDate, i.icdoPersonAccountMedicarePartDHistory.start_date, i.icdoPersonAccountMedicarePartDHistory.end_date)
                   && i.icdoPersonAccountMedicarePartDHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled).FirstOrDefault();

               if (!lbusPersonAccountMedicarePartDHistory.IsNull())
               {
                   lbusPersonAccountMedicarePartDHistory.GetTotalPremiumAmountForMedicareInsuranceRateChanage(adtCurrentEffectiveDate);
                   lobjInsurancePremium.idecCurrentMedicarePartDPremium = lbusPersonAccountMedicarePartDHistory.TotalMonthlyPremiumAmount;
               }
            }
            else if (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdEAP)
            {
                var lobjPersonAccountEAP = new busPersonAccountEAP { icdoPersonAccount = new cdoPersonAccount() };
                lobjPersonAccountEAP.icdoPersonAccount.LoadData(adrRow);

                lobjPersonAccountEAP.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                lobjPersonAccountEAP.ibusPerson.icdoPerson.LoadData(adrRow);

                lobjInsurancePremium.istrIBSMemberFullName = lobjPersonAccountEAP.ibusPerson.icdoPerson.FullName;
                lobjInsurancePremium.ibusPerson = lobjPersonAccountEAP.ibusPerson;
                lobjPersonAccountEAP.ibusPaymentElection = new busPersonAccountPaymentElection
                {
                    icdoPersonAccountPaymentElection = new cdoPersonAccountPaymentElection()
                };

                lobjPersonAccountEAP.ibusPaymentElection.icdoPersonAccountPaymentElection.LoadData(adrRow);

                if (lobjPersonAccountEAP.ibusPaymentElection.icdoPersonAccountPaymentElection.payment_method_value == busConstant.IBSModeOfPaymentPensionCheck)
                {
                    lobjInsurancePremium.istrIsPERSPensionPayment = busConstant.Flag_Yes;
                    lobjInsurancePremium.iintPayeeAccountID = lobjPersonAccountEAP.ibusPaymentElection.icdoPersonAccountPaymentElection.payee_account_id;
                }
                if (adtbEAPHistory != null && adtbEAPHistory.Rows.Count > 0)
                {
                    DataRow[] larrRow = adtbEAPHistory.FilterTable(busConstant.DataType.Numeric,
                                                                                            "person_account_id",
                                                                                            lobjPersonAccountEAP.icdoPersonAccount.person_account_id);
                    lobjPersonAccountEAP.iclbEAPHistory = GetCollection<busPersonAccountEAPHistory>(larrRow, "icdoPersonAccountEAPHistory");
                }
                //Load New Premium
                LoadEAPPremiumForGivenDate(lobjPersonAccountEAP, lobjInsurancePremium, true);
                //Load Old Premium
                LoadEAPPremiumForGivenDate(lobjPersonAccountEAP, lobjInsurancePremium, false);
            }
            else if (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdGroupLife)
            {
                var lobjPersonAccountLife = new busPersonAccountLife { icdoPersonAccountLife = new cdoPersonAccountLife() };
                lobjPersonAccountLife.icdoPersonAccountLife.LoadData(adrRow);

                lobjPersonAccountLife.icdoPersonAccount = new cdoPersonAccount();
                lobjPersonAccountLife.icdoPersonAccount.LoadData(adrRow);

                lobjPersonAccountLife.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                lobjPersonAccountLife.ibusPerson.icdoPerson.LoadData(adrRow);

                lobjInsurancePremium.istrIBSMemberFullName = lobjPersonAccountLife.ibusPerson.icdoPerson.FullName;
                lobjInsurancePremium.ibusPerson = lobjPersonAccountLife.ibusPerson;

                lobjPersonAccountLife.ibusPaymentElection = new busPersonAccountPaymentElection
                                                                {
                                                                    icdoPersonAccountPaymentElection = new cdoPersonAccountPaymentElection()
                                                                };

                lobjPersonAccountLife.ibusPaymentElection.icdoPersonAccountPaymentElection.LoadData(adrRow);

                if (lobjPersonAccountLife.ibusPaymentElection.icdoPersonAccountPaymentElection.payment_method_value == busConstant.IBSModeOfPaymentPensionCheck)
                {
                    lobjInsurancePremium.istrIsPERSPensionPayment = busConstant.Flag_Yes;
                    lobjInsurancePremium.iintPayeeAccountID = lobjPersonAccountLife.ibusPaymentElection.icdoPersonAccountPaymentElection.payee_account_id;
                }

                if (adtbLifeOptionHistory != null && adtbLifeOptionHistory.Rows.Count > 0)
                {
                    DataRow[] larrRow = adtbLifeOptionHistory.FilterTable(busConstant.DataType.Numeric,
                                                                                            "person_account_id",
                                                                                            lobjPersonAccountLife.icdoPersonAccount.person_account_id);
                    lobjPersonAccountLife.LoadLifeOptionDataFromHistory(larrRow);
                }

                if (adtbLifeHistory != null && adtbLifeHistory.Rows.Count > 0)
                {
                    DataRow[] larrRow = adtbLifeHistory.FilterTable(busConstant.DataType.Numeric,
                                                                                            "person_account_id",
                                                                                            lobjPersonAccountLife.icdoPersonAccount.person_account_id);
                    lobjPersonAccountLife.iclbPersonAccountLifeHistory = GetCollection<busPersonAccountLifeHistory>(larrRow, "icdoPersonAccountLifeHistory");
                }

                //New Premium
                LoadLifePremiumForGivenDate(lobjPersonAccountLife, lobjInsurancePremium, true, ablnOrgToBill);

                if (ablnLoadCoverageLevelPremium)
                {
                    foreach (busPersonAccountLifeOption lobjLifeOption in lobjPersonAccountLife.iclbLifeOption)
                    {
                        //PIR 1935 : DO NOT Include Coverage Level Records if the Effective End Date is less than Batch Effective Date
                        if ((lobjLifeOption.icdoPersonAccountLifeOption.coverage_amount != 0) &&
                            ((lobjLifeOption.icdoPersonAccountLifeOption.effective_end_date == DateTime.MinValue) ||
                            ((lobjLifeOption.icdoPersonAccountLifeOption.effective_end_date != DateTime.MinValue) &&
                           (lobjLifeOption.icdoPersonAccountLifeOption.effective_end_date < icdoRateChangeLetterRequest.effective_date))))
                        {
                            //Load the History Object for the Given Life Option and the Date
                            busPersonAccountLifeHistory lobjPALifeHistory = lobjPersonAccountLife.LoadHistoryByDate(lobjLifeOption, adtNewEffectiveDate);
                            if (lobjPALifeHistory.icdoPersonAccountLifeHistory.person_account_life_history_id > 0)
                            {
                                if (lobjPALifeHistory.icdoPersonAccountLifeHistory.level_of_coverage_value == busConstant.LevelofCoverage_Basic)
                                {
                                    if (lobjPersonAccountLife.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id > 0)
                                    {
                                        if (!ablnOrgToBill) continue;
                                    }
                                    else
                                    {
                                        if (ablnOrgToBill) continue;
                                    }
                                }

                                if (lobjPALifeHistory.icdoPersonAccountLifeHistory.level_of_coverage_value == busConstant.LevelofCoverage_Supplemental)
                                {
                                    if (lobjPersonAccountLife.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_supplemental_org_id > 0)
                                    {
                                        if (!ablnOrgToBill) continue;
                                    }
                                    else
                                    {
                                        if (ablnOrgToBill) continue;
                                    }
                                }

                                if ((lobjPALifeHistory.icdoPersonAccountLifeHistory.level_of_coverage_value == busConstant.LevelofCoverage_SpouseSupplemental) ||
                                    (lobjPALifeHistory.icdoPersonAccountLifeHistory.level_of_coverage_value == busConstant.LevelofCoverage_DependentSupplemental))
                                {
                                    if (ablnOrgToBill) continue;
                                }

                                busInsurancePremium lbusInsurancePremium = new busInsurancePremium();
                                lbusInsurancePremium.istrLevelOfCoverageValue = lobjPALifeHistory.icdoPersonAccountLifeHistory.level_of_coverage_value;
                                lbusInsurancePremium.ibusPerson = lobjPersonAccountLife.ibusPerson;
                                lbusInsurancePremium.idecCoverageAmount = lobjPALifeHistory.icdoPersonAccountLifeHistory.coverage_amount;
                                lbusInsurancePremium.idecNewPremium = lobjLifeOption.icdoPersonAccountLifeOption.Monthly_Premium;
                                lobjInsurancePremium.iclbCoverageLevelLifePremium.Add(lbusInsurancePremium);
                            }
                        }
                    }
                }

                //Old Premium
                LoadLifePremiumForGivenDate(lobjPersonAccountLife, lobjInsurancePremium, false, ablnOrgToBill);

                if (ablnLoadCoverageLevelPremium)
                {
                    foreach (busPersonAccountLifeOption lobjLifeOption in lobjPersonAccountLife.iclbLifeOption)
                    {
                        //PIR 1935 : DO NOT Include Coverage Level Records if the Effective End Date is less than Batch Effective Date
                        if ((lobjLifeOption.icdoPersonAccountLifeOption.coverage_amount != 0) &&
                            ((lobjLifeOption.icdoPersonAccountLifeOption.effective_end_date == DateTime.MinValue) ||
                            ((lobjLifeOption.icdoPersonAccountLifeOption.effective_end_date != DateTime.MinValue) &&
                           (lobjLifeOption.icdoPersonAccountLifeOption.effective_end_date < icdoRateChangeLetterRequest.effective_date))))
                        {
                            //Load the History Object for the Given Life Option and the Date
                            busPersonAccountLifeHistory lobjPALifeHistory = lobjPersonAccountLife.LoadHistoryByDate(lobjLifeOption, adtCurrentEffectiveDate);
                            if (lobjPALifeHistory.icdoPersonAccountLifeHistory.person_account_life_history_id > 0)
                            {

                                busInsurancePremium lbusInsurancePremium =
                                    lobjInsurancePremium.iclbCoverageLevelLifePremium.Where(
                                        i => i.istrLevelOfCoverageValue == lobjPALifeHistory.icdoPersonAccountLifeHistory.level_of_coverage_value).FirstOrDefault();

                                if (lbusInsurancePremium == null)
                                {
                                    if (lobjPALifeHistory.icdoPersonAccountLifeHistory.level_of_coverage_value == busConstant.LevelofCoverage_Basic)
                                    {
                                        if (lobjPersonAccountLife.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id > 0)
                                        {
                                            if (!ablnOrgToBill) continue;
                                        }
                                        else
                                        {
                                            if (ablnOrgToBill) continue;
                                        }
                                    }

                                    if (lobjPALifeHistory.icdoPersonAccountLifeHistory.level_of_coverage_value == busConstant.LevelofCoverage_Supplemental)
                                    {
                                        if (lobjPersonAccountLife.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_supplemental_org_id > 0)
                                        {
                                            if (!ablnOrgToBill) continue;
                                        }
                                        else
                                        {
                                            if (ablnOrgToBill) continue;
                                        }
                                    }

                                    if ((lobjPALifeHistory.icdoPersonAccountLifeHistory.level_of_coverage_value == busConstant.LevelofCoverage_SpouseSupplemental) ||
                                        (lobjPALifeHistory.icdoPersonAccountLifeHistory.level_of_coverage_value == busConstant.LevelofCoverage_DependentSupplemental))
                                    {
                                        if (ablnOrgToBill) continue;
                                    }

                                    lbusInsurancePremium = new busInsurancePremium();
                                    lbusInsurancePremium.istrLevelOfCoverageValue = lobjPALifeHistory.icdoPersonAccountLifeHistory.level_of_coverage_value;
                                    lbusInsurancePremium.ibusPerson = lobjPersonAccountLife.ibusPerson;
                                    lbusInsurancePremium.idecCoverageAmount = lobjPALifeHistory.icdoPersonAccountLifeHistory.coverage_amount;
                                    lbusInsurancePremium.idecCurrentPremium = lobjLifeOption.icdoPersonAccountLifeOption.Monthly_Premium;
                                    lobjInsurancePremium.iclbCoverageLevelLifePremium.Add(lbusInsurancePremium);
                                }
                                else
                                {
                                    lbusInsurancePremium.idecCurrentPremium = lobjLifeOption.icdoPersonAccountLifeOption.Monthly_Premium;
                                }
                            }
                        }
                    }
                }
            }
            else if (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdLTC)
            {
                var lobjPersonAccountLTC = new busPersonAccountLtc { icdoPersonAccount = new cdoPersonAccount() };
                lobjPersonAccountLTC.icdoPersonAccount.LoadData(adrRow);

                lobjPersonAccountLTC.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                lobjPersonAccountLTC.ibusPerson.icdoPerson.LoadData(adrRow);

                lobjInsurancePremium.istrIBSMemberFullName = lobjPersonAccountLTC.ibusPerson.icdoPerson.FullName;
                lobjInsurancePremium.ibusPerson = lobjPersonAccountLTC.ibusPerson;

                lobjPersonAccountLTC.ibusPaymentElection = new busPersonAccountPaymentElection
                {
                    icdoPersonAccountPaymentElection = new cdoPersonAccountPaymentElection()
                };

                lobjPersonAccountLTC.ibusPaymentElection.icdoPersonAccountPaymentElection.LoadData(adrRow);

                if (lobjPersonAccountLTC.ibusPaymentElection.icdoPersonAccountPaymentElection.payment_method_value == busConstant.IBSModeOfPaymentPensionCheck)
                {
                    lobjInsurancePremium.istrIsPERSPensionPayment = busConstant.Flag_Yes;
                    lobjInsurancePremium.iintPayeeAccountID = lobjPersonAccountLTC.ibusPaymentElection.icdoPersonAccountPaymentElection.payee_account_id;
                }

                lobjPersonAccountLTC.LoadLtcOptionUpdateMember();
                lobjPersonAccountLTC.LoadLtcOptionUpdateSpouse();
                //from history datatable get the history for this person account
                if (adtbLTCHistory != null && adtbLTCHistory.Rows.Count > 0)
                {
                    DataRow[] larrRow = adtbLTCHistory.FilterTable(busConstant.DataType.Numeric,
                                                                       "person_account_id",
                                                                       lobjPersonAccountLTC.icdoPersonAccount.person_account_id);

                    lobjPersonAccountLTC.iclbLtcHistory = GetCollection<busPersonAccountLtcOptionHistory>(larrRow, "icdoPersonAccountLtcOptionHistory");
                }

                //New Premium
                LoadLTCPremiumForGivenDate(lobjPersonAccountLTC, lobjInsurancePremium, true);

                if (ablnLoadCoverageLevelPremium)
                {
                    foreach (busPersonAccountLtcOption lobjLtcOption in lobjPersonAccountLTC.iclbLtcOptionMember)
                    {
                        if (lobjLtcOption.icdoPersonAccountLtcOption.effective_start_date != DateTime.MinValue)
                        {
                            busInsurancePremium lbusInsurancePremiumLtcOption = new busInsurancePremium();
                            lbusInsurancePremiumLtcOption.istrLevelOfCoverageValue = lobjLtcOption.icdoPersonAccountLtcOption.level_of_coverage_value;
                            lbusInsurancePremiumLtcOption.idecNewPremium = lobjLtcOption.idecMonthlyPremium;
                            lobjInsurancePremium.iclbCoverageLevelLTCMemberPremium.Add(lbusInsurancePremiumLtcOption);
                        }
                    }
                    foreach (busPersonAccountLtcOption lobjLtcOption in lobjPersonAccountLTC.iclbLtcOptionSpouse)
                    {
                        if (lobjLtcOption.icdoPersonAccountLtcOption.effective_start_date != DateTime.MinValue)
                        {
                            busInsurancePremium lbusInsurancePremiumLtcOption = new busInsurancePremium();
                            lbusInsurancePremiumLtcOption.istrLevelOfCoverageValue = lobjLtcOption.icdoPersonAccountLtcOption.level_of_coverage_value;
                            lbusInsurancePremiumLtcOption.idecNewPremium = lobjLtcOption.idecMonthlyPremium;
                            lobjInsurancePremium.iclbCoverageLevelLTCSpousePremium.Add(lbusInsurancePremiumLtcOption);
                        }
                    }
                }

                //Old Premium
                LoadLTCPremiumForGivenDate(lobjPersonAccountLTC, lobjInsurancePremium, false);

                if (ablnLoadCoverageLevelPremium)
                {
                    foreach (busPersonAccountLtcOption lobjLtcOption in lobjPersonAccountLTC.iclbLtcOptionMember)
                    {
                        if (lobjLtcOption.icdoPersonAccountLtcOption.effective_start_date != DateTime.MinValue)
                        {
                            busInsurancePremium lbusInsurancePremiumLtcOption =
                                lobjInsurancePremium.iclbCoverageLevelLTCMemberPremium.Where(
                                    i => i.istrLevelOfCoverageValue == lobjLtcOption.icdoPersonAccountLtcOption.level_of_coverage_value).
                                    FirstOrDefault();

                            if (lbusInsurancePremiumLtcOption == null)
                            {
                                lbusInsurancePremiumLtcOption = new busInsurancePremium();
                                lbusInsurancePremiumLtcOption.istrLevelOfCoverageValue = lobjLtcOption.icdoPersonAccountLtcOption.level_of_coverage_value;
                                lbusInsurancePremiumLtcOption.idecCurrentPremium = lobjLtcOption.idecMonthlyPremium;
                                lobjInsurancePremium.iclbCoverageLevelLTCMemberPremium.Add(lbusInsurancePremiumLtcOption);
                            }
                            else
                            {
                                lbusInsurancePremiumLtcOption.idecCurrentPremium = lobjLtcOption.idecMonthlyPremium;
                            }
                        }
                    }
                    foreach (busPersonAccountLtcOption lobjLtcOption in lobjPersonAccountLTC.iclbLtcOptionSpouse)
                    {
                        if (lobjLtcOption.icdoPersonAccountLtcOption.effective_start_date != DateTime.MinValue)
                        {
                            busInsurancePremium lbusInsurancePremiumLtcOption =
                                lobjInsurancePremium.iclbCoverageLevelLTCSpousePremium.Where(
                                    i => i.istrLevelOfCoverageValue == lobjLtcOption.icdoPersonAccountLtcOption.level_of_coverage_value).
                                    FirstOrDefault();

                            if (lbusInsurancePremiumLtcOption == null)
                            {
                                lbusInsurancePremiumLtcOption = new busInsurancePremium();
                                lbusInsurancePremiumLtcOption.istrLevelOfCoverageValue = lobjLtcOption.icdoPersonAccountLtcOption.level_of_coverage_value;
                                lbusInsurancePremiumLtcOption.idecCurrentPremium = lobjLtcOption.idecMonthlyPremium;
                                lobjInsurancePremium.iclbCoverageLevelLTCSpousePremium.Add(lbusInsurancePremiumLtcOption);
                            }
                            else
                            {
                                lbusInsurancePremiumLtcOption.idecCurrentPremium = lobjLtcOption.idecMonthlyPremium;
                            }
                        }
                    }
                }
            }
            return lobjInsurancePremium;
        }

        private busInsurancePremium LoadMedicarePremium(busPerson abusPerson)
        {
            busInsurancePremium lbusInsurancePremium = new busInsurancePremium();
            if (abusPerson.icolPersonAccountByPlan == null)
                abusPerson.LoadPersonAccountByPlan(busConstant.PlanIdMedicarePartD);

            if (abusPerson.icolPersonAccountByPlan.Count > 0)
            {
                busPersonAccount lbusPersonAccount = abusPerson.icolPersonAccountByPlan[0];
                var lobjPersonAccountGHDV = new busPersonAccountGhdv { icdoPersonAccountGhdv = new cdoPersonAccountGhdv() };
                lobjPersonAccountGHDV.FindGHDVByPersonAccountID(lbusPersonAccount.icdoPersonAccount.person_account_id);
                lobjPersonAccountGHDV.icdoPersonAccount = lbusPersonAccount.icdoPersonAccount;
                lobjPersonAccountGHDV.ibusPerson = abusPerson;
                lobjPersonAccountGHDV.LoadPersonAccountGHDVHistory();

                //Load the New Premium
                LoadGHDVPremiumByGivenDate(lobjPersonAccountGHDV, true, lbusInsurancePremium);

                //Load the Old Premium
                LoadGHDVPremiumByGivenDate(lobjPersonAccountGHDV, false, lbusInsurancePremium);
            }
            return lbusInsurancePremium;
        }

        private void LoadLTCPremiumForGivenDate(busPersonAccountLtc lobjPersonAccountLTC, busInsurancePremium lobjInsurancePremium, bool ablnIsNewPremium)
        {
            DateTime ldtEffectiveDate = adtCurrentEffectiveDate;
            if (ablnIsNewPremium)
                ldtEffectiveDate = adtNewEffectiveDate;

            //Loading Provider Org Plan based on Effective Date
            busPersonAccountLtcOptionHistory lobjPersonAccountLTCOptionHistory = new busPersonAccountLtcOptionHistory
            {
                icdoPersonAccountLtcOptionHistory = new cdoPersonAccountLtcOptionHistory()
            };
            if (lobjPersonAccountLTC.iclbLtcOptionMember.Count > 0)
            {
                //Load the Provider Org ID by History
                lobjPersonAccountLTCOptionHistory = lobjPersonAccountLTC.LoadHistoryByDate(lobjPersonAccountLTC.iclbLtcOptionMember[0],
                                                                                                ldtEffectiveDate);
            }

            lobjPersonAccountLTC.LoadActiveProviderOrgPlan(ldtEffectiveDate);

            lobjPersonAccountLTC.GetMonthlyPremiumAmount(ldtEffectiveDate);

            if (ablnIsNewPremium)
                lobjInsurancePremium.idecNewPremium = lobjPersonAccountLTC.idecTotalMonthlyPremium;
            else
                lobjInsurancePremium.idecCurrentPremium = lobjPersonAccountLTC.idecTotalMonthlyPremium;
        }

        private void LoadEAPPremiumForGivenDate(busPersonAccountEAP lobjPersonAccountEAP, busInsurancePremium lobjInsurancePremium, bool ablnIsNewPremium)
        {
            DateTime ldtEffectiveDate = adtCurrentEffectiveDate;
            if (ablnIsNewPremium)
                ldtEffectiveDate = adtNewEffectiveDate;

            busPersonAccountEAPHistory lobjPersonAccountEAPHistory = lobjPersonAccountEAP.LoadHistoryByDate(ldtEffectiveDate);

            lobjPersonAccountEAP = lobjPersonAccountEAPHistory.LoadEAPObject(lobjPersonAccountEAP);

            if (lobjPersonAccountEAP.icdoPersonAccount.provider_org_id > 0)
            {
                lobjPersonAccountEAP.ibusProviderOrgPlan = LoadProviderOrgPlanByProviderOrgId(lobjPersonAccountEAP.icdoPersonAccount.provider_org_id,
                                            icdoRateChangeLetterRequest.iintPlanId, ldtEffectiveDate);
            }
            else
            {
                lobjPersonAccountEAP.LoadActiveProviderOrgPlan(ldtEffectiveDate);
            }
            lobjPersonAccountEAP.GetMonthlyPremium();

            if (ablnIsNewPremium)
                lobjInsurancePremium.idecNewPremium = lobjPersonAccountEAP.idecMonthlyPremium;
            else
                lobjInsurancePremium.idecCurrentPremium = lobjPersonAccountEAP.idecMonthlyPremium;
        }

        public void LoadLifePremiumForGivenDate(busPersonAccountLife lobjPersonAccountLife, busInsurancePremium lobjInsurancePremium, bool ablnIsNewPremium, bool ablnIsOrgToBill)
        {
            DateTime ldtEffectiveDate = adtCurrentEffectiveDate;
            if (ablnIsNewPremium)
                ldtEffectiveDate = adtNewEffectiveDate;

            busPersonAccountLifeHistory lobjPALifeHistory = new busPersonAccountLifeHistory();
            lobjPALifeHistory.icdoPersonAccountLifeHistory = new cdoPersonAccountLifeHistory();
            foreach (busPersonAccountLifeOption lobjPALifeOption in lobjPersonAccountLife.iclbLifeOption)
            {
                if (lobjPALifeOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_Basic)
                {
                    lobjPALifeHistory = lobjPersonAccountLife.LoadHistoryByDate(lobjPALifeOption, ldtEffectiveDate);
                    break;
                }
            }

            lobjPersonAccountLife.LoadActiveProviderOrgPlan(ldtEffectiveDate);

            lobjPersonAccountLife.LoadMemberAge(ldtEffectiveDate);

            lobjPersonAccountLife.GetMonthlyPremiumAmount(ldtEffectiveDate);

            decimal ldecFinalPremiumAmount = 0.00M;

            if (ablnIsOrgToBill)
            {
                //Exclude the Amount which are paid by ORG PIR : 1793
                if (lobjPersonAccountLife.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id > 0)
                {
                    ldecFinalPremiumAmount += lobjPersonAccountLife.idecLifeBasicPremiumAmt;
                }

                if (lobjPersonAccountLife.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_supplemental_org_id > 0)
                {
                    ldecFinalPremiumAmount += lobjPersonAccountLife.idecLifeSupplementalPremiumAmount;
                }
            }
            else
            {
                ldecFinalPremiumAmount = lobjPersonAccountLife.idecTotalMonthlyPremium;
            }

            if (ablnIsNewPremium)
            {
                lobjInsurancePremium.idecNewPremium = ldecFinalPremiumAmount;
            }
            else
            {
                lobjInsurancePremium.idecCurrentPremium = ldecFinalPremiumAmount;
            }
        }

        public void LoadGHDVPremiumByGivenDate(busPersonAccountGhdv aobjPersonAccountGHDV, bool ablnIsNewPremium, busInsurancePremium aobjInsurancePremium)
        {
            decimal ldecPremium = 0.00M;
            DateTime ldtEffectiveDate = adtCurrentEffectiveDate;
            if (ablnIsNewPremium)
                ldtEffectiveDate = adtNewEffectiveDate;

            //load history for the given date
            busPersonAccountGhdvHistory lobjPAGhdvHistory = aobjPersonAccountGHDV.LoadHistoryByDate(ldtEffectiveDate);

            //load the object for this history
            aobjPersonAccountGHDV = lobjPAGhdvHistory.LoadGHDVObject(aobjPersonAccountGHDV);
            //For Dependent COBRA, we need to load Member Employment
            if (aobjPersonAccountGHDV.icdoPersonAccount.from_person_account_id > 0)
            {
                //Load Member GHDV Object
                aobjPersonAccountGHDV.ibusMemberGHDVForDependent = new busPersonAccountGhdv();
                aobjPersonAccountGHDV.ibusMemberGHDVForDependent.FindGHDVByPersonAccountID(aobjPersonAccountGHDV.icdoPersonAccount.from_person_account_id);
                aobjPersonAccountGHDV.ibusMemberGHDVForDependent.FindPersonAccount(aobjPersonAccountGHDV.icdoPersonAccount.from_person_account_id);
                aobjPersonAccountGHDV.iblnIsDependentCobra = true;
            }

            //we need org plan object for determining health participation date for IBS COBRA Members
            if (aobjPersonAccountGHDV.icdoPersonAccountGhdv.cobra_type_value.IsNotNullOrEmpty())
            {
                //For Dependent COBRA, we need to load Member Employment
                if (aobjPersonAccountGHDV.icdoPersonAccount.from_person_account_id > 0)
                {
                    aobjPersonAccountGHDV.LoadEmploymentDetailByDate(aobjPersonAccountGHDV.idtPlanEffectiveDate, aobjPersonAccountGHDV.ibusMemberGHDVForDependent, true, true);
                }
                else
                {
                    aobjPersonAccountGHDV.LoadEmploymentDetailByDate(aobjPersonAccountGHDV.idtPlanEffectiveDate, true);
                }
                if (aobjPersonAccountGHDV.icdoPersonAccount.person_employment_dtl_id > 0)
                {
                    aobjPersonAccountGHDV.LoadPersonEmploymentDetail();
                    aobjPersonAccountGHDV.ibusPersonEmploymentDetail.LoadPersonEmployment();
                    aobjPersonAccountGHDV.LoadOrgPlan(aobjPersonAccountGHDV.idtPlanEffectiveDate);
                }
            }
            aobjPersonAccountGHDV.LoadActiveProviderOrgPlan(ldtEffectiveDate);
            if (iblnIsInsuranceRateChangeLetterBatch) ibusProviderOrgPlan = aobjPersonAccountGHDV.ibusProviderOrgPlan; //PIR 6933
            //Load the Premium for Current Effective Date
            if ((icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdGroupHealth)
                || (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdMedicarePartD))
            {
                // PROD PIR 8335 : All the History was not loaded, which results in wrong HealthParticipationDate, which inturn results in wrong Coverage Ref ID
                aobjPersonAccountGHDV.LoadPersonAccountGHDVHistory();
                if (aobjPersonAccountGHDV.icdoPersonAccountGhdv.overridden_structure_code.IsNotNullOrEmpty())
                {
                    aobjPersonAccountGHDV.LoadRateStructureForUserStructureCode();
                }
                else
                {
                    aobjPersonAccountGHDV.LoadHealthParticipationDate();
                    //To Get the Rate Structure Code (Derived Field)
                    aobjPersonAccountGHDV.LoadRateStructure(ldtEffectiveDate);
                }

                //Get the Coverage Ref ID
                if (ablnIsNewPremium)
                    aobjPersonAccountGHDV.LoadCoverageRefID();
                else
                    aobjPersonAccountGHDV.LoadPreviousCoverageRefID(); // PIR 11237

                //Get the Premium Amount
                aobjPersonAccountGHDV.GetMonthlyPremiumAmountByRefID(ldtEffectiveDate);

                ldecPremium = aobjPersonAccountGHDV.icdoPersonAccountGhdv.MonthlyPremiumAmount;
            }
            if (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdDental)
            {
                ldecPremium = busRateHelper.GetDentalPremiumAmount(ibusProviderOrgPlan.icdoOrgPlan.org_plan_id,
                                                                                   lobjPAGhdvHistory.icdoPersonAccountGhdvHistory.dental_insurance_type_value,
                                                                                   lobjPAGhdvHistory.icdoPersonAccountGhdvHistory.level_of_coverage_value,
                                                                                 ldtEffectiveDate, ibusDBCacheData.idtbCachedDentalRate, iobjPassInfo);

            }
            if (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdVision)
            {
                ldecPremium = busRateHelper.GetVisionPremiumAmount(ibusProviderOrgPlan.icdoOrgPlan.org_plan_id,
                                                                                   lobjPAGhdvHistory.icdoPersonAccountGhdvHistory.vision_insurance_type_value,
                                                                                   lobjPAGhdvHistory.icdoPersonAccountGhdvHistory.level_of_coverage_value,
                                                                                   ldtEffectiveDate, ibusDBCacheData.idtbCachedVisionRate, iobjPassInfo);
            }
            if (ablnIsNewPremium)
            {
                aobjInsurancePremium.idecNewPremium = ldecPremium;
                aobjInsurancePremium.idecNewRHICAmount = aobjPersonAccountGHDV.icdoPersonAccountGhdv.total_rhic_amount;
            }
            else
            {
                aobjInsurancePremium.idecCurrentPremium = ldecPremium;
                aobjInsurancePremium.idecCurrentRHICAmount = aobjPersonAccountGHDV.icdoPersonAccountGhdv.total_rhic_amount;
            }
        }

        public void LoadAllApprovedRHICCombine()
        {
            idtbRHICApprovedCombine = busNeoSpinBase.Select<cdoBenefitRhicCombine>(
                                        new string[3] { "status_value", "action_status_value", "apply_to_value" },
                                        new object[3] { busConstant.RHICStatusValid, busConstant.RHICActionStatusApproved, busConstant.Flag_Yes }, null, "end_date");
        }
        //PIR 14346 - Commented this method
        /// <summary>
        /// RHIC combine record needs to be cancelled and create a new when the premium is getting revised.
        /// </summary>
        //public void UpdateRHICCombine(busPerson aobjPerson, decimal adecNewPremiumAmount)
        //{
        //    DataRow[] ldtrRHIC = idtbRHICApprovedCombine.FilterTable(busConstant.DataType.Numeric, "PERSON_ID", aobjPerson.icdoPerson.person_id);
        //    if (ldtrRHIC.Count() > 0 && adtNewEffectiveDate != DateTime.MinValue && adecNewPremiumAmount > 0M)
        //    {
        //        busBenefitRhicCombine lbusBenefitRhicCombine = new busBenefitRhicCombine { icdoBenefitRhicCombine = new cdoBenefitRhicCombine() };
        //        lbusBenefitRhicCombine.icdoBenefitRhicCombine.LoadData(ldtrRHIC[0]);

        //        DateTime ldteNewStartDate = adtNewEffectiveDate;
        //        if (lbusBenefitRhicCombine.icdoBenefitRhicCombine.start_date > ldteNewStartDate)
        //            ldteNewStartDate = lbusBenefitRhicCombine.icdoBenefitRhicCombine.start_date;

        //        if (adecNewPremiumAmount < lbusBenefitRhicCombine.icdoBenefitRhicCombine.total_other_rhic_amount ||
        //            (adecNewPremiumAmount > lbusBenefitRhicCombine.icdoBenefitRhicCombine.total_other_rhic_amount &&
        //            lbusBenefitRhicCombine.icdoBenefitRhicCombine.combined_rhic_amount > lbusBenefitRhicCombine.icdoBenefitRhicCombine.total_other_rhic_amount))
        //        {
        //            busBenefitRhicCombine lbusBenefitNewRhicCombine = new busBenefitRhicCombine
        //            {
        //                icdoBenefitRhicCombine = new cdoBenefitRhicCombine()
        //            };
        //            lbusBenefitRhicCombine.ibusPerson = aobjPerson;
        //            lbusBenefitNewRhicCombine.icdoBenefitRhicCombine.start_date = ldteNewStartDate;
        //            lbusBenefitNewRhicCombine.icdoBenefitRhicCombine.person_id = lbusBenefitRhicCombine.icdoBenefitRhicCombine.person_id;
        //            lbusBenefitNewRhicCombine.ienmAutomaticRhicCombineTrigger = busConstant.automatic_rhic_combine_trigger.health_premium_change;
        //            lbusBenefitNewRhicCombine.CreateAutomaticRHICCombine();
        //        }
        //    }
        //}

        //prod pir 7008
        //--start--//
        public void GenerateEmployerLetterForHealthTFFRPensionCheck(busOrgPlan abusOrgPlan)
        {
            GenerateEmployerLetterForHealthTFFRPensionCheckByDate(abusOrgPlan, adtCurrentEffectiveDate, true);
            GenerateEmployerLetterForHealthTFFRPensionCheckByDate(abusOrgPlan, adtNewEffectiveDate, false);
        }


        public void GenerateEmployerLetterForHealthTFFRPensionCheckByDate(busOrgPlan abusOrgPlan, DateTime adtEffectiveDate, bool ablnOldPremium)
        {
            busInsurancePremium lbusNewInsurancePremium;
            //prod pir 6846 : new field to store health participation start date
            DateTime ldtEffectiveDate = abusOrgPlan.icdoOrgPlan.health_participation_start_date == DateTime.MinValue ?
                abusOrgPlan.icdoOrgPlan.participation_start_date : abusOrgPlan.icdoOrgPlan.health_participation_start_date;
            var lenumFirstFilter =
                        ibusDBCacheData.idtbCachedRateStructureRef.AsEnumerable().Where(
                            row => row.Field<string>("health_insurance_type_value") == busConstant.HealthInsuranceTypeRetiree &&
                                    row.Field<DateTime>("EFFECTIVE_DATE") <= adtEffectiveDate);

            string lstrRateStructureValue, lstrLowIncome, lstrGrandFatherRate;
            if (lenumFirstFilter.Count() > 0)
            {
                foreach (DataRow ldrRateStructureRef in lenumFirstFilter)
                {
                    lstrRateStructureValue = lstrLowIncome = lstrGrandFatherRate = string.Empty;
                    if (ldrRateStructureRef["RATE_STRUCTURE_VALUE"] != DBNull.Value)
                    {
                        lstrRateStructureValue = ldrRateStructureRef["RATE_STRUCTURE_VALUE"].ToString();
                        lstrLowIncome = ldrRateStructureRef["LOW_INCOME"] == DBNull.Value ? "0" : ldrRateStructureRef["LOW_INCOME"].ToString();
                        lstrGrandFatherRate = ldrRateStructureRef["ALTERNATE_STRUCTURE_CODE_VALUE"] == DBNull.Value ? null : ldrRateStructureRef["ALTERNATE_STRUCTURE_CODE_VALUE"].ToString();
                        Collection<busInsurancePremium> lclbResult = GetHealthMedicarePremiumForTFFR(abusOrgPlan, lstrRateStructureValue, lstrLowIncome, lstrGrandFatherRate, adtEffectiveDate);

                        foreach (var lbusInsurancePremium in lclbResult)
                        {
                            busInsurancePremium lobjInsurancePremium = abusOrgPlan.iclbTFFRPensionCheckPremium
                                            .Where(o => o.istrRateSturctureCode == lbusInsurancePremium.istrRateSturctureCode &&
                                                        o.istrCoverageCode == lbusInsurancePremium.istrCoverageCode).FirstOrDefault();

                            if (lobjInsurancePremium == null)
                            {
                                lbusNewInsurancePremium = new busInsurancePremium();
                                lbusNewInsurancePremium.istrClientHealthDescription = lbusInsurancePremium.istrCoverageCode + "-" + lbusInsurancePremium.istrClientHealthDescription;
                                lbusNewInsurancePremium.istrRateSturctureCode = lbusInsurancePremium.istrRateSturctureCode;
                                lbusNewInsurancePremium.iintRateRefID = lbusInsurancePremium.iintRateRefID;
                                lbusNewInsurancePremium.istrCoverageCode = lbusInsurancePremium.istrCoverageCode;
                                if (ablnOldPremium)
                                    lbusNewInsurancePremium.idecCurrentPremium = lbusInsurancePremium.idecTempHealthMedicarePremium;
                                else
                                    lbusNewInsurancePremium.idecNewPremium = lbusInsurancePremium.idecTempHealthMedicarePremium;
                                abusOrgPlan.iclbTFFRPensionCheckPremium.Add(lbusNewInsurancePremium);
                            }
                            else
                            {
                                if (ablnOldPremium)
                                    lobjInsurancePremium.idecCurrentPremium = lbusInsurancePremium.idecTempHealthMedicarePremium;
                                else
                                    lobjInsurancePremium.idecNewPremium = lbusInsurancePremium.idecTempHealthMedicarePremium;
                            }
                        }

                    }
                }
            }
        }

        private Collection<busInsurancePremium> GetHealthMedicarePremiumForTFFR(busOrgPlan abusOrgPlan, string astrRateStructureValue, string astrLowIncome, 
            string astrAlternateStructureCode, DateTime adtEffectiveDate)
        {
            Collection<busInsurancePremium> lclbResult = new Collection<busInsurancePremium>();
                        
            var lenumSecondFilter = ibusDBCacheData.idtbCachedRateRef.AsEnumerable().Where
                (row => row.Field<string>("health_insurance_type_value") == busConstant.HealthInsuranceTypeRetiree
                        && row.Field<string>("rate_structure_value") == astrRateStructureValue
                        && row.Field<string>("low_income") == astrLowIncome
                        && row.Field<string>("ALTERNATE_STRUCTURE_CODE_VALUE") == astrAlternateStructureCode);

            if (lenumSecondFilter.IsNotNull())
            {
                DataTable ldtSecondFileterRows = lenumSecondFilter.AsDataTable();
                foreach(DataRow ldrRateRef in ldtSecondFileterRows.Rows)
                {
                    int lintRateRefId = (Int32)ldrRateRef["org_plan_group_health_medicare_part_d_rate_ref_id"];
                    string lstrRateStructureCode = Convert.ToString(ldrRateRef["rate_structure_code"]);
                    //Load All Possible Coverage for each Rate Ref Id and Populate the Premium too
                    var lenumThirdFilter = ibusDBCacheData.idtbCachedCoverageRef.AsEnumerable().Where(row =>
                            row.Field<int>("org_plan_group_health_medicare_part_d_rate_ref_id") == lintRateRefId);

                    if (lenumThirdFilter.IsNotNull())
                    {
                        foreach (DataRow ldrRow in lenumThirdFilter.AsDataTable().Rows)
                        {
                            int lintCoverageRefId = Convert.ToInt32(ldrRow["ORG_PLAN_GROUP_HEALTH_MEDICARE_PART_D_COVERAGE_REF_ID"].ToString());

                            busInsurancePremium lobjInsurancePremium = new busInsurancePremium();
                            //prod pir 6982 : need to display only client description
                            lobjInsurancePremium.istrClientHealthDescription = ldrRow["CLIENT_DESCRIPTION"] == DBNull.Value ? string.Empty : ldrRow["CLIENT_DESCRIPTION"].ToString();
                            lobjInsurancePremium.iintRateRefID = lintRateRefId;
                            lobjInsurancePremium.istrCoverageCode = ldrRow["COVERAGE_CODE"] == DBNull.Value ? string.Empty : ldrRow["COVERAGE_CODE"].ToString();
                            lobjInsurancePremium.istrRateSturctureCode = lstrRateStructureCode;
                            //Get the Premium Amount
                            decimal ldecFeeAmt = 0;
                            decimal ldecMedicarePartDAmount = 0;
                            //pir 7705
                            decimal ldecHealthSavingsAmount = 0;
                            decimal ldecHSAVendorAmt = 0;
                            decimal ldecBuydownAmount = 0;
                            decimal ldecHealthPremiumAmount =
                               busRateHelper.GetHealthPremiumAmount(lintCoverageRefId,
                                                                    adtEffectiveDate, 0.00M,
                                                                    ref ldecFeeAmt, ref ldecBuydownAmount,
                                                                    ref ldecMedicarePartDAmount,ref ldecHealthSavingsAmount,ref ldecHSAVendorAmt,
                                                                    ibusDBCacheData.idtbCachedHealthRate, iobjPassInfo);
                            if (icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdGroupHealth)
                            {
                                lobjInsurancePremium.idecTempHealthMedicarePremium = ldecHealthPremiumAmount + ldecFeeAmt + ldecHealthSavingsAmount - ldecBuydownAmount + ldecMedicarePartDAmount;//PIR 14271
                            }
                            else
                            {
                                lobjInsurancePremium.idecTempHealthMedicarePremium = ldecMedicarePartDAmount + ldecFeeAmt - ldecBuydownAmount;
                            }
                            lclbResult.Add(lobjInsurancePremium);
                        }
                    }
                }
            }
            return lclbResult;
        }
        //--end--//PROD PIR 7008

        //PIR 6933
        public DataTable idtbIBSMembersGHealthHistory { get; set; }
        public DataTable idtbIBSMembersDentalHistory { get; set; }
        public DataTable idtbIBSMembersVisionHistory { get; set; }
        public DataTable idtbIBSHealthMembers { get; set; }
        public DataTable idtbIBSDentalMembers { get; set; }
        public DataTable idtbIBSVisionMembers { get; set; }
        public DataTable idtbIBSLifeMembers { get; set; }
        public DataTable idtbIBSMedicareMembers { get; set; }
        public DataTable idtbCombinedRhicAmount { get; set; }
        public DataTable idtbInitialSelectedMembersForRateChange { get; set; }

    }
}
