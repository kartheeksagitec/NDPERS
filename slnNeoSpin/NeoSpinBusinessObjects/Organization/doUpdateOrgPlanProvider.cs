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
	/// Class NeoSpin.DataObjects.doUpdateOrgPlanProvider:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doUpdateOrgPlanProvider : doBase
    {
         
         public doUpdateOrgPlanProvider() : base()
         {
         }
         public int request_id { get; set; }
         public int employer_org_id { get; set; }
         public int from_provider_org_id { get; set; }
         public int to_provider_org_id { get; set; }
         public DateTime effective_date { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public int plan_id { get; set; }
    }
    [Serializable]
    public enum enmUpdateOrgPlanProvider
    {
         request_id ,
         employer_org_id ,
         from_provider_org_id ,
         to_provider_org_id ,
         effective_date ,
         status_id ,
         status_description ,
         status_value ,
         plan_id ,
    }
}

