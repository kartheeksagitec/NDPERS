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
using System.Text.RegularExpressions;

#endregion

namespace NeoSpin.BusinessObjects
{
    public class busReloadInsuranceBatch : busExtendBase
    {
        public void DeletePayrollDetails(busEmployerPayrollHeader aobjPayrollHeader)
        {
            //PROD PIR 933
            DBFunction.DBNonQuery("cdoEmployerPayrollHeader.DeletePersonAccountDependentLinkTable",
                            new object[1] { aobjPayrollHeader.icdoEmployerPayrollHeader.employer_payroll_header_id },
                            iobjPassInfo.iconFramework,
                            iobjPassInfo.itrnFramework);

            // Delete all the Payroll Detail for the Header.
            DBFunction.DBNonQuery("cdoEmployerPayrollHeader.DeleteErrorsFromEmployerPayrollDetail",
                            new object[1] { aobjPayrollHeader.icdoEmployerPayrollHeader.employer_payroll_header_id },
                            iobjPassInfo.iconFramework,
                            iobjPassInfo.itrnFramework);

            DBFunction.DBNonQuery("cdoEmployerPayrollHeader.DeletePayrollDetails",
                            new object[1] { aobjPayrollHeader.icdoEmployerPayrollHeader.employer_payroll_header_id },
                            iobjPassInfo.iconFramework,
                            iobjPassInfo.itrnFramework);            
        }

        public void DeleteRemittanceAllocation(busEmployerPayrollHeader aobjPayrollHeader)
        {
            // Delete all the Payroll Detail for the Header.
            DBFunction.DBNonQuery("cdoEmployerPayrollHeader.DeleteAllocatedRemittanceByHeaderID",
                            new object[1] { aobjPayrollHeader.icdoEmployerPayrollHeader.employer_payroll_header_id },
                            iobjPassInfo.iconFramework,
                            iobjPassInfo.itrnFramework);

            if (aobjPayrollHeader.icdoEmployerPayrollHeader.balancing_status_value != busConstant.PayrollHeaderBalancingStatusNoRemittance)
            {
                aobjPayrollHeader.icdoEmployerPayrollHeader.balancing_status_value = busConstant.PayrollHeaderBalancingStatusNoRemittance;
                aobjPayrollHeader.icdoEmployerPayrollHeader.Update();
            }
        }

        //Satya Comment : Always set it to Ready to Post When it comes from Reload Insurance Batch
        public void OverrideHeaderStatus(busEmployerPayrollHeader aobjPayrollHeader, bool ablnIsFromReloadInsuranceBatch = false)
        {
            //Overriding the Status that are set by Framework for the specific Business rules         
            //called only for insurance, so if anyone detail record is valid change header to Valid  
            if (aobjPayrollHeader.icdoEmployerPayrollHeader.report_type_value == busConstant.PayrollHeaderReportTypeRegular)
            {
                if (aobjPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeInsr)
                {
                    if (ablnIsFromReloadInsuranceBatch)
                    {
                        aobjPayrollHeader.icdoEmployerPayrollHeader.status_value = busConstant.PayrollHeaderStatusReadyToPost;
                    }
                    //commented as per satya, we need not change the status of header if getting called from btn_Reload button.
                    //change status only from batch and if clicked reload, user has to click accept button from ESS or save button from PERSLink to make it valid
                    //else
                    //{
                    //    bool lblnReviewPendingRecordsFound = false;
                    //    foreach (busEmployerPayrollDetail lobjEmployerReportDetail in aobjPayrollHeader.iclbEmployerPayrollDetail)
                    //    {
                    //        if ((lobjEmployerReportDetail.icdoEmployerPayrollDetail.status_value == busConstant.PayrollDetailStatusReview) ||
                    //            (lobjEmployerReportDetail.icdoEmployerPayrollDetail.status_value == busConstant.PayrollDetailStatusPending))
                    //        {
                    //            lblnReviewPendingRecordsFound = true;
                    //            break;
                    //        }
                    //    }
                    //    if ((!lblnReviewPendingRecordsFound) && (aobjPayrollHeader.iclbEmployerPayrollDetail.Count > 0))
                    //    {
                    //        aobjPayrollHeader.icdoEmployerPayrollHeader.status_value = busConstant.PayrollHeaderStatusReadyToPost;
                    //    }
                    //}
                }
            }
        }
    }
}
