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
    public class doBenefitRhicReductionFactor : doBase
    {
         
         public doBenefitRhicReductionFactor() : base()
         {
         }
		private int _benefit_rhic_reduction_factor_id;
		public int benefit_rhic_reduction_factor_id
		{
			get
			{
				return _benefit_rhic_reduction_factor_id;
			}

			set
			{
				_benefit_rhic_reduction_factor_id = value;
			}
		}

		private int _min_age;
		public int min_age
		{
			get
			{
				return _min_age;
			}

			set
			{
				_min_age = value;
			}
		}

		private int _max_age;
		public int max_age
		{
			get
			{
				return _max_age;
			}

			set
			{
				_max_age = value;
			}
		}

		private decimal _factor;
		public decimal factor
		{
			get
			{
				return _factor;
			}

			set
			{
				_factor = value;
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

