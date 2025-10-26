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
	/// Class NeoSpin.DataObjects.doBenefitApplicationPersonAccount:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doBenefitApplicationPersonAccount : doBase
    {
         public doBenefitApplicationPersonAccount() : base()
         {
         }
         public int benefit_application_person_account_id { get; set; }
         public int benefit_application_id { get; set; }
         public int person_account_id { get; set; }
         public string is_person_account_selected_flag { get; set; }
         public int payee_account_id { get; set; }
         public string is_application_person_account_flag { get; set; }
    }
    [Serializable]
    public enum enmBenefitApplicationPersonAccount
    {
         benefit_application_person_account_id ,
         benefit_application_id ,
         person_account_id ,
         is_person_account_selected_flag ,
         payee_account_id ,
         is_application_person_account_flag ,
    }
}

