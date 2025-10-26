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
	/// Class NeoSpin.DataObjects.do1099rPaymentHistoryLink:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class do1099rPaymentHistoryLink : doBase
    {
         
         public do1099rPaymentHistoryLink() : base()
         {
         }
         public int payment_1099r_history_link_id { get; set; }
         public int payment_1099r_id { get; set; }
         public int payment_history_header_id { get; set; }
         public int benefit_overpayment_id { get; set; }
    }
    [Serializable]
    public enum enm1099rPaymentHistoryLink
    {
         payment_1099r_history_link_id ,
         payment_1099r_id ,
         payment_history_header_id ,
         benefit_overpayment_id ,
    }
}

