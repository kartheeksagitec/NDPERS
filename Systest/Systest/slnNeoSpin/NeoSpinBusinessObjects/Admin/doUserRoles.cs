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
    public class doUserRoles : doBase
    {
         public doUserRoles() : base()
         {
         }
		private int _user_serial_id;
		public int user_serial_id
		{
			get
			{
				return _user_serial_id;
			}

			set
			{
				_user_serial_id = value;
			}
		}

		private int _role_id;
		public int role_id
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

		private DateTime _effective_start_date;
		public DateTime effective_start_date
		{
			get
			{
				return _effective_start_date;
			}

			set
			{
				_effective_start_date = value;
			}
		}

		private DateTime _effective_end_date;
		public DateTime effective_end_date
		{
			get
			{
				return _effective_end_date;
			}

			set
			{
				_effective_end_date = value;
			}
		}

    }
}

