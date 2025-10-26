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
    public class doPayeeAccountTaxWithholdingItemDetail : doBase
    {
         
         public doPayeeAccountTaxWithholdingItemDetail() : base()
         {
         }
		private int _payee_account_tax_withholding_item_dtl_id;
		public int payee_account_tax_withholding_item_dtl_id
		{
			get
			{
				return _payee_account_tax_withholding_item_dtl_id;
			}

			set
			{
				_payee_account_tax_withholding_item_dtl_id = value;
			}
		}

		private int _payee_account_tax_withholding_id;
		public int payee_account_tax_withholding_id
		{
			get
			{
				return _payee_account_tax_withholding_id;
			}

			set
			{
				_payee_account_tax_withholding_id = value;
			}
		}

		private int _payee_account_payment_item_type_id;
		public int payee_account_payment_item_type_id
		{
			get
			{
				return _payee_account_payment_item_type_id;
			}

			set
			{
				_payee_account_payment_item_type_id = value;
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

		private decimal _amount;
		public decimal amount
		{
			get
			{
				return _amount;
			}

			set
			{
				_amount = value;
			}
		}

    }
}

