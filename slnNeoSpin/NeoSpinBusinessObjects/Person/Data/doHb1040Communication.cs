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
	/// Class NeoSpin.DataObjects.doHb1040Communication:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doHb1040Communication : doBase
    {
         public doHb1040Communication() : base()
         {
         }
         public int hb1040_communication_id { get; set; }
         public int person_id { get; set; }
         public int person_account_id { get; set; }
         public DateTime nrd { get; set; }
         public decimal age_at_nrd { get; set; }
         public decimal fas { get; set; }
         public decimal accrued_benefit_amt { get; set; }
         public decimal db_account_balance { get; set; }
         public decimal db_dc_transfer_amt { get; set; }
         public DateTime member_dob { get; set; }
         public decimal jan_25_DB_Acct_Bal { get; set; }
        public string letter_generated_1 { get; set; }
        public DateTime letter_generated_date_1 { get; set; }
        public string letter_generated_2 { get; set; }
        public DateTime letter_generated_date_2 { get; set; }
    }
    [Serializable]
    public enum enmHb1040Communication
    {
         hb1040_communication_id ,
         person_id ,
         person_account_id ,
         nrd ,
         age_at_nrd ,
         fas ,
         accrued_benefit_amt ,
         db_account_balance ,
         db_dc_transfer_amt ,
         member_dob,
         letter_generated_1,
         letter_generated_date_1,
        letter_generated_2,
        letter_generated_date_2,

    }
}
