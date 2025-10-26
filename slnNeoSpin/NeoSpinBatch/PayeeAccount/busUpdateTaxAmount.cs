#region Using directives
using System;
using System.Data;
using NeoSpin.CustomDataObjects;
using NeoSpin.BusinessObjects;
using System.Collections.ObjectModel;
using Sagitec.BusinessObjects;
using System.Linq;
using System.Linq.Expressions;

#endregion

namespace NeoSpinBatch
{
    public class busUpdateTaxAmount : busNeoSpinBatch
    {
        private int BatchSheduleID = 0;
    
        private DateTime _last_benefit_date;
        public DateTime last_benefit_date
        {
            get { return _last_benefit_date; }
            set { _last_benefit_date = value; }
        }

        public void LoadLastBenefitDate()
        {
            _last_benefit_date = busPayeeAccountHelper.GetLastBenefitPaymentDate();
        }

        private DateTime _next_benefit_date;
        public DateTime next_benefit_date
        {
            get
            {
                return _next_benefit_date;
            }
            set
            {
                _next_benefit_date = value;
            }
        }

        private decimal federal_threshold_amount
        {
            get { return Convert.ToDecimal(busGlobalFunctions.GetData1ByCodeValue(52, "FTTH", iobjPassInfo)); }
        }

        private decimal state_threshold_amount
        {
            get { return Convert.ToDecimal(busGlobalFunctions.GetData1ByCodeValue(52, "STTH", iobjPassInfo)); }
        }


        public DataTable idtFederalTaxChanges { get; set; }

        public DataTable idtStateTaxChanges { get; set; }

        public DataTable idtTaxExceptions { get; set; }

        public DataTable idtPAPIT{ get; set; }

        public DataTable idtTaxWithholding { get; set; }
        public void UpdateFederalAndStateTaxAmount(bool ablnDoNotRecalculateStateTax)
        {            
            istrProcessName = "Update Federal and State Tax Amount Batch";

            DateTime ldtNextPaymentDate = busPayeeAccountHelper.GetLastBenefitPaymentDate().AddMonths(1);
            // Load all Payee Account other than Refund
            DataTable ldtbResult = busBase.Select("cdoPayeeAccount.LoadPayeeAccountUpdateTaxRateBatch", new object[1] { ldtNextPaymentDate});

            idtPAPIT = busBase.Select("cdoPayeeAccount.LoadPAPITForRecalculatingTax", new object[1] { ldtNextPaymentDate });

            idtTaxWithholding = busBase.Select("cdoPayeeAccount.LoadTaxWithHoldingForRecalculatingTax", new object[1] { ldtNextPaymentDate });

         

            //set the next payment date
            if (last_benefit_date == DateTime.MinValue)
                LoadLastBenefitDate();
            next_benefit_date = last_benefit_date.AddMonths(1);

            //set the batch schedule id
            BatchSheduleID = 58;

            ReCalculateTax(ldtbResult,false, ablnDoNotRecalculateStateTax);

            // Create Federal Tax Rate Change Report
            if (idtFederalTaxChanges.Rows.Count > 0)
                CreateFederalTaxRateChangeReport();

            // Create State Tax Rate Change Report
            if (idtStateTaxChanges.Rows.Count > 0)
                CreateStateTaxRateChangeReport();

            // Create Tax Exception Report
            if (idtTaxExceptions.Rows.Count > 0)
                CreateExceptionReport();
        }

