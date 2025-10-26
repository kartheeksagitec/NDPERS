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
    public class doPayeeAccountRolloverItemDetail : doBase
    {
         
         public doPayeeAccountRolloverItemDetail() : base()
         {
         }
		private int _payee_account_rollover_item_detail_id;
		public int payee_account_rollover_item_detail_id
		{
			get
			{
				return _payee_account_rollover_item_detail_id;
			}

			set
			{
				_payee_account_rollover_item_detail_id = value;
			}
		}

		private int _payee_account_rollover_detail_id;
		public int payee_account_rollover_detail_id
		{
			get
			{
				return _payee_account_rollover_detail_id;
			}

			set
			{
				_payee_account_rollover_detail_id = value;
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

    }
}

