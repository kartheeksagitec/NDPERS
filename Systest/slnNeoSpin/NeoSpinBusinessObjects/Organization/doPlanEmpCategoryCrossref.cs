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
    public class doPlanEmpCategoryCrossref : doBase
    {
         
         public doPlanEmpCategoryCrossref() : base()
         {
         }
		private int _plan_emp_category_id;
		public int plan_emp_category_id
		{
			get
			{
				return _plan_emp_category_id;
			}

			set
			{
				_plan_emp_category_id = value;
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

		private int _emp_category_id;
		public int emp_category_id
		{
			get
			{
				return _emp_category_id;
			}

			set
			{
				_emp_category_id = value;
			}
		}

		private string _emp_category_description;
		public string emp_category_description
		{
			get
			{
				return _emp_category_description;
			}

			set
			{
				_emp_category_description = value;
			}
		}

		private string _emp_category_value;
		public string emp_category_value
		{
			get
			{
				return _emp_category_value;
			}

			set
			{
				_emp_category_value = value;
			}
		}

		private int _required_flag_option_id;
		public int required_flag_option_id
		{
			get
			{
				return _required_flag_option_id;
			}

			set
			{
				_required_flag_option_id = value;
			}
		}

		private string _required_flag_option_description;
		public string required_flag_option_description
		{
			get
			{
				return _required_flag_option_description;
			}

			set
			{
				_required_flag_option_description = value;
			}
		}

		private string _required_flag_option_value;
		public string required_flag_option_value
		{
			get
			{
				return _required_flag_option_value;
			}

			set
			{
				_required_flag_option_value = value;
			}
		}

    }
}

