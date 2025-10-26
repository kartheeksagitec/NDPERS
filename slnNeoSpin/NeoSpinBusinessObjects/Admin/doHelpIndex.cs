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
    public class doHelpIndex : doBase
    {
         public doHelpIndex() : base()
         {
         }
		private int _help_index_id;
		public int help_index_id
		{
			get
			{
				return _help_index_id;
			}

			set
			{
				_help_index_id = value;
			}
		}

		private string _help_topic;
		public string help_topic
		{
			get
			{
				return _help_topic;
			}

			set
			{
				_help_topic = value;
			}
		}

		private string _help_group;
		public string help_group
		{
			get
			{
				return _help_group;
			}

			set
			{
				_help_group = value;
			}
		}

		private string _help_link_file;
		public string help_link_file
		{
			get
			{
				return _help_link_file;
			}

			set
			{
				_help_link_file = value;
			}
		}

    }
}

