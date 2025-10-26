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
    public class doTestcaseDetail : doBase
    {
         
         public doTestcaseDetail() : base()
         {
         }
		private int _testcase_dtl_id;
		public int testcase_dtl_id
		{
			get
			{
				return _testcase_dtl_id;
			}

			set
			{
				_testcase_dtl_id = value;
			}
		}

		private int _testcase_id;
		public int testcase_id
		{
			get
			{
				return _testcase_id;
			}

			set
			{
				_testcase_id = value;
			}
		}

		private string _testcase_dtl_key;
		public string testcase_dtl_key
		{
			get
			{
				return _testcase_dtl_key;
			}

			set
			{
				_testcase_dtl_key = value;
			}
		}

		private string _testcase_dtl_desc;
		public string testcase_dtl_desc
		{
			get
			{
				return _testcase_dtl_desc;
			}

			set
			{
				_testcase_dtl_desc = value;
			}
		}

		private string _notes;
		public string notes
		{
			get
			{
				return _notes;
			}

			set
			{
				_notes = value;
			}
		}

    }
}

