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
	/// Class NeoSpin.DataObjects.doPeoplesoftPlanOrgCrossRef:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPeoplesoftPlanOrgCrossRef : doBase
    {
         
         public doPeoplesoftPlanOrgCrossRef() : base()
         {
         }
         public int sgt_peoplesoft_plan_org_cross_ref_id { get; set; }
         public int plan_id { get; set; }
         public int people_soft_org_group_id { get; set; }
         public string people_soft_org_group_description { get; set; }
         public string people_soft_org_group_value { get; set; }
         public int people_soft_file_date_type_id { get; set; }
         public string people_soft_file_date_type_description { get; set; }
         public string people_soft_file_date_type_value { get; set; }
         public int people_soft_file_date_id { get; set; }
         public string people_soft_file_date_description { get; set; }
         public string people_soft_file_date_value { get; set; }
         public string org_code { get; set; }
         public int ps_file_change_event_id { get; set; }
         public string ps_file_change_event_description { get; set; }
         public string ps_file_change_event_value { get; set; }
         public int ps_coverage_election_id { get; set; }
         public string ps_coverage_election_description { get; set; }
         public string ps_coverage_election_value { get; set; }
    }
    [Serializable]
    public enum enmPeoplesoftPlanOrgCrossRef
    {
         sgt_peoplesoft_plan_org_cross_ref_id ,
         plan_id ,
         people_soft_org_group_id ,
         people_soft_org_group_description ,
         people_soft_org_group_value ,
         people_soft_file_date_type_id ,
         people_soft_file_date_type_description ,
         people_soft_file_date_type_value ,
         people_soft_file_date_id ,
         people_soft_file_date_description ,
         people_soft_file_date_value ,
         org_code ,
         ps_file_change_event_id ,
         ps_file_change_event_description ,
         ps_file_change_event_value ,
         ps_coverage_election_id ,
         ps_coverage_election_description ,
         ps_coverage_election_value ,
    }
}