        public void ReCalculateTax(DataTable adtbResult, bool ablnPayrollBatch, bool ablnDoNotRecalculateStateTax = false)
        {
            if (ablnPayrollBatch)
            {
                //set the next payment date
                if (last_benefit_date == DateTime.MinValue)
                    LoadLastBenefitDate();
                next_benefit_date = last_benefit_date.AddMonths(2);

            }
            idtFederalTaxChanges = CreateNewDataTable();
            idtStateTaxChanges = CreateNewDataTable();
            idtTaxExceptions = CreateNewDataTable();

            foreach (DataRow ldtrPayeeAccount in adtbResult.Rows)
            {
                busPayeeAccount lobjPayeeAccount = new busPayeeAccount();
                lobjPayeeAccount.icdoPayeeAccount = new cdoPayeeAccount();
                lobjPayeeAccount.icdoPayeeAccount.LoadData(ldtrPayeeAccount);
                if (lobjPayeeAccount.icdoPayeeAccount.payee_perslink_id != 0)
                {
                    lobjPayeeAccount.ibusPayee = new busPerson();
                    lobjPayeeAccount.ibusPayee.icdoPerson = new cdoPerson();
                    lobjPayeeAccount.ibusPayee.icdoPerson.LoadData(ldtrPayeeAccount);
                }
                if (lobjPayeeAccount.icdoPayeeAccount.payee_org_id != 0)
                {
                    lobjPayeeAccount.ibusRecipientOrganization = new busOrganization();
                    lobjPayeeAccount.ibusRecipientOrganization.icdoOrganization = new cdoOrganization();
                    lobjPayeeAccount.ibusRecipientOrganization.icdoOrganization.LoadData(ldtrPayeeAccount);
                }
                if (!ablnPayrollBatch)
                {
                    busBase lobjBase = new busBase();
                    // Calculate New Federal Tax Amount
                    DataTable ldtbPayeePAPIT = idtPAPIT.AsEnumerable().Where(o =>
                        o.Field<int>("payee_account_id") == lobjPayeeAccount.icdoPayeeAccount.payee_account_id).AsDataTable();

                    lobjPayeeAccount.iclbPayeeAccountPaymentItemType = new Collection<busPayeeAccountPaymentItemType>();

                    foreach (DataRow ldr in ldtbPayeePAPIT.Rows)
                    {
                        busPayeeAccountPaymentItemType lobjPAPIT = new busPayeeAccountPaymentItemType { icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType() };
                        lobjPAPIT.icdoPayeeAccountPaymentItemType.LoadData(ldr);
                        //Load data will laod the first occurence of column name into CDO property
                        //while UPDATE is called, framework looks into ihstOldValues and current value then update, so if wrong data is loaded into CDO property
                        //audit columns wont be updated properly; so assigning the correct values explicitly
                        lobjPAPIT.icdoPayeeAccountPaymentItemType.update_seq = Convert.ToInt32(ldr["papit_update_seq"]);
                        lobjPAPIT.icdoPayeeAccountPaymentItemType.created_by = Convert.ToString(ldr["PAPIT_CREATED_BY"]);                        
                        lobjPAPIT.icdoPayeeAccountPaymentItemType.created_date = Convert.ToDateTime(ldr["PAPIT_CREATED_DATE"]);
                        lobjPAPIT.icdoPayeeAccountPaymentItemType.modified_by = Convert.ToString(ldr["PAPIT_MODIFIED_BY"]);
                        lobjPAPIT.icdoPayeeAccountPaymentItemType.modified_date = Convert.ToDateTime(ldr["PAPIT_MODIFIED_DATE"]);
                        lobjPAPIT.icdoPayeeAccountPaymentItemType.ihstOldValues["update_seq"] = Convert.ToInt32(ldr["papit_update_seq"]);
                        lobjPAPIT.icdoPayeeAccountPaymentItemType.ihstOldValues["created_by"] = Convert.ToString(ldr["PAPIT_CREATED_BY"]);
                        lobjPAPIT.icdoPayeeAccountPaymentItemType.ihstOldValues["created_date"] = Convert.ToDateTime(ldr["PAPIT_CREATED_DATE"]);
                        lobjPAPIT.icdoPayeeAccountPaymentItemType.ihstOldValues["modified_by"] = Convert.ToString(ldr["PAPIT_MODIFIED_BY"]);
                        lobjPAPIT.icdoPayeeAccountPaymentItemType.ihstOldValues["modified_date"] = Convert.ToDateTime(ldr["PAPIT_MODIFIED_DATE"]);

                        lobjPAPIT.ibusPaymentItemType = new busPaymentItemType { icdoPaymentItemType = new cdoPaymentItemType() };
                        lobjPAPIT.ibusPaymentItemType.icdoPaymentItemType.LoadData(ldr);

                        lobjPayeeAccount.iclbPayeeAccountPaymentItemType.Add(lobjPAPIT);
                        lobjPAPIT = null;
                    }                        

                    DataTable ldtbPayeeFedTaxWithholding = idtTaxWithholding.AsEnumerable().Where(o =>
                      o.Field<int>("payee_account_id") == lobjPayeeAccount.icdoPayeeAccount.payee_account_id &&
                       o.Field<string>("tax_identifier_value") != null &&
                        o.Field<string>("tax_identifier_value") == busConstant.PayeeAccountTaxIdentifierFedTax).AsDataTable();

                    lobjPayeeAccount.iclbPayeeAccountFedTaxWithHolding = lobjBase.GetCollection<busPayeeAccountTaxWithholding>(ldtbPayeeFedTaxWithholding, "icdoPayeeAccountTaxWithholding");

                    DataTable ldtbPayeeStateTaxWithholding = idtTaxWithholding.AsEnumerable().Where(o =>
                      o.Field<int>("payee_account_id") == lobjPayeeAccount.icdoPayeeAccount.payee_account_id &&
                       o.Field<string>("tax_identifier_value") != null &&
                        o.Field<string>("tax_identifier_value") == busConstant.PayeeAccountTaxIdentifierStateTax).AsDataTable();

                    lobjPayeeAccount.iclbPayeeAccountStateTaxWithHolding = lobjBase.GetCollection<busPayeeAccountTaxWithholding>(ldtbPayeeStateTaxWithholding, "icdoPayeeAccountTaxWithholding");
                }
                if (lobjPayeeAccount.iclbPayeeAccountPaymentItemType == null)
                    lobjPayeeAccount.LoadPayeeAccountPaymentItemType();
                if (lobjPayeeAccount.idecTotalTaxableAmountForVariableTaxExcludingRetro == 0.00M)
                    lobjPayeeAccount.LoadTaxableAmountForVariableTaxExcludeRetroItem(next_benefit_date);
                if (lobjPayeeAccount.idecBenefitAmount == 0.00M)
                    lobjPayeeAccount.LoadBenefitAmount();
                if (lobjPayeeAccount.iclbPayeeAccountFedTaxWithHolding == null)
                    lobjPayeeAccount.LoadFedTaxWithHoldingInfo();
                // ****** FEDERAL TAX CALCULATIONS ****** //
                bool lblnFedTaxItemExists = false;
                foreach (busPayeeAccountTaxWithholding lobjTaxWithholding in lobjPayeeAccount.iclbPayeeAccountFedTaxWithHolding)
                {
                    //PIR 25084 - Tax Otion Not Applicable Anymore
                    //if ((lobjTaxWithholding.icdoPayeeAccountTaxWithholding.tax_option_value == "FTIR") ||
                    //    (lobjTaxWithholding.icdoPayeeAccountTaxWithholding.tax_option_value == "FTIA"))
                    if (lobjTaxWithholding.icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionMonthlyBenefit)
                    {
                        if (lobjTaxWithholding.icdoPayeeAccountTaxWithholding.end_date == DateTime.MinValue)
                        {
                            lobjTaxWithholding.iblnSkipSave = true;

                            // Calculate Current Federal Tax
                            lobjPayeeAccount.icdoPayeeAccount.current_federal_tax = lobjTaxWithholding.CalculateW4PTaxWithholdingAmount(lobjPayeeAccount.idecTotalTaxableAmountForVariableTaxExcludingRetro,
                                                        next_benefit_date);

                            foreach (busPayeeAccountPaymentItemType lobjPAPaymentType in lobjPayeeAccount.iclbPayeeAccountPaymentItemType)
                            {
                                if (lobjPAPaymentType.ibusPaymentItemType == null)
                                    lobjPAPaymentType.LoadPaymentItemType();
                                if ((lobjPAPaymentType.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.PAPITFederalTaxAmount) &&
                                      (lobjPAPaymentType.icdoPayeeAccountPaymentItemType.end_date == DateTime.MinValue))
                                {
                                    lblnFedTaxItemExists = true;
                                    lobjPayeeAccount.icdoPayeeAccount.last_federal_tax += lobjPAPaymentType.icdoPayeeAccountPaymentItemType.amount;
                                    // Calculate the new Benefit amount with New Tax rate
                                    decimal ldecBenefitAmount = lobjPayeeAccount.idecBenefitAmount +
                                                                lobjPayeeAccount.icdoPayeeAccount.last_federal_tax -
                                                                lobjPayeeAccount.icdoPayeeAccount.current_federal_tax;
                                    // Check for Exceptions
                                    if (ldecBenefitAmount < federal_threshold_amount)
                                    {
                                        lobjPayeeAccount.icdoPayeeAccount.fed_tax_error_message = "Federal Tax amount is below threshhold";
                                        lobjPayeeAccount.icdoPayeeAccount.fed_tax_type_of_error = "Warning";
                                        // Add to Collection to print in Report
                                        CreateNewDataRow(lobjPayeeAccount, false, true);
                                        break;
                                    }
                                    else if (ldecBenefitAmount < 0)
                                    {
                                        lobjPayeeAccount.icdoPayeeAccount.fed_tax_error_message = "Amount is less than ZERO";
                                        lobjPayeeAccount.icdoPayeeAccount.fed_tax_type_of_error = "Error";
                                        CreateNewDataRow(lobjPayeeAccount, false, true);
                                        break;
                                    }
                                    // No Exceptions
                                    else
                                    {
                                        if (lobjPayeeAccount.icdoPayeeAccount.current_federal_tax != lobjPayeeAccount.icdoPayeeAccount.last_federal_tax)
                                        {
                                            if (lobjPAPaymentType.icdoPayeeAccountPaymentItemType.start_date < next_benefit_date)
                                            {
                                                lobjPAPaymentType.icdoPayeeAccountPaymentItemType.end_date = next_benefit_date.AddDays(-1);
                                                //as per meeting with satya on Aug 13,2010
                                                lobjPAPaymentType.icdoPayeeAccountPaymentItemType.batch_schedule_id = BatchSheduleID;
                                                //lobjPAPaymentType.icdoPayeeAccountPaymentItemType.modified_by = null;
                                                lobjPAPaymentType.icdoPayeeAccountPaymentItemType.Update();
                                                if (lobjPayeeAccount.icdoPayeeAccount.current_federal_tax != 0)
                                                {
                                                    int lintNewPAPITID = InsertPayeeAccountPaymentItemType(lobjPayeeAccount, false);
                                                    int lintPaymentItemTypeId = lobjPayeeAccount.GetPaymentItemTypeIDByItemCode(busConstant.PAPITFederalTaxAmount);
                                                    InsertTaxWithholdingItemDetail(
                                                        lobjTaxWithholding.icdoPayeeAccountTaxWithholding.payee_account_tax_withholding_id,
                                                        lintNewPAPITID, lobjPayeeAccount.icdoPayeeAccount.current_federal_tax, lintPaymentItemTypeId);
                                                }
                                            }
                                            else
                                            {
                                                lobjPAPaymentType.icdoPayeeAccountPaymentItemType.amount = lobjPayeeAccount.icdoPayeeAccount.current_federal_tax;
                                                lobjPAPaymentType.icdoPayeeAccountPaymentItemType.batch_schedule_id = BatchSheduleID;
                                                //lobjPAPaymentType.icdoPayeeAccountPaymentItemType.modified_by = null;
                                                lobjPAPaymentType.icdoPayeeAccountPaymentItemType.Update();
                                                lobjTaxWithholding.LoadTaxWithHoldingTaxItems();
                                                busPayeeAccountTaxWithholdingItemDetail lobjDetail =
                                                    lobjTaxWithholding.iclbTaxWithHoldingTaxItems
                                                    .Where(o => o.icdoPayeeAccountTaxWithholdingItemDetail.payee_account_payment_item_type_id ==
                                                        lobjPAPaymentType.icdoPayeeAccountPaymentItemType.payee_account_payment_item_type_id).FirstOrDefault();
                                                if (lobjDetail != null)
                                                {
                                                    lobjDetail.icdoPayeeAccountTaxWithholdingItemDetail.amount = lobjPAPaymentType.icdoPayeeAccountPaymentItemType.amount;
                                                    lobjDetail.icdoPayeeAccountTaxWithholdingItemDetail.Update();
                                                }
                                            }
                                            // Add to Collection to print in Report
                                            CreateNewDataRow(lobjPayeeAccount, false, false);
                                        }
                                    }
                                }
                            }
                            if (!lblnFedTaxItemExists && lobjPayeeAccount.icdoPayeeAccount.current_federal_tax > 0)
                            {
                                int lintNewPAPITID = InsertPayeeAccountPaymentItemType(lobjPayeeAccount, false);
                                int lintPaymentItemTypeId = lobjPayeeAccount.GetPaymentItemTypeIDByItemCode(busConstant.PAPITFederalTaxAmount);
                                InsertTaxWithholdingItemDetail(
                                    lobjTaxWithholding.icdoPayeeAccountTaxWithholding.payee_account_tax_withholding_id,
                                    lintNewPAPITID, lobjPayeeAccount.icdoPayeeAccount.current_federal_tax, lintPaymentItemTypeId);
                                // Add to Collection to print in Report
                                CreateNewDataRow(lobjPayeeAccount, false, false);
                            }
                        }
                    }
                }
                if (!ablnDoNotRecalculateStateTax)
                {
                    // ****** STATE TAX CALCULATIONS ****** //
                    if (lobjPayeeAccount.iclbPayeeAccountStateTaxWithHolding == null)
                        lobjPayeeAccount.LoadStateTaxWithHoldingInfo();
                    bool lblnStateTaxItemExists = false;
                    foreach (busPayeeAccountTaxWithholding lobjTaxWithholding in lobjPayeeAccount.iclbPayeeAccountStateTaxWithHolding)
                    {
                        if ((lobjTaxWithholding.icdoPayeeAccountTaxWithholding.tax_option_value == "RSTS") ||
                            (lobjTaxWithholding.icdoPayeeAccountTaxWithholding.tax_option_value == "STST"))
                        {
                            if (lobjTaxWithholding.icdoPayeeAccountTaxWithholding.end_date == DateTime.MinValue)
                            {
                                string lstrMaritalStatus = string.Empty;
                                if (lobjTaxWithholding.icdoPayeeAccountTaxWithholding.marital_status_value ==
                                    busConstant.MaritalStatusMarriedWithholdAtSingleRate)
                                    lstrMaritalStatus = busConstant.PersonMaritalStatusSingle;
                                else
                                    lstrMaritalStatus = lobjTaxWithholding.icdoPayeeAccountTaxWithholding.marital_status_value;
                                // Calculate Current State Tax
                                lobjPayeeAccount.icdoPayeeAccount.current_state_tax = busPayeeAccountHelper.CalculateFedOrStateTax(
                                                            lobjPayeeAccount.idecTotalTaxableAmountForVariableTaxExcludingRetro,
                                                            lobjTaxWithholding.icdoPayeeAccountTaxWithholding.tax_allowance,
                                                            next_benefit_date, lstrMaritalStatus,
                                                            busConstant.PayeeAccountTaxIdentifierStateTax,
                                                            lobjTaxWithholding.icdoPayeeAccountTaxWithholding.additional_tax_amount);
                                foreach (busPayeeAccountPaymentItemType lobjPAPaymentType in lobjPayeeAccount.iclbPayeeAccountPaymentItemType)
                                {
                                    if (lobjPAPaymentType.ibusPaymentItemType == null)
                                        lobjPAPaymentType.LoadPaymentItemType();
                                    if ((lobjPAPaymentType.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.PAPITNDStateTaxAmount) &&
                                          (lobjPAPaymentType.icdoPayeeAccountPaymentItemType.end_date == DateTime.MinValue))
                                    {
                                        lblnStateTaxItemExists = true;
                                        // Calculate the new Benefit amount with New Tax rate
                                        lobjPayeeAccount.icdoPayeeAccount.last_state_tax += lobjPAPaymentType.icdoPayeeAccountPaymentItemType.amount;
                                        decimal ldecBenefitAmount = lobjPayeeAccount.idecBenefitAmount + lobjPayeeAccount.icdoPayeeAccount.last_state_tax -
                                            lobjPayeeAccount.icdoPayeeAccount.current_state_tax;
                                        // Check for Exceptions
                                        if (ldecBenefitAmount < state_threshold_amount)
                                        {
                                            lobjPayeeAccount.icdoPayeeAccount.state_tax_error_message = "State Tax amount is below threshhold";
                                            lobjPayeeAccount.icdoPayeeAccount.state_tax_type_of_error = "Warning";
                                            CreateNewDataRow(lobjPayeeAccount, true, true);
                                            break;
                                        }
                                        else if (ldecBenefitAmount < 0)
                                        {
                                            lobjPayeeAccount.icdoPayeeAccount.state_tax_error_message = "Amount is less than ZERO";
                                            lobjPayeeAccount.icdoPayeeAccount.state_tax_type_of_error = "Error";
                                            CreateNewDataRow(lobjPayeeAccount, true, true);
                                            break;
                                        }
                                        // No Exceptions
                                        else
                                        {
                                            if (lobjPayeeAccount.icdoPayeeAccount.current_state_tax != lobjPayeeAccount.icdoPayeeAccount.last_state_tax)
                                            {
                                                if (lobjPAPaymentType.icdoPayeeAccountPaymentItemType.start_date < next_benefit_date)
                                                {
                                                    lobjPAPaymentType.icdoPayeeAccountPaymentItemType.end_date = next_benefit_date.AddDays(-1);
                                                    //as per meeting with satya on Aug 13,2010
                                                    lobjPAPaymentType.icdoPayeeAccountPaymentItemType.batch_schedule_id = BatchSheduleID;
                                                    //lobjPAPaymentType.icdoPayeeAccountPaymentItemType.modified_by = null;
                                                    lobjPAPaymentType.icdoPayeeAccountPaymentItemType.Update();
                                                    if (lobjPayeeAccount.icdoPayeeAccount.current_state_tax != 0)
                                                    {
                                                        int lintNewPAPITID = InsertPayeeAccountPaymentItemType(lobjPayeeAccount, true);
                                                        int lintPaymentItemTypeId = lobjPayeeAccount.GetPaymentItemTypeIDByItemCode(busConstant.PAPITNDStateTaxAmount);
                                                        InsertTaxWithholdingItemDetail(
                                                            lobjTaxWithholding.icdoPayeeAccountTaxWithholding.payee_account_tax_withholding_id,
                                                            lintNewPAPITID, lobjPayeeAccount.icdoPayeeAccount.current_state_tax, lintPaymentItemTypeId);
                                                    }
                                                }
                                                else
                                                {
                                                    lobjPAPaymentType.icdoPayeeAccountPaymentItemType.amount = lobjPayeeAccount.icdoPayeeAccount.current_state_tax;
                                                    lobjPAPaymentType.icdoPayeeAccountPaymentItemType.batch_schedule_id = BatchSheduleID;
                                                    //lobjPAPaymentType.icdoPayeeAccountPaymentItemType.modified_by = null;
                                                    lobjPAPaymentType.icdoPayeeAccountPaymentItemType.Update();
                                                    lobjTaxWithholding.LoadTaxWithHoldingTaxItems();
                                                    busPayeeAccountTaxWithholdingItemDetail lobjDetail =
                                                        lobjTaxWithholding.iclbTaxWithHoldingTaxItems
                                                        .Where(o => o.icdoPayeeAccountTaxWithholdingItemDetail.payee_account_payment_item_type_id ==
                                                            lobjPAPaymentType.icdoPayeeAccountPaymentItemType.payee_account_payment_item_type_id).FirstOrDefault();
                                                    if (lobjDetail != null)
                                                    {
                                                        lobjDetail.icdoPayeeAccountTaxWithholdingItemDetail.amount = lobjPAPaymentType.icdoPayeeAccountPaymentItemType.amount;
                                                        lobjDetail.icdoPayeeAccountTaxWithholdingItemDetail.Update();
                                                    }
                                                }
                                                // Add to Collection to print in Report
                                                CreateNewDataRow(lobjPayeeAccount, true, false);
                                            }
                                        }
                                    }
                                }
                                if (!lblnStateTaxItemExists && lobjPayeeAccount.icdoPayeeAccount.current_state_tax > 0)
                                {
                                    int lintNewPAPITID = InsertPayeeAccountPaymentItemType(lobjPayeeAccount, true);
                                    int lintPaymentItemTypeId = lobjPayeeAccount.GetPaymentItemTypeIDByItemCode(busConstant.PAPITNDStateTaxAmount);
                                    InsertTaxWithholdingItemDetail(
                                        lobjTaxWithholding.icdoPayeeAccountTaxWithholding.payee_account_tax_withholding_id,
                                        lintNewPAPITID, lobjPayeeAccount.icdoPayeeAccount.current_state_tax, lintPaymentItemTypeId);
                                    // Add to Collection to print in Report
                                    CreateNewDataRow(lobjPayeeAccount, true, false);
                                }
                            }
                        }
                    }
                }
            }
        }
        
