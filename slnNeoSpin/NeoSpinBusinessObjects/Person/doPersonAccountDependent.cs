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
	/// Class NeoSpin.DataObjects.doPersonAccountDependent:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPersonAccountDependent : doBase
    {
         
         public doPersonAccountDependent() : base()
         {
         }
         public int person_account_dependent_id { get; set; }
         public int person_dependent_id { get; set; }
         public int person_account_id { get; set; }
         public DateTime start_date { get; set; }
         public DateTime end_date { get; set; }
         public string nmso_co_flag { get; set; }
         public string out_area_flag { get; set; }
         public string included_to_hmo_file_flag { get; set; }
         public string is_bcbs_file_sent_flag { get; set; }
         public string is_modified_after_bcbs_file_sent_flag { get; set; }
         public string is_drop_dependent_letter_sent_flag { get; set; }
    }
    [Serializable]
    public enum enmPersonAccountDependent
    {
         person_account_dependent_id ,
         person_dependent_id ,
         person_account_id ,
         start_date ,
         end_date ,
         nmso_co_flag ,
         out_area_flag ,
         included_to_hmo_file_flag ,
         is_bcbs_file_sent_flag ,
         is_modified_after_bcbs_file_sent_flag ,
         is_drop_dependent_letter_sent_flag ,
    }
}

