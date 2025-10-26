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
using System.Globalization;
using System.Collections;
using Sagitec.CorBuilder;
#endregion
namespace NeoSpinBatch
{
    class busMergeEmployerBatch : busNeoSpinBatch
    {
        public busMergeEmployerBatch()
        {
        }
        public void ProcessMerging()
        {
            istrProcessName = "Merge Employer Batch";
            idlgUpdateProcessLog("Merging Employees between employers ", "INFO", istrProcessName);

            busBase lobjBase = new busBase();
            DataTable ldtbHeaderList = busNeoSpinBase.Select<cdoMergeEmployerHeader>(
                new string[2] { "merge_status_value", "status_value" },
                new object[2] { busConstant.EmployerMergeStatusQueued, busConstant.StatusValid }, null, null);
            Collection<busMergeEmployerHeader> lclbMergeEmployerHeader = new Collection<busMergeEmployerHeader>();
            lclbMergeEmployerHeader = lobjBase.GetCollection<busMergeEmployerHeader>(ldtbHeaderList, "icdoMergeEmployerHeader");
            foreach (busMergeEmployerHeader lobjMergeEmployerHeader in lclbMergeEmployerHeader)
            {
                lobjMergeEmployerHeader.LoadFromEmployer();
                lobjMergeEmployerHeader.LoadToEmployer();
                lobjMergeEmployerHeader.LoadFromOrgPlans();
                lobjMergeEmployerHeader.LoadToOrgPlans();
                lobjMergeEmployerHeader.ValidateSoftErrors();
                if (lobjMergeEmployerHeader.ibusSoftErrors.iclbError.Count > 0)
                {
                    idlgUpdateProcessLog("Batch could not complete : Error in Header Record ", "INFO", istrProcessName);
                }
                else
                {
                    lobjMergeEmployerHeader.ibusFromEmployer.LoadPersonEmployment();
                    lobjMergeEmployerHeader.ibusFromEmployer.LoadOrgPlan();
                    idlgUpdateProcessLog("Merging Employees", "INFO", istrProcessName);
                    foreach (busPersonEmployment lobjPersonEmployment in lobjMergeEmployerHeader.ibusFromEmployer.iclbPersonEmployment)
                    {                      
                        MergeEmployer(lobjMergeEmployerHeader, lobjPersonEmployment);
                    }
                    lobjMergeEmployerHeader.ibusFromEmployer.icdoOrganization.status_value = busConstant.OrganizationStatusMerged;
                    lobjMergeEmployerHeader.ibusFromEmployer.icdoOrganization.Update();
                    lobjMergeEmployerHeader.icdoMergeEmployerHeader.merge_status_value = busConstant.EmployerMergeStatusCompleted;
                    lobjMergeEmployerHeader.icdoMergeEmployerHeader.Update();
                    lobjMergeEmployerHeader.icdoMergeEmployerHeader.Select();
                    lobjMergeEmployerHeader.UpdateValidateStatus();
                    idlgUpdateProcessLog("Merge Completed ", "INFO", istrProcessName);
                }
            }
        }
        public void MergeEmployer(busMergeEmployerHeader lobjMergeEmployerHeader, busPersonEmployment lobjPersonEmployment)
        {           
            if (lobjPersonEmployment.icdoPersonEmployment.end_date == DateTime.MinValue)
            {
                busMergeEmployerDetail lobjMergeEmployerDetail = new busMergeEmployerDetail();
                lobjMergeEmployerDetail.icdoMergeEmployerDetail = new cdoMergeEmployerDetail();               
                lobjMergeEmployerDetail.icdoMergeEmployerDetail.merge_employer_header_id = lobjMergeEmployerHeader.icdoMergeEmployerHeader.merge_employer_header_id;
                lobjMergeEmployerDetail.icdoMergeEmployerDetail.person_id = lobjPersonEmployment.icdoPersonEmployment.person_id;
                lobjMergeEmployerDetail.icdoMergeEmployerDetail.Insert();
                lobjMergeEmployerDetail.LoadPerson();
                lobjMergeEmployerDetail.LoadMergeEmployerHeader();
                lobjMergeEmployerDetail.ValidateSoftErrors();
                lobjMergeEmployerDetail.UpdateValidateStatus();
                if (lobjMergeEmployerDetail.ibusSoftErrors.iclbError.Count > 0)
                {
                    idlgUpdateProcessLog("Error in Merging PERSLinkID" + 
                        lobjMergeEmployerDetail.icdoMergeEmployerDetail.person_id.ToString(), "INFO", istrProcessName);                 
                }
                else
                {
                    lobjPersonEmployment.icdoPersonEmployment.end_date = lobjMergeEmployerHeader.icdoMergeEmployerHeader.effective_date.AddDays(-1);
                    lobjPersonEmployment.icdoPersonEmployment.Update();
                    cdoPersonEmployment lcdoPersonEmployment = new cdoPersonEmployment();
                    lcdoPersonEmployment.person_id = lobjPersonEmployment.icdoPersonEmployment.person_id;
                    lcdoPersonEmployment.org_id = lobjMergeEmployerHeader.icdoMergeEmployerHeader.to_employer_id;
                    lcdoPersonEmployment.start_date = lobjMergeEmployerHeader.icdoMergeEmployerHeader.effective_date;
                    lcdoPersonEmployment.Insert();
                    lobjPersonEmployment.LoadPersonEmploymentDetail();
                    foreach (busPersonEmploymentDetail lobjPersonEmploymentDetail in lobjPersonEmployment.icolPersonEmploymentDetail)
                    {
                        if (lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date == DateTime.MinValue)
                        {
                            lobjPersonEmploymentDetail.LoadAllPersonAccountEmploymentDetails();
                            lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date = lobjMergeEmployerHeader.icdoMergeEmployerHeader.effective_date.AddDays(-1);
                            lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.Update();
                            cdoPersonEmploymentDetail lcdoPersonEmploymentDetail = new cdoPersonEmploymentDetail();
                            lcdoPersonEmploymentDetail.person_employment_id = lcdoPersonEmployment.person_employment_id;
                            lcdoPersonEmploymentDetail.start_date = lobjMergeEmployerHeader.icdoMergeEmployerHeader.effective_date;
                            lcdoPersonEmploymentDetail.derived_member_type_value = lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.derived_member_type_value;
                            lcdoPersonEmploymentDetail.hourly_value = lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.hourly_value;
                            lcdoPersonEmploymentDetail.seasonal_value = lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.seasonal_value;
                            lcdoPersonEmploymentDetail.type_value = lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value;
                            lcdoPersonEmploymentDetail.job_class_value = lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.job_class_value;
                            lcdoPersonEmploymentDetail.term_begin_date = lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.term_begin_date;
                            lcdoPersonEmploymentDetail.official_list_value = lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.official_list_value;
                            lcdoPersonEmploymentDetail.recertified_date = lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.recertified_date;
                            lcdoPersonEmploymentDetail.status_value = lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.status_value;
                            lcdoPersonEmploymentDetail.recertified_batch_run_flag = lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.recertified_batch_run_flag;
                            lcdoPersonEmploymentDetail.Insert();
                            foreach (busPersonAccountEmploymentDetail lobjPAEmploymentDetail in lobjPersonEmploymentDetail.iclbAllPersonAccountEmpDtl)
                            {
                                lobjPAEmploymentDetail.icdoPersonAccountEmploymentDetail.person_employment_dtl_id = lcdoPersonEmploymentDetail.person_employment_dtl_id;
                                lobjPAEmploymentDetail.icdoPersonAccountEmploymentDetail.Insert();
                            }
                        }
                    }
                }
            }
        }               
    }
}