        // Create Federal Tax Rate Change Report
        private void CreateFederalTaxRateChangeReport()
        {
            idlgUpdateProcessLog("Creating Federal Tax Rate Change Report", "INFO", istrProcessName);            
            CreateReport("rptFederalTaxRateChange.rpt", idtFederalTaxChanges);
            idlgUpdateProcessLog("Successfully Created Federal Tax Rate Change Report", "INFO", istrProcessName);
        }

        // Create State Tax Rate Change Report
        private void CreateStateTaxRateChangeReport()
        {
            idlgUpdateProcessLog("Creating State Tax Rate Change Report", "INFO", istrProcessName);           
            CreateReport("rptStateTaxRateChange.rpt", idtStateTaxChanges);
            idlgUpdateProcessLog("Successfully Created State Tax Rate Change Report", "INFO", istrProcessName);
        }

        // Create Tax Exception Report
        private void CreateExceptionReport()
        {
            idlgUpdateProcessLog("Creating Tax Rate Change Exception Report", "INFO", istrProcessName);          
            CreateReport("rptTaxExceptionReport.rpt", idtTaxExceptions);
            idlgUpdateProcessLog("Successfully Created Tax Rate Change Exception Report", "INFO", istrProcessName);
        }

        private DataTable CreateNewDataTable()
        {
            DataTable ldtbTaxRateChange = new DataTable();
            DataColumn ldtcFullName = new DataColumn("FullName", Type.GetType("System.String"));
            DataColumn ldtcPERSLinkID = new DataColumn("PERSLinkID", Type.GetType("System.Int32"));
            DataColumn ldtcBeforeRate = new DataColumn("BeforeRate", Type.GetType("System.Decimal"));
            DataColumn ldtcAfterRate = new DataColumn("AfterRate", Type.GetType("System.Decimal"));
            DataColumn ldtcDifference = new DataColumn("Difference", Type.GetType("System.Decimal"));
            DataColumn ldtcMessage = new DataColumn("Message", Type.GetType("System.String"));
            DataColumn ldtcTypeOfMessage = new DataColumn("TypeOfMessage", Type.GetType("System.String"));
            ldtbTaxRateChange.Columns.Add(ldtcFullName);
            ldtbTaxRateChange.Columns.Add(ldtcPERSLinkID);
            ldtbTaxRateChange.Columns.Add(ldtcBeforeRate);
            ldtbTaxRateChange.Columns.Add(ldtcAfterRate);
            ldtbTaxRateChange.Columns.Add(ldtcDifference);
            ldtbTaxRateChange.Columns.Add(ldtcMessage);
            ldtbTaxRateChange.Columns.Add(ldtcTypeOfMessage);
            ldtbTaxRateChange.TableName = busConstant.ReportTableName;
            return ldtbTaxRateChange;
        }

