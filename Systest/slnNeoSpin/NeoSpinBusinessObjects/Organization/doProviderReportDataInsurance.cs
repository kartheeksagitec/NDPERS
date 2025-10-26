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
	/// Class NeoSpin.DataObjects.doProviderReportDataInsurance:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doProviderReportDataInsurance : doBase
    {
         
         public doProviderReportDataInsurance() : base()
         {
         }
         public int provider_report_data_insurance_comp_id { get; set; }
         public int subsystem_id { get; set; }
         public string subsystem_description { get; set; }
         public string subsystem_value { get; set; }
         public int subsystem_ref_id { get; set; }
         public int person_id { get; set; }
         public string ssn { get; set; }
         public string first_name { get; set; }
         public string last_name { get; set; }
         public int provider_org_id { get; set; }
         public int plan_id { get; set; }
         public DateTime effective_date { get; set; }
         public int record_type_id { get; set; }
         public string record_type_description { get; set; }
         public string record_type_value { get; set; }
         public decimal premium_amount { get; set; }
         public int batch_request_id { get; set; }
         public decimal fee_amount { get; set; }
         public string group_number { get; set; }
         public string rate_structure_code { get; set; }
         public string coverage_code { get; set; }
         public string hsa_flag { get; set; }
         public DateTime hsa_effective_date { get; set; }
         public decimal buydown_amount { get; set; }
         public decimal medicare_part_d_amt { get; set; }
    }
    [Serializable]
    public enum enmProviderReportDataInsurance
    {
         provider_report_data_insurance_comp_id ,
         subsystem_id ,
         subsystem_description ,
         subsystem_value ,
         subsystem_ref_id ,
         person_id ,
         ssn ,
         first_name ,
         last_name ,
         provider_org_id ,
         plan_id ,
         effective_date ,
         record_type_id ,
         record_type_description ,
         record_type_value ,
         premium_amount ,
         batch_request_id ,
         fee_amount ,
         group_number ,
         rate_structure_code ,
         coverage_code ,
         hsa_flag ,
         hsa_effective_date ,
         buydown_amount ,
         medicare_part_d_amt ,
    }
}

