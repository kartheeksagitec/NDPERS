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
    public class doContactMgmtServicePurchase : doBase
    {
         
         public doContactMgmtServicePurchase() : base()
         {
         }
		private int _service_purchase_id;
		public int service_purchase_id
		{
			get
			{
				return _service_purchase_id;
			}

			set
			{
				_service_purchase_id = value;
			}
		}

		private int _contact_ticket_id;
		public int contact_ticket_id
		{
			get
			{
				return _contact_ticket_id;
			}

			set
			{
				_contact_ticket_id = value;
			}
		}

		private DateTime _retirement_effective_date;
		public DateTime retirement_effective_date
		{
			get
			{
				return _retirement_effective_date;
			}

			set
			{
				_retirement_effective_date = value;
			}
		}

		private string _conver_unused_sick_leave_flag;
		public string conver_unused_sick_leave_flag
		{
			get
			{
				return _conver_unused_sick_leave_flag;
			}

			set
			{
				_conver_unused_sick_leave_flag = value;
			}
		}

		private int _unused_sick_leave_hours;
		public int unused_sick_leave_hours
		{
			get
			{
				return _unused_sick_leave_hours;
			}

			set
			{
				_unused_sick_leave_hours = value;
			}
		}

		private string _addi_generic_flag;
		public string addi_generic_flag
		{
			get
			{
				return _addi_generic_flag;
			}

			set
			{
				_addi_generic_flag = value;
			}
		}

		private int _addi_generic_month;
		public int addi_generic_month
		{
			get
			{
				return _addi_generic_month;
			}

			set
			{
				_addi_generic_month = value;
			}
		}

		private decimal _addi_generic_amount;
		public decimal addi_generic_amount
		{
			get
			{
				return _addi_generic_amount;
			}

			set
			{
				_addi_generic_amount = value;
			}
		}

		private decimal _est_funds_to_be_util_amount;
		public decimal est_funds_to_be_util_amount
		{
			get
			{
				return _est_funds_to_be_util_amount;
			}

			set
			{
				_est_funds_to_be_util_amount = value;
			}
		}

    }
}

