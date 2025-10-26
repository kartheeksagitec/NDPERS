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
using Sagitec.ExceptionPub;
using System.Linq;

#endregion

namespace NeoSpinBatch
{
    class busWelcomeBatch : busNeoSpinBatch
    {
        private bool IsEmployerOfferGivenPlan(busPerson abusPerson, int aintPlanId)
        {
            if (abusPerson.icolPersonEmployment == null)
                abusPerson.LoadPersonEmployment();

            foreach (busPersonEmployment lbusPersonEmployment in abusPerson.icolPersonEmployment)
            {
                //Active Employment
                if (lbusPersonEmployment.icdoPersonEmployment.end_date_no_null >= DateTime.Now)
                {
                    if (lbusPersonEmployment.ibusOrganization == null)
                        lbusPersonEmployment.LoadOrganization();

                    if (lbusPersonEmployment.ibusOrganization.iclbOrganizationOfferedPlans == null)
                        lbusPersonEmployment.ibusOrganization.LoadOrganizationOfferedPlans();

                    foreach (busOrgPlan lbusOrgPlan in lbusPersonEmployment.ibusOrganization.iclbOrganizationOfferedPlans)
                    {
                        if (lbusOrgPlan.icdoOrgPlan.plan_id == aintPlanId)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public void CreateWelcomeBatchCorrespondence()
        {
            istrProcessName = "Welcome Batch";
            idlgUpdateProcessLog("Loading All New Members", "INFO", istrProcessName);
            DataTable ldbtMember = busNeoSpinBase.Select("cdoPerson.LoadWelcomeBatchLetterMembers", new object[1] { iobjSystemManagement.icdoSystemManagement.batch_date });
            DataTable ldtbLoadAllPrevEmployment = busNeoSpinBase.Select("cdoPerson.LoadAllRefundedEndedEmployment", new object[1] { iobjSystemManagement.icdoSystemManagement.batch_date });
            //utlPassInfo.iobjPassInfo.idictParams[utlConstants.istrProcessAuditLogSync] = true;
            foreach (DataRow ldrRow in ldbtMember.Rows)
            {
                busPerson lbusPerson = new busPerson { icdoPerson = new cdoPerson() };
                lbusPerson.icdoPerson.LoadData(ldrRow);

                busPersonEmployment lbusPersonEmployment = new busPersonEmployment() { icdoPersonEmployment = new cdoPersonEmployment() };
                lbusPersonEmployment.icdoPersonEmployment.LoadData(ldrRow);
                
                try
                {
                    idlgUpdateProcessLog("Generating Welcome Letter for Person ID" + lbusPerson.icdoPerson.person_id.ToString(), "INFO", istrProcessName);
                    if (utlPassInfo.iobjPassInfo.iconFramework.ConnectionString.IsNullOrEmpty())
                        utlPassInfo.iobjPassInfo.iconFramework.ConnectionString = DBFunction.GetDBConnection().ConnectionString;
                    utlPassInfo.iobjPassInfo.BeginTransaction();

                    if (lbusPerson.icolPersonAccount == null)
                        lbusPerson.LoadPersonAccount(false);

                    if (lbusPerson.icolPersonEmployment == null)
                        lbusPerson.LoadPersonEmployment();

                    lbusPerson.iclbRefundIssuedEmployment = new Collection<busPersonEmployment>();

                    if (lbusPerson.IsMemberHasRefundApplication())
                    {
                        DataRow[] ldtrPrevEmployment = busGlobalFunctions.FilterTable(ldtbLoadAllPrevEmployment, busConstant.DataType.Numeric,
                                                                                        "PERSON_ID", lbusPerson.icdoPerson.person_id);
                        foreach (DataRow ldtrRow in ldtrPrevEmployment)
                        {
                            busPersonEmployment lobjPersonEmployment = new busPersonEmployment
                            {
                                icdoPersonEmployment = new cdoPersonEmployment(),
                                ibusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() }
                            };
                            lobjPersonEmployment.icdoPersonEmployment.LoadData(ldtrRow);
                            lobjPersonEmployment.ibusOrganization.icdoOrganization.LoadData(ldtrRow);
                            lbusPerson.iclbRefundIssuedEmployment.Add(lobjPersonEmployment);
                        }
                        if (lbusPerson.iclbRefundIssuedEmployment.Count > 0)
                            lbusPerson.is_permanent_has_past_refund = true;
                    }

                    if (
                        IsEmployerOfferGivenPlan(lbusPerson, busConstant.PlanIdFlex) &&
                        lbusPerson.IsPermanentMember() &&
                        (!lbusPerson.IsMemberEnrolledInPlan(busConstant.PlanIdFlex))
                        )
                    {
                        lbusPerson.is_permanent_and_not_in_flex_plan = true;
                    }

                    if (
                        IsEmployerOfferGivenPlan(lbusPerson, busConstant.PlanIdDeferredCompensation) &&
                        lbusPerson.IsPermanentMember() &&
                        (!lbusPerson.IsMemberEnrolledInPlan(busConstant.PlanIdDeferredCompensation))
                        )
                    {
                        lbusPerson.is_permanent_and_not_in_deff_comp_plan = true;
                    }
                    lbusPersonEmployment.LoadOrganization();
                    lbusPersonEmployment.LoadLatestPersonEmploymentDetail();
                    lbusPerson.LoadEmployerWelcomeBenefitPlans(lbusPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id);
                    //ArrayList larrList = new ArrayList();
                    //larrList.Add(lbusPerson);
                    Hashtable lshtTemp = new Hashtable();
                    lshtTemp.Add("sfwCallingForm", "Batch");

                    //if (lbusPersonEmployment.ibusOrganization.icdoOrganization.mss_access_value == busConstant.OrganizationFullAccess)
                    CreateCorrespondence("ENR-5953", lbusPerson, lshtTemp);
                    //else if (lbusPersonEmployment.ibusOrganization.icdoOrganization.mss_access_value == busConstant.OrganizationLimitedAccess)
                        //CreateCorrespondence("ENR-5950", lbusPerson, lshtTemp);
                    CreateContactTicket(lbusPerson.icdoPerson.person_id);

                    lbusPerson.icdoPerson.welcome_batch_letter_sent_flag = busConstant.Flag_Yes;
                    lbusPerson.icdoPerson.Update();
                    utlPassInfo.iobjPassInfo.Commit();
                }
                catch (Exception _exc)
                {
                    ExceptionManager.Publish(_exc);
                    utlPassInfo.iobjPassInfo.Rollback();
                    idlgUpdateProcessLog("Welcome Batch Failed for Person ID " + lbusPerson.icdoPerson.person_id.ToString() + " " +
                        " Message : " + _exc.Message, "ERR", iobjBatchSchedule.step_name);
                }
            }
            //utlPassInfo.iobjPassInfo.idictParams.Remove(utlConstants.istrProcessAuditLogSync);
            idlgUpdateProcessLog("Welcome Batch Ended", "INFO", istrProcessName);
        }

        // Create Contact Ticket
        private void CreateContactTicket(int aintPersonID)
        {
            cdoContactTicket lobjContactTicket = new cdoContactTicket();
            CreateContactTicket(aintPersonID, busConstant.ContactTicketTypeInsuranceRetiree, lobjContactTicket);
        }
    }
}
