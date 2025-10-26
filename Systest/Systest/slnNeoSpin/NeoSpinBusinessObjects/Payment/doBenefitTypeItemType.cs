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
    public class doBenefitTypeItemType : doBase
    {
         
         public doBenefitTypeItemType() : base()
         {
         }
		private int _benefit_type_item_type_id;
		public int benefit_type_item_type_id
		{
			get
			{
				return _benefit_type_item_type_id;
			}

			set
			{
				_benefit_type_item_type_id = value;
			}
		}

		private int _payment_item_type_id;
		public int payment_item_type_id
		{
			get
			{
				return _payment_item_type_id;
			}

			set
			{
				_payment_item_type_id = value;
			}
		}

		private int _benefit_type_id;
		public int benefit_type_id
		{
			get
			{
				return _benefit_type_id;
			}

			set
			{
				_benefit_type_id = value;
			}
		}

		private string _benefit_type_description;
		public string benefit_type_description
		{
			get
			{
				return _benefit_type_description;
			}

			set
			{
				_benefit_type_description = value;
			}
		}

		private string _benefit_type_value;
		public string benefit_type_value
		{
			get
			{
				return _benefit_type_value;
			}

			set
			{
				_benefit_type_value = value;
			}
		}

		private int _benefit_sub_type_id;
		public int benefit_sub_type_id
		{
			get
			{
				return _benefit_sub_type_id;
			}

			set
			{
				_benefit_sub_type_id = value;
			}
		}

		private string _benefit_sub_type_description;
		public string benefit_sub_type_description
		{
			get
			{
				return _benefit_sub_type_description;
			}

			set
			{
				_benefit_sub_type_description = value;
			}
		}

		private string _benefit_sub_type_value;
		public string benefit_sub_type_value
		{
			get
			{
				return _benefit_sub_type_value;
			}

			set
			{
				_benefit_sub_type_value = value;
			}
		}

    }
}

