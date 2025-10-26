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
    public class doCaseFileDetail : doBase
    {
         
         public doCaseFileDetail() : base()
         {
         }
		public int case_file_detail_id { get; set; }
		
		public int case_id { get; set; }
		
		public string document_id { get; set; }
		
		public string document_description { get; set; }
		
		public string document_title { get; set; }
		
		public string document_code { get; set; }
		
		public string version_series_id { get; set; }
		
		public DateTime filenet_received_date { get; set; }
		
		public string is_file_visible_flag { get; set; }
		
    }
}

