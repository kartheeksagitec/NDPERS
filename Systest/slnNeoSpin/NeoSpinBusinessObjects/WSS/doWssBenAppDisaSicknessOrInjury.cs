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
	/// Class NeoSpin.DataObjects.doWssBenAppDisaSicknessOrInjury:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doWssBenAppDisaSicknessOrInjury : doBase
    {
        
         public doWssBenAppDisaSicknessOrInjury() : base()
         {
         }
         public int disa_sickness_or_injury_id { get; set; }
         public int wss_ben_app_id { get; set; }
         public DateTime date_of_sickness_or_injury { get; set; }
         public DateTime first_noticed_symptoms_date { get; set; }
         public DateTime first_physician_saw_date { get; set; }
         public string cause_of_disability { get; set; }
         public string name_of_treating_physician { get; set; }
         public string address { get; set; }
         public string city { get; set; }
         public string state { get; set; }
         public string zip_code { get; set; }
         public string name_of_hospital { get; set; }
         public DateTime admitted_date { get; set; }
         public DateTime released_date { get; set; }
         public string is_bed_confined { get; set; }
         public string is_house_confined { get; set; }
         public string did_you_have_same_kind_of_sickness_or_injury { get; set; }
         public DateTime specified_date { get; set; }
         public string spcfd_physcn { get; set; }
         public string spcfd_physcn_addr_line_1 { get; set; }
         public string spcfd_physcn_addr_line_2 { get; set; }
         public string spcfd_physcn_addr_city { get; set; }
         public int spcfd_physcn_addr_state_id { get; set; }
         public string spcfd_physcn_addr_state_description { get; set; }
         public string spcfd_physcn_addr_state_value { get; set; }
         public int spcfd_physcn_addr_country_id { get; set; }
         public string spcfd_physcn_addr_country_description { get; set; }
         public string spcfd_physcn_addr_country_value { get; set; }
         public string spcfd_physcn_addr_zip_code { get; set; }
         public string spcfd_physcn_addr_zip_4_code { get; set; }
         public DateTime date_and_time_of_accident { get; set; }
         public string was_accident_work_related { get; set; }
         public string place_of_accident { get; set; }
         public DateTime able_to_leave_home_date { get; set; }
         public DateTime able_to_work_date { get; set; }
    }
    [Serializable]
    public enum enmWssBenAppDisaSicknessOrInjury
    {
         disa_sickness_or_injury_id ,
         wss_ben_app_id ,
         date_of_sickness_or_injury ,
         first_noticed_symptoms_date ,
         first_physician_saw_date ,
         cause_of_disability ,
         name_of_treating_physician ,
         address ,
         city ,
         state ,
         zip_code ,
         name_of_hospital ,
         admitted_date ,
         released_date ,
         is_bed_confined ,
         is_house_confined ,
         did_you_have_same_kind_of_sickness_or_injury ,
         specified_date ,
         spcfd_physcn ,
         spcfd_physcn_addr_line_1 ,
         spcfd_physcn_addr_line_2 ,
         spcfd_physcn_addr_city ,
         spcfd_physcn_addr_state_id ,
         spcfd_physcn_addr_state_description ,
         spcfd_physcn_addr_state_value ,
         spcfd_physcn_addr_country_id ,
         spcfd_physcn_addr_country_description ,
         spcfd_physcn_addr_country_value ,
         spcfd_physcn_addr_zip_code ,
         spcfd_physcn_addr_zip_4_code ,
         date_and_time_of_accident ,
         was_accident_work_related ,
         place_of_accident ,
         able_to_leave_home_date ,
         able_to_work_date ,
    }
}

