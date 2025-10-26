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
    public class doTaxUpdateBatchSchedule : doBase
    {
         
         public doTaxUpdateBatchSchedule() : base()
         {
         }
		private int _tax_update_batch_schedule_id;
		public int tax_update_batch_schedule_id
		{
			get
			{
				return _tax_update_batch_schedule_id;
			}

			set
			{
				_tax_update_batch_schedule_id = value;
			}
		}

		private int _tax_identifier_id;
		public int tax_identifier_id
		{
			get
			{
				return _tax_identifier_id;
			}

			set
			{
				_tax_identifier_id = value;
			}
		}

		private string _tax_identifier_description;
		public string tax_identifier_description
		{
			get
			{
				return _tax_identifier_description;
			}

			set
			{
				_tax_identifier_description = value;
			}
		}

		private string _tax_identifier_value;
		public string tax_identifier_value
		{
			get
			{
				return _tax_identifier_value;
			}

			set
			{
				_tax_identifier_value = value;
			}
		}

		private string _execute_batch_flag;
		public string execute_batch_flag
		{
			get
			{
				return _execute_batch_flag;
			}

			set
			{
				_execute_batch_flag = value;
			}
		}

		private DateTime _batch_run_completed_date;
		public DateTime batch_run_completed_date
		{
			get
			{
				return _batch_run_completed_date;
			}

			set
			{
				_batch_run_completed_date = value;
			}
		}

    }
}

