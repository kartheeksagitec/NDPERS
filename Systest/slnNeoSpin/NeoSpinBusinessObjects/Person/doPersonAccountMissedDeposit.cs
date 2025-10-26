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
    public class doPersonAccountMissedDeposit : doBase
    {
         
         public doPersonAccountMissedDeposit() : base()
         {
         }
		private int _person_account_missed_deposit_id;
		public int person_account_missed_deposit_id
		{
			get
			{
				return _person_account_missed_deposit_id;
			}

			set
			{
				_person_account_missed_deposit_id = value;
			}
		}

		private int _person_account_id;
		public int person_account_id
		{
			get
			{
				return _person_account_id;
			}

			set
			{
				_person_account_id = value;
			}
		}

		private int _pay_period_month;
		public int pay_period_month
		{
			get
			{
				return _pay_period_month;
			}

			set
			{
				_pay_period_month = value;
			}
		}

		private int _pay_period_year;
		public int pay_period_year
		{
			get
			{
				return _pay_period_year;
			}

			set
			{
				_pay_period_year = value;
			}
		}

    }
}

