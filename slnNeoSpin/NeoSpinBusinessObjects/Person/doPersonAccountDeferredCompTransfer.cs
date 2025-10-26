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
	/// Class NeoSpin.DataObjects.doPersonAccountDeferredCompTransfer:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPersonAccountDeferredCompTransfer : doBase
    {
         
         public doPersonAccountDeferredCompTransfer() : base()
         {
         }
         public int person_account_deferred_comp_transfer_id { get; set; }
         public DateTime transfer_date { get; set; }
         public int from_person_account_id { get; set; }
         public int to_person_account_id { get; set; }
         public string comment { get; set; }
         public int transfer_type_id { get; set; }
         public string transfer_type_description { get; set; }
         public string transfer_type_value { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public DateTime posted_date { get; set; }
         public string posted_by { get; set; }
    }
    [Serializable]
    public enum enmPersonAccountDeferredCompTransfer
    {
         person_account_deferred_comp_transfer_id ,
         transfer_date ,
         from_person_account_id ,
         to_person_account_id ,
         comment ,
         transfer_type_id ,
         transfer_type_description ,
         transfer_type_value ,
         status_id ,
         status_description ,
         status_value ,
         posted_date ,
         posted_by ,
    }
}

