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
    public class doSerPurServiceType : doBase
    {
         
         public doSerPurServiceType() : base()
         {
         }
		private int _ser_pur_service_type_id;
		public int ser_pur_service_type_id
		{
			get
			{
				return _ser_pur_service_type_id;
			}

			set
			{
				_ser_pur_service_type_id = value;
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

		private int _service_type_id;
		public int service_type_id
		{
			get
			{
				return _service_type_id;
			}

			set
			{
				_service_type_id = value;
			}
		}

		private string _service_type_description;
		public string service_type_description
		{
			get
			{
				return _service_type_description;
			}

			set
			{
				_service_type_description = value;
			}
		}

		private string _service_type_value;
		public string service_type_value
		{
			get
			{
				return _service_type_value;
			}

			set
			{
				_service_type_value = value;
			}
		}

		private string _service_type_checked_flag;
		public string service_type_checked_flag
		{
			get
			{
				return _service_type_checked_flag;
			}

			set
			{
				_service_type_checked_flag = value;
			}
		}

		private DateTime _service_type_from_date;
		public DateTime service_type_from_date
		{
			get
			{
				return _service_type_from_date;
			}

			set
			{
				_service_type_from_date = value;
			}
		}

		private DateTime _service_type_to_date;
		public DateTime service_type_to_date
		{
			get
			{
				return _service_type_to_date;
			}

			set
			{
				_service_type_to_date = value;
			}
		}

    }
}

