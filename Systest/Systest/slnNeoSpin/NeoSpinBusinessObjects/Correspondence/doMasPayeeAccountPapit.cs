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
	/// Class NeoSpin.DataObjects.doMasPayeeAccountPapit:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doMasPayeeAccountPapit : doBase
    {
        
         public doMasPayeeAccountPapit() : base()
         {
         }
         public int mas_payee_account_papit_id { get; set; }
         public int mas_payee_account_id { get; set; }
         public int payment_item_type_id { get; set; }
         public decimal amount { get; set; }
    }
    [Serializable]
    public enum enmMasPayeeAccountPapit
    {
         mas_payee_account_papit_id ,
         mas_payee_account_id ,
         payment_item_type_id ,
         amount ,
    }
}

