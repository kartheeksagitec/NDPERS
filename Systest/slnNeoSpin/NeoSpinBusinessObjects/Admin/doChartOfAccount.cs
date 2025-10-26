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
    public class doChartOfAccount : doBase
    {
         public doChartOfAccount() : base()
         {
         }
		private int _chart_of_account_id;
		public int chart_of_account_id
		{
			get
			{
				return _chart_of_account_id;
			}

			set
			{
				_chart_of_account_id = value;
			}
		}

		private string _gl_account_number;
		public string gl_account_number
		{
			get
			{
				return _gl_account_number;
			}

			set
			{
				_gl_account_number = value;
			}
		}

		private int _account_type_id;
		public int account_type_id
		{
			get
			{
				return _account_type_id;
			}

			set
			{
				_account_type_id = value;
			}
		}

		private string _account_type_description;
		public string account_type_description
		{
			get
			{
				return _account_type_description;
			}

			set
			{
				_account_type_description = value;
			}
		}

		private string _account_type_value;
		public string account_type_value
		{
			get
			{
				return _account_type_value;
			}

			set
			{
				_account_type_value = value;
			}
		}

		private string _account_name;
		public string account_name
		{
			get
			{
				return _account_name;
			}

			set
			{
				_account_name = value;
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