        private void CreateNewDataRow(busPayeeAccount AobjbusPayeeAccount, bool AblnIsStateTax, bool AblnIsException)
        {
            DataRow ldtrRow;
            if (AblnIsException)
                ldtrRow = idtTaxExceptions.NewRow();
            else if (AblnIsStateTax)
                ldtrRow = idtStateTaxChanges.NewRow();
            else
                ldtrRow = idtFederalTaxChanges.NewRow();
            if (AobjbusPayeeAccount.icdoPayeeAccount.payee_perslink_id != 0)
            {
                ldtrRow["FullName"] = AobjbusPayeeAccount.ibusPayee.icdoPerson.FullName;
                ldtrRow["PERSLinkID"] = AobjbusPayeeAccount.ibusPayee.icdoPerson.person_id;
            }
            if (AobjbusPayeeAccount.icdoPayeeAccount.payee_org_id != 0)
            {
                ldtrRow["FullName"] = AobjbusPayeeAccount.ibusRecipientOrganization.icdoOrganization.RecipientOrg;
                ldtrRow["PERSLinkID"] = Convert.ToInt32(AobjbusPayeeAccount.ibusRecipientOrganization.icdoOrganization.org_code);
            }
            if (AblnIsException)
            {
                if (AobjbusPayeeAccount.icdoPayeeAccount.last_state_tax == 0)
                    ldtrRow["BeforeRate"] = AobjbusPayeeAccount.icdoPayeeAccount.last_federal_tax;
                else
                    ldtrRow["BeforeRate"] = AobjbusPayeeAccount.icdoPayeeAccount.last_state_tax;
                if (AblnIsStateTax)
                {
                    ldtrRow["Message"] = AobjbusPayeeAccount.icdoPayeeAccount.state_tax_error_message;
                    ldtrRow["TypeOfMessage"] = AobjbusPayeeAccount.icdoPayeeAccount.state_tax_type_of_error;
                }
                else
                {
                    ldtrRow["Message"] = AobjbusPayeeAccount.icdoPayeeAccount.fed_tax_error_message;
                    ldtrRow["TypeOfMessage"] = AobjbusPayeeAccount.icdoPayeeAccount.fed_tax_type_of_error;
                }
            }
            else
            {
                if (AblnIsStateTax)
                {
                    ldtrRow["BeforeRate"] = AobjbusPayeeAccount.icdoPayeeAccount.last_state_tax;
                    ldtrRow["AfterRate"] = AobjbusPayeeAccount.icdoPayeeAccount.current_state_tax;
                    ldtrRow["Difference"] = AobjbusPayeeAccount.icdoPayeeAccount.state_difference;
                }
                else
                {
                    ldtrRow["BeforeRate"] = AobjbusPayeeAccount.icdoPayeeAccount.last_federal_tax;
                    ldtrRow["AfterRate"] = AobjbusPayeeAccount.icdoPayeeAccount.current_federal_tax;
                    ldtrRow["Difference"] = AobjbusPayeeAccount.icdoPayeeAccount.federal_difference;
                }
            }
            if (AblnIsException)
                idtTaxExceptions.Rows.Add(ldtrRow);
            else if (AblnIsStateTax)
                idtStateTaxChanges.Rows.Add(ldtrRow);
            else
                idtFederalTaxChanges.Rows.Add(ldtrRow);
        }

