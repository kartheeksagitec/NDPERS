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
    public class doOrgPlanEapRate : doBase
    {
         
         public doOrgPlanEapRate() : base()
         {
         }
		private int _org_plan_eap_rate_id;
		public int org_plan_eap_rate_id
		{
			get
			{
				return _org_plan_eap_rate_id;
			}

			set
			{
				_org_plan_eap_rate_id = value;
			}
		}

		private int _org_plan_id;
		public int org_plan_id
		{
			get
			{
				return _org_plan_id;
			}

			set
			{
				_org_plan_id = value;
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

		private int _eap_insurance_type_id;
		public int eap_insurance_type_id
		{
			get
			{
				return _eap_insurance_type_id;
			}

			set
			{
				_eap_insurance_type_id = value;
			}
		}

		private string _eap_insurance_type_description;
		public string eap_insurance_type_description
		{
			get
			{
				return _eap_insurance_type_description;
			}

			set
			{
				_eap_insurance_type_description = value;
			}
		}

		private string _eap_insurance_type_value;
		public string eap_insurance_type_value
		{
			get
			{
				return _eap_insurance_type_value;
			}

			set
			{
				_eap_insurance_type_value = value;
			}
		}

		private decimal _premium_amt;
		public decimal premium_amt
		{
			get
			{
				return _premium_amt;
			}

			set
			{
				_premium_amt = value;
			}
		}

    }
}

