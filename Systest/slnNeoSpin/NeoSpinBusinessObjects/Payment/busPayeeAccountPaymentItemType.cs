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
using System.Collections.Generic;
#endregion

namespace NeoSpin.BusinessObjects
{
	[Serializable]
	public class busPayeeAccountPaymentItemType : busPayeeAccountPaymentItemTypeGen
    {
        public decimal idecItemAmount { get; set; }//For MSS Layout change
        // Load the Vendor Org object for the selected Vendor.
        public void LoadVendor()
        {
            if (ibusVendor == null)
                ibusVendor = new busOrganization();
            ibusVendor.FindOrganization(icdoPayeeAccountPaymentItemType.vendor_org_id);
        }

        // Load the Payment Item Type(Deduction) object.
        public void LoadPaymentItemType()
        {
            if (ibusPaymentItemType == null)
                ibusPaymentItemType = new busPaymentItemType();
            ibusPaymentItemType.FindPaymentItemType(icdoPayeeAccountPaymentItemType.payment_item_type_id);
        }

        // Load the Payee account for the Deduction
        public void LoadPayeeAccount()
        {
            if (ibusPayeeAccount == null)
                ibusPayeeAccount = new busPayeeAccount();
            ibusPayeeAccount.FindPayeeAccount(icdoPayeeAccountPaymentItemType.payee_account_id);
        }
        public bool iblnIsNewMode { get; set; }

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            // To Validate the Vendor Org
            if (icdoPayeeAccountPaymentItemType.vendor_org_id != 0)
                LoadVendor();
            // To Validate the Selected Deduction
            if (icdoPayeeAccountPaymentItemType.payment_item_type_id != 0)
                if (ibusPaymentItemType == null)
                    LoadPaymentItemType();
            icdoPayeeAccountPaymentItemType.vendor_org_id = busGlobalFunctions.GetOrgIdFromOrgCode(icdoPayeeAccountPaymentItemType.vendor_org_code);
          
            //Commented the below code as Dues amount query depends on start date, which will be validated only later
            //LoadDuesAmount();
            if (aenmPageMode == utlPageMode.New)
                iblnIsNewMode = true;
            else
                iblnIsNewMode = false;

            base.BeforeValidate(aenmPageMode);
        }

        public decimal idecDuesAmount { get; set; }
        public void LoadDuesAmount()
        {
            idecDuesAmount= Convert.ToDecimal(DBFunction.DBExecuteScalar("cdoPayeeAccountPaymentItemType.GetDuesAmount",
                                            new object[2] { icdoPayeeAccountPaymentItemType.vendor_org_id, icdoPayeeAccountPaymentItemType.start_date },
                                            iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
        }

        public bool IsDuesAmountZero()
        {
            //Added below code so that dues amount depends on start date and if start date is null, should not validate this condition
            if (icdoPayeeAccountPaymentItemType.start_date == DateTime.MinValue)
                return false;
            if(idecDuesAmount==0.0m)
                LoadDuesAmount();
            if (IsPaymentItemDues() && idecDuesAmount == 0.0m && !IsVendorToBeSelected())
            {
                return true;
            }
            return false;
        }

        public override void BeforePersistChanges()
        {
            // *** BR-074-02 *** System must select the Vendor Org if only one Org is associated to the selected Deduction Type.     
            if (iclbOrgDeductionType == null)
                LoadOrgDeductionType(icdoPayeeAccountPaymentItemType.start_date);
            if (iclbOrgDeductionType.Count == 1)
                icdoPayeeAccountPaymentItemType.vendor_org_id = iclbOrgDeductionType[0].icdoOrgDeductionType.org_id;
            if (IsPaymentItemDues())
            {
                if (idecDuesAmount == 0.0m)
                    LoadDuesAmount();
                icdoPayeeAccountPaymentItemType.amount = idecDuesAmount;
            }
        }

        //NOTE :- do not use below Persitchanges from any batch, since batch schedule id is set to zero
        //assumption is PersistChanges in this business object gets called only from screen
        public override int PersistChanges()
        {
            if (icdoPayeeAccountPaymentItemType.payee_account_payment_item_type_id > 0)
            {
                //as per meeting with satya on Aug 13,2010
                icdoPayeeAccountPaymentItemType.batch_schedule_id = 0;
                icdoPayeeAccountPaymentItemType.Update();
            }
            else
                icdoPayeeAccountPaymentItemType.Insert();
            return 1;
        }
        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            // *** BR-074-13 *** System must change the Payee Status to Review immediately after the deduction information change.
            LoadVendor();
            icdoPayeeAccountPaymentItemType.vendor_org_code = ibusVendor.icdoOrganization.org_code;
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.idecBenefitAmount == 0.0m)
                ibusPayeeAccount.LoadBenefitAmount();

            if (IsSelectedDeductionItemRefund())
                CreateDeductionRefundDetails();

            if (ibusBaseActivityInstance.IsNotNull())
            {
                
                ibusPayeeAccount.ibusBaseActivityInstance = ibusBaseActivityInstance;
                //ibusPayeeAccount.SetWorkflowActivityReferenceID();
                ibusPayeeAccount.SeBpmActivityInstanceReferenceID();
                // ibusPayeeAccount.SetProcessInstanceParameters();
                ibusPayeeAccount.SetCaseInstanceParameters();
            }

            base.AfterPersistChanges();

            if (ibusPayeeAccount.ibusSoftErrors == null)
                ibusPayeeAccount.LoadErrors();
          
            ibusPayeeAccount.iblnClearSoftErrors = false;
            ibusPayeeAccount.ibusSoftErrors.iblnClearError = false;

            if (ibusPayeeAccount.idecBenefitAmount < 0.0m)
                ibusPayeeAccount.iblnInvalidNetAmountIndicator = true;
            ibusPayeeAccount.CreateReviewPayeeAccountStatus();
            ibusPayeeAccount.iblnDedudtionInfoChangeIndicator = true;
            ibusPayeeAccount.ValidateSoftErrors();
            ibusPayeeAccount.UpdateValidateStatus();
            EvaluateInitialLoadRules();
        }