        private int InsertPayeeAccountPaymentItemType(busPayeeAccount AobjPayeeAccount, bool AblnIsStateTax)
        {
            busPayeeAccountPaymentItemType lobjPAPaymentItemType = new busPayeeAccountPaymentItemType();
            lobjPAPaymentItemType.icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType();
            lobjPAPaymentItemType.icdoPayeeAccountPaymentItemType.payee_account_id = AobjPayeeAccount.icdoPayeeAccount.payee_account_id;
            if (AblnIsStateTax)
            {
                int lintVendorOrgId = busPayeeAccountHelper.GetStateTaxVendorID();
           
                lobjPAPaymentItemType.icdoPayeeAccountPaymentItemType.payment_item_type_id
                    = AobjPayeeAccount.GetPaymentItemTypeIDByItemCode(busConstant.PAPITNDStateTaxAmount);
                lobjPAPaymentItemType.icdoPayeeAccountPaymentItemType.amount = AobjPayeeAccount.icdoPayeeAccount.current_state_tax;
                lobjPAPaymentItemType.icdoPayeeAccountPaymentItemType.vendor_org_id = lintVendorOrgId;
            }
            else
            {
                int lintVendorOrgId = busPayeeAccountHelper.GetFedTaxVendorID();
                     
                lobjPAPaymentItemType.icdoPayeeAccountPaymentItemType.payment_item_type_id
                    = AobjPayeeAccount.GetPaymentItemTypeIDByItemCode(busConstant.PAPITFederalTaxAmount);
                lobjPAPaymentItemType.icdoPayeeAccountPaymentItemType.amount = AobjPayeeAccount.icdoPayeeAccount.current_federal_tax;
                lobjPAPaymentItemType.icdoPayeeAccountPaymentItemType.vendor_org_id = lintVendorOrgId;
            }
            lobjPAPaymentItemType.icdoPayeeAccountPaymentItemType.batch_schedule_id = BatchSheduleID;
            lobjPAPaymentItemType.icdoPayeeAccountPaymentItemType.start_date = next_benefit_date;
            lobjPAPaymentItemType.icdoPayeeAccountPaymentItemType.Insert();
            return lobjPAPaymentItemType.icdoPayeeAccountPaymentItemType.payee_account_payment_item_type_id;
        }

