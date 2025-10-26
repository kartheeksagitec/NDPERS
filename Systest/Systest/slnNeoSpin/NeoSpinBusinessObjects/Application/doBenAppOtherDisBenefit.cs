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
    public class doBenAppOtherDisBenefit : doBase
    {
         public doBenAppOtherDisBenefit() : base()
         {
         }
		private int _ben_app_other_dis_benefit_id;
		public int ben_app_other_dis_benefit_id
		{
			get
			{
				return _ben_app_other_dis_benefit_id;
			}

			set
			{
				_ben_app_other_dis_benefit_id = value;
			}
		}

		private int _benefit_application_id;
		public int benefit_application_id
		{
			get
			{
				return _benefit_application_id;
			}

			set
			{
				_benefit_application_id = value;
			}
		}

		private int _other_disability_benefit_id;
		public int other_disability_benefit_id
		{
			get
			{
				return _other_disability_benefit_id;
			}

			set
			{
				_other_disability_benefit_id = value;
			}
		}

		private string _other_disability_benefit_description;
		public string other_disability_benefit_description
		{
			get
			{
				return _other_disability_benefit_description;
			}

			set
			{
				_other_disability_benefit_description = value;
			}
		}

		private string _other_disability_benefit_value;
		public string other_disability_benefit_value
		{
			get
			{
				return _other_disability_benefit_value;
			}

			set
			{
				_other_disability_benefit_value = value;
			}
		}

		private DateTime _benefit_begin_date;
		public DateTime benefit_begin_date
		{
			get
			{
				return _benefit_begin_date;
			}

			set
			{
				_benefit_begin_date = value;
			}
		}

		private DateTime _benefit_end_date;
		public DateTime benefit_end_date
		{
			get
			{
				return _benefit_end_date;
			}

			set
			{
				_benefit_end_date = value;
			}
		}

		private decimal _monthly_benefit_amount;
		public decimal monthly_benefit_amount
		{
			get
			{
				return _monthly_benefit_amount;
			}

			set
			{
				_monthly_benefit_amount = value;
			}
		}

    }
}

