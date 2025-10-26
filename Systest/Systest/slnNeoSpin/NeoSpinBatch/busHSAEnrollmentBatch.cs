using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using NeoSpin.BusinessObjects;
using Sagitec.BusinessObjects;
using System.Collections.ObjectModel;
using System.Collections;
using Sagitec.CustomDataObjects;
using NeoSpin.CustomDataObjects;
using Sagitec.Common;
namespace NeoSpinBatch
{
    class busHSAEnrollmentBatch : busNeoSpinBatch
    {
        public Collection<busPersonAccountGhdv> iclbPersonAccountGHDV;
        public DateTime idtBatchRunDate { get; set; }
        public void CreateHSAEnrollmentFile()
        {
            idtBatchRunDate = iobjSystemManagement.icdoSystemManagement.batch_date;
            DataTable ldtbHSAEnrollment = busBase.Select("cdoPersonAccountGhdv.fleHSAEnrollmentFile", new object[1] { idtBatchRunDate });
            iclbPersonAccountGHDV = new Collection<busPersonAccountGhdv>();
            foreach (DataRow dr in ldtbHSAEnrollment.Rows)
            {
                busPersonAccountGhdv lobjPersonAccountGHDV = new busPersonAccountGhdv
                {
                    icdoPersonAccount = new cdoPersonAccount(),
                    icdoPersonAccountGhdv = new cdoPersonAccountGhdv(),
                    ibusPerson = new busPerson { icdoPerson = new cdoPerson() },
                    ibusPersonEmploymentDetail = new busPersonEmploymentDetail
                    {
                        icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail(),
                        ibusPersonEmployment = new busPersonEmployment { icdoPersonEmployment = new cdoPersonEmployment() }
                    }
                };

                lobjPersonAccountGHDV.icdoPersonAccount.LoadData(dr);
                lobjPersonAccountGHDV.icdoPersonAccountGhdv.LoadData(dr);                
                
                lobjPersonAccountGHDV.LoadPerson();
                lobjPersonAccountGHDV.istrHSATerminationDate = FindHSATerminationDate(lobjPersonAccountGHDV) == DateTime.MinValue ?
                    string.Empty    :   (FindHSATerminationDate(lobjPersonAccountGHDV)).ToString(busConstant.DateFormatMMddyyyy);
                lobjPersonAccountGHDV.ibusPerson.LoadPersonCurrentAddress();
                lobjPersonAccountGHDV.LoadAllPersonEmploymentDetails(false);

                foreach (busPersonEmploymentDetail lobjEmpDtl in lobjPersonAccountGHDV.iclbEmploymentDetail)
                    lobjEmpDtl.LoadPersonEmployment();

                if (lobjPersonAccountGHDV.iclbEmploymentDetail.Count > 0)
                {
                    //Set the Start Date as Least Employment Start Date
                    int lintLastEntry = lobjPersonAccountGHDV.iclbEmploymentDetail.Count - 1;
                    lobjPersonAccountGHDV.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.start_date =
                            lobjPersonAccountGHDV.iclbEmploymentDetail[lintLastEntry].ibusPersonEmployment.icdoPersonEmployment.start_date;

                    //Set the End Date as Top Employment End Date
                    lobjPersonAccountGHDV.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.end_date =
                            lobjPersonAccountGHDV.iclbEmploymentDetail[0].ibusPersonEmployment.icdoPersonEmployment.end_date;
                    // PIR 9370
                    int calenderYear = lobjPersonAccountGHDV.iclbEmploymentDetail[0].ibusPersonEmployment.icdoPersonEmployment.end_date.Year;
                    DateTime currentDate = DateTime.Today;
                    int currentCalenderYear = currentDate.Year;
                    if ((dr["PLAN_PARTICIPATION_STATUS_VALUE"].ToString() == busConstant.PlanParticipationStatusInsuranceSuspended)
                        && (calenderYear == currentCalenderYear))
                    {
                        lobjPersonAccountGHDV.LoadPreviousEmploymentDetail();
                        lobjPersonAccountGHDV.ibusPreviousEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.end_date =
                                lobjPersonAccountGHDV.iclbEmploymentDetail[0].ibusPersonEmployment.icdoPersonEmployment.end_date;
                    }
                }
                lobjPersonAccountGHDV.istrCoverageCodeDescription = FindCoverageCodeValue(lobjPersonAccountGHDV);

                //PIR 12122 - Records with no current address are ignored and message is posted in process log.
                if (lobjPersonAccountGHDV.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.person_address_id > 0)
                {
                    iclbPersonAccountGHDV.Add(lobjPersonAccountGHDV);
                    istrProcessName = "HSA Enrollment - Outbound File";
                    idlgUpdateProcessLog("Enrolling Person ID " + lobjPersonAccountGHDV.ibusPerson.icdoPerson.person_id + " ", "INFO", istrProcessName);
                }
                else
                {
                    lobjPersonAccountGHDV.ibusPerson.LoadAddresses();
                    busPersonAddress lobjPersonAddress = lobjPersonAccountGHDV.ibusPerson.icolPersonAddress.OrderByDescending(o => o.icdoPersonAddress.start_date).FirstOrDefault();
                    if (lobjPersonAddress != null)
                    {
                        lobjPersonAccountGHDV.ibusPerson.ibusPersonCurrentAddress = lobjPersonAddress;
                        lobjPersonAccountGHDV.ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress = lobjPersonAddress.icdoPersonAddress;
                        iclbPersonAccountGHDV.Add(lobjPersonAccountGHDV);
                    }
                    else
                    {
                        lobjPersonAccountGHDV.ibusPerson.ibusPersonCurrentAddress = new busPersonAddress();
                        lobjPersonAccountGHDV.ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress = new cdoPersonAddress();
                        iclbPersonAccountGHDV.Add(lobjPersonAccountGHDV);
                    }

                    istrProcessName = "HSA Enrollment - Outbound File";
                    idlgUpdateProcessLog("Person " + lobjPersonAccountGHDV.ibusPerson.icdoPerson.person_id + " does not have address as of today", "ERR", istrProcessName);
                }
            }
            if (iclbPersonAccountGHDV != null && iclbPersonAccountGHDV.Count > 0)
            {
                busProcessOutboundFile lobjProcessFiles = new busProcessOutboundFile();
                lobjProcessFiles.iarrParameters = new object[2];
                lobjProcessFiles.iarrParameters[0] = iclbPersonAccountGHDV;
                lobjProcessFiles.iarrParameters[1] = iobjSystemManagement.icdoSystemManagement.batch_date.AddDays(1);
                lobjProcessFiles.CreateOutboundFile(90);
            }
         }

