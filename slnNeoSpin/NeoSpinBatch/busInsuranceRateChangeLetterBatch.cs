using System;
using System.Linq;
using System.Data;
using NeoSpin.BusinessObjects;
using System.Collections.ObjectModel;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
using System.Collections;
using Sagitec.ExceptionPub;
using System.Collections.Generic;


namespace NeoSpinBatch
{
    class busInsuranceRateChangeLetterBatch : busNeoSpinBatch
    {
        public string istrRateStructureCode { get; set; }
        public busInsuranceRateChangeLetterBatch()
        { }

        //public void GenerateLetterForRateChange()
        //{
        //    istrProcessName = iobjBatchSchedule.step_name;
        //    idlgUpdateProcessLog("Loading All the Pending Requests", "INFO", istrProcessName);
        //    DataTable ldtbList = busBase.Select<cdoRateChangeLetterRequest>(new string[1] { "STATUS_VALUE" },
        //                                                                        new object[1] { busConstant.LetterStatuValuePending }, null, null);
        //    if (ldtbList.Rows.Count > 0)
        //    {
        //        foreach (DataRow dr in ldtbList.Rows)
        //        {
        //            busRateChangeLetterRequest lobjRateChangeLetterRequest = new busRateChangeLetterRequest
        //            {
        //                icdoRateChangeLetterRequest = new cdoRateChangeLetterRequest()
        //            };
        //            lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.LoadData(dr);
        //            try
        //            {
        //                //For Letter Type RHIC, we assign the Health as a Default Plan ID here
        //                lobjRateChangeLetterRequest.GetPlanIDFromLetterType();
        //                lobjRateChangeLetterRequest.LoadPlan();

        //                //Loading All the Active Providers for the Given Effective Date / Provider / PlanID
        //                lobjRateChangeLetterRequest.LoadAllProviderOrgPlans();

        //                //load cached data as per plan
        //                lobjRateChangeLetterRequest.LoadDBCacheData();

        //                //Load New and current effective date                    
        //                lobjRateChangeLetterRequest.LoadCurrentAndNewRateEffectiveDate();

        //                //Load the RHIC Combine Approved records
        //                lobjRateChangeLetterRequest.iblnIsInsuranceRateChangeLetterBatch = true;
        //                if (lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdGroupHealth ||
        //                    lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdMedicarePartD)
        //                    lobjRateChangeLetterRequest.LoadAllApprovedRHICCombine();

        //                //Load the RHIC Factor for the Letter Type RHIC
        //                if (lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.letter_type_value == busConstant.LetterTypeValueRHIC)
        //                {
        //                    lobjRateChangeLetterRequest.LoadCurrentAndNewRHICBenefitFactor();
        //                }

        //                //Load Provider Org Plan for New Effective Date
        //                lobjRateChangeLetterRequest.LoadProviderOrgPlan();

        //                //PIR-9093 Start  
        //                //Loading the Rate collections for Health Vision and Dental Plans for 
        //                DataTable ldtbHealthRateStructure = busBase.Select("cdoRateChangeLetterRequest.LoadHealthRateStructureCodes",
        //                    new object[1] { lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.effective_date });
        //                Collection<busOrgPlanGroupHealthMedicarePartDRateRef> lclbHealthRateStructure = new Collection<busOrgPlanGroupHealthMedicarePartDRateRef>();
        //                lclbHealthRateStructure = new busBase().GetCollection<busOrgPlanGroupHealthMedicarePartDRateRef>(ldtbHealthRateStructure, "icdoOrgPlanGroupHealthMedicarePartDRateRef");

        //                DataTable ldtbVisionRateStructure = busBase.Select("cdoRateChangeLetterRequest.LoadVisionlRateStructureCodes",
        //                   new object[1] { lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.effective_date });

        //                Collection<busOrgPlanVisionRate> lclbVisionRateStructure = new Collection<busOrgPlanVisionRate>();
        //                lclbVisionRateStructure = new busBase().GetCollection<busOrgPlanVisionRate>(ldtbVisionRateStructure, "icdoOrgPlanVisionRate");

        //                DataTable ldtbDentalRateStructure = busBase.Select("cdoRateChangeLetterRequest.LoadDentalRateStructureCodes",
        //                   new object[1] { lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.effective_date });
        //                Collection<busOrgPlanDentalRate> lclbDentalRateStructure = new Collection<busOrgPlanDentalRate>();
        //                lclbDentalRateStructure = new busBase().GetCollection<busOrgPlanDentalRate>(ldtbDentalRateStructure, "icdoOrgPlanDentalRate");
        //                //PIR-9093  End

        //                //load history that will be needed for RHIC also
        //                //Loading All IBS Members History
        //                #region Loading All IBS Members History
        //                idlgUpdateProcessLog("Loading All IBS Members History Records", "INFO", istrProcessName);

        //                if ((lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdDental)
        //                    || (lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdVision)
        //                    || (lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdGroupHealth))
        //                {
        //                    lobjRateChangeLetterRequest.idtbIBSMembersGHDVHistory =
        //                        busBase.Select("cdoRateChangeLetterRequest.LoadIBSMembersGHDVHistory",
        //                                       new object[2]
        //                                                   {
        //                                                       lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.iintPlanId,
        //                                                       lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.effective_date
        //                                                   });
        //                }
        //                else if (lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdLTC)
        //                {
        //                    lobjRateChangeLetterRequest.idtbIBSMembersLTCHistory = busBase.Select("cdoRateChangeLetterRequest.LoadIBSMembersLTCHistory",
        //                                                                new object[1] { lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.effective_date });
        //                }
        //                else if (lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdGroupLife)
        //                {
        //                    lobjRateChangeLetterRequest.idtbIBSMembersLIFEHistory = busBase.Select("cdoRateChangeLetterRequest.LoadIBSMembersLifeHistory",
        //                                                                new object[1] { lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.effective_date });

        //                    lobjRateChangeLetterRequest.idtbIBSMembersLIFEOption = busBase.Select("cdoRateChangeLetterRequest.LoadIBSMembersLifeOptions",
        //                                                                 new object[1] { lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.effective_date });
        //                }
        //                //PIR 15683
        //                else if (lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdMedicarePartD)
        //                {
        //                    lobjRateChangeLetterRequest.idtbIBSMembersMedicarePartDHistory = busBase.Select("cdoRateChangeLetterRequest.LoadIBSMembersMedicareHistory",
        //                                                               new object[1] { lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.effective_date });
        //                }
        //                #endregion
        //                //Ignore Employer Rate Change Letter if the Letter Type is RHIC

        //                if (lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.letter_type_value != busConstant.LetterTypeValueRHIC)
        //                {
        //                    //Loading All Employer Org Plans 
        //                    idlgUpdateProcessLog("Loading All Employer Org Plans", "INFO", istrProcessName);
        //                    lobjRateChangeLetterRequest.LoadAllEmployerOrgPlans();

