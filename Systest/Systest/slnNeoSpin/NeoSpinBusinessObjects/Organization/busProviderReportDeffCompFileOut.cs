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
using Sagitec.ExceptionPub;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busProviderReportDeffCompFileOut : busFileBaseOut
    {
        public busProviderReportDeffCompFileOut()
        {
        }

        private int lintEmptyRowCount = 0;
        private string lstrProviderOrgCodeID = string.Empty;
        private int lintProviderOrgID;

        private Collection<busProviderReportDataDeffComp> _iclbProviderReportDataDeffComp;
        public Collection<busProviderReportDataDeffComp> iclbProviderReportDataDeffComp
        {
            get { return _iclbProviderReportDataDeffComp; }
            set { _iclbProviderReportDataDeffComp = value; }
        }

        private string _lintBatchControlRecordCount;
        public string lintBatchControlRecordCount
        {
            get
            {
                if (_iclbProviderReportDataDeffComp != null)
                    _lintBatchControlRecordCount = Convert.ToString(_iclbProviderReportDataDeffComp.Count);
                return _lintBatchControlRecordCount.PadLeft(6, '0');
            }
        }

        private string _lintFileControlRecordCount;
        public string lintFileControlRecordCount
        {
            get
            {
                if (_iclbProviderReportDataDeffComp != null)
                    _lintFileControlRecordCount = Convert.ToString(_iclbProviderReportDataDeffComp.Count);
                return _lintFileControlRecordCount.PadLeft(8, '0');
            }
        }

        private string _lintKansasRecordCount;
        public string lintKansasRecordCount
        {
            get
            {
                if (_iclbProviderReportDataDeffComp != null)
                    _lintKansasRecordCount = Convert.ToString(_iclbProviderReportDataDeffComp.Count);
                return _lintKansasRecordCount.PadLeft(9, '0');
            }
        }

        private DateTime _ldtTodaysDate;
        public DateTime ldtTodaysDate
        {
            get
            {
                _ldtTodaysDate = DateTime.Now;
                return _ldtTodaysDate;
            }
        }

        private int _lintBlockCount;
        public int lintBlockCount
        {
            get { return _lintBlockCount; }
            set { _lintBlockCount = value; }
        }

        private string _lstrBlockCount;
        public string lstrBlockCount
        {
            get
            {
                _lstrBlockCount = Convert.ToString(_lintBlockCount);
                return _lstrBlockCount.PadLeft(6, '0');
            }
        }

        private int _lintTotalRoutingNo;
        public int lintTotalRoutingNo
        {
            get { return _lintTotalRoutingNo; }
            set { _lintTotalRoutingNo = value; }
        }

        private string _lstrEntryHash;
        public string lstrEntryHash
        {
            get
            {
                _lstrEntryHash = Convert.ToString(_lintTotalRoutingNo);
                return _lstrEntryHash.PadLeft(10, '0');
            }
        }

        public int iintTotalRoutingNo { get; set; }

        public string istrEntryHash
        {
            get
            {
                return iintTotalRoutingNo.ToString().PadLeft(10, '0');
            }
        }

        public void LoadTotalRoutingNo()
        {
            if (_iclbProviderReportDataDeffComp != null)
            {
                int lintRoutingNo = Convert.ToInt32(DBFunction.DBExecuteScalar("cdoProviderReportDataDeffComp.GetRoutingNo", new object[1] { lintProviderOrgID },
                                                        iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
                _lintTotalRoutingNo = lintRoutingNo * (_iclbProviderReportDataDeffComp.Count);
                string lstrRoutingNo = lintRoutingNo.ToString().PadLeft(9, '0');
                int lintFirst8DigitsofRoutingNo = Convert.ToInt32(lstrRoutingNo.Substring(0, lstrRoutingNo.Length - 1).PadLeft(8, '0'));

                if (ibusOrgCodeValue.icdoCodeValue.code_value == busConstant.Provider_WaddellAndReed)
                    iintTotalRoutingNo = busConstant.WaddelDFINumber * iclbProviderReportDataDeffComp.Count;
                else if (ibusOrgCodeValue.icdoCodeValue.code_value == busConstant.Provider_BankOfNorthDakota)
                    iintTotalRoutingNo = busConstant.BNDDFINumber * iclbProviderReportDataDeffComp.Count;
                else
                    iintTotalRoutingNo = lintFirst8DigitsofRoutingNo * iclbProviderReportDataDeffComp.Count;      
            }
        }

        private decimal _ldclTotalDebitAmount;
        public decimal ldclTotalDebitAmount
        {
            get { return _ldclTotalDebitAmount; }
            set { _ldclTotalDebitAmount = value; }
        }

        private decimal _ldclTotalCreditAmount;
        public decimal ldclTotalCreditAmount
        {
            get { return _ldclTotalCreditAmount; }
            set { _ldclTotalCreditAmount = value; }
        }

        public string ldclTotalDebitAmountFormatted
        {
            get
            {
                return (Convert.ToInt32((ldclTotalDebitAmount * 100))).ToString().PadLeft(12, '0');
            }
        }
        public string ldclTotalCreditAmounttFormatted
        {
            get
            {
                return (Convert.ToInt32((ldclTotalCreditAmount * 100))).ToString().PadLeft(12, '0');
            }
        }

        public void LoadTotalDebitAndCreditAmount()
        {
            if (_iclbProviderReportDataDeffComp != null)
            {
                foreach (busProviderReportDataDeffComp lobjDeffComp in _iclbProviderReportDataDeffComp)
                {
                    //IF AMOUNT IS NEGATIVE, ITS CREDIT FOR MEMBER AND DEBIT FOR PROVIDER AND FILE IS SEND TO PROVIDER
                    if (lobjDeffComp.icdoProviderReportDataDeffComp.contribution_amount > 0)
                        _ldclTotalCreditAmount += lobjDeffComp.icdoProviderReportDataDeffComp.contribution_amount;
                    else
                        _ldclTotalDebitAmount += lobjDeffComp.icdoProviderReportDataDeffComp.contribution_amount;
                }
            }
        }

        public string lstrTotalAmount
        {
            get
            {
                decimal ldclTemp = _ldclTotalAmount * 100;
                return ldclTemp.ToString("#").PadLeft(15, '0');
            }
        }

        private decimal _ldclTotalAmount;
        public decimal ldclTotalAmount
        {
            get { return _ldclTotalAmount; }
            set { _ldclTotalAmount = value; }
        }

        private void LoadTotalAmount()
        {
            if (_iclbProviderReportDataDeffComp != null)
            {
                foreach (busProviderReportDataDeffComp lobjDeffComp in _iclbProviderReportDataDeffComp)
                {
                    _ldclTotalAmount += lobjDeffComp.icdoProviderReportDataDeffComp.contribution_amount;
                }
            }
        }

        private string _lstrServiceClassCode;
        public string lstrServiceClassCode
        {
            get { return _lstrServiceClassCode; }
            set { _lstrServiceClassCode = value; }
        }
        public string istrACHFileName { get; set; }

        public void LoadServiceClassCode()
        {
            if (_iclbProviderReportDataDeffComp != null)
            {
                int lintNegAdjCount = 0;
                foreach (busProviderReportDataDeffComp lobjDeffComp in _iclbProviderReportDataDeffComp)
                {
                    if (lobjDeffComp.icdoProviderReportDataDeffComp.contribution_amount < 0)
                        lintNegAdjCount += 1;
                }
                //IF AMOUNT IS NEGATIVE, ITS CREDIT FOR MEMBER AND DEBIT FOR PROVIDER AND FILE IS SEND TO PROVIDER, SO SHOULD BE DEBIT
                if (lintNegAdjCount == _iclbProviderReportDataDeffComp.Count)
                    _lstrServiceClassCode = busConstant.ServiceCode_DebitsOnly;
                else if (lintNegAdjCount == 0)
                    _lstrServiceClassCode = busConstant.ServiceCode_CreditsOnly;
                else
                    _lstrServiceClassCode = busConstant.ServiceCode_CreditDebitMixed;
            }
        }

        public busCodeValue ibusOrgCodeValue { get; set; }

        private void LoadOrgCodeValue()
        {
            if (ibusOrgCodeValue.IsNull()) ibusOrgCodeValue = new busCodeValue { icdoCodeValue = new cdoCodeValue() };
            Collection<busCodeValue> lclbCodeValue = busGlobalFunctions.LoadCodeValueByData1(5012, lstrProviderOrgCodeID);
            if (lclbCodeValue.Count > 0)
            {
                ibusOrgCodeValue = lclbCodeValue[0];
            }
        }

        public override void InitializeFile()
        {
            string lstrFileName = string.Empty;
            string lstrCodeValue = ibusOrgCodeValue.icdoCodeValue.code_value;
            if (lstrCodeValue == busConstant.Provider_HartFordLife || lstrCodeValue == busConstant.Provider_SYMETRA ||
                lstrCodeValue == busConstant.Provider_ING || lstrCodeValue == busConstant.Provider_Fidelity || lstrCodeValue == busConstant.Provider_KEMPER)
                lstrFileName = DateTime.Now.ToString(busConstant.DateFormat) + "_" + lstrProviderOrgCodeID + busConstant.FileFormatcsv;
            else
                lstrFileName = DateTime.Now.ToString(busConstant.DateFormat) + "_" + lstrProviderOrgCodeID + busConstant.FileFormattxt;
            switch (ibusOrgCodeValue.icdoCodeValue.code_value)
            {
                case busConstant.Provider_AXA:
                    istrFileName = "DF.ER666711_" + lstrFileName;
                    break;
                case busConstant.Provider_Fidelity:
                    istrFileName = "DF.ER666712_" + lstrFileName;
                    break;
                case busConstant.Provider_HartFordLife:
                    istrFileName = "DF.ER666713_" + lstrFileName;
                    break;
                case busConstant.Provider_ING:
                    istrFileName = "DF.ER666714_" + lstrFileName;
                    break;
                case busConstant.Provider_SYMETRA:
                    istrFileName = "DF.ER666715_" + lstrFileName;
                    break;
                case busConstant.Provider_AIGVALIC:
                    istrFileName = "DF.ER666716_" + lstrFileName;
                    break;
                case busConstant.Provider_NationWideLife:
                    istrFileName = "DF.ER666717_" + lstrFileName;
                    break;
                case busConstant.Provider_KANSAS:
                    istrFileName = "DF.ER666718_" + lstrFileName;
                    break;
                case busConstant.Provider_AmericanTrustCenter:
                    istrFileName = "DF.ER666719_" + lstrFileName;
                    break;
                case busConstant.Provider_JacksonNationalLife:
                    istrFileName = "DF.ER666722_" + lstrFileName;
                    break;
                case busConstant.Provider_KEMPER:
                    istrFileName = "DF.ER666723_" + lstrFileName;
                    break;
                case busConstant.Provider_LincolnNational:
                    istrFileName = "DF.ER666724_" + lstrFileName;
                    break;
                case busConstant.Provider_WaddellAndReed:
                    FileName="DF.BK870061";
                    istrFileName = "DF.BK870061_" + lstrFileName;
                    break;
                case busConstant.Provider_BankOfNorthDakota:
                     FileName = "DF.BK870069";
                    istrFileName = "DF.BK870069_" + lstrFileName;
                    break;
                default:
                    break;
            }
        }

        public void LoadProviderReportDataDeffComp(DataTable ldtbProviderDeffComp)
        {
            lstrProviderOrgCodeID = Convert.ToString(iarrParameters[0]);
            lintProviderOrgID = busGlobalFunctions.GetOrgIdFromOrgCode(lstrProviderOrgCodeID);
            DateTime ldtPaymentDate = Convert.ToDateTime(iarrParameters[1]);
            istrFromEmail = Convert.ToString(iarrParameters[2]);
            _iclbProviderReportDataDeffComp = new Collection<busProviderReportDataDeffComp>();
            LoadOrgCodeValue();
            DataTable ldtbProviderReportData = busBase.Select("cdoProviderReportDataDeffComp.LoadReportDataByProvider",
                new object[2] { lintProviderOrgID, ldtPaymentDate });
             DataTable ldtbDefProviderData = new DataTable();
            //prod pir 4788
            if (ibusOrgCodeValue.icdoCodeValue.code_value == busConstant.Provider_Fidelity)
            {
                ldtbDefProviderData = busBase.Select("cdoProviderReportDataDeffComp.LoadDeffCompProviderDetails",
                new object[2] { lintProviderOrgID, ldtPaymentDate });
            }
            int lintDetailCounter = 1, lintPersonAccountID = 0, lintProviderOrgPlanID = 0;
            foreach (DataRow dr in ldtbProviderReportData.Rows)
            {
                lintPersonAccountID = lintProviderOrgPlanID = 0;
                busProviderReportDataDeffComp lobjDeffComp = new busProviderReportDataDeffComp();
                lobjDeffComp.icdoProviderReportDataDeffComp = new cdoProviderReportDataDeffComp();
                lobjDeffComp.icdoProviderReportDataDeffComp.LoadData(dr);
                if (ibusOrgCodeValue.icdoCodeValue.code_value == busConstant.Provider_Fidelity)
                {
                    lobjDeffComp.icdoProviderReportDataDeffComp.org_code = lstrProviderOrgCodeID;
                    //prod pir 4788
                    //--Start--//
                    lintPersonAccountID = dr["person_account_id"] == DBNull.Value ? 0 : Convert.ToInt32(dr["person_account_id"]);
                    lintProviderOrgPlanID = dr["org_plan_id"] == DBNull.Value ? 0 : Convert.ToInt32(dr["org_plan_id"]);
                    DataTable ldtFilteredDefCompProvd = ldtbDefProviderData.AsEnumerable()
                                                            .Where(o => o.Field<int?>("person_account_id") == lintPersonAccountID &&
                                                                o.Field<int?>("provider_org_plan_id") == lintProviderOrgPlanID)
                                                            .AsDataTable();
                    if (ldtFilteredDefCompProvd.Rows.Count > 0)
                    {
                        if (ldtFilteredDefCompProvd.Rows[0]["mutual_fund_window_flag"] != DBNull.Value &&
                            ldtFilteredDefCompProvd.Rows[0]["mutual_fund_window_flag"].ToString() == busConstant.Flag_Yes)
                            lobjDeffComp.lstrSource = "02";
                        else
                            lobjDeffComp.lstrSource = "01";
                    }
                    //--End--//
                }
                //PIR 26240 Update the Nationwide Plan number to be 12 digits // update with field ach account number from org
                busOrganization lbusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
                lbusOrganization.FindOrganization(lintProviderOrgID);
                if (lbusOrganization.IsNotNull() && lbusOrganization.icdoOrganization.IsNotNull() && lbusOrganization.icdoOrganization.ach_account_number.IsNotNull())
                {
                    lobjDeffComp.icdoProviderReportDataDeffComp.lstrOrgAccAccountNumber = lbusOrganization.icdoOrganization.ach_account_number;
                }
                lobjDeffComp.icdoProviderReportDataDeffComp.istrDetailCounter = lintDetailCounter.ToString().PadLeft(7, '0');
                lintDetailCounter++;
                lobjDeffComp.payment_date = ldtPaymentDate; // PROD PIR ID 5682
                _iclbProviderReportDataDeffComp.Add(lobjDeffComp);
            }
            LoadTotalDebitAndCreditAmount();
            LoadServiceClassCode();
            LoadTotalRoutingNo();
            LoadTotalAmount();
            LoadBlockCount();
        }

        public override bool ValidateFile()
        {
            bool lblnFlag = true;
            if (_iclbProviderReportDataDeffComp.Count < 0)
            {
                this.istrError = "No Records Exists.";
                lblnFlag = false;
            }
            return lblnFlag;
        }

        public void LoadBlockCount()
        {
            if ((ibusOrgCodeValue.icdoCodeValue.code_value == busConstant.Provider_BankOfNorthDakota) ||
                (ibusOrgCodeValue.icdoCodeValue.code_value == busConstant.Provider_WaddellAndReed))
            {
                int lintRowCount = 4;    // Other than Detail Record count
                if (_iclbProviderReportDataDeffComp != null)
                    lintRowCount += _iclbProviderReportDataDeffComp.Count;
                lintEmptyRowCount = 10 - (lintRowCount % 10);
                if (lintEmptyRowCount == 10)
                    _lintBlockCount = lintRowCount / 10;
                else
                    _lintBlockCount = (lintRowCount + lintEmptyRowCount) / 10;
            }
        }

        private string istrFromEmail;

        public override void FinalizeFile()
        {
            if ((ibusOrgCodeValue.icdoCodeValue.code_value == busConstant.Provider_BankOfNorthDakota) ||
                (ibusOrgCodeValue.icdoCodeValue.code_value == busConstant.Provider_WaddellAndReed))
            {
                if (lintEmptyRowCount != 10)
                {
                    string istr = string.Empty;
                    for (int i = 0; i < 94; i++)
                        istr += "9";
                    for (int i = 0; i < lintEmptyRowCount; i++)
                        iswrOut.WriteLine(istr);
                }
                istrACHFileName = ibusOrgCodeValue.icdoCodeValue.data3;
                CreateACHTransmittalReport();
            }

            try
            {
                // PROD PIR ID 512
                busGlobalFunctions.SendMail(busGlobalFunctions.GetSysManagementEmailNotification(), ibusOrgCodeValue.icdoCodeValue.data2,
                                                istrFileName + " file is generated.", ibusOrgCodeValue.icdoCodeValue.comments, true, true);
            }
            catch (Exception Ex)
            {
                ExceptionManager.Publish(Ex);
            }
            
        }

        private Hashtable lhstPositiveValues = new Hashtable();
        private Hashtable lhstNegativeValues = new Hashtable();

        private void LoadSigns()
        {
            lhstPositiveValues.Add("0", "{");
            lhstPositiveValues.Add("1", "A");
            lhstPositiveValues.Add("2", "B");
            lhstPositiveValues.Add("3", "C");
            lhstPositiveValues.Add("4", "D");
            lhstPositiveValues.Add("5", "E");
            lhstPositiveValues.Add("6", "F");
            lhstPositiveValues.Add("7", "G");
            lhstPositiveValues.Add("8", "H");
            lhstPositiveValues.Add("9", "I");

            lhstNegativeValues.Add("0", "}");
            lhstNegativeValues.Add("1", "J");
            lhstNegativeValues.Add("2", "K");
            lhstNegativeValues.Add("3", "L");
            lhstNegativeValues.Add("4", "M");
            lhstNegativeValues.Add("5", "N");
            lhstNegativeValues.Add("6", "O");
            lhstNegativeValues.Add("7", "P");
            lhstNegativeValues.Add("8", "Q");
            lhstNegativeValues.Add("9", "R");
        }

        private string _lstrTotalContributionAmount;
        private string FileName;
        public string lstrTotalContributionAmount
        {
            get
            {
                decimal ldclTemp = 0.0M;
                string lstrKey, lstrValue;
                /// Loads the HashTable Values
                LoadSigns();
                /// To Display the amount field as mentioned in Appendix 036-1.xls. Ex: 120.01 would be like 000000001200A
                if (_ldclTotalAmount < 0)
                {
                    ldclTemp = _ldclTotalAmount * (-100);
                    _lstrTotalContributionAmount = ldclTemp.ToString("#");
                    _lstrTotalContributionAmount = _lstrTotalContributionAmount.PadLeft(13, '0');
                    lstrKey = _lstrTotalContributionAmount.Substring(_lstrTotalContributionAmount.Length - 1, 1);
                    _lstrTotalContributionAmount = _lstrTotalContributionAmount.Substring(0, _lstrTotalContributionAmount.Length - 1);
                    lstrValue = Convert.ToString(lhstNegativeValues[lstrKey]);
                    _lstrTotalContributionAmount = _lstrTotalContributionAmount + lstrValue;
                }
                else
                {
                    ldclTemp = _ldclTotalAmount * 100;
                    _lstrTotalContributionAmount = ldclTemp.ToString("#");
                    _lstrTotalContributionAmount = _lstrTotalContributionAmount.PadLeft(13, '0');
                    lstrKey = _lstrTotalContributionAmount.Substring(_lstrTotalContributionAmount.Length - 1, 1);
                    _lstrTotalContributionAmount = _lstrTotalContributionAmount.Substring(0, _lstrTotalContributionAmount.Length - 1);
                    lstrValue = Convert.ToString(lhstPositiveValues[lstrKey]);
                    _lstrTotalContributionAmount = _lstrTotalContributionAmount + lstrValue;
                }
                return _lstrTotalContributionAmount;
            }
        }
        public string lstrAXATotalContributionAmount
        {
            get
            {
                string lstrContributionAmount = string.Empty;
                decimal ldclTemp = 0.0M;
                /// To Display the amount field as mentioned in Appendix 036-1.xls. Ex: 120.01 would be like 000000001200A
                if (_ldclTotalAmount < 0)
                {
                    ldclTemp = _ldclTotalAmount * (-100);
                    lstrContributionAmount = ldclTemp.ToString("#");
                    lstrContributionAmount = lstrContributionAmount.PadLeft(10, '0');
                    lstrContributionAmount = lstrContributionAmount.PadRight(1, '0');
                }
                else
                {
                    ldclTemp = _ldclTotalAmount * 100;
                    lstrContributionAmount = ldclTemp.ToString("#");
                    lstrContributionAmount = lstrContributionAmount.PadLeft(10, '0');
                    lstrContributionAmount = lstrContributionAmount.PadRight(1, '0');
                }
                return lstrContributionAmount;
            }
        }

        public void CreateACHTransmittalReport()
        {
            DataTable ldtbReportTable = new DataTable();
            ldtbReportTable.TableName = busConstant.ReportTableName;
            // Defining the Columns in DataTable
            DataColumn ldcBatchCount = new DataColumn("BatchCount", Type.GetType("System.Int32"));
            DataColumn ldcBlockCount = new DataColumn("BlockCount", Type.GetType("System.Int32"));
            DataColumn ldcAddendaCount = new DataColumn("AddendaCount", Type.GetType("System.Int32"));
            DataColumn ldcHashCount = new DataColumn("HashCount", Type.GetType("System.String"));
            DataColumn ldclCreditAmount = new DataColumn("TotalCredit", Type.GetType("System.Decimal"));
            DataColumn ldclDebitAmount = new DataColumn("TotalDebit", Type.GetType("System.Decimal"));
            DataColumn ldclEffectiveDate = new DataColumn("EffectiveDate", Type.GetType("System.DateTime"));
            DataColumn ldclFilename = new DataColumn("Filename", Type.GetType("System.String"));
            // Adding the Columns in DataTable
            ldtbReportTable.Columns.Add(ldcBatchCount);
            ldtbReportTable.Columns.Add(ldcBlockCount);
            ldtbReportTable.Columns.Add(ldcAddendaCount);
            ldtbReportTable.Columns.Add(ldcHashCount);
            ldtbReportTable.Columns.Add(ldclCreditAmount);
            ldtbReportTable.Columns.Add(ldclDebitAmount);
            ldtbReportTable.Columns.Add(ldclEffectiveDate);
            ldtbReportTable.Columns.Add(ldclFilename);
            // Adding Values to the Cells.
            DataRow dr = ldtbReportTable.NewRow();
            dr[ldcBatchCount] = 1;
            dr[ldcBlockCount] = _lintBlockCount;
            if (lintFileControlRecordCount != null)
                dr[ldcAddendaCount] = lintFileControlRecordCount;
            dr[ldcHashCount] = istrEntryHash ?? string.Empty;
            dr[ldclCreditAmount] = ldclTotalCreditAmount;
            dr[ldclDebitAmount] = ldclTotalDebitAmount;
            dr[ldclEffectiveDate] = ldtTodaysDate;
            dr[ldclFilename] = FileName +" "+ istrACHFileName;
            ldtbReportTable.Rows.Add(dr);
            // Create Report Method.
            busNeoSpinBase lobjNSBase = new busNeoSpinBase();
            string lstrReportName = !string.IsNullOrEmpty(istrFileName) && istrFileName.LastIndexOf(".") > 0 ?
                istrFileName.Substring(0, istrFileName.LastIndexOf(".")) : "ACHTransmittalReport" + DateTime.Now.ToString(busConstant.DateFormat);
            lobjNSBase.CreateReportWithGivenName("rptPreNoteTransmittalRegister.rpt", ldtbReportTable, lstrReportName);
        }
    }
}
