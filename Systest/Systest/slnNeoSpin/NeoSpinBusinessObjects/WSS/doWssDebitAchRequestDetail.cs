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
	/// Class NeoSpin.DataObjects.doWssDebitAchRequestDetail:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doWssDebitAchRequestDetail : doBase
    {
         
         public doWssDebitAchRequestDetail() : base()
         {
         }
         public int debit_ach_request_dtl_id { get; set; }
         public int debit_ach_request_id { get; set; }
         public int deposit_id { get; set; }
    }
    [Serializable]
    public enum enmWssDebitAchRequestDetail
    {
         debit_ach_request_dtl_id ,
         debit_ach_request_id ,
         deposit_id ,
    }
}

