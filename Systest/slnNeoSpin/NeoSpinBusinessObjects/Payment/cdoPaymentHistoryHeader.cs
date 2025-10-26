#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using NeoSpin.DataObjects;
using NeoSpin.BusinessObjects;

#endregion

namespace NeoSpin.CustomDataObjects
{
    [Serializable]
    public class cdoPaymentHistoryHeader : doPaymentHistoryHeader
    {
        public cdoPaymentHistoryHeader() : base()
        {
        }
        //these properties used for Load Payment history tab in Payee account maitenance screen
        public int payment_year { get; set; }
        public decimal gross_amount { get; set; }
        public decimal taxable_amount { get; set; }
        public decimal NonTaxable_Amount { get; set; }
        public decimal deduction_amount { get; set; }
        public decimal net_amount { get; set; }
        public DateTime PaymentYearStartDate { get; set; }
        public DateTime PaymentYearEndDate { get; set; }      
        public string istrStateTaxTaxExemption { get; set; }
        public string istrFedTaxExemtion { get; set; }
        public string istrPersonIdOrOrgCode { get; set; }
        public string istrPaymentDate
        {
            get
            {
                return payment_date.ToString(busConstant.DateFormatLongDate);
            }
        }
        //PROD PIR 1454
        public decimal taxable_rollover_amount { get; set; }
        public decimal nontaxable_rollover_amount { get; set; }

        //PIR 8520
        public DateTime transaction_date { get; set; }
        //PIR 25699
        public decimal idecFedTaxAmount { get; set; }
        public decimal idecNDStateTaxAmount { get; set; }
    }
}