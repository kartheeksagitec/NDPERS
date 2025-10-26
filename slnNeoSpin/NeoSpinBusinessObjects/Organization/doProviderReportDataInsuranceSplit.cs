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
	/// Class NeoSpin.DataObjects.doProviderReportDataInsuranceSplit:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doProviderReportDataInsuranceSplit : doBase
    {
         
         public doProviderReportDataInsuranceSplit() : base()
         {
         }
         public int provider_report_data_insurance_split_id { get; set; }
         public int provider_report_data_insurance_comp_id { get; set; }
         public int person_id { get; set; }
         public string ssn { get; set; }
         public string first_name { get; set; }
         public string last_name { get; set; }
         public decimal premium_amount { get; set; }
         public decimal fee_amount { get; set; }
         public string coverage_code { get; set; }
         public string medicare_claim_no { get; set; }
    }
    [Serializable]
    public enum enmProviderReportDataInsuranceSplit
    {
         provider_report_data_insurance_split_id ,
         provider_report_data_insurance_comp_id ,
         person_id ,
         ssn ,
         first_name ,
         last_name ,
         premium_amount ,
         fee_amount ,
         coverage_code ,
         medicare_claim_no ,
    }
}

