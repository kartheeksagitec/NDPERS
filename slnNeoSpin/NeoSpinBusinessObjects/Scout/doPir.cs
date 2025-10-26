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
	/// <summary>
	/// Class NeoSpin.DataObjects.doPir:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPir : doBase
    {
         
         public doPir() : base()
         {
         }
         public int pir_id { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public string pir_description { get; set; }
         public int priority_id { get; set; }
         public string priority_description { get; set; }
         public string priority_value { get; set; }
         public int severity_id { get; set; }
         public string severity_description { get; set; }
         public string severity_value { get; set; }
         public int reported_by_id { get; set; }
         public int assigned_to_id { get; set; }
         public string screen_affected { get; set; }
         public string otherui_objects { get; set; }
         public int testcase_scenario { get; set; }
         public string release_info { get; set; }
         public string update_doc_flag { get; set; }
         public int critical_pir_priority { get; set; }
         public DateTime priority_date { get; set; }
         public int pir_type_id { get; set; }
         public string pir_type_description { get; set; }
         public string pir_type_value { get; set; }
        public int referent_id { get; set; }
    }
    [Serializable]
    public enum enmPir
    {
         pir_id ,
         status_id ,
         status_description ,
         status_value ,
         pir_description ,
         priority_id ,
         priority_description ,
         priority_value ,
         severity_id ,
         severity_description ,
         severity_value ,
         reported_by_id ,
         assigned_to_id ,
         screen_affected ,
         otherui_objects ,
         testcase_scenario ,
         release_info ,
         update_doc_flag ,
         critical_pir_priority ,
         priority_date ,
         pir_type_id ,
         pir_type_description ,
         pir_type_value ,
         referent_id ,
    }
}