        //                    //Loading All Employer Org To Bill Members History (Optimization)
        //                    #region Loading All Employer Org To Bill History
        //                    idlgUpdateProcessLog("Loading All Org To Bill Employer Plan History", "INFO", istrProcessName);
        //                    if ((lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdDental)
        //                        || (lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdVision)
        //                        || (lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdGroupHealth)
        //                        || (lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdMedicarePartD))
        //                    {
        //                        lobjRateChangeLetterRequest.idtbOrgToBillGHDVHistory =
        //                            busBase.Select("cdoRateChangeLetterRequest.LoadOrgToBillGHDVHistory",
        //                                           new object[2]
        //                                                   {
        //                                                       lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.iintPlanId,
        //                                                       lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.effective_date
        //                                                   });
        //                    }
        //                    else if (lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdEAP)
        //                    {
        //                        lobjRateChangeLetterRequest.idtbOrgToBillEAPHistory = busBase.Select("cdoRateChangeLetterRequest.LoadOrgToBillEAPHistory",
        //                                                             new object[1] { lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.effective_date });
        //                    }
        //                    else if (lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdLTC)
        //                    {
        //                        lobjRateChangeLetterRequest.idtbOrgToBillLTCHistory = busBase.Select("cdoRateChangeLetterRequest.LoadOrgToBillLTCHistory",
        //                                                                    new object[1] { lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.effective_date });
        //                    }
        //                    else if (lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdGroupLife)
        //                    {
        //                        lobjRateChangeLetterRequest.idtbOrgToBillLIFEHistory = busBase.Select("cdoRateChangeLetterRequest.LoadOrgToBillLifeHistory",
        //                                                                    new object[1] { lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.effective_date });

        //                        lobjRateChangeLetterRequest.idtbOrgToBillLIFEOption = busBase.Select("cdoRateChangeLetterRequest.LoadOrgToBillLifeOptions",
        //                                                                     new object[1] { lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.effective_date });
        //                    }
        //                    #endregion

        //                    //Loading All TFFR Pension Check Member History
        //                    #region Loading All TFFR Pension Check Member History
        //                    //Loading All Employer Org To Bill History (Optimization)
        //                    idlgUpdateProcessLog("Loading All TFFR Pension Check Members History Records", "INFO", istrProcessName);

        //                    if ((lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdDental)
        //                        || (lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdVision)
        //                        || (lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdGroupHealth)
        //                        || (lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdMedicarePartD))
        //                    {
        //                        lobjRateChangeLetterRequest.idtbTFFRPensionCheckGHDVHistory =
        //                            busBase.Select("cdoRateChangeLetterRequest.LoadTFFRPensionCheckGHDVHistory",
        //                                           new object[2]
        //                                                   {
        //                                                       lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.iintPlanId,
        //                                                       lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.effective_date
        //                                                   });
        //                    }
        //                    else if (lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdEAP)
        //                    {
        //                        lobjRateChangeLetterRequest.idtbTFFRPensionCheckEAPHistory =
        //                            busBase.Select("cdoRateChangeLetterRequest.LoadTFFRPensionCheckEAPHistory",
        //                                           new object[1] { lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.effective_date });
        //                    }
        //                    else if (lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdLTC)
        //                    {
        //                        lobjRateChangeLetterRequest.idtbTFFRPensionCheckLTCHistory = busBase.Select("cdoRateChangeLetterRequest.LoadTFFRPensionCheckLTCHistory",
        //                                                                    new object[1] { lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.effective_date });
        //                    }
        //                    else if (lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdGroupLife)
        //                    {
        //                        lobjRateChangeLetterRequest.idtbTFFRPensionCheckLIFEHistory = busBase.Select("cdoRateChangeLetterRequest.LoadTFFRPensionCheckLifeHistory",
        //                                                                    new object[1] { lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.effective_date });

        //                        lobjRateChangeLetterRequest.idtbTFFRPensionCheckLIFEOption = busBase.Select("cdoRateChangeLetterRequest.LoadTFFRPensionCheckLifeOptions",
        //                                                                     new object[1] { lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.effective_date });
        //                    }
        //                    #endregion

        //                    //As per mail from Maik dated - 04/17/2015. Org Letter generation code commented for template PAY-4306.
        //                    //Generating Employer Rate Change Letter
        //                    //    foreach (busOrgPlan lobjOrgPlan in lobjRateChangeLetterRequest.iclbOrgPlan)
        //                    //    {
        //                    //        //initialize all collection to avoid error
        //                    //        lobjOrgPlan.iclbHealthCOBRAPremium = new Collection<busInsurancePremium>();
        //                    //        lobjOrgPlan.iclbHealthPremium = new Collection<busInsurancePremium>();
        //                    //        lobjOrgPlan.iclbDentalPremium = new Collection<busInsurancePremium>();
        //                    //        lobjOrgPlan.iclbVisionPremium = new Collection<busInsurancePremium>();
        //                    //        lobjOrgPlan.iclbEAPPremium = new Collection<busInsurancePremium>();
        //                    //        lobjOrgPlan.iclbTFFRPensionCheckPremium = new Collection<busInsurancePremium>();
        //                    //        lobjOrgPlan.iclbOrgToBillPremium = new Collection<busInsurancePremium>();

        //                    //        idlgUpdateProcessLog(
        //                    //            "Generating Employer Rate Change Letter for ORG Code : " +
        //                    //            lobjOrgPlan.ibusOrganization.icdoOrganization.org_code, "INFO",
        //                    //            istrProcessName);

        //                    //        lobjRateChangeLetterRequest.GenerateEmployerRateChangeLetter(lobjOrgPlan);

        //                    //        //Generating ORG to Bill Letters
        //                    //        idlgUpdateProcessLog("Generating Employer Letter for Org To Bill Members of Org Code : " +
        //                    //            lobjOrgPlan.ibusOrganization.icdoOrganization.org_code, "INFO", istrProcessName);

        //                    //        lobjRateChangeLetterRequest.GenerateEmployerLetterForOrgToBill(lobjOrgPlan, false);

        //                    //        //TFFR Pension Check Members Logic Starts Here
        //                    //        if (lobjOrgPlan.ibusOrganization.icdoOrganization.org_code == busConstant.RetirementAndInvestmentOrgCodeId)
        //                    //        {
        //                    //            idlgUpdateProcessLog("Generating Employer Letter for TFFR Pension Check Members", "INFO", istrProcessName);
        //                    //            //prod pir 7008
        //                    //            if (lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdGroupHealth)
        //                    //            {
        //                    //                lobjRateChangeLetterRequest.GenerateEmployerLetterForHealthTFFRPensionCheck(lobjOrgPlan);
        //                    //            }
        //                    //            else
        //                    //            {
        //                    //                lobjRateChangeLetterRequest.GenerateEmployerLetterForTFFRPensionCheck(lobjOrgPlan, false);
        //                    //            }
        //                    //        }

        //                    //        lobjOrgPlan.ibusRateChangeLetterRequest = lobjRateChangeLetterRequest;

        //                    //        //Generating Employer Letter Correspondence
        //                    //        idlgUpdateProcessLog(
        //                    //           "Creating Employer Letter Correspondence for Org Code : " +
        //                    //           lobjOrgPlan.ibusOrganization.icdoOrganization.org_code, "INFO",
        //                    //           istrProcessName);
        //                    //        CreateEmployerLetterCorrespondence(lobjOrgPlan);
        //                    //        //break; //??? Remove this After Test , Used only For Testing Few Records
        //                    //    }
        //                }

        //                //Ignore IBS Member Letter for EAP Plans
        //                if (lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.iintPlanId != busConstant.PlanIdEAP)
        //                {
        //                    //IBS Member Letter Logic Starts
        //                    idlgUpdateProcessLog("Generating IBS Member Rate Change Letter Started", "INFO", iobjBatchSchedule.step_name);

