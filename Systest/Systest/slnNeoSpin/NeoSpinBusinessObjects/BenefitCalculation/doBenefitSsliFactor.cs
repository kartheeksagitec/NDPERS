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
    public class doBenefitSsliFactor : doBase
    {
         
         public doBenefitSsliFactor() : base()
         {
         }
		private int _benefit_ssli_factor_id;
		public int benefit_ssli_factor_id
		{
			get
			{
				return _benefit_ssli_factor_id;
			}

			set
			{
				_benefit_ssli_factor_id = value;
			}
		}

		private decimal _member_age;
		public decimal member_age
		{
			get
			{
				return _member_age;
			}

			set
			{
				_member_age = value;
			}
		}

		private decimal _ssli_age;
		public decimal ssli_age
		{
			get
			{
				return _ssli_age;
			}

			set
			{
				_ssli_age = value;
			}
		}

		private decimal _ssli_factor;
		public decimal ssli_factor
		{
			get
			{
				return _ssli_factor;
			}

			set
			{
				_ssli_factor = value;
			}
		}

		private int _plan_id;
		public int plan_id
		{
			get
			{
				return _plan_id;
			}

			set
			{
				_plan_id = value;
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

    }
}