        public bool IsAmountEnteredDues()
        {
            if (IsPaymentItemDues())
            {
                if (iblnIsNewMode && icdoPayeeAccountPaymentItemType.amount > 0.0m)
                {
                    return true;
                }
                else
                {
                    decimal ldecOldAmount = 0.0M;
                    if (icdoPayeeAccountPaymentItemType.ihstOldValues.Count > 0 && icdoPayeeAccountPaymentItemType.ihstOldValues["amount"] != null)
                        ldecOldAmount = Convert.ToDecimal(icdoPayeeAccountPaymentItemType.ihstOldValues["amount"]);
                    if (ldecOldAmount > 0.0M && icdoPayeeAccountPaymentItemType.amount != ldecOldAmount && icdoPayeeAccountPaymentItemType.amount > 0.0m)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        //dues record cannot be created from screen

        public bool IsPaymentItemDues()
        {   
            //Always reload to check the current payment item
            LoadPaymentItemType();
            if (ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.PAPITDues)
            {
                return true;
            }
            return false;
        }

        //Create Deduction refund details if the selected item is deduction refund item
        private void CreateDeductionRefundDetails()
        {
            ibusPayeeAccountDeductionRefund.icdoPayeeAccountDeductionRefund.payee_account_id = ibusPayeeAccount.icdoPayeeAccount.payee_account_id;
            ibusPayeeAccountDeductionRefund.icdoPayeeAccountDeductionRefund.payee_account_payment_item_type_id = icdoPayeeAccountPaymentItemType.payee_account_payment_item_type_id;
            ibusPayeeAccountDeductionRefund.icdoPayeeAccountDeductionRefund.payment_item_type_id = icdoPayeeAccountPaymentItemType.payment_item_type_id;
            ibusPayeeAccountDeductionRefund.icdoPayeeAccountDeductionRefund.amount = icdoPayeeAccountPaymentItemType.amount;
            ibusPayeeAccountDeductionRefund.icdoPayeeAccountDeductionRefund.account_number = icdoPayeeAccountPaymentItemType.account_number;
            ibusPayeeAccountDeductionRefund.icdoPayeeAccountDeductionRefund.vendor_org_id = icdoPayeeAccountPaymentItemType.vendor_org_id;
            ibusPayeeAccountDeductionRefund.icdoPayeeAccountDeductionRefund.start_date = icdoPayeeAccountPaymentItemType.start_date;
            ibusPayeeAccountDeductionRefund.icdoPayeeAccountDeductionRefund.end_date = icdoPayeeAccountPaymentItemType.end_date;
            if (ibusPayeeAccountDeductionRefund.icdoPayeeAccountDeductionRefund.payee_account_deduction_refund_id > 0)
                ibusPayeeAccountDeductionRefund.icdoPayeeAccountDeductionRefund.Update();
            else
                ibusPayeeAccountDeductionRefund.icdoPayeeAccountDeductionRefund.Insert();
        }

        // *** BR-074-03 *** If no vendor was selected and multiple vendors exist for the selected Deduction Type.
        public bool IsVendorToBeSelected()
        {
            if (ibusPaymentItemType == null)
                LoadPaymentItemType();
            if (iclbOrgDeductionType == null)
                LoadOrgDeductionType(icdoPayeeAccountPaymentItemType.start_date);
            // PIR ID 971
            if (iclbOrgDeductionType.Count!=1 && string.IsNullOrEmpty(icdoPayeeAccountPaymentItemType.vendor_org_code))
                return true;
            else
                return false;
        }

        //Start date should be equal to or less than next benefit payment date and it should be first of month
        //else throw an error
        public bool IsStartdateNotValid()
        {
            if ((icdoPayeeAccountPaymentItemType.start_date != DateTime.MinValue) && (!AreFieldsReadOnly()))
            {
                if (ibusPayeeAccount == null)
                    LoadPayeeAccount();
                if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                    ibusPayeeAccount.LoadNexBenefitPaymentDate();
                DateTime ldtStartDate = new DateTime(ibusPayeeAccount.idtNextBenefitPaymentDate.Year, ibusPayeeAccount.idtNextBenefitPaymentDate.Month, 1);
                if (icdoPayeeAccountPaymentItemType.start_date < ldtStartDate)
                {
                    return true;
                }
                else if (icdoPayeeAccountPaymentItemType.start_date >= ldtStartDate)
                {
                    if (icdoPayeeAccountPaymentItemType.start_date.Day != 1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        //If  End Date is less than or equal to next benefit payment date or one day before next benefit payment date ,then throw an error 
        public bool IsEnddateNotValid()
        {
            if ((icdoPayeeAccountPaymentItemType.start_date != DateTime.MinValue) && (icdoPayeeAccountPaymentItemType.end_date != DateTime.MinValue))
            {
                if (ibusPayeeAccount == null)
                    LoadPayeeAccount();
                if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                    ibusPayeeAccount.LoadNexBenefitPaymentDate();
                DateTime ldtBatchDate = ibusPayeeAccount.idtNextBenefitPaymentDate;
                if (icdoPayeeAccountPaymentItemType.end_date < ldtBatchDate.AddDays(-1))
                {
                    return true;
                }
                else if (icdoPayeeAccountPaymentItemType.end_date >= ldtBatchDate.AddDays(-1))
                {
                    DateTime ldtLastDateOfMonth = busGlobalFunctions.GetLastDayOfMonth(icdoPayeeAccountPaymentItemType.end_date);
                    if (ldtLastDateOfMonth.Day != icdoPayeeAccountPaymentItemType.end_date.Day)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public Collection<busOrgDeductionType> iclbOrgDeductionType { get; set; }
        // Load the list of Vendors corresponding to this Payment Item Type(Deduction) ID.
        public void LoadOrgDeductionType(DateTime adtGivenDate)
        {
            if (iclbOrgDeductionType == null)
                iclbOrgDeductionType = new Collection<busOrgDeductionType>();
            DataTable ldtbResult = Select<cdoOrgDeductionType>(
                    new string[1] { "payment_item_type_id" },
                    new object[1] { icdoPayeeAccountPaymentItemType.payment_item_type_id }, null, null);
            foreach (DataRow dr in ldtbResult.Rows)
            {
                busOrgDeductionType lobjOrgDeductionType = new busOrgDeductionType();
                lobjOrgDeductionType.icdoOrgDeductionType = new cdoOrgDeductionType();
                lobjOrgDeductionType.icdoOrgDeductionType.LoadData(dr);
                lobjOrgDeductionType.LoadOrganization();
                if (busGlobalFunctions.CheckDateOverlapping(adtGivenDate,
                                                            lobjOrgDeductionType.icdoOrgDeductionType.start_date,
                                                            lobjOrgDeductionType.icdoOrgDeductionType.end_date))
                    iclbOrgDeductionType.Add(lobjOrgDeductionType);
            }
        }
        //If Start Date is less than  next benefit payment date or not first of month,then throw an error
        public bool AreFieldsReadOnly()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                ibusPayeeAccount.LoadNexBenefitPaymentDate();
            if ((icdoPayeeAccountPaymentItemType.ienuObjectState != ObjectState.Insert) && (icdoPayeeAccountPaymentItemType.start_date != DateTime.MinValue)
                && (icdoPayeeAccountPaymentItemType.start_date < ibusPayeeAccount.idtNextBenefitPaymentDate))
            {                
                return true;
            }
            //if deduction item type is updatable then only show in editable mode, else read only mode
            DataTable ldtDeductionItems = Select("cdoPaymentItemType.LoadDeductionItems", new object[0] { });
            if (icdoPayeeAccountPaymentItemType.ienuObjectState == ObjectState.Insert ||
                ldtDeductionItems.AsEnumerable().Where(o => o.Field<int>("payment_item_type_id") == icdoPayeeAccountPaymentItemType.payment_item_type_id).Any())
            {
                return false;
            }
            else
                return true;
        }

        // *** BR-074-07 *** The System must not allow same Type and Vendor within overlapping Start and End dates
        public bool IsOverlappingDeductionsExists()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.iclbDeductions == null)
                ibusPayeeAccount.LoadDeductions();
            var lvarDeductions = ibusPayeeAccount.iclbDeductions.Where(lobjDeduction =>
               (lobjDeduction.icdoPayeeAccountPaymentItemType.payee_account_payment_item_type_id !=
                                                                            icdoPayeeAccountPaymentItemType.payee_account_payment_item_type_id) &&
                    (lobjDeduction.icdoPayeeAccountPaymentItemType.payment_item_type_id == icdoPayeeAccountPaymentItemType.payment_item_type_id) &&
                    (lobjDeduction.icdoPayeeAccountPaymentItemType.vendor_org_id == icdoPayeeAccountPaymentItemType.vendor_org_id) &&
                    (busGlobalFunctions.CheckDateOverlapping(lobjDeduction.icdoPayeeAccountPaymentItemType.start_date,
                                                             lobjDeduction.icdoPayeeAccountPaymentItemType.end_date,
                                                             icdoPayeeAccountPaymentItemType.start_date,
                                                             icdoPayeeAccountPaymentItemType.end_date)));
            if (lvarDeductions.Count() > 0)
                return true;
            
            return false;
        }

        // *** BR-074-17 *** Organization Type should be Vendor
        public bool IsOrgTypeVendor()
        {
            if (ibusVendor == null)
                LoadVendor();
            if (ibusVendor.icdoOrganization.org_type_value == busConstant.OrganizationTypeVendor)
                return true;
            else
                return false;
        }

        public override int Delete()
        {
            // *** BR-074-13 *** System must change the Payee Status to Review immediately after the deduction information change.
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            ibusPayeeAccount.CreateReviewPayeeAccountStatus();
            ibusPayeeAccount.iblnDedudtionInfoChangeIndicator = true;
            ibusPayeeAccount.ValidateSoftErrors();
            ibusPayeeAccount.icdoPayeeAccount.Select();
            ibusPayeeAccount.UpdateValidateStatus();
            //if the selected deduction item is refund
            if (IsSelectedDeductionItemRefund())
                if (ibusPayeeAccountDeductionRefund.FindPayeeAccountDeductionRefundByPayeeAccountPaymentItemID(icdoPayeeAccountPaymentItemType.payee_account_payment_item_type_id))
                    ibusPayeeAccountDeductionRefund.icdoPayeeAccountDeductionRefund.Delete();                       
            
            return base.Delete();
        }        
        // Load to Display in Deduction Maintenance History Grid
        public void LoadDeductionHistory()
        {
            iclbDeductionHistory = new Collection<busPayeeAccountPaymentItemType>();
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.iclbDeductions == null)
                ibusPayeeAccount.LoadDeductions();
            foreach (busPayeeAccountPaymentItemType lobjDeduction in ibusPayeeAccount.iclbDeductions)
            {
                if ((lobjDeduction.icdoPayeeAccountPaymentItemType.payee_account_payment_item_type_id !=
                                                        icdoPayeeAccountPaymentItemType.payee_account_payment_item_type_id))
                     iclbDeductionHistory.Add(lobjDeduction);
            }
        }

        // To check whether the Vendor is associated to the Deduction Type for the given period.
        public bool IsDeductionAssociatedToVendor()
        {
            if ((icdoPayeeAccountPaymentItemType.payment_item_type_id != 0) &&
                (icdoPayeeAccountPaymentItemType.vendor_org_id != 0) &&
                (icdoPayeeAccountPaymentItemType.start_date != DateTime.MinValue))
            {
                LoadOrgDeductionType(icdoPayeeAccountPaymentItemType.start_date);
                var lvarorgdeductiontype = iclbOrgDeductionType.Where(lobjOrgDeductionType =>
                       (lobjOrgDeductionType.ibusOrganization.icdoOrganization.org_code == icdoPayeeAccountPaymentItemType.vendor_org_code)
                          && busGlobalFunctions.CheckDateOverlapping(icdoPayeeAccountPaymentItemType.start_date,
                                                                          lobjOrgDeductionType.icdoOrgDeductionType.start_date,
                                                                          lobjOrgDeductionType.icdoOrgDeductionType.end_date));
                if (lvarorgdeductiontype.Count() > 0)
                    return true;
            }
            return false;
        }
        //Payment option should be negative for negative items
        public bool IsPaymentOptionInvalid()
        {
            //Load always for validation
            LoadPaymentItemType();
            if (ibusPaymentItemType.icdoPaymentItemType.item_type_direction == -1 &&
                ibusPayeeAccountDeductionRefund.icdoPayeeAccountDeductionRefund.payment_option_value != busConstant.PaymentDeductionOptionRegular)
                return true;
            return false;
        }

        //Check the Selected deduction item is refund or not
        public bool IsSelectedDeductionItemRefund()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.iclbPaymentItemType == null)
                ibusPayeeAccount.LoadPaymentItemType();
            int lintCount = (from lobjPaymentItemType in ibusPayeeAccount.iclbPaymentItemType
                             where lobjPaymentItemType.icdoPaymentItemType.deduction_value == busConstant.DeductionIndicatorUpdatable
                             && lobjPaymentItemType.icdoPaymentItemType.item_type_direction == 1 && 
                             lobjPaymentItemType.icdoPaymentItemType.payment_item_type_id==icdoPayeeAccountPaymentItemType.payment_item_type_id
                             select lobjPaymentItemType).Count();
            if (lintCount > 0)
            {
                return true;
            }
            return false;
        }
        // PIR ID 973 - Total Deductions amount cannot be Greater Than Monthly Benefit Amount
        public bool IsDeductionAmountGreaterThanBenefitAmount()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            ibusPayeeAccount.LoadBenefitAmount(icdoPayeeAccountPaymentItemType.start_date);
            //PIR 1632
            if (ibusPaymentItemType == null)
                LoadPaymentItemType();
            if (ibusPaymentItemType.icdoPaymentItemType.item_type_direction==1 && icdoPayeeAccountPaymentItemType.amount > ibusPayeeAccount.idecBenefitAmount)
                return true;
            return false;
        }      

        public override busBase GetCorPerson()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.ibusPayee == null)
                ibusPayeeAccount.LoadPayee();
            return ibusPayeeAccount.ibusPayee;
        }

        public override busBase GetCorOrganization()
        {
            if (ibusVendor == null)
                LoadVendor();
            return ibusVendor;
        }

        public bool IsValidationRequired()
        {
            if (icdoPayeeAccountPaymentItemType.ienuObjectState == ObjectState.Update)
            {
                if (Convert.ToDateTime(icdoPayeeAccountPaymentItemType.ihstOldValues["end_date"]) != icdoPayeeAccountPaymentItemType.end_date)
                    return false;
            }
            return true;
        }

        //uat pir 1420
        public bool IsPayeeAccountStatusCancelPendingOrCancelled()
        {
            bool lblnResult = false;
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.ibusPayeeAccountActiveStatus == null)
                ibusPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();

            if (ibusPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusCancelPending() || ibusPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusCancelled())
                lblnResult = true;

            return lblnResult;
        }

