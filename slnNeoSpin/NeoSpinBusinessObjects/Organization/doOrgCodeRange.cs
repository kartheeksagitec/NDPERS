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
    public class doOrgCodeRange : doBase
    {
         
         public doOrgCodeRange() : base()
         {
         }
		private int _org_code_range_id;
		public int org_code_range_id
		{
			get
			{
				return _org_code_range_id;
			}

			set
			{
				_org_code_range_id = value;
			}
		}

		private int _last_entered_org_code;
		public int last_entered_org_code
		{
			get
			{
				return _last_entered_org_code;
			}

			set
			{
				_last_entered_org_code = value;
			}
		}

		private int _org_code_max;
		public int org_code_max
		{
			get
			{
				return _org_code_max;
			}

			set
			{
				_org_code_max = value;
			}
		}

		private int _org_code_min;
		public int org_code_min
		{
			get
			{
				return _org_code_min;
			}

			set
			{
				_org_code_min = value;
			}
		}

    }
}

