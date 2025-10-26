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
      public class doSystemPaths : doBase
      {
		public doSystemPaths() : base()
		{
		}
		private Int32 _path_id;
		public Int32 path_id
		{
			get
			{
				return _path_id;
			}

			set
			{
				_path_id = value;
			}
		}
		private string _path_code;
		public string path_code
		{
			get
			{
				return _path_code;
			}

			set
			{
				_path_code = value;
			}
		}
		private string _path_value;
		public string path_value
		{
			get
			{
				return _path_value;
			}

			set
			{
				_path_value = value;
			}
		}
		private string _path_description;
		public string path_description
		{
			get
			{
				return _path_description;
			}

			set
			{
				_path_description = value;
			}
		}
	}
}

