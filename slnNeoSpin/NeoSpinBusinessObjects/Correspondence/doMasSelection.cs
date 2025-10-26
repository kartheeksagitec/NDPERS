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
	/// Class NeoSpin.DataObjects.doMasSelection:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doMasSelection : doBase
    {
         
         public doMasSelection() : base()
         {
         }
         public int mas_selection_id { get; set; }
         public int mas_batch_request_id { get; set; }
         public int person_id { get; set; }
         public int payee_org_id { get; set; }
         public string is_data_pulled_flag { get; set; }
         public string is_report_created_flag { get; set; }
    }
    [Serializable]
    public enum enmMasSelection
    {
         mas_selection_id ,
         mas_batch_request_id ,
         person_id ,
         payee_org_id ,
         is_data_pulled_flag ,
         is_report_created_flag ,
    }
}

