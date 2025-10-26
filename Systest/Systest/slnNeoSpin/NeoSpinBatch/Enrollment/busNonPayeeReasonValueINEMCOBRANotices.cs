#region Using directives
using System;
using System.Data;
using System.Collections.Generic;
using System.Diagnostics;
using NeoSpin.CustomDataObjects;
using NeoSpin.BusinessObjects;
using System.Collections.ObjectModel;
using Sagitec.DataObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.CustomDataObjects;
using Sagitec.BusinessObjects;
using System.Collections;
using Sagitec.CorBuilder;
using System.Linq;
#endregion


namespace NeoSpinBatch.Enrollment
{
    class busNonPayeeReasonValueINEMCOBRANotices : busNeoSpinBatch
    {
        public void GenerateCorrespondenceForNonPayeeReasonValueINEMCOBRANotices()
        {
            istrProcessName = "Employment Status Change - COBRA Notice";
            idlgUpdateProcessLog("Creating Correspondence for Employment Status Change", "INFO", istrProcessName);
            int lintPreviousPersonID = 0;
            //bool lblnCorGeneratedForFlex = false;
            //The Query will fetch the record where Reason value is INEM.
            DataTable ldtResult = DBFunction.DBSelect("entPersonAccount.ChangeReasonINEMCOBRALetter", new object[] { },
                                            iobjPassInfo.iconFramework,
                                            iobjPassInfo.itrnFramework);
            foreach (DataRow ldtr in ldtResult.Rows)
            {
                busPersonAccountGhdv lbusPersonAccountGhdv = new busPersonAccountGhdv() { icdoPersonAccountGhdv = new cdoPersonAccountGhdv(),icdoPersonAccount = new cdoPersonAccount(), ibusPersonAccount = new busPersonAccount() { icdoPersonAccount = new cdoPersonAccount() } };
                busPersonAccountFlexComp lbusPersonAccountFlexComp = new busPersonAccountFlexComp() { icdoPersonAccountFlexComp = new cdoPersonAccountFlexComp(), ibusPersonAccount = new busPersonAccount() { icdoPersonAccount = new cdoPersonAccount() } };

                lbusPersonAccountGhdv.icdoPersonAccountGhdv.LoadData(ldtr);
                //lbusPersonAccountGhdv.icdoPersonAccount.LoadData(ldtr);
                lbusPersonAccountGhdv.FindByPrimaryKey(lbusPersonAccountGhdv.icdoPersonAccountGhdv.person_account_ghdv_id);
                lbusPersonAccountGhdv.ibusPersonAccount.FindByPrimaryKey(lbusPersonAccountGhdv.icdoPersonAccountGhdv.person_account_id);
                lbusPersonAccountGhdv.icdoPersonAccount = lbusPersonAccountGhdv.ibusPersonAccount.icdoPersonAccount;

                //lbusPersonAccountGhdv.LoadObject(nameof(busPersonAccountGhdv), "person_account_ghdv_id");
                //lbusPersonAccountGhdv.LoadObject(nameof(lbusPersonAccountGhdv.ibusPersonAccount), "person_account_id");
                //PIR 25994 - load objects for creating PER-0104 for Flex comp plan
                lbusPersonAccountFlexComp.icdoPersonAccountFlexComp.LoadData(ldtr);
                lbusPersonAccountFlexComp.ibusPersonAccount.icdoPersonAccount.LoadData(ldtr);
                lbusPersonAccountFlexComp.FindByPrimaryKey(lbusPersonAccountFlexComp.icdoPersonAccountFlexComp.person_account_id);
                lbusPersonAccountFlexComp.ibusPersonAccount.FindByPrimaryKey(lbusPersonAccountFlexComp.icdoPersonAccountFlexComp.person_account_id);
                
                //PIR 25994 - check GHDV Plans from GHDV objects
                if (lbusPersonAccountGhdv.IsNotNull() &&  lbusPersonAccountGhdv.ibusPersonAccount.IsNotNull() && lbusPersonAccountGhdv.icdoPersonAccountGhdv.person_account_ghdv_id > 0 && 
                    ((lbusPersonAccountGhdv.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth) ||
                       (lbusPersonAccountGhdv.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdVision) ||
                       (lbusPersonAccountGhdv.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDental)))
                {
                    //if (lintPreviousPersonID != lbusPersonAccountGhdv.ibusPersonAccount.icdoPersonAccount.person_id)
                    //    lblnCorGeneratedForFlex = false;

                    lbusPersonAccountGhdv.icdoPersonAccount.person_employment_dtl_id = lbusPersonAccountGhdv.ibusPersonAccount.GetEmploymentDetailID();
                    if (lbusPersonAccountGhdv.icdoPersonAccount.person_employment_dtl_id == 0)
                    {
                        lbusPersonAccountGhdv.ibusPersonAccount.idtPlanEffectiveDate = lbusPersonAccountGhdv.ibusPersonAccount.icdoPersonAccount.end_date_no_null;
                        lbusPersonAccountGhdv.icdoPersonAccount.person_employment_dtl_id = lbusPersonAccountGhdv.ibusPersonAccount.GetEmploymentDetailID();
                    }
                    if (lbusPersonAccountGhdv.icdoPersonAccount.person_employment_dtl_id > 0)
                    {
                        lbusPersonAccountGhdv.LoadPersonEmploymentDetail();
                    }

                    if (((lbusPersonAccountGhdv.icdoPersonAccount.person_employment_dtl_id > 0) && lbusPersonAccountGhdv.ibusPersonEmploymentDetail.IsNotNull() &&
                        (lbusPersonAccountGhdv.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.cobra_letter_status_value.IsNull() ||
                        lbusPersonAccountGhdv.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.cobra_letter_status_value == busConstant.COBRALetterStatusNotSent)))
                    {
                        lbusPersonAccountGhdv.ibusPersonEmploymentDetail.LoadPersonEmployment();
                        //lbusPersonAccountGhdv.ibusPersonAccount = new busPersonAccount() { icdoPersonAccount = new cdoPersonAccount() };
                        //lbusPersonAccountGhdv.ibusPersonAccount.icdoPersonAccount = lbusPersonAccountGhdv.icdoPersonAccount;
                        lbusPersonAccountGhdv.LoadPlan();
                        lbusPersonAccountGhdv.LoadPerson();

                        if ((lbusPersonAccountGhdv.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdGroupHealth) ||
                           (lbusPersonAccountGhdv.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdVision) ||
                           (lbusPersonAccountGhdv.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdDental))
                        {
                            if (lbusPersonAccountGhdv.icdoPersonAccount.person_account_id > 0)
                            {
                                // ** BR - 242 ** The Batch correspondence letter will generate in the following sequence.
                                lintPreviousPersonID = lbusPersonAccountGhdv.ibusPerson.icdoPerson.person_id;
                                CreateAddressEnvelope(lbusPersonAccountGhdv.ibusPerson);
                                CreateCOBRAInsuranceNotice(lbusPersonAccountGhdv.ibusPersonEmploymentDetail.ibusPersonEmployment, lbusPersonAccountGhdv.ibusPersonAccount);
                                lbusPersonAccountGhdv.UpdatePersonAccountDependentList();
                            }
                            lbusPersonAccountGhdv.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.cobra_letter_status_value = busConstant.COBRALetterStatusSent;
                            lbusPersonAccountGhdv.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.Update();
                        }
                    }
                }
                //PIR 25994 - check flex Plans from flexcomp history objects
                if (lbusPersonAccountFlexComp.IsNotNull() && lbusPersonAccountFlexComp.ibusPersonAccount.IsNotNull() &&
                    lbusPersonAccountFlexComp.icdoPersonAccountFlexComp.person_account_id > 0 &&
                    lbusPersonAccountFlexComp.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdFlex)
                {
                    lbusPersonAccountFlexComp.ibusPersonAccount.icdoPersonAccount.person_employment_dtl_id = lbusPersonAccountFlexComp.ibusPersonAccount.GetEmploymentDetailID();
                    if (lbusPersonAccountFlexComp.ibusPersonAccount.icdoPersonAccount.person_employment_dtl_id == 0)
                    {
                        lbusPersonAccountFlexComp.ibusPersonAccount.idtPlanEffectiveDate = lbusPersonAccountFlexComp.ibusPersonAccount.icdoPersonAccount.end_date_no_null;
                        lbusPersonAccountFlexComp.ibusPersonAccount.icdoPersonAccount.person_employment_dtl_id = lbusPersonAccountFlexComp.ibusPersonAccount.GetEmploymentDetailID();
                    }
                    if (lbusPersonAccountFlexComp.ibusPersonAccount.icdoPersonAccount.person_employment_dtl_id > 0)
                    {
                        lbusPersonAccountFlexComp.ibusPersonAccount.LoadPersonEmploymentDetail();
                    }

                    if (((lbusPersonAccountFlexComp.ibusPersonAccount.icdoPersonAccount.person_employment_dtl_id > 0) && lbusPersonAccountFlexComp.ibusPersonAccount.ibusPersonEmploymentDetail.IsNotNull()))
                    {
                        if ((lintPreviousPersonID == lbusPersonAccountFlexComp.ibusPersonAccount.icdoPersonAccount.person_id) ||
                        //if ((lintPreviousPersonID == lbusPersonAccountFlexComp.ibusPersonAccount.icdoPersonAccount.person_id && !lblnCorGeneratedForFlex) ||
                            lbusPersonAccountFlexComp.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.cobra_letter_status_value.IsNull() ||
                            lbusPersonAccountFlexComp.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.cobra_letter_status_value == busConstant.COBRALetterStatusNotSent)
                        {
                            lbusPersonAccountFlexComp.ibusPersonAccount.ibusPersonEmploymentDetail.LoadPersonEmployment();
                            lbusPersonAccountFlexComp.LoadPlan();
                            lbusPersonAccountFlexComp.ibusPersonAccount.LoadPerson();

                            if (lbusPersonAccountFlexComp.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdFlex)
                            {
                                if (lbusPersonAccountFlexComp.ibusPersonAccount.icdoPersonAccount.person_account_id > 0)
                                {
                                    lbusPersonAccountFlexComp.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.lintBatchIDCOBRANotice = this.iobjBatchSchedule.batch_schedule_id;
                                    //if (lbusPersonAccountFlexComp.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusFlexSuspended)
                                    //    lbusPersonAccountFlexComp.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.Suspended_Plan_Start_Date_Long_Date =
                                    //        lbusPersonAccountFlexComp.ibusPersonAccount.icdoPersonAccount.end_date.AddDays(1).ToString(busConstant.DateFormatLongDate);
                                    lbusPersonAccountFlexComp.LoadFlexCompHistory();
                                    if (lbusPersonAccountFlexComp.iclbFlexCompHistory.IsNotNull() && lbusPersonAccountFlexComp.iclbFlexCompHistory.Count > 0)
                                    {
                                        lbusPersonAccountFlexComp.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.Suspended_Plan_Start_Date_Long_Date =
                                        lbusPersonAccountFlexComp.iclbFlexCompHistory.FirstOrDefault(objFlexCompHistory=> objFlexCompHistory.icdoPersonAccountFlexCompHistory.plan_participation_status_value == busConstant.PlanParticipationStatusFlexSuspended)
                                        .icdoPersonAccountFlexCompHistory.effective_start_date.ToString(busConstant.DateFormatLongDate);
                                    }
                                    // ** BR - 242 ** The Batch correspondence letter will generate in the following sequence.
                                    if (lintPreviousPersonID != lbusPersonAccountFlexComp.ibusPersonAccount.icdoPersonAccount.person_id)
                                    {
                                        CreateAddressEnvelope(lbusPersonAccountFlexComp.ibusPersonAccount.ibusPerson);
                                    }
                                    //lblnCorGeneratedForFlex = true;
                                    CreateCOBRAFlexCompNotice(lbusPersonAccountFlexComp.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment);
                                    lbusPersonAccountFlexComp.ibusPersonAccount.UpdatePersonAccountDependentList();
                                }
                                lbusPersonAccountFlexComp.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.cobra_letter_status_value = busConstant.COBRALetterStatusSent;
                                lbusPersonAccountFlexComp.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.Update();
                            }
                        }
                    }
                }
            }
        }

