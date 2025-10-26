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
    public class doServicePurchaseAmortizationSchedule : doBase
    {
         
         public doServicePurchaseAmortizationSchedule() : base()
         {
         }
		private int _service_purchase_amortization_schedule_id;
		public int service_purchase_amortization_schedule_id
		{
			get
			{
				return _service_purchase_amortization_schedule_id;
			}

			set
			{
				_service_purchase_amortization_schedule_id = value;
			}
		}

		private int _service_purchase_header_id;
		public int service_purchase_header_id
		{
			get
			{
				return _service_purchase_header_id;
			}

			set
			{
				_service_purchase_header_id = value;
			}
		}

		private DateTime _payment_due_date;
		public DateTime payment_due_date
		{
			get
			{
				return _payment_due_date;
			}

			set
			{
				_payment_due_date = value;
			}
		}

        private DateTime _payment_date;
        public DateTime payment_date
        {
            get
            {
                return _payment_date;
            }

            set
            {
                _payment_date = value;
            }
        }

		private int _payment_number;
		public int payment_number
		{
			get
			{
				return _payment_number;
			}

			set
			{
				_payment_number = value;
			}
		}

        private decimal _payment_amount;
        public decimal payment_amount
        {
            get
            {
                return _payment_amount;
            }

            set
            {
                _payment_amount = value;
            }
        }

		private decimal _expected_payment_amount;
		public decimal expected_payment_amount
		{
			get
			{
				return _expected_payment_amount;
			}

			set
			{
				_expected_payment_amount = value;
			}
		}

		private decimal _principle_in_payment_amount;
		public decimal principle_in_payment_amount
		{
			get
			{
				return _principle_in_payment_amount;
			}

			set
			{
				_principle_in_payment_amount = value;
			}
		}

		private decimal _interest_in_payment_amount;
		public decimal interest_in_payment_amount
		{
			get
			{
				return _interest_in_payment_amount;
			}

			set
			{
				_interest_in_payment_amount = value;
			}
		}

		private decimal _principle_balance;
		public decimal principle_balance
		{
			get
			{
				return _principle_balance;
			}

			set
			{
				_principle_balance = value;
			}
		}

		private decimal _applied_amt;
		public decimal applied_amt
		{
			get
			{
				return _applied_amt;
			}

			set
			{
				_applied_amt = value;
			}
		}

		private decimal _prorated_vsc;
		public decimal prorated_vsc
		{
			get
			{
				return _prorated_vsc;
			}

			set
			{
				_prorated_vsc = value;
			}
		}

		private decimal _prorated_psc;
		public decimal prorated_psc
		{
			get
			{
				return _prorated_psc;
			}

			set
			{
				_prorated_psc = value;
			}
		}
        public DateTime interest_accrual_date { get; set; }
        public decimal interest_accrual { get; set; }
    }
}

