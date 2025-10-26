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
    public class doCycleHistory : doBase
    {
         public doCycleHistory() : base()
         {
         }
		private int _cycle_no;
		public int cycle_no
		{
			get
			{
				return _cycle_no;
			}

			set
			{
				_cycle_no = value;
			}
		}

		private DateTime _start_time;
		public DateTime start_time
		{
			get
			{
				return _start_time;
			}

			set
			{
				_start_time = value;
			}
		}

		private DateTime _end_time;
		public DateTime end_time
		{
			get
			{
				return _end_time;
			}

			set
			{
				_end_time = value;
			}
		}

    }
}

