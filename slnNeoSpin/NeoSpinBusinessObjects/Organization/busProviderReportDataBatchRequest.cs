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
using System.Linq;
using System.Linq.Expressions;
#endregion

namespace NeoSpin.BusinessObjects
{
	[Serializable]
	public class busProviderReportDataBatchRequest : busProviderReportDataBatchRequestGen
	{
        // Plan Name to be displayed in Lookup Screen.
        private busPlan _ibusPlan;
        public busPlan ibusPlan
        {
            get { return _ibusPlan; }
            set { _ibusPlan = value; }
        }

        // Org Code ID to be displayed in Lookup Screen.
        private busOrganization _ibusOrganization;
        public busOrganization ibusOrganization
        {
            get { return _ibusOrganization; }
            set { _ibusOrganization = value; }
        }
        //Load Organization details by org code in new mode
        public void LoadOrganizationByOrgCode()
        {
            if (ibusOrganization == null)
                ibusOrganization = new busOrganization();
            ibusOrganization.FindOrganizationByOrgCode(icdoProviderReportDataBatchRequest.org_code);
        }
        //Load Organization details by org code in update mode
        public void LoadOrganizationByOrgId()
        {
            if (ibusOrganization == null)
                ibusOrganization = new busOrganization();
            ibusOrganization.FindOrganization(icdoProviderReportDataBatchRequest.org_id);
        }
        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            //Load Organization details by org code in new mode
            if (icdoProviderReportDataBatchRequest.org_code != null)
            {
                if (ibusOrganization == null)
                    LoadOrganizationByOrgCode();
                if (icdoProviderReportDataBatchRequest.org_id == 0)
                {
                    icdoProviderReportDataBatchRequest.org_id = ibusOrganization.icdoOrganization.org_id;
                }
            }
            icdoProviderReportDataBatchRequest.status_value = busConstant.Vendor_Payment_Status_NotProcessed;
            icdoProviderReportDataBatchRequest.visibility_flag = busConstant.Flag_Yes;
            base.BeforeValidate(aenmPageMode);
        }

        //PROD PIR 7974
        public bool IsReloadInsuranceBatchRunForCurrentRequest()
        {
            DataTable ldtReloadInsuranceBatchSchedule = busBase.Select<cdoBatchSchedule>(new string[1] { "batch_schedule_id" }, new object[1] { 15 }, null, null);
            DataTable ldtDisburseFundsBatchSchedule = busBase.Select<cdoBatchSchedule>(new string[1] { "batch_schedule_id" }, new object[1] { 16 }, null, null);
            if (ldtReloadInsuranceBatchSchedule.Rows.Count > 0 && ldtDisburseFundsBatchSchedule.Rows.Count > 0)
            {
                DateTime ldteReloadBatchNextRunDate = Convert.ToDateTime(ldtReloadInsuranceBatchSchedule.Rows[0]["next_run_date"]);
                String lstrReloadBatchActiveFlag = ldtReloadInsuranceBatchSchedule.Rows[0]["active_flag"].ToString();
                DateTime ldteDisburseFundsBatchNextRunDate = Convert.ToDateTime(ldtDisburseFundsBatchSchedule.Rows[0]["next_run_date"]);
                if (ldteReloadBatchNextRunDate >= DateTime.Now.GetFirstDayofNextMonth() ||
                    (ldteReloadBatchNextRunDate == busGlobalFunctions.GetSysManagementBatchDate() && lstrReloadBatchActiveFlag == busConstant.Flag_Yes) ||
                    (ldteReloadBatchNextRunDate == ldteDisburseFundsBatchNextRunDate && lstrReloadBatchActiveFlag == busConstant.Flag_Yes))
                {
                    return true;
                }
            }
            return false;
        }
        //Check whether selected org is vendor or provider
        public bool IsSelectedOrgvendorOrProvider()
        {
            if (ibusOrganization.icdoOrganization.org_type_value == busConstant.OrgTypeProvider 
                || ibusOrganization.icdoOrganization.org_type_value == busConstant.OrgTypeVendor)
            {
                return true;
            }
            return false;
        }
        //Get state tax Vendor Org Code for ucs -075 validations
        public bool IsSelectedOrgvendor()
        {
            DataTable ldtbSTVendor = iobjPassInfo.isrvDBCache.GetCodeDescription(busConstant.SystemConstantCodeID, busConstant.SystemConstantStateTaxVendor);
            string lstrVedorOrgCode = ldtbSTVendor.Rows.Count > 0 ? ldtbSTVendor.Rows[0]["data1"].ToString() : string.Empty;
            if (lstrVedorOrgCode == icdoProviderReportDataBatchRequest.org_code)
            {
                return true;
            }
            return false;
        }
        //if the select org is state tax vendor ,then there should be only one record for the given org,plan,effective start date and effective end date
        //if the select org is not state tax vendor ,then there should be only one record for the given org,plan,effective start date and effective end date is not required
        public bool IsRecordExist()
        {
            DataTable ldtbPaymentRecords = busBase.Select("cdoProviderReportDataBatchRequest.CheckRecordExistsForSameParameters", 
                new object[2] {icdoProviderReportDataBatchRequest.org_id,icdoProviderReportDataBatchRequest.plan_id });
            Collection<busProviderReportDataBatchRequest> lclbRequest =GetCollection<busProviderReportDataBatchRequest>(ldtbPaymentRecords,"icdoProviderReportDataBatchRequest");
            if (IsSelectedOrgvendor() && lclbRequest.Count > 0)
            {
                if (lclbRequest.Where(o => busGlobalFunctions.CheckDateOverlapping(icdoProviderReportDataBatchRequest.effective_start_date,
                    icdoProviderReportDataBatchRequest.effective_end_date,
                    o.icdoProviderReportDataBatchRequest.effective_start_date,
                    o.icdoProviderReportDataBatchRequest.effective_end_date) &&
                    icdoProviderReportDataBatchRequest.provider_report_data_batch_request_id !=
                    o.icdoProviderReportDataBatchRequest.provider_report_data_batch_request_id).Count() > 0)
                {
                    return true;
                }
            }
            else if (!IsSelectedOrgvendor() && lclbRequest.Count > 0)
            {
                if (lclbRequest.Where(o => (o.icdoProviderReportDataBatchRequest.effective_start_date==icdoProviderReportDataBatchRequest.effective_start_date  ||
                    o.icdoProviderReportDataBatchRequest.status_value == busConstant.Vendor_Payment_Status_NotProcessed) &&
                    icdoProviderReportDataBatchRequest.provider_report_data_batch_request_id !=
                    o.icdoProviderReportDataBatchRequest.provider_report_data_batch_request_id).Count() > 0)
                {
                    return true;
                }
            }
            return false;
        }
        //PIR 24921 load Provider Report Data DC
        private Collection<busProviderReportDataDC> _iclbProviderReportDataDC;
        public Collection<busProviderReportDataDC> iclbProviderReportDataDC
        {
            get
            {
                return _iclbProviderReportDataDC;
            }

            set
            {
                _iclbProviderReportDataDC = value;
            }
        }

