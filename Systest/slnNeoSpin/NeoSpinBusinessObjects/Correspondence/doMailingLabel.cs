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
    public class doMailingLabel : doBase
    {
         
         public doMailingLabel() : base()
         {
         }
		private int _mailing_label_batch_id;
		public int mailing_label_batch_id
		{
			get
			{
				return _mailing_label_batch_id;
			}

			set
			{
				_mailing_label_batch_id = value;
			}
		}

		private string _file_name;
		public string file_name
		{
			get
			{
				return _file_name;
			}

			set
			{
				_file_name = value;
			}
		}

		private int _output_format_id;
		public int output_format_id
		{
			get
			{
				return _output_format_id;
			}

			set
			{
				_output_format_id = value;
			}
		}

		private string _output_format_description;
		public string output_format_description
		{
			get
			{
				return _output_format_description;
			}

			set
			{
				_output_format_description = value;
			}
		}

		private string _output_format_value;
		public string output_format_value
		{
			get
			{
				return _output_format_value;
			}

			set
			{
				_output_format_value = value;
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

		private string _exclude_email_preference_flag;
		public string exclude_email_preference_flag
		{
			get
			{
				return _exclude_email_preference_flag;
			}

			set
			{
				_exclude_email_preference_flag = value;
			}
		}

		private int _employment_status_id;
		public int employment_status_id
		{
			get
			{
				return _employment_status_id;
			}

			set
			{
				_employment_status_id = value;
			}
		}

		private string _employment_status_description;
		public string employment_status_description
		{
			get
			{
				return _employment_status_description;
			}

			set
			{
				_employment_status_description = value;
			}
		}

		private string _employment_status_value;
		public string employment_status_value
		{
			get
			{
				return _employment_status_value;
			}

			set
			{
				_employment_status_value = value;
			}
		}

		private int _org_type_id;
		public int org_type_id
		{
			get
			{
				return _org_type_id;
			}

			set
			{
				_org_type_id = value;
			}
		}

		private string _org_type_description;
		public string org_type_description
		{
			get
			{
				return _org_type_description;
			}

			set
			{
				_org_type_description = value;
			}
		}

		private string _org_type_value;
		public string org_type_value
		{
			get
			{
				return _org_type_value;
			}

			set
			{
				_org_type_value = value;
			}
		}

		private int _employer_type_id;
		public int employer_type_id
		{
			get
			{
				return _employer_type_id;
			}

			set
			{
				_employer_type_id = value;
			}
		}

		private string _employer_type_description;
		public string employer_type_description
		{
			get
			{
				return _employer_type_description;
			}

			set
			{
				_employer_type_description = value;
			}
		}

		private string _employer_type_value;
		public string employer_type_value
		{
			get
			{
				return _employer_type_value;
			}

			set
			{
				_employer_type_value = value;
			}
		}

		private int _org_contact_role_id;
		public int org_contact_role_id
		{
			get
			{
				return _org_contact_role_id;
			}

			set
			{
				_org_contact_role_id = value;
			}
		}

		private string _org_contact_role_description;
		public string org_contact_role_description
		{
			get
			{
				return _org_contact_role_description;
			}

			set
			{
				_org_contact_role_description = value;
			}
		}

		private string _org_contact_role_value;
		public string org_contact_role_value
		{
			get
			{
				return _org_contact_role_value;
			}

			set
			{
				_org_contact_role_value = value;
			}
		}

		private string _county;
		public string county
		{
			get
			{
				return _county;
			}

			set
			{
				_county = value;
			}
		}

		private string _city;
		public string city
		{
			get
			{
				return _city;
			}

			set
			{
				_city = value;
			}
		}

		private string _zip_code;
		public string zip_code
		{
			get
			{
				return _zip_code;
			}

			set
			{
				_zip_code = value;
			}
		}

		private DateTime _run_date;
		public DateTime run_date
		{
			get
			{
				return _run_date;
			}

			set
			{
				_run_date = value;
			}
		}

		private string _user_id;
		public string user_id
		{
			get
			{
				return _user_id;
			}

			set
			{
				_user_id = value;
			}
		}

    }
}

