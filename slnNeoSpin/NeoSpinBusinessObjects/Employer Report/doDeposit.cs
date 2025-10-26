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
	/// Class NeoSpin.DataObjects.doDeposit:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doDeposit : doBase
    {
         
         public doDeposit() : base()
         {
         }
         public int deposit_id { get; set; }
         public int deposit_tape_id { get; set; }
         public int person_id { get; set; }
         public int org_id { get; set; }
         public string reference_no { get; set; }
         public DateTime payment_date { get; set; }
         public decimal deposit_amount { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public string ach_retrieve_flag { get; set; }
         public int deposit_source_id { get; set; }
         public string deposit_source_description { get; set; }
         public string deposit_source_value { get; set; }
         public int payment_history_header_id { get; set; }
         public int employer_payroll_header_id { get; set; }
         public int contact_ticket_id { get; set; }
         public string notes { get; set; }
         public DateTime deposit_date { get; set; }
    }
    [Serializable]
    public enum enmDeposit
    {
         deposit_id ,
         deposit_tape_id ,
         person_id ,
         org_id ,
         reference_no ,
         payment_date ,
         deposit_amount ,
         status_id ,
         status_description ,
         status_value ,
         ach_retrieve_flag ,
         deposit_source_id ,
         deposit_source_description ,
         deposit_source_value ,
         payment_history_header_id ,
         employer_payroll_header_id ,
         contact_ticket_id ,
         notes ,
         deposit_date ,
    }
}

