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
using System.Linq;
using Sagitec.Bpm;
#endregion

namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class NeoSpin.BusinessObjects.busWssEmploymentChangeRequest:
    /// Inherited from busWssEmploymentChangeRequestGen, the class is used to customize the business object busWssEmploymentChangeRequestGen.
    /// </summary>
    [Serializable]
    public class busWssEmploymentChangeRequest : busWssEmploymentChangeRequestGen
    {
        public string istrIsEmployeeOnTeachingContract { get; set; }
        public bool iblnClassificationChange { get; set; }
        public bool iblnEmploymentTypeChange { get; set; }
        public bool iblnLoa { get; set; }
        public string istrTypeOfChange { get; set; }


        public bool iblnisFromPs { get; set; } // PIR-11030
        public bool iblnIsFromESS { get; set; } // PIR 14073
        public bool iblnIsESSTerminateEmployment { get; set; } // PIR 23963
        //For PIR 7952
        public busContact ibusContact { get; set; }
        public int iintContact_id { get; set; }
        public string lstrOldStatusValue { get; set; }

        public busPersonEmployment ibusPersonEmployment { get; set; }
        public void LoadEmployment()
        {
            if (ibusPersonEmployment == null)
                ibusPersonEmployment = new busPersonEmployment();
            ibusPersonEmployment.FindPersonEmployment(icdoWssEmploymentChangeRequest.person_employment_id);
        }

        public busPersonEmploymentDetail ibusPersonEmploymentDetail { get; set; }
        public void LoadEmploymentDetail()
        {
            if (ibusPersonEmploymentDetail == null)
                ibusPersonEmploymentDetail = new busPersonEmploymentDetail();
            ibusPersonEmploymentDetail.FindPersonEmploymentDetail(icdoWssEmploymentChangeRequest.person_employment_detail_id);
        }

        public busPerson ibusPerson { get; set; }
        public void LoadPerson()
        {
            if (ibusPerson == null)
                ibusPerson = new busPerson();
            ibusPerson.FindPerson(icdoWssEmploymentChangeRequest.person_id);
        }

        public busOrganization ibusOrganization { get; set; }
        public void LoadOrganization()
        {
            if (ibusOrganization == null)
                ibusOrganization = new busOrganization();
            ibusOrganization.FindOrganization(icdoWssEmploymentChangeRequest.org_id);
        }
        public string istrLess12hourWorkIndicator { get; set; }
        public string istrIsTermsAndConditionsAgreed { get; set; }
        //PIR 24663
        //start
        public Collection<cdoCodeValue> LoadRetirementParticipationStatusValueDropdown()
        {
            if(ibusOrganization == null)
            {
                LoadOrganization();
            }
            return ibusOrganization.LoadEmployerStatusValueDropdown();
        }
        //end
        public Collection<cdoCodeValue> LoadJobClassByEmployerCategory()
        {
            Collection<cdoCodeValue> lclbJobClasses = new Collection<cdoCodeValue>();
            //PROD Pir - 4555
            if (ibusOrganization.iclbOrgPlan == null)
                ibusOrganization.LoadOrgPlan();
            bool lblnLEExists = ibusOrganization.iclbOrgPlan.Where(o => o.icdoOrgPlan.plan_id == busConstant.PlanIdLE ||
                                                                   o.icdoOrgPlan.plan_id == busConstant.PlanIdLEWithoutPS ||
                                                                   o.icdoOrgPlan.plan_id == busConstant.PlanIdBCILawEnf || // pir 7943
                                                                   o.icdoOrgPlan.plan_id == busConstant.PlanIdStatePublicSafety || // PIR 25729
                                                                   o.icdoOrgPlan.plan_id == busConstant.PlanIdNG).Any(); //PIR 25729
            if (icdoWssEmploymentChangeRequest.change_type_value == busConstant.EmploymentChangeRequestChangeTypeClassification || icdoWssEmploymentChangeRequest.change_type_value == busConstant.EmploymentChangeRequestChangeTypeEmployment)
            {
                DataTable ldtbResult = Select<cdoCodeValue>(new string[1] { "CODE_ID" }, new object[1] { 322 }, null, null);
                ldtbResult = ldtbResult.AsEnumerable().Where(o =>
                    (o.Field<string>("DATA1") == ibusOrganization.icdoOrganization.emp_category_value) &&
                    ((o.Field<string>("DATA3") == ibusOrganization.icdoOrganization.org_code) || (o.Field<string>("DATA3").IsEmpty()))).AsDataTable();
                DataTable ldtbTemp = Select<cdoCodeValue>(new string[1] { "CODE_ID" }, new object[1] { 314 }, null, null);
                foreach (DataRow ldtr in ldtbResult.Rows)
                {
                    foreach (DataRow ldtrRow in ldtbTemp.Rows)
                    {
                        if ((Convert.ToString(ldtrRow["CODE_VALUE"]) == (Convert.ToString(ldtr["DATA2"])))
                            && (Convert.ToString(ldtrRow["DATA1"]) == busConstant.Flag_Yes))
                        {
                            //PROD Pir 4555
                            if ((Convert.ToString(ldtrRow["CODE_VALUE"]) == busConstant.JobClassCorrectionalOfficer ||
                                Convert.ToString(ldtrRow["CODE_VALUE"]) == busConstant.JobClassPeaceOfficer) && lblnLEExists)
                            {
                                cdoCodeValue lcdoCV = new cdoCodeValue();
                                lcdoCV.LoadData(ldtrRow);
                                lclbJobClasses.Add(lcdoCV);
                            }
                            else if (Convert.ToString(ldtrRow["CODE_VALUE"]) != busConstant.JobClassCorrectionalOfficer &&
                                Convert.ToString(ldtrRow["CODE_VALUE"]) != busConstant.JobClassPeaceOfficer)
                            {
                                cdoCodeValue lcdoCV = new cdoCodeValue();
                                lcdoCV.LoadData(ldtrRow);
                                lclbJobClasses.Add(lcdoCV);
                            }
                        }
                    }
                }
            }
            return lclbJobClasses;
        }
        public bool iblnIsNewMode = false;
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
        public override void BeforeWizardStepValidate(utlPageMode aenmPageMode, string astrWizardName, string astrWizardStepName, utlWizardNavigationEventArgs we = null)
        {

            if (aenmPageMode == utlPageMode.New)
                iblnIsNewMode = true;
            else
                icdoWssEmploymentChangeRequest.status_value = busConstant.EmploymentChangeRequestStatusReview;
            switch (astrWizardStepName)
            {
                case "wzsLOA":
                    if (ibusPerson == null)
                        LoadPerson();
                    ibusPerson.iintOrgID = ibusOrganization.icdoOrganization.org_id;
                    ibusPerson.ESSLoadPersonEmployment();
                    this.LoadCodeDescription();
                    break;
                case "wzsEmpTypeChange":
                    if (icdoWssEmploymentChangeRequest.employment_type_change_effective_date != DateTime.MinValue)
                        icdoWssEmploymentChangeRequest.job_class_change_effective_date = icdoWssEmploymentChangeRequest.employment_type_change_effective_date;
                    if (icdoWssEmploymentChangeRequest.job_class_value != busConstant.PersonJobClassStateAppointedOfficial &&
                        icdoWssEmploymentChangeRequest.job_class_value != busConstant.PersonJobClassNonStateAppointedOfficial)
                        icdoWssEmploymentChangeRequest.official_list_value = string.Empty;
                    if (icdoWssEmploymentChangeRequest.job_class_value != busConstant.PersonJobClassStateElectedOfficial && icdoWssEmploymentChangeRequest.job_class_value != busConstant.PersonJobClassNonStateElectedOfficial)
                        icdoWssEmploymentChangeRequest.term_begin_date = DateTime.MinValue;
                    if (istrLess12hourWorkIndicator != busConstant.Flag_Yes_Value)
                    {
                        icdoWssEmploymentChangeRequest.seasonal_description = string.Empty;
                        icdoWssEmploymentChangeRequest.seasonal_value = string.Empty;
                    }
                    LoadCodeDescription();
                    break;
                case "wzsTerminateEmployee":
                    icdoWssEmploymentChangeRequest.is_on_teaching_contract_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(311,
                                                                        icdoWssEmploymentChangeRequest.is_on_teaching_contract_value);
                    break;
                default:
                    LoadCodeDescription();
                    break;
            }
            if (!string.IsNullOrEmpty(icdoWssEmploymentChangeRequest.istrLastRetirementTransmittalOfDeduction))
                icdoWssEmploymentChangeRequest.last_retirement_transmittal_of_deduction = Convert.ToDateTime(icdoWssEmploymentChangeRequest.istrLastRetirementTransmittalOfDeduction.ToString());
            if (!string.IsNullOrEmpty(icdoWssEmploymentChangeRequest.istrLastMonthOnEmployerBilling))
                icdoWssEmploymentChangeRequest.last_month_on_employer_billing = Convert.ToDateTime(icdoWssEmploymentChangeRequest.istrLastMonthOnEmployerBilling.ToString());
            EvaluateInitialLoadRules();
            base.BeforeWizardStepValidate(aenmPageMode, astrWizardName, astrWizardStepName);
        }

        private void LoadCodeDescription()
        {
            if (!icdoWssEmploymentChangeRequest.job_class_value.IsNullOrEmpty())
            {
                icdoWssEmploymentChangeRequest.job_class_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(314, icdoWssEmploymentChangeRequest.job_class_value);
            }

            if (!icdoWssEmploymentChangeRequest.type_value.IsNullOrEmpty())
            {
                icdoWssEmploymentChangeRequest.type_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(313, icdoWssEmploymentChangeRequest.type_value);
            }

            if (!icdoWssEmploymentChangeRequest.official_list_value.IsNullOrEmpty())
            {
                icdoWssEmploymentChangeRequest.official_list_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(318, icdoWssEmploymentChangeRequest.official_list_value);
            }

            if (!icdoWssEmploymentChangeRequest.hourly_value.IsNullOrEmpty())
            {
                icdoWssEmploymentChangeRequest.hourly_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(311, icdoWssEmploymentChangeRequest.hourly_value);
            }

            if (!icdoWssEmploymentChangeRequest.seasonal_value.IsNullOrEmpty())
            {
                icdoWssEmploymentChangeRequest.seasonal_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(312, icdoWssEmploymentChangeRequest.seasonal_value);
            }

            if (!icdoWssEmploymentChangeRequest.employment_status_value.IsNullOrEmpty())
                icdoWssEmploymentChangeRequest.employment_status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(310, icdoWssEmploymentChangeRequest.employment_status_value);
            if (!icdoWssEmploymentChangeRequest.change_type_value.IsNullOrEmpty())
                icdoWssEmploymentChangeRequest.change_type_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(3506, icdoWssEmploymentChangeRequest.change_type_value);

        }
        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            if (iblnIsNewMode)
                InitializeWorkFlow();
            else if (icdoWssEmploymentChangeRequest.ihstOldValues.Count > 0
                && lstrOldStatusValue == busConstant.EmploymentChangeRequestStatusRejected)
            {
                DataTable ldtRunningInstance = busWorkflowHelper.LoadRunningInstancesByPersonAndProcess(icdoWssEmploymentChangeRequest.person_id, busConstant.Map_Employment_Change_Request);
                if (ldtRunningInstance.Rows.Count > 0)
                {
                    //busActivityInstance lbusActivityInstance = new busActivityInstance { icdoActivityInstance = new cdoActivityInstance() };
                    //lbusActivityInstance.icdoActivityInstance.LoadData(ldtRunningInstance.Rows[0]);
                    busSolBpmCaseInstance lbusBpmCaseInstance = new busSolBpmCaseInstance();
                    busBpmActivityInstance lbusActivityInstance = lbusBpmCaseInstance.LoadWithActivityInst(Convert.ToInt32(ldtRunningInstance.Rows[0]["activity_instance_id"]));
                    busWorkflowHelper.UpdateWorkflowActivityByStatus(busConstant.ActivityStatusResumed, lbusActivityInstance, iobjPassInfo);
                }
                else
                {
                    InitializeWorkFlow();
                }
            }
        }
        public override void BeforePersistChanges()
        {
            if (icdoWssEmploymentChangeRequest.ihstOldValues.Count > 0)
                lstrOldStatusValue = icdoWssEmploymentChangeRequest.ihstOldValues["status_value"].ToString();
            base.BeforePersistChanges();
        }

        private void InitializeWorkFlow()
        {
            busWorkflowHelper.InitiateBpmRequest(busConstant.Map_Employment_Change_Request, icdoWssEmploymentChangeRequest.person_id, 0, icdoWssEmploymentChangeRequest.employment_change_request_id, iobjPassInfo, busConstant.WorkflowProcessSource_Batch);
        }

        public ArrayList Post_click()
        {
            ArrayList larrReturn = new ArrayList();
            utlError lobjError = new utlError();

            //PIR 10237 --  User should not allow to modify their own record 
            DataTable dtUserInfo = iobjPassInfo.isrvDBCache.GetUserInfo(iobjPassInfo.istrUserID);

            if (dtUserInfo?.Rows.Count > 0 && !String.IsNullOrEmpty(Convert.ToString(dtUserInfo.Rows[0]["Person_ID"])) && icdoWssEmploymentChangeRequest.person_id == Convert.ToInt32(dtUserInfo.Rows[0]["Person_ID"]))
            {
                utlError lerror = new utlError();
                lerror.istrErrorID = "10275";
                lerror.istrErrorMessage = "Request can not be posted by the same user who created the request.";
                larrReturn.Add(lerror);
                return larrReturn;

            }

            if (ibusPerson == null)
                LoadPerson();
            if (ibusPerson.ibusESSPersonEmployment == null)
                ibusPerson.ESSLoadPersonEmployment();
            busPersonEmploymentDetail lobjPersonEmploymentDetail = ibusPerson.ibusESSPersonEmployment.ibusESSLatestPersonEmploymentDetail;
            ibusPerson.ibusESSPersonEmployment.ibusESSLatestPersonEmploymentDetail.LoadAllPersonAccountEmploymentDetails();
            Collection<busPersonAccountEmploymentDetail> lclbPAEmpDetail = ibusPerson.ibusESSPersonEmployment.ibusESSLatestPersonEmploymentDetail.iclbAllPersonAccountEmpDtl;
            DateTime ldtStartDate = DateTime.MinValue;
            // PIR 25845
            DateTime ldtEmploymentEndDate = Convert.ToDateTime(icdoWssEmploymentChangeRequest.istrEmploymentEndDateDefault);

            if (icdoWssEmploymentChangeRequest.employment_end_date != DateTime.MinValue &&
                ibusPerson.ibusESSPersonEmployment.ibusESSLatestPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date > icdoWssEmploymentChangeRequest.employment_end_date)
            {
                lobjError = AddError(8535, "");
                larrReturn.Add(lobjError);
                return larrReturn;
            }
            //PIR 25101 - Error moved from ESS Employee termination wizard to Post click on LOB
            if (ldtEmploymentEndDate != DateTime.MinValue && ibusPerson.ibusESSPersonEmployment.icdoPersonEmployment.start_date != DateTime.MinValue &&
                ldtEmploymentEndDate < ibusPerson.ibusESSPersonEmployment.ibusESSLatestPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date)
            {
                lobjError = AddError(8584, "");
                larrReturn.Add(lobjError);
                return larrReturn;
            }

            if (!string.IsNullOrEmpty(icdoWssEmploymentChangeRequest.change_type_value))
            {
                if (icdoWssEmploymentChangeRequest.change_type_value != busConstant.EmploymentChangeRequestChangeTypeLOAR)// PIR 8560
                {

                    if (icdoWssEmploymentChangeRequest.change_type_value == busConstant.EmploymentChangeRequestChangeTypeLOA ||
                        icdoWssEmploymentChangeRequest.change_type_value == busConstant.EmploymentChangeRequestChangeTypeLOAM ||
                        icdoWssEmploymentChangeRequest.change_type_value == busConstant.EmploymentStatusFMLA) //pir 8127
                    {
                        if ((ibusPerson.ibusESSPersonEmployment.ibusESSLatestPersonEmploymentDetail.icdoPersonEmploymentDetail.status_value ==
                            busConstant.EmploymentStatusContributing || // pir 7391
                            ibusPerson.ibusESSPersonEmployment.ibusESSLatestPersonEmploymentDetail.icdoPersonEmploymentDetail.status_value ==
                            busConstant.EmploymentStatusNonContributing
                            ) && icdoWssEmploymentChangeRequest.loa_start_date != DateTime.MinValue) //pir 8127,8528
                        {
                            if (ibusPerson.ibusESSPersonEmployment.ibusESSLatestPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date.Date == icdoWssEmploymentChangeRequest.loa_start_date.Date)
                                ibusPerson.ibusESSPersonEmployment.ibusESSLatestPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date = icdoWssEmploymentChangeRequest.loa_start_date;
                            else
                                ibusPerson.ibusESSPersonEmployment.ibusESSLatestPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date =
                                    icdoWssEmploymentChangeRequest.loa_start_date.AddDays(-1);

                            ldtStartDate = icdoWssEmploymentChangeRequest.loa_start_date;
                        }
                        if ((ibusPerson.ibusESSPersonEmployment.ibusESSLatestPersonEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOA ||
                            ibusPerson.ibusESSPersonEmployment.ibusESSLatestPersonEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOAM ||
                            ibusPerson.ibusESSPersonEmployment.ibusESSLatestPersonEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusFMLA)
                            && icdoWssEmploymentChangeRequest.date_of_return != DateTime.MinValue) //pir 8127,8528
                        {
                            if (ibusPerson.ibusESSPersonEmployment.ibusESSLatestPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date.Date == icdoWssEmploymentChangeRequest.date_of_return.Date)
                                ibusPerson.ibusESSPersonEmployment.ibusESSLatestPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date = icdoWssEmploymentChangeRequest.date_of_return;
                            else
                                ibusPerson.ibusESSPersonEmployment.ibusESSLatestPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date = icdoWssEmploymentChangeRequest.date_of_return.AddDays(-1);
                            ldtStartDate = icdoWssEmploymentChangeRequest.date_of_return;
                        }
                        if ((ibusPerson.ibusESSPersonEmployment.ibusESSLatestPersonEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOA ||
                            ibusPerson.ibusESSPersonEmployment.ibusESSLatestPersonEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOAM ||
                            ibusPerson.ibusESSPersonEmployment.ibusESSLatestPersonEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusFMLA)
                            && icdoWssEmploymentChangeRequest.recertified_date != DateTime.MinValue)
                        {
                            lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.recertified_date = icdoWssEmploymentChangeRequest.recertified_date;
                        }

                        ibusPerson.ibusESSPersonEmployment.ibusESSLatestPersonEmploymentDetail.icdoPersonEmploymentDetail.Update();// pir 7901
                    }
                    else if (icdoWssEmploymentChangeRequest.change_type_value == busConstant.EmploymentChangeRequestChangeTypeClassification)
                    {
                        if (ibusPerson.ibusESSPersonEmployment.ibusESSLatestPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date.Date ==
                                                                    icdoWssEmploymentChangeRequest.job_class_change_effective_date.Date)
                            ibusPerson.ibusESSPersonEmployment.ibusESSLatestPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date =
                                icdoWssEmploymentChangeRequest.job_class_change_effective_date;
                        else
                            ibusPerson.ibusESSPersonEmployment.ibusESSLatestPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date =
                                    icdoWssEmploymentChangeRequest.job_class_change_effective_date.AddDays(-1);
                        ldtStartDate = icdoWssEmploymentChangeRequest.job_class_change_effective_date;
                        ibusPerson.ibusESSPersonEmployment.ibusESSLatestPersonEmploymentDetail.icdoPersonEmploymentDetail.Update();// pir 7901
                        lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.status_value = icdoWssEmploymentChangeRequest.employment_status_value; // PIR 12910
                    }
                    else if (icdoWssEmploymentChangeRequest.change_type_value == busConstant.EmploymentChangeRequestChangeTypeEmployment)
                    {
                        if (ibusPerson.ibusESSPersonEmployment.ibusESSLatestPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date.Date ==
                                                                    icdoWssEmploymentChangeRequest.employment_type_change_effective_date.Date)
                            ibusPerson.ibusESSPersonEmployment.ibusESSLatestPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date =
                                icdoWssEmploymentChangeRequest.employment_type_change_effective_date;
                        else
                            ibusPerson.ibusESSPersonEmployment.ibusESSLatestPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date =
                                    icdoWssEmploymentChangeRequest.employment_type_change_effective_date.AddDays(-1);
                        ldtStartDate = icdoWssEmploymentChangeRequest.employment_type_change_effective_date;
                        ibusPerson.ibusESSPersonEmployment.ibusESSLatestPersonEmploymentDetail.icdoPersonEmploymentDetail.Update();// pir 7901
                        //prod pir 5136
                        lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.status_value = icdoWssEmploymentChangeRequest.employment_status_value;
                    }
                    //ibusPerson.ibusESSPersonEmployment.ibusESSLatestPersonEmploymentDetail.icdoPersonEmploymentDetail.Update();// pir 7901
                    if (ldtStartDate != DateTime.MinValue) //pir 8127,8528
                    {
                        lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date = ldtStartDate;
                        if (icdoWssEmploymentChangeRequest.employment_end_date != DateTime.MinValue)
                            lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date = icdoWssEmploymentChangeRequest.employment_end_date;
                        else
                            lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date = DateTime.MinValue;
                        if (!string.IsNullOrEmpty(icdoWssEmploymentChangeRequest.job_class_value))
                            lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.job_class_value = icdoWssEmploymentChangeRequest.job_class_value;
                        if (!string.IsNullOrEmpty(icdoWssEmploymentChangeRequest.type_value))
                            lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value = icdoWssEmploymentChangeRequest.type_value;
                        //PROD PIR 4139
                        //even null value is allowed
                        // if (!string.IsNullOrEmpty(icdoWssEmploymentChangeRequest.seasonal_value))
                        lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.seasonal_value = icdoWssEmploymentChangeRequest.seasonal_value;
                        // if (!string.IsNullOrEmpty(icdoWssEmploymentChangeRequest.hourly_value))
                        lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.hourly_value = icdoWssEmploymentChangeRequest.hourly_value;
                        // if (!string.IsNullOrEmpty(icdoWssEmploymentChangeRequest.official_list_value))
                        lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.official_list_value = icdoWssEmploymentChangeRequest.official_list_value;
                        // if (icdoWssEmploymentChangeRequest.term_begin_date != DateTime.MinValue)
                        lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.term_begin_date = icdoWssEmploymentChangeRequest.term_begin_date;
                        // if (icdoWssEmploymentChangeRequest.recertified_date != DateTime.MinValue)
                        lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.recertified_date = icdoWssEmploymentChangeRequest.recertified_date;
                        if ((icdoWssEmploymentChangeRequest.change_type_value == busConstant.EmploymentChangeRequestChangeTypeLOA ||
                            icdoWssEmploymentChangeRequest.change_type_value == busConstant.EmploymentChangeRequestChangeTypeLOAM ||
                        icdoWssEmploymentChangeRequest.change_type_value == busConstant.EmploymentStatusFMLA) //pir 8127
                            && icdoWssEmploymentChangeRequest.date_of_return != DateTime.MinValue)
                        {
                            //PIR 22915 - Employment status value will be updated from request if not null
                            if (!string.IsNullOrEmpty(icdoWssEmploymentChangeRequest.employment_status_value))
                                lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.status_value = icdoWssEmploymentChangeRequest.employment_status_value;
                            else
                            {
                                int aintPersonEmploymentId = ibusPerson.ibusESSPersonEmployment.ibusESSLatestPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_id;
                                int aintPersonEmploymentDtlId = ibusPerson.ibusESSPersonEmployment.ibusESSLatestPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id;
                                DataTable ldtbPersonDetails = Select("cdoWssPersonEmploymentDetail.LoadPreviousStatusValue", new object[2] { aintPersonEmploymentId, aintPersonEmploymentDtlId });
                                string astrEmploymentStatusValue = Convert.ToString(ldtbPersonDetails.Rows[0]["STATUS_VALUE"]);
                                //Assign Previous Status value it should be CONT/NCONT.
                                lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.status_value = astrEmploymentStatusValue;
                            }
                        }
                        else if (icdoWssEmploymentChangeRequest.change_type_value == busConstant.EmploymentChangeRequestChangeTypeLOA
                            && icdoWssEmploymentChangeRequest.loa_start_date != DateTime.MinValue)
                        {
                            lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.status_value = busConstant.EmploymentStatusLOA;
                        }
                        //pir 8127
                        else if (icdoWssEmploymentChangeRequest.change_type_value == busConstant.EmploymentChangeRequestChangeTypeLOAM
                            && icdoWssEmploymentChangeRequest.loa_start_date != DateTime.MinValue)
                        {
                            lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.status_value = busConstant.EmploymentStatusLOAM;
                        }
                        //PIR 22835
                        else if (icdoWssEmploymentChangeRequest.change_type_value == busConstant.EmploymentStatusFMLA
                            && icdoWssEmploymentChangeRequest.loa_start_date != DateTime.MinValue)
                        {
                            lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.status_value = busConstant.EmploymentStatusFMLA;
                        }
                        lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.person_id = ibusPerson.ibusESSPersonEmployment.icdoPersonEmployment.person_id;
                        lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.Insert();
                        foreach (busPersonAccountEmploymentDetail lobjPersonAccEmpDetail in lclbPAEmpDetail)
                        {
                            if (lobjPersonAccEmpDetail.ibusPlan == null)
                                lobjPersonAccEmpDetail.LoadPlan();

                            busPersonAccountEmploymentDetail lbusPersonAccountEmploymentDetail = new busPersonAccountEmploymentDetail
                            {
                                icdoPersonAccountEmploymentDetail = new cdoPersonAccountEmploymentDetail()
                            };
                            lbusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.person_employment_dtl_id = lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id;
                            //prod pir 5136
                            //PIR 21266 - If the status is Non Contributing then we should not link Retirement but we do need to link the Def Comp (Plan ID 8 or 19)
                            if (!(lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusNonContributing &&
                                lobjPersonAccEmpDetail.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeRetirement))
                            {
                                lbusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.person_account_id = lobjPersonAccEmpDetail.icdoPersonAccountEmploymentDetail.person_account_id;
                                lbusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.election_value = lobjPersonAccEmpDetail.icdoPersonAccountEmploymentDetail.election_value;
                            }
                            lbusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.plan_id = lobjPersonAccEmpDetail.icdoPersonAccountEmploymentDetail.plan_id;
                            lbusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.Insert();

                        }
                    }
                }
                else// PIR 8560
                {
                    lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.recertified_date = icdoWssEmploymentChangeRequest.recertified_date;
                    ibusPerson.ibusESSPersonEmployment.ibusESSLatestPersonEmploymentDetail.icdoPersonEmploymentDetail.Update();
                }
            }
            if (icdoWssEmploymentChangeRequest.date_of_last_regular_paycheck != DateTime.MinValue)
            {
                icdoWssEmploymentChangeRequest.employment_end_date = Convert.ToDateTime(icdoWssEmploymentChangeRequest.istrEmploymentEndDateDefault);
                if (icdoWssEmploymentChangeRequest.employment_end_date == DateTime.MinValue)
                {
                    lobjError = AddError(4140, "");
                    larrReturn.Add(lobjError);
                    return larrReturn;
                }
                else
                {
                    if (ibusPersonEmployment != null && icdoWssEmploymentChangeRequest.employment_end_date < ibusPersonEmployment.icdoPersonEmployment.start_date)
                    {
                        lobjError = AddError(8524, "");
                        larrReturn.Add(lobjError);
                        return larrReturn;
                    }

                }
                ibusPerson.ibusESSPersonEmployment.icdoPersonEmployment.end_date = icdoWssEmploymentChangeRequest.employment_end_date;
                //PIR-11030 16 October
                if (ibusPerson.ibusESSPersonEmployment.IsContributionExistsAfterEmploymentEndDate())
                {
                    lobjError = AddError(2045, "");
                    larrReturn.Add(lobjError);
                    return larrReturn;
                }                

                ibusPerson.ibusESSPersonEmployment.UpdateTerminateEmployment();

                if (ibusPerson.ibusESSPersonEmployment.iblnIsHireDateSameMonthAsTerminationDate)
                    istrMessageToShowAfterPostClick = busGlobalFunctions.GetMessageTextByMessageID(10505, iobjPassInfo);
                
                ibusPerson.ibusESSPersonEmployment.icdoPersonEmployment.Update();
            }

            if(ibusPerson.ibusESSPersonEmployment.iblnIsHireDateSameMonthAsTerminationDate)
                icdoWssEmploymentChangeRequest.status_value = busConstant.EmploymentChangeRequestStatusReview;
            else
                icdoWssEmploymentChangeRequest.status_value = busConstant.EmploymentChangeRequestStatusProcessed;
            icdoWssEmploymentChangeRequest.end_dated_employment_dtl_id = ibusPerson.ibusESSPersonEmployment.ibusESSLatestPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id;
            icdoWssEmploymentChangeRequest.new_employment_dtl_id = lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id;
            icdoWssEmploymentChangeRequest.posted_date = DateTime.Now;
            icdoWssEmploymentChangeRequest.posted_in_perslink_by = iobjPassInfo.istrUserID;
            icdoWssEmploymentChangeRequest.Update();


            if ((icdoWssEmploymentChangeRequest.person_id > 0) && !string.IsNullOrEmpty(icdoWssEmploymentChangeRequest.peoplesoft_id) && (ibusPerson.icdoPerson.peoplesoft_id != icdoWssEmploymentChangeRequest.peoplesoft_id))
            {
                ibusPerson.icdoPerson.peoplesoft_id = icdoWssEmploymentChangeRequest.peoplesoft_id;
                ibusPerson.icdoPerson.Update();
            }
            //PIR 23949 - When Employment is ended (from screen or PS) and there is a first activity for WFL Process IDs 236,237,238,239,256 
            //(Activity IDs 27,32,28,31,74) in a Suspended status it should be updated to RESU
            //if (icdoWssEmploymentChangeRequest.change_type_value == busConstant.EmploymentChangeRequestChangeTypeTEEM)
            //{
                if (icdoWssEmploymentChangeRequest.person_id > 0)
                {
                    busWorkflowHelper.UpdateSuspendedInstancestoResumed(icdoWssEmploymentChangeRequest.person_id);
                }
            //}

            //PROD PIR 4971
            //--start--//
            if (ibusPerson.ibusESSPersonEmployment.ibusOrganization == null)
                ibusPerson.ibusESSPersonEmployment.LoadOrganization();
            int lintContactID = 0;
            if (icdoWssEmploymentChangeRequest.contact_id > 0)
                lintContactID = icdoWssEmploymentChangeRequest.contact_id;
            else
            {
                if (ibusPerson.ibusESSPersonEmployment.ibusOrganization.ibusESSPrimaryOrgContact == null)
                    ibusPerson.ibusESSPersonEmployment.ibusOrganization.LoadESSPrimaryAuthorizedContact();
                lintContactID = ibusPerson.ibusESSPersonEmployment.ibusOrganization.ibusESSPrimaryOrgContact.icdoOrgContact.contact_id;
            }
            if (ibusPerson.ibusESSPersonEmployment.ibusOrganization.icdoOrganization.org_id > 0 && lintContactID > 0)
            {
                string lstrPrioityValue = string.Empty;
                busWSSHelper.PublishESSMessage(0, 0, string.Format(busGlobalFunctions.GetDBMessageTextByDBMessageID(4, iobjPassInfo, ref lstrPrioityValue), ibusPerson.icdoPerson.FullName, ibusPerson.icdoPerson.person_id),
                   lstrPrioityValue, aintOrgID: ibusPerson.ibusESSPersonEmployment.ibusOrganization.icdoOrganization.org_id,
                    aintContactID: lintContactID);
            }
            //--end--//
            this.LoadCurrentlyEnrolledPlans();
            this.EvaluateInitialLoadRules();
            larrReturn.Add(this);
            return larrReturn;

        }
        public ArrayList Reject_click()
        {
            ArrayList larrReturn = new ArrayList();
            utlError lobjError = null;

            if (string.IsNullOrEmpty(icdoWssEmploymentChangeRequest.rejection_reason))
            {

                lobjError = AddError(8521, String.Empty);
                larrReturn.Add(lobjError);
                return larrReturn;
            }

            string lstrPrioityValue = string.Empty;
            if (icdoWssEmploymentChangeRequest.contact_id > 0)
            {
                busWSSHelper.PublishESSMessage(0, 0, string.Format(busGlobalFunctions.GetDBMessageTextByDBMessageID(3, iobjPassInfo, ref lstrPrioityValue), ibusPerson.icdoPerson.FullName, "@" + icdoWssEmploymentChangeRequest.employment_change_request_id),
                    lstrPrioityValue, 0, aintOrgID: icdoWssEmploymentChangeRequest.org_id,
                    aintContactID: icdoWssEmploymentChangeRequest.contact_id);
            }
            else
            {
                //PIR 24101
                busWSSHelper.PublishESSMessage(0, 0, string.Format(busGlobalFunctions.GetDBMessageTextByDBMessageID(3, iobjPassInfo, ref lstrPrioityValue), ibusPerson.icdoPerson.FullName, "@" + icdoWssEmploymentChangeRequest.employment_change_request_id),
                lstrPrioityValue, 0, aintOrgID: icdoWssEmploymentChangeRequest.org_id,
                aclbOrgContacts: busGlobalFunctions.LoadPrimaryAutOrAAByOrgId(icdoWssEmploymentChangeRequest.org_id));
            }

            icdoWssEmploymentChangeRequest.status_value = busConstant.EmploymentChangeRequestStatusRejected;
            icdoWssEmploymentChangeRequest.Update();
            this.EvaluateInitialLoadRules();
            larrReturn.Add(this);
            return larrReturn;
        }

        /// <summary>
        /// prod pir 4846 : to ignore requests
        /// </summary>
        /// <returns></returns>
        public ArrayList Ignore_click()
        {
            ArrayList larrReturn = new ArrayList();
            utlError lobjError = new utlError();
            icdoWssEmploymentChangeRequest.status_value = busConstant.EmploymentChangeRequestStatusIgnored;
            icdoWssEmploymentChangeRequest.Update();
            this.EvaluateInitialLoadRules();
            larrReturn.Add(this);
            return larrReturn;
        }

        //Code to validate that entered date is greater than the latest employment detail's start date - pir 6805
        public bool IsDateGreaterThanPersonEmploymentStartDate()
        {
            if (ibusPersonEmploymentDetail.IsNull()) LoadEmploymentDetail();
            DateTime ldtstartDate = ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date;
            if (ldtstartDate != DateTime.MinValue)
            {
                if ((!icdoWssEmploymentChangeRequest.recertified_date.Equals(DateTime.MinValue)) &&
                    (ldtstartDate != icdoWssEmploymentChangeRequest.recertified_date))
                {
                    return (ldtstartDate < icdoWssEmploymentChangeRequest.recertified_date);
                }
                if ((!icdoWssEmploymentChangeRequest.job_class_change_effective_date.Equals(DateTime.MinValue)) &&
                    (ldtstartDate != icdoWssEmploymentChangeRequest.job_class_change_effective_date))
                {
                    return (ldtstartDate < icdoWssEmploymentChangeRequest.job_class_change_effective_date);
                }
                if ((!icdoWssEmploymentChangeRequest.employment_type_change_effective_date.Equals(DateTime.MinValue)) &&
                     (ldtstartDate != icdoWssEmploymentChangeRequest.employment_type_change_effective_date))
                {
                    return (ldtstartDate < icdoWssEmploymentChangeRequest.employment_type_change_effective_date);
                }
                //if ((!icdoWssEmploymentChangeRequest.last_date_of_service.Equals(DateTime.MinValue)) &&
                //    (ldtstartDate != icdoWssEmploymentChangeRequest.last_date_of_service))
                //{
                //    return (ldtstartDate <= icdoWssEmploymentChangeRequest.last_date_of_service);
                //}
            }
            return true;
        }

        /// <summary>
        /// pir 5465 : Checks if loa date is valid
        /// </summary>
        /// <returns></returns>
        public bool IsLoaStartDateValid()
        {
            return (busGlobalFunctions.CheckEmploymentStartDate(icdoWssEmploymentChangeRequest.loa_start_date));

        }

        /// <summary>
        /// pir 5465 : Checks if loa date of return is valid
        /// </summary>
        /// <returns></returns>
        public bool IsLoaDateOfReturnValid()
        {
            return busGlobalFunctions.CheckEmploymentStartDate(icdoWssEmploymentChangeRequest.date_of_return);
        }

        /// <summary>
        /// pir 5465 : Checks if Recertification Date is valid
        /// </summary>
        /// <returns></returns>
        public bool IsRecertificationDateValid()
        {
            //return busGlobalFunctions.CheckEmploymentStartDate(icdoWssEmploymentChangeRequest.recertified_date); 
            DateTime minDate = Convert.ToDateTime("01/01/1980");
            DateTime maxDate = Convert.ToDateTime("12/31/2099");
            return (icdoWssEmploymentChangeRequest.recertified_date != DateTime.MinValue && (icdoWssEmploymentChangeRequest.recertified_date >= minDate && icdoWssEmploymentChangeRequest.recertified_date <= maxDate));
        }
        public bool IsRecertificationDateGreaterThanOneYearOfStartDate()
        {
            DateTime dtLOAStartDate = new DateTime();
            if (!String.IsNullOrEmpty(icdoWssEmploymentChangeRequest.istrEmpDetailStartDate))
                dtLOAStartDate = Convert.ToDateTime(icdoWssEmploymentChangeRequest.istrEmpDetailStartDate).AddYears(1);
            if (icdoWssEmploymentChangeRequest.recertified_date < dtLOAStartDate)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks difference between LOA Return Date &  LOA start Date is greater than  one year.
        /// </summary>
        /// <returns></returns>
        public bool IsLOAReturnDateGreaterThanOneYear()
        {
            DateTime dtLOAStartDate = new DateTime();
            if (!String.IsNullOrEmpty(icdoWssEmploymentChangeRequest.istrEmpDetailStartDate))
                dtLOAStartDate = Convert.ToDateTime(icdoWssEmploymentChangeRequest.istrEmpDetailStartDate).AddYears(1);
            if ((icdoWssEmploymentChangeRequest.date_of_return != DateTime.MinValue)
               && (icdoWssEmploymentChangeRequest.date_of_return > dtLOAStartDate)
               && (icdoWssEmploymentChangeRequest.recertified_date.Equals(DateTime.MinValue))
               && ibusPerson.ibusESSPersonEmployment.IsNotNull()
               && ibusPerson.ibusESSPersonEmployment.ibusESSLatestPersonEmploymentDetail.icdoPersonEmploymentDetail.recertified_date.Equals(DateTime.MinValue))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks if Recertification Date is valid.
        /// </summary>
        /// <returns></returns>
        public bool IsVisibleRecertificationDate()
        {
            if ((icdoWssEmploymentChangeRequest.recertified_date.IsNotNull()) && (icdoWssEmploymentChangeRequest.recertified_date != DateTime.MinValue))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks if Return Date is valid.
        /// </summary>
        /// <returns></returns>
        public bool IsVisibleLOAReturnDate()
        {
            if ((icdoWssEmploymentChangeRequest.date_of_return.IsNotNull()) && (icdoWssEmploymentChangeRequest.date_of_return != DateTime.MinValue))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks LOA TYPE is selected or not.
        /// </summary>
        /// <returns></returns>
        public bool IsLOATypeSelected()
        {
            if (string.IsNullOrEmpty(icdoWssEmploymentChangeRequest.change_type_value) && string.IsNullOrEmpty(icdoWssEmploymentChangeRequest.istrEmpDetailStatusDescription))
                return false;
            else
                return true;
        }

        public bool IsLOALDateValidated()
        {

            if (IsLOATypeSelected())
            {
                if (icdoWssEmploymentChangeRequest.change_type_value == "LOAL")
                {
                    if (icdoWssEmploymentChangeRequest.loa_start_date > DateTime.Now.AddDays(7))
                    {
                        return false;
                    }
                }

            }

            return true;
        }

        public bool IsLOAMDateValidated()
        {

            if (IsLOATypeSelected())
            {
                if (icdoWssEmploymentChangeRequest.change_type_value == "LOAM")
                {
                    if (icdoWssEmploymentChangeRequest.loa_start_date > DateTime.Now.AddMonths(1))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        /// <summary>
        /// pir 5465 : Checks if Job Class Effective Date is valid
        /// </summary>
        /// <returns></returns>
        public bool IsJobClassEffectiveDateValid()
        {
            return (busGlobalFunctions.CheckEmploymentStartDate(icdoWssEmploymentChangeRequest.job_class_change_effective_date));
        }

        /// <summary>
        /// pir 5465 : Checks if Employment Type Change Effective Date is valid
        /// </summary>
        /// <returns></returns>
        public bool IsEmploymentTypeChangeEffectiveDateValid()
        {
            return (busGlobalFunctions.CheckEmploymentStartDate(icdoWssEmploymentChangeRequest.employment_type_change_effective_date));
        }

        /// <summary>
        /// pir 5465 : Checks if Last Date Of Service is valid
        /// </summary>
        /// <returns></returns>
        public bool IsLastDateOfServiceValid()
        {
            return (busGlobalFunctions.CheckEmploymentEndDate(icdoWssEmploymentChangeRequest.last_date_of_service));
        }

        /// <summary>
        /// pir 7952 : Checks Created_by contains Underscore character i.e.'_' or Not
        /// </summary>
        /// <returns>True or False</returns>
        public bool IsCreatedByContainsUnderScore()
        {
            if (!icdoWssEmploymentChangeRequest.created_by.Contains('_'))
            {
                return false;
            }
            return true;
        }
        //PIR-11030 16 October
        public bool IsOfficialListNullOREmpty()
        {
            if (icdoWssEmploymentChangeRequest.official_list_value == null || icdoWssEmploymentChangeRequest.official_list_value.Trim() == string.Empty)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// pir 7952 : Set details of Panel Requested By
        /// </summary>
        /// <returns></returns>
        public void SetRequestedByDetails()
        {
            if (icdoWssEmploymentChangeRequest.contact_id > 0) {
                iintContact_id = icdoWssEmploymentChangeRequest.contact_id;
                ibusContact.FindContact(iintContact_id);
                icdoWssEmploymentChangeRequest.Requested_By_Contact_id = ibusContact.icdoContact.contact_id.ToString();
                icdoWssEmploymentChangeRequest.Requested_By_Contact_Name = ibusContact.icdoContact.ContactName;
                icdoWssEmploymentChangeRequest.Requested_By_Contact_Phone_No = ibusContact.icdoContact.phone_no;
                icdoWssEmploymentChangeRequest.istrRequestByContactEmail = ibusContact.icdoContact.email_address;//PIR 11653
            }
            //if (IsCreatedByContainsUnderScore())
            //{
            //    iintContact_id = Convert.ToInt32(icdoWssEmploymentChangeRequest.created_by.Substring(icdoWssEmploymentChangeRequest.created_by.IndexOf('_') + 1));
            //    ibusContact.FindContact(iintContact_id);
            //    icdoWssEmploymentChangeRequest.Requested_By_Contact_id = ibusContact.icdoContact.contact_id.ToString();
            //    icdoWssEmploymentChangeRequest.Requested_By_Contact_Name = ibusContact.icdoContact.ContactName;
            //    icdoWssEmploymentChangeRequest.Requested_By_Contact_Phone_No = ibusContact.icdoContact.phone_no;
            //}
            //else
            //{
            //    busUser lbusUser = new busUser() { icdoUser = new cdoUser() };
            //    lbusUser.icdoUser.user_id = this.icdoWssEmploymentChangeRequest.created_by;
            //    if (lbusUser.FindUserByUserName(lbusUser.icdoUser.user_id))
            //    {
            //        icdoWssEmploymentChangeRequest.Requested_By_Contact_id = lbusUser.icdoUser.user_id;
            //        icdoWssEmploymentChangeRequest.Requested_By_Contact_Name = lbusUser.icdoUser.User_Name;
            //    }
            //}
        }
        //PIR 8127
        public string istrLOAHealthInsuranceContinue
        {
            get
            {
                return busGlobalFunctions.GetDescriptionByCodeValue(3510, icdoWssEmploymentChangeRequest.health_insurance_continued, iobjPassInfo);
            }
        }
        public string istrLOALifeInsuranceContinue
        {
            get
            {
                return busGlobalFunctions.GetDescriptionByCodeValue(3510, icdoWssEmploymentChangeRequest.life_insurance_continued, iobjPassInfo);
            }
        }
        public string istrLOADentalInsuranceContinue
        {
            get
            {
                return busGlobalFunctions.GetDescriptionByCodeValue(3510, icdoWssEmploymentChangeRequest.dental_insurance_continued, iobjPassInfo);
            }
        }
        public string istrLOAVisionInsuranceContinue
        {
            get
            {
                return busGlobalFunctions.GetDescriptionByCodeValue(3510, icdoWssEmploymentChangeRequest.vision_insurance_continued, iobjPassInfo);
            }
        }
        public string istrLOAEAPInsuranceContinue
        {
            get
            {
                return busGlobalFunctions.GetDescriptionByCodeValue(3510, icdoWssEmploymentChangeRequest.eap_insurance_continued, iobjPassInfo);
            }
        }
        public string istrLOAFlexInsuranceContinue
        {
            get
            {
                return busGlobalFunctions.GetDescriptionByCodeValue(3510, icdoWssEmploymentChangeRequest.flex_comp_continued, iobjPassInfo);
            }
        }
        public string istrLOALTCInsuranceContinue
        {
            get
            {
                return busGlobalFunctions.GetDescriptionByCodeValue(3510, icdoWssEmploymentChangeRequest.ltc_continued, iobjPassInfo);
            }
        }
        public bool istrEmpCategSchoolDistrictHealthEnrolled { get; set; }

        public string istrIsTFFRorTIAA { get; set; }
        public string istrRejectedByName { get; set; } //PIR 23900
        public DateTime idtmRejectedDate { get; set; } //PIR 24183
        public Collection<busPersonAccountEmploymentDetail> icolEnrolledPlansForGrid { get; private set; }

        public bool IsMemberEnrolledInInsurancePlans()
        {
            DataTable ldtbEnrolledInsurancePlanList = Select("cdoPersonAccount.LoadCurrentlyEnrolledPlans", new object[3] { icdoWssEmploymentChangeRequest.person_employment_detail_id, busConstant.PlanBenefitTypeInsurance, busConstant.PlanParticipationStatusInsuranceEnrolled });
            if (ibusOrganization.IsNull())
                LoadOrganization();
            if (ldtbEnrolledInsurancePlanList.AsEnumerable().Where(i => i.Field<int>("PLAN_ID") == 12).Count() > 0 && ibusOrganization.icdoOrganization.emp_category_value == busConstant.EmployerCategorySchoolDistrict)
                istrEmpCategSchoolDistrictHealthEnrolled = true;
            return ldtbEnrolledInsurancePlanList.Rows.Count > 0 ? true : false;
        }
        public bool IsEmployeeEnrolledInDBorDCPlan()
        {
            DataTable ldtbList = Select("cdoPersonAccount.LoadCurrentlyEnrolledPlans", new object[3] { icdoWssEmploymentChangeRequest.person_employment_detail_id, busConstant.PlanBenefitTypeRetirement, busConstant.PlanParticipationStatusRetirementEnrolled });
            return ldtbList.Rows.Count > 0 ? true : false;
        }
        public bool IsEmployerTypeSchoolDistrict()
        {
            if (ibusOrganization.IsNull()) LoadOrganization();
            return ibusOrganization.icdoOrganization.emp_category_value == busConstant.EmployerCategorySchoolDistrict ? true : false;

        }
        public bool IsEmployerTypePeopleSoftOrgGroupValueHigherEd()
        {
            if (ibusOrganization.IsNull()) LoadOrganization();
            return ibusOrganization.icdoOrganization.peoplesoft_org_group_value == busConstant.PeopleSoftOrgGroupValueHigherEd ? true : false;
        }

        public bool IsEmployeeEnrolledInFlexPlan()
        {
            if (ibusPerson.IsNull())
                LoadPerson();
            return ibusPerson.IsMemberEnrolledInPlan(busConstant.PlanIdFlex);
        }
        public bool IsEmployeeEnrolledInEAP()
        {
            if (ibusPerson.IsNull())
                LoadPerson();
            return ibusPerson.IsMemberEnrolledInPlan(busConstant.PlanIdEAP);
        }
        public bool IsNullLOAHealthInsuranceContinue()
        {
            if (string.IsNullOrEmpty(icdoWssEmploymentChangeRequest.health_insurance_continued) && string.IsNullOrEmpty(icdoWssEmploymentChangeRequest.istrEmpDetailStartDate))
                return true;
            return false;
        }
        public bool IsNullLOALifeInsuranceContinue()
        {
            if (string.IsNullOrEmpty(icdoWssEmploymentChangeRequest.life_insurance_continued) && string.IsNullOrEmpty(icdoWssEmploymentChangeRequest.istrEmpDetailStartDate))
                return true;
            return false;
        }
        public bool IsNullLOADentalInsuranceContinue()
        {
            if (string.IsNullOrEmpty(icdoWssEmploymentChangeRequest.dental_insurance_continued) && string.IsNullOrEmpty(icdoWssEmploymentChangeRequest.istrEmpDetailStartDate))
                return true;
            return false;
        }
        public bool IsNullLOAVisionInsuranceContinue()
        {
            if (string.IsNullOrEmpty(icdoWssEmploymentChangeRequest.vision_insurance_continued) && string.IsNullOrEmpty(icdoWssEmploymentChangeRequest.istrEmpDetailStartDate))
                return true;
            return false;
        }
        public bool IsNullLOAEAPInsuranceContinue()
        {
            if (string.IsNullOrEmpty(icdoWssEmploymentChangeRequest.eap_insurance_continued) && string.IsNullOrEmpty(icdoWssEmploymentChangeRequest.istrEmpDetailStartDate))
                return true;
            return false;
        }
        public bool IsNullFlexInsuranceContinue()
        {
            if (string.IsNullOrEmpty(icdoWssEmploymentChangeRequest.flex_comp_continued) && string.IsNullOrEmpty(icdoWssEmploymentChangeRequest.istrEmpDetailStartDate))
                return true;
            return false;
        }
        public bool IsNullLOALTCInsuranceContinue()
        {
            if (string.IsNullOrEmpty(icdoWssEmploymentChangeRequest.ltc_continued) && string.IsNullOrEmpty(icdoWssEmploymentChangeRequest.istrEmpDetailStartDate))
                return true;
            return false;
        }
        public bool IsLastMonthCoverageNull()
        {
            if (IsMemberEnrolledInInsurancePlans() && (icdoWssEmploymentChangeRequest.health_insurance_continued == busConstant.Flag_No_CAPS || icdoWssEmploymentChangeRequest.dental_insurance_continued == busConstant.Flag_No_CAPS
                || icdoWssEmploymentChangeRequest.life_insurance_continued == busConstant.Flag_No_CAPS || icdoWssEmploymentChangeRequest.vision_insurance_continued == busConstant.Flag_No_CAPS
                || icdoWssEmploymentChangeRequest.eap_insurance_continued == busConstant.Flag_No_CAPS || icdoWssEmploymentChangeRequest.flex_comp_continued == busConstant.Flag_No_CAPS
                || icdoWssEmploymentChangeRequest.ltc_continued == busConstant.Flag_No_CAPS))
                return true;
            return false;
        }
        //PIR 14474 - Land the user up on summary page when wizard opened in update mode.
        public override void ProcessWizardData(utlWizardNavigationEventArgs we, string astrWizardName, string astrWizardStepName)
        {
            switch (icdoWssEmploymentChangeRequest.change_type_value)
            {
                case busConstant.EmploymentChangeRequestChangeTypeTEEM:
                    we.istrNextStepID = (icdoWssEmploymentChangeRequest.employment_change_request_id > 0) ? "wzsSummary" : we.istrNextStepID;
                    break;
                case busConstant.EmploymentChangeRequestChangeTypeLOAR:
                case busConstant.EmploymentChangeRequestChangeTypeLOAM:
                case busConstant.EmploymentChangeRequestChangeTypeLOA:
                case busConstant.EmploymentStatusFMLA: // PIR 22835
                case busConstant.EmploymentChangeRequestChangeTypeClassification:
                    we.istrNextStepID = (icdoWssEmploymentChangeRequest.employment_change_request_id > 0) ? "wzsAuthorization" : we.istrNextStepID;
                    break;
                default:
                    break;
            }
            base.ProcessWizardData(we, astrWizardName, astrWizardStepName);
        }
        public bool iblnEmployeedidnotstart()
        {
            if(icdoWssEmploymentChangeRequest.employee_never_started == "Y")
            {
                return true;
            }
            return false;
        }
        public void LoadRejectionDetails()
        {
            if (icdoWssEmploymentChangeRequest.status_value == busConstant.EmploymentChangeRequestStatusRejected)
            {
                DataTable ldtbUserInfo = iobjPassInfo.isrvDBCache.GetUserInfo(icdoWssEmploymentChangeRequest.modified_by);
                if (ldtbUserInfo?.Rows?.Count > 0)
                {
                    istrRejectedByName = ldtbUserInfo.Rows[0]["FIRST_NAME"] + " " + ldtbUserInfo.Rows[0]["LAST_NAME"];
                    idtmRejectedDate = icdoWssEmploymentChangeRequest.modified_date;
                }
            }
        }
        public void GetDefaultDatesForTermination()
        {
            //suspend effective date
            //Def Comp and Flex should have 1st day of Month following Last Date of Service
            ibusPerson.ibusESSPersonEmployment.idtelastDateOfService = icdoWssEmploymentChangeRequest.last_date_of_service;
            ibusPerson.ibusESSPersonEmployment.idtmSuspendEffectiveDateDCandFlex = icdoWssEmploymentChangeRequest.last_date_of_service.GetFirstDayofNextMonth();
            //GHDV, Life follow EAP Suspend logic
            ibusPerson.ibusESSPersonEmployment.idtmSuspendEffectiveDateDefaultInsPlans = !string.IsNullOrEmpty(icdoWssEmploymentChangeRequest.istrEmploymentEndDateDefault) ? Convert.ToDateTime(icdoWssEmploymentChangeRequest.istrEmploymentEndDateDefault).GetFirstDayofNextMonth().AddMonths(1) : DateTime.MinValue;
            ibusPerson.ibusESSPersonEmployment.idtmDefCompProviderEndDateWhenTEEM = icdoWssEmploymentChangeRequest.last_date_of_service;

            //set up property if hire date same month as termination date
            if (((ibusPerson.ibusESSPersonEmployment.icdoPersonEmployment.start_date.Month == (Convert.ToDateTime(icdoWssEmploymentChangeRequest.istrEmploymentEndDateDefault)).Month) &&
                (ibusPerson.ibusESSPersonEmployment.icdoPersonEmployment.start_date.Year == (Convert.ToDateTime(icdoWssEmploymentChangeRequest.istrEmploymentEndDateDefault)).Year)) ||
                ((ibusPerson.ibusESSPersonEmployment.icdoPersonEmployment.start_date.Month == (Convert.ToDateTime(icdoWssEmploymentChangeRequest.last_date_of_service)).Month) &&
                (ibusPerson.ibusESSPersonEmployment.icdoPersonEmployment.start_date.Year == (Convert.ToDateTime(icdoWssEmploymentChangeRequest.last_date_of_service)).Year))) //PIR 26517
                ibusPerson.ibusESSPersonEmployment.iblnIsHireDateSameMonthAsTerminationDate = true;

            if (ibusPerson.ibusESSPersonEmployment.iblnIsHireDateSameMonthAsTerminationDate)
                ibusPerson.ibusESSPersonEmployment.idtmSuspendEffectiveDateDefaultInsPlans = !string.IsNullOrEmpty(icdoWssEmploymentChangeRequest.istrEmploymentEndDateDefault) ? Convert.ToDateTime(icdoWssEmploymentChangeRequest.istrEmploymentEndDateDefault).GetFirstDayofNextMonth() : DateTime.MinValue;
        }
        public bool CheckDateBeforeTerminationRequestPost()
        {
            if (icdoWssEmploymentChangeRequest.date_of_last_regular_paycheck.ToString("MM/yyyy") == icdoWssEmploymentChangeRequest.istrLastRetirementTransmittalOfDeduction)
                return false;
            else if (icdoWssEmploymentChangeRequest.last_date_of_service.ToString("MM/yyyy") == icdoWssEmploymentChangeRequest.istrLastRetirementTransmittalOfDeduction)
                return false;
            else
                return true;

        }
        //Load plans linked to terminated employment (by, empl dtl id)
        public void LoadCurrentlyEnrolledPlans()
        {
            icolEnrolledPlansForGrid = new Collection<busPersonAccountEmploymentDetail>();
            busPersonEmployment lbusTerminatedEmployment = new busPersonEmployment { icdoPersonEmployment = new cdoPersonEmployment() };
            DataTable ldtPersonEmployment = Select<cdoPersonEmployment>(new string[2] { "PERSON_ID", "ORG_ID" }, new object[2] { icdoWssEmploymentChangeRequest.person_id, icdoWssEmploymentChangeRequest.org_id }, null, "case when end_date is null then 0 else 1 end, start_date DESC, end_date DESC");
            if (ldtPersonEmployment.Rows.Count > 0)
            {
                lbusTerminatedEmployment.icdoPersonEmployment.LoadData(ldtPersonEmployment.Rows[0]);
            }
            lbusTerminatedEmployment.LoadLatestPersonEmploymentDetail();
            lbusTerminatedEmployment.ibusLatestEmploymentDetail.LoadEnrolledPersonAccountEmploymentDetailsWithPersonAccount();
            foreach (busPersonAccountEmploymentDetail lbusPAEmpDetail in lbusTerminatedEmployment.ibusLatestEmploymentDetail.iclbPersonAccountEmpDtl)
            {
                if (lbusPAEmpDetail.icdoPersonAccountEmploymentDetail.person_employment_dtl_id == lbusTerminatedEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id)
                {
                    if (ibusPerson == null)
                        LoadPerson();
                    ibusPerson.LoadPersonAccount();

                    if (lbusPAEmpDetail.ibusPlan.IsNull())
                        lbusPAEmpDetail.LoadPlan(); //PIR 24980
                    ibusPerson.LoadPersonAccountByPlan(lbusPAEmpDetail.ibusPlan.icdoPlan.plan_id);

                    //setting date defaults for each plan
                    if (lbusPAEmpDetail.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdDeferredCompensation || lbusPAEmpDetail.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdOther457 ||
                        lbusPAEmpDetail.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdFlex)
                        lbusPAEmpDetail.idtmSuspendEffectiveDate = icdoWssEmploymentChangeRequest.last_date_of_service.GetFirstDayofNextMonth();
                    else if (lbusPAEmpDetail.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdGroupHealth || lbusPAEmpDetail.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdDental ||
                            lbusPAEmpDetail.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdVision || lbusPAEmpDetail.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdEAP ||
                            lbusPAEmpDetail.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdGroupLife)
                    {
                        if (ibusPerson.ibusESSPersonEmployment.iblnIsHireDateSameMonthAsTerminationDate)
                            lbusPAEmpDetail.idtmSuspendEffectiveDate = icdoWssEmploymentChangeRequest.last_date_of_service.GetFirstDayofNextMonth();
                        else
                            lbusPAEmpDetail.idtmSuspendEffectiveDate = icdoWssEmploymentChangeRequest.last_date_of_service.GetFirstDayofNextMonth().AddMonths(1);
                    }
                    else if (lbusPAEmpDetail.ibusPlan.icdoPlan.benefit_type_value == busConstant.ApplicationBenefitTypeRetirement)
                        lbusPAEmpDetail.idtmSuspendEffectiveDate = Convert.ToDateTime(icdoWssEmploymentChangeRequest.istrEmploymentEndDateDefault).GetFirstDayofNextMonth();

                    //setting values for status message, shown after process is posted

                    foreach (busPersonAccount lbusPersonAccount in ibusPerson.icolPersonAccount)
                    {
                        if ((lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth ||
                            lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDental ||
                            lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdVision) &&
                            (lbusPersonAccount.ibusPersonAccountGHDV == null))
                            lbusPersonAccount.LoadPersonAccountGHDV();
                        if (lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdFlex && lbusPersonAccount.ibusPersonAccountFlex == null)
                            lbusPersonAccount.LoadPersonAccountFlex();
                        if (lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupLife && lbusPersonAccount.ibusPersonAccountLife == null)
                            lbusPersonAccount.LoadPersonAccountLife();

                        if (lbusPersonAccount.icdoPersonAccount.person_account_id == lbusPAEmpDetail.icdoPersonAccountEmploymentDetail.person_account_id &&
                            lbusPersonAccount.icdoPersonAccount.plan_id == lbusPAEmpDetail.ibusPlan.icdoPlan.plan_id)
                        {
                            if ((lbusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusDefCompSuspended ||
                                  lbusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended ||
                                  lbusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusFlexSuspended ||
                                  lbusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetimentSuspended))
                                lbusPAEmpDetail.istrSuspendManually = "Successfully processed.";
                            //PIR 26517
                            else if (lbusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusDefCompCancelled ||
                                  lbusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceCancelled ||
                                  lbusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusFlexCancelled ||
                                  lbusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirmentCancelled)
                                lbusPAEmpDetail.istrSuspendManually = "Successfully processed.";

                            else if (lbusPersonAccount.icdoPersonAccount.history_change_date >= lbusPAEmpDetail.idtmSuspendEffectiveDate ||
                                lbusTerminatedEmployment.IsOtherEmploymentOpenForPersonAccount(lbusPersonAccount) ||
                             ((lbusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusDefCompEnrolled ||
                              lbusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled ||
                              lbusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusFlexCompEnrolled ||
                              lbusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled) &&
                             ((lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth && (lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeRetiree ||
                              lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.cobra_type_value.IsNotNullOrEmpty())) ||
                             (lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDental && (lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.dental_insurance_type_value == busConstant.DentalInsuranceTypeCOBRA ||
                              lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.dental_insurance_type_value == busConstant.DentalInsuranceTypeRetiree)) ||
                             (lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdVision && (lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.vision_insurance_type_value == busConstant.VisionInsuranceTypeCOBRA ||
                              lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.vision_insurance_type_value == busConstant.VisionInsuranceTypeRetiree)) ||
                             (lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupLife && (lbusPersonAccount.ibusPersonAccountLife.icdoPersonAccountLife.life_insurance_type_value == busConstant.LifeInsuranceTypeRetireeMember)) ||
                             (lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdFlex && (lbusPersonAccount.ibusPersonAccountFlex.icdoPersonAccountFlexComp.flex_comp_type_value == busConstant.FlexCompTypeValueCOBRA)))) ||                             
                             (ibusPerson.ibusESSPersonEmployment.iblnIsHireDateSameMonthAsTerminationDate && !lbusPersonAccount.ibusPlan.IsRetirementPlan()))

                                lbusPAEmpDetail.istrSuspendManually = "Update failed. Please suspend plan manually";

                            else
                                lbusPAEmpDetail.istrSuspendManually = "Successfully processed.";

                            //boolean with which we apply check to update status from review to processed after all plans are suspended
                            if (!iblnAreAllPlansNotSuspended &&
                                (lbusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusDefCompEnrolled ||
                                  lbusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled ||
                                  lbusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusFlexCompEnrolled ||
                                  lbusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled))
                                iblnAreAllPlansNotSuspended = true;
                        }
                    }
                    //saving status value, used in visibility rule
                    lbusPAEmpDetail.istrTerminationStatusValue = icdoWssEmploymentChangeRequest.status_value;
                    if (ibusPerson.ibusESSPersonEmployment.icdoPersonEmployment.end_date != DateTime.MinValue)
                        lbusPAEmpDetail.iblnIsEmploymentEnded = true;
                    icolEnrolledPlansForGrid.Add(lbusPAEmpDetail);
                }
            }
            if ((icolEnrolledPlansForGrid?.Count > 0))
            {
                icolEnrolledPlansForGrid.ForEach(detail => detail.LoadPlan());
                icolEnrolledPlansForGrid = busGlobalFunctions.Sort<busPersonAccountEmploymentDetail>("ibusPlan.icdoPlan.sort_order", icolEnrolledPlansForGrid);
            }

        }
        public bool iblnAreAllPlansNotSuspended { get; set; }
        public string istrMessageToShowAfterPostClick { get; set; }
    }
}
