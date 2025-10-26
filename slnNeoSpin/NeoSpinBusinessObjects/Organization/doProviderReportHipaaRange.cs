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
    public class doProviderReportHipaaRange : doBase
    {
         
         public doProviderReportHipaaRange() : base()
         {
         }
		private int _provider_report_hipaa_range_id;
		public int provider_report_hipaa_range_id
		{
			get
			{
				return _provider_report_hipaa_range_id;
			}

			set
			{
				_provider_report_hipaa_range_id = value;
			}
		}

		private string _provider_org_code_id;
		public string provider_org_code_id
		{
			get
			{
				return _provider_org_code_id;
			}

			set
			{
				_provider_org_code_id = value;
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

		private int _last_sequence;
		public int last_sequence
		{
			get
			{
				return _last_sequence;
			}

			set
			{
				_last_sequence = value;
			}
		}

    }
}

