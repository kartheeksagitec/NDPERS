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
    public class doMailingLabelPlan : doBase
    {
         
         public doMailingLabelPlan() : base()
         {
         }
		private int _mailing_label_plan_id;
		public int mailing_label_plan_id
		{
			get
			{
				return _mailing_label_plan_id;
			}

			set
			{
				_mailing_label_plan_id = value;
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

    }
}

