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
    public class doSerPurRolloverInfo : doBase
    {
         
         public doSerPurRolloverInfo() : base()
         {
         }
		private int _ser_pur_rollover_id;
		public int ser_pur_rollover_id
		{
			get
			{
				return _ser_pur_rollover_id;
			}

			set
			{
				_ser_pur_rollover_id = value;
			}
		}

		private int _service_purchase_id;
		public int service_purchase_id
		{
			get
			{
				return _service_purchase_id;
			}

			set
			{
				_service_purchase_id = value;
			}
		}

		private int _rollover_id;
		public int rollover_id
		{
			get
			{
				return _rollover_id;
			}

			set
			{
				_rollover_id = value;
			}
		}

		private string _rollover_description;
		public string rollover_description
		{
			get
			{
				return _rollover_description;
			}

			set
			{
				_rollover_description = value;
			}
		}

		private string _rollover_value;
		public string rollover_value
		{
			get
			{
				return _rollover_value;
			}

			set
			{
				_rollover_value = value;
			}
		}

    }
}

