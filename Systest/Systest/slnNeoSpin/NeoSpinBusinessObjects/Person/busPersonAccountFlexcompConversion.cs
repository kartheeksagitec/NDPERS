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
using NeoSpin.DataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPersonAccountFlexcompConversion : busPersonAccountFlexcompConversionGen
    {
        public Collection<cdoOrganization> LoadFlexCompProviders()
        {
            //UAT PIR: 970. Method moved to base person account.
            return ibusPersonAccount.LoadActiveProviders();
        }
        public Boolean iblnHaveVisionDentalOrLifePlan { get; set; }
        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            if (ibusPersonAccount.IsNull()) LoadPersonAccount();
            if (ibusPersonAccount.ibusPerson.IsNull()) ibusPersonAccount.LoadPerson();
            if (ibusPersonAccount.ibusPerson.icolPersonAccount.IsNull()) ibusPersonAccount.ibusPerson.LoadPersonAccount();

            //Reload the Selected Provider
            LoadProvider();

            //Maik Comment on Phone - Only on New Mode Set the End Date Automatically except for Prudential Life Insurance Org Code
            //if (icdoPersonAccountFlexcompConversion.ienuObjectState == ObjectState.Insert)
            //{
            //    if ((icdoPersonAccountFlexcompConversion.effective_start_date != DateTime.MinValue) && (icdoPersonAccountFlexcompConversion.org_id > 0))
            //    {
            //        if (ibusProvider.icdoOrganization.org_code != busConstant.PrudentialLifeInsuranceOrgCode)
            //            icdoPersonAccountFlexcompConversion.effective_end_date = new DateTime(icdoPersonAccountFlexcompConversion.effective_start_date.Year, 12, 31);
            //    }
            //}

            base.BeforeValidate(aenmPageMode);
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            LoadProvider();

            InsertIntoEnrollmentData();
        }

        //PIR 12056
        public void UpdateReportFlag()
        {
            if (icdoPersonAccountFlexcompConversion.person_account_flex_comp_conversion_id > 0)
            {
                DBFunction.DBNonQuery("cdoPersonAccountFlexcompConversion.UpdateBenefitEnrollmentReportFlag",
                                    new object[1] { icdoPersonAccountFlexcompConversion.person_account_flex_comp_conversion_id },
                                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            }
        }

        // BR-022-00c - UAT PIR ID 472
        public bool IsValidProvider()
        {
            if (icdoPersonAccountFlexcompConversion.org_id != 0)
            {
                if (ibusProvider.IsNull()) LoadProvider();
                if (ibusProvider.iclbOrgPlan.IsNull()) ibusProvider.LoadOrgPlan();

                //UAT PIR 472 - This validation is only for Health / Dental / Vision / Life Providers
                bool lblnIsProviderToValidate = ibusProvider.iclbOrgPlan.Any(i => i.icdoOrgPlan.plan_id == busConstant.PlanIdGroupHealth ||
                                                                             i.icdoOrgPlan.plan_id == busConstant.PlanIdMedicarePartD ||
                                                                             i.icdoOrgPlan.plan_id == busConstant.PlanIdGroupLife ||
                                                                             i.icdoOrgPlan.plan_id == busConstant.PlanIdDental ||
                                                                             i.icdoOrgPlan.plan_id == busConstant.PlanIdVision);
                //PIR 7987  Providers that have Flex and other PERS plans (Vision, Dental, Life) should not appear in the list 
                iblnHaveVisionDentalOrLifePlan = ibusProvider.iclbOrgPlan.Any(i => i.icdoOrgPlan.plan_id == busConstant.PlanIdGroupLife ||
                                                                             i.icdoOrgPlan.plan_id == busConstant.PlanIdDental ||
                                                                             i.icdoOrgPlan.plan_id == busConstant.PlanIdVision);
                if (lblnIsProviderToValidate)
                {

                    foreach (busOrgPlan lobjOrgPlan in ibusProvider.iclbOrgPlan)
                    {
                        if (lobjOrgPlan.icdoOrgPlan.plan_id != busConstant.PlanIdFlex)
                        {
                            if (busGlobalFunctions.CheckDateOverlapping(
                                        icdoPersonAccountFlexcompConversion.effective_start_date,
                                        icdoPersonAccountFlexcompConversion.effective_end_date,
                                        lobjOrgPlan.icdoOrgPlan.participation_start_date,
                                        lobjOrgPlan.icdoOrgPlan.participation_end_date))
                            {
                                if (lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdGroupLife)
                                {
                                    busPersonAccountLife lobjLife = new busPersonAccountLife
                                    {
                                        icdoPersonAccount = new cdoPersonAccount(),
                                        icdoPersonAccountLife = new cdoPersonAccountLife(),
                                        iclbLifeOption = new Collection<busPersonAccountLifeOption>()
                                    };
                                    if (ibusPersonAccount.ibusPerson == null)
                                        ibusPersonAccount.LoadPerson();
                                    busPersonAccount lobjPersonAccount = ibusPersonAccount.ibusPerson.LoadActivePersonAccountByPlan(lobjOrgPlan.icdoOrgPlan.plan_id);
                                    lobjLife.icdoPersonAccount = lobjPersonAccount.icdoPersonAccount;
                                    DataTable ldtbResult = Select<cdoPersonAccountLifeOption>(new string[1] { "PERSON_ACCOUNT_ID" },
                                                            new object[1] { lobjPersonAccount.icdoPersonAccount.person_account_id }, null, null);
                                    lobjLife.iclbLifeOption = GetCollection<busPersonAccountLifeOption>(ldtbResult, "icdoPersonAccountLifeOption");
                                    if (lobjLife.IsSupplementalEntered())
                                    {
                                        return true;
                                    }
                                }
                                else
                                {
                                    if (ibusPersonAccount.ibusPerson == null)
                                        ibusPersonAccount.LoadPerson();
                                    if (ibusPersonAccount.ibusPerson.IsMemberEnrolledInPlan(lobjOrgPlan.icdoOrgPlan.plan_id))
                                        return true;
                                }
                            }
                        }
                    }
                    return false;
                }
            }
            return true;
        }

        // UAT PIR ID 1041
        public bool IsProviderEditable()
        {
            Collection<cdoOrganization> lclbProviders = LoadFlexCompProviders();
            if (lclbProviders.Count == 0)
            {
                if (icdoPersonAccountFlexcompConversion.org_id != 0)
                    return false;
            }
            return true;
        }

        public void InsertIntoEnrollmentData()
        {
            if (ibusPersonAccount.IsNull())
                LoadPersonAccount();

            if (ibusPerson.IsNull())
                LoadPerson();

            if (ibusPersonAccountFlexComp.IsNull())
                LoadFlexComp();

            if (ibusPersonEmploymentDetail.IsNull()) LoadPersonEmploymentDetail();
            if (ibusPersonEmploymentDetail.ibusPersonEmployment.IsNull()) ibusPersonEmploymentDetail.LoadPersonEmployment();

            busEnrollmentData lobjEnrollmentData = new busEnrollmentData();
            lobjEnrollmentData.icdoEnrollmentData = new doEnrollmentData();

            if (ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusFlexCompEnrolled && ibusPersonAccount.icdoPersonAccount.status_value == busConstant.PersonAccountStatusValid)
            {
                lobjEnrollmentData.icdoEnrollmentData.source_id = icdoPersonAccountFlexcompConversion.person_account_flex_comp_conversion_id;
                lobjEnrollmentData.icdoEnrollmentData.plan_id = ibusPersonAccount.icdoPersonAccount.plan_id;
                lobjEnrollmentData.icdoEnrollmentData.ssn = ibusPerson.icdoPerson.ssn;
                lobjEnrollmentData.icdoEnrollmentData.ndpers_member_id = ibusPerson.icdoPerson.person_id;
                lobjEnrollmentData.icdoEnrollmentData.peoplesoft_id = ibusPerson.icdoPerson.peoplesoft_id;
                lobjEnrollmentData.icdoEnrollmentData.person_account_id = icdoPersonAccountFlexcompConversion.person_account_id;
                lobjEnrollmentData.icdoEnrollmentData.provider_org_id = icdoPersonAccountFlexcompConversion.org_id;
                lobjEnrollmentData.icdoEnrollmentData.employment_type_value = ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value;
                lobjEnrollmentData.icdoEnrollmentData.change_reason_value = ibusPersonAccountFlexComp.icdoPersonAccountFlexComp.reason_value;
                lobjEnrollmentData.icdoEnrollmentData.employer_org_id = ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id;
                lobjEnrollmentData.icdoEnrollmentData.plan_status_value = ibusPersonAccount.icdoPersonAccount.plan_participation_status_value;
                lobjEnrollmentData.icdoEnrollmentData.start_date = icdoPersonAccountFlexcompConversion.effective_start_date;
                lobjEnrollmentData.icdoEnrollmentData.end_date = icdoPersonAccountFlexcompConversion.effective_end_date;
                lobjEnrollmentData.icdoEnrollmentData.is_benefit_enrollment_report_generated = busConstant.Flag_No;
                lobjEnrollmentData.icdoEnrollmentData.peoplesoft_file_sent_flag = busConstant.Flag_Yes;
                lobjEnrollmentData.icdoEnrollmentData.Insert();
            }
        }
    }
}
