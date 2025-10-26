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
    public class busPayment1099rFileOut : busFileBaseOut
    {
        //Collection of 1099r details for all the payees
        public Collection<busPayment1099rFile> iclbPayment1099r { get; set; }

        public decimal idecTotalDistribtionPercentage { get; set; }
        public decimal idecTotalGrossAmount { get; set; }
        public decimal idecTotalTaxableAmount { get; set; }
        public decimal idecTotatlNonTaxableAmount { get; set; }
        public decimal idecTotalCapitalGain { get; set; }
        public decimal idecTotalFedTaxWithheld { get; set; }
        public decimal idecTotalMemberContributions { get; set; }

        public string istrRecordSequenceNumber_C { get; set; }

        public string istrRecordSequenceNumber_F { get; set; }

        public string istrPriorYearDataIndiacator { get; set; }
        public string istrTestFileIdicatior { get; set; }
        public string istrTotalNoOfPayees { get; set; }
        public int iintTotalNoOfAPayees { get; set; }
        public string istrNameControl { get; set; }

        //Property to indicate whether file generating from correction batch annual batchs
        public bool iblnCorrectionBatchIndicator { get; set; }
        //Property to load Tax year
        public int iintTaxYear { get; set; }

        //formatted amount fields
        public string istrFormattedTotalGrossAmount
        {
            get
            {
                decimal ldecTotalGrossAmount = idecTotalGrossAmount * 100;
                if (ldecTotalGrossAmount.ToString().IndexOf('.') > 0)
                    return ldecTotalGrossAmount.ToString().Substring(0, ldecTotalGrossAmount.ToString().IndexOf('.')).PadLeft(18, '0');
                else
                    return ldecTotalGrossAmount.ToString().PadLeft(18, '0');
            }
        }
        public string istrFormattedTotalTaxableAmount
        {
            get
            {
                decimal ldecTotalTaxableAmount = idecTotalTaxableAmount * 100;
                if (ldecTotalTaxableAmount.ToString().IndexOf('.') > 0)
                return ldecTotalTaxableAmount.ToString().Substring(0, ldecTotalTaxableAmount.ToString().IndexOf('.')).PadLeft(18, '0');
                else
                    return ldecTotalTaxableAmount.ToString().PadLeft(18, '0');
            }
        }
        public string istrFormattedTotalCapitalGain
        {
            get
            {
                decimal ldecTotalCapitalGain = idecTotalCapitalGain * 100;
                if (ldecTotalCapitalGain.ToString().IndexOf('.') > 0)
                    return ldecTotalCapitalGain.ToString().Substring(0, ldecTotalCapitalGain.ToString().IndexOf('.')).PadLeft(18, '0');
                else
                    return ldecTotalCapitalGain.ToString().PadLeft(18, '0');
            }
        }
        public string istrFormattedTotalFedTaxWithheld
        {
            get
            {
                decimal ldecTotalFedTaxWithheld = idecTotalFedTaxWithheld * 100;
                if (ldecTotalFedTaxWithheld.ToString().IndexOf('.') > 0)
                    return ldecTotalFedTaxWithheld.ToString().Substring(0, ldecTotalFedTaxWithheld.ToString().IndexOf('.')).PadLeft(18, '0');
                else
                    return ldecTotalFedTaxWithheld.ToString().PadLeft(18, '0');
            }
        }
        public string istrFormattedTotalNonTaxableAmount
        {
            get
            {
                decimal ldecTotalNonTaxableAmount = idecTotatlNonTaxableAmount * 100;
                if (ldecTotalNonTaxableAmount.ToString().IndexOf('.') > 0)
                    return ldecTotalNonTaxableAmount.ToString().Substring(0, ldecTotalNonTaxableAmount.ToString().IndexOf('.')).PadLeft(18, '0');
                else
                    return ldecTotalNonTaxableAmount.ToString().PadLeft(18, '0');
            }
        }
        public string istrFormattedTotalMemberContributions
        {
            get
            {
                decimal ldecTotalMemberContributions = idecTotalMemberContributions * 100;
                if (ldecTotalMemberContributions.ToString().IndexOf('.') > 0)
                    return ldecTotalMemberContributions.ToString().Substring(0, ldecTotalMemberContributions.ToString().IndexOf('.')).PadLeft(18, '0');
                else
                    return ldecTotalMemberContributions.ToString().PadLeft(18, '0');
            }
        }

        public string istrBlank241_C
        {
            get
            { 
                string lstr=string.Empty;
                return lstr.PadRight(241, ' ');
            }
        }
        public string istrBlank2_C
        {
            get
            {
                string lstr = string.Empty;
                return lstr.PadRight(2, ' ');
            }
        }
        public string istrBlank241_F
        {
            get
            {
                string lstr = string.Empty;
                return lstr.PadRight(241, ' ');
            }
        }
        public string istrBlank2_F
        {
            get
            {
                string lstr = string.Empty;
                return lstr.PadRight(2, ' ');
            }
        }
        public override void InitializeFile()
        {
            istrFileName = "Payment1099rFile" + "_" + DateTime.Now.ToString(busConstant.DateFormat) + "_" + iintTaxYear.ToString() + busConstant.FileFormattxt;
            base.InitializeFile();
        }
        public void Load1099rDetails(DataTable ldtb1099rDetails)
        {
            iclbPayment1099r = new Collection<busPayment1099rFile>();   

            iintTaxYear = (int)iarrParameters[0];
            iblnCorrectionBatchIndicator = (bool)iarrParameters[1];
            ldtb1099rDetails = (DataTable)iarrParameters[2];
            int lintRecordSequenceNumber = 3;
            foreach (DataRow dr in ldtb1099rDetails.Rows)
            {
                busPayment1099rFile lobjPayment1099rFile = new busPayment1099rFile { icdoPayment1099r = new cdoPayment1099r() };
                lobjPayment1099rFile.icdoPayment1099r.LoadData(dr);
                //uat pir 1794 -- Start
                lobjPayment1099rFile.icdoPayment1099r.addr_line_1 =
                    (!string.IsNullOrEmpty(lobjPayment1099rFile.icdoPayment1099r.addr_line_1) ? lobjPayment1099rFile.icdoPayment1099r.addr_line_1.ToUpper() : string.Empty) + " " +
                    (!string.IsNullOrEmpty(lobjPayment1099rFile.icdoPayment1099r.addr_line_2) ? lobjPayment1099rFile.icdoPayment1099r.addr_line_2.ToUpper() : string.Empty);
                if (lobjPayment1099rFile.icdoPayment1099r.addr_line_1.Length > 40)
                    lobjPayment1099rFile.icdoPayment1099r.addr_line_1 = lobjPayment1099rFile.icdoPayment1099r.addr_line_1.ToString().Substring(0, 40);
                else
                    lobjPayment1099rFile.icdoPayment1099r.addr_line_1 = lobjPayment1099rFile.icdoPayment1099r.addr_line_1.ToString().PadRight(40, ' ');
                //uat pir 1794 -- End
                lobjPayment1099rFile.icdoPayment1099r.addr_city = !string.IsNullOrEmpty(lobjPayment1099rFile.icdoPayment1099r.addr_city) ?
                    lobjPayment1099rFile.icdoPayment1099r.addr_city.ToUpper() : string.Empty;
                if (dr["addr_state"] != DBNull.Value)
                {
                    lobjPayment1099rFile.addr_state = dr["addr_state"].ToString().ToUpper();
                }

                if (dr["ADDR_LINE_3"].ToString() != " ") // PIR 12370 issue #7
                {
                    lobjPayment1099rFile.istrForeignEntityIndicator  = "1";
                }
                else
                {
                    lobjPayment1099rFile.istrForeignEntityIndicator = " ";
                }
                lobjPayment1099rFile.LoadPayeeAccount();
                lobjPayment1099rFile.istrRecordSequenceNumber = lintRecordSequenceNumber.ToString();
                lobjPayment1099rFile.istrRecordSequenceNumber = lobjPayment1099rFile.istrRecordSequenceNumber.PadLeft(8, '0');
                //Type of TIN will be blank if Payee is Person else "2" if payee is org
                lobjPayment1099rFile.istrTypeOfTIN = string.IsNullOrEmpty(lobjPayment1099rFile.icdoPayment1099r.ssn) ? " " : "2";
                //TIN will be ssn if  be blank if Payee is Person else fedreal id if payee is org
                lobjPayment1099rFile.istrTIN = string.IsNullOrEmpty(lobjPayment1099rFile.icdoPayment1099r.ssn) ?
                    lobjPayment1099rFile.icdoPayment1099r.federal_id : lobjPayment1099rFile.icdoPayment1099r.ssn;
                //lobjPayment1099rFile.istrTaxAmountNotDefined = lobjPayment1099rFile.icdoPayment1099r.taxable_amount > 0.0m ? "1" : " ";
                //Format percentage
                //systest pir 2396 : should be 'G' if corrected else empty
                lobjPayment1099rFile.istrCorrectedIndicator = iblnCorrectionBatchIndicator ? "G" : " ";

                if (lobjPayment1099rFile.icdoPayment1099r.dist_percentage > 0 && lobjPayment1099rFile.icdoPayment1099r.dist_percentage < 100)
                {
                    lobjPayment1099rFile.distribution_percentage = Convert.ToInt32(Math.Floor(lobjPayment1099rFile.icdoPayment1099r.dist_percentage)).ToString().PadLeft(2, '0');
                }
                else
                {
                    lobjPayment1099rFile.distribution_percentage = "  ";
                }

                //First 40 characters in Payee name
                if (lobjPayment1099rFile.icdoPayment1099r.name.Length > 39)
                {
                    lobjPayment1099rFile.PayeeName = lobjPayment1099rFile.icdoPayment1099r.name.Substring(0, 39).ToUpper();
                }
                else
                {
                    lobjPayment1099rFile.PayeeName = lobjPayment1099rFile.icdoPayment1099r.name.ToUpper();
                }
                lobjPayment1099rFile.istrTotalDistributionIndicator = lobjPayment1099rFile.icdoPayment1099r.total_distribution_flag == busConstant.Flag_Yes ?
                    "1" : " ";
                string lstrTemp = Convert.ToString(lobjPayment1099rFile.icdoPayment1099r.payee_account_id) + "-" + lobjPayment1099rFile.icdoPayment1099r.id_suffix; //PROD PIR 8707
                lobjPayment1099rFile.istrPayeeAccountID = lstrTemp.PadRight(20, ' ');
                iclbPayment1099r.Add(lobjPayment1099rFile);
                lintRecordSequenceNumber++;
            }
            istrRecordSequenceNumber_C = (lintRecordSequenceNumber).ToString().PadLeft(8, '0');
            istrRecordSequenceNumber_F = (lintRecordSequenceNumber + 1).ToString().PadLeft(8, '0');
            LoadHeaderDetails();
            LoadFooterDetails();
        }

        //Load object fields specified in the footer section
        public void LoadFooterDetails()
        {
            idecTotalCapitalGain = iclbPayment1099r.Sum(o => o.icdoPayment1099r.capital_gain);
            idecTotalTaxableAmount = iclbPayment1099r.Sum(o => o.icdoPayment1099r.taxable_amount);
            idecTotatlNonTaxableAmount = iclbPayment1099r.Sum(o => o.icdoPayment1099r.non_taxable_amount);
            idecTotalGrossAmount = iclbPayment1099r.Sum(o => o.icdoPayment1099r.gross_benefit_amount);
            idecTotalFedTaxWithheld = iclbPayment1099r.Sum(o => o.icdoPayment1099r.fed_tax_amount);
            idecTotalMemberContributions = iclbPayment1099r.Sum(o => o.icdoPayment1099r.total_employee_contrib_amt);
        }
        //load object fields specified in the file header section
        public void LoadHeaderDetails()
        {
            istrPriorYearDataIndiacator = iblnCorrectionBatchIndicator ? "P" : " ";
            iintTotalNoOfAPayees = iclbPayment1099r.Count;
            istrTotalNoOfPayees = (iintTotalNoOfAPayees).ToString().PadLeft(8, '0');
        }
    }
}