// -----------------------------------------------------------------------
// <copyright file="busDropDependentCOBRANoticeLetterBatch.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
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
namespace NeoSpinBatch
{
    public class busDropDependentCOBRANoticeLetterBatch : busNeoSpinBatch
    {
        private DateTime _idtPlanEffectiveDate;
        public DateTime idtPlanEffectiveDate
        {
            get
            {
                return _idtPlanEffectiveDate;
            }
            set
            {
                _idtPlanEffectiveDate = value;
            }
        }
        public DataTable idtbCachedDentalRate { get; set; }
        public DataTable idtbCachedVisionRate { get; set; }
        
        
        public void GenerateCorrepondenceForDropDependent()
        
        {
            /// PIR 20584 - Generate one Correspondence For one Dependent and all Enrolled plans listed in the PLANNAME bookmark
            istrProcessName = "Drop Dependent COBRA Notice Letter Batch";
            idlgUpdateProcessLog("Loading all Persons Plans which are end dated", "INFO", istrProcessName);
            DataTable ldtbGetAllDropDependent = busBase.Select("cdoPersonAccountDependent.GenerateCorrespondenceForDropDependent", new object[] { });
            IList<DataTable> ldtbGetAllDropDependentGroup = ldtbGetAllDropDependent.AsEnumerable().GroupBy(row => new
            {
                PersonDependentId = row.Field<int>("PersonDependentId")
            }).Select(g => g.CopyToDataTable()).ToList();
            // This Loop for distinct PERSON_DEPENDENT_ID
            foreach (DataTable dt in ldtbGetAllDropDependentGroup)
            {
                idlgUpdateProcessLog("Processing Persons who's Dependents have ended", "INFO", istrProcessName);
                busPersonDependent lobjPersonDependent = new busPersonDependent { icdoPersonDependent = new cdoPersonDependent() };
                busPersonAccountDependent lobjPersonAccountDependent = new busPersonAccountDependent { icdoPersonAccountDependent = new cdoPersonAccountDependent ()};
               
                lobjPersonDependent.FindPersonDependent(Convert.ToInt32(dt.Rows[0]["PersonDependentId"]));
                lobjPersonAccountDependent.icdoPersonDependent = lobjPersonDependent.icdoPersonDependent;
                lobjPersonDependent.LoadDependentInfo();
                lobjPersonAccountDependent.icdoPersonDependent.dependent_name = lobjPersonDependent.icdoPersonDependent.dependent_name;
                lobjPersonAccountDependent.icdoPersonDependent.first_name = lobjPersonDependent.icdoPersonDependent.first_name;
                string lstrPlanNames = string.Empty;
                if (lobjPersonAccountDependent.iclbCorPersonAccountDependent == null)
                    lobjPersonAccountDependent.iclbCorPersonAccountDependent = new Collection<busPersonAccountDependent>();
                // This Loop for PERSON_DEPENDENT - Plans and other details
                foreach (DataRow dr in dt.Rows)
                {
                    lobjPersonAccountDependent.ibusPersonAccount = new busPersonAccount();
                    lobjPersonAccountDependent.FindPersonAccountDependent(Convert.ToInt32(dr["PersonAccountDependentId"]));
                    lobjPersonAccountDependent.ibusPersonAccount.FindPersonAccount(Convert.ToInt32(dr["PERSON_ACCOUNT_ID"]));
                    lobjPersonAccountDependent.ibusPersonAccount.LoadPlan();
                    lstrPlanNames += lobjPersonAccountDependent.ibusPersonAccount.ibusPlan.icdoPlan.istr_PLAN_NAME + "/";


                    lobjPersonAccountDependent.ibusPersonAccount.LoadPersonAccountGHDV();
                lobjPersonAccountDependent.ibusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccount = lobjPersonAccountDependent.ibusPersonAccount.icdoPersonAccount;
                lobjPersonAccountDependent.ibusPersonAccount.icdoPersonAccount.person_employment_dtl_id= lobjPersonAccountDependent.ibusPersonAccount.GetEmploymentDetailID();
                lobjPersonAccountDependent.ibusPersonAccount.ibusPersonAccountGHDV.LoadPlan();
                lobjPersonAccountDependent.ibusPersonAccount.ibusPersonAccountGHDV.LoadOrgPlan();
                lobjPersonAccountDependent.ibusPersonAccount.ibusPersonAccountGHDV.LoadProviderOrgPlan();
                                
                switch (lobjPersonAccountDependent.ibusPersonAccount.ibusPlan.icdoPlan.plan_id)
               {        
                    case busConstant.PlanIdGroupHealth:
                    case busConstant.PlanIdMedicarePartD:
                    case busConstant.PlanIdVision:
                    case busConstant.PlanIdDental:                    
                       lobjPersonAccountDependent.ibusPersonAccount.ibusPersonAccountGHDV.InitializeObjects();
                       lobjPersonAccountDependent.ibusPersonAccount.ibusPersonAccountGHDV.LoadPlanEffectiveDate();
                       lobjPersonAccountDependent.ibusPersonAccount.ibusPersonAccountGHDV.DetermineEnrollmentAndLoadObjects(lobjPersonAccountDependent.ibusPersonAccount.ibusPersonAccountGHDV.idtPlanEffectiveDate, false);
                       //PIR 6918
                       if (lobjPersonAccountDependent.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDental)
                       {
                           lobjPersonAccountDependent.ibusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.dental_insurance_type_value = busConstant.DentalInsuranceTypeCOBRA;
                           lobjPersonAccountDependent.ibusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.level_of_coverage_value = busConstant.DentalLevelofCoverageIndividual; 
                       }
                       else if (lobjPersonAccountDependent.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdVision)
                       {
                           lobjPersonAccountDependent.ibusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.vision_insurance_type_value = busConstant.VisionInsuranceTypeCOBRA;
                           lobjPersonAccountDependent.ibusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.level_of_coverage_value = busConstant.VisionLevelofCoverageIndividual;
                       }
                       //PIR 6918 End

                       if (lobjPersonAccountDependent.ibusPersonAccount.ibusPersonAccountGHDV.IsHealthOrMedicare)
                        {
                            //PIR 6918  Coverage Code is set as Single 
                            lobjPersonAccountDependent.ibusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.coverage_code = "0004";                            

                            lobjPersonAccountDependent.ibusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.cobra_type_value = busConstant.COBRAType18Month;
                            //PIR 6918 End
                            
                            if (lobjPersonAccountDependent.ibusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.overridden_structure_code.IsNotNullOrEmpty())
                            {
                                lobjPersonAccountDependent.ibusPersonAccount.ibusPersonAccountGHDV.LoadRateStructureForUserStructureCode();
                            }
                            else
                            {
                                //Load the Health Plan Participation Date (based on effective Date)
                                lobjPersonAccountDependent.ibusPersonAccount.ibusPersonAccountGHDV.LoadHealthParticipationDate();
                                //To Get the Rate Structure Code (Derived Field)
                                lobjPersonAccountDependent.ibusPersonAccount.ibusPersonAccountGHDV.LoadRateStructure();
                            }

                            //Get the Coverage Ref ID
                            lobjPersonAccountDependent.ibusPersonAccount.ibusPersonAccountGHDV.LoadCoverageRefID();

                            lobjPersonAccountDependent.ibusPersonAccount.ibusPersonAccountGHDV.GetMonthlyPremiumAmountByRefID();
                        }
                        else
                        {

                            lobjPersonAccountDependent.ibusPersonAccount.ibusPersonAccountGHDV.LoadPlanEffectiveDate();
                            if (lobjPersonAccountDependent.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDental)
                            {
                                lobjPersonAccountDependent.ibusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.MonthlyPremiumAmount=busRateHelper.GetDentalPremiumAmount(lobjPersonAccountDependent.ibusPersonAccount.ibusPersonAccountGHDV.ibusProviderOrgPlan.icdoOrgPlan.org_plan_id,
                                lobjPersonAccountDependent.ibusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.dental_insurance_type_value,
                                lobjPersonAccountDependent.ibusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.level_of_coverage_value,
                                lobjPersonAccountDependent.ibusPersonAccount.ibusPersonAccountGHDV.idtPlanEffectiveDate, idtbCachedDentalRate, iobjPassInfo);
                            }
                            else if (lobjPersonAccountDependent.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdVision)
                            {
                                lobjPersonAccountDependent.ibusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.MonthlyPremiumAmount = busRateHelper.GetVisionPremiumAmount(lobjPersonAccountDependent.ibusPersonAccount.ibusPersonAccountGHDV.ibusProviderOrgPlan.icdoOrgPlan.org_plan_id,
                                lobjPersonAccountDependent.ibusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.vision_insurance_type_value,
                                lobjPersonAccountDependent.ibusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.level_of_coverage_value,
                                lobjPersonAccountDependent.ibusPersonAccount.ibusPersonAccountGHDV.idtPlanEffectiveDate, idtbCachedVisionRate, iobjPassInfo);
                            }
                        }
                            lobjPersonAccountDependent.icdoPersonAccountDependent.Total_Premium_Amount += lobjPersonAccountDependent.ibusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.MonthlyPremiumAmount.RoundToTwoDecimalPoints();   //Rounded the value to two Decimal's                     
                            busPersonAccountDependent lTempCorPersonAccountDependent = new busPersonAccountDependent { icdoPersonAccountDependent = new cdoPersonAccountDependent() };
                            lTempCorPersonAccountDependent.idecMonthlyPremiumAmount = lobjPersonAccountDependent.ibusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.MonthlyPremiumAmount.RoundToTwoDecimalPoints();   //Rounded the value to two Decimal's                     
                            lTempCorPersonAccountDependent.istrCorPlanName = lobjPersonAccountDependent.ibusPersonAccount.ibusPlan.icdoPlan.plan_name;
                            lobjPersonAccountDependent.iclbCorPersonAccountDependent.Add(lTempCorPersonAccountDependent);
                            break;
                }

                lobjPersonAccountDependent.icdoPersonAccountDependent.is_drop_dependent_letter_sent_flag = busConstant.Flag_Yes;
                lobjPersonAccountDependent.icdoPersonAccountDependent.Update();
              }
                lobjPersonAccountDependent.istrAllPlanNames = lstrPlanNames.TrimEnd('/');
                CreateCorrespondence(lobjPersonAccountDependent);
            }
        }

        private void CreateCorrespondence(busPersonAccountDependent lbusPersonAccountDependent)
        {
            //ArrayList larrlist = new ArrayList();
            //larrlist.Add(lbusPersonAccountDependent);
            Hashtable lhstDummyTable = new Hashtable();
            lhstDummyTable.Add("sfwCallingForm", "Batch");
            CreateCorrespondence("ENR-5450", lbusPersonAccountDependent, lhstDummyTable);
        }
    }
}
