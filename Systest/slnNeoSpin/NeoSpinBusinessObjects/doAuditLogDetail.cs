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
    public class doAuditLogDetail : doBase
    {
         
         public doAuditLogDetail() : base()
         {
         }
		private int _audit_log_detail_id;
		public int audit_log_detail_id
		{
			get
			{
				return _audit_log_detail_id;
			}

			set
			{
				_audit_log_detail_id = value;
			}
		}

		private int _audit_log_id;
		public int audit_log_id
		{
			get
			{
				return _audit_log_id;
			}

			set
			{
				_audit_log_id = value;
			}
		}

		private string _column_name;
		public string column_name
		{
			get
			{
				return _column_name;
			}

			set
			{
				_column_name = value;
			}
		}

		private string _old_value;
		public string old_value
		{
			get
			{
				return _old_value;
			}

			set
			{
				_old_value = value;
			}
		}

		private string _new_value;
		public string new_value
		{
			get
			{
				return _new_value;
			}

			set
			{
				_new_value = value;
			}
		}

    }
}

