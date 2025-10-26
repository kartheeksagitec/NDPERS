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
    public class doBenefitApplicationDbTffrTransfer : doBase
    {
         public doBenefitApplicationDbTffrTransfer() : base()
         {
         }
		private int _benefit_application_db_tffr_transfer_id;
		public int benefit_application_db_tffr_transfer_id
		{
			get
			{
				return _benefit_application_db_tffr_transfer_id;
			}

			set
			{
				_benefit_application_db_tffr_transfer_id = value;
			}
		}

		private int _benefit_application_id;
		public int benefit_application_id
		{
			get
			{
				return _benefit_application_id;
			}

			set
			{
				_benefit_application_id = value;
			}
		}

		private DateTime _start_date;
		public DateTime start_date
		{
			get
			{
				return _start_date;
			}

			set
			{
				_start_date = value;
			}
		}

		private DateTime _end_date;
		public DateTime end_date
		{
			get
			{
				return _end_date;
			}

			set
			{
				_end_date = value;
			}
		}

    }
}

