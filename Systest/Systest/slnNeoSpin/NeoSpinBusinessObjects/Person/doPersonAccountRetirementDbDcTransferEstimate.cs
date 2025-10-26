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
    public class doPersonAccountRetirementDbDcTransferEstimate : doBase
    {
         
         public doPersonAccountRetirementDbDcTransferEstimate() : base()
         {
         }
		private int _db_dc_transfer_estimate_id;
		public int db_dc_transfer_estimate_id
		{
			get
			{
				return _db_dc_transfer_estimate_id;
			}

			set
			{
				_db_dc_transfer_estimate_id = value;
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

		private decimal _proj_monthly_salary_amount;
		public decimal proj_monthly_salary_amount
		{
			get
			{
				return _proj_monthly_salary_amount;
			}

			set
			{
				_proj_monthly_salary_amount = value;
			}
		}

		private int _proj_month_no;
		public int proj_month_no
		{
			get
			{
				return _proj_month_no;
			}

			set
			{
				_proj_month_no = value;
			}
		}

		private decimal _proj_total_ee_amount;
		public decimal proj_total_ee_amount
		{
			get
			{
				return _proj_total_ee_amount;
			}

			set
			{
				_proj_total_ee_amount = value;
			}
		}

		private decimal _proj_total_er_amount;
		public decimal proj_total_er_amount
		{
			get
			{
				return _proj_total_er_amount;
			}

			set
			{
				_proj_total_er_amount = value;
			}
		}

		private decimal _proj_total_transfer_amount;
		public decimal proj_total_transfer_amount
		{
			get
			{
				return _proj_total_transfer_amount;
			}

			set
			{
				_proj_total_transfer_amount = value;
			}
		}

		private decimal _proj_ee_interest_amount;
		public decimal proj_ee_interest_amount
		{
			get
			{
				return _proj_ee_interest_amount;
			}

			set
			{
				_proj_ee_interest_amount = value;
			}
		}

		private decimal _proj_er_interest_amount;
		public decimal proj_er_interest_amount
		{
			get
			{
				return _proj_er_interest_amount;
			}

			set
			{
				_proj_er_interest_amount = value;
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

		private string _suppress_warnings_flag;
		public string suppress_warnings_flag
		{
			get
			{
				return _suppress_warnings_flag;
			}

			set
			{
				_suppress_warnings_flag = value;
			}
		}

		private string _suppress_warnings_by;
		public string suppress_warnings_by
		{
			get
			{
				return _suppress_warnings_by;
			}

			set
			{
				_suppress_warnings_by = value;
			}
		}

		private DateTime _suppress_warnings_date;
		public DateTime suppress_warnings_date
		{
			get
			{
				return _suppress_warnings_date;
			}

			set
			{
				_suppress_warnings_date = value;
			}
		}

    }
}

