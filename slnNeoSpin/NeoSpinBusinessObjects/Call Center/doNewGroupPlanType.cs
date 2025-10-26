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
    public class doNewGroupPlanType : doBase
    {
         
         public doNewGroupPlanType() : base()
         {
         }
		private int _new_group_plan_type_id;
		public int new_group_plan_type_id
		{
			get
			{
				return _new_group_plan_type_id;
			}

			set
			{
				_new_group_plan_type_id = value;
			}
		}

		private int _new_group_id;
		public int new_group_id
		{
			get
			{
				return _new_group_id;
			}

			set
			{
				_new_group_id = value;
			}
		}

		private int _plan_type_id;
		public int plan_type_id
		{
			get
			{
				return _plan_type_id;
			}

			set
			{
				_plan_type_id = value;
			}
		}

		private string _plan_type_description;
		public string plan_type_description
		{
			get
			{
				return _plan_type_description;
			}

			set
			{
				_plan_type_description = value;
			}
		}

		private string _plan_type_value;
		public string plan_type_value
		{
			get
			{
				return _plan_type_value;
			}

			set
			{
				_plan_type_value = value;
			}
		}

    }
}