        private DateTime FindHSATerminationDate(busPersonAccountGhdv aobjPersonAccountGHDV)
        {
            DateTime ldtHSATerminationDate = DateTime.MinValue;
            aobjPersonAccountGHDV.LoadPersonAccountGHDVHistory();
            if (aobjPersonAccountGHDV.iclbPersonAccountGHDVHistory.IsNotNull())
            {
                var lenumLatestHistory = aobjPersonAccountGHDV.iclbPersonAccountGHDVHistory.Where(i => (i.icdoPersonAccountGhdvHistory.start_date!= i.icdoPersonAccountGhdvHistory.end_date ) &&
                   i.icdoPersonAccountGhdvHistory.alternate_structure_code_value == busConstant.AlternateStructureCodeHDHP
                    && i.icdoPersonAccountGhdvHistory.end_date == DateTime.MinValue && i.icdoPersonAccountGhdvHistory.plan_participation_status_value=="ENL2"
                    ).OrderByDescending(i => i.icdoPersonAccountGhdvHistory.person_account_ghdv_history_id).FirstOrDefault();
                if (lenumLatestHistory.IsNotNull())
                {                    
                    if (!(aobjPersonAccountGHDV.icdoPersonAccountGhdv.cobra_type_value.IsNotNull()
                        && aobjPersonAccountGHDV.icdoPersonAccountGhdv.alternate_structure_code_value == busConstant.AlternateStructureCodeHDHP)) // PIR 10913
                    {
                        //Means there is an Active HDHP History record
                        lenumLatestHistory.icdoPersonAccountGhdvHistory.hsa_enrollment_flag = busConstant.Flag_Yes;
                        lenumLatestHistory.icdoPersonAccountGhdvHistory.Update();
                        return ldtHSATerminationDate;
                    }
                }
                var lenumHistoryByYear = aobjPersonAccountGHDV.iclbPersonAccountGHDVHistory.Where(i => (i.icdoPersonAccountGhdvHistory.start_date!= i.icdoPersonAccountGhdvHistory.end_date) 
                  ).OrderBy(i => i.icdoPersonAccountGhdvHistory.person_account_ghdv_history_id);
                foreach (busPersonAccountGhdvHistory lobjHistory in lenumHistoryByYear)
                {
                    if (lobjHistory.icdoPersonAccountGhdvHistory.alternate_structure_code_value == busConstant.AlternateStructureCodeHDHP
                        && (lobjHistory.icdoPersonAccountGhdvHistory.end_date != DateTime.MinValue && lobjHistory.icdoPersonAccountGhdvHistory.end_date.AddMonths(1).Year == idtBatchRunDate.Year) 
                        && lobjHistory.icdoPersonAccountGhdvHistory.start_date != lobjHistory.icdoPersonAccountGhdvHistory.end_date)
                    {
                        ldtHSATerminationDate = lobjHistory.icdoPersonAccountGhdvHistory.end_date;
                        ldtHSATerminationDate = ldtHSATerminationDate.AddMonths(1);
                        lobjHistory.icdoPersonAccountGhdvHistory.hsa_enrollment_flag = busConstant.Flag_Yes;
                        lobjHistory.icdoPersonAccountGhdvHistory.Update();
                    }
                }
            }
            return ldtHSATerminationDate;
        }      
 