        public bool IsEndDateReadOnly()
        {
            bool lblnResult = false;
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                ibusPayeeAccount.LoadNexBenefitPaymentDate();
            if (icdoPayeeAccountPaymentItemType.end_date != DateTime.MinValue && icdoPayeeAccountPaymentItemType.end_date < ibusPayeeAccount.idtNextBenefitPaymentDate)
                lblnResult = true;
            return lblnResult;
        }

        public DataSet rptPensionPaymentHistory(int aintPayeeAccountId, DateTime adtStartDate, DateTime adtEndDate)
        {
            utlPassInfo lobjPassInfo = utlPassInfo.iobjPassInfo;
            DataSet ldsReport = new DataSet("Retirement Missing Payroll");
            DataTable ldtTable = CreatePensionPaymentHistoryTable();
            DataTable ldtPersonTable = CreatePersonTable();
            ldtTable.TableName = busConstant.ReportTableName;
            DataRow ldtrPersonRow = ldtPersonTable.NewRow();
            if (adtEndDate == DateTime.MinValue)
                adtEndDate = DateTime.MaxValue;
            if (aintPayeeAccountId > 0)
            {
                DataTable ldtbNDPERSPhoneNumberlist = lobjPassInfo.isrvDBCache.GetCodeValues(52);
                DataTable ltbResults = Select("cdoPayeeAccountPaymentItemType.rptPensionPaymentHistory", new object[3] { aintPayeeAccountId, adtStartDate, adtEndDate });
                foreach (DataRow ldtr in ltbResults.Rows)
                {
                    DataRow ldtrRow = ldtTable.NewRow();
                    ldtrRow["ITEM_DESCRIPTION"] = ldtr["ITEM_DESCRIPTION"];
                    ldtrRow["ITEM_AMOUNT"] = ldtr["ITEM_AMOUNT"];
                    ldtrRow["StartDate"] = ldtr["StartDate"];
                    ldtrRow["EndDate"] = ldtr["EndDate"];
                    ldtTable.Rows.Add(ldtrRow);
                }
                if (ibusPayeeAccount == null)
                    ibusPayeeAccount = new busPayeeAccount();
                ibusPayeeAccount.FindPayeeAccount(aintPayeeAccountId);
                ibusPayeeAccount.LoadPayee();
                ibusPayeeAccount.ibusPayee.LoadPersonCurrentAddress();
                if (ibusPayeeAccount.ibusPayee.ibusPersonCurrentAddress.icdoPersonCurrentAddress.IsNull())
                    ibusPayeeAccount.ibusPayee.ibusPersonCurrentAddress.icdoPersonCurrentAddress = new cdoPersonAddress();
                ldtrPersonRow["PERSON_ID"] = ibusPayeeAccount.ibusPayee.icdoPerson.person_id;
                ldtrPersonRow["FULL_NAME"] = ibusPayeeAccount.ibusPayee.icdoPerson.FullName;
                ldtrPersonRow["STREET1"] = String.IsNullOrEmpty(ibusPayeeAccount.ibusPayee.ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_line_1)?
                                            String.Empty : ibusPayeeAccount.ibusPayee.ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_line_1.ToUpper();
                ldtrPersonRow["STREET2"] = String.IsNullOrEmpty(ibusPayeeAccount.ibusPayee.ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_line_2) ?
                                            String.Empty : ibusPayeeAccount.ibusPayee.ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_line_2.ToUpper();
                ldtrPersonRow["CITY"] = String.IsNullOrEmpty(ibusPayeeAccount.ibusPayee.ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_city) ?
                                            String.Empty : ibusPayeeAccount.ibusPayee.ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_city.ToUpper();
                ldtrPersonRow["STATE"] = String.IsNullOrEmpty(ibusPayeeAccount.ibusPayee.ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_state_value) ?
                                            String.Empty : ibusPayeeAccount.ibusPayee.ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_state_value.ToUpper();
                ldtrPersonRow["ZIP"] = ibusPayeeAccount.ibusPayee.ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_zip_code;
                ldtrPersonRow["BATCHDATE"] = DateTime.Now;
                ldtrPersonRow["SALUTATION"] = ibusPayeeAccount.ibusPayee.icdoPerson.FullName;

                if ((ldtbNDPERSPhoneNumberlist != null) && (ldtbNDPERSPhoneNumberlist.Rows.Count > 0))
                {
                    foreach (DataRow dr in ldtbNDPERSPhoneNumberlist.Rows)
                    {
                        if ((dr["CODE_VALUE"]).ToString() == "NDPH")
                        {
                            string lstrPhoneNumber = string.Empty;
                            lstrPhoneNumber = dr["DATA1"].ToString();
                            ldtrPersonRow["NDPERSPhoneNumber"] = HelperFunction.FormatData(lstrPhoneNumber, "{0:(###) ###-####}");

                        }
                        if (dr["CODE_VALUE"].ToString() == "NDTP")
                        {
                            string lstrPhoneNumber = string.Empty;
                            lstrPhoneNumber = dr["DATA1"].ToString();
                            ldtrPersonRow["NDPERSTollFreePhoneNumber"] = HelperFunction.FormatData(lstrPhoneNumber, "{0:(###) ###-####}");
                        }
                    }
                }
            }
            ldtPersonTable.Rows.Add(ldtrPersonRow);
            ldsReport.Tables.Add(ldtPersonTable);
            ldsReport.Tables.Add(ldtTable);
            return ldsReport;
        }

