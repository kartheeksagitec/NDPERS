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
    public class doJsRhicRemittanceAllocation : doBase
    {
         
         public doJsRhicRemittanceAllocation() : base()
         {
         }
		private int _rhic_remittance_allocation_id;
		public int rhic_remittance_allocation_id
		{
			get
			{
				return _rhic_remittance_allocation_id;
			}

			set
			{
				_rhic_remittance_allocation_id = value;
			}
		}

		private int _remittance_id;
		public int remittance_id
		{
			get
			{
				return _remittance_id;
			}

			set
			{
				_remittance_id = value;
			}
		}

		private int _ibs_header_id;
		public int ibs_header_id
		{
			get
			{
				return _ibs_header_id;
			}

			set
			{
				_ibs_header_id = value;
			}
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

		private int _rhic_allocation_status_id;
		public int rhic_allocation_status_id
		{
			get
			{
				return _rhic_allocation_status_id;
			}

			set
			{
				_rhic_allocation_status_id = value;
			}
		}

		private string _rhic_allocation_status_description;
		public string rhic_allocation_status_description
		{
			get
			{
				return _rhic_allocation_status_description;
			}

			set
			{
				_rhic_allocation_status_description = value;
			}
		}

		private string _rhic_allocation_status_value;
		public string rhic_allocation_status_value
		{
			get
			{
				return _rhic_allocation_status_value;
			}

			set
			{
				_rhic_allocation_status_value = value;
			}
		}

    }
}

