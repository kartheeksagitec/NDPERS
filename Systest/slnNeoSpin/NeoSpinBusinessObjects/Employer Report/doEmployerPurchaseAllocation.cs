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
    public class doEmployerPurchaseAllocation : doBase
    {
         
         public doEmployerPurchaseAllocation() : base()
         {
         }
		private int _employer_purchase_allocation_id;
		public int employer_purchase_allocation_id
		{
			get
			{
				return _employer_purchase_allocation_id;
			}

			set
			{
				_employer_purchase_allocation_id = value;
			}
		}

		private int _service_purchase_header_id;
		public int service_purchase_header_id
		{
			get
			{
				return _service_purchase_header_id;
			}

			set
			{
				_service_purchase_header_id = value;
			}
		}

		private int _employer_payroll_detail_id;
		public int employer_payroll_detail_id
		{
			get
			{
				return _employer_payroll_detail_id;
			}

			set
			{
				_employer_payroll_detail_id = value;
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

		private DateTime _allocated_date;
		public DateTime allocated_date
		{
			get
			{
				return _allocated_date;
			}

			set
			{
				_allocated_date = value;
			}
		}

    }
}