        public void UpdateTaxWitholdingDetail(int aintOldPAPITID, int aintNewPAPITID, decimal adecAmount)
        {
            DataTable ldtbResult = busBase.Select<cdoPayeeAccountTaxWithholdingItemDetail>(
                                        new string[1] { "payee_account_payment_item_type_id" },
                                        new object[1] { aintOldPAPITID }, null, null);
            foreach (DataRow dr in ldtbResult.Rows)
            {
                cdoPayeeAccountTaxWithholdingItemDetail lcdoPayeeAccountTaxWitholdingDetail = new cdoPayeeAccountTaxWithholdingItemDetail();
                lcdoPayeeAccountTaxWitholdingDetail.LoadData(dr);
                lcdoPayeeAccountTaxWitholdingDetail.payee_account_payment_item_type_id = aintNewPAPITID;
                lcdoPayeeAccountTaxWitholdingDetail.amount = adecAmount;
                lcdoPayeeAccountTaxWitholdingDetail.Update();
            }
        }

        public void UpdateRolloverItemDetail(int aintOldPAPITID, int aintNewPAPITID, decimal adecAmount)
        {
            DataTable ldtbResult = busBase.Select<cdoPayeeAccountRolloverItemDetail>(
                            new string[1] { "payee_account_payment_item_type_id" },
                            new object[1] { aintOldPAPITID }, null, null);
            foreach (DataRow dr in ldtbResult.Rows)
            {
                cdoPayeeAccountRolloverItemDetail lcdoPARolloverItemDetail = new cdoPayeeAccountRolloverItemDetail();
                lcdoPARolloverItemDetail.LoadData(dr);
                lcdoPARolloverItemDetail.payee_account_payment_item_type_id = aintNewPAPITID;
                lcdoPARolloverItemDetail.Update();
            }
        }

