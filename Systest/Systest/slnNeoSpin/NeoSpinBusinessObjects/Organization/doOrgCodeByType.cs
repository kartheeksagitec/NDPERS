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
    public class doOrgCodeByType : doBase
    {
         
         public doOrgCodeByType() : base()
         {
         }
		private int _org_code_by_type_id;
		public int org_code_by_type_id
		{
			get
			{
				return _org_code_by_type_id;
			}

			set
			{
				_org_code_by_type_id = value;
			}
		}

		private int _org_type_id;
		public int org_type_id
		{
			get
			{
				return _org_type_id;
			}

			set
			{
				_org_type_id = value;
			}
		}

		private string _org_type_description;
		public string org_type_description
		{
			get
			{
				return _org_type_description;
			}

			set
			{
				_org_type_description = value;
			}
		}

		private string _org_type_value;
		public string org_type_value
		{
			get
			{
				return _org_type_value;
			}

			set
			{
				_org_type_value = value;
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

		private int _org_code_range_id;
		public int org_code_range_id
		{
			get
			{
				return _org_code_range_id;
			}

			set
			{
				_org_code_range_id = value;
			}
		}

    }
}

