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
	/// Class NeoSpin.DataObjects.doWssDebitAchRequest:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doWssDebitAchRequest : doBase
    {
         
         public doWssDebitAchRequest() : base()
         {
         }
         public int debit_ach_request_id { get; set; }
         public int employer_payroll_header_id { get; set; }
         public int org_bank_id { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
    }
    [Serializable]
    public enum enmWssDebitAchRequest
    {
         debit_ach_request_id ,
         employer_payroll_header_id ,
         org_bank_id ,
         status_id ,
         status_description ,
         status_value ,
    }
}