        /*public void InsertTaxWithholdingItemDetail(int aintOldPAPITID, int aintNewPAPITID, decimal adecAmount)
        {
            DataTable ldtbResult = busBase.Select<cdoPayeeAccountTaxWithholdingItemDetail>(new String[1] { "PAYEE_ACCOUNT_PAYMENT_ITEM_TYPE_ID" },
                                            new object[1] { aintOldPAPITID }, null, null);
            foreach (DataRow ldr in ldtbResult.Rows)
            {
                busPayeeAccountTaxWithholdingItemDetail lobjTaxWithholdingItemDetail = new busPayeeAccountTaxWithholdingItemDetail();
                lobjTaxWithholdingItemDetail.icdoPayeeAccountTaxWithholdingItemDetail = new cdoPayeeAccountTaxWithholdingItemDetail();
                lobjTaxWithholdingItemDetail.icdoPayeeAccountTaxWithholdingItemDetail.LoadData(ldr);

                busPayeeAccountTaxWithholdingItemDetail lobjTaxwithholdingDetail = new busPayeeAccountTaxWithholdingItemDetail();
                lobjTaxwithholdingDetail.icdoPayeeAccountTaxWithholdingItemDetail = new cdoPayeeAccountTaxWithholdingItemDetail();
                lobjTaxwithholdingDetail.icdoPayeeAccountTaxWithholdingItemDetail = lobjTaxWithholdingItemDetail.icdoPayeeAccountTaxWithholdingItemDetail;
                lobjTaxwithholdingDetail.icdoPayeeAccountTaxWithholdingItemDetail.payee_account_payment_item_type_id = aintNewPAPITID;
                lobjTaxwithholdingDetail.icdoPayeeAccountTaxWithholdingItemDetail.amount = adecAmount;
                //Reseting the audit log fields
                lobjTaxwithholdingDetail.icdoPayeeAccountTaxWithholdingItemDetail.created_by = null;
                lobjTaxwithholdingDetail.icdoPayeeAccountTaxWithholdingItemDetail.created_date = DateTime.MinValue;
                lobjTaxwithholdingDetail.icdoPayeeAccountTaxWithholdingItemDetail.modified_by = null;
                lobjTaxwithholdingDetail.icdoPayeeAccountTaxWithholdingItemDetail.modified_date = DateTime.MinValue;
                lobjTaxwithholdingDetail.icdoPayeeAccountTaxWithholdingItemDetail.update_seq = 0;
                lobjTaxwithholdingDetail.icdoPayeeAccountTaxWithholdingItemDetail.Insert();
            }
        }*/

        public void InsertTaxWithholdingItemDetail(int aintTaxwithholdingID, int aintNewPAPITID, decimal adecAmount, int aintPaymentTypeID)
        {
            busPayeeAccountTaxWithholdingItemDetail lobjTaxwithholdingDetail = new busPayeeAccountTaxWithholdingItemDetail();
            lobjTaxwithholdingDetail.icdoPayeeAccountTaxWithholdingItemDetail = new cdoPayeeAccountTaxWithholdingItemDetail();
            lobjTaxwithholdingDetail.icdoPayeeAccountTaxWithholdingItemDetail.payee_account_payment_item_type_id = aintNewPAPITID;
            lobjTaxwithholdingDetail.icdoPayeeAccountTaxWithholdingItemDetail.amount = adecAmount;
            lobjTaxwithholdingDetail.icdoPayeeAccountTaxWithholdingItemDetail.payee_account_tax_withholding_id = aintTaxwithholdingID;
            lobjTaxwithholdingDetail.icdoPayeeAccountTaxWithholdingItemDetail.payment_item_type_id = aintPaymentTypeID;
            lobjTaxwithholdingDetail.icdoPayeeAccountTaxWithholdingItemDetail.Insert();
        }
    }
}
