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
	/// Class NeoSpin.DataObjects.doFedStateTaxRate:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doFedStateTaxRate : doBase
    {
         public doFedStateTaxRate() : base()
         {
         }
         public int fed_state_tax_id { get; set; }
         public int tax_identifier_id { get; set; }
         public string tax_identifier_description { get; set; }
         public string tax_identifier_value { get; set; }
         public DateTime effective_date { get; set; }
         public int marital_status_id { get; set; }
         public string marital_status_description { get; set; }
         public string marital_status_value { get; set; }
         public decimal minimum_amount { get; set; }
         public decimal maximum_amount { get; set; }
         public decimal tax_amount { get; set; }
         public decimal percentage { get; set; }
         public decimal allowance_amount { get; set; }
         public string approved_flag { get; set; }
         public int filing_status_id { get; set; }
         public string filing_status_description { get; set; }
         public string filing_status_value { get; set; }
        public string tax_ref { get; set; }
    }
    [Serializable]
    public enum enmFedStateTaxRate
    {
         fed_state_tax_id ,
         tax_identifier_id ,
         tax_identifier_description ,
         tax_identifier_value ,
         effective_date ,
         marital_status_id ,
         marital_status_description ,
         marital_status_value ,
         minimum_amount ,
         maximum_amount ,
         tax_amount ,
         percentage ,
         allowance_amount ,
         approved_flag ,
         filing_status_id ,
         filing_status_description ,
         filing_status_value ,
         tax_ref ,
    }
}