        //                    idlgUpdateProcessLog("Load All Net Check Amount for Pension Check Members", "INFO", iobjBatchSchedule.step_name);

        //                    lobjRateChangeLetterRequest.idtbNetCheckAmountForPenionCheckMembers =
        //                            busBase.Select("cdoRateChangeLetterRequest.LoadAllPensionCheckMembersNetCheckAmount",
        //                                           new object[1]
        //                                                   {
        //                                                       lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.effective_date
        //                                                   });

        //                    idlgUpdateProcessLog("Load All IBS Members", "INFO", iobjBatchSchedule.step_name);

        //                    DataTable ldtbIBSMembers = lobjRateChangeLetterRequest.LoadIBSMembers();
        //                    foreach (DataRow ldrRow in ldtbIBSMembers.Rows)
        //                    {
        //                        busInsurancePremium lobjInsurancePremium = lobjRateChangeLetterRequest.ProcessInsurancePremiumForIBSMember(ldrRow);
        //                        lobjInsurancePremium.ibusRateChangeLetterRequest = lobjRateChangeLetterRequest;
        //                        bool lblnGenerateLetter = true;
        //                        lobjInsurancePremium.LoadPerson(Convert.ToInt32(ldrRow["PersonId"]));

        //                        //PIR 15347 - Medicare Part D bookmarks
        //                        //if (lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.iintPlanId == busConstant.PlanIdMedicarePartD)
        //                        //{
        //                        //    busPersonAccountMedicarePartDHistory lbusPersonAccountMedicarePartDHistory = new busPersonAccountMedicarePartDHistory { icdoPersonAccountMedicarePartDHistory = new cdoPersonAccountMedicarePartDHistory { person_id = Convert.ToInt32(ldrRow["PersonId"]) }, icdoPersonAccount = new cdoPersonAccount() };
        //                        //    //lbusPersonAccountMedicarePartDHistory.FindMedicarePartDHistory(lobjDeathNotification.ibusPerson.icdoPerson.person_id);
        //                        //    lbusPersonAccountMedicarePartDHistory.GetTotalPremiumAmountForMedicareInsuranceRateChanage();
        //                        //    lobjInsurancePremium.idecTotalMedicareDPremiumAmount = lbusPersonAccountMedicarePartDHistory.TotalMonthlyPremiumAmount;
        //                        //}

        //                        //PIR 14304 - for template PAY-4305
        //                        lobjInsurancePremium.ibusPerson.LoadLatestBenefitRhicCombine();
        //                        if (lobjInsurancePremium.ibusPerson.ibusLatestBenefitRhicCombine.IsNotNull())
        //                            lobjInsurancePremium.idecTotalRHIC = lobjInsurancePremium.ibusPerson.ibusLatestBenefitRhicCombine.icdoBenefitRhicCombine.combined_rhic_amount;

        //                        lobjInsurancePremium.ibusPerson.LoadPersonAccount();
        //                        lobjInsurancePremium.ibusPerson.LoadPersonCurrentAddress();//PIR 11355
        //                        foreach (busPersonAccount lbusPersonAccount in lobjInsurancePremium.ibusPerson.icolPersonAccount)
        //                        {
        //                            if (lbusPersonAccount.icdoPersonAccount.plan_id == 12)
        //                            {
        //                                lbusPersonAccount.LoadPersonAccountGHDV();
        //                                if (lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.overridden_structure_code.IsNotNullOrEmpty())
        //                                {
        //                                    lbusPersonAccount.ibusPersonAccountGHDV.LoadRateStructureForUserStructureCode();
        //                                    istrRateStructureCode = lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.overridden_structure_code;
        //                                }
        //                                else
        //                                {
        //                                    lbusPersonAccount.ibusPersonAccountGHDV.LoadHealthParticipationDate();
        //                                    lbusPersonAccount.ibusPersonAccountGHDV.LoadRateStructure();
        //                                    istrRateStructureCode = lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.rate_structure_code;
        //                                }
        //                            }
        //                        }


        //                        //Systest PIR 2020 : Dont Generate Letter if either of the Amount is ZERO
        //                        //UAT PIR 1190 : If both amounts are same, dont generate the letter too.
        //                        //For Life and LTC, if any option premium matches, dont print it. if all of them matches, dont generate letters.
        //                        switch (lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.letter_type_value)
        //                        {
        //                            case busConstant.LetterTypeValueRHIC:
        //                                if ((lobjInsurancePremium.idecCurrentRHICAmount == 0) ||
        //                                    (lobjInsurancePremium.idecNewRHICAmount == 0) ||
        //                                    (lobjInsurancePremium.idecNewRHICAmount == lobjInsurancePremium.idecCurrentRHICAmount))
        //                                    lblnGenerateLetter = false;
        //                                break;
        //                            case busConstant.LetterTypeValueLIFE:
        //                                bool lblnMiniumumOneChangesFound = false;
        //                                if (lobjInsurancePremium.iclbCoverageLevelLifePremium.Count > 0)
        //                                {
        //                                    for (int i = lobjInsurancePremium.iclbCoverageLevelLifePremium.Count - 1; i >= 0; i--)
        //                                    {
        //                                        if ((lobjInsurancePremium.iclbCoverageLevelLifePremium[i].idecCurrentPremium == 0) ||
        //                                            (lobjInsurancePremium.iclbCoverageLevelLifePremium[i].idecNewPremium == 0))
        //                                        {
        //                                            lobjInsurancePremium.iclbCoverageLevelLifePremium.RemoveAt(i);
        //                                        }
        //                                    }
        //                                    //UAT PIR 1190 - If any one changes found, display everything                                            
        //                                    for (int i = lobjInsurancePremium.iclbCoverageLevelLifePremium.Count - 1; i >= 0; i--)
        //                                    {
        //                                        if (lobjInsurancePremium.iclbCoverageLevelLifePremium[i].idecNewPremium != lobjInsurancePremium.iclbCoverageLevelLifePremium[i].idecCurrentPremium)
        //                                        {
        //                                            lblnMiniumumOneChangesFound = true;
        //                                            break;
        //                                        }
        //                                    }
        //                                }
        //                                if ((lobjInsurancePremium.iclbCoverageLevelLifePremium.Count == 0) || (!lblnMiniumumOneChangesFound))
        //                                    lblnGenerateLetter = false;
        //                                break;
        //                            case busConstant.LetterTypeValueLTC:
        //                                bool lblnMiniumumOneChangesFoundInMember = false;
        //                                bool lblnMiniumumOneChangesFoundInSpouse = false;
        //                                if (lobjInsurancePremium.iclbCoverageLevelLTCMemberPremium.Count > 0)
        //                                {
        //                                    for (int i = lobjInsurancePremium.iclbCoverageLevelLTCMemberPremium.Count - 1; i >= 0; i--)
        //                                    {
        //                                        if ((lobjInsurancePremium.iclbCoverageLevelLTCMemberPremium[i].idecCurrentPremium == 0) ||
        //                                            (lobjInsurancePremium.iclbCoverageLevelLTCMemberPremium[i].idecNewPremium == 0))
        //                                        {
        //                                            lobjInsurancePremium.iclbCoverageLevelLTCMemberPremium.RemoveAt(i);
        //                                        }
        //                                    }

