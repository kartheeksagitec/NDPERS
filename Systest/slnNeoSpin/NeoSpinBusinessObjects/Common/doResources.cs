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
	public class doResources : doBase
	{
		public doResources() : base()
      {
      }
      private int _resource_id;
      public int resource_id
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
      private int _resource_type_id;
      public int resource_type_id
      {
	        get
	        {
		    return _resource_type_id;
	        }

	        set
	        {
		        _resource_type_id = value;
	        }
      }
      private string _resource_type_description;
      public string resource_type_description
      {
	        get
	        {
		    return _resource_type_description;
	        }

	        set
	        {
		        _resource_type_description = value;
	        }
      }
      private string _resource_type_value;
      public string resource_type_value
      {
	        get
	        {
		    return _resource_type_value;
	        }

	        set
	        {
		        _resource_type_value = value;
	        }
      }
      private string _resource_description;
      public string resource_description
      {
	        get
	        {
		    return _resource_description;
	        }

	        set
	        {
		        _resource_description = value;
	        }
      }
  }
}
