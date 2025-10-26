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
	/// Class NeoSpin.DataObjects.doPayment1099r:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPayment1099r : doBase
    {
         
         public doPayment1099r() : base()
         {
         }
         public int payment_1099r_id { get; set; }
         public int payee_account_id { get; set; }
         public int tax_year { get; set; }
         public string corrected_flag { get; set; }
         public string total_distribution_flag { get; set; }
         public string distribution_code { get; set; }
         public int old_payment_1099r_id { get; set; }
         public decimal dist_percentage { get; set; }
         public decimal gross_benefit_amount { get; set; }
         public decimal taxable_amount { get; set; }
         public decimal non_taxable_amount { get; set; }
         public decimal capital_gain { get; set; }
         public decimal fed_tax_amount { get; set; }
         public decimal state_tax_amount { get; set; }
         public string ssn { get; set; }
         public string federal_id { get; set; }
         public int person_id { get; set; }
         public int org_id { get; set; }
         public string name { get; set; }
         public string addr_line_1 { get; set; }
         public string addr_line_2 { get; set; }
         public string addr_city { get; set; }
         public int addr_state_id { get; set; }
         public string addr_state_description { get; set; }
         public string addr_state_value { get; set; }
         public int addr_country_id { get; set; }
         public string addr_country_description { get; set; }
         public string addr_country_value { get; set; }
         public string addr_zip_code { get; set; }
         public string addr_zip_4_code { get; set; }
         public string foreign_province { get; set; }
         public string foreign_postal_code { get; set; }
         public string rollover_payment_flag { get; set; }
         public string age59_split_flag { get; set; }
         public string id_suffix { get; set; }
         public string created_annual_1099r_form { get; set; }
    }
    [Serializable]
    public enum enmPayment1099r
    {
         payment_1099r_id ,
         payee_account_id ,
         tax_year ,
         corrected_flag ,
         total_distribution_flag ,
         distribution_code ,
         old_payment_1099r_id ,
         dist_percentage ,
         gross_benefit_amount ,
         taxable_amount ,
         non_taxable_amount ,
         capital_gain ,
         fed_tax_amount ,
         state_tax_amount ,
         ssn ,
         federal_id ,
         person_id ,
         org_id ,
         name ,
         addr_line_1 ,
         addr_line_2 ,
         addr_city ,
         addr_state_id ,
         addr_state_description ,
         addr_state_value ,
         addr_country_id ,
         addr_country_description ,
         addr_country_value ,
         addr_zip_code ,
         addr_zip_4_code ,
         foreign_province ,
         foreign_postal_code ,
         rollover_payment_flag ,
         age59_split_flag ,
         id_suffix ,
         created_annual_1099r_form ,
    }
}

