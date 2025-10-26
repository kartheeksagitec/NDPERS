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
	/// Class NeoSpin.DataObjects.doPersonAccountDependentBillingLink:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPersonAccountDependentBillingLink : doBase
    {
         
         public doPersonAccountDependentBillingLink() : base()
         {
         }
         public int person_account_dependent_billing_link_id { get; set; }
         public int ibs_detail_id { get; set; }
         public int employer_payroll_detail_id { get; set; }
         public int person_account_dependent_id { get; set; }
    }
    [Serializable]
    public enum enmPersonAccountDependentBillingLink
    {
         person_account_dependent_billing_link_id ,
         ibs_detail_id ,
         employer_payroll_detail_id ,
         person_account_dependent_id ,
    }
}

