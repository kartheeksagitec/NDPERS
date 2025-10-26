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
	/// Class NeoSpin.DataObjects.doBenefitCalculationPayee:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doBenefitCalculationPayee : doBase
    {
         public doBenefitCalculationPayee() : base()
         {
         }
         public int benefit_calculation_payee_id { get; set; }
         public int benefit_calculation_id { get; set; }
         public int benefit_application_id { get; set; }
         public int payee_account_id { get; set; }
         public int payee_person_id { get; set; }
         public int payee_org_id { get; set; }
         public string payee_first_name { get; set; }
         public string payee_middle_name { get; set; }
         public string payee_last_name { get; set; }
         public DateTime payee_date_of_birth { get; set; }
         public decimal benefit_percentage { get; set; }
         public int account_relationship_id { get; set; }
         public string account_relationship_description { get; set; }
         public string account_relationship_value { get; set; }
         public int family_relationship_id { get; set; }
         public string family_relationship_description { get; set; }
         public string family_relationship_value { get; set; }
         public string member_account_negated_flag { get; set; }
         public int payee_sort_order { get; set; }
    }
    [Serializable]
    public enum enmBenefitCalculationPayee
    {
         benefit_calculation_payee_id ,
         benefit_calculation_id ,
         benefit_application_id ,
         payee_account_id ,
         payee_person_id ,
         payee_org_id ,
         payee_first_name ,
         payee_middle_name ,
         payee_last_name ,
         payee_date_of_birth ,
         benefit_percentage ,
         account_relationship_id ,
         account_relationship_description ,
         account_relationship_value ,
         family_relationship_id ,
         family_relationship_description ,
         family_relationship_value ,
         member_account_negated_flag ,
         payee_sort_order ,
    }
}

