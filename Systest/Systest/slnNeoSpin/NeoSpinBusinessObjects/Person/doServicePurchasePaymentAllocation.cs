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
    public class doServicePurchasePaymentAllocation : doBase
    {
         
         public doServicePurchasePaymentAllocation() : base()
         {
         }
		private int _service_purchase_payment_allocation_id;
		public int service_purchase_payment_allocation_id
		{
			get
			{
				return _service_purchase_payment_allocation_id;
			}

			set
			{
				_service_purchase_payment_allocation_id = value;
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

		private int _remittance_id;
		public int remittance_id
		{
			get
			{
				return _remittance_id;
			}

			set
			{
				_remittance_id = value;
			}
		}

		private int _employer_payroll_detail_id;
		public int employer_payroll_detail_id
		{
			get
			{
				return _employer_payroll_detail_id;
			}

			set
			{
				_employer_payroll_detail_id = value;
			}
		}

		private int _service_purchase_payment_class_id;
		public int service_purchase_payment_class_id
		{
			get
			{
				return _service_purchase_payment_class_id;
			}

			set
			{
				_service_purchase_payment_class_id = value;
			}
		}

		private string _service_purchase_payment_class_description;
		public string service_purchase_payment_class_description
		{
			get
			{
				return _service_purchase_payment_class_description;
			}

			set
			{
				_service_purchase_payment_class_description = value;
			}
		}

		private string _service_purchase_payment_class_value;
		public string service_purchase_payment_class_value
		{
			get
			{
				return _service_purchase_payment_class_value;
			}

			set
			{
				_service_purchase_payment_class_value = value;
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

		private decimal _applied_amount;
		public decimal applied_amount
		{
			get
			{
				return _applied_amount;
			}

			set
			{
				_applied_amount = value;
			}
		}

		private string _posted_flag;
		public string posted_flag
		{
			get
			{
				return _posted_flag;
			}

			set
			{
				_posted_flag = value;
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

		private decimal _post_tax_er_amount;
		public decimal post_tax_er_amount
		{
			get
			{
				return _post_tax_er_amount;
			}

			set
			{
				_post_tax_er_amount = value;
			}
		}

		private decimal _post_tax_ee_amount;
		public decimal post_tax_ee_amount
		{
			get
			{
				return _post_tax_ee_amount;
			}

			set
			{
				_post_tax_ee_amount = value;
			}
		}

		private decimal _pre_tax_er_amount;
		public decimal pre_tax_er_amount
		{
			get
			{
				return _pre_tax_er_amount;
			}

			set
			{
				_pre_tax_er_amount = value;
			}
		}

		private decimal _pre_tax_ee_amount;
		public decimal pre_tax_ee_amount
		{
			get
			{
				return _pre_tax_ee_amount;
			}

			set
			{
				_pre_tax_ee_amount = value;
			}
		}

		private decimal _ee_rhic_amount;
		public decimal ee_rhic_amount
		{
			get
			{
				return _ee_rhic_amount;
			}

			set
			{
				_ee_rhic_amount = value;
			}
		}

		private decimal _er_rhic_amount;
		public decimal er_rhic_amount
		{
			get
			{
				return _er_rhic_amount;
			}

			set
			{
				_er_rhic_amount = value;
			}
		}

    }
}

