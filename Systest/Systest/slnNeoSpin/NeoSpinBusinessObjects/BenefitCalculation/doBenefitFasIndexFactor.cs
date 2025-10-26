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
    public class doBenefitFasIndexFactor : doBase
    {
         public doBenefitFasIndexFactor() : base()
         {
         }
		private int _benefit_fas_index_factor_id;
		public int benefit_fas_index_factor_id
		{
			get
			{
				return _benefit_fas_index_factor_id;
			}

			set
			{
				_benefit_fas_index_factor_id = value;
			}
		}

		private DateTime _fas_start_date;
		public DateTime fas_start_date
		{
			get
			{
				return _fas_start_date;
			}

			set
			{
				_fas_start_date = value;
			}
		}

		private DateTime _fas_end_date;
		public DateTime fas_end_date
		{
			get
			{
				return _fas_end_date;
			}

			set
			{
				_fas_end_date = value;
			}
		}

		private decimal _average_increase_factor;
		public decimal average_increase_factor
		{
			get
			{
				return _average_increase_factor;
			}

			set
			{
				_average_increase_factor = value;
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

