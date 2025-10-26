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
	/// Class NeoSpin.DataObjects.doWssBenAppDisaEducation:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doWssBenAppDisaEducation : doBase
    {
         
         public doWssBenAppDisaEducation() : base()
         {
         }
         public int disa_education_id { get; set; }
         public int wss_ben_app_id { get; set; }
         public string last_year_completed { get; set; }
         public string name_of_school { get; set; }
         public string last_year_in_school { get; set; }
         public string degree_certificate { get; set; }
         public string additional_training { get; set; }
         public string attitude_towards_school { get; set; }
         public string favourable_courses { get; set; }
    }
    [Serializable]
    public enum enmWssBenAppDisaEducation
    {
         disa_education_id ,
         wss_ben_app_id ,
         last_year_completed ,
         name_of_school ,
         last_year_in_school ,
         degree_certificate ,
         additional_training ,
         attitude_towards_school ,
         favourable_courses ,
    }
}

