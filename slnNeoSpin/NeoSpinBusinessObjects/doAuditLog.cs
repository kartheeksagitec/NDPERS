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
	/// Class NeoSpin.DataObjects.doAuditLog:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doAuditLog : doBase
    {
         
         public doAuditLog() : base()
         {
         }
         public int audit_log_id { get; set; }
         public string form_name { get; set; }
         public string table_name { get; set; }
         public long primary_key { get; set; }
         public int person_id { get; set; }
         public int org_id { get; set; }
         public int org_plan_id { get; set; }
         public string change_type { get; set; }
         public string client_ip_address { get; set; }
         public long bo_primary_key { get; set; }
         public Guid guid_transaction_id { get; set; }
    }
    [Serializable]
    public enum enmAuditLog
    {
         audit_log_id ,
         form_name ,
         table_name ,
         primary_key ,
         person_id ,
         org_id ,
         org_plan_id ,
         change_type ,
         client_ip_address ,
         bo_primary_key ,
         guid_transaction_id ,
    }
}

