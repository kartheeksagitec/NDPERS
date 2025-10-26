
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
using Sagitec.ExceptionPub;
using System.Linq;
#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busFileEmployerPayrollPurchaseInbound : busFileBase
    {
        public busFileEmployerPayrollPurchaseInbound()
        {
        }
        private static int lintHeaderGroupValue = 0;
        private string lstrHeaderGroupValue = "0";

        public override void SetHeaderGroupValue()
        {
            if (icdoFileDtl != null)
            {
                if (icdoFileDtl.transaction_code_value == "1")
                {
                    lintHeaderGroupValue++;
                    if (lintHeaderGroupValue < 10)
                        lstrHeaderGroupValue = "0" + lintHeaderGroupValue.ToString();
                    else
                        lstrHeaderGroupValue = lintHeaderGroupValue.ToString();
                }
                icdoFileDtl.header_group_value = lstrHeaderGroupValue;
            }
        }

        private busEmployerPayrollHeader _ibusEmployerPayrollHeader;
        public busEmployerPayrollHeader ibusEmployerPayrollHeader
        {
            get { return _ibusEmployerPayrollHeader; }
            set { _ibusEmployerPayrollHeader = value; }
        }

        public DateTime idtBatchRunDate { get; set; }

        public override busBase NewHeader()
        {
            _ibusEmployerPayrollHeader = new busEmployerPayrollHeader();
            _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader();
            _ibusEmployerPayrollHeader.iblnValidateDetail = true;
            _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.reporting_source_value = busConstant.PayrollHeaderReportingSourceWebRpt;
            _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.balancing_status_value = busConstant.PayrollHeaderBalancingStatusNoRemittance;
            _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.report_type_value = busConstant.PayrollHeaderReportTypeRegular;
            _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value = busConstant.PayrollHeaderBenefitTypePurchases;
            _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.interest_waiver_flag = busConstant.Flag_No;
            //PROD Pir 4064
            _ibusEmployerPayrollHeader.iblnFromFile = true;
            idtBatchRunDate = busGlobalFunctions.GetSysManagementBatchDate();
            _ibusEmployerPayrollHeader.icdoFileHdr = icdoFileHdr;
            return _ibusEmployerPayrollHeader;
        }
        
        public override busBase NewDetail()
        {
            if (icdoFileDtl.transaction_code_value == "1")
                return _ibusEmployerPayrollHeader;

            return _ibusEmployerPayrollHeader.CreateNewEmployerPayrollDetail();
        }

        public override void ProcessHeader()
        {            
            //Load the Organization Object for the Header Validation
            if (!(String.IsNullOrEmpty(_ibusEmployerPayrollHeader.istrOrgCodeId)))
            {
                _ibusEmployerPayrollHeader.ibusOrganization = new busOrganization();
                _ibusEmployerPayrollHeader.ibusOrganization.FindOrganizationByOrgCode(_ibusEmployerPayrollHeader.istrOrgCodeId);
                _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.org_id = _ibusEmployerPayrollHeader.ibusOrganization.icdoOrganization.org_id;
            }
            base.ProcessHeader();
        }

        public override void BeforePersisitHeader()
        {
            //Assigning Prepopulated Values into Header          
            _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.received_date = DateTime.Now;
            _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.submitted_date = DateTime.Now;
            //Inserting the Header Record
            _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.Insert();
            if (_ibusEmployerPayrollHeader.iclbEmployerPayrollDetail == null)
            {
                _ibusEmployerPayrollHeader.iclbEmployerPayrollDetail = new Collection<busEmployerPayrollDetail>();
            }
            foreach (busEmployerPayrollDetail lobjEmployerPayrollDetail in _ibusEmployerPayrollHeader.iclbEmployerPayrollDetail)
            {
                lobjEmployerPayrollDetail.ibusEmployerPayrollHeader = ibusEmployerPayrollHeader;
                lobjEmployerPayrollDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader = ibusEmployerPayrollHeader.icdoEmployerPayrollHeader;
                lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.employer_payroll_header_id = _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.employer_payroll_header_id;

                //PERSON ID, FIRST NAME, LAST NAME
                if (!String.IsNullOrEmpty(lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ssn))
                {
                    DataTable ldtbSSN = busBase.Select<cdoPerson>(new string[1] { "ssn" }, new object[1] { lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ssn }, null, null);
                    if (ldtbSSN.Rows.Count > 0)
                    {
                        lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.person_id = Convert.ToInt32(ldtbSSN.Rows[0]["person_id"]);
                        //// PIR 2329 fix. Satya asked to post any retirement plan_id for Purchase
                        //if (lobjEmployerPayrollDetail.ibusPerson == null)
                        //    lobjEmployerPayrollDetail.LoadPerson();
                        //if (lobjEmployerPayrollDetail.ibusPerson.icolPersonAccount == null)
                        //    lobjEmployerPayrollDetail.ibusPerson.LoadPersonAccount();
                        //Collection<busPersonAccount> lclbPersonAccount = busGlobalFunctions.Sort<busPersonAccount>("icdoPersonAccount.plan_id desc",
                        //    lobjEmployerPayrollDetail.ibusPerson.icolPersonAccount);
                        //if (lclbPersonAccount.Count > 0 && lclbPersonAccount.Where(o => o.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeRetirement).Count() > 0)
                        //{
                        //    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.plan_id =
                        //        lclbPersonAccount.Where(o => o.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeRetirement).FirstOrDefault().icdoPersonAccount.plan_id;
                        //}
                    }
                }
                //Assign the Default Record Type for Purchase
                lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.record_type_value = busConstant.PayrollDetailRecordTypePurchase;
                lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.suppress_warnings_flag = "N";
                lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.purchase_pay_period_date = idtBatchRunDate;
                lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.purchase_amount_original = lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.purchase_amount_reported;
                lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.payment_class_original_value = lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.payment_class_value;
            }
        }

        public override string BeforeFieldAssigned(string astrFieldName, string astrFieldValue)
        {
            string lstrReturnValue = astrFieldValue;
            string lstrObjectField = astrFieldName.IndexOf(".") > -1 ? astrFieldName.Substring(astrFieldName.LastIndexOf(".") + 1) : astrFieldName;

            lstrObjectField = lstrObjectField.ToLower();

            //convert Org CODE id into Orgid
            if ((lstrObjectField == "istrorgcodeid") && (!(String.IsNullOrEmpty(astrFieldValue))))
            {
                lstrReturnValue = astrFieldValue;
            }

            if (lstrObjectField == "payment_class_value")
            {
                if ((!(String.IsNullOrEmpty(astrFieldValue))))
                {
                    if (astrFieldValue == "02")
                    {
                        lstrReturnValue = busConstant.Service_Purchase_Class_Employer_Installment_PreTax;
                    }
                    else if (astrFieldValue == "03")
                    {
                        lstrReturnValue = busConstant.Service_Purchase_Class_Employer_Installment_PostTax;
                    }
                }
            }
            return lstrReturnValue;
        }
        public override sfwOnFileError ContinueOnValueError(string astrObjectField, out string astrValue)
        {
            astrValue = String.Empty;
            string lstrObjectField = astrObjectField.IndexOf(".") > -1 ? astrObjectField.Substring(astrObjectField.LastIndexOf(".") + 1) : astrObjectField;
            //NOT YET IMPLEMENTED.. BASE IS DEFINED...  
            switch (lstrObjectField.ToLower())
            {
                default: return base.ContinueOnValueError(astrObjectField, out astrValue);
            }
        }

        public override void AfterReject(int aintErrorRecNo, string astrErrorMessage)
        {
            if (icdoFileHdr.reference_id > 0) // PIR - 7202
            {
                string lstrPrioityValue = string.Empty;
                busWSSHelper.PublishESSMessage(0, 0, string.Format(busGlobalFunctions.GetDBMessageTextByDBMessageID(6, iobjPassInfo, ref lstrPrioityValue),
                            icdoFileHdr.mailbox_file_name), lstrPrioityValue, aintOrgID: icdoFileHdr.reference_id);
                busGlobalFunctions.MoveFileFromProcessedToErrorFolder(icdoFileHdr, iobjPassInfo);
            }
        }

        public override void FinalizeFile()
        {
            lintHeaderGroupValue = 0;
            //PIR-13996 Added code to display message on Dashboard when Header Status is Review or Processed With Warnings.
            if ((icdoFileHdr != null && ((icdoFileHdr.status_value == busConstant.PayrollHeaderStatusReview) || (icdoFileHdr.status_value == busConstant.PayrollHeaderStatusProcessedWithWarnings)))
                && ((base.iarrErrors != null && base.iarrErrors.Count > 0) || (_ibusEmployerPayrollHeader.iclbDetailError != null && _ibusEmployerPayrollHeader.iclbDetailError.Count > 0) || (_ibusEmployerPayrollHeader.iclbEmployerPayrollHeaderError != null && _ibusEmployerPayrollHeader.iclbEmployerPayrollHeaderError.Count > 0)) && (icdoFileHdr.reference_id > 0))
            {
                string lstrPrioityValue = string.Empty;
                busWSSHelper.PublishESSMessage(0, 0, string.Format(busGlobalFunctions.GetDBMessageTextByDBMessageID(7, iobjPassInfo, ref lstrPrioityValue), icdoFileHdr.mailbox_file_name), lstrPrioityValue, aintOrgID: icdoFileHdr.reference_id);
            }
            base.FinalizeFile();
        }
    }
}
