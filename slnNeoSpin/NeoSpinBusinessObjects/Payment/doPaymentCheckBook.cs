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
	/// Class NeoSpin.DataObjects.doPaymentCheckBook:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPaymentCheckBook : doBase
    {
        
         public doPaymentCheckBook() : base()
         {
         }
         public int check_book_id { get; set; }
         public DateTime effective_date { get; set; }
         public string first_check_number { get; set; }
         public string max_check_number { get; set; }
         public string last_check_number { get; set; }
    }
    [Serializable]
    public enum enmPaymentCheckBook
    {
         check_book_id ,
         effective_date ,
         first_check_number ,
         max_check_number ,
         last_check_number ,
    }
}