        public void CreateAddressEnvelope(busPerson abusPerson)
        {
            //ArrayList larrList = new ArrayList();
            //larrList.Add(abusPerson);

            Hashtable lshtTemp = new Hashtable();
            lshtTemp.Add("FormTable", "Batch");

            idlgUpdateProcessLog("Creating Envelope", "INFO", istrProcessName);
            CreateCorrespondence("PER-0950", abusPerson, lshtTemp);
            //CreateContactTicket(abusPerson.icdoPerson.person_id);
        }
        public void CreateCOBRAInsuranceNotice(busPersonEmployment abusPersonEmployment, busPersonAccount abusPersonAccount)
        {
            //Assign the Person Employment into Person Account in order to display Employment End Date
            if (abusPersonAccount.ibusPersonEmploymentDetail == null)
                abusPersonAccount.ibusPersonEmploymentDetail = new busPersonEmploymentDetail();
            abusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment = abusPersonEmployment;
            if (abusPersonAccount.ibusPlan == null)
                abusPersonAccount.LoadPlan();

            //PROD PIR 4518
            if ((abusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDental) ||
                (abusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdVision) ||
                (abusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth) ||
                (abusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMedicarePartD))
            {
                if (abusPersonAccount.ibusPersonAccountGHDV == null)
                    abusPersonAccount.LoadPersonAccountGHDV();

                abusPersonAccount.ibusPersonAccountGHDV.LoadPlanEffectiveDate();
                abusPersonAccount.ibusPersonAccountGHDV.DetermineEnrollmentAndLoadObjects(abusPersonAccount.ibusPersonAccountGHDV.idtPlanEffectiveDate, false);

                if (abusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDental)
                    abusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.dental_insurance_type_value = busConstant.DentalInsuranceTypeCOBRA;
                else if (abusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdVision)
                    abusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.vision_insurance_type_value = busConstant.VisionInsuranceTypeCOBRA;

                if (abusPersonAccount.ibusPersonAccountGHDV.IsHealthOrMedicare)
                {
                    if (abusPersonAccount.ibusPersonAccountGHDV.IsCoverageCodeCodeSingle())
                        abusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.coverage_code = "0004";
                    else
                        abusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.coverage_code = "0005";

                    abusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.cobra_type_value = busConstant.COBRAType18Month;

                    if (abusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.overridden_structure_code.IsNotNullOrEmpty())
                    {
                        abusPersonAccount.ibusPersonAccountGHDV.LoadRateStructureForUserStructureCode();
                    }
                    else
                    {
                        abusPersonAccount.ibusPersonAccountGHDV.LoadHealthParticipationDate();
                        abusPersonAccount.ibusPersonAccountGHDV.LoadHealthPlanOption();
                        //To Get the Rate Structure Code (Derived Field)
                        abusPersonAccount.ibusPersonAccountGHDV.LoadRateStructure();
                    }

                    //Get the Coverage Ref ID
                    abusPersonAccount.ibusPersonAccountGHDV.LoadCoverageRefID();

                    //Get the Premium Amount
                    abusPersonAccount.ibusPersonAccountGHDV.GetMonthlyPremiumAmountByRefID();
                }
                else
                {
                    abusPersonAccount.ibusPersonAccountGHDV.GetMonthlyPremiumAmount();
                }

                abusPersonAccount.ldclTotalCOBRAPremiumAmount = abusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.MonthlyPremiumAmount;
            }
            abusPersonAccount.ibusPersonAccountGHDV.LoadEnrolledPlanAndPremiumForPersonAccountGHDV(); // PIR 6919
            //ArrayList larrList = new ArrayList();
            //larrList.Add(abusPersonAccount);

            Hashtable lshtTemp = new Hashtable();
            lshtTemp.Add("FormTable", "Batch");
            idlgUpdateProcessLog("Creating Correspondence for COBRA Insurance", "INFO", istrProcessName);
            CreateCorrespondence("PER-0105", abusPersonAccount, lshtTemp);
            //CreateContactTicket(abusPersonEmployment.icdoPersonEmployment.person_id);
        }

        private void CreateCOBRAFlexCompNotice(busPersonEmployment abusPersonEmployment)
        {
            if (abusPersonEmployment.ibusOrganization == null)
                abusPersonEmployment.LoadOrganization();

            if (abusPersonEmployment.ibusPerson == null)
                abusPersonEmployment.LoadPerson();

            //ArrayList larrList = new ArrayList();
            //larrList.Add(abusPersonEmployment);

            Hashtable lshtTemp = new Hashtable();
            lshtTemp.Add("FormTable", "Batch");
            idlgUpdateProcessLog("Creating Correspondence for COBRA FlexComp", "INFO", istrProcessName);
            CreateCorrespondence("PER-0104", abusPersonEmployment, lshtTemp);
            //CreateContactTicket(abusPersonEmployment.icdoPersonEmployment.person_id);
        }
    }
}
