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
	/// Class NeoSpin.DataObjects.doCorPayeeAccount:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doCorPayeeAccount : doBase
    {
         public doCorPayeeAccount() : base()
         {
         }
         public int cor_payee_account_id { get; set; }
         public int death_notification_id { get; set; }
         public int payee_account_id { get; set; }
    }
    [Serializable]
    public enum enmCorPayeeAccount
    {
         cor_payee_account_id ,
         death_notification_id ,
         payee_account_id ,
    }
}
