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
    public class doMailingLabelPersonType : doBase
    {
         
         public doMailingLabelPersonType() : base()
         {
         }
		private int _mailing_label_person_type_id;
		public int mailing_label_person_type_id
		{
			get
			{
				return _mailing_label_person_type_id;
			}

			set
			{
				_mailing_label_person_type_id = value;
			}
		}

		private int _mailing_label_batch_id;
		public int mailing_label_batch_id
		{
			get
			{
				return _mailing_label_batch_id;
			}

			set
			{
				_mailing_label_batch_id = value;
			}
		}

		private int _person_type_id;
		public int person_type_id
		{
			get
			{
				return _person_type_id;
			}

			set
			{
				_person_type_id = value;
			}
		}

		private string _person_type_description;
		public string person_type_description
		{
			get
			{
				return _person_type_description;
			}

			set
			{
				_person_type_description = value;
			}
		}

		private string _person_type_value;
		public string person_type_value
		{
			get
			{
				return _person_type_value;
			}

			set
			{
				_person_type_value = value;
			}
		}

    }
}