        //                                    for (int i = lobjInsurancePremium.iclbCoverageLevelLTCMemberPremium.Count - 1; i >= 0; i--)
        //                                    {
        //                                        if (lobjInsurancePremium.iclbCoverageLevelLTCMemberPremium[i].idecNewPremium != lobjInsurancePremium.iclbCoverageLevelLTCMemberPremium[i].idecCurrentPremium)
        //                                        {
        //                                            lblnMiniumumOneChangesFoundInMember = true;
        //                                            break;
        //                                        }
        //                                    }
        //                                }

        //                                if (lobjInsurancePremium.iclbCoverageLevelLTCSpousePremium.Count > 0)
        //                                {
        //                                    for (int i = lobjInsurancePremium.iclbCoverageLevelLTCSpousePremium.Count - 1; i >= 0; i--)
        //                                    {
        //                                        if ((lobjInsurancePremium.iclbCoverageLevelLTCSpousePremium[i].idecCurrentPremium == 0) ||
        //                                            (lobjInsurancePremium.iclbCoverageLevelLTCSpousePremium[i].idecNewPremium == 0))
        //                                        {
        //                                            lobjInsurancePremium.iclbCoverageLevelLTCSpousePremium.RemoveAt(i);
        //                                        }
        //                                    }

        //                                    for (int i = lobjInsurancePremium.iclbCoverageLevelLTCSpousePremium.Count - 1; i >= 0; i--)
        //                                    {
        //                                        if (lobjInsurancePremium.iclbCoverageLevelLTCSpousePremium[i].idecNewPremium != lobjInsurancePremium.iclbCoverageLevelLTCSpousePremium[i].idecCurrentPremium)
        //                                        {
        //                                            lblnMiniumumOneChangesFoundInSpouse = true;
        //                                            break;
        //                                        }
        //                                    }
        //                                }

        //                                if (((lobjInsurancePremium.iclbCoverageLevelLTCMemberPremium.Count == 0) && (lobjInsurancePremium.iclbCoverageLevelLTCSpousePremium.Count == 0))
        //                                    || ((!lblnMiniumumOneChangesFoundInMember) && (!lblnMiniumumOneChangesFoundInSpouse)))
        //                                    lblnGenerateLetter = false;
        //                                break;
        //                            case busConstant.LetterTypeValueDental:
        //                                if ((lclbDentalRateStructure.Count(o => o.icdoOrgPlanDentalRate.dental_insurance_type_value == ldrRow["DentalInsurance"].ToString() &&
        //                                    o.icdoOrgPlanDentalRate.level_of_coverage_value == ldrRow["LevelOfCoverage"].ToString()) <= 0) ||
        //                                    ((lobjInsurancePremium.idecCurrentPremium == 0) ||
        //                                    (lobjInsurancePremium.idecNewPremium == 0) ||
        //                                     (lobjInsurancePremium.idecNewPremium == lobjInsurancePremium.idecCurrentPremium)))
        //                                {
        //                                    lblnGenerateLetter = false;
        //                                }
        //                                break;
        //                            case busConstant.LetterTypeValueVision:
        //                                if ((lclbVisionRateStructure.Count(o => o.icdoOrgPlanVisionRate.vision_insurance_type_value == ldrRow["VisionInsurance"].ToString() &&
        //                                    o.icdoOrgPlanVisionRate.level_of_coverage_value == ldrRow["LevelOfCoverage"].ToString()) <= 0) ||
        //                                    ((lobjInsurancePremium.idecCurrentPremium == 0) ||
        //                                    (lobjInsurancePremium.idecNewPremium == 0) ||
        //                                     (lobjInsurancePremium.idecNewPremium == lobjInsurancePremium.idecCurrentPremium)))
        //                                {
        //                                    lblnGenerateLetter = false;
        //                                }
        //                                break;
        //                            case busConstant.LetterTypeValueHealth:
        //                                //PIR-9093 Start
        //                                if ((lclbHealthRateStructure.Count(o => o.icdoOrgPlanGroupHealthMedicarePartDRateRef.rate_structure_code == istrRateStructureCode) <= 0) || // ldrRow["RateStructureCode"].ToString()) > 0 
        //                                    ((lobjInsurancePremium.idecCurrentPremium == 0) ||
        //                                    (lobjInsurancePremium.idecNewPremium == 0) ||
        //                                     (lobjInsurancePremium.idecNewPremium == lobjInsurancePremium.idecCurrentPremium)))
        //                                {
        //                                    lblnGenerateLetter = false;
        //                                }
        //                                //PIR-9093 - End 
        //                                break;
        //                            case busConstant.LetterTypeValueMedicare:
        //                                if ((lobjInsurancePremium.idecCurrentMedicarePartDPremium == 0) ||
        //                                    (lobjInsurancePremium.idecNewMedicarePartDPremium == 0) ||
        //                                    (lobjInsurancePremium.idecNewMedicarePartDPremium == lobjInsurancePremium.idecCurrentMedicarePartDPremium))
        //                                    lblnGenerateLetter = false;
        //                                break;
        //                        }

        //                        if (lblnGenerateLetter)
        //                        {
        //                            //Get the Current and New Net Check Amount for the Payee Account
        //                            if (lobjInsurancePremium.iintPayeeAccountID > 0)
        //                            {
        //                                //Filter from the Loaded Collection
        //                                DataRow[] larrRow = lobjRateChangeLetterRequest.idtbNetCheckAmountForPenionCheckMembers.FilterTable(busConstant.DataType.Numeric,
        //                                                                   "payee_account_id",
        //                                                                   lobjInsurancePremium.iintPayeeAccountID);
        //                                if (larrRow != null && larrRow.Count() > 0)
        //                                {
        //                                    lobjInsurancePremium.idecCurrentNetCheckAmount = larrRow.First().Field<decimal>("NetCheckAmount");
        //                                    lobjInsurancePremium.idecNewNetCheckAmount =
        //                                        lobjInsurancePremium.idecCurrentNetCheckAmount +
        //                                        (lobjInsurancePremium.idecCurrentPremium - lobjInsurancePremium.idecNewPremium) +
        //                                        (lobjInsurancePremium.idecNewRHICAmount - lobjInsurancePremium.idecCurrentRHICAmount);
        //                                }
        //                            }

        //                            idlgUpdateProcessLog("Creating IBS Member Correspondence for " + lobjInsurancePremium.istrIBSMemberFullName, "INFO", iobjBatchSchedule.step_name);
        //                            CreateIBSLetterCorrespondence(lobjInsurancePremium);
        //                            //break; //??? Remove this After Test , Used only For Testing Few Records
        //                        }
        //                    }
        //                }
        //                lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.status_value = busConstant.StatusProcessed;
        //                lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.Update();
        //            }
        //            catch (Exception e)
        //            {
        //                ExceptionManager.Publish(e);
        //                idlgUpdateProcessLog("Rate Change Letter Request Failed for the following reason " + e, "ERR", istrProcessName);
        //                lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.status_value = busConstant.StatusFailed;
        //                lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.Update();
        //            }
        //        }
        //    }
        //    else
        //    {
        //        idlgUpdateProcessLog("Batch Ended since no rate change request found with Pending status", "INFO", istrProcessName);
        //    }
        //}

