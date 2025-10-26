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
	/// Class NeoSpin.DataObjects.doPersonEmployment:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPersonEmployment : doBase
    {
         
         public doPersonEmployment() : base()
         {
         }
         public int person_employment_id { get; set; }
         public int person_id { get; set; }
         public DateTime start_date { get; set; }
         public DateTime end_date { get; set; }
         public int org_id { get; set; }
         public int unused_sick_leave { get; set; }
         public int termination_letter_status_id { get; set; }
         public string termination_letter_status_description { get; set; }
         public string termination_letter_status_value { get; set; }
         public string suppress_ssn_flag { get; set; }
         public string ps_empl_record_number { get; set; }
         public string voya_sent { get; set; }
         public DateTime date_of_last_regular_paycheck { get; set; }
    }
    [Serializable]
    public enum enmPersonEmployment
    {
         person_employment_id ,
         person_id ,
         start_date ,
         end_date ,
         org_id ,
         unused_sick_leave ,
         termination_letter_status_id ,
         termination_letter_status_description ,
         termination_letter_status_value ,
         suppress_ssn_flag ,
         ps_empl_record_number ,
         voya_sent ,
         date_of_last_regular_paycheck ,
    }
}

