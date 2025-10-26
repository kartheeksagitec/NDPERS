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
    public class doOrgContact : doBase
    {
         
         public doOrgContact() : base()
         {
         }
		private int _org_contact_id;
		public int org_contact_id
		{
			get
			{
				return _org_contact_id;
			}

			set
			{
				_org_contact_id = value;
			}
		}

		private int _org_id;
		public int org_id
		{
			get
			{
				return _org_id;
			}

			set
			{
				_org_id = value;
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

		private int _contact_id;
		public int contact_id
		{
			get
			{
				return _contact_id;
			}

			set
			{
				_contact_id = value;
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

		private int _primary_address_id;
		public int primary_address_id
		{
			get
			{
				return _primary_address_id;
			}

			set
			{
				_primary_address_id = value;
			}
		}

    }
}

