using NeoSpin.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using NeoSpin.CustomDataObjects;

namespace NeoSpinBatch
{
    class busValidateEmployerPayrollDetailBatch : busNeoSpinBatch
    {

        public busValidateEmployerPayrollDetailBatch()
        {

        }
        public void ValidateDetails()
        {
            idlgUpdateProcessLog("Loading Payroll Headers With Review Details for validation.",
                        "INFO", iobjBatchSchedule.step_name);
            DataTable ldtPayrollHeadersWithReviewDetails = busNeoSpinBase.Select("cdoEmployerPayrollDetail.LoadPayrollHeadersWithReviewDetails", new object[0] { });
            if (ldtPayrollHeadersWithReviewDetails.Rows.Count > 0)
            {
                foreach (DataRow ldrow in ldtPayrollHeadersWithReviewDetails.Rows)
                {
                    busEmployerPayrollHeader lbusEmployerPayrollHeader = new busEmployerPayrollHeader { icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader(), ibusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() } };
                    lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.LoadData(ldrow);
                    lbusEmployerPayrollHeader.ibusOrganization.icdoOrganization.LoadData(ldrow);
                    lbusEmployerPayrollHeader.LoadEmployerPayrollDetailWithOtherObjects();
                    lbusEmployerPayrollHeader.LoadAllPersonAccounts();
                    lbusEmployerPayrollHeader.LoadAllPAEmpDetailWithChildData();
                    lbusEmployerPayrollHeader.LoadAllPlanMemberTypeCrossRef();
                    lbusEmployerPayrollHeader.LoadAllOrgPlans();
                    lbusEmployerPayrollHeader.LoadAllOrgPlanMemberType();
                    try
                    {
                        lbusEmployerPayrollHeader.ValidateReviewDetails();
                        idlgUpdateProcessLog("Validation process completed for Header ID " + lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.employer_payroll_header_id.ToString(), "INFO", iobjBatchSchedule.step_name);

                    }
                    catch (Exception ex)
                    {
                        idlgUpdateProcessLog("Exception occurred while validating Header ID " + lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.employer_payroll_header_id.ToString() + ", Exception : " + ex.Message, "ERR", iobjBatchSchedule.step_name);
                    }
                    lbusEmployerPayrollHeader = null;
                }
            }
            else
            {
                idlgUpdateProcessLog("No Payroll Headers To Validate.",
                            "INFO", iobjBatchSchedule.step_name);
            }
        }
    }
}
