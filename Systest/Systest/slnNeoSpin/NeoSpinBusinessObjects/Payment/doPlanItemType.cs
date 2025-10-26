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
    public class doPlanItemType : doBase
    {
         
         public doPlanItemType() : base()
         {
         }
		private int _plan_item_type_id;
		public int plan_item_type_id
		{
			get
			{
				return _plan_item_type_id;
			}

			set
			{
				_plan_item_type_id = value;
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

		private int _payment_item_type_id;
		public int payment_item_type_id
		{
			get
			{
				return _payment_item_type_id;
			}

			set
			{
				_payment_item_type_id = value;
			}
		}

    }
}

