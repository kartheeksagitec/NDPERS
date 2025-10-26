#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using Sagitec.Common;
using Sagitec.DataObjects;
#endregion

namespace NeoSpin.DataObjects
{
    [Serializable]
	public class doSecurity : doBase
	{
		public doSecurity() : base()
		{
		}
		private Int32 _role_id;
		public Int32 role_id
		{
			get
			{
				return _role_id;
			}

			set
			{
				_role_id = value;
			}
		}

		private Int32 _resource_id;
		public Int32 resource_id
		{
			get
			{
				return _resource_id;
			}

			set
			{
				_resource_id = value;
			}
		}

		private Int32 _security_id;
		public Int32 security_id
		{
			get
			{
				return _security_id;
			}

			set
			{
				_security_id = value;
			}
		}

		private string _security_description;
		public string security_description
		{
			get
			{
				return _security_description;
			}

			set
			{
				_security_description = value;
			}
		}

		private Int32 _security_value;
		public Int32 security_value
		{
			get
			{
				return _security_value;
			}

			set
			{
				_security_value = value;
			}
		}

	}
}

