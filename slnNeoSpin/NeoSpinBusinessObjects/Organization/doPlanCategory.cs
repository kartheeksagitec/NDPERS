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
    public class doPlanCategory : doBase
    {
         
         public doPlanCategory() : base()
         {
         }
		private int _plan_category_id;
		public int plan_category_id
		{
			get
			{
				return _plan_category_id;
			}

			set
			{
				_plan_category_id = value;
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

    }
}

