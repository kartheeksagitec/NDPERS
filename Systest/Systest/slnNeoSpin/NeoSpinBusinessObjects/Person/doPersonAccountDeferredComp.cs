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
    public class doPersonAccountDeferredComp : doBase
    {
         
         public doPersonAccountDeferredComp() : base()
         {
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

		private DateTime _catch_up_start_date;
		public DateTime catch_up_start_date
		{
			get
			{
				return _catch_up_start_date;
			}

			set
			{
				_catch_up_start_date = value;
			}
		}

		private DateTime _catch_up_end_date;
		public DateTime catch_up_end_date
		{
			get
			{
				return _catch_up_end_date;
			}

			set
			{
				_catch_up_end_date = value;
			}
		}

		private int _limit_457_id;
		public int limit_457_id
		{
			get
			{
				return _limit_457_id;
			}

			set
			{
				_limit_457_id = value;
			}
		}

		private string _limit_457_description;
		public string limit_457_description
		{
			get
			{
				return _limit_457_description;
			}

			set
			{
				_limit_457_description = value;
			}
		}

		private string _limit_457_value;
		public string limit_457_value
		{
			get
			{
				return _limit_457_value;
			}

			set
			{
				_limit_457_value = value;
			}
		}

		private string _hardship_withdrawal_flag;
		public string hardship_withdrawal_flag
		{
			get
			{
				return _hardship_withdrawal_flag;
			}

			set
			{
				_hardship_withdrawal_flag = value;
			}
		}

		private DateTime _hardship_withdrawal_effective_date;
		public DateTime hardship_withdrawal_effective_date
		{
			get
			{
				return _hardship_withdrawal_effective_date;
			}

			set
			{
				_hardship_withdrawal_effective_date = value;
			}
		}

		private string _de_minimus_distribution_flag;
		public string de_minimus_distribution_flag
		{
			get
			{
				return _de_minimus_distribution_flag;
			}

			set
			{
				_de_minimus_distribution_flag = value;
			}
		}

		private string _file_457_sent_flag;
		public string file_457_sent_flag
		{
			get
			{
				return _file_457_sent_flag;
			}

			set
			{
				_file_457_sent_flag = value;
			}
		}

    }
}

