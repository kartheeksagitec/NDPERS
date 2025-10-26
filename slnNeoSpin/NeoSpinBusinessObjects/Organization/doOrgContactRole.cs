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
    public class doOrgContactRole : doBase
    {
         
         public doOrgContactRole() : base()
         {
         }
		private int _org_contact_role_id;
		public int org_contact_role_id
		{
			get
			{
				return _org_contact_role_id;
			}

			set
			{
				_org_contact_role_id = value;
			}
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

		private int _contact_role_id;
		public int contact_role_id
		{
			get
			{
				return _contact_role_id;
			}

			set
			{
				_contact_role_id = value;
			}
		}

		private string _contact_role_description;
		public string contact_role_description
		{
			get
			{
				return _contact_role_description;
			}

			set
			{
				_contact_role_description = value;
			}
		}

		private string _contact_role_value;
		public string contact_role_value
		{
			get
			{
				return _contact_role_value;
			}

			set
			{
				_contact_role_value = value;
			}
		}

    }
}

