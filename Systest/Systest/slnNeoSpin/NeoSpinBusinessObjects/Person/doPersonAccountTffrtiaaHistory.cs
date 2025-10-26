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
    public class doPersonAccountTffrtiaaHistory : doBase
    {
         
         public doPersonAccountTffrtiaaHistory() : base()
         {
         }
		private int _person_account_tffrtiaa_history_id;
		public int person_account_tffrtiaa_history_id
		{
			get
			{
				return _person_account_tffrtiaa_history_id;
			}

			set
			{
				_person_account_tffrtiaa_history_id = value;
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

		private int _provider_org_id;
		public int provider_org_id
		{
			get
			{
				return _provider_org_id;
			}

			set
			{
				_provider_org_id = value;
			}
		}

		private int _plan_participation_status_id;
		public int plan_participation_status_id
		{
			get
			{
				return _plan_participation_status_id;
			}

			set
			{
				_plan_participation_status_id = value;
			}
		}

		private string _plan_participation_status_description;
		public string plan_participation_status_description
		{
			get
			{
				return _plan_participation_status_description;
			}

			set
			{
				_plan_participation_status_description = value;
			}
		}

		private string _plan_participation_status_value;
		public string plan_participation_status_value
		{
			get
			{
				return _plan_participation_status_value;
			}

			set
			{
				_plan_participation_status_value = value;
			}
		}

		private int _status_id;
		public int status_id
		{
			get
			{
				return _status_id;
			}

			set
			{
				_status_id = value;
			}
		}

		private string _status_description;
		public string status_description
		{
			get
			{
				return _status_description;
			}

			set
			{
				_status_description = value;
			}
		}

		private string _status_value;
		public string status_value
		{
			get
			{
				return _status_value;
			}

			set
			{
				_status_value = value;
			}
		}

		private int _from_person_account_id;
		public int from_person_account_id
		{
			get
			{
				return _from_person_account_id;
			}

			set
			{
				_from_person_account_id = value;
			}
		}

		private int _to_person_account_id;
		public int to_person_account_id
		{
			get
			{
				return _to_person_account_id;
			}

			set
			{
				_to_person_account_id = value;
			}
		}

		private string _suppress_warnings_flag;
		public string suppress_warnings_flag
		{
			get
			{
				return _suppress_warnings_flag;
			}

			set
			{
				_suppress_warnings_flag = value;
			}
		}

		private string _suppress_warnings_by;
		public string suppress_warnings_by
		{
			get
			{
				return _suppress_warnings_by;
			}

			set
			{
				_suppress_warnings_by = value;
			}
		}

		private DateTime _suppress_warnings_date;
		public DateTime suppress_warnings_date
		{
			get
			{
				return _suppress_warnings_date;
			}

			set
			{
				_suppress_warnings_date = value;
			}
		}

    }
}

