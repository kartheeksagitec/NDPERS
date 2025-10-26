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
    public class doMergeEmployerDetail : doBase
    {
         
         public doMergeEmployerDetail() : base()
         {
         }
		private int _merge_employer_detail_id;
		public int merge_employer_detail_id
		{
			get
			{
				return _merge_employer_detail_id;
			}

			set
			{
				_merge_employer_detail_id = value;
			}
		}

		private int _merge_employer_header_id;
		public int merge_employer_header_id
		{
			get
			{
				return _merge_employer_header_id;
			}

			set
			{
				_merge_employer_header_id = value;
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

    }
}