        private void CreateEmployerLetterCorrespondence(busOrgPlan aobjOrgPlan)
        {
            //ArrayList larrlist = new ArrayList();
            //larrlist.Add(aobjOrgPlan);
            Hashtable lhstDummyTable = new Hashtable();
            lhstDummyTable.Add("sfwCallingForm", "Batch");
            CreateCorrespondence("PAY-4306", aobjOrgPlan, lhstDummyTable);
        }

        private void CreateIBSLetterCorrespondence(busInsurancePremium lobjInsurancePremium)
        {
            //ArrayList larrlist = new ArrayList();
            //larrlist.Add(lobjInsurancePremium);
            Hashtable lhstDummyTable = new Hashtable();
            lhstDummyTable.Add("sfwCallingForm", "Batch");
            CreateCorrespondence("PAY-4305", lobjInsurancePremium, lhstDummyTable);
        }
        public void GenerateLettersForRateChange()
        {
            istrProcessName = iobjBatchSchedule.step_name;
            idlgUpdateProcessLog("Loading the Pending Request", "INFO", istrProcessName);
            DataTable ldtbList = busBase.Select<cdoRateChangeLetterRequest>(new string[1] { "STATUS_VALUE" },
                                                                                new object[1] { busConstant.LetterStatuValuePending }, null, null);
            if (ldtbList.Rows.Count > 0)
            {
                busRateChangeLetterRequest lobjRateChangeLetterRequest = new busRateChangeLetterRequest
                {
                    icdoRateChangeLetterRequest = new cdoRateChangeLetterRequest()
                };
                lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.LoadData(ldtbList.Rows[0]);
                //load cached data as per plan
                try
                {
                    lobjRateChangeLetterRequest.LoadDBCacheData();
                    lobjRateChangeLetterRequest.iblnIsInsuranceRateChangeLetterBatch = true;
                    idlgUpdateProcessLog("Loading All IBS Members History Records", "INFO", istrProcessName);
                    lobjRateChangeLetterRequest.idtbIBSMembersGHealthHistory =
                            busBase.Select("cdoRateChangeLetterRequest.LoadIBSMembersGHDVHistory",
                                            new object[2]
                                                        {
                                                            busConstant.PlanIdGroupHealth,
                                                            lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.effective_date
                                                        });
                    lobjRateChangeLetterRequest.idtbIBSMembersDentalHistory =
                            busBase.Select("cdoRateChangeLetterRequest.LoadIBSMembersGHDVHistory",
                                            new object[2]
                                                        {
                                                            busConstant.PlanIdDental,
                                                            lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.effective_date
                                                        });
                    lobjRateChangeLetterRequest.idtbIBSMembersVisionHistory =
                            busBase.Select("cdoRateChangeLetterRequest.LoadIBSMembersGHDVHistory",
                                            new object[2]
                                                        {
                                                            busConstant.PlanIdVision,
                                                            lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.effective_date
                                                        });
                    lobjRateChangeLetterRequest.idtbIBSMembersLIFEHistory = busBase.Select("cdoRateChangeLetterRequest.LoadIBSMembersLifeHistory",
                                                                new object[1] { lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.effective_date });

                    lobjRateChangeLetterRequest.idtbIBSMembersLIFEOption = busBase.Select("cdoRateChangeLetterRequest.LoadIBSMembersLifeOptions",
                                                                    new object[1] { lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.effective_date });
                    lobjRateChangeLetterRequest.idtbIBSMembersMedicarePartDHistory = busBase.Select("cdoRateChangeLetterRequest.LoadIBSMembersMedicareHistory",
                                                                new object[1] { lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.effective_date });
                    lobjRateChangeLetterRequest.idtbIBSHealthMembers = busBase.Select("cdoRateChangeLetterRequest.LoadIBSGHDVMembers",
                                                                new object[2] { lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.effective_date, busConstant.PlanIdGroupHealth });
                    lobjRateChangeLetterRequest.idtbIBSDentalMembers = busBase.Select("cdoRateChangeLetterRequest.LoadIBSGHDVMembers",
                                                                new object[2] { lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.effective_date, busConstant.PlanIdDental });
                    lobjRateChangeLetterRequest.idtbIBSVisionMembers = busBase.Select("cdoRateChangeLetterRequest.LoadIBSGHDVMembers",
                                                                new object[2] { lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.effective_date, busConstant.PlanIdVision });
                    lobjRateChangeLetterRequest.idtbIBSMedicareMembers = busBase.Select("cdoRateChangeLetterRequest.LoadIBSMedicareMembers",
                                                                new object[1] { lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.effective_date });
                    lobjRateChangeLetterRequest.idtbIBSLifeMembers = busBase.Select("cdoRateChangeLetterRequest.LoadIBSLifeMembers",
                                                                new object[1] { lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.effective_date });
                    lobjRateChangeLetterRequest.idtbCombinedRhicAmount = busBase.Select("cdoRateChangeLetterRequest.LoadCombinedRHICAmount",
                                                                new object[1] { lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.effective_date });
                    idlgUpdateProcessLog("Loading All Retiree Members For The Current Rate Change Request", "INFO", iobjBatchSchedule.step_name);
                    lobjRateChangeLetterRequest.idtbInitialSelectedMembersForRateChange = busBase.Select("cdoRateChangeLetterRequest.LoadIBSRetireeMembers",
                                                                new object[1] { lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.effective_date });

                }
                catch (Exception e)
                {
                    ExceptionManager.Publish(e);
                    idlgUpdateProcessLog("Rate Change Letter Request Failed for the following reason " + e, "ERR", istrProcessName);
                    lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.status_value = busConstant.StatusFailed;
                    lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.Update();
                    return;
                }
                if (lobjRateChangeLetterRequest.idtbInitialSelectedMembersForRateChange.IsNotNull())
                    idlgUpdateProcessLog("Calculation of Current And New Premiums  For The Selected Members, Count :  " + lobjRateChangeLetterRequest.idtbInitialSelectedMembersForRateChange.Rows.Count + 
                        " Started.", "INFO", iobjBatchSchedule.step_name);
                bool lblnInTransaction = false;
                try
                {
                    if (!lblnInTransaction)
                    {
                        iobjPassInfo.BeginTransaction();
                        lblnInTransaction = true;
                    }
                    if (lobjRateChangeLetterRequest.idtbInitialSelectedMembersForRateChange.IsNotNull() && lobjRateChangeLetterRequest.idtbInitialSelectedMembersForRateChange.Rows.Count > 0)
                    {
                        IEnumerable<int> lenuDistinctRatePersons = lobjRateChangeLetterRequest.idtbInitialSelectedMembersForRateChange.AsEnumerable().Select(ldr => ldr.Field<int>("PERSON_ID")).Distinct();
                        if (lenuDistinctRatePersons.IsNotNull() && lenuDistinctRatePersons.Count() > 0)
                        {
                            foreach (int lintRatePerson in lenuDistinctRatePersons)
                            {
                                bool lblnIsPremiumChanged = false;
                                busInsurancePremium lbusInsurancePremium = new busInsurancePremium();
                                cdoRateChangeLetterDetail lcdoRateChangeLetterDetail = new cdoRateChangeLetterDetail();
                                lcdoRateChangeLetterDetail.person_id = lintRatePerson;
                                lcdoRateChangeLetterDetail.batch_request_id = lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.rate_change_letter_request_id;
                                DataRow[] larrHealthRow = lobjRateChangeLetterRequest.idtbIBSHealthMembers.FilterTable(busConstant.DataType.Numeric, "PERSON_ID", lintRatePerson);
                                if (larrHealthRow.IsNotNull() && larrHealthRow.Count() > 0)
                                {

                                    lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.iintPlanId = busConstant.PlanIdGroupHealth;
                                    lbusInsurancePremium = lobjRateChangeLetterRequest.ProcessInsurancePremiumForIBSMember(larrHealthRow.First());
                                    lcdoRateChangeLetterDetail.new_health_prem = lbusInsurancePremium.idecNewPremium;
                                    lcdoRateChangeLetterDetail.curr_health_prem = lbusInsurancePremium.idecCurrentPremium;
                                    if (lcdoRateChangeLetterDetail.new_health_prem != lcdoRateChangeLetterDetail.curr_health_prem)
                                        lblnIsPremiumChanged = true;
                                }
                                DataRow[] larrDentalRow = lobjRateChangeLetterRequest.idtbIBSDentalMembers.FilterTable(busConstant.DataType.Numeric, "PERSON_ID", lintRatePerson);
                                if (larrDentalRow.IsNotNull() && larrDentalRow.Count() > 0)
                                {

                                    lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.iintPlanId = busConstant.PlanIdDental;
                                    lbusInsurancePremium = lobjRateChangeLetterRequest.ProcessInsurancePremiumForIBSMember(larrDentalRow.First());
                                    lcdoRateChangeLetterDetail.new_den_prem = lbusInsurancePremium.idecNewPremium;
                                    lcdoRateChangeLetterDetail.curr_den_prem = lbusInsurancePremium.idecCurrentPremium;
                                    if(lcdoRateChangeLetterDetail.new_den_prem !=lcdoRateChangeLetterDetail.curr_den_prem)
                                        lblnIsPremiumChanged = true;
                                }
                                DataRow[] larrVisionRow = lobjRateChangeLetterRequest.idtbIBSVisionMembers.FilterTable(busConstant.DataType.Numeric, "PERSON_ID", lintRatePerson);
                                if (larrVisionRow.IsNotNull() && larrVisionRow.Count() > 0)
                                {
                                    lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.iintPlanId = busConstant.PlanIdVision;
                                    lbusInsurancePremium = lobjRateChangeLetterRequest.ProcessInsurancePremiumForIBSMember(larrVisionRow.First());
                                    lcdoRateChangeLetterDetail.new_vis_prem = lbusInsurancePremium.idecNewPremium;
                                    lcdoRateChangeLetterDetail.curr_vis_prem = lbusInsurancePremium.idecCurrentPremium;
                                    if(lcdoRateChangeLetterDetail.new_vis_prem != lcdoRateChangeLetterDetail.curr_vis_prem)
                                        lblnIsPremiumChanged = true;
                                }
                                DataRow[] larrMedicareRow = lobjRateChangeLetterRequest.idtbIBSMedicareMembers.FilterTable(busConstant.DataType.Numeric, "MEMBER_PERSON_ID", lintRatePerson);
                                if (larrMedicareRow.IsNotNull() && larrMedicareRow.Count() > 0)
                                {
                                    lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.iintPlanId = busConstant.PlanIdMedicarePartD;
                                    lbusInsurancePremium = lobjRateChangeLetterRequest.ProcessInsurancePremiumForIBSMember(larrMedicareRow.First());
                                    lcdoRateChangeLetterDetail.new_medd_prem = lbusInsurancePremium.idecNewMedicarePartDPremium;
                                    lcdoRateChangeLetterDetail.curr_medd_prem = lbusInsurancePremium.idecCurrentMedicarePartDPremium;
                                    if(lcdoRateChangeLetterDetail.new_medd_prem != lcdoRateChangeLetterDetail.curr_medd_prem)
                                        lblnIsPremiumChanged = true;
                                }
                                DataRow[] larrLifeRow = lobjRateChangeLetterRequest.idtbIBSLifeMembers.FilterTable(busConstant.DataType.Numeric, "PERSON_ID", lintRatePerson);
                                if (larrLifeRow.IsNotNull() && larrLifeRow.Count() > 0)
                                {
                                    lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.iintPlanId = busConstant.PlanIdGroupLife;
                                    lbusInsurancePremium = lobjRateChangeLetterRequest.ProcessInsurancePremiumForIBSMember(larrLifeRow.First());
                                    if (lbusInsurancePremium.iclbCoverageLevelLifePremium.IsNotNull() && lbusInsurancePremium.iclbCoverageLevelLifePremium.Count > 0)
                                    {
                                        foreach (busInsurancePremium lbusInsuranceCoveragePremium in lbusInsurancePremium.iclbCoverageLevelLifePremium)
                                        {
                                            switch (lbusInsuranceCoveragePremium.istrLevelOfCoverageValue)
                                            {
                                                case busConstant.LevelofCoverage_Basic:
                                                    lcdoRateChangeLetterDetail.life_basic_amt = lbusInsuranceCoveragePremium.idecCoverageAmount;
                                                    lcdoRateChangeLetterDetail.new_basic_prem = lbusInsuranceCoveragePremium.idecNewPremium;
                                                    lcdoRateChangeLetterDetail.curr_basic_prem = lbusInsuranceCoveragePremium.idecCurrentPremium;
                                                    if(lcdoRateChangeLetterDetail.new_basic_prem != lcdoRateChangeLetterDetail.curr_basic_prem)
                                                        lblnIsPremiumChanged = true;
                                                    break;
                                                case busConstant.LevelofCoverage_Supplemental:
                                                    lcdoRateChangeLetterDetail.life_supp_amt = lbusInsuranceCoveragePremium.idecCoverageAmount;
                                                    lcdoRateChangeLetterDetail.new_supp_prem = lbusInsuranceCoveragePremium.idecNewPremium;
                                                    lcdoRateChangeLetterDetail.curr_supp_prem = lbusInsuranceCoveragePremium.idecCurrentPremium;
                                                    if(lcdoRateChangeLetterDetail.new_supp_prem !=lcdoRateChangeLetterDetail.curr_supp_prem)
                                                        lblnIsPremiumChanged = true;
                                                    break;
                                                case busConstant.LevelofCoverage_DependentSupplemental:
                                                    lcdoRateChangeLetterDetail.life_dep_supp_amt = lbusInsuranceCoveragePremium.idecCoverageAmount;
                                                    lcdoRateChangeLetterDetail.new_dep_supp_prem = lbusInsuranceCoveragePremium.idecNewPremium;
                                                    lcdoRateChangeLetterDetail.curr_dep_supp_prem = lbusInsuranceCoveragePremium.idecCurrentPremium;
                                                    if(lcdoRateChangeLetterDetail.new_dep_supp_prem != lcdoRateChangeLetterDetail.curr_dep_supp_prem)
                                                        lblnIsPremiumChanged = true;
                                                        break;
                                                case busConstant.LevelofCoverage_SpouseSupplemental:
                                                    lcdoRateChangeLetterDetail.life_sp_supp_amt = lbusInsuranceCoveragePremium.idecCoverageAmount;
                                                    lcdoRateChangeLetterDetail.new_sp_supp_prem = lbusInsuranceCoveragePremium.idecNewPremium;
                                                    lcdoRateChangeLetterDetail.curr_sp_supp_prem = lbusInsuranceCoveragePremium.idecCurrentPremium;
                                                    if(lcdoRateChangeLetterDetail.new_sp_supp_prem != lcdoRateChangeLetterDetail.curr_sp_supp_prem)
                                                        lblnIsPremiumChanged = true;
                                                    break;
                                            }
                                        }
                                    }
                                }
                                DataRow[] larrRHICRow = lobjRateChangeLetterRequest.idtbCombinedRhicAmount.FilterTable(busConstant.DataType.Numeric, "PERSON_ID", lintRatePerson);
                                if (larrRHICRow.IsNotNull() && larrRHICRow.Count() > 0)
                                {
                                    lcdoRateChangeLetterDetail.rhic_amount = Convert.ToDecimal(larrRHICRow.First()["COMBINED_RHIC_AMOUNT"]);
                                }
                                if (lblnIsPremiumChanged == true) // PIR - 17501 As per maik's mail insert all plans for a member,only if premium amount is changed for at least 1 plan
                                  lcdoRateChangeLetterDetail.Insert();
                            }
                        }
                    }
                    if (lblnInTransaction)
                    {
                        iobjPassInfo.Commit();
                        lblnInTransaction = false;
                    }
                }
                catch (Exception e)
                {
                    if (lblnInTransaction)
                    {
                        iobjPassInfo.Rollback();
                        lblnInTransaction = false;
                        //lblnSuccess = false;
                    }
                    ExceptionManager.Publish(e);
                    idlgUpdateProcessLog("Error Occured While Writing Premiums To The Rate Detail Table with Message = " + e.Message, "ERR", istrProcessName);
                    lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.status_value = busConstant.StatusFailed;
                    lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.Update();
                    return;
                }
                idlgUpdateProcessLog("Current And New Premiums For All Selected Members Written To The Rate Detail Table.", "INFO", iobjBatchSchedule.step_name);
                //Read the Correspondance data and generate correspondance logic
                idlgUpdateProcessLog("Generation of Rate Change Letters Started. ", "INFO", iobjBatchSchedule.step_name);
                try
                {
                    DataTable ldtbRateChangeLetters = busBase.Select("cdoRateChangeLetterDetail.LoadRetireeMembersForCorrespondance",
                                                                    new object[1] { lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.rate_change_letter_request_id });
                    foreach (DataRow ldrRateChangedPerson in ldtbRateChangeLetters.Rows)
                    {
                        busRateChangeLetterDetail lbusRateChangeLetterDetail = new busRateChangeLetterDetail
                                                                                {
                                                                                    icdoRateChangeLetterDetail = new cdoRateChangeLetterDetail()
                                                                                    ,
                                                                                    ibusPerson = new busPerson
                                                                                    {
                                                                                        icdoPerson = new cdoPerson(),
                                                                                        ibusPersonCurrentAddress = new busPersonAddress { icdoPersonAddress = new cdoPersonAddress() }
                                                                                    }

                                                                                };

                        lbusRateChangeLetterDetail.ibusRateChangeLetterRequest = lobjRateChangeLetterRequest;
                        lbusRateChangeLetterDetail.ibusRateChangeLetterRequest.icdoRateChangeLetterRequest = lobjRateChangeLetterRequest.icdoRateChangeLetterRequest;
                        lbusRateChangeLetterDetail.icdoRateChangeLetterDetail.LoadData(ldrRateChangedPerson);
                        lbusRateChangeLetterDetail.ibusPerson.icdoPerson.LoadData(ldrRateChangedPerson);
                        lbusRateChangeLetterDetail.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.LoadData(ldrRateChangedPerson);
                        LoadCorrPropertiesForLetterChange(lbusRateChangeLetterDetail);
                        if (lbusRateChangeLetterDetail.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.person_address_id > 0 
                            
                            //&&
                            //((string.IsNullOrEmpty(lbusRateChangeLetterDetail.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.undeliverable_address)) ||
                            //(lbusRateChangeLetterDetail.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.undeliverable_address == busConstant.Flag_No))
                            
                            )
                        {
                            idlgUpdateProcessLog("Generating Rate Change Letter for " + lbusRateChangeLetterDetail.ibusPerson.icdoPerson.FullName, "INFO", iobjBatchSchedule.step_name);
                            GenerateRetireeLetters(lbusRateChangeLetterDetail);
                            lbusRateChangeLetterDetail.icdoRateChangeLetterDetail.letter_generated = busConstant.Flag_Yes;
                            lbusRateChangeLetterDetail.icdoRateChangeLetterDetail.Update();
                        }
                    }
                }
                catch (Exception e)
                {
                    ExceptionManager.Publish(e);
                    idlgUpdateProcessLog("Error Occured with Message = " + e.Message, "ERR", istrProcessName);
                    lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.status_value = busConstant.StatusFailed;
                    lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.Update();
                    return;
                }
                lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.status_value = busConstant.StatusProcessed;
                lobjRateChangeLetterRequest.icdoRateChangeLetterRequest.Update();
            }
            else
            {
                idlgUpdateProcessLog("Batch Ended since no rate change request found with Pending status", "INFO", istrProcessName);
            }
        }

