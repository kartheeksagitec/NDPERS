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
    public class doPirHistory : doBase
    {
         
         public doPirHistory() : base()
         {
         }
		private int _pir_history_id;
		public int pir_history_id
		{
			get
			{
				return _pir_history_id;
			}

			set
			{
				_pir_history_id = value;
			}
		}

		private int _pir_id;
		public int pir_id
		{
			get
			{
				return _pir_id;
			}

			set
			{
				_pir_id = value;
			}
		}

		private string _long_description;
		public string long_description
		{
			get
			{
				return _long_description;
			}

			set
			{
				_long_description = value;
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

		private int _assigned_to_id;
		public int assigned_to_id
		{
			get
			{
				return _assigned_to_id;
			}

			set
			{
				_assigned_to_id = value;
			}
		}

    }
}

