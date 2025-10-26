using System;
using System.Collections.Generic;
using System.Text;
using NeoSpin.BusinessObjects;
using Sagitec.BusinessObjects;
using System.Collections;
using System.Collections.ObjectModel;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.CorBuilder;
using Sagitec.DataObjects;
using System.Data;
using Sagitec.CustomDataObjects;
using NeoSpin.CustomDataObjects;


namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPersonAccountTFFRFileOut : busFileBaseOut
    {
        private Collection<busPersonAccountTFFRFile> _iclbTFFR;
        public Collection<busPersonAccountTFFRFile> iclbTFFR
        {
            get { return _iclbTFFR; }
            set { _iclbTFFR = value; }
        }

        private busPersonAccountTFFRFile _ibusTFFRFile;
        public busPersonAccountTFFRFile ibusTFFRFile
        {
            get { return _ibusTFFRFile; }
            set { _ibusTFFRFile = value; }
        }

        public override void InitializeFile()
        {
            istrFileName = "TFFR_RIO_Benefits" + busConstant.FileFormatcsv;
        }

        public busDBCacheData ibusDBCacheData { get; set; }

        public void LoadTFFR(DataTable ldtbTFFR)
        {
            //Loading the DB Cache Data
            LoadDBCacheData();

            _iclbTFFR = new Collection<busPersonAccountTFFRFile>();
            // Get Modified GHDV Member accounts where Payment mode is TFFR. 
            ldtbTFFR = busBase.Select("cdoPersonAccountGhdv.NDPERS_TFFR_Enrollment", new object[] { });
            foreach (DataRow dr in ldtbTFFR.Rows)
            {
                busPersonAccountGhdv lobjGHDV = new busPersonAccountGhdv();
                lobjGHDV.icdoPersonAccount = new cdoPersonAccount();
                lobjGHDV.icdoPersonAccountGhdv = new cdoPersonAccountGhdv();
                lobjGHDV.ibusPerson = new busPerson();
                lobjGHDV.ibusPerson.icdoPerson = new cdoPerson();
                lobjGHDV.ibusPlan = new busPlan();
                lobjGHDV.ibusPlan.icdoPlan = new cdoPlan();
                lobjGHDV.icdoPersonAccount.LoadData(dr);
                lobjGHDV.icdoPersonAccountGhdv.LoadData(dr);
                lobjGHDV.ibusPerson.icdoPerson.LoadData(dr);
                lobjGHDV.ibusPlan.icdoPlan.LoadData(dr);

                lobjGHDV.idtbCachedCoverageRef = ibusDBCacheData.idtbCachedCoverageRef;
                lobjGHDV.idtbCachedDentalRate = ibusDBCacheData.idtbCachedDentalRate;
                lobjGHDV.idtbCachedHealthRate = ibusDBCacheData.idtbCachedHealthRate;
                lobjGHDV.idtbCachedHmoRate = ibusDBCacheData.idtbCachedHMORate;
                lobjGHDV.idtbCachedRateRef = ibusDBCacheData.idtbCachedRateRef;
                lobjGHDV.idtbCachedRateStructureRef = ibusDBCacheData.idtbCachedRateStructureRef;
                lobjGHDV.idtbCachedVisionRate = ibusDBCacheData.idtbCachedVisionRate;

                // Passing the same object to the LoadCoverages method will update the previous history coverage to the current object by reference.
                // In order to avoid the wrong update a copy of the same object is created and passed.
                // Mail with subject: Rate Structure 12 Vs Rate Structure 11
                // *****Starts here *****//
                busPersonAccountGhdv lobjGHDVCopy = new busPersonAccountGhdv
                {
                    icdoPersonAccount = new cdoPersonAccount(),
                    icdoPersonAccountGhdv = new cdoPersonAccountGhdv(),
                    ibusPerson = new busPerson { icdoPerson = new cdoPerson() },
                    ibusPlan = new busPlan { icdoPlan = new cdoPlan() }
                };
                lobjGHDVCopy.icdoPersonAccount.LoadData(dr);
                lobjGHDVCopy.icdoPersonAccountGhdv.LoadData(dr);
                lobjGHDVCopy.ibusPerson.icdoPerson.LoadData(dr);
                lobjGHDVCopy.ibusPlan.icdoPlan.LoadData(dr);
                lobjGHDVCopy.idtbCachedCoverageRef = ibusDBCacheData.idtbCachedCoverageRef;
                lobjGHDVCopy.idtbCachedDentalRate = ibusDBCacheData.idtbCachedDentalRate;
                lobjGHDVCopy.idtbCachedHealthRate = ibusDBCacheData.idtbCachedHealthRate;
                lobjGHDVCopy.idtbCachedHmoRate = ibusDBCacheData.idtbCachedHMORate;
                lobjGHDVCopy.idtbCachedRateRef = ibusDBCacheData.idtbCachedRateRef;
                lobjGHDVCopy.idtbCachedRateStructureRef = ibusDBCacheData.idtbCachedRateStructureRef;
                lobjGHDVCopy.idtbCachedVisionRate = ibusDBCacheData.idtbCachedVisionRate;
                // *****Ends here *****//

                _ibusTFFRFile = new busPersonAccountTFFRFile();
                _ibusTFFRFile.ssn = lobjGHDV.ibusPerson.icdoPerson.ssn;
                _ibusTFFRFile.last_name = lobjGHDV.ibusPerson.icdoPerson.last_name;
                _ibusTFFRFile.first_name = lobjGHDV.ibusPerson.icdoPerson.first_name;
                _ibusTFFRFile.insurance_type = lobjGHDV.ibusPlan.icdoPlan.istr_PLAN_NAME;

                // Load the GHDV history
                lobjGHDV.LoadPersonAccountGHDVHistory();
                if (lobjGHDV.iclbPersonAccountGHDVHistory.Count > 0)
                {
                    if (lobjGHDV.iclbPersonAccountGHDVHistory.Count == 1)
                    {
                        // Just enrolled, no modification occurred ie. No old coverage info.
                        LoadCoverages(lobjGHDV.iclbPersonAccountGHDVHistory[0], lobjGHDVCopy, true);
                    }
                    else if (lobjGHDV.iclbPersonAccountGHDVHistory.Count > 1)
                    {
                        busPersonAccountGhdvHistory lobjNewGHDVHistory = lobjGHDV.iclbPersonAccountGHDVHistory[0];
                        busPersonAccountGhdvHistory lobjOldGHDVHistory = lobjGHDV.iclbPersonAccountGHDVHistory[1];
                        // Currently Cancelled or Suspended but future Enrolled Records
                        if ((lobjNewGHDVHistory.icdoPersonAccountGhdvHistory.plan_participation_status_value ==
                                                busConstant.PlanParticipationStatusInsuranceCancelled) ||
                            (lobjNewGHDVHistory.icdoPersonAccountGhdvHistory.plan_participation_status_value ==
                                                busConstant.PlanParticipationStatusInsuranceSuspended))
                        {
                            if ((lobjNewGHDVHistory.icdoPersonAccountGhdvHistory.start_date > DateTime.Now) &&
                                (lobjOldGHDVHistory.icdoPersonAccountGhdvHistory.plan_participation_status_value ==
                                                busConstant.PlanParticipationStatusInsuranceEnrolled))
                            {
                                // Still enrolled actively in Plan so load only New Info.
                                LoadCoverages(lobjOldGHDVHistory, lobjGHDVCopy, true);
                            }
                        }
                        else if (lobjNewGHDVHistory.icdoPersonAccountGhdvHistory.plan_participation_status_value ==
                                                busConstant.PlanParticipationStatusInsuranceEnrolled)
                        {
                            LoadCoverages(lobjNewGHDVHistory, lobjGHDVCopy, true);
                            LoadCoverages(lobjOldGHDVHistory, lobjGHDVCopy, false);
                        }
                    }
                    _ibusTFFRFile.enrollment_change_reason = lobjGHDV.icdoPersonAccountGhdv.reason_description;
                    _ibusTFFRFile.effective_start_date = lobjGHDV.iclbPersonAccountGHDVHistory[0].icdoPersonAccountGhdvHistory.start_date;
                    _ibusTFFRFile.effective_end_date = lobjGHDV.iclbPersonAccountGHDVHistory[0].icdoPersonAccountGhdvHistory.end_date;
                }
                // Update the GHDV object
                lobjGHDV.icdoPersonAccountGhdv.modified_after_tffr_file_sent_flag = busConstant.Flag_No;
                lobjGHDV.icdoPersonAccountGhdv.Update();

                _iclbTFFR.Add(_ibusTFFRFile);
            }

            // Get Modified Life Member accounts where Payment mode is TFFR.
            ldtbTFFR = busBase.Select("cdoPersonAccountLife.NDPERS_TFFR_Enrollment", new object[] { });
            foreach (DataRow dr in ldtbTFFR.Rows)
            {
                busPersonAccountLife lobjLife = new busPersonAccountLife();
                lobjLife.icdoPersonAccount = new cdoPersonAccount();
                lobjLife.icdoPersonAccountLife = new cdoPersonAccountLife();
                lobjLife.ibusPerson = new busPerson();
                lobjLife.ibusPerson.icdoPerson = new cdoPerson();
                lobjLife.ibusPlan = new busPlan();
                lobjLife.ibusPlan.icdoPlan = new cdoPlan();
                lobjLife.icdoPersonAccount.LoadData(dr);
                lobjLife.icdoPersonAccountLife.LoadData(dr);
                lobjLife.ibusPerson.icdoPerson.LoadData(dr);
                lobjLife.ibusPlan.icdoPlan.LoadData(dr);
                lobjLife.idtbCachedLifeRate = ibusDBCacheData.idtbCachedLifeRate;
                lobjLife.LoadMemberAge();
                lobjLife.LoadPreviousHistory();
                decimal ldecEmpPremimumAmt = 0;
                decimal ldecADAndDBasicRate = 0.0000M;
                decimal ldecADAndDSupplementalRate = 0.0000M;
                foreach (busPersonAccountLifeHistory lobjLifeHistory in lobjLife.iclbPreviousHistory)
                {
                    _ibusTFFRFile = new busPersonAccountTFFRFile();
                    _ibusTFFRFile.ssn = lobjLife.ibusPerson.icdoPerson.ssn;
                    _ibusTFFRFile.last_name = lobjLife.ibusPerson.icdoPerson.last_name;
                    _ibusTFFRFile.first_name = lobjLife.ibusPerson.icdoPerson.first_name;
                    _ibusTFFRFile.insurance_type = lobjLife.ibusPlan.icdoPlan.istr_PLAN_NAME;
                    // Check Previous History
                    DataTable ldtbResult = busBase.SelectWithOperator<cdoPersonAccountLifeHistory>(
                                        new string[3] { "person_account_id", "level_of_coverage_value", "person_account_life_history_id" },
                                        new string[3] { "=", "=", "<" },
                                        new object[3] { lobjLife.icdoPersonAccount.person_account_id,
                                                        lobjLifeHistory.icdoPersonAccountLifeHistory.level_of_coverage_value,
                                                        lobjLifeHistory.icdoPersonAccountLifeHistory.person_account_life_history_id },
                                                        "person_account_life_history_id desc");
                    if (ldtbResult.Rows.Count > 0)
                    {
                        // Load Old Premium amount        
                        busPersonAccountLifeHistory lobjPreviousHistory = new busPersonAccountLifeHistory();
                        lobjPreviousHistory.icdoPersonAccountLifeHistory = new cdoPersonAccountLifeHistory();
                        lobjPreviousHistory.icdoPersonAccountLifeHistory.LoadData(ldtbResult.Rows[0]);
                        DateTime ldteNewPlanEffectiveDate = lobjLife.LoadPlanEffectiveDate(lobjPreviousHistory);
                        //Loading the Provider Org Plan                        
                        lobjLife.LoadActiveProviderOrgPlan(ldteNewPlanEffectiveDate);
                        _ibusTFFRFile.old_coverage_description = lobjPreviousHistory.icdoPersonAccountLifeHistory.level_of_coverage_description;
                        _ibusTFFRFile.old_premium_amount = busRateHelper.GetLifePremiumAmount(
                                                           lobjPreviousHistory.icdoPersonAccountLifeHistory.life_insurance_type_value,
                                                           lobjPreviousHistory.icdoPersonAccountLifeHistory.level_of_coverage_value,
                                                           lobjPreviousHistory.icdoPersonAccountLifeHistory.coverage_amount,
                                                           lobjLife.icdoPersonAccountLife.Life_Insurance_Age,
                                                           lobjLife.ibusProviderOrgPlan.icdoOrgPlan.org_plan_id,
                                                           ldteNewPlanEffectiveDate,
                                                           ref ldecEmpPremimumAmt,
                                                           ibusDBCacheData.idtbCachedLifeRate,
                                                           iobjPassInfo,
                                                           ref ldecADAndDBasicRate, ref ldecADAndDSupplementalRate);
                        _ibusTFFRFile.old_premium_amount += ldecEmpPremimumAmt; // UAT PIR ID 1081
                    }
                    DateTime ldtePlanEffectiveDate = lobjLife.LoadPlanEffectiveDate(lobjLifeHistory);
                    lobjLife.LoadActiveProviderOrgPlan(ldtePlanEffectiveDate);
                    _ibusTFFRFile.new_coverage_description = lobjLifeHistory.icdoPersonAccountLifeHistory.level_of_coverage_description;
                    _ibusTFFRFile.new_premium_amount = busRateHelper.GetLifePremiumAmount(lobjLifeHistory.icdoPersonAccountLifeHistory.life_insurance_type_value,
                                                           lobjLifeHistory.icdoPersonAccountLifeHistory.level_of_coverage_value,
                                                           lobjLifeHistory.icdoPersonAccountLifeHistory.coverage_amount,
                                                           lobjLife.icdoPersonAccountLife.Life_Insurance_Age,
                                                           lobjLife.ibusProviderOrgPlan.icdoOrgPlan.org_plan_id,
                                                           ldtePlanEffectiveDate,
                                                           ref ldecEmpPremimumAmt,
                                                           ibusDBCacheData.idtbCachedLifeRate,
                                                           iobjPassInfo,
                                                           ref ldecADAndDBasicRate, ref ldecADAndDSupplementalRate);
                    _ibusTFFRFile.new_premium_amount += ldecEmpPremimumAmt; // UAT PIR ID 1081
                    _ibusTFFRFile.effective_start_date = lobjLifeHistory.icdoPersonAccountLifeHistory.effective_start_date;
                    _ibusTFFRFile.effective_end_date = lobjLifeHistory.icdoPersonAccountLifeHistory.effective_end_date;
                    _iclbTFFR.Add(_ibusTFFRFile);
                }
                // Update the Life object
                lobjLife.icdoPersonAccountLife.modified_after_tffr_file_sent_flag = busConstant.Flag_No;
                lobjLife.icdoPersonAccountLife.Update();
            }
            
            
            ldtbTFFR = busBase.Select("cdoPersonAccountMedicarePartDHistory.NDPERS_TFFR_Enrollment", new object[] { });
            foreach (DataRow dr in ldtbTFFR.Rows)
            {
                busPersonAccountMedicarePartDHistory lobjMedicare = new busPersonAccountMedicarePartDHistory();
                lobjMedicare.icdoPersonAccount = new cdoPersonAccount();
                lobjMedicare.icdoPersonAccountMedicarePartDHistory = new cdoPersonAccountMedicarePartDHistory();
                lobjMedicare.ibusPerson = new busPerson();
                lobjMedicare.ibusPerson.icdoPerson = new cdoPerson();
                lobjMedicare.ibusPlan = new busPlan();
                lobjMedicare.ibusPlan.icdoPlan = new cdoPlan();
                lobjMedicare.icdoPersonAccount.LoadData(dr);
                lobjMedicare.icdoPersonAccountMedicarePartDHistory.LoadData(dr);
                lobjMedicare.ibusPerson.icdoPerson.LoadData(dr);
                lobjMedicare.ibusPlan.icdoPlan.LoadData(dr);

                lobjMedicare.idtbCachedHealthRate = ibusDBCacheData.idtbCachedMedicarePartDRate;

                _ibusTFFRFile = new busPersonAccountTFFRFile();
                _ibusTFFRFile.ssn = lobjMedicare.ibusPerson.icdoPerson.ssn;
                _ibusTFFRFile.last_name = lobjMedicare.ibusPerson.icdoPerson.last_name;
                _ibusTFFRFile.first_name = lobjMedicare.ibusPerson.icdoPerson.first_name;
                _ibusTFFRFile.insurance_type = lobjMedicare.ibusPlan.icdoPlan.istr_PLAN_NAME;


                lobjMedicare.LoadPersonAccountMedicarePartDHistory(lobjMedicare.icdoPersonAccountMedicarePartDHistory.person_id);
                foreach (busPersonAccountMedicarePartDHistory lobj in lobjMedicare.iclbPersonAccountMedicarePartDHistory)
                {
                    lobj.FindPersonAccount(lobj.icdoPersonAccountMedicarePartDHistory.person_account_id);
                }
                if (lobjMedicare.iclbPersonAccountMedicarePartDHistory.Count > 0)
                {
                    if (lobjMedicare.iclbPersonAccountMedicarePartDHistory.Count == 1)
                    {
                        // Just enrolled, no modification occurred ie. No old coverage info.
                        LoadOldAndNewPremiumAmountForMedicare(lobjMedicare.iclbPersonAccountMedicarePartDHistory[0], true);
                    }
                    else if (lobjMedicare.iclbPersonAccountMedicarePartDHistory.Count > 1)
                    {
                        busPersonAccountMedicarePartDHistory lobjNewMedicareHistory = lobjMedicare.iclbPersonAccountMedicarePartDHistory[0];
                        busPersonAccountMedicarePartDHistory lobjOldMedicareHistory = lobjMedicare.iclbPersonAccountMedicarePartDHistory[1];
                        
                        // Currently Cancelled or Suspended but future Enrolled Records
                        if ((lobjNewMedicareHistory.icdoPersonAccountMedicarePartDHistory.plan_participation_status_value ==
                                                busConstant.PlanParticipationStatusInsuranceCancelled) ||
                            (lobjNewMedicareHistory.icdoPersonAccountMedicarePartDHistory.plan_participation_status_value ==
                                                busConstant.PlanParticipationStatusInsuranceSuspended))
                        {
                            if ((lobjNewMedicareHistory.icdoPersonAccountMedicarePartDHistory.start_date > DateTime.Now) &&
                                (lobjOldMedicareHistory.icdoPersonAccountMedicarePartDHistory.plan_participation_status_value ==
                                                busConstant.PlanParticipationStatusInsuranceEnrolled))
                            {
                                // Still enrolled actively in Plan so load only New Info.
                                LoadOldAndNewPremiumAmountForMedicare(lobjOldMedicareHistory, true);
                            }
                        }
                        else if (lobjNewMedicareHistory.icdoPersonAccountMedicarePartDHistory.plan_participation_status_value ==
                                                busConstant.PlanParticipationStatusInsuranceEnrolled)
                        {
                            LoadOldAndNewPremiumAmountForMedicare(lobjNewMedicareHistory, true);
                            LoadOldAndNewPremiumAmountForMedicare(lobjOldMedicareHistory, false);
                        }
                    }
                    _ibusTFFRFile.enrollment_change_reason = lobjMedicare.iclbPersonAccountMedicarePartDHistory[0].icdoPersonAccountMedicarePartDHistory.reason_description;
                    _ibusTFFRFile.effective_start_date = lobjMedicare.iclbPersonAccountMedicarePartDHistory[0].icdoPersonAccountMedicarePartDHistory.start_date;
                    _ibusTFFRFile.effective_end_date = lobjMedicare.iclbPersonAccountMedicarePartDHistory[0].icdoPersonAccountMedicarePartDHistory.end_date;
                }

                lobjMedicare.icdoPersonAccountMedicarePartDHistory.modified_after_tffr_file_sent_flag = busConstant.Flag_No;
                lobjMedicare.icdoPersonAccountMedicarePartDHistory.Update();
                _iclbTFFR.Add(_ibusTFFRFile);
            }
        }

        private void LoadCoverages(busPersonAccountGhdvHistory aobjGHDVHistory, busPersonAccountGhdv aobjGHDV, bool iblnIsNewCoverage)
        {
            decimal amt = 0.0M;
            busPersonAccountGhdv lobjGhdv = new busPersonAccountGhdv();
            lobjGhdv = aobjGHDV;
            if ((aobjGHDV.icdoPersonAccount.plan_id == busConstant.PlanIdVision) ||
                (aobjGHDV.icdoPersonAccount.plan_id == busConstant.PlanIdDental))
            {
                lobjGhdv.LoadActiveProviderOrgPlan(aobjGHDVHistory.icdoPersonAccountGhdvHistory.start_date);
            }
            if (iblnIsNewCoverage)
            {
                // Load the New Coverage Values and Premium amount.
                switch (aobjGHDV.ibusPlan.icdoPlan.plan_id)
                {
                    case busConstant.PlanIdGroupHealth:
                        _ibusTFFRFile.new_coverage_code = aobjGHDVHistory.icdoPersonAccountGhdvHistory.coverage_code;
                        LoadGHDVPremiumAmount(aobjGHDVHistory, lobjGhdv);
                        _ibusTFFRFile.new_premium_amount = lobjGhdv.icdoPersonAccountGhdv.PremiumExcludingFeeAmount;
                        _ibusTFFRFile.new_rate_structure = lobjGhdv.icdoPersonAccountGhdv.rate_structure_code;
                        break;
                    case busConstant.PlanIdDental:
                        _ibusTFFRFile.new_premium_amount =
                            busRateHelper.GetDentalPremiumAmount(lobjGhdv.ibusProviderOrgPlan.icdoOrgPlan.org_plan_id,
                            aobjGHDVHistory.icdoPersonAccountGhdvHistory.dental_insurance_type_value,
                            aobjGHDVHistory.icdoPersonAccountGhdvHistory.level_of_coverage_value,
                            aobjGHDVHistory.icdoPersonAccountGhdvHistory.start_date, lobjGhdv.idtbCachedDentalRate, iobjPassInfo);
                        _ibusTFFRFile.new_coverage_description = aobjGHDVHistory.icdoPersonAccountGhdvHistory.level_of_coverage_description;
                        break;
                    case busConstant.PlanIdVision:
                        _ibusTFFRFile.new_premium_amount =
                            busRateHelper.GetVisionPremiumAmount(lobjGhdv.ibusProviderOrgPlan.icdoOrgPlan.org_plan_id,
                            aobjGHDVHistory.icdoPersonAccountGhdvHistory.vision_insurance_type_value,
                            aobjGHDVHistory.icdoPersonAccountGhdvHistory.level_of_coverage_value,
                            aobjGHDVHistory.icdoPersonAccountGhdvHistory.start_date, lobjGhdv.idtbCachedVisionRate, iobjPassInfo);
                        _ibusTFFRFile.new_coverage_description = aobjGHDVHistory.icdoPersonAccountGhdvHistory.level_of_coverage_description;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                // Load the Old Coverage Values and Premium amount.
                switch (aobjGHDV.ibusPlan.icdoPlan.plan_id)
                {
                    case busConstant.PlanIdGroupHealth:
                        _ibusTFFRFile.old_coverage_code = aobjGHDVHistory.icdoPersonAccountGhdvHistory.coverage_code;
                        LoadGHDVPremiumAmount(aobjGHDVHistory, lobjGhdv);
                        _ibusTFFRFile.old_premium_amount = lobjGhdv.icdoPersonAccountGhdv.PremiumExcludingFeeAmount;
                        _ibusTFFRFile.old_rate_structure = lobjGhdv.icdoPersonAccountGhdv.rate_structure_code;
                        break;
                    case busConstant.PlanIdDental:
                        _ibusTFFRFile.old_premium_amount =
                            busRateHelper.GetDentalPremiumAmount(lobjGhdv.ibusProviderOrgPlan.icdoOrgPlan.org_plan_id,
                            aobjGHDVHistory.icdoPersonAccountGhdvHistory.dental_insurance_type_value,
                            aobjGHDVHistory.icdoPersonAccountGhdvHistory.level_of_coverage_value,
                            aobjGHDVHistory.icdoPersonAccountGhdvHistory.start_date, lobjGhdv.idtbCachedDentalRate, iobjPassInfo);
                        _ibusTFFRFile.old_coverage_description = aobjGHDVHistory.icdoPersonAccountGhdvHistory.level_of_coverage_description;
                        break;
                    case busConstant.PlanIdVision:
                        _ibusTFFRFile.old_premium_amount =
                            busRateHelper.GetVisionPremiumAmount(lobjGhdv.ibusProviderOrgPlan.icdoOrgPlan.org_plan_id,
                            aobjGHDVHistory.icdoPersonAccountGhdvHistory.vision_insurance_type_value,
                            aobjGHDVHistory.icdoPersonAccountGhdvHistory.level_of_coverage_value,
                            aobjGHDVHistory.icdoPersonAccountGhdvHistory.start_date, lobjGhdv.idtbCachedVisionRate, iobjPassInfo);
                        _ibusTFFRFile.old_coverage_description = aobjGHDVHistory.icdoPersonAccountGhdvHistory.level_of_coverage_description;
                        break;
                    default:
                        break;
                }
            }
        }

        private void LoadOldAndNewPremiumAmountForMedicare(busPersonAccountMedicarePartDHistory aobjMedicareHistory, bool iblnIsNewCoverage)
        {
            if (iblnIsNewCoverage)
            {
                LoadMedicarePremiumAmount(aobjMedicareHistory);
                _ibusTFFRFile.new_premium_amount = aobjMedicareHistory.icdoPersonAccountMedicarePartDHistory.MonthlyPremiumAmount;
            }
            else
            {
                LoadMedicarePremiumAmount(aobjMedicareHistory);
                _ibusTFFRFile.old_premium_amount = aobjMedicareHistory.icdoPersonAccountMedicarePartDHistory.MonthlyPremiumAmount;
                
            }
        }

        private void LoadGHDVPremiumAmount(busPersonAccountGhdvHistory abusPersonAccountGhdvHistory, busPersonAccountGhdv abusPersonAccountGhdv)
        {
            abusPersonAccountGhdv = abusPersonAccountGhdvHistory.LoadGHDVObject(abusPersonAccountGhdv);
            abusPersonAccountGhdv.LoadPerson();
            abusPersonAccountGhdv.LoadPlan();
            //Initialize the Org Object to Avoid the NULL error
            abusPersonAccountGhdv.InitializeObjects();

            abusPersonAccountGhdv.idtPlanEffectiveDate = abusPersonAccountGhdvHistory.icdoPersonAccountGhdvHistory.start_date;
            abusPersonAccountGhdv.DetermineEnrollmentAndLoadObjects(abusPersonAccountGhdv.idtPlanEffectiveDate, false);

            if (abusPersonAccountGhdv.icdoPersonAccountGhdv.overridden_structure_code.IsNotNullOrEmpty())
            {
                abusPersonAccountGhdv.LoadRateStructureForUserStructureCode();
            }
            else
            {
                //Load the Health Plan Participation Date (based on effective Date)
                abusPersonAccountGhdv.LoadHealthParticipationDate();

                //To Get the Rate Structure Code (Derived Field)
                abusPersonAccountGhdv.LoadRateStructure();
            }

            //Get the Coverage Ref ID
            abusPersonAccountGhdv.LoadCoverageRefID();

            //Get the Premium Amount
            abusPersonAccountGhdv.GetMonthlyPremiumAmountByRefID();
        }

        private void LoadMedicarePremiumAmount(busPersonAccountMedicarePartDHistory abusPersonAccountMedicarePartDHistory)
        {
            abusPersonAccountMedicarePartDHistory.GetMonthlyPremiumAmountForMedicarePartD();
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
            ibusDBCacheData.idtbCachedVisionRate = busGlobalFunctions.LoadVisionRateCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedMedicarePartDRate = busGlobalFunctions.LoadMedicarePartDRateCacheData(iobjPassInfo);
        }
    }
}