        private DataTable CreatePersonTable()
        {
            DataTable ldtbReportTable = new DataTable();
            DataColumn dc1 = new DataColumn("PERSON_ID", Type.GetType("System.Int32"));
            DataColumn dc2 = new DataColumn("FULL_NAME", Type.GetType("System.String"));
            DataColumn dc3 = new DataColumn("STREET1", Type.GetType("System.String"));
            DataColumn dc4 = new DataColumn("STREET2", Type.GetType("System.String"));
            DataColumn dc5 = new DataColumn("CITY", Type.GetType("System.String"));
            DataColumn dc6 = new DataColumn("STATE", Type.GetType("System.String"));
            DataColumn dc7 = new DataColumn("ZIP", Type.GetType("System.String"));
            DataColumn dc8 = new DataColumn("BATCHDATE", Type.GetType("System.DateTime"));
            DataColumn dc9 = new DataColumn("SALUTATION", Type.GetType("System.String"));
            DataColumn dc10 = new DataColumn("NDPERSPhoneNumber", Type.GetType("System.String"));
            DataColumn dc11 = new DataColumn("NDPERSTollFreePhoneNumber", Type.GetType("System.String"));
            ldtbReportTable.Columns.Add(dc1);
            ldtbReportTable.Columns.Add(dc2);
            ldtbReportTable.Columns.Add(dc3);
            ldtbReportTable.Columns.Add(dc4);
            ldtbReportTable.Columns.Add(dc5);
            ldtbReportTable.Columns.Add(dc6);
            ldtbReportTable.Columns.Add(dc7);
            ldtbReportTable.Columns.Add(dc8);
            ldtbReportTable.Columns.Add(dc9);
            ldtbReportTable.Columns.Add(dc10);
            ldtbReportTable.Columns.Add(dc11);
            return ldtbReportTable;
        }

        private DataTable CreatePensionPaymentHistoryTable()
        {
            DataTable ldtbReportTable = new DataTable();
            DataColumn dc1 = new DataColumn("ITEM_DESCRIPTION", Type.GetType("System.String"));
            DataColumn dc2 = new DataColumn("ITEM_AMOUNT", Type.GetType("System.Decimal"));
            DataColumn dc3 = new DataColumn("StartDate", Type.GetType("System.DateTime"));
            DataColumn dc4 = new DataColumn("EndDate", Type.GetType("System.DateTime"));
            ldtbReportTable.Columns.Add(dc1);
            ldtbReportTable.Columns.Add(dc2);
            ldtbReportTable.Columns.Add(dc3);
            ldtbReportTable.Columns.Add(dc4);
            return ldtbReportTable;
        }
    }
}
