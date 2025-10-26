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
    public class doItemTypeSourceTypeCrossref : doBase
    {
         public doItemTypeSourceTypeCrossref() : base()
         {
         }
		private int _item_type_source_type_crossref_id;
		public int item_type_source_type_crossref_id
		{
			get
			{
				return _item_type_source_type_crossref_id;
			}

			set
			{
				_item_type_source_type_crossref_id = value;
			}
		}

		private int _item_type_id;
		public int item_type_id
		{
			get
			{
				return _item_type_id;
			}

			set
			{
				_item_type_id = value;
			}
		}

		private string _item_type_description;
		public string item_type_description
		{
			get
			{
				return _item_type_description;
			}

			set
			{
				_item_type_description = value;
			}
		}

		private string _item_type_value;
		public string item_type_value
		{
			get
			{
				return _item_type_value;
			}

			set
			{
				_item_type_value = value;
			}
		}

		private int _source_type_id;
		public int source_type_id
		{
			get
			{
				return _source_type_id;
			}

			set
			{
				_source_type_id = value;
			}
		}

		private string _source_type_description;
		public string source_type_description
		{
			get
			{
				return _source_type_description;
			}

			set
			{
				_source_type_description = value;
			}
		}

		private string _source_type_value;
		public string source_type_value
		{
			get
			{
				return _source_type_value;
			}

			set
			{
				_source_type_value = value;
			}
		}

    }
}

