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
	/// Class NeoSpin.DataObjects.doPersonAccountInsuranceTransferContribution:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPersonAccountInsuranceTransferContribution : doBase
    {
         
         public doPersonAccountInsuranceTransferContribution() : base()
         {
         }
         public int person_account_insurance_transfer_contribution_id { get; set; }
         public int person_account_insurance_transfer_id { get; set; }
         public int health_insurance_contribution_id { get; set; }
         public string transfer_flag { get; set; }
    }
    [Serializable]
    public enum enmPersonAccountInsuranceTransferContribution
    {
         person_account_insurance_transfer_contribution_id ,
         person_account_insurance_transfer_id ,
         health_insurance_contribution_id ,
         transfer_flag ,
    }
}

