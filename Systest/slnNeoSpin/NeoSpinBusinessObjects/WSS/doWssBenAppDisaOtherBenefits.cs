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
	/// Class NeoSpin.DataObjects.doWssBenAppDisaOtherBenefits:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doWssBenAppDisaOtherBenefits : doBase
    {
         
         public doWssBenAppDisaOtherBenefits() : base()
         {
         }
         public int disa_othr_benefit_id { get; set; }
         public int wss_ben_app_id { get; set; }
         public int othr_disability_benefit_id { get; set; }
         public string othr_disability_benefit_description { get; set; }
         public string othr_disability_benefit_value { get; set; }
         public DateTime othr_disa_bene_begin_date { get; set; }
         public DateTime othr_disa_bene_end_date { get; set; }
         public decimal othr_disa_mon_benefit_amount { get; set; }
         public string is_social_sec_or_workcomp_benefit_applied { get; set; }
         public int othr_disa_freq_id { get; set; }
         public string othr_disa_freq_description { get; set; }
         public string othr_disa_freq_value { get; set; }
    }
    [Serializable]
    public enum enmWssBenAppDisaOtherBenefits
    {
         disa_othr_benefit_id ,
         wss_ben_app_id ,
         othr_disability_benefit_id ,
         othr_disability_benefit_description ,
         othr_disability_benefit_value ,
         othr_disa_bene_begin_date ,
         othr_disa_bene_end_date ,
         othr_disa_mon_benefit_amount ,
         is_social_sec_or_workcomp_benefit_applied ,
         othr_disa_freq_id ,
         othr_disa_freq_description ,
         othr_disa_freq_value ,
    }
}

