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
	public class doRoles : doBase
	{
		public doRoles() : base()
      {
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
      private string _role_description;
      public string role_description
      {
	        get
	        {
		    return _role_description;
	        }

	        set
	        {
		        _role_description = value;
	        }
      }
  }
}
