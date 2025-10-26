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
	/// Class NeoSpin.DataObjects.doWssPersonEmployment:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doWssPersonEmployment : doBase
    {
         
         public doWssPersonEmployment() : base()
         {
         }
         public int wss_person_employment_id { get; set; }
         public int member_record_request_id { get; set; }
         public int person_employment_id { get; set; }
         public DateTime start_date { get; set; }
         public int org_id { get; set; }
         public string ps_empl_record_number { get; set; }
         public DateTime end_date { get; set; }
    }
    [Serializable]
    public enum enmWssPersonEmployment
    {
         wss_person_employment_id ,
         member_record_request_id ,
         person_employment_id ,
         start_date ,
         org_id ,
         ps_empl_record_number ,
         end_date ,
    }
}

