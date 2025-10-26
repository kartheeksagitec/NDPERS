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
    public class doBenefitProvisionMultiplier : doBase
    {
         
         public doBenefitProvisionMultiplier() : base()
         {
         }
		private int _benefit_provision_multiplier_id;
		public int benefit_provision_multiplier_id
		{
			get
			{
				return _benefit_provision_multiplier_id;
			}

			set
			{
				_benefit_provision_multiplier_id = value;
			}
		}

		private int _benefit_provision_id;
		public int benefit_provision_id
		{
			get
			{
				return _benefit_provision_id;
			}

			set
			{
				_benefit_provision_id = value;
			}
		}

		private DateTime _effective_date;
		public DateTime effective_date
		{
			get
			{
				return _effective_date;
			}

			set
			{
				_effective_date = value;
			}
		}

		private int _benefit_account_type_id;
		public int benefit_account_type_id
		{
			get
			{
				return _benefit_account_type_id;
			}

			set
			{
				_benefit_account_type_id = value;
			}
		}

		private string _benefit_account_type_description;
		public string benefit_account_type_description
		{
			get
			{
				return _benefit_account_type_description;
			}

			set
			{
				_benefit_account_type_description = value;
			}
		}

		private string _benefit_account_type_value;
		public string benefit_account_type_value
		{
			get
			{
				return _benefit_account_type_value;
			}

			set
			{
				_benefit_account_type_value = value;
			}
		}

		private string _is_flat_percentage_flag;
		public string is_flat_percentage_flag
		{
			get
			{
				return _is_flat_percentage_flag;
			}

			set
			{
				_is_flat_percentage_flag = value;
			}
		}

		private int _service_from;
		public int service_from
		{
			get
			{
				return _service_from;
			}

			set
			{
				_service_from = value;
			}
		}

		private int _service_to;
		public int service_to
		{
			get
			{
				return _service_to;
			}

			set
			{
				_service_to = value;
			}
		}

		private decimal _multipier_percentage;
		public decimal multipier_percentage
		{
			get
			{
				return _multipier_percentage;
			}

			set
			{
				_multipier_percentage = value;
			}
		}

		private int _tier_number;
		public int tier_number
		{
			get
			{
				return _tier_number;
			}

			set
			{
				_tier_number = value;
			}
		}

		private string _comments;
		public string comments
		{
			get
			{
				return _comments;
			}

			set
			{
				_comments = value;
			}
		}

		private string _isconversion;
		public string isconversion
		{
			get
			{
				return _isconversion;
			}

			set
			{
				_isconversion = value;
			}
		}
		public int benefit_multiplier_type_id { get; set; }
        public string benefit_multiplier_type_description { get; set; }
        public string benefit_multiplier_type_value { get; set; }

    }
}

