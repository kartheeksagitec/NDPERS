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
    public class doBenefitDnroPlsoEarlyFactor : doBase
    {
         public doBenefitDnroPlsoEarlyFactor() : base()
         {
         }
		private int _benefit_dnro_plso_factor_id;
		public int benefit_dnro_plso_factor_id
		{
			get
			{
				return _benefit_dnro_plso_factor_id;
			}

			set
			{
				_benefit_dnro_plso_factor_id = value;
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

		private decimal _ben_age;
		public decimal ben_age
		{
			get
			{
				return _ben_age;
			}

			set
			{
				_ben_age = value;
			}
		}

		private string _tran_type;
		public string tran_type
		{
			get
			{
				return _tran_type;
			}

			set
			{
				_tran_type = value;
			}
		}

		private string _sub_type;
		public string sub_type
		{
			get
			{
				return _sub_type;
			}

			set
			{
				_sub_type = value;
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

