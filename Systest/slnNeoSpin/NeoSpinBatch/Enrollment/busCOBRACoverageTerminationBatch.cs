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

#endregion

namespace NeoSpinBatch
{
    class busCOBRACoverageTerminationBatch : busNeoSpinBatch
    {
        public void CreateCOBRATerminationLetters()
        {
            istrProcessName = "COBRA Termination Batch Letters";
            idlgUpdateProcessLog("Creating Correspondence for COBRA Termination", "INFO", istrProcessName);
            DataTable ldtbCOBRAs = DBFunction.DBSelect("cdoPersonAccountGhdv.COBRACoverageTermination", new object[] { },
                                            iobjPassInfo.iconFramework,
                                            iobjPassInfo.itrnFramework);
            foreach (DataRow dr in ldtbCOBRAs.Rows)
            {
                busPersonAccountGhdv lobjPersonAccountGHDV = new busPersonAccountGhdv();
                lobjPersonAccountGHDV.icdoPersonAccountGhdv = new cdoPersonAccountGhdv();
                lobjPersonAccountGHDV.icdoPersonAccount = new cdoPersonAccount();
                lobjPersonAccountGHDV.icdoPersonAccount.LoadData(dr);
                lobjPersonAccountGHDV.icdoPersonAccountGhdv.LoadData(dr);
                lobjPersonAccountGHDV.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                lobjPersonAccountGHDV.ibusPerson.icdoPerson.LoadData(dr);
                lobjPersonAccountGHDV.ibusPlan = new busPlan { icdoPlan = new cdoPlan() };
                lobjPersonAccountGHDV.ibusPlan.icdoPlan.LoadData(dr);

                //As per discussion with Maik, 07July2011, we need to generate letter if the member's cobra termination date is reached, 
                //Depending on Payee accounts only the section in correpondence changes
                ////UAT PIR 973:  Ignore Suspended Payee Account Records
                //lobjPersonAccountGHDV.ibusPerson.LoadPayeeAccount(true);
                //bool lblnOtherThanRefundPayeeAccountExists = false;
                //foreach (busPayeeAccount lbusPayeeAccount in lobjPersonAccountGHDV.ibusPerson.iclbPayeeAccount)
                //{
                //    if (lbusPayeeAccount.icdoPayeeAccount.benefit_account_type_value != busConstant.ApplicationBenefitTypeRefund)
                //    {
                //        lblnOtherThanRefundPayeeAccountExists = true;
                //        break;
                //    }
                //}

                //if (lobjPersonAccountGHDV.ibusPerson.iclbPayeeAccount.Count > 0 && (!lblnOtherThanRefundPayeeAccountExists)) continue;

                idlgUpdateProcessLog("Processing COBRA Termination Letter for Person ID : " + lobjPersonAccountGHDV.icdoPersonAccount.person_id, "INFO", istrProcessName);

                //Loading Current Premium
                //Initialize the Org Object to Avoid the NULL error
                lobjPersonAccountGHDV.InitializeObjects();

                lobjPersonAccountGHDV.LoadPlanEffectiveDate();
                lobjPersonAccountGHDV.DetermineEnrollmentAndLoadObjects(lobjPersonAccountGHDV.idtPlanEffectiveDate, false);

                if (lobjPersonAccountGHDV.IsHealthOrMedicare)
                {
                    if (lobjPersonAccountGHDV.icdoPersonAccountGhdv.overridden_structure_code.IsNotNullOrEmpty())
                    {
                        lobjPersonAccountGHDV.LoadRateStructureForUserStructureCode();
                    }
                    else
                    {
                        //Load the Health Plan Participation Date (based on effective Date)
                        lobjPersonAccountGHDV.LoadHealthParticipationDate();
                        //To Get the Rate Structure Code (Derived Field)
                        lobjPersonAccountGHDV.LoadRateStructure();
                    }
                    //Get the Coverage Ref ID
                    lobjPersonAccountGHDV.LoadCoverageRefID();

                    lobjPersonAccountGHDV.GetMonthlyPremiumAmountByRefID();
                }
                else
                {
                    lobjPersonAccountGHDV.GetMonthlyPremiumAmount();
                }

                lobjPersonAccountGHDV.idecCurrentMonthlyPremium = lobjPersonAccountGHDV.icdoPersonAccountGhdv.MonthlyPremiumAmount;
                lobjPersonAccountGHDV.idecCurrentMonthlyRhic = lobjPersonAccountGHDV.icdoPersonAccountGhdv.total_rhic_amount;

                //Update the Record to Suspended
                lobjPersonAccountGHDV.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusInsuranceSuspended;
                //PIR 17358 - If cobra_expiration_date is null then history_change_date should be the 1st day of month in which the member turns 65.
                //And if the DOB is the 1st day of month then history_change_date should be 1st day of month prior.
                if (lobjPersonAccountGHDV.icdoPersonAccount.cobra_expiration_date != DateTime.MinValue)
                    lobjPersonAccountGHDV.icdoPersonAccount.history_change_date = lobjPersonAccountGHDV.icdoPersonAccount.cobra_expiration_date.GetFirstDayofNextMonth();
                else
                {
                    if (lobjPersonAccountGHDV.ibusPerson.icdoPerson.date_of_birth.Day == 1)
                        lobjPersonAccountGHDV.icdoPersonAccount.history_change_date = lobjPersonAccountGHDV.ibusPerson.icdoPerson.date_of_birth.AddYears(65).GetFirstDayofCurrentMonth().AddMonths(-1);
                    else
                        lobjPersonAccountGHDV.icdoPersonAccount.history_change_date = lobjPersonAccountGHDV.ibusPerson.icdoPerson.date_of_birth.AddYears(65).GetFirstDayofCurrentMonth();
                }
                lobjPersonAccountGHDV.icdoPersonAccountGhdv.reason_value = busConstant.EnrollmentReasonEndofCOBRA;
                lobjPersonAccountGHDV.iarrChangeLog.Add(lobjPersonAccountGHDV.icdoPersonAccountGhdv);
                lobjPersonAccountGHDV.iarrChangeLog.Add(lobjPersonAccountGHDV.icdoPersonAccount);
                lobjPersonAccountGHDV.icdoPersonAccountGhdv.ienuObjectState = ObjectState.Update;
                lobjPersonAccountGHDV.icdoPersonAccount.ienuObjectState = ObjectState.Update;
                lobjPersonAccountGHDV.BeforeValidate(utlPageMode.Update);
                lobjPersonAccountGHDV.BeforePersistChanges();
                lobjPersonAccountGHDV.PersistChanges();
                lobjPersonAccountGHDV.ValidateSoftErrors();
                lobjPersonAccountGHDV.UpdateValidateStatus();
                lobjPersonAccountGHDV.AfterPersistChanges();


                //Load the Projected Premium by setting up the retiree
                if (lobjPersonAccountGHDV.IsHealthOrMedicare)
                {
                    lobjPersonAccountGHDV.icdoPersonAccountGhdv.health_insurance_type_value = busConstant.HealthInsuranceTypeRetiree;
                    lobjPersonAccountGHDV.idtPlanEffectiveDate = lobjPersonAccountGHDV.icdoPersonAccount.history_change_date_no_null;
                    lobjPersonAccountGHDV.icdoPersonAccountGhdv.plan_option_value = null;
                    lobjPersonAccountGHDV.icdoPersonAccountGhdv.overridden_structure_code = string.Empty;
                    //Email from Maik Dated On 7/7/2010
                    if (lobjPersonAccountGHDV.icdoPersonAccountGhdv.coverage_code == "0004")
                        lobjPersonAccountGHDV.icdoPersonAccountGhdv.coverage_code = "0021";
                    else if (lobjPersonAccountGHDV.icdoPersonAccountGhdv.coverage_code == "0005")
                        lobjPersonAccountGHDV.icdoPersonAccountGhdv.coverage_code = "0022";
                }
                else if (lobjPersonAccountGHDV.icdoPersonAccount.plan_id == busConstant.PlanIdDental)
                {
                    lobjPersonAccountGHDV.icdoPersonAccountGhdv.dental_insurance_type_value = busConstant.DentalInsuranceTypeRetiree;
                }
                else if (lobjPersonAccountGHDV.icdoPersonAccount.plan_id == busConstant.PlanIdVision)
                {
                    lobjPersonAccountGHDV.icdoPersonAccountGhdv.vision_insurance_type_value = busConstant.VisionInsuranceTypeRetiree;
                }
                lobjPersonAccountGHDV.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusInsuranceEnrolled;
                lobjPersonAccountGHDV.icdoPersonAccountGhdv.CobraTypeValue = lobjPersonAccountGHDV.icdoPersonAccountGhdv.cobra_type_value;//PIR 8518
                lobjPersonAccountGHDV.icdoPersonAccountGhdv.cobra_type_value = null;

                if (lobjPersonAccountGHDV.IsHealthOrMedicare)
                {
                    if (lobjPersonAccountGHDV.icdoPersonAccountGhdv.overridden_structure_code.IsNotNullOrEmpty())
                    {
                        lobjPersonAccountGHDV.LoadRateStructureForUserStructureCode();
                    }
                    else
                    {
                        //Load the Health Plan Participation Date (based on effective Date)
                        lobjPersonAccountGHDV.LoadHealthParticipationDate();
                        //To Get the Rate Structure Code (Derived Field)
                        lobjPersonAccountGHDV.LoadRateStructure();
                    }
                    //Get the Coverage Ref ID
                    lobjPersonAccountGHDV.LoadCoverageRefID();

                    lobjPersonAccountGHDV.GetMonthlyPremiumAmountByRefID();
                }
                else
                {
                    lobjPersonAccountGHDV.GetMonthlyPremiumAmount();
                }
                lobjPersonAccountGHDV.idecProjectedMonthlyPremium = lobjPersonAccountGHDV.icdoPersonAccountGhdv.MonthlyPremiumAmount;

                //ArrayList larrList = new ArrayList();
                //larrList.Add(lobjPersonAccountGHDV);

                Hashtable lshtTemp = new Hashtable();
                lshtTemp.Add("FormTable", "Batch");
                CreateCorrespondence("PER-0150", lobjPersonAccountGHDV, lshtTemp);
                CreateContactTicket(lobjPersonAccountGHDV.ibusPerson.icdoPerson.person_id);
            }
            idlgUpdateProcessLog("Correspondence created successfully", "INFO", istrProcessName);
        }

        private void CreateContactTicket(int aintPersonID)
        {
            cdoContactTicket lobjContactTicket = new cdoContactTicket();
            CreateContactTicket(aintPersonID, busConstant.ContactTicketTypeInsuranceRetiree, lobjContactTicket);
        }
    }
}
