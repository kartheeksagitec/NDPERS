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
    public class doJsRhicBill : doBase
    {
         
         public doJsRhicBill() : base()
         {
         }
		private int _js_rhic_bill_id;
		public int js_rhic_bill_id
		{
			get
			{
				return _js_rhic_bill_id;
			}

			set
			{
				_js_rhic_bill_id = value;
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

		private DateTime _bill_date;
		public DateTime bill_date
		{
			get
			{
				return _bill_date;
			}

			set
			{
				_bill_date = value;
			}
		}

		private decimal _bill_amount;
		public decimal bill_amount
		{
			get
			{
				return _bill_amount;
			}

			set
			{
				_bill_amount = value;
			}
		}

		private decimal _allocated_amount;
		public decimal allocated_amount
		{
			get
			{
				return _allocated_amount;
			}

			set
			{
				_allocated_amount = value;
			}
		}

    }
}