        private void LoadCorrPropertiesForLetterChange(busRateChangeLetterDetail abusRateChangeLetterDetail)
        {
            abusRateChangeLetterDetail.iclbInsuPlansExcLifePremDetails = new Collection<busRateChangeLetterDetail>();
            abusRateChangeLetterDetail.iclbInsuPlansLifePremDetails = new Collection<busRateChangeLetterDetail>();
            if (abusRateChangeLetterDetail.icdoRateChangeLetterDetail.new_health_prem > 0 || abusRateChangeLetterDetail.icdoRateChangeLetterDetail.curr_health_prem > 0)
            {
                LoadCoveragePlanDetails(abusRateChangeLetterDetail, busConstant.PlanHealthName);
            }
            if (abusRateChangeLetterDetail.icdoRateChangeLetterDetail.new_medd_prem > 0 || abusRateChangeLetterDetail.icdoRateChangeLetterDetail.curr_medd_prem > 0)
            {
                LoadCoveragePlanDetails(abusRateChangeLetterDetail, busConstant.PlanMedicareName);
            }
            if (abusRateChangeLetterDetail.icdoRateChangeLetterDetail.new_den_prem > 0 || abusRateChangeLetterDetail.icdoRateChangeLetterDetail.curr_den_prem > 0)
            {
                LoadCoveragePlanDetails(abusRateChangeLetterDetail, busConstant.PlanDentalName);
            }
            if (abusRateChangeLetterDetail.icdoRateChangeLetterDetail.new_vis_prem > 0 || abusRateChangeLetterDetail.icdoRateChangeLetterDetail.curr_vis_prem > 0)
            {
                LoadCoveragePlanDetails(abusRateChangeLetterDetail, busConstant.PlanVisionName);
            }
            if (abusRateChangeLetterDetail.icdoRateChangeLetterDetail.life_basic_amt > 0)
            {
                LoadCoveragePlanDetails(abusRateChangeLetterDetail, busConstant.LifeCoverageBasic);
            }
            if (abusRateChangeLetterDetail.icdoRateChangeLetterDetail.life_supp_amt > 0)
            {
                LoadCoveragePlanDetails(abusRateChangeLetterDetail, busConstant.LifeCoverageSupplemental);
            }
            if (abusRateChangeLetterDetail.icdoRateChangeLetterDetail.life_dep_supp_amt > 0)
            {
                LoadCoveragePlanDetails(abusRateChangeLetterDetail, busConstant.LifeCoverageDepSupplemental);
            }
            if (abusRateChangeLetterDetail.icdoRateChangeLetterDetail.life_sp_supp_amt > 0)
            {
                LoadCoveragePlanDetails(abusRateChangeLetterDetail, busConstant.LifeCoverageSpouseSupplemental);
            }
        }

