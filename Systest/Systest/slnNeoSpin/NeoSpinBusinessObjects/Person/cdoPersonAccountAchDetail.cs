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
	public class cdoPersonAccountAchDetail : doPersonAccountAchDetail
	{
		public cdoPersonAccountAchDetail() : base()
		{
		}
        private string _org_code;

        public string org_code
        {
            get { return _org_code; }
            set { _org_code = value; }
        }
        public DateTime ach_start_date_no_null
        {
            get
            {
                if (ach_start_date == DateTime.MinValue)
                    return DateTime.MaxValue;
                return ach_start_date;
            }
        }

        //For MSS Layout change
        public string scrambled_account_number
        {
            get
            {
                string acc_number_part = String.Empty;
                string scrambled_part = String.Empty;
                string scrambled = String.Empty;

                if (!String.IsNullOrEmpty(bank_account_number))
                {
                    //If Account Number's length is less than 4, then pad it with leading 0's. 
                    //Scramble the remaining Account Number upto 8 chars.
                    if (bank_account_number.Length < 4)
                        bank_account_number = bank_account_number.PadLeft(4, '0');
                    acc_number_part = bank_account_number.Trim().Right(4);
                    scrambled_part = bank_account_number.Trim().Left(bank_account_number.Trim().Length - 4);
                    scrambled = acc_number_part.PadLeft(12, 'X');
                }

                return scrambled;

            }
        }

        //PIR 18503
        public string istrRoutingNumber { get; set; }
        public string istrBankName { get; set; }

        public int plan_id_ach { get; set; }
    } 
} 
