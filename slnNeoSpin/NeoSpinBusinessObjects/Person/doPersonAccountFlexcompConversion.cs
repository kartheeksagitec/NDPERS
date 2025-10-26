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
    public class doPersonAccountFlexcompConversion : doBase
    {
        
         public doPersonAccountFlexcompConversion() : base()
         {
         }
		private int _person_account_flex_comp_conversion_id;
		public int person_account_flex_comp_conversion_id
		{
			get
			{
				return _person_account_flex_comp_conversion_id;
			}

			set
			{
				_person_account_flex_comp_conversion_id = value;
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

		private int _org_id;
		public int org_id
		{
			get
			{
				return _org_id;
			}

			set
			{
				_org_id = value;
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


        private string _is_enrollment_report_generated;
        public string is_enrollment_report_generated
        {
            get
            {
                return _is_enrollment_report_generated;
            }

            set
            {
                _is_enrollment_report_generated = value;
            }
        }

    }
}

