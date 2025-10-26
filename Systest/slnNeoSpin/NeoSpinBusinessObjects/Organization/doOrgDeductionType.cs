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
    public class doOrgDeductionType : doBase
    {
         
         public doOrgDeductionType() : base()
         {
         }
		private int _org_deduction_type_id;
		public int org_deduction_type_id
		{
			get
			{
				return _org_deduction_type_id;
			}

			set
			{
				_org_deduction_type_id = value;
			}
		}

		private int _org_id;
		public int org_id
		{
			get
			{
				return _org_id;
			}

			set
			{
				_org_id = value;
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

		private DateTime _start_date;
		public DateTime start_date
		{
			get
			{
				return _start_date;
			}

			set
			{
				_start_date = value;
			}
		}

		private DateTime _end_date;
		public DateTime end_date
		{
			get
			{
				return _end_date;
			}

			set
			{
				_end_date = value;
			}
		}

    }
}

