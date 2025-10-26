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
	/// Class NeoSpin.DataObjects.doFedStateFlatTaxRate:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doFedStateFlatTaxRate : doBase
    {
        
         public doFedStateFlatTaxRate() : base()
         {
         }
         public int fed_state_flat_tax_id { get; set; }
         public int tax_identifier_id { get; set; }
         public string tax_identifier_description { get; set; }
         public string tax_identifier_value { get; set; }
         public DateTime effective_date { get; set; }
         public int benefit_account_type_id { get; set; }
         public string benefit_account_type_description { get; set; }
         public string benefit_account_type_value { get; set; }
         public int benefit_sub_type_id { get; set; }
         public string benefit_sub_type_description { get; set; }
         public string benefit_sub_type_value { get; set; }
         public int account_relation_id { get; set; }
         public string account_relation_description { get; set; }
         public string account_relation_value { get; set; }
         public string plso_flag { get; set; }
         public decimal flat_tax_percentage { get; set; }
         public int family_relation_id { get; set; }
         public string family_relation_description { get; set; }
         public string family_relation_value { get; set; }
         public string supl_check_flag { get; set; }
         public string is_rmd { get; set; }
         public decimal min_tax_percentage { get; set; }
         public decimal max_tax_percentage { get; set; }
        public string tax_ref { get; set; }
    }
    [Serializable]
    public enum enmFedStateFlatTaxRate
    {
         fed_state_flat_tax_id ,
         tax_identifier_id ,
         tax_identifier_description ,
         tax_identifier_value ,
         effective_date ,
         benefit_account_type_id ,
         benefit_account_type_description ,
         benefit_account_type_value ,
         benefit_sub_type_id ,
         benefit_sub_type_description ,
         benefit_sub_type_value ,
         account_relation_id ,
         account_relation_description ,
         account_relation_value ,
         plso_flag ,
         flat_tax_percentage ,
         family_relation_id ,
         family_relation_description ,
         family_relation_value ,
         supl_check_flag ,
         is_rmd ,
         min_tax_percentage ,
         max_tax_percentage ,
         tax_ref ,
    }
}

