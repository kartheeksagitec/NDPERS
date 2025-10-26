#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using NeoSpin.DataObjects;
using Sagitec.BusinessObjects;
using NeoSpin.BusinessObjects;
using System.Text.RegularExpressions;

#endregion

namespace NeoSpin.CustomDataObjects
{
    [Serializable]
	public class cdoPayeeAccountAchDetail : doPayeeAccountAchDetail
	{
		public cdoPayeeAccountAchDetail() : base()
		{
		}

        private string _org_code;
        public string org_code
        {
            get { return _org_code; }
            set { _org_code = value; }
        }

        // Pre-Note ACH Verification Batch
        private string _bank_DFI_Account_no;
        public string bank_DFI_Account_no
        {
            get { return _bank_DFI_Account_no; }
            set { _bank_DFI_Account_no = value; }
        }

        //For MSS Layout change
        public string scrambled_account_number
        {
            get
            {
                string acc_number_part = String.Empty;
                string scrambled_part = String.Empty;
                string scrambled = String.Empty;
                 
                if (!String.IsNullOrEmpty(account_number))
                {
                    //If Account Number's length is less than 4, then pad it with leading 0's. 
                    //Scramble the remaining Account Number upto 8 chars.
                    if (account_number.Length < 4)
                        account_number = account_number.PadLeft(4, '0');
                    acc_number_part = account_number.Trim().Right(4);
                    scrambled_part = account_number.Trim().Left(account_number.Trim().Length - 4);
                    scrambled = acc_number_part.PadLeft(12, 'X');
                }

                return scrambled ;

            }
        }

        //PIR 18503
        public bool iblnIsRouitngNumberExists1 { get; set; }
        public bool iblnIsRouitngNumberExists2 { get; set; }
        public string istrBankName1 { get; set; }
        public string istrBankName2 { get; set; }
        public string istrRoutingNumber1 { get; set; }
        public string istrRoutingNumber2 { get; set; }
        public decimal idecRemainderOfBenefit { get; set; }
        public bool iblnIsOrgWorkflowRequired1 { get; set; }
        public bool iblnIsOrgWorkflowRequired2 { get; set; }
        public bool iblnPartialAmount1 { get; set; }
        public bool iblnOrgIDDoesNotExist1 { get; set; }
        public bool iblnOrgIDDoesNotExist2 { get; set; }

        public string suppress_warnings_flag { get; set; }
    } 
} 