        private void LoadCoveragePlanDetails(busRateChangeLetterDetail abusRateChangeLetterDetail, string astrCoveragePlan)
        {
            busRateChangeLetterDetail lbusRateChangeLetterDetail = new busRateChangeLetterDetail { icdoRateChangeLetterDetail = new cdoRateChangeLetterDetail() };
            switch (astrCoveragePlan)
            {
                case busConstant.PlanHealthName:
                    lbusRateChangeLetterDetail.istrInsurancePlanName = busConstant.PlanHealthName;
                    lbusRateChangeLetterDetail.idecNewPremium = abusRateChangeLetterDetail.icdoRateChangeLetterDetail.new_health_prem;
                    lbusRateChangeLetterDetail.idecCurrPremium = abusRateChangeLetterDetail.icdoRateChangeLetterDetail.curr_health_prem;
                    abusRateChangeLetterDetail.iclbInsuPlansExcLifePremDetails.Add(lbusRateChangeLetterDetail);
                    break;
                case busConstant.PlanMedicareName:
                    lbusRateChangeLetterDetail.istrInsurancePlanName = busConstant.PlanMedicareName;
                    lbusRateChangeLetterDetail.idecNewPremium = abusRateChangeLetterDetail.icdoRateChangeLetterDetail.new_medd_prem;
                    lbusRateChangeLetterDetail.idecCurrPremium = abusRateChangeLetterDetail.icdoRateChangeLetterDetail.curr_medd_prem;
                    abusRateChangeLetterDetail.iclbInsuPlansExcLifePremDetails.Add(lbusRateChangeLetterDetail);
                    break;
                case busConstant.PlanDentalName:
                    lbusRateChangeLetterDetail.istrInsurancePlanName = busConstant.PlanDentalName;
                    lbusRateChangeLetterDetail.idecNewPremium = abusRateChangeLetterDetail.icdoRateChangeLetterDetail.new_den_prem;
                    lbusRateChangeLetterDetail.idecCurrPremium = abusRateChangeLetterDetail.icdoRateChangeLetterDetail.curr_den_prem;
                    abusRateChangeLetterDetail.iclbInsuPlansExcLifePremDetails.Add(lbusRateChangeLetterDetail);
                    break;
                case busConstant.PlanVisionName:
                    lbusRateChangeLetterDetail.istrInsurancePlanName = busConstant.PlanVisionName;
                    lbusRateChangeLetterDetail.idecNewPremium = abusRateChangeLetterDetail.icdoRateChangeLetterDetail.new_vis_prem;
                    lbusRateChangeLetterDetail.idecCurrPremium = abusRateChangeLetterDetail.icdoRateChangeLetterDetail.curr_vis_prem;
                    abusRateChangeLetterDetail.iclbInsuPlansExcLifePremDetails.Add(lbusRateChangeLetterDetail);
                    break;
                case busConstant.LifeCoverageBasic:
                    lbusRateChangeLetterDetail.istrInsurancePlanName = busConstant.LifeCoverageBasic;
                    lbusRateChangeLetterDetail.idecCoverageAmount = abusRateChangeLetterDetail.icdoRateChangeLetterDetail.life_basic_amt;
                    lbusRateChangeLetterDetail.idecNewPremium = abusRateChangeLetterDetail.icdoRateChangeLetterDetail.new_basic_prem;
                    lbusRateChangeLetterDetail.idecCurrPremium = abusRateChangeLetterDetail.icdoRateChangeLetterDetail.curr_basic_prem;
                    abusRateChangeLetterDetail.iclbInsuPlansLifePremDetails.Add(lbusRateChangeLetterDetail);
                    break;
                case busConstant.LifeCoverageSupplemental:
                    lbusRateChangeLetterDetail.istrInsurancePlanName = busConstant.LifeCoverageSupplemental;
                    lbusRateChangeLetterDetail.idecCoverageAmount = abusRateChangeLetterDetail.icdoRateChangeLetterDetail.life_supp_amt;
                    lbusRateChangeLetterDetail.idecNewPremium = abusRateChangeLetterDetail.icdoRateChangeLetterDetail.new_supp_prem;
                    lbusRateChangeLetterDetail.idecCurrPremium = abusRateChangeLetterDetail.icdoRateChangeLetterDetail.curr_supp_prem;
                    abusRateChangeLetterDetail.iclbInsuPlansLifePremDetails.Add(lbusRateChangeLetterDetail);
                    break;
                case busConstant.LifeCoverageDepSupplemental:
                    lbusRateChangeLetterDetail.istrInsurancePlanName = busConstant.LifeCoverageDepSupplemental;
                    lbusRateChangeLetterDetail.idecCoverageAmount = abusRateChangeLetterDetail.icdoRateChangeLetterDetail.life_dep_supp_amt;
                    lbusRateChangeLetterDetail.idecNewPremium = abusRateChangeLetterDetail.icdoRateChangeLetterDetail.new_dep_supp_prem;
                    lbusRateChangeLetterDetail.idecCurrPremium = abusRateChangeLetterDetail.icdoRateChangeLetterDetail.curr_dep_supp_prem;
                    abusRateChangeLetterDetail.iclbInsuPlansLifePremDetails.Add(lbusRateChangeLetterDetail);
                    break;
                case busConstant.LifeCoverageSpouseSupplemental:
                    lbusRateChangeLetterDetail.istrInsurancePlanName = busConstant.LifeCoverageSpouseSupplemental;
                    lbusRateChangeLetterDetail.idecCoverageAmount = abusRateChangeLetterDetail.icdoRateChangeLetterDetail.life_sp_supp_amt;
                    lbusRateChangeLetterDetail.idecNewPremium = abusRateChangeLetterDetail.icdoRateChangeLetterDetail.new_sp_supp_prem;
                    lbusRateChangeLetterDetail.idecCurrPremium = abusRateChangeLetterDetail.icdoRateChangeLetterDetail.curr_sp_supp_prem;
                    abusRateChangeLetterDetail.iclbInsuPlansLifePremDetails.Add(lbusRateChangeLetterDetail);
                    break;
            }

        }
        private void GenerateRetireeLetters(busRateChangeLetterDetail lbusRateChangeLetterDetail)
        {
            //ArrayList larrlist = new ArrayList();
            //larrlist.Add(lbusRateChangeLetterDetail);
            Hashtable lhstDummyTable = new Hashtable();
            lhstDummyTable.Add("sfwCallingForm", "Batch");
            CreateCorrespondence("PAY-4305", lbusRateChangeLetterDetail, lhstDummyTable);
        }
    }
}
