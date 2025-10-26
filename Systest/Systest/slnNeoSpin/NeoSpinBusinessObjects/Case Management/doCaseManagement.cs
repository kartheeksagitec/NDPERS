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
    public class doCaseManagement : doBase
    {
         
         public doCaseManagement() : base()
         {
         }
		public int case_management_id { get; set; }
		
		public int case_type_id { get; set; }
		
		public string case_type_description { get; set; }
		
		public string case_type_value { get; set; }
		
		public int appeal_type_id { get; set; }
		
		public string appeal_type_description { get; set; }
		
		public string appeal_type_value { get; set; }
		
		public int step_name_id { get; set; }
		
		public string step_name_description { get; set; }
		
		public string step_name_value { get; set; }
		
		public int number_of_days { get; set; }
		
		public DateTime start_date { get; set; }
		
		public DateTime end_date { get; set; }
		
		public int sort_order { get; set; }
		
    }
}

