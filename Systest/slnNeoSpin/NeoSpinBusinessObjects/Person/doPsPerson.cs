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
	/// Class NeoSpin.DataObjects.doPsPerson:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPsPerson : doBase
    {
         
         public doPsPerson() : base()
         {
         }
         public int ps_person_id { get; set; }
         public string peoplesoft_id { get; set; }
         public string name_prefix_value { get; set; }
         public string first_name { get; set; }
         public string middle_name { get; set; }
         public string last_name { get; set; }
         public string name_suffix_value { get; set; }
         public DateTime date_of_birth { get; set; }
         public string gender_value { get; set; }
         public string ssn { get; set; }
         public string marital_status_value { get; set; }
         public DateTime ms_change_date { get; set; }
         public string work_phone_no { get; set; }
         public string work_phone_ext { get; set; }
         public string cell_phone_no { get; set; }
         public string home_phone_no { get; set; }
         public string processed_flag { get; set; }
         public string error { get; set; }
    }
    [Serializable]
    public enum enmPsPerson
    {
         ps_person_id ,
         peoplesoft_id ,
         name_prefix_value ,
         first_name ,
         middle_name ,
         last_name ,
         name_suffix_value ,
         date_of_birth ,
         gender_value ,
         ssn ,
         marital_status_value ,
         ms_change_date ,
         work_phone_no ,
         work_phone_ext ,
         cell_phone_no ,
         home_phone_no ,
         processed_flag ,
         error ,
    }
}

