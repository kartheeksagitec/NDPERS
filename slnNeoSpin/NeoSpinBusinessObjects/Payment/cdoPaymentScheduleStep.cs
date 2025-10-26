#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using NeoSpin.DataObjects;

#endregion

namespace NeoSpin.CustomDataObjects
{
    [Serializable]
	public class cdoPaymentScheduleStep : doPaymentScheduleStep
	{
		public cdoPaymentScheduleStep() : base()
		{
		}
        public int run_sequence { get; set; }
        public int batch_schedule_id { get; set; }
    } 
} 
