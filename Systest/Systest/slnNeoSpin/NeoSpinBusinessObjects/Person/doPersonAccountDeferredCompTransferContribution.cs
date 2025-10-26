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
	/// Class NeoSpin.DataObjects.doPersonAccountDeferredCompTransferContribution:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPersonAccountDeferredCompTransferContribution : doBase
    {
         
         public doPersonAccountDeferredCompTransferContribution() : base()
         {
         }
         public int person_account_deferred_comp_transfer_contribution_id { get; set; }
         public int person_account_deferred_comp_transfer_id { get; set; }
         public int deferred_comp_contribution_id { get; set; }
         public string transfer_flag { get; set; }
    }
    [Serializable]
    public enum enmPersonAccountDeferredCompTransferContribution
    {
         person_account_deferred_comp_transfer_contribution_id ,
         person_account_deferred_comp_transfer_id ,
         deferred_comp_contribution_id ,
         transfer_flag ,
    }
}

