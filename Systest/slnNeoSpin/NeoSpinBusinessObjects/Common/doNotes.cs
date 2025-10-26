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
    public class doNotes : doBase
    {
         
         public doNotes() : base()
         {
         }
		private int _note_id;
		public int note_id
		{
			get
			{
				return _note_id;
			}

			set
			{
				_note_id = value;
			}
		}

		private int _person_id;
		public int person_id
		{
			get
			{
				return _person_id;
			}

			set
			{
				_person_id = value;
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

		private int _subsystem_id;
		public int subsystem_id
		{
			get
			{
				return _subsystem_id;
			}

			set
			{
				_subsystem_id = value;
			}
		}

		private string _subsystem_description;
		public string subsystem_description
		{
			get
			{
				return _subsystem_description;
			}

			set
			{
				_subsystem_description = value;
			}
		}

		private string _subsystem_value;
		public string subsystem_value
		{
			get
			{
				return _subsystem_value;
			}

			set
			{
				_subsystem_value = value;
			}
		}

		private int _subsystem_ref_id;
		public int subsystem_ref_id
		{
			get
			{
				return _subsystem_ref_id;
			}

			set
			{
				_subsystem_ref_id = value;
			}
		}

		private string _notes;
		public string notes
		{
			get
			{
				return _notes;
			}

			set
			{
				_notes = value;
			}
		}

		private string _specific_flag;
		public string specific_flag
		{
			get
			{
				return _specific_flag;
			}

			set
			{
				_specific_flag = value;
			}
		}

    }
}