        private string FindCoverageCodeValue(busPersonAccountGhdv aobjPersonAccountGhdv)
        {
            if (aobjPersonAccountGhdv.idtbCachedRateRef == null)
                aobjPersonAccountGhdv.idtbCachedRateRef = busGlobalFunctions.LoadHealthRateRefCacheData(iobjPassInfo);

            if (aobjPersonAccountGhdv.idtbCachedCoverageRef == null)
                aobjPersonAccountGhdv.idtbCachedCoverageRef = busGlobalFunctions.LoadHealthCoverageRefCacheData(iobjPassInfo);

            Collection<cdoOrgPlanGroupHealthMedicarePartDRateRef> lclbRateRef = new Collection<cdoOrgPlanGroupHealthMedicarePartDRateRef>();
            lclbRateRef = Sagitec.DataObjects.doBase.GetCollection<cdoOrgPlanGroupHealthMedicarePartDRateRef>(aobjPersonAccountGhdv.idtbCachedRateRef);

            Collection<cdoOrgPlanGroupHealthMedicarePartDCoverageRef> lclbCoverageRef = new Collection<cdoOrgPlanGroupHealthMedicarePartDCoverageRef>();
            lclbCoverageRef = Sagitec.DataObjects.doBase.GetCollection<cdoOrgPlanGroupHealthMedicarePartDCoverageRef>(aobjPersonAccountGhdv.idtbCachedCoverageRef);

            var lclbList = lclbCoverageRef.Join(lclbRateRef,
                                                c => c.org_plan_group_health_medicare_part_d_rate_ref_id,
                                                r => r.org_plan_group_health_medicare_part_d_rate_ref_id,
                                                (c, r) => new
                                                {
                                                    c.coverage_code,
                                                    c.short_description,
                                                    c.org_plan_group_health_medicare_part_d_rate_ref_id,
                                                    c.employment_type_value,
                                                    c.cobra_in,
                                                    r.rate_structure_code,
                                                    r.health_insurance_type_value,
                                                    r.rate_structure_value,
                                                    r.plan_option_value,
                                                    r.wellness,
                                                    r.low_income
                                                }).Where( c=> c.coverage_code == aobjPersonAccountGhdv.icdoPersonAccountGhdv.coverage_code);

            //Filter based on the values           
            if (aobjPersonAccountGhdv.icdoPersonAccountGhdv.Rate_Ref_ID > 0)
                lclbList = lclbList.Where(o => o.org_plan_group_health_medicare_part_d_rate_ref_id == aobjPersonAccountGhdv.icdoPersonAccountGhdv.Rate_Ref_ID);

            if (aobjPersonAccountGhdv.icdoPersonAccountGhdv.employment_type_value != null)
                lclbList = lclbList.Where(o => o.employment_type_value == aobjPersonAccountGhdv.icdoPersonAccountGhdv.employment_type_value);

            lclbList = lclbList.Where(o => o.cobra_in == aobjPersonAccountGhdv.COBRAIn);

            if (aobjPersonAccountGhdv.icdoPersonAccountGhdv.overridden_structure_code.IsNullOrEmpty())
            {
                if (aobjPersonAccountGhdv.icdoPersonAccountGhdv.rate_structure_code.IsNotNullOrEmpty())
                    lclbList = lclbList.Where(o => o.rate_structure_code == aobjPersonAccountGhdv.icdoPersonAccountGhdv.rate_structure_code);

                if (aobjPersonAccountGhdv.icdoPersonAccountGhdv.health_insurance_type_value != null)
                    lclbList = lclbList.Where(o => o.health_insurance_type_value == aobjPersonAccountGhdv.icdoPersonAccountGhdv.health_insurance_type_value);

                if (aobjPersonAccountGhdv.icdoPersonAccountGhdv.rate_structure_value.IsNotNullOrEmpty())
                    lclbList = lclbList.Where(o => o.rate_structure_value == aobjPersonAccountGhdv.icdoPersonAccountGhdv.rate_structure_value);

                lclbList = lclbList.Where(o => o.plan_option_value == aobjPersonAccountGhdv.icdoPersonAccountGhdv.plan_option_value);

                lclbList = lclbList.Where(o => o.wellness == aobjPersonAccountGhdv.WellnessFlag);

                //PIR 11192
                if (aobjPersonAccountGhdv.istrLowIncomeCreditFlag == busConstant.Flag_Yes)
                    lclbList = lclbList.Where(o => o.low_income == "0");
                else
                    lclbList = lclbList.Where(o => o.low_income == aobjPersonAccountGhdv.LowIncome);
            }

            return lclbList.First().short_description;

        }
    }
}
