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
	/// Class NeoSpin.DataObjects.doWssBenAppDisaWorkHistory:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doWssBenAppDisaWorkHistory : doBase
    {
         
         public doWssBenAppDisaWorkHistory() : base()
         {
         }
         public int disa_work_history_id { get; set; }
         public int wss_ben_app_id { get; set; }
         public string employer_name { get; set; }
         public string supervisor_name { get; set; }
         public string job_titles { get; set; }
         public DateTime employment_from_date { get; set; }
         public DateTime employment_to_date { get; set; }
         public decimal employment_salary { get; set; }
         public string employment_duties { get; set; }
    }
    [Serializable]
    public enum enmWssBenAppDisaWorkHistory
    {
         disa_work_history_id ,
         wss_ben_app_id ,
         employer_name ,
         supervisor_name ,
         job_titles ,
         employment_from_date ,
         employment_to_date ,
         employment_salary ,
         employment_duties ,
    }
}

