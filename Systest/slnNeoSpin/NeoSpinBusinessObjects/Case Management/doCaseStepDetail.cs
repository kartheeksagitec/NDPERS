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
    public class doCaseStepDetail : doBase
    {
         public doCaseStepDetail() : base()
         {
         }
		public int case_step_detail_id { get; set; }
		
		public int case_id { get; set; }
		
		public int step_name_id { get; set; }
		
		public string step_name_description { get; set; }
		
		public string step_name_value { get; set; }
		
		public DateTime start_date { get; set; }
		
		public DateTime end_date { get; set; }
		
		public DateTime target_date { get; set; }
		
		public int number_of_days { get; set; }
		
		public string comments { get; set; }
		
		public int status_id { get; set; }
		
		public string status_description { get; set; }
		
		public string status_value { get; set; }
		
    }
}

