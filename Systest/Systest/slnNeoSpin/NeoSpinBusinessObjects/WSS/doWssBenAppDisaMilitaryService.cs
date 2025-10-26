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
	/// Class NeoSpin.DataObjects.doWssBenAppDisaMilitaryService:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doWssBenAppDisaMilitaryService : doBase
    {
         
         public doWssBenAppDisaMilitaryService() : base()
         {
         }
         public int disa_military_service_id { get; set; }
         public int wss_ben_app_id { get; set; }
         public string branch { get; set; }
         public DateTime date_from { get; set; }
         public DateTime date_to { get; set; }
         public int discharge_id { get; set; }
         public string discharge_description { get; set; }
         public string discharge_value { get; set; }
         public string duties_or_responsibilities { get; set; }
         public string rank { get; set; }
         public string special_training { get; set; }
         public string service_connected_disabilities { get; set; }
    }
    [Serializable]
    public enum enmWssBenAppDisaMilitaryService
    {
         disa_military_service_id ,
         wss_ben_app_id ,
         branch ,
         date_from ,
         date_to ,
         discharge_id ,
         discharge_description ,
         discharge_value ,
         duties_or_responsibilities ,
         rank ,
         special_training ,
         service_connected_disabilities ,
    }
}

