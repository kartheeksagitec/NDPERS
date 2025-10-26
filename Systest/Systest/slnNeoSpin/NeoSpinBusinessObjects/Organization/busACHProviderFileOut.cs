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
using System.Linq;
using System.Linq.Expressions;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busACHProviderFileOut : busFileBaseOut
    {
        public busACHProviderFileOut()
        {
        }

        private int lintEmptyRowCount = 0;        
        private string istrIsPullACH = string.Empty;
        private bool iblnIsPreNoteVerification = false;
        private bool iblnIsACHPaymentDistribution = false;
        private string FileName = string.Empty;

        public string istrBenefitType { get; set; }

        private Collection<busACHProviderReportData> _iclbACHProvider;
        public Collection<busACHProviderReportData> iclbACHProvider
        {
            get { return _iclbACHProvider; }
            set { _iclbACHProvider = value; }
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

        public DateTime idtEffectiveDate { get; set; }

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
                return _lstrBlockCount.PadLeft(6,'0');
            }
        }

        private long _lintTotalRoutingNo;
        public long lintTotalRoutingNo
        {
            get { return _lintTotalRoutingNo; }
            set { _lintTotalRoutingNo = value; }
        }

        public long iintTotalRoutingNumber { get; set; }

        private string _lstrEntryHash;
        public string lstrEntryHash
        {
            get 
            {
                _lstrEntryHash = Convert.ToString(_lintTotalRoutingNo);
                return _lstrEntryHash.PadLeft(10, '0');
            }
        }

        public string istrEntryHash
        {
            get
            {
                string lstrTempEntryHash = Convert.ToString(iintTotalRoutingNumber);
                if (lstrTempEntryHash.Length > 10)
                    lstrTempEntryHash = lstrTempEntryHash.Right(10);
                return lstrTempEntryHash.PadLeft(10, '0');
            }
        }

        public void LoadTotalRoutingNo()
        {
            if (_iclbACHProvider != null)
            {
                foreach (busACHProviderReportData lobjACH in _iclbACHProvider)
                {
                    if (!string.IsNullOrEmpty(lobjACH.lstrRoutingNumber))
                        _lintTotalRoutingNo += Convert.ToInt32(lobjACH.lstrRoutingNumber);
                    if (!string.IsNullOrEmpty(lobjACH.istrRoutingNumberFirstEightDigits))
                        iintTotalRoutingNumber += Convert.ToInt32(lobjACH.istrRoutingNumberFirstEightDigits);
                }
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
                return (Convert.ToInt64((ldclTotalDebitAmount * 100))).ToString().PadLeft(12, '0');
            }
        }
        public string ldclTotalCreditAmounttFormatted 
        {
            get
            {
                return (Convert.ToInt64((ldclTotalCreditAmount * 100))).ToString().PadLeft(12, '0');
            }
        }
        public void LoadTotalDebitAndCreditAmount()
        {
            if (_iclbACHProvider != null)
            {
                foreach (busACHProviderReportData lobjACH in _iclbACHProvider)
                {
                    if (lobjACH.ldclContributionAmount<0)
                        _ldclTotalCreditAmount += lobjACH.ldclContributionAmount;
                    else
                        _ldclTotalDebitAmount += lobjACH.ldclContributionAmount;

                    lobjACH.ldclContributionAmount = Math.Abs(lobjACH.ldclContributionAmount);
                }
                _ldclTotalCreditAmount = Math.Abs(_ldclTotalCreditAmount);
                _ldclTotalDebitAmount = Math.Abs(_ldclTotalDebitAmount);
            }
        }

        private string _lintBatchControlRecordCount;
        public string lintBatchControlRecordCount
        {
            get
            {
                if (_iclbACHProvider != null)
                    _lintBatchControlRecordCount = Convert.ToString(_iclbACHProvider.Count);
                return _lintBatchControlRecordCount.PadLeft(6, '0');
            }
        }

        private string _lintFileControlRecordCount;
        public string lintFileControlRecordCount
        {
            get
            {
                if (_iclbACHProvider != null)
                    _lintFileControlRecordCount = Convert.ToString(_iclbACHProvider.Count);
                return _lintFileControlRecordCount.PadLeft(8, '0');
            }
        }

        private string _lstrServiceClassCode;
        private string istrACHFilename;
        public string lstrServiceClassCode
        {
            get { return _lstrServiceClassCode; }
            set { _lstrServiceClassCode = value; }
        }

        public void LoadServiceClassCode()
        {
            if (_iclbACHProvider != null)
            {
                int lintNegAdjCount = 0;
                foreach (busACHProviderReportData lobjACH in _iclbACHProvider)
                {
                    if (lobjACH.ldclContributionAmount<0)
                        lintNegAdjCount += 1;
                }
                if (lintNegAdjCount == _iclbACHProvider.Count)
                    _lstrServiceClassCode = busConstant.ServiceCode_CreditsOnly;
                else if (lintNegAdjCount == 0)
                    _lstrServiceClassCode = busConstant.ServiceCode_DebitsOnly;
                else
                    _lstrServiceClassCode = busConstant.ServiceCode_CreditDebitMixed;
            }
        }

        public override void InitializeFile()
        {
            if(istrIsPullACH==busConstant.Flag_Yes)
                istrFileName = "ACHFile" + "_" + DateTime.Now.ToString(busConstant.DateFormat) + busConstant.FileFormattxt;

            if (!string.IsNullOrEmpty(istrBenefitType))
                istrFileName = istrBenefitType + "_" + "ACHFile" + "_" + DateTime.Now.ToString(busConstant.DateFormat) + busConstant.FileFormattxt;

            if(iblnIsPreNoteVerification || iblnIsACHPaymentDistribution)
                istrFileName = "ACHFile" + "_" + DateTime.Now.ToString(busConstant.DateFormat) + busConstant.FileFormattxt;     
       
            if(!string.IsNullOrEmpty(FileName))
                istrFileName = FileName + "_" + DateTime.Now.ToString(busConstant.DateFormat) + busConstant.FileFormattxt;
        }

        /// <summary>
        /// Generate ACH for Deferred Comp, Insurance and Retirement Benefit Type UCS-036
        /// </summary>
        /// <param name="ldtbProviderDeffComp"></param>
        public void LoadACHEntryDetail(DataTable ldtbProviderDeffComp)
        {
            _iclbACHProvider = new Collection<busACHProviderReportData>();
            _iclbACHProvider = (Collection<busACHProviderReportData>)iarrParameters[0];
            istrBenefitType = Convert.ToString(iarrParameters[1]);
            FileName = Convert.ToString(iarrParameters[2]);
            LoadDetailCounter();
            //LoadTotalDebitAndCreditAmount();
            //LoadServiceClassCode();
            //As per Satya, both Payment batch and Vendor payment batch, its credit
            ldclTotalCreditAmount = Math.Abs(iclbACHProvider.Sum(o => o.ldclContributionAmount));
            _lstrServiceClassCode = busConstant.ServiceCode_CreditsOnly;
            LoadTotalRoutingNo();
            LoadBlockCount();
        }

        /// <summary>
        /// Pull ACH Button from Deposit Tape Maintenance Screen UCS-033
        /// </summary>
        /// <param name="ldtbPullACH"></param>
        public void LoadPullACH(DataTable ldtbPullACH)
        {
            _iclbACHProvider = new Collection<busACHProviderReportData>();
            _iclbACHProvider = (Collection<busACHProviderReportData>)iarrParameters[0];
            istrIsPullACH = Convert.ToString(iarrParameters[1]);
            istrBenefitType = Convert.ToString(iarrParameters[2]);
            FileName = Convert.ToString(iarrParameters[3]);
            idtEffectiveDate = Convert.ToDateTime(iarrParameters[4]);
            LoadDetailCounter();
            //LoadTotalDebitAndCreditAmount();
            //LoadServiceClassCode();
            //As per Satya, Pull Ach should have opposite to what both Payment batch and Vendor payment batch
            ldclTotalDebitAmount = Math.Abs(iclbACHProvider.Sum(o => o.ldclContributionAmount));
            _lstrServiceClassCode = busConstant.ServiceCode_DebitsOnly;
            LoadTotalRoutingNo();
            LoadBlockCount();
        }
        
        public void LoadDetailCounter()
        {
            int lintCounter = 1;
            foreach (busACHProviderReportData lobjData in iclbACHProvider)
            {
                lobjData.istrDetailCount = "09130028" + lintCounter.ToString().PadLeft(7, '0');
                lintCounter++;
            }
        }

        /// <summary>
        ///  UCS - 071 -- 7.4 ACH - Pre-Note Verification Batch
        /// </summary>
        /// <param name="ldtbPreNote"></param>
        public void LoadPreNoteVerificationACH(DataTable ldtbPreNote)
        {
            iblnIsPreNoteVerification = Convert.ToBoolean(iarrParameters[0]);
            _iclbACHProvider=new Collection<busACHProviderReportData>();

            // Load Payee Account ACH Detail and Person Account ACH Detail
            ldtbPreNote = busBase.Select("cdoPayeeAccountAchDetail.ACHPreNoteVerificationBatch", new object[] { });
            int lintDetailCounter = 1;
            bool lblnDebitExists = false, lblnCreditExists = false;
            foreach (DataRow dr in ldtbPreNote.Rows)
            {
                busACHProviderReportData lobjACH=new busACHProviderReportData();
                lobjACH.istrDetailCount = lintDetailCounter.ToString().PadLeft(7, '0');
                if (dr["routing_no"] != DBNull.Value)
                {
                    lobjACH.lstrRoutingNumber = Convert.ToString(dr["routing_no"]).PadLeft(9, '0');
                    lobjACH.istrRoutingNumberFirstEightDigits = lobjACH.lstrRoutingNumber.Substring(0, lobjACH.lstrRoutingNumber.Length - 1).PadLeft(8, '0');                    
                }
                if(dr["account_no"] !=DBNull.Value)
                    lobjACH.lstrDFIAccountNo=Convert.ToString(dr["account_no"]);
                if(dr["person_id"]!=DBNull.Value)
                    lobjACH.lintPERSLinkID = Convert.ToInt32(dr["person_id"]);
                if (dr["transaction_code"] != DBNull.Value)
                    lobjACH.lstrTransactionCode = Convert.ToString(dr["transaction_code"]);

                // Update Pre-Note Flag for Payee Account ACH Detail and Person Account ACH Detail
                string lstrACHType=string.Empty;
                int lintACHID=0;
                if ((dr["type"]!=DBNull.Value) &&
                    (dr["ach_detail_id"]!=DBNull.Value))
                {
                    lstrACHType=Convert.ToString(dr["type"]);
                    lintACHID = Convert.ToInt32(dr["ach_detail_id"]);
                    if(lstrACHType=="PE")
                    {
                        busPayeeAccountAchDetail lobjPEACHDetail=new busPayeeAccountAchDetail();
                        lobjPEACHDetail.FindPayeeAccountAchDetail(lintACHID);
                        lobjPEACHDetail.icdoPayeeAccountAchDetail.pre_note_completion_date=DateTime.Now;
                        lobjPEACHDetail.icdoPayeeAccountAchDetail.pre_note_flag=busConstant.Flag_No;
                        lobjPEACHDetail.icdoPayeeAccountAchDetail.Update();
                        if (lobjACH.lstrTransactionCode == "SAVE")
                        {
                            lobjACH.lstrTransactionCode = busConstant.CreditTransactionCodePrenoteSavings;
                            lblnCreditExists = true;
                        }
                        else if (lobjACH.lstrTransactionCode == "CHKG")
                        {
                            lobjACH.lstrTransactionCode = busConstant.CreditTransactionCodePrenoteChecking;
                            lblnCreditExists = true;
                        }
                        else
                            lobjACH.lstrTransactionCode = "00";
                    }
                    else if(lstrACHType=="PA")
                    {
                        busPersonAccountAchDetail lobjPAACHDetail=new busPersonAccountAchDetail();
                        lobjPAACHDetail.FindPersonAccountAchDetail(lintACHID);
                        lobjPAACHDetail.icdoPersonAccountAchDetail.pre_note_completion_date=DateTime.Now;
                        lobjPAACHDetail.icdoPersonAccountAchDetail.pre_note_flag=busConstant.Flag_No;
                        lobjPAACHDetail.icdoPersonAccountAchDetail.Update();
                        if (lobjACH.lstrTransactionCode == "SAV")
                        {
                            lobjACH.lstrTransactionCode = busConstant.DebitTransactionCodePrenoteSavings;
                            lblnDebitExists = true;
                        }
                        else if (lobjACH.lstrTransactionCode == "CHK")
                        {
                            lobjACH.lstrTransactionCode = busConstant.DebitTransactionCodePrenoteChecking;
                            lblnDebitExists = true;
                        }
                        else
                            lobjACH.lstrTransactionCode = "00";
                    }
                }                
                _iclbACHProvider.Add(lobjACH);
                lintDetailCounter++;
            }
            if (lblnDebitExists && lblnCreditExists)
            {
                lstrServiceClassCode = busConstant.ServiceCode_CreditDebitMixed;
            }
            else if (lblnCreditExists)
                lstrServiceClassCode = busConstant.ServiceCode_CreditsOnly;
            else if (lblnDebitExists)
                lstrServiceClassCode = busConstant.ServiceCode_DebitsOnly;

            LoadTotalRoutingNo();
            LoadBlockCount();
        }

        public void LoadBlockCount()
        {
            int lintRowCount = 4;    // Other than Detail Record count

            if (_iclbACHProvider != null)
                lintRowCount += _iclbACHProvider.Count;

            lintEmptyRowCount = 10 - (lintRowCount % 10);
            if (lintEmptyRowCount == 10)
                _lintBlockCount = lintRowCount / 10;
            else
                _lintBlockCount = (lintRowCount + lintEmptyRowCount) / 10;
        }

        public override void FinalizeFile()
        {
            if (lintEmptyRowCount != 10)
            {
                string istr = string.Empty;
                for (int i = 0; i < 94; i++)
                    istr += "9";
                for (int i = 0; i < lintEmptyRowCount; i++)
                    iswrOut.WriteLine(istr);
            }

            if ((iblnIsPreNoteVerification || FileName == busConstant.ACHFileNameDefCompVendorPayment || FileName == busConstant.ACHFileNameInsuranceVendorPayment ||
                FileName == busConstant.ACHFileNameRetirmentVendorPayment || istrIsPullACH == busConstant.Flag_Yes || iblnIsACHPaymentDistribution) && (_iclbACHProvider != null))
            {
                if (FileName == busConstant.ACHFileNameRetirmentVendorPayment)
                    istrACHFilename = busConstant.RetirementVendorPayment;
                else if (FileName == busConstant.ACHFileNameInsuranceVendorPayment)
                    istrACHFilename = busConstant.InsuranceVendorPayment;
                else if (FileName == busConstant.ACHFileNameDefCompVendorPayment)
                    istrACHFilename = busConstant.DefferedcompVendorPayment;
                else if (FileName == busConstant.ACHFileNameRetirmentEmployerPayment)
                    istrACHFilename = busConstant.RetirementEmployerPayment;
                else if (FileName == busConstant.ACHFileNameInsuranceEmployerPayment)
                    istrACHFilename = busConstant.InsuranceEmployerPayment;
                else if (FileName == busConstant.ACHFileNameDefCompEmployerPayment)
                    istrACHFilename = busConstant.DefferedcompEmployerPayment;
                if (_iclbACHProvider.Count > 0)
                    CreatePreNoteTransmittalReport();
            }
        }

        public override bool ValidateFile()
        {
            // Generate the file only the collection has Detail records.
            if (iblnIsPreNoteVerification)
            {
                if (_iclbACHProvider.Count == 0)
                    return false;
            }
            return true;
        }

        /// <summary>
        ///  UCS-071 Create Pre-Note Transmittal Register Report
        /// </summary>
        public void CreatePreNoteTransmittalReport()
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
            dr[ldclCreditAmount] = (iblnIsPreNoteVerification || istrIsPullACH == busConstant.Flag_Yes) ? 0.0M : ldclTotalCreditAmount;
            dr[ldclDebitAmount] = istrIsPullACH == busConstant.Flag_Yes ? ldclTotalDebitAmount : 0.0M;
            dr[ldclEffectiveDate] = idtEffectiveDate == DateTime.MinValue ? ldtTodaysDate : idtEffectiveDate;
            if( istrIsPullACH==busConstant.Flag_Yes && FileName.IsEmpty())
            {
                dr[ldclFilename] = "DF.BK870024" + " "+busConstant.InsurancePremiums;
            }
            else if (iblnIsPreNoteVerification)
            {
                dr[ldclFilename] = "DF.BK870024" + " "+ busConstant.RetirementAndInsurancePrenote;
            }
            else if(iblnIsACHPaymentDistribution)
            {
                dr[ldclFilename] = "DF.BK870002" + " " + busConstant.PensionPayments;
            }
            else
            {
                dr[ldclFilename] = FileName + " " + istrACHFilename;
            }
            ldtbReportTable.Rows.Add(dr);
            // Create Report Method.
            busNeoSpinBase lobjNSBase = new busNeoSpinBase();
            string lstrReportName = !string.IsNullOrEmpty(istrFileName) && istrFileName.LastIndexOf(".") > 0 ?
                istrFileName.Substring(0, istrFileName.LastIndexOf(".")) : "ACHTransmittalReport" + DateTime.Now.ToString(busConstant.DateFormat);
            lobjNSBase.CreateReportWithGivenName("rptPreNoteTransmittalRegister.rpt", ldtbReportTable, lstrReportName);
        }

        /// <summary>
        /// Method to load the ACH provider collection with data
        /// </summary>
        /// <param name="adtACHPaymentDistribution">Data table</param>
        public void LoadACHPaymentHistoryDistribution(DataTable adtACHPaymentDistribution)
        {
            string lstrTransactionCode = string.Empty;
            iblnIsACHPaymentDistribution = Convert.ToBoolean(iarrParameters[0]);            
            DataTable ldtACHPaymentDistribution = (DataTable)iarrParameters[1];
            idtEffectiveDate = Convert.ToDateTime(iarrParameters[2]);
            _iclbACHProvider = new Collection<busACHProviderReportData>();
            int lintCounter = 1;
            foreach (DataRow dr in ldtACHPaymentDistribution.Rows)
            {
                // Add to ACH file collection
                busACHProviderReportData lobjProviderReportData = new busACHProviderReportData();
                if (dr["PERSON_ID"] != DBNull.Value)
                    lobjProviderReportData.lintPERSLinkID = Convert.ToInt32(dr["PERSON_ID"]);

                if (dr["NET_AMOUNT"] != DBNull.Value)
                    lobjProviderReportData.ldclContributionAmount = Convert.ToDecimal(dr["NET_AMOUNT"]);

                if (dr["ACCOUNT_NUMBER"] != DBNull.Value)                
                    lobjProviderReportData.lstrDFIAccountNo = Convert.ToString(dr["ACCOUNT_NUMBER"]);
                
                if (dr["ROUTING_NUMBER"] != DBNull.Value)
                {
                    lobjProviderReportData.lstrRoutingNumber = Convert.ToString(dr["ROUTING_NUMBER"]);
                    lobjProviderReportData.istrRoutingNumberFirstEightDigits = Convert.ToString(dr["ROUTING_NUMBER"])
                        .Substring(0, Convert.ToString(dr["ROUTING_NUMBER"]).Length - 1).PadLeft(8, '0');
                    lobjProviderReportData.istrCheckLastDigit = Convert.ToString(dr["ROUTING_NUMBER"])
                                                                           .Substring(Convert.ToString(dr["ROUTING_NUMBER"]).Length - 1, 1);
                }

                lstrTransactionCode = (dr["TRANSACTION_CODE"] != DBNull.Value ? Convert.ToString(dr["TRANSACTION_CODE"]) : string.Empty);
                if (lstrTransactionCode == busConstant.BankAccountSavings)
                    lobjProviderReportData.lstrTransactionCode = lobjProviderReportData.ldclContributionAmount >= 0 ?
                        busConstant.CreditTransactionCodeNonPrenoteSavings : busConstant.DebitTransactionCodeNonPrenoteSavings;
                else if (lstrTransactionCode == busConstant.BankAccountChecking)
                    lobjProviderReportData.lstrTransactionCode = lobjProviderReportData.ldclContributionAmount >= 0 ?
                        busConstant.CreditTransactionCodeNonPrenoteChecking : busConstant.DebitTransactionCodePrenoteChecking;
                lobjProviderReportData.istrDetailCount = "09130028" + lintCounter.ToString().PadLeft(7, '0');
                if (!string.IsNullOrEmpty(lobjProviderReportData.lstrDFIAccountNo))
                {
                    _iclbACHProvider.Add(lobjProviderReportData);
                    lintCounter++;
                }                
            }

            //uat pir - 2205 : from payroll only credit transactions
            ldclTotalCreditAmount = Math.Abs(iclbACHProvider.Sum(o => o.ldclContributionAmount));
            _lstrServiceClassCode = busConstant.ServiceCode_CreditsOnly;
            LoadTotalRoutingNo();
            LoadBlockCount();
        }
    }
}
