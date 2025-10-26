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
	/// Class NeoSpin.DataObjects.doProviderReportDataDc:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doProviderReportDataDc : doBase
    {
         
         public doProviderReportDataDc() : base()
         {
         }
         public int provider_report_data_dc_id { get; set; }
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
         public decimal ee_contribution { get; set; }
         public decimal ee_pre_tax { get; set; }
         public decimal ee_employer_pickup { get; set; }
         public decimal er_contribution { get; set; }
         public decimal member_interest { get; set; }
         public decimal employer_interest { get; set; }
         public decimal employer_rhic_interest { get; set; }
         public int batch_request_id { get; set; }
        //public decimal ee_pretax_addl { get; set; } //PIR 25920 New Plan DC 2025
        //public decimal ee_post_tax_addl { get; set; }   //PIR 25920 New Plan DC 2025
        //public decimal er_pretax_match { get; set; }    //PIR 25920 New Plan DC 2025
    }
    [Serializable]
    public enum enmProviderReportDataDc
    {
         provider_report_data_dc_id ,
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
         ee_contribution ,
         ee_pre_tax ,
         ee_employer_pickup ,
         er_contribution ,
         member_interest ,
         employer_interest ,
         employer_rhic_interest ,
         batch_request_id ,
         //ee_pretax_addl,
         //ee_post_tax_addl,
         //er_pretax_match
    }
}

