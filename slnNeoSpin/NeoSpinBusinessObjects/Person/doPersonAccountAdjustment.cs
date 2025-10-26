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
    public class doPersonAccountAdjustment : doBase
    {
         
         public doPersonAccountAdjustment() : base()
         {
         }
		private int _person_account_adjustment_id;
		public int person_account_adjustment_id
		{
			get
			{
				return _person_account_adjustment_id;
			}

			set
			{
				_person_account_adjustment_id = value;
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

		private DateTime _transaction_date;
		public DateTime transaction_date
		{
			get
			{
				return _transaction_date;
			}

			set
			{
				_transaction_date = value;
			}
		}

		private decimal _rhic_benfit_amount;
		public decimal rhic_benfit_amount
		{
			get
			{
				return _rhic_benfit_amount;
			}

			set
			{
				_rhic_benfit_amount = value;
			}
		}

		private decimal _capital_gain;
		public decimal capital_gain
		{
			get
			{
				return _capital_gain;
			}

			set
			{
				_capital_gain = value;
			}
		}

		private string _comment;
		public string comment
		{
			get
			{
				return _comment;
			}

			set
			{
				_comment = value;
			}
		}

		private int _status_id;
		public int status_id
		{
			get
			{
				return _status_id;
			}

			set
			{
				_status_id = value;
			}
		}

		private string _status_description;
		public string status_description
		{
			get
			{
				return _status_description;
			}

			set
			{
				_status_description = value;
			}
		}

		private string _status_value;
		public string status_value
		{
			get
			{
				return _status_value;
			}

			set
			{
				_status_value = value;
			}
		}

		private string _posted_by;
		public string posted_by
		{
			get
			{
				return _posted_by;
			}

			set
			{
				_posted_by = value;
			}
		}

		private DateTime _posted_date;
		public DateTime posted_date
		{
			get
			{
				return _posted_date;
			}

			set
			{
				_posted_date = value;
			}
		}

    }
}

