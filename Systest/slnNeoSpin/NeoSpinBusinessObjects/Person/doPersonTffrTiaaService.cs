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
	/// Class NeoSpin.DataObjects.doPersonTffrTiaaService:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPersonTffrTiaaService : doBase
    {
         
         public doPersonTffrTiaaService() : base()
         {
         }
         public int tffr_tiaa_service_id { get; set; }
         public int person_id { get; set; }
         public int tffr_service { get; set; }
         public int tffr_service_status_id { get; set; }
         public string tffr_service_status_description { get; set; }
         public string tffr_service_status_value { get; set; }
         public int tiaa_service { get; set; }
         public int tiaa_service_status_id { get; set; }
         public string tiaa_service_status_description { get; set; }
         public string tiaa_service_status_value { get; set; }
         public string comment { get; set; }
    }
    [Serializable]
    public enum enmPersonTffrTiaaService
    {
         tffr_tiaa_service_id ,
         person_id ,
         tffr_service ,
         tffr_service_status_id ,
         tffr_service_status_description ,
         tffr_service_status_value ,
         tiaa_service ,
         tiaa_service_status_id ,
         tiaa_service_status_description ,
         tiaa_service_status_value ,
         comment ,
    }
}

