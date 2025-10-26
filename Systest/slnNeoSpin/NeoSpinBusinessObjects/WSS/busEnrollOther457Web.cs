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
using Sagitec.CustomDataObjects;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using NeoSpin.DataObjects;

#endregion


namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busEnrollOther457Web : busExtendBase
    {
        public DateTime idtPayPeriodBeginDate { get; set; }

        public decimal idecAmountPerPayPeriod { get; set; }

        public int person_employment_id { get; set; }

        public int person_employment_detail_id { get; set; }

        public int person_id { get; set; }
        public int org_id { get; set; }

        public int plan_id { get; set; }

        public bool iblnIsFromESS { get; set; }

        public decimal idecOldAmountPerPayPeriod { get; set; }

        public busPersonEmployment ibusPersonEmployment { get; set; }
        public void LoadEmployment()
        {
            if (ibusPersonEmployment == null)
                ibusPersonEmployment = new busPersonEmployment();
            ibusPersonEmployment.FindPersonEmployment(person_employment_id);
        }
        public string istrIsTermsAndConditionsAgreed { get; set; }
        public busPersonEmploymentDetail ibusPersonEmploymentDetail { get; set; }

        public bool IsPersonEnrolledIn457 { get; set; }

        public void LoadEmploymentDetail()
        {
            if (ibusPersonEmploymentDetail == null)
                ibusPersonEmploymentDetail = new busPersonEmploymentDetail();
            ibusPersonEmploymentDetail.FindPersonEmploymentDetail(person_employment_detail_id);
        }

        public busPerson ibusPerson { get; set; }
        public void LoadPerson()
        {
            if (ibusPerson == null)
                ibusPerson = new busPerson();
            ibusPerson.FindPerson(person_id);
        }

        public busOrganization ibusOrganization { get; set; }
        public void LoadOrganization()
        {
            if (ibusOrganization == null)
                ibusOrganization = new busOrganization();
            ibusOrganization.FindOrganization(org_id);
        }
        public override void BeforePersistChanges()
        {
            base.BeforePersistChanges();
            Enroll457();
        }
        public override int PersistChanges()
        {
            return base.PersistChanges();
        }

        public void Enroll457()
        {
            LoadPersonAccountDefComp();
            if (ibusPerson == null)
                LoadPerson();
            //if (ibusPerson.icolPersonAccount == null)  //PIR-15596 Commented to reload the PersonAccount Collection
            ibusPerson.LoadPersonAccount(false);

            busPersonAccount lbusOther457PersonAccount = ibusPerson.icolPersonAccount.Where(i => i.icdoPersonAccount.plan_id == busConstant.PlanIdOther457 &&
                busGlobalFunctions.CheckDateOverlapping(idtPayPeriodBeginDate, i.icdoPersonAccount.start_date, i.icdoPersonAccount.end_date)).FirstOrDefault();

            busPersonAccountDeferredComp lbusPersonAccountDeferredComp = new busPersonAccountDeferredComp();
            if (lbusOther457PersonAccount == null)
            {
                lbusPersonAccountDeferredComp = CreateDeferredCompPersonAccount();
                if (idecAmountPerPayPeriod > 0)
                    CreateDeferredCompProvider(lbusPersonAccountDeferredComp);
            }
            else
            {
                if (lbusPersonAccountDeferredComp.FindPersonAccountDeferredComp(lbusOther457PersonAccount.icdoPersonAccount.person_account_id))
                {
                    //pir 6370 updates the person account employment detail when enrolling into other 457 plan from 2nd org (dual employment)
                    ibusPersonEmploymentDetail.LoadAllPersonAccountEmploymentDetails();
                    if (ibusPersonEmploymentDetail.iclbAllPersonAccountEmpDtl.Where(o => o.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdOther457).Count() == 0)
                    {
                        ibusPersonEmploymentDetail.InsertPersonAccountEmploymentDetail(busConstant.PlanIdOther457);
                    }
                    if (lbusPersonAccountDeferredComp.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusDefCompEnrolled)
                    {
                        lbusPersonAccountDeferredComp.iblnIsFromESS = true;
                        lbusPersonAccountDeferredComp.icdoPersonAccount.person_employment_dtl_id = ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id;
                        lbusPersonAccountDeferredComp.SetPersonAccountIDInPersonAccountEmploymentDetail();
                    }
                    else
                    {
                        lbusPersonAccountDeferredComp.icdoPersonAccount.history_change_date = idtPayPeriodBeginDate;
                        lbusPersonAccountDeferredComp.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusDefCompEnrolled;
                        lbusPersonAccountDeferredComp.icdoPersonAccount.person_employment_dtl_id = ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id;
                        lbusPersonAccountDeferredComp.iarrChangeLog.Add(lbusPersonAccountDeferredComp.icdoPersonAccount);
                        lbusPersonAccountDeferredComp.iarrChangeLog.Add(lbusPersonAccountDeferredComp.icdoPersonAccountDeferredComp);
                        lbusPersonAccountDeferredComp.iblnIsFromESS = true;
                        lbusPersonAccountDeferredComp.icdoPersonAccount.ienuObjectState = ObjectState.Update;
                        lbusPersonAccountDeferredComp.icdoPersonAccountDeferredComp.ienuObjectState = ObjectState.Update;
                        lbusPersonAccountDeferredComp.BeforeValidate(utlPageMode.Update);
                        lbusPersonAccountDeferredComp.BeforePersistChanges();
                        lbusPersonAccountDeferredComp.PersistChanges();
                        lbusPersonAccountDeferredComp.AfterPersistChanges();
                    }
                    lbusPersonAccountDeferredComp.LoadActivePersonAccountProviders();
                    busPersonAccountDeferredCompProvider lbusPersonAccountDeferredCompProvider = new busPersonAccountDeferredCompProvider
                    {
                        icdoPersonAccountDeferredCompProvider = new cdoPersonAccountDeferredCompProvider()
                    };
                    if (ibusPersonEmployment == null)
                        LoadEmployment();
                    lbusPersonAccountDeferredCompProvider = lbusPersonAccountDeferredComp.icolPersonAccountDeferredCompProvider.Where(o =>
                         busGlobalFunctions.CheckDateOverlapping(idtPayPeriodBeginDate, o.icdoPersonAccountDeferredCompProvider.start_date,
                         o.icdoPersonAccountDeferredCompProvider.end_date_no_null) &&
                         o.icdoPersonAccountDeferredCompProvider.person_employment_id == ibusPersonEmployment.icdoPersonEmployment.person_employment_id).FirstOrDefault();
                    if (lbusPersonAccountDeferredCompProvider != null)
                    {
                        if (lbusPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.start_date == idtPayPeriodBeginDate)
                            lbusPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.end_date = idtPayPeriodBeginDate;
                        else
                            lbusPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.end_date =
                                idtPayPeriodBeginDate.AddDays(-1);
                        lbusPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.Update();
                        if (idecAmountPerPayPeriod >= 0) //PIR 9999
                            CreateDeferredCompProvider(lbusPersonAccountDeferredComp);
                    }
                    else if (idecAmountPerPayPeriod > 0)
                    {
                        CreateDeferredCompProvider(lbusPersonAccountDeferredComp);
                    }
                }
            }
        }

        public busPersonAccountDeferredComp CreateDeferredCompPersonAccount()
        {
            busPersonAccountDeferredComp lbusPersonAccountDeferredComp = new busPersonAccountDeferredComp
            {
                icdoPersonAccount = new cdoPersonAccount(),
                icdoPersonAccountDeferredComp = new cdoPersonAccountDeferredComp()
            };
            ibusPersonEmploymentDetail.LoadAllPersonAccountEmploymentDetails();
            if (ibusPersonEmploymentDetail.iclbAllPersonAccountEmpDtl.Where(o => o.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdOther457).Count() == 0)
            {
                ibusPersonEmploymentDetail.InsertPersonAccountEmploymentDetail(busConstant.PlanIdOther457);
            }
            lbusPersonAccountDeferredComp.icdoPersonAccount.person_id = person_id;
            lbusPersonAccountDeferredComp.icdoPersonAccount.plan_id = busConstant.PlanIdOther457;
            lbusPersonAccountDeferredComp.icdoPersonAccount.start_date = idtPayPeriodBeginDate;
            lbusPersonAccountDeferredComp.icdoPersonAccount.history_change_date = idtPayPeriodBeginDate;
            lbusPersonAccountDeferredComp.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusDefCompEnrolled;
            lbusPersonAccountDeferredComp.icdoPersonAccount.person_employment_dtl_id = ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id;
            lbusPersonAccountDeferredComp.icdoPersonAccount.status_value = busConstant.StatusValid;
            lbusPersonAccountDeferredComp.iarrChangeLog.Add(lbusPersonAccountDeferredComp.icdoPersonAccount);
            lbusPersonAccountDeferredComp.iarrChangeLog.Add(lbusPersonAccountDeferredComp.icdoPersonAccountDeferredComp);
            lbusPersonAccountDeferredComp.iblnIsFromESS = true;
            lbusPersonAccountDeferredComp.icdoPersonAccount.ienuObjectState = ObjectState.Insert;
            lbusPersonAccountDeferredComp.icdoPersonAccountDeferredComp.ienuObjectState = ObjectState.Insert;
            lbusPersonAccountDeferredComp.BeforeValidate(utlPageMode.New);
            lbusPersonAccountDeferredComp.BeforePersistChanges();
            lbusPersonAccountDeferredComp.PersistChanges();
            lbusPersonAccountDeferredComp.AfterPersistChanges();
            return lbusPersonAccountDeferredComp;
        }

        public void CreateDeferredCompProvider(busPersonAccountDeferredComp lbusPersonAccountDeferredComp)
        {
            busPersonAccountDeferredCompProvider lbusPersonAccountDeferredCompProvider = new busPersonAccountDeferredCompProvider
            {
                icdoPersonAccountDeferredCompProvider = new cdoPersonAccountDeferredCompProvider()
            };
            lbusPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.person_account_id
                = lbusPersonAccountDeferredComp.icdoPersonAccountDeferredComp.person_account_id;
            lbusPersonAccountDeferredCompProvider.ibusPersonAccountDeferredComp = lbusPersonAccountDeferredComp;
            lbusPersonAccountDeferredCompProvider.ibusPersonAccountDeferredComp.LoadPerson();
            lbusPersonAccountDeferredCompProvider.ibusPersonAccountDeferredComp.icdoPersonAccount.person_employment_dtl_id = person_employment_detail_id;
            lbusPersonAccountDeferredCompProvider.ibusPersonAccountDeferredComp.LoadPersonEmploymentDetail();
            lbusPersonAccountDeferredCompProvider.ibusPersonAccountDeferredComp.ibusPersonEmploymentDetail.LoadPersonEmployment();
            lbusPersonAccountDeferredCompProvider.ibusPersonAccountDeferredComp.LoadOrgPlan();
            lbusPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.per_pay_period_contribution_amt = idecAmountPerPayPeriod;
            lbusPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.start_date = idtPayPeriodBeginDate;
            //string lstrProvider = iobjPassInfo.isrvDBCache.GetCodeDescription(52, busConstant.Provider_Other457).Rows.Count > 0 ?
            //    iobjPassInfo.isrvDBCache.GetCodeDescription(52, busConstant.Provider_Other457).Rows[0]["data1"].ToString() : string.Empty;
            lbusPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.istrProviderOrgCode = istrProviderOrgCode;
            lbusPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.person_employment_id =
                lbusPersonAccountDeferredCompProvider.ibusPersonAccountDeferredComp.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.person_employment_id;
            lbusPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.ienuObjectState = ObjectState.Insert;
            //prod pir 7176,7177,7178
            lbusPersonAccountDeferredCompProvider.iblnFromESS = true;
            lbusPersonAccountDeferredCompProvider.BeforeValidate(utlPageMode.New);
            //PIR 12576 - Provider_agent_contact_id must be updated with org contact id, not contact id
            DataTable ldtProviderAgent = DBFunction.DBSelect("cdoOrgContact.LoadAgentOrgContact", new object[2] {lbusPersonAccountDeferredCompProvider.ibusProviderOrgPlan.icdoOrgPlan.org_id,
                                lbusPersonAccountDeferredCompProvider.ibusProviderOrgPlan.icdoOrgPlan.plan_id}, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            //Select<cdoOrgContact>(new string[3] { "org_id", "plan_id", "status_value" },
            //new object[3]{lbusPersonAccountDeferredCompProvider.ibusProviderOrgPlan.icdoOrgPlan.org_id,
            //                lbusPersonAccountDeferredCompProvider.ibusProviderOrgPlan.icdoOrgPlan.plan_id,
            //                busConstant.StatusActive}, null, null);
            if (ldtProviderAgent.Rows.Count > 0)
            {
                lbusPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.provider_agent_contact_id =
                    Convert.ToInt32(ldtProviderAgent.Rows[0]["org_contact_id"]);
            }
            lbusPersonAccountDeferredCompProvider.BeforePersistChanges();
            lbusPersonAccountDeferredCompProvider.PersistChanges();
            lbusPersonAccountDeferredCompProvider.AfterPersistChanges();
        }
        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            //LoadPersonAccountDefComp();
            ibusPerson.ESSLoadPersonAccountForEnrolledPlans();
        }

        public string istrProviderOrgCode { get; set; }
        public bool iblnValidProviderOrgPlanExists { get; set; }
        public void LoadProviderOrgCode()
        {
            DataTable ldtOrgPlan = Select<cdoOrgPlan>(new string[2] { "org_id", "plan_id" },
                                                    new object[2] { org_id, busConstant.PlanIdOther457 }, null, null);
            int lintOrgPlanID = 0;
            foreach (DataRow dr in ldtOrgPlan.Rows)
            {
                if (busGlobalFunctions.CheckDateOverlapping(idtPayPeriodBeginDate, Convert.ToDateTime(dr["participation_start_date"]),
                    dr["participation_end_date"] == DBNull.Value ? DateTime.MaxValue : Convert.ToDateTime(dr["participation_end_date"])))
                {
                    lintOrgPlanID = Convert.ToInt32(dr["org_plan_id"]);
                    break;
                }
            }
            if (lintOrgPlanID > 0)
            {
                DataTable ldtOrgPlanProvider = Select<cdoOrgPlanProvider>(new string[2] { "org_plan_id", "status_value" },
                                                new object[2] { lintOrgPlanID, busConstant.StatusActive }, null, null);
                if (ldtOrgPlanProvider.Rows.Count == 1)
                {
                    iblnValidProviderOrgPlanExists = true;
                    busOrgPlanProvider lobjOrgPlanProvd = new busOrgPlanProvider { icdoOrgPlanProvider = new cdoOrgPlanProvider() };
                    lobjOrgPlanProvd.icdoOrgPlanProvider.LoadData(ldtOrgPlanProvider.Rows[0]);
                    lobjOrgPlanProvd.LoadProviderOrg();
                    istrProviderOrgCode = lobjOrgPlanProvd.ibusProviderOrg.icdoOrganization.org_code;
                }
            }
        }

        public override void BeforeWizardStepValidate(utlPageMode aenmPageMode, string astrWizardName, string astrWizardStepName, utlWizardNavigationEventArgs we = null)
        {
            base.BeforeWizardStepValidate(aenmPageMode, astrWizardName, astrWizardStepName);
            LoadProviderOrgCode();
            LoadPersonAccountDefComp();
        }

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            base.BeforeValidate(aenmPageMode);
            LoadProviderOrgCode();
            LoadPersonAccountDefComp();
        }

        public bool iblnNoEnrollmentAndAmountIsZero { get; set; }
        public void LoadPersonAccountDefComp()
        {
            if (ibusPerson == null)
                LoadPerson();
            if (ibusPerson.icolPersonAccount == null)
                ibusPerson.LoadPersonAccount(false);

            busPersonAccount lbusOther457PersonAccount = ibusPerson.icolPersonAccount.Where(i => i.icdoPersonAccount.plan_id == busConstant.PlanIdOther457 &&
                busGlobalFunctions.CheckDateOverlapping(idtPayPeriodBeginDate, i.icdoPersonAccount.start_date, i.icdoPersonAccount.end_date)).FirstOrDefault();

            busPersonAccountDeferredComp lbusPersonAccountDeferredComp = new busPersonAccountDeferredComp();
            if (idecAmountPerPayPeriod <= 0)
            {
                if (lbusOther457PersonAccount == null)
                {
                    iblnNoEnrollmentAndAmountIsZero = true;
                }
                else
                {
                    if (lbusPersonAccountDeferredComp.FindPersonAccountDeferredComp(lbusOther457PersonAccount.icdoPersonAccount.person_account_id))
                    {
                        lbusPersonAccountDeferredComp.LoadActivePersonAccountProviders();
                        busPersonAccountDeferredCompProvider lbusPersonAccountDeferredCompProvider = new busPersonAccountDeferredCompProvider
                        {
                            icdoPersonAccountDeferredCompProvider = new cdoPersonAccountDeferredCompProvider()
                        };
                        if (ibusPersonEmployment == null)
                            LoadEmployment();
                        lbusPersonAccountDeferredCompProvider = lbusPersonAccountDeferredComp.icolPersonAccountDeferredCompProvider.Where(o =>
                             busGlobalFunctions.CheckDateOverlapping(idtPayPeriodBeginDate, o.icdoPersonAccountDeferredCompProvider.start_date,
                             o.icdoPersonAccountDeferredCompProvider.end_date_no_null) &&
                             o.icdoPersonAccountDeferredCompProvider.person_employment_id == ibusPersonEmployment.icdoPersonEmployment.person_employment_id).FirstOrDefault();
                        if (lbusPersonAccountDeferredCompProvider == null)
                        {
                            iblnNoEnrollmentAndAmountIsZero = true;
                        }
                        else
                        {
                            iblnNoEnrollmentAndAmountIsZero = false;
                        }
                    }
                    else
                    {
                        iblnNoEnrollmentAndAmountIsZero = false;
                    }
                }
            }
            else
            {
                iblnNoEnrollmentAndAmountIsZero = false;
            }
        }

        /// <summary>
        /// prod pir 6110 : to check any future enrollment for 457 plan exists
        /// </summary>
        /// <returns></returns>
        public bool IsFuture457PersonAccountExists()
        {
            bool lblnResult = false;

            if (ibusPerson == null)
                LoadPerson();
            if (ibusPerson.icolPersonAccount == null)
                ibusPerson.LoadPersonAccount(false);

            busPersonAccount lbusOther457PersonAccount = ibusPerson.icolPersonAccount.Where(i => i.icdoPersonAccount.plan_id == busConstant.PlanIdOther457).FirstOrDefault();

            if (lbusOther457PersonAccount != null && lbusOther457PersonAccount.icdoPersonAccount.start_date > idtPayPeriodBeginDate)
                lblnResult = true;

            return lblnResult;
        }

        public bool IsEnrolledIn457()
        {
            if (ibusPerson == null)
                LoadPerson();
            if (ibusPerson.icolPersonAccount == null)
                ibusPerson.LoadPersonAccount(false);

            busPersonAccount lbusOther457PersonAccount = ibusPerson.icolPersonAccount.Where(i => i.icdoPersonAccount.plan_id == busConstant.PlanIdOther457 &&
                busGlobalFunctions.CheckDateOverlapping(idtPayPeriodBeginDate, i.icdoPersonAccount.start_date, i.icdoPersonAccount.end_date)).FirstOrDefault();

            busPersonAccountDeferredComp lbusPersonAccountDeferredComp = new busPersonAccountDeferredComp();


            if (lbusOther457PersonAccount != null)
            {
                if (lbusPersonAccountDeferredComp.FindPersonAccountDeferredComp(lbusOther457PersonAccount.icdoPersonAccount.person_account_id))
                {
                    lbusPersonAccountDeferredComp.LoadActivePersonAccountProviders();
                    busPersonAccountDeferredCompProvider lbusPersonAccountDeferredCompProvider = new busPersonAccountDeferredCompProvider
                    {
                        icdoPersonAccountDeferredCompProvider = new cdoPersonAccountDeferredCompProvider()
                    };
                    if (ibusPersonEmployment == null)
                        LoadEmployment();
                    //PIR 9999
                    lbusPersonAccountDeferredCompProvider = lbusPersonAccountDeferredComp.icolPersonAccountDeferredCompProvider.Where(o =>
                             busGlobalFunctions.CheckDateOverlapping(idtPayPeriodBeginDate, o.icdoPersonAccountDeferredCompProvider.start_date,
                             o.icdoPersonAccountDeferredCompProvider.end_date_no_null) &&
                             o.icdoPersonAccountDeferredCompProvider.person_employment_id == ibusPersonEmployment.icdoPersonEmployment.person_employment_id).FirstOrDefault();

                    //PIR 13930 - Added code to check Null condition
                    if (lbusPersonAccountDeferredCompProvider.IsNotNull() && lbusPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.per_pay_period_contribution_amt > 0)
                    {
                        idecOldAmountPerPayPeriod = lbusPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.per_pay_period_contribution_amt;
                        IsPersonEnrolledIn457 = true;
                    }
                    else
                    {
                        IsPersonEnrolledIn457 = false;
                    }
                }
            }
            return IsPersonEnrolledIn457;
        }
        public override void ValidateGroupRules(string astrGroupName, utlPageMode aenmPageMode)
        {
            base.ValidateGroupRules(astrGroupName, aenmPageMode);
            if (iblnIsFromESS)
            {
                foreach (utlError lobjError in iarrErrors)
                {
                    lobjError.istrErrorID = string.Empty;
                }
            }


        }

        /// <summary>
        /// PIR 21234 - Validate Pay Period Amount Based On Org's Pay period frequency
        /// </summary>
        /// <returns></returns>
        public bool IsAmtPerPayPeriodAsPerOrgFrequency()
        {
            if (ibusOrganization.IsNull()) LoadOrganization();
            if (ibusOrganization.iclbOrgPlan.IsNull()) ibusOrganization.LoadOrgPlan();
            busOrgPlan lbusOrgPlan = ibusOrganization
                                    .iclbOrgPlan
                                    .FirstOrDefault(orgplan => orgplan.icdoOrgPlan.plan_id == busConstant.PlanIdOther457 &&
                                    busGlobalFunctions
                                    .CheckDateOverlapping(idtPayPeriodBeginDate, orgplan.icdoOrgPlan.participation_start_date, orgplan.icdoOrgPlan.participation_end_date));
            if (lbusOrgPlan.IsNotNull())
            {
                string lstrReportFreqValue = lbusOrgPlan.icdoOrgPlan.report_frequency_value;
                if (!string.IsNullOrEmpty(lstrReportFreqValue))
                {
                    decimal ldecComparedAmt = 0.0M;
                    if ((lbusOrgPlan.icdoOrgPlan.report_frequency_value == busConstant.DeffCompFrequencyBiWeekly) ||
                              (lbusOrgPlan.icdoOrgPlan.report_frequency_value == busConstant.DeffCompFrequencySemiMonthly))
                    {
                        ldecComparedAmt = 12.50M;
                    }
                    else if (lbusOrgPlan.icdoOrgPlan.report_frequency_value == busConstant.DeffCompFrequencyMonthly)
                    {
                        ldecComparedAmt = 25.00M;
                    }
                    if (ldecComparedAmt > 0.0M && idecAmountPerPayPeriod > 0)
                    {
                        return idecAmountPerPayPeriod >= ldecComparedAmt;
                    }
                }
            }
            return true;
        }
        public bool IsPayPeriodBeginDatePriorToExistingRecord()
        {
            if (ibusPerson == null)
                LoadPerson();
            if (ibusPerson.icolPersonAccount == null)
                ibusPerson.LoadPersonAccount(false);
            if (ibusPersonEmployment.IsNull())
                LoadEmployment();
            busPersonAccount lbusOther457PersonAccount = ibusPerson.icolPersonAccount.FirstOrDefault(i => i.icdoPersonAccount.plan_id == busConstant.PlanIdOther457);
            busPersonAccountDeferredCompProvider lobjDefCompProviders = new busPersonAccountDeferredCompProvider { icdoPersonAccountDeferredCompProvider = new cdoPersonAccountDeferredCompProvider() };
            if (lbusOther457PersonAccount.IsNotNull())
            {
                DataTable ldtbList = Select<cdoPersonAccountDeferredCompProvider>(
                    new string[3] { "person_account_id", "end_date", "person_employment_id" },
                    new object[3] { lbusOther457PersonAccount.icdoPersonAccount.person_account_id, DateTime.MinValue, ibusPersonEmployment.icdoPersonEmployment.person_employment_id }, null, null);
                if (ldtbList.Rows.Count > 0)
                {
                    lobjDefCompProviders.icdoPersonAccountDeferredCompProvider.LoadData(ldtbList.Rows[0]);
                    if (this.idtPayPeriodBeginDate < lobjDefCompProviders.icdoPersonAccountDeferredCompProvider.start_date)
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
            return false;
        }
    }
}
