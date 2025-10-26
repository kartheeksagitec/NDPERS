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
    public class doProviderReportDataDeffComp : doBase
    {
         
         public doProviderReportDataDeffComp() : base()
         {
         }
		private int _provider_report_data_deff_comp_id;
		public int provider_report_data_deff_comp_id
		{
			get
			{
				return _provider_report_data_deff_comp_id;
			}

			set
			{
				_provider_report_data_deff_comp_id = value;
			}
		}

		private int _subsystem_id;
		public int subsystem_id
		{
			get
			{
				return _subsystem_id;
			}

			set
			{
				_subsystem_id = value;
			}
		}

		private string _subsystem_description;
		public string subsystem_description
		{
			get
			{
				return _subsystem_description;
			}

			set
			{
				_subsystem_description = value;
			}
		}

		private string _subsystem_value;
		public string subsystem_value
		{
			get
			{
				return _subsystem_value;
			}

			set
			{
				_subsystem_value = value;
			}
		}

		private int _subsystem_ref_id;
		public int subsystem_ref_id
		{
			get
			{
				return _subsystem_ref_id;
			}

			set
			{
				_subsystem_ref_id = value;
			}
		}

		private int _person_id;
		public int person_id
		{
			get
			{
				return _person_id;
			}

			set
			{
				_person_id = value;
			}
		}

		private string _ssn;
		public string ssn
		{
			get
			{
				return _ssn;
			}

			set
			{
				_ssn = value;
			}
		}

		private string _first_name;
		public string first_name
		{
			get
			{
                if(!string.IsNullOrEmpty(_first_name))
				    return _first_name.Trim().ToUpper();
                return _first_name;
			}

			set
			{
				_first_name = value;
			}
		}

		private string _last_name;
		public string last_name
		{
			get
			{
                if (!String.IsNullOrEmpty(_last_name))
                    return _last_name.Trim().ToUpper();
				return _last_name;
			}

			set
			{
				_last_name = value;
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

		private int _plan_id;
		public int plan_id
		{
			get
			{
				return _plan_id;
			}

			set
			{
				_plan_id = value;
			}
		}

		private DateTime _effective_start_date;
		public DateTime effective_start_date
		{
			get
			{
				return _effective_start_date;
			}

			set
			{
				_effective_start_date = value;
			}
		}

		private DateTime _effective_end_date;
		public DateTime effective_end_date
		{
			get
			{
				return _effective_end_date;
			}

			set
			{
				_effective_end_date = value;
			}
		}

		private decimal _contribution_amount;
		public decimal contribution_amount
		{
			get
			{
				return _contribution_amount;
			}

			set
			{
				_contribution_amount = value;
			}
		}

		private int _batch_request_id;
		public int batch_request_id
		{
			get
			{
				return _batch_request_id;
			}

			set
			{
				_batch_request_id = value;
			}
		}
        public string report_frequency_value { get; set; } //PIR 24781

        public decimal er_pretax_match { get; set; }    //PIR 25920 New Plan DC 2025
    }
}