        public void LoadProviderReportDataDC()
        {
            DataTable ldtbList = Select("entProviderReportDataBatchRequest.LoadProviderDataDC", new object[1] { icdoProviderReportDataBatchRequest.provider_report_data_batch_request_id });
            _iclbProviderReportDataDC = GetCollection<busProviderReportDataDC>(ldtbList, "icdoProviderReportDataDc");
        }
        //PIR 24921 load Provider Report Data Deff Comp
        private Collection<busProviderReportDataDeffComp> _iclbProviderReportDataDeffComp;
        public Collection<busProviderReportDataDeffComp> iclbProviderReportDataDeffComp
        {
            get
            {
                return _iclbProviderReportDataDeffComp;
            }

            set
            {
                _iclbProviderReportDataDeffComp = value;
            }
        }

        public void LoadProviderReportDataDeffcompDetails()
        {
            DataTable ldtbList = Select("entProviderReportDataBatchRequest.LoadProviderDataDiffComp", new object[1] { icdoProviderReportDataBatchRequest.provider_report_data_batch_request_id });
            _iclbProviderReportDataDeffComp = GetCollection<busProviderReportDataDeffComp>(ldtbList, "icdoProviderReportDataDeffComp");
        }

        private Collection<busProviderReportDataInsurance> _iclbProviderReportDataInsurance;
        public Collection<busProviderReportDataInsurance> iclbProviderReportDataInsurance
        {
            get
            {
                return _iclbProviderReportDataInsurance;
            }

            set
            {
                _iclbProviderReportDataInsurance = value;
            }
        }

        public void LoadProviderReportDataInsurance()
        {
            DataTable ldtbList = Select("entProviderReportDataBatchRequest.LaodProviderDataInsurance", new object[1] { icdoProviderReportDataBatchRequest.provider_report_data_batch_request_id});
            _iclbProviderReportDataInsurance = GetCollection<busProviderReportDataInsurance>(ldtbList, "icdoProviderReportDataInsurance");
        }
        private Collection<busProviderReportDataMedicarePartD> _iclbProviderReportDataMedicarePartD;
        public Collection<busProviderReportDataMedicarePartD> iclbProviderReportDataMedicarePartD
        {
            get
            {
                return _iclbProviderReportDataMedicarePartD;
            }

            set
            {
                _iclbProviderReportDataMedicarePartD = value;
            }
        }

        public void LoadProviderReportDataMedicarePartD()
        {

            DataTable ldtbList = Select("entProviderReportDataBatchRequest.LoadProviderReportDataMedicarePartD", new object[1] { icdoProviderReportDataBatchRequest.provider_report_data_batch_request_id });
            _iclbProviderReportDataMedicarePartD = GetCollection<busProviderReportDataMedicarePartD>(ldtbList, "icdoProviderReportDataMedicare");

        }
    }
}
