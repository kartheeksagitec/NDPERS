#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sagitec.Common;
using Sagitec.DataObjects;
#endregion

namespace NeoSpin.DataObjects
{
    [Serializable]
    public class doPersonAccountAchDetail : doBase
    {
         
         public doPersonAccountAchDetail() : base()
         {
         }
		private int _person_account_ach_detail_id;
		public int person_account_ach_detail_id
		{
			get
			{
				return _person_account_ach_detail_id;
			}

			set
			{
				_person_account_ach_detail_id = value;
			}
		}

		private int _person_account_id;
		public int person_account_id
		{
			get
			{
				return _person_account_id;
			}

			set
			{
				_person_account_id = value;
			}
		}

		private DateTime _ach_start_date;
		public DateTime ach_start_date
		{
			get
			{
				return _ach_start_date;
			}

			set
			{
				_ach_start_date = value;
			}
		}

		private DateTime _ach_end_date;
		public DateTime ach_end_date
		{
			get
			{
				return _ach_end_date;
			}

			set
			{
				_ach_end_date = value;
			}
		}

		private int _bank_org_id;
		public int bank_org_id
		{
			get
			{
				return _bank_org_id;
			}

			set
			{
				_bank_org_id = value;
			}
		}

		private int _bank_account_type_id;
		public int bank_account_type_id
		{
			get
			{
				return _bank_account_type_id;
			}

			set
			{
				_bank_account_type_id = value;
			}
		}

		private string _bank_account_type_description;
		public string bank_account_type_description
		{
			get
			{
				return _bank_account_type_description;
			}

			set
			{
				_bank_account_type_description = value;
			}
		}

		private string _bank_account_type_value;
		public string bank_account_type_value
		{
			get
			{
				return _bank_account_type_value;
			}

			set
			{
				_bank_account_type_value = value;
			}
		}

		private string _bank_account_number;
		public string bank_account_number
		{
			get
			{
				return _bank_account_number;
			}

			set
			{
				_bank_account_number = value;
			}
		}

		private int _aba_number;
		public int aba_number
		{
			get
			{
				return _aba_number;
			}

			set
			{
				_aba_number = value;
			}
		}

		private string _pre_note_flag;
		public string pre_note_flag
		{
			get
			{
				return _pre_note_flag;
			}

			set
			{
				_pre_note_flag = value;
			}
		}

		private DateTime _pre_note_completion_date;
		public DateTime pre_note_completion_date
		{
			get
			{
				return _pre_note_completion_date;
			}

			set
			{
				_pre_note_completion_date = value;
			}
		}

		private int _insurance_term_payment_id;
		public int insurance_term_payment_id
		{
			get
			{
				return _insurance_term_payment_id;
			}

			set
			{
				_insurance_term_payment_id = value;
			}
		}

		private string _insurance_term_payment_description;
		public string insurance_term_payment_description
		{
			get
			{
				return _insurance_term_payment_description;
			}

			set
			{
				_insurance_term_payment_description = value;
			}
		}

		private string _insurance_term_payment_value;
		public string insurance_term_payment_value
		{
			get
			{
				return _insurance_term_payment_value;
			}

			set
			{
				_insurance_term_payment_value = value;
			}
		}

    }
}